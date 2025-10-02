using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class UpdateAllLatestButtonClickEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class UpdateAllLatestButtonClickEventData : IAnalytic.IData
#else
        internal class UpdateAllLatestButtonClickEventData : BaseEventData
#endif
        { }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "UpdateAllLatestButtonClick";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        readonly UpdateAllLatestButtonClickEventData m_Data;

        internal UpdateAllLatestButtonClickEvent()
        {
            m_Data = new UpdateAllLatestButtonClickEventData();
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class UpdateAllLatestButtonClickEventAnalytic : IAnalytic
        {
            readonly UpdateAllLatestButtonClickEventData m_Data;

            public UpdateAllLatestButtonClickEventAnalytic(UpdateAllLatestButtonClickEventData data)
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
            return new UpdateAllLatestButtonClickEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
