using System;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// A class representing a JWT token.
    /// </summary>
    class JwtToken
    {
        /// <summary>
        /// The required sub property of a JWT token.
        /// </summary>
        public string sub { get; set; }

        /// <summary>
        /// The required exp property of a JWT token.
        /// </summary>
        public int exp { get; set; }
    }
}
