using System;
using System.Security.Authentication;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface to decode a <see cref="JwtToken"/> from a string.
    /// </summary>
    interface IJwtDecoder
    {
        /// <summary>
        /// A function to decode a <see cref="JwtToken"/> from a string.
        /// </summary>
        /// <returns>The decoded <see cref="JwtToken"/>.</returns>
        /// <param name="jwtToken">The JWT token string to decode.</param>
        /// <exception cref="System.Security.Authentication.InvalidCredentialException">Thrown if the string does not match expected JWT format.</exception>
        JwtToken Decode(string jwtToken);
    }

    class JwtDecoder : IJwtDecoder
    {
        /// <inheritdoc/>
        public JwtToken Decode(string jwtToken)
        {
            var parts = jwtToken.Split(".");
            if (parts.Length != 3) throw new InvalidCredentialException("Invalid JWT format.");
            var encodedPayload = parts[1];
            var payloadString = Encoding.UTF8.GetString(Base64UrlDecode(encodedPayload));
            return JsonSerialization.Deserialize<JwtToken>(payloadString);
        }

        byte[] Base64UrlDecode(string payload)
        {
            // Replace url safe chars
            var safeCharsPayload = payload.Replace('-', '+').Replace('_', '/');
            // base64 conversion requires the length of the payload to be a multiple of 4
            var paddingCharLength = safeCharsPayload.Length % 4;
            if (paddingCharLength > 0)
                safeCharsPayload += new string('=', 4 - paddingCharLength);
            return Convert.FromBase64String(safeCharsPayload);
        }
    }
}
