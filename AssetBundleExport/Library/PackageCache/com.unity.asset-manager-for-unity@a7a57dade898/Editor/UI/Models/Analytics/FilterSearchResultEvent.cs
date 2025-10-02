using System;
using System.Collections.Generic;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class FilterSearchResultEvent : IBaseEvent
    {
        [Serializable]
        internal class FilterData
        {
            public string FilterName;
            public string FilterValue;
        }

        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class FilterSearchResultEventData : IAnalytic.IData
#else
        internal class FilterSearchResultEventData : BaseEventData
#endif
        {
            public List<FilterData> SelectedFilters;
            public int ResultCount;
            public string PageTitle;
        }

        const string k_UnknownPageTitle = "Unknown";

        internal const string k_EventName = AnalyticsSender.EventPrefix + "SearchFilteredResult";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        FilterSearchResultEventData m_Data;

        internal FilterSearchResultEvent(List<FilterData> selectedFilter, int resultCount, string activePage = k_UnknownPageTitle)
        {
            m_Data = new FilterSearchResultEventData
            {
                SelectedFilters = selectedFilter,
                ResultCount = resultCount,
                PageTitle = activePage
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class FilterSearchResultEventAnalytic : IAnalytic
        {
            FilterSearchResultEventData m_Data;

            public FilterSearchResultEventAnalytic(FilterSearchResultEventData data)
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
            return new FilterSearchResultEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
