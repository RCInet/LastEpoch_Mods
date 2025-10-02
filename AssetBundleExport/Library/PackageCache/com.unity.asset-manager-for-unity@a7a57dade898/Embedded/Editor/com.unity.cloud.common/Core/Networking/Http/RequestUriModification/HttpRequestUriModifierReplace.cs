using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Replaces the properties of a request URI based on a given schema.
    /// </summary>
    class HttpRequestUriModifierReplace
    {
        readonly HttpRequestUriModifierReplaceSchema m_Schema;

        /// <summary>
        /// Creates ans instance of <see cref="HttpRequestUriModifierReplace"/> from a given schema.
        /// </summary>
        /// <param name="schema">The schema for URI modification.</param>
        public HttpRequestUriModifierReplace(HttpRequestUriModifierReplaceSchema schema)
        {
            m_Schema = schema;
        }

        /// <summary>
        /// Replaces the properties of a request URI based on a given schema.
        /// </summary>
        /// <param name="requestUri">The request URI</param>
        /// <returns>The modified URI</returns>
        public Uri Replace(Uri requestUri)
        {
            var newUri = new UriBuilder(requestUri);
            if (!string.IsNullOrEmpty(m_Schema.Host))
                newUri.Host = m_Schema.Host;

            if (m_Schema.Port > 0)
                newUri.Port = m_Schema.Port;

            if (!string.IsNullOrEmpty(m_Schema.Scheme))
                newUri.Scheme = m_Schema.Scheme;

            return newUri.Uri;
        }
    }
}
