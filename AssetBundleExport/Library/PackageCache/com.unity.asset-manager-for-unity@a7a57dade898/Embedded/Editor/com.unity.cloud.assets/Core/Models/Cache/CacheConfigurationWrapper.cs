namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Internal helper class for Asset, Dataset and File cache configuration.
    /// </summary>
    sealed class CacheConfigurationWrapper
    {
        public AssetRepositoryCacheConfiguration DefaultConfiguration { get; }
        public AssetCacheConfiguration AssetConfiguration { get; private set; }
        public DatasetCacheConfiguration DatasetConfiguration { get; private set; }
        public FileCacheConfiguration FileConfiguration { get; private set; }

        public CacheConfigurationWrapper(AssetRepositoryCacheConfiguration defaultConfiguration)
        {
            DefaultConfiguration = defaultConfiguration;
            SetAssetConfiguration(DefaultConfiguration.AssetCacheConfiguration);
        }

        public void SetAssetConfiguration(AssetCacheConfiguration? configuration)
        {
            configuration ??= new AssetCacheConfiguration(DefaultConfiguration);
            AssetConfiguration = configuration.Value;
            SetDatasetConfiguration(AssetConfiguration.DatasetCacheConfiguration);
        }

        public void SetDatasetConfiguration(DatasetCacheConfiguration? configuration)
        {
            configuration ??= new DatasetCacheConfiguration(DefaultConfiguration.AssetCacheConfiguration);
            DatasetConfiguration = configuration.Value;
            SetFileConfiguration(DatasetConfiguration.FileCacheConfiguration);
        }

        public void SetFileConfiguration(FileCacheConfiguration? configuration)
        {
            configuration ??= new FileCacheConfiguration(DefaultConfiguration.AssetCacheConfiguration.DatasetCacheConfiguration);
            FileConfiguration = configuration.Value;
        }

        public FieldsFilter GetAssetFieldsFilter()
        {
            var fieldsFilter = new FieldsFilter().Set(AssetConfiguration, AssetConfiguration.CacheMetadataFieldKeys, AssetConfiguration.CacheSystemMetadataFieldKeys);

            if (AssetConfiguration.CacheDatasetList)
            {
                fieldsFilter.Set(DatasetConfiguration, AssetConfiguration.CacheMetadataFieldKeys, AssetConfiguration.CacheSystemMetadataFieldKeys);
            }

            if (DatasetConfiguration.CacheFileList)
            {
                fieldsFilter.Set(FileConfiguration, AssetConfiguration.CacheMetadataFieldKeys, AssetConfiguration.CacheSystemMetadataFieldKeys);
            }

            return fieldsFilter;
        }

        public FieldsFilter GetDatasetFieldsFilter()
        {
            var fieldsFilter = new FieldsFilter().Set(DatasetConfiguration, AssetConfiguration.CacheMetadataFieldKeys, AssetConfiguration.CacheSystemMetadataFieldKeys);

            if (DatasetConfiguration.CacheFileList)
            {
                fieldsFilter.Set(FileConfiguration, AssetConfiguration.CacheMetadataFieldKeys, AssetConfiguration.CacheSystemMetadataFieldKeys);
            }

            return fieldsFilter;
        }

        public FieldsFilter GetFileFieldsFilter()
        {
            return new FieldsFilter().Set(FileConfiguration, AssetConfiguration.CacheMetadataFieldKeys, AssetConfiguration.CacheSystemMetadataFieldKeys);
        }
    }
}
