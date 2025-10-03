using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An implementation of <see cref="IRetryPolicy"/> that tries only once.
    /// </summary>
    class NoRetryPolicy : IRetryPolicy
    {
        /// <inheritdoc/>
        public async Task<T> ExecuteAsync<T>(IRetryPolicy.RetriedOperation<T> retriedOperation, IRetryPolicy.ShouldRetryChecker<T> shouldRetryChecker,
            CancellationToken cancellationToken = default, IProgress<RetryQueuedProgress> progress = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Switched from ConfigureAwait(false) to regular await operators to avoid unnecessary context switching.
            // The user is responsible for calling the method in the right synchronization context.
            var (actionTask, shouldRetryResult) = await RetryPolicyHelpers.RunRetryOperation(retriedOperation, shouldRetryChecker, cancellationToken);

            return RetryPolicyHelpers.GetRetryResult(actionTask, shouldRetryResult);
        }
    }
}
