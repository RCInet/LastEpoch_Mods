using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class FilterDropdownEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class FilterDropdownEventData : IAnalytic.IData
#else
        internal class FilterDropdownEventData : BaseEventData
#endif
        {
            public string PageTitle;
        }

        const string k_UnknownPageTitle = "Unknown";

        internal const string k_EventName = AnalyticsSender.EventPrefix + "FilterDropdown";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        FilterDropdownEventData m_Data;

        internal FilterDropdownEvent(string activePage = k_UnknownPageTitle)
        {
            m_Data = new FilterDropdownEventData
            {
                PageTitle = activePage
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class FilterDropdownEventAnalytic : IAnalytic
        {
            FilterDropdownEventData m_Data;

            public FilterDropdownEventAnalytic(FilterDropdownEventData data)
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
            return new FilterDropdownEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
