using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The result of a group and count operation.
    /// </summary>
    class GroupableFieldValue
    {
        readonly object m_Value;

        /// <summary>
        /// The type of the value.
        /// </summary>
        public GroupableFieldValueType Type { get; }

        internal GroupableFieldValue(GroupableFieldValueType type, object value)
        {
            Type = type;
            m_Value = value;
        }

        /// <summary>
        /// Returns a string representation of the value.
        /// </summary>
        /// <returns>A string. </returns>
        public string AsString()
        {
            return Type switch
            {
                GroupableFieldValueType.AssetType => ((AssetType) m_Value).GetValueAsString(),
                GroupableFieldValueType.CollectionDescriptor => ((CollectionDescriptor) m_Value).Path,
                _ => m_Value.ToString()
            };
        }

        /// <summary>
        /// Returns the value as a user ID.
        /// </summary>
        /// <returns>A <see cref="UserId"/>. </returns>
        public UserId AsUserId() => (UserId) m_Value;

        /// <summary>
        /// Returns the value as an asset ID.
        /// </summary>
        /// <returns>An <see cref="AssetId"/>. </returns>
        public AssetId AsAssetId() => (AssetId) m_Value;

        /// <summary>
        /// Returns the value as an asset version.
        /// </summary>
        /// <returns>An <see cref="AssetVersion"/>. </returns>
        public AssetVersion AsAssetVersion() => (AssetVersion) m_Value;

        /// <summary>
        /// Returns the value as a dataset ID.
        /// </summary>
        /// <returns>A <see cref="DatasetId"/>. </returns>
        public DatasetId AsDatasetId() => (DatasetId) m_Value;

        /// <summary>
        /// Returns the value as a collection descriptor.
        /// </summary>
        /// <returns>A <see cref="CollectionDescriptor"/>. </returns>
        public CollectionDescriptor AsCollectionDescriptor() => (CollectionDescriptor) m_Value;

        /// <summary>
        /// Returns the value as an asset type.
        /// </summary>
        /// <returns>An <see cref="AssetType"/>. </returns>
        public AssetType AsAssetType() => (AssetType) m_Value;

        /// <summary>
        /// Returns the value as a metadata value.
        /// </summary>
        /// <returns>A <see cref="MetadataValue"/>. </returns>
        public MetadataValue AsMetadataValue() => (MetadataValue) m_Value;

        public override string ToString() => m_Value.ToString();
    }
}
