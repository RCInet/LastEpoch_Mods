using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this TransformationEntity transformation, ITransformationData data)
        {
            if (transformation.CacheConfiguration.CacheProperties)
                transformation.Properties = data.From();
        }

        internal static TransformationProperties From(this ITransformationData data)
        {
            return new TransformationProperties
            {
                OutputDatasetId = data.OutputDatasetId,
                LinkDatasetId = data.LinkDatasetId,
                InputFilePaths = data.InputFiles,
                WorkflowType = WorkflowTypeUtilities.FromJsonValue(data.WorkflowType),
                WorkflowName = data.WorkflowType,
                Status = data.Status,
                ErrorMessage = data.ErrorMessage,
                Progress = data.Progress,
                Created = data.CreatedOn,
                Updated = data.UpdatedAt,
                Started = data.StartedAt,
                UserId = data.UserId,
                JobId = data.JobId
            };
        }

        internal static ITransformation From(this ITransformationData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            TransformationDescriptor transformationDescriptor, TransformationCacheConfiguration? cacheConfigurationOverride = null)
        {
            var transformation = new TransformationEntity(assetDataSource, defaultCacheConfiguration, transformationDescriptor, cacheConfigurationOverride);
            transformation.MapFrom(data);
            return transformation;
        }

        internal static ITransformation From(this ITransformationData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            ProjectDescriptor projectDescriptor, TransformationCacheConfiguration? cacheConfigurationOverride = null)
        {
            var assetDescriptor = new AssetDescriptor(projectDescriptor, data.AssetId, data.AssetVersion);
            var datasetDescriptor = new DatasetDescriptor(assetDescriptor, data.InputDatasetId);
            return data.From(assetDataSource, defaultCacheConfiguration, new TransformationDescriptor(datasetDescriptor, data.Id), cacheConfigurationOverride);
        }
    }
}
