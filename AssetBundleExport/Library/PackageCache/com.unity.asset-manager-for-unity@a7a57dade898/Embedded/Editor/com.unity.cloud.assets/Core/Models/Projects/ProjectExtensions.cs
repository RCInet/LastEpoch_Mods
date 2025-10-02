using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static class ProjectExtensions
    {
        /// <summary>
        /// Returns the latest version of the asset.
        /// </summary>
        /// <param name="assetProject">The project to query. </param>
        /// <param name="assetId">The id of the asset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IAsset"/>. </returns>
        public static Task<IAsset> GetAssetWithLatestVersionAsync(this IAssetProject assetProject, AssetId assetId, CancellationToken cancellationToken)
        {
            return assetProject.GetAssetAsync(assetId, "Latest", cancellationToken);
        }

        /// <summary>
        /// Links the assets to the project.
        /// </summary>
        /// <param name="assetProject">The target project. </param>
        /// <param name="sourceProjectDescriptor">The id of the project that is common to the assets. </param>
        /// <param name="assets">The assets to link. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        public static Task LinkAssetsAsync(this IAssetProject assetProject, ProjectDescriptor sourceProjectDescriptor, IEnumerable<IAsset> assets, CancellationToken cancellationToken)
        {
            return assetProject.LinkAssetsAsync(sourceProjectDescriptor, assets.Select(AssetExtensions.SelectId), cancellationToken);
        }

        /// <summary>
        /// Unlinks the assets from the project.
        /// </summary>
        /// <param name="assetProject">The target project. </param>
        /// <param name="assets">The assets to unlink from the project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        public static Task UnlinkAssetsAsync(this IAssetProject assetProject, IEnumerable<IAsset> assets, CancellationToken cancellationToken)
        {
            return assetProject.UnlinkAssetsAsync(assets.Select(AssetExtensions.SelectId), cancellationToken);
        }

        /// <summary>
        /// Returns the total count of assets in the specified projects based on the provided criteria.
        /// </summary>
        /// <param name="assetProject">The <see cref="IAssetProject"/>. </param>
        /// <param name="assetSearchFilter">The filter specifying the search criteria. Can be null. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an asset count. </returns>
        public static async Task<int> CountAssetsAsync(this IAssetProject assetProject, [AllowNull] IAssetSearchFilter assetSearchFilter, CancellationToken cancellationToken)
        {
            var count = 0;
            var asyncEnumerable = assetProject.GroupAndCountAssets()
                .SelectWhereMatchesFilter(assetSearchFilter)
                .LimitTo(int.MaxValue).ExecuteAsync((Groupable) GroupableField.Type, cancellationToken);
            await foreach (var kvp in asyncEnumerable)
            {
                count += kvp.Value;
            }

            return count;
        }

        /// <summary>
        /// Returns the collections of the project.
        /// </summary>
        /// <param name="assetProject">The <see cref="IAssetProject"/>. </param>
        /// <param name="range">The range of results to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns></returns>
        public static IAsyncEnumerable<IAssetCollection> ListCollectionsAsync(this IAssetProject assetProject, Range range, CancellationToken cancellationToken)
        {
            return assetProject.QueryCollections().LimitTo(range).ExecuteAsync(cancellationToken);
        }
    }
}
