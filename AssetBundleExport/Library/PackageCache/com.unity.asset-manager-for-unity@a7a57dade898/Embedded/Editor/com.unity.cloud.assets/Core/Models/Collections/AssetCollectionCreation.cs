using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This object contains the information about an asset collection.
    /// </summary>
    class AssetCollectionCreation : IAssetCollectionCreation
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public CollectionPath ParentPath { get; set; }

        public AssetCollectionCreation(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
