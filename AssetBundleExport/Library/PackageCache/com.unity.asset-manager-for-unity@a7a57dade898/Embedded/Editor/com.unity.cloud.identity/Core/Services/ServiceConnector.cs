using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// A struct holding authenticated services references.
    /// </summary>
    struct ServiceConnector
    {
        /// <summary>
        /// The resolved <see cref="ICompositeAuthenticator"/> instance.
        /// </summary>
        public readonly ICompositeAuthenticator CompositeAuthenticator;

        /// <summary>
        /// Create a <see cref="IServiceHttpClient"/> class holding authenticated services references.
        /// </summary>
        public readonly IServiceHttpClient ServiceHttpClient;

        /// <summary>
        /// The resolved <see cref="IServiceHostResolver"/> instance.
        /// </summary>
        public readonly IServiceHostResolver ServiceHostResolver;

        /// <summary>
        /// Create a <see cref="ServiceConnector"/> class holding authenticated services references.
        /// </summary>
        /// <param name="compositeAuthenticatorSettings">A built <see cref="CompositeAuthenticatorSettings"/> to inject in the <see cref="CompositeAuthenticator"/>.</param>
        /// <param name="serviceHostResolver">A <see cref="IServiceHostResolver"/> instance.</param>
        /// <param name="httpClient">A <see cref="IHttpClient"/> instance.</param>
        /// <param name="appIdProvider">A <see cref="IAppIdProvider"/> instance.</param>
        public ServiceConnector(CompositeAuthenticatorSettings compositeAuthenticatorSettings, IServiceHostResolver serviceHostResolver, IHttpClient httpClient, IAppIdProvider appIdProvider)
        {
            CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);
            ServiceHostResolver = serviceHostResolver;
            ServiceHttpClient = new ServiceHttpClient(httpClient, CompositeAuthenticator, appIdProvider);
        }
    }
}
