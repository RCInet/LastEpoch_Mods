using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// A factory to create a <see cref="IPkceConfigurationProvider"/> for the Unity services gateway or for a fully qualified domain name.
    /// </summary>
    static class PkceConfigurationProviderFactory
    {
        /// <summary>
        /// The environment variable key for the service fully qualified domain name override.
        /// </summary>
        internal static string SystemOverrideOpenIdConfigurationUrlVariableName => "UNITY_CLOUD_SERVICES_OPENID_CONFIGURATION_URL";

        /// <summary>
        /// The environment variable key for the service fully qualified domain name prefix override.
        /// </summary>
        internal static string SystemOverrideOpenIdClientIdVariableName => "UNITY_CLOUD_SERVICES_OPENID_CLIENT_ID";

        /// <summary>
        /// Create a <see cref="IPkceConfigurationProvider"/>.
        /// Default to the Unity services gateway.
        /// Any system-level overrides for a fully qualified domain name set via environment variables will take priority.
        /// </summary>
        /// <param name="serviceHostResolver">The <see cref="IServiceHostResolver"/> instance.</param>
        /// <param name="httpClient">The <see cref="IHttpClient"/> instance.</param>
        /// <returns>The created <see cref="IPkceConfigurationProvider"/>.</returns>
        public static IPkceConfigurationProvider Create(IServiceHostResolver serviceHostResolver, IHttpClient httpClient)
        {
            var openIdConfigurationUrl = Environment.GetEnvironmentVariable(SystemOverrideOpenIdConfigurationUrlVariableName);
            var openIdClientName = Environment.GetEnvironmentVariable(SystemOverrideOpenIdClientIdVariableName);
            if (string.IsNullOrEmpty(openIdConfigurationUrl)) return CreateForUnityServicesGateway(serviceHostResolver);

            var basePkceConfiguration = new PkceConfiguration
            {
                CacheRefreshToken = true,
                ClientId = new ClientId(openIdClientName ?? OpenIdPkceConfigurationProvider.DefaultClientId)
            };
            return new OpenIdPkceConfigurationProvider(serviceHostResolver, httpClient, openIdConfigurationUrl, basePkceConfiguration);
        }

        /// <summary>
        /// Create a <see cref="IPkceConfigurationProvider"/> that returns a <see cref="PkceConfiguration"/> to authenticate user on the Unity Services Gateway.
        /// </summary>
        /// <param name="serviceHostResolver">The service host resolver for the service Url.</param>
        /// <returns>The created <see cref="IPkceConfigurationProvider"/>.</returns>
        static IPkceConfigurationProvider CreateForUnityServicesGateway(
            IServiceHostResolver serviceHostResolver)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return new PkceConfigurationProvider(serviceHostResolver);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Create a <see cref="IPkceConfigurationProvider"/> that fetches a public openid configuration JSON file to generate a <see cref="PkceConfiguration"/>.
        /// </summary>
        /// <param name="serviceHostResolver">The service host resolver for the service Url.</param>
        /// <param name="httpClient">The http client required to fetch the public configuration.</param>
        /// <param name="openIdConfigurationUrl">The full URL to the public openid configuration.</param>
        /// <param name="clientName">The scoped openid client name.</param>
        /// <returns>The created <see cref="IPkceConfigurationProvider"/>.</returns>
        public static IPkceConfigurationProvider CreateForFullyQualifiedDomainName(
            IServiceHostResolver serviceHostResolver, IHttpClient httpClient,
            string openIdConfigurationUrl, string clientName)
        {
            var basePkceConfiguration = new PkceConfiguration
            {
                ClientId = new ClientId(clientName),
            };
            return new OpenIdPkceConfigurationProvider(serviceHostResolver, httpClient, openIdConfigurationUrl, basePkceConfiguration);
        }

        /// <summary>
        /// Create a <see cref="IPkceConfigurationProvider"/> that fetches a public openid configuration JSON file to generate a <see cref="PkceConfiguration"/>.
        /// </summary>
        /// <param name="serviceHostResolver">The service host resolver for the service Url.</param>
        /// <param name="httpClient">The http client required to fetch the public configuration.</param>
        /// <param name="openIdConfigurationUrl">The full URL to the public openid configuration.</param>
        /// <param name="basePkceConfiguration">The PkceConfiguration base that will be completed with values fetched from the openid configuration.</param>
        /// <returns>The created <see cref="IPkceConfigurationProvider"/>.</returns>
        public static IPkceConfigurationProvider CreateForFullyQualifiedDomainName(
            IServiceHostResolver serviceHostResolver, IHttpClient httpClient,
            string openIdConfigurationUrl, PkceConfiguration basePkceConfiguration)
        {
            return new OpenIdPkceConfigurationProvider(serviceHostResolver, httpClient, openIdConfigurationUrl, basePkceConfiguration);
        }

    }
}
