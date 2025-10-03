using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helper methods for <see cref="HttpRequestMessage"/>.
    /// </summary>
    static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Gets a string representing the content of an <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <returns>A string representing the content of an <see cref="HttpRequestMessage"/></returns>
        public static async Task<string> GetContentAsStringAsync(this HttpRequestMessage request)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var stream = await request.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
#else
            return await request.Content.ReadAsStringAsync();
#endif
        }

        /// <summary>
        /// Gets a byte array representing the content of an <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <returns>A byte array representing the content of an <see cref="HttpRequestMessage"/></returns>
        public static async Task<byte[]> GetContentAsByteArrayAsync(this HttpRequestMessage request)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            using var memoryStream = new MemoryStream();
            var stream = await request.Content.ReadAsStreamAsync();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
#else
            return await request.Content.ReadAsByteArrayAsync();
#endif
        }
    }
}
