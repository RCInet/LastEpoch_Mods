using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this AssetEntity asset, IAssetDataSource assetDataSource, IAssetData assetData, FieldsFilter includeFields)
        {
            includeFields ??= new FieldsFilter();

            if (asset.CacheConfiguration.CacheProperties)
                asset.Properties = assetData.From(asset.Descriptor, includeFields);

            if (includeFields.AssetFields.HasFlag(AssetFields.metadata))
                asset.MetadataEntity.Properties = assetData.Metadata?.From(assetDataSource, asset.Descriptor.OrganizationId) ?? new Dictionary<string, MetadataObject>();

            if (includeFields.AssetFields.HasFlag(AssetFields.systemMetadata))
                asset.SystemMetadataEntity.Properties = assetData.SystemMetadata?.From() ?? new Dictionary<string, MetadataObject>();

            if (includeFields.AssetFields.HasFlag(AssetFields.previewFileUrl))
                asset.PreviewFileUrl = Uri.TryCreate(assetData.PreviewFileUrl, UriKind.RelativeOrAbsolute, out var previewFileDownloadUri) ? previewFileDownloadUri : null;

            asset.Datasets.Clear();
            asset.DatasetMap.Clear();

            var files = new List<FileData>();
            if (includeFields.AssetFields.HasFlag(AssetFields.files) && assetData.Files != null)
                files.AddRange(assetData.Files);

            if (includeFields.AssetFields.HasFlag(AssetFields.datasets) && assetData.Datasets != null)
            {
                foreach (var dataset in assetData.Datasets)
                {
                    dataset.Files = files.Where(x => x.DatasetIds.Contains(dataset.DatasetId)).ToList();

                    asset.Datasets.Add(dataset);
                    asset.DatasetMap[dataset.DatasetId] = dataset;
                }
            }
        }

        internal static AssetProperties From(this IAssetData assetData, AssetDescriptor assetDescriptor, FieldsFilter includeFields)
        {
            var organizationId = assetDescriptor.OrganizationId;

            if (string.IsNullOrEmpty(assetData.Type) || !assetData.Type.TryGetAssetTypeFromString(out var assetType))
            {
                assetType = AssetType.Other;
            }

            var assetProperties = new AssetProperties
            {
                LinkedProjects = assetData.LinkedProjectIds?.Select(projectId => new ProjectDescriptor(organizationId, projectId)).ToArray() ?? Array.Empty<ProjectDescriptor>(),
                SourceProject = new ProjectDescriptor(organizationId, assetData.SourceProjectId),
                Name = assetData.Name,
                Tags = assetData.Tags ?? Array.Empty<string>(),
                SystemTags = assetData.SystemTags ?? Array.Empty<string>(),
                Type = assetType,
                StatusName = assetData.Status
            };

            includeFields ??= new FieldsFilter();

            if (includeFields.AssetFields.HasFlag(AssetFields.description))
            {
                assetProperties.Description = assetData.Description ?? string.Empty;
            }

            if (includeFields.AssetFields.HasFlag(AssetFields.versioning))
            {
                if (assetData.IsFrozen)
                {
                    assetProperties.State = AssetState.Frozen;
                }
                else if (assetData.AutoSubmit)
                {
                    assetProperties.State = AssetState.PendingFreeze;
                }
                else
                {
                    assetProperties.State = AssetState.Unfrozen;
                }

                assetProperties.FrozenSequenceNumber = assetData.VersionNumber;
                assetProperties.Changelog = assetData.Changelog;
                assetProperties.ParentVersion = assetData.ParentVersion;
                assetProperties.ParentFrozenSequenceNumber = assetData.ParentVersionNumber;
            }

            if (includeFields.AssetFields.HasFlag(AssetFields.labels))
            {
                assetProperties.Labels = assetData.Labels?.Select(x => new LabelDescriptor(organizationId, x)) ?? Array.Empty<LabelDescriptor>();
                assetProperties.ArchivedLabels = assetData.ArchivedLabels?.Select(x => new LabelDescriptor(organizationId, x)) ?? Array.Empty<LabelDescriptor>();
            }

            if (includeFields.AssetFields.HasFlag(AssetFields.previewFile))
            {
                assetProperties.StatusFlowDescriptor = new StatusFlowDescriptor(organizationId, assetData.StatusFlowId);
                var previewFileDatasetDescriptor = new DatasetDescriptor(assetDescriptor, assetData.PreviewFileDatasetId);
                assetProperties.PreviewFileDescriptor = new FileDescriptor(previewFileDatasetDescriptor, assetData.PreviewFilePath ?? string.Empty);
            }

            if (includeFields.AssetFields.HasFlag(AssetFields.authoring))
                assetProperties.AuthoringInfo = new AuthoringInfo(assetData.CreatedBy, assetData.Created, assetData.UpdatedBy, assetData.Updated);

            return assetProperties;
        }

        internal static AssetCreateData From(this IAssetCreation assetCreation)
        {
            return new AssetCreateData
            {
                Name = assetCreation.Name,
                Description = assetCreation.Description,
                Tags = assetCreation.Tags?.Where(s => !string.IsNullOrWhiteSpace(s)),
                StatusFlowId = assetCreation.StatusFlowDescriptor?.StatusFlowId,
                Type = assetCreation.Type.GetValueAsString(),
                Metadata = assetCreation.Metadata?.ToObjectDictionary() ?? new Dictionary<string, object>(),
                Collections = assetCreation.Collections,
            };
        }

        internal static IAssetUpdateData From(this IAssetUpdate assetUpdate)
        {
            return new AssetUpdateData
            {
                Name = assetUpdate.Name,
                Description = assetUpdate.Description,
                Tags = assetUpdate.Tags?.Where(s => !string.IsNullOrWhiteSpace(s)),
                Type = assetUpdate.Type?.GetValueAsString(),
                PreviewFile = assetUpdate.PreviewFile,
            };
        }

        internal static AssetEntity From(this IAssetData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            OrganizationId organizationId, IEnumerable<ProjectId> availableProjects, FieldsFilter includeFields, AssetCacheConfiguration? assetCacheConfiguration = null)
        {
            var validProjects = new HashSet<ProjectId>(availableProjects);
            validProjects.IntersectWith(data.LinkedProjectIds ?? Array.Empty<ProjectId>());

            var projectId = data.SourceProjectId;
            if (validProjects.Any() && !validProjects.Contains(projectId))
            {
                projectId = validProjects.First();
            }

            return data.From(assetDataSource, defaultCacheConfiguration, new ProjectDescriptor(organizationId, projectId), includeFields, assetCacheConfiguration);
        }

        internal static AssetEntity From(this IAssetData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            ProjectDescriptor projectDescriptor, FieldsFilter includeFields, AssetCacheConfiguration? assetCacheConfiguration = null)
        {
            var descriptor = new AssetDescriptor(projectDescriptor, data.Id, data.Version);
            return data.From(assetDataSource, defaultCacheConfiguration, descriptor, includeFields, assetCacheConfiguration);
        }

        internal static AssetEntity From(this IAssetData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            AssetDescriptor assetDescriptor, FieldsFilter includeFields, AssetCacheConfiguration? assetCacheConfiguration = null)
        {
            var asset = new AssetEntity(assetDataSource, defaultCacheConfiguration, assetDescriptor, assetCacheConfiguration);
            asset.MapFrom(assetDataSource, data, includeFields);
            return asset;
        }

        internal static IAsset From(this AssetDataWithIdentifiers data, IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, AssetCacheConfiguration assetCacheConfiguration)
        {
            var wrapper = new CacheConfigurationWrapper(defaultCacheConfiguration);
            wrapper.SetAssetConfiguration(assetCacheConfiguration);

#pragma warning disable 618
            var assetDescriptor = string.IsNullOrEmpty(data.Descriptor) ? data.Identifier.From() : AssetDescriptor.FromJson(data.Descriptor);
            return data.Data.From(dataSource, defaultCacheConfiguration, assetDescriptor, wrapper.GetAssetFieldsFilter(), assetCacheConfiguration);
#pragma warning restore 618
        }

        internal static AssetDescriptor From(this AssetIdentifier ids)
        {
            var projectDescriptor = new ProjectDescriptor(ids.OrganizationId, ids.ProjectId);
            return new AssetDescriptor(projectDescriptor, ids.Id, ids.Version);
        }

        static AssetData From(this AssetProperties assetProperties)
        {
            return new AssetData
            {
                Name = assetProperties.Name,
                Description = assetProperties.Description,
                Tags = assetProperties.Tags,
                SystemTags = assetProperties.SystemTags,
                Type = assetProperties.Type.GetValueAsString(),
                PreviewFilePath = assetProperties.PreviewFileDescriptor?.Path,
                PreviewFileDatasetId = assetProperties.PreviewFileDescriptor?.DatasetId ?? default,
                Status = assetProperties.StatusName,
                Created = assetProperties.AuthoringInfo?.Created,
                CreatedBy = assetProperties.AuthoringInfo?.CreatedBy.ToString(),
                Updated = assetProperties.AuthoringInfo?.Updated,
                UpdatedBy = assetProperties.AuthoringInfo?.UpdatedBy.ToString(),
                SourceProjectId = assetProperties.SourceProject.ProjectId,
                LinkedProjectIds = assetProperties.LinkedProjects?.Select(project => project.ProjectId).ToList(),
                Labels = assetProperties.Labels?.Select(x => x.LabelName),
                ArchivedLabels = assetProperties.ArchivedLabels?.Select(x => x.LabelName),
                ParentVersion = assetProperties.ParentVersion,
                ParentVersionNumber = assetProperties.ParentFrozenSequenceNumber,
                IsFrozen = assetProperties.State == AssetState.Frozen,
                AutoSubmit = assetProperties.State == AssetState.PendingFreeze,
                VersionNumber = assetProperties.FrozenSequenceNumber,
                Changelog = assetProperties.Changelog,
            };
        }

        internal static AssetData From(this AssetEntity assetEntity)
        {
            var data = assetEntity.Properties.From();
            data.Id = assetEntity.Descriptor.AssetId;
            data.Version = assetEntity.Descriptor.AssetVersion;
            data.PreviewFileUrl = assetEntity.PreviewFileUrl?.ToString();
            data.Metadata = assetEntity.MetadataEntity.From();

            return data;
        }

        internal static bool HasValues(this IAssetUpdate assetUpdate)
        {
            return assetUpdate.Name != null ||
                assetUpdate.Description != null ||
                assetUpdate.Tags != null ||
                assetUpdate.Type.HasValue ||
                assetUpdate.PreviewFile != null;
        }
    }
}
