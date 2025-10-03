using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    class AssetCreation : AssetInfo, IAssetCreation
    {
        /// <inheritdoc/>
        public AssetType Type { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, MetadataValue> Metadata { get; set; }

        /// <inheritdoc/>
        public List<CollectionPath> Collections { get; set; }

        /// <inheritdoc/>
        public StatusFlowDescriptor? StatusFlowDescriptor { get; set; }

        public AssetCreation(string name)
        {
            Name = name;
        }
    }
}
