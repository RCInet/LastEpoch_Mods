using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IMetadataPersistenceV2
    {
        public string FieldKey { get; }
        public string Name { get; }
        public MetadataFieldType Type { get; }
        public object GetValue();

        public IMetadataPersistenceV2 Clone();
    }

    abstract class MetadataBasePersistenceV2<T> : IMetadataPersistenceV2
    {
        [SerializeField]
        string m_FieldKey;

        [SerializeField]
        string m_Name;

        [SerializeField]
        MetadataFieldType m_Type;

        [SerializeField]
        T m_Value;

        public string FieldKey => m_FieldKey;
        public string Name => m_Name;
        public MetadataFieldType Type => m_Type;

        public T Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        protected MetadataBasePersistenceV2(MetadataFieldType type, string fieldKey, string name, T value)
        {
            m_FieldKey = fieldKey;
            m_Type = type;
            m_Name = name;
            m_Value = value;
        }

        public IMetadataPersistenceV2 Clone()
        {
            return Activator.CreateInstance(GetType(), m_FieldKey, m_Name, m_Value) as IMetadataPersistenceV2;
        }

        public object GetValue() => m_Value;
    }

    [Serializable]
    class TextMetadataPersistenceV2 : MetadataBasePersistenceV2<string>
    {
        public TextMetadataPersistenceV2(string fieldKey, string name, string value)
            : base(MetadataFieldType.Text, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    class BooleanMetadataPersistenceV2 : MetadataBasePersistenceV2<bool>
    {
        public BooleanMetadataPersistenceV2(string fieldKey, string name, bool value)
            : base(MetadataFieldType.Boolean, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    class NumberMetadataPersistenceV2 : MetadataBasePersistenceV2<double>
    {
        public NumberMetadataPersistenceV2(string fieldKey, string name, double value)
            : base(MetadataFieldType.Number, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    struct UriEntryPersistenceV2 : ISerializationCallbackReceiver
    {
        [SerializeField]
        string m_SerializedUri;

        [SerializeField]
        string m_Label;

        public Uri Uri { get; set; }

        public string Label
        {
            get => m_Label;
            set => m_Label = value;
        }

        public UriEntryPersistenceV2(Uri uri, string label)
        {
            Uri = uri;
            m_Label = label;
            m_SerializedUri = null;
        }

        public void OnBeforeSerialize()
        {
            m_SerializedUri = Uri?.ToString();
        }

        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(m_SerializedUri))
                return;

            Uri = new Uri(m_SerializedUri);
        }
    }

    [Serializable]
    class UrlMetadataPersistenceV2 : MetadataBasePersistenceV2<UriEntryPersistenceV2>
    {
        public UrlMetadataPersistenceV2(string fieldKey, string name, UriEntryPersistenceV2 value)
            : base(MetadataFieldType.Url, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    struct DateTimeEntryPersistenceV2 : ISerializationCallbackReceiver
    {
        [SerializeField]
        long m_SerializedDataTime;

        [SerializeField]
        DateTimeKind m_SerializedDataTimeKind;

        public DateTime DateTime { get; set; }

        public DateTimeEntryPersistenceV2(DateTime dateTime)
        {
            DateTime = dateTime;
            m_SerializedDataTime = 0;
            m_SerializedDataTimeKind = DateTimeKind.Unspecified;
        }

        public void OnBeforeSerialize()
        {
            m_SerializedDataTime = DateTime.Ticks;
            m_SerializedDataTimeKind = DateTime.Kind;
        }

        public void OnAfterDeserialize()
        {
            DateTime = new DateTime(m_SerializedDataTime, m_SerializedDataTimeKind);
        }
    }

    [Serializable]
    class TimestampMetadataPersistenceV2 : MetadataBasePersistenceV2<DateTimeEntryPersistenceV2>
    {
        public TimestampMetadataPersistenceV2(string fieldKey, string name, DateTimeEntryPersistenceV2 value)
            : base(MetadataFieldType.Timestamp, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    class UserMetadataPersistenceV2 : MetadataBasePersistenceV2<string>
    {
        public UserMetadataPersistenceV2(string fieldKey, string name, string value)
            : base(MetadataFieldType.User, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    class SingleSelectionMetadataPersistenceV2 : MetadataBasePersistenceV2<string>
    {
        public SingleSelectionMetadataPersistenceV2(string fieldKey, string name, string value)
            : base(MetadataFieldType.SingleSelection, fieldKey, name, value)
        {
        }
    }

    [Serializable]
    class MultiSelectionMetadataPersistenceV2 : MetadataBasePersistenceV2<List<string>>
    {
        public MultiSelectionMetadataPersistenceV2(string fieldKey, string name, List<string> value)
            : base(MetadataFieldType.MultiSelection, fieldKey, name, value)
        {
        }
    }

    class PersistenceV2 : IPersistenceVersion
    {
        public int MajorVersion => 2;
        public int MinorVersion => 0;

        [Serializable]
        class TrackedAssetVersionPersisted
        {
            [SerializeField]
            public string versionId;

            [SerializeField]
            public string name;

            [SerializeField]
            public int sequenceNumber;

            [SerializeField]
            public int parentSequenceNumber;

            [SerializeField]
            public string changelog;

            [SerializeField]
            public AssetType assetType;

            [SerializeField]
            public string status;

            [SerializeField]
            public string description;

            [SerializeField]
            public string created;

            [SerializeField]
            public string updated;

            [SerializeField]
            public string createdBy;

            [SerializeField]
            public string updatedBy;

            [SerializeField]
            public string previewFilePath;

            [SerializeField]
            public bool isFrozen;

            [SerializeField]
            public List<string> tags;
        }

        [Serializable]
        class TrackedAssetIdentifierPersisted
        {
            [SerializeField]
            public string organizationId;

            [SerializeField]
            public string projectId;

            [SerializeField]
            public string assetId;

            [SerializeField]
            public string versionId;

            [SerializeField]
            public string versionLabel;
        }

        [Serializable]
        class TrackedAssetPersisted : TrackedAssetVersionPersisted
        {
            [SerializeField]
            public int[] serializationVersion;

            [SerializeField]
            public string organizationId;

            [SerializeField]
            public string projectId;

            [SerializeField]
            public string assetId;

            [SerializeField]
            public List<TrackedAssetIdentifierPersisted> dependencyAssets;

            [SerializeField]
            public List<TrackedFilePersisted> files;

            [SerializeField]
            public List<TrackedDatasetPersisted> datasets;

            [SerializeReference]
            public List<IMetadataPersistenceV2> metadata;
        }

        [Serializable]
        class TrackedFilePersisted
        {
            [SerializeField]
            public string datasetId;

            [SerializeField]
            public string path; // key

            [SerializeField]
            public string trackedUnityGuid; // only set if this is a tracked asset

            [SerializeField]
            public string extension;

            [SerializeField]
            public bool available;

            [SerializeField]
            public string description;

            [SerializeField]
            public long fileSize;

            [SerializeField]
            public List<string> tags = new();

            [SerializeField]
            public string checksum;

            [SerializeField]
            public long timestamp;

            [SerializeField]
            public string metaFileChecksum;

            [SerializeField]
            public long metaFileTimestamp;
        }

        [Serializable]
        class TrackedDatasetPersisted
        {
            [SerializeField]
            public string id;

            [SerializeField]
            public string name;

            [SerializeField]
            public List<string> systemTags;

            [SerializeField]
            public List<string> fileKeys;
        }

        public ImportedAssetInfo ConvertToImportedAssetInfo(string content)
        {
            content = MapTypes(content);
            var trackedAsset = JsonUtility.FromJson<TrackedAssetPersisted>(content);
            var cache = new Persistence.ReadCache();

            return Convert(trackedAsset, cache);
        }

        static readonly List<(string original, string replacement)> s_ClassMappings = new()
        {
            ("TextMetadata", "TextMetadataPersistenceV2"),
            ("BooleanMetadata", "BooleanMetadataPersistenceV2"),
            ("NumberMetadata", "NumberMetadataPersistenceV2"),
            ("UrlMetadata", "UrlMetadataPersistenceV2"),
            ("TimestampMetadata", "TimestampMetadataPersistenceV2"),
            ("UserMetadata", "UserMetadataPersistenceV2"),
            ("SingleSelectionMetadata", "SingleSelectionMetadataPersistenceV2"),
            ("MultiSelectionMetadata", "MultiSelectionMetadataPersistenceV2")
        };

        static string MapTypes(string jsonString, bool inverseMapping = false)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            // Maps the types persisted in the files to newly created data-identical types
            foreach (var (original, replacement) in s_ClassMappings)
            {
                var searchFor = original;
                var replaceWith = replacement;
                if (inverseMapping)
                {
                    searchFor = replacement;
                    replaceWith = original;
                }

                jsonString = Regex.Replace(jsonString,
                        $"\"class\"[\u0009\u000D\u000A\u0020]*:[\u0009\u000D\u000A\u0020]*\"{searchFor}\"", // skip whitespaces around the ':'
                        $"\"class\":\"{replaceWith}\"", RegexOptions.None, TimeSpan.FromMilliseconds(100));
            }

            return jsonString;
        }

        public string SerializeEntry(AssetData assetData, IEnumerable<ImportedFileInfo> fileInfos)
        {
            var trackedAsset = new TrackedAssetPersisted();
            trackedAsset.serializationVersion = new[] { MajorVersion, MinorVersion };

            FillVersionFrom(trackedAsset, assetData);
            trackedAsset.organizationId = assetData.Identifier.OrganizationId;
            trackedAsset.projectId = assetData.Identifier.ProjectId;
            trackedAsset.assetId = assetData.Identifier.AssetId;
            trackedAsset.dependencyAssets = assetData.Dependencies
                .Select(Convert)
                .ToList();

            var importedFileInfos =
                fileInfos.ToDictionary(x => x.OriginalPath.Replace('\\', '/'), x => x);

            var files = new List<TrackedFilePersisted>();
            foreach (var dataset in assetData.Datasets)
            {
                files.AddRange(dataset.Files
                    .Select(x => ConvertToFile(dataset.Id, x, importedFileInfos.GetValueOrDefault(x.Path))));
            }

            trackedAsset.files = files;
            trackedAsset.datasets = assetData.Datasets
                .Select(x => new TrackedDatasetPersisted
                {
                    id = x.Id,
                    name = x.Name,
                    systemTags = x.SystemTags.ToList(),
                    fileKeys = x.Files.Select(f => f.Path).ToList()
                })
                .ToList();

            trackedAsset.metadata = assetData.Metadata.Select(Convert).ToList();

            return SerializeEntry(trackedAsset);
        }

        static AssetIdentifier ExtractAssetIdentifier(TrackedAssetPersisted trackedAsset)
        {
            return new AssetIdentifier(trackedAsset.organizationId, trackedAsset.projectId, trackedAsset.assetId,
                trackedAsset.versionId);
        }

        static AssetDataFile ConvertFile(TrackedFilePersisted trackedFile)
        {
            if (trackedFile == null)
            {
                return null;
            }

            var assetDataFile = new AssetDataFile(
                trackedFile.path,
                trackedFile.extension,
                null,
                trackedFile.description,
                trackedFile.tags,
                trackedFile.fileSize,
                trackedFile.available);

            return assetDataFile;
        }

        static IEnumerable<AssetDataset> ReconstructDatasets(List<TrackedDatasetPersisted> datasets, List<TrackedFilePersisted> files)
        {
            var datasetFiles = files.ToDictionary(x => x.path, x => x);
            foreach (var dataset in datasets)
            {
                var datasetFilesPaths = dataset.fileKeys;
                var datasetFilesData = datasetFilesPaths.Select(x => datasetFiles.GetValueOrDefault(x)).ToList();
                var datasetFilesDataFiltered = datasetFilesData.Where(x => x != null).ToList();
                var datasetFilesConverted = datasetFilesDataFiltered.Select(ConvertFile).ToList();
                yield return new AssetDataset(dataset.id, dataset.name, dataset.systemTags, datasetFilesConverted);
            }
        }

        static ImportedAssetInfo Convert(TrackedAssetPersisted trackedAsset, Persistence.ReadCache cache)
        {
            var assetIdentifier = ExtractAssetIdentifier(trackedAsset);
            var assetData = cache.GetAssetDataFor(assetIdentifier);

            var datasets = ReconstructDatasets(trackedAsset.datasets, trackedAsset.files);
            assetData.FillFromPersistence(
                new AssetIdentifier(trackedAsset.organizationId,
                    trackedAsset.projectId,
                    trackedAsset.assetId,
                    trackedAsset.versionId),
                trackedAsset.sequenceNumber,
                trackedAsset.parentSequenceNumber,
                trackedAsset.changelog,
                trackedAsset.name,
                trackedAsset.assetType,
                trackedAsset.status,
                trackedAsset.description,
                DateTime.Parse(trackedAsset.created, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.RoundtripKind),
                DateTime.Parse(trackedAsset.updated, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.RoundtripKind),
                trackedAsset.createdBy,
                trackedAsset.updatedBy,
                trackedAsset.previewFilePath,
                trackedAsset.isFrozen,
                trackedAsset.tags,
                datasets,
                trackedAsset.dependencyAssets
                    .Select(x => new AssetIdentifier(x.organizationId, x.projectId, x.assetId, x.versionId, x.versionLabel)),
                trackedAsset.metadata.Select(Convert));

            return new ImportedAssetInfo(
                assetData,
                trackedAsset.files
                    .Where(x => !string.IsNullOrEmpty(x.trackedUnityGuid))
                    .Select(x => new ImportedFileInfo(x.datasetId, x.trackedUnityGuid, x.path, x.checksum, x.timestamp, x.metaFileChecksum, x.metaFileTimestamp)));
        }

        static TrackedAssetIdentifierPersisted Convert(AssetIdentifier identifier)
        {
            return new TrackedAssetIdentifierPersisted()
            {
                organizationId = identifier.OrganizationId,
                projectId = identifier.ProjectId,
                assetId = identifier.AssetId,
                versionId = identifier.Version,
                versionLabel = identifier.VersionLabel
            };
        }

         static IMetadataPersistenceV2 Convert(IMetadata metadata)
        {
            switch (metadata)
            {
                case TextMetadata textMetadata:
                    return new TextMetadataPersistenceV2(textMetadata.FieldKey, textMetadata.Name, textMetadata.Value);
                case BooleanMetadata booleanMetadata:
                    return new BooleanMetadataPersistenceV2(booleanMetadata.FieldKey, booleanMetadata.Name,
                        booleanMetadata.Value);
                case NumberMetadata numberMetadata:
                    return new NumberMetadataPersistenceV2(numberMetadata.FieldKey, numberMetadata.Name,
                        numberMetadata.Value);
                case UrlMetadata urlMetadata:
                    return new UrlMetadataPersistenceV2(urlMetadata.FieldKey, urlMetadata.Name,
                        new UriEntryPersistenceV2(urlMetadata.Value.Uri, urlMetadata.Value.Label));
                case TimestampMetadata dateTimeMetadata:
                    return new TimestampMetadataPersistenceV2(dateTimeMetadata.FieldKey, dateTimeMetadata.Name,
                        new DateTimeEntryPersistenceV2(dateTimeMetadata.Value.DateTime));
                case UserMetadata userMetadata:
                    return new UserMetadataPersistenceV2(userMetadata.FieldKey, userMetadata.Name, userMetadata.Value);
                case SingleSelectionMetadata singleSelectionMetadata:
                    return new SingleSelectionMetadataPersistenceV2(singleSelectionMetadata.FieldKey,
                        singleSelectionMetadata.Name, singleSelectionMetadata.Value);
                case MultiSelectionMetadata multiSelectionMetadata:
                    return new MultiSelectionMetadataPersistenceV2(multiSelectionMetadata.FieldKey,
                        multiSelectionMetadata.Name, multiSelectionMetadata.Value.ToList());
                default:
                    Utilities.DevAssert(false, "Unknown metadata type");
                    return null;
            }
        }

        static IMetadata Convert(IMetadataPersistenceV2 metadata)
        {
            switch (metadata)
            {
                case TextMetadataPersistenceV2 textMetadata:
                    return new TextMetadata(textMetadata.FieldKey, textMetadata.Name, textMetadata.Value);
                case BooleanMetadataPersistenceV2 booleanMetadata:
                    return new BooleanMetadata(booleanMetadata.FieldKey, booleanMetadata.Name,
                        booleanMetadata.Value);
                case NumberMetadataPersistenceV2 numberMetadata:
                    return new NumberMetadata(numberMetadata.FieldKey, numberMetadata.Name,
                        numberMetadata.Value);
                case UrlMetadataPersistenceV2 urlMetadata:
                    return new UrlMetadata(urlMetadata.FieldKey, urlMetadata.Name,
                        new UriEntry(urlMetadata.Value.Uri, urlMetadata.Value.Label));
                case TimestampMetadataPersistenceV2 dateTimeMetadata:
                    return new TimestampMetadata(dateTimeMetadata.FieldKey, dateTimeMetadata.Name,
                        new DateTimeEntry(dateTimeMetadata.Value.DateTime));
                case UserMetadataPersistenceV2 userMetadata:
                    return new UserMetadata(userMetadata.FieldKey, userMetadata.Name, userMetadata.Value);
                case SingleSelectionMetadataPersistenceV2 singleSelectionMetadata:
                    return new SingleSelectionMetadata(singleSelectionMetadata.FieldKey,
                        singleSelectionMetadata.Name, singleSelectionMetadata.Value);
                case MultiSelectionMetadataPersistenceV2 multiSelectionMetadata:
                    return new MultiSelectionMetadata(multiSelectionMetadata.FieldKey,
                        multiSelectionMetadata.Name, multiSelectionMetadata.Value.ToList());
                default:
                    Utilities.DevAssert(false, "Unknown metadata type");
                    return null;
            }
        }

        static void FillVersionFrom(TrackedAssetVersionPersisted version, AssetData assetData)
        {
            version.versionId = assetData.Identifier.Version;
            version.name = assetData.Name;
            version.sequenceNumber = assetData.SequenceNumber;
            version.parentSequenceNumber = assetData.ParentSequenceNumber;
            version.changelog = assetData.Changelog;
            version.assetType = assetData.AssetType;
            version.status = assetData.Status;
            version.description = assetData.Description;
            version.created = assetData.Created?.ToString("o");
            version.updated = assetData.Updated?.ToString("o");
            version.createdBy = assetData.CreatedBy;
            version.updatedBy = assetData.UpdatedBy;
            version.previewFilePath = assetData.PreviewFilePath;
            version.isFrozen = assetData.IsFrozen;
            version.tags = assetData.Tags?.ToList();
        }

        static TrackedFilePersisted ConvertToFile(string datasetId, BaseAssetDataFile assetDataFile, ImportedFileInfo fileInfo)
        {
            return new TrackedFilePersisted
            {
                datasetId = datasetId,
                path = assetDataFile.Path,
                trackedUnityGuid = fileInfo?.Guid,
                extension = assetDataFile.Extension,
                available = assetDataFile.Available,
                description = assetDataFile.Description,
                fileSize = assetDataFile.FileSize,
                tags = assetDataFile.Tags?.ToList(),
                checksum = fileInfo?.Checksum,
                timestamp = fileInfo?.Timestamp ?? 0L,
                metaFileChecksum = fileInfo?.MetaFileChecksum,
                metaFileTimestamp = fileInfo?.MetalFileTimestamp ?? 0L
            };
        }

        static string SerializeEntry(TrackedAssetPersisted trackedAsset)
        {
            var jsonString = JsonUtility.ToJson(trackedAsset);
            jsonString = MapTypes(jsonString, inverseMapping: true);
            return jsonString;
        }
    }
}
