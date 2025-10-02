using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.Editor
{
    /// <summary>
    /// Provides executing operations on the Asset Manager.
    /// </summary>
    public static class AssetManagerClient
    {
        const int k_Timeout = 10000;

        /// <summary>
        /// Triggers an import operation for assets matching the specified search filter.
        /// </summary>
        /// <param name="filter">A search filter which targets assets for import. </param>
        /// <param name="settings">Additional information for the import. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        public static async Task<ImportResult> ImportAsync(ImportSearchFilter filter, ImportSettings settings = default, CancellationToken cancellationToken = default)
        {
            if (filter.IsEmpty())
            {
                throw new ArgumentException("Empty search filter provided. Cannot import assets.", nameof(filter));
            }

            await ThrowIfServicesUnreachableAsync(cancellationToken);
            ThrowIfCloudServicesUnreachable();
            ThrowIfNotLoggedIn();

            return await ImportAsync(Map(filter), filter.ProjectIds, settings, cancellationToken);
        }

        /// <summary>
        /// Triggers an import operation for the specified assets.
        /// </summary>
        /// <param name="assetIds">An enumeration of asset ids. </param>
        /// <param name="settings">Additional information for the import. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        public static async Task<ImportResult> ImportAsync(IEnumerable<string> assetIds, ImportSettings settings = default, CancellationToken cancellationToken = default)
        {
            if (assetIds == null || !assetIds.Any())
            {
                throw new ArgumentException("No asset IDs provided. Cannot import assets.", nameof(assetIds));
            }

            await ThrowIfServicesUnreachableAsync(cancellationToken);
            ThrowIfCloudServicesUnreachable();
            ThrowIfNotLoggedIn();

            var searchFilter = new AssetSearchFilter
            {
                AssetIds = assetIds.ToList()
            };

            return await ImportAsync(searchFilter, null, settings, cancellationToken);
        }

        /// <summary>
        /// Returns the key for a metadata field based on its display name.
        /// </summary>
        /// <param name="displayName">The display name of the metadata field.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>The key for a metadata field.</returns>
        public static async Task<string> GetMetadataKeyFromDisplayNameAsync(string displayName, CancellationToken cancellationToken = default)
        {
            await ThrowIfServicesUnreachableAsync(cancellationToken);

            var organizationProvider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();
            var metadataFieldDefinition = organizationProvider.SelectedOrganization.MetadataFieldDefinitions.FirstOrDefault(x => x.DisplayName == displayName);
            if (metadataFieldDefinition == null)
            {
                throw new ArgumentException($"No metadata field definition found for display name: {displayName}", nameof(displayName));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return metadataFieldDefinition.Key;
        }

        /// <summary>
        /// Returns the user ID for the specified user name.
        /// </summary>
        /// <remarks>
        /// Because the user name may not be unique, this method may return multiple user IDs.
        /// </remarks>
        /// <param name="userName">A user name to convert to id(s).</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>The list of user ids. </returns>
        public static async Task<IEnumerable<string>> GetUserIdsFromUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            await ThrowIfServicesUnreachableAsync(cancellationToken);

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            var organizationProvider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();
            var userInfos = await organizationProvider.SelectedOrganization.GetUserInfosAsync(cancellationToken: cancellationToken);
            var userIds = userInfos.Where(userInfo => userInfo.Name == userName)
                .Select(userInfo => userInfo.UserId)
                .ToList();

            return userIds;
        }

        /// <summary>
        /// Returns the Unity GUIDs associated with the given asset identifier
        /// </summary>
        /// <param name="assetId">The asset identifier.</param>
        /// <returns>If the given asset identifier is tracked, return all Unity GUIDs related to this asset. Else, return an empty enumerable</returns>
        public static IEnumerable<GUID> GetImportedAssetGUIDs(string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentNullException(nameof(assetId), "No asset identifier provided");
            }

            // Are we initialized?
            if (!ServicesContainer.instance.IsInitialized())
            {
                throw new InitializationException("Asset Manager is not initialized.");
            }

            var assetDataManager = ServicesContainer.instance.Resolve<IAssetDataManager>();
            if (assetDataManager == null)
            {
                throw new InitializationException("Asset Manager is not initialized and cannot access internal service.");
            }

            var importedAssetInfo = assetDataManager.GetImportedAssetInfo(assetId);
            if (importedAssetInfo == null)
            {
                // The asset identifier is unknown
                yield break;
            }

            foreach (var fileInfo in importedAssetInfo.FileInfos)
            {
                if (GUID.TryParse(fileInfo.Guid, out var guid))
                {
                    yield return guid;
                }
                else
                {
                    Utilities.DevLog($"Unable to parse GUID {fileInfo.Guid} from file info for asset {assetId}");
                }
            }
        }

        static async Task<ImportResult> ImportAsync(AssetSearchFilter searchFilter, IEnumerable<string> projectIds, ImportSettings settings, CancellationToken cancellationToken = default)
        {
            if (searchFilter == null)
            {
                throw new ArgumentNullException(nameof(searchFilter), "No search filter provided. Cannot import assets.");
            }

            // Update import settings
            settings.Type = ImportOperation.ImportType.UpdateToLatest;

            var applicationProxy = ServicesContainer.instance.Resolve<IApplicationProxy>();

            if (string.IsNullOrEmpty(settings.DestinationPathOverride))
            {
                settings.DestinationPathOverride = null;
            }
            else
            {
                if (!Path.IsPathRooted(settings.DestinationPathOverride))
                {
                    settings.DestinationPathOverride =
                        PathUtils.Combine(applicationProxy.DataPath, settings.DestinationPathOverride);
                }

                settings.DestinationPathOverride = Utilities.GetPathRelativeToAssetsFolderIncludeAssets(settings.DestinationPathOverride);
            }

            ValidateSettings(settings);

            var organizationProvider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();
            var assetsProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();

            var query = assetsProvider.SearchAsync(
                organizationProvider.SelectedOrganization.Id,
                projectIds,
                searchFilter,
                SortField.Name,
                SortingOrder.Ascending,
                0, 0,
                cancellationToken);

            var assetDatas = new List<BaseAssetData>();
            await foreach (var result in query)
            {
                assetDatas.Add(result);
            }

            ReportExplicitlyProvidedAssetIdsAreImported(searchFilter, assetDatas);

            return await ImportAsync(assetDatas, settings, cancellationToken);
        }

        // Warn if any asset Ids from the assetDatasToImport list are not in the searchFilter.
        // If that happens, this means a user expects certain assets to be imported, but they were not found.
        static void ReportExplicitlyProvidedAssetIdsAreImported(AssetSearchFilter searchFilter,
            List<BaseAssetData> assetDatasToImport)
        {
            HashSet<string> assetIdsToImport = new HashSet<string>(searchFilter.AssetIds ?? Enumerable.Empty<string>());
            HashSet<string> importedAssetIds = new HashSet<string>(assetDatasToImport.Select(x => x.Identifier.AssetId));

            var missingAssetIds = assetIdsToImport.Except(importedAssetIds).ToList();
            if (missingAssetIds.Any())
            {
                UnityEngine.Debug.LogWarning($"The following explicitly provided asset IDs were not found in the search results and will not be imported:\n{ string.Join("\n", missingAssetIds.Select(x => $"{x}"))}");
            }
        }

        static async Task<ImportResult> ImportAsync(IEnumerable<BaseAssetData> assetDatas, ImportSettings settings, CancellationToken cancellationToken)
        {
            var hasAssetsToImport = assetDatas != null && assetDatas.Any();

            Utilities.DevLog(hasAssetsToImport
                ? $"{assetDatas.Count()} assets identified for import."
                : "No assets to import.");

            if (!hasAssetsToImport)
            {
                return new ImportResult {ImportedAssetIds = Array.Empty<string>()};
            }

            // Start the import
            var importer = ServicesContainer.instance.Resolve<IAssetImporter>();
            Utilities.DevLog("Starting asset import...");
            var result = await importer.StartImportAsync(ImportTrigger.AutoImport, assetDatas.ToList(), settings, cancellationToken);

            if (result.OperationInProgress)
            {
                throw new NotSupportedException("Another import operation is still in progress. Please wait for it to finish.");
            }

            if (result.Cancelled)
            {
                throw new OperationCanceledException("The import operation was cancelled.");
            }

            return new ImportResult
            {
                ImportedAssetIds = result.AssetsAndDependencies?.Select(x => x.Identifier.AssetId).ToList()
            };
        }

        static async Task ThrowIfServicesUnreachableAsync(CancellationToken cancellationToken)
        {
            // Are we initialized?
            if (!ServicesContainer.instance.IsInitialized())
            {
                throw new InitializationException("Asset Manager is not initialized.");
            }

            // If this base service is missing, assume the Asset Manager did not initialized.
            var projectOrganizationProvider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();
            if (projectOrganizationProvider == null)
            {
                Utilities.DevLogError("IProjectOrganizationProvider is missing.");
                throw new InitializationException("Asset Manager failed to initialize.");
            }

            // Wait for the project organization provider to load
            if (projectOrganizationProvider.IsLoading)
            {
                Utilities.DevLogWarning("Waiting for services to finish loading...");
            }

            var maxDelay = k_Timeout;

            while (projectOrganizationProvider.IsLoading)
            {
                maxDelay -= 100;
                await Task.Delay(100, cancellationToken);

                if (maxDelay <= 0)
                {
                    throw new TimeoutException("Asset Manager initialization timed out.");
                }
            }

            if (projectOrganizationProvider.SelectedOrganization == null)
            {
                throw new InitializationException("You have not linked an organization to this project.");
            }
        }

        static void ThrowIfCloudServicesUnreachable()
        {
            var unityConnectProxy = ServicesContainer.instance.Resolve<IUnityConnectProxy>();
            if (unityConnectProxy == null)
            {
                Utilities.DevLogError("IUnityConnectProxy is missing.");
                throw new InitializationException("Asset Manager failed to initialize.");
            }

            if (!unityConnectProxy.AreCloudServicesReachable)
            {
                throw new InitializationException("Network is unreachable.");
            }
        }

        static void ThrowIfNotLoggedIn()
        {
            var permissionsManager = ServicesContainer.instance.Resolve<IPermissionsManager>();
            if (permissionsManager is not {AuthenticationState: AuthenticationState.LoggedIn})
            {
                if (permissionsManager == null)
                {
                    Utilities.DevLogError("IPermissionsManager is missing.");
                }

                throw new AuthenticationFailedException("You are not logged in.");
            }
        }

        static void ValidateSettings(ImportSettings settings)
        {
            if (!string.IsNullOrEmpty(settings.DestinationPathOverride))
            {
                var ioProxy = ServicesContainer.instance.Resolve<IIOProxy>();
                if (!ioProxy.DirectoryExists(settings.DestinationPathOverride))
                {
                    throw new ArgumentException("Destination path does not exist.", nameof(settings));
                }
            }
        }

        static AssetSearchFilter Map(ImportSearchFilter filter)
        {
            if (filter.Collections != null && filter.Collections.Any() && (filter.ProjectIds == null || filter.ProjectIds.Count() != 1))
            {
                throw new InvalidOperationException("Collections can only be used with a single project. Please include a single project in the search.");
            }

            return new AssetSearchFilter
            {
                IsExactMatchSearch = true,
                AssetIds = filter.AssetIds?.Where(x => !string.IsNullOrEmpty(x)).ToList(),
                AssetTypes = filter.AssetTypes?.Select(Map).ToList(),
                Tags = filter.Tags?.Where(x => !string.IsNullOrEmpty(x)).ToList(),
                Status = filter.Statuses?.Where(x => !string.IsNullOrEmpty(x)).ToList(),
                Labels = filter.Labels?.Where(x => !string.IsNullOrEmpty(x)).ToList(),
                Collection = filter.Collections?.Where(x => !string.IsNullOrEmpty(x)).ToList(),
                CreatedBy = filter.CreatedByUserIds?.Where(x => !string.IsNullOrEmpty(x)).ToList(),
                UpdatedBy = filter.UpdatedByUserIds?.Where(x => !string.IsNullOrEmpty(x)).ToList(),
                CustomMetadata = Map(filter.CustomMetadata.Values)
            };
        }

        static Core.Editor.AssetType Map(AssetType assetType)
        {
            return assetType switch
            {
                AssetType.Animation => Core.Editor.AssetType.Animation,
                AssetType.Audio => Core.Editor.AssetType.Audio,
                AssetType.Audio_Mixer => Core.Editor.AssetType.AudioMixer,
                AssetType.Font => Core.Editor.AssetType.Font,
                AssetType.Material => Core.Editor.AssetType.Material,
                AssetType.Model_3D => Core.Editor.AssetType.Model3D,
                AssetType.Physics_Material => Core.Editor.AssetType.PhysicsMaterial,
                AssetType.Prefab => Core.Editor.AssetType.Prefab,
                AssetType.Unity_Scene => Core.Editor.AssetType.UnityScene,
                AssetType.Script => Core.Editor.AssetType.Script,
                AssetType.Shader => Core.Editor.AssetType.Shader,
                AssetType.Asset_2D => Core.Editor.AssetType.Asset2D,
                AssetType.Visual_Effect => Core.Editor.AssetType.VisualEffect,
                AssetType.Assembly_Definition => Core.Editor.AssetType.AssemblyDefinition,
                AssetType.Asset => Core.Editor.AssetType.Asset,
                AssetType.Configuration => Core.Editor.AssetType.Configuration,
                AssetType.Document => Core.Editor.AssetType.Document,
                AssetType.Environment => Core.Editor.AssetType.Environment,
                AssetType.Image => Core.Editor.AssetType.Image,
                AssetType.Playable => Core.Editor.AssetType.Playable,
                AssetType.Shader_Graph => Core.Editor.AssetType.ShaderGraph,
                AssetType.Unity_Package => Core.Editor.AssetType.UnityPackage,
                AssetType.Scene => Core.Editor.AssetType.Scene,
                AssetType.Unity_Editor => Core.Editor.AssetType.UnityEditor,
                AssetType.Video => Core.Editor.AssetType.Video,
                _ => Core.Editor.AssetType.Other
            };
        }

        static List<IMetadata> Map(IEnumerable<Metadata> metadatas)
        {
            ValidateMetadatas(metadatas);

            var metadataList = new List<IMetadata>();

            foreach (var metadata in metadatas)
            {
                switch (metadata)
                {
                    case StringMetadata stringMetadata:
                        metadataList.Add(new Core.Editor.TextMetadata(stringMetadata.Key, string.Empty, stringMetadata.Value));
                        break;
                    case NumberMetadata numberMetadata:
                        metadataList.Add(new Core.Editor.NumberMetadata(numberMetadata.Key, string.Empty, numberMetadata.Value));
                        break;
                    case BooleanMetadata booleanMetadata:
                        metadataList.Add(new Core.Editor.BooleanMetadata(booleanMetadata.Key, string.Empty, booleanMetadata.Value));
                        break;
                    case DateTimeMetadata timestampMetadata:
                        var dateTimeString = timestampMetadata.DateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        metadataList.Add(new Core.Editor.TextMetadata(timestampMetadata.Key, string.Empty, dateTimeString));
                        break;
                    case DateTimeRangeMetadata dateTimeRangeMetadata:
                        metadataList.Add(new Core.Editor.TimestampMetadata(dateTimeRangeMetadata.Key, string.Empty, new DateTimeEntry(dateTimeRangeMetadata.StartDateTime)));
                        metadataList.Add(new Core.Editor.TimestampMetadata(dateTimeRangeMetadata.Key, string.Empty, new DateTimeEntry(dateTimeRangeMetadata.EndDateTime)));
                        break;
                    case MultiValueMetadata multiValueMetadata:
                        metadataList.Add(new Core.Editor.MultiSelectionMetadata(multiValueMetadata.Key, string.Empty, multiValueMetadata.Values.ToList()));
                        break;
                }
            }

            return metadataList;
        }

        static void ValidateMetadatas(IEnumerable<Metadata> metadatas)
        {
            if (metadatas == null || !metadatas.Any())
            {
                return;
            }

            var keys = new HashSet<string>();

            foreach (var metadata in metadatas)
            {
                if (!keys.Add(metadata.Key))
                {
                    throw new ArgumentException("Duplicate key found: " + metadata.Key);
                }

                metadata.Validate();
            }
        }
    }
}
