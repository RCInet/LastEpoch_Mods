using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.CommonEmbedded
{
    internal delegate IEnumerable<TimeSpan> TimeSeries();

    static class TimeSeriesBuilder
    {
        #pragma warning disable S2245 // Security Hotspot: Make sure that using this pseudorandom number generator is safe here.
        // As the jitter is meant solely to add randomness to the retry policy, it shouldn't pose security issues
        static readonly Random s_RetryJitterRandomizer = new Random();
        #pragma warning restore S2245

        internal static TimeSeries Empty => () => Enumerable.Empty<TimeSpan>();

        internal static TimeSeries Default => () => ExponentialBackoffWithJitter(
            initialWaitTime: TimeSpan.FromSeconds(1), maxWaitTime: TimeSpan.MaxValue,
            maxTotalWaitTime: TimeSpan.FromMinutes(1), maxJitter: TimeSpan.FromSeconds(1))();

        internal static TimeSeries ExponentialBackoffWithJitter(
            TimeSpan initialWaitTime,
            TimeSpan maxWaitTime,
            TimeSpan maxTotalWaitTime,
            TimeSpan maxJitter)
        {
            return () =>
            {
                var total = TimeSpan.Zero;
                return initialWaitTime
                    .ExponentialBackoffs(maxWaitTime)
                    .Select(d => new[] { d, maxWaitTime }.Min())
                    .TakeWhile(d =>
                    {
                        var exceeded = total >= maxTotalWaitTime;
                        total += d;
                        return !exceeded;
                    })
                    .Select(d =>
                    {
                        // Added jitter cannot be more than the current delay
                        var max = new[] { d, maxJitter }.Min();
                        var jitter = d.AddJitter(max);
                        return jitter;
                    });
            };
        }

        static IEnumerable<TimeSpan> ExponentialBackoffs(this TimeSpan initialDelay, TimeSpan maxWaitTime)
        {
            for (var retry = 0; retry < int.MaxValue; retry++)
            {
                var total = maxWaitTime.TotalMilliseconds;
                var initial = initialDelay.TotalMilliseconds;

                var multiplier = Math.Pow(2, retry);
                var delay = Math.Min(total, multiplier * initial);

                yield return TimeSpan.FromMilliseconds(delay);
            }
        }

        static TimeSpan AddJitter(this TimeSpan delay, TimeSpan maxJitter)
        {
            var jitter = maxJitter.TotalMilliseconds * s_RetryJitterRandomizer.NextDouble();
            return delay + TimeSpan.FromMilliseconds(jitter);
        }
    }
}
