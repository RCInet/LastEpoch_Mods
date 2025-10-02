using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An implementation of an HTTP client which abstracts the Task of sending <see cref="HttpRequestMessage"/> and adds
    /// a fixed set of predefined headers to each request.
    /// </summary>
    class ServiceHttpClientHeaderModifier : IServiceHttpClient
    {
        readonly IServiceHttpClient m_BaseClient;
        readonly Dictionary<string, string> m_Headers;
        readonly string m_UrlFilter;

        /// <inheritdoc />
        public TimeSpan Timeout
        {
            get => m_BaseClient.Timeout;
            set => m_BaseClient.Timeout = value;
        }

        /// <summary>
        /// Creates and instance of <see cref="ServiceHttpClientHeaderModifier"/>.
        /// </summary>
        /// <param name="serviceHttpClient">The client who's requests will have headers added.</param>
        /// <param name="headers">The headers to add to each request.</param>
        /// <param name="urlFilter">The optional url filter to determine which requests should have the headers added. A null or empty filter will add the headers to all requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceHttpClient"/> or any key in <paramref name="headers"/> is null.</exception>
        public ServiceHttpClientHeaderModifier(IServiceHttpClient serviceHttpClient, Dictionary<string, string> headers, string urlFilter = null)
        {
            m_BaseClient = serviceHttpClient ?? throw new ArgumentNullException(nameof(serviceHttpClient));
            m_Headers = headers != null && !headers.Keys.Any(string.IsNullOrWhiteSpace) ? headers : throw new ArgumentNullException(nameof(headers), $"A key in {nameof(headers)} is null or white space.");
            m_UrlFilter = urlFilter;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            AddHeaders(request);
            return m_BaseClient.SendAsync(request, completionOption, progress, cancellationToken);
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,  ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            AddHeaders(request);
            return m_BaseClient.SendAsync(request, options, completionOption, progress, cancellationToken);
        }

        void AddHeaders(HttpRequestMessage request)
        {
            if (string.IsNullOrEmpty(m_UrlFilter)|| Regex.IsMatch(request.RequestUri.ToString(), m_UrlFilter, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))
            {
                foreach (var header in m_Headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }
    }
}
