using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Builds a <see cref="PkceAuthenticatorSettings"/> to inject into the <see cref="PkceAuthenticator"/>.
    /// </summary>
    class PkceAuthenticatorSettingsBuilder
    {
        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;
        readonly IServiceHostResolver m_ServiceHostResolver;

        IHttpClient m_HttpClient;
        IAppIdProvider m_AppIdProvider;
        IAppNamespaceProvider m_AppNamespaceProvider;
        IPkceConfigurationProvider m_PkceConfigurationProvider;
        IPkceRequestHandler m_PkceRequestHandler;
        IAccessTokenExchanger<string, UnityServicesToken> m_AccessTokenExchanger;
        IJwtDecoder m_JwtDecoder;

        /// <summary>
        /// Creates a <see cref="PkceAuthenticatorSettingsBuilder"/> that builds a <see cref="PkceAuthenticatorSettings"/> to inject into the <see cref="PkceAuthenticator"/>.
        /// </summary>
        /// <param name="authenticationPlatformSupport">An <see cref="IAuthenticationPlatformSupport"/> instance.</param>
        /// <param name="serviceHostResolver">An <see cref="IServiceHostResolver"/> instance.</param>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public PkceAuthenticatorSettingsBuilder(IAuthenticationPlatformSupport authenticationPlatformSupport,
            IServiceHostResolver serviceHostResolver)
        {
            ThrowIfNull(authenticationPlatformSupport, nameof(authenticationPlatformSupport));
            ThrowIfNull(serviceHostResolver, nameof(serviceHostResolver));

            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
            m_ServiceHostResolver = serviceHostResolver;
        }

        /// <summary>
        /// Adds a default implementation of <see cref="IPkceConfigurationProvider"/> to the authenticator settings.
        /// </summary>
        /// <param name="httpClient">The <see cref="IHttpClient"/> with which to build the default <see cref="IPkceRequestHandler"/>.</param>
        /// <param name="appNamespaceProvider">The <see cref="IAppNamespaceProvider"/> to fetch the namespace required to identify the app on the device.</param>
        /// <returns>The modified <see cref="PkceAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public PkceAuthenticatorSettingsBuilder AddDefaultConfigurationProviderAndRequestHandler(IHttpClient httpClient,
            IAppNamespaceProvider appNamespaceProvider)
        {
            ThrowIfNull(httpClient, nameof(httpClient));
            ThrowIfNull(appNamespaceProvider, nameof(appNamespaceProvider));

            m_PkceConfigurationProvider = new PkceConfigurationProvider(m_ServiceHostResolver);
            m_PkceRequestHandler = new HttpPkceRequestHandler(httpClient, m_PkceConfigurationProvider);
            m_HttpClient = httpClient;
            m_AppNamespaceProvider = appNamespaceProvider;

            return this;
        }

        /// <summary>
        /// Adds a <see cref="IPkceConfigurationProvider"/> to the authenticator settings.
        /// </summary>
        /// <param name="pkceConfigurationProvider">The <see cref="IPkceConfigurationProvider"/> to add to the authenticator settings.</param>
        /// <returns>The modified <see cref="PkceAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public PkceAuthenticatorSettingsBuilder AddConfigurationProvider(
            IPkceConfigurationProvider pkceConfigurationProvider)
        {
            ThrowIfNull(pkceConfigurationProvider, nameof(pkceConfigurationProvider));

            m_PkceConfigurationProvider = pkceConfigurationProvider;
            return this;
        }

        /// <summary>
        /// Adds a default implementation of <see cref="IAppIdProvider"/> to the authenticator settings.
        /// </summary>
        /// <param name="appIdProvider">The <see cref="IAppIdProvider"/> to provide with the app registered Id.</param>
        /// <returns>The modified <see cref="PkceAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public PkceAuthenticatorSettingsBuilder AddAppIdProvider(IAppIdProvider appIdProvider)
        {
            ThrowIfNull(appIdProvider, nameof(appIdProvider));
            m_AppIdProvider = appIdProvider;
            return this;
        }

        /// <summary>
        /// Adds a default implementation of <see cref="IAppNamespaceProvider"/> to the authenticator settings.
        /// </summary>
        /// <param name="appNamespaceProvider">The <see cref="IAppNamespaceProvider"/> to provide with the app namespace.</param>
        /// <returns>The modified <see cref="PkceAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public PkceAuthenticatorSettingsBuilder AddAppNamespaceProvider(IAppNamespaceProvider appNamespaceProvider)
        {
            ThrowIfNull(appNamespaceProvider, nameof(appNamespaceProvider));
            m_AppNamespaceProvider = appNamespaceProvider;
            return this;
        }

        /// <summary>
        /// Adds a <see cref="IPkceRequestHandler"/> to the authenticator settings.
        /// </summary>
        /// <param name="pkceRequestHandler">The <see cref="IPkceRequestHandler"/> to add to the authenticator settings.</param>
        /// <returns>The modified <see cref="PkceAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public PkceAuthenticatorSettingsBuilder AddRequestHandler(IPkceRequestHandler pkceRequestHandler)
        {
            ThrowIfNull(pkceRequestHandler, nameof(pkceRequestHandler));

            m_PkceRequestHandler = pkceRequestHandler;
            return this;
        }

        /// <summary>
        /// Adds a <see cref="IServiceHostResolver"/> to the authenticator settings.
        /// </summary>
        /// <param name="httpClient">The <see cref="IHttpClient"/> with which to build the default <see cref="IAccessTokenExchanger{DeviceToken, UnityServicesToken}"/>.</param>
        /// <returns>The modified <see cref="PkceAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public PkceAuthenticatorSettingsBuilder AddDefaultAccessTokenExchanger(IHttpClient httpClient)
        {
            ThrowIfNull(httpClient, nameof(httpClient));

            m_AccessTokenExchanger = new AccessTokenToUnityServicesTokenExchanger(httpClient, m_ServiceHostResolver);
            return this;
        }

        /// <summary>
        /// Adds a <see cref="IServiceHostResolver"/> to the authenticator settings.
        /// </summary>
        /// <param name="accessTokenExchanger">The <see cref="IAccessTokenExchanger{DeviceToken, UnityServicesToken}"/> to add to the authenticator settings.</param>
        /// <returns>The modified <see cref="PkceAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public PkceAuthenticatorSettingsBuilder AddAccessTokenExchanger(IAccessTokenExchanger<string, UnityServicesToken> accessTokenExchanger)
        {
            ThrowIfNull(accessTokenExchanger, nameof(accessTokenExchanger));

            m_AccessTokenExchanger = accessTokenExchanger;
            return this;
        }

        /// <summary>
        /// Adds a <see cref="IHttpClient"/> to the authenticator settings.
        /// </summary>
        /// <param name="httpClient">The <see cref="IHttpClient"/> to add to the authenticator settings.</param>
        /// <returns>The modified <see cref="PkceAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public PkceAuthenticatorSettingsBuilder AddHttpClient(IHttpClient httpClient)
        {
            ThrowIfNull(httpClient, nameof(httpClient));

            m_HttpClient = httpClient;
            return this;
        }

        /// <summary>
        /// Adds a <see cref="IJwtDecoder"/> to the authenticator settings.
        /// </summary>
        /// <param name="jwtDecoder">The <see cref="IJwtDecoder"/> to add to the authenticator settings.</param>
        /// <returns>The modified <see cref="PkceAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public PkceAuthenticatorSettingsBuilder AddJwtDecoder(IJwtDecoder jwtDecoder)
        {
            ThrowIfNull(jwtDecoder, nameof(jwtDecoder));

            m_JwtDecoder = jwtDecoder;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="PkceAuthenticatorSettings"/> to inject into the <see cref="PkceAuthenticator"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="PkceAuthenticatorSettings"/>.
        /// </returns>
        public PkceAuthenticatorSettings Build()
        {
            // Backward compatibility
            m_JwtDecoder ??= new JwtDecoder();
            // Use the NoOpAccessTokenExchanger if the access token exchanger is null and service host resolver targets non-unity service
            m_AccessTokenExchanger ??= m_ServiceHostResolver is not ServiceHostResolver ?  new NoOpAccessTokenExchanger() : new AccessTokenToUnityServicesTokenExchanger(m_HttpClient, m_ServiceHostResolver);
            m_PkceRequestHandler ??= new HttpPkceRequestHandler(m_HttpClient, m_PkceConfigurationProvider);

            ValidateRequiredSettings();

            return new PkceAuthenticatorSettings(
                m_AuthenticationPlatformSupport,
                m_PkceConfigurationProvider,
                m_PkceRequestHandler,
                m_AccessTokenExchanger,
                m_ServiceHostResolver,
                m_HttpClient,
                m_AppIdProvider,
                m_AppNamespaceProvider,
                m_JwtDecoder
                );
        }

        /// <summary>
        /// Validates that all required settings for building a <see cref="PkceAuthenticatorSettings"/> have been added to the builder.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if any of the required settings are null.</exception>
        void ValidateRequiredSettings()
        {
            var settingsAreMissing = false;
            var missingSettingsMessage = $"The following settings must be set in order to build a {nameof(PkceAuthenticator)}: ";

            ValidateRequiredSetting(m_AuthenticationPlatformSupport, ref missingSettingsMessage, ref settingsAreMissing);
            ValidateRequiredSetting(m_PkceConfigurationProvider, ref missingSettingsMessage, ref settingsAreMissing);
            ValidateRequiredSetting(m_ServiceHostResolver, ref missingSettingsMessage, ref settingsAreMissing);

            // If any settings are missing, throw an exception.
            if (settingsAreMissing)
                throw new ArgumentNullException(missingSettingsMessage);
        }

        /// <summary>
        /// Validate if the setting is null, and append to the exception message if it is.
        /// </summary>
        /// <param name="setting">The setting to validate.</param>
        /// <param name="nullSettingsMessage">The exception message to append to.</param>
        /// <param name="anySettingsNull">Whether a setting is already null.</param>
        /// <typeparam name="T"></typeparam>
        static void ValidateRequiredSetting<T>(T setting, ref string nullSettingsMessage, ref bool anySettingsNull) where T : class
        {
            if (setting == null)
            {
                if (anySettingsNull)
                    nullSettingsMessage += ", ";
                nullSettingsMessage += typeof(T).Name;

                anySettingsNull = true;
            }
        }

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> exception if the given field is null.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="parameterName">The name of the parameter to include in the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown if the parameter is null.</exception>
        static void ThrowIfNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName);
        }
    }
}
