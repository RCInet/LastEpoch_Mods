namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This object contains the information about an asset collection.
    /// </summary>
    interface IAssetCollectionCreation
    {
        /// <inheritdoc cref="IAssetCollection.Name"/>
        string Name { get; }

        /// <inheritdoc cref="IAssetCollection.Description"/>
        string Description { get; }

        /// <inheritdoc cref="IAssetCollection.ParentPath"/>
        CollectionPath ParentPath { get; set; }
    }
}
