using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial class AssetDataSource
    {
        /// <inheritdoc />
        public async IAsyncEnumerable<IStatusFlowData> ListStatusFlowsAsync(OrganizationId organizationId, PaginationData paginationData, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 99;

            var (offset, length) = await paginationData.Range.GetOffsetAndLengthAsync(token => GetTotalCount(new StatusRequest(organizationId, 0, 1), token), cancellationToken);

            if (length == 0) yield break;

            var pageSize = Math.Min(maxPageSize, length);

            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = new StatusRequest(organizationId, offset, pageSize);
                var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                    cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var statusFlowPageDto = IsolatedSerialization.Deserialize<StatusFlowPageDto>(jsonContent, IsolatedSerialization.defaultSettings);

                if (statusFlowPageDto.StatusFlows == null || statusFlowPageDto.StatusFlows.Length == 0) break;

                for (var i = 0; i < statusFlowPageDto.StatusFlows.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (count >= length) break; // Unlikely we should ever break here as we are capping the page size at every iteration

                    ++count;
                    yield return statusFlowPageDto.StatusFlows[i];
                }

                // We have reached the maximum number of items to return
                if (count >= statusFlowPageDto.Total) break;

                // Increment the offset
                offset += statusFlowPageDto.StatusFlows.Length;

                // Cap the page size to the remaining number of items
                pageSize = Math.Min(maxPageSize, length - count);
            } while (count < length);
        }

        /// <inheritdoc />
        public async Task<(StatusFlowDescriptor, IStatusData)> GetAssetStatusAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var dto = await GetAssetStatusDtoAsync(assetDescriptor, cancellationToken);

            var statusFlowDescriptor = new StatusFlowDescriptor(assetDescriptor.OrganizationId, dto.StatusFlow.Id);
            var status = dto.StatusFlow.Statuses.FirstOrDefault(x => x.Id == dto.CurrentStatusId);

            return (statusFlowDescriptor, status);
        }

        /// <inheritdoc />
        public async Task<(StatusFlowDescriptor, IStatusData[])> GetReachableStatusesAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = AssetStatusRequest.GetReachableStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var dto = IsolatedSerialization.Deserialize<ReachableStatusesDto>(jsonContent, IsolatedSerialization.defaultSettings);

            var statusFlowDescriptor = new StatusFlowDescriptor(assetDescriptor.OrganizationId, dto.StatusFlowId);
            var assetStatusDto = await GetAssetStatusDtoAsync(assetDescriptor, cancellationToken);
            var statuses = assetStatusDto.StatusFlow.Statuses
                .Where(x => dto.ReachableStatusNames.Contains(x.Name))
                .ToArray();

            return (statusFlowDescriptor, statuses);
        }

        /// <inheritdoc />
        public async Task<string[]> GetReachableStatusNamesAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = AssetStatusRequest.GetReachableStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var dto = IsolatedSerialization.Deserialize<ReachableStatusesDto>(jsonContent, IsolatedSerialization.defaultSettings);

            return dto.ReachableStatusNames;
        }

        /// <inheritdoc />
        public Task UpdateAssetStatusFlowAsync(AssetDescriptor assetDescriptor, StatusFlowDescriptor statusFlowDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AssignAssetStatusFlowRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, statusFlowDescriptor.StatusFlowId);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        async Task<AssetStatusDto> GetAssetStatusDtoAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = AssetStatusRequest.GetCurrentStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.Deserialize<AssetStatusDto>(jsonContent, IsolatedSerialization.defaultSettings);
        }
    }
}
