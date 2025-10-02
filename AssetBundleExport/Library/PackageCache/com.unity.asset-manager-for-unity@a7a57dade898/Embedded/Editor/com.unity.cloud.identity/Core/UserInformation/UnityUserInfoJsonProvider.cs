using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal interface IUnityUserInfoJsonProvider
    {
        public Task<UnityUserInfoJson> GetUnityUserInfoJsonAsync();
    }

    internal class UnityUserInfoJsonProvider : IUnityUserInfoJsonProvider
    {
        readonly string m_UserId;
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IServiceHttpClient m_ServiceHttpClient;

        readonly GetRequestResponseCache<UnityUserInfoJson> m_GetUnityUserOrganizationRequestResponseCache;

        public UnityUserInfoJsonProvider(string userId, IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
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

            m_GetUnityUserOrganizationRequestResponseCache = new GetRequestResponseCache<UnityUserInfoJson>(60);
        }

        public async Task<UnityUserInfoJson> GetUnityUserInfoJsonAsync()
        {
            var url = m_ServiceHostResolver.GetResolvedRequestUri($"/api/unity/legacy/v1/users/{m_UserId}/organizations");
            UnityUserInfoJson userInfoJson;
            if (m_GetUnityUserOrganizationRequestResponseCache.TryGetRequestResponseFromCache(url, out UnityUserInfoJson value))
            {
                userInfoJson = value;
            }
            else
            {
                var response = await m_ServiceHttpClient.GetAsync(url);
                var deserializedUnityUserInfoResponse = await response.JsonDeserializeAsync<UnityUserInfoJson>();
                userInfoJson = m_GetUnityUserOrganizationRequestResponseCache.AddGetRequestResponseToCache(url, deserializedUnityUserInfoResponse);
            }
            return userInfoJson;
        }
    }
}
