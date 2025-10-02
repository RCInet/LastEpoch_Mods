using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// A factory to create a <see cref="ServiceConnector"/> instance for the Unity services gateway or for a fully qualified domain name.
    /// </summary>
    static class ServiceConnectorFactory
    {
        /// <summary>
        /// Creates the <see cref="ServiceConnector"/> for the Unity services gateway or for a fully qualified domain name.
        /// Default to the Unity services gateway.
        /// Any system-level overrides for a fully qualified domain name set via environment variables will take priority.
        /// </summary>
        /// <param name="platformSupport">An <see cref="IAuthenticationPlatformSupport"/> instance.</param>
        /// <param name="httpClient">An <see cref="IHttpClient"/> instance.</param>
        /// <param name="appIdProvider">An <see cref="IAppIdProvider"/> instance.</param>
        /// <param name="appNamespaceProvider">An <see cref="IAppNamespaceProvider"/> instance.</param>
        /// <returns>The <see cref="ServiceConnector"/> instance.</returns>
        public static ServiceConnector Create(IAuthenticationPlatformSupport platformSupport, IHttpClient httpClient, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider)
        {
            // Use a factory method that allows environment variables override
            var serviceHostResolver = ServiceHostResolverFactory.Create();
            // Use a factory method that allows environment variables override
            var pkceConfigurationProvider = PkceConfigurationProviderFactory.Create(serviceHostResolver, httpClient);

            // Build settings for PkceAuthenticator
            var pkceAuthenticatorSettingsBuilder = new PkceAuthenticatorSettingsBuilder(platformSupport, serviceHostResolver);
            pkceAuthenticatorSettingsBuilder.AddConfigurationProvider(pkceConfigurationProvider)
                .AddAppIdProvider(appIdProvider)
                .AddAppNamespaceProvider(appNamespaceProvider)
                .AddHttpClient(httpClient);

            // Build settings for ServiceAccountAuthenticator
            var serviceAccountAuthenticatorSettingsBuilder =
                new ServiceAccountAuthenticatorSettingsBuilder(httpClient, serviceHostResolver, platformSupport)
                    .SetAppIdProvider(appIdProvider);

            // Inject an access token exchanger if service host resolver targets non-unity service
            if (serviceHostResolver is not ServiceHostResolver)
            {
                serviceAccountAuthenticatorSettingsBuilder.SetServiceAccountCredentialsExchanger(pkceConfigurationProvider);
            }

            // Build settings for CompositeAuthenticator
            var compositeAuthenticatorSettings = BuildCompositeAuthenticatorSettings(
                platformSupport, httpClient, serviceHostResolver, appIdProvider,
                pkceAuthenticatorSettingsBuilder, serviceAccountAuthenticatorSettingsBuilder);

            return new ServiceConnector(compositeAuthenticatorSettings, serviceHostResolver, httpClient, appIdProvider);
        }

        /// <summary>
        /// Creates the <see cref="ServiceConnector"/> for a fully qualified domain name.
        /// </summary>
        /// <param name="platformSupport">An <see cref="IAuthenticationPlatformSupport"/> instance.</param>
        /// <param name="httpClient">An <see cref="IHttpClient"/> instance.</param>
        /// <param name="appIdProvider">An <see cref="IAppIdProvider"/> instance.</param>
        /// <param name="appNamespaceProvider">An <see cref="IAppNamespaceProvider"/> instance.</param>
        /// <param name="fullyQualifiedDomainName">the fully qualified domain name.</param>
        /// <param name="openIdConfigurationUrl">the openId configuration URL.</param>
        /// <param name="pathPrefix">the optional path prefix.</param>
        /// <param name="clientId">the optional client id. By convention, the default value is "sdk".</param>
        /// <returns>The <see cref="ServiceConnector"/> instance.</returns>
        public static ServiceConnector CreateForFullyQualifiedDomainName(IAuthenticationPlatformSupport platformSupport, IHttpClient httpClient, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string fullyQualifiedDomainName, string openIdConfigurationUrl,  string pathPrefix = "/", string clientId = null)
        {
            clientId ??= OpenIdPkceConfigurationProvider.DefaultClientId;
            var serviceHostResolver =  ServiceHostResolverFactory.CreateForFullyQualifiedDomainName(fullyQualifiedDomainName, pathPrefix);
            var basePkceConfiguration = new PkceConfiguration { ClientId = new ClientId(clientId) };
            var pkceConfigurationProvider = new OpenIdPkceConfigurationProvider(serviceHostResolver, httpClient, openIdConfigurationUrl, basePkceConfiguration);

            // Build settings for PkceAuthenticator
            var pkceAuthenticatorSettingsBuilder = new PkceAuthenticatorSettingsBuilder(platformSupport, serviceHostResolver);
            pkceAuthenticatorSettingsBuilder.AddConfigurationProvider(pkceConfigurationProvider)
                .AddAppIdProvider(appIdProvider)
                .AddAppNamespaceProvider(appNamespaceProvider)
                .AddHttpClient(httpClient);

            // Build settings for ServiceAccountAuthenticator
            var serviceAccountAuthenticatorSettingsBuilder =
                new ServiceAccountAuthenticatorSettingsBuilder(httpClient, serviceHostResolver, platformSupport)
                    .SetAppIdProvider(appIdProvider)
                    .SetServiceAccountCredentialsExchanger(pkceConfigurationProvider);

            // Build settings for CompositeAuthenticator
            var compositeAuthenticatorSettings = BuildCompositeAuthenticatorSettings(
                platformSupport, httpClient, serviceHostResolver, appIdProvider,
                pkceAuthenticatorSettingsBuilder, serviceAccountAuthenticatorSettingsBuilder);

            return new ServiceConnector(compositeAuthenticatorSettings, serviceHostResolver, httpClient, appIdProvider);
        }

        /// <summary>
        /// Creates the <see cref="ServiceConnector"/> for a fully qualified domain name.
        /// </summary>
        /// <param name="platformSupport">An <see cref="IAuthenticationPlatformSupport"/> instance.</param>
        /// <param name="httpClient">An <see cref="IHttpClient"/> instance.</param>
        /// <param name="appIdProvider">An <see cref="IAppIdProvider"/> instance.</param>
        /// <param name="appNamespaceProvider">An <see cref="IAppNamespaceProvider"/> instance.</param>
        /// <param name="fullyQualifiedDomainName">the fully qualified domain name.</param>
        /// <param name="openIdConfigurationUrl">the openId configuration URL.</param>
        /// <param name="basePkceConfigurationOverride">The base <see cref="PkceConfiguration"/> override.</param>
        /// <param name="pathPrefix">the optional path prefix.</param>
        /// <returns>The <see cref="ServiceConnector"/> instance.</returns>
        public static ServiceConnector CreateForFullyQualifiedDomainName(IAuthenticationPlatformSupport platformSupport, IHttpClient httpClient, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string fullyQualifiedDomainName, string openIdConfigurationUrl, PkceConfiguration basePkceConfigurationOverride, string pathPrefix = "/")
        {
            var serviceHostResolver =  ServiceHostResolverFactory.CreateForFullyQualifiedDomainName(fullyQualifiedDomainName, pathPrefix);
            var pkceConfigurationProvider = new OpenIdPkceConfigurationProvider(serviceHostResolver, httpClient, openIdConfigurationUrl, basePkceConfigurationOverride);

            // Build settings for PkceAuthenticator
            var pkceAuthenticatorSettingsBuilder = new PkceAuthenticatorSettingsBuilder(platformSupport, serviceHostResolver);
            pkceAuthenticatorSettingsBuilder.AddConfigurationProvider(pkceConfigurationProvider)
                .AddAppIdProvider(appIdProvider)
                .AddAppNamespaceProvider(appNamespaceProvider)
                .AddHttpClient(httpClient);

            // Build settings for ServiceAccountAuthenticator
            var serviceAccountAuthenticatorSettingsBuilder =
                    new ServiceAccountAuthenticatorSettingsBuilder(httpClient, serviceHostResolver, platformSupport)
                        .SetAppIdProvider(appIdProvider)
                        .SetServiceAccountCredentialsExchanger(pkceConfigurationProvider);

            // Build settings for CompositeAuthenticator
            var compositeAuthenticatorSettings = BuildCompositeAuthenticatorSettings(
                platformSupport, httpClient, serviceHostResolver, appIdProvider,
                pkceAuthenticatorSettingsBuilder, serviceAccountAuthenticatorSettingsBuilder);

            return new ServiceConnector(compositeAuthenticatorSettings, serviceHostResolver, httpClient, appIdProvider);
        }

        static CompositeAuthenticatorSettings BuildCompositeAuthenticatorSettings(
            IAuthenticationPlatformSupport platformSupport,
            IHttpClient httpClient,
            IServiceHostResolver serviceHostResolver,
            IAppIdProvider appIdProvider,
            PkceAuthenticatorSettingsBuilder pkceAuthenticatorSettingsBuilder,
            ServiceAccountAuthenticatorSettingsBuilder serviceAccountAuthenticatorSettingsBuilder)
        {
            var pkceAuthenticator = new PkceAuthenticator(pkceAuthenticatorSettingsBuilder.Build());
            var serviceAccountAuthenticator = new ServiceAccountAuthenticator(serviceAccountAuthenticatorSettingsBuilder.Build());

            var compositeAuthenticatorSettingsBuilder =
                new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver, appIdProvider)
                    // prioritize  ServiceAccountAuthenticator
                    .AddAuthenticator(serviceAccountAuthenticator)
                    // Fallback to PkceAuthenticator
                    .AddAuthenticator(pkceAuthenticator);

            return compositeAuthenticatorSettingsBuilder.Build();
        }
    }
}
