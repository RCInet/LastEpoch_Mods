using System;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Handles the access to a <see cref="PkceConfiguration"/>.
    /// </summary>
    class PkceConfigurationProvider : IPkceConfigurationProvider
    {
        readonly IServiceHostResolver m_ServiceHostResolver;

        /// <summary>
        /// Builds a `PkceConfigurationProvider` handles the access to a <see cref="PkceConfiguration"/>.
        /// </summary>
        /// <param name="serviceHostResolver">The service host resolver for the service Url.</param>
        [Obsolete("Use the PkceConfigurationProviderFactory instead.")]
        public PkceConfigurationProvider(IServiceHostResolver serviceHostResolver)
        {
            m_ServiceHostResolver = serviceHostResolver;
        }

        /// <summary>
        /// Creates a task that results in a <see cref="PkceConfiguration"/> when internal update is completed.
        /// </summary>
        /// <returns>
        /// A task that results in a <see cref="PkceConfiguration"/> when internal update is completed.
        /// </returns>
        public async Task<PkceConfiguration> GetPkceConfigurationAsync()
        {
            return await UpdatePkceConfiguration();
        }

        async Task<PkceConfiguration> UpdatePkceConfiguration()
        {
            var pkceConfiguration = CreateConfiguration();
            return await Task.FromResult(pkceConfiguration);
        }

        PkceConfiguration CreateConfiguration()
        {
            var serviceDomainHost =  GetServiceDomainHost();
            var serviceEnvironment = m_ServiceHostResolver?.GetResolvedEnvironment();

            var genesisSubdomain = serviceEnvironment switch
            {
                ServiceEnvironment.Staging => "api-staging",
                ServiceEnvironment.Test => "api-staging",
                _ => "api",
            };

            return new PkceConfiguration
            {
                CacheRefreshToken = true,
                ClientId = new ClientId("unity_cloud"),
                ProxyLoginRedirectRoute = $"{serviceDomainHost}/app-linking/v1/login/redirect/",
                ProxyLoginCompletedRoute = $"{serviceDomainHost}/app-linking/v1/login/completed/",
                ProxySignOutCompletedRoute = $"{serviceDomainHost}/app-linking/v1/signout/completed/",
                LoginUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/authorize",
                TokenUrl = $"https://{serviceDomainHost}/app-linking/v1/token",
                RefreshTokenUrl = $"https://{serviceDomainHost}/app-linking/v1/token",
                LogoutUrl = $"https://{serviceDomainHost}/app-linking/v1/token/revoke",
                SignOutUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/end-session?post_logout_redirect_uri=",
                UserInfoUrl = $"https://{genesisSubdomain}.unity.com/v1/users/current/openid",
                CustomLoginParams = ""
            };
        }

        string GetServiceDomainHost()
        {
            var serviceAddress = m_ServiceHostResolver?.GetResolvedAddress();
            if (serviceAddress != null)
            {
                var serviceAddressUri = new Uri(serviceAddress);
                return serviceAddressUri.Host;
            }

            return string.Empty;
        }
    }
}
