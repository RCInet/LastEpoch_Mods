using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class SearchAttemptEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class SearchAttemptEventData : IAnalytic.IData
#else
        internal class SearchAttemptEventData : BaseEventData
#endif
        {
            public int KeywordCount;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "SearchAttempt";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        SearchAttemptEventData m_Data;

        internal SearchAttemptEvent(int count)
        {
            m_Data = new SearchAttemptEventData
            {
                KeywordCount = count
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        class SearchAttemptEventAnalytic : IAnalytic
        {
            SearchAttemptEventData m_Data;

            public SearchAttemptEventAnalytic(SearchAttemptEventData data)
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
            return new SearchAttemptEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
