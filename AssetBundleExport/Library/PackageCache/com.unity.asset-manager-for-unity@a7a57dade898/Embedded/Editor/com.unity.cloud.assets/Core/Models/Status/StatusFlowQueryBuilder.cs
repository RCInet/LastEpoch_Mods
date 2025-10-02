using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that builds and executes a query to return a set of statuses.
    /// </summary>
    sealed class StatusFlowQueryBuilder
    {
        readonly IAssetDataSource m_DataSource;
        readonly OrganizationId m_OrganizationId;

        Range m_Range = Range.All;

        internal StatusFlowQueryBuilder(IAssetDataSource dataSource, OrganizationId organizationId)
        {
            m_DataSource = dataSource;
            m_OrganizationId = organizationId;
        }

        /// <summary>
        /// Sets the range of results to return.
        /// </summary>
        /// <param name="range">The range of results. </param>
        /// <returns>The calling <see cref="LabelQueryBuilder"/>. </returns>
        public StatusFlowQueryBuilder LimitTo(Range range)
        {
            m_Range = range;
            return this;
        }

        /// <summary>
        /// Executes the query and returns the status flows that satisfy the criteria.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IStatusFlow"/>. </returns>
        public async IAsyncEnumerable<IStatusFlow> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var pagination = new PaginationData
            {
                Range = m_Range
            };

            var results = m_DataSource.ListStatusFlowsAsync(m_OrganizationId, pagination, cancellationToken);
            await foreach (var statusFlow in results)
            {
                yield return statusFlow.From(m_OrganizationId);
            }
        }
    }
}
