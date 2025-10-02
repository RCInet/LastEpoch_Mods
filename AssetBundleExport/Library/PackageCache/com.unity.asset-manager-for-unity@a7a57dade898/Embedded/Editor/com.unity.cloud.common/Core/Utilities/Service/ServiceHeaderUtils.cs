using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helper methods for managing HTTP headers.
    /// </summary>
    static class ServiceHeaderUtils
    {
        /// <summary>
        /// Header for the API source information.
        /// </summary>
        public const string k_ApiSourceHeader = "X-Unity-Cloud-Api-Source";

        /// <summary>
        /// Header for the client-id information.
        /// </summary>
        internal const string k_ClientIdHeader = "X-Client-Id";

        /// <summary>
        /// Header value for the client-id information.
        /// </summary>
        internal const string k_ClientIdHeaderValue = "unity-cloud-sdk";

        /// <summary>
        /// A regex pattern matching Unity API URLs.
        /// </summary>
        public const string k_UnityApiPattern = @"https.*(?:[./])unity\.com/api/.*|localhost:.*\/api/.*|https.*(?:[./])services.api.unity.com";

        /// <summary>
        /// The bearer scheme for authorization.
        /// </summary>
        public const string k_BearerScheme = "Bearer";

        /// <summary>
        /// The basic scheme for authorization.
        /// </summary>
        public const string k_BasicScheme = "Basic";

        const string k_AuthHeader = "Authorization";
        const string k_AppIdHeader = "X-Unity-Cloud-AppId";
        const string k_ClientTraceHeader = "X-Unity-Cloud-ClientTrace";
        const string k_TraceHeader = "X-Unity-Cloud-Trace";
        const string k_TraceEnvVarName = "UNITY_CLOUD_TRACE";

        const string k_DeprecatedClientTraceHeader = "X-Digital-Twins-ClientTrace";

        static Dictionary<string, string> s_HeaderToQueryMapping = new()
        {
            {k_AuthHeader, "authorization"},
            {k_AppIdHeader, "app_id"},
            {k_ClientTraceHeader, "client_trace"},
            {k_DeprecatedClientTraceHeader, "client_trace"},
        };

        /// <summary>
        /// Add HTTP headers to the specified Uri as query arguments.
        /// </summary>
        /// <param name="uri">The Uri to add HTTP headers as queries to.</param>
        /// <param name="headers">The HTTP headers to append as queries.</param>
        /// <returns>The modified Uri.</returns>
        public static Uri AddHeadersAsQuery(this Uri uri, HttpHeaders headers)
        {
            var uriBuilder = new UriBuilder(uri);
            if (headers != null)
            {
                var query = uriBuilder.Query;
                var queryPrefix = !string.IsNullOrWhiteSpace(query) ? "&" : "?";

                foreach (var (name, values) in headers)
                {
                    if (s_HeaderToQueryMapping.TryGetValue(name, out var queryName))
                    {
                        var escapedValue = Uri.EscapeUriString(values.Aggregate((v1, v2) => $"{v1},{v2}"));

                        if (name == k_AuthHeader)
                        {
                            if (escapedValue.Contains(k_BearerScheme))
                                escapedValue = escapedValue.Remove(0, k_BearerScheme.Length);
                            else if (escapedValue.Contains(k_BasicScheme))
                                escapedValue = escapedValue.Remove(0, k_BasicScheme.Length);
                        }

                        query += $"{queryPrefix}{queryName}={escapedValue}";

                        queryPrefix = "&";
                    }
                }

                uriBuilder.Query = query;
            }
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Adds the HTTP headers with specific values for app Id and client trace.
        /// </summary>
        /// <param name="headers">The HTTP headers to add to.</param>
        /// <param name="appId">The app Id.</param>
        /// <param name="clientTrace">The client trace.</param>
        public static void AddAppIdAndClientTrace(this HttpHeaders headers, AppId appId, string clientTrace)
        {
            var appIdString = appId.ToString();
            if (!string.IsNullOrEmpty(appIdString))
            {
                if (!headers.Contains(k_AppIdHeader))
                    headers.Add(k_AppIdHeader, appIdString);
            }

            if (!string.IsNullOrEmpty(clientTrace))
            {
                if (!headers.Contains(k_ClientTraceHeader))
                    headers.Add(k_ClientTraceHeader, clientTrace);

                if (!headers.Contains(k_DeprecatedClientTraceHeader))
                    headers.Add(k_DeprecatedClientTraceHeader, clientTrace);
            }

            // Service Gateway requires X-Client-Id for tracking purpose
            headers.Add(k_ClientIdHeader, k_ClientIdHeaderValue);

            // Value of user's trace environment variable
            var envTraceId = Environment.GetEnvironmentVariable(k_TraceEnvVarName);
            if (!string.IsNullOrEmpty(envTraceId))
            {
                if(!headers.Contains(k_TraceHeader))
                    headers.Add(k_TraceHeader, envTraceId);
            }
        }

        /// <summary>
        /// Add the HTTP header with a specific value for authorization.
        /// </summary>
        /// <param name="headers">The HTTP headers to add to.</param>
        /// <param name="authValue">The authorization value.</param>
        /// <param name="authScheme">The authorization scheme. Set to "Bearer" by default.</param>
        public static void AddAuthorization(this HttpHeaders headers, string authValue, string authScheme)
        {
            if (headers is HttpRequestHeaders casted)
            {
                if (!casted.Contains(k_AuthHeader))
                    casted.Authorization = new AuthenticationHeaderValue(authScheme, authValue);
            }
            else
            {
                if (!headers.Contains(k_AuthHeader))
                    headers.Add(k_AuthHeader, $"{authScheme} {authValue}");
            }
        }

        /// <summary>
        /// Returns the data contained in the <see cref="ApiSourceVersion"/> formatted as a string for the HTTP header value.
        /// </summary>
        /// <param name="apiSourceVersion">The version information with which to generate the header value.</param>
        /// <returns>The contents <see cref="ApiSourceVersion"/> formatted as a string for the HTTP header value.</returns>
        public static string GetHeaderValue(this ApiSourceVersion apiSourceVersion) => $"{apiSourceVersion.Name}@{apiSourceVersion.Version}";


        /// <summary>
        /// Returns whether the specified URL is a Unity API URL.
        /// </summary>
        /// <param name="url">The url to verify.</param>
        /// <returns>Whether the specified URL is a Unity API URL.</returns>
        /// <remarks>Some custom headers should only be added to requests to Unity APIs.</remarks>
        internal static bool IsUnityApi(string url)
        {
            return Regex.IsMatch(url, k_UnityApiPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }
        /// <summary>
        /// Returns whether the specified URI is a Unity API URL.
        /// </summary>
        /// <param name="uri">The url to verify.</param>
        /// <returns>Whether the specified URI is a Unity API URL.</returns>
        /// <remarks>Some custom headers should only be added to requests to Unity APIs.</remarks>
        internal static bool IsUnityApi(Uri uri)
        {
            return IsUnityApi(uri.ToString());
        }
    }
}

