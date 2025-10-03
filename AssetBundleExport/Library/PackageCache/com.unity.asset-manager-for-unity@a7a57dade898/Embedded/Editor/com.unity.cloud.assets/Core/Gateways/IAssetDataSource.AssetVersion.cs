using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial interface IAssetDataSource
    {
        /// <summary>
        /// Retrieves the asset versions for a given asset given the criteria.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project the asset belongs to.</param>
        /// <param name="assetId">The asset to search in. </param>
        /// <param name="parameters">The search parameters. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>An async enumeration of assets that satisfy the criteria. </returns>
        IAsyncEnumerable<IAssetData> ListAssetVersionsAsync(ProjectDescriptor projectDescriptor, AssetId assetId, SearchRequestParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Creates an asset version.
        /// </summary>
        /// <param name="parentAssetDescriptor">The descriptor of the asset from which to branch off a new version.</param>
        /// <param name="statusFlowId">The status flow to apply to the new version. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a new version id of the asset. </returns>
        Task<AssetVersion> CreateUnfrozenAssetVersionAsync(AssetDescriptor parentAssetDescriptor, string statusFlowId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an asset version.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result.</returns>
        Task DeleteUnfrozenAssetVersionAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Submits an asset version to be frozen.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="changeLog">An optional change log describing the version changes. </param>
        /// <param name="forceFreeze">Whether ongoing transformations should be cancelled. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is the new sequence id of the asset version. </returns>
        Task<int?> FreezeAssetVersionAsync(AssetDescriptor assetDescriptor, string changeLog, bool? forceFreeze, CancellationToken cancellationToken);

        /// <summary>
        /// Cancels the freeze of an asset version.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result.</returns>
        Task CancelFreezeAssetVersionAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken);
    }
}
