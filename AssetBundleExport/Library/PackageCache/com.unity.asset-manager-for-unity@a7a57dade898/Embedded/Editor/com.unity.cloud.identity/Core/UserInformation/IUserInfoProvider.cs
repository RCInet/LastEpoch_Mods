using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface that exposes methods to fetch user information.
    /// </summary>
    interface IUserInfoProvider
    {
        /// <summary>
        /// A task to fetch asynchronously user information.
        /// </summary>
        /// <returns>An <see cref="IUserInfo"/> instance.</returns>
        Task<IUserInfo> GetUserInfoAsync();
    }
}
