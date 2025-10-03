using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Implement this interface to transform user facing data like <see cref="IAssetData"/> into service DTOs
    /// </summary>
    partial interface IAssetDataSource
    {
        /// <summary>
        /// Gets the asset collections.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>An enumeration of collections. </returns>
        Task<IEnumerable<IAssetCollectionData>> GetAssetCollectionsAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the collections in a project.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="range">The range of the collections to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is an async enumeration of <see cref="IAssetCollectionData"/>. </returns>
        IAsyncEnumerable<IAssetCollectionData> ListCollectionsAsync(ProjectDescriptor projectDescriptor, Range range, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the collection at the specified path.
        /// </summary>
        /// <param name="collectionDescriptor">The object containing the necessary information to identify the collection. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is an <see cref="IAssetCollectionData"/>. </returns>
        Task<IAssetCollectionData> GetCollectionAsync(CollectionDescriptor collectionDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new collection within a project.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="assetCollection">The <see cref="IAssetCollectionData"/> to push to the cloud. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is the path to the collection. </returns>
        Task<CollectionPath> CreateCollectionAsync(ProjectDescriptor projectDescriptor, IAssetCollectionData assetCollection, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a collection in a project.
        /// </summary>
        /// <param name="collectionDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="assetCollection">The <see cref="IAssetCollectionData"/> to push to the cloud. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result.</returns>
        Task UpdateCollectionAsync(CollectionDescriptor collectionDescriptor, IAssetCollectionData assetCollection, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a collection from a project.
        /// </summary>
        /// <param name="collectionDescriptor">The object containing the necessary information to identify the collection. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result.</returns>
        Task DeleteCollectionAsync(CollectionDescriptor collectionDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Adds assets to a collection in a project.
        /// </summary>
        /// <param name="collectionDescriptor">The object containing the necessary information to identify the collection. </param>
        /// <param name="assets">List of asset ids to add to the collection</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns></returns>
        Task AddAssetsToCollectionAsync(CollectionDescriptor collectionDescriptor, IEnumerable<AssetId> assets, CancellationToken cancellationToken);

        /// <summary>
        /// Removes assets from a collection in a project.
        /// </summary>
        /// <param name="collectionDescriptor">The object containing the necessary information to identify the collection. </param>
        /// <param name="assets"></param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns></returns>
        Task RemoveAssetsFromCollectionAsync(CollectionDescriptor collectionDescriptor, IEnumerable<AssetId> assets, CancellationToken cancellationToken);

        /// <summary>
        /// Moves a collection in a project to a new path.
        /// </summary>
        /// <param name="collectionDescriptor">The object containing the necessary information to identify the collection. </param>
        /// <param name="newCollectionPath"></param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns></returns>
        Task<CollectionPath> MoveCollectionToNewPathAsync(CollectionDescriptor collectionDescriptor, CollectionPath newCollectionPath, CancellationToken cancellationToken);
    }
}
