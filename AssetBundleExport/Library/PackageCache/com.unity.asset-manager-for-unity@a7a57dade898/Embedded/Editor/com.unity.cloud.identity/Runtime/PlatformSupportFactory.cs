using System;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.AppLinkingEmbedded.Runtime;
using UnityEngine;
using Unity.Cloud.CommonEmbedded;
using Unity.Cloud.CommonEmbedded.Runtime;

namespace Unity.Cloud.IdentityEmbedded.Runtime
{

    /// <summary>
    /// A static factory that handles instantiation of platform-specific IActivatePlatformSupport and IAuthenticationPlatformSupport.
    /// </summary>
    static class PlatformSupportFactory
    {
        /// <summary>
        /// A static factory that handles instantiation of a platform-specific <see cref="IAuthenticationPlatformSupport"/>.
        /// </summary>
        /// <param name="urlRedirectionInterceptor">An optional <see cref="IUrlRedirectionInterceptor"/> instance.</param>
        /// <param name="appIdProvider">An optional <see cref="IAppIdProvider"/> instance.</param>
        /// <param name="appNamespaceProvider">An optional <see cref="IAppNamespaceProvider"/> instance.</param>
        /// <param name="cacheStorePath">An optional full path to a readable/writable directory.</param>
        /// <returns>
        /// A platform-specific <see cref="IAuthenticationPlatformSupport"/> instance.
        /// </returns>
        /// <exception cref="NotImplementedException">Throws a NotImplementedException if current execution platform cannot be determined.</exception>
        public static IAuthenticationPlatformSupport GetAuthenticationPlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor = null, IAppIdProvider appIdProvider = null, IAppNamespaceProvider appNamespaceProvider = null, string cacheStorePath = null)
        {
            urlRedirectionInterceptor ??= UrlRedirectionInterceptor.GetInstance();

            appIdProvider ??= UnityCloudPlayerSettings.Instance;
            appNamespaceProvider ??= UnityCloudPlayerSettings.Instance;
            cacheStorePath ??= Application.persistentDataPath;

            IUrlProcessor urlProcessor = new UnityRuntimeUrlProcessor();

#if UNITY_EDITOR
            return new EditorPkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath);
#elif UNITY_STANDALONE_WIN
            return new WindowsPkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath);
#elif UNITY_STANDALONE_OSX
            return new BasePkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, Application.absoluteURL);
#elif UNITY_STANDALONE_LINUX
            return new BasePkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, Application.absoluteURL);
#elif UNITY_IOS
            return new IosPkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, Application.absoluteURL);
#elif UNITY_ANDROID
            return new BasePkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, Application.absoluteURL);
#elif UNITY_WEBGL
            return new WebglPkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, Application.absoluteURL);
#else
            throw new NotImplementedException("No PKCE platform support found for the current platform.");
#endif
        }
    }
}
