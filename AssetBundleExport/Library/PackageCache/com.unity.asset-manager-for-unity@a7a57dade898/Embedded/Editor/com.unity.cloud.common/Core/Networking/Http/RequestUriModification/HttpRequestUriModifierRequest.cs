using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Modifies a request URI if it matches a filter based on a provided schema.
    /// </summary>
    class HttpRequestUriModifierRequest
    {
        readonly HttpRequestUriModifierFilter m_Filter;
        readonly HttpRequestUriModifierReplace m_Replace;

        /// <summary>
        /// Creates an instance of <see cref="HttpRequestUriModifierRequest"/> based on a provided schema.
        /// </summary>
        /// <param name="schema">The schema for URI modification.</param>
        public HttpRequestUriModifierRequest(HttpRequestUriModifierRequestSchema schema)
        {
            m_Filter = new HttpRequestUriModifierFilter(schema.Filter);
            m_Replace = new HttpRequestUriModifierReplace(schema.Replace);
        }

        /// <summary>
        /// Will modify the request URI if it matches the filter.
        /// </summary>
        /// <param name="requestUri">The request URI to modify.</param>
        /// <param name="modifiedUri">The modified URI. Null if no modification was applied.</param>
        /// <returns>Whether the request URI was modified.</returns>
        public bool TryModify(Uri requestUri, out Uri modifiedUri)
        {
            modifiedUri = null;

            if (m_Filter.Filter(requestUri))
            {
                modifiedUri = m_Replace.Replace(requestUri);

                return true;
            }

            return false;
        }
    }
}
