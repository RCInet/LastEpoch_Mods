using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An interface for awaiting a specific amount of time.
    /// </summary>
    interface ITimeAwaiter
    {
        /// <summary>
        /// Awaits a specified amount of time.
        /// </summary>
        /// <param name="delay">The amount of time to wait.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        /// <returns>A <see cref="Task"/> for the wait operation.</returns>
        Task AwaitTimeAsync(TimeSpan delay, CancellationToken cancellationToken);
    }

    /// <summary>
    /// A class for awaiting a specific amount of time.
    /// </summary>
    class TimeAwaiter : ITimeAwaiter
    {
        async Task ITimeAwaiter.AwaitTimeAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            await UnityTask.Delay(delay, cancellationToken).UnityConfigureAwait(false);
        }
    }

    /// <summary>
    /// Helper methods for <see cref="ITimeAwaiter"/>.
    /// </summary>
    static class TimeAwaiterExtensions
    {
        public static Task AwaitTimeAsync(this ITimeAwaiter timeAwaiter, int delayMilliseconds, CancellationToken cancellationToken)
        {
            return timeAwaiter.AwaitTimeAsync(new TimeSpan(0, 0, 0, 0, delayMilliseconds), cancellationToken);
        }
    }
}
