using System;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.CommonEmbedded;
using UnityEngine;

namespace Unity.Cloud.IdentityEmbedded.Runtime
{
    /// <summary>
    /// This class contains Windows standalone platform-specific logic to handle app activation from an url or key value pairs.
    /// </summary>
    class WindowsActivatePlatformSupport : BasePkcePlatformSupport
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<WindowsActivatePlatformSupport>();

        /// <inheritdoc/>
        public WindowsActivatePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, activationUrl)
        {
            // Check deep link on startup
            var launchArgumentsParser = new LaunchArgumentsParser();

            if (Uri.TryCreate(launchArgumentsParser.ActivationUrl, UriKind.Absolute, out Uri _))
            {
                s_Logger.LogDebug($"App was activated from url: {launchArgumentsParser.ActivationUrl}");
                ActivationUrl = launchArgumentsParser.ActivationUrl;
            }

            // Could hold query params from ActivationURL or launch arguments
            ActivationKeyValue = launchArgumentsParser.ActivationKeyValues;
        }
    }

    /// <summary>
    /// This class handles Windows standalone platform-specific features in the authentication flow.
    /// </summary>
    class WindowsPkcePlatformSupport : WindowsActivatePlatformSupport
    {
        /// <summary>
        /// Get a string value override for the default random state used in the authentication flow.
        /// </summary>
        /// <returns>
        /// A string value corresponding to the main window pointer of the app.
        /// </returns>
        public override string GetAppStateOverride() => UrlRedirectionInterceptor.GetRedirectProcessId().ToString();

        /// <inheritdoc/>
        public WindowsPkcePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, activationUrl)
        {
        }
    }
}
