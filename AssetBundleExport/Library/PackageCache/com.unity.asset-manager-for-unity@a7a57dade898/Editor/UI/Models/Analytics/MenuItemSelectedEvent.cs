using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.Analytics;

namespace Unity.AssetManager.UI.Editor
{
    class MenuItemSelectedEvent : IBaseEvent
    {
        public enum MenuItemType
        {
            Refresh,
            GoToDashboard,
            GotoSubscriptions,
            ProjectSettings,
            Preferences
        }

        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class MenuItemSelectedEventData : IAnalytic.IData
#else
        internal class MenuItemSelectedEventData : BaseEventData
#endif
        {
            public string MenuItemSelected;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "MenuItemSelected";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        MenuItemSelectedEventData m_Data;

        internal MenuItemSelectedEvent(MenuItemType menuItemType)
        {
            m_Data = new MenuItemSelectedEventData
            {
                MenuItemSelected = menuItemType.ToString()
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        class MenuItemSelectedEventAnalytic : IAnalytic
        {
            MenuItemSelectedEventData m_Data;

            public MenuItemSelectedEventAnalytic(MenuItemSelectedEventData data)
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
            return new MenuItemSelectedEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
