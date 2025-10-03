using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Handles all HTTP requests required in the Proof Key Code Exchange authentication flow.
    /// </summary>
    class HttpPkceRequestHandler : IPkceRequestHandler
    {
        readonly IHttpClient m_HttpClient;
        readonly IPkceConfigurationProvider m_PkceConfigurationProvider;

        DeviceToken m_DeviceToken;
        PkceUserInfoClaims m_PkceUserInfoClaims;

        /// <summary>
        /// Creates a <see cref="HttpPkceRequestHandler"/> that handles all HTTP requests required in the Proof Key Code Exchange authentication flow.
        /// </summary>
        /// <param name="httpClient">The <see cref="IHttpClient"/> instance required to make HTTP requests.</param>
        /// <param name="pkceConfigurationProvider">The <see cref="IPkceConfigurationProvider"/> instance used to fetch the <see cref="PkceConfiguration"/> holding endpoints url.</param>
        public HttpPkceRequestHandler(IHttpClient httpClient, IPkceConfigurationProvider pkceConfigurationProvider)
        {
            m_HttpClient = httpClient;
            m_PkceConfigurationProvider = pkceConfigurationProvider;
        }

        /// <summary>
        /// Creates a task that sends an HTTP request to the <see cref="PkceConfiguration.TokenUrl"/> to exchange a code for a <see cref="DeviceToken"/>.
        /// </summary>
        /// <param name="tokenEndPointParams">The application/x-www-form-urlencoded string value that holds all parameters required to reach the <see cref="PkceConfiguration.TokenUrl"/> endpoint.</param>
        /// <returns>
        /// A task that results in a <see cref="DeviceToken"/> when completed.
        /// </returns>
        public async Task<DeviceToken> ExchangeCodeForDeviceTokenAsync(string tokenEndPointParams)
        {
            var pkceConfiguration = await m_PkceConfigurationProvider.GetPkceConfigurationAsync();
            var response = await m_HttpClient.PostAsync(pkceConfiguration.TokenUrl, new StringContent(tokenEndPointParams, Encoding.UTF8, "application/x-www-form-urlencoded"));
            var exchangeCodeToken = await response.JsonDeserializeAsync<ExchangeCodeToken>();
            m_DeviceToken = new DeviceToken(exchangeCodeToken.access_token, exchangeCodeToken.refresh_token, exchangeCodeToken.expires_in);
            m_PkceUserInfoClaims = null;
            return m_DeviceToken;
        }

        /// <summary>
        /// Creates a task that sends an HTTP request to the <see cref="PkceConfiguration.RefreshTokenUrl"/> to refresh the current <see cref="DeviceToken"/>.
        /// </summary>
        /// <param name="tokenEndPointParams">The application/x-www-form-urlencoded string value that holds all parameters required to reach the <see cref="PkceConfiguration.RefreshTokenUrl"/> endpoint.</param>
        /// <param name="refreshToken">The refresh token string value to persist if no refresh token value is returned from <see cref="PkceConfiguration.RefreshTokenUrl"/> endpoint.</param>
        /// <returns>
        /// A task that results in a <see cref="DeviceToken"/> when completed.
        /// </returns>
        public async Task<DeviceToken> RefreshTokenAsync(string tokenEndPointParams, string refreshToken)
        {
            var pkceConfiguration = await m_PkceConfigurationProvider.GetPkceConfigurationAsync();
            var response = await m_HttpClient.PostAsync(pkceConfiguration.RefreshTokenUrl, new StringContent(tokenEndPointParams, Encoding.UTF8, "application/x-www-form-urlencoded"));
            var refreshDeviceToken = await response.JsonDeserializeAsync<RefreshDeviceToken>();
            m_DeviceToken = new DeviceToken(refreshDeviceToken.access_token, refreshDeviceToken.refresh_token, refreshDeviceToken.expires_in, refreshToken);
            m_PkceUserInfoClaims = null;
            return m_DeviceToken;
        }

        /// <summary>
        /// Creates a Task that sends an HTTP request to the <see cref="PkceConfiguration.LogoutUrl"/> to revoke the current <see cref="DeviceToken"/>.
        /// </summary>
        /// <param name="revokeEndPointParams">The application/x-www-form-urlencoded string value that holds all parameters required to reach the <see cref="PkceConfiguration.LogoutUrl"/> endpoint.</param>
        /// <returns>
        /// A task.
        /// </returns>
        public async Task RevokeRefreshTokenAsync(string revokeEndPointParams)
        {
            m_PkceUserInfoClaims = null;
            var pkceConfiguration = await m_PkceConfigurationProvider.GetPkceConfigurationAsync();
            await m_HttpClient.PostAsync(pkceConfiguration.LogoutUrl, new StringContent(revokeEndPointParams, Encoding.UTF8, "application/x-www-form-urlencoded"));
        }

        /// <inheritdoc />
        public async Task<string> GetUserInfoAsync(string userInfoClaim)
        {
            if (m_PkceUserInfoClaims == null)
            {
                var pkceConfiguration = await m_PkceConfigurationProvider.GetPkceConfigurationAsync();
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, pkceConfiguration.UserInfoUrl);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", m_DeviceToken?.AccessToken);
                var response = await m_HttpClient.SendAsync(requestMessage);
                m_PkceUserInfoClaims = await response.JsonDeserializeAsync<PkceUserInfoClaims>();
            }
            return m_PkceUserInfoClaims.GetUserInfo(userInfoClaim);
        }
    }
}
