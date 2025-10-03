
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Creates the <see cref="PkceAuthenticatorSettings"/> required to inject in a <see cref="PkceAuthenticator"/>.
    /// </summary>
    readonly struct PkceAuthenticatorSettings
    {
        /// <summary>
        /// The <see cref="IAuthenticationPlatformSupport"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IAuthenticationPlatformSupport AuthenticationPlatformSupport;

        /// <summary>
        /// The <see cref="IPkceConfigurationProvider"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IPkceConfigurationProvider PkceConfigurationProvider;

        /// <summary>
        /// The <see cref="CommonEmbedded.ServiceHostConfiguration"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IServiceHostResolver ServiceHostResolver;

        /// <summary>
        /// The <see cref="IAccessTokenExchanger{TInput,TOutput}"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IAccessTokenExchanger<string, UnityServicesToken> AccessTokenExchanger;

        /// <summary>
        /// The <see cref="IPkceRequestHandler"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IPkceRequestHandler PkceRequestHandler;

        /// <summary>
        /// The <see cref="IHttpClient"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IHttpClient HttpClient;

        /// <summary>
        /// The <see cref="IAppIdProvider"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IAppIdProvider AppIdProvider;

        /// <summary>
        /// The <see cref="IAppNamespaceProvider"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IAppNamespaceProvider AppNamespaceProvider;

        /// <summary>
        /// The <see cref="IJwtDecoder"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IJwtDecoder JwtDecoder;

        /// <summary>
        /// Creates a <see cref="PkceAuthenticatorSettings"/> to inject in a <see cref="PkceAuthenticator"/>.
        /// </summary>
        internal PkceAuthenticatorSettings(
            IAuthenticationPlatformSupport authenticationPlatformSupport,
            IPkceConfigurationProvider pkceConfigurationProvider,
            IPkceRequestHandler pkceRequestHandler,
            IAccessTokenExchanger<string, UnityServicesToken> accessTokenExchanger,
            IServiceHostResolver serviceHostResolver,
            IHttpClient httpClient,
            IAppIdProvider appIdProvider,
            IAppNamespaceProvider appNamespaceProvider,
            IJwtDecoder jwtDecoder
            )
        {
            AuthenticationPlatformSupport = authenticationPlatformSupport;
            PkceConfigurationProvider = pkceConfigurationProvider;
            PkceRequestHandler = pkceRequestHandler;
            AccessTokenExchanger = accessTokenExchanger;
            ServiceHostResolver = serviceHostResolver;
            HttpClient = httpClient;
            AppIdProvider = appIdProvider;
            AppNamespaceProvider = appNamespaceProvider;
            JwtDecoder = jwtDecoder;
        }
    }
}
