using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class AssetData : BaseAssetData
    {
        static readonly int s_MaxThumbnailSize = 180;

        [SerializeField]
        AssetIdentifier m_Identifier;

        [SerializeField]
        int m_SequenceNumber;

        [SerializeField]
        int m_ParentSequenceNumber;

        [SerializeField]
        string m_Changelog;

        [SerializeField]
        string m_Name;

        [SerializeField]
        AssetType m_AssetType;

        [SerializeField]
        string m_Status;

        [SerializeField]
        string m_Description;

        [SerializeField]
        long m_Created;

        [SerializeField]
        long m_Updated;

        [SerializeField]
        string m_CreatedBy;

        [SerializeField]
        string m_UpdatedBy;

        [SerializeField]
        string m_PreviewFilePath;

        [SerializeField]
        bool m_IsFrozen;

        [SerializeField]
        List<string> m_Tags;

        [SerializeField]
        List<AssetIdentifier> m_Dependencies = new();

        [SerializeField]
        string m_ThumbnailUrl;

        [SerializeField]
        bool m_ThumbnailProcessed; // Flag to avoid multiple requests for assets with no thumbnail

        [SerializeField]
        bool m_DatasetProcessed; // Flag to avoid multiple requests for assets with no primary extension

        [SerializeReference]
        List<BaseAssetData> m_Versions = new();

        [SerializeReference]
        List<AssetLabel> m_Labels = new();

        Task<Uri> m_GetPreviewStatusTask;
        Task m_AssetDataAttributesTask;
        Task m_DatasetTask;
        Task m_RefreshPropertiesTask;
        Task m_RefreshDependenciesTask;
        Task m_RefreshVersionsTask;
        Task m_ThumbnailUrlTask;
        Task m_LinkedProjectsTask;

        IThumbnailDownloader m_ThumbnailDownloader;

        public override AssetIdentifier Identifier => m_Identifier;
        public override int SequenceNumber => m_SequenceNumber;
        public override int ParentSequenceNumber => m_ParentSequenceNumber;
        public override string Changelog => m_Changelog;
        public override string Name => m_Name;
        public override AssetType AssetType => m_AssetType;
        public override string Status => m_Status;
        public override string Description => m_Description;
        public override DateTime? Created => new DateTime(m_Created, DateTimeKind.Utc);
        public override DateTime? Updated => new DateTime(m_Updated, DateTimeKind.Utc);
        public override string CreatedBy => m_CreatedBy;
        public override string UpdatedBy => m_UpdatedBy;
        public string PreviewFilePath => m_PreviewFilePath;
        public bool IsFrozen => m_IsFrozen;
        public override IEnumerable<string> Tags => m_Tags;

        public override IEnumerable<AssetIdentifier> Dependencies
        {
            get => m_Dependencies;
            internal set => m_Dependencies = value?.ToList() ?? new List<AssetIdentifier>();
        }

        public override IEnumerable<BaseAssetData> Versions => m_Versions;

        IThumbnailDownloader ThumbnailDownloader =>
            m_ThumbnailDownloader ??= ServicesContainer.instance.Resolve<IThumbnailDownloader>();

        public override IEnumerable<AssetLabel> Labels => m_Labels;

        public AssetData() { }

#pragma warning disable S107 // Disabling the warning regarding too many parameters.
        public AssetData(AssetIdentifier assetIdentifier,
            int sequenceNumber,
            int parentSequenceNumber,
            string changelog,
            string name,
            AssetType assetType,
            string status,
            string description,
            DateTime created,
            DateTime updated,
            string createdBy,
            string updatedBy,
            string previewFilePath,
            bool isFrozen,
            IEnumerable<string> tags,
            IEnumerable<AssetLabel> labels = null,
            IEnumerable<ProjectIdentifier> linkedProjects = null)
        {
            m_Identifier = assetIdentifier;
            m_SequenceNumber = sequenceNumber;
            m_ParentSequenceNumber = parentSequenceNumber;
            m_Changelog = changelog;
            m_Name = name;
            m_AssetType = assetType;
            m_Status = status;
            m_Description = description;
            m_Created = created.Ticks;
            m_Updated = updated.Ticks;
            m_CreatedBy = createdBy;
            m_UpdatedBy = updatedBy;
            m_PreviewFilePath = previewFilePath;
            m_IsFrozen = isFrozen;
            m_Tags = tags?.ToList() ?? new List<string>();
            m_Labels = labels?.ToList() ?? new List<AssetLabel>();
            m_LinkedProjects = linkedProjects?.ToList() ?? new List<ProjectIdentifier>();
        }
#pragma warning restore S107

#pragma warning disable S107  // Disabling the warning regarding too many parameters.
        // Used when de-serialized from version 0.0 to fill data not in the IAsset
        public void FillFromPersistenceLegacy(IEnumerable<AssetIdentifier> dependencyAssets,
            string thumbnailUrl,
            IEnumerable<BaseAssetDataFile> sourceFiles,
            BaseAssetDataFile primarySourceFile)
        {
            m_Dependencies = dependencyAssets?.ToList();
            m_ThumbnailUrl = thumbnailUrl;

            var sourceAssetDataFiles = sourceFiles?.ToList();
            // Add a default dataset for the source files. Add a system tag to identify it is manually added. (NotSynced)
            m_Datasets = new List<AssetDataset>
            {
                new(k_Source, new List<string> { k_Source, k_NotSynced }, sourceAssetDataFiles)
            };

            m_PrimarySourceFile = primarySourceFile;
        }
#pragma warning restore S107

#pragma warning disable S107 // Disabling the warning regarding too many parameters.
        // Used when de-serialized from version 1.0
        public void FillFromPersistence(AssetIdentifier assetIdentifier,
            int sequenceNumber,
            int parentSequenceNumber,
            string changelog,
            string name,
            AssetType assetType,
            string status,
            string description,
            DateTime created,
            DateTime updated,
            string createdBy,
            string updatedBy,
            string previewFilePath,
            bool isFrozen,
            IEnumerable<string> tags,
            IEnumerable<AssetDataFile> sourceFiles,
            IEnumerable<AssetIdentifier> dependencies,
            IEnumerable<IMetadata> metadata)
        {
            var sourceAssetDataFiles = sourceFiles?.Cast<BaseAssetDataFile>().ToList();

            // Add a default dataset for the source files. Add a system tag to identify it is manually added. (NotSynced)
            var datasets = new List<AssetDataset>{
                new (k_Source, new List<string> { k_Source, k_NotSynced }, sourceAssetDataFiles)
            };

            FillFromPersistence(assetIdentifier, sequenceNumber, parentSequenceNumber, changelog, name, assetType, status, description, created, updated, createdBy, updatedBy, previewFilePath, isFrozen, tags, datasets, dependencies, metadata);
        }
#pragma warning restore S107

#pragma warning disable S107 // Disabling the warning regarding too many parameters.
        // Used when de-serialized from version 2.0 and 3.0
        public void FillFromPersistence(AssetIdentifier assetIdentifier,
            int sequenceNumber,
            int parentSequenceNumber,
            string changelog,
            string name,
            AssetType assetType,
            string status,
            string description,
            DateTime created,
            DateTime updated,
            string createdBy,
            string updatedBy,
            string previewFilePath,
            bool isFrozen,
            IEnumerable<string> tags,
            IEnumerable<AssetDataset> datasets,
            IEnumerable<AssetIdentifier> dependencies,
            IEnumerable<IMetadata> metadata)
        {
            m_Identifier = assetIdentifier;
            m_SequenceNumber = sequenceNumber;
            m_ParentSequenceNumber = parentSequenceNumber;
            m_Changelog = changelog;
            m_Name = name;
            m_AssetType = assetType;
            m_Status = status;
            m_Description = description;
            m_Created = created.Ticks;
            m_Updated = updated.Ticks;
            m_CreatedBy = createdBy;
            m_UpdatedBy = updatedBy;
            m_PreviewFilePath = previewFilePath;
            m_IsFrozen = isFrozen;
            m_Tags = tags.ToList();

            m_Dependencies = dependencies?.ToList();
            m_Metadata = new MetadataContainer(metadata);

            m_Datasets = datasets.ToList();

            ResolvePrimaryExtension();
        }
#pragma warning restore S107

        void FillFromOther(AssetData other)
        {
            m_Identifier = other.Identifier;
            m_SequenceNumber = other.SequenceNumber;
            m_ParentSequenceNumber = other.ParentSequenceNumber;
            m_Changelog = other.Changelog;
            m_Name = other.Name;
            m_AssetType = other.AssetType;
            m_Status = other.Status;
            m_Description = other.Description;
            m_Created = other.Created?.Ticks ?? 0;
            m_Updated = other.Updated?.Ticks ?? 0;
            m_CreatedBy = other.CreatedBy;
            m_UpdatedBy = other.UpdatedBy;
            m_PreviewFilePath = other.PreviewFilePath;
            m_IsFrozen = other.IsFrozen;
            m_Tags = other.Tags?.ToList() ?? new List<string>();
            m_Metadata = new MetadataContainer(other.m_Metadata?.ToList() ?? new List<IMetadata>());
            m_Labels = other.Labels.ToList();
            m_LinkedProjects = other.LinkedProjects.ToList();

            // Focus on copying the primary datasets.
            foreach (var dataset in other.Datasets)
            {
                _ = AssetDataset.k_PrimaryDatasetSystemTags.FirstOrDefault(x => TryCopyDataset(dataset, x));
            }
        }

        public override async Task GetThumbnailAsync(CancellationToken token = default)
        {
            // Because a AssetData is tied to a version, and preview modification creates a new version,
            // we can assume that the thumbnail is always the same.
            if (Thumbnail != null || m_ThumbnailProcessed)
            {
                return;
            }

            // Look inside the cache before making any request
            var cachedThumbnail = ThumbnailDownloader.GetCachedThumbnail(Identifier);
            if (cachedThumbnail != null)
            {
                m_ThumbnailProcessed = true;
                Thumbnail = cachedThumbnail;
                return;
            }

            m_ThumbnailUrlTask ??= GetThumbnailUrlAsync(token);

            try
            {
                await m_ThumbnailUrlTask;
            }
            finally
            {
                m_ThumbnailUrlTask = null;
            }

            ThumbnailDownloader.DownloadThumbnail(Identifier, m_ThumbnailUrl,
                (_, texture) =>
                {
                    m_ThumbnailProcessed = true;
                    Thumbnail = texture;
                });
        }

        async Task GetThumbnailUrlAsync(CancellationToken token)
        {
            var assetsSdkProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();

            m_GetPreviewStatusTask ??= assetsSdkProvider.GetPreviewUrlAsync(this, s_MaxThumbnailSize, token);

            Uri previewFileUrl = null;

            try
            {
                previewFileUrl = await m_GetPreviewStatusTask;
            }
            catch (NotFoundException)
            {
                // Ignore if the Asset is not found
            }
            catch (ForbiddenException)
            {
                // Ignore if the Asset is unavailable
            }
            catch (HttpRequestException)
            {
                // Ignore unreachable host
            }
            finally
            {
                m_GetPreviewStatusTask = null;
            }

            m_ThumbnailUrl = previewFileUrl?.ToString() ?? string.Empty;
        }

        public override async Task RefreshAssetDataAttributesAsync(CancellationToken token = default)
        {
            var assetDataManager = ServicesContainer.instance.Resolve<IAssetDataManager>();
            var isInProject = assetDataManager.IsInProject(Identifier);

            if (AssetDataAttributeCollection != null && AssetDataAttributeCollection.HasAttribute<ImportAttribute>())
            {
                // Check if the asset is still in the project anymore, if not clear the import status.
                if (!isInProject)
                {
                    AssetDataAttributeCollection = null;
                }
            }

            var unityConnectProxy = ServicesContainer.instance.Resolve<IUnityConnectProxy>();
            if (m_AssetDataAttributesTask == null && unityConnectProxy.AreCloudServicesReachable && isInProject)
            {
                m_AssetDataAttributesTask = GetDatasetAttributesInternalAsync(token);
            }

            if (m_AssetDataAttributesTask != null)
            {
                try
                {
                    await m_AssetDataAttributesTask;
                }
                finally
                {
                    m_AssetDataAttributesTask = null;
                }
            }
        }

        async Task GetDatasetAttributesInternalAsync(CancellationToken token = default)
        {
            var assetsSdkProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();
            var results = await assetsSdkProvider.GatherImportStatusesAsync(new[] {this}, token);

            if (results.TryGetValue(Identifier, out var status))
            {
                AssetDataAttributeCollection = new AssetDataAttributeCollection(new ImportAttribute(status));
            }
            else
            {
                ResetAssetDataAttributes();
            }
        }

        public override async Task ResolveDatasetsAsync(CancellationToken token = default)
        {
            // Because an AssetData is tied to a version, and files modification creates a new version,
            // we can assume that the primary extension is always the same.
            if (m_DatasetProcessed || !string.IsNullOrEmpty(PrimaryExtension))
                return;

            // Wait for the refresh of properties as dataset info will be bundled
            if (m_RefreshPropertiesTask != null)
            {
                await m_RefreshPropertiesTask;
            }

            m_DatasetTask ??= ResolveDatasetInternalAsync(token);

            try
            {
                await m_DatasetTask;
                m_DatasetProcessed = true;
            }
            catch (ForbiddenException)
            {
                // Ignore if the Asset is unavailable
            }
            finally
            {
                m_DatasetTask = null;
            }
        }

        async Task ResolveDatasetInternalAsync(CancellationToken token = default)
        {
            try
            {
                var assetsProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();

                var sourceDataset = Datasets.FirstOrDefault(d => d.IsSource);

                if (sourceDataset == null)
                {
                    Utilities.DevLogWarning($"Source dataset not set for asset {Name}");

                    sourceDataset = new AssetDataset(k_Source, new List<string> {k_Source, k_NotSynced}, null);
                    Datasets = new List<AssetDataset> {sourceDataset};
                }

                var tasks = Datasets.Select(dataset => dataset.GetFilesAsync(assetsProvider, Identifier, token));
                await Task.WhenAll(tasks);

                token.ThrowIfCancellationRequested();

                ResolvePrimaryExtension();

                InvokeEvent(AssetDataEventType.FilesChanged);
            }
            catch (HttpRequestException)
            {
                // Ignore unreachable host
            }
        }

        public override async Task RefreshPropertiesAsync(CancellationToken token = default)
        {
            m_RefreshPropertiesTask ??= RefreshPropertiesInternalAsync(token);
            try
            {
                await m_RefreshPropertiesTask;
            }
            catch (HttpRequestException)
            {
                // Ignore unreachable host
            }
            finally
            {
                m_RefreshPropertiesTask = null;
            }
        }

        async Task RefreshPropertiesInternalAsync(CancellationToken token = default)
        {
            var assetsSdkProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();
            var updatedAsset = await assetsSdkProvider.GetAssetAsync(Identifier, token);
            
            if (updatedAsset == null)
            {
                Utilities.DevLogError($"Asset {Identifier.AssetId} not found; properties could not be returned.");
                return;
            }

            FillFromOther(updatedAsset);

            InvokeEvent(AssetDataEventType.PropertiesChanged);
        }

        public override async Task RefreshDependenciesAsync(CancellationToken token = default)
        {
            m_RefreshDependenciesTask ??= RefreshDependenciesInternalAsync(token);
            try
            {
                await m_RefreshDependenciesTask;
            }
            catch (HttpRequestException)
            {
                // Ignore unreachable host
                Utilities.DevLogError("Failed to refresh dependencies");
            }
            catch (TaskCanceledException)
            {
                Utilities.DevLog("Refresh dependencies cancelled");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                m_RefreshDependenciesTask = null;
            }
        }

        async Task RefreshDependenciesInternalAsync(CancellationToken token)
        {
            var dependencies = new List<AssetIdentifier>();
            await foreach (var dependency in AssetDataDependencyHelper.LoadDependenciesAsync(this, token))
            {
                dependencies.Add(dependency);
            }

            m_Dependencies = dependencies;

            InvokeEvent(AssetDataEventType.DependenciesChanged);
        }

        public override async Task RefreshVersionsAsync(CancellationToken token = default)
        {
            m_RefreshVersionsTask ??= RefreshVersionsInternalAsync(token);
            try
            {
                await m_RefreshVersionsTask;
            }
            catch (HttpRequestException)
            {
                // Ignore unreachable host
            }
            finally
            {
                m_RefreshVersionsTask = null;
            }
        }

        async Task RefreshVersionsInternalAsync(CancellationToken token)
        {
            var assetsSdkProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();
            var versions = new List<BaseAssetData>();
            try
            {
                await foreach (var assetData in assetsSdkProvider.ListVersionInDescendingOrderAsync(m_Identifier, token))
                {
                    versions.Add(assetData);
                    assetData.m_Versions = versions;
                }
            }
            catch (NotFoundException)
            {
                versions.Clear();
            }

            m_Versions = versions;
        }

        public override async Task RefreshLinkedProjectsAsync(CancellationToken token = default)
        {
            m_LinkedProjectsTask ??= RefreshLinkedProjectsInternalAsync(token);
            try
            {
                await m_LinkedProjectsTask;
            }
            catch (HttpRequestException)
            {
                // Ignore unreachable host
            }
            finally
            {
                m_LinkedProjectsTask = null;
            }
        }

        async Task RefreshLinkedProjectsInternalAsync(CancellationToken token = default)
        {
            var assetsSdkProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();
            var linkedProjects = await assetsSdkProvider.GetLinkedProjectsAsync(this, token);

            LinkedProjects = linkedProjects;
        }

        bool TryCopyDataset(AssetDataset dataset, string targetSystemLabel)
        {
            if (dataset.SystemTags.Contains(targetSystemLabel))
            {
                var existingDataset = m_Datasets.Find(x => x.SystemTags.Contains(targetSystemLabel));
                if (existingDataset == null)
                {
                    m_Datasets.Add(dataset);
                }
                else
                {
                    existingDataset.Copy(dataset);
                }

                return true;
            }

            return false;
        }
    }
}
