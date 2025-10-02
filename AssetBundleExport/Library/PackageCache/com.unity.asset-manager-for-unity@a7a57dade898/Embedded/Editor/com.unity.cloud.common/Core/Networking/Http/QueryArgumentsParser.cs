using System;
using System.Collections.Generic;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helper methods to parse query arguments from a <see cref="Uri"/>.
    /// </summary>
    static class QueryArgumentsParser
    {
        /// <summary>
        /// Parse the a Uri and returns a Dictionary of keys and values as string.
        /// </summary>
        /// <param name="uri">The Uri containing a query.</param>
        /// <param name="allowOverwrite">Whether to allow a query to be overwritten if it exists more than once.</param>
        /// <returns>The resulting <see cref="Dictionary{TKey,TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException"> Thrown when uri is null.</exception>
        public static Dictionary<string, string> GetDictionaryFromArguments(Uri uri, bool allowOverwrite = false)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            var queryArgs = new Dictionary<string, string>();
            if (uri.Query.Length > 1)
            {
                return GetDictionaryFromString(uri.Query.Substring(1), allowOverwrite);
            }

            return queryArgs;
        }

        /// <summary>
        /// Parse the a query and returns a Dictionary of keys and values as string.
        /// </summary>
        /// <param name="queryString">The query to parse.</param>
        /// <param name="allowOverwrite">Whether to allow a query to be overwritten if it exists more than once.</param>
        /// <returns>The resulting <see cref="Dictionary{TKey,TValue}"/>.</returns>
        public static Dictionary<string, string> GetDictionaryFromString(string queryString, bool allowOverwrite = false)
        {
            var queryArgs = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(queryString))
            {
                var keyValuePairs = queryString.Split('&');
                foreach (var keyValuePair in keyValuePairs)
                {
                    var splitStr = keyValuePair.Split('=');
                    // We only ingest once a query key in a query string overwrite is set to false
                    if (allowOverwrite || !queryArgs.ContainsKey(splitStr[0]))
                    {
                        queryArgs.Add(splitStr[0], splitStr.Length > 1 ? splitStr[1] : string.Empty);
                    }
                }
            }

            return queryArgs;
        }
    }
}
