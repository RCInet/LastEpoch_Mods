using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class WindowDockedEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class WindowDockedEventData : IAnalytic.IData
#else
        internal class WindowDockedEventData : BaseEventData
#endif
        {
            public bool IsDocked;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "WindowDocked";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        WindowDockedEventData m_Data;

        internal WindowDockedEvent(bool isDocked)
        {
            m_Data = new WindowDockedEventData
            {
                IsDocked = isDocked
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        class WindowDockedEventAnalytic : IAnalytic
        {
            WindowDockedEventData m_Data;

            public WindowDockedEventAnalytic(WindowDockedEventData data)
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
            return new WindowDockedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
