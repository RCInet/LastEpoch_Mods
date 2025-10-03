using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Creates the <see cref="ServiceAccountAuthenticatorSettings"/> required to inject in a <see cref="ServiceAccountAuthenticator"/>.
    /// </summary>
    struct ServiceAccountAuthenticatorSettings
    {
        /// <summary>
        /// The <see cref="IAuthenticationPlatformSupport"/> to use to intercept activation url.
        /// </summary>
        internal readonly IAuthenticationPlatformSupport AuthenticationPlatformSupport;

        /// <summary>
        /// The <see cref="IServiceHostResolver"/> to use to resolve the service host.
        /// </summary>
        internal readonly IServiceHostResolver ServiceHostResolver;

        /// <summary>
        /// The <see cref="IAccessTokenExchanger{TInput,TOutput}"/> to use to exchange the Service Account Credentials for a JWT.
        /// </summary>
        internal readonly IAccessTokenExchanger<ServiceAccountCredentials, UnityServicesToken> AccessTokenExchanger;

        /// <summary>
        /// The <see cref="IHttpClient"/> to use for HTTP requests.
        /// </summary>
        internal readonly IHttpClient HttpClient;

        /// <summary>
        /// The <see cref="IAppIdProvider"/> to use to inject header in the <see cref="ServiceHttpClient"/>.
        /// </summary>
        internal readonly IAppIdProvider AppIdProvider;

        /// <summary>
        /// The <see cref="IJwtDecoder"/> to use to decode JWT.
        /// </summary>
        internal readonly IJwtDecoder JwtDecoder;

        /// <summary>
        /// Creates a <see cref="ServiceAccountAuthenticatorSettings"/> to inject in a <see cref="ServiceAccountAuthenticator"/>.
        /// </summary>
        internal ServiceAccountAuthenticatorSettings(

            IAuthenticationPlatformSupport authenticationPlatformSupport,
            IAccessTokenExchanger<ServiceAccountCredentials, UnityServicesToken> accessTokenExchanger,
            IServiceHostResolver serviceHostResolver,
            IHttpClient httpClient,
            IAppIdProvider appIdProvider,
            IJwtDecoder jwtDecoder
            )
        {
            AuthenticationPlatformSupport = authenticationPlatformSupport;
            AccessTokenExchanger = accessTokenExchanger;
            ServiceHostResolver = serviceHostResolver;
            HttpClient = httpClient;
            AppIdProvider = appIdProvider;
            JwtDecoder = jwtDecoder;
        }
    }
}
