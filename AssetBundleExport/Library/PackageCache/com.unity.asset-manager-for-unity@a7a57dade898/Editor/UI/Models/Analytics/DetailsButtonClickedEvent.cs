using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class DetailsButtonClickedEvent : IBaseEvent
    {
        public enum ButtonType
        {
            Import,
            Reimport,
            Show,
            Remove,
            ImportAll,
            ReImportAll,
            RemoveAll,
            RemoveSelected,
            RemoveSelectedAll,
            StopTracking,
            StopTrackingAll,
            StopTrackingSelected,
            StopTrackingSelectedAll
        }

        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class DetailsButtonClickedEventData : IAnalytic.IData
#else
        internal class DetailsButtonClickedEventData : BaseEventData
#endif
        {
            public string ButtonType;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "DetailsButtonClicked";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        DetailsButtonClickedEventData m_Data;

        internal DetailsButtonClickedEvent(ButtonType buttonType)
        {
            m_Data = new DetailsButtonClickedEventData
            {
                ButtonType = buttonType.ToString()
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class DetailsButtonClickedEventAnalytic : IAnalytic
        {
            DetailsButtonClickedEventData m_Data;

            public DetailsButtonClickedEventAnalytic(DetailsButtonClickedEventData data)
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
            return new DetailsButtonClickedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
