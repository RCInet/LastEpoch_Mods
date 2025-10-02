using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal interface IGuestProjectJsonProvider
    {
        public IAsyncEnumerable<ProjectJson> GetGuestProjectsAsync(Range range, CancellationToken cancellationToken);
    }

    internal class GuestProjectJsonProvider : IGuestProjectJsonProvider
    {
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IServiceHttpClient m_ServiceHttpClient;

        readonly GetRequestResponseCache<RangeResultsJson<ProjectJson>> m_GetGuestProjectRequestResponseCache;

        readonly string m_UserId;

        public GuestProjectJsonProvider(string userId, IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            // If service host is the public unity services gateway
            if (serviceHostResolver is ServiceHostResolver unityServiceHostResolver && unityServiceHostResolver.GetResolvedHost().EndsWith("services.api.unity.com"))
            {
                // Switch to using the internal unity services gateway host
                serviceHostResolver = unityServiceHostResolver.CreateCopyWithDomainResolverOverride(new UnityServicesDomainResolver(true));
            }

            m_UserId = userId;
            m_ServiceHostResolver = serviceHostResolver;
            m_ServiceHttpClient = serviceHttpClient;
            m_GetGuestProjectRequestResponseCache = new GetRequestResponseCache<RangeResultsJson<ProjectJson>>(60);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProjectJson> GetGuestProjectsAsync(Range range,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var rangeRequest = new RangeRequest<ProjectJson>(GetGuestProjects, 1000);
            var requestBasePath = $"/api/unity/legacy/v1/users/{m_UserId}/guest-projects";
            var results = rangeRequest.Execute(requestBasePath, range, cancellationToken);
            await foreach (var projectJson in results)
            {
                yield return projectJson;
            }
        }

        async Task<RangeResultsJson<ProjectJson>> GetGuestProjects(string rangeRequestPath, CancellationToken cancellationToken)
        {
            var url = m_ServiceHostResolver.GetResolvedRequestUri(rangeRequestPath);
            if (m_GetGuestProjectRequestResponseCache.TryGetRequestResponseFromCache(url, out RangeResultsJson<ProjectJson> value))
            {
                return value;
            }

            var response = await m_ServiceHttpClient.GetAsync(url, cancellationToken: cancellationToken);
            var deserializedResponse = await response.JsonDeserializeAsync<RangeResultsJson<ProjectJson>>();
            return m_GetGuestProjectRequestResponseCache.AddGetRequestResponseToCache(url, deserializedResponse);
        }
    }
}
