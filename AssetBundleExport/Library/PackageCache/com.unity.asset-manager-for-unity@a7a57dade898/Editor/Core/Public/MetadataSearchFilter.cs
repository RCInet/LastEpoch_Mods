using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.Editor
{
    /// <summary>
    /// A filter for selecting assets to import based on custom metadata.
    /// </summary>
    /// <remarks>
    /// All metadata fields are combined with "AND" logic, meaning that an asset must match all metadata fields to be included in the search results.
    /// </remarks>
    [Serializable]
    public class MetadataSearchFilter : IDictionary<string, Metadata>, ISerializationCallbackReceiver
    {
        [SerializeReference]
        List<Metadata> m_Metadata = new();

        readonly Dictionary<string, Metadata> m_MetadataDictionary = new();

        /// <inheritdoc />
        public int Count => m_MetadataDictionary.Count;

        /// <inheritdoc />
        public Metadata this[string key]
        {
            get => m_MetadataDictionary[key];
            set
            {
                if (ContainsKey(key))
                {
                    m_MetadataDictionary[key] = value;

                    var index = m_Metadata.FindIndex(metadata => metadata.Key == key);
                    if (index != -1)
                    {
                        m_Metadata[index] = value;
                    }
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <inheritdoc />
        public ICollection<string> Keys => m_MetadataDictionary.Keys;
        /// <inheritdoc />
        public ICollection<Metadata> Values => m_MetadataDictionary.Values;

        /// <summary>
        /// Adds a search term to the metadata filter.
        /// </summary>
        /// <param name="key">The key of the metdata field to add. This can be retrieved from the display name using the <see cref="AssetManagerClient.GetMetadataKeyFromDisplayNameAsync"/> method.</param>
        /// <param name="value">The metadata value to search for.</param>
        public void Add(string key, Metadata value)
        {
            m_MetadataDictionary.Add(key, value);

            value.Key = key;
            m_Metadata.Add(value);
        }

        /// <summary>
        /// Adds a string search term to the metadata filter.
        /// </summary>
        /// <param name="key">The key of the metdata field to add. This can be retrieved from the display name using the <see cref="AssetManagerClient.GetMetadataKeyFromDisplayNameAsync"/> method.</param>
        /// <param name="value">The string value to search for.</param>
        public void AddOrReplace(string key, string value) => this[key] = new StringMetadata(value) {Key = key};

        /// <summary>
        /// Adds a number search term to the metadata filter.
        /// </summary>
        /// <param name="key">The key of the metdata field to add. This can be retrieved from the display name using the <see cref="AssetManagerClient.GetMetadataKeyFromDisplayNameAsync"/> method.</param>
        /// <param name="value">The number value to search for.</param>
        public void AddOrReplace(string key, double value) => this[key] = new NumberMetadata(value) {Key = key};

        /// <summary>
        /// Adds a boolean search term to the metadata filter.
        /// </summary>
        /// <param name="key">The key of the metdata field to add. This can be retrieved from the display name using the <see cref="AssetManagerClient.GetMetadataKeyFromDisplayNameAsync"/> method.</param>
        /// <param name="value">The boolean value to search for.</param>
        public void AddOrReplace(string key, bool value) => this[key] = new BooleanMetadata(value) {Key = key};

        /// <summary>
        /// Adds a time stamp search term to the metadata filter.
        /// </summary>
        /// <param name="key">The key of the metdata field to add. This can be retrieved from the display name using the <see cref="AssetManagerClient.GetMetadataKeyFromDisplayNameAsync"/> method.</param>
        /// <param name="value">The timestamp value to search for.</param>
        public void AddOrReplace(string key, DateTime value) => this[key] = new DateTimeMetadata(value) {Key = key};

        /// <summary>
        /// Adds an array of string values to the metadata filter.
        /// </summary>
        /// <param name="key">The key of the metdata field to add. This can be retrieved from the display name using the <see cref="AssetManagerClient.GetMetadataKeyFromDisplayNameAsync"/> method.</param>
        /// <param name="values">The array of string values to search for.</param>
        public void AddOrReplace(string key, IEnumerable<string> values) => this[key] = new MultiValueMetadata(values) {Key = key};

        /// <inheritdoc />
        public void Clear()
        {
            m_MetadataDictionary.Clear();
            m_Metadata.Clear();
        }

        /// <inheritdoc />
        public bool ContainsKey(string key) => m_MetadataDictionary.ContainsKey(key);

        /// <inheritdoc />
        public bool Remove(string key)
        {
            if (m_MetadataDictionary.Remove(key))
            {
                m_Metadata.RemoveAll(metadata => metadata.Key == key);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool TryGetValue(string key, out Metadata value) => m_MetadataDictionary.TryGetValue(key, out value);

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, Metadata>> GetEnumerator() => m_MetadataDictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<string, Metadata>>.Add(KeyValuePair<string, Metadata> item)
        {
            (m_MetadataDictionary as ICollection<KeyValuePair<string, Metadata>>).Add(item);

            // Update the key of the item
            item.Value.Key = item.Key;
            m_Metadata.Add(item.Value);
        }

        bool ICollection<KeyValuePair<string, Metadata>>.Contains(KeyValuePair<string, Metadata> item) => (m_MetadataDictionary as ICollection<KeyValuePair<string, Metadata>>).Contains(item);

        void ICollection<KeyValuePair<string, Metadata>>.CopyTo(KeyValuePair<string, Metadata>[] array, int arrayIndex) => (m_MetadataDictionary as ICollection<KeyValuePair<string, Metadata>>).CopyTo(array, arrayIndex);

        bool ICollection<KeyValuePair<string, Metadata>>.Remove(KeyValuePair<string, Metadata> item) => (m_MetadataDictionary as ICollection<KeyValuePair<string, Metadata>>).Remove(item);

        bool ICollection<KeyValuePair<string, Metadata>>.IsReadOnly => (m_MetadataDictionary as ICollection<KeyValuePair<string, Metadata>>).IsReadOnly;

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
            m_Metadata?.RemoveAll(x => x == null || string.IsNullOrEmpty(x.Key));
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            m_Metadata?.RemoveAll(x => x == null || string.IsNullOrEmpty(x.Key));
            m_Metadata ??= new List<Metadata>();

            m_MetadataDictionary.Clear();
            foreach (var metadata in m_Metadata)
            {
                if (!m_MetadataDictionary.TryAdd(metadata.Key, metadata))
                {
                    Utilities.DevLogWarning($"[OnAfterDeserialize] Duplicate metadata key found: {metadata.Key}");
                }
            }
        }
    }
}
