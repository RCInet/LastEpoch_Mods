using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class DatasetMetadataDataSource : MetadataDataSource
    {
        readonly DatasetDescriptor m_Descriptor;

        protected override OrganizationId OrganizationId => m_Descriptor.OrganizationId;

        internal DatasetMetadataDataSource(DatasetDescriptor datasetDescriptor, IAssetDataSource dataSource, MetadataDataSourceSpecification specification)
            : base(dataSource, specification)
        {
            m_Descriptor = datasetDescriptor;
        }

        /// <inheritdoc />
        public override Task AddOrUpdateAsync(Dictionary<string, object> properties, CancellationToken cancellationToken)
        {
            var data = new DatasetUpdateData
            {
                Metadata = properties
            };
            return m_DataSource.UpdateDatasetAsync(m_Descriptor, data, cancellationToken);
        }

        /// <inheritdoc />
        public override Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            return m_DataSource.RemoveDatasetMetadataAsync(m_Descriptor, m_MetadataSpecification.ToString(), keys, cancellationToken);
        }

        /// <inheritdoc />
        protected override FieldsFilter GetFieldsFilter()
        {
            return new FieldsFilter
            {
                DatasetFields = m_MetadataSpecification == MetadataDataSourceSpecification.metadata ? DatasetFields.metadata : DatasetFields.systemMetadata,
            };
        }

        /// <inheritdoc />
        protected override async Task<IMetadataInfo> GetMetadataInfoAsync(FieldsFilter filter, CancellationToken cancellationToken)
        {
            return await m_DataSource.GetDatasetAsync(m_Descriptor, filter, cancellationToken);
        }
    }
}
