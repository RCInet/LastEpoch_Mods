using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This is a base class containing the information necessary to update an asset.
    /// </summary>
    interface IAssetUpdate : IAssetInfo
    {
        /// <inheritdoc cref="IAsset.Type"/>
        AssetType? Type { get; }

        /// <inheritdoc cref="IAsset.PreviewFile"/>
        string PreviewFile { get; }
    }
}
