using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class OrganizationSelectedEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class OrganizationSelectedEventData : IAnalytic.IData
#else
        internal class OrganizationSelectedEventData : BaseEventData
#endif
        { }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "OrganizationSelected";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        readonly OrganizationSelectedEventData m_Data;

        internal OrganizationSelectedEvent()
        {
            m_Data = new OrganizationSelectedEventData();
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class OrganizationSelectedEventAnalytic : IAnalytic
        {
            readonly OrganizationSelectedEventData m_Data;

            public OrganizationSelectedEventAnalytic(OrganizationSelectedEventData data)
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
            return new OrganizationSelectedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
