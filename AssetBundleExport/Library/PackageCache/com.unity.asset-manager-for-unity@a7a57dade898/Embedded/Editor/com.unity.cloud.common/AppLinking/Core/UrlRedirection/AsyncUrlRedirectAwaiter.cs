using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <inheritdoc/>
    class AsyncUrlRedirectAwaiter : IUrlRedirectAwaiter
    {
        const int k_DefaultRefreshDelay = 500;
        const int k_DefaultTimeoutDelay = 600000; // 10 minutes

        readonly ITimeAwaiter m_Awaiter = new TimeAwaiter();

        int m_RefreshDelay;
        int m_TimeoutDelay;
        int m_CurrentWait;
        UrlRedirectResult? m_RedirectResult;

        /// <inheritdoc/>
        public int RefreshDelay
        {
            get => m_RefreshDelay;
            set => m_RefreshDelay = Math.Max(value, 0);
        }

        /// <inheritdoc/>
        public int TimeoutDelay
        {
            get => m_TimeoutDelay;
            set => m_TimeoutDelay = Math.Max(value, 0);
        }

        /// <inheritdoc/>
        public bool HasTimedOut => m_CurrentWait >= m_TimeoutDelay;

        /// <inheritdoc/>
        public bool HasResult => m_RedirectResult.HasValue;

        /// <inheritdoc/>
        public UrlRedirectResult? RedirectResult => m_RedirectResult;

        /// <summary>
        /// Creates an instance of <see cref="AsyncUrlRedirectAwaiter"/>.
        /// </summary>
        /// <param name="refreshDelay">The delay in milliseconds between checking for a <see cref="UrlRedirectResult"/>.</param>
        /// <param name="timeoutDelay">The max amount of time in milliseconds to wait for redirect before a timeout.</param>
        public AsyncUrlRedirectAwaiter(int refreshDelay = k_DefaultRefreshDelay, int timeoutDelay = k_DefaultTimeoutDelay)
        {
            RefreshDelay = refreshDelay;
            TimeoutDelay = timeoutDelay;
        }

        /// <summary>
        /// Creates an instance of <see cref="AsyncUrlRedirectAwaiter"/>.
        /// </summary>
        /// <param name="awaiter">An implementation of <see cref="ITimeAwaiter"/> for awaiting a specific amount of time.</param>
        /// <param name="refreshDelay">The delay in milliseconds between checking for a <see cref="UrlRedirectResult"/>.</param>
        /// <param name="timeoutDelay">The max amount of time in milliseconds to wait for redirect before a timeout.</param>
        public AsyncUrlRedirectAwaiter(ITimeAwaiter awaiter, int refreshDelay = k_DefaultRefreshDelay, int timeoutDelay = k_DefaultTimeoutDelay)
            : this(refreshDelay, timeoutDelay)
        {
            m_Awaiter = awaiter;
        }

        /// <inheritdoc/>
        public void BeginWait()
        {
            m_CurrentWait = 0;
            m_RedirectResult = null;
        }

        /// <inheritdoc/>
        public void SetResult(UrlRedirectResult? result)
        {
            m_RedirectResult = result;
        }

        /// <inheritdoc/>
        public async Task WaitForRefreshAsync(CancellationToken cancellationToken = default)
        {
            await m_Awaiter.AwaitTimeAsync(RefreshDelay, cancellationToken);
            m_CurrentWait += RefreshDelay;
        }
    }
}
