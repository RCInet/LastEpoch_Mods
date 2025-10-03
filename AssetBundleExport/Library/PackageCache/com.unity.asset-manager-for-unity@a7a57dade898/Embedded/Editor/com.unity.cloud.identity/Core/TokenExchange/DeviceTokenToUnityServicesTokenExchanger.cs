using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An <see cref="IAccessTokenExchanger{T, T}"/> where the T1 input is a DeviceToken and T2 output is a <see cref="UnityServicesToken"/>.
    /// </summary>
    [Obsolete("Deprecated in favor of AccessTokenToUnityServicesTokenExchanger.")]
class DeviceTokenToUnityServicesTokenExchanger : IAccessTokenExchanger<DeviceToken, UnityServicesToken>
    {
        readonly AccessTokenToUnityServicesTokenExchanger m_AccessTokenToUnityServicesTokenExchanger;

        /// <summary>
        /// Provides Unity Services token from DeviceToken
        /// </summary>
        /// <param name="httpClient">An <see cref="IHttpClient"/> instance.</param>
        /// <param name="serviceHostResolver">An <see cref="IServiceHostResolver"/> instance.</param>
        public DeviceTokenToUnityServicesTokenExchanger(IHttpClient httpClient, IServiceHostResolver serviceHostResolver)
        {
            m_AccessTokenToUnityServicesTokenExchanger =
                new AccessTokenToUnityServicesTokenExchanger(httpClient, serviceHostResolver);
        }

        /// <inheritdoc/>
        public async Task<UnityServicesToken> ExchangeAsync(DeviceToken deviceToken)
        {
            return await m_AccessTokenToUnityServicesTokenExchanger.ExchangeAsync(deviceToken.AccessToken);
        }
    }

    /// <summary>
    /// An <see cref="IAccessTokenExchanger{T, T}"/> where the T1 input is a string and T2 output is a <see cref="UnityServicesToken"/>.
    /// </summary>
    class AccessTokenToUnityServicesTokenExchanger : IAccessTokenExchanger<string, UnityServicesToken>
    {
        readonly IHttpClient m_HttpClient;
        readonly IServiceHostResolver m_ServiceHostResolver;
        /// <summary>
        /// Provides Unity Services token from DeviceToken
        /// </summary>
        /// <param name="httpClient">An <see cref="IHttpClient"/> instance.</param>
        /// <param name="serviceHostResolver">An <see cref="IServiceHostResolver"/> instance.</param>
        public AccessTokenToUnityServicesTokenExchanger(IHttpClient httpClient, IServiceHostResolver serviceHostResolver)
        {
            m_HttpClient = httpClient;
            m_ServiceHostResolver = serviceHostResolver;
        }

        // PKCE access token returned from Genesis requires a first exchange targeting a specific targetClientId
        // before reaching Unity Services exchange endpoint
        async Task<UnityServicesToken> ExchangeGenesisAccessTokenRequestAsync(string genesisAccessToken, string targetClientId = "ads-publisher")
        {
            var url = m_ServiceHostResolver.GetResolvedRequestUri("/app-linking/v1/token/exchange");

            var exchangeGenesisTokenRequest = new ExchangeGenesisTokenRequest
            {
                accessToken = genesisAccessToken, grantType = "EXCHANGE_ACCESS_TOKEN", targetClientId = targetClientId
            };
            var stringContent = new StringContent(JsonSerialization.Serialize(exchangeGenesisTokenRequest), Encoding.UTF8,
                    "application/json");

            var clientTargetIdTokenResponse = await m_HttpClient.PostAsync(url, stringContent);
            var unityServicesToken = await clientTargetIdTokenResponse.JsonDeserializeAsync<ExchangeTargetClientIdTokenResponse>();

            return new UnityServicesToken{ AccessToken = unityServicesToken.token};
        }

        /// <inheritdoc/>
        public async Task<UnityServicesToken> ExchangeAsync(string accessToken)
        {
            return await ExchangeGenesisAccessTokenRequestAsync(accessToken);
        }
    }
}
