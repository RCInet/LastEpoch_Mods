using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that builds and executes a query to return a set of transformations.
    /// </summary>
    class TransformationQueryBuilder
    {
        readonly IAssetDataSource m_DataSource;
        readonly AssetRepositoryCacheConfiguration m_DefaultCacheConfiguration;
        readonly ProjectDescriptor m_ProjectDescriptor;

        TransformationCacheConfiguration? m_TransformationCacheConfiguration;
        TransformationSearchFilter m_SearchFilter;
        Range m_Range = Range.All;

        internal TransformationQueryBuilder(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, ProjectDescriptor projectDescriptor)
        {
            m_DataSource = dataSource;
            m_DefaultCacheConfiguration = defaultCacheConfiguration;
            m_ProjectDescriptor = projectDescriptor;
        }

        /// <summary>
        /// Sets an override to the default cache configuration for the query.
        /// </summary>
        /// <param name="transformationCacheConfiguration">The configuration to apply when populating the transformations. </param>
        /// <returns>The calling <see cref="TransformationQueryBuilder"/>. </returns>
        public TransformationQueryBuilder WithCacheConfiguration(TransformationCacheConfiguration transformationCacheConfiguration)
        {
            m_TransformationCacheConfiguration = transformationCacheConfiguration;
            return this;
        }

        /// <summary>
        /// Sets the filter to use for the query.
        /// </summary>
        /// <param name="searchFilter">The search criteria. </param>
        /// <returns>The calling <see cref="TransformationQueryBuilder"/>. </returns>
        public TransformationQueryBuilder SelectWhereMatchesFilter(TransformationSearchFilter searchFilter)
        {
            m_SearchFilter = searchFilter;
            return this;
        }

        /// <summary>
        /// Sets the range of results to return.
        /// </summary>
        /// <param name="range">The range of results. </param>
        /// <returns>The calling <see cref="TransformationQueryBuilder"/>. </returns>
        public TransformationQueryBuilder LimitTo(Range range)
        {
            m_Range = range;
            return this;
        }

        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="ITransformation"/>. </returns>
        public async IAsyncEnumerable<ITransformation> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            m_SearchFilter ??= new TransformationSearchFilter();
            var data = new TransformationSearchData
            {
                AssetId = m_SearchFilter.AssetId.GetValue() == AssetId.None ? null : m_SearchFilter.AssetId.GetValue().ToString(),
                AssetVersion = m_SearchFilter.AssetVersion.GetValue() == AssetVersion.None ? null : m_SearchFilter.AssetVersion.GetValue().ToString(),
                DatasetId = m_SearchFilter.DatasetId.GetValue() == DatasetId.None ? null : m_SearchFilter.DatasetId.GetValue().ToString(),
                Status = m_SearchFilter.Status.GetValue() == null ? null : m_SearchFilter.Status.GetValue(),
                UserId = m_SearchFilter.UserId.GetValue() == UserId.None ? null : m_SearchFilter.UserId.GetValue().ToString()
            };

            var (start, length) = m_Range.GetValidatedOffsetAndLength(int.MaxValue);

            if (length == 0) yield break;

            var results = await m_DataSource.GetTransformationsAsync(m_ProjectDescriptor, data, cancellationToken);
            for (var i = start; i < results.Length && i < start + length; ++i)
            {
                if (cancellationToken.IsCancellationRequested) yield break;

                yield return results[i].From(m_DataSource, m_DefaultCacheConfiguration, m_ProjectDescriptor, m_TransformationCacheConfiguration);
            }
        }
    }
}
