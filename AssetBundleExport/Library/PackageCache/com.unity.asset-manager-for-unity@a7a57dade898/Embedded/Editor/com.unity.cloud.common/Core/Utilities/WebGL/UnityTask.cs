using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    internal static class UnityTask
    {
        internal static Task Run(Func<Task> function, CancellationToken cancellationToken = default)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            ThrowIfNull(function, nameof(function));
            ThrowIfCancelled(cancellationToken);
            return AwaitTask(function);
#else
            return Task.Run(function, cancellationToken);
#endif
        }

        internal static Task Run(Action action, CancellationToken cancellationToken = default)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            ThrowIfNull(action, nameof(action));
            ThrowIfCancelled(cancellationToken);
            return AwaitTask(() =>
            {
                action();
                return Task.CompletedTask;
            });
#else
            return Task.Run(action, cancellationToken);
#endif
        }

        internal static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken = default)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            ThrowIfNull(function, nameof(function));
            ThrowIfCancelled(cancellationToken);
            return AwaitTask(function);
#else
            return Task.Run(function, cancellationToken);
#endif
        }

        internal static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken = default)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            ThrowIfNull(function, nameof(function));
            ThrowIfCancelled(cancellationToken);
            return AwaitTask(() => Task.FromResult(function()));
#else
            return Task.Run(function, cancellationToken);
#endif
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        static readonly TimeSpan k_InfiniteDelay = TimeSpan.FromMilliseconds(-1);
        const string k_IntExceptionMessage = "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer.";
#endif

        internal static Task Delay(int millisecondsDelay, CancellationToken cancellationToken = default)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException(k_IntExceptionMessage, nameof(millisecondsDelay));
            }
            return SafeDelay(TimeSpan.FromMilliseconds(millisecondsDelay), cancellationToken);
#else
            return Task.Delay(millisecondsDelay, cancellationToken);
#endif

        }

        internal static Task Delay(TimeSpan delay, CancellationToken cancellationToken = default)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if ((delay.TotalMilliseconds < 0 && delay != k_InfiniteDelay) || delay.TotalMilliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(delay), k_IntExceptionMessage);
            }
            return SafeDelay(delay, cancellationToken);
#else
            return Task.Delay(delay, cancellationToken);
#endif
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        static async Task SafeDelay(TimeSpan delay, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            Func<bool> predicate = delay == k_InfiniteDelay ? () => true : () => stopwatch.Elapsed < delay;
            while (predicate())
            {
                ThrowIfCancelled(cancellationToken);
                await Task.Yield();
            }
        }

        static void ThrowIfNull(object argument, string paramName)
        {
            if (argument == null)
                throw new ArgumentNullException(paramName);
        }

        static void ThrowIfCancelled(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();
        }

        async static Task<TResult> AwaitTask<TResult>(Func<Task<TResult>> function)
        {
            try
            {
                return await function();
            }
            catch (Exception e)
            {
                throw new AggregateException(e);
            }
        }

        async static Task AwaitTask(Func<Task> function)
        {
            try
            {
                await function();
            }
            catch (Exception e)
            {
                throw new AggregateException(e);
            }
        }
#endif
    }
}
