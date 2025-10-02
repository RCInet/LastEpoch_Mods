using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This is a class containing the information about an asset.
    /// </summary>
    sealed class AssetEntity : IAsset
    {
        readonly IAssetDataSource m_DataSource;
        readonly CacheConfigurationWrapper m_CacheConfiguration;

        /// <inheritdoc />
        public AssetDescriptor Descriptor { get; }

        /// <inheritdoc />
        public AssetState State => Properties.State;

        /// <inheritdoc />
        public int FrozenSequenceNumber => Properties.FrozenSequenceNumber;

        /// <inheritdoc />
        public string Changelog => Properties.Changelog;

        /// <inheritdoc />
        public AssetVersion ParentVersion => Properties.ParentVersion;

        /// <inheritdoc />
        public int ParentFrozenSequenceNumber => Properties.ParentFrozenSequenceNumber;

        /// <inheritdoc />
        public ProjectDescriptor SourceProject => Properties.SourceProject;

        /// <inheritdoc />
        public IEnumerable<ProjectDescriptor> LinkedProjects => Properties.LinkedProjects;

        /// <inheritdoc />
        public string Name => Properties.Name;

        /// <inheritdoc />
        public string Description => Properties.Description;

        /// <inheritdoc />
        public IEnumerable<string> Tags => Properties.Tags;

        /// <inheritdoc />
        public IEnumerable<string> SystemTags => Properties.SystemTags;

        /// <inheritdoc />
        public IEnumerable<LabelDescriptor> Labels => Properties.Labels;

        /// <inheritdoc />
        public IEnumerable<LabelDescriptor> ArchivedLabels => Properties.ArchivedLabels;

        /// <inheritdoc />
        public AssetType Type => Properties.Type;

        /// <inheritdoc />
        public IMetadataContainer Metadata => MetadataEntity;

        /// <inheritdoc />
        public IReadOnlyMetadataContainer SystemMetadata => SystemMetadataEntity;

        /// <inheritdoc />
        public string PreviewFile => PreviewFileDescriptor.Path;

        /// <inheritdoc />
        public FileDescriptor PreviewFileDescriptor => Properties.PreviewFileDescriptor ?? new FileDescriptor(new DatasetDescriptor(), string.Empty);

        /// <inheritdoc />
        public string Status => StatusName;

        /// <inheritdoc />
        public string StatusName => Properties.StatusName;

        /// <inheritdoc />
        public StatusFlowDescriptor StatusFlowDescriptor => Properties.StatusFlowDescriptor;

        /// <inheritdoc />
        public AuthoringInfo AuthoringInfo => Properties.AuthoringInfo;

        /// <inheritdoc />
        public AssetCacheConfiguration CacheConfiguration => m_CacheConfiguration.AssetConfiguration;

        AssetRepositoryCacheConfiguration DefaultCacheConfiguration => m_CacheConfiguration.DefaultConfiguration;

        internal AssetProperties Properties { get; set; }
        internal Uri PreviewFileUrl { get; set; }
        internal List<IDatasetData> Datasets { get; } = new();
        internal Dictionary<DatasetId, IDatasetData> DatasetMap { get; } = new();
        internal MetadataContainerEntity MetadataEntity { get; }
        internal ReadOnlyMetadataContainerEntity SystemMetadataEntity { get; }

        internal AssetEntity(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, AssetDescriptor assetDescriptor, AssetCacheConfiguration? cacheConfigurationOverride = null)
        {
            m_DataSource = dataSource;
            Descriptor = assetDescriptor;

            m_CacheConfiguration = new CacheConfigurationWrapper(defaultCacheConfiguration);
            m_CacheConfiguration.SetAssetConfiguration(cacheConfigurationOverride);

            MetadataEntity = new MetadataContainerEntity(new AssetMetadataDataSource(Descriptor, m_DataSource, MetadataDataSourceSpecification.metadata));
            SystemMetadataEntity = new ReadOnlyMetadataContainerEntity(new AssetMetadataDataSource(Descriptor, m_DataSource, MetadataDataSourceSpecification.systemMetadata));
        }

        /// <inheritdoc />
        public Task<IAsset> WithCacheConfigurationAsync(AssetCacheConfiguration assetConfiguration, CancellationToken cancellationToken)
        {
            return GetConfiguredAsync(m_DataSource, DefaultCacheConfiguration, Descriptor, assetConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        public IAsset WithProject(ProjectDescriptor projectDescriptor)
        {
            if (projectDescriptor == Descriptor.ProjectDescriptor) return this;

            if (projectDescriptor.OrganizationId != Descriptor.OrganizationId || CacheConfiguration.CacheProperties && Properties.LinkedProjects.All(p => p.ProjectId != projectDescriptor.ProjectId))
                throw new InvalidArgumentException("The asset does not belong to the specified project.");

            return new AssetEntity(m_DataSource, DefaultCacheConfiguration, new AssetDescriptor(projectDescriptor, Descriptor.AssetId, Descriptor.AssetVersion))
            {
                Properties = Properties,
                PreviewFileUrl = PreviewFileUrl,
                MetadataEntity = {Properties = MetadataEntity.Properties},
                SystemMetadataEntity = {Properties = SystemMetadataEntity.Properties}
            };
        }

        /// <inheritdoc />
        public Task<IAsset> WithProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            var assetDescriptor = new AssetDescriptor(projectDescriptor, Descriptor.AssetId, Descriptor.AssetVersion);
            return GetConfiguredAsync(m_DataSource, DefaultCacheConfiguration, assetDescriptor, CacheConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IAsset> WithVersionAsync(AssetVersion assetVersion, CancellationToken cancellationToken)
        {
            var assetDescriptor = new AssetDescriptor(Descriptor.ProjectDescriptor, Descriptor.AssetId, assetVersion);
            return GetConfiguredAsync(m_DataSource, DefaultCacheConfiguration, assetDescriptor, CacheConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        /// <exception cref="NotFoundException">If a version with the corresponding <paramref name="label"/> is not found. </exception>
        public async Task<IAsset> WithVersionAsync(string label, CancellationToken cancellationToken)
        {
            var fieldsFilter = m_CacheConfiguration.GetAssetFieldsFilter();

            var data = await m_DataSource.GetAssetAsync(Descriptor.ProjectDescriptor, Descriptor.AssetId, label, fieldsFilter, cancellationToken);

            if (data == null)
            {
                throw new NotFoundException($"Could not retrieve asset with label '{label}'.");
            }

            var assetDescriptor = new AssetDescriptor(Descriptor.ProjectDescriptor, Descriptor.AssetId, data.Version);
            return data.From(m_DataSource, DefaultCacheConfiguration, assetDescriptor, fieldsFilter, CacheConfiguration);
        }

        /// <inheritdoc />
        public async Task RefreshAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.HasCachingRequirements)
            {
                var fieldsFilter = m_CacheConfiguration.GetAssetFieldsFilter();
                var data = await m_DataSource.GetAssetAsync(Descriptor, fieldsFilter, cancellationToken);
                this.MapFrom(m_DataSource, data, fieldsFilter);
            }
        }

        /// <inheritdoc />
        public async Task<AssetProperties> GetPropertiesAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.CacheProperties)
            {
                return Properties;
            }

            var fieldsFilter = FieldsFilter.DefaultAssetIncludes;
            var data = await m_DataSource.GetAssetAsync(Descriptor, fieldsFilter, cancellationToken);
            return data.From(Descriptor, fieldsFilter);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(IAssetUpdate assetUpdate, CancellationToken cancellationToken)
        {
            if (assetUpdate.HasValues())
            {
                await m_DataSource.UpdateAssetAsync(Descriptor, assetUpdate.From(), cancellationToken);
            }

            // Update status flow descriptor next
            if (assetUpdate.StatusFlowDescriptor.HasValue)
            {
                await m_DataSource.UpdateAssetStatusFlowAsync(Descriptor, assetUpdate.StatusFlowDescriptor.Value, cancellationToken);
            }
        }

        /// <inheritdoc />
        [Obsolete("Use UpdateAsync(IAssetUpdate, CancellationToken) instead.")]
        public Task UpdateStatusAsync(AssetStatusAction statusAction, CancellationToken cancellationToken)
        {
            var status = IsolatedSerialization.SerializeWithConverters(statusAction, IsolatedSerialization.StringEnumConverter).Replace("\"", "");
            return m_DataSource.UpdateAssetStatusAsync(Descriptor, status, cancellationToken);
        }

        /// <inheritdoc />
        public Task UpdateStatusAsync(string statusName, CancellationToken cancellationToken)
        {
            return m_DataSource.UpdateAssetStatusAsync(Descriptor, statusName, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IAsset> CreateUnfrozenVersionAsync(CancellationToken cancellationToken)
        {
            var assetDescriptor = await CreateUnfrozenVersionLiteAsync(cancellationToken);
            return await GetConfiguredAsync(m_DataSource, DefaultCacheConfiguration, assetDescriptor, CacheConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<AssetDescriptor> CreateUnfrozenVersionLiteAsync(CancellationToken cancellationToken)
        {
            var version = await m_DataSource.CreateUnfrozenAssetVersionAsync(Descriptor, null, cancellationToken);
            return new AssetDescriptor(Descriptor.ProjectDescriptor, Descriptor.AssetId, version);
        }

        /// <inheritdoc />
        public async Task<int> FreezeAsync(string changeLog, CancellationToken cancellationToken)
        {
            return await m_DataSource.FreezeAssetVersionAsync(Descriptor, changeLog, false, cancellationToken) ?? -1;
        }

        /// <inheritdoc />
        public Task FreezeAsync(IAssetFreeze assetFreeze, CancellationToken cancellationToken)
        {
            bool? forceFreeze = assetFreeze.Operation switch
            {
                AssetFreezeOperation.CancelTransformations => true,
                AssetFreezeOperation.IgnoreIfTransformations => false,
                _ => null
            };
            return m_DataSource.FreezeAssetVersionAsync(Descriptor, assetFreeze.ChangeLog, forceFreeze, cancellationToken);
        }

        public Task CancelPendingFreezeAsync(CancellationToken cancellationToken)
        {
            return m_DataSource.CancelFreezeAssetVersionAsync(Descriptor, cancellationToken);
        }

        /// <inheritdoc />
        public VersionQueryBuilder QueryVersions()
        {
            return new VersionQueryBuilder(m_DataSource, DefaultCacheConfiguration, Descriptor.ProjectDescriptor, Descriptor.AssetId);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IAsset> ListVersionsAsync(Range range, CancellationToken cancellationToken)
        {
            return new VersionQueryBuilder(m_DataSource, DefaultCacheConfiguration, Descriptor.ProjectDescriptor, Descriptor.AssetId)
                .WithCacheConfiguration(CacheConfiguration)
                .OrderBy("versionNumber", SortingOrder.Descending)
                .LimitTo(range)
                .ExecuteAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetProject> GetLinkedProjectsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var properties = await GetPropertiesAsync(cancellationToken);
            foreach (var projectDescriptor in properties.LinkedProjects)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return await AssetProjectEntity.GetConfiguredAsync(m_DataSource, DefaultCacheConfiguration, projectDescriptor, null, cancellationToken);
            }
        }

        /// <inheritdoc />
        public Task LinkToProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            return m_DataSource.LinkAssetToProjectAsync(Descriptor, projectDescriptor, cancellationToken);
        }

        /// <inheritdoc />
        public Task UnlinkFromProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            var assetDescriptor = new AssetDescriptor(projectDescriptor, Descriptor.AssetId, Descriptor.AssetVersion);
            return m_DataSource.UnlinkAssetFromProjectAsync(assetDescriptor, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Uri> GetPreviewUrlAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.CachePreviewUrl)
            {
                return PreviewFileUrl;
            }

            var fieldsFilter = new FieldsFilter {AssetFields = AssetFields.previewFileUrl};
            var data = await m_DataSource.GetAssetAsync(Descriptor, fieldsFilter, cancellationToken);
            this.MapFrom(m_DataSource, data, fieldsFilter);

            return PreviewFileUrl;
        }

        /// <inheritdoc />
        public async Task<IDictionary<string, Uri>> GetAssetDownloadUrlsAsync(CancellationToken cancellationToken)
        {
            var fileUrls = await m_DataSource.GetAssetDownloadUrlsAsync(Descriptor, null, cancellationToken);

            var urls = new Dictionary<string, Uri>();
            foreach (var url in fileUrls)
            {
                urls.Add(url.FilePath, url.DownloadUrl);
            }

            return urls;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CollectionDescriptor> ListLinkedAssetCollectionsAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var enumerable = await m_DataSource.GetAssetCollectionsAsync(Descriptor, cancellationToken);

            var collectionDatas = enumerable?.ToArray() ?? Array.Empty<IAssetCollectionData>();
            if (collectionDatas.Length > 0)
            {
                var (start, length) = range.GetValidatedOffsetAndLength(collectionDatas.Length);
                for (var i = start; i < start + length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return new CollectionDescriptor(Descriptor.ProjectDescriptor, collectionDatas[i].GetFullCollectionPath());
                }
            }
        }

        /// <inheritdoc />
        public Task<IDataset> CreateDatasetAsync(DatasetCreation datasetCreation, CancellationToken cancellationToken)
            => CreateDatasetAsync((IDatasetCreation) datasetCreation, cancellationToken);

        /// <inheritdoc />
        public async Task<IDataset> CreateDatasetAsync(IDatasetCreation datasetCreation, CancellationToken cancellationToken)
        {
            var datasetDescriptor = await CreateDatasetLiteAsync(datasetCreation, cancellationToken);
            return await DatasetEntity.GetConfiguredAsync(m_DataSource, DefaultCacheConfiguration, datasetDescriptor, CacheConfiguration.DatasetCacheConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        public Task<DatasetDescriptor> CreateDatasetLiteAsync(IDatasetCreation datasetCreation, CancellationToken cancellationToken)
        {
            return m_DataSource.CreateDatasetAsync(Descriptor, datasetCreation.From(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IDataset> GetDatasetAsync(DatasetId datasetId, CancellationToken cancellationToken)
        {
            var datasetDescriptor = new DatasetDescriptor(Descriptor, datasetId);

            if (CacheConfiguration.CacheDatasetList)
            {
                var datasetData = DatasetMap.GetValueOrDefault(datasetId);
                return datasetData?.From(m_DataSource, DefaultCacheConfiguration,
                    datasetDescriptor, m_CacheConfiguration.GetDatasetFieldsFilter().DatasetFields,
                    CacheConfiguration.DatasetCacheConfiguration);
            }

            return await DatasetEntity.GetConfiguredAsync(m_DataSource, DefaultCacheConfiguration, datasetDescriptor, CacheConfiguration.DatasetCacheConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IDataset> ListDatasetsAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var fieldsFilter = m_CacheConfiguration.GetDatasetFieldsFilter();
            var asyncEnumerable = CacheConfiguration.CacheDatasetList
                ? ListCachedDatasetsAsync(range, cancellationToken)
                : ListRemoteDatasetsAsync(range, fieldsFilter, cancellationToken);
            await foreach (var datasetData in asyncEnumerable.WithCancellation(cancellationToken))
            {
                yield return datasetData.From(m_DataSource, DefaultCacheConfiguration, Descriptor, fieldsFilter.DatasetFields, CacheConfiguration.DatasetCacheConfiguration);
            }
        }

        async IAsyncEnumerable<IDatasetData> ListCachedDatasetsAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var (start, length) = range.GetValidatedOffsetAndLength(Datasets.Count);
            for (var i = start; i < start + length; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return Datasets[i];
            }

            await Task.CompletedTask;
        }

        IAsyncEnumerable<IDatasetData> ListRemoteDatasetsAsync(Range range, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            return m_DataSource.ListDatasetsAsync(Descriptor, range, includedFieldsFilter, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IFile> GetFileAsync(string filePath, CancellationToken cancellationToken)
        {
            var fieldsFilter = m_CacheConfiguration.GetFileFieldsFilter();
            fieldsFilter.AssetFields |= AssetFields.files;

            var asset = await m_DataSource.GetAssetAsync(Descriptor, fieldsFilter, cancellationToken);

            IFile file = null;

            var fileData = asset.Files?.FirstOrDefault(x => x.Path == filePath);
            if (fileData != null)
            {
                file = fileData.From(m_DataSource, DefaultCacheConfiguration, Descriptor, fieldsFilter.FileFields, CacheConfiguration.DatasetCacheConfiguration.FileCacheConfiguration);
            }

            if (file == null)
            {
                throw new NotFoundException($"File {filePath} not found.");
            }

            return file;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IFile> ListFilesAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var fieldsFilter = m_CacheConfiguration.GetFileFieldsFilter();
            fieldsFilter.AssetFields |= AssetFields.files;

            var asset = await m_DataSource.GetAssetAsync(Descriptor, fieldsFilter, cancellationToken);

            var files = asset.Files?.ToArray() ?? Array.Empty<IFileData>();
            var (start, length) = range.GetValidatedOffsetAndLength(files.Length);
            for (var i = start; i < start + length; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return files[i].From(m_DataSource, DefaultCacheConfiguration, Descriptor, fieldsFilter.FileFields, CacheConfiguration.DatasetCacheConfiguration.FileCacheConfiguration);
            }
        }

        /// <inheritdoc />
        public AssetLabelQueryBuilder QueryLabels()
        {
            return new AssetLabelQueryBuilder(m_DataSource, Descriptor.ProjectDescriptor, Descriptor.AssetId);
        }

        /// <inheritdoc />
        public Task AssignLabelsAsync(IEnumerable<string> labels, CancellationToken cancellationToken)
        {
            return m_DataSource.AssignLabelsAsync(Descriptor, labels, cancellationToken);
        }

        /// <inheritdoc />
        public Task UnassignLabelsAsync(IEnumerable<string> labels, CancellationToken cancellationToken)
        {
            return m_DataSource.UnassignLabelsAsync(Descriptor, labels, cancellationToken);
        }

        /// <inheritdoc />
        public Task<string[]> GetReachableStatusNamesAsync(CancellationToken cancellationToken)
        {
            return m_DataSource.GetReachableStatusNamesAsync(Descriptor, cancellationToken);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IAssetReference> ListReferencesAsync(Range range, CancellationToken cancellationToken)
        {
            var filter = new AssetReferenceSearchFilter();
            filter.AssetVersion.WhereEquals(Descriptor.AssetVersion);

            return new AssetReferenceQueryBuilder(m_DataSource, Descriptor.ProjectDescriptor, Descriptor.AssetId)
                .SelectWhereMatchesFilter(filter)
                .LimitTo(range)
                .ExecuteAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<IAssetReference> AddReferenceAsync(AssetId targetAssetId, AssetVersion targetAssetVersion, CancellationToken cancellationToken)
        {
            var assetIdentifier = new AssetIdentifierDto
            {
                Id = targetAssetId.ToString(),
                Version = targetAssetVersion.ToString()
            };
            return AddReferenceAsync(assetIdentifier, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IAssetReference> AddReferenceAsync(AssetId targetAssetId, string targetLabel, CancellationToken cancellationToken)
        {
            var assetIdentifier = new AssetIdentifierDto
            {
                Id = targetAssetId.ToString(),
                Label = targetLabel
            };
            return AddReferenceAsync(assetIdentifier, cancellationToken);
        }

        async Task<IAssetReference> AddReferenceAsync(AssetIdentifierDto targetAssetIdentifier, CancellationToken cancellationToken)
        {
            var referenceId = await m_DataSource.CreateAssetReferenceAsync(Descriptor, targetAssetIdentifier, cancellationToken);

            // Ideally we would fetch the newly created reference data here, but the API does not have an entry for returning a single reference.
            return new AssetReference(Descriptor.ProjectDescriptor, referenceId)
            {
                IsValid = true,
                SourceAssetId = Descriptor.AssetId,
                SourceAssetVersion = Descriptor.AssetVersion,
                TargetAssetId = new AssetId(targetAssetIdentifier.Id),
                TargetAssetVersion = string.IsNullOrWhiteSpace(targetAssetIdentifier.Version) ? null : new AssetVersion(targetAssetIdentifier.Version),
                TargetLabel = string.IsNullOrWhiteSpace(targetAssetIdentifier.Label) ? null : targetAssetIdentifier.Label,
            };
        }

        /// <inheritdoc />
        public Task RemoveReferenceAsync(string referenceId, CancellationToken cancellationToken)
        {
            return m_DataSource.DeleteAssetReferenceAsync(Descriptor.ProjectDescriptor, Descriptor.AssetId, referenceId, cancellationToken);
        }

        /// <inheritdoc />
        public string SerializeIdentifiers()
        {
            return Descriptor.ToJson();
        }

        /// <inheritdoc />
        public string Serialize()
        {
            var data = new AssetDataWithIdentifiers
            {
                Descriptor = Descriptor.ToJson(),
                Data = this.From()
            };
            return IsolatedSerialization.SerializeWithDefaultConverters(data);
        }

        /// <summary>
        /// Returns an asset configured with the specified cache configuration.
        /// </summary>
        internal static async Task<IAsset> GetConfiguredAsync(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, AssetDescriptor descriptor, AssetCacheConfiguration? configuration, CancellationToken cancellationToken)
        {
            var asset = new AssetEntity(dataSource, defaultCacheConfiguration, descriptor, configuration);

            if (asset.CacheConfiguration.HasCachingRequirements)
            {
                await asset.RefreshAsync(cancellationToken);
            }

            return asset;
        }
    }
}
