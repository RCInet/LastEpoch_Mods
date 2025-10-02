using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// This exception is thrown by <see cref="IRetryPolicy.ExecuteAsync{T}(IRetryPolicy.RetriedOperation{T}, IRetryPolicy.ShouldRetryChecker{T}, System.Threading.CancellationToken, IProgress{RetryQueuedProgress})"/>
    /// when a critical exception (passed as innerException) occurs during execution of the retry policy.
    /// </summary>
    [Serializable]
class RetryExecutionFailedException : Exception
    {
        /// <summary>
        /// Creates an instance of <see cref="RetryExecutionFailedException"/>.
        /// </summary>
        /// <param name="innerException">The inner exception that triggered this failure of retry execution.</param>
        public RetryExecutionFailedException(Exception innerException) : base(default, innerException)
        { }

        /// <summary>
        /// Creates an instance of <see cref="RetryExecutionFailedException"/>.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">the streaming context.</param>
        protected RetryExecutionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }

}
