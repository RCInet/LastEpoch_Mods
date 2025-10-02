using System;
using System.Collections.Generic;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class FilterSearchEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class FilterSearchEventData : IAnalytic.IData
#else
        internal class FilterSearchEventData : BaseEventData
#endif
        {
            public string FilterName;
            public string FilterValue;
            public string PageTitle;
        }

        const string k_UnknownPageTitle = "Unknown";

        internal const string k_EventName = AnalyticsSender.EventPrefix + "FilterSearch";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        FilterSearchEventData m_Data;

        internal FilterSearchEvent(string filterName, List<string> filterValues, string activePage = k_UnknownPageTitle)
        {
            m_Data = new FilterSearchEventData
            {
                FilterName = filterName,
                FilterValue = filterValues != null ? string.Join(",", filterValues) : null,
                PageTitle = activePage
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class FilterSearchEventAnalytic : IAnalytic
        {
            FilterSearchEventData m_Data;

            public FilterSearchEventAnalytic(FilterSearchEventData data)
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
            return new FilterSearchEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
