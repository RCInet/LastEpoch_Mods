using System;
using UnityEngine.Analytics;

namespace Unity.AssetManager.Core.Editor
{
    class SavedViewAppliedEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class SavedViewAppliedEventData : IAnalytic.IData
#else
        internal class SavedViewAppliedEventData : BaseEventData
#endif
        {
            public bool IsFilterApplied;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "SavedViewApplied";
        internal const int k_EventVersion = 2;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        readonly SavedViewAppliedEventData m_Data;

        internal SavedViewAppliedEvent(bool isFilterApplied)
        {
            m_Data = new SavedViewAppliedEventData
            {
                IsFilterApplied = isFilterApplied
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class SavedViewAppliedEventAnalytic : IAnalytic
        {
            readonly SavedViewAppliedEventData m_Data;

            public SavedViewAppliedEventAnalytic(SavedViewAppliedEventData data)
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
            return new SavedViewAppliedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
