using System;

namespace Unity.Cloud.IdentityEmbedded
{

    /// <summary>
    /// A high-level interface for composite authentication flow that implements <see cref="IUrlRedirectionAuthenticator"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="ICompositeAuthenticator"/> exposes all authentication features in a single interface so it can adapt to runtime execution context.
    /// </remarks>
    interface ICompositeAuthenticator : IUrlRedirectionAuthenticator
    {
        /// <summary>
        /// Whether the <see cref="ICompositeAuthenticator"/> implementation requires a graphical user interface (GUI) for the authentication flow. 
        /// </summary>
        /// <remarks>A GUI is required for a user to click login and logout buttons and provide credentials when requested. When no GUI is required, the login is done automatically in the <see cref="IAuthenticator.InitializeAsync"/> method.</remarks>
        bool RequiresGUI { get; }
    }
}
