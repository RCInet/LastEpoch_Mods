using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IAsset"/> search request.
    /// </summary>
    class AssetSearchFilter : IAssetSearchFilter
    {
        readonly AssetSearchCriteria m_Include = new();
        readonly AssetSearchCriteria m_Exclude = new();
        readonly AssetSearchCriteriaWithMinimumMatch m_Any = new();

        /// <inheritdoc />
        public QueryListParameter<CollectionPath> Collections { get; } = new();

        /// <summary>
        /// All properties populated here will be considered for exact matching when searching for assets.
        /// </summary>
        public AssetSearchCriteria Include() => m_Include;

        /// <summary>
        /// All properties populated here will be considered for exact matching when excluding assets.
        /// </summary>
        public AssetSearchCriteria Exclude() => m_Exclude;

        /// <summary>
        /// Any properties populated here will be considered when searching for assets.
        /// The minimum number of matches required can be set using <see cref="AssetSearchCriteriaWithMinimumMatch.MinimumMatch"/>.
        /// </summary>
        public AssetSearchCriteriaWithMinimumMatch Any() => m_Any;

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object> AccumulateIncludedCriteria()
        {
            var criteria = new Dictionary<string, object>();

            m_Include.Include(criteria);

            return criteria.Count > 0 ? criteria : null;
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object> AccumulateExcludedCriteria()
        {
            var criteria = new Dictionary<string, object>();

            m_Exclude.Include(criteria);

            return criteria.Count > 0 ? criteria : null;
        }

        /// <inheritdoc/>
        public (IReadOnlyDictionary<string, object> criteria, int minimumMatches) AccumulateAnyCriteria()
        {
            var criteria = new Dictionary<string, object>();

            m_Any.Include(criteria);

            return (criteria.Count > 0 ? criteria : null, m_Any.MinimumMatch);
        }
    }
}
