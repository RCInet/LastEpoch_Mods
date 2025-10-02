using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An interface that represents a provider for an app namespace.
    /// </summary>
    interface IAppNamespaceProvider
    {
        /// <summary>
        /// Returns the App namespace uniquely identifying an App on a device.
        /// </summary>
        /// <returns>The App namespace.</returns>
        string GetAppNamespace();
    }
}
