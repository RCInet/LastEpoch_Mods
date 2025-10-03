using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this AssetCollection collection, IAssetCollectionData collectionData)
        {
            if (collection.CacheConfiguration.CacheProperties)
                collection.Properties = collectionData.From();
        }

        internal static AssetCollectionProperties From(this IAssetCollectionData data)
        {
            return new AssetCollectionProperties
            {
                Description = data.Description,
            };
        }

        internal static AssetCollection From(this IAssetCollectionData data, IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            ProjectDescriptor projectDescriptor, AssetCollectionCacheConfiguration? cacheConfigurationOverride = null)
        {
            return data.From(dataSource, defaultCacheConfiguration, new CollectionDescriptor(projectDescriptor, data.GetFullCollectionPath()), cacheConfigurationOverride);
        }

        internal static AssetCollection From(this IAssetCollectionData data, IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            CollectionDescriptor collectionDescriptor, AssetCollectionCacheConfiguration? cacheConfigurationOverride = null)
        {
            var collection = new AssetCollection(dataSource, defaultCacheConfiguration, collectionDescriptor, cacheConfigurationOverride);
            collection.MapFrom(data);
            return collection;
        }

        internal static IAssetCollectionData From(this IAssetCollectionCreation assetCollection)
        {
            return new AssetCollectionData(assetCollection.Name, assetCollection.ParentPath)
            {
                Description = assetCollection.Description,
            };
        }

        internal static IAssetCollectionData From(this IAssetCollectionUpdate assetCollectionUpdate)
        {
            Validate("not null", assetCollectionUpdate.Description);

            return new AssetCollectionData(assetCollectionUpdate.Name)
            {
                Description = assetCollectionUpdate.Description,
            };
        }

        internal static void Validate(this IAssetCollectionCreation assetCollectionCreation) => Validate(assetCollectionCreation.Name, assetCollectionCreation.Description);

        static void Validate(string name, string description)
        {
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(IAssetCollectionCreation.Name), "The name of the collection cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(IAssetCollectionCreation.Description), "The description of the collection cannot be null or empty.");
            }
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one
        }
    }
}
