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
        public async Task<DatasetDescriptor> CreateDatasetAsync(AssetDescriptor assetDescriptor, IDatasetBaseData datasetCreation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateDatasetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, datasetCreation);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var createdDatasetResponse = IsolatedSerialization.DeserializeWithConverters<CreatedDatasetDto>(jsonContent, IsolatedSerialization.DatasetIdConverter);

            return new DatasetDescriptor(assetDescriptor, createdDatasetResponse.DatasetId);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IDatasetData> ListDatasetsAsync(AssetDescriptor assetDescriptor, Range range, FieldsFilter includedFieldsFilter, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var countRequest = new DatasetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, FieldsFilter.None, limit: 1);

            Func<string, int, ApiRequest> getListRequest = (next, pageSize) => new DatasetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, includedFieldsFilter, next, pageSize);

            await foreach (var data in ListEntitiesAsync<DatasetData>(countRequest, getListRequest, range, cancellationToken))
            {
                yield return data;
            }
        }

        async IAsyncEnumerable<T> ListEntitiesAsync<T>(ApiRequest countRequest, Func<string, int, ApiRequest> getListRequest, Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 1000;

            cancellationToken.ThrowIfCancellationRequested();

            var (offset, length) = await range.GetOffsetAndLengthAsync(token => GetTotalCount(countRequest, token), cancellationToken);

            if (length == 0) yield break;

            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));

            string next = null;

            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = getListRequest(next, pageSize);
                var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var pageDto = IsolatedSerialization.DeserializeWithDefaultConverters<EntityPageDto<T>>(jsonContent);

                if (pageDto.Results == null || pageDto.Results.Length == 0) break;

                // Cap the length to the total number of results.
                length = Math.Min(length, pageDto.Total);

                // Update the next token.
                next = pageDto.Next;

                for (var i = 0; i < pageDto.Results.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Bring offset to 0 before starting to yield results.
                    if (offset-- > 0) continue;

                    // Stop yielding results if we have reached the desired count.
                    if (count >= length) break;

                    ++count;
                    yield return pageDto.Results[i];
                }
            } while (count < length && !string.IsNullOrEmpty(next));
        }

        /// <inheritdoc />
        public async Task<IDatasetData> GetDatasetAsync(DatasetDescriptor datasetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new DatasetRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId, includedFieldsFilter);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<DatasetData>(jsonContent);
        }

        /// <inheritdoc />
        public Task UpdateDatasetAsync(DatasetDescriptor datasetDescriptor, IDatasetUpdateData datasetUpdate, CancellationToken cancellationToken)
        {
            var request = new DatasetRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId, datasetUpdate);
            return RateLimitedServiceClient(request, HttpClientExtensions.HttpMethodPatch).PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task ReferenceFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, DatasetId sourceDatasetId, string filePath, CancellationToken cancellationToken)
        {
            var request = new AddFileReferenceRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, sourceDatasetId, filePath, datasetDescriptor.DatasetId);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        public Task RemoveFileFromDatasetAsync(DatasetDescriptor datasetDescriptor, string filePath, CancellationToken cancellationToken)
        {
            var request = new FileRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId, filePath);
            return RateLimitedServiceClient(request, HttpMethod.Delete).DeleteAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> CheckDatasetIsInProjectAssetVersionAsync(DatasetDescriptor datasetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CheckDatasetBelongsToAssetRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var dto = JsonSerialization.Deserialize<DatasetAssetCheckDto>(jsonContent);

            return !string.IsNullOrEmpty(dto.DatasetVersionId);
        }

        /// <inheritdoc />
        public Task RemoveDatasetMetadataAsync(DatasetDescriptor datasetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var request = new RemoveMetadataRequest(datasetDescriptor.ProjectId,
                datasetDescriptor.AssetId,
                datasetDescriptor.AssetVersion,
                datasetDescriptor.DatasetId,
                metadataType,
                keys);
            return RateLimitedServiceClient(request, HttpMethod.Delete).DeleteAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }
    }
}
