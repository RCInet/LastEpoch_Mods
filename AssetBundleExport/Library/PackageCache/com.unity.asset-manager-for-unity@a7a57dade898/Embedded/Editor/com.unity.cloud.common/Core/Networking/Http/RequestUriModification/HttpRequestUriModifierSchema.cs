using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// The schema for URI modification requests.
    /// </summary>
    [Serializable]
    class HttpRequestUriModifierSchema
    {
        public HttpRequestUriModifierRequestSchema[] Requests { get; set; }
    }

    /// <summary>
    /// The request schema for URI modification, which includes a filter and a replacement.
    /// </summary>
    [Serializable]
    class HttpRequestUriModifierRequestSchema
    {
        public HttpRequestUriModifierFilterSchema Filter { get; set; }
        public HttpRequestUriModifierReplaceSchema Replace { get; set; }
    }

    /// <summary>
    /// The schema for URI filtering.
    /// </summary>
    [Serializable]
    class HttpRequestUriModifierFilterSchema
    {
        public string UriProperty { get; set; }
        public string IsMatch { get; set; }
    }

    /// <summary>
    /// The schema for URI property replacement.
    /// </summary>
    [Serializable]
    class HttpRequestUriModifierReplaceSchema
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Scheme { get; set; }
    }
}
