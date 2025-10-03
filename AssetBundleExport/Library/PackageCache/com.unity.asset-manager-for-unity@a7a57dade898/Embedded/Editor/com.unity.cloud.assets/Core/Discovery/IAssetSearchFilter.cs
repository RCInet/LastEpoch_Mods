using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A structure that defines the criteria of an <see cref="IAsset"/> search query.
    /// </summary>
    interface IAssetSearchFilter
    {
        /// <summary>
        /// Sets the collections the search criteria will be applied to.
        /// </summary>
        QueryListParameter<CollectionPath> Collections => new();

        /// <summary>
        /// Gets the required search criteria of the filter.
        /// </summary>
        /// <returns>A dictionary containing the required search criteria. </returns>
        IReadOnlyDictionary<string, object> AccumulateIncludedCriteria();

        /// <summary>
        /// Gets the excluded search criteria of the filter.
        /// </summary>
        /// <returns>A dictionary containing the excluded search criteria. </returns>
        IReadOnlyDictionary<string, object> AccumulateExcludedCriteria();

        /// <summary>
        /// Gets the optional search criteria of the filter.
        /// </summary>
        /// <returns>A dictionary containing the optional search criteria and the minimum matches required to statisfy the condition. </returns>
        (IReadOnlyDictionary<string, object> criteria, int minimumMatches) AccumulateAnyCriteria();
    }
}
