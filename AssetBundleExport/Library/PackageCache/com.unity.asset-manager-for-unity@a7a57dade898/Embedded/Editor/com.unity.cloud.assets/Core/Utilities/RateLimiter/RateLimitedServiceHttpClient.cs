using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /*
    The rate-limited service HTTP client is a decorator for the service HTTP client that adds rate limiting to the service HTTP client.
    The algorithm used for rate limiting is a token bucket rate limiter using the microsoft runtime library. We slightly modified the
    original implementation to fit our needs. In the original implementation, a never-ending timer is used to replenish the tokens. In our
    implementation, the timer is only started after the first request is made, and disposed when it resolves. This is to prevent the timer
    from running indefinitely when the rate-limited service HTTP client is not used. This also closely resembles the behavior of the
    cloud gateway back-end rate limiter, which works similarly, but without actual timer.

    Once the quota for a request is reached, the rate limiter will block the request until the quota is replenished, and then the request
    will be sent. The rate limiter is configured with a queue limit, tokens per period, token limit, and replenishment period. When setting
    the configuration values for the rate-limiter, we recommend using the same values as the back-end rate limiter, but cut in half. This
    prevents the rate-limited service HTTP client from blocking requests when the back-end rate limiter is not blocking requests, caused by
    a difference in time between the rate-limited service HTTP client and the back-end rate limiter.

    For example, if the back-end rate limiter is configured with a queue limit of 10, tokens per period of 10, token limit of 10, and
    replenishment period of 1 second, the rate-limited service HTTP client should be configured with a queue limit of 5, tokens per period
    of 5, token limit of 5, and replenishment period of 0.5 seconds.
    */
    class RateLimitedServiceHttpClient : IServiceHttpClient
    {
        readonly IServiceHttpClient m_ServiceHttpClient;
        readonly TokenBucketRateLimiter m_RateLimiter;

        public RateLimitedServiceHttpClient(IServiceHttpClient serviceHttpClient, int queueLimit, int tokensPerPeriod, int tokenLimit, TimeSpan replenishmentPeriod)
        {
            m_ServiceHttpClient = serviceHttpClient;
            m_RateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions()
            {
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = true,
                QueueLimit = queueLimit,
                TokensPerPeriod = tokensPerPeriod,
                TokenLimit = tokenLimit,
                ReplenishmentPeriod = replenishmentPeriod
            });
        }

        /// <inheritdoc />
        public TimeSpan Timeout
        {
            get => m_ServiceHttpClient.Timeout;
            set => m_ServiceHttpClient.Timeout = value;
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, IProgress<HttpProgress> progress,
            CancellationToken cancellationToken)
        {
            await m_RateLimiter.AcquireAsync(1, cancellationToken);
            return await m_ServiceHttpClient.SendAsync(request, completionOption, progress, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            await m_RateLimiter.AcquireAsync(1, cancellationToken);
            return await m_ServiceHttpClient.SendAsync(request, options, completionOption, progress, cancellationToken);
        }
    }
}
