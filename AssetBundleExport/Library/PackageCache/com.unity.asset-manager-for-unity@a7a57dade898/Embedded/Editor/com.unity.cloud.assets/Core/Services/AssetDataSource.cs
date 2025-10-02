using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial class AssetDataSource : IAssetDataSource
    {
        const int k_QueueLimit = 100000;
        const int k_DefaultTokensPerPeriod = 30;
        const int k_DefaultTokenLimit = 30;
        const int k_SlowTokensPerPeriod = 5;
        const int k_SlowTokenLimit = 5;
        const double k_ReplenishmentPeriod = 0.45; // we add 0.05s to each period to have a safety margin
        const double k_SlowReplenishmentPeriod = 1;
        const string k_PublicApiPath = "/assets/v1";

        static readonly UCLogger k_Logger = LoggerProvider.GetLogger<AssetDataSource>();

        readonly IServiceHttpClient m_ServiceHttpClient;
        readonly IServiceHostResolver m_PublicServiceHostResolver;
        readonly Dictionary<string, IServiceHttpClient> m_HttpClients = new();
        readonly object m_Lock = new();

        internal AssetDataSource(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            if (serviceHostResolver is ServiceHostResolver unityServiceHostResolver &&
                unityServiceHostResolver.GetResolvedEnvironment() == ServiceEnvironment.Test)
            {
                var headers = new Dictionary<string, string>
                {
                    {"x-backend-host", "https://api.fd.amc.test.transformation.unity.com"}
                };
                serviceHttpClient = new ServiceHttpClientHeaderModifier(serviceHttpClient, headers);
            }

            m_ServiceHttpClient = serviceHttpClient;
            m_PublicServiceHostResolver = serviceHostResolver;
        }

        string GetPublicRequestUri(ApiRequest request)
        {
            return m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(k_PublicApiPath));
        }

        /// <inheritdoc/>
        public async Task<IAssetData> GetAssetAsync(AssetDescriptor assetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AssetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, includedFieldsFilter);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<AssetData>(jsonContent);
        }

        /// <inheritdoc/>
        public async Task<IAssetData> GetAssetAsync(ProjectDescriptor projectDescriptor, AssetId assetId, string label, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AssetRequest(projectDescriptor.ProjectId, assetId, label, includedFieldsFilter);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<AssetData>(jsonContent);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(ProjectDescriptor projectDescriptor, SearchRequestParameters parameters, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (offset, length) = await parameters.PaginationRange.GetOffsetAndLengthAsync(token => GetAssetCountAsync(projectDescriptor, token), cancellationToken);
            if (length == 0) yield break;

            var request = new SearchRequest(projectDescriptor.ProjectId, parameters);
            var results = ListAssetsAsync(request, parameters, offset, length, cancellationToken);
            await foreach (var asset in results)
            {
                yield return asset;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, SearchRequestParameters parameters, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (offset, length) = await parameters.PaginationRange.GetOffsetAndLengthAsync(token => GetAcrossProjectsTotalCount(organizationId, projectIds, token), cancellationToken);
            if (length == 0) yield break;

            var request = new AcrossProjectsSearchRequest(organizationId, parameters);
            var results = ListAssetsAsync(request, parameters, offset, length, cancellationToken);
            await foreach (var asset in results)
            {
                yield return asset;
            }
        }

        /// <inheritdoc />
        public async Task<AggregateDto[]> GetAssetAggregateAsync(ProjectDescriptor projectDescriptor, SearchAndAggregateRequestParameters parameters, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new SearchAndAggregateRequest(projectDescriptor.ProjectId, parameters);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonSerialization.Deserialize<AggregationsDto>(jsonContent).Aggregations;
        }

        /// <inheritdoc />
        public async Task<AggregateDto[]> GetAssetAggregateAsync(OrganizationId organizationId, AcrossProjectsSearchAndAggregateRequestParameters parameters, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AcrossProjectsSearchAndAggregateRequest(organizationId, parameters);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonSerialization.Deserialize<AggregationsDto>(jsonContent).Aggregations;
        }

        /// <inheritdoc />
        public async Task<AssetDescriptor> CreateAssetAsync(ProjectDescriptor projectDescriptor, IAssetCreateData assetCreation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateAssetRequest(projectDescriptor.ProjectId, assetCreation);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var createdAsset = IsolatedSerialization.DeserializeWithDefaultConverters<CreatedAssetDto>(jsonContent);

            return new AssetDescriptor(projectDescriptor, createdAsset.AssetId, createdAsset.AssetVersion);
        }

        /// <inheritdoc />
        public Task UpdateAssetAsync(AssetDescriptor assetDescriptor, IAssetUpdateData data, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AssetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, data);
            return RateLimitedServiceClient(request, HttpClientExtensions.HttpMethodPatch).PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AssetDownloadUrl>> GetAssetDownloadUrlsAsync(AssetDescriptor assetDescriptor, DatasetId[] datasetIds, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GetAssetDownloadUrlsRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, datasetIds, null);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var assetDownloadUrlsDto = JsonSerialization.Deserialize<AssetDownloadUrlsDto>(jsonContent);

            var urlList = assetDownloadUrlsDto.FileUrls.Select(f => new AssetDownloadUrl
            {
                FilePath = f.Path,
                DownloadUrl = GetEscapedUri(f.Url)
            }).ToList();

            return urlList;
        }

        /// <inheritdoc />
        public Task LinkAssetToProjectAsync(AssetDescriptor assetDescriptor, ProjectDescriptor destinationProject, CancellationToken cancellationToken)
        {
            var request = new LinkAssetToProjectRequest(assetDescriptor.ProjectId, destinationProject.ProjectId, assetDescriptor.AssetId);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task UnlinkAssetFromProjectAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new UnlinkAssetFromProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> CheckIsProjectAssetSourceAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CheckProjectIsAssetSourceProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            return bool.Parse(await response.GetContentAsString());
        }

        /// <inheritdoc />
        public async Task<bool> CheckAssetBelongsToProjectAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CheckAssetBelongsToProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            return bool.Parse(await response.GetContentAsString());
        }

        /// <inheritdoc />
        public Task UpdateAssetStatusAsync(AssetDescriptor assetDescriptor, string statusName, CancellationToken cancellationToken)
        {
            var request = new ChangeAssetStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, statusName);
            return RateLimitedServiceClient(request, HttpClientExtensions.HttpMethodPatch).PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        async Task<int> GetAcrossProjectsTotalCount(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, CancellationToken cancellationToken)
        {
            var parameters = new AcrossProjectsSearchAndAggregateRequestParameters(projectIds.ToArray(), AssetTypeSearchCriteria.SearchKey);
            var aggregations = await GetAssetAggregateAsync(organizationId, parameters, cancellationToken);
            var total = 0;
            foreach (var aggregate in aggregations)
            {
                total += aggregate.Count;
            }

            return total;
        }

        /// <inheritdoc />
        public async Task UploadContentAsync(Uri uploadUri, Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            const string blobTypeHeaderKey = "X-Ms-Blob-Type";
            const string blobTypeHeaderValue = "BlockBlob";

            cancellationToken.ThrowIfCancellationRequested();

            if (uploadUri == null)
            {
                throw new InvalidUrlException("Upload url is null or empty");
            }

            using var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Put;
            httpRequestMessage.RequestUri = uploadUri;
            httpRequestMessage.Content = new StreamContent(sourceStream);

            httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            httpRequestMessage.Headers.Add(blobTypeHeaderKey, blobTypeHeaderValue);

            var response = await RateLimitedServiceClient("UploadFile", HttpMethod.Put)
                .SendAsync(httpRequestMessage, ServiceHttpClientOptions.SkipDefaultAuthenticationOption(), HttpCompletionOption.ResponseContentRead, progress, cancellationToken);

            var result = response.EnsureSuccessStatusCode();
            if (!result.IsSuccessStatusCode)
            {
                throw new UploadFailedException($"Upload of content stream for file id {uploadUri} failed.");
            }
        }

        /// <inheritdoc />
        public async Task DownloadContentAsync(Uri downloadUri, Stream destinationStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (downloadUri == null)
            {
                throw new InvalidUrlException("Download url is null or empty");
            }

            using var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = downloadUri;

            using var response = await m_ServiceHttpClient.SendAsync(httpRequestMessage, ServiceHttpClientOptions.SkipDefaultAuthenticationOption(), HttpCompletionOption.ResponseContentRead, progress, cancellationToken);
            response.EnsureSuccessStatusCode();

            var source = await response.Content.ReadAsStreamAsync();

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await source.CopyToAsync(destinationStream, cancellationToken);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Could not write to {nameof(destinationStream)}", nameof(destinationStream), e);
            }
            finally
            {
                await source.DisposeAsync();
            }
        }

        /// <inheritdoc />
        public Task RemoveAssetMetadataAsync(AssetDescriptor assetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var request = new RemoveMetadataRequest(assetDescriptor.ProjectId,
                assetDescriptor.AssetId,
                assetDescriptor.AssetVersion,
                metadataType,
                keys);

            return RateLimitedServiceClient(request, HttpMethod.Delete).DeleteAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Uri GetServiceRequestUrl(string relativePath)
        {
            return new Uri(m_PublicServiceHostResolver.GetResolvedRequestUri(relativePath));
        }

        async IAsyncEnumerable<IAssetData> ListAssetsAsync(ApiRequest request, SearchRequestParameters parameters, int offset, int length, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (length == 0) yield break;

            const int maxPageSize = 99;

            var pagination = parameters.Pagination;

            var lastIndex = offset + length;
            var pageSize = Math.Min(maxPageSize, lastIndex);
            parameters.Pagination.Limit = pageSize;

            var startPage = offset / pageSize;
            var currentIndex = offset;

            var firstPage = await AdvanceTokenToFirstPageAsync(request, pagination, startPage, cancellationToken);

            for (var i = offset % pageSize; i < firstPage.Assets.Length; ++i)
            {
                if (currentIndex++ >= lastIndex) break;

                cancellationToken.ThrowIfCancellationRequested();

                yield return firstPage.Assets[i];
            }

            pagination.Token = firstPage.Token;

            pageSize = Math.Min(maxPageSize, length);
            pagination.Limit = pageSize;

            var results = GetNextAsset(request, pagination, currentIndex, offset, length, cancellationToken);
            await foreach (var result in results)
            {
                yield return result;
            }
        }

        async Task<AssetPageDto> AdvanceTokenToFirstPageAsync(ApiRequest request, SearchRequestPagination pagination, int startPage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestUri = GetPublicRequestUri(request);

            var currentPage = 0;

            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            string jsonContent;
            while (currentPage < startPage)
            {
                ++currentPage;

                jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var pageTokenDto = JsonSerialization.Deserialize<PageTokenDto>(jsonContent);
                pagination.Token = pageTokenDto.Token;

                cancellationToken.ThrowIfCancellationRequested();

                response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(requestUri, request.ConstructBody(),
                    ServiceHttpClientOptions.Default(), cancellationToken);
            }

            jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<AssetPageDto>(jsonContent);
        }

        async IAsyncEnumerable<IAssetData> GetNextAsset(ApiRequest request, SearchRequestPagination pagination, int index, int offset, int length, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var requestUri = GetPublicRequestUri(request);

            var cutoff = offset + length;
            while (index < cutoff)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(pagination.Token)) break;

                var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(requestUri, request.ConstructBody(),
                    ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var dto = IsolatedSerialization.DeserializeWithDefaultConverters<AssetPageDto>(jsonContent);

                // To prevent an infinite loop, return if no assets were returned
                if (dto.Assets.Length == 0) break;

                foreach (var asset in dto.Assets)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (++index < offset) continue;
                    if (index > cutoff) yield break;

                    yield return asset;
                }

                pagination.Token = dto.Token;
            }
        }

        IServiceHttpClient RateLimitedServiceClient(ApiRequest request, HttpMethod httpMethod)
        {
            return RateLimitedServiceClient(request, httpMethod.ToString());
        }

        IServiceHttpClient RateLimitedServiceClient(ApiRequest request, string httpMethod)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return m_ServiceHttpClient;
#else
            var requestKey = request.GetType() + httpMethod;
            IServiceHttpClient client;

            lock (m_Lock)
            {
                if (m_HttpClients.TryGetValue(requestKey, out client)) return client;

                client = IsSlowRequest(request)
                    ? new RateLimitedServiceHttpClient(m_ServiceHttpClient, k_QueueLimit, k_SlowTokensPerPeriod,
                        k_SlowTokenLimit, TimeSpan.FromSeconds(k_SlowReplenishmentPeriod))
                    : new RateLimitedServiceHttpClient(m_ServiceHttpClient, k_QueueLimit, k_DefaultTokensPerPeriod,
                        k_DefaultTokenLimit, TimeSpan.FromSeconds(k_ReplenishmentPeriod));

                m_HttpClients[requestKey] = client;
            }

            return client;
#endif
        }

        IServiceHttpClient RateLimitedServiceClient(string requestType, HttpMethod httpMethod)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return m_ServiceHttpClient;
#else
            var requestKey = requestType + httpMethod;
            IServiceHttpClient client;

            lock (m_Lock)
            {
                if (m_HttpClients.TryGetValue(requestKey, out client)) return client;

                client = new RateLimitedServiceHttpClient(m_ServiceHttpClient, k_QueueLimit, k_DefaultTokensPerPeriod,
                    k_SlowTokenLimit, TimeSpan.FromSeconds(k_ReplenishmentPeriod));

                m_HttpClients[requestKey] = client;
            }

            return client;
#endif
        }

        static bool IsSlowRequest(ApiRequest request)
        {
            return request is SearchRequest or AcrossProjectsSearchRequest or SearchAndAggregateRequest or AcrossProjectsSearchAndAggregateRequest;
        }

        static Uri GetEscapedUri(string url)
        {
            var uri = new Uri(url);
            // Using the AbsoluteUri of an existing Uri ensures that the url is properly escaped.
            return new Uri(uri.AbsoluteUri);
        }
    }
}
