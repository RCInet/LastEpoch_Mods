using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class ManageCollectionEvent : IBaseEvent
    {
        public enum CollectionOperationType
        {
            Create,
            Rename,
            Delete
        }

        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class ManageCollectionEventData : IAnalytic.IData
#else
        internal class ManageCollectionEventData : BaseEventData
#endif
        {
            public string CollectionOperation;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "ManageCollection";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        readonly ManageCollectionEventData m_Data;

        internal ManageCollectionEvent(CollectionOperationType collectionOperation)
        {
            m_Data = new ManageCollectionEventData
            {
                CollectionOperation = collectionOperation.ToString()
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        class ManageCollectionEventAnalytic : IAnalytic
        {
            readonly ManageCollectionEventData m_Data;

            public ManageCollectionEventAnalytic(ManageCollectionEventData data)
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
            return new ManageCollectionEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
