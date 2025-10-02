using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    // Dev note: [DataContract] and [EnumMember] are artifacts of the old serialization strategy.
    // The attributes are maintained for compatibility reasons and to avoid a breaking change.
    [DataContract]
enum GroupableField
    {
        [EnumMember(Value = "name")]
        Name = 0,
        [EnumMember(Value = "assetVersion")]
        Version,
        [EnumMember(Value = "primaryType")]
        Type,
        [EnumMember(Value = "status")]
        Status,
        [EnumMember(Value = "tags")]
        Tags,
        [EnumMember(Value = "systemTags")]
        SystemTags,
        [EnumMember(Value = "previewFile")]
        PreviewFile,
        [EnumMember(Value = "createdBy")]
        CreatedBy,
        [EnumMember(Value = "updatedBy")]
        UpdateBy,

        AssetId,
        Description,
        ChangeLog,
        Labels,
        ArchivedLabels,
        StatusFlowName,

        DatasetName = 30,
        DatasetsDescription,
        DatasetPrimaryType,
        DatasetTags,
        DatasetSystemTags,
        DatasetWorkflowName,
        DatasetCreatedBy,
        DatasetUpdatedBy,

        FilePath = 60,
        FileDescription,
        FilePrimaryType,
        FileStatus,
        FileTags,
        FileSystemTags,
        FileUserChecksum,
        FileCreatedBy,
        FileUpdatedBy,

        Collections = 100,
    }

    /// <summary>
    /// The type of <see cref="GroupableFieldValue"/>.
    /// </summary>
    enum GroupableFieldValueType
    {
        String = 0,
        UserId,
        AssetId,
        AssetVersion,
        DatasetId,
        CollectionDescriptor,
        AssetType,
        MetadataValue,
    }

    enum MetadataOwner
    {
        Asset,
        Dataset,
        File
    }

    static class GroupAndCountExtensions
    {
        internal static string From(this GroupableField field)
        {
            return field switch
            {
                GroupableField.Name => "name",
                GroupableField.Version => "assetVersion",
                GroupableField.Type => "primaryType",
                GroupableField.Status => "status",
                GroupableField.Tags => "tags",
                GroupableField.SystemTags => "systemTags",
                GroupableField.PreviewFile => "previewFile",
                GroupableField.CreatedBy => "createdBy",
                GroupableField.UpdateBy => "updatedBy",
                GroupableField.AssetId => "assetId",
                GroupableField.Description => "description",
                GroupableField.ChangeLog => "changeLog",
                GroupableField.Labels => "labels",
                GroupableField.ArchivedLabels => "archivedLabels",
                GroupableField.StatusFlowName => "statusFlowName",
                GroupableField.DatasetName => "datasets.name",
                GroupableField.DatasetsDescription => "datasets.description",
                GroupableField.DatasetPrimaryType => "datasets.primaryType",
                GroupableField.DatasetTags => "datasets.tags",
                GroupableField.DatasetSystemTags => "datasets.systemTags",
                GroupableField.DatasetWorkflowName => "datasets.workflowName",
                GroupableField.DatasetCreatedBy => "datasets.createdBy",
                GroupableField.DatasetUpdatedBy => "datasets.updatedBy",
                GroupableField.FilePath => "files.filePath",
                GroupableField.FileDescription => "files.description",
                GroupableField.FilePrimaryType => "files.primaryType",
                GroupableField.FileStatus => "files.status",
                GroupableField.FileTags => "files.tags",
                GroupableField.FileSystemTags => "files.systemTags",
                GroupableField.FileUserChecksum => "files.userChecksum",
                GroupableField.FileCreatedBy => "files.createdBy",
                GroupableField.FileUpdatedBy => "files.updatedBy",
                GroupableField.Collections => "collections",
                _ => string.Empty
            };

            // Custom execute methods:

            // [x] metadata.{field}
            // [x] systemMetadata.{field}
            // [x] datasets.metadata.{field}
            // [x] datasets.systemMetadata.{field}
            // [x] files.metadata.{field}
            // [x] files.systemMetadata.{field}

            // [] statusFlowId
            // [] previewFileDatasetId

            // [] datasets.fileOrder

            // [] files.datasetIds
        }

        internal static string FromMetadata(this MetadataOwner metadataOwner)
        {
            return metadataOwner.From("metadata");
        }

        internal static string FromSystemMetadata(this MetadataOwner metadataOwner)
        {
            return metadataOwner.From("systemMetadata");
        }

        static string From(this MetadataOwner metadataOwner, string prefix)
        {
            return metadataOwner switch
            {
                MetadataOwner.Asset => prefix,
                MetadataOwner.Dataset => $"datasets.{prefix}",
                MetadataOwner.File => $"files.{prefix}",
                _ => throw new ArgumentOutOfRangeException(nameof(metadataOwner), metadataOwner, null)
            };
        }
    }
}
