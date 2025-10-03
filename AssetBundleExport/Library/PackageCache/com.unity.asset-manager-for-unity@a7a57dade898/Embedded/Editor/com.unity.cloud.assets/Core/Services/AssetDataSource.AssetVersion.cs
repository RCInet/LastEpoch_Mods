using System;
using System.Collections.Generic;
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
        public async IAsyncEnumerable<IAssetData> ListAssetVersionsAsync(ProjectDescriptor projectDescriptor, AssetId assetId, SearchRequestParameters parameters, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (offset, length) = await parameters.PaginationRange.GetOffsetAndLengthAsync(_ => Task.FromResult(int.MaxValue), cancellationToken);
            if (length == 0) yield break;

            var request = new SearchAssetVersionRequest(projectDescriptor.ProjectId, assetId, parameters);
            var results = ListAssetsAsync(request, parameters, offset, length, cancellationToken);
            await foreach (var asset in results)
            {
                yield return asset;
            }
        }

        /// <inheritdoc />
        public async Task<AssetVersion> CreateUnfrozenAssetVersionAsync(AssetDescriptor parentAssetDescriptor, string statusFlowId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateAssetVersionRequest(parentAssetDescriptor.ProjectId, parentAssetDescriptor.AssetId, parentAssetDescriptor.AssetVersion, statusFlowId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var dto = IsolatedSerialization.Deserialize<AssetVersionDto>(jsonContent, IsolatedSerialization.defaultSettings);
            return new AssetVersion(dto.Version);
        }
        
        /// <inheritdoc />
        public Task DeleteUnfrozenAssetVersionAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AssetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion);
            return RateLimitedServiceClient(request, HttpMethod.Delete).DeleteAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int?> FreezeAssetVersionAsync(AssetDescriptor assetDescriptor, string changeLog, bool? forceFreeze, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            HttpResponseMessage response;

            if (forceFreeze.HasValue)
            {
                var request = new SubmitVersionRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, changeLog, forceFreeze);
                response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                    ServiceHttpClientOptions.Default(), cancellationToken);
            }
            else
            {
                response = await AutoFreezeAssetVersionAsync(assetDescriptor, changeLog, cancellationToken);
            }

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var dto = IsolatedSerialization.Deserialize<VersionNumberDto>(jsonContent, IsolatedSerialization.defaultSettings);
            return dto.VersionNumber;
        }

        Task<HttpResponseMessage> AutoFreezeAssetVersionAsync(AssetDescriptor assetDescriptor, string changelog, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AutoSubmitAssetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, changelog);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task CancelFreezeAssetVersionAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = AutoSubmitAssetRequest.GetDisableRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }
    }
}
