using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class SortEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class SortEventData : IAnalytic.IData
#else
        internal class SortEventData : BaseEventData
#endif
        {
            public string SortValue;
            public bool IsAscending;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "Sort";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        readonly SortEventData m_Data;

        internal SortEvent(string value, bool isAscending)
        {
            m_Data = new SortEventData
            {
                SortValue = value,
                IsAscending = isAscending
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        class SortEventAnalytic : IAnalytic
        {
            readonly SortEventData m_Data;

            public SortEventAnalytic(SortEventData data)
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
            return new SortEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
