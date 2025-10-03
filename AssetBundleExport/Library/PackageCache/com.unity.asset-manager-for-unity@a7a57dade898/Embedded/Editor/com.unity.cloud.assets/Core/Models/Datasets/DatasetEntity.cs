using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class DatasetEntity : IDataset
    {
        const int k_MD5_bufferSize = 4096;

        readonly IAssetDataSource m_DataSource;
        readonly CacheConfigurationWrapper m_CacheConfiguration;

        /// <inheritdoc />
        public DatasetDescriptor Descriptor { get; }

        /// <inheritdoc />
        public string Name => Properties.Name;

        /// <inheritdoc />
        public string Description => Properties.Description;

        /// <inheritdoc />
        public IEnumerable<string> Tags => Properties.Tags;

        /// <inheritdoc />
        public IEnumerable<string> SystemTags => Properties.SystemTags;

        /// <inheritdoc />
        public string Status => Properties.StatusName;

        /// <inheritdoc />
        public AuthoringInfo AuthoringInfo => Properties.AuthoringInfo;

        /// <inheritdoc />
        public IMetadataContainer Metadata => MetadataEntity;

        /// <inheritdoc />
        public IReadOnlyMetadataContainer SystemMetadata => SystemMetadataEntity;

        /// <inheritdoc />
        public IEnumerable<string> FileOrder => Properties.FileOrder;

        /// <inheritdoc />
        public bool IsVisible => Properties.IsVisible;

        /// <inheritdoc />
        public DatasetCacheConfiguration CacheConfiguration => m_CacheConfiguration.DatasetConfiguration;

        AssetRepositoryCacheConfiguration DefaultCacheConfiguration => m_CacheConfiguration.DefaultConfiguration;

        internal DatasetProperties Properties { get; set; }
        internal List<IFileData> Files { get; } = new();
        internal Dictionary<string, IFileData> FileMap { get; } = new();
        internal MetadataContainerEntity MetadataEntity { get; }
        internal ReadOnlyMetadataContainerEntity SystemMetadataEntity { get; }

        internal DatasetEntity(IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, DatasetDescriptor datasetDescriptor, DatasetCacheConfiguration? cacheConfigurationOverride = null)
        {
            m_DataSource = assetDataSource;
            Descriptor = datasetDescriptor;

            m_CacheConfiguration = new CacheConfigurationWrapper(defaultCacheConfiguration);
            m_CacheConfiguration.SetDatasetConfiguration(cacheConfigurationOverride);

            MetadataEntity = new MetadataContainerEntity(new DatasetMetadataDataSource(Descriptor, m_DataSource, MetadataDataSourceSpecification.metadata));
            SystemMetadataEntity = new ReadOnlyMetadataContainerEntity(new DatasetMetadataDataSource(Descriptor, m_DataSource, MetadataDataSourceSpecification.systemMetadata));
        }

        /// <inheritdoc />
        public Task<IDataset> WithCacheConfigurationAsync(DatasetCacheConfiguration datasetConfiguration, CancellationToken cancellationToken)
        {
            return GetConfiguredAsync(m_DataSource, DefaultCacheConfiguration, Descriptor, datasetConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<DatasetProperties> GetPropertiesAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.CacheProperties)
            {
                return Properties;
            }

            var fieldsFilter = FieldsFilter.DefaultDatasetIncludes;
            var data = await m_DataSource.GetDatasetAsync(Descriptor, fieldsFilter, cancellationToken);
            return data.From(fieldsFilter.DatasetFields);
        }

        /// <inheritdoc />
        public async Task RefreshAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.HasCachingRequirements)
            {
                var fieldsFilter = m_CacheConfiguration.GetDatasetFieldsFilter();
                var data = await m_DataSource.GetDatasetAsync(Descriptor, fieldsFilter, cancellationToken);
                this.MapFrom(m_DataSource, data, fieldsFilter.DatasetFields);
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(IDatasetUpdate datasetUpdate, CancellationToken cancellationToken)
        {
            await m_DataSource.UpdateDatasetAsync(Descriptor, datasetUpdate.From(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IFile> AddExistingFileAsync(string filePath, DatasetId sourceDatasetId, CancellationToken cancellationToken)
        {
            await AddExistingFileLiteAsync(filePath, sourceDatasetId, cancellationToken);
            return await GetFileAsync(filePath, cancellationToken);
        }

        /// <inheritdoc />
        public Task AddExistingFileLiteAsync(string filePath, DatasetId sourceDatasetId, CancellationToken cancellationToken)
        {
            return m_DataSource.ReferenceFileFromDatasetAsync(Descriptor, sourceDatasetId, filePath, cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveFileAsync(string filePath, CancellationToken cancellationToken)
        {
            return m_DataSource.RemoveFileFromDatasetAsync(Descriptor, filePath, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IFile> GetFileAsync(string filePath, CancellationToken cancellationToken)
        {
            var fileDescriptor = new FileDescriptor(Descriptor, filePath);

            if (CacheConfiguration.CacheFileList)
            {
                var fileData = FileMap.GetValueOrDefault(filePath);
                return fileData?.From(m_DataSource, DefaultCacheConfiguration,
                    fileDescriptor, m_CacheConfiguration.GetFileFieldsFilter().FileFields,
                    CacheConfiguration.FileCacheConfiguration);
            }

            return await FileEntity.GetConfiguredAsync(m_DataSource, DefaultCacheConfiguration, fileDescriptor, CacheConfiguration.FileCacheConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<string, Uri>> GetDownloadUrlsAsync(CancellationToken cancellationToken)
        {
            var fileUrls = await m_DataSource.GetAssetDownloadUrlsAsync(Descriptor.AssetDescriptor, new[] {Descriptor.DatasetId}, cancellationToken);

            var urls = new Dictionary<string, Uri>();
            foreach (var url in fileUrls)
            {
                urls.Add(url.FilePath, url.DownloadUrl);
            }

            return urls;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IFile> ListFilesAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var fieldsFilter = m_CacheConfiguration.GetFileFieldsFilter();
            var data = CacheConfiguration.CacheFileList
                ? ListCachedFilesAsync(range, cancellationToken)
                : ListRemoteFilesAsync(range, fieldsFilter, cancellationToken);
            await foreach (var fileData in data.WithCancellation(cancellationToken))
            {
                yield return fileData.From(m_DataSource, DefaultCacheConfiguration, new FileDescriptor(Descriptor, fileData.Path), fieldsFilter.FileFields, CacheConfiguration.FileCacheConfiguration);
            }
        }

        async IAsyncEnumerable<IFileData> ListCachedFilesAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var (start, length) = range.GetValidatedOffsetAndLength(Files.Count);
            for (var i = start; i < start + length; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return Files[i];
            }

            await Task.CompletedTask;
        }

        IAsyncEnumerable<IFileData> ListRemoteFilesAsync(Range range, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            return m_DataSource.ListFilesAsync(Descriptor, range, includedFieldsFilter, cancellationToken);
        }

        /// <inheritdoc />
        public Uri GetFileUrl(string filePath)
        {
            filePath = Uri.EscapeDataString(filePath);
            var uriRelativePath = $"/assets/storage/v1/projects/{Descriptor.ProjectId}/assets/{Descriptor.AssetId}/versions/{Descriptor.AssetVersion}/datasets/{Descriptor.DatasetId}/files/{filePath}";
            var fileUriBuilder = new UriBuilder(m_DataSource.GetServiceRequestUrl(uriRelativePath));

            return fileUriBuilder.Uri;
        }

        /// <inheritdoc />
        public async Task<IFile> UploadFileAsync(IFileCreation fileCreation, Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            var fileDescriptor = await UploadFileLiteAsync(fileCreation, sourceStream, progress, cancellationToken);
            return await GetFileAsync(fileDescriptor.Path, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<FileDescriptor> UploadFileLiteAsync(IFileCreation fileCreation, Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            var filePath = fileCreation.Path.Replace('\\', '/');

            var creationData = fileCreation.From();
            creationData.SizeBytes = sourceStream.Length;
            creationData.UserChecksum = await CalculateMD5ChecksumAsync(sourceStream, cancellationToken);

            var uploadUrl = await m_DataSource.CreateFileAsync(Descriptor, creationData, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                await m_DataSource.RemoveFileFromDatasetAsync(Descriptor, filePath, default);
                cancellationToken.ThrowIfCancellationRequested();
            }

            // Creation may have failed to get the upload url, try to get it again
            if (uploadUrl == null)
            {
                uploadUrl = await m_DataSource.GetFileUploadUrlAsync(new FileDescriptor(Descriptor, filePath), null, cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                await m_DataSource.RemoveFileFromDatasetAsync(Descriptor, filePath, default);
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (uploadUrl != null)
            {
                try
                {
                    await m_DataSource.UploadContentAsync(uploadUrl, sourceStream, progress, cancellationToken);
                    await m_DataSource.FinalizeFileUploadAsync(new FileDescriptor(Descriptor, filePath), fileCreation.DisableAutomaticTransformations, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    await m_DataSource.RemoveFileFromDatasetAsync(Descriptor, filePath, default);
                    throw;
                }
            }

            return new FileDescriptor(Descriptor, filePath);
        }

        /// <inheritdoc />
        public async Task<ITransformation> StartTransformationAsync(ITransformationCreation transformationCreation, CancellationToken cancellationToken)
        {
            var transformationDescriptor = await StartTransformationLiteAsync(transformationCreation, cancellationToken);
            return await GetTransformationAsync(transformationDescriptor.TransformationId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TransformationDescriptor> StartTransformationLiteAsync(ITransformationCreation transformationCreation, CancellationToken cancellationToken)
        {
            string workflowName;
            switch (transformationCreation.WorkflowType)
            {
                case WorkflowType.Custom:
                    if (string.IsNullOrEmpty(transformationCreation.CustomWorkflowName))
                    {
                        throw new InvalidArgumentException($"A workflow name must be provided when {nameof(WorkflowType.Custom)} is selected.");
                    }

                    workflowName = transformationCreation.CustomWorkflowName;
                    break;
                default:
                    workflowName = transformationCreation.WorkflowType.ToJsonValue();
                    break;
            }

            var transformationId = await m_DataSource.StartTransformationAsync(Descriptor, workflowName, transformationCreation.InputFilePaths, transformationCreation.GetExtraParameters(), cancellationToken);
            return new TransformationDescriptor(Descriptor, transformationId);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<ITransformation> ListTransformationsAsync(Range range, CancellationToken cancellationToken)
        {
            var searchFilter = new TransformationSearchFilter();
            searchFilter.AssetId.WhereEquals(Descriptor.AssetId);
            searchFilter.AssetVersion.WhereEquals(Descriptor.AssetVersion);
            searchFilter.DatasetId.WhereEquals(Descriptor.DatasetId);

            return new TransformationQueryBuilder(m_DataSource, DefaultCacheConfiguration, Descriptor.AssetDescriptor.ProjectDescriptor)
                .SelectWhereMatchesFilter(searchFilter)
                .LimitTo(range)
                .ExecuteAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<ITransformation> GetTransformationAsync(TransformationId transformationId, CancellationToken cancellationToken)
        {
            var descriptor = new TransformationDescriptor(Descriptor, transformationId);
            return TransformationEntity.GetConfiguredAsync(m_DataSource, DefaultCacheConfiguration, descriptor, null, cancellationToken);
        }

        static async Task<string> CalculateMD5ChecksumAsync(Stream stream, CancellationToken cancellationToken)
        {
            var position = stream.Position;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                //In this method, MD5 algorythm is used for calculating checksum of a stream or a file before uploading it.
                //It is not used in a sensitive context.
#pragma warning disable S4790 //Using weak hashing algorithms is security-sensitive
                using (var md5 = MD5.Create())
#pragma warning restore S4790
                {
#if UNITY_WEBGL
                    await CalculateMD5ChecksumInternalAsync(md5, stream, cancellationToken);
#else
                    var result = new TaskCompletionSource<bool>();
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await CalculateMD5ChecksumInternalAsync(md5, stream, cancellationToken);
                        }
                        finally
                        {
                            result.SetResult(true);
                        }
                    }, cancellationToken);
                    await result.Task;
#endif
                    return BitConverter.ToString(md5.Hash).Replace("-", "").ToLowerInvariant();
                }
            }
            catch (Exception e)
            {
                throw new AggregateException(e);
            }
            finally
            {
                stream.Position = position;
            }
        }

        static async Task CalculateMD5ChecksumInternalAsync(MD5 md5, Stream stream, CancellationToken cancellationToken)
        {
            var buffer = new byte[k_MD5_bufferSize];
            int bytesRead;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
#if UNITY_WEBGL && !UNITY_EDITOR
                bytesRead = await Task.FromResult(stream.Read(buffer, 0, k_MD5_bufferSize));
#else
                bytesRead = await stream.ReadAsync(buffer, 0, k_MD5_bufferSize, cancellationToken);
#endif
                if (bytesRead > 0)
                {
                    md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                }
            } while (bytesRead > 0);

            md5.TransformFinalBlock(buffer, 0, 0);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Returns a dataset configured with the specified cache configuration.
        /// </summary>
        internal static async Task<IDataset> GetConfiguredAsync(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, DatasetDescriptor descriptor, DatasetCacheConfiguration? configuration, CancellationToken cancellationToken)
        {
            var dataset = new DatasetEntity(dataSource, defaultCacheConfiguration, descriptor, configuration);

            if (dataset.CacheConfiguration.HasCachingRequirements)
            {
                await dataset.RefreshAsync(cancellationToken);
            }

            return dataset;
        }
    }
}
