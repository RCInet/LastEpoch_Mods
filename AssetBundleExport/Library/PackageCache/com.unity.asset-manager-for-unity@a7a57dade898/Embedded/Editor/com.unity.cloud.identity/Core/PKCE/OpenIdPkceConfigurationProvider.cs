using System;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An <see cref="IPkceConfigurationProvider"/> that fetches a public openid configuration JSON file to generate a <see cref="PkceConfiguration"/>.
    /// </summary>
    internal class OpenIdPkceConfigurationProvider : IPkceConfigurationProvider
    {
        public static readonly string DefaultClientId = "sdk";
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IHttpClient m_HttpClient;
        readonly string m_OpenIdConfigurationUrl;
        readonly PkceConfiguration m_BasePkceConfiguration;
        OpenIdConfigurationJson m_OpenIdConfigurationJson;

        /// <summary>
        /// Builds an <see cref="IPkceConfigurationProvider"/> that fetches a public openid configuration JSON file to generate a <see cref="PkceConfiguration"/>.
        /// </summary>
        /// <param name="serviceHostResolver">The service host resolver for the service Url.</param>
        /// <param name="httpClient">The http client required to fetch the public configuration.</param>
        /// <param name="openIdConfigurationUrl">The full URL to the public openid configuration.</param>
        /// <param name="basePkceConfiguration">An optional PkceConfiguration base that will be completed with values fetched from the openid configuration.</param>
        /// <remarks>Use the <see cref="basePkceConfiguration"/> parameter to override any openid configuration fetched values, if needed.</remarks>
        public OpenIdPkceConfigurationProvider(IServiceHostResolver serviceHostResolver, IHttpClient httpClient, string openIdConfigurationUrl, PkceConfiguration basePkceConfiguration = null)
        {
            m_ServiceHostResolver = serviceHostResolver;
            m_HttpClient = httpClient;
            m_OpenIdConfigurationUrl = openIdConfigurationUrl;
            m_BasePkceConfiguration = basePkceConfiguration ?? new PkceConfiguration { ClientId = new ClientId(DefaultClientId)};
        }

        /// <inheritdoc/>
        public async Task<PkceConfiguration> GetPkceConfigurationAsync()
        {
            return await UpdatePkceConfiguration();
        }

        async Task<PkceConfiguration> UpdatePkceConfiguration()
        {
            // Only fetch the openid configuration once per session
            if (m_OpenIdConfigurationJson == null)
            {
                var result = await m_HttpClient.GetAsync(m_OpenIdConfigurationUrl);
                m_OpenIdConfigurationJson = await result.JsonDeserializeAsync<OpenIdConfigurationJson>();
                m_OpenIdConfigurationJson = ReplaceUnsecureProtocol(m_OpenIdConfigurationJson);
            }

            var serviceDomainHostUri = new Uri(m_ServiceHostResolver.GetResolvedRequestUri(""));
            var path = serviceDomainHostUri.AbsolutePath.Equals("/") ? string.Empty : serviceDomainHostUri.AbsolutePath;
            var serviceDomainHost = $"{serviceDomainHostUri.Host}{path}";
            return new PkceConfiguration
            {
                CacheRefreshToken = m_BasePkceConfiguration.CacheRefreshToken,
                ClientId = m_BasePkceConfiguration.ClientId,
                ProxyLoginRedirectRoute = !string.IsNullOrEmpty(m_BasePkceConfiguration.ProxyLoginRedirectRoute) ? m_BasePkceConfiguration.ProxyLoginRedirectRoute : $"{serviceDomainHost}/app-linking/v1/login/redirect/",
                ProxyLoginCompletedRoute = !string.IsNullOrEmpty(m_BasePkceConfiguration.ProxyLoginCompletedRoute) ? m_BasePkceConfiguration.ProxyLoginCompletedRoute : $"{serviceDomainHost}/app-linking/v1/login/completed/",
                ProxySignOutCompletedRoute = !string.IsNullOrEmpty(m_BasePkceConfiguration.ProxySignOutCompletedRoute) ? m_BasePkceConfiguration.ProxySignOutCompletedRoute : $"{serviceDomainHost}/app-linking/v1/signout/completed/",
                LoginUrl = !string.IsNullOrEmpty(m_BasePkceConfiguration.LoginUrl) ? m_BasePkceConfiguration.LoginUrl : m_OpenIdConfigurationJson.authorization_endpoint,
                TokenUrl = !string.IsNullOrEmpty(m_BasePkceConfiguration.TokenUrl) ? m_BasePkceConfiguration.TokenUrl :  m_OpenIdConfigurationJson.token_endpoint,
                RefreshTokenUrl = !string.IsNullOrEmpty(m_BasePkceConfiguration.RefreshTokenUrl) ? m_BasePkceConfiguration.RefreshTokenUrl : m_OpenIdConfigurationJson.token_endpoint,
                LogoutUrl = !string.IsNullOrEmpty(m_BasePkceConfiguration.LogoutUrl) ? m_BasePkceConfiguration.LogoutUrl : m_OpenIdConfigurationJson.end_session_endpoint,
                SignOutUrl = !string.IsNullOrEmpty(m_BasePkceConfiguration.SignOutUrl) ? m_BasePkceConfiguration.SignOutUrl : $"{m_OpenIdConfigurationJson.end_session_endpoint}?post_logout_redirect_uri=",
                UserInfoUrl = !string.IsNullOrEmpty(m_BasePkceConfiguration.UserInfoUrl) ? m_BasePkceConfiguration.UserInfoUrl : m_OpenIdConfigurationJson.userinfo_endpoint,
                CustomLoginParams = !string.IsNullOrEmpty(m_BasePkceConfiguration.CustomLoginParams) ? m_BasePkceConfiguration.CustomLoginParams : ""
            };
        }

        // If unsecure http protocol are set for OIDC endpoints, we replace them with secure https protocol
        OpenIdConfigurationJson ReplaceUnsecureProtocol(OpenIdConfigurationJson openIdConfigurationJson)
        {
            var httpsProtocol = "https://";
            var httpProtocol = "http://";

            openIdConfigurationJson.token_endpoint = openIdConfigurationJson.token_endpoint.Replace(httpProtocol, httpsProtocol);
            openIdConfigurationJson.authorization_endpoint = openIdConfigurationJson.authorization_endpoint.Replace(httpProtocol, httpsProtocol);
            openIdConfigurationJson.end_session_endpoint = openIdConfigurationJson.end_session_endpoint.Replace(httpProtocol, httpsProtocol);
            openIdConfigurationJson.userinfo_endpoint = openIdConfigurationJson.userinfo_endpoint.Replace(httpProtocol, httpsProtocol);

            return openIdConfigurationJson;
        }

    }
}
