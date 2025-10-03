using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that builds and executes a query to return a set of labels assigned to an asset.
    /// </summary>
    sealed class AssetLabelQueryBuilder
    {
        readonly IAssetDataSource m_DataSource;
        readonly ProjectDescriptor m_ProjectDescriptor;
        readonly AssetId m_AssetId;

        bool? m_IsArchived;
        Range m_Range = Range.All;

        internal AssetLabelQueryBuilder(IAssetDataSource dataSource, ProjectDescriptor projectDescriptor, AssetId assetId)
        {
            m_DataSource = dataSource;
            m_ProjectDescriptor = projectDescriptor;
            m_AssetId = assetId;
        }

        /// <summary>
        /// Sets the query to return the labels of the given status.
        /// </summary>
        /// <param name="isArchived">Whether the returned labels are archived or not. </param>
        /// <returns>The calling <see cref="AssetLabelQueryBuilder"/>. </returns>
        public AssetLabelQueryBuilder WhereIsArchivedEquals(bool isArchived)
        {
            m_IsArchived = isArchived;
            return this;
        }

        /// <summary>
        /// Sets the range of results to return.
        /// </summary>
        /// <param name="range">The range of results. </param>
        /// <returns>The calling <see cref="TransformationQueryBuilder"/>. </returns>
        public AssetLabelQueryBuilder LimitTo(Range range)
        {
            m_Range = range;
            return this;
        }

        /// <summary>
        /// Excetues the query and returns the list of label names associated to each version of the specified asset.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>An async enumeration of tuples of an <see cref="AssetDescriptor"/> and an enumeration of label names. </returns>
        public async IAsyncEnumerable<(AssetDescriptor, IEnumerable<string>)> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var pagination = new PaginationData
            {
                Range = m_Range
            };

            var results = m_DataSource.ListLabelsAcrossAssetVersions(m_ProjectDescriptor, m_AssetId, pagination, cancellationToken);

            await foreach (var result in results)
            {
                var assetDescriptor = new AssetDescriptor(m_ProjectDescriptor, m_AssetId, new AssetVersion(result.AssetVersion));
                var labelList = new List<string>();

                if (!m_IsArchived.HasValue || !m_IsArchived.Value)
                {
                    labelList.AddRange(result.Labels.Select(x => x.Name));
                }

                if (!m_IsArchived.HasValue || m_IsArchived.Value)
                {
                    labelList.AddRange(result.ArchivedLabels.Select(x => x.Name));
                }

                if (labelList.Count == 0) continue;

                yield return (assetDescriptor, labelList);
            }
        }
    }
}
