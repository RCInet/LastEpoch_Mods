using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    class PersistenceV3 : IPersistenceVersion
    {
        public int MajorVersion => 3;
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

            [SerializeField]
            public TrackedMetadataPersisted metadata;
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

        [Serializable]
        class TrackedMetadataPersisted
        {
            [SerializeField]
            public List<TrackedStringMetadataPersisted> textMedatatas = new();
            [SerializeField]
            public List<TarckedBooleanMetadataPersisted> booleanMedatatas = new();
            [SerializeField]
            public List<TrackedNumberMetadataPersisted> numberMedatatas = new();
            [SerializeField]
            public List<TrackedUrlMetadataPersisted> urlMedatatas = new() ;
            [SerializeField]
            public List<TrackedStringMetadataPersisted> timestampMedatatas = new();
            [SerializeField]
            public List<TrackedStringMetadataPersisted> userMedatatas = new();
            [SerializeField]
            public List<TrackedStringMetadataPersisted> singleSelectionMedatatas = new();
            [SerializeField]
            public List<TrackedStringListMetadataPersisted> multiSelectionMedatatas = new();
        }

        [Serializable]
        class TrackedStringMetadataPersisted
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string displayName;

            [SerializeField]
            public string value;
        }

        [Serializable]
        class TarckedBooleanMetadataPersisted
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string displayName;

            [SerializeField]
            public bool value;
        }

        [Serializable]
        class TrackedNumberMetadataPersisted
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string displayName;

            [SerializeField]
            public double value;
        }

        [Serializable]
        struct UriPersisted
        {
            [SerializeField]
            public string uri;

            [SerializeField]
            public string label;
        }

        [Serializable]
        class TrackedUrlMetadataPersisted
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string displayName;

            [SerializeField]
            public UriPersisted value;
        }

        [Serializable]
        class TrackedStringListMetadataPersisted
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string displayName;

            [SerializeField]
            public List<string> value;
        }

        public ImportedAssetInfo ConvertToImportedAssetInfo(string content)
        {
            var trackedAsset = JsonUtility.FromJson<TrackedAssetPersisted>(content);
            var cache = new Persistence.ReadCache();

            return Convert(trackedAsset, cache);
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

            trackedAsset.metadata = Convert(assetData.Metadata.ToList());

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
                Convert(trackedAsset.metadata));

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

        static TrackedMetadataPersisted Convert(List<IMetadata> metadatas)
        {
            var trackedMetadata = new TrackedMetadataPersisted();

            foreach (var metadata in metadatas)
            {
                switch (metadata)
                {
                    case TextMetadata textMetadata:
                        trackedMetadata.textMedatatas.Add(
                            new TrackedStringMetadataPersisted()
                            {
                                key = textMetadata.FieldKey,
                                displayName = textMetadata.Name,
                                value = textMetadata.Value
                            });
                        break;
                    case BooleanMetadata booleanMetadata:
                        trackedMetadata.booleanMedatatas.Add(
                            new TarckedBooleanMetadataPersisted()
                            {
                                key = booleanMetadata.FieldKey,
                                displayName = booleanMetadata.Name,
                                value = booleanMetadata.Value
                            });
                        break;
                    case NumberMetadata numberMetadata:
                        trackedMetadata.numberMedatatas.Add(
                            new TrackedNumberMetadataPersisted()
                            {
                                key = numberMetadata.FieldKey,
                                displayName = numberMetadata.Name,
                                value = numberMetadata.Value
                            });
                        break;
                    case UrlMetadata urlMetadata:
                        trackedMetadata.urlMedatatas.Add(
                            new TrackedUrlMetadataPersisted()
                            {
                                key = urlMetadata.FieldKey,
                                displayName = urlMetadata.Name,
                                value = new UriPersisted()
                                {
                                    uri = urlMetadata.Value.Uri.ToString(),
                                    label = urlMetadata.Value.Label
                                }
                            });
                        break;
                    case TimestampMetadata dateTimeMetadata:
                        trackedMetadata.timestampMedatatas.Add(
                            new TrackedStringMetadataPersisted()
                            {
                                key = dateTimeMetadata.FieldKey,
                                displayName = dateTimeMetadata.Name,
                                value = dateTimeMetadata.Value.DateTime.ToString("o")
                            });
                        break;
                    case UserMetadata userMetadata:
                        trackedMetadata.userMedatatas.Add(
                            new TrackedStringMetadataPersisted()
                            {
                                key = userMetadata.FieldKey,
                                displayName = userMetadata.Name,
                                value = userMetadata.Value
                            });
                        break;
                    case SingleSelectionMetadata singleSelectionMetadata:
                        trackedMetadata.singleSelectionMedatatas.Add(
                            new TrackedStringMetadataPersisted()
                            {
                                key = singleSelectionMetadata.FieldKey,
                                displayName = singleSelectionMetadata.Name,
                                value = singleSelectionMetadata.Value
                            });
                        break;
                    case MultiSelectionMetadata multiSelectionMetadata:
                        trackedMetadata.multiSelectionMedatatas.Add(
                            new TrackedStringListMetadataPersisted()
                            {
                                key = multiSelectionMetadata.FieldKey,
                                displayName = multiSelectionMetadata.Name,
                                value = multiSelectionMetadata.Value
                            });
                        break;
                    default:
                        Utilities.DevAssert(false, "Unknown metadata type");
                        return null;
                }
            }
            return trackedMetadata;
        }

        static List<IMetadata> Convert(TrackedMetadataPersisted trackedMetadataPersisted)
        {
            var metadata = new List<IMetadata>();
            foreach (var textMetadata in trackedMetadataPersisted.textMedatatas)
            {
                metadata.Add(new TextMetadata(textMetadata.key, textMetadata.displayName, textMetadata.value));
            }
            foreach (var booleanMetadata in trackedMetadataPersisted.booleanMedatatas)
            {
                metadata.Add(new BooleanMetadata(booleanMetadata.key, booleanMetadata.displayName, booleanMetadata.value));
            }
            foreach (var numberMetadata in trackedMetadataPersisted.numberMedatatas)
            {
                metadata.Add(new NumberMetadata(numberMetadata.key, numberMetadata.displayName, numberMetadata.value));
            }
            foreach (var urlMetadata in trackedMetadataPersisted.urlMedatatas)
            {
                metadata.Add(new UrlMetadata(urlMetadata.key, urlMetadata.displayName,
                    new UriEntry(new Uri(urlMetadata.value.uri), urlMetadata.value.label)));
            }
            foreach (var dateTimeMetadata in trackedMetadataPersisted.timestampMedatatas)
            {
                metadata.Add(
                    new TimestampMetadata(
                        dateTimeMetadata.key,
                        dateTimeMetadata.displayName,
                        new DateTimeEntry(DateTime.Parse(dateTimeMetadata.value, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.RoundtripKind))));
            }
            foreach (var userMetadata in trackedMetadataPersisted.userMedatatas)
            {
                metadata.Add(new UserMetadata(userMetadata.key, userMetadata.displayName, userMetadata.value));
            }
            foreach (var singleSelectionMetadata in trackedMetadataPersisted.singleSelectionMedatatas)
            {
                metadata.Add(new SingleSelectionMetadata(singleSelectionMetadata.key, singleSelectionMetadata.displayName, singleSelectionMetadata.value));
            }
            foreach (var multiSelectionMetadata in trackedMetadataPersisted.multiSelectionMedatatas)
            {
                metadata.Add(new MultiSelectionMetadata(multiSelectionMetadata.key, multiSelectionMetadata.displayName, multiSelectionMetadata.value));
            }
            return metadata;
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
            return JsonUtility.ToJson(trackedAsset);
        }
    }
}
