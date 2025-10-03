using System;

namespace Unity.Cloud.AssetsEmbedded
{
    struct DatasetCacheConfiguration
    {
        /// <summary>
        /// Whether to cache the properties of the dataset.
        /// </summary>
        public bool CacheProperties { get; set; }

        /// <summary>
        /// Whether to cache the file list of the dataset.
        /// </summary>
        public bool CacheFileList { get; set; }

        /// <summary>
        /// Whether to cache the metadata of the dataset.
        /// </summary>
        public bool CacheMetadata { get; set; }

        /// <summary>
        /// Whether to cache the system metadata of the dataset.
        /// </summary>
        public bool CacheSystemMetadata { get; set; }

        /// <summary>
        /// Defines the cache configuration for files retrieved from the `IDataset`.
        /// </summary>
        public FileCacheConfiguration FileCacheConfiguration { get; set; }

        bool Equals(DatasetCacheConfiguration other)
        {
            if (CacheFileList && !FileCacheConfiguration.Equals(other.FileCacheConfiguration)) return false;

            return CacheProperties == other.CacheProperties
                && CacheMetadata == other.CacheMetadata
                && CacheSystemMetadata == other.CacheSystemMetadata
                && CacheFileList == other.CacheFileList;
        }

        public override bool Equals(object obj)
        {
            return obj is DatasetCacheConfiguration other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CacheProperties, CacheFileList, CacheMetadata, CacheSystemMetadata, FileCacheConfiguration);
        }

        public static DatasetCacheConfiguration NoCaching => new()
        {
            CacheProperties = false,
            CacheFileList = false,
            CacheMetadata = false,
            CacheSystemMetadata = false,
            FileCacheConfiguration = FileCacheConfiguration.NoCaching
        };

        internal static DatasetCacheConfiguration Legacy => new()
        {
            CacheProperties = true,
            CacheFileList = false,
            CacheMetadata = false,
            CacheSystemMetadata = false,
            FileCacheConfiguration = FileCacheConfiguration.Legacy
        };

        internal bool HasCachingRequirements => CacheProperties || CacheFileList || CacheMetadata || CacheSystemMetadata;

        internal DatasetCacheConfiguration(AssetCacheConfiguration defaultCacheConfiguration)
        {
            CacheProperties = defaultCacheConfiguration.DatasetCacheConfiguration.CacheProperties;
            CacheFileList = defaultCacheConfiguration.DatasetCacheConfiguration.CacheFileList;
            CacheMetadata = defaultCacheConfiguration.DatasetCacheConfiguration.CacheMetadata;
            CacheSystemMetadata = defaultCacheConfiguration.DatasetCacheConfiguration.CacheSystemMetadata;
            FileCacheConfiguration = new FileCacheConfiguration(defaultCacheConfiguration.DatasetCacheConfiguration);
        }
    }
}
