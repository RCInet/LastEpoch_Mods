// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// From https://github.com/dotnet/runtime/ at commit 06062e79faab44195f56cd7e4079b22d2380aedd
#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Abstraction that specifies that the <see cref="RateLimiter"/> implementation is capable of replenishing tokens.
    /// </summary>
    internal abstract class ReplenishingRateLimiter : RateLimiter
    {
        /// <summary>
        /// Specifies how often the <see cref="ReplenishingRateLimiter"/> will replenish tokens.
        /// If <see cref="IsAutoReplenishing"/> is <see langword="false"/> then this is how often <see cref="TryReplenish"/> should be called.
        /// </summary>
        public abstract TimeSpan ReplenishmentPeriod { get; }

        /// <summary>
        /// Specifies if the <see cref="ReplenishingRateLimiter"/> is automatically replenishing
        /// its tokens or if it expects an external source to regularly call <see cref="TryReplenish"/>.
        /// </summary>
        public abstract bool IsAutoReplenishing { get; }

        /// <summary>
        /// Attempts to replenish tokens.
        /// </summary>
        /// <returns>
        /// Generally returns <see langword="false"/> if <see cref="IsAutoReplenishing"/> is enabled
        /// or if no tokens were replenished. Otherwise <see langword="true"/>.
        /// </returns>
        public abstract bool TryReplenish();
    }

    /// <summary>
    /// Represents a limiter type that users interact with to determine if an operation can proceed.
    /// </summary>
    internal abstract class RateLimiter : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Gets a snapshot of the <see cref="RateLimiter"/> statistics if available.
        /// </summary>
        /// <returns>An instance of <see cref="RateLimiterStatistics"/> containing a snapshot of the <see cref="RateLimiter"/> statistics.</returns>
        public abstract RateLimiterStatistics? GetStatistics();

        /// <summary>
        /// Specifies how long the <see cref="RateLimiter"/> has had all permits available. Used by RateLimiter managers that may want to
        /// clean up unused RateLimiters.
        /// </summary>
        /// <remarks>
        /// Returns <see langword="null"/> when the <see cref="RateLimiter"/> is in use or is not ready to be idle.
        /// </remarks>
        public abstract TimeSpan? IdleDuration { get; }

        /// <summary>
        /// Fast synchronous attempt to acquire permits.
        /// </summary>
        /// <remarks>
        /// Set <paramref name="permitCount"/> to 0 to get whether permits are exhausted.
        /// </remarks>
        /// <param name="permitCount">Number of permits to try and acquire.</param>
        /// <returns>A successful or failed lease.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public RateLimitLease AttemptAcquire(int permitCount = 1)
        {
            if (permitCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(permitCount));
            }

            return AttemptAcquireCore(permitCount);
        }

        /// <summary>
        /// Method that <see cref="RateLimiter"/> implementations implement for <see cref="AttemptAcquire"/>.
        /// </summary>
        /// <param name="permitCount">Number of permits to try and acquire.</param>
        /// <returns></returns>
        protected abstract RateLimitLease AttemptAcquireCore(int permitCount);

        /// <summary>
        /// Wait until the requested permits are available or permits can no longer be acquired.
        /// </summary>
        /// <remarks>
        /// Set <paramref name="permitCount"/> to 0 to wait until permits are replenished.
        /// </remarks>
        /// <param name="permitCount">Number of permits to try and acquire.</param>
        /// <param name="cancellationToken">Optional token to allow canceling a queued request for permits.</param>
        /// <returns>A task that completes when the requested permits are acquired or when the requested permits are denied.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ValueTask<RateLimitLease> AcquireAsync(int permitCount = 1, CancellationToken cancellationToken = default)
        {
            if (permitCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(permitCount));
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<RateLimitLease>(Task.FromCanceled<RateLimitLease>(cancellationToken));
            }

            return AcquireAsyncCore(permitCount, cancellationToken);
        }

        /// <summary>
        /// Method that <see cref="RateLimiter"/> implementations implement for <see cref="AcquireAsync"/>.
        /// </summary>
        /// <param name="permitCount">Number of permits to try and acquire.</param>
        /// <param name="cancellationToken">Optional token to allow canceling a queued request for permits.</param>
        /// <returns>A task that completes when the requested permits are acquired or when the requested permits are denied.</returns>
        protected abstract ValueTask<RateLimitLease> AcquireAsyncCore(int permitCount, CancellationToken cancellationToken);

        /// <summary>
        /// Dispose method for implementations to write.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        /// Disposes the RateLimiter. This completes any queued acquires with a failed lease.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// DisposeAsync method for implementations to write.
        /// </summary>
        protected virtual ValueTask DisposeAsyncCore()
        {
            return default;
        }

        /// <summary>
        /// Disposes the RateLimiter asynchronously.
        /// </summary>
        /// <returns>ValueTask representing the completion of the disposal.</returns>
        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await DisposeAsyncCore().ConfigureAwait(false);

            // Dispose of unmanaged resources.
            Dispose(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
