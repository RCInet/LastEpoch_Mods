using System;
using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this DatasetEntity dataset, IAssetDataSource assetDataSource, IDatasetData datasetData, DatasetFields includeFields)
        {
            if (dataset.CacheConfiguration.CacheProperties)
                dataset.Properties = datasetData.From(includeFields);

            if (includeFields.HasFlag(DatasetFields.metadata))
                dataset.MetadataEntity.Properties = datasetData.Metadata?.From(assetDataSource, dataset.Descriptor.OrganizationId) ?? new Dictionary<string, MetadataObject>();
            if (includeFields.HasFlag(DatasetFields.systemMetadata))
                dataset.SystemMetadataEntity.Properties = datasetData.SystemMetadata?.From() ?? new Dictionary<string, MetadataObject>();

            dataset.Files.Clear();
            dataset.FileMap.Clear();

            if (includeFields.HasFlag(DatasetFields.files) && datasetData.Files != null)
            {
                foreach (var file in datasetData.Files)
                {
                    dataset.Files.Add(file);
                    dataset.FileMap[file.Path] = file;
                }
            }
        }

        internal static DatasetProperties From(this IDatasetData datasetData, DatasetFields includeFields)
        {
            if (!includeFields.HasFlag(DatasetFields.primaryType) ||
                string.IsNullOrEmpty(datasetData.Type) ||
                !datasetData.Type.TryGetAssetTypeFromString(out var type))
            {
                type = AssetType.Other;
            }
            
            var datasetProperties = new DatasetProperties
            {
                Name = datasetData.Name,
                Type = type,
                Tags = datasetData.Tags ?? Array.Empty<string>(),
                SystemTags = datasetData.SystemTags ?? Array.Empty<string>(),
                StatusName = datasetData.Status,
                IsVisible = datasetData.IsVisible ?? false,
            };

            if (includeFields.HasFlag(DatasetFields.description))
                datasetProperties.Description = datasetData.Description;
            if (includeFields.HasFlag(DatasetFields.authoring))
                datasetProperties.AuthoringInfo = new AuthoringInfo(datasetData.CreatedBy, datasetData.Created, datasetData.UpdatedBy, datasetData.Updated);
            if (includeFields.HasFlag(DatasetFields.filesOrder))
                datasetProperties.FileOrder = datasetData.FileOrder;
            if (includeFields.HasFlag(DatasetFields.workflowName))
                datasetProperties.WorkflowName = datasetData.WorkflowName;

            return datasetProperties;
        }

        internal static DatasetEntity From(this IDatasetData datasetData, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            AssetDescriptor assetDescriptor, DatasetFields includeFields, DatasetCacheConfiguration? cacheConfigurationOverride = null)
        {
            return datasetData.From(assetDataSource, defaultCacheConfiguration, new DatasetDescriptor(assetDescriptor, datasetData.DatasetId), includeFields, cacheConfigurationOverride);
        }

        internal static DatasetEntity From(this IDatasetData datasetData, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            DatasetDescriptor datasetDescriptor, DatasetFields includeFields, DatasetCacheConfiguration? cacheConfigurationOverride = null)
        {
            var dataset = new DatasetEntity(assetDataSource, defaultCacheConfiguration, datasetDescriptor, cacheConfigurationOverride);
            dataset.MapFrom(assetDataSource, datasetData, includeFields);
            return dataset;
        }

        internal static IDatasetUpdateData From(this IDatasetUpdate dataset)
        {
            return new DatasetUpdateData
            {
                Name = dataset.Name,
                Type = dataset.Type?.GetValueAsString(),
                Description = dataset.Description,
                Tags = dataset.Tags,
                FileOrder = dataset.FileOrder,
                IsVisible = dataset.IsVisible,
            };
        }

        internal static IDatasetBaseData From(this IDatasetCreation dataset)
        {
            return new DatasetBaseData
            {
                Name = dataset.Name,
                Type = dataset.Type?.GetValueAsString(),
                Description = dataset.Description,
                Metadata = dataset.Metadata?.ToObjectDictionary() ?? new Dictionary<string, object>(),
                Tags = dataset.Tags ?? new List<string>(), // WORKAROUND until backend supports null metadata
                IsVisible = dataset.IsVisible,
            };
        }
    }
}
