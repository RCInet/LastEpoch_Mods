using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// This interface abstracts the Task of sending <see cref="HttpRequestMessage"/>.
    /// </summary>
    interface IServiceHttpClient : IHttpClient
    {
        /// <summary>
        /// Send an asynchronous HTTP request.
        /// </summary>
        /// <param name="request">The request to be sent.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">Optional cancellation token that will try to cancel the operation.</param>
        /// <returns>A task that will hold the HttpResponseMessage once the request is completed</returns>
        /// <exception cref="ArgumentNullException">Thrown when the request is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ServiceHttpClientOptions options,
            HttpCompletionOption completionOption, IProgress<HttpProgress> progress, CancellationToken cancellationToken);
    }
}
