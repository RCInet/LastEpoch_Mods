using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{

    /// <summary>
    /// Contains information about a RetryQueue event, when using <see cref="IRetryPolicy.ExecuteAsync{T}(IRetryPolicy.RetriedOperation{T}, IRetryPolicy.ShouldRetryChecker{T}, CancellationToken, IProgress{RetryQueuedProgress})"/>
    /// or its additional extensions.
    /// </summary>
    readonly struct RetryQueuedProgress
    {
        /// <summary>
        /// The number of retries since the first attempt.
        /// </summary>
        public int RetryCount { get; }

        /// <summary>
        /// The delay that will be awaited before next attempt.
        /// </summary>
        public TimeSpan DelayUntilNextTry { get; }

        /// <summary>
        /// Creates a <see cref="RetryQueuedProgress"/>.
        /// </summary>
        /// <param name="count">The number of retries since the first attempt.</param>
        /// <param name="delay">The delay that will be awaited before next attempt.</param>
        public RetryQueuedProgress(int count, TimeSpan delay)
        {
            RetryCount = count;
            DelayUntilNextTry = delay;
        }
    }

    /// <summary>
    /// Interface which abstracts Retry Policies.
    /// </summary>
    interface IRetryPolicy
    {
        /// <summary>
        /// Delegate to define the operation that might be retried.
        /// </summary>
        /// <typeparam name="T">The type of the result for the operation.</typeparam>
        /// <param name="cancellationToken">Use this token to handle cancellation.</param>
        public delegate Task<T> RetriedOperation<T>(CancellationToken cancellationToken);

        /// <summary>
        /// Delegate to define the operation that might be retried.
        /// </summary>
        /// <param name="cancellationToken">Use this token to handle cancellation.</param>
        public delegate Task RetriedOperation(CancellationToken cancellationToken);

        /// <summary>
        /// Delegate to define whether an operation should be retried.
        /// <see cref="RetryExecutionFailedException"/> is the only valid exception that can be thrown.
        /// </summary>
        /// <typeparam name="T">The type of the result for the operation.</typeparam>
        /// <param name="operationTask">Use this task to await, and handle the result.</param>
        /// <returns>Whether the operation should be retried.</returns>
        /// <exception cref="RetryExecutionFailedException">Throw this exception if you want to immediately bubble up its innerException. This will stop the RetryPolicy.</exception>
        public delegate Task<bool> ShouldRetryChecker<T>(Task<T> operationTask);

        /// <summary>
        /// Use this delegate to define which exceptions trigger a retry.
        /// It should NOT throw any exception.
        /// </summary>
        /// <param name="thrownException">Use this exception to decide whether a retry is needed.</param>
        /// <returns>Whether the operation should be retried.</returns>
        public delegate bool ShouldRetryExceptionChecker(Exception thrownException);

        /// <summary>
        /// Use this delegate to define which results trigger a retry.
        /// It should NOT throw any exception.
        /// </summary>
        /// <typeparam name="T">The type of the result for the operation.</typeparam>
        /// <param name="result">Use this result to decide whether a retry is needed.</param>
        /// <returns>Whether the operation should be retried.</returns>
        public delegate Task<bool> ShouldRetryResultChecker<T>(T result);

        /// <summary>
        /// An async method that can retry an operation if it hasn't succeeded.
        /// </summary>
        /// <typeparam name="T">The type of the result for the operation.</typeparam>
        /// <param name="retriedOperation">The <see cref="RetriedOperation{T}"/> that needs to be performed.</param>
        /// <param name="shouldRetryChecker">A <see cref="ShouldRetryChecker{T}"/> that helps determine whether the <paramref name="retriedOperation"/> should be retried.</param>
        /// <param name="cancellationToken">Token to cancel the task execution.</param>
        /// <param name="progress">Provider for progress updates on queued retries.</param>
        /// <returns>
        /// The result of the <paramref name="retriedOperation"/>, if available, after <paramref name="shouldRetryChecker"/> returns false.
        /// </returns>
        /// <exception cref="RetryExecutionFailedException">
        /// - When <paramref name="shouldRetryChecker"/> returns false after <paramref name="retriedOperation"/> throws an exception, in which case the internal exception is passed through the former's innerException field.
        /// - When <paramref name="shouldRetryChecker"/> throws an <see cref="RetryExecutionFailedException"/>, in which case this exception is immediately bubbled up.
        /// </exception>
        /// <exception cref="TimeoutException">When the retry policy expires.</exception>
        /// <exception cref="OperationCanceledException">When the <paramref name="retriedOperation"/> or the <paramref name="cancellationToken"/> is cancelled.</exception>
        /// <exception cref="InvalidArgumentException">
        /// - When <paramref name="shouldRetryChecker"/> throws an invalid exception.
        /// - When <paramref name="shouldRetryChecker"/> does not await the completion of operationTask.
        /// </exception>
        Task<T> ExecuteAsync<T>(RetriedOperation<T> retriedOperation, ShouldRetryChecker<T> shouldRetryChecker,
            CancellationToken cancellationToken = default, IProgress<RetryQueuedProgress> progress = default);
    }

    /// <summary>
    /// Helper methods for <see cref="IRetryPolicy"/>.
    /// </summary>
    static class RetryPolicyExtensions
    {
        /// <summary>
        /// An async method that can retry an operation if it hasn't succeeded ; validation checking is performed on the exception thrown by the operation.
        /// </summary>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="retriedOperation">The <see cref="IRetryPolicy.RetriedOperation"/> that needs to be performed.</param>
        /// <param name="shouldRetryExceptionChecker">A <see cref="IRetryPolicy.ShouldRetryExceptionChecker"/> that helps determine whether the <paramref name="retriedOperation"/> should be retried, based on its exception.</param>
        /// <param name="cancellationToken">Token to cancel the task execution.</param>
        /// <param name="progress">Provider for progress updates on queued retries.</param>
        /// <returns>
        /// The result of the <paramref name="retriedOperation"/>, after <paramref name="shouldRetryExceptionChecker"/> returns false.
        /// </returns>
        /// <exception cref="RetryExecutionFailedException">When <paramref name="shouldRetryExceptionChecker"/> returns false after <paramref name="retriedOperation"/> throws an exception. The internal exception is passed through the former's innerException field.</exception>
        /// <exception cref="TimeoutException">When the retry policy expires.</exception>
        /// <exception cref="OperationCanceledException">When the <paramref name="retriedOperation"/> or the <paramref name="cancellationToken"/> is cancelled.</exception>
        public static Task ExecuteAsyncWithExceptionValidation(this IRetryPolicy retryPolicy, IRetryPolicy.RetriedOperation retriedOperation,
            IRetryPolicy.ShouldRetryExceptionChecker shouldRetryExceptionChecker, CancellationToken cancellationToken = default, IProgress<RetryQueuedProgress> progress = default)
        {
            return retryPolicy.ExecuteAsyncWithExceptionValidation<bool>(async ct => { await retriedOperation(ct); return true; },
                shouldRetryExceptionChecker, cancellationToken, progress);
        }

        /// <summary>
        /// An async method that can retry an operation if it hasn't succeeded ; validation checking is performed on the exception thrown by the operation.
        /// </summary>
        /// <typeparam name="T">The type of the result for the operation.</typeparam>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="retriedOperation">The <see cref="IRetryPolicy.RetriedOperation{T}"/> that needs to be performed.</param>
        /// <param name="shouldRetryExceptionChecker">A <see cref="IRetryPolicy.ShouldRetryExceptionChecker"/> that helps determine whether the <paramref name="retriedOperation"/> should be retried, based on its exception.</param>
        /// <param name="cancellationToken">Token to cancel the task execution.</param>
        /// <param name="progress">Provider for progress updates on queued retries.</param>
        /// <returns>
        /// The result of the <paramref name="retriedOperation"/>, after <paramref name="shouldRetryExceptionChecker"/> returns false.
        /// </returns>
        /// <exception cref="RetryExecutionFailedException">When <paramref name="shouldRetryExceptionChecker"/> returns false after <paramref name="retriedOperation"/> throws an exception. The internal exception is passed through the former's innerException field.</exception>
        /// <exception cref="TimeoutException">When the retry policy expires.</exception>
        /// <exception cref="OperationCanceledException">When the <paramref name="retriedOperation"/> or the <paramref name="cancellationToken"/> is cancelled.</exception>
        public static Task<T> ExecuteAsyncWithExceptionValidation<T>(this IRetryPolicy retryPolicy, IRetryPolicy.RetriedOperation<T> retriedOperation,
            IRetryPolicy.ShouldRetryExceptionChecker shouldRetryExceptionChecker, CancellationToken cancellationToken = default, IProgress<RetryQueuedProgress> progress = default)
        {
            return retryPolicy.ExecuteAsync<T>(retriedOperation, async result =>
            {
                T taskResult = default;
                try
                {
                    // Switched from ConfigureAwait(false) to regular await operators to avoid unnecessary context switching.
                    // The user is responsible for calling the method in the right synchronization context.
                    taskResult = await result;
                    return false;
                }
                catch (Exception exception)
                {
                    return shouldRetryExceptionChecker(exception);
                }
            }, cancellationToken, progress);
        }

        /// <summary>
        /// An async method that can retry an operation if it hasn't succeeded ; validation checking is performed on the result of the operation.
        /// </summary>
        /// <typeparam name="T">The type of the result for the operation.</typeparam>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="retriedOperation">The <see cref="IRetryPolicy.RetriedOperation{T}"/> that needs to be performed.</param>
        /// <param name="shouldRetryResultChecker">A <see cref="IRetryPolicy.ShouldRetryResultChecker{T}"/> that helps determine whether the <paramref name="retriedOperation"/> should be retried, based on its result.</param>
        /// <param name="cancellationToken">Token to cancel the task execution.</param>
        /// <param name="progress">Provider for progress updates on queued retries.</param>
        /// <returns>
        /// The result of the <paramref name="retriedOperation"/>, after <paramref name="shouldRetryResultChecker"/> returns false.
        /// </returns>
        /// <exception cref="RetryExecutionFailedException">When <paramref name="retriedOperation"/> throws an exception, regardless of the result of <paramref name="shouldRetryResultChecker"/>. The internal exception is passed through the former's innerException field.</exception>
        /// <exception cref="TimeoutException">When the retry policy expires.</exception>
        /// <exception cref="OperationCanceledException">When the <paramref name="retriedOperation"/> or the <paramref name="cancellationToken"/> is cancelled.</exception>
        public static Task<T> ExecuteAsyncWithResultValidation<T>(this IRetryPolicy retryPolicy, IRetryPolicy.RetriedOperation<T> retriedOperation, IRetryPolicy.ShouldRetryResultChecker<T> shouldRetryResultChecker,
            CancellationToken cancellationToken = default, IProgress<RetryQueuedProgress> progress = default)
        {
            return retryPolicy.ExecuteAsync(retriedOperation, async result =>
            {
                T taskResult;
                try
                {
                    // Switched from ConfigureAwait(false) to regular await operators to avoid unnecessary context switching.
                    // The user is responsible for calling the method in the right synchronization context.
                    taskResult = await result;
                }
                catch (Exception e)
                {
                    // Stop the retry and bubble up the exception
                    throw new RetryExecutionFailedException(e);
                }

                return await shouldRetryResultChecker(taskResult);
            }, cancellationToken, progress);
        }
    }
}
