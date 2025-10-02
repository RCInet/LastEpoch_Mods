using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IAsset"/> search request.
    /// </summary>
    class AssetSearchCriteria : CompoundSearchCriteria
    {
        /// <inheritdoc cref="AssetId"/>
        public SearchCriteria<string> Id { get; } = new(nameof(AssetDescriptor.AssetId), "assetId");

        /// <inheritdoc cref="AssetVersion"/>
        public SearchCriteria<string> Version { get; } = new(nameof(AssetDescriptor.AssetVersion), "assetVersion");

        /// <inheritdoc cref="AssetProperties.State"/>
        public AssetStateSearchCriteria State { get; } = new(nameof(AssetProperties.State));

        /// <inheritdoc cref="IAsset.IsFrozen"/>
        [Obsolete("Use State instead.")]
        public NullableSearchCriteria<bool> IsFrozen { get; } = new(nameof(IAsset.IsFrozen), "isFrozen");

        /// <inheritdoc cref="AssetProperties.FrozenSequenceNumber"/>
        public NullableSearchCriteria<int> FrozenSequenceNumber { get; } = new(nameof(AssetProperties.FrozenSequenceNumber), "versionNumber");

        /// <inheritdoc cref="AssetProperties.ParentVersion"/>
        public SearchCriteria<string> ParentVersion { get; } = new(nameof(AssetProperties.ParentVersion), "parentAssetVersion");

        /// <inheritdoc cref="AssetProperties.ParentFrozenSequenceNumber"/>
        public NullableSearchCriteria<int> ParentFrozenSequenceNumber { get; } = new(nameof(AssetProperties.ParentFrozenSequenceNumber), "parentVersionNumber");

        /// <inheritdoc cref="AssetProperties.Name"/>
        public StringSearchCriteria Name { get; } = new(nameof(AssetProperties.Name), "name");

        /// <inheritdoc cref="AssetProperties.Description"/>
        public StringSearchCriteria Description { get; } = new(nameof(AssetProperties.Description), "description");

        /// <inheritdoc cref="AssetProperties.Type"/>
        public AssetTypeSearchCriteria Type { get; } = new(nameof(AssetProperties.Type));

        /// <inheritdoc cref="AssetProperties.StatusName"/>
        public SearchCriteria<string> Status { get; } = new("Status", "status");

        /// <inheritdoc cref="AssetProperties.Tags"/>
        public ListSearchCriteria<string> Tags { get; } = new(nameof(AssetProperties.Tags), "tags");

        /// <inheritdoc cref="AssetProperties.SystemTags"/>
        public ListSearchCriteria<string> SystemTags { get; } = new(nameof(AssetProperties.SystemTags), "systemTags");

        /// <inheritdoc cref="AssetProperties.Labels"/>
        public ListSearchCriteria<string> Labels { get; } = new(nameof(AssetProperties.Labels), "labels");

        /// <inheritdoc cref="AssetProperties.ArchivedLabels"/>
        public ListSearchCriteria<string> ArchivedLabels { get; } = new(nameof(AssetProperties.ArchivedLabels), "archivedLabels");

        /// <inheritdoc cref="IAsset.Metadata"/>
        public MetadataSearchCriteria Metadata { get; } = new(nameof(IAsset.Metadata), "metadata");

        /// <inheritdoc cref="IAsset.SystemMetadata"/>
        public MetadataSearchCriteria SystemMetadata { get; } = new(nameof(IAsset.SystemMetadata), "systemMetadata");

        /// <inheritdoc cref="AssetProperties.PreviewFileDescriptor"/>
        public StringSearchCriteria PreviewFile { get; } = new("PreviewFile", "previewFile");

        /// <inheritdoc cref="AssetProperties.SourceProject"/>
        public SearchCriteria<string> SourceProjectId { get; } = new(nameof(AssetProperties.SourceProject), "sourceProjectId");

        /// <inheritdoc cref="AssetProperties.AuthoringInfo"/>
        public AuthoringInfoSearchFilter AuthoringInfo { get; } = new(nameof(AssetProperties.AuthoringInfo), string.Empty);

        /// <inheritdoc cref="FileProperties"/>
        public FileSearchCriteria Files { get; } = new("Files", "files");

        /// <inheritdoc cref="DatasetProperties"/>
        public DatasetSearchCriteria Datasets { get; } = new("Datasets", "datasets");

        internal AssetSearchCriteria()
            : base(string.Empty, string.Empty) { }
    }
}
