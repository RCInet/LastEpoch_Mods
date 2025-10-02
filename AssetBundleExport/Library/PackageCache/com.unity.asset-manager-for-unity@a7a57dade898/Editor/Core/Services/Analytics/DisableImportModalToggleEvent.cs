using System;
using UnityEngine.Analytics;

namespace Unity.AssetManager.Core.Editor
{
    class DisableImportModalToggleEvent : SettingsToggleEvent
    {
        public DisableImportModalToggleEvent(bool isToggleOn)
            : base(isToggleOn) { }

        const string k_EventName = AnalyticsSender.EventPrefix + "DisableImportModalToggled";
        const int k_EventVersion = 1;

        public override string EventName => k_EventName;
        public override int EventVersion => k_EventVersion;

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class DisableImportModalToggleEventAnalytic : IAnalytic
        {
            readonly SettingsToggleEventData m_Data;

            public DisableImportModalToggleEventAnalytic(SettingsToggleEventData data)
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

        public override IAnalytic GetAnalytic()
        {
            return new DisableImportModalToggleEventAnalytic(m_Data);
        }
#endif
    }
}
