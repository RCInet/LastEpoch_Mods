using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal class OrganizationProjectsJsonProvider : IOrganizationProjectsJsonProvider
    {
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IServiceHttpClient m_ServiceHttpClient;

        readonly GetRequestResponseCache<RangeResultsJson<ProjectJson>> m_GetRequestResponseCache;

        public OrganizationProjectsJsonProvider(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            // If service host is the public unity services gateway
            if (serviceHostResolver is ServiceHostResolver unityServiceHostResolver && unityServiceHostResolver.GetResolvedHost().EndsWith("services.api.unity.com"))
            {
                // Switch to using the internal unity services gateway host
                serviceHostResolver = unityServiceHostResolver.CreateCopyWithDomainResolverOverride(new UnityServicesDomainResolver(true));
            }

            m_ServiceHostResolver = serviceHostResolver;
            m_ServiceHttpClient = serviceHttpClient;

            m_GetRequestResponseCache = new GetRequestResponseCache<RangeResultsJson<ProjectJson>>(60);
        }

        public async IAsyncEnumerable<ProjectJson> GetOrganizationProjectsJson(OrganizationId organizationId, IEntityRoleProvider entityRoleProvider,
            Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var rangeRequest = new RangeRequest<ProjectJson>(GetOrganizationProjects, 1000);
            var requestBasePath = $"/api/unity/legacy/v1/organizations/{organizationId}/projects";
            var results = rangeRequest.Execute(requestBasePath, range, cancellationToken);
            await foreach (var projectJson in results)
            {
                yield return projectJson;
            }
        }

        async Task<RangeResultsJson<ProjectJson>> GetOrganizationProjects(string rangeRequestPath, CancellationToken cancellationToken)
        {
            var url = m_ServiceHostResolver.GetResolvedRequestUri(rangeRequestPath);
            if (m_GetRequestResponseCache.TryGetRequestResponseFromCache(url, out RangeResultsJson<ProjectJson> value))
            {
                return value;
            }
            var response = await m_ServiceHttpClient.GetAsync(url, cancellationToken: cancellationToken);
            var deserializedResponse = await response.JsonDeserializeAsync<RangeResultsJson<ProjectJson>>();
            return m_GetRequestResponseCache.AddGetRequestResponseToCache(url, deserializedResponse);
        }

    }
}

