using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.AssetsEmbedded;
using Unity.Cloud.CommonEmbedded;
using UnityEngine;
using UnityEngine.TestTools;
using Task = System.Threading.Tasks.Task;

namespace Unity.AssetManager.Core.Editor
{
    enum AssetSearchGroupBy
    {
        Name,
        Status,
        CreatedBy,
        UpdatedBy,
        Type
    }

    enum SortField
    {
        Name,
        Updated,
        Created,
        Description,
        PrimaryType,
        Status,
        ImportStatus
    }

    enum SortingOrder
    {
        Ascending,
        Descending
    }

    interface IAssetsProvider : IService
    {
        int DefaultSearchPageSize { get; }

        // Assets

        Task<AssetData> GetAssetAsync(AssetIdentifier assetIdentifier, CancellationToken token);
        Task<AssetData> GetLatestAssetVersionAsync(AssetIdentifier assetIdentifier, CancellationToken token);
        Task<string> GetLatestAssetVersionLiteAsync(AssetIdentifier assetIdentifier, CancellationToken token);
        IAsyncEnumerable<AssetData> ListVersionInDescendingOrderAsync(AssetIdentifier assetIdentifier, CancellationToken token);

        IAsyncEnumerable<AssetData> SearchAsync(string organizationId, IEnumerable<string> projectIds,
            AssetSearchFilter assetSearchFilter, SortField sortField, SortingOrder sortingOrder, int startIndex,
            int pageSize, CancellationToken token);
        IAsyncEnumerable<AssetIdentifier> SearchLiteAsync(string organizationId, IEnumerable<string> projectIds,
            AssetSearchFilter assetSearchFilter, SortField sortField, SortingOrder sortingOrder, int startIndex,
            int pageSize, CancellationToken token);
        Task<List<string>> GetFilterSelectionsAsync(string organizationId, IEnumerable<string> projectIds,
            AssetSearchFilter assetSearchFilter, AssetSearchGroupBy groupBy, CancellationToken token);
        Task<List<string>> GetFilterSelectionsAsync(string organizationId, IEnumerable<string> projectIds,
            AssetSearchFilter assetSearchFilter, string metadataField, CancellationToken token);

        Task<AssetData> CreateAssetAsync(ProjectIdentifier projectIdentifier, AssetCreation assetCreation, CancellationToken token);
        Task<AssetData> CreateUnfrozenVersionAsync(AssetData assetData, CancellationToken token);
        Task RemoveUnfrozenAssetVersion(AssetIdentifier assetIdentifier, CancellationToken token);
        Task RemoveAsset(AssetIdentifier assetIdentifier, CancellationToken token);
        Task UpdateAsync(AssetData assetData, AssetUpdate assetUpdate, CancellationToken token);
        Task UpdateStatusAsync(AssetData assetData, string statusName, CancellationToken token);
        Task FreezeAsync(AssetData assetData, string changeLog, CancellationToken token);
        Task<Uri> GetPreviewUrlAsync(AssetData assetData, int maxDimension, CancellationToken token);

        Task<ImportStatuses> GatherImportStatusesAsync(IEnumerable<BaseAssetData> assetDatas, CancellationToken token);

        IAsyncEnumerable<AssetDependency> GetDependenciesAsync(AssetIdentifier assetIdentifier, Range range, CancellationToken token);
        IAsyncEnumerable<AssetIdentifier> GetDependentsAsync(AssetIdentifier assetIdentifier, Range range, CancellationToken token);
        Task UpdateDependenciesAsync(AssetIdentifier assetIdentifier, IEnumerable<AssetIdentifier> assetDependencies, IEnumerable<AssetDependency> existingDependencies, CancellationToken token);

        Task<IEnumerable<ProjectIdentifier>> GetLinkedProjectsAsync(AssetData assetData, CancellationToken token);

        // Files

        Task<AssetDataFile> UploadThumbnail(AssetData assetData, Texture2D thumbnail, IProgress<HttpProgress> progress, CancellationToken token);
        Task RemoveThumbnail(AssetData assetData, CancellationToken token);
        Task<AssetDataFile> UploadFile(AssetData assetData, string destinationPath, Stream stream, IProgress<HttpProgress> progress, CancellationToken token);
        Task RemoveAllFiles(AssetData assetData, CancellationToken token);
        IAsyncEnumerable<AssetDataFile> ListFilesAsync(AssetIdentifier assetIdentifier, AssetDataset assetDataset, Range range, CancellationToken token);

        // Datasets

        Task<AssetDataset> GetDatasetAsync(AssetData assetData, IEnumerable<string> systemTags, CancellationToken token);
        Task<IReadOnlyDictionary<string, Uri>> GetDatasetDownloadUrlsAsync(AssetIdentifier assetIdentifier, AssetDataset assetDataset, IProgress<FetchDownloadUrlsProgress> progress, CancellationToken token);
        
        // Utilities

        string GetValueAsString(AssetType assetType);
        bool TryParse(string assetTypeString, out AssetType assetType);
    }

    [Serializable]
    class AssetsSdkProvider : BaseSdkService, IAssetsProvider, AssetsSdkProvider.IDataMapper
    {
        public override Type RegistrationType => typeof(IAssetsProvider);

        const string k_ThumbnailFilename = "unity_thumbnail.png";
        static readonly string k_SourceDatasetTag = "Source";
        static readonly string k_PreviewDatasetTag = "Preview";

        [SerializeReference]
        ISettingsManager m_SettingsManager;

        IDataMapper m_DataMapperOverride;

        IDataMapper DataMapper => m_DataMapperOverride ?? this;

        public int DefaultSearchPageSize => 99;

        public AssetsSdkProvider() { }

        /// <inheritdoc />
        /// <remarks>
        /// IMPORTANT: Since m_AssetRepositoryOverride does not support domain reload, the AssetsSdkProvider constructed cannot
        /// be used across domain reloads
        /// </remarks>
        internal AssetsSdkProvider(SdkServiceOverride sdkServiceOverride)
            : base(sdkServiceOverride) { }

        [ServiceInjection]
        public void Inject(ISettingsManager settingsManager)
        {
            m_SettingsManager = settingsManager;
        }

        /// <summary>
        /// Sets parameter for testing
        /// </summary>
        internal AssetsSdkProvider With(ISettingsManager settingsManager)
        {
            m_SettingsManager = settingsManager;
            return this;
        }

        /// <summary>
        /// Sets parameter for testing
        /// </summary>
        internal AssetsSdkProvider With(IDataMapper dataMapperOverride)
        {
            m_DataMapperOverride = dataMapperOverride;
            return this;
        }

        public async IAsyncEnumerable<AssetData> SearchAsync(string organizationId, IEnumerable<string> projectIds,
            AssetSearchFilter assetSearchFilter, SortField sortField, SortingOrder sortingOrder, int startIndex,
            int pageSize, [EnumeratorCancellation] CancellationToken token)
        {
            var cacheConfiguration = new AssetCacheConfiguration
            {
                CacheProperties = true,
                CacheDatasetList = true,
                DatasetCacheConfiguration = new DatasetCacheConfiguration {CacheProperties = true}
            };

            await foreach (var asset in SearchAsync(new OrganizationId(organizationId), projectIds, assetSearchFilter,
                               sortField, sortingOrder, startIndex, pageSize, cacheConfiguration, token))
            {
                yield return asset == null ? null : await DataMapper.From(asset, token);
            }
        }

        public async IAsyncEnumerable<AssetIdentifier> SearchLiteAsync(string organizationId, IEnumerable<string> projectIds,
            AssetSearchFilter assetSearchFilter, SortField sortField, SortingOrder sortingOrder, int startIndex,
            int pageSize, [EnumeratorCancellation] CancellationToken token)
        {
            await foreach (var asset in SearchAsync(new OrganizationId(organizationId), projectIds, assetSearchFilter,
                               sortField, sortingOrder, startIndex, pageSize, AssetCacheConfiguration.NoCaching, token))
            {
                yield return Map(asset.Descriptor);
            }
        }

        public Task<List<string>> GetFilterSelectionsAsync(string organizationId, IEnumerable<string> projectIds,
            AssetSearchFilter assetSearchFilter, AssetSearchGroupBy groupBy, CancellationToken token)
        {
            return GetFilterSelectionsAsync(organizationId, projectIds, assetSearchFilter, Map(groupBy), token);
        }

        public Task<List<string>> GetFilterSelectionsAsync(string organizationId, IEnumerable<string> projectIds,
            AssetSearchFilter assetSearchFilter, string metadataField, CancellationToken token)
        {
            var groupable = Groupable.FromMetadata(MetadataOwner.Asset, metadataField);
            return GetFilterSelectionsAsync(organizationId, projectIds, assetSearchFilter, groupable, token);
        }

        async Task<List<string>> GetFilterSelectionsAsync(string organizationId, IEnumerable<string> projectIds,
            AssetSearchFilter assetSearchFilter, Groupable groupable, CancellationToken token)
        {
            var strongTypedOrgId = new OrganizationId(organizationId);
            var projectDescriptors = projectIds.Select(p => new ProjectDescriptor(strongTypedOrgId, new ProjectId(p))).ToList();

            var query = AssetRepository.GroupAndCountAssets(projectDescriptors)?
                .SelectWhereMatchesFilter(Map(assetSearchFilter));

            var keys = new HashSet<string>();
            await foreach (var kvp in DataMapper.GroupAndCountAsync(query, groupable, token))
            {
                keys.Add(kvp.Key);
            }

            var sortedList = keys.ToList();
            sortedList.Sort();
            return sortedList;
        }

        public async Task<AssetData> CreateAssetAsync(ProjectIdentifier projectIdentifier, AssetCreation assetCreation, CancellationToken token)
        {
            var projectDescriptor = Map(projectIdentifier);
            var cloudAssetCreation = Map(assetCreation);
            var project = await AssetRepository.GetAssetProjectAsync(projectDescriptor, token);
            var assetDescriptor = await project.CreateAssetLiteAsync(cloudAssetCreation, token);

            return await Map(assetDescriptor, token);
        }

        public async Task<AssetData> CreateUnfrozenVersionAsync(AssetData assetData, CancellationToken token)
        {
            var asset = await InternalGetAssetAsync(assetData, token);
            if (asset != null)
            {
                try
                {
                    var unfrozenVersionDescriptor = await asset.CreateUnfrozenVersionLiteAsync(token);
                    return await Map(unfrozenVersionDescriptor, token);
                }
                catch (NotFoundException e)
                {
                    Utilities.DevLog(e.Detail);
                }
            }

            return null;
        }

        Task<IAsset> InternalGetAssetAsync(AssetData assetData, CancellationToken token)
        {
            if (assetData == null)
            {
                throw new ArgumentNullException(nameof(assetData));
            }

            return InternalGetAssetAsync(assetData.Identifier, token);
        }

        async Task<IAsset> InternalGetAssetAsync(AssetIdentifier assetIdentifier, CancellationToken token)
        {
            return await AssetRepository.GetAssetAsync(Map(assetIdentifier), token);
        }

        public async Task RemoveUnfrozenAssetVersion(AssetIdentifier assetIdentifier, CancellationToken token)
        {
            if (assetIdentifier == null)
            {
                return;
            }

            var project = await AssetRepository.GetAssetProjectAsync(Map(assetIdentifier.ProjectIdentifier), token);
            await project.DeleteUnfrozenAssetVersionAsync(new AssetId(assetIdentifier.AssetId), new AssetVersion(assetIdentifier.Version), token);
        }
        public async Task RemoveAsset(AssetIdentifier assetIdentifier, CancellationToken token)
        {
            if (assetIdentifier == null)
            {
                return;
            }

            var project = await AssetRepository.GetAssetProjectAsync(Map(assetIdentifier.ProjectIdentifier), token);
            await project.UnlinkAssetsAsync(new[] {new AssetId(assetIdentifier.AssetId)}, token);
        }

        public async Task<AssetData> GetAssetAsync(AssetIdentifier assetIdentifier, CancellationToken token)
        {
            return await Map(Map(assetIdentifier), token);
        }

        async Task UpdateMetadata(AssetIdentifier assetIdentifier, List<IMetadata> metadata, CancellationToken token)
        {
            var asset = await InternalGetAssetAsync(assetIdentifier, token);

            var keyToDelete = new List<string>();
            var metadataToAddOrUpdate = metadata.ToDictionary(m => m.FieldKey, Map);

            await foreach (var key in DataMapper.ListMetadataKeysAsync(asset.Metadata as IReadOnlyMetadataContainer, token))
            {
                if (!metadataToAddOrUpdate.ContainsKey(key))
                {
                    keyToDelete.Add(key);
                }
            }

            if (keyToDelete.Count > 0)
            {
                await asset.Metadata.RemoveAsync(keyToDelete, token);
            }

            if (metadataToAddOrUpdate.Count > 0)
            {
                try
                {
                    await asset.Metadata.AddOrUpdateAsync(metadataToAddOrUpdate, token);
                }
                catch (InvalidArgumentException ex)
                {
                    Utilities.DevLogError("Metadata update failed. Please make sure every metadata value is valid. Error: " + ex.Message);
                }
            }
        }

        public async Task<AssetData> GetLatestAssetVersionAsync(AssetIdentifier assetIdentifier, CancellationToken token)
        {
            var asset = await GetLatestAssetVersionAsync(assetIdentifier, GetAssetCacheConfigurationForMapping(), token);
            return await DataMapper.From(asset, token);
        }

        public async Task<string> GetLatestAssetVersionLiteAsync(AssetIdentifier assetIdentifier, CancellationToken token)
        {
            var asset = await GetLatestAssetVersionAsync(assetIdentifier, AssetCacheConfiguration.NoCaching, token);
            return asset?.Descriptor.AssetVersion.ToString();
        }

        async Task<IAsset> GetLatestAssetVersionAsync(AssetIdentifier assetIdentifier, AssetCacheConfiguration assetCacheConfiguration, CancellationToken token)
        {
            if (assetIdentifier == null)
            {
                throw new ArgumentNullException(nameof(assetIdentifier));
            }

            var projectDescriptor = Map(assetIdentifier.ProjectIdentifier);
            var assetId = new AssetId(assetIdentifier.AssetId);

            try
            {
                var asset = await AssetRepository.GetAssetAsync(projectDescriptor, assetId, "Latest", token);
                return await asset.WithCacheConfigurationAsync(assetCacheConfiguration, token);
            }
            catch (NotFoundException)
            {
                try
                {
                    Utilities.DevLog($"Latest version not found, fetching the latest version in descending order (slower): {assetIdentifier.AssetId}");

                    var project = await AssetRepository.GetAssetProjectAsync(projectDescriptor, token);
                    var versionQuery = project.QueryAssetVersions(assetId)?
                        .WithCacheConfiguration(assetCacheConfiguration)
                        .LimitTo(new Range(0, 1));

                    var enumerator = DataMapper.ListAssetsAsync(versionQuery, token).GetAsyncEnumerator(token);

                    return await enumerator.MoveNextAsync() ? enumerator.Current : default;
                }
                catch (NotFoundException)
                {
                    // Ignore, asset could not be found on cloud.
                    return null;
                }
            }
        }

        public async IAsyncEnumerable<AssetData> ListVersionInDescendingOrderAsync(AssetIdentifier assetIdentifier, [EnumeratorCancellation] CancellationToken token)
        {
            if (assetIdentifier == null)
            {
                yield break;
            }

            var project = await AssetRepository.GetAssetProjectAsync(Map(assetIdentifier.ProjectIdentifier), token);
            var assetId = new AssetId(assetIdentifier.AssetId);

            var versionQuery = project.QueryAssetVersions(assetId)
                .WithCacheConfiguration(GetAssetCacheConfigurationForMapping())
                .OrderBy("versionNumber", Unity.Cloud.AssetsEmbedded.SortingOrder.Descending);

            await foreach (var version in DataMapper.ListAssetsAsync(versionQuery, token))
            {
                yield return await DataMapper.From(version, token);
            }
        }

        async Task<IAsset> FindAssetAsync(OrganizationId organizationId, Cloud.AssetsEmbedded.AssetSearchFilter filter, AssetCacheConfiguration cacheConfiguration, CancellationToken token)
        {
            var assetsQuery = AssetRepository.QueryAssets(organizationId)
                .SelectWhereMatchesFilter(filter)
                .WithCacheConfiguration(cacheConfiguration)
                .LimitTo(new Range(0, 1));

            var enumerator = DataMapper.ListAssetsAsync(assetsQuery, token).GetAsyncEnumerator(token);

            return await enumerator.MoveNextAsync() ? enumerator.Current : default;
        }

        public async Task<ImportStatuses> GatherImportStatusesAsync(IEnumerable<BaseAssetData> assetDatas,
            CancellationToken token)
        {
            // Split the searches by organization
            var assetsByOrg = new Dictionary<string, List<BaseAssetData>>();
            foreach (var assetData in assetDatas)
            {
                if (string.IsNullOrEmpty(assetData.Identifier.OrganizationId))
                    continue;

                if (!assetsByOrg.ContainsKey(assetData.Identifier.OrganizationId))
                {
                    assetsByOrg.Add(assetData.Identifier.OrganizationId, new List<BaseAssetData>());
                }

                assetsByOrg[assetData.Identifier.OrganizationId].Add(assetData);
            }

            if (assetsByOrg.Count > 1)
            {
                Utilities.DevLog("Initiating search in multiple organizations.");
            }

            var results = await Task.WhenAll(assetsByOrg.Select(kvp => GatherImportStatusesAsync(kvp.Key, kvp.Value, token)));
            return new ImportStatuses(results);
        }

        async Task<ImportStatuses> GatherImportStatusesAsync(string organizationId, List<BaseAssetData> assetDatas, CancellationToken token)
        {
            // Split the asset list into chunks for multiple searches.

            var strongTypedOrganizationId = new OrganizationId(organizationId);

            var tasks = new List<Task<ImportStatuses>>();
            var startIndex = 0;
            while (startIndex < assetDatas.Count)
            {
                var maxCount = Math.Min(DefaultSearchPageSize, assetDatas.Count - startIndex);
                tasks.Add(GatherImportStatusesAsync(strongTypedOrganizationId, assetDatas.GetRange(startIndex, maxCount), token));
                startIndex += DefaultSearchPageSize;
            }

            try
            {
                var results = await Task.WhenAll(tasks);
                return new ImportStatuses(results);
            }
            catch (ForbiddenException)
            {
                Utilities.DevLog($"Organization {organizationId} cannot be accessed at this time.");

                var errorStatuses = new ImportStatuses();
                assetDatas.ForEach(kvp => errorStatuses[kvp.Identifier] = ImportAttribute.ImportStatus.ErrorSync);
                return errorStatuses;
            }
        }

        async Task<ImportStatuses> GatherImportStatusesAsync(OrganizationId organizationId, List<BaseAssetData> assetDatas, CancellationToken token)
        {
            // If there is only 1 asset, fetch that asset info directly (search has more overhead than a direct fetch).
            if (assetDatas.Count == 1)
            {
                var assetData = assetDatas[0];
                var result = await GatherImportStatusesAsync(assetData, token);
                return new ImportStatuses
                {
                    {assetData.Identifier, result}
                };
            }

            var results = new ImportStatuses();

            // When there are multiple assets, initiate a search to batch asset infos.
            var searchFilter = new Unity.Cloud.AssetsEmbedded.AssetSearchFilter();

            var assetIds = assetDatas.Select(x => new AssetId(x.Identifier.AssetId)).ToArray();
            searchFilter.Include().Id.WithValue(string.Join(' ', assetIds));

            var assetsQuery = AssetRepository.QueryAssets(organizationId)?
                .SelectWhereMatchesFilter(searchFilter)
                .WithCacheConfiguration(new AssetCacheConfiguration
                {
                    CacheProperties = true
                });

            try
            {
                await foreach (var asset in DataMapper.ListAssetsAsync(assetsQuery, token))
                {
                    var identifier = Map(asset.Descriptor);
                    var assetData = assetDatas.Find(x =>
                        x.Identifier.OrganizationId == identifier.OrganizationId &&
                        x.Identifier.ProjectId == identifier.ProjectId &&
                        x.Identifier.AssetId == identifier.AssetId);

                    if (assetData != null)
                    {
                        var status = await GatherImportStatusesAsync(assetData, asset, token);
                        results[assetData.Identifier] = status;
                    }
                }
            }
            catch (ForbiddenException e)
            {
                Utilities.DevLogException(e);
            }
            catch (NotFoundException e)
            {
                Utilities.DevLogException(e);
            }
            finally
            {
                // Add all datas still missing a status
                foreach (var assetData in assetDatas)
                {
                    results.TryAdd(assetData.Identifier, ImportAttribute.ImportStatus.ErrorSync);
                }
            }

            return results;
        }

        async Task<ImportAttribute.ImportStatus> GatherImportStatusesAsync(BaseAssetData assetData, CancellationToken token)
        {
            if (assetData == null)
            {
                return ImportAttribute.ImportStatus.NoImport;
            }

            var assetCacheConfiguration = new AssetCacheConfiguration
            {
                CacheProperties = true
            };
            var cloudAsset = await GetLatestAssetVersionAsync(assetData.Identifier, assetCacheConfiguration, token);

            return await GatherImportStatusesAsync(assetData, cloudAsset, token);
        }

        async Task<ImportAttribute.ImportStatus> GatherImportStatusesAsync(BaseAssetData assetData, IAsset cloudAsset,
            CancellationToken token)
        {
            if (cloudAsset == null)
            {
                return ImportAttribute.ImportStatus.ErrorSync;
            }

            // Even if cloudAsset is != null, we might need to check if the project is archived or not.

            var cloudAssetProperties = await DataMapper.From(cloudAsset, token);

            return assetData.SequenceNumber == cloudAssetProperties.SequenceNumber &&
                assetData.Updated != null && assetData.Updated == cloudAssetProperties.Updated
                    ? ImportAttribute.ImportStatus.UpToDate
                    : ImportAttribute.ImportStatus.OutOfDate;
        }

        public async IAsyncEnumerable<AssetDependency> GetDependenciesAsync(AssetIdentifier assetIdentifier, Range range,
            [EnumeratorCancellation] CancellationToken token)
        {
            var assetDescriptor = Map(assetIdentifier);
            var project = await AssetRepository.GetAssetProjectAsync(assetDescriptor.ProjectDescriptor, token);

            await foreach (var reference in GetDependenciesAsync(project, assetDescriptor, range,
                               AssetReferenceSearchFilter.Context.Source, token))
            {
                var referenceIdentifier = await FindAssetIdentifierAsync(project, reference, token);
                if (referenceIdentifier != null)
                {
                    yield return new AssetDependency(reference.ReferenceId, referenceIdentifier);
                }
            }
        }

        async Task<AssetIdentifier> FindAssetIdentifierAsync(IAssetProject project, IAssetReference reference, CancellationToken token)
        {
            // Referenced by version
            if (reference.TargetAssetVersion.HasValue)
            {
                return new AssetIdentifier(project.Descriptor.OrganizationId.ToString(),
                    project.Descriptor.ProjectId.ToString(),
                    reference.TargetAssetId.ToString(),
                    reference.TargetAssetVersion.Value.ToString());
            }

            // Referenced by label
            if (!string.IsNullOrEmpty(reference.TargetLabel))
            {
                try
                {
                    // Try to fetch the asset from the current project
                    var asset = await project.GetAssetAsync(reference.TargetAssetId, reference.TargetLabel, token);
                    var identifier = Map(asset.Descriptor);
                    identifier.VersionLabel = reference.TargetLabel;
                    return identifier;
                }
                catch (NotFoundException)
                {
                    // Continue to search for the asset in the entire organization
                }

                var filter = new Cloud.AssetsEmbedded.AssetSearchFilter();
                filter.Include().Id.WithValue(reference.TargetAssetId.ToString());
                filter.Include().Labels.WithValue(reference.TargetLabel);

                var result = await FindAssetAsync(project.Descriptor.OrganizationId, filter, AssetCacheConfiguration.NoCaching, token);
                if (result == null) return null;

                var resultIdentifier = Map(result.Descriptor);
                resultIdentifier.VersionLabel = reference.TargetLabel;
                return resultIdentifier;
            }

            return null;
        }

        async IAsyncEnumerable<IAssetReference> GetDependenciesAsync(IAssetProject project, AssetDescriptor assetDescriptor, Range range, AssetReferenceSearchFilter.Context context, [EnumeratorCancellation] CancellationToken token)
        {
            var filter = new AssetReferenceSearchFilter();
            filter.AssetVersion.WhereEquals(assetDescriptor.AssetVersion);
            filter.ReferenceContext.WhereEquals(context);

            var query = project.QueryAssetReferences(assetDescriptor.AssetId)
                .SelectWhereMatchesFilter(filter)
                .LimitTo(range);

            var enumerator = DataMapper.ListAssetReferencessAsync(query, token).GetAsyncEnumerator(token);

            while (await MoveNextAsync())
            {
                var reference = enumerator.Current;
                if (reference is not {IsValid: true})
                {
                    continue;
                }

                yield return reference;
            }

            yield break;

            async Task<bool> MoveNextAsync()
            {
                try
                {
                    return await enumerator.MoveNextAsync();
                }
                catch (TaskCanceledException)
                {
                    // Ignore
                }
                catch (NotFoundException e)
                {
                    Utilities.DevLogError($"Asset {assetDescriptor.AssetId} not found; reference list could not be returned.\n{e.Message}");
                }

                return false;
            }
        }

        public async IAsyncEnumerable<AssetIdentifier> GetDependentsAsync(AssetIdentifier assetIdentifier, Range range, [EnumeratorCancellation] CancellationToken token)
        {
            var assetDescriptor = Map(assetIdentifier);
            var project = await AssetRepository.GetAssetProjectAsync(assetDescriptor.ProjectDescriptor, token);

            await foreach(var reference in GetDependenciesAsync(project, assetDescriptor, range, AssetReferenceSearchFilter.Context.Target, token))
            {
                yield return new AssetIdentifier(assetIdentifier.OrganizationId,
                    assetIdentifier.ProjectId,
                    reference.SourceAssetId.ToString(),
                    reference.SourceAssetVersion.ToString());
            }
        }

        public async Task UpdateDependenciesAsync(AssetIdentifier assetIdentifier,
            IEnumerable<AssetIdentifier> assetDependencies, IEnumerable<AssetDependency> existingDependencies,
            CancellationToken token)
        {
            var assetDescriptor = Map(assetIdentifier);
            var project = await AssetRepository.GetAssetProjectAsync(assetDescriptor.ProjectDescriptor, token);
            var asset = await AssetRepository.GetAssetAsync(assetDescriptor, token);

            // For instance:
            // Current dependencies = d1, d2, d3
            // New dependencies = d1, d4
            // Dependencies to remove from target asset: d2, d3
            // Dependencies to add to target asset: d4
            var assetDependenciesList = assetDependencies.Select(Map).ToList();

            var tasks = new List<Task>();

            // Remove all dependencies that are not explicitly ignored.
            // This will remove d1 from assetDependenciesList
            // And remove d2, d3 from A
            foreach (var existingDependency in existingDependencies) // For each d1, d2, d3
            {
                // Remove existing dependencies from the input list.
                // d1 will be removed
                var nb = assetDependenciesList.RemoveAll(dep =>
                    dep.AssetId.ToString() == existingDependency.TargetAssetIdentifier.AssetId &&
                    dep.AssetVersion.ToString() == existingDependency.TargetAssetIdentifier.Version);

                // If this dependency is not in the input list, remove it from target asset
                // d2 and d3 will be removed
                if (nb == 0)
                {
                    tasks.Add(asset.RemoveReferenceAsync(existingDependency.ReferenceId, token));
                }
            }

            // Add any remaining dependencies
            // d4 will be added
            tasks.AddRange(m_SettingsManager.IsUploadDependenciesUsingLatestLabel
                ? assetDependenciesList.Select(dependencyDescriptor => asset.AddReferenceAsync(dependencyDescriptor.AssetId, "Latest", token))
                : assetDependenciesList.Select(dependencyDescriptor => asset.AddReferenceAsync(dependencyDescriptor.AssetId, dependencyDescriptor.AssetVersion, token)));

            await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<ProjectIdentifier>> GetLinkedProjectsAsync(AssetData assetData, CancellationToken token)
        {
            var asset = await InternalGetAssetAsync(assetData, token);

            try
            {
                return await DataMapper.GetLinkedProjectsAsync(asset, token);
            }
            catch (ForbiddenException e)
            {
                Utilities.DevLog(e.Detail);
                return Array.Empty<ProjectIdentifier>();
            }
            catch (NotFoundException e)
            {
                Utilities.DevLog(e.Detail);
                return Array.Empty<ProjectIdentifier>();
            }
        }

        async IAsyncEnumerable<IAsset> SearchAsync(OrganizationId organizationId, IEnumerable<string> projectIds,
            AssetSearchFilter assetSearchFilter, SortField sortField, SortingOrder sortingOrder, int startIndex, int pageSize,
            AssetCacheConfiguration assetCacheConfiguration, [EnumeratorCancellation] CancellationToken token)
        {
            // Ensure that page size stays within range
            if (int.MaxValue - startIndex < pageSize)
            {
                pageSize = int.MaxValue - startIndex;
            }

            // Current issue in SDK when calculating limit requires temporary local fix:
            // var range = new Range(startIndex, pageSize > 0 ? startIndex + pageSize : Index.FromEnd(pageSize));
            var range = new Range(startIndex, pageSize > 0 ? startIndex + pageSize : Math.Max(0, int.MaxValue - startIndex - pageSize));

            var projectDescriptors = projectIds?
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(id => new ProjectDescriptor(organizationId, new ProjectId(id)))
                .ToList();

            var assetsQuery = projectDescriptors?.Count > 0
                ? AssetRepository.QueryAssets(projectDescriptors)
                : AssetRepository.QueryAssets(organizationId);
            assetsQuery?.LimitTo(range)
                .WithCacheConfiguration(assetCacheConfiguration)
                .SelectWhereMatchesFilter(Map(assetSearchFilter))
                .OrderBy(sortField.ToString(), Map(sortingOrder));

            await foreach (var asset in DataMapper.ListAssetsAsync(assetsQuery, token))
            {
                yield return asset;
            }
        }

        public async Task UpdateAsync(AssetData assetData, AssetUpdate assetUpdate, CancellationToken token)
        {
            var cloudAssetUpdate = Map(assetUpdate);
            var asset = await InternalGetAssetAsync(assetData, token);
            if (asset == null)
            {
                return;
            }

            await asset.UpdateAsync(cloudAssetUpdate, token);

            // If the metadata is empty, we may still need to remove existing metadata
            if (assetUpdate.Metadata != null)
            {
                await UpdateMetadata(assetData.Identifier, assetUpdate.Metadata, token);
            }
        }

        public async Task UpdateStatusAsync(AssetData assetData, string statusName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(statusName))
            {
                throw new ArgumentException("Status name cannot be null or empty", nameof(statusName));
            }

            var asset = await InternalGetAssetAsync(assetData, token);
            if (asset == null)
            {
                return;
            }

            await asset.UpdateStatusAsync(statusName, token);
        }

        public async Task FreezeAsync(AssetData assetData, string changeLog, CancellationToken token)
        {
            var asset = await InternalGetAssetAsync(assetData, token);
            if (asset == null)
            {
                return;
            }

            var assetFreeze = new AssetFreeze(changeLog);
            await asset.FreezeAsync(assetFreeze, token);
        }

        public async Task<Uri> GetPreviewUrlAsync(AssetData assetData, int maxDimension, CancellationToken token)
        {
            try
            {
                var previewFilePath = assetData.PreviewFilePath;

                // [Backwards Compatability] If the preview file path has not been set, we need to fetch it from the asset
                if (string.IsNullOrEmpty(previewFilePath))
                {
                    previewFilePath = await GetPreviewFilePathAsync(assetData, token);
                }

                if (previewFilePath == "/")
                {
                    return null;
                }

                var assetDescriptor = Map(assetData.Identifier);

                var indexOfFirstSlash = previewFilePath.IndexOf("/");
                var datasetId = previewFilePath[..indexOfFirstSlash];
                var filePath = previewFilePath.Substring(indexOfFirstSlash + 1, previewFilePath.Length - datasetId.Length - 1);

                var fileDescriptor = new FileDescriptor(new DatasetDescriptor(assetDescriptor, new DatasetId(datasetId)), filePath);

                var file = await AssetRepository.GetFileAsync(fileDescriptor, token);
                if (file == null)
                {
                    return null;
                }

                return await file.GetResizedImageDownloadUrlAsync(maxDimension, token);
            }
            catch (NotFoundException)
            {
                // Ignore if the preview is not found
            }
            catch (InvalidArgumentException)
            {
                // Ignore if the preview doesn't support resizing
            }
            catch (ServiceException e)
            {
                // Private Cloud services throw 500 error for unsupported preview resizing.
                // If the status code is anything other than InternalServerError, we rethrow the exception.
                if (e.StatusCode != HttpStatusCode.InternalServerError)
                {
                    throw;
                }
            }

            return null;
        }

        async Task<string> GetPreviewFilePathAsync(AssetData assetData, CancellationToken token)
        {
            var asset = await InternalGetAssetAsync(assetData, token);
            return await DataMapper.GetPreviewFilePath(asset, token);
        }

        public async Task RemoveThumbnail(AssetData assetData, CancellationToken token)
        {
            var dataset = await GetDatasetAsync(assetData, k_PreviewDatasetTag, default, token);
            if (dataset != null)
            {
                try
                {
                    await dataset.RemoveFileAsync(k_ThumbnailFilename, token);
                }
                catch (NotFoundException)
                {
                    // Ignore if the file is not found
                }
            }
        }

        public async Task<AssetDataFile> UploadFile(AssetData assetData, string destinationPath, Stream stream, IProgress<HttpProgress> progress, CancellationToken token)
        {
            if (assetData == null || string.IsNullOrEmpty(destinationPath) || stream == null)
            {
                return null;
            }

            var dataset = await GetDatasetAsync(assetData, k_SourceDatasetTag, default, token);
            if (dataset != null)
            {
                var file = await UploadFileToDataset(dataset, destinationPath, stream, progress, token);
                if (file != null)
                {
                    // For files that are textures which are preview supported, generate tags if applicable.
                    if (AssetDataTypeHelper.IsSupportingPreviewGeneration(Path.GetExtension(destinationPath)))
                    {
                        await GenerateAndAssignTags(file, token);
                    }

                    await LinkToPreviewAsync(assetData, file.Descriptor, token);

                    return await DataMapper.From(file, token);
                }
            }

            return null;
        }

        async Task LinkToPreviewAsync(BaseAssetData assetData, FileDescriptor fileDescriptor, CancellationToken token)
        {
            var unityAssetType = AssetDataTypeHelper.GetUnityAssetType(Path.GetExtension(fileDescriptor.Path));
            if (unityAssetType == AssetType.Audio)
            {
                // Datasets must be in a 'commit' status before referencing files from them.
                var sourceDataset = await AssetRepository.GetDatasetAsync(fileDescriptor.DatasetDescriptor, token);
                await DataMapper.WaitForDatasetCommitAsync(sourceDataset, token);

                var dataset = await GetDatasetAsync(assetData, k_PreviewDatasetTag, default, token);
                if (dataset != null)
                {
                    await dataset.AddExistingFileLiteAsync(fileDescriptor.Path, fileDescriptor.DatasetId, token);
                }
            }
        }

        public async Task RemoveAllFiles(AssetData assetData, CancellationToken token)
        {
            var asset = await InternalGetAssetAsync(assetData, token);
            if (asset == null)
            {
                return;
            }

            var cacheConfiguration = new AssetCacheConfiguration
            {
                DatasetCacheConfiguration = new DatasetCacheConfiguration
                {
                    CacheProperties = true,
                }
            };
            asset = await asset.WithCacheConfigurationAsync(cacheConfiguration, token);

            await foreach (var dataset in asset.ListDatasetsAsync(Range.All, token))
            {
                var systemTags = await DataMapper.GetDatasetSystemTagsAsync(dataset, token);
                if (systemTags.Contains(k_SourceDatasetTag))
                {
                    await RemoveAllFiles(dataset, token);
                    break;
                }
            }
        }

        static async Task RemoveAllFiles(IDataset dataset, CancellationToken token)
        {
            if (dataset != null)
            {
                var filesToWipe = new List<Task>();
                await foreach (var file in dataset.ListFilesAsync(Range.All, token))
                {
                    filesToWipe.Add(dataset.RemoveFileAsync(file.Descriptor.Path, token));
                }

                try
                {
                    await Task.WhenAll(filesToWipe);
                }
                catch (Exception)
                {
                    var remainingFileCount = 0;
                    await foreach (var _ in dataset.ListFilesAsync(Range.All, token))
                    {
                        ++remainingFileCount;
                    }

                    if (remainingFileCount > 0)
                    {
                        Utilities.DevLogError($"Failed to remove all files from dataset. {remainingFileCount} files remain of {filesToWipe.Count}.");
                        throw new AggregateException(filesToWipe.Where(x => x.IsFaulted).Select(x => x.Exception));
                    }

                    Utilities.DevLogWarning("Exception occurred while removing files from dataset, but all files were successfully removed.");
                }
            }
        }

        public async IAsyncEnumerable<AssetDataFile> ListFilesAsync(AssetIdentifier assetIdentifier, AssetDataset assetDataset, Range range, [EnumeratorCancellation] CancellationToken token)
        {
            if (assetDataset == null)
            {
                yield break;
            }

            var cacheConfiguration = new DatasetCacheConfiguration
            {
                CacheProperties = true,
                CacheFileList = true,
                FileCacheConfiguration = new FileCacheConfiguration
                {
                    CacheProperties = true
                }
            };
            var dataset = await GetDatasetAsync(assetIdentifier, assetDataset, cacheConfiguration, token);

            if (dataset == null)
            {
                yield break;
            }

            if (!dataset.CacheConfiguration.FileCacheConfiguration.CacheProperties)
            {
                Utilities.DevLogWarning("File properties are not cached. Please ensure caching of properties for optimal AssetDataFile mapping.");
            }

            await foreach (var file in dataset.ListFilesAsync(range, token))
            {
                yield return await DataMapper.From(file, token);
            }
        }

        public async Task<AssetDataset> GetDatasetAsync(AssetData assetData, IEnumerable<string> systemTags, CancellationToken token)
        {
            var dataset = await GetDatasetAsync(assetData.Identifier, systemTags, default, token);
            return await DataMapper.From(dataset, token);
        }

        public async Task<AssetDataFile> UploadThumbnail(AssetData assetData, Texture2D thumbnail, IProgress<HttpProgress> progress, CancellationToken token)
        {
            if (assetData == null || thumbnail == null)
            {
                return null;
            }

            AssetDataFile result = null;
            byte[] bytes;
            try
            {
                bytes = thumbnail.EncodeToPNG();
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to encode thumbnail before uploading it. Error message is \"{e.Message}\"");
                throw;
            }

            using var stream = new MemoryStream(bytes);
            var dataset = await GetDatasetAsync(assetData, k_PreviewDatasetTag, default, token);
            if (dataset != null)
            {
                var file = await UploadFileToDataset(dataset, k_ThumbnailFilename, stream, progress, token);
                if (file != null)
                {
                    await GenerateAndAssignTags(file, token);

                    result = await DataMapper.From(file, token);
                }
            }

            return result;
        }

        async Task GenerateAndAssignTags(IFile file, CancellationToken token)
        {
            if (!m_SettingsManager.IsTagsCreationUploadEnabled)
                return;

            IEnumerable<GeneratedTag> generatedTags;

            try
            {
                generatedTags = await file.GenerateSuggestedTagsAsync(token);
            }
            catch (ServiceException e)
            {
                // Private Cloud services can throw 500 error for tag generation as they generally don't support the AI service that creates the tags.
                // Ideally, it should be returning an empty list.
                if (e.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Utilities.DevLog("Service does not support tag generation.");
                    return;
                }
                throw;
            }

            var tags = new List<string>();
            foreach (var tag in generatedTags)
            {
                if (tag.Confidence < m_SettingsManager.TagsConfidenceThreshold)
                {
                    continue;
                }

                tags.Add(tag.Value);
            }

            if (tags.Count > 0)
            {
                var existingTags = await DataMapper.GetFileTagsAsync(file, token) ?? Array.Empty<string>();
                var fileUpdate = new FileUpdate
                {
                    Tags = existingTags.Union(tags).ToArray()
                };
                await file.UpdateAsync(fileUpdate, token);
            }
        }

        async Task<IFile> UploadFileToDataset(IDataset dataset, string destinationPath, Stream stream, IProgress<HttpProgress> progress, CancellationToken token)
        {
            if (dataset == null || string.IsNullOrEmpty(destinationPath) || stream == null)
            {
                return null;
            }

            IFile file = null;
            var fileCreation = new FileCreation(destinationPath.Replace('\\', '/')) // Backend doesn't support backslashes AMECO-2616
            {
                // Preview transformation prevents us from freezing the asset or cause unwanted modification in the asset. Remove this line when Preview will not affect the asset anymore AMECO-2759
                DisableAutomaticTransformations = true
            };

            try
            {
                file = await dataset.UploadFileAsync(fileCreation, stream, progress, token);
            }
            catch (ServiceException e)
            {
                if (e.StatusCode != HttpStatusCode.Conflict)
                {
                    Debug.LogError($"Unable to upload file {destinationPath} to dataset {dataset.Descriptor.DatasetId}. Error code is {e.ErrorCode} with message \"{e.Message}\"");
                    throw;
                }
            }

            return file;
        }

        public async Task<IReadOnlyDictionary<string, Uri>> GetDatasetDownloadUrlsAsync(AssetIdentifier assetIdentifier, AssetDataset assetDataset, IProgress<FetchDownloadUrlsProgress> progress, CancellationToken token)
        {
            if (assetDataset == null)
            {
                throw new ArgumentNullException(nameof(assetDataset));
            }

            var cacheConfiguration = new DatasetCacheConfiguration
            {
                CacheProperties = true,
                CacheFileList = true,
                FileCacheConfiguration = new FileCacheConfiguration
                {
                    CacheDownloadUrl = true
                }
            };
            var dataset = await GetDatasetAsync(assetIdentifier, assetDataset, cacheConfiguration, token);
            return await GetDatasetDownloadUrlsAsync(dataset, progress, token);
        }

        static async Task<IReadOnlyDictionary<string, Uri>> GetDatasetDownloadUrlsAsync(IDataset dataset, IProgress<FetchDownloadUrlsProgress> progress, CancellationToken token)
        {
            var result = new Dictionary<string, Uri>();

            if (dataset == null)
            {
                return result;
            }

            var files = new List<IFile>();
            await foreach (var file in dataset.ListFilesAsync(Range.All, token))
            {
                files.Add(file);
            }

            for (var i = 0; i < files.Count; ++i)
            {
                var file = files[i];
                if (MetafilesHelper.IsOrphanMetafile(file.Descriptor.Path, files.Select(f => f.Descriptor.Path)))
                {
                    continue;
                }

                if (AssetDataDependencyHelper.IsASystemFile(file.Descriptor.Path))
                {
                    continue;
                }

                progress?.Report(new FetchDownloadUrlsProgress(Path.GetFileName(file.Descriptor.Path), (float) i / files.Count));

                var url = await file.GetDownloadUrlAsync(token);
                result[file.Descriptor.Path!] = url;
            }

            progress?.Report(new FetchDownloadUrlsProgress("Completed downloading urls", 1.0f));

            return result;
        }

        async Task<IDataset> GetDatasetAsync(BaseAssetData assetData, string datasetSystemTag, DatasetCacheConfiguration cacheConfiguration, CancellationToken token)
        {
            if (assetData == null)
            {
                return null;
            }

            var assetDataset = assetData.Datasets.FirstOrDefault(d => d.SystemTags.Contains(datasetSystemTag));

            if (assetDataset != null)
            {
                return await GetDatasetAsync(assetData.Identifier, assetDataset, cacheConfiguration, token);
            }

            return await GetDatasetAsync(assetData.Identifier, new HashSet<string> { datasetSystemTag }, cacheConfiguration, token);
        }

        async Task<IDataset> GetDatasetAsync(AssetIdentifier assetIdentifier, AssetDataset assetDataset, DatasetCacheConfiguration cacheConfiguration, CancellationToken token)
        {
            if (assetDataset == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(assetDataset.Id))
            {
                return await GetDatasetAsync(assetIdentifier, assetDataset.SystemTags, cacheConfiguration, token);
            }

            var assetDescriptor = Map(assetIdentifier);
            var datasetDescriptor = new DatasetDescriptor(assetDescriptor, new DatasetId(assetDataset.Id));

            var dataset = await AssetRepository.GetDatasetAsync(datasetDescriptor, token);

            try
            {
                return await dataset.WithCacheConfigurationAsync(cacheConfiguration, token);
            }
            catch (NotFoundException e)
            {
                Utilities.DevLogError($"Asset {assetDescriptor.AssetId} not found; dataset {datasetDescriptor.DatasetId} could not be returned.\n{e.Message}");
                return null;
            }
        }

        async Task<IDataset> GetDatasetAsync(AssetIdentifier assetIdentifier, IEnumerable<string> systemTags, DatasetCacheConfiguration cacheConfiguration, CancellationToken token)
        {
            var asset = await InternalGetAssetAsync(assetIdentifier, token);
            if (asset == null)
            {
                return null;
            }

            var assetCacheConfiguration = new AssetCacheConfiguration
            {
                DatasetCacheConfiguration = cacheConfiguration
            };
            asset = await asset.WithCacheConfigurationAsync(assetCacheConfiguration, token);

            try
            {
                await foreach (var dataset in asset.ListDatasetsAsync(Range.All, token))
                {
                    var datasetSystemTags = await DataMapper.GetDatasetSystemTagsAsync(dataset, token);
                    if (datasetSystemTags.Any(t => systemTags.Any(tag => tag == t)))
                    {
                        return dataset;
                    }
                }
            }
            catch (NotFoundException)
            {
                // Ignore, asset could not be found on cloud.
            }

            return null;
        }

        public string GetValueAsString(AssetType assetType) => Map(assetType).GetValueAsString();

        public bool TryParse(string assetTypeString, out AssetType assetType)
        {
            if (assetTypeString.TryGetAssetTypeFromString(out var sdkAssetType))
            {
                assetType = Map(sdkAssetType);
                return true;
            }

            assetType = AssetType.Other;
            return false;
        }

        [Obsolete("IAsset serialization is not supported")]
        public AssetData DeserializeAssetData(string content)
        {
            var fullName = typeof(IAsset).FullName;

            // If the Cloud SDKs have been enbedded, we need to modify the namespaces
            if (!string.IsNullOrEmpty(fullName) && fullName != "Unity.Cloud.Assets.IAsset")
            {
                var updatedNamespace = fullName.Replace("IAsset", string.Empty);
                content = content.Replace("Unity.Cloud.Assets.", updatedNamespace);
            }

            var asset = AssetRepository.DeserializeAsset(content);
            return asset == null
                ? null
                : new AssetData(
                    Map(asset.Descriptor),
                    asset.FrozenSequenceNumber,
                    asset.ParentFrozenSequenceNumber,
                    asset.Changelog,
                    asset.Name,
                    Map(asset.Type),
                    asset.StatusName,
                    asset.Description,
                    asset.AuthoringInfo.Created,
                    asset.AuthoringInfo.Updated,
                    asset.AuthoringInfo.CreatedBy.ToString(),
                    asset.AuthoringInfo.UpdatedBy.ToString(),
                    Map(asset.PreviewFileDescriptor),
                    asset.State == AssetState.Frozen,
                    asset.Tags,
                    Map(asset.Labels),
                    Map(asset.LinkedProjects));
        }

        AssetCacheConfiguration GetAssetCacheConfigurationForMapping()
        {
            return new AssetCacheConfiguration
            {
                CacheProperties = true,
                CacheMetadata = true,
                CacheDatasetList = true,
                DatasetCacheConfiguration = new DatasetCacheConfiguration
                {
                    CacheProperties = true
                }
            };
        }

        static string OptimizeVersionForSearch(string version)
        {
            // Because of how elastic search tokenizes strings, we need to manipulate the version to minimize false positive results
            // We will therefore keep only that last component of the version string
            return version.Split('-')[^1];
        }

        static void ParseFileExtensions(List<string> fileExtensions, Cloud.AssetsEmbedded.AssetSearchFilter cloudAssetSearchFilter)
        {
            if (fileExtensions == null || !fileExtensions.Any())
            {
                return;
            }

            var pattern = new StringBuilder(fileExtensions[0]);
            for (var i = 1; i < fileExtensions.Count; ++i)
            {
                pattern.Append($"|{fileExtensions[i]}");
            }

            cloudAssetSearchFilter.Include().Files.Path.WithValue(new Regex($".*({pattern})", RegexOptions.IgnoreCase));
        }

        static bool TryParseSearchTerms(List<string> searchTerms, Cloud.AssetsEmbedded.AssetSearchFilter cloudAssetSearchFilter)
        {
            if (searchTerms == null || !searchTerms.Any())
            {
                return false;
            }

            // Search Name and Description by predicate, any term in the list
            var stringPredicate = new StringPredicate(searchTerms[0], StringSearchOption.Wildcard);
            for (var i = 1; i < searchTerms.Count; ++i)
            {
                stringPredicate = stringPredicate.Or(searchTerms[i], StringSearchOption.Wildcard);
            }

            cloudAssetSearchFilter.Any().Name.WithValue(stringPredicate);
            cloudAssetSearchFilter.Any().Description.WithValue(stringPredicate);

            // Search Tags by list
            cloudAssetSearchFilter.Any().Tags.WithValue(searchTerms);

            return true;
        }

        async Task<AssetData> Map(AssetDescriptor assetDescriptor, CancellationToken token)
        {
            try
            {
                var asset = await AssetRepository.GetAssetAsync(assetDescriptor, token);
                asset = await asset.WithCacheConfigurationAsync(GetAssetCacheConfigurationForMapping(), token);
                return await DataMapper.From(asset, token);
            }
            catch (NotFoundException)
            {
                // Ignore, asset could not be found on cloud.
                return null;
            }
        }

        static IEnumerable<AssetLabel> Map(IEnumerable<LabelDescriptor> labelDescriptors)
        {
            return labelDescriptors.Select(labelDescriptor => new AssetLabel(labelDescriptor.LabelName, labelDescriptor.OrganizationId.ToString()));
        }

        static IEnumerable<ProjectIdentifier> Map(IEnumerable<ProjectDescriptor> projectDescriptors)
        {
            return projectDescriptors.Select(projectDescriptor => new ProjectIdentifier(projectDescriptor.OrganizationId.ToString(), projectDescriptor.ProjectId.ToString()));
        }

        internal static AssetType Map(Unity.Cloud.AssetsEmbedded.AssetType assetType)
        {
            return assetType switch
            {
                Cloud.AssetsEmbedded.AssetType.Animation => AssetType.Animation,
                Cloud.AssetsEmbedded.AssetType.Audio => AssetType.Audio,
                Cloud.AssetsEmbedded.AssetType.Audio_Mixer => AssetType.AudioMixer,
                Cloud.AssetsEmbedded.AssetType.Font => AssetType.Font,
                Cloud.AssetsEmbedded.AssetType.Material => AssetType.Material,
                Cloud.AssetsEmbedded.AssetType.Model_3D => AssetType.Model3D,
                Cloud.AssetsEmbedded.AssetType.Physics_Material => AssetType.PhysicsMaterial,
                Cloud.AssetsEmbedded.AssetType.Prefab => AssetType.Prefab,
                Cloud.AssetsEmbedded.AssetType.Unity_Scene => AssetType.UnityScene,
                Cloud.AssetsEmbedded.AssetType.Script => AssetType.Script,
                Cloud.AssetsEmbedded.AssetType.Shader => AssetType.Shader,
                Cloud.AssetsEmbedded.AssetType.Asset_2D => AssetType.Asset2D,
                Cloud.AssetsEmbedded.AssetType.Visual_Effect => AssetType.VisualEffect,
                Cloud.AssetsEmbedded.AssetType.Assembly_Definition => AssetType.AssemblyDefinition,
                Cloud.AssetsEmbedded.AssetType.Asset => AssetType.Asset,
                Cloud.AssetsEmbedded.AssetType.Configuration => AssetType.Configuration,
                Cloud.AssetsEmbedded.AssetType.Document => AssetType.Document,
                Cloud.AssetsEmbedded.AssetType.Environment => AssetType.Environment,
                Cloud.AssetsEmbedded.AssetType.Image => AssetType.Image,
                Cloud.AssetsEmbedded.AssetType.Playable => AssetType.Playable,
                Cloud.AssetsEmbedded.AssetType.Shader_Graph => AssetType.ShaderGraph,
                Cloud.AssetsEmbedded.AssetType.Unity_Package => AssetType.UnityPackage,
                Cloud.AssetsEmbedded.AssetType.Scene => AssetType.Scene,
                Cloud.AssetsEmbedded.AssetType.Unity_Editor => AssetType.UnityEditor,
                Cloud.AssetsEmbedded.AssetType.Video => AssetType.Video,
                _ => AssetType.Other
            };
        }

        static AssetIdentifier Map(AssetDescriptor descriptor)
        {
            return new AssetIdentifier(descriptor.OrganizationId.ToString(),
                descriptor.ProjectId.ToString(),
                descriptor.AssetId.ToString(),
                descriptor.AssetVersion.ToString());
        }

        static AssetDescriptor Map(AssetIdentifier assetIdentifier)
        {
            if (assetIdentifier == null)
            {
                throw new ArgumentNullException(nameof(assetIdentifier));
            }
            return new AssetDescriptor(
                new ProjectDescriptor(
                    new OrganizationId(assetIdentifier.OrganizationId),
                    new ProjectId(assetIdentifier.ProjectId)),
                new AssetId(assetIdentifier.AssetId),
                new AssetVersion(assetIdentifier.Version));
        }

        static ProjectDescriptor Map(ProjectIdentifier projectIdentifier)
        {
            if (projectIdentifier == null)
            {
                throw new ArgumentNullException(nameof(projectIdentifier));
            }

            return new ProjectDescriptor(
                new OrganizationId(projectIdentifier.OrganizationId),
                new ProjectId(projectIdentifier.ProjectId));
        }

        static string Map(FileDescriptor? fileDescriptor)
        {
            return string.IsNullOrEmpty(fileDescriptor?.Path) ? "/" : $"{fileDescriptor.Value.DatasetId}/{fileDescriptor.Value.Path}";
        }

        static IAssetSearchFilter Map(AssetSearchFilter assetSearchFilter)
        {
            var cloudAssetSearchFilter = new Cloud.AssetsEmbedded.AssetSearchFilter();
            var minimumAnyRequirement = 0;

            if (assetSearchFilter.CreatedBy != null && assetSearchFilter.CreatedBy.Any())
            {
                cloudAssetSearchFilter.Include().AuthoringInfo.CreatedBy.WithValue(string.Join(" ", assetSearchFilter.CreatedBy));
            }

            if (assetSearchFilter.UpdatedBy != null && assetSearchFilter.UpdatedBy.Any())
            {
                cloudAssetSearchFilter.Include().AuthoringInfo.UpdatedBy.WithValue(string.Join(" ", assetSearchFilter.UpdatedBy));
            }

            if (assetSearchFilter.Status != null && assetSearchFilter.Status.Any())
            {
                cloudAssetSearchFilter.Include().Status.WithValue(string.Join(" ", assetSearchFilter.Status));
            }

            if (assetSearchFilter.AssetTypes != null && assetSearchFilter.AssetTypes.Any())
            {
                var assetTypes = assetSearchFilter.AssetTypes
                    .Select(Map)
                    .ToArray();

                if (assetTypes.Length > 0)
                {
                    cloudAssetSearchFilter.Include().Type.WithValue(assetTypes);
                }
            }
            else if (assetSearchFilter.AssetTypeStrings != null && assetSearchFilter.AssetTypeStrings.Any())
            {
                var assetTypes = new List<Cloud.AssetsEmbedded.AssetType>();
                foreach (var typeString in assetSearchFilter.AssetTypeStrings)
                {
                    if (typeString.TryGetAssetTypeFromString(out var assetType))
                    {
                        assetTypes.Add(assetType);
                    }
                }

                if (assetTypes.Count > 0)
                {
                    cloudAssetSearchFilter.Include().Type.WithValue(assetTypes.ToArray());
                }
            }

            if (assetSearchFilter.Tags != null && assetSearchFilter.Tags.Any())
            {
                cloudAssetSearchFilter.Include().Tags.WithValue(string.Join(" ", assetSearchFilter.Tags));
            }

            if (assetSearchFilter.Labels != null && assetSearchFilter.Labels.Any())
            {
                cloudAssetSearchFilter.Include().Labels.WithValue(assetSearchFilter.Labels);
            }

            if (assetSearchFilter.CustomMetadata != null)
            {
                foreach (var metadataGroup in assetSearchFilter.CustomMetadata.GroupBy(m => m.FieldKey))
                {
                    var metadataList = metadataGroup.ToList();
                    if (!metadataList.Any())
                        continue;

                    if (metadataList[0].Type == MetadataFieldType.Timestamp)
                    {
                        var minValue = metadataList.Min(m => ((TimestampMetadata) m).Value.DateTime);
                        var maxValue = metadataList.Max(m => ((TimestampMetadata) m).Value.DateTime);
                        cloudAssetSearchFilter.Include().Metadata.WithTimestampValue(metadataGroup.Key, minValue, true, maxValue);
                    }
                    else
                    {
                        var metadataValue = metadataList.Select(Map).FirstOrDefault(x => x != null);
                        if (metadataValue == null)
                            continue;

                        // Strings should be searched by predicate
                        if (metadataValue is Cloud.AssetsEmbedded.StringMetadata stringMetadata)
                        {
                            var stringPredicate = new StringPredicate(stringMetadata.Value, assetSearchFilter.IsExactMatchSearch
                                ? StringSearchOption.ExactMatch
                                : StringSearchOption.Prefix);
                            cloudAssetSearchFilter.Include().Metadata.WithTextValue(metadataGroup.Key, stringPredicate);
                        }

                        // Urls should be searched by label or, when no label defined, by the URL itself
                        else if (metadataValue is Cloud.AssetsEmbedded.UrlMetadata urlMetadata)
                        {
                            if (!string.IsNullOrEmpty(urlMetadata.Label))
                            {
                                var stringPredicate = new StringPredicate($"[{urlMetadata.Label}]", StringSearchOption.Prefix);
                                cloudAssetSearchFilter.Include().Metadata.WithTextValue(metadataGroup.Key, stringPredicate);
                            }
                            else if (urlMetadata.Uri != null)
                            {
                                cloudAssetSearchFilter.Include().Metadata.WithValue(metadataGroup.Key, urlMetadata);
                            }
                        }

                        // Multiselection values need to be searched by Any() to perform OR logical search between values
                        else if (metadataValue is Cloud.AssetsEmbedded.MultiSelectionMetadata multiSelectionMetadata)
                        {
                            cloudAssetSearchFilter.Any().Metadata.WithValue(metadataGroup.Key, multiSelectionMetadata);
                            ++minimumAnyRequirement;
                        }

                        // All other metadata are by exact match.
                        else
                        {
                            cloudAssetSearchFilter.Include().Metadata.WithValue(metadataGroup.Key, metadataValue);
                        }
                    }
                }
            }

            if (assetSearchFilter.Collection != null)
            {
                var collectionPaths = assetSearchFilter.Collection
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => new CollectionPath(x));
                cloudAssetSearchFilter.Collections.WhereContains(collectionPaths);
            }

            if (assetSearchFilter.Searches is {Count: > 0})
            {
                var fileExtensions = assetSearchFilter.Searches.Where(x => x.StartsWith('.')).ToList();
                ParseFileExtensions(fileExtensions, cloudAssetSearchFilter);

                var searches = assetSearchFilter.Searches.Where(x => !fileExtensions.Contains(x)).ToList();
                if (TryParseSearchTerms(searches, cloudAssetSearchFilter))
                {
                    ++minimumAnyRequirement; // We need to search to match in at least one field
                }
            }

            if (assetSearchFilter.AssetIds is {Count: > 0})
            {
                var searchString = string.Join(' ', assetSearchFilter.AssetIds);
                cloudAssetSearchFilter.Include().Id.WithValue(searchString);
            }

            if (assetSearchFilter.AssetVersions is {Count: > 0})
            {
                var searchString = string.Join(' ', assetSearchFilter.AssetVersions.Select(OptimizeVersionForSearch));
                cloudAssetSearchFilter.Any().Version.WithValue(searchString);
                cloudAssetSearchFilter.Any().Labels.WithValue("*");
                ++minimumAnyRequirement;
            }

            cloudAssetSearchFilter.Any().WhereMinimumMatchEquals(Math.Max(1, minimumAnyRequirement));

            return cloudAssetSearchFilter;
        }

        static GroupableField Map(AssetSearchGroupBy groupBy)
        {
            return groupBy switch
            {
                AssetSearchGroupBy.Name => GroupableField.Name,
                AssetSearchGroupBy.Status => GroupableField.Status,
                AssetSearchGroupBy.CreatedBy => GroupableField.CreatedBy,
                AssetSearchGroupBy.UpdatedBy => GroupableField.UpdateBy,
                AssetSearchGroupBy.Type => GroupableField.Type,
                _ => throw new ArgumentOutOfRangeException(nameof(groupBy), groupBy, null)
            };
        }

        static Unity.Cloud.AssetsEmbedded.AssetCreation Map(AssetCreation assetCreation)
        {
            if (assetCreation == null)
            {
                throw new ArgumentNullException(nameof(assetCreation));
            }

            return new Unity.Cloud.AssetsEmbedded.AssetCreation(assetCreation.Name)
            {
                Type = Map(assetCreation.Type),
                Collections = assetCreation.Collections?.Select(x => new CollectionPath(x)).ToList(),
                Tags = assetCreation.Tags,
                Metadata = Map(assetCreation.Metadata)
            };
        }

        static Dictionary<string, MetadataValue> Map(List<IMetadata> metadataList)
        {
            var metadataDictionary = new Dictionary<string, MetadataValue>();
            foreach (var metadata in metadataList)
            {
                metadataDictionary.Add(metadata.FieldKey, Map(metadata));
            }

            return metadataDictionary;
        }

        static MetadataValue Map(IMetadata metadata) => metadata.Type switch
        {
            MetadataFieldType.Boolean => new Cloud.AssetsEmbedded.BooleanMetadata(((BooleanMetadata) metadata).Value),
            MetadataFieldType.Text => new Cloud.AssetsEmbedded.StringMetadata(((TextMetadata) metadata).Value),
            MetadataFieldType.Number => new Cloud.AssetsEmbedded.NumberMetadata(((NumberMetadata) metadata).Value),
            MetadataFieldType.Url => new Cloud.AssetsEmbedded.UrlMetadata(((UrlMetadata) metadata).Value.Uri, ((UrlMetadata) metadata).Value.Label),
            MetadataFieldType.Timestamp => new Cloud.AssetsEmbedded.DateTimeMetadata(((TimestampMetadata) metadata).Value.DateTime),
            MetadataFieldType.User => new Cloud.AssetsEmbedded.UserMetadata(new UserId(((UserMetadata) metadata).Value)),
            MetadataFieldType.SingleSelection => new Cloud.AssetsEmbedded.SingleSelectionMetadata(((SingleSelectionMetadata) metadata).Value),
            MetadataFieldType.MultiSelection => new Cloud.AssetsEmbedded.MultiSelectionMetadata(((MultiSelectionMetadata) metadata).Value?.ToArray()),
            _ => throw new InvalidOperationException("Unexpected metadata field type was encountered.")
        };

        internal static IAssetUpdate Map(AssetUpdate assetUpdate)
        {
            if (assetUpdate == null)
            {
                throw new ArgumentNullException(nameof(assetUpdate));
            }

            return new Unity.Cloud.AssetsEmbedded.AssetUpdate
            {
                Name = assetUpdate.Name,
                Type = assetUpdate.Type.HasValue ? Map(assetUpdate.Type.Value) : null,
                PreviewFile = assetUpdate.PreviewFile,
                Tags = assetUpdate.Tags
            };
        }

        internal static Cloud.AssetsEmbedded.AssetType Map(AssetType assetType)
        {
            return assetType switch
            {
                AssetType.Animation => Cloud.AssetsEmbedded.AssetType.Animation,
                AssetType.Audio => Cloud.AssetsEmbedded.AssetType.Audio,
                AssetType.AudioMixer => Cloud.AssetsEmbedded.AssetType.Audio_Mixer,
                AssetType.Font => Cloud.AssetsEmbedded.AssetType.Font,
                AssetType.Material => Cloud.AssetsEmbedded.AssetType.Material,
                AssetType.Model3D => Cloud.AssetsEmbedded.AssetType.Model_3D,
                AssetType.PhysicsMaterial => Cloud.AssetsEmbedded.AssetType.Physics_Material,
                AssetType.Prefab => Cloud.AssetsEmbedded.AssetType.Prefab,
                AssetType.UnityScene => Cloud.AssetsEmbedded.AssetType.Unity_Scene,
                AssetType.Script => Cloud.AssetsEmbedded.AssetType.Script,
                AssetType.Shader => Cloud.AssetsEmbedded.AssetType.Shader,
                AssetType.Asset2D => Cloud.AssetsEmbedded.AssetType.Asset_2D,
                AssetType.VisualEffect => Cloud.AssetsEmbedded.AssetType.Visual_Effect,
                AssetType.AssemblyDefinition => Cloud.AssetsEmbedded.AssetType.Assembly_Definition,
                AssetType.Asset => Cloud.AssetsEmbedded.AssetType.Asset,
                AssetType.Configuration => Cloud.AssetsEmbedded.AssetType.Configuration,
                AssetType.Document => Cloud.AssetsEmbedded.AssetType.Document,
                AssetType.Environment => Cloud.AssetsEmbedded.AssetType.Environment,
                AssetType.Image => Cloud.AssetsEmbedded.AssetType.Image,
                AssetType.Playable => Cloud.AssetsEmbedded.AssetType.Playable,
                AssetType.ShaderGraph => Cloud.AssetsEmbedded.AssetType.Shader_Graph,
                AssetType.UnityPackage => Cloud.AssetsEmbedded.AssetType.Unity_Package,
                AssetType.Scene => Cloud.AssetsEmbedded.AssetType.Scene,
                AssetType.UnityEditor => Cloud.AssetsEmbedded.AssetType.Unity_Editor,
                AssetType.Video => Cloud.AssetsEmbedded.AssetType.Video,
                _ => Cloud.AssetsEmbedded.AssetType.Other
            };
        }

        static Cloud.AssetsEmbedded.SortingOrder Map(SortingOrder sortingOrder)
        {
            return sortingOrder switch
            {
                SortingOrder.Ascending => Cloud.AssetsEmbedded.SortingOrder.Ascending,
                SortingOrder.Descending => Cloud.AssetsEmbedded.SortingOrder.Descending,
                _ => throw new ArgumentOutOfRangeException(nameof(sortingOrder), sortingOrder, null)
            };
        }

        /// <summary>
        /// A wrapper for certain SDK functions and value types that are not easily mockable.
        /// </summary>
        internal interface IDataMapper
        {
            [ExcludeFromCoverage]
            async Task WaitForDatasetCommitAsync(IDataset dataset, CancellationToken token)
            {
                const int waitTime = 100;
                var timeout = 10000;

                var properties = await dataset.GetPropertiesAsync(token);
                while (timeout > 0 && properties.StatusName != "Committed")
                {
                    await Task.Delay(waitTime, token);

                    timeout -= waitTime;

                    properties = await dataset.GetPropertiesAsync(token);
                }

                if (properties.StatusName != "Committed")
                {
                    Utilities.DevLogError($"Dataset {dataset.Descriptor.DatasetId} did not commit within the timeout period.");
                }
            }

            [ExcludeFromCoverage]
            async Task<string> GetPreviewFilePath(IAsset asset, CancellationToken token)
            {
                if (asset == null)
                {
                    return null;
                }

                var properties = await asset.GetPropertiesAsync(token);
                return Map(properties.PreviewFileDescriptor);
            }

            [ExcludeFromCoverage]
            async Task<IEnumerable<string>> GetDatasetSystemTagsAsync(IDataset dataset, CancellationToken token)
            {
                if (dataset == null)
                {
                    return Array.Empty<string>();
                }

                var datasetProperties = await dataset.GetPropertiesAsync(token);
                var systemTags = datasetProperties.SystemTags?.ToList() ?? new List<string>();
                if (!string.IsNullOrEmpty(datasetProperties.WorkflowName))
                {
                    systemTags.Add(datasetProperties.WorkflowName);
                }

                return systemTags;
            }

            [ExcludeFromCoverage]
            async Task<IEnumerable<string>> GetFileTagsAsync(IFile file, CancellationToken token)
            {
                if (file == null)
                {
                    return Array.Empty<string>();
                }

                var fileProperties = await file.GetPropertiesAsync(token);
                return fileProperties.Tags ?? Array.Empty<string>();
            }

            [ExcludeFromCoverage]
            IAsyncEnumerable<IAsset> ListAssetsAsync(AssetQueryBuilder query, CancellationToken token)
            {
                return query.ExecuteAsync(token);
            }

            [ExcludeFromCoverage]
            IAsyncEnumerable<IAsset> ListAssetsAsync(VersionQueryBuilder query, CancellationToken token)
            {
                return query.ExecuteAsync(token);
            }

            [ExcludeFromCoverage]
            IAsyncEnumerable<IAssetReference> ListAssetReferencessAsync(AssetReferenceQueryBuilder query, CancellationToken token)
            {
                return query.ExecuteAsync(token);
            }

            [ExcludeFromCoverage]
            async IAsyncEnumerable<KeyValuePair<string, int>> GroupAndCountAsync(GroupAndCountAssetsQueryBuilder query, Groupable groupable, [EnumeratorCancellation] CancellationToken token)
            {
                await foreach(var kvp in query.ExecuteAsync(groupable, token))
                {
                    yield return new KeyValuePair<string, int>(kvp.Key.AsString(), kvp.Value);
                }
            }

            [ExcludeFromCoverage]
            async Task<IEnumerable<ProjectIdentifier>> GetLinkedProjectsAsync(IAsset asset, CancellationToken token)
            {
                if (asset == null)
                {
                    return Array.Empty<ProjectIdentifier>();
                }

                var properties = await asset.GetPropertiesAsync(token);
                return Map(properties.LinkedProjects);
            }

            [ExcludeFromCoverage]
            async Task<AssetData> From(IAsset asset, CancellationToken token)
            {
                if (asset == null)
                {
                    return null;
                }

                if (!asset.CacheConfiguration.CacheProperties)
                {
                    Utilities.DevLogWarning("Asset properties are not cached. Please ensure caching of properties for optimal AssetData mapping.");
                }

                var properties = await asset.GetPropertiesAsync(token);
                var data = From(asset.Descriptor, properties);

                if (data == null)
                {
                    return null;
                }

                if (asset.CacheConfiguration.CacheMetadata)
                {
                    var metadata = await GetMetadataAsync(asset.Metadata as IReadOnlyMetadataContainer, token);
                    data.SetMetadata(metadata);
                }

                if (asset.CacheConfiguration.CacheDatasetList)
                {
                    var datasets = new List<AssetDataset>();
                    await foreach (var dataset in asset.ListDatasetsAsync(Range.All, token))
                    {
                        var assetDataset = await From(dataset, token);
                        datasets.Add(assetDataset);
                    }

                    data.Datasets = datasets;
                }

                return data;
            }

            [ExcludeFromCoverage]
            static AssetData From(AssetDescriptor descriptor, AssetProperties properties)
            {
                return new AssetData(
                    Map(descriptor),
                    properties.FrozenSequenceNumber,
                    properties.ParentFrozenSequenceNumber,
                    properties.Changelog,
                    properties.Name,
                    Map(properties.Type),
                    properties.StatusName,
                    properties.Description,
                    properties.AuthoringInfo.Created,
                    properties.AuthoringInfo.Updated,
                    properties.AuthoringInfo.CreatedBy.ToString(),
                    properties.AuthoringInfo.UpdatedBy.ToString(),
                    Map(properties.PreviewFileDescriptor),
                    properties.State == AssetState.Frozen,
                    properties.Tags,
                    Map(properties.Labels),
                    Map(properties.LinkedProjects));
            }

            [ExcludeFromCoverage]
            async Task<AssetDataset> From(IDataset dataset, CancellationToken token)
            {
                if (dataset == null)
                {
                    return null;
                }

                if (!dataset.CacheConfiguration.CacheProperties)
                {
                    Utilities.DevLogWarning("Dataset properties are not cached. Please ensure caching of properties for optimal AssetDataset mapping.");
                }

                var properties = await dataset.GetPropertiesAsync(token);

                var systemTags = properties.SystemTags?.ToList() ?? new List<string>();
                if (!string.IsNullOrEmpty(properties.WorkflowName))
                {
                    systemTags.Add(properties.WorkflowName);
                }

                return new AssetDataset(
                    dataset.Descriptor.DatasetId.ToString(),
                    properties.Name,
                    systemTags);
            }

            [ExcludeFromCoverage]
            async Task<AssetDataFile> From(IFile file, CancellationToken token)
            {
                if (file == null)
                {
                    return null;
                }

                var properties = await file.GetPropertiesAsync(token);
                return From(file.Descriptor.Path, properties);
            }

            [ExcludeFromCoverage]
            static AssetDataFile From(string filePath, FileProperties fileProperties)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return null;
                }

                var available = string.IsNullOrEmpty(fileProperties.StatusName) ||
                    fileProperties.StatusName.Equals("Uploaded", StringComparison.OrdinalIgnoreCase);

                return new AssetDataFile(
                    filePath,
                    Path.GetExtension(filePath).ToLower(),
                    null,
                    fileProperties.Description,
                    fileProperties.Tags,
                    fileProperties.SizeBytes,
                    available);
            }

            [ExcludeFromCoverage]
            async IAsyncEnumerable<string> ListMetadataKeysAsync(IReadOnlyMetadataContainer metadataContainer, [EnumeratorCancellation] CancellationToken token)
            {
                await foreach (var result in metadataContainer.Query().ExecuteAsync(token))
                {
                    yield return result.Key;
                }
            }

            [ExcludeFromCoverage]
            async Task<List<IMetadata>> GetMetadataAsync(IReadOnlyMetadataContainer metadataContainer, CancellationToken token = default)
            {
                if (metadataContainer == null)
                {
                    return null;
                }

                var projectOrganizationProvider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();
                var fieldDefinitions = projectOrganizationProvider.SelectedOrganization.MetadataFieldDefinitions;

                var metadataFields = new List<IMetadata>();
                await foreach (var result in metadataContainer.Query().ExecuteAsync(token))
                {
                    var def = fieldDefinitions.Find(x => x.Key == result.Key);

                    if (def == null)
                        continue;

                    IMetadata value = def.Type switch
                    {
                        MetadataFieldType.Text => new TextMetadata(def.Key, def.DisplayName, result.Value.AsText().Value),
                        MetadataFieldType.Boolean => new BooleanMetadata(def.Key, def.DisplayName, result.Value.AsBoolean().Value),
                        MetadataFieldType.Number => new NumberMetadata(def.Key, def.DisplayName, result.Value.AsNumber().Value),
                        MetadataFieldType.Url => new UrlMetadata(def.Key, def.DisplayName, new UriEntry(result.Value.AsUrl().Uri, result.Value.AsUrl().Label)),
                        MetadataFieldType.Timestamp => new TimestampMetadata(def.Key, def.DisplayName, new DateTimeEntry(result.Value.AsTimestamp().Value)),
                        MetadataFieldType.User => new UserMetadata(def.Key, def.DisplayName, result.Value.AsUser().UserId.ToString()),
                        MetadataFieldType.SingleSelection => new SingleSelectionMetadata(def.Key, def.DisplayName, result.Value.AsSingleSelection().SelectedValue),
                        MetadataFieldType.MultiSelection => new MultiSelectionMetadata(def.Key, def.DisplayName, result.Value.AsMultiSelection().SelectedValues),
                        _ => null
                    };

                    if (value != null)
                        metadataFields.Add(value);
                }

                return metadataFields;
            }
        }
    }
}
