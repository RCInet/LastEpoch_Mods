using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class ServicesInitializationCompletedEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class ServicesInitializationCompletedEventData : IAnalytic.IData
#else
        internal class ServicesInitializationCompletedEventData : BaseEventData
#endif
        {
            public Vector2 WindowResolution;
            public long TimeToInitialize;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "ServicesInitializationCompleted";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        ServicesInitializationCompletedEventData m_Data;

        internal ServicesInitializationCompletedEvent(Vector2 windowResolution, long timeToInitialize = 0)
        {
            m_Data = new ServicesInitializationCompletedEventData
            {
                WindowResolution = windowResolution,
                TimeToInitialize = timeToInitialize
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class ServicesInitializationCompletedEventAnalytic : IAnalytic
        {
            ServicesInitializationCompletedEventData m_Data;

            public ServicesInitializationCompletedEventAnalytic(ServicesInitializationCompletedEventData data)
            {
                m_Data = data;
            }

            public bool TryGatherData(out IAnalytic.IData data, out Exception error)
            {
                error = null;
                data = m_Data;
                return data != null;
            }
        }

        public IAnalytic GetAnalytic()
        {
            return new ServicesInitializationCompletedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
