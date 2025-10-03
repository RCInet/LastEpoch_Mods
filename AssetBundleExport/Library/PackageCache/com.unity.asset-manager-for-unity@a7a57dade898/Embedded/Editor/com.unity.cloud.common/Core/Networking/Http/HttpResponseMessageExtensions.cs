using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helper methods for <see cref="HttpResponseMessage"/>.
    /// </summary>
    static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Deserializes the content of an <see cref="HttpResponseMessage"/> to a specified type.
        /// </summary>
        /// <param name="response">The HTTP response message to deserialize.</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns></returns>
        public static async Task<T> JsonDeserializeAsync<T>(this HttpResponseMessage response)
        {
            var content = await response.GetContentAsStringAsync();

            return JsonSerialization.Deserialize<T>(content);
        }

        /// <summary>
        /// Gets a string representing the content of an <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="response">The HTTP response message.</param>
        /// <returns></returns>
        [Obsolete("Use GetContentAsStringAsync instead.")]
        public static async Task<string> GetContentAsString(this HttpResponseMessage response)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
#else
            return await response.Content.ReadAsStringAsync();
#endif
        }

        /// <summary>
        /// Gets a string representing the content of an <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="response">The HTTP response message.</param>
        /// <returns></returns>
        public static async Task<string> GetContentAsStringAsync(this HttpResponseMessage response)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
#else
            return await response.Content.ReadAsStringAsync();
#endif
        }
    }
}
