using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static class AssetExtensions
    {
        /// <summary>
        /// Returns the latest version of the asset.
        /// </summary>
        /// <param name="asset">The asset to query. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IAsset"/>. </returns>
        public static Task<IAsset> WithLatestVersionAsync(this IAsset asset, CancellationToken cancellationToken)
        {
            return asset.WithVersionAsync("Latest", cancellationToken);
        }

        /// <summary>
        /// Returns the version of the asset with the specified sequence number.
        /// </summary>
        /// <param name="asset">The asset to query. </param>
        /// <param name="frozenSequenceNumber">The sequence number of the version of the asset to fetch. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <exception cref="NotFoundException">If a version with the corresponding <paramref name="frozenSequenceNumber"/> is not found. </exception>
        /// <returns>A task whose result is the <see cref="IAsset"/> with the frozen version attributed to the specified sequence number. </returns>
        public static async Task<IAsset> WithVersionAsync(this IAsset asset, int frozenSequenceNumber, CancellationToken cancellationToken)
        {
            var filter = new AssetSearchFilter();
            filter.Include().FrozenSequenceNumber.WithValue(frozenSequenceNumber);

#pragma warning disable 618
            var query = asset.QueryVersions()
                .SelectWhereMatchesFilter(filter)
                .LimitTo(new Range(0, 1))
                .ExecuteAsync(cancellationToken);
#pragma warning restore 618

            IAsset version = null;

            var enumerator = query.GetAsyncEnumerator(cancellationToken);
            if (await enumerator.MoveNextAsync())
            {
                version = enumerator.Current;
            }

            await enumerator.DisposeAsync();

            if (version == null)
            {
                throw new NotFoundException($"Version {frozenSequenceNumber} not found for asset {asset.Descriptor.AssetId}");
            }

            return version;
        }

        /// <summary>
        /// Returns the Preview dataset for the asset.
        /// </summary>
        /// <param name="asset">The asset to query. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IDataset"/>. </returns>
        public static Task<IDataset> GetPreviewDatasetAsync(this IAsset asset, CancellationToken cancellationToken)
        {
            const string previewTag = "Preview";
            return GetDatasetAsync(asset, previewTag, cancellationToken);
        }

        /// <summary>
        /// Returns the Source dataset for the asset.
        /// </summary>
        /// <param name="asset">The asset to query. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IDataset"/>. </returns>
        public static Task<IDataset> GetSourceDatasetAsync(this IAsset asset, CancellationToken cancellationToken)
        {
            const string sourceTag = "Source";
            return GetDatasetAsync(asset, sourceTag, cancellationToken);
        }

        /// <summary>
        /// Adds the specified tags to the asset if they are not already present.
        /// </summary>
        /// <param name="asset">An asset. </param>
        /// <param name="tagsToAdd">A set of tags to add. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        public static async Task AddTagsAsync(this IAsset asset, IEnumerable<string> tagsToAdd, CancellationToken cancellationToken)
        {
            await asset.RefreshAsync(cancellationToken);
            var properties = await asset.GetPropertiesAsync(cancellationToken);
            var update = new AssetUpdate
            {
                Tags = properties.Tags.Union(tagsToAdd).ToList()
            };
            await asset.UpdateAsync(update, cancellationToken);
        }

        /// <summary>
        /// Removes all instances of the specified tags from the asset.
        /// </summary>
        /// <param name="asset">An asset. </param>
        /// <param name="tagsToRemove">A set of tags to remove. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        public static async Task RemoveTagsAsync(this IAsset asset, IEnumerable<string> tagsToRemove, CancellationToken cancellationToken)
        {
            await asset.RefreshAsync(cancellationToken);
            var properties = await asset.GetPropertiesAsync(cancellationToken);
            var update = new AssetUpdate
            {
                Tags = properties.Tags.Except(tagsToRemove).ToList()
            };
            await asset.UpdateAsync(update, cancellationToken);
        }

        /// <summary>
        /// Creates a reference between the asset and another asset, where the asset is the source of the reference.
        /// </summary>
        /// <param name="asset">An asset. </param>
        /// <param name="targetAssetDescriptor">The descriptor of the asset which is a target. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the reference between the assets. </returns>
        public static Task<IAssetReference> AddReferenceAsync(this IAsset asset, AssetDescriptor targetAssetDescriptor, CancellationToken cancellationToken)
        {
            return asset.AddReferenceAsync(targetAssetDescriptor.AssetId, targetAssetDescriptor.AssetVersion, cancellationToken);
        }

        static async Task<IDataset> GetDatasetAsync(this IAsset asset, string systemTag, CancellationToken cancellationToken)
        {
            // Get a handle on the original cache configuration of the asset.
            var assetCacheConfiguration = asset.CacheConfiguration;

            // To reduce the number of requests, we check if the asset has the required cache configuration.
            if (!assetCacheConfiguration.DatasetCacheConfiguration.CacheProperties)
            {
                var configuration = new AssetCacheConfiguration
                {
                    CacheDatasetList = true,
                    DatasetCacheConfiguration = new DatasetCacheConfiguration {CacheProperties = true}
                };

                asset = await asset.WithCacheConfigurationAsync(configuration, cancellationToken);
            }

            // This request is synchronous as we have already cached the dataset list.
            await foreach (var dataset in asset.ListDatasetsAsync(Range.All, cancellationToken))
            {
                // This request is synchronous as we have already cached the dataset properties.
                var properties = await dataset.GetPropertiesAsync(cancellationToken);
                if (properties.SystemTags != null && properties.SystemTags.Contains(systemTag))
                {
                    // If we had to update the cache configuration to batch requests, we ensure the dataset is returned with the original configuration.
                    if (!assetCacheConfiguration.DatasetCacheConfiguration.Equals(dataset.CacheConfiguration))
                    {
                        return await dataset.WithCacheConfigurationAsync(assetCacheConfiguration.DatasetCacheConfiguration, cancellationToken);
                    }

                    return dataset;
                }
            }

            return null;
        }

        internal static AssetId SelectId(IAsset asset)
        {
            return asset.Descriptor.AssetId;
        }
    }
}
