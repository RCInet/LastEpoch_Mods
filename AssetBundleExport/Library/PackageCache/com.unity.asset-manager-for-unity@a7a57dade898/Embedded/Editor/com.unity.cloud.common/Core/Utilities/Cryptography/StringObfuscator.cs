using System;
using System.Security.Cryptography;
using System.Text;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An IStringObfuscator implementation that uses AesCryptoServiceProvider encryption and decryption methods to obfuscate string values.
    /// </summary>
    class AesStringObfuscator : IStringObfuscator
    {
        readonly string m_EncodingKey;

        /// <summary>
        /// Create a AesStringObfuscator that uses AesCryptoServiceProvider encryption and decryption methods to obfuscate string values.
        /// </summary>
        /// <param name="encodingKey">The encoding key for obfuscation.</param>
        public AesStringObfuscator(string encodingKey)
        {
            m_EncodingKey = encodingKey;
        }

        AesCryptoServiceProvider GetAesCryptoProvider()
        {
            var hashAlgorithm = new SHA256Managed();
            var key = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(m_EncodingKey));
            return new AesCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        }

        /// <summary>
        /// Encrypts a string value.
        /// </summary>
        /// <param name="value">The string to encrypt.</param>
        /// <returns>The encrypted string.</returns>
        public string Encrypt(string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            var aesCryptoProvider = GetAesCryptoProvider();
            var transform = aesCryptoProvider.CreateEncryptor();
            var resultsByteArray = transform.TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(resultsByteArray);
        }

        /// <summary>
        /// Decrypts a string value.
        /// </summary>
        /// <param name="value">The string to decrypt.</param>
        /// <returns>The decrypted string.</returns>
        public string Decrypt(string value)
        {
            var data = Convert.FromBase64String(value);
            var aesCryptoProvider = GetAesCryptoProvider();
            var transform = aesCryptoProvider.CreateDecryptor();
            var resultsByteArray = transform.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(resultsByteArray);
        }
    }
}
