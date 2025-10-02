using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.Upload.Editor
{
    enum UploadEndedStatus
    {
        Success,
        Error,
        Cancelled
    }

    interface IUploadManager : IService
    {
        event Action UploadBegan;
        event Action<UploadEndedStatus> UploadEnded;

        bool IsUploading { get; }

        void CancelUpload();
        Task UploadAsync(IReadOnlyCollection<IUploadAsset> uploadEntries);
    }

    [Serializable]
    class UploadManager : BaseService<IUploadManager>, IUploadManager
    {
        class AssetUploadInfo
        {
            public AssetUploadInfo(IUploadAsset uploadAsset, AssetData targetAssetData, bool targetAssetDataWasRecycled)
            {
                UploadAsset = uploadAsset;
                TargetAssetData = targetAssetData;
                TargetAssetDataWasRecycled = targetAssetDataWasRecycled;
            }

            public IUploadAsset UploadAsset { get; }
            public AssetData TargetAssetData { get; }
            public bool TargetAssetDataWasRecycled { get; }
        }

        [SerializeReference]
        IAssetOperationManager m_AssetOperationManager;

        [SerializeReference]
        IImportedAssetsTracker m_ImportTracker;

        [SerializeReference]
        IAssetsProvider m_AssetsProvider;

        CancellationTokenSource m_TokenSource;

        bool m_Uploading;

        public bool IsUploading => m_Uploading;
        public event Action UploadBegan;
        public event Action<UploadEndedStatus> UploadEnded;

        [ServiceInjection]
        public void Inject(IAssetOperationManager assetOperationManager, IImportedAssetsTracker importTracker, IAssetsProvider assetsProvider, IAssetDataManager assetDataManager)
        {
            m_AssetOperationManager = assetOperationManager;
            m_ImportTracker = importTracker;
            m_AssetsProvider = assetsProvider;
        }

        public async Task UploadAsync(IReadOnlyCollection<IUploadAsset> uploadEntries)
        {
            if (m_Uploading)
                return;

            m_Uploading = true;
            UploadBegan?.Invoke();

            var uploadEntryToAssetUploadInfoLookup = new Dictionary<IUploadAsset, AssetUploadInfo>();
            var uploadEntryToOperationLookup = new Dictionary<IUploadAsset, UploadOperation>();
            var identifierToAssetLookup = new Dictionary<AssetIdentifier, AssetData>();

            m_TokenSource = new CancellationTokenSource();
            var token = m_TokenSource.Token;
            var uploadEndedStatus = UploadEndedStatus.Success;

            try
            {
                var assetEntriesWithAllDependencies = new List<IUploadAsset>();

                var database = uploadEntries.ToDictionary(entry => entry.LocalIdentifier);

                // Get all assets, including their dependencies
                foreach (var uploadEntry in uploadEntries)
                {
                    AddDependencies(uploadEntry, assetEntriesWithAllDependencies, database);
                }

                // Prepare the IAssets
                var createAssetTasks = await TaskUtils.RunAllTasks(assetEntriesWithAllDependencies,
                    (uploadEntry) =>
                    {
                        var operation = StartNewOperation(uploadEntry);
                        uploadEntryToOperationLookup[uploadEntry] = operation;

                        // Intentionally not cancellable in order to retrieve the AssetUploadInfo
                        return CreateOrRecycleAsset(operation, uploadEntry, CancellationToken.None);
                    });

                foreach (var task in createAssetTasks)
                {
                    var assetUploadInfo = ((Task<AssetUploadInfo>)task).Result;

                    if (assetUploadInfo == null) // Something went wrong during asset creation and the error was already reported
                        continue;

                    var uploadEntry = assetUploadInfo.UploadAsset;

                    uploadEntryToAssetUploadInfoLookup[uploadEntry] = assetUploadInfo;
                    identifierToAssetLookup[uploadEntry.LocalIdentifier] = assetUploadInfo.TargetAssetData;
                }

                // Since we don't check the cancellation token during the creation of the asset to ensure they complete,
                // let's check if cancel was requested here before moving on
                token.ThrowIfCancellationRequested();

                // Prepare a cloud asset for every asset entry that we want to upload
                await TaskUtils.RunAllTasks(uploadEntryToAssetUploadInfoLookup,
                    (entry) =>
                    {
                        var operation = uploadEntryToOperationLookup[entry.Key];
                        return FetchAssetDependenciesAsync(operation, entry.Value.TargetAssetData, identifierToAssetLookup,
                            token);
                    });

                // Upload the assets
                await TaskUtils.RunAllTasks(uploadEntryToAssetUploadInfoLookup,
                    (entry) =>
                    {
                        var operation = uploadEntryToOperationLookup[entry.Key];
                        return UploadAssetAsync(entry.Value, operation, token);
                    });

                // Update the dependencies after the upload
                await TaskUtils.RunAllTasks(uploadEntryToAssetUploadInfoLookup,
                    (entry) =>
                    {
                        var operation = uploadEntryToOperationLookup[entry.Key];
                        return UpdateDependenciesAsync(operation, entry.Value.TargetAssetData, token);
                    });

                // Track the assets
                await TaskUtils.RunAllTasks(uploadEntryToAssetUploadInfoLookup,
                    (entry) =>
                    {
                        return TrackAsset(entry.Value.UploadAsset, entry.Value.TargetAssetData, token);
                    });
            }
            catch (OperationCanceledException e)
            {
                uploadEndedStatus = UploadEndedStatus.Cancelled;

                await TaskUtils.RunAllTasks(uploadEntryToAssetUploadInfoLookup.Values,
                    assetUploadInfo => RemoveAttemptedUploadAssetAsync(assetUploadInfo));

                foreach (var (_, operation) in uploadEntryToOperationLookup)
                {
                    if (operation.Status is not OperationStatus.Error)
                    {
                        operation.Finish(OperationStatus.Cancelled);
                    }
                }

                AnalyticsSender.SendEvent(new UploadEndEvent(UploadEndStatus.Cancelled, e.Message));
            }
            catch (Exception)
            {
                uploadEndedStatus = UploadEndedStatus.Error;
            }
            finally
            {
                m_Uploading = false;
                CancelUpload(); // Any failure should cancel whatever is left; if there's nothing left like on success, it's a no-op
                m_TokenSource?.Dispose();
                m_TokenSource = null;
                UploadEnded?.Invoke(uploadEndedStatus);
            }
        }

        public void CancelUpload()
        {
            m_TokenSource?.Cancel();
        }

        async Task TrackAsset(IUploadAsset uploadAsset, BaseAssetData asset, CancellationToken token)
        {
            try
            {
                IEnumerable<(string originalPath, string finalPath, string checksum)> assetPaths = new List<(string, string, string)>();

                foreach (var f in uploadAsset.Files)
                {
                    assetPaths = assetPaths.Append((originalPath: f.DestinationPath, f.SourcePath, null));
                }

                BaseAssetData assetData;
                try
                {
                    var cloudAsset = await m_AssetsProvider.GetAssetAsync(asset.Identifier, token);
                    assetData = cloudAsset;

                    // Make sure additional data is populated.
                    var tasks = new List<Task>
                    {
                        assetData.ResolveDatasetsAsync(token),
                        assetData.RefreshDependenciesAsync(token),
                    };
                    await Task.WhenAll(tasks);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while trying to track the asset from the cloud: " + e.Message);
                    assetData = asset;
                }

                await m_ImportTracker.TrackAssets(assetPaths, assetData);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        async Task<AssetUploadInfo> CreateOrRecycleAsset(BaseOperation operation, IUploadAsset uploadAsset,
            CancellationToken token)
        {
            try
            {
                return await CreateOrRecycleAsset(uploadAsset, token);
            }
            catch (OperationCanceledException)
            {
                // Do nothing if cancelled
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                operation.Finish(OperationStatus.Error);
            }

            return null;
        }

        async Task<AssetUploadInfo> CreateOrRecycleAsset(IUploadAsset uploadAsset, CancellationToken token)
        {
            AssetData originalAsset = null;

            if (uploadAsset.ExistingAssetIdentifier != null)
            {
                originalAsset = await m_AssetsProvider.GetAssetAsync(uploadAsset.ExistingAssetIdentifier, token);
            }

            var isRecycled = originalAsset != null;

            var targetAssetData = isRecycled
                ? await RecycleAsset(uploadAsset, originalAsset, token)
                : await CreateNewAsset(uploadAsset, token);

            return new AssetUploadInfo(uploadAsset, targetAssetData, isRecycled);
        }

        UploadOperation StartNewOperation(IUploadAsset uploadAsset)
        {
            var operation = new UploadOperation(uploadAsset);
            m_AssetOperationManager.RegisterOperation(operation);

            operation.Start();

            return operation;
        }

        async Task FetchAssetDependenciesAsync(UploadOperation operation, AssetData targetAssetData,
            IDictionary<AssetIdentifier, AssetData> identifierToAssetLookup, CancellationToken token = default)
        {
            try
            {
                await operation.FetchAssetDependenciesAsync(targetAssetData, identifierToAssetLookup, token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                operation.Finish(OperationStatus.Error);
                AnalyticsSender.SendEvent(new UploadEndEvent(UploadEndStatus.PreparationError, e.Message));
                throw;
            }
        }

        async Task UpdateDependenciesAsync(UploadOperation operation, AssetData targetAssetData, CancellationToken token = default)
        {
            try
            {
                await operation.UpdateDependenciesAsync(targetAssetData, token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                operation.Finish(OperationStatus.Error);
                AnalyticsSender.SendEvent(new UploadEndEvent(UploadEndStatus.PreparationError, e.Message));
                throw;
            }
        }

        async Task UploadAssetAsync(AssetUploadInfo assetUploadInfo, UploadOperation operation, CancellationToken token = default)
        {
            try
            {
                await operation.UploadAsync(assetUploadInfo.TargetAssetData, token);
                operation.Finish(OperationStatus.Success);
                AnalyticsSender.SendEvent(new UploadEndEvent(UploadEndStatus.Ok));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                operation.Finish(OperationStatus.Error);
                AnalyticsSender.SendEvent(new UploadEndEvent(UploadEndStatus.UploadError, e.Message));
                throw;
            }
        }

        async Task RemoveAttemptedUploadAssetAsync(AssetUploadInfo uploadInfo)
        {
            if (uploadInfo.TargetAssetDataWasRecycled)
            {
                await m_AssetsProvider.RemoveUnfrozenAssetVersion(uploadInfo.TargetAssetData.Identifier,
                    CancellationToken.None);
            }
            else
            {
                await m_AssetsProvider.RemoveAsset(uploadInfo.TargetAssetData.Identifier, CancellationToken.None);
            }
        }

        void AddDependencies(IUploadAsset uploadAsset, ICollection<IUploadAsset> assetEntries,
            IReadOnlyDictionary<AssetIdentifier, IUploadAsset> database)
        {
            if (assetEntries.Contains(uploadAsset))
                return;

            assetEntries.Add(uploadAsset);
            foreach (var id in uploadAsset.Dependencies)
            {
                if (database.TryGetValue(id, out var child))
                {
                    AddDependencies(child, assetEntries, database);
                }
            }
        }

        async Task<AssetData> CreateNewAsset(IUploadAsset uploadAsset, CancellationToken token)
        {
            var targetCollection = uploadAsset.TargetCollection;
            var assetCreation = new AssetCreation
            {
                Name = uploadAsset.Name,
                Collections = string.IsNullOrEmpty(targetCollection) ? null : new List<string> { new(targetCollection) },
                Type = uploadAsset.AssetType,
                Tags = uploadAsset.Tags.ToList(),
                Metadata = uploadAsset.Metadata.ToList()
            };

            return await m_AssetsProvider.CreateAssetAsync(uploadAsset.TargetProject, assetCreation, token);
        }

        async Task<AssetData> RecycleAsset(IUploadAsset uploadAsset, AssetData asset, CancellationToken token)
        {
            const ComparisonResults filesChanged = ComparisonResults.FilesAdded | ComparisonResults.FilesRemoved | ComparisonResults.FilesModified;
            const ComparisonResults metadataChanged = ComparisonResults.MetadataAdded | ComparisonResults.MetadataRemoved | ComparisonResults.MetadataModified;

            if (asset.IsFrozen)
            {
                asset = await m_AssetsProvider.CreateUnfrozenVersionAsync(asset, token);
            }

            var assetUpdate = new AssetUpdate
            {
                Name = uploadAsset.Name,
                Type = uploadAsset.AssetType,
                Tags = uploadAsset.Tags.ToList(),
            };

            // Only update metadata if it was considered modified in some way.
            if ((uploadAsset.ComparisonResults & metadataChanged) != 0)
            {
                assetUpdate.Metadata = uploadAsset.Metadata.ToList();
            }

            var tasks = new List<Task>
            {
                m_AssetsProvider.UpdateAsync(asset, assetUpdate, token),
                m_AssetsProvider.RemoveThumbnail(asset, token),
            };

            // Only remove files if they were considered modified in some way.
            if ((uploadAsset.ComparisonResults & filesChanged) != 0)
            {
                tasks.Add(m_AssetsProvider.RemoveAllFiles(asset, token));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                // If any of the tasks fail, we need to remove the asset version that was created before throwing the exception.
                await m_AssetsProvider.RemoveUnfrozenAssetVersion(asset.Identifier, token);
                throw;
            }

            return asset;
        }
    }
}
