using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal class ServiceAccountCredentialsToUnityServicesTokenExchanger : IAccessTokenExchanger<ServiceAccountCredentials, UnityServicesToken>
    {
        readonly IHttpClient m_HttpClient;
        readonly IPkceConfigurationProvider m_IPkceConfigurationProvider;
        /// <summary>
        /// Provides Unity Services token from Service Account credentials
        /// </summary>
        /// <param name="httpClient">An <see cref="IHttpClient"/> instance.</param>
        /// <param name="pkceConfigurationProvider">An <see cref="IPkceConfigurationProvider"/> instance.</param>
        public ServiceAccountCredentialsToUnityServicesTokenExchanger(IHttpClient httpClient, IPkceConfigurationProvider pkceConfigurationProvider)
        {
            m_HttpClient = httpClient;
            m_IPkceConfigurationProvider = pkceConfigurationProvider;
        }

        public async Task<UnityServicesToken> ExchangeAsync(ServiceAccountCredentials serviceAccountCredentials)
        {
            var pkceConfiguration = await m_IPkceConfigurationProvider.GetPkceConfigurationAsync();

            var url = pkceConfiguration.TokenUrl;
            var requestStringParam = "grant_type=client_credentials";
            var stringContent = new StringContent(requestStringParam, Encoding.UTF8, "application/x-www-form-urlencoded");
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = stringContent
            };
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(ServiceHeaderUtils.k_BasicScheme, serviceAccountCredentials.ToBase64String());

            var response = await m_HttpClient.SendAsync(httpRequestMessage);
            var unityServicesToken = await response.JsonDeserializeAsync<ExchangeGenesisAccessTokenResponse>();
            return new UnityServicesToken{ AccessToken = unityServicesToken.access_token};
        }
    }
}
