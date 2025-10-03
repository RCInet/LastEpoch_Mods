using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Interface that represent a client for making http requests.
    /// </summary>
    interface IHttpClient
    {
        /// <summary>
        /// The timespan to wait before the request times out.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Sends an asynchronous HTTP request.
        /// </summary>
        /// <param name="request">The request to be sent.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="cancellationToken">Cancellation token that will try to cancel the operation.</param>
        /// <returns>A task that will hold the HttpResponseMessage once the request is completed</returns>
        /// <exception cref="ArgumentNullException">Thrown when the request is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token or due to timeout.</exception>
        /// A <see cref="TimeoutException"/> is nested in the exception as an inner exception in case of timeout.
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, IProgress<HttpProgress> progress,
            CancellationToken cancellationToken);
    }
}
