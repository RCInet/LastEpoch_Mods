using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this FileEntity file, IAssetDataSource assetDataSource, IFileData fileData, FileFields includeFields)
        {
            if (file.CacheConfiguration.CacheProperties)
                file.Properties = fileData.From(file.Descriptor, includeFields);

            if (includeFields.HasFlag(FileFields.downloadURL))
                file.DownloadUrl = Uri.TryCreate(fileData.DownloadUrl, UriKind.RelativeOrAbsolute, out var downloadUri) ? downloadUri : null;
            if (includeFields.HasFlag(FileFields.previewURL))
                file.PreviewUrl = Uri.TryCreate(fileData.PreviewUrl, UriKind.RelativeOrAbsolute, out var previewUri) ? previewUri : null;
            if (includeFields.HasFlag(FileFields.metadata))
                file.MetadataEntity.Properties = fileData.Metadata?.From(assetDataSource, file.Descriptor.OrganizationId) ?? new Dictionary<string, MetadataObject>();
            if (includeFields.HasFlag(FileFields.systemMetadata))
                file.SystemMetadataEntity.Properties = fileData.SystemMetadata?.From() ?? new Dictionary<string, MetadataObject>();
        }

        internal static FileProperties From(this IFileData fileData, FileDescriptor fileDescriptor, FileFields includeFields)
        {
            var assetDescriptor = fileDescriptor.DatasetDescriptor.AssetDescriptor;

            var fileProperties = new FileProperties
            {
                LinkedDatasets = fileData.DatasetIds?.Select(id => new DatasetDescriptor(assetDescriptor, id)).ToArray() ?? Array.Empty<DatasetDescriptor>(),
                Tags = fileData.Tags ?? Array.Empty<string>(),
                SystemTags = fileData.SystemTags ?? Array.Empty<string>(),
                StatusName = fileData.Status
            };

            if (includeFields.HasFlag(FileFields.description))
                fileProperties.Description = fileData.Description ?? string.Empty;
            if (includeFields.HasFlag(FileFields.authoring))
                fileProperties.AuthoringInfo = new AuthoringInfo(fileData.CreatedBy, fileData.Created, fileData.UpdatedBy, fileData.Updated);
            if (includeFields.HasFlag(FileFields.userChecksum))
                fileProperties.UserChecksum = fileData.UserChecksum;
            if (includeFields.HasFlag(FileFields.fileSize))
                fileProperties.SizeBytes = fileData.SizeBytes;

            return fileProperties;
        }

        internal static FileEntity From(this IFileData fileData, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            FileDescriptor fileDescriptor, FileFields includeFields, FileCacheConfiguration? cacheConfigurationOverride = null)
        {
            var file = new FileEntity(assetDataSource, defaultCacheConfiguration, fileDescriptor, cacheConfigurationOverride);
            file.MapFrom(assetDataSource, fileData, includeFields);
            return file;
        }

        internal static FileEntity From(this IFileData fileData, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            AssetDescriptor assetDescriptor, FileFields includeFields, FileCacheConfiguration? cacheConfigurationOverride = null)
        {
            // Because actions cannot be performed on a file that is not linked to at least 1 dataset, we ignore these files.
            if (fileData.DatasetIds == null || !fileData.DatasetIds.Any()) return null;

            var fileDescriptor = new FileDescriptor(new DatasetDescriptor(assetDescriptor, fileData.DatasetIds.First()), fileData.Path);
            return fileData.From(assetDataSource, defaultCacheConfiguration, fileDescriptor, includeFields, cacheConfigurationOverride);
        }

        internal static FileCreateData From(this IFileCreation fileCreation)
        {
            return new FileCreateData
            {
                Path = fileCreation.Path,
                Description = fileCreation.Description,
                Tags = fileCreation.Tags?.ToList() ?? new List<string>(),
                Metadata = fileCreation.Metadata?.ToObjectDictionary() ?? new Dictionary<string, object>(),
            };
        }

        internal static IFileBaseData From(this IFileUpdate fileUpdate)
        {
            return new FileBaseData
            {
                Description = fileUpdate.Description,
                Tags = fileUpdate.Tags,
            };
        }
    }
}
