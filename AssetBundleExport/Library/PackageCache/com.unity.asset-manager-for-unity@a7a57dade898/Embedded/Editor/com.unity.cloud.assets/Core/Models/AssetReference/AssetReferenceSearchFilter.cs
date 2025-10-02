using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that defines search criteria for an <see cref="IAssetReference"/> query.
    /// </summary>
    sealed class AssetReferenceSearchFilter
    {
        public enum Context
        {
            Both = 0,
            Source,
            Target
        }

        /// <summary>
        /// Whether the results should be filtered for a specific asset version.
        /// </summary>
        public QueryParameter<AssetVersion> AssetVersion { get; } = new(CommonEmbedded.AssetVersion.None);

        /// <summary>
        /// Whether the results should be for references where the asset is a source, target, or both.
        /// </summary>
        public QueryParameter<Context> ReferenceContext { get; } = new();
    }
}
