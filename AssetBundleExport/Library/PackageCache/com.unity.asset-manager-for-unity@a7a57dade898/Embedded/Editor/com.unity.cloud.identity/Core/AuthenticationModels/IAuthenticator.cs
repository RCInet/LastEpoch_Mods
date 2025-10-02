using System;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{

    /// <summary>
    /// An interface for authentication flow that implements <see cref="IServiceAuthorizer"/>, <see cref="IAuthenticationStateProvider"/>, <see cref="IUserInfoProvider"/> and <see cref="IOrganizationRepository"/>.
    /// </summary>
    interface IAuthenticator : IServiceAuthorizer, IAuthenticationStateProvider, IUserInfoProvider, IOrganizationRepository
    {
        /// <summary>
        /// Indicates if the `IAuthenticator` has valid preconditions to provide authentication in the current execution context.
        /// </summary>
        /// <returns>A task that when completed indicates if the `IAuthenticator` has valid preconditions to provide authentication in the current execution context.</returns>
        Task<bool> HasValidPreconditionsAsync();

        /// <summary>
        /// A task to initialize the <see cref="AuthenticationState"/> from either cache or direct injection value.
        /// </summary>
        /// <returns>A task.</returns>
        Task InitializeAsync();
    }
}
