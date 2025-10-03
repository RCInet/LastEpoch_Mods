using System;
using System.Security.Cryptography;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    static class PkceHelper
    {
        public static string GenerateState()
        {
            var bytes = new byte[43];
            var randomNumber = RandomNumberGenerator.Create();
            randomNumber.GetBytes(bytes);
            return UrlEncodeBase64String(Convert.ToBase64String(bytes));
        }

        public static void GenerateChallengeVerifier(out string codeVerifier, out string codeChallenge)
        {
            // Create codeVerifier first
            var bytes = new byte[43];
            var randomNumber = RandomNumberGenerator.Create();
            randomNumber.GetBytes(bytes);
            codeVerifier = UrlEncodeBase64String(Convert.ToBase64String(bytes));

            // create codeChallenge from codeVerifier
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                codeChallenge = UrlEncodeBase64String(Convert.ToBase64String(challengeBytes));
            }
        }

        static string UrlEncodeBase64String(string base64String)
        {
            return base64String.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        static bool IsLoopBackRedirection(string redirectUri)
        {
            return redirectUri.StartsWith("http://localhost");
        }

        static bool IsHttpsHostedRedirection(string redirectUri)
        {
            return redirectUri.StartsWith("https://");
        }

        public static string CreateRefreshTokenUrlRequestStringContent(string refreshToken, PkceConfiguration pkceConfiguration)
        {
            return $"client_id={pkceConfiguration.ClientId}&grant_type=refresh_token&refresh_token={refreshToken}";
        }

        public static string CreateRevokeRefreshTokenUrlRequestStringContent(string token, PkceConfiguration pkceConfiguration)
        {
            return $"client_id={pkceConfiguration.ClientId}&token={token}";
        }

        public static string CreateTokenUrlRequestStringContent(string appNamespace, string authCode, string codeVerifier, string redirectUri, PkceConfiguration pkceConfiguration, string overriddenLoginState = "")
        {
            var redirectHint = CreateRedirectHint($"{appNamespace}", redirectUri);
            var redirectProcessId = !string.IsNullOrEmpty(overriddenLoginState) ? $"{overriddenLoginState}/" : "";
            redirectUri = $"https://{pkceConfiguration.ProxyLoginCompletedRoute}{redirectHint}/{redirectProcessId}";
            return $"client_id={pkceConfiguration.ClientId}&grant_type=authorization_code&code={authCode}&code_verifier={codeVerifier}&redirect_uri={redirectUri}";
        }

        public static string CreateAuthenticateUrl(string appNamespace, string state, string codeChallenge, string redirectUri, PkceConfiguration pkceConfiguration, string overriddenLoginState = "")
        {
            var redirectHint = CreateRedirectHint($"{appNamespace}", redirectUri);
            var redirectProcessId = !string.IsNullOrEmpty(overriddenLoginState) ? $"/{overriddenLoginState}" : "";
            var encodedAuthorizeEndpointUrl = System.Net.WebUtility.UrlEncode(pkceConfiguration.LoginUrl);
            var genesisLoginRedirectUrl = $"https://{pkceConfiguration.ProxyLoginRedirectRoute}{redirectHint}/{encodedAuthorizeEndpointUrl}{redirectProcessId}";
            return $"{genesisLoginRedirectUrl}?client_id={pkceConfiguration.ClientId}{pkceConfiguration.CustomLoginParams}&state={state}&code_challenge={codeChallenge}&code_challenge_method=S256&response_type=code";
        }

        public static string CreateSignOutUrl(string appNamespace,  string state, string redirectUri, PkceConfiguration pkceConfiguration, string overriddenState = "")
        {
            var redirectHint = CreateRedirectHint($"{appNamespace}", redirectUri);
            var redirectProcessId = !string.IsNullOrEmpty(overriddenState) ? $"/{overriddenState}" : "";
            var redirectProxyUri = $"https://{pkceConfiguration.ProxySignOutCompletedRoute}{redirectHint}{redirectProcessId}/?state={state}";
            // External IDP will need to support
            return $"{pkceConfiguration.SignOutUrl}{redirectProxyUri}";
        }

        static string CreateRedirectHint(string customUri, string redirectUri)
        {
            var redirectHint = customUri;
            if (IsLoopBackRedirection(redirectUri) || IsHttpsHostedRedirection(redirectUri))
            {
                // use base64 encoding for redirection safe transport in urls
                var plainTextBytes = Encoding.UTF8.GetBytes(redirectUri);
                redirectHint = Convert.ToBase64String(plainTextBytes);
            }
            return redirectHint;
        }
    }
}
