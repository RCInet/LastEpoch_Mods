using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    abstract class AssetInfo : IAssetInfo
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public List<string> Tags { get; set; }
    }
}
