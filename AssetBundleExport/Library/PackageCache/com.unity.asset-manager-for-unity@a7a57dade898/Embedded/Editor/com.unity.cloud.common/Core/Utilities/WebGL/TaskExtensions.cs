using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    static class TaskExtensions
    {
        public static bool MultithreadingEnabled { get; set; } = true;

        public static async Task<T> UnityConfigureAwait<T>(this Task<T> task, bool continueOnCapturedContext)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return await task;
#else
            return await task.ConfigureAwait(continueOnCapturedContext);
#endif
        }

        public static async Task UnityConfigureAwait(this Task task, bool continueOnCapturedContext)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            await task;
#else
            await task.ConfigureAwait(continueOnCapturedContext);
#endif
        }

        public static ConfiguredTaskAwaitable ConfigureAwaitFalse(this Task task)
        {
            return task.ConfigureAwait(!MultithreadingEnabled);
        }

        public static ConfiguredValueTaskAwaitable ConfigureAwaitFalse(this ValueTask task)
        {
            return task.ConfigureAwait(!MultithreadingEnabled);
        }

        public static ConfiguredValueTaskAwaitable<T> ConfigureAwaitFalse<T>(this ValueTask<T> task)
        {
            return task.ConfigureAwait(!MultithreadingEnabled);
        }
    }
}
