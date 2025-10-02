using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IAsset"/> search request.
    /// </summary>
    class AssetSearchCriteriaWithMinimumMatch : AssetSearchCriteria
    {
        internal int MinimumMatch { get; private set; } = 1;

        public void WhereMinimumMatchEquals(int minimumMatch)
        {
            MinimumMatch = minimumMatch;
        }
    }
}
