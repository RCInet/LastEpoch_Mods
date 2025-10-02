using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    enum MetadataDataSourceSpecification
    {
        metadata,
        systemMetadata,
    }

    abstract class MetadataDataSource : IMetadataDataSource
    {
        protected readonly IAssetDataSource m_DataSource;
        protected readonly MetadataDataSourceSpecification m_MetadataSpecification;

        protected abstract OrganizationId OrganizationId { get; }

        private protected MetadataDataSource(IAssetDataSource dataSource, MetadataDataSourceSpecification type)
        {
            m_DataSource = dataSource;
            m_MetadataSpecification = type;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, MetadataObject>> GetAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var filter = GetFieldsFilter();
            IMetadataInfo data;
            switch (m_MetadataSpecification)
            {
                case MetadataDataSourceSpecification.metadata:
                    filter?.MetadataFields.AddRange(keys);
                    data = await GetMetadataInfoAsync(filter, cancellationToken);
                    return data.Metadata?.From(m_DataSource, OrganizationId) ?? new Dictionary<string, MetadataObject>();
                case MetadataDataSourceSpecification.systemMetadata:
                    filter?.SystemMetadataFields.AddRange(keys);
                    data = await GetMetadataInfoAsync(filter, cancellationToken);
                    return data.SystemMetadata?.From() ?? new Dictionary<string, MetadataObject>();
            }

            return null;
        }

        /// <inheritdoc />
        public abstract Task AddOrUpdateAsync(Dictionary<string, object> properties, CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a filter for fetching metadata.
        /// </summary>
        /// <returns>A <see cref="FieldsFilter"/>. </returns>
        protected abstract FieldsFilter GetFieldsFilter();

        /// <summary>
        /// Returns metadata info for the given filter.
        /// </summary>
        /// <param name="filter">A filter for fetching metadata. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A <see cref="IMetadataInfo"/>. </returns>
        protected abstract Task<IMetadataInfo> GetMetadataInfoAsync(FieldsFilter filter, CancellationToken cancellationToken);
    }
}
