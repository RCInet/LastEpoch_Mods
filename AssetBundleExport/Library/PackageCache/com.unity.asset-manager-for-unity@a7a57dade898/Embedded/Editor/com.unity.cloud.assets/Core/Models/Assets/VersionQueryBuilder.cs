using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that builds and executes a query to return a the versions of an asset.
    /// </summary>
    sealed class VersionQueryBuilder
    {
        readonly IAssetDataSource m_AssetDataSource;
        readonly CacheConfigurationWrapper m_CacheConfiguration;
        readonly ProjectDescriptor m_ProjectDescriptor;
        readonly AssetId m_AssetId;

        IAssetSearchFilter m_AssetSearchFilter;
        Range m_Range = Range.All;
        string m_SortingField = "versionNumber";
        SortingOrder m_SortingOrder = SortingOrder.Ascending;

        internal VersionQueryBuilder(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, ProjectDescriptor projectDescriptor, AssetId assetId)
        {
            m_AssetDataSource = dataSource;
            m_CacheConfiguration = new CacheConfigurationWrapper(defaultCacheConfiguration);
            m_ProjectDescriptor = projectDescriptor;
            m_AssetId = assetId;
        }

        /// <summary>
        /// Sets an override to the default cache configuration for assets.
        /// </summary>
        /// <param name="assetCacheConfiguration">The configuration to apply when populating the assets. </param>
        /// <returns>The calling <see cref="VersionQueryBuilder"/>. </returns>
        public VersionQueryBuilder WithCacheConfiguration(AssetCacheConfiguration assetCacheConfiguration)
        {
            m_CacheConfiguration.SetAssetConfiguration(assetCacheConfiguration);
            return this;
        }

        /// <summary>
        /// Sets the filter to be used when querying the versions of the asset.
        /// </summary>
        /// <param name="assetSearchFilter">The query filter. </param>
        /// <returns>The calling <see cref="VersionQueryBuilder"/>. </returns>
        public VersionQueryBuilder SelectWhereMatchesFilter(IAssetSearchFilter assetSearchFilter)
        {
            m_AssetSearchFilter = assetSearchFilter;
            return this;
        }

        /// <summary>
        /// Sets the order in which the results will be returned.
        /// </summary>
        /// <param name="sortingField">The field by which to sort the results. </param>
        /// <param name="sortingOrder">The sorting order (Ascending|Descending). </param>
        /// <returns>The calling <see cref="VersionQueryBuilder"/>. </returns>
        public VersionQueryBuilder OrderBy(string sortingField, SortingOrder sortingOrder = SortingOrder.Ascending)
        {
            m_SortingField = sortingField;
            m_SortingOrder = sortingOrder;
            return this;
        }

        /// <summary>
        /// Sets the range of results to return.
        /// </summary>
        /// <param name="range">The range of results. </param>
        /// <returns>The calling <see cref="VersionQueryBuilder"/>. </returns>
        public VersionQueryBuilder LimitTo(Range range)
        {
            m_Range = range;
            return this;
        }

        /// <summary>
        /// Executes the query and returns the versions of the asset that satisfy the critiera.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IAsset"/> with the same <see cref="AssetId"/>. </returns>
        public async IAsyncEnumerable<IAsset> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var fieldsFilter = m_CacheConfiguration.GetAssetFieldsFilter();

            var parameters = new SearchRequestParameters(fieldsFilter)
            {
                Filter = m_AssetSearchFilter?.From(),
                Pagination = new SearchRequestPagination(m_SortingField, m_SortingOrder),
                PaginationRange = m_Range
            };

            var results = m_AssetDataSource.ListAssetVersionsAsync(m_ProjectDescriptor, m_AssetId, parameters, cancellationToken);
            await foreach (var result in results)
            {
                yield return result.From(m_AssetDataSource, m_CacheConfiguration.DefaultConfiguration, m_ProjectDescriptor, fieldsFilter, m_CacheConfiguration.AssetConfiguration);
            }
        }
    }
}
