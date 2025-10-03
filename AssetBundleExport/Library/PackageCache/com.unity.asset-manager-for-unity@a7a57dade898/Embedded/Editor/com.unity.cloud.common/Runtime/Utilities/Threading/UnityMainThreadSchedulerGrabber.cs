using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    static class UnityMainThreadSchedulerGrabber
    {
        static TaskScheduler s_UnityMainThreadSchedulerValue;

        /// <summary>
        /// Return the Unity main thread TaskScheduler.
        /// Important: do not cache this value unless you ensure you do this after it has been set.
        /// </summary>
        internal static TaskScheduler s_UnityMainThreadScheduler
        {
            get => s_UnityMainThreadSchedulerValue;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RuntimeGrab()
        {
            TaskExtensions.MultithreadingEnabled = Application.platform != RuntimePlatform.WebGLPlayer;
            s_UnityMainThreadSchedulerValue = TaskScheduler.FromCurrentSynchronizationContext();
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void EditorGrab()
        {
            RuntimeGrab();
        }
#endif
    }
}
