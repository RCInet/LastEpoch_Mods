using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A class which provides the default App namespace.
    /// </summary>
    class DefaultAppNamespaceProvider : IAppNamespaceProvider
    {
        /// <inheritdoc/>
        public string GetAppNamespace()
        {
            return "com.unity.cloud";
        }
    }
}
