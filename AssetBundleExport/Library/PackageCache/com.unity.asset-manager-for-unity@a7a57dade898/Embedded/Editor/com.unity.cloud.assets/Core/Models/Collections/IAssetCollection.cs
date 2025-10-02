using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This object contains the information about an asset collection.
    /// </summary>
    interface IAssetCollection
    {
        /// <summary>
        /// The descriptor of the collection.
        /// </summary>
        CollectionDescriptor Descriptor { get; }

        /// <summary>
        /// The name of the collection.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Describes the collection.
        /// </summary>
        [Obsolete("Use IAssestCollectionProperties.Description instead.")]
        string Description { get; }

        /// <summary>
        /// The path to the parent collection; can be empty.
        /// </summary>
        CollectionPath ParentPath { get; }

        /// <summary>
        /// Returns the full path to the collection.
        /// </summary>
        /// <returns>The path of the collection. </returns>
        [Obsolete("Use Descriptor.Path instead.")]
        string GetFullCollectionPath();

        /// <summary>
        /// The caching configuration for the collection.
        /// </summary>
        AssetCollectionCacheConfiguration CacheConfiguration => throw new NotImplementedException();

        /// <summary>
        /// Returns an asset collection configured with the specified caching configuration.
        /// </summary>
        /// <param name="assetCollectionCacheConfiguration">The caching configuration for the collection. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IAssetCollection"/> with cached values specified by the caching configurations. </returns>
        Task<IAssetCollection> WithCacheConfigurationAsync(AssetCollectionCacheConfiguration assetCollectionCacheConfiguration, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Refreshes the collection properties.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task RefreshAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the properties of the collection.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the <see cref="AssetCollectionProperties"/> of the collection. </returns>
        Task<AssetCollectionProperties> GetPropertiesAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Updates the collection.
        /// </summary>
        /// <param name="assetCollectionUpdate">The object containing the collection information to update. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task UpdateAsync(IAssetCollectionUpdate assetCollectionUpdate, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a set of asset references to the collection.
        /// </summary>
        /// <param name="assets">The assets to link to the collection. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task LinkAssetsAsync(IEnumerable<IAsset> assets, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a set of asset references to the collection.
        /// </summary>
        /// <param name="assetIds">The ids of the assets to link to the collection. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task LinkAssetsAsync(IEnumerable<AssetId> assetIds, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Removes a set of asset references from the collection.
        /// </summary>
        /// <param name="assets">The assets to unlink from the collection. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task UnlinkAssetsAsync(IEnumerable<IAsset> assets, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a set of asset references from the collection.
        /// </summary>
        /// <param name="assetIds">The ids of the assets to unlink from the collection. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task UnlinkAssetsAsync(IEnumerable<AssetId> assetIds, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Creates a new path for the collection.
        /// </summary>
        /// <param name="newCollectionPath">The new parent path. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task MoveToNewPathAsync(CollectionPath newCollectionPath, CancellationToken cancellationToken);
    }
}
