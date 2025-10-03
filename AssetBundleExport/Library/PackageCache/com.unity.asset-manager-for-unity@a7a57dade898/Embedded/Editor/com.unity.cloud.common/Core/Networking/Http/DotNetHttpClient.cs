using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An implementation of IHttpClient for .Net specific applications
    /// </summary>
    class DotNetHttpClient : IHttpClient, IDisposable
    {
        const string k_TimeoutMessage = "The operation has timed out.";

        HttpClient m_HttpClient;

        /// <summary>
        /// Initializes and returns an instance of <see cref="DotNetHttpClient"/>.
        /// </summary>
        public DotNetHttpClient()
        {
            m_HttpClient = new System.Net.Http.HttpClient();
        }

        /// <inheritdoc/>
        public TimeSpan Timeout
        {
            get => m_HttpClient.Timeout;
            set => m_HttpClient.Timeout = value;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                return await TrySendRequestAsync(request, completionOption, progress, cancellationToken);
            }
            catch (InvalidOperationException)
            {
                // We clone the request and re-send it to bypass the InvalidOperationException
                return await TrySendRequestAsync(await CloneRequest(request), completionOption, progress, cancellationToken);
            }
        }

        /// <summary>
        /// Ensure internal disposal of any IDisposable references.
        /// </summary>
        /// <param name="disposing">Dispose pattern boolean value received from public Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_HttpClient?.Dispose();
                m_HttpClient = null;
            }
        }

        /// <summary>
        /// Ensure disposal of any IDisposable references.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        async Task<HttpResponseMessage> TrySendRequestAsync(HttpRequestMessage request, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;

            try
            {
                var requestContentLength = request.Content?.Headers.ContentLength;
                await using var uploadStream =
                    request.Content != null ? await request.Content.ReadAsStreamAsync() : null;
                var keepReportingProgress = true;

                if (progress != null)
                {
                    float? initialUploadProgress = requestContentLength.HasValue ? 0 : null;
                    progress.Report(new HttpProgress(0, initialUploadProgress));

                    if (requestContentLength.HasValue)
                    {
                        new Task(() =>
                        {
                            ReportUploadProgressAsync(uploadStream, progress, ref keepReportingProgress);
                        }).Start();
                    }
                }

                response = await m_HttpClient.SendAsync(request, completionOption, cancellationToken);
                keepReportingProgress = false;

                await HandleDownloadProgressAsync(response, requestContentLength, progress, cancellationToken);
            }
            catch (Exception exception)
            {
                if ((exception is TaskCanceledException && !cancellationToken.IsCancellationRequested) ||
                    (exception.InnerException is WebException webException && webException.Status == WebExceptionStatus.Timeout))
                    throw new TaskCanceledException(exception.Message, new TimeoutException(k_TimeoutMessage));

                throw;
            }

            return response;
        }

        void ReportUploadProgressAsync(Stream stream, IProgress<HttpProgress> progress, ref bool keepReportingProgress)
        {
            while (keepReportingProgress)
            {
                progress.Report(new HttpProgress(0, (float)stream.Position / stream.Length));

                Thread.Sleep(100);
            }
        }

        async Task HandleDownloadProgressAsync(HttpResponseMessage response, long? requestContentLength, IProgress<HttpProgress> progress,
            CancellationToken cancellationToken = default)
        {
            var responseContentLength = response.Content?.Headers.ContentLength;
            float? finalUploadProgress = requestContentLength.HasValue ? 1 : null;

            if (progress != null)
            {
                if (responseContentLength.HasValue)
                {
                    var downloadStream = await response.Content.ReadAsStreamAsync();
                    await ReportDownloadProgressAsync(downloadStream, responseContentLength.Value,
                        finalUploadProgress, progress, cancellationToken);
                    downloadStream.Position = 0;
                }

                float? finalDownloadProgress = responseContentLength.HasValue ? 1 : null;
                progress.Report(new HttpProgress(finalDownloadProgress, finalUploadProgress));
            }
        }

        async Task ReportDownloadProgressAsync(Stream stream, long contentLength, float? uploadProgress, IProgress<HttpProgress> progress,
            CancellationToken cancellationToken = default)
        {
            var buffer = new byte[81920];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                totalBytesRead += bytesRead;
                progress.Report(new HttpProgress((float)totalBytesRead / contentLength, uploadProgress));
            }
        }

        async Task<HttpRequestMessage> CloneRequest(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);
            clone.Content = await CloneContent(request.Content);
            clone.Version = request.Version;

            foreach (var property in request.Properties)
                clone.Properties.Add(property);

            foreach (var header in request.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }

        async Task<HttpContent> CloneContent(HttpContent content)
        {
            if (content == null)
                return null;

            var memoryStream = new MemoryStream();
            await content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var clone = new StreamContent(memoryStream);
            foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
                clone.Headers.Add(header.Key, header.Value);

            return clone;
        }
    }
}
