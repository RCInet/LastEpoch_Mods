using System;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class AssetCollectionData : IAssetCollectionData
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public CollectionPath ParentPath { get; set; }

        internal AssetCollectionData() { }

        public AssetCollectionData(string name)
        {
            Name = name;
        }

        public AssetCollectionData(string name, string parentPath)
        {
            Name = name;
            ParentPath = new CollectionPath(parentPath);
        }

        public string GetFullCollectionPath()
        {

            return CollectionPath.CombinePaths(ParentPath, Name);
        }
    }
}
