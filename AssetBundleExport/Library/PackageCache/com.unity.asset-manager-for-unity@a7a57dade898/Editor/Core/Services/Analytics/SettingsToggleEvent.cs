using System;
using UnityEngine.Analytics;

namespace Unity.AssetManager.Core.Editor
{
    abstract class SettingsToggleEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class SettingsToggleEventData : IAnalytic.IData
#else
        internal class SettingsToggleEventData : BaseEventData
#endif
        {
            public bool IsToggleOn;
        }

        public abstract string EventName { get; }
        public abstract int EventVersion { get; }

        protected readonly SettingsToggleEventData m_Data;

        internal SettingsToggleEvent(bool isToggleOn)
        {
            m_Data = new SettingsToggleEventData
            {
                IsToggleOn = isToggleOn
            };
        }

#if UNITY_2023_2_OR_NEWER
        public abstract IAnalytic GetAnalytic();
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
