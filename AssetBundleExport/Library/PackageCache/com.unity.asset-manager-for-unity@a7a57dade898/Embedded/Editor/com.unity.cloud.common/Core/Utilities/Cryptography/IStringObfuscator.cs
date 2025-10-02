using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An interface providing encryption and decryption methods to obfuscate string values.
    /// </summary>
    interface IStringObfuscator
    {
        /// <summary>
        /// Encrypts a string value.
        /// </summary>
        /// <param name="value">The string to encrypt.</param>
        /// <returns>The encrypted string.</returns>
        string Encrypt(string value);

        /// <summary>
        /// Decrypts a string value.
        /// </summary>
        /// <param name="value">The string to decrypt.</param>
        /// <returns>The decrypted string.</returns>
        string Decrypt(string value);
    }
}
