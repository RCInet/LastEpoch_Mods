using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Unity.Cloud.AppLinkingEmbedded.Runtime
{
    static class UnitySynchronizationContextGrabber
    {
        static SynchronizationContext s_UnitySynchronizationContextValue;

        /// <summary>
        /// Return the UnitySynchronizationContext.
        /// Important: do not cache this value unless you ensure you do this after it has been set.
        /// </summary>
        public static SynchronizationContext s_UnitySynchronizationContext
        {
            get => s_UnitySynchronizationContextValue;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RuntimeGrab()
        {
            s_UnitySynchronizationContextValue = SynchronizationContext.Current;
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
