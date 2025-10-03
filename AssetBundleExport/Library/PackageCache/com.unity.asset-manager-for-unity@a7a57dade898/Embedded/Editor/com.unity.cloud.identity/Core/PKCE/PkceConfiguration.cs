using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Contains all information required to perform Proof Key Code Exchange (PKCE) authentication.
    /// </summary>
    [Serializable]
class PkceConfiguration
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        /// <summary>
        /// The unique client identifier as registered with the authentication service.
        /// </summary>
        public ClientId ClientId = ClientId.None;
        /// <summary>
        /// The login redirect proxy route.
        /// </summary>
        public string ProxyLoginRedirectRoute = "";
        /// <summary>
        /// The login completed redirect proxy route.
        /// </summary>
        public string ProxyLoginCompletedRoute = "";
        /// <summary>
        /// The sign out completed redirect proxy route.
        /// </summary>
        public string ProxySignOutCompletedRoute = "";
        /// <summary>
        /// The login page url on the authentication service.
        /// </summary>
        public string LoginUrl = "";
        /// <summary>
        /// The endpoint url on the authentication service to reach to exchange returned login code for a valid set of tokens.
        /// </summary>
        public string TokenUrl = "";
        /// <summary>
        /// The endpoint url on the authentication service to reach to refresh current set of tokens.
        /// </summary>
        public string RefreshTokenUrl = "";
        /// <summary>
        /// The endpoint url on the authentication service to reach to revoke the set of tokens.
        /// </summary>
        public string LogoutUrl = "";
        /// <summary>
        /// The endpoint url on the authentication service to reach to sign out.
        /// </summary>
        public string SignOutUrl = "";
        /// <summary>
        /// The endpoint url on the authentication service to reach to get user information using .
        /// </summary>
        public string UserInfoUrl = "";
        /// <summary>
        /// The additional custom url formatted parameters to append to the LoginUrl.
        /// </summary>
        /// <remarks>
        /// See documentation of your authentication service to learn what additional parameters can be required.
        /// </remarks>
        public string CustomLoginParams = "";
        /// <summary>
        /// Boolean value holding the capabilities of the app to cache the refresh token.
        /// </summary>
        /// <remarks>
        /// Caching the refresh token allows to skip authentication and resume a user session when the app is restarted.
        /// Set this value to false if your app requires a high security level. User will then be forced to login each time the application is started.
        /// </remarks>
        public bool CacheRefreshToken = true;
        /// <summary>
        /// Boolean value holding the capabilities of the app to support guest user access.
        /// </summary>
        [Obsolete("This property will be removed in a future version.")]
        public bool AllowAnonymous = true;
#pragma warning restore S1104

        /// <summary>
        /// This functions can be used after deserialization, to ensure the format is correct.
        /// </summary>
        public void Sanitize()
        {
            ClientId = new ClientId(ClientId.ToString().Trim());
            LoginUrl = LoginUrl?.Trim();
            TokenUrl = TokenUrl?.Trim();
            RefreshTokenUrl = RefreshTokenUrl?.Trim();
            LogoutUrl = LogoutUrl?.Trim();
            CustomLoginParams = CustomLoginParams?.Trim();
        }
    }
}
