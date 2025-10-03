using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    class LegacyRequestHandler
    {
        const string k_ContentEncodingHeaderKey = "Content-Encoding";
        const string k_ContentLengthHeaderKey = "Content-Length";
        const string k_ContentTypeHeaderKey = "Content-Type";
        const string k_TimeoutErrorMessage = "Request timeout";

        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Send an asynchronous HTTP request or file download request.
        /// </summary>
        /// <param name="httpRequestMessage">The request message.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="uploadHandler">Optional path of files to be uploaded.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">Optional cancellation token that will try to cancel the operation.</param>
        /// <returns>A task that will hold the HttpResponseMessage once the request is completed.</returns>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="TaskCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        public async Task<HttpResponseMessage> RequestAsync(HttpRequestMessage httpRequestMessage, UploadHandler uploadHandler, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            var factoryTask = await Task.Factory.StartNew(
                async () => await RequestInternalAsync(httpRequestMessage, uploadHandler, completionOption, progress, cancellationToken),
                cancellationToken,
                TaskCreationOptions.DenyChildAttach,
                UnityMainThreadSchedulerGrabber.s_UnityMainThreadScheduler);

            return await factoryTask;
        }

        async Task<HttpResponseMessage> RequestInternalAsync(HttpRequestMessage httpRequestMessage, UploadHandler uploadHandler, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            var state = new RequestState(httpRequestMessage, null, null, Timeout);

            await HandleRequestWithRedirection(state, uploadHandler, completionOption, progress, cancellationToken);
            if(completionOption == HttpCompletionOption.ResponseContentRead)
                CompleteRequest(state);

            return state.Response;
        }

        static async Task HandleRequestWithRedirection(RequestState state, UploadHandler uploadHandler,  HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            /*
            UnityWebRequest internal handling of redirection httpStatusCode 302 will not modify the headers when issuing follow-up requests.
            This limitation makes some requests fail because they contain unsupported headers.
            Here, we set the redirectLimit to 0 in order to handle this use case, and properly handle headers on redirected requests.
            */
            var responseStatusCode = HttpStatusCode.Redirect;
            // Matching UnityWebRequest redirectLimit default value of 32
            var maxHttpRedirection = 32;
            var httpRedirectionCount = 0;
            // Memorize original request scheme and host

            while (responseStatusCode == HttpStatusCode.Redirect && httpRedirectionCount < maxHttpRedirection)
            {
                // After first redirection
                if (httpRedirectionCount >= 1 && !TryHandleRedirection(state))
                {
                    // exit loop and complete request
                    break;
                }

                if (completionOption == HttpCompletionOption.ResponseContentRead)
                    await ProcessRequestWithResponseContentReadOption(state, uploadHandler, progress, cancellationToken);
                else
                    await ProcessRequestWithResponseHeadersReadOption(state, uploadHandler, progress, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();

                responseStatusCode = state.Response.StatusCode;

                if (responseStatusCode == HttpStatusCode.Redirect)
                    httpRedirectionCount++;
            }
        }

        static bool TryHandleRedirection(RequestState state)
        {
            var redirectLocation = state.Request.GetResponseHeader("Location");
            if (Uri.TryCreate(redirectLocation, UriKind.Absolute, out Uri redirectUri))
            {
                var originalRequestHost = state.OriginalHttpRequestMessage.RequestUri.Host;
                var originalRequestScheme = state.OriginalHttpRequestMessage.RequestUri.Scheme;

                // If redirection to a different scheme or host, reset HttpRequestMessage
                if (!redirectUri.Scheme.Equals(originalRequestScheme) ||
                    !redirectUri.Host.Equals((originalRequestHost)))
                {
                    RebuildHttpRequestMessageWithoutAuthorizationHeader(state, redirectLocation);
                }
                // Otherwise, keep original headers and body, only reset Uri
                else
                {
                    state.HttpRequestMessage.RequestUri = redirectUri;
                }
                return true;
            }
            return false;
        }

        static void RebuildHttpRequestMessageWithoutAuthorizationHeader(RequestState state, string redirectLocation)
        {
            var redirectHttRequestMessage = new HttpRequestMessage();
            redirectHttRequestMessage.RequestUri = new Uri(redirectLocation);

            state.HttpRequestMessage = redirectHttRequestMessage;
            state.HttpRequestMessage.Content = state.OriginalHttpRequestMessage.Content;
            state.HttpRequestMessage.Method = state.OriginalHttpRequestMessage.Method;
        }

        static async Task ProcessRequestWithResponseContentReadOption(RequestState state, UploadHandler uploadHandler,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            await PrepareAndStartRequest(state, () => { }, () => tcs.TrySetResult(true),
                uploadHandler, progress, cancellationToken);
            await tcs.Task;
        }

        static async Task ProcessRequestWithResponseHeadersReadOption(RequestState state, UploadHandler uploadHandler,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            void TryCompleteRequest(RequestState state, TaskCompletionSource<bool> tcs)
            {
                if (state.Response.StatusCode != HttpStatusCode.Redirect)
                    CompleteRequest(state);

                tcs.TrySetResult(true);
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            await PrepareAndStartRequest(state, () => tcs.TrySetResult(true),
                () => TryCompleteRequest(state, tcs), uploadHandler, progress, cancellationToken);
            await tcs.Task;
        }

        static async Task PrepareAndStartRequest(RequestState state, Action onHeadersReceived, Action onCompleted, UploadHandler uploadHandler,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            var httpRequestMessage = state.HttpRequestMessage;

            var request = new UnityWebRequest(httpRequestMessage.RequestUri, httpRequestMessage.Method.ToString());
            state.Request = request;

            foreach (var header in httpRequestMessage.Headers)
            {
                var value = string.Join(",", header.Value);
                request.SetRequestHeader(header.Key, value);
            }

            if (httpRequestMessage.Content != null)
            {
                foreach (var header in httpRequestMessage.Content.Headers)
                {
                    if (header.Key == k_ContentLengthHeaderKey)
                        continue;

                    var value = string.Join(",", header.Value);
                    request.SetRequestHeader(header.Key, value);
                }
            }

            var queueStream = new QueueStream();
            var memoryStreamDownloadHandler = new MemoryStreamDownloadHandler(queueStream.Writer);
            memoryStreamDownloadHandler.HeadersReceived += () =>
            {
                SetHeaders(state);
                onHeadersReceived();
            };

            request.downloadHandler = memoryStreamDownloadHandler;
            if (uploadHandler != null)
                request.uploadHandler = uploadHandler;
            request.disposeDownloadHandlerOnDispose = true;
            request.disposeUploadHandlerOnDispose = true;

#if !UNITY_WEBGL || UNITY_EDITOR
            // Force handling the Redirect response from server
            request.redirectLimit = 0;
#endif

            state.CancellationTokenRegistration = cancellationToken.Register(() =>
            {
                request.Abort();
                state.RequestDisposed = true;
                request.Dispose();
            });

            if (state.Timeout != default)
            {
                request.timeout = state.Timeout.Seconds;
            }

            var response = new SafeHttpResponseMessageAccessor();
            response.RequestMessage = httpRequestMessage;
            response.Content = new StreamContent(queueStream.Reader);
            response.StatusCode = 0;
            state.Response = response;

            await HandleRequestAndProgress(state, onCompleted, memoryStreamDownloadHandler, progress);
        }

        static async Task HandleRequestAndProgress(RequestState state, Action onCompleted, MemoryStreamDownloadHandler memoryStreamDownloadHandler, IProgress<HttpProgress> progress)
        {
            var request = state.Request;
            var asyncOp = request.SendWebRequest();
            asyncOp.completed += _ =>
            {
                if (!state.RequestDisposed)
                    state.Response.StatusCode = (HttpStatusCode)state.Request.responseCode;
                onCompleted();
            };

            if (progress == null)
                return;

            bool isUpload = request.uploadHandler != null;
            float? initialUploadProgress = isUpload ? 0 : null;
            progress.Report(new HttpProgress(0, initialUploadProgress));

            while (!asyncOp.isDone)
            {
                progress.Report(new HttpProgress(request.downloadProgress, isUpload ? request.uploadProgress : null));
                await Task.Yield();
            }

            float? finalDownloadProgress = memoryStreamDownloadHandler.DataReceived ? 1 : null;
            float? finalUploadProgress = isUpload ? 1 : null;
            progress.Report(new HttpProgress(finalDownloadProgress,
                finalUploadProgress));
        }

        static void CompleteRequest(RequestState state)
        {
            state.CancellationTokenRegistration?.Dispose();

            var request = state.Request;

            if (state.RequestDisposed)
                return;

            if (request.result != UnityWebRequest.Result.Success)
            {
                if (request.downloadHandler is MemoryStreamDownloadHandler memoryStreamDownloadHandler)
                {
                    memoryStreamDownloadHandler.ForceCompleteContent();
                    memoryStreamDownloadHandler.EffectiveDispose();
                }

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    var errorMessage = request.error;
                    state.RequestDisposed = true;
                    request.Dispose();

                    if (errorMessage == k_TimeoutErrorMessage)
                        throw new TaskCanceledException(errorMessage, new TimeoutException(errorMessage));

                    throw new HttpRequestException(errorMessage);
                }
            }

            SetHeaders(state);

            state.RequestDisposed = true;
            request.Dispose();
        }

        static void SetHeaders(RequestState state)
        {
            state.Response.StatusCode = (HttpStatusCode)state.Request.responseCode;

            var headers = state.Request.GetResponseHeaders();
            if (headers?.ContainsKey(k_ContentTypeHeaderKey) ?? false)
            {
                var substrings = headers[k_ContentTypeHeaderKey].Split(";");
                state.Response.Content.Headers.ContentType = new MediaTypeHeaderValue(substrings[0]);
            }

            if (headers?.ContainsKey(k_ContentLengthHeaderKey) ?? false)
            {
                if (long.TryParse(headers[k_ContentLengthHeaderKey], out var contentLength))
                    state.Response.Content.Headers.ContentLength = contentLength;
            }

            if (headers?.ContainsKey(k_ContentEncodingHeaderKey) ?? false)
            {
                var encodings = headers[k_ContentEncodingHeaderKey].Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var encoding in encodings)
                    state.Response.Content.Headers.ContentEncoding.Add(encoding);
            }
        }

        class RequestState
        {
            public HttpRequestMessage HttpRequestMessage;
            public HttpRequestMessage OriginalHttpRequestMessage;
            public UnityWebRequest Request;
            public SafeHttpResponseMessageAccessor Response;
            public IDisposable CancellationTokenRegistration;
            public TimeSpan Timeout;
            public bool RequestDisposed;

            public RequestState(
                HttpRequestMessage httpRequestMessage,
                UnityWebRequest request,
                SafeHttpResponseMessageAccessor response,
                TimeSpan timeout)
            {
                HttpRequestMessage = httpRequestMessage;
                OriginalHttpRequestMessage = httpRequestMessage;
                Request = request;
                Response = response;
                Timeout = timeout;
            }
        }

        class SafeHttpResponseMessageAccessor
        {
            SafeHttpResponseMessage m_Response = new();

            public HttpRequestMessage RequestMessage
            {
                set
                {
                    if (m_Response.IsDisposed)
                        return;

                    m_Response.RequestMessage = value;
                }
            }

            public HttpContent Content
            {
                get => m_Response.Content;
                set
                {
                    if (m_Response.IsDisposed)
                        return;

                    m_Response.Content = value;
                }
            }

            public HttpStatusCode StatusCode
            {
                get => m_Response.StatusCode;
                set
                {
                    if (m_Response.IsDisposed)
                        return;

                    m_Response.StatusCode = value;
                }
            }

            public static implicit operator HttpResponseMessage(SafeHttpResponseMessageAccessor obj) => obj.m_Response;

            class SafeHttpResponseMessage : HttpResponseMessage
            {
                protected override void Dispose(bool disposing)
                {
                    if (!IsDisposed)
                        IsDisposed = true;

                    base.Dispose(disposing);
                }

                public bool IsDisposed { get; private set; }
            }
        }
    }
}
