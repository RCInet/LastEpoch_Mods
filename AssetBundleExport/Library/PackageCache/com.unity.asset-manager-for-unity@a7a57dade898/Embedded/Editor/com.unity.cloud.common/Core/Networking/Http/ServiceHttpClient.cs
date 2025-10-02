using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A class to configure options for a <see cref="IServiceHttpClient"/>
    /// </summary>
    struct ServiceHttpClientOptions
    {
        /// <summary>
        /// Whether to skip the default authentication flow.
        /// </summary>
        public bool SkipDefaultAuthentication { get; }

        /// <summary>
        /// Whether to fill the default headers.
        /// </summary>
        public bool SkipDefaultHeaders { get; }

        /// <summary>
        /// Whether to skip error processing.
        /// </summary>
        public bool SkipErrorProcessing { get; }

        /// <summary>
        /// Whether to use the base <see cref="IHttpClient"/> to send requests.
        /// </summary>
        public bool UseBaseHttpClientOnly { get; }

        IRetryPolicy m_RetryPolicy;

        /// <summary>
        /// The <see cref="IRetryPolicy"/> to be used by the client.
        /// </summary>
        public IRetryPolicy RetryPolicy => m_RetryPolicy ??= new ExponentialBackoffRetryPolicy();

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceHttpClientOptions"/>.
        /// </summary>
        /// <param name="skipDefaultAuthentication">Whether to skip the default authentication flow.</param>
        /// <param name="skipDefaultHeaders">Whether to fill the default headers.</param>
        /// <param name="skipErrorProcessing">Whether to skip error processing.</param>
        /// <param name="useBaseHttpClientOnly">Whether to use the base HTTP client to send requests.</param>
        /// <param name="retryPolicy">The retry policy to use for the client.</param>
        public ServiceHttpClientOptions(bool skipDefaultAuthentication, bool skipDefaultHeaders, bool skipErrorProcessing,
            bool useBaseHttpClientOnly, IRetryPolicy retryPolicy = null)
        {
            SkipDefaultAuthentication = skipDefaultAuthentication;
            SkipDefaultHeaders = skipDefaultHeaders;
            SkipErrorProcessing = skipErrorProcessing;
            UseBaseHttpClientOnly = useBaseHttpClientOnly;
            m_RetryPolicy = retryPolicy ?? CreateDefaultRetryPolicy();
        }

        static IRetryPolicy CreateDefaultRetryPolicy()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return new ExponentialBackoffRetryPolicy(TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(100));
#else
            return new ExponentialBackoffRetryPolicy();
#endif
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceHttpClientOptions"/> with default settings.
        /// </summary>
        /// <returns>The specific client options.</returns>
        public static ServiceHttpClientOptions Default() => new ServiceHttpClientOptions(false, false, false, false, CreateDefaultRetryPolicy());

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceHttpClientOptions"/> which skips default authentication.
        /// </summary>
        /// <returns>The specific client options.</returns>
        public static ServiceHttpClientOptions SkipDefaultAuthenticationOption() => new ServiceHttpClientOptions(true, false, false, false, CreateDefaultRetryPolicy());

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceHttpClientOptions"/> which skips adding default headers.
        /// </summary>
        /// <returns>The specific client options.</returns>
        public static ServiceHttpClientOptions SkipDefaultHeadersOption() => new ServiceHttpClientOptions(false, true, false, false, CreateDefaultRetryPolicy());

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceHttpClientOptions"/> which skips error processing.
        /// </summary>
        /// <returns>The specific client options.</returns>
        public static ServiceHttpClientOptions SkipErrorProcessingOption() => new ServiceHttpClientOptions(false, false, true, false, CreateDefaultRetryPolicy());

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceHttpClientOptions"/> which uses the base <see cref="IHttpClient"/>.
        /// </summary>
        /// <returns>The specific client options.</returns>
        public static ServiceHttpClientOptions UseBaseHttpClientOnlyOption() => new ServiceHttpClientOptions(false, false, false, true, CreateDefaultRetryPolicy());

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceHttpClientOptions"/> with no retry policy.
        /// </summary>
        /// <returns>The specific client options.</returns>
        public static ServiceHttpClientOptions NoRetryOption() => new ServiceHttpClientOptions(false, false, false, false, new NoRetryPolicy());
    }

    /// <summary>
    /// An implementation of an HTTP client which abstracts the Task of sending <see cref="HttpRequestMessage"/>.
    /// </summary>
    class ServiceHttpClient : IServiceHttpClient
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<ServiceHttpClient>();

        readonly IHttpClient m_BaseHttpClient;
        readonly IServiceAuthorizer m_ServiceAuthorizer;
        readonly IAppIdProvider m_AppIdProvider;

        /// <summary>
        /// The client trace.
        /// </summary>
        public static readonly string ClientTrace = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceHttpClient"/>
        /// </summary>
        /// <param name="baseHttpClient">The base HTTP client.</param>
        /// <param name="serviceAuthorizer">The authorizer to apply the authorization information to requests.</param>
        /// <param name="appIdProvider">The App ID provider.</param>
        /// <exception cref="ArgumentNullException">Thrown if either <paramref name="baseHttpClient"/> or <paramref name="serviceAuthorizer"/> are null.</exception>
        public ServiceHttpClient(IHttpClient baseHttpClient,
            IServiceAuthorizer serviceAuthorizer,
            IAppIdProvider appIdProvider)
        {
            m_BaseHttpClient = baseHttpClient ?? throw new ArgumentNullException(nameof(baseHttpClient));
            m_ServiceAuthorizer = serviceAuthorizer ?? throw new ArgumentNullException(nameof(serviceAuthorizer));
            m_AppIdProvider = appIdProvider;
        }

        /// <summary>
        /// The timespan to wait before the request times out.
        /// </summary>
        public TimeSpan Timeout
        {
            get => m_BaseHttpClient.Timeout;
            set => m_BaseHttpClient.Timeout = value;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            return SendAsync(request, ServiceHttpClientOptions.Default(), completionOption, progress, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            if (options.UseBaseHttpClientOnly)
            {
                return await SendBaseHttpClientAsync(request, completionOption, options, ResponseValidatorNoErrorProcessing, progress, cancellationToken);
            }

            if (!options.SkipDefaultHeaders)
            {
                request.Headers.AddAppIdAndClientTrace(m_AppIdProvider?.GetAppId() ?? AppId.None, ClientTrace);
            }

            if (!options.SkipDefaultAuthentication)
            {
                await m_ServiceAuthorizer.AddAuthorization(request.Headers);
            }

            if (options.SkipErrorProcessing)
            {
                return await SendBaseHttpClientAsync(request, completionOption, options, ResponseValidatorNoErrorProcessing, progress,
                    cancellationToken);
            }

            return await SendWithErrorProcessingAsync(request, completionOption, options, progress, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ServiceHttpClientOptions options,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            return await SendAsync(request, options, HttpCompletionOption.ResponseContentRead, progress, cancellationToken);
        }

        async Task<HttpResponseMessage> SendBaseHttpClientAsync(HttpRequestMessage request, HttpCompletionOption completionOption, ServiceHttpClientOptions options,
            IRetryPolicy.ShouldRetryResultChecker<HttpResponseMessage> responseValidator, IProgress<HttpProgress> progress = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await ExecuteAsyncWithResultAndExceptionValidation(options.RetryPolicy,
                    ct => m_BaseHttpClient.SendAsync(request, completionOption, progress, ct), responseValidator,
                    cancellationToken);
            }
            catch (RetryExecutionFailedException retryExecutionFailedException)
            {
                // We throw the inner exception to hide IRetryPolicy exceptions and keep IServiceHttpClient
                // closer to IHttpClient in terms of exceptions.
                if(retryExecutionFailedException.InnerException != null)
                    throw retryExecutionFailedException.InnerException;

                throw;
            }
        }

        Task<HttpResponseMessage> ExecuteAsyncWithResultAndExceptionValidation(IRetryPolicy retryPolicy,
            IRetryPolicy.RetriedOperation<HttpResponseMessage> retriedOperation,
            IRetryPolicy.ShouldRetryResultChecker<HttpResponseMessage> shouldRetryResultChecker,
            CancellationToken cancellationToken = default, IProgress<RetryQueuedProgress> progress = default)
        {
            return retryPolicy.ExecuteAsync(retriedOperation, async result =>
            {
                HttpResponseMessage taskResult;
                try
                {
                    // Switched from ConfigureAwait(false) to regular await operators to avoid unnecessary context switching.
                    // The user is responsible for calling the method in the right synchronization context.
                    taskResult = await result;
                }
                catch (HttpRequestException)
                {
                    return true;
                }
                catch (OperationCanceledException operationCanceledException)
                {
                    if (operationCanceledException.InnerException is TimeoutException)
                        return true;

                    throw new RetryExecutionFailedException(operationCanceledException);
                }
                catch (Exception exception)
                {
                    // Stop the retry and bubble up the exception
                    throw new RetryExecutionFailedException(exception);
                }

                return await shouldRetryResultChecker(taskResult);
            }, cancellationToken, progress);
        }

        Task<bool> ResponseValidatorNoErrorProcessing(HttpResponseMessage response)
        {
            return Task.FromResult(response.StatusCode == HttpStatusCode.RequestTimeout || (int)response.StatusCode == 503 || (int)response.StatusCode == 504);
        }

        async Task<HttpResponseMessage> SendWithErrorProcessingAsync(HttpRequestMessage request, HttpCompletionOption completionOption, ServiceHttpClientOptions options,
            IProgress<HttpProgress> progress = default, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await SendBaseHttpClientAsync(request, completionOption, options, ResponseValidatorNoErrorProcessing, progress,
                    cancellationToken);
                return await ProcessResponseErrorAsync(response);
            }
            catch (HttpRequestException e)
            {
                throw new ConnectionException(ConnectionException.k_DefaultMessage, e);
            }
        }

        async Task<HttpResponseMessage> ProcessResponseErrorAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            ServiceError serviceError;
            try
            {
                s_Logger.LogDebug($"Unsuccessful StatusCode {response.StatusCode } on {response.RequestMessage.Method} request: {response.RequestMessage.RequestUri}");
                var content = await response.GetContentAsStringAsync();
                serviceError = string.IsNullOrEmpty(content) ? new ServiceError { Status = response.StatusCode } : JsonSerialization.Deserialize<ServiceError>(content);
                serviceError ??= new ServiceError { Status = response.StatusCode };
            }
            catch (Exception)
            {
                serviceError = new ServiceError { Status = response.StatusCode };
            }

            throw ServiceExceptionFactory.Build(serviceError);
        }
    }
}
