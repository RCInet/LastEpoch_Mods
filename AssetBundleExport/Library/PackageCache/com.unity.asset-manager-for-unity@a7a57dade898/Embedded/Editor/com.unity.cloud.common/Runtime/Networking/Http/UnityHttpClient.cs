using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// An implementation of IHttpClient for Unity specific platforms
    /// </summary>
    class UnityHttpClient : IHttpClient
    {
        const long k_DefaultMaximumUploadSizeForMemoryStorageBytes = 1_000_000;

        readonly LegacyRequestHandler m_RequestHandler;
        readonly long m_MaximumUploadSizeForMemoryStorage;

        /// <summary>
        /// Initializes and returns an instance of <see cref="UnityHttpClient"/>.
        /// </summary>
        public UnityHttpClient() : this(k_DefaultMaximumUploadSizeForMemoryStorageBytes)
        { }

        /// <summary>
        /// Initializes and returns an instance of <see cref="UnityHttpClient"/>.
        /// </summary>
        /// <param name="maximumUploadSizeForMemoryStorage">The maximum upload size that the client will store in memory.
        /// Bigger payloads will be stored in a temporary file.</param>
        public UnityHttpClient(long maximumUploadSizeForMemoryStorage)
        {
            m_RequestHandler = new LegacyRequestHandler();

            m_MaximumUploadSizeForMemoryStorage = maximumUploadSizeForMemoryStorage;
        }

        /// <inheritdoc/>
        public TimeSpan Timeout
        {
            get => m_RequestHandler.Timeout;
            set => m_RequestHandler.Timeout = value;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            string tempFilepath = null;
            try
            {
                UploadHandler uploadHandler = null;

                var requestContent = request.Content;
                if (requestContent != null)
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    uploadHandler = new UploadHandlerRaw(await request.GetContentAsByteArrayAsync());
#else
                    await using var source = await requestContent.ReadAsStreamAsync();

                    if (source is FileStream fileStream)
                    {
                        uploadHandler = new UploadHandlerFile(fileStream.Name);
                    }
                    else if (requestContent.Headers.ContentLength > m_MaximumUploadSizeForMemoryStorage)
                    {
                        tempFilepath = Path.GetTempPath() + Guid.NewGuid();

                        await using var destination = File.OpenWrite(tempFilepath);
                        await source.CopyToAsync(destination, cancellationToken);

                        uploadHandler = new UploadHandlerFile(tempFilepath);
                    }
                    else
                    {
                        uploadHandler = new UploadHandlerRaw(await requestContent.ReadAsByteArrayAsync());
                    }
#endif
                }

                using var trace = NetworkProfiler.Trace();
                var progressTracer = trace.CreateProgressTracer(progress);
                trace.SetRequestData(request, completionOption);
                var response = await m_RequestHandler.RequestAsync(request, uploadHandler, completionOption, progressTracer, cancellationToken);
                trace.SetResponseData(response);

                return response;
            }
            finally
            {
#if !UNITY_WEBGL
                if (!string.IsNullOrEmpty(tempFilepath) && File.Exists(tempFilepath))
                    File.Delete(tempFilepath);
#endif
            }
        }
    }
}
