using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IFile"/> search request.
    /// </summary>
    class FileSearchCriteria : CompoundSearchCriteria
    {
        /// <inheritdoc cref="FileDescriptor.Path"/>
        public StringSearchCriteria Path { get; } = new(nameof(FileDescriptor.Path), "filePath");

        /// <inheritdoc cref="FileProperties.Description"/>
        public StringSearchCriteria Description { get; } = new(nameof(FileProperties.Description), "description");

        /// <inheritdoc cref="FileProperties.StatusName"/>
        public SearchCriteria<string> Status { get; } = new("Status", "status");

        /// <inheritdoc cref="FileProperties.Tags"/>
        public ListSearchCriteria<string> Tags { get; } = new(nameof(FileProperties.Tags), "tags");

        /// <inheritdoc cref="FileProperties.SystemTags"/>
        public ListSearchCriteria<string> SystemTags { get; } = new(nameof(FileProperties.SystemTags), "systemTags");

        /// <inheritdoc cref="FileProperties.AuthoringInfo"/>
        public AuthoringInfoSearchFilter AuthoringInfo { get; } = new(nameof(FileProperties.AuthoringInfo), string.Empty);

        /// <inheritdoc cref="FileProperties.SizeBytes"/>
        [Obsolete("Use Size instead.")]
        public SearchCriteria<long> SizeBytes { get; } = new(nameof(FileProperties.SizeBytes), "fileSize");

        /// <inheritdoc cref="FileProperties.SizeBytes"/>
        public NumericSearchCriteria<long> Size { get; } = new(nameof(FileProperties.SizeBytes), "fileSize");

        /// <inheritdoc cref="IFile.Metadata"/>
        public MetadataSearchCriteria Metadata { get; } = new(nameof(IFile.Metadata), "metadata");

        /// <inheritdoc cref="IFile.SystemMetadata"/>
        public MetadataSearchCriteria SystemMetadata { get; } = new(nameof(IFile.SystemMetadata), "systemMetadata");

        internal FileSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }
    }
}
