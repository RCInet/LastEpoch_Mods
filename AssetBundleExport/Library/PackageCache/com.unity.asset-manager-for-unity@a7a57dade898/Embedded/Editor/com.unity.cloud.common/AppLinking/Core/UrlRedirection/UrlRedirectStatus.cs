using System;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <summary>
    /// Lists the possible statuses for a UrlRedirect operation.
    /// </summary>
    enum UrlRedirectStatus
    {
        /// <summary>
        /// No redirection is expected.
        /// </summary>
        /// <remarks>
        /// In a browser hosted app, redirection from previous login page will be intercepted on hosting page reload, when app is restarted.
        /// </remarks>
        NotApplicable,
        /// <summary>
        /// Redirection from browser is successful.
        /// </summary>
        Success,
    }
}
