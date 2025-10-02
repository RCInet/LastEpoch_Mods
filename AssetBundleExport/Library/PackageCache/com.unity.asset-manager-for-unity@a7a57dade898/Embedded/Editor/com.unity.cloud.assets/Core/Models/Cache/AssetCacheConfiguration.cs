using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Sets the cache configuration for an asset.
    /// </summary>
    struct AssetCacheConfiguration
    {
        /// <summary>
        /// Whether to cache the properties of the asset.
        /// </summary>
        public bool CacheProperties { get; set; }

        /// <summary>
        /// Whether to cache the preview URL of the asset.
        /// </summary>
        public bool CachePreviewUrl { get; set; }

        /// <summary>
        /// Whether to cache the list of datasets of the asset.
        /// </summary>
        public bool CacheDatasetList { get; set; }

        /// <summary>
        /// Whether to cache the metadata of the asset.
        /// </summary>
        public bool CacheMetadata { get; set; }

        /// <summary>
        /// Whether to cache the system metadata of the asset.
        /// </summary>
        public bool CacheSystemMetadata { get; set; }

        /// <summary>
        /// Which subset of metadata field keys to cache. Will apply to Asset, Dataset, and File Metadata.
        /// </summary>
        public IEnumerable<string> CacheMetadataFieldKeys { get; set; }

        /// <summary>
        /// Which subset of system metadata field keys to cache. Will apply to Asset, Dataset, and File System Metadata.
        /// </summary>
        public IEnumerable<string> CacheSystemMetadataFieldKeys { get; set; }

        /// <summary>
        /// Defines the cache configuration for datasets retrieved from the `IAsset`.
        /// </summary>
        public DatasetCacheConfiguration DatasetCacheConfiguration { get; set; }

        bool HasMetadataCachingRequirements => CacheMetadata || DatasetCacheConfiguration.CacheMetadata || DatasetCacheConfiguration.FileCacheConfiguration.CacheMetadata;
        bool HasSystemMetadataCachingRequirements => CacheSystemMetadata || DatasetCacheConfiguration.CacheSystemMetadata || DatasetCacheConfiguration.FileCacheConfiguration.CacheSystemMetadata;

        bool Equals(AssetCacheConfiguration other)
        {
            if (HasMetadataCachingRequirements && !Equals(CacheMetadataFieldKeys, other.CacheMetadataFieldKeys)) return false;
            if (HasSystemMetadataCachingRequirements && !Equals(CacheSystemMetadataFieldKeys, other.CacheSystemMetadataFieldKeys)) return false;
            if (CacheDatasetList && !DatasetCacheConfiguration.Equals(other.DatasetCacheConfiguration)) return false;

            return CacheProperties == other.CacheProperties
                && CachePreviewUrl == other.CachePreviewUrl
                && CacheDatasetList == other.CacheDatasetList
                && CacheMetadata == other.CacheMetadata
                && CacheSystemMetadata == other.CacheSystemMetadata;
        }

        public override bool Equals(object obj)
        {
            return obj is AssetCacheConfiguration other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CacheProperties, CachePreviewUrl, CacheDatasetList, CacheMetadata, CacheSystemMetadata, CacheMetadataFieldKeys, CacheSystemMetadataFieldKeys, DatasetCacheConfiguration);
        }

        public static AssetCacheConfiguration NoCaching => new()
        {
            CacheProperties = false,
            CachePreviewUrl = false,
            CacheDatasetList = false,
            CacheMetadata = false,
            CacheSystemMetadata = false,
            CacheMetadataFieldKeys = Array.Empty<string>(),
            CacheSystemMetadataFieldKeys = Array.Empty<string>(),
            DatasetCacheConfiguration = DatasetCacheConfiguration.NoCaching
        };

        internal static AssetCacheConfiguration Legacy => new()
        {
            CacheProperties = true,
            CachePreviewUrl = false,
            CacheDatasetList = false,
            CacheMetadata = false,
            CacheSystemMetadata = false,
            CacheMetadataFieldKeys = Array.Empty<string>(),
            CacheSystemMetadataFieldKeys = Array.Empty<string>(),
            DatasetCacheConfiguration = DatasetCacheConfiguration.Legacy
        };

        internal bool HasCachingRequirements => CacheProperties || CachePreviewUrl || CacheDatasetList || CacheMetadata || CacheSystemMetadata;

        internal AssetCacheConfiguration(AssetRepositoryCacheConfiguration defaultCacheConfiguration)
        {
            CacheProperties = defaultCacheConfiguration.AssetCacheConfiguration.CacheProperties;
            CachePreviewUrl = defaultCacheConfiguration.AssetCacheConfiguration.CachePreviewUrl;
            CacheDatasetList = defaultCacheConfiguration.AssetCacheConfiguration.CacheDatasetList;
            CacheMetadata = defaultCacheConfiguration.AssetCacheConfiguration.CacheMetadata;
            CacheSystemMetadata = defaultCacheConfiguration.AssetCacheConfiguration.CacheSystemMetadata;
            CacheMetadataFieldKeys = defaultCacheConfiguration.AssetCacheConfiguration.CacheMetadataFieldKeys;
            CacheSystemMetadataFieldKeys = defaultCacheConfiguration.AssetCacheConfiguration.CacheSystemMetadataFieldKeys;
            DatasetCacheConfiguration = new DatasetCacheConfiguration(defaultCacheConfiguration.AssetCacheConfiguration);
        }
    }
}
