using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An interface that represents a provider of an app ID.
    /// </summary>
    interface IAppIdProvider
    {
        /// <summary>
        /// Returns the App Id uniquely identifying an App on the cloud services.
        /// </summary>
        /// <returns>The App Id.</returns>
        AppId GetAppId();
    }
}
