using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that builds and executes a query to return a set of collections.
    /// </summary>
    class CollectionQueryBuilder
    {
        readonly IAssetDataSource m_DataSource;
        readonly AssetRepositoryCacheConfiguration m_DefaultCacheConfiguration;
        readonly ProjectDescriptor m_ProjectDescriptor;

        AssetCollectionCacheConfiguration? m_AssetCollectionCacheConfiguration;
        Range m_Range = Range.All;

        internal CollectionQueryBuilder(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, ProjectDescriptor projectDescriptor)
        {
            m_DataSource = dataSource;
            m_DefaultCacheConfiguration = defaultCacheConfiguration;
            m_ProjectDescriptor = projectDescriptor;
        }

        /// <summary>
        /// Sets an override to the default cache configuration for the query.
        /// </summary>
        /// <param name="assetCollectionCacheConfiguration">The configuration to apply when populating the collections. </param>
        /// <returns>The calling <see cref="CollectionQueryBuilder"/>. </returns>
        public CollectionQueryBuilder WithCacheConfiguration(AssetCollectionCacheConfiguration assetCollectionCacheConfiguration)
        {
            m_AssetCollectionCacheConfiguration = assetCollectionCacheConfiguration;
            return this;
        }

        /// <summary>
        /// Sets the range of results to return.
        /// </summary>
        /// <param name="range">The range of results to return. </param>
        /// <returns>The calling <see cref="CollectionQueryBuilder"/>. </returns>
        public CollectionQueryBuilder LimitTo(Range range)
        {
            m_Range = range;
            return this;
        }

        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IAssetCollection"/>. </returns>
        public async IAsyncEnumerable<IAssetCollection> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var results = m_DataSource.ListCollectionsAsync(m_ProjectDescriptor, m_Range, cancellationToken);
            await foreach (var data in results)
            {
                if (cancellationToken.IsCancellationRequested) yield break;

                yield return data.From(m_DataSource, m_DefaultCacheConfiguration, m_ProjectDescriptor, m_AssetCollectionCacheConfiguration);
            }
        }
    }
}
