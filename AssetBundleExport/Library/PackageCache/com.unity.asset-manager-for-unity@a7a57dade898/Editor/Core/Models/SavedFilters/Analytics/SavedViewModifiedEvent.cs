using System;
using UnityEngine.Analytics;

namespace Unity.AssetManager.Core.Editor
{
    class SavedViewModifiedEvent : IBaseEvent
    {
        public enum ModificationType
        {
            Rename,
            Update,
            Delete
        }

        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class SavedViewModifiedEventData : IAnalytic.IData
#else
        internal class SavedViewModifiedEventData : BaseEventData
#endif
        {
            public string Action;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "SavedViewModified";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        readonly SavedViewModifiedEventData m_Data;

        internal SavedViewModifiedEvent(ModificationType modificationType)
        {
            m_Data = new SavedViewModifiedEventData
            {
                Action = modificationType.ToString()
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class SavedViewModifiedEventAnalytic : IAnalytic
        {
            readonly SavedViewModifiedEventData m_Data;

            public SavedViewModifiedEventAnalytic(SavedViewModifiedEventData data)
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
            return new SavedViewModifiedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
