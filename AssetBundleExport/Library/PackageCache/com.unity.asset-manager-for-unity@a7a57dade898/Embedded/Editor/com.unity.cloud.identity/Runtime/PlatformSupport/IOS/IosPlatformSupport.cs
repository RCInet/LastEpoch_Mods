using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.CommonEmbedded;
using UnityEngine;

namespace Unity.Cloud.IdentityEmbedded.Runtime
{

    /// <summary>
    /// This class handles iOS platform-specific features in the authentication flow.
    /// </summary>
    class IosPkcePlatformSupport : BasePkcePlatformSupport
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void LaunchCaptiveSafariWebViewUrl(string url);
        [DllImport("__Internal")]
        static extern void DismissCaptiveSafariWebView();
#endif
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<IosPkcePlatformSupport>();

        /// <inheritdoc/>
        public IosPkcePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, activationUrl)
        {
        }

        /// <summary>
        /// Creates an awaitable Task that opens an url in a browser and completes when response is intercepted, validated and returns a <see cref="UrlRedirectResult"/>.
        /// </summary>
        /// <param name="url">The url to open. It must trigger a redirection to the Uri referenced by <see cref="BasePkcePlatformSupport.GetRedirectUri"/>.</param>
        /// <param name="awaitedQueryArguments">The list of query arguments to validate when receiving the awaited callback url.</param>
        /// <returns>
        /// A Task that results in a <see cref="UrlRedirectResult"/> when completed.
        /// </returns>
        public override async Task<UrlRedirectResult> OpenUrlAndWaitForRedirectAsync(string url, List<string> awaitedQueryArguments = null)
        {
            m_LoginUrl = url;
            s_Logger.LogDebug($"Awaiting redirect on url: {url}");
#if UNITY_IOS && !UNITY_EDITOR
            // Append extra parameters to remove cookie banner in Genesis login page and avoid App Tracking Authorization requirements from Apple.
            LaunchCaptiveSafariWebViewUrl($"{url}&extra_hide_cookie=true&extra_hide_onetrust=true");
#endif
            await Task.Delay(50);
            var result = await UrlRedirectionInterceptor.AwaitRedirectAsync(awaitedQueryArguments);

#if UNITY_IOS && !UNITY_EDITOR
            DismissCaptiveSafariWebView();
#endif

            return result;
        }
    }
}
