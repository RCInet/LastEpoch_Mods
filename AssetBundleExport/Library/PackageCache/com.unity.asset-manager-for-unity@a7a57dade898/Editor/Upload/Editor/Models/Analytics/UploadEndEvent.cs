using System;
using Unity.AssetManager.Core.Editor;

#if UNITY_2023_2_OR_NEWER
using UnityEngine.Analytics;
#endif

namespace Unity.AssetManager.Upload.Editor
{
    enum UploadEndStatus
    {
        Ok = 0,
        UploadError = 1,
        PreparationError = 3,
        Cancelled = 4
    }

    class UploadEndEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class UploadEndEventData : IAnalytic.IData
#else
        internal class UploadEndEventData : BaseEventData
#endif
        {
            /// <summary>
            /// The error message if any
            /// </summary>
            public string ErrorMessage;

            /// <summary>
            /// The ending status of the operation. See Enums/UploadEndStatus.cs
            /// </summary>
            public string Status;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "UploadEnd";
        internal const int k_EventVersion = 1;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        UploadEndEventData m_Data;

        internal UploadEndEvent(UploadEndStatus status, string error = "")
        {
            m_Data = new UploadEndEventData
            {
                Status = status.ToString(),
                ErrorMessage = error
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class UploadEndEventAnalytic : IAnalytic
        {
            UploadEndEventData m_Data;

            public UploadEndEventAnalytic(UploadEndEventData data)
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
            return new UploadEndEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
