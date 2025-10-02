namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Sets the cache configuration for a label.
    /// </summary>
    struct LabelCacheConfiguration
    {
        /// <summary>
        /// Whether to cache the properties of the label.
        /// </summary>
        public bool CacheProperties { get; set; }

        public static LabelCacheConfiguration NoCaching => new()
        {
            CacheProperties = false,
        };

        internal static LabelCacheConfiguration Legacy => new()
        {
            CacheProperties = true,
        };

        internal bool HasCachingRequirements => CacheProperties;

        internal LabelCacheConfiguration(AssetRepositoryCacheConfiguration defaultCacheConfiguration)
        {
            CacheProperties = defaultCacheConfiguration.LabelCacheConfiguration.CacheProperties;
        }
    }
}
