using System;
using System.Collections.Generic;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <summary>
    /// This struct contains information about url redirection operation.
    /// </summary>
    struct UrlRedirectResult
    {
        /// <summary>
        /// The status of the redirect operation.
        /// </summary>
        public UrlRedirectStatus Status { get; set; }

        /// <summary>
        /// The query arguments returned in the redirected url formatted as a dictionary of key value pairs.
        /// </summary>
        public Dictionary<string, string> QueryArguments { get; set; }
    }
}
