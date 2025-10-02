using System;
using System.Collections.Generic;

namespace Unity.Cloud.CommonEmbedded
{
    /// <inheritdoc/>
    class HttpRequestUriModifier: IHttpRequestUriModifier
    {
        readonly List<HttpRequestUriModifierRequest> m_RequestModifiers = new();

        /// <summary>
        /// Creates an instance of <see cref="HttpRequestUriModifier"/> with the provided <paramref name="schema"/>.
        /// </summary>
        /// <param name="schema">The schema for URI modification.</param>
        public HttpRequestUriModifier(HttpRequestUriModifierSchema schema)
        {
            Initialize(schema);
        }

        /// <summary>
        /// Creates an instance of <see cref="HttpRequestUriModifier"/> with the provided <paramref name="schemaJsonContent"/>.
        /// </summary>
        /// <param name="schemaJsonContent">The schema for URI modification.</param>
        public HttpRequestUriModifier(string schemaJsonContent)
        {
            Initialize(JsonSerialization.Deserialize<HttpRequestUriModifierSchema>(schemaJsonContent));
        }

        void Initialize(HttpRequestUriModifierSchema schema)
        {
            foreach (var requestModifierSchema in schema.Requests)
            {
                m_RequestModifiers.Add(new HttpRequestUriModifierRequest(requestModifierSchema));
            }
        }

        /// <inheritdoc/>
        public string Modify(string requestUri) => Modify(new Uri(requestUri)).ToString();

        /// <inheritdoc/>
        public Uri Modify(Uri requestUri)
        {
#pragma warning disable S3267
            // Applying SonarQube's solution for code smell S3257 (Loops should be simplified with "LINQ" expressions)
            // creates another code smell and makes the code more confusing.
            foreach (var requestModifier in m_RequestModifiers)
            {
                if (requestModifier.TryModify(requestUri, out var modifiedUri))
                    return modifiedUri; // only apply the first match
            }

            return requestUri;
#pragma warning disable S3267
        }
    }
}
