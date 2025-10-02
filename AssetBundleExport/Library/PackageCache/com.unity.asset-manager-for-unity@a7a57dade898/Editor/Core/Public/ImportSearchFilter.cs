using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Editor
{
    /// <summary>
    /// A filter for selecting assets to import.
    /// </summary>
    /// <remarks>
    /// The values within each enumerable property will be combined by "OR" logic, meaning that if any of the values in the list match, the asset will be included in the search results.
    /// The properties will be combined by "AND" logic, meaning that there must exist a match in each populated property for an asset to be included in the search results.
    /// </remarks>
    [Serializable]
    public class ImportSearchFilter
    {
        [SerializeField]
        List<string> m_ProjectIds;
        [SerializeField]
        List<string> m_Collections;
        [SerializeField]
        List<string> m_AssetIds;
        [SerializeField]
        List<AssetType> m_AssetTypes;
        [SerializeField]
        List<string> m_Tags;
        [SerializeField]
        List<string> m_Statuses;
        [SerializeField]
        List<string> m_Labels;
        [SerializeField]
        List<string> m_CreatedByIds;
        [SerializeField]
        List<string> m_UpdatedByIds;
        [SerializeReference]
        MetadataSearchFilter m_CustomMetadata;

        /// <summary>
        /// Searches for assets in the specified projects; if left empty, the search will be performed in all projects.
        /// </summary>
        /// <example>
        /// Expects a Guid: 00aa0aa0-a0aa-0a0a-a0a0-a0a0aa0a0a0a
        /// </example>
        public IEnumerable<string> ProjectIds
        {
            get => m_ProjectIds;
            set => m_ProjectIds = value?.ToList();
        }

        /// <summary>
        /// Searches for assets in the specified collections. You must provide the full path to the collection including parent collections.
        /// </summary>
        /// <remarks>
        /// Searching in collections can only be performed if exactly 1 project is added to the <see cref="ProjectIds"/> property.
        /// </remarks>
        /// <example>
        /// For a collection without parent, use the collection name only: My Collection
        /// For a nested collection, provide the full path: Parent Collection/Child Collection/My Collection
        /// </example>
        public IEnumerable<string> Collections
        {
            get => m_Collections;
            set => m_Collections = value?.ToList();
        }

        /// <summary>
        /// Searches for all assets with the specified IDs.
        /// </summary>
        /// <example>
        /// 669a8e0594386f320df8e315
        /// </example>
        public IEnumerable<string> AssetIds
        {
            get => m_AssetIds;
            set => m_AssetIds = value?.ToList();
        }

        /// <summary>
        /// Searches for assets with any of the specified types.
        /// </summary>
        public IEnumerable<AssetType> AssetTypes
        {
            get => m_AssetTypes;
            set => m_AssetTypes = value?.ToList();
        }

        /// <summary>
        /// Searches for assets with any of the specified tags.
        /// </summary>
        /// <example>
        /// Prefab, Material, Texture2D, Texture, etc.
        /// </example>
        public IEnumerable<string> Tags
        {
            get => m_Tags;
            set => m_Tags = value?.ToList();
        }

        /// <summary>
        /// Searches for assets with any of the specified statuses.
        /// </summary>
        /// <example>
        /// Draft, In review, Approved, Rejected, Published, Withdrawn, etc.
        /// </example>
        public IEnumerable<string> Statuses
        {
            get => m_Statuses;
            set => m_Statuses = value?.ToList();
        }

        /// <summary>
        /// Searches for assets with any of the specified version labels.
        /// </summary>
        /// <example>
        /// Latest, Pending, etc.
        /// </example>
        public IEnumerable<string> Labels
        {
            get => m_Labels;
            set => m_Labels = value?.ToList();
        }

        /// <summary>
        /// Searches for assets created by any of the specified users, by their IDs.
        /// </summary>
        /// <remarks>
        /// To convert a user name to an id, use <see cref="AssetManagerClient.GetUserIdsFromUserNameAsync"/> when user ids are unknown.
        /// </remarks>
        /// <example>
        /// 9876543211234
        /// </example>
        public IEnumerable<string> CreatedByUserIds
        {
            get => m_CreatedByIds;
            set => m_CreatedByIds = value?.ToList();
        }

        /// <summary>
        /// Searches for assets updated by any of the specified users, by their IDs.
        /// </summary>
        /// <remarks>
        /// To convert a user name to an id, use <see cref="AssetManagerClient.GetUserIdsFromUserNameAsync"/> when user ids are unknown.
        /// </remarks>
        /// <example>
        /// 9876543211234
        /// </example>
        public IEnumerable<string> UpdatedByUserIds
        {
            get => m_UpdatedByIds;
            set => m_UpdatedByIds = value?.ToList();
        }

        /// <summary>
        /// Searches for assets with any of the specified custom metadata fields,
        /// where the key represents the field definition key and the value is a searchable field value.
        /// Keys can be converted from display names using the <see cref="AssetManagerClient.GetMetadataKeyFromDisplayNameAsync"/> method.
        /// </summary>
        public MetadataSearchFilter CustomMetadata
        {
            get => m_CustomMetadata ??= new MetadataSearchFilter();
            set => m_CustomMetadata = value ?? new MetadataSearchFilter();
        }

        internal bool IsEmpty()
        {
            return IsNullOrEmpty(m_ProjectIds)
                && IsNullOrEmpty(m_Collections)
                && IsNullOrEmpty(m_AssetIds)
                && IsNullOrEmpty(m_AssetTypes)
                && IsNullOrEmpty(m_Tags)
                && IsNullOrEmpty(m_Statuses)
                && IsNullOrEmpty(m_Labels)
                && IsNullOrEmpty(m_CreatedByIds)
                && IsNullOrEmpty(m_UpdatedByIds)
                && CustomMetadata.Count == 0;
        }

        static bool IsNullOrEmpty<T>(IEnumerable<T> enumerable) => enumerable == null || !enumerable.Any();
    }
}
