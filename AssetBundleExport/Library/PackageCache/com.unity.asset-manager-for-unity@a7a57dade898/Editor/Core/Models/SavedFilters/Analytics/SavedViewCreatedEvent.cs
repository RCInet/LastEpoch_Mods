using System;
using UnityEngine.Analytics;

namespace Unity.AssetManager.Core.Editor
{
    class SavedViewCreatedEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class SavedViewCreatedEventData : IAnalytic.IData
#else
        internal class SavedViewCreatedEventData : BaseEventData
#endif
        { }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "SavedViewCreated";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        readonly SavedViewCreatedEventData m_Data;

        internal SavedViewCreatedEvent()
        {
            m_Data = new SavedViewCreatedEventData();
        }

        #if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class SavedViewCreatedEventAnalytic : IAnalytic
        {
            readonly SavedViewCreatedEventData m_Data;

            public SavedViewCreatedEventAnalytic(SavedViewCreatedEventData data)
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
            return new SavedViewCreatedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
