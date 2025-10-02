using System;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <summary>
    /// An interface to handle the execution of an URL.
    /// </summary>
    interface IUrlProcessor
    {
        /// <summary>
        /// Gets the host URL of the current running application.
        /// </summary>
        string HostUrl { get; }

        /// <summary>
        /// Handles the execution of an URL.
        /// </summary>
        /// <param name="url">The URL to process.</param>
        void ProcessURL(string url);
    }
}
