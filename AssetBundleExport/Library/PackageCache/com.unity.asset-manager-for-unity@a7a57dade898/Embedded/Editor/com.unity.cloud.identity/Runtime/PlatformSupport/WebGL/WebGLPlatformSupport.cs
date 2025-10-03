using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.CommonEmbedded;
using Unity.Cloud.CommonEmbedded.Runtime;
using UnityEngine;

namespace Unity.Cloud.IdentityEmbedded.Runtime
{
    /// <summary>
    /// This class contains WebGL platform-specific logic to handle app activation from an url or key value pairs.
    /// </summary>
    class WebglActivatePlatformSupport : BasePkcePlatformSupport
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<WebglActivatePlatformSupport>();

        /// <inheritdoc/>
        public WebglActivatePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, activationUrl)
        {
            ActivationKeyValue = new Dictionary<string, string>();
            if (Uri.TryCreate(activationUrl, UriKind.Absolute, out Uri uri) && !string.IsNullOrEmpty(uri.Query))
            {
                s_Logger.LogDebug($"App was activated from url: {activationUrl}");
                ActivationUrl = activationUrl;
                ActivationKeyValue = QueryArgumentsParser.GetDictionaryFromArguments(uri);
            }
            HostUrl = string.Empty;
#if UNITY_WEBGL
            HostUrl = CommonBrowserInterop.GetURLFromPage();
#endif
        }
    }

    /// <summary>
    /// This class handles WebGL platform-specific features in the authentication flow.
    /// </summary>
    class WebglPkcePlatformSupport : WebglActivatePlatformSupport
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<WebglPkcePlatformSupport>();

        static readonly string s_CachedActivationUrl = "cached_activation_url";

        /// <inheritdoc/>
        public override IKeyValueStore SecretCacheStore { get; } = new BrowserKeyValueStore();

        /// <inheritdoc/>
        public override IKeyValueStore CodeVerifierCacheStore { get; } = new BrowserKeyValueStore();

        /// <inheritdoc/>
        public WebglPkcePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, activationUrl)
        {
        }

        /// <summary>
        /// Creates an awaitable Task that opens an url in a browser and completes when WebGL host page is reloaded and login is intercepted, validated and returns a <see cref="UrlRedirectResult"/>.
        /// </summary>
        /// <param name="url">The url to open. It must trigger a redirection to the Uri referenced by <see cref="GetRedirectUri"/>.</param>
        /// <param name="awaitedQueryArguments">The list of query arguments to validate when receiving the awaited callback url.</param>
        /// <returns>
        /// A Task that results in a <see cref="UrlRedirectResult"/>  when completed.
        /// </returns>
        public override async Task<UrlRedirectResult> OpenUrlAndWaitForRedirectAsync(string url, List<string> awaitedQueryArguments = null)
        {
            s_Logger.LogDebug($"Awaiting redirect on url: {url}");

            // If login while an ActivationUrl has not been consumed
            if (!string.IsNullOrEmpty(ActivationUrl) && !ActivationUrlHasCodeAndStateParams(ActivationUrl))
            {
                await SecretCacheStore.WriteToCacheAsync(s_CachedActivationUrl, ActivationUrl);
            }
#if UNITY_WEBGL
            CommonBrowserInterop.Navigate(url);
#endif
            var urlRedirectResult = new UrlRedirectResult
            {
                Status = UrlRedirectStatus.NotApplicable
            };
            return urlRedirectResult;
        }

        /// <inheritdoc/>
        public override void ExportServiceAuthorizerToken(string type, string token)
        {
#if UNITY_WEBGL
            CommonBrowserInterop.SaveAuthorizationCookie(token);
#endif
        }

        bool ActivationUrlHasCodeAndStateParams(string activationUrl)
        {
            var uriQuery = new Uri(activationUrl).Query;
            if (string.IsNullOrEmpty(uriQuery))
            {
                return false;
            }
            var queryArgs = QueryArgumentsParser.GetDictionaryFromString(uriQuery.Substring(1));
            return queryArgs.ContainsKey("state") && queryArgs.ContainsKey("code");
        }

        /// <summary>
        /// On WebGL this method will return the <see cref="UrlRedirectResult"/> captured at app initializing time, or null if none available.
        /// </summary>
        /// <returns>
        /// The <see cref="UrlRedirectResult"/> captured at app initializing time, or null if none available.
        /// </returns>
        public override UrlRedirectResult? GetRedirectionResult()
        {
            return UrlRedirectionInterceptor.GetRedirectionResult();
        }

        /// <inheritdoc/>
        public override string GetRedirectUri(string operation = null)
        {
            var urlString = HostUrl;
            if (urlString.IndexOf('?') != -1)
            {
                urlString = urlString.Split('?')[0];
            }
            Debug.Log($"urlString FROM HOST: {urlString}");
            return urlString;
        }

        /// <inheritdoc/>
        public override Task<string> GetRedirectUriAsync(string operation = null)
        {
            return Task.FromResult(GetRedirectUri(operation));
        }


        /// <summary>
        /// Get the cancellation Uri expected from the browser when calling <see cref="OpenUrlAndWaitForRedirectAsync"/>.
        /// </summary>
        /// <returns>
        /// The cancellation Uri expected from the browser when calling <see cref="OpenUrlAndWaitForRedirectAsync"/>.
        /// </returns>
        public override string GetCancellationUri()
        {
            throw new NotSupportedException("By design, WebGL platform does not support login cancellation.");
        }

        /// <summary>
        /// Process activation url to complete login or consume an authenticated app resource.
        /// </summary>
        /// <param name="awaitedQueryArguments">The list of query arguments to validate when processing the activation url.</param>
        public override void ProcessActivationUrl(List<string> awaitedQueryArguments = null)
        {
            if (!string.IsNullOrEmpty(ActivationUrl))
            {
                UrlRedirectionInterceptor.InterceptAwaitedUrl(ActivationUrl, awaitedQueryArguments);
                // Only process once
                ActivationUrl = string.Empty;
            }
        }
    }
}
