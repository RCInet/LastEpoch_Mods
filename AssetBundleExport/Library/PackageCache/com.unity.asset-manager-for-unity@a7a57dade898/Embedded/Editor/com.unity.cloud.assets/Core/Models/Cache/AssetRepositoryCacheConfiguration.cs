using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Sets the cache configuration for all entities of the asset repository.
    /// </summary>
    struct AssetRepositoryCacheConfiguration
    {
        /// <summary>
        /// Defines the cache configuration for field definitions retrieved from the `IAssetRepository`.
        /// </summary>
        public FieldDefinitionCacheConfiguration FieldDefinitionCacheConfiguration { get; set; }

        /// <summary>
        /// Defines the cache configuration for labels retrieved from the `IAssetRepository`.
        /// </summary>
        public LabelCacheConfiguration LabelCacheConfiguration { get; set; }

        /// <summary>
        /// Defines the cache configuration for asset projects retrieved from the `IAssetRepository`.
        /// </summary>
        public AssetProjectCacheConfiguration AssetProjectCacheConfiguration { get; set; }

        /// <summary>
        /// Defines the cache configuration for asset collections retrieved from the `IAssetRepository`.
        /// </summary>
        public AssetCollectionCacheConfiguration AssetCollectionCacheConfiguration { get; set; }

        /// <summary>
        /// Defines the cache configuration for assets retrieved from the `IAssetRepository`.
        /// </summary>
        public AssetCacheConfiguration AssetCacheConfiguration { get; set; }

        /// <summary>
        /// Defines the cache configuration for transformations retrieved from the `IAssetRepository`.
        /// </summary>
        public TransformationCacheConfiguration TransformationCacheConfiguration { get; set; }

        public static AssetRepositoryCacheConfiguration NoCaching => new()
        {
            FieldDefinitionCacheConfiguration = FieldDefinitionCacheConfiguration.NoCaching,
            LabelCacheConfiguration = LabelCacheConfiguration.NoCaching,
            AssetProjectCacheConfiguration = AssetProjectCacheConfiguration.NoCaching,
            AssetCollectionCacheConfiguration = AssetCollectionCacheConfiguration.NoCaching,
            AssetCacheConfiguration = AssetCacheConfiguration.NoCaching,
            TransformationCacheConfiguration = TransformationCacheConfiguration.NoCaching,
        };

        /// <summary>
        /// Setup for non-breaking changes to the cache configuration.
        /// </summary>
        internal static AssetRepositoryCacheConfiguration Legacy => new()
        {
            FieldDefinitionCacheConfiguration = FieldDefinitionCacheConfiguration.Legacy,
            LabelCacheConfiguration = LabelCacheConfiguration.Legacy,
            AssetProjectCacheConfiguration = AssetProjectCacheConfiguration.Legacy,
            AssetCollectionCacheConfiguration = AssetCollectionCacheConfiguration.Legacy,
            AssetCacheConfiguration = AssetCacheConfiguration.Legacy,
            TransformationCacheConfiguration = TransformationCacheConfiguration.Legacy,
        };
    }
}
