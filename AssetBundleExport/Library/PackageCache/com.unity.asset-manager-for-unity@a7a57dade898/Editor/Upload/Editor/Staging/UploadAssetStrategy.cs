using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.Upload.Editor
{
    static class UploadAssetStrategy
    {
        public static IEnumerable<UploadAssetData> GenerateUploadAssets(UploadEdits uploadEdits, UploadSettings settings, Action<string, float> progressCallback = null)
        {
            Utilities.DevAssert(uploadEdits != null, "UploadEdits cannot be null");
            if (uploadEdits == null)
                return Enumerable.Empty<UploadAssetData>();

            Utilities.DevAssert(settings != null, "UploadSettings cannot be null");
            if (settings == null)
                return Enumerable.Empty<UploadAssetData>();

            IReadOnlyCollection<string> mainGuids = uploadEdits.MainAssetGuids;

            ISet<string> dependencies = new HashSet<string>();
            if (settings.DependencyMode == UploadDependencyMode.Separate)
            {
                dependencies = ResolveDependencies(mainGuids);
            }

            var allGuids = new HashSet<string>(mainGuids);
            allGuids.UnionWith(dependencies);

            var cache = new Dictionary<string, UploadAssetData>();
            var cacheAllScriptsGuids = new AllScriptGuidSnapshot();

            // Track circular dependencies that need to be resolved after all objects are created
            var circularDependencies = new Dictionary<string, List<string>>();

            var total = allGuids.Count;
            var count = 0f;

            foreach (var guid in allGuids)
            {
                progressCallback?.Invoke(guid, count++ / total);
                GenerateUploadAssetRecursive(guid, uploadEdits, settings, cache, cacheAllScriptsGuids, circularDependencies);
            }

            // Second pass: resolve circular dependencies
            ResolveCircularDependencies(cache, circularDependencies);

            // Set the dependencies for each asset
            foreach (var asset in cache.Values)
            {
                asset.IsDependency = !mainGuids.Contains(asset.Guid);
            }

            return cache.Values;
        }

        static UploadAssetData GenerateUploadAssetRecursive(string guid,
            UploadEdits uploadEdits,
            UploadSettings settings,
            Dictionary<string, UploadAssetData> cache,
            AllScriptGuidSnapshot cacheAllScriptGuids,
            Dictionary<string, List<string>> circularDependencies)
        {
            if (cache.TryGetValue(guid, out var result))
            {
                return result;
            }

            // Make sure guid is added to cache to avoid recursive calls
            cache[guid] = null;

            IEnumerable<string> dependencyGuids = null;
            IReadOnlyCollection<string> ignoredGuids = uploadEdits.IgnoredAssetGuids;
            IEnumerable<string> mainFileGuids = new List<string> { guid };
            IEnumerable<string> additionalFileGuids = Enumerable.Empty<string>();

            switch (settings.DependencyMode)
            {
                case UploadDependencyMode.Embedded:
                    // When embedding, we put all asset file + dependencies in the main files list
                    mainFileGuids = mainFileGuids.Concat(DependencyUtils.GetValidAssetDependencyGuids(guid, true));

                    // If requested to include all scripts, those goes in the additional files list
                    if (uploadEdits.IncludesAllScriptsForGuids.Contains(guid))
                    {

                        additionalFileGuids = additionalFileGuids
                            .Concat(cacheAllScriptGuids.GetCache().Where(g => g != guid));
                    }
                    break;

                case UploadDependencyMode.Separate:

                    // When creating separated assets, we build the list of dependencies starting from the main asset
                    dependencyGuids = DependencyUtils.GetValidAssetDependencyGuids(guid, false);

                    // If requested to include all scripts, we add those scripts to the dependency list
                    if (uploadEdits.IncludesAllScriptsForGuids.Contains(guid))
                    {
                        dependencyGuids = dependencyGuids
                            .Concat(cacheAllScriptGuids.GetCache().Where(g => g != guid))
                            .Distinct();
                    }
                    break;
            }

            // Filters both main and additional files to remove ignored assets
            mainFileGuids = mainFileGuids.Where(fileGuid => fileGuid == guid || !ignoredGuids.Contains(fileGuid));
            additionalFileGuids = additionalFileGuids.Where(fileGuid => fileGuid == guid || !ignoredGuids.Contains(fileGuid));

            // Loop recursively to generate all dependencies
            var deps = new List<UploadAssetData>();
            if (dependencyGuids != null)
            {
                foreach (var dependencyGuid in dependencyGuids)
                {
                    if (cache.TryGetValue(dependencyGuid, out var dep))
                    {
                        if (dep == null)
                        {
                            // Circular dependency detected - track it for later resolution
                            if (!circularDependencies.ContainsKey(guid))
                                circularDependencies[guid] = new List<string>();
                            circularDependencies[guid].Add(dependencyGuid);
                            continue;
                        }
                        deps.Add(dep);
                        continue;
                    }

                    dep = GenerateUploadAssetRecursive(dependencyGuid, uploadEdits, settings, cache, cacheAllScriptGuids, circularDependencies);
                    if (dep != null)
                    {
                        deps.Add(dep);
                    }
                }
            }

            var projectId = settings.UploadMode == UploadAssetMode.ForceNewAsset ? settings.ProjectId : null;
            var existingAssetData = AssetDataDependencyHelper.GetAssetAssociatedWithGuid(guid, settings.OrganizationId, projectId);
            var projectIdentifier = existingAssetData?.Identifier.ProjectIdentifier ??
                                    new ProjectIdentifier(settings.OrganizationId, settings.ProjectId);

            var assetUploadEntry = new UploadAssetData(new AssetIdentifier(guid), guid, mainFileGuids, additionalFileGuids, deps, existingAssetData, projectIdentifier, settings.FilePathMode);

            // NO SONAR
            cache[guid] = assetUploadEntry;

            return assetUploadEntry;
        }

        static void ResolveCircularDependencies(Dictionary<string, UploadAssetData> cache, Dictionary<string, List<string>> circularDependencies)
        {
            foreach (var (assetGuid, circularDeps) in circularDependencies)
            {
                if (!cache.TryGetValue(assetGuid, out var asset)) continue;

                // Get current dependencies and add the circular ones
                var currentDeps = asset.Dependencies.ToList();

                foreach (var circularDepGuid in circularDeps)
                {
                    if (!cache.TryGetValue(circularDepGuid, out var circularDepAsset)) continue;

                    // Add the circular dependency if it's not already present
                    if (currentDeps.All(dep => dep.AssetId != circularDepAsset.Identifier.AssetId))
                    {
                        currentDeps.Add(circularDepAsset.Identifier);
                    }
                }

                // Update the dependencies using the internal setter
                asset.Dependencies = currentDeps;
            }
        }

        public static ISet<string> ResolveMainSelection(params string[] guids)
        {
            var processed = new HashSet<string>();

            // Process main assets first
            foreach (var mainGuid in guids)
            {
                processed.UnionWith(ProcessAssetsAndFolders(mainGuid));
            }

            return processed;
        }

        static ISet<string> ResolveDependencies(IReadOnlyCollection<string> mainGuids)
        {
            var processed = new HashSet<string>(mainGuids);

            // Process Dependencies
            foreach (var guid in mainGuids)
            {
                processed.UnionWith(DependencyUtils.GetValidAssetDependencyGuids(guid, true));
            }

            return processed;
        }

        static IEnumerable<string> ProcessAssetsAndFolders(string guid)
        {
            var assetDatabaseProxy = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();
            var assetPath = assetDatabaseProxy.GuidToAssetPath(guid);
            return assetDatabaseProxy.IsValidFolder(assetPath)
                ? assetDatabaseProxy.GetAssetsInFolder(assetPath)
                : new[] { guid };
        }

        /// <summary>
        /// Tiny utility that creates a snapshot of all scripts in the project at first usage and keep those
        /// to ensure this list doesn't change.
        /// </summary>
        class AllScriptGuidSnapshot
        {
            IEnumerable<string> m_Cache;

            public IEnumerable<string> GetCache()
            {
                if (m_Cache == null)
                {
                    m_Cache = DependencyUtils.GetAllScriptGuids();
                }

                return m_Cache;
            }
        }
    }
}
