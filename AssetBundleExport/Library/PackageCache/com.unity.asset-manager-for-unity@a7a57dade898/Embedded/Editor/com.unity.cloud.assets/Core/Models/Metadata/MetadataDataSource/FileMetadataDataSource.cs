using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class FileMetadataDataSource : MetadataDataSource
    {
        readonly FileDescriptor m_Descriptor;

        protected override OrganizationId OrganizationId => m_Descriptor.OrganizationId;

        internal FileMetadataDataSource(FileDescriptor fileDescriptor, IAssetDataSource dataSource, MetadataDataSourceSpecification specification)
            : base(dataSource, specification)
        {
            m_Descriptor = fileDescriptor;
        }

        /// <inheritdoc />
        public override Task AddOrUpdateAsync(Dictionary<string, object> properties, CancellationToken cancellationToken)
        {
            var data = new FileBaseData
            {
                Metadata = properties
            };
            return m_DataSource.UpdateFileAsync(m_Descriptor, data, cancellationToken);
        }

        /// <inheritdoc />
        public override Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            return m_DataSource.RemoveFileMetadataAsync(m_Descriptor, m_MetadataSpecification.ToString(), keys, cancellationToken);
        }

        /// <inheritdoc />
        protected override FieldsFilter GetFieldsFilter()
        {
            return new FieldsFilter
            {
                FileFields = m_MetadataSpecification == MetadataDataSourceSpecification.systemMetadata ? FileFields.systemMetadata : FileFields.metadata
            };
        }

        /// <inheritdoc />
        protected override async Task<IMetadataInfo> GetMetadataInfoAsync(FieldsFilter filter, CancellationToken cancellationToken)
        {
            return await m_DataSource.GetFileAsync(m_Descriptor, filter, cancellationToken);
        }
    }
}
