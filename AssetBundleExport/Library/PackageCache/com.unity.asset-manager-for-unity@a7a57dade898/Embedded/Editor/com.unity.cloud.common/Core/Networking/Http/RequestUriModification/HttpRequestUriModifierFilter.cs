using System;
using System.Text.RegularExpressions;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Filters a request URI based on a given schema to determine if it should be modified.
    /// </summary>
    class HttpRequestUriModifierFilter
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<HttpRequestUriModifierFilter>();

        readonly HttpRequestUriModifierFilterSchema m_Schema;

        readonly Regex m_IsMatchRegex;

        readonly bool m_Valid;

        /// <summary>
        /// Creates ans instance of <see cref="HttpRequestUriModifierFilter"/> from a given schema.
        /// </summary>
        /// <param name="schema">The schema for URI modification.</param>
        public HttpRequestUriModifierFilter(HttpRequestUriModifierFilterSchema schema)
        {
            m_Schema = schema;
            m_Valid = true;

            try
            {
                m_IsMatchRegex = new Regex(schema.IsMatch, RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            }
            catch (ArgumentException)
            {
                s_Logger.LogError($"Unable to create regex expression from isMatch {schema.IsMatch}");
                m_Valid = false;
            }
        }

        /// <summary>
        /// Filters the request URI based on the schema.
        /// </summary>
        /// <param name="requestUri">The request URI</param>
        /// <returns>Whether the request URI matches the schema's filter.</returns>
        public bool Filter(Uri requestUri)
        {
            if (!m_Valid)
                return false;

            var uriPropertyValue = GetUriPropertyValue(requestUri, m_Schema.UriProperty);
            return uriPropertyValue != null && m_IsMatchRegex.IsMatch(uriPropertyValue);
        }

        static string GetUriPropertyValue(Uri uri, string propertyName)
        {
            var propertyInfo = uri.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                var propertyValue = propertyInfo.GetValue(uri);
                if (propertyValue != null)
                    return propertyValue.ToString();
            }

            return null;
        }
    }
}
