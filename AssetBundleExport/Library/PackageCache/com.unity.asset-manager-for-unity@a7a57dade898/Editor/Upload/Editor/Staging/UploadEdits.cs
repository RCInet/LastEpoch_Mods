using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.Upload.Editor
{
    [Serializable]
    // Quick solution to hold manual edits information between two UploadStaging.GenerateUploadAssetData
    // Without this, if the user manually edits assets, then changes the Dependency Mode, edits will be lost
    // Ideally, we should only generate the UploadAssetData once or find a way to re-use the same UploadAssetData instances
    class UploadEdits
    {
        [SerializeField]
        // Assets manually selected by the user
        List<string> m_MainAssetGuids = new();

        [SerializeField]
        // Assets manually ignored by the user
        List<string> m_IgnoredAssetGuids = new();

        [SerializeField]
        // Assets that should include All Scripts
        List<string> m_IncludesAllScripts = new();

        [SerializeReference]
        MetadataModification m_ModifiedMetadata = new();

        public IReadOnlyCollection<string> MainAssetGuids => m_MainAssetGuids;
        public IReadOnlyCollection<string> IgnoredAssetGuids => m_IgnoredAssetGuids;
        public IReadOnlyCollection<string> IncludesAllScriptsForGuids => m_IncludesAllScripts;

        public void AddToSelection(string assetOrFolderGuid)
        {
            // Parse selection to extract folder content
            var mainGuids = UploadAssetStrategy.ResolveMainSelection(assetOrFolderGuid);

            foreach (var guid in mainGuids)
            {
                if (m_MainAssetGuids.Contains(guid))
                    continue;

                m_MainAssetGuids.Add(guid);
            }
        }

        public bool IsSelected(string guid)
        {
            return m_MainAssetGuids.Contains(guid);
        }

        public bool IsEmpty()
        {
            return m_MainAssetGuids.Count == 0;
        }

        public bool RemoveFromSelection(string guid)
        {
            if (!m_MainAssetGuids.Contains(guid))
                return false;

            m_MainAssetGuids.Remove(guid);
            return true;
        }

        public void Clear()
        {
            m_MainAssetGuids.Clear();
            m_IgnoredAssetGuids.Clear();
            m_IncludesAllScripts.Clear();
            m_ModifiedMetadata.Dictionary.Clear();
        }

        public void SetIgnore(string assetGuid, bool ignore)
        {
            if (ignore && !m_IgnoredAssetGuids.Contains(assetGuid))
            {
                m_IgnoredAssetGuids.Add(assetGuid);
            }
            else if (!ignore && m_IgnoredAssetGuids.Contains(assetGuid))
            {
                m_IgnoredAssetGuids.Remove(assetGuid);
            }
        }

        public bool IsIgnored(string assetGuid)
        {
            return m_IgnoredAssetGuids.Contains(assetGuid);
        }

        public bool IncludesAllScripts(string assetDataGuid)
        {
            return m_IncludesAllScripts.Contains(assetDataGuid);
        }

        public void SetIncludesAllScripts(string assetDataGuid, bool include)
        {
            if (include && !m_IncludesAllScripts.Contains(assetDataGuid))
            {
                m_IncludesAllScripts.Add(assetDataGuid);
            }
            else if (!include && m_IncludesAllScripts.Contains(assetDataGuid))
            {
                m_IncludesAllScripts.Remove(assetDataGuid);
            }
        }

        public void SetModifiedMetadata(string assetDataGuid, string projectId, IMetadataContainer metadataContainer)
        {
            var key = GetKey(assetDataGuid, projectId);
            m_ModifiedMetadata.Dictionary[key] = metadataContainer;
        }

        public bool TryGetModifiedMetadata(string assetDataGuid, string projectId, out IReadOnlyCollection<IMetadata> metadata)
        {
            var key = GetKey(assetDataGuid, projectId);
            if (m_ModifiedMetadata.Dictionary.TryGetValue(key, out var dictionary))
            {
                metadata = dictionary.ToList();
                return true;
            }

            metadata = null;
            return false;
        }

        // We want to store the metadata per-Project basis
        // This is because in case of a re-upload, the metadata might not be the same for the same asset
        // We can try and think for a better solution in the future
        static string GetKey(string assetDataGuid, string projectId)
        {
            return assetDataGuid + "__" + projectId;
        }
    }

    [Serializable]
    class MetadataModification : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<string> m_Keys = new();

        [SerializeReference]
        List<IMetadataContainer> m_Values = new();

        Dictionary<string, IMetadataContainer> m_Dictionary = new();
        public Dictionary<string, IMetadataContainer> Dictionary => m_Dictionary;

        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();

            foreach (var kvp in Dictionary)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            m_Dictionary = new Dictionary<string, IMetadataContainer>();
            Utilities.DevAssert(m_Keys.Count == m_Values.Count);

            try
            {
                for (int i = 0; i < m_Keys.Count; i++)
                {
                    m_Dictionary[m_Keys[i]] = m_Values[i];
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
