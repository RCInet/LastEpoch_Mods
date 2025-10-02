using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    internal static class RetryPolicyHelpers
    {
        /// <exception cref="RetryExecutionFailedException">When <paramref name="shouldRetryChecker"/> throws a <see cref="RetryExecutionFailedException"/>, it is bubbled up.</exception>
        /// <exception cref="InvalidArgumentException">When <paramref name="shouldRetryChecker"/> throws any exception other than <see cref="RetryExecutionFailedException"/>. It is passed as innerException.</exception>
        public async static Task<(Task<T>, bool)> RunRetryOperation<T>(IRetryPolicy.RetriedOperation<T> retriedOperation, IRetryPolicy.ShouldRetryChecker<T> shouldRetryChecker, CancellationToken cancellationToken)
        {
            // First, run the task, and make sure it always returns a task
            Task<T> actionTask;
            try
            {
                actionTask = retriedOperation(cancellationToken);
            }
            catch (Exception e)
            {
                // Converting "immediate exceptions" into faulted tasks
                actionTask = Task.FromException<T>(e);
            }

            // Second, apply the shouldRetry method, and ensure it does not throw any invalid exception
            bool shouldRetryResult;
            try
            {
                // Switched from ConfigureAwait(false) to regular await operators to avoid unnecessary context switching.
                // The user is responsible for calling the method in the right synchronization context.
                shouldRetryResult = await shouldRetryChecker(actionTask);
            }
            catch (RetryExecutionFailedException)
            {
                // We bubble up the exception
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidArgumentException($"Provided shouldRetryChecker argument threw an exception : {ex.Message}", ex);
            }

            return (actionTask, shouldRetryResult);
        }

        /// <exception cref="OperationCanceledException">When shouldRetryResult is false, but the actionTask is cancelled.</exception>
        /// <exception cref="RetryExecutionFailedException">When shouldRetryResult is false, but the actionTask throws an exception, which is passed as innerException.</exception>
        /// <exception cref="TimeoutException">When shouldRetryResult is true, which means the RetryPolicy expired.</exception>
        /// <exception cref="InvalidArgumentException">When provided <paramref name="actionTask"/> is not completed.</exception>
        public static T GetRetryResult<T>(Task<T> actionTask, bool shouldRetryResult)
        {
            if (!actionTask.IsCompleted)
                throw new InvalidArgumentException("Provided shouldRetryChecker argument did not await for the operationTask.");

            if (!shouldRetryResult)
            {
                // ShouldRetry returned false.
                // We should bubble up the result or exception.

                if (actionTask.IsCompletedSuccessfully)
                    return actionTask.Result;

                if (actionTask.IsCanceled)
                    throw new OperationCanceledException();

                // If none of the above is valid, we assume the execution failed.
                throw new RetryExecutionFailedException(actionTask.Exception.InnerException);
            }

            // RetryPolicy expired.
            throw new TimeoutException("The retry policy for the operation has expired.", actionTask.Exception?.InnerException);
        }
    }
}
