using System;
using System.Threading.Tasks;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface that manages the application authentication state.
    /// </summary>
    interface IAuthenticationStateProvider
    {
        /// <summary>
        /// Holds the current `AuthenticationState`.
        /// </summary>
        AuthenticationState AuthenticationState { get; }

        /// <summary>
        /// Triggers when the <see cref="AuthenticationState"/> of the current user changes.
        /// </summary>
        /// <example>
        /// <code source="../../Samples/Documentation/Scripting/AuthenticationStateProviderExample.cs" region="AuthenticationStateChanged"/>
        /// </example>
        /// <remarks>
        /// Subscribers of this event should restrict or allow access to available resources and features based on the returned value.
        /// </remarks>
        event Action<AuthenticationState> AuthenticationStateChanged;
    }
}
