namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface to expose authenticated user info claims.
    /// </summary>
    internal interface IOpenIdUserInfoProvider
    {
        /// <summary>
        /// A method to retrieve the string value for an <see cref="OpenIdUserInfoClaims"/>.
        /// </summary>
        /// <param name="key">An OpenId user info claim key name.</param>
        /// <returns>The string value for the given key name.</returns>
        string GetUserInfo(string key);
    }

    /// <summary>
    /// A static class exposing authenticated user info claims key name.
    /// </summary>
    static class OpenIdUserInfoClaims
    {
        /// <summary>
        /// The access token used to fetch the claims from the /userinfo endpoint.
        /// </summary>
        public const string AccessToken = "access_token";

        /// <summary>
        /// The unique identifier of the user as provided by the authenticator party in the "sub" property of the /userinfo endpoint response.
        /// </summary>
        /// <remarks>This value is not guaranteed to be a valid Unity Id account, since it can be provided by any authenticator party.</remarks>
        public const string Id = "id";

        /// <summary>
        /// The full name of the user as provided by the authenticator party.
        /// </summary>
        public const string Name = "name";

        /// <summary>
        /// The unique email used to authenticate the user in the authentication flow.
        /// </summary>
        public const string Email = "email";

        /// <summary>
        /// The absolute https path to the user picture as provided by the authenticator party.
        /// </summary>
        public const string Picture = "picture";

        /// <summary>
        /// The given name of the user as provided by the authenticator party.
        /// </summary>
        public const string GivenName = "given_name";

        /// <summary>
        /// The family name of the user as provided by the authenticator party.
        /// </summary>
        public const string FamilyName = "family_name";
    }

    internal class PkceUserInfoClaims : IOpenIdUserInfoProvider
    {
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The subject identifier for the user.
        /// </summary>
        public string sub { get; set; }
        /// <summary>
        /// The primary email for the user.
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// The profile picture url.
        /// </summary>
        public string picture { get; set; }
        /// <summary>
        /// The preferred username.
        /// </summary>
        public string preferred_username { get; set; }
        /// <summary>
        /// The given name or first name.
        /// </summary>
        public string given_name { get; set; }
        /// <summary>
        /// The family name or last name.
        /// </summary>
        public string family_name { get; set; }
        /// <summary>
        /// The valid access token used to fetch the authenticated values.
        /// </summary>
        public string access_token { get; set; }

        public string GetUserInfo(string key)
        {
            return key switch
            {
                OpenIdUserInfoClaims.AccessToken => access_token,
                OpenIdUserInfoClaims.Id => sub,
                OpenIdUserInfoClaims.Name => name,
                OpenIdUserInfoClaims.Email => email,
                OpenIdUserInfoClaims.Picture => picture,
                OpenIdUserInfoClaims.GivenName => given_name,
                OpenIdUserInfoClaims.FamilyName => family_name,
                _ => null
            };
        }
    }
}
