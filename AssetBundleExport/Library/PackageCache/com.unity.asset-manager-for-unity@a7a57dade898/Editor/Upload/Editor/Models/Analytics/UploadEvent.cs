using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_2023_2_OR_NEWER
using UnityEngine.Analytics;
#endif

namespace Unity.AssetManager.Upload.Editor
{
    internal class UploadEvent : IBaseEvent
    {
        [Serializable]
        internal struct CustomMetadataInfo
        {
            public int AssetsWithAddedFields;
            public int AssetsWithModifiedFields;
            public int AssetsWithRemovedFields;
            public int AssetsWithoutMetadataChanges;
        }

        [Serializable]
        internal struct UploadSettings
        {
            public string UploadMode;
            public string DependencyMode;
            public string FilePathMode;
            public bool UseCollection;
            public bool UseLatestDependencies;
        }

        [Serializable]
#if UNITY_2023_2_OR_NEWER
        internal class UploadEventData : IAnalytic.IData
#else
        internal class UploadEventData : BaseEventData
#endif
        {
            /// <summary>
            /// The number of file selected to be uploaded
            /// </summary>
            public int FileCount;

            /// <summary>
            /// File extensions
            /// </summary>
            public string[] FileExtensions;

            /// <summary>
            /// Upload mode.
            /// </summary>
            public string UploadMode;

            /// <summary>
            /// Dependency mode.
            /// </summary>
            public string DependencyMode;

            /// <summary>
            /// File Paths mode.
            /// </summary>
            public string FilePathMode;

            /// <summary>
            /// If the upload is done into a collection
            /// </summary>
            public bool UseCollection;

            /// <summary>
            /// If the option to use the latest dependencies label is set
            /// </summary>
            public bool UseLatestDependencies;

            /// <summary>
            /// Information about the custom metadata fields added or modified in the uploaded assets
            /// </summary>
            public CustomMetadataInfo CustomMetadataInfo;
        }

        internal const string k_EventName = AnalyticsSender.EventPrefix + "Upload";
        internal const int k_EventVersion = 3;

        public string EventName => k_EventName;
        public int EventVersion => k_EventVersion;

        internal readonly UploadEventData m_Data;

        internal UploadEvent(int fileCount, string[] fileExtensions, UploadSettings settings, CustomMetadataInfo customMetadataInfo)
        {
            m_Data = new UploadEventData
            {
                FileCount = fileCount,
                FileExtensions = fileExtensions,
                UploadMode = settings.UploadMode,
                DependencyMode = settings.DependencyMode,
                FilePathMode = settings.FilePathMode,
                UseCollection = settings.UseCollection,
                UseLatestDependencies = settings.UseLatestDependencies,
                CustomMetadataInfo = customMetadataInfo
            };
        }

        internal static UploadEvent CreateFromUploadData(IReadOnlyCollection<IUploadAsset> uploadEntries, UploadSettings settings)
        {
            var fileExtensions = GetFileExtensionAnalytics(uploadEntries);
            var customMetadataInfo = GetCustomMetadataAnalytics(uploadEntries);

            return new UploadEvent(uploadEntries.Count, fileExtensions, settings, customMetadataInfo);
        }

        internal static string[] GetFileExtensionAnalytics(IReadOnlyCollection<IUploadAsset> uploadEntries)
        {
            var fileExtensions = uploadEntries.SelectMany(e => e.Files)
                .Where(f => !MetafilesHelper.IsMetafile(f.SourcePath))
                .Select(f =>
                {
                    var extension = Path.GetExtension(f.SourcePath);
                    if (extension.Length > 1)
                    {
                        extension = extension[1..];
                    }

                    return extension;
                })
                .ToArray();
            return fileExtensions;
        }

        internal static CustomMetadataInfo GetCustomMetadataAnalytics(IReadOnlyCollection<IUploadAsset> uploadEntries)
        {
            const ComparisonResults combinedMetadataFlags = ComparisonResults.MetadataAdded | ComparisonResults.MetadataModified | ComparisonResults.MetadataRemoved;

            var customMetadataInfo = new CustomMetadataInfo();

            foreach (var entry in uploadEntries)
            {
                // An asset has no metadata changes if it is new (ComparisonResults.None) and has no metadata or no metadata changes are detected
                if ((entry.ComparisonResults != ComparisonResults.None && (entry.ComparisonResults & combinedMetadataFlags) == 0) ||
                    (entry.ComparisonResults == ComparisonResults.None && entry.Metadata?.Count == 0))
                {
                    customMetadataInfo.AssetsWithoutMetadataChanges++;
                }
                else
                {
                    if (entry.ComparisonResults.HasFlag(ComparisonResults.MetadataAdded) ||
                        (entry.ComparisonResults == ComparisonResults.None && entry.Metadata?.Count > 0)) // Covers the case of a new asset with added metadata
                        customMetadataInfo.AssetsWithAddedFields++;

                    if (entry.ComparisonResults.HasFlag(ComparisonResults.MetadataModified))
                        customMetadataInfo.AssetsWithModifiedFields++;

                    if (entry.ComparisonResults.HasFlag(ComparisonResults.MetadataRemoved))
                        customMetadataInfo.AssetsWithRemovedFields++;
                }

            }

            return customMetadataInfo;
        }

#if UNITY_2023_2_OR_NEWER
        [AnalyticInfo(eventName:k_EventName, vendorKey:AnalyticsSender.VendorKey, version:k_EventVersion, maxEventsPerHour:1000,maxNumberOfElements:1000)]
        class UploadEventAnalytic : IAnalytic
        {
            readonly UploadEventData m_Data;

            public UploadEventAnalytic(UploadEventData data)
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
            return new UploadEventAnalytic(m_Data);
        }
#else
        public BaseEventData EventData => m_Data;
#endif
    }
}
