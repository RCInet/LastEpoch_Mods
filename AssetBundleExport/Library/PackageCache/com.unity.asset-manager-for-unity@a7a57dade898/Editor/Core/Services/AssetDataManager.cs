using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.AssetManager.Core.Editor
{
    class AssetChangeArgs
    {
        public IReadOnlyCollection<TrackedAssetIdentifier> Added = Array.Empty<TrackedAssetIdentifier>();
        public IReadOnlyCollection<TrackedAssetIdentifier> Removed = Array.Empty<TrackedAssetIdentifier>();
        public IReadOnlyCollection<TrackedAssetIdentifier> Updated = Array.Empty<TrackedAssetIdentifier>();
    }

    interface IAssetDataManager : IService
    {
        event Action<AssetChangeArgs> ImportedAssetInfoChanged;
        event Action<AssetChangeArgs> AssetDataChanged;

        IReadOnlyCollection<ImportedAssetInfo> ImportedAssetInfos { get; }

        void SetImportedAssetInfos(IReadOnlyCollection<ImportedAssetInfo> allImportedInfos);

        void AddOrUpdateGuidsToImportedAssetInfo(BaseAssetData assetData,
            IReadOnlyCollection<ImportedFileInfo> fileInfos);

        void RemoveFilesFromImportedAssetInfos(IReadOnlyCollection<string> guidsToRemove);
        void AddOrUpdateAssetDataFromCloudAsset(IEnumerable<BaseAssetData> assetDatas);
        ImportedAssetInfo GetImportedAssetInfo(AssetIdentifier assetIdentifier);
        ImportedAssetInfo GetImportedAssetInfo(string assetId);
        void RemoveImportedAssetInfo(IEnumerable<AssetIdentifier> assetIdentifiers);
        List<ImportedAssetInfo> GetImportedAssetInfosFromFileGuid(string guid);
        string GetImportedFileGuid(AssetIdentifier assetIdentifier, string path);
        BaseAssetData GetAssetData(AssetIdentifier assetIdentifier);
        List<BaseAssetData> GetAssetsData(IEnumerable<AssetIdentifier> ids);
        Task<BaseAssetData> GetOrSearchAssetData(AssetIdentifier assetIdentifier, CancellationToken token);
        bool IsInProject(AssetIdentifier id);
        HashSet<AssetIdentifier> FindExclusiveDependencies(IEnumerable<AssetIdentifier> assetIdentifiersToDelete);
    }

    [Serializable]
    class AssetDataManager : BaseService<IAssetDataManager>, IAssetDataManager, ISerializationCallbackReceiver
    {
        class Node
        {
            readonly TrackedAssetIdentifier m_Identifier;
            readonly HashSet<Node> m_Dependencies = new();
            readonly HashSet<Node> m_DependentBy = new();

            public TrackedAssetIdentifier Identifier => m_Identifier;
            public HashSet<Node> Dependencies => m_Dependencies;
            public HashSet<Node> DependentBy => m_DependentBy;
            public bool IsRoot { get; set; }

            public Node(TrackedAssetIdentifier identifier)
            {
                m_Identifier = identifier;
            }

            public override bool Equals(object obj)
            {
                return obj is Node node && Identifier.Equals(node.Identifier);
            }

            public override int GetHashCode()
            {
                return m_Identifier.GetHashCode();
            }
        }

        class TrackedIdentifierMap : IDictionary<TrackedAssetIdentifier, ImportedAssetInfo>
        {
            readonly Dictionary<TrackedAssetIdentifier, ImportedAssetInfo> m_ImportedAssetInfoLookup = new();
            readonly Dictionary<TrackedAssetIdentifier, HashSet<TrackedAssetIdentifier>> m_DependenciesMap = new();
            readonly Dictionary<TrackedAssetIdentifier, HashSet<TrackedAssetIdentifier>> m_DependentsMap = new();

            public IReadOnlyDictionary<TrackedAssetIdentifier, HashSet<TrackedAssetIdentifier>> DependentsMap => m_DependentsMap;

            public ImportedAssetInfo this[TrackedAssetIdentifier key]
            {
                get => m_ImportedAssetInfoLookup[key];
                set
                {
                    m_ImportedAssetInfoLookup[key] = value;
                    UpdateDependencies();
                }
            }

            public ICollection<TrackedAssetIdentifier> Keys => m_ImportedAssetInfoLookup.Keys;
            public ICollection<ImportedAssetInfo> Values => m_ImportedAssetInfoLookup.Values;
            public int Count => m_ImportedAssetInfoLookup.Count;
            public bool IsReadOnly => ((IDictionary<TrackedAssetIdentifier, ImportedAssetInfo>)m_ImportedAssetInfoLookup).IsReadOnly;

            public void Add(TrackedAssetIdentifier key, ImportedAssetInfo value)
            {
                m_ImportedAssetInfoLookup.Add(key, value);
                UpdateDependencies();
            }

            public void Add(KeyValuePair<TrackedAssetIdentifier, ImportedAssetInfo> item)
            {
                m_ImportedAssetInfoLookup.Add(item.Key, item.Value);
                UpdateDependencies();
            }

            public bool Remove(TrackedAssetIdentifier key)
            {
                var result = m_ImportedAssetInfoLookup.Remove(key);
                if (result)
                    RemoveFromDependencies(key);
                return result;
            }

            public bool Remove(KeyValuePair<TrackedAssetIdentifier, ImportedAssetInfo> item)
            {
                var result = m_ImportedAssetInfoLookup.Remove(item.Key);
                if (result)
                    RemoveFromDependencies(item.Key);
                return result;
            }

            public bool TryGetValue(TrackedAssetIdentifier key, out ImportedAssetInfo value) => m_ImportedAssetInfoLookup.TryGetValue(key, out value);
            public bool ContainsKey(TrackedAssetIdentifier key) => m_ImportedAssetInfoLookup.ContainsKey(key);
            public bool Contains(KeyValuePair<TrackedAssetIdentifier, ImportedAssetInfo> item) => m_ImportedAssetInfoLookup.Contains(item);

            public void CopyTo(KeyValuePair<TrackedAssetIdentifier, ImportedAssetInfo>[] array, int arrayIndex) => ((IDictionary<TrackedAssetIdentifier, ImportedAssetInfo>)m_ImportedAssetInfoLookup).CopyTo(array, arrayIndex);

            public void Clear()
            {
                m_ImportedAssetInfoLookup.Clear();
                ClearDependencyMaps();
            }

            void ClearDependencyMaps()
            {
                m_DependentsMap.Clear();
                m_DependenciesMap.Clear();
            }

            public IEnumerator<KeyValuePair<TrackedAssetIdentifier, ImportedAssetInfo>> GetEnumerator() => m_ImportedAssetInfoLookup.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => m_ImportedAssetInfoLookup.GetEnumerator();

            public HashSet<TrackedAssetIdentifier> GetDependencies(TrackedAssetIdentifier trackedAssetIdentifier)
                => m_DependenciesMap.TryGetValue(trackedAssetIdentifier, out var directDependencies) ? directDependencies : UpdateDependencies(trackedAssetIdentifier);

            void UpdateDependencies()
            {
                UpdateDependencyMap();
                UpdateDependentsMap();
            }

            void UpdateDependencyMap()
            {
                foreach (var id in m_ImportedAssetInfoLookup.Keys)
                    UpdateDependencies(id);
            }

            HashSet<TrackedAssetIdentifier> UpdateDependencies(TrackedAssetIdentifier id)
            {
                var dependencies = new HashSet<TrackedAssetIdentifier>();

                if(m_ImportedAssetInfoLookup.TryGetValue(id, out var importedAssetInfo))
                {
                    foreach (var dependencyId in importedAssetInfo.AssetData.Dependencies)
                    {
                        var trackedDependencyId = new TrackedAssetIdentifier(dependencyId);

                        // Only add the dependency if it's an imported asset
                        if (m_ImportedAssetInfoLookup.ContainsKey(trackedDependencyId))
                            dependencies.Add(new TrackedAssetIdentifier(dependencyId));
                    }
                }

                m_DependenciesMap[id] = dependencies;
                return dependencies;
            }

            void UpdateDependentsMap()
            {
                foreach (var id in m_ImportedAssetInfoLookup.Keys)
                    UpdateDependents(id);
            }

            void UpdateDependents(TrackedAssetIdentifier id)
            {
                if (!m_DependentsMap.ContainsKey(id))
                    m_DependentsMap[id] = new HashSet<TrackedAssetIdentifier>();

                foreach (var dependency in GetDependencies(id))
                {
                    if (!m_DependentsMap.ContainsKey(dependency))
                        m_DependentsMap[dependency] = new HashSet<TrackedAssetIdentifier>();

                    m_DependentsMap[dependency].Add(id);
                }
            }

            void RemoveFromDependencies(TrackedAssetIdentifier id)
            {
                RemoveFromDependencyMap(id);
                RemoveFromDependentsMap(id);
            }

            void RemoveFromDependencyMap(TrackedAssetIdentifier id) => m_DependenciesMap.Remove(id);

            void RemoveFromDependentsMap(TrackedAssetIdentifier id)
            {
                m_DependentsMap.Remove(id);
                foreach (var dependency in m_DependentsMap.Values)
                    dependency.Remove(id);
            }

        }

        [SerializeField]
        ImportedAssetInfo[] m_SerializedImportedAssetInfos = Array.Empty<ImportedAssetInfo>();

        [SerializeReference]
        BaseAssetData[] m_SerializedAssetData = Array.Empty<BaseAssetData>();

        [SerializeReference]
        IPermissionsManager m_PermissionsManager;

        readonly Dictionary<TrackedAssetIdentifier, BaseAssetData> m_AssetData = new();
        readonly Dictionary<string, List<ImportedAssetInfo>> m_FileGuidToImportedAssetInfosMap = new();
        readonly TrackedIdentifierMap m_TrackedIdentifierMap = new();

        public event Action<AssetChangeArgs> ImportedAssetInfoChanged = delegate { };
        public event Action<AssetChangeArgs> AssetDataChanged = delegate { };

        public IReadOnlyCollection<ImportedAssetInfo> ImportedAssetInfos =>
            (IReadOnlyCollection<ImportedAssetInfo>) m_TrackedIdentifierMap.Values;

        [ServiceInjection]
        public void Inject(IPermissionsManager permissionsManager)
        {
            m_PermissionsManager = permissionsManager;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (m_PermissionsManager != null)
            {
                m_PermissionsManager.AuthenticationStateChanged += OnAuthenticationStateChanged;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();

            if (m_PermissionsManager != null)
            {
                m_PermissionsManager.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            }
        }

        void OnAuthenticationStateChanged(AuthenticationState newState)
        {
            if (newState != AuthenticationState.LoggedIn)
            {
                m_AssetData.Clear();
            }
        }

        public void SetImportedAssetInfos(IReadOnlyCollection<ImportedAssetInfo> allImportedInfos)
        {
            allImportedInfos ??= Array.Empty<ImportedAssetInfo>();
            var oldAssetIds = m_TrackedIdentifierMap.Keys.ToHashSet();
            m_FileGuidToImportedAssetInfosMap.Clear();
            m_TrackedIdentifierMap.Clear();

            var added = new HashSet<TrackedAssetIdentifier>();
            var updated = new HashSet<TrackedAssetIdentifier>();
            foreach (var info in allImportedInfos)
            {
                var id = new TrackedAssetIdentifier(info.Identifier);

                AddOrUpdateImportedAssetInfo(info);
                if (oldAssetIds.Contains(id))
                {
                    updated.Add(id);
                }
                else
                {
                    added.Add(id);
                }
            }

            foreach (var newInfo in m_TrackedIdentifierMap.Values)
            {
                oldAssetIds.Remove(new TrackedAssetIdentifier(newInfo.Identifier));
            }

            if (added.Count + updated.Count + oldAssetIds.Count > 0)
            {
                ImportedAssetInfoChanged?.Invoke(new AssetChangeArgs
                    { Added = added, Removed = oldAssetIds, Updated = updated });
            }
        }

        public void AddOrUpdateGuidsToImportedAssetInfo(BaseAssetData assetData,
            IReadOnlyCollection<ImportedFileInfo> fileInfos)
        {
            if (assetData == null)
                return;

            var added = new HashSet<TrackedAssetIdentifier>();
            var updated = new HashSet<TrackedAssetIdentifier>();

            var id = new TrackedAssetIdentifier(assetData.Identifier);
            var info = new ImportedAssetInfo(assetData, fileInfos ?? Array.Empty<ImportedFileInfo>());

            if (GetImportedAssetInfo(assetData.Identifier) == null)
            {
                added.Add(id);
            }
            else
            {
                updated.Add(id);
            }

            AddOrUpdateImportedAssetInfo(info);

            ImportedAssetInfoChanged?.Invoke(new AssetChangeArgs { Added = added, Updated = updated });
        }

        public void RemoveFilesFromImportedAssetInfos(IReadOnlyCollection<string> guidsToRemove)
        {
            guidsToRemove ??= Array.Empty<string>();
            if (guidsToRemove.Count <= 0)
                return;

            var updated = new List<TrackedAssetIdentifier>();
            var removed = new List<TrackedAssetIdentifier>();
            foreach (var fileGuid in guidsToRemove)
            {
                var assetInfos = GetImportedAssetInfosFromFileGuid(fileGuid);
                if (assetInfos == null)
                    continue;

                m_FileGuidToImportedAssetInfosMap.Remove(fileGuid);
                foreach (var asset in assetInfos)
                {
                    var id = new TrackedAssetIdentifier(asset.Identifier);

                    asset.FileInfos.RemoveAll(i => i.Guid == fileGuid);
                    if (asset.FileInfos.Count > 0)
                    {
                        updated.Add(id);
                    }
                    else
                    {
                        updated.Remove(id);
                        removed.Add(id);
                        m_TrackedIdentifierMap.Remove(id);
                    }
                }
            }

            if (updated.Count + removed.Count > 0)
            {
                ImportedAssetInfoChanged?.Invoke(new AssetChangeArgs { Removed = removed, Updated = updated });
            }
        }

        public void AddOrUpdateAssetDataFromCloudAsset(IEnumerable<BaseAssetData> assetDatas)
        {
            var assetChangeArgs = new AssetChangeArgs();
            var updated = new HashSet<TrackedAssetIdentifier>();
            var added = new HashSet<TrackedAssetIdentifier>();

            if (assetDatas == null || !assetDatas.Any())
                return;

            foreach (var assetData in assetDatas)
            {
                var id = new TrackedAssetIdentifier(assetData.Identifier);

                if (m_AssetData.ContainsKey(id))
                {
                    updated.Add(id);
                }
                else
                {
                    added.Add(id);
                }

                m_AssetData[id] = assetData;
            }

            assetChangeArgs.Added = added;
            assetChangeArgs.Updated = updated;
            AssetDataChanged?.Invoke(assetChangeArgs);
        }

        public ImportedAssetInfo GetImportedAssetInfo(AssetIdentifier assetIdentifier)
            => GetImportedAssetInfo(new TrackedAssetIdentifier(assetIdentifier));

        public ImportedAssetInfo GetImportedAssetInfo(TrackedAssetIdentifier assetIdentifier)
        {
            return assetIdentifier?.IsIdValid() == true &&
                   m_TrackedIdentifierMap.TryGetValue(assetIdentifier,
                       out var result) ?
                result :
                null;
        }

        // Retrieve the asset identifier using the assetId
        public ImportedAssetInfo GetImportedAssetInfo(string assetId)
            => GetImportedAssetInfo(m_TrackedIdentifierMap.Keys.FirstOrDefault(x => x.AssetId == assetId));

        public void RemoveImportedAssetInfo(IEnumerable<AssetIdentifier> assetIdentifiers)
        {
            var idsToRemove = new List<TrackedAssetIdentifier>();

            foreach (var assetIdentifier in assetIdentifiers)
            {
                var id = new TrackedAssetIdentifier(assetIdentifier);
                idsToRemove.Add(id);

                if (m_TrackedIdentifierMap.Remove(id, out var importedAssetInfo))
                {
                    // Remove all file guids related to that imported asset too
                    foreach (var fileInfo in importedAssetInfo.FileInfos)
                    {
                        if (m_FileGuidToImportedAssetInfosMap.TryGetValue(fileInfo.Guid, out var importedAssetInfos))
                        {
                            var entry = importedAssetInfos.Find(info => info.Identifier.Equals(assetIdentifier));

                            if (entry == null)
                                continue;

                            importedAssetInfos.Remove(entry);

                            if (importedAssetInfos.Count == 0)
                            {
                                m_FileGuidToImportedAssetInfosMap.Remove(fileInfo.Guid);
                            }
                        }
                    }
                }
            }

            ImportedAssetInfoChanged?.Invoke(new AssetChangeArgs { Removed = idsToRemove });
        }

        public List<ImportedAssetInfo> GetImportedAssetInfosFromFileGuid(string guid)
        {
            return m_FileGuidToImportedAssetInfosMap.GetValueOrDefault(guid);
        }

        public string GetImportedFileGuid(AssetIdentifier assetIdentifier, string path)
        {
            if (assetIdentifier == null)
                return null;

            var importedInfo = GetImportedAssetInfo(assetIdentifier);
            var importedFileInfo = importedInfo?.FileInfos?.Find(f => Utilities.ComparePaths(f.OriginalPath, path));

            return importedFileInfo?.Guid;
        }

        public BaseAssetData GetAssetData(AssetIdentifier assetIdentifier)
        {
            if (assetIdentifier?.IsIdValid() != true)
            {
                return null;
            }

            var id = new TrackedAssetIdentifier(assetIdentifier);

            if (m_TrackedIdentifierMap.TryGetValue(id, out var info))
            {
                return info?.AssetData;
            }

            return m_AssetData.GetValueOrDefault(id);
        }

        public List<BaseAssetData> GetAssetsData(IEnumerable<AssetIdentifier> ids)
        {
            var result = new List<BaseAssetData>();

            foreach (var id in ids)
            {
                var assetData = GetAssetData(id);
                if (assetData != null)
                {
                    result.Add(assetData);
                }
            }

            return result;
        }

        public async Task<BaseAssetData> GetOrSearchAssetData(AssetIdentifier assetIdentifier, CancellationToken token)
        {
            var assetData = GetAssetData(assetIdentifier);

            if (assetData != null)
            {
                return assetData;
            }

            try
            {
                var assetProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();
                assetData = await assetProvider.GetAssetAsync(assetIdentifier, token);
            }
            catch (ForbiddenException)
            {
                // Unavailable
            }
            catch (NotFoundException)
            {
                // Not found
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (assetData != null)
            {
                AddOrUpdateAssetDataFromCloudAsset(new[] { assetData });
            }

            return assetData;
        }

        public bool IsInProject(AssetIdentifier id)
        {
            Utilities.DevAssert(!id.IsLocal(), "Calling IsInProject on a local identifier doesn't make sense.");

            if (id.IsLocal())
            {
                return false;
            }

            return GetImportedAssetInfo(id) != null;
        }

        // Returns the assets that are self-contained and have no dependencies on other assets
        // Return value includes the input assets
        public HashSet<AssetIdentifier> FindExclusiveDependencies(IEnumerable<AssetIdentifier> assetIdentifiersToDelete)
        {
            if (assetIdentifiersToDelete == null || !assetIdentifiersToDelete.Any())
            {
                return new HashSet<AssetIdentifier>();
            }

            var graph = BuildDependenciesGraph();
            var assetsToDelete = new HashSet<TrackedAssetIdentifier>(assetIdentifiersToDelete.Where(IsInProject)
                .Select(id => new TrackedAssetIdentifier(id)));
            var finalAssetsToDelete = new HashSet<TrackedAssetIdentifier>();

            foreach (var id in assetsToDelete)
            {
                if (graph.TryGetValue(id, out var value))
                    finalAssetsToDelete.UnionWith(DeleteNodeAndOrphanedDependencies(graph, value));
            }

            return finalAssetsToDelete.Select(a => m_TrackedIdentifierMap[a].Identifier).ToHashSet();
        }

        public void OnBeforeSerialize()
        {
            m_SerializedImportedAssetInfos = m_TrackedIdentifierMap.Values.ToArray();
            m_SerializedAssetData = m_AssetData.Values.ToArray();
        }

        public void OnAfterDeserialize()
        {
            foreach (var info in m_SerializedImportedAssetInfos)
            {
                AddOrUpdateImportedAssetInfo(info);
            }

            foreach (var data in m_SerializedAssetData)
            {
                m_AssetData[new TrackedAssetIdentifier(data.Identifier)] = data;
            }
        }

        Dictionary<TrackedAssetIdentifier, Node> BuildDependenciesGraph()
        {
            var dependentsMap = m_TrackedIdentifierMap.DependentsMap;

            var graph = new Dictionary<TrackedAssetIdentifier, Node>();

            foreach (var id in m_TrackedIdentifierMap.Keys)
            {
                var node = new Node(id);
                graph[id] = node;
            }

            foreach (var id in m_TrackedIdentifierMap.Keys)
            {
                var node = graph[id];
                foreach (var dependency in m_TrackedIdentifierMap.GetDependencies(id))
                {
                    if (graph.TryGetValue(dependency, out var dependencyNode))
                    {
                        node.Dependencies.Add(dependencyNode);
                    }
                }

                foreach (var dependent in dependentsMap[id])
                {
                    if (graph.TryGetValue(dependent, out var dependentNode))
                    {
                        node.DependentBy.Add(dependentNode);
                    }
                }

                if(node.DependentBy.Count == 0)
                {
                    node.IsRoot = true;
                }
            }

            return graph;
        }

        IEnumerable<TrackedAssetIdentifier> DeleteNodeAndOrphanedDependencies(Dictionary<TrackedAssetIdentifier, Node> graph, Node nodeToDelete)
        {
            if (!graph.TryGetValue(nodeToDelete.Identifier, out _))
            {
                return new List<TrackedAssetIdentifier>();
            }

            var nodesToProcess = new HashSet<Node>(); // Track all nodes we need to evaluate
            var nodesToDelete = new HashSet<Node>(); // Track nodes that will be deleted
            var nodesToCheck = new HashSet<Node>(); // Track nodes that need to be checked for circular dependencies

            // Start with the initial node
            nodesToProcess.Add(nodeToDelete);
            nodesToDelete.Add(nodeToDelete);

            // First pass, remove all references to this node from nodes that depend on it
            foreach (var dependent in nodeToDelete.DependentBy)
            {
                dependent.Dependencies.Remove(nodeToDelete);
            }

            // Second pass: Collect all potentially orphaned nodes
            while (nodesToProcess.Count > 0)
            {
                var current = nodesToProcess.First();
                nodesToProcess.Remove(current);

                foreach (var dependency in current.Dependencies)
                {
                    // Remove the current node from dependency's reverse references
                    dependency.DependentBy.Remove(current);

                    // If this dependency has no other dependents, and we haven't processed it yet
                    if (dependency.DependentBy.Count == 0 && !nodesToDelete.Contains(dependency))
                    {
                        nodesToProcess.Add(dependency);
                        nodesToDelete.Add(dependency);
                    }
                    else
                    {
                        // Add to check for potential orphan loop later
                        nodesToCheck.Add(dependency);
                    }
                }
            }

            // Third pass: Check for orphan loops
            var nodesToCheckQueue = new Queue<Node>(nodesToCheck);
            while(nodesToCheckQueue.Count > 0)
            {
                var node = nodesToCheckQueue.Dequeue();
                var isRootFound = false;
                var visited = new HashSet<Node>();
                nodesToProcess.Add(node);

                while (nodesToProcess.Count > 0 && !isRootFound)
                {
                    var current = nodesToProcess.First();
                    nodesToProcess.Remove(current);
                    visited.Add(current);

                    foreach (var dependent in current.DependentBy)
                    {
                        if(dependent.IsRoot)
                        {
                            isRootFound = true;
                            break;
                        }

                        if(!visited.Contains(dependent))
                        {
                            nodesToProcess.Add(dependent);
                        }
                    }
                }

                // If it is not linked to a root node, it's an orphan node
                if(!isRootFound && !nodesToDelete.Contains(node))
                {
                    nodesToDelete.Add(node);

                    foreach (var dependency in node.Dependencies)
                    {
                        dependency.DependentBy.Remove(node);
                        nodesToCheckQueue.Enqueue(dependency);
                    }
                }
            }

            // Fourth pass: Clean up all references between nodes that will be deleted
            foreach (var node in nodesToDelete)
            {
                node.Dependencies.Clear();
                node.DependentBy.Clear();
            }

            foreach (var node in nodesToDelete)
            {
                graph.Remove(node.Identifier);
            }

            return nodesToDelete.Select(n => n.Identifier);
        }

        void AddOrUpdateImportedAssetInfo(ImportedAssetInfo info)
        {
            // This method updates both m_TrackedIdentifierMap and m_FileGuidToImportedAssetInfosMap

            var trackId = new TrackedAssetIdentifier(info.Identifier);

            // Iterate through every imported file related to that imported asset
            foreach (var fileInfo in info.FileInfos)
            {
                if (!m_FileGuidToImportedAssetInfosMap.TryGetValue(fileInfo.Guid, out var importedAssetInfos))
                {
                    // If that file Guid is not related to any existing imported asset, we can simply create a new entry to track it
                    m_FileGuidToImportedAssetInfosMap[fileInfo.Guid] = new List<ImportedAssetInfo> { info };
                }
                else
                {
                    // Otherwise, we need to verify to which asset info this file is related to
                    // If the file was related to a different (or same) version, we need to untrack it first, before tracking it again using updated imported info
                    // Maybe a dictionary would have improved the readability of this code
                    var duplicate =
                        importedAssetInfos.Find(i => new TrackedAssetIdentifier(i.Identifier).Equals(trackId));
                    if (duplicate != null)
                    {
                        importedAssetInfos.Remove(duplicate);
                    }

                    importedAssetInfos.Add(info);
                }
            }

            // m_TrackedIdentifierMap must contain the updated imported info, ignoring its version, because only one version can be imported at a time.
            m_TrackedIdentifierMap[trackId] = info;
        }
    }
}
