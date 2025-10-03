using System;
using System.Runtime.InteropServices;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// Interoperability methods for WebGL browser functionality.
    /// </summary>
    static class CommonBrowserInterop
    {
#if UNITY_WEBGL
        /// <summary>
        /// Get the URL from the page.
        /// </summary>
        /// <returns>The URL.</returns>
        [DllImport("__Internal")]
        public static extern string GetURLFromPage();

        /// <summary>
        /// Get the query parameter by id.
        /// </summary>
        /// <param name="paramId">The query parameter id.</param>
        /// <returns>The query parameter.</returns>
        [DllImport("__Internal")]
        public static extern string GetQueryParam(string paramId);

        /// <summary>
        /// Copy a string value to the clipboard.
        /// </summary>
        /// <param name="value">The value to copy.</param>
        /// <returns>Whether the copy was successful.</returns>
        [DllImport("__Internal")]
        public static extern bool CopyToClipboard(string value);

        /// <summary>
        /// Store the value in the browser cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        [DllImport("__Internal")]
        public static extern void CacheValue(string key, string value);

        /// <summary>
        /// Clear the value from the browser cache.
        /// </summary>
        /// <param name="key">The key to clear.</param>
        [DllImport("__Internal")]
        public static extern void ClearCache(string key);

        /// <summary>
        /// Stores the authorization cookie in the browser.
        /// </summary>
        /// <param name="token">The authorization cookie.</param>
        [DllImport("__Internal")]
        public static extern void SaveAuthorizationCookie(string token);

        /// <summary>
        /// Navigate to a specific URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="windowId">The window id.</param>
        [DllImport("__Internal")]
        public static extern void Navigate(string url, string windowId = "_self");

        /// <summary>
        /// Retrieve a value from the browser cache.
        /// </summary>
        /// <param name="key">The key for the value to retrieve.</param>
        /// <returns>The value to retrieve.</returns>
        [DllImport("__Internal")]
        public static extern string RetrieveCachedValue(string key);
#else
        /// <summary>
        /// Get the URL from the page.
        /// </summary>
        /// <returns>The URL.</returns>
        public static string GetURLFromPage() => throw new PlatformNotSupportedException();

        /// <summary>
        /// Get the query parameter by id.
        /// </summary>
        /// <param name="paramId">The query parameter id.</param>
        /// <returns>The query parameter.</returns>
        public static string GetQueryParam(string paramId) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Copy a string value to the clipboard.
        /// </summary>
        /// <param name="value">The value to copy.</param>
        /// <returns>Whether the copy was successful.</returns>
        public static bool CopyToClipboard(string value) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Store the value in the browser cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void CacheValue(string key, string value) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Clear the value from the browser cache.
        /// </summary>
        /// <param name="key">The key to clear.</param>
        public static void ClearCache(string key) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Stores the authorization cookie in the browser.
        /// </summary>
        /// <param name="token">The authorization cookie.</param>
        public static void SaveAuthorizationCookie(string token) => throw new PlatformNotSupportedException();

        /// <summary>
        /// Navigate to a specific URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="windowId">The window id.</param>
        public static void Navigate(string url, string windowId = "_self") => throw new PlatformNotSupportedException();

        /// <summary>
        /// Retrieve a value from the browser cache.
        /// </summary>
        /// <param name="key">The key for the value to retrieve.</param>
        /// <returns>The value to retrieve.</returns>
        public static string RetrieveCachedValue(string key) => throw new PlatformNotSupportedException();
#endif
    }
}
