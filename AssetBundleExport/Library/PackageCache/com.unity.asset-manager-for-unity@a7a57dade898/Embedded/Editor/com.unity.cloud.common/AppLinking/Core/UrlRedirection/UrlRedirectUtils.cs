using System;
using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <summary>
    /// Utilities for url redirection.
    /// </summary>
    static class UrlRedirectUtils
    {
        /// <summary>
        /// Returns whether the url has all the awaited query arguments.
        /// </summary>
        /// <param name="queryArgumentDictionary">The URL's query arguments.</param>
        /// <param name="awaitedQueryArguments">The query arguments being awaited.</param>
        /// <returns>Whether the URL has the awaited arguments.</returns>
        public static bool UrlHasAwaitedQueryArguments(Dictionary<string, string> queryArgumentDictionary, List<string> awaitedQueryArguments)
        {
            if (awaitedQueryArguments == null)
            {
                return true;
            }
            var hasAllQueryArguments = true;
            foreach (var keyName in awaitedQueryArguments)
            {
                if (!queryArgumentDictionary.ContainsKey(keyName))
                {
                    hasAllQueryArguments = false;
                    break;
                }
            }
            return hasAllQueryArguments;
        }

        /// <summary>
        /// Verifies if the url is valid.
        /// </summary>
        /// <param name="url">The url to validate.</param>
        /// <param name="uri">The resulting <see cref="Uri"/>.</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="url"/> is empty, null, or invalid.</exception>
        public static void ValidateUrlArgument(string url, out Uri uri)
        {
            if (url == null)
                throw new ArgumentException("The url cannot be null.", nameof(url));

            if (string.IsNullOrEmpty(url?.Trim()))
                throw new ArgumentException("The url cannot be empty.", nameof(url));

            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                throw new ArgumentException("The url is not a valid uri.", nameof(url));
        }

        /// <summary>
        /// Tries to intercept the redirection url for specific query arguments.
        /// </summary>
        /// <param name="uri">The uri to intercept.</param>
        /// <param name="awaitedQueryArguments">The awaited query arguemtns.</param>
        /// <param name="urlRedirectResult">The redirection result.</param>
        /// <returns>Whether the interception succeeded.</returns>
        public static bool TryInterceptRedirectionUrl(Uri uri, List<string> awaitedQueryArguments, out UrlRedirectResult urlRedirectResult)
        {
            urlRedirectResult = default;
            var queryArgs = QueryArgumentsParser.GetDictionaryFromArguments(uri);
            if (awaitedQueryArguments != null && UrlHasAwaitedQueryArguments(queryArgs, awaitedQueryArguments))
            {
                urlRedirectResult = new UrlRedirectResult
                {
                    Status = UrlRedirectStatus.Success,
                    QueryArguments = queryArgs
                };
                return true;
            }
            return false;
        }
    }
}
