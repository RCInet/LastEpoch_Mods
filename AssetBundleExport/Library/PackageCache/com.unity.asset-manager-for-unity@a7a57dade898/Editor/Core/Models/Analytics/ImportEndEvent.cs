using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity.AssetManager.Core.Editor
{
    enum ImportEndStatus
    {
        Ok = 0,
        GenericError = 1,
        HttpError = 2,
        GenerationError = 3,
        DownloadError = 4,
        Cancelled = 5
    }

    class ImportEndEvent : IBaseEvent
    {
        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class ImportEndEventData : IAnalytic.IData
#else
        internal class ImportEndEventData : BaseEventData
#endif
        {
            /// <summary>
            /// The trigger that started the import operation
            /// </summary>
            public string Trigger;

            /// <summary>
            /// The ID of the asset being imported
            /// </summary>
            public List<string> AssetIds;

            /// <summary>
            /// The amount of milliseconds the import operation took
            /// </summary>
            public long ElapsedTime;

            /// <summary>
            /// The error message if any
            /// </summary>
            public string ErrorMessage;

            /// <summary>
            /// Timestamp when the operation started
            /// </summary>
            public long StartTime;

            /// <summary>
            /// The ending status of the operation. See Enums/ImportEndStatus.cs
            /// </summary>
            public string Status;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "ImportEnd";
        internal const int k_EventVersion = 4;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        ImportEndEventData m_Data;

        internal ImportEndEvent(ImportTrigger trigger, ImportEndStatus status, List<string> assetIds, DateTime startTime, DateTime finishTime, string error = "")
        {
            Utilities.DevAssert(trigger != null);
            var elapsedTime = finishTime - startTime;
            m_Data = new ImportEndEventData
            {
                Trigger = trigger?.ToString(),
                AssetIds = assetIds,
                ElapsedTime = (long)elapsedTime.TotalMilliseconds, // we don't need fractional milliseconds... also, event is set to numerical, so it fails if we send fractional
                ErrorMessage = error,
                StartTime = Utilities.DatetimeToTimestamp(startTime),
                Status = status.ToString()
            };
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        internal class ImportEndEventAnalytic : IAnalytic
        {
            ImportEndEventData m_Data;

            public ImportEndEventAnalytic(ImportEndEventData data)
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
            return new ImportEndEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
