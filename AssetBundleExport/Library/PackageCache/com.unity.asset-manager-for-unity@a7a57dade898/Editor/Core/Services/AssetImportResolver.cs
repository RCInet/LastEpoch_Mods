using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IAssetImportDecisionMaker
    {
        Task<IEnumerable<ResolutionData>> ResolveConflicts(UpdatedAssetData data, ImportSettingsInternal importSettings);
    }

    interface IAssetImportResolver : IService
    {
        Task<IEnumerable<BaseAssetData>> Resolve(IEnumerable<BaseAssetData> assets, ImportSettingsInternal importSettings, CancellationToken token);

        void SetConflictResolver(IAssetImportDecisionMaker conflictResolver);
    }

    enum ResolutionSelection
    {
        Replace,
        Ignore
    }

    class ResolutionData
    {
        public BaseAssetData AssetData;
        public ResolutionSelection ResolutionSelection;
    }

    [Serializable]
    class AssetImportResolver : BaseService<IAssetImportResolver>, IAssetImportResolver
    {
        [Serializable]
        readonly struct DependencyNode
        {
            // Any node with a null m_AssetData should be trashed on Domain Reload.
            readonly BaseAssetData m_AssetData;

            // Not started => -1
            // Started => 0
            // Completed => 1
            readonly int m_DependenciesTraversalState;

            public BaseAssetData AssetData => m_AssetData;
            public bool IsDependencyTraversalStarted => m_DependenciesTraversalState >= 0;
            public bool IsDependencyTraversalCompleted => m_DependenciesTraversalState == 1;

            public DependencyNode(BaseAssetData assetData, int dependenciesTraversalState = -1)
            {
                m_AssetData = assetData;
                m_DependenciesTraversalState = dependenciesTraversalState;
            }
        }

        [SerializeReference]
        IAssetDataManager m_AssetDataManager;

        [SerializeReference]
        IAssetsProvider m_AssetsProvider;

        [SerializeReference]
        IIOProxy m_IOProxy;

        [SerializeReference]
        ISettingsManager m_SettingsManager;

        [SerializeReference]
        IAssetImportDecisionMaker m_ConflictResolver;

        [ServiceInjection]
        public void Inject(IAssetDataManager assetDataManager, IAssetsProvider assetsProvider, IIOProxy ioProxy, ISettingsManager settingsManager)
        {
            m_AssetDataManager = assetDataManager;
            m_AssetsProvider = assetsProvider;
            m_IOProxy = ioProxy;
            m_SettingsManager = settingsManager;
        }

        protected override void ValidateServiceDependencies()
        {
            base.ValidateServiceDependencies();

            m_AssetDataManager ??= ServicesContainer.instance.Get<IAssetDataManager>();
            m_AssetsProvider ??= ServicesContainer.instance.Get<IAssetsProvider>();
            m_IOProxy ??= ServicesContainer.instance.Get<IIOProxy>();
            m_SettingsManager ??= ServicesContainer.instance.Get<ISettingsManager>();
        }

        public void SetConflictResolver(IAssetImportDecisionMaker conflictResolver)
        {
            m_ConflictResolver = conflictResolver;
        }

        public async Task<IEnumerable<BaseAssetData>> Resolve(IEnumerable<BaseAssetData> assets, ImportSettingsInternal importSettings, CancellationToken token)
        {
            try
            {
                if (assets == null || !assets.Any())
                {
                    return null;
                }

                var assetsAndDependencies = await GetUpdatedAssetDataAndDependenciesAsync(assets, importSettings.ImportType, token);

                if (IsDebugLogsEnabled())
                {
                    UnityEngine.Debug.Log(
                        $"The assets and dependencies to consider for import are:\n{string.Join("\n", assetsAndDependencies.Select(x => x.Identifier.ToString()))}");
                }

                if (!CheckIfAssetsAlreadyInProject(assets, importSettings.ImportPath, assetsAndDependencies, out var updatedAssetData))
                {
                    return assetsAndDependencies;
                }

                // Check if the assets have changes
                await CheckUpdatedAssetDataAsync(updatedAssetData, token);

                Utilities.DevAssert(m_ConflictResolver != null);

                if (m_ConflictResolver == null)
                {
                    // In case there is no decision maker, we will just replace and reimport all the assets
                    return updatedAssetData.Assets.Select(c => c.AssetData);
                }

                var resolutions = await m_ConflictResolver.ResolveConflicts(updatedAssetData, importSettings);
                return resolutions?.Where(c => c.ResolutionSelection == ResolutionSelection.Replace)
                    .Select(c => c.AssetData);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        bool CheckIfAssetsAlreadyInProject(IEnumerable<BaseAssetData> assets, string importDestination,
            HashSet<BaseAssetData> assetsAndDependencies, out UpdatedAssetData updatedAssetData)
        {
            updatedAssetData = new UpdatedAssetData();

            var isFoundAtLeastOne = false;
            foreach (var asset in assetsAndDependencies)
            {
                var existingFiles = asset.GetFiles().Where(f => Exists(importDestination, asset, f));
                var resolutionInfo = new AssetDataResolutionInfo(asset, existingFiles, m_AssetDataManager);

                isFoundAtLeastOne |= resolutionInfo.HasConflicts;

                if (assets.Any(a => TrackedAssetIdentifier.IsFromSameAsset(a.Identifier, asset.Identifier)))
                {
                    updatedAssetData.Assets.Add(resolutionInfo);
                }
                else
                {
                    updatedAssetData.Dependants.Add(resolutionInfo);
                }

                isFoundAtLeastOne |= resolutionInfo.Existed;
            }

            return isFoundAtLeastOne;
        }

        bool Exists(string destinationPath, BaseAssetData assetData, BaseAssetDataFile file)
        {
            if (m_SettingsManager.IsSubfolderCreationEnabled)
            {
                var regex = new Regex(@"[\\\/:*?""<>|]", RegexOptions.None, TimeSpan.FromMilliseconds(100));
                var sanitizedAssetName = regex.Replace(assetData.Name, "");
                destinationPath = Path.Combine(destinationPath, $"{sanitizedAssetName.Trim()}");
            }

            var filePath = Path.Combine(destinationPath, file.Path);
            return m_IOProxy.FileExists(filePath);
        }

        async Task CheckUpdatedAssetDataAsync(UpdatedAssetData updatedAssetData, CancellationToken token)
        {
            var resolutionInfos = updatedAssetData.Assets.Union(updatedAssetData.Dependants).ToList();
            if (resolutionInfos.Count == 0)
                return;

            var tasks = new List<Task>();
            foreach (var assetDataInfo in resolutionInfos)
            {
                tasks.Add(assetDataInfo.GatherFileConflictsAsync(m_AssetDataManager, token));
            }

            // TODO in the future, this could also check upward dependencies

            await Task.WhenAll(tasks);
        }

        async Task<HashSet<BaseAssetData>> GetUpdatedAssetDataAndDependenciesAsync(
            IEnumerable<BaseAssetData> assetDatas, ImportOperation.ImportType importType, CancellationToken token)
        {
            assetDatas = await GetUpdatedAssetDataAsync(assetDatas.Select(x => x.Identifier), importType, token);

#if AM4U_DEV
            var t = new Stopwatch();
            t.Start();
#endif

            // Key = "project-Id/asset-Id"
            // Value = the AssetData with the highest SequenceNumber
            // DOMAIN_RELOAD : serializing this dictionary would allow recovery
            var dependencies = new Dictionary<string, DependencyNode>();
            foreach (var assetData in assetDatas)
            {
                dependencies[BuildDependencyKey(assetData)] = new DependencyNode(assetData);
            }

            var depTasks = new List<Task>();
            foreach (var asset in assetDatas)
            {
                depTasks.Add(GetDependenciesRecursivelyAsync(ImportOperation.ImportType.Import, asset, dependencies, token));
            }

            await Task.WhenAll(depTasks);

#if AM4U_DEV
            t.Stop();
            Utilities.DevLog($"Took {t.ElapsedMilliseconds / 1000f:F2} s to gather dependencies");
#endif

            return dependencies
                .Select(x => x.Value.AssetData)
                .Where(x => x != null)
                .ToHashSet();
        }

        async Task GetDependenciesRecursivelyAsync(ImportOperation.ImportType importType, BaseAssetData root,
            Dictionary<string, DependencyNode> assetDatas, CancellationToken token)
        {
            var key = BuildDependencyKey(root);

            // Check if the root asset data is already being traversed
            lock (assetDatas)
            {
                if (assetDatas.TryGetValue(key, out var rootNode))
                {
                    if (rootNode.IsDependencyTraversalStarted)
                        return;

                    root = ChooseLatest(rootNode.AssetData, root);
                }

                assetDatas[key] = new DependencyNode(root, 0);
            }

            // List dependencies, but only retain those that are not already in the dictionary
            var dependencyIdentifiers = new List<AssetIdentifier>();
            foreach (var assetIdentifier in root.Dependencies)
            {
                var dependencyKey = BuildDependencyKey(assetIdentifier);
                lock (assetDatas)
                {
                    // If the dependency is already in the dictionary, the node has been visited.
                    if (assetDatas.ContainsKey(dependencyKey))
                    {
                        Utilities.DevLog("Skipping dependency.");
                        continue;
                    }

                    assetDatas[dependencyKey] = new DependencyNode(null);
                }

                dependencyIdentifiers.Add(assetIdentifier);
            }

            if (dependencyIdentifiers.Count == 0)
                return;

            // Make sure those dependencies are the most up to date.
            var dependencies = (await GetUpdatedAssetDataAsync(dependencyIdentifiers, importType, token)).ToArray();

            // Create new entries for each dependency
            for (var i = 0; i < dependencies.Length; ++i)
            {
                var temp = dependencies[i];

                var dependencyKey = BuildDependencyKey(temp);
                lock (assetDatas)
                {
                    if (assetDatas.TryGetValue(dependencyKey, out var dependencyNode))
                    {
                        if (dependencyNode.IsDependencyTraversalStarted)
                            continue;

                        dependencies[i] = ChooseLatest(dependencyNode.AssetData, temp);
                    }

                    assetDatas[key] = new DependencyNode(temp, 0);
                }
            }

            // DOMAIN_RELOAD : this is useful to track which nodes will need to be re-traversed
            // Once all dependency nodes are setup, update the root node
            lock (assetDatas)
            {
                assetDatas[key] = new DependencyNode(root, 1);
            }

            // Start tasks to traverse each dependency
            var tasks = new List<Task>();

            foreach (var dependency in dependencies)
            {
                tasks.Add(GetDependenciesRecursivelyAsync(importType, dependency, assetDatas, token));
            }

            await Task.WhenAll(tasks);
        }

        async Task<IEnumerable<BaseAssetData>> GetUpdatedAssetDataAsync(IEnumerable<AssetIdentifier> assetIdentifiers,
            ImportOperation.ImportType importType, CancellationToken token)
        {
#if AM4U_DEV
            var t = new Stopwatch();
            t.Start();
#endif

            var assetDatas = await SearchUpdatedAssetDataAsync(assetIdentifiers, importType, token);

#if AM4U_DEV
            Utilities.DevLog($"Took {t.ElapsedMilliseconds / 1000f:F2} s to update {assetDatas.Count()} assets.");
            t.Restart();
#endif

            var updateTasks = new List<Task>();
            foreach (var asset in assetDatas)
            {
                // Updates asset file list
                updateTasks.Add(asset.ResolveDatasetsAsync(token));

                // Updates dependency list
                updateTasks.Add(asset.RefreshDependenciesAsync(token));
            }

            await Task.WhenAll(updateTasks);

#if AM4U_DEV
            t.Stop();
            Utilities.DevLog($"Took {t.ElapsedMilliseconds / 1000f:F2} s for non-batchable update asset calls.");
#endif

            return assetDatas;
        }

        async Task<IEnumerable<BaseAssetData>> SearchUpdatedAssetDataAsync(IEnumerable<AssetIdentifier> assetIdentifiers,
            ImportOperation.ImportType importType, CancellationToken token)
        {
            // Split the searches by organization

            var assetsByOrg = new Dictionary<string, List<AssetIdentifier>>();
            foreach (var assetIdentifier in assetIdentifiers)
            {
                if (string.IsNullOrEmpty(assetIdentifier.OrganizationId))
                    continue;

                if (!assetsByOrg.ContainsKey(assetIdentifier.OrganizationId))
                {
                    assetsByOrg.Add(assetIdentifier.OrganizationId, new List<AssetIdentifier>());
                }

                assetsByOrg[assetIdentifier.OrganizationId].Add(assetIdentifier);
            }

            if (assetsByOrg.Count > 1)
            {
                Utilities.DevLog("Initiating search in multiple organizations.");
            }

            var tasks = assetsByOrg
                .Select(kvp => SearchUpdatedAssetDataAsync(kvp.Key, kvp.Value, importType, token))
                .ToArray();

            await Task.WhenAll(tasks);

            var targetAssetDatas = new List<BaseAssetData>();

            foreach (var task in tasks)
            {
                targetAssetDatas.AddRange(task.Result);
            }

            return targetAssetDatas;
        }

        async Task<IEnumerable<BaseAssetData>> SearchUpdatedAssetDataAsync(string organizationId,
            List<AssetIdentifier> assetIdentifiers, ImportOperation.ImportType importType, CancellationToken token)
        {
            // If there is only 1 asset, fetch that asset info directly (search has more overhead than a direct fetch).
            if (assetIdentifiers.Count == 1)
            {
                var identifier = assetIdentifiers[0];
                var asset = importType switch
                {
                    ImportOperation.ImportType.Import =>
                        await m_AssetsProvider.GetAssetAsync(identifier, token),
                    _ => await m_AssetsProvider.GetLatestAssetVersionAsync(identifier, token)
                };

                return new[] {asset};
            }

            // Split the asset list into chunks for multiple searches.

            var tasks = new List<Task<IEnumerable<BaseAssetData>>>();
            var startIndex = 0;
            while (startIndex < assetIdentifiers.Count)
            {
                var maxCount = Math.Min(m_AssetsProvider.DefaultSearchPageSize, assetIdentifiers.Count - startIndex);

                var assetIdentifierRange = assetIdentifiers.GetRange(startIndex, maxCount);
                var searchFilter = BuildSearchFilter(assetIdentifierRange, importType);
                tasks.Add(SearchUpdatedAssetDataAsync(m_AssetsProvider, organizationId, searchFilter,
                    assetIdentifierRange, token));

                startIndex += m_AssetsProvider.DefaultSearchPageSize;
            }

            await Task.WhenAll(tasks);

            var targetAssetDatas = new List<BaseAssetData>();

            foreach (var task in tasks)
            {
                targetAssetDatas.AddRange(task.Result);
            }

            return targetAssetDatas;
        }

        static async Task<IEnumerable<BaseAssetData>> SearchUpdatedAssetDataAsync(IAssetsProvider assetsProvider,
            string organizationId, AssetSearchFilter assetSearchFilter, IEnumerable<AssetIdentifier> assetIdentifiers,
            CancellationToken token)
        {
            var validAssetIds = assetIdentifiers.Select(x => x.AssetId).ToHashSet();

            var query = assetsProvider.SearchAsync(organizationId, null, assetSearchFilter,
                SortField.Name, SortingOrder.Ascending, 0, 0, token);

            var assets = new List<BaseAssetData>();

            await foreach (var asset in query)
            {
                // Ignore any false positive result.
                if (!validAssetIds.Contains(asset.Identifier.AssetId))
                {
                    Utilities.DevLogWarning($"Skipping false positive search result {asset.Name}.");
                    continue;
                }

                assets.Add(asset);

                if (assets.Count > assetsProvider.DefaultSearchPageSize)
                {
                    Utilities.DevLogWarning("Exceeding the expected number of searched assets.");
                    break;
                }
            }

            return assets;
        }

        static AssetSearchFilter BuildSearchFilter(IEnumerable<AssetIdentifier> assetIdentifiers, ImportOperation.ImportType importType)
        {
            if (!assetIdentifiers.Any())
            {
                throw new ArgumentException("Search list cannot be empty.", nameof(assetIdentifiers));
            }

            var assetSearchFilter = new AssetSearchFilter();

            switch (importType)
            {
                // Search by version specifically
                case ImportOperation.ImportType.Import:
                    assetSearchFilter.AssetVersions = new List<string>(assetIdentifiers.Select(x => x.Version));
                    break;

                // Search by assetId
                case ImportOperation.ImportType.UpdateToLatest:
                    assetSearchFilter.AssetIds = new List<string>(assetIdentifiers.Select(x => x.AssetId));
                    break;
            }

            return assetSearchFilter;
        }

        static string BuildDependencyKey(BaseAssetData assetData)
        {
            return BuildDependencyKey(assetData.Identifier);
        }

        static string BuildDependencyKey(AssetIdentifier assetIdentifier)
        {
            return $"{assetIdentifier.ProjectId}/{assetIdentifier.AssetId}";
        }

        static BaseAssetData ChooseLatest(BaseAssetData a, BaseAssetData b)
        {
            if (a == null) return b;
            if (b == null) return a;

            if (a.SequenceNumber > b.SequenceNumber)
                return a;

            if (a.SequenceNumber < b.SequenceNumber)
                return b;

            return a.Updated > b.Updated ? a : b;
        }

        bool IsDebugLogsEnabled()
        {
            return m_SettingsManager != null && m_SettingsManager.IsDebugLogsEnabled;
        }
    }
}
