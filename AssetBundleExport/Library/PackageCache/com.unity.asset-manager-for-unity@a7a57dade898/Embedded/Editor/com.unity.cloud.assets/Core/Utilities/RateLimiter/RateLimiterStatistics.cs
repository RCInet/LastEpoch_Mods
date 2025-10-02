// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// From https://github.com/dotnet/runtime/ at commit 06062e79faab44195f56cd7e4079b22d2380aedd

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Snapshot of statistics for a <see cref="RateLimiter"/>.
    /// </summary>
    internal class RateLimiterStatistics
    {
        /// <summary>
        /// Initializes an instance of <see cref="RateLimiterStatistics"/>.
        /// </summary>
        public RateLimiterStatistics() { }

        /// <summary>
        /// Gets the number of permits currently available for the <see cref="RateLimiter"/>.
        /// </summary>
        public long CurrentAvailablePermits { get; set; }

        /// <summary>
        /// Gets the number of queued permits for the <see cref="RateLimiter"/>.
        /// </summary>
        public long CurrentQueuedCount { get; set; }

        /// <summary>
        /// Gets the total number of failed <see cref="RateLimitLease"/>s returned.
        /// </summary>
        public long TotalFailedLeases { get; set; }

        /// <summary>
        /// Gets the total number of successful <see cref="RateLimitLease"/>s returned.
        /// </summary>
        public long TotalSuccessfulLeases { get; set; }
    }
}
