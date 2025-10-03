using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    abstract class BaseEventData { }

    interface IBaseEvent
    {
        string EventName { get; }
        int EventVersion { get; }

#if UNITY_2023_2_OR_NEWER
        IAnalytic GetAnalytic();
#else
        BaseEventData EventData { get; }
#endif
    }

    static class AnalyticsSender
    {
#if !AM4U_DEV && !UNITY_2023_2_OR_NEWER
        static readonly int k_MaxEventsPerHour = 1000;
        static readonly int k_MaxNumberOfElements = 1000;
#endif

        // Vendor key must start with unity.
        public const string VendorKey = "unity.asset-explorer";
        public const string EventPrefix = "assetManager";

        internal static AnalyticsResult SendEvent(IBaseEvent aEvent)
        {
#if AM4U_DEV
            return AnalyticsResult.AnalyticsDisabled;
#else
            if (PrivateCloudSettings.Load().ServicesEnabled)
            {
                return AnalyticsResult.AnalyticsDisabled;
            }

#if UNITY_2023_2_OR_NEWER
            var analytic = aEvent.GetAnalytic();
            return EditorAnalytics.SendAnalytic(analytic);
#else
            var register = EditorAnalytics.RegisterEventWithLimit(aEvent.EventName, k_MaxEventsPerHour,
                k_MaxNumberOfElements, VendorKey, aEvent.EventVersion);

            // This is one here for each event, so that if the call fails we don't try to send the event for nothing
            if (register != AnalyticsResult.Ok)
                return register;

            return EditorAnalytics.SendEventWithLimit(aEvent.EventName, aEvent.EventData, aEvent.EventVersion);
#endif
#endif
        }
    }
}
