using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class AssetDataCollection<T> : IReadOnlyCollection<T>, ISerializationCallbackReceiver where T : BaseAssetData
    {
        [SerializeField]
        List<AssetIdentifier> m_Identifiers;

        List<T> m_AssetList = new();

        public int Count => m_AssetList.Count;

        public void RebuildAssetList(IAssetDataManager assetDataManager)
        {
            // We want this collection to always use instances from the AssetDataManager.
            // Ideally we have called this in OnAfterDeserialize, but we can't because AssetDataManager might not be available at that time.

            // A null m_Identifiers means there is nothing to rebuild.
            if (m_Identifiers == null)
                return;

            m_AssetList.Clear();

            if (m_Identifiers.Count == 0)
                return;

            m_AssetList = m_Identifiers.Select(assetDataManager.GetAssetData).Cast<T>().ToList();
            m_Identifiers = null;
        }

        public void Add(T item)
        {
            m_AssetList.Add(item);
            m_Identifiers = null;
        }

        public object IndexOf(T item)
        {
            return m_AssetList.IndexOf(item);
        }

        public void Replace(T item, T newItem)
        {
            m_AssetList[m_AssetList.IndexOf(item)] = newItem;
            m_Identifiers = null;
        }

        public void Clear()
        {
            m_AssetList.Clear();
            m_Identifiers = null;
        }

        public bool Exists(Predicate<T> func)
        {
            return m_AssetList.Exists(func);
        }

        public T Find(Predicate<T> func)
        {
            return m_AssetList.Find(func);
        }

        public void SetValues(IEnumerable<T> items)
        {
            m_AssetList.Clear();
            m_AssetList.AddRange(items);
            m_Identifiers = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_AssetList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void OnBeforeSerialize()
        {
            m_Identifiers = m_AssetList.Select(asstData => asstData.Identifier).ToList();
        }

        public void OnAfterDeserialize()
        {
            // Nothing
        }
    }
}
