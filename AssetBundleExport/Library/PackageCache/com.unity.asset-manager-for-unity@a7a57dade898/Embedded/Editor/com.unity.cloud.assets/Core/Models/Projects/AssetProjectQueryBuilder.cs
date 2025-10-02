using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that builds and executes a query to return a set of projects.
    /// </summary>
    class AssetProjectQueryBuilder
    {
        readonly IAssetDataSource m_AssetDataSource;
        readonly AssetRepositoryCacheConfiguration m_DefaultCacheConfiguration;
        readonly OrganizationId m_OrganizationId;

        AssetProjectCacheConfiguration? m_AssetProjectCacheConfiguration;
        Range m_Range = Range.All;

        internal AssetProjectQueryBuilder(IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, OrganizationId organizationId)
        {
            m_AssetDataSource = assetDataSource;
            m_DefaultCacheConfiguration = defaultCacheConfiguration;
            m_OrganizationId = organizationId;
        }

        /// <summary>
        /// Sets an override to the default cache configuration for the query.
        /// </summary>
        /// <param name="assetProjectCacheConfiguration">The configuration to apply when populating the projects. </param>
        /// <returns>The calling <see cref="AssetProjectQueryBuilder"/>. </returns>
        public AssetProjectQueryBuilder WithCacheConfiguration(AssetProjectCacheConfiguration assetProjectCacheConfiguration)
        {
            m_AssetProjectCacheConfiguration = assetProjectCacheConfiguration;
            return this;
        }

        /// <summary>
        /// Sets the range of results to return.
        /// </summary>
        /// <param name="range">The range of results. </param>
        /// <returns>The calling <see cref="AssetProjectQueryBuilder"/>. </returns>
        public AssetProjectQueryBuilder LimitTo(Range range)
        {
            m_Range = range;
            return this;
        }

        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IAssetProject"/>. </returns>
        public async IAsyncEnumerable<IAssetProject> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var pagination = new PaginationData
            {
                Range = m_Range
            };
            var results = m_AssetDataSource.ListProjectsAsync(m_OrganizationId, pagination, cancellationToken);
            await foreach(var project in results)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return project.From(m_AssetDataSource, m_DefaultCacheConfiguration, m_OrganizationId, m_AssetProjectCacheConfiguration);
            }
        }
    }
}
