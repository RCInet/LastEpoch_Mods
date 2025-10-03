using System;
using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this AssetProjectEntity project, IProjectData projectData)
        {
            if (project.CacheConfiguration.CacheProperties)
                project.Properties = projectData.From();
        }

        internal static AssetProjectEntity From(this IProjectData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            OrganizationId organizationId, AssetProjectCacheConfiguration? cacheConfigurationOverride = null)
        {
            return data.From(assetDataSource, defaultCacheConfiguration, new ProjectDescriptor(organizationId, data.Id), cacheConfigurationOverride);
        }

        internal static AssetProjectEntity From(this IProjectData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            ProjectDescriptor projectDescriptor, AssetProjectCacheConfiguration? cacheConfigurationOverride = null)
        {
            var project = new AssetProjectEntity(assetDataSource, defaultCacheConfiguration, projectDescriptor, cacheConfigurationOverride);
            project.MapFrom(data);
            return project;
        }

        internal static AssetProjectProperties From(this IProjectData data)
        {
            return new AssetProjectProperties
            {
                Name = data.Name,
                HasCollection = data.HasCollection,
                Metadata = new Dictionary<string, string>(data.Metadata ?? new Dictionary<string, string>()),
                Created = data.Metadata?.ExtractDateTime("CreatedAt") ?? default,
                Updated = data.Metadata?.ExtractDateTime("UpdatedAt") ?? default
            };
        }

        static DateTime ExtractDateTime(this Dictionary<string, string> metadata, string key)
        {
            if (metadata.TryGetValue(key, out var value) && DateTime.TryParse(value, out var dateTime))
            {
                return dateTime;
            }

            return default;
        }
    }
}
