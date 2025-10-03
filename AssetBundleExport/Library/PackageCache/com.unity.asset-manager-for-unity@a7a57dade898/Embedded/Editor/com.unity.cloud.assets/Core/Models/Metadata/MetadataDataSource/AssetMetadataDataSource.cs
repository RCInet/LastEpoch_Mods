using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class AssetMetadataDataSource : MetadataDataSource
    {
        readonly AssetDescriptor m_Descriptor;

        protected override OrganizationId OrganizationId => m_Descriptor.OrganizationId;

        internal AssetMetadataDataSource(AssetDescriptor assetDescriptor, IAssetDataSource dataSource, MetadataDataSourceSpecification specification)
            : base(dataSource, specification)
        {
            m_Descriptor = assetDescriptor;
        }

        /// <inheritdoc />
        public override Task AddOrUpdateAsync(Dictionary<string, object> properties, CancellationToken cancellationToken)
        {
            var data = new AssetUpdateData
            {
                Metadata = properties
            };

            return m_DataSource.UpdateAssetAsync(m_Descriptor, data, cancellationToken);
        }

        /// <inheritdoc />
        public override Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            return m_DataSource.RemoveAssetMetadataAsync(m_Descriptor, m_MetadataSpecification.ToString(), keys, cancellationToken);
        }

        /// <inheritdoc />
        protected override FieldsFilter GetFieldsFilter()
        {
            return new FieldsFilter
            {
                AssetFields = m_MetadataSpecification == MetadataDataSourceSpecification.metadata ? AssetFields.metadata : AssetFields.systemMetadata
            };
        }

        /// <inheritdoc />
        protected override async Task<IMetadataInfo> GetMetadataInfoAsync(FieldsFilter filter, CancellationToken cancellationToken)
        {
            return await m_DataSource.GetAssetAsync(m_Descriptor, filter, cancellationToken);
        }
    }
}
