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
        public async IAsyncEnumerable<IFieldDefinitionData> ListFieldDefinitionsAsync(OrganizationId organizationId, PaginationData pagination, Dictionary<string, string> queryParameters, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 99;

            var (offset, length) = await pagination.Range.GetOffsetAndLengthAsync(_cancellationToken =>
            {
                _cancellationToken.ThrowIfCancellationRequested();
                return Task.FromResult(int.MaxValue);
            }, cancellationToken);
            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));
            var nextPageToken = string.Empty;

            var startIndex = offset % pageSize;
            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = new GetFieldDefinitionListRequest(organizationId, pageSize, pagination.SortingOrder, nextPageToken, queryParameters);
                var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                    cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var fieldDefinitionPage = IsolatedSerialization.DeserializeWithDefaultConverters<FieldDefinitionListDto>(jsonContent);

                nextPageToken = fieldDefinitionPage.NextPageToken;

                if (fieldDefinitionPage.FieldDefinitions == null || fieldDefinitionPage.FieldDefinitions.Length == 0) break;

                for (var i = 0; i < fieldDefinitionPage.FieldDefinitions.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (count == 0 && i < startIndex) continue;
                    if (count >= length) break;

                    ++count;
                    yield return fieldDefinitionPage.FieldDefinitions[i];
                }
            } while (count < length && !string.IsNullOrEmpty(nextPageToken));
        }

        /// <inheritdoc/>
        public async Task<IFieldDefinitionData> GetFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new FieldDefinitionRequest(fieldDefinitionDescriptor.OrganizationId, fieldDefinitionDescriptor.FieldKey);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonSerialization.Deserialize<FieldDefinitionData>(jsonContent);
        }

        /// <inheritdoc/>
        public async Task<FieldDefinitionDescriptor> CreateFieldDefinitionAsync(OrganizationId organizationId, IFieldDefinitionCreateData fieldCreation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateFieldDefinitionRequest(organizationId, fieldCreation);
            await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            return new FieldDefinitionDescriptor(organizationId, fieldCreation.Name);
        }

        /// <inheritdoc/>
        public Task DeleteFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken)
        {
            var request = new FieldDefinitionRequest(fieldDefinitionDescriptor.OrganizationId, fieldDefinitionDescriptor.FieldKey);
            return RateLimitedServiceClient(request, HttpMethod.Delete).DeleteAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc/>
        public Task UpdateFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, IFieldDefinitionBaseData fieldUpdate, CancellationToken cancellationToken)
        {
            var request = new FieldDefinitionRequest(fieldDefinitionDescriptor.OrganizationId, fieldDefinitionDescriptor.FieldKey, fieldUpdate);
            return RateLimitedServiceClient(request, HttpClientExtensions.HttpMethodPatch).PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        public Task AddAcceptedValuesToFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, IEnumerable<string> acceptedValues, CancellationToken cancellationToken)
        {
            var request = new ModifyFieldDefinitionSelectionRequest(fieldDefinitionDescriptor.OrganizationId, fieldDefinitionDescriptor.FieldKey, acceptedValues);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        public Task RemoveAcceptedValuesFromFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, IEnumerable<string> acceptedValues, CancellationToken cancellationToken)
        {
            var request = new ModifyFieldDefinitionSelectionRequest(fieldDefinitionDescriptor.OrganizationId, fieldDefinitionDescriptor.FieldKey, acceptedValues);
            return RateLimitedServiceClient(request, HttpMethod.Delete).DeleteAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);
        }
    }
}
