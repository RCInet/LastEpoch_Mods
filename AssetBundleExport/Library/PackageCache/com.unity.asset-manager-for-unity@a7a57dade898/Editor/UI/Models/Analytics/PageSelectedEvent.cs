using System;
using UnityEngine.Analytics;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    class PageSelectedEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class PageSelectedEventData : IAnalytic.IData
#else
        internal class PageSelectedEventData : BaseEventData
#endif
        {
            public string SelectedPage;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "PageSelected";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        PageSelectedEventData m_Data;

        internal PageSelectedEvent(string selectPage)
        {
            m_Data = new PageSelectedEventData
            {
                SelectedPage = selectPage
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        class PageSelectedEventAnalytic : IAnalytic
        {
            PageSelectedEventData m_Data;

            public PageSelectedEventAnalytic(PageSelectedEventData data)
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
            return new PageSelectedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
