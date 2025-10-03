using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <summary>
    /// Configures the refresh rate and timeout delay for awaiting a <see cref="UrlRedirectResult"/>.
    /// </summary>
    interface IUrlRedirectAwaiter
    {
        /// <summary>
        /// The delay in milliseconds between checking for a <see cref="UrlRedirectResult"/>.
        /// </summary>
        int RefreshDelay { get; set; }

        /// <summary>
        /// The max amount of time in milliseconds to wait for redirect before a timeout.
        /// </summary>
        int TimeoutDelay { get; set; }

        /// <summary>
        /// Whether the total wait time has exceeded the <see cref="TimeoutDelay"/>.
        /// </summary>
        bool HasTimedOut { get; }

        /// <summary>
        /// Whether expected result has been received.
        /// </summary>
        bool HasResult { get; }

        /// <summary>
        /// The awaited <see cref="UrlRedirectResult"/>.
        /// </summary>
        UrlRedirectResult? RedirectResult { get; }

        /// <summary>
        /// Initializes the wait period and clears any pre-existing results.
        /// </summary>
        void BeginWait();

        /// <summary>
        /// Sets the awaited <see cref="UrlRedirectResult"/>.
        /// </summary>
        /// <param name="result">The result of the redirect operation.</param>
        void SetResult(UrlRedirectResult? result);

        /// <summary>
        /// Waits for the specific <see cref="RefreshDelay"/> period.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        /// <returns>A <see cref="Task"/> that completes when the <see cref="UrlRedirectResult"/> has been received or the timeout period has elapsed.</returns>
        /// <exception cref="TaskCanceledException">Thrown if the task is cancelled</exception>
        Task WaitForRefreshAsync(CancellationToken cancellationToken = default);
    }
}
