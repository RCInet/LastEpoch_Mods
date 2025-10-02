using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    [Flags]
    enum AssetFields
    {
        /// <summary>
        /// Only the default fields will be populated.
        /// </summary>
        none = 0,
        all = ~none,
        description = 1,
        authoring = 2,
        metadata = 4,
        systemMetadata = 8,
        previewFileUrl = 32,
        /// <summary>
        /// Will populate the dataset cache with only the default fields; use DatasetFields to specify which fields to populate.
        /// </summary>
        datasets = 64,
        /// <summary>
        /// Will populate the file cache with only the default fields; use FileFields to specify which fields to populate.
        /// </summary>
        files = 128,
        versioning = 256,
        labels = 512,
        previewFile = 1024,
    }

    [Flags]
    enum DatasetFields
    {
        /// <summary>
        /// Only the default fields will be populated
        /// </summary>
        none = 0,
        all = ~none,
        description = 1,
        authoring = 2,
        metadata = 8,
        systemMetadata = 16,
        files = 32,
        filesOrder = 64,
        primaryType = 128,
        workflowName = 256,
    }

    [Flags]
    enum FileFields
    {
        /// <summary>
        /// Only the default fields will be populated
        /// </summary>
        none = 0,
        all = ~none,
        description = 1,
        authoring = 2,
        downloadURL = 4,
        metadata = 16,
        systemMetadata = 32,
        userChecksum = 64,
        fileSize = 128,
        previewURL = 256,
    }

    class FieldsFilter
    {
        public AssetFields AssetFields { get; set; } = AssetFields.none;
        public DatasetFields DatasetFields { get; set; } = DatasetFields.none;
        public FileFields FileFields { get; set; } = FileFields.none;
        public List<string> MetadataFields { get; } = new();
        public List<string> SystemMetadataFields { get; } = new();

        public void UnionMetadataFields(IEnumerable<string> fields)
        {
            if (fields == null) return;

            foreach (var field in fields)
            {
                if (!MetadataFields.Contains(field))
                {
                    MetadataFields.Add(field);
                }
            }
        }

        public void UnionSystemMetadataFields(IEnumerable<string> fields)
        {
            if (fields == null) return;

            foreach (var field in fields)
            {
                if (!SystemMetadataFields.Contains(field))
                {
                    SystemMetadataFields.Add(field);
                }
            }
        }

        public static FieldsFilter None => new()
        {
            AssetFields = AssetFields.none,
            DatasetFields = DatasetFields.none,
            FileFields = FileFields.none,
        };

        public static FieldsFilter DefaultAssetIncludes => new()
        {
            AssetFields = AssetFields.description | AssetFields.authoring | AssetFields.versioning | AssetFields.labels | AssetFields.previewFile,
            DatasetFields = DatasetFields.none,
            FileFields = FileFields.none,
        };

        public static FieldsFilter DefaultDatasetIncludes => new()
        {
            AssetFields = AssetFields.datasets,
            DatasetFields = DatasetFields.description | DatasetFields.authoring | DatasetFields.filesOrder | DatasetFields.primaryType | DatasetFields.workflowName,
            FileFields = FileFields.none,
        };

        public static FieldsFilter DefaultFileIncludes => new()
        {
            AssetFields = AssetFields.files,
            DatasetFields = DatasetFields.none,
            FileFields = FileFields.description | FileFields.authoring | FileFields.userChecksum | FileFields.fileSize
        };

        public static FieldsFilter All => new()
        {
            AssetFields = AssetFields.all,
            DatasetFields = DatasetFields.all,
            FileFields = FileFields.all,
        };
    }
}
