using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Modifies the request URI.
    /// </summary>
    interface IHttpRequestUriModifier
    {
        /// <summary>
        /// Modifies the request URI.
        /// </summary>
        /// <param name="requestUri">The URI to modify.</param>
        /// <returns>The modified URI.</returns>
        string Modify(string requestUri);

        /// <summary>
        /// Modifies the request URI.
        /// </summary>
        /// <param name="requestUri">The URI to modify.</param>
        /// <returns>The modified URI.</returns>
        Uri Modify(Uri requestUri);
    }
}
