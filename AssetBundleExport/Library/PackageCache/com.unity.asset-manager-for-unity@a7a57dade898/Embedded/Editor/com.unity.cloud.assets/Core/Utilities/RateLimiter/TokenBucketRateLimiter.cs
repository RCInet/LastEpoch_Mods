// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// From https://github.com/dotnet/runtime/ at commit 06062e79faab44195f56cd7e4079b22d2380aedd
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Controls the behavior of <see cref="RateLimiter.AcquireAsync"/> when not enough resources can be leased.
    /// </summary>
    enum QueueProcessingOrder
    {
        /// <summary>
        /// Lease the oldest queued <see cref="RateLimiter.AcquireAsync"/> call.
        /// </summary>
        OldestFirst,

        /// <summary>
        /// Lease the newest queued <see cref="RateLimiter.AcquireAsync"/> call.
        /// </summary>
        NewestFirst
    }

    sealed class TokenBucketRateLimiterOptions
    {
        /// <summary>
        /// Specifies the minimum period between replenishments.
        /// Must be set to a value greater than <see cref="TimeSpan.Zero" /> by the time these options are passed to the constructor of <see cref="TokenBucketRateLimiter"/>.
        /// </summary>
        public TimeSpan ReplenishmentPeriod { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Specifies the maximum number of tokens to restore each replenishment.
        /// Must be set to a value > 0 by the time these options are passed to the constructor of <see cref="TokenBucketRateLimiter"/>.
        /// </summary>
        public int TokensPerPeriod { get; set; }

        /// <summary>
        /// Specified whether the <see cref="TokenBucketRateLimiter"/> is automatically replenishing tokens or if someone else
        /// will be calling <see cref="TokenBucketRateLimiter.TryReplenish"/> to replenish tokens.
        /// </summary>
        /// <value>
        /// <see langword="true" /> by default.
        /// </value>
        public bool AutoReplenishment { get; set; } = true;

        /// <summary>
        /// Maximum number of tokens that can be in the bucket at any time.
        /// Must be set to a value > 0 by the time these options are passed to the constructor of <see cref="TokenBucketRateLimiter"/>.
        /// </summary>
        public int TokenLimit { get; set; }

        /// <summary>
        /// Determines the behaviour of <see cref="RateLimiter.AcquireAsync"/> when not enough resources can be leased.
        /// </summary>
        /// <value>
        /// <see cref="QueueProcessingOrder.OldestFirst"/> by default.
        /// </value>
        public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;

        /// <summary>
        /// Maximum cumulative token count of queued acquisition requests.
        /// Must be set to a value >= 0 by the time these options are passed to the constructor of <see cref="TokenBucketRateLimiter"/>.
        /// </summary>
        public int QueueLimit { get; set; }
    }

    /// <summary>
    /// <see cref="RateLimiter"/> implementation that replenishes tokens periodically instead of via a release mechanism.
    /// </summary>
    sealed class TokenBucketRateLimiter : ReplenishingRateLimiter
    {
        private double _tokenCount;
        private int _queueCount;
        private long _lastReplenishmentTick;
        private long? _idleSince;
        private bool _disposed;

        private long _failedLeasesCount;
        private long _successfulLeasesCount;

        private readonly double _fillRate;
        private Timer? _renewTimer;
        private readonly TokenBucketRateLimiterOptions _options;
        private readonly Deque<RequestRegistration> _queue = new Deque<RequestRegistration>();

        // Use the queue as the lock field so we don't need to allocate another object for a lock and have another field in the object
        private object Lock => _queue;

        private static readonly RateLimitLease SuccessfulLease = new TokenBucketLease(true, null);
        private static readonly RateLimitLease FailedLease = new TokenBucketLease(false, null);
        private static readonly double TickFrequency = (double) TimeSpan.TicksPerSecond / Stopwatch.Frequency;

        /// <inheritdoc />
        public override TimeSpan? IdleDuration => _idleSince is null
            ? null
            : new TimeSpan((long) ((Stopwatch.GetTimestamp() - _idleSince) * TickFrequency));

        /// <inheritdoc />
        public override bool IsAutoReplenishing => _options.AutoReplenishment;

        /// <inheritdoc />
        public override TimeSpan ReplenishmentPeriod => _options.ReplenishmentPeriod;

        /// <summary>
        /// Initializes the <see cref="TokenBucketRateLimiter"/>.
        /// </summary>
        /// <param name="options">Options to specify the behavior of the <see cref="TokenBucketRateLimiter"/>.</param>
        public TokenBucketRateLimiter(TokenBucketRateLimiterOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.TokenLimit <= 0)
            {
                throw new ArgumentException("Negative Token Limit", nameof(options));
            }

            if (options.TokensPerPeriod <= 0)
            {
                throw new ArgumentException("Negative Refresh Value", nameof(options));
            }

            if (options.QueueLimit < 0)
            {
                throw new ArgumentException("Negative Queue Size", nameof(options));
            }

            if (options.ReplenishmentPeriod <= TimeSpan.Zero)
            {
                throw new ArgumentException(
                    $"NegativeRefresh Period \n{nameof(options.ReplenishmentPeriod)},\n {nameof(options)}");
            }

            _options = new TokenBucketRateLimiterOptions
            {
                TokenLimit = options.TokenLimit,
                QueueProcessingOrder = options.QueueProcessingOrder,
                QueueLimit = options.QueueLimit,
                ReplenishmentPeriod = options.ReplenishmentPeriod,
                TokensPerPeriod = options.TokensPerPeriod,
                AutoReplenishment = options.AutoReplenishment
            };

            _tokenCount = options.TokenLimit;
            _fillRate = (double) options.TokensPerPeriod / options.ReplenishmentPeriod.Ticks;

            _idleSince = _lastReplenishmentTick = Stopwatch.GetTimestamp();

        }

        /// <inheritdoc/>
        public override RateLimiterStatistics GetStatistics()
        {
            ThrowIfDisposed();
            return new RateLimiterStatistics
            {
                CurrentAvailablePermits = (long) _tokenCount,
                CurrentQueuedCount = _queueCount,
                TotalFailedLeases = Interlocked.Read(ref _failedLeasesCount),
                TotalSuccessfulLeases = Interlocked.Read(ref _successfulLeasesCount),
            };
        }

        /// <inheritdoc/>
        protected override RateLimitLease AttemptAcquireCore(int permitCount)
        {
            // These amounts of resources can never be acquired
            if (permitCount > _options.TokenLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(permitCount), permitCount,
                    $"{nameof(permitCount)} exceeds {nameof(_options.TokenLimit)} {_tokenCount} > {_options.TokenLimit}");
            }

            // Return SuccessfulLease or FailedLease depending to indicate limiter state
            if (permitCount == 0 && !_disposed)
            {
                if (_tokenCount > 0)
                {
                    Interlocked.Increment(ref _successfulLeasesCount);
                    return SuccessfulLease;
                }

                Interlocked.Increment(ref _failedLeasesCount);
                return CreateFailedTokenLease(permitCount);
            }

            lock (Lock)
            {
                if (TryLeaseUnsynchronized(permitCount, out RateLimitLease? lease))
                {
                    return lease;
                }

                Interlocked.Increment(ref _failedLeasesCount);
                return CreateFailedTokenLease(permitCount);
            }
        }

        /// <inheritdoc/>
        protected override ValueTask<RateLimitLease> AcquireAsyncCore(int permitCount,
            CancellationToken cancellationToken)
        {
            // These amounts of resources can never be acquired
            if (permitCount > _options.TokenLimit)
            {
                throw new ArgumentOutOfRangeException();
            }

            ThrowIfDisposed();

            // Return SuccessfulAcquisition if requestedCount is 0 and resources are available
            if (permitCount == 0 && _tokenCount > 0)
            {
                Interlocked.Increment(ref _successfulLeasesCount);
                return new ValueTask<RateLimitLease>(SuccessfulLease);
            }

            using var disposer = default(RequestRegistration.Disposer);
            lock (Lock)
            {
                if (_options.AutoReplenishment && _renewTimer is null)
                {
                    _renewTimer = new Timer(Replenish, this, _options.ReplenishmentPeriod, Timeout.InfiniteTimeSpan);
                }

                if (TryLeaseUnsynchronized(permitCount, out RateLimitLease? lease))
                {
                    return new ValueTask<RateLimitLease>(lease);
                }

                // Avoid integer overflow by using subtraction instead of addition
                Debug.Assert(_options.QueueLimit >= _queueCount);
                if (_options.QueueLimit - _queueCount < permitCount)
                {
                    if (_options.QueueProcessingOrder == QueueProcessingOrder.NewestFirst &&
                        permitCount <= _options.QueueLimit)
                    {
                        // remove oldest items from queue until there is space for the newest acquisition request
                        do
                        {
                            RequestRegistration oldestRequest = _queue.DequeueHead();
                            _queueCount -= oldestRequest.Count;
                            Debug.Assert(_queueCount >= 0);
                            if (!oldestRequest.TrySetResult(FailedLease))
                            {
                                // Updating queue count is handled by the cancellation code
                                _queueCount += oldestRequest.Count;
                            }
                            else
                            {
                                Interlocked.Increment(ref _failedLeasesCount);
                            }

                            disposer.Add(oldestRequest);
                        } while (_options.QueueLimit - _queueCount < permitCount);
                    }
                    else
                    {
                        Interlocked.Increment(ref _failedLeasesCount);

                        // Don't queue if queue limit reached and QueueProcessingOrder is OldestFirst
                        return new ValueTask<RateLimitLease>(CreateFailedTokenLease(permitCount));
                    }
                }

                var registration = new RequestRegistration(permitCount, this, cancellationToken);
                _queue.EnqueueTail(registration);
                _queueCount += permitCount;
                Debug.Assert(_queueCount <= _options.QueueLimit);

                return new ValueTask<RateLimitLease>(registration.Task);
            }
        }

        private TokenBucketLease CreateFailedTokenLease(int tokenCount)
        {
            int replenishAmount = tokenCount - (int) _tokenCount + _queueCount;

            // can't have 0 replenish periods, that would mean it should be a successful lease
            // if TokensPerPeriod is larger than the replenishAmount needed then it would be 0
            Debug.Assert(_options.TokensPerPeriod > 0);
            int replenishPeriods = Math.Max(replenishAmount / _options.TokensPerPeriod, 1);

            return new TokenBucketLease(false,
                TimeSpan.FromTicks(_options.ReplenishmentPeriod.Ticks * replenishPeriods));
        }

        private bool TryLeaseUnsynchronized(int tokenCount, [NotNullWhen(true)] out RateLimitLease? lease)
        {
            ThrowIfDisposed();

            // if permitCount is 0 we want to queue it if there are no available permits
            if (_tokenCount >= tokenCount && _tokenCount != 0)
            {
                if (tokenCount == 0)
                {
                    Interlocked.Increment(ref _successfulLeasesCount);

                    // Edge case where the check before the lock showed 0 available permits but when we got the lock some permits were now available
                    lease = SuccessfulLease;
                    return true;
                }

                // a. if there are no items queued we can lease
                // b. if there are items queued but the processing order is newest first, then we can lease the incoming request since it is the newest
                if (_queueCount == 0 ||
                    (_queueCount > 0 && _options.QueueProcessingOrder == QueueProcessingOrder.NewestFirst))
                {
                    _idleSince = null;
                    _tokenCount -= tokenCount;
                    Debug.Assert(_tokenCount >= 0);
                    Interlocked.Increment(ref _successfulLeasesCount);
                    lease = SuccessfulLease;
                    return true;
                }
            }

            lease = null;
            return false;
        }

        /// <summary>
        /// Attempts to replenish the bucket.
        /// </summary>
        /// <returns>
        /// <see langword="false"/> if <see cref="TokenBucketRateLimiterOptions.AutoReplenishment"/> is enabled, otherwise <see langword="true"/>.
        /// Does not reflect if tokens were replenished.
        /// </returns>
        public override bool TryReplenish()
        {
            if (_options.AutoReplenishment)
            {
                return false;
            }

            Replenish(this);
            return true;
        }

        private void Replenish(object? state)
        {
            _renewTimer?.Dispose();
            _renewTimer = null;

            if (_disposed || (int) _tokenCount == _options.TokenLimit)
            {
                return;
            }

            TokenBucketRateLimiter limiter = (state as TokenBucketRateLimiter)!;
            Debug.Assert(limiter is not null);

            // Use Stopwatch instead of DateTime.UtcNow to avoid issues on systems where the clock can change
            long nowTicks = Stopwatch.GetTimestamp();

            limiter.ReplenishInternal(nowTicks);
        }

        // Used in tests to avoid dealing with real time
        private void ReplenishInternal(long nowTicks)
        {
            using var disposer = default(RequestRegistration.Disposer);

            // method is re-entrant (from Timer), lock to avoid multiple simultaneous replenishes
            lock (Lock)
            {
                // Trust the timer to be close enough to when we want to replenish, this avoids issues with Timer jitter where it might be .99 seconds instead of 1, and 1.1 seconds the next time etc.
                double add = _options.TokensPerPeriod;
                if (!_options.AutoReplenishment)
                {
                    add = _fillRate * (nowTicks - _lastReplenishmentTick) * TickFrequency;
                }

                _tokenCount = Math.Min(_options.TokenLimit, _tokenCount + add);

                _lastReplenishmentTick = nowTicks;

                // Process queued requests
                Debug.Assert(_tokenCount <= _options.TokenLimit);
                while (_queue.Count > 0)
                {
                    RequestRegistration nextPendingRequest = PeekRequest();

                    // Request was handled already, either via cancellation or being kicked from the queue due to a newer request being queued.
                    // We just need to remove the item and let the next queued item be considered for completion.
                    if (nextPendingRequest.Task.IsCompleted)
                    {
                        DequeueRequest(disposer);
                    }
                    else if (_tokenCount >= nextPendingRequest.Count)
                    {
                        // Request can be fulfilled
                        nextPendingRequest = DequeueRequest(disposer);

                        _queueCount -= nextPendingRequest.Count;
                        _tokenCount -= nextPendingRequest.Count;
                        Debug.Assert(_tokenCount >= 0);

                        if (!nextPendingRequest.TrySetResult(SuccessfulLease))
                        {
                            // Queued item was canceled so add count back
                            _tokenCount += nextPendingRequest.Count;

                            // Updating queue count is handled by the cancellation code
                            _queueCount += nextPendingRequest.Count;
                        }
                        else
                        {
                            Interlocked.Increment(ref _successfulLeasesCount);
                        }

                        Debug.Assert(_queueCount >= 0);
                    }
                    else
                    {
                        // Request cannot be fulfilled; if auto-replenish, wait until next renewal
                        if (_options.AutoReplenishment && _renewTimer is null)
                        {
                            _renewTimer = new Timer(Replenish, this, _options.ReplenishmentPeriod, Timeout.InfiniteTimeSpan);
                        }

                        break;
                    }
                }

                if ((int) _tokenCount == _options.TokenLimit)
                {
                    Debug.Assert(_idleSince is null);
                    _idleSince = Stopwatch.GetTimestamp();
                }
            }
        }

        RequestRegistration PeekRequest()
        {
            return _options.QueueProcessingOrder == QueueProcessingOrder.OldestFirst
                ? _queue.PeekHead()
                : _queue.PeekTail();
        }

        RequestRegistration DequeueRequest(RequestRegistration.Disposer disposer)
        {
            var request = _options.QueueProcessingOrder == QueueProcessingOrder.OldestFirst
                ? _queue.DequeueHead()
                : _queue.DequeueTail();
            disposer.Add(request);
            return request;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            using var disposer = default(RequestRegistration.Disposer);
            lock (Lock)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _renewTimer?.Dispose();
                while (_queue.Count > 0)
                {
                    RequestRegistration next = DequeueRequest(disposer);
                    next.TrySetResult(FailedLease);
                }
            }
        }

        /// <inheritdoc />
        protected override ValueTask DisposeAsyncCore()
        {
            Dispose(true);

            return default;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(TokenBucketRateLimiter));
            }
        }

        private sealed class TokenBucketLease : RateLimitLease
        {
            private static readonly string[] s_allMetadataNames = new[] {MetadataName.RetryAfter.Name};

            private readonly TimeSpan? _retryAfter;

            public TokenBucketLease(bool isAcquired, TimeSpan? retryAfter)
            {
                IsAcquired = isAcquired;
                _retryAfter = retryAfter;
            }

            public override bool IsAcquired { get; }

            public override IEnumerable<string> MetadataNames => s_allMetadataNames;

            public override bool TryGetMetadata(string metadataName, out object? metadata)
            {
                if (metadataName == MetadataName.RetryAfter.Name && _retryAfter.HasValue)
                {
                    metadata = _retryAfter.Value;
                    return true;
                }

                metadata = default;
                return false;
            }
        }

        private sealed class RequestRegistration : TaskCompletionSource<RateLimitLease>
        {
            private readonly CancellationToken _cancellationToken;
            private CancellationTokenRegistration _cancellationTokenRegistration;

            // this field is used only by the disposal mechanics and never shared between threads
            private RequestRegistration? _next;

            public RequestRegistration(int permitCount, TokenBucketRateLimiter limiter,
                CancellationToken cancellationToken)
                : base(limiter, TaskCreationOptions.RunContinuationsAsynchronously)
            {
                Count = permitCount;
                _cancellationToken = cancellationToken;

                // RequestRegistration objects are created while the limiter lock is held
                // if cancellationToken fires before or while the lock is held, UnsafeRegister
                // is going to invoke the callback synchronously, but this does not create
                // a deadlock because lock are reentrant
                if (cancellationToken.CanBeCanceled)
                    _cancellationTokenRegistration = cancellationToken.Register(Cancel, this);
            }

            public int Count { get; }

            private static void Cancel(object? state)
            {
                if (state is RequestRegistration registration &&
                    registration.TrySetCanceled(registration._cancellationToken))
                {
                    var limiter = (TokenBucketRateLimiter) registration.Task.AsyncState!;
                    lock (limiter.Lock)
                    {
                        limiter._queueCount -= registration.Count;
                    }
                }
            }

            /// <summary>
            /// Collects registrations to dispose outside the limiter lock to avoid deadlock.
            /// </summary>
            internal struct Disposer : IDisposable
            {
                private RequestRegistration? _next;

                public void Add(RequestRegistration request)
                {
                    request._next = _next;
                    _next = request;
                }

                public void Dispose()
                {
                    for (var current = _next; current is not null; current = current._next)
                    {
                        current._cancellationTokenRegistration.Dispose();
                    }

                    _next = null;
                }
            }
        }
    }
}
