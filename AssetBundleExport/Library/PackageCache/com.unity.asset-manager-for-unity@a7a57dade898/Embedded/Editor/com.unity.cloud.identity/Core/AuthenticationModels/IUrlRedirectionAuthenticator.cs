using System;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface for manual login and logout operations using redirection flows.
    /// </summary>
    interface IUrlRedirectionAuthenticator : IAuthenticator
    {
        /// <summary>
        /// Performs a login operation.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="AuthenticationFailedException"></exception>
        /// <returns>
        /// A task.
        /// </returns>
        Task LoginAsync();


        /// <summary>
        /// Cancels the awaiting login operation.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        void CancelLogin();

        /// <summary>
        /// Performs a logout operation.
        /// </summary>
        /// <param name="clearBrowserCache">An optional boolean value that, if set to true, triggers a navigation to the OS default browser to clear any cached session.</param>
        /// <remarks>A logout operation clears the user session in the application only.
        /// Unless the user manually clears the session in the browser that is used for authentication, a user can get automatically logged in again from cached values,
        /// without entering any credentials.
        /// Use the clearBrowserCache boolean to also clear the session in the browser to prevent automatic login from a cached session.</remarks>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>
        /// A task.
        /// </returns>
        Task LogoutAsync(bool clearBrowserCache = false);
    }
}
