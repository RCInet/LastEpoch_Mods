using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helper methods for <see cref="IServiceHttpClient"/>.
    /// </summary>
    static class ServiceHttpClientExtensions
    {
        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="request">The  request to be sent.</param>
        /// <param name="options">The options for the client.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> SendAsync(this IServiceHttpClient httpClient, HttpRequestMessage request, ServiceHttpClientOptions options)
        {
            return httpClient.SendAsync(request, options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="request">The  request to be sent.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> SendAsync(this IServiceHttpClient httpClient, HttpRequestMessage request, ServiceHttpClientOptions options,
            CancellationToken cancellationToken)
        {
            return httpClient.SendAsync(request, options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="request">The  request to be sent.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> SendAsync(this IServiceHttpClient httpClient, HttpRequestMessage request,
            ServiceHttpClientOptions options, HttpCompletionOption completionOption)
        {
            return httpClient.SendAsync(request, options, completionOption, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="request">The  request to be sent.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> SendAsync(this IServiceHttpClient httpClient, HttpRequestMessage request, ServiceHttpClientOptions options,
            HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return httpClient.SendAsync(request, options, completionOption, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> GetAsync(this IServiceHttpClient httpClient, string requestUri, ServiceHttpClientOptions options)
        {
            return httpClient.GetAsync(HttpClientExtensions.CreateUri(requestUri), options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> GetAsync(this IServiceHttpClient httpClient, string requestUri, ServiceHttpClientOptions options,
            CancellationToken cancellationToken)
        {
            return httpClient.GetAsync(HttpClientExtensions.CreateUri(requestUri), options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> GetAsync(this IServiceHttpClient httpClient, string requestUri, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            return httpClient.GetAsync(HttpClientExtensions.CreateUri(requestUri), options, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> GetAsync(this IServiceHttpClient httpClient, Uri requestUri, ServiceHttpClientOptions options)
        {
            return httpClient.SendAsync(HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Get, requestUri), options,
                HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> GetAsync(this IServiceHttpClient httpClient, Uri requestUri, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            return httpClient.SendAsync(HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Get, requestUri), options,
                completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> GetAsync(this IServiceHttpClient httpClient, Uri requestUri, ServiceHttpClientOptions options,
            CancellationToken cancellationToken)
        {
            return httpClient.SendAsync(HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Get, requestUri), options,
                HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous POST request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PostAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            return httpClient.PostAsync(HttpClientExtensions.CreateUri(requestUri), content, options, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous POST request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PostAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options,
            CancellationToken cancellationToken)
        {
            return httpClient.PostAsync(HttpClientExtensions.CreateUri(requestUri), content, options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous POST request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PostAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options)
        {
            return httpClient.PostAsync(HttpClientExtensions.CreateUri(requestUri), content, options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous POST request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PostAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous POST request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PostAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options,
            CancellationToken cancellationToken = default)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous POST request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PostAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PatchAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            return httpClient.PatchAsync(HttpClientExtensions.CreateUri(requestUri), content, options, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PatchAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options,
           CancellationToken cancellationToken)
        {
            return httpClient.PatchAsync(HttpClientExtensions.CreateUri(requestUri), content, options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PatchAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options)
        {
            return httpClient.PatchAsync(HttpClientExtensions.CreateUri(requestUri), content, options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PatchAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpClientExtensions.HttpMethodPatch, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PatchAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpClientExtensions.HttpMethodPatch, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PatchAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpClientExtensions.HttpMethodPatch, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous PUT request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PutAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            return httpClient.PutAsync(HttpClientExtensions.CreateUri(requestUri), content, options, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PUT request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PutAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options,
           CancellationToken cancellationToken)
        {
            return httpClient.PutAsync(HttpClientExtensions.CreateUri(requestUri), content, options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PUT request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PutAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options)
        {
            return httpClient.PutAsync(HttpClientExtensions.CreateUri(requestUri), content, options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous PUT request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PutAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Put, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PUT request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PutAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Put, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PUT request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> PutAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Put, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, string requestUri, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            return httpClient.DeleteAsync(HttpClientExtensions.CreateUri(requestUri), options, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, string requestUri, ServiceHttpClientOptions options,
            CancellationToken cancellationToken)
        {
            return httpClient.DeleteAsync(HttpClientExtensions.CreateUri(requestUri), options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, string requestUri, ServiceHttpClientOptions options)
        {
            return httpClient.DeleteAsync(HttpClientExtensions.CreateUri(requestUri), options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, Uri requestUri, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            return httpClient.SendAsync(HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Delete, requestUri), options,
                completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, Uri requestUri, ServiceHttpClientOptions options,
            CancellationToken cancellationToken)
        {
            return httpClient.SendAsync(HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Delete, requestUri), options,
                HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, Uri requestUri, ServiceHttpClientOptions options)
        {
            return httpClient.SendAsync(HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Delete, requestUri), options,
                HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            return httpClient.DeleteAsync(HttpClientExtensions.CreateUri(requestUri), content, options, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options,
            CancellationToken cancellationToken)
        {
            return httpClient.DeleteAsync(HttpClientExtensions.CreateUri(requestUri), content, options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, string requestUri, HttpContent content, ServiceHttpClientOptions options)
        {
            return httpClient.DeleteAsync(HttpClientExtensions.CreateUri(requestUri), content, options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Delete, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, completionOption, progress, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Delete, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, HttpCompletionOption.ResponseContentRead, null, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified Uri.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="requestUri">The uri for the request.</param>
        /// <param name="content">The HTTP content for the request.</param>
        /// <param name="options">The options for the client.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this IServiceHttpClient httpClient, Uri requestUri, HttpContent content, ServiceHttpClientOptions options)
        {
            HttpRequestMessage request = HttpClientExtensions.CreateHttpRequestMessage(HttpMethod.Delete, requestUri);
            request.Content = content;
            return httpClient.SendAsync(request, options, HttpCompletionOption.ResponseContentRead, null, CancellationToken.None);
        }

        /// <summary>
        /// Creates an instance of <see cref="ServiceHttpClientHeaderModifier"/> which adds the API source headers to each request.
        /// </summary>
        /// <param name="baseServiceHttpClient">The client for which to modify the request headers.</param>
        /// <param name="name">The API source name.</param>
        /// <param name="version">The API source version.</param>
        /// <returns>The created <see cref="ServiceHttpClientHeaderModifier"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown is <paramref name="name"/> or <paramref name="version"/> are null or empty.</exception>
        public static IServiceHttpClient WithApiSourceHeaders(this IServiceHttpClient baseServiceHttpClient, string name, string version)
        {
            var logger = LoggerProvider.GetLogger(typeof(ServiceHttpClientExtensions).FullName);

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(version))
                throw new ArgumentNullException(nameof(version));

            // Create a header based on the API source name and version.
            var apiSourceVersion = new ApiSourceVersion(name, version);
            var sourceHeaders = new Dictionary<string, string>() {{ServiceHeaderUtils.k_ApiSourceHeader, apiSourceVersion.GetHeaderValue()}};

            logger.LogDebug($"Creating a {nameof(ServiceHttpClientHeaderModifier)} to add source headers for {apiSourceVersion.GetHeaderValue()}");

            return new ServiceHttpClientHeaderModifier(baseServiceHttpClient, sourceHeaders, ServiceHeaderUtils.k_UnityApiPattern);
        }

        /// <summary>
        /// Creates an instance of <see cref="ServiceHttpClientHeaderModifier"/> which adds the API source headers to each request.
        /// The source values are retrieved from the <see cref="ApiSourceVersionAttribute"/> which must be defined in the calling <see cref="Assembly"/>.
        /// </summary>
        /// <remarks>An instance of the <see cref="ApiSourceVersionAttribute"/> must be defined at the assembly-level in the calling <see cref="Assembly"/> in order
        /// for the correct API source values to be added as a header.</remarks>
        /// <param name="baseServiceHttpClient">The client for which to modify the request headers.</param>
        /// <param name="assembly">The target assembly.</param>
        /// <returns>The created <see cref="ServiceHttpClientHeaderModifier"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null or the name or version defined in the retrieved <see cref="ApiSourceVersionAttribute"/> are null or white space.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="ApiSourceVersionAttribute"/> does not exist or is not initialized in the calling assembly.</exception>
        /// <exception cref="InvalidArgumentException">Thrown if <see cref="ApiSourceVersionAttribute"/> is initialized with null or empty values in the calling assembly.</exception>
        public static IServiceHttpClient WithApiSourceHeadersFromAssembly(this IServiceHttpClient baseServiceHttpClient, Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var apiSourceVersion = ApiSourceVersion.GetApiSourceVersionForAssembly(assembly);
            return baseServiceHttpClient.WithApiSourceHeaders(apiSourceVersion.Name, apiSourceVersion.Version);
        }
    }
}
