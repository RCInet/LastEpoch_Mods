using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This is a base class containing the information necessary to update an asset.
    /// </summary>
    class AssetUpdate : AssetInfo, IAssetUpdate
    {
        /// <inheritdoc/>
        public AssetType? Type { get; set; }

        /// <inheritdoc/>
        public string PreviewFile { get; set; }

        /// <inheritdoc/>
        public StatusFlowDescriptor? StatusFlowDescriptor { get; set; }

        public AssetUpdate() { }

        [Obsolete("Use the default constructor instead.")]
        public AssetUpdate(IAsset asset)
        {
            Name = asset.Name;
            Description = asset.Description;
            Tags = asset.Tags?.ToList() ?? new List<string>();
            Type = asset.Type;
            PreviewFile = asset.PreviewFileDescriptor.Path;
        }
    }
}
