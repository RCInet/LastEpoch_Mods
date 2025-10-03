using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An <see cref="IAccessTokenExchanger{T, T}"/> where the T1 input is a <see cref="TargetClientIdToken"/> and T2 output is a <see cref="UnityServicesToken"/>
    /// </summary>
    class TargetClientIdTokenToUnityServicesTokenExchanger : IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken>
    {
        readonly IHttpClient m_HttpClient;

        static readonly string s_BaseUnityServicesApiUrl = "services.unity.com";
        readonly string m_UnityServicesApiUrl = "services.unity.com";

        /// <summary>
        /// Provides Unity Services token from TargetClientIdToken
        /// </summary>
        /// <param name="httpClient">An <see cref="IHttpClient"/> instance.</param>
        /// <param name="serviceHostResolver">An <see cref="IServiceHostResolver"/> instance.</param>
        public TargetClientIdTokenToUnityServicesTokenExchanger(IHttpClient httpClient, IServiceHostResolver serviceHostResolver)
        {
            m_HttpClient = httpClient;

            var environment = serviceHostResolver?.GetResolvedEnvironment();

            m_UnityServicesApiUrl = environment switch
            {
                ServiceEnvironment.Staging => string.Concat("staging.", s_BaseUnityServicesApiUrl),
                ServiceEnvironment.Test => string.Concat("staging.", s_BaseUnityServicesApiUrl),
                _ => s_BaseUnityServicesApiUrl
            };
        }

        /// <inheritdoc/>
        public async Task<UnityServicesToken> ExchangeAsync(TargetClientIdToken exchangeToken)
        {
            var response = await m_HttpClient.PostAsync($"https://{m_UnityServicesApiUrl}/api/auth/v1/genesis-token-exchange/unity", new StringContent(JsonSerialization.Serialize(exchangeToken), Encoding.UTF8, "application/json"));
            var unityServicesToken = await response.JsonDeserializeAsync<ExchangeTargetClientIdTokenResponse>();
            return new UnityServicesToken{ AccessToken = unityServicesToken.token};
        }
    }
}
