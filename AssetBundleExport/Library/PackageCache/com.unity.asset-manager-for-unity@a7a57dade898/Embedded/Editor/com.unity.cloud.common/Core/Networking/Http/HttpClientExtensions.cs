using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helper methods for <see cref="IHttpClient"/>.
    /// </summary>
    static class HttpClientExtensions
    {
        // We use the HttpMethod constructor here because HttpMethod.Patch throws PlatformNotSupportedException
        static HttpMethod m_HttpMethodPatch;

        /// <summary>
        /// An <see cref="HttpMethod"/> for "PATCH".
        /// </summary>
        public static HttpMethod HttpMethodPatch => m_HttpMethodPatch ?? new("PATCH");

        /// <summary>
        /// Sends an asynchronous HTTP request.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="request">The request to be sent.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> SendAsync(this IHttpClient httpClient, HttpRequestMessage request)
        {
            return httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous HTTP request.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="request">The request to be sent.</param>
        /// <param name="cancellationToken">Cancellation token that will try to cancel the operation.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> SendAsync(this IHttpClient httpClient, HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous HTTP request.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="request">The request to be sent.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> SendAsync(this IHttpClient httpClient, HttpRequestMessage request,
            HttpCompletionOption completionOption)
        {
            return httpClient.SendAsync(request, completionOption, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous HTTP request.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="request">The request to be sent.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="cancellationToken">Cancellation token that will try to cancel the operation.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> SendAsync(this IHttpClient httpClient, HttpRequestMessage request,
            HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return httpClient.SendAsync(request, completionOption, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> GetAsync(this IHttpClient httpClient, string requestUri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            return httpClient.GetAsync(CreateUri(requestUri), completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the requestUri is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> GetAsync(this IHttpClient httpClient, Uri requestUri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            return httpClient.SendAsync(CreateHttpRequestMessage(HttpMethod.Get, requestUri), completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous POST request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PostAsync(this IHttpClient httpClient, string requestUri, HttpContent content, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            return httpClient.PostAsync(CreateUri(requestUri), content, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous POST request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the requestUri is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PostAsync(this IHttpClient httpClient, Uri requestUri, HttpContent content, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage request = CreateHttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PUT request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PutAsync(this IHttpClient httpClient, string requestUri, HttpContent content,HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            return httpClient.PutAsync(CreateUri(requestUri), content, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PUT request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the requestUri is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PutAsync(this IHttpClient httpClient, Uri requestUri, HttpContent content,HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage request = CreateHttpRequestMessage(HttpMethod.Put, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PatchAsync(this IHttpClient httpClient, string requestUri, HttpContent content, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            return httpClient.PatchAsync(CreateUri(requestUri), content, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the requestUri is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PatchAsync(this IHttpClient httpClient, Uri requestUri, HttpContent content, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            // We use the HttpMethod constructor here because HttpMethod.Patch throws PlatformNotSupportedException
            HttpRequestMessage request = CreateHttpRequestMessage(HttpMethodPatch, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IHttpClient httpClient, string requestUri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            return httpClient.DeleteAsync(CreateUri(requestUri), completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the requestUri is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IHttpClient httpClient, Uri requestUri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            return httpClient.SendAsync(CreateHttpRequestMessage(HttpMethod.Delete, requestUri), completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IHttpClient httpClient, string requestUri, HttpContent content, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            return httpClient.DeleteAsync(CreateUri(requestUri), content, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the requestUri is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IHttpClient httpClient, Uri requestUri, HttpContent content, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            HttpRequestMessage request = CreateHttpRequestMessage(HttpMethod.Delete, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Creates a <see cref="Uri"/> from a specified <see cref="string"/>.
        /// </summary>
        /// <param name="uri">The <see cref="string"/> to convert.</param>
        /// <returns>The created <see cref="Uri"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when an unhandled exception is thrown constructing the URI.</exception>
        public static Uri CreateUri(String uri)
        {
            if (!Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri result))
                throw new ArgumentException(nameof(uri));

            return result;
        }

        /// <summary>
        /// Creates an <see cref="HttpRequestMessage"/> from an <see cref="HttpMethod"/> and a <see cref="Uri"/>.
        /// </summary>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="uri">The <see cref="Uri"/> to request.</param>
        /// <returns>The created <see cref="HttpRequestMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the URI is null.</exception>
        public static HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            return new HttpRequestMessage(httpMethod, uri);
        }
    }
}
