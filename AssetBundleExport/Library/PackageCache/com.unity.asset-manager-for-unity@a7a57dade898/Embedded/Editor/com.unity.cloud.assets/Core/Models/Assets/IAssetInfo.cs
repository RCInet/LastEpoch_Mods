using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IAssetInfo
    {
        /// <inheritdoc cref="IAsset.Name"/>
        string Name { get; }

        /// <inheritdoc cref="IAsset.Description"/>
        string Description { get; }

        /// <inheritdoc cref="IAsset.Tags"/>
        List<string> Tags { get; }

        /// <summary>
        /// The status flow of the asset.
        /// </summary>
        StatusFlowDescriptor? StatusFlowDescriptor => null;
    }
}
