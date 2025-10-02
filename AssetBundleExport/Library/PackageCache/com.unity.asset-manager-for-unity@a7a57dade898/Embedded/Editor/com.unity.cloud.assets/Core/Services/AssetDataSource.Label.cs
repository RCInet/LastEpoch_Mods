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
        /// <inheritdoc/>
        public async IAsyncEnumerable<ILabelData> ListLabelsAsync(OrganizationId organizationId, PaginationData pagination, bool? archived, bool? systemLabels, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 1000;

            var countRequest = new GetLabelListRequest(organizationId, 0, 1, archived, systemLabels);
            var (offset, length) = await pagination.Range.GetOffsetAndLengthAsync(token => GetTotalCount(countRequest, token), cancellationToken);
            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));

            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = new GetLabelListRequest(organizationId, offset, pageSize, archived, systemLabels);
                var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var pageDto = IsolatedSerialization.DeserializeWithDefaultConverters<LabelListDto>(jsonContent);

                if (pageDto.Labels == null || pageDto.Labels.Length == 0) break;

                for (var i = 0; i < pageDto.Labels.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (count >= length) break;

                    ++count;
                    yield return pageDto.Labels[i];
                }

                // Cap the length to the total number of entries.
                length = Math.Min(length, pageDto.Total);
                // Update the offset and page size for the next iteration
                offset += pageSize;
                pageSize = Math.Min(pageSize, length - offset);
            } while (count < length);
        }

        /// <inheritdoc/>
        public async Task<ILabelData> GetLabelAsync(LabelDescriptor labelDescriptor, CancellationToken cancellationToken)
        {
            // Not yet implemented in backend, we need to pass through search all API
            /*
            cancellationToken.ThrowIfCancellationRequested();

            var request = new LabelRequest(labelDescriptor.OrganizationId, labelDescriptor.LabelName);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonSerialization.Deserialize<LabelData>(jsonContent);
            */

            var results = ListLabelsAsync(labelDescriptor.OrganizationId, new PaginationData {Range = Range.All}, null, null, cancellationToken);
            await foreach (var result in results.WithCancellation(cancellationToken))
            {
                if (result.Name == labelDescriptor.LabelName)
                {
                    return result;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<LabelDescriptor> CreateLabelAsync(OrganizationId organizationId, ILabelBaseData labelCreation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateLabelRequest(organizationId, labelCreation);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var createdLabel = JsonSerialization.Deserialize<CreatedLabelDto>(jsonContent);
            if (createdLabel.Name != Uri.EscapeDataString(labelCreation.Name))
            {
                k_Logger.LogWarning($"The created label name '{createdLabel.Name}' does not match the requested label name '{labelCreation.Name}' when URL escaped as '{Uri.EscapeDataString(labelCreation.Name)}'.");
            }

            return new LabelDescriptor(organizationId, createdLabel.Name);
        }

        /// <inheritdoc/>
        public Task UpdateLabelAsync(LabelDescriptor labelDescriptor, ILabelBaseData labelUpdate, CancellationToken cancellationToken)
        {
            var request = new LabelRequest(labelDescriptor.OrganizationId, labelDescriptor.LabelName, labelUpdate);
            return RateLimitedServiceClient(request, "PATCH").PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc/>
        public Task UpdateLabelStatusAsync(LabelDescriptor labelDescriptor, bool archive, CancellationToken cancellationToken)
        {
            var request = new UpdateLabelStatusRequest(labelDescriptor.OrganizationId, labelDescriptor.LabelName, archive);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<AssetLabelsDto> ListLabelsAcrossAssetVersions(ProjectDescriptor projectDescriptor, AssetId assetId, PaginationData pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 1000;

            var countRequest = new AssetLabelRequest(projectDescriptor.ProjectId, assetId, 0, 1);
            var (offset, length) = await pagination.Range.GetOffsetAndLengthAsync(token => GetTotalCount(countRequest, token), cancellationToken);
            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));

            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = new AssetLabelRequest(projectDescriptor.ProjectId, assetId, offset, pageSize);
                var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var pageDto = IsolatedSerialization.DeserializeWithDefaultConverters<AssetLabelListDto>(jsonContent);

                if (pageDto.AssetLabels == null || pageDto.AssetLabels.Length == 0) break;

                for (var i = 0; i < pageDto.AssetLabels.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (count >= length) break;

                    ++count;
                    yield return pageDto.AssetLabels[i];
                }

                // Cap the length to the total number of entries.
                length = Math.Min(length, pageDto.Total);
                // Update the offset and page size for the next iteration
                offset += pageSize;
                pageSize = Math.Min(pageSize, length - offset);
            } while (count < length);
        }

        /// <inheritdoc/>
        public Task AssignLabelsAsync(AssetDescriptor assetDescriptor, IEnumerable<string> labels, CancellationToken cancellationToken)
        {
            var request = new AssignLabelRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, true, labels);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc/>
        public Task UnassignLabelsAsync(AssetDescriptor assetDescriptor, IEnumerable<string> labels, CancellationToken cancellationToken)
        {
            var request = new AssignLabelRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, false, labels);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        async Task<int> GetTotalCount(ApiRequest apiRequest, CancellationToken cancellationToken)
        {
            var response = await RateLimitedServiceClient(apiRequest, HttpMethod.Get).GetAsync(GetPublicRequestUri(apiRequest), ServiceHttpClientOptions.Default(), cancellationToken);
            var jsonContent = await response.GetContentAsString();
            var pageDto = IsolatedSerialization.Deserialize<PaginationDto>(jsonContent, IsolatedSerialization.defaultSettings);
            return pageDto.Total;
        }
    }
}
