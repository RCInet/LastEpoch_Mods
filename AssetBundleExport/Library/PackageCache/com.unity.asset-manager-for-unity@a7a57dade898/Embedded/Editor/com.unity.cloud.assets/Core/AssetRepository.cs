using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Interface to transform user facing data like <see cref="IProjectData"/> into service DTOs.
    /// </summary>
    class AssetRepository : IAssetRepository
    {
        readonly IAssetDataSource m_DataSource;

        public AssetRepositoryCacheConfiguration CacheConfiguration { get; }

        internal AssetRepository(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration configuration)
        {
            m_DataSource = dataSource;
            CacheConfiguration = configuration;
        }

        /// <inheritdoc />
        public AssetProjectQueryBuilder QueryAssetProjects(OrganizationId organizationId)
        {
            return new AssetProjectQueryBuilder(m_DataSource, CacheConfiguration, organizationId);
        }

        /// <inheritdoc />
        public Task<IAssetProject> GetAssetProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            return AssetProjectEntity.GetConfiguredAsync(m_DataSource, CacheConfiguration, projectDescriptor, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IAssetProject> EnableProjectForAssetManagerAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            await EnableProjectForAssetManagerLiteAsync(projectDescriptor, cancellationToken);
            return await GetAssetProjectAsync(projectDescriptor, cancellationToken);
        }

        /// <inheritdoc />
        public Task EnableProjectForAssetManagerLiteAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            return m_DataSource.EnableProjectAsync(projectDescriptor, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IAssetProject> CreateAssetProjectAsync(OrganizationId organizationId, IAssetProjectCreation projectCreation, CancellationToken cancellationToken)
        {
            var projectDescriptor = await CreateAssetProjectLiteAsync(organizationId, projectCreation, cancellationToken);
            return await GetAssetProjectAsync(projectDescriptor, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ProjectDescriptor> CreateAssetProjectLiteAsync(OrganizationId organizationId, IAssetProjectCreation projectCreation, CancellationToken cancellationToken)
        {
            var data = new ProjectBaseData
            {
                Name = projectCreation.Name,
                Metadata = projectCreation.Metadata?.GetAs<Dictionary<string, string>>() ?? new Dictionary<string, string>()
            };
            return m_DataSource.CreateProjectAsync(organizationId, data, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IAssetCollection> GetAssetCollectionAsync(CollectionDescriptor collectionDescriptor, CancellationToken cancellationToken)
        {
            return AssetCollection.GetConfiguredAsync(m_DataSource, CacheConfiguration, collectionDescriptor, null, cancellationToken);
        }

        /// <inheritdoc />
        public AssetQueryBuilder QueryAssets(IEnumerable<ProjectDescriptor> projectDescriptors)
        {
            return new AssetQueryBuilder(m_DataSource, CacheConfiguration, projectDescriptors);
        }

        /// <inheritdoc />
        public AssetQueryBuilder QueryAssets(OrganizationId organizationId)
        {
            return new AssetQueryBuilder(m_DataSource, CacheConfiguration, organizationId);
        }

        /// <inheritdoc />
        public GroupAndCountAssetsQueryBuilder GroupAndCountAssets(IEnumerable<ProjectDescriptor> projectDescriptors)
        {
            return new GroupAndCountAssetsQueryBuilder(m_DataSource, projectDescriptors);
        }

        /// <inheritdoc />
        public GroupAndCountAssetsQueryBuilder GroupAndCountAssets(OrganizationId organizationId)
        {
            return new GroupAndCountAssetsQueryBuilder(m_DataSource, organizationId);
        }

        /// <inheritdoc />
        public Task<IAsset> GetAssetAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            return AssetEntity.GetConfiguredAsync(m_DataSource, CacheConfiguration, assetDescriptor, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IAsset> GetAssetAsync(ProjectDescriptor projectDescriptor, AssetId assetId, string label, CancellationToken cancellationToken)
        {
            var fieldsFilter = CacheConfiguration.GetAssetFieldsFilter();

            var assetData = await m_DataSource.GetAssetAsync(projectDescriptor, assetId, label, fieldsFilter, cancellationToken);
            return assetData.From(m_DataSource, CacheConfiguration, projectDescriptor, fieldsFilter);
        }

        /// <inheritdoc />
        public Task<IDataset> GetDatasetAsync(DatasetDescriptor datasetDescriptor, CancellationToken cancellationToken)
        {
            return DatasetEntity.GetConfiguredAsync(m_DataSource, CacheConfiguration, datasetDescriptor, null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ITransformation> GetTransformationAsync(TransformationDescriptor transformationDescriptor, CancellationToken cancellationToken)
        {
            return TransformationEntity.GetConfiguredAsync(m_DataSource, CacheConfiguration, transformationDescriptor, null, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IFile> GetFileAsync(FileDescriptor fileDescriptor, CancellationToken cancellationToken)
        {
            return FileEntity.GetConfiguredAsync(m_DataSource, CacheConfiguration, fileDescriptor, null, cancellationToken);
        }

        /// <inheritdoc />
        public FieldDefinitionQueryBuilder QueryFieldDefinitions(OrganizationId organizationId)
        {
            return new FieldDefinitionQueryBuilder(m_DataSource, CacheConfiguration, organizationId);
        }

        /// <inheritdoc />
        public Task<IFieldDefinition> GetFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken)
        {
            return FieldDefinitionEntity.GetConfiguredAsync(m_DataSource, CacheConfiguration, fieldDefinitionDescriptor, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IFieldDefinition> CreateFieldDefinitionAsync(OrganizationId organizationId, IFieldDefinitionCreation fieldDefinitionCreation, CancellationToken cancellationToken)
        {
            var fieldDefinitionDescriptor = await CreateFieldDefinitionLiteAsync(organizationId, fieldDefinitionCreation, cancellationToken);

            var fieldDefinition = await GetFieldDefinitionAsync(fieldDefinitionDescriptor, cancellationToken);
            return fieldDefinitionCreation.Type == FieldDefinitionType.Selection ? fieldDefinition.AsSelectionFieldDefinition() : fieldDefinition;
        }

        /// <inheritdoc />
        public Task<FieldDefinitionDescriptor> CreateFieldDefinitionLiteAsync(OrganizationId organizationId, IFieldDefinitionCreation fieldDefinitionCreation, CancellationToken cancellationToken)
        {
            return m_DataSource.CreateFieldDefinitionAsync(organizationId, fieldDefinitionCreation.From(), cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken)
        {
            return m_DataSource.DeleteFieldDefinitionAsync(fieldDefinitionDescriptor, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ILabel> CreateLabelAsync(OrganizationId organizationId, ILabelCreation labelCreation, CancellationToken cancellationToken)
        {
            var labelDescriptor = await CreateLabelLiteAsync(organizationId, labelCreation, cancellationToken);
            return await GetLabelAsync(labelDescriptor, cancellationToken);
        }

        /// <inheritdoc />
        public Task<LabelDescriptor> CreateLabelLiteAsync(OrganizationId organizationId, ILabelCreation labelCreation, CancellationToken cancellationToken)
        {
            return m_DataSource.CreateLabelAsync(organizationId, labelCreation.From(), cancellationToken);
        }

        /// <inheritdoc />
        public LabelQueryBuilder QueryLabels(OrganizationId organizationId)
        {
            return new LabelQueryBuilder(m_DataSource, CacheConfiguration, organizationId);
        }

        /// <inheritdoc />
        public Task<ILabel> GetLabelAsync(LabelDescriptor labelDescriptor, CancellationToken cancellationToken)
        {
            return LabelEntity.GetConfiguredAsync(m_DataSource, CacheConfiguration, labelDescriptor, null, cancellationToken);
        }

        /// <inheritdoc />
        public StatusFlowQueryBuilder QueryStatusFlows(OrganizationId organizationId)
        {
            return new StatusFlowQueryBuilder(m_DataSource, organizationId);
        }

        /// <inheritdoc />
        public AssetDescriptor DeserializeAssetIdentifiers(string jsonSerialization)
        {
            // Verify old deprecated serialization format first
            var ids = IsolatedSerialization.DeserializeWithDefaultConverters<AssetIdentifier>(jsonSerialization);
            var projectId = ids.ProjectId.ToString();
            if (!string.IsNullOrEmpty(projectId) && projectId != ProjectId.None.ToString())
            {
                return ids.From();
            }

            return AssetDescriptor.FromJson(jsonSerialization);
        }

        /// <inheritdoc />
        public IAsset DeserializeAsset(string jsonSerialization)
        {
            if (jsonSerialization.Contains(AssetDataWithIdentifiers.SerializedType))
            {
                var data = IsolatedSerialization.DeserializeWithDefaultConverters<AssetDataWithIdentifiers>(jsonSerialization);
                // Consider a deserialized asset as having everything cached (with the exception of urls which are short-lived)
                var assetCacheConfiguration = new AssetCacheConfiguration
                {
                    CacheProperties = true,
                    CacheMetadata = true,
                    CacheSystemMetadata = true,
                    CacheDatasetList = true,
                    DatasetCacheConfiguration = new DatasetCacheConfiguration
                    {
                        CacheProperties = true,
                        CacheMetadata = true,
                        CacheSystemMetadata = true,
                        CacheFileList = true,
                        FileCacheConfiguration = new FileCacheConfiguration
                        {
                            CacheProperties = true,
                            CacheMetadata = true,
                            CacheSystemMetadata = true
                        }
                    }
                };
                return data.From(m_DataSource, CacheConfiguration, assetCacheConfiguration);
            }

            return null;
        }
    }
}
