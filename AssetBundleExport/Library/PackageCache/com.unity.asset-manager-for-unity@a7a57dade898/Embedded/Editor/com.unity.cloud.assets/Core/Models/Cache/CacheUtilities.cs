using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    static class CacheUtilities
    {
        public static FieldsFilter GetAssetFieldsFilter(this AssetRepositoryCacheConfiguration defaulCacheConfiguration)
        {
            return new CacheConfigurationWrapper(defaulCacheConfiguration).GetAssetFieldsFilter();
        }

        public static FieldsFilter Set(this FieldsFilter fieldsFilter, AssetCacheConfiguration assetCacheConfiguration, IEnumerable<string> metadataFields, IEnumerable<string> systemMetadataFields)
        {
            assetCacheConfiguration.Set(fieldsFilter, metadataFields, systemMetadataFields);
            return fieldsFilter;
        }

        public static FieldsFilter Set(this FieldsFilter fieldsFilter, DatasetCacheConfiguration datasetCacheConfiguration, IEnumerable<string> metadataFields, IEnumerable<string> systemMetadataFields)
        {
            datasetCacheConfiguration.Set(fieldsFilter, metadataFields, systemMetadataFields);
            return fieldsFilter;
        }

        public static FieldsFilter Set(this FieldsFilter fieldsFilter, FileCacheConfiguration fileCacheConfiguration, IEnumerable<string> metadataFields, IEnumerable<string> systemMetadataFields)
        {
            fileCacheConfiguration.Set(fieldsFilter, metadataFields, systemMetadataFields);
            return fieldsFilter;
        }

        static void Set(this AssetCacheConfiguration configuration, FieldsFilter fieldsFilter, IEnumerable<string> metadataFields, IEnumerable<string> systemMetadataFields)
        {
            if (configuration.CacheProperties)
            {
                fieldsFilter.AssetFields = FieldsFilter.DefaultAssetIncludes.AssetFields;
            }

            if (configuration.CacheDatasetList)
            {
                fieldsFilter.AssetFields |= AssetFields.datasets;
            }

            if (configuration.CachePreviewUrl)
            {
                fieldsFilter.AssetFields |= AssetFields.previewFileUrl;
            }

            if (configuration.CacheMetadata)
            {
                fieldsFilter.AssetFields |= AssetFields.metadata;
                fieldsFilter.UnionMetadataFields(metadataFields);
            }

            if (configuration.CacheSystemMetadata)
            {
                fieldsFilter.AssetFields |= AssetFields.systemMetadata;
                fieldsFilter.UnionSystemMetadataFields(systemMetadataFields);
            }
        }

        static void Set(this DatasetCacheConfiguration configuration, FieldsFilter fieldsFilter, IEnumerable<string> metadataFields, IEnumerable<string> systemMetadataFields)
        {
            if (configuration.CacheProperties)
            {
                fieldsFilter.DatasetFields = FieldsFilter.DefaultDatasetIncludes.DatasetFields;
            }

            if (configuration.CacheFileList)
            {
                fieldsFilter.DatasetFields |= DatasetFields.files;
            }

            if (configuration.CacheMetadata)
            {
                fieldsFilter.DatasetFields |= DatasetFields.metadata;
                fieldsFilter.UnionMetadataFields(metadataFields);
            }

            if (configuration.CacheSystemMetadata)
            {
                fieldsFilter.DatasetFields |= DatasetFields.systemMetadata;
                fieldsFilter.UnionSystemMetadataFields(systemMetadataFields);
            }
        }

        static void Set(this FileCacheConfiguration configuration, FieldsFilter fieldsFilter, IEnumerable<string> metadataFields, IEnumerable<string> systemMetadataFields)
        {
            if (configuration.CacheProperties)
            {
                fieldsFilter.FileFields = FieldsFilter.DefaultFileIncludes.FileFields;
            }

            if (configuration.CacheDownloadUrl)
            {
                fieldsFilter.FileFields |= FileFields.downloadURL;
            }

            if (configuration.CachePreviewUrl)
            {
                fieldsFilter.FileFields |= FileFields.previewURL;
            }

            if (configuration.CacheMetadata)
            {
                fieldsFilter.FileFields |= FileFields.metadata;
                fieldsFilter.UnionMetadataFields(metadataFields);
            }

            if (configuration.CacheSystemMetadata)
            {
                fieldsFilter.FileFields |= FileFields.systemMetadata;
                fieldsFilter.UnionSystemMetadataFields(systemMetadataFields);
            }
        }
    }
}
