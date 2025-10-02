using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial interface IAssetDataSource
    {
        IAsyncEnumerable<IAssetReferenceData> ListAssetReferencesAsync(ProjectDescriptor projectDescriptor, AssetId assetId, AssetVersion? assetVersion, string context, Range range, CancellationToken cancellationToken);
        Task<string> CreateAssetReferenceAsync(AssetDescriptor assetDescriptor, AssetIdentifierDto assetIdentifierDto, CancellationToken cancellationToken);
        Task DeleteAssetReferenceAsync(ProjectDescriptor projectDescriptor, AssetId assetId, string referenceId, CancellationToken cancellationToken);
    }
}
