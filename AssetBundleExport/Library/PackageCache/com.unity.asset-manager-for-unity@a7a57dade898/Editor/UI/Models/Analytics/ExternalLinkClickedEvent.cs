using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class ExternalLinkClickedEvent : IBaseEvent
    {
        public enum ExternalLinkType
        {
            OpenDashboard,
            OpenAsset,
            UpgradeCloudStoragePlan,
        }

        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class ExternalLinkClickedEventData : IAnalytic.IData
#else
        internal class ExternalLinkClickedEventData : BaseEventData
#endif
        {
            public string ExternalLinkTypeLinkType;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "ExternalLinkClicked";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        ExternalLinkClickedEventData m_Data;

        internal ExternalLinkClickedEvent(ExternalLinkType externalLinkType)
        {
            m_Data = new ExternalLinkClickedEventData
            {
                ExternalLinkTypeLinkType = externalLinkType.ToString()
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class ExternalLinkClickedEventAnalytic : IAnalytic
        {
            ExternalLinkClickedEventData m_Data;

            public ExternalLinkClickedEventAnalytic(ExternalLinkClickedEventData data)
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
            return new ExternalLinkClickedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
