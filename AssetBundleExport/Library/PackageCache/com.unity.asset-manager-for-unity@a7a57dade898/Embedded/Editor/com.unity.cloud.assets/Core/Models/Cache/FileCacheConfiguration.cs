using System;

namespace Unity.Cloud.AssetsEmbedded
{
    struct FileCacheConfiguration
    {
        /// <summary>
        /// Whether to cache the properties of the file.
        /// </summary>
        public bool CacheProperties { get; set; }

        /// <summary>
        /// Whether to cache the download URL of the file.
        /// </summary>
        public bool CacheDownloadUrl { get; set; }

        /// <summary>
        /// Whether to cache the preview URL of the file.
        /// </summary>
        public bool CachePreviewUrl { get; set; }

        /// <summary>
        /// Whether to cache the metadata of the file.
        /// </summary>
        public bool CacheMetadata { get; set; }

        /// <summary>
        /// Whether to cache the system metadata of the file.
        /// </summary>
        public bool CacheSystemMetadata { get; set; }

        bool Equals(FileCacheConfiguration other)
        {
            return CacheProperties == other.CacheProperties
                && CacheDownloadUrl == other.CacheDownloadUrl
                && CachePreviewUrl == other.CachePreviewUrl
                && CacheMetadata == other.CacheMetadata
                && CacheSystemMetadata == other.CacheSystemMetadata;
        }

        public override bool Equals(object obj)
        {
            return obj is FileCacheConfiguration other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CacheProperties, CacheDownloadUrl, CachePreviewUrl, CacheMetadata, CacheSystemMetadata);
        }

        public static FileCacheConfiguration NoCaching => new()
        {
            CacheProperties = false,
            CacheDownloadUrl = false,
            CachePreviewUrl = false,
            CacheMetadata = false,
            CacheSystemMetadata = false
        };

        internal static FileCacheConfiguration Legacy => new()
        {
            CacheProperties = true,
            CacheDownloadUrl = false,
            CachePreviewUrl = false,
            CacheMetadata = false,
            CacheSystemMetadata = false
        };

        internal bool HasCachingRequirements => CacheProperties || CacheDownloadUrl || CachePreviewUrl || CacheMetadata || CacheSystemMetadata;

        internal FileCacheConfiguration(DatasetCacheConfiguration defaultCacheConfiguration)
        {
            CacheProperties = defaultCacheConfiguration.FileCacheConfiguration.CacheProperties;
            CacheDownloadUrl = defaultCacheConfiguration.FileCacheConfiguration.CacheDownloadUrl;
            CachePreviewUrl = defaultCacheConfiguration.FileCacheConfiguration.CachePreviewUrl;
            CacheMetadata = defaultCacheConfiguration.FileCacheConfiguration.CacheMetadata;
            CacheSystemMetadata = defaultCacheConfiguration.FileCacheConfiguration.CacheSystemMetadata;
        }
    }
}
