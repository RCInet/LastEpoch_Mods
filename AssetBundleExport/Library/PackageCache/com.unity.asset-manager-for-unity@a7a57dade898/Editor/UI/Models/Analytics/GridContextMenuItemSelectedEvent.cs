using System;
using UnityEngine.Analytics;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    class GridContextMenuItemSelectedEvent : IBaseEvent
    {
        public enum ContextMenuItemType
        {
            Import,
            Reimport,
            CancelImport,
            Remove,
            ShowInProject,
            ShowInDashboard,
            IgnoreUploadedAsset,
            ImportAll,
            RemoveAll,
            UpdateAllToLatest,
        }

        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class GridContextMenuItemSelectedEventData : IAnalytic.IData
#else
        internal class GridContextMenuItemSelectedEventData : BaseEventData
#endif
        {
            public string ContextMenuItemSelected;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "GridContextMenuItemSelected";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        GridContextMenuItemSelectedEventData m_Data;

        internal GridContextMenuItemSelectedEvent(ContextMenuItemType itemType)
        {
            m_Data = new GridContextMenuItemSelectedEventData
            {
                ContextMenuItemSelected = itemType.ToString()
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class GridContextMenuItemSelectedEventAnalytic : IAnalytic
        {
            GridContextMenuItemSelectedEventData m_Data;

            public GridContextMenuItemSelectedEventAnalytic(GridContextMenuItemSelectedEventData data)
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
            return new GridContextMenuItemSelectedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
