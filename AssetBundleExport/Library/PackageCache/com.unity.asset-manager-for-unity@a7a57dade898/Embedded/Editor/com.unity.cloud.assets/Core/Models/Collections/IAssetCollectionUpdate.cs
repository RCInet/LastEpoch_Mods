namespace Unity.Cloud.AssetsEmbedded
{
    interface IAssetCollectionUpdate
    {
        /// <inheritdoc cref="IAssetCollection.Name"/>
        string Name { get; }

        /// <inheritdoc cref="IAssetCollection.Description"/>
        string Description { get; }
    }
}
