using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IDataset"/> search request.
    /// </summary>
    class DatasetSearchCriteria : CompoundSearchCriteria
    {
        /// <inheritdoc cref="DatasetProperties.Name"/>
        public StringSearchCriteria Name { get; } = new(nameof(DatasetProperties.Name), "name");
        
        /// <inheritdoc cref="DatasetProperties.Type"/>
        public AssetTypeSearchCriteria Type { get; } = new(nameof(DatasetProperties.Type));

        /// <inheritdoc cref="DatasetProperties.Description"/>
        public StringSearchCriteria Description { get; } = new(nameof(DatasetProperties.Description), "description");

        /// <inheritdoc cref="DatasetProperties.StatusName"/>
        public SearchCriteria<string> Status { get; } = new("Status", "status");

        /// <inheritdoc cref="DatasetProperties.Tags"/>
        public ListSearchCriteria<string> Tags { get; } = new(nameof(DatasetProperties.Tags), "tags");

        /// <inheritdoc cref="DatasetProperties.SystemTags"/>
        public ListSearchCriteria<string> SystemTags { get; } = new(nameof(DatasetProperties.SystemTags), "systemTags");

        /// <inheritdoc cref="DatasetProperties.AuthoringInfo"/>
        public AuthoringInfoSearchFilter AuthoringInfo { get; } = new(nameof(DatasetProperties.AuthoringInfo), string.Empty);

        /// <inheritdoc cref="IDataset.Metadata"/>
        public MetadataSearchCriteria Metadata { get; } = new(nameof(IDataset.Metadata), "metadata");

        /// <inheritdoc cref="IDataset.SystemMetadata"/>
        public MetadataSearchCriteria SystemMetadata { get; } = new(nameof(IDataset.SystemMetadata), "metadata");

        /// <inheritdoc cref="DatasetProperties.IsVisible"/>
        public NullableSearchCriteria<bool> IsVisible { get; } = new(nameof(DatasetProperties.IsVisible), "isVisible");
        
        /// <inheritdoc cref="DatasetProperties.WorkflowName"/>
        public StringSearchCriteria WorkflowName { get; } = new(nameof(DatasetProperties.WorkflowName), "workflowName");

        internal DatasetSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }
    }
}
