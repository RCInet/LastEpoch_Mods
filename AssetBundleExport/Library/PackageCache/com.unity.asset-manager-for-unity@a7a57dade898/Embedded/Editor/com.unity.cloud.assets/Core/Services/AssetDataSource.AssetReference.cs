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
        public async IAsyncEnumerable<IAssetReferenceData> ListAssetReferencesAsync(ProjectDescriptor projectDescriptor, AssetId assetId, AssetVersion? assetVersion, string context, Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            const int maxPageSize = 254;

            var countRequest = new AssetReferenceRequest(projectDescriptor.ProjectId, assetId, assetVersion, context, 0, 1);
            var (offset, length) = await range.GetOffsetAndLengthAsync(token => GetTotalCount(countRequest, token), cancellationToken);
            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));

            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = new AssetReferenceRequest(projectDescriptor.ProjectId, assetId, assetVersion, context, offset, pageSize);
                var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var pageDto = IsolatedSerialization.DeserializeWithDefaultConverters<AssetReferenceListDto>(jsonContent);

                if (pageDto.AssetReferences == null || pageDto.AssetReferences.Length == 0) break;

                for (var i = 0; i < pageDto.AssetReferences.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (count >= length) break;

                    ++count;
                    yield return pageDto.AssetReferences[i];
                }

                // Cap the length to the total number of entries.
                length = Math.Min(length, pageDto.Total);
                // Update the offset and page size for the next iteration
                offset += pageSize;
                pageSize = Math.Min(pageSize, length - offset);
            } while (count < length);
        }

        public async Task<string> CreateAssetReferenceAsync(AssetDescriptor assetDescriptor, AssetIdentifierDto assetIdentifierDto, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestBody = new CreateAssetReferenceRequestBody
            {
                AssetVersion = assetDescriptor.AssetVersion.ToString(),
                Target = assetIdentifierDto
            };
            var request = new AssetReferenceRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, requestBody);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var dto = DeserializeCollectionPath<CreateAssetReferenceResponseBody>(jsonContent);

            return dto.ReferenceId;
        }

        public Task DeleteAssetReferenceAsync(ProjectDescriptor projectDescriptor, AssetId assetId, string referenceId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AssetReferenceRequest(projectDescriptor.ProjectId, assetId, referenceId);
            return RateLimitedServiceClient(request, HttpMethod.Delete).DeleteAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);
        }
    }
}
