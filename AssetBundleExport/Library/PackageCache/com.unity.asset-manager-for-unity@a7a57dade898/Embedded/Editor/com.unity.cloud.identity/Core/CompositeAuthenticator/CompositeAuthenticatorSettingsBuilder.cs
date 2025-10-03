using System;
using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{

    /// <summary>
    /// Creates a <see cref="CompositeAuthenticatorSettingsBuilder"/> that builds a <see cref="CompositeAuthenticatorSettings"/> to inject into the <see cref="CompositeAuthenticator"/>.
    /// </summary>
    class CompositeAuthenticatorSettingsBuilder
    {
        readonly IAppIdProvider m_AppIdProvider;
        readonly IHttpClient m_HttpClient;
        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;
        readonly IServiceHostResolver m_ServiceHostResolver;

        internal readonly List<IAuthenticator> m_Authenticators = new List<IAuthenticator>();

        /// <summary>
        /// Creates a <see cref="CompositeAuthenticatorSettingsBuilder"/> that builds a <see cref="CompositeAuthenticatorSettings"/> to inject into the <see cref="CompositeAuthenticator"/>.
        /// </summary>
        /// <param name="httpClient">An <see cref="IHttpClient"/> instance.</param>
        /// <param name="authenticationPlatformSupport">An <see cref="IAuthenticationPlatformSupport"/> instance.</param>
        /// <param name="serviceHostResolver">An <see cref="IServiceHostResolver"/> instance.</param>
        public CompositeAuthenticatorSettingsBuilder(IHttpClient httpClient, IAuthenticationPlatformSupport authenticationPlatformSupport, IServiceHostResolver serviceHostResolver)
        {
            m_HttpClient = httpClient;
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
            m_ServiceHostResolver = serviceHostResolver;
        }

        /// <summary>
        /// Creates a <see cref="CompositeAuthenticatorSettingsBuilder"/> that builds a <see cref="CompositeAuthenticatorSettings"/> to inject into the <see cref="CompositeAuthenticator"/>.
        /// </summary>
        /// <param name="httpClient">An <see cref="IHttpClient"/> instance.</param>
        /// <param name="authenticationPlatformSupport">An <see cref="IAuthenticationPlatformSupport"/> instance.</param>
        /// <param name="serviceHostResolver">An <see cref="IServiceHostResolver"/> instance.</param>
        /// <param name="appIdProvider">An <see cref="IAppIdProvider"/> instance.</param>
        public CompositeAuthenticatorSettingsBuilder(IHttpClient httpClient, IAuthenticationPlatformSupport authenticationPlatformSupport, IServiceHostResolver serviceHostResolver, IAppIdProvider appIdProvider)
        {
            m_HttpClient = httpClient;
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
            m_ServiceHostResolver = serviceHostResolver;
            m_AppIdProvider = appIdProvider;
        }

        /// <summary>
        /// Adds a default <see cref="PkceAuthenticator"/> to the list of <see cref="IAuthenticator"/>.
        /// </summary>
        /// <param name="appNamespaceProvider">An <see cref="IAppNamespaceProvider"/> instance.</param>
        /// <returns>The modified <see cref="CompositeAuthenticatorSettingsBuilder"/>.</returns>
        public CompositeAuthenticatorSettingsBuilder AddDefaultPkceAuthenticator(IAppNamespaceProvider appNamespaceProvider)
        {
            var pkceAuthenticatorSettingsBuilder = new PkceAuthenticatorSettingsBuilder(m_AuthenticationPlatformSupport, m_ServiceHostResolver);
            pkceAuthenticatorSettingsBuilder.AddDefaultConfigurationProviderAndRequestHandler(m_HttpClient, appNamespaceProvider)
                .AddAppIdProvider(m_AppIdProvider)
                .AddDefaultAccessTokenExchanger(m_HttpClient);

            var pkceAuthenticatorSettings = pkceAuthenticatorSettingsBuilder.Build();

            m_Authenticators.Add(new PkceAuthenticator(pkceAuthenticatorSettings));
            return this;
        }

        /// <summary>
        /// Adds a default <see cref="BrowserAuthenticatedAccessTokenProvider"/> to the list of <see cref="IAuthenticator"/>.
        /// </summary>
        /// <param name="appNamespaceProvider">An <see cref="IAppNamespaceProvider"/> instance.</param>
        /// <param name="localStorageKeyNames">An optional Dictionary to define, per hosted domain, what key name to look for an access token in local storage of the browser.</param>
        /// <returns>The modified <see cref="CompositeAuthenticatorSettingsBuilder"/>.</returns>
        public CompositeAuthenticatorSettingsBuilder AddDefaultBrowserAuthenticatedAccessTokenProvider(IAppNamespaceProvider appNamespaceProvider, Dictionary<string, string> localStorageKeyNames = null)
        {
            localStorageKeyNames ??= BrowserAuthenticatedAccessTokenProvider.DefaultLocalStorageKeyNames;
            var pkceAuthenticatorSettingsBuilder = new PkceAuthenticatorSettingsBuilder(m_AuthenticationPlatformSupport, m_ServiceHostResolver);
            pkceAuthenticatorSettingsBuilder.AddDefaultConfigurationProviderAndRequestHandler(m_HttpClient, appNamespaceProvider)
                .AddAppIdProvider(m_AppIdProvider)
                .AddDefaultAccessTokenExchanger(m_HttpClient);

            var pkceAuthenticatorSettings = pkceAuthenticatorSettingsBuilder.Build();

            m_Authenticators.Add(new BrowserAuthenticatedAccessTokenProvider(pkceAuthenticatorSettings, localStorageKeyNames));
            return this;
        }

        /// <summary>
        /// Adds any <see cref="IAuthenticator"/> to the list of <see cref="IAuthenticator"/>.
        /// </summary>
        /// <param name="authenticator">An <see cref="IAuthenticator"/> instance.</param>
        /// <returns>The modified <see cref="CompositeAuthenticatorSettingsBuilder"/>.</returns>
        public CompositeAuthenticatorSettingsBuilder AddAuthenticator(IAuthenticator authenticator)
        {
            m_Authenticators.Add(authenticator);
            return this;
        }

        /// <summary>
        /// Builds the <see cref="CompositeAuthenticatorSettings"/> to inject into the <see cref="CompositeAuthenticator"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="CompositeAuthenticatorSettings"/>.
        /// </returns>
        public CompositeAuthenticatorSettings Build()
        {
            return new CompositeAuthenticatorSettings(m_Authenticators);
        }
    }
}
