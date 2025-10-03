using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// This class contains platform shared logic to handle the authentication flow.
    /// </summary>
    class BasePkcePlatformSupport : IAuthenticationPlatformSupport
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<BasePkcePlatformSupport>();

        /// <inheritdoc/>
        public virtual IUrlRedirectionInterceptor UrlRedirectionInterceptor { get; internal set; }

        /// <inheritdoc cref="Unity.Cloud.AppLinkingEmbedded.IActivatePlatformSupport"/>
        public virtual string HostUrl { get; protected set; }

        /// <inheritdoc cref="Unity.Cloud.AppLinkingEmbedded.IActivatePlatformSupport"/>
        public virtual string ActivationUrl { get; protected set; }

        /// <inheritdoc cref="Unity.Cloud.AppLinkingEmbedded.IActivatePlatformSupport"/>
        public virtual Dictionary<string, string> ActivationKeyValue { get; protected set; }

        string m_CacheStorePath { get; }

        /// <inheritdoc/>
        public virtual string GetAppStateOverride() => null;

        /// <inheritdoc/>
        public virtual IKeyValueStore SecretCacheStore { get; }

        /// <inheritdoc/>
        public virtual IKeyValueStore CodeVerifierCacheStore { get; } = null;

        /// <summary>
        /// The url used to initiate a login operation in the default OS browser.
        /// </summary>
        protected string m_LoginUrl = string.Empty;

        /// <summary>
        /// The <see cref="IUrlProcessor"/> that manages url processing after interception.
        /// </summary>
        protected readonly IUrlProcessor m_UrlProcessor;
        readonly IAppIdProvider m_AppIdProvider;
        readonly IAppNamespaceProvider m_AppNamespaceProvider;

        /// <summary>
        /// Creates a BasePkcePlatformSupport that handles app activation from an url or key value pairs.
        /// </summary>
        /// <param name="urlRedirectionInterceptor">An <see cref="IUrlRedirectionInterceptor"/> that manages url redirection interception.</param>
        /// <param name="urlProcessor">An <see cref="IUrlProcessor"/> that manages url processing after interception.</param>
        /// <param name="appIdProvider">An <see cref="IAppIdProvider"/> instance.</param>
        /// <param name="appNamespaceProvider">An <see cref="IAppNamespaceProvider"/> instance.</param>
        /// <param name="cacheStorePath">A full path to a readable/writable directory.</param>
        /// <param name="activationUrl">An optional activation URL.</param>
        public BasePkcePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string cacheStorePath, string activationUrl = null)
        {
            m_UrlProcessor = urlProcessor;
            m_AppIdProvider = appIdProvider;
            m_AppNamespaceProvider = appNamespaceProvider;
            m_CacheStorePath = cacheStorePath;

            var appIdString = m_AppIdProvider.GetAppId().ToString();
            SecretCacheStore = new FileKeyValueStore(m_CacheStorePath, new AesStringObfuscator(!string.IsNullOrEmpty(appIdString) ? appIdString : "default"));
            if (!string.IsNullOrEmpty(activationUrl) && Uri.TryCreate(activationUrl, UriKind.Absolute, out Uri _))
            {
                s_Logger.LogDebug($"App was activated from url: {activationUrl}");
                ActivationUrl = activationUrl;
            }

            // Could hold query params from ActivationURL
            ActivationKeyValue = new Dictionary<string, string>();

            UrlRedirectionInterceptor = urlRedirectionInterceptor;
        }

        /// <inheritdoc/>
        public virtual async Task<UrlRedirectResult> OpenUrlAndWaitForRedirectAsync(string url, List<string> awaitedQueryArguments = null)
        {
            m_LoginUrl = url;
            s_Logger.LogDebug($"Awaiting redirect on url: {url}");

            OpenUrlAction(url);
            await Task.Delay(50);
            return await UrlRedirectionInterceptor.AwaitRedirectAsync(awaitedQueryArguments);
        }

        /// <inheritdoc/>
        public virtual void ExportServiceAuthorizerToken(string type, string token)
        {
        }

        void OpenUrlAction(string url)
        {
            if (m_UrlProcessor != null)
            {
                m_UrlProcessor.ProcessURL(url);
            }
        }

        /// <inheritdoc/>
        public virtual string GetRedirectUri(string operation = null)
        {
            var operationPath = string.IsNullOrEmpty(operation) ? string.Empty : $"/{operation}";
            return $"{m_AppNamespaceProvider.GetAppNamespace()}://implicit/callback{operationPath}";
        }

        /// <inheritdoc/>
        public virtual Task<string> GetRedirectUriAsync(string operation = null)
        {
            return Task.FromResult(GetRedirectUri(operation));
        }

        /// <inheritdoc/>
        public virtual string GetCancellationUri()
        {
            if (string.IsNullOrEmpty(m_LoginUrl))
                throw new InvalidOperationException("No cancellation Uri available. Awaiting login operation to be initiated.");

            var loginHost = new Uri(m_LoginUrl).Host;
            return $"https://{loginHost}?code=none&state=cancelled";
        }

        /// <inheritdoc/>
        public virtual void ProcessActivationUrl(List<string> awaitedQueryArguments = null)
        {
            if (!string.IsNullOrEmpty(ActivationUrl))
            {
                UrlRedirectionInterceptor.InterceptAwaitedUrl(ActivationUrl, awaitedQueryArguments);
                // Only process once
                ActivationUrl = string.Empty;
            }
        }

        /// <inheritdoc/>
        public virtual UrlRedirectResult? GetRedirectionResult()
        {
            return null;
        }
    }
}
