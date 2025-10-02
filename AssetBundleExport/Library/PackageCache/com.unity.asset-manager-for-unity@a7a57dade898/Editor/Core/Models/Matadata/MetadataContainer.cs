using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IMetadataContainer : IEnumerable<IMetadata>
    {
        int Count();
        bool ContainsKey(string fieldKey);
        bool ContainsMatch(IMetadata otherMetadata);
    }

    [Serializable]
    class MetadataContainer : IMetadataContainer, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<string> m_Keys = new();

        [SerializeReference]
        List<IMetadata> m_Values = new();

        Dictionary<string, IMetadata> m_Dictionary;

        public MetadataContainer()
        {
            m_Dictionary = new Dictionary<string, IMetadata>();
        }

        public MetadataContainer(IEnumerable<IMetadata> metadata)
        {
            Set(metadata);
        }

        public void Set(IEnumerable<IMetadata> metadata)
        {
            m_Dictionary = metadata.ToDictionary(x => x.FieldKey);
        }

        public bool ContainsKey(string fieldKey)
        {
            return m_Dictionary.ContainsKey(fieldKey);
        }

        public bool ContainsMatch(IMetadata otherMetadata)
        {
            if (!m_Dictionary.TryGetValue(otherMetadata.FieldKey, out var existingMetadata))
                return false;

            return existingMetadata.Equals(otherMetadata);
        }

        public int Count()
        {
            return m_Dictionary.Count;
        }

        public void Remove(string fieldKey)
        {
            m_Dictionary.Remove(fieldKey);
        }

        public void Add(IMetadata metadata)
        {
            Utilities.DevAssert(metadata != null);
            if (metadata == null)
                return;

            m_Dictionary.TryAdd(metadata.FieldKey, metadata);
        }

        public IEnumerator<IMetadata> GetEnumerator()
        {
            return m_Dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();

            if (m_Dictionary == null)
                return;

            foreach (var kvp in m_Dictionary)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            m_Dictionary = new Dictionary<string, IMetadata>();
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
