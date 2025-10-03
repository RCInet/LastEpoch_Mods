using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The properties of an <see cref="IAssetCollection"/>.
    /// </summary>
    struct AssetCollectionProperties
    {
        /// <summary>
        /// Describes the collection.
        /// </summary>
        public string Description { get; internal set; }
    }
}
