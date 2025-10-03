namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Sets the cache configuration for a project.
    /// </summary>
    struct AssetProjectCacheConfiguration
    {
        /// <summary>
        /// Whether to cache properties of the asset project.
        /// </summary>
        public bool CacheProperties { get; set; }

        public static AssetProjectCacheConfiguration NoCaching => new()
        {
            CacheProperties = false,
        };

        internal static AssetProjectCacheConfiguration Legacy => new()
        {
            CacheProperties = true,
        };

        internal bool HasCachingRequirements => CacheProperties;

        internal AssetProjectCacheConfiguration(AssetRepositoryCacheConfiguration defaultCacheConfiguration)
        {
            CacheProperties = defaultCacheConfiguration.AssetProjectCacheConfiguration.CacheProperties;
        }
    }
}
