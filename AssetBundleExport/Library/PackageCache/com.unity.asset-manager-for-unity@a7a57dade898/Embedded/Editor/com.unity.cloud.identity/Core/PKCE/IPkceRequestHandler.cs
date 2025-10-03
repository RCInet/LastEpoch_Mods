using System.Threading.Tasks;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// This interface defines methods for all Pkce authenticator http requests.
    /// </summary>
    interface IPkceRequestHandler
    {
        /// <summary>
        /// Retrieves the device token from specified end-point
        /// </summary>
        /// <param name="tokenEndPointParams">The content of the request</param>
        /// <returns>
        /// A task that results in a <see cref="DeviceToken"/> when completed.
        /// </returns>
        Task<DeviceToken> ExchangeCodeForDeviceTokenAsync(string tokenEndPointParams);

        /// <summary>
        /// Updates the device token from specified end-point
        /// </summary>
        /// <param name="tokenEndPointParams">The content of the request</param>
        /// <param name="refreshToken">The refresh token needed for the refresh request</param>
        /// <returns>
        /// A task that results in a <see cref="DeviceToken"/> when completed.
        /// </returns>
        Task<DeviceToken> RefreshTokenAsync(string tokenEndPointParams, string refreshToken);

        /// <summary>
        /// Revokes the current refresh token
        /// </summary>
        /// <param name="revokeEndPointParams">The content of the request</param>
        /// <returns>
        /// A task.
        /// </returns>
        Task RevokeRefreshTokenAsync(string revokeEndPointParams);

        /// <summary>
        /// Returns the user information claims from the PKCEConfiguration /userinfo endpoint.
        /// </summary>
        /// <param name="userInfoClaims">The userInfo claims to fetch.</param>
        /// <returns>
        /// A task that results in the string value of the userInfo claims when completed.
        /// </returns>
        Task<string> GetUserInfoAsync(string userInfoClaims);
    }
}
