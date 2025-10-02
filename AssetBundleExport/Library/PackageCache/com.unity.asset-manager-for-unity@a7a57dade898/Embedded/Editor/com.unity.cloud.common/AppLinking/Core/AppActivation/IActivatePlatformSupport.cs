using System;
using System.Collections.Generic;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <summary>
    /// An interface that abstracts platform-specific logic to handle application activation from a URL or key value pairs.
    /// </summary>
    interface IActivatePlatformSupport
    {
        /// <summary>
        /// Retrieves the URL used to activate the application.
        /// </summary>
        string HostUrl { get; }

        /// <summary>
        /// Retrieves the URL used to activate the application.
        /// </summary>
        string ActivationUrl { get; }

        /// <summary>
        /// Retrieves the key value pairs used to activate the application.
        /// </summary>
        Dictionary<string, string> ActivationKeyValue { get; }
    }
}
