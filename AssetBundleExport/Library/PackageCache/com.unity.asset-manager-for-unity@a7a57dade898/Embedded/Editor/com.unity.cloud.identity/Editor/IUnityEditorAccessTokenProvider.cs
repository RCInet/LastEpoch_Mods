using System;
using System.Threading.Tasks;

namespace Unity.Cloud.IdentityEmbedded.Editor
{
    /// <summary>
    /// An interface to provide a Unity Editor access token.
    /// </summary>
    interface IUnityEditorAccessTokenProvider
    {
        /// <summary>
        /// Returns an access token.
        /// </summary>
        /// <returns>
        /// A task that once completed returns an access token.
        /// </returns>
        Task<string> GetAccessTokenAsync();
    }
}
