using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Builds a <see cref="ServiceAccountAuthenticatorSettings"/> to inject into the <see cref="ServiceAccountAuthenticator"/>.
    /// </summary>
    class ServiceAccountAuthenticatorSettingsBuilder
    {
        IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;
        IServiceHostResolver m_ServiceHostResolver;
        IHttpClient m_HttpClient;
        IAccessTokenExchanger<ServiceAccountCredentials, UnityServicesToken> m_AccessTokenExchanger;
        IAppIdProvider m_AppIdProvider;
        IJwtDecoder m_JwtDecoder;

        /// <summary>
        /// Constructor for the <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.
        /// </summary>
        /// <param name="httpClient">The <see cref="IHttpClient"/> to add to the authenticator settings.</param>
        /// <param name="serviceHostResolver">The <see cref="IServiceHostResolver"/> ionstance.</param>
        /// <param name="authenticationPlatformSupport">The <see cref="IAuthenticationPlatformSupport"/> ionstance.</param>
        public ServiceAccountAuthenticatorSettingsBuilder(IHttpClient httpClient, IServiceHostResolver serviceHostResolver, IAuthenticationPlatformSupport authenticationPlatformSupport)
        {
            ThrowIfNull(httpClient, nameof(httpClient));
            ThrowIfNull(serviceHostResolver, nameof(serviceHostResolver));
            ThrowIfNull(authenticationPlatformSupport, nameof(authenticationPlatformSupport));

            m_HttpClient = httpClient;
            m_ServiceHostResolver = serviceHostResolver;
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
        }

        /// <summary>
        /// Sets the Service Account credentials exchanger to the authenticator settings.
        /// </summary>
        /// <param name="pkceConfigurationProvider">The <see cref="IPkceConfigurationProvider"/> to inject in the Service Account credentials exchanger.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ServiceAccountAuthenticatorSettingsBuilder SetServiceAccountCredentialsExchanger(
            IPkceConfigurationProvider pkceConfigurationProvider)
        {
            ThrowIfNull(pkceConfigurationProvider, nameof(pkceConfigurationProvider));

            m_AccessTokenExchanger = new ServiceAccountCredentialsToUnityServicesTokenExchanger(m_HttpClient, pkceConfigurationProvider);
            return this;
        }

        /// <summary>
        /// Sets a Service Account credentials exchanger to the authenticator settings.
        /// </summary>
        /// <param name="accessTokenExchanger">The <see cref="IAccessTokenExchanger{T, T}"/> to add to the authenticator settings.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ServiceAccountAuthenticatorSettingsBuilder SetServiceAccountCredentialsExchanger(
            IAccessTokenExchanger<ServiceAccountCredentials, UnityServicesToken> accessTokenExchanger)
        {
            ThrowIfNull(accessTokenExchanger, nameof(accessTokenExchanger));

            m_AccessTokenExchanger = accessTokenExchanger;
            return this;
        }

        /// <summary>
        /// Sets an <see cref="IAppIdProvider"/> to the authenticator settings.
        /// </summary>
        /// <param name="appIdProvider">The <see cref="IAppIdProvider"/> to provide with the app Id.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ServiceAccountAuthenticatorSettingsBuilder SetAppIdProvider(IAppIdProvider appIdProvider)
        {
            ThrowIfNull(appIdProvider, nameof(appIdProvider));
            m_AppIdProvider = appIdProvider;
            return this;
        }

        /// <summary>
        /// Sets a <see cref="IJwtDecoder"/> to the authenticator settings.
        /// </summary>
        /// <param name="jwtDecoder">The <see cref="IJwtDecoder"/> to add to the authenticator settings.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        internal ServiceAccountAuthenticatorSettingsBuilder SetJwtDecoder(IJwtDecoder jwtDecoder)
        {
            ThrowIfNull(jwtDecoder, nameof(jwtDecoder));

            m_JwtDecoder = jwtDecoder;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="ServiceAccountAuthenticatorSettings"/> to inject into the <see cref="ServiceAccountAuthenticator"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceAccountAuthenticatorSettings"/>.
        /// </returns>
        public ServiceAccountAuthenticatorSettings Build()
        {
            m_JwtDecoder ??= new JwtDecoder();

            ValidateRequiredSettings();

            return new ServiceAccountAuthenticatorSettings(
                m_AuthenticationPlatformSupport,
                m_AccessTokenExchanger,
                m_ServiceHostResolver,
                m_HttpClient,
                m_AppIdProvider,
                m_JwtDecoder
                );
        }

        /// <summary>
        /// Validates that all required settings for building a <see cref="ServiceAccountAuthenticatorSettings"/> have been added to the builder.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if any of the required settings are null.</exception>
        void ValidateRequiredSettings()
        {
            var settingsAreMissing = false;
            var missingSettingsMessage = $"The following settings must be set in order to build a {nameof(ServiceAccountAuthenticator)}: ";

            ValidateRequiredSetting(m_AuthenticationPlatformSupport, ref missingSettingsMessage, ref settingsAreMissing);
            ValidateRequiredSetting(m_ServiceHostResolver, ref missingSettingsMessage, ref settingsAreMissing);
            ValidateRequiredSetting(m_HttpClient, ref missingSettingsMessage, ref settingsAreMissing);

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
