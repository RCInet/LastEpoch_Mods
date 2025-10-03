using System;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Indicates the current authentication state in an application's lifetime.
    /// </summary>
    /// <example>
    /// <code source="../../Samples/Documentation/Scripting/AuthenticationStateExample.cs" region="AuthenticationState"/>
    /// </example>
    enum AuthenticationState
    {
        /// <summary>
        /// Indicates the application is waiting for the completion of the initialization.
        /// </summary>
        AwaitingInitialization,
        /// <summary>
        /// Indicates when an authenticated user is logged in.
        /// </summary>
        LoggedIn,
        /// <summary>
        /// Indicates no authenticated user is available.
        /// </summary>
        LoggedOut,
        /// <summary>
        /// Indicates the application is waiting for the completion of a login operation.
        /// </summary>
        AwaitingLogin,
        /// <summary>
        /// Indicates the application is waiting for the completion of a logout operation.
        /// </summary>
        AwaitingLogout
    };
}
