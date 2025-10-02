using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial interface IAssetDataSource
    {
        IAsyncEnumerable<IStatusFlowData> ListStatusFlowsAsync(OrganizationId organizationId, PaginationData paginationData, CancellationToken cancellationToken);

        Task<(StatusFlowDescriptor, IStatusData)> GetAssetStatusAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken);

        Task<(StatusFlowDescriptor, IStatusData[])> GetReachableStatusesAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken);

        Task<string[]> GetReachableStatusNamesAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken);

        Task UpdateAssetStatusFlowAsync(AssetDescriptor assetDescriptor, StatusFlowDescriptor statusFlowDescriptor, CancellationToken cancellationToken);
    }
}
