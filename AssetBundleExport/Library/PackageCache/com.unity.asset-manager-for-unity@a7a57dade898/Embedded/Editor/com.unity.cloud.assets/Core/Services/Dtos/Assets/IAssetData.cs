using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This interface contains all the information about a cloud asset.
    /// </summary>
    interface IAssetData : IAssetBaseData, IAuthoringData
    {
        /// <summary>
        /// The id of the asset.
        /// </summary>
        [DataMember(Name = "assetId")]
        AssetId Id { get; }

        /// <summary>
        /// The version of the asset.
        /// </summary>
        [DataMember(Name = "assetVersion")]
        AssetVersion Version { get; }

        /// <summary>
        /// Whether the asset is frozen.
        /// </summary>
        [DataMember(Name = "isFrozen")]
        bool IsFrozen { get; set; }

        /// <summary>
        /// The version number of the asset.
        /// </summary>
        [DataMember(Name = "versionNumber")]
        int VersionNumber
        {
            get => default;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The change log of this asset version.
        /// </summary>
        [DataMember(Name = "changeLog")]
        string Changelog
        {
            get => default;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The parent version of the asset.
        /// </summary>
        [DataMember(Name = "parentAssetVersion")]
        AssetVersion ParentVersion
        {
            get => default;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The parent version number of the asset.
        /// </summary>
        [DataMember(Name = "parentVersionNumber")]
        int ParentVersionNumber
        {
            get => default;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        [DataMember(Name = "systemTags")]
        IEnumerable<string> SystemTags { get; set; }

        /// <summary>
        /// The labels of the asset.
        /// </summary>
        [DataMember(Name = "labels")]
        IEnumerable<string> Labels
        {
            get => default;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The labels of the asset.
        /// </summary>
        [DataMember(Name = "archivedLabels")]
        IEnumerable<string> ArchivedLabels
        {
            get => default;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The status of the asset.
        /// </summary>
        [DataMember(Name = "status")]
        string Status { get; set; }

        /// <summary>
        /// The source id of the project the asset belongs to.
        /// </summary>
        [DataMember(Name = "sourceProjectId")]
        ProjectId SourceProjectId { get; set; }

        /// <summary>
        /// The project ids to which the asset is linked.
        /// </summary>
        [DataMember(Name = "projectIds")]
        IEnumerable<ProjectId> LinkedProjectIds { get; set; }

        /// <summary>
        /// The dataset id of the preview file.
        /// </summary>
        [DataMember(Name = "previewFileDatasetId")]
        DatasetId PreviewFileDatasetId
        {
            get => default;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The preview file ID of the asset.
        /// </summary>
        [DataMember(Name = "previewFile")]
        string PreviewFilePath
        {
            get => default;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The preview file ID of the asset.
        /// </summary>
        [DataMember(Name = "previewFileUrl")]
        string PreviewFileUrl
        {
            get => default;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The files associated with the asset's datasets.
        /// </summary>
        [DataMember(Name = "files")]
        IEnumerable<FileData> Files { get; set; }

        /// <summary>
        /// The datasets of the asset.
        /// </summary>
        [DataMember(Name = "datasets")]
        IEnumerable<DatasetData> Datasets { get; set; }

        /// <summary>
        /// The collections the asset belongs to
        /// </summary>
        [DataMember(Name = "collections")]
        IEnumerable<CollectionPath> Collections { get; set; }

        [DataMember(Name = "statusFlowId")]
        string StatusFlowId
        {
            get => default;
            set => throw new NotImplementedException();
        }

        [DataMember(Name = "statusFlowName")]
        string StatusFlowName
        {
            get => default;
            set => throw new NotImplementedException();
        }

        [DataMember(Name = "autoSubmit")]
        bool AutoSubmit
        {
            get => default;
            set => throw new NotImplementedException();
        }
    }
}
