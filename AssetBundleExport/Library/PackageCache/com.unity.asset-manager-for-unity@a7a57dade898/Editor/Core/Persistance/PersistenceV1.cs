using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    class PersistenceV1 : IPersistenceVersion
    {
        public int MajorVersion => 1;
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

            [SerializeReference]
            public List<IMetadata> metadata;
        }

        [Serializable]
        class TrackedFilePersisted
        {
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

            var files = new List<BaseAssetDataFile>();
            foreach (var dataset in assetData.Datasets)
            {
                files.AddRange(dataset.Files);
            }

            trackedAsset.files = files
                .Select(x => x as AssetDataFile)
                .Where(x => x != null)
                .Select(x => ConvertToFile(x, importedFileInfos.GetValueOrDefault(x.Path)))
                .ToList();

            trackedAsset.metadata = assetData.Metadata.ToList();

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

        static ImportedAssetInfo Convert(TrackedAssetPersisted trackedAsset, Persistence.ReadCache cache)
        {
            var assetIdentifier = ExtractAssetIdentifier(trackedAsset);
            var assetData = cache.GetAssetDataFor(assetIdentifier);

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
                trackedAsset.files
                    .Select(ConvertFile),
                trackedAsset.dependencyAssets
                    .Select(x => new AssetIdentifier(x.organizationId, x.projectId, x.assetId, x.versionId)),
                trackedAsset.metadata);

            return new ImportedAssetInfo(
                assetData,
                trackedAsset.files
                    .Where(x => !string.IsNullOrEmpty(x.trackedUnityGuid))
                    .Select(x => new ImportedFileInfo(string.Empty, x.trackedUnityGuid, x.path, x.checksum, x.timestamp, x.metaFileChecksum, x.metaFileTimestamp)));
        }

        static TrackedAssetIdentifierPersisted Convert(AssetIdentifier identifier)
        {
            return new TrackedAssetIdentifierPersisted()
            {
                organizationId = identifier.OrganizationId,
                projectId = identifier.ProjectId,
                assetId = identifier.AssetId,
                versionId = identifier.Version
            };
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

        static TrackedFilePersisted ConvertToFile(AssetDataFile assetDataFile, ImportedFileInfo fileInfo)
        {
            var trackedFile = new TrackedFilePersisted();
            trackedFile.path = assetDataFile.Path;
            trackedFile.trackedUnityGuid = fileInfo?.Guid;
            trackedFile.extension = assetDataFile.Extension;
            trackedFile.available = assetDataFile.Available;
            trackedFile.description = assetDataFile.Description;
            trackedFile.fileSize = assetDataFile.FileSize;
            trackedFile.tags = assetDataFile.Tags?.ToList();
            trackedFile.checksum = fileInfo?.Checksum;
            trackedFile.timestamp = fileInfo?.Timestamp ?? 0L;
            trackedFile.metaFileChecksum = fileInfo?.MetaFileChecksum;
            trackedFile.metaFileTimestamp = fileInfo?.MetalFileTimestamp ?? 0L;

            return trackedFile;
        }

        static string SerializeEntry(TrackedAssetPersisted trackedAsset)
        {
            return JsonUtility.ToJson(trackedAsset);
        }

    }
}
