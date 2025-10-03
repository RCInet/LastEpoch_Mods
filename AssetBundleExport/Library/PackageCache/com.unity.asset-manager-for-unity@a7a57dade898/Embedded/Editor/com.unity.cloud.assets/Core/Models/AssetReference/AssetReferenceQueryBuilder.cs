using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that builds and executes a query to return a set of references to an asset.
    /// </summary>
    sealed class AssetReferenceQueryBuilder
    {
        readonly IAssetDataSource m_DataSource;
        readonly ProjectDescriptor m_ProjectDescriptor;
        readonly AssetId m_AssetId;

        AssetReferenceSearchFilter m_SearchFilter;
        Range m_Range = Range.All;

        internal AssetReferenceQueryBuilder(IAssetDataSource dataSource, ProjectDescriptor projectDescriptor, AssetId assetId)
        {
            m_DataSource = dataSource;
            m_ProjectDescriptor = projectDescriptor;
            m_AssetId = assetId;
        }

        /// <summary>
        /// Sets the filter to use for the query.
        /// </summary>
        /// <param name="searchFilter">The search criteria. </param>
        /// <returns>The calling <see cref="AssetReferenceQueryBuilder"/>. </returns>
        public AssetReferenceQueryBuilder SelectWhereMatchesFilter(AssetReferenceSearchFilter searchFilter)
        {
            m_SearchFilter = searchFilter;
            return this;
        }

        /// <summary>
        /// Sets the range of results to return.
        /// </summary>
        /// <param name="range">The range of results. </param>
        /// <returns>The calling <see cref="AssetReferenceQueryBuilder"/>. </returns>
        public AssetReferenceQueryBuilder LimitTo(Range range)
        {
            m_Range = range;
            return this;
        }

        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="ITransformation"/>. </returns>
        public async IAsyncEnumerable<IAssetReference> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var filterVersion = m_SearchFilter?.AssetVersion.GetValue();

            // Determine if we are filtering by version
            var assetVersion = filterVersion.HasValue && filterVersion.Value != AssetVersion.None && !string.IsNullOrEmpty(filterVersion.Value.ToString()) ? filterVersion : null;

            // Determine if we are filtering by a specific context, default is both
            var contextString = "Both";
            var context = m_SearchFilter?.ReferenceContext.GetValue() ?? AssetReferenceSearchFilter.Context.Both;
            if (context is AssetReferenceSearchFilter.Context.Source or AssetReferenceSearchFilter.Context.Target)
            {
                contextString = context.ToString();
            }

            var data = m_DataSource.ListAssetReferencesAsync(m_ProjectDescriptor, m_AssetId, assetVersion, contextString, m_Range, cancellationToken);
            await foreach (var referenceData in data)
            {
                yield return new AssetReference(m_ProjectDescriptor, referenceData.ReferenceId)
                {
                    IsValid = referenceData.IsValid,
                    SourceAssetId = new AssetId(referenceData.Source.Id),
                    SourceAssetVersion =  new AssetVersion(referenceData.Source.Version),
                    TargetAssetId = new AssetId(referenceData.Target.Id),
                    TargetAssetVersion = string.IsNullOrWhiteSpace(referenceData.Target.Version) ? null : new AssetVersion(referenceData.Target.Version),
                    TargetLabel = string.IsNullOrEmpty(referenceData.Target.Label) ? null : referenceData.Target.Label
                };
            }
        }
    }
}
