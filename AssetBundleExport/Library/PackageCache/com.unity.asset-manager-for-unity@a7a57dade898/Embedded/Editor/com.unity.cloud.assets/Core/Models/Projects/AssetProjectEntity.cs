using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This class contains all the information about a cloud project.
    /// </summary>
    sealed class AssetProjectEntity : IAssetProject
    {
        readonly IAssetDataSource m_DataSource;
        readonly AssetRepositoryCacheConfiguration m_DefaultCacheConfiguration;

        /// <inheritdoc />
        public ProjectDescriptor Descriptor { get; }

        /// <inheritdoc />
        public string Name
        {
            get => Properties.Name;
            set { }
        }

        /// <inheritdoc />
        public IDeserializable Metadata
        {
            get => new JsonObject(Properties.Metadata);
            set { }
        }

        /// <inheritdoc/>
        public bool HasCollection => Properties.HasCollection;

        /// <inheritdoc/>
        public AssetProjectCacheConfiguration CacheConfiguration { get; }

        internal AssetProjectProperties Properties { get; set; }

        internal AssetProjectEntity(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, ProjectDescriptor projectDescriptor, AssetProjectCacheConfiguration? localCacheConfiguration = null)
        {
            m_DataSource = dataSource;
            m_DefaultCacheConfiguration = defaultCacheConfiguration;
            Descriptor = projectDescriptor;

            CacheConfiguration = localCacheConfiguration ?? new AssetProjectCacheConfiguration(m_DefaultCacheConfiguration);
        }

        /// <inheritdoc />
        public Task<IAssetProject> WithCacheConfigurationAsync(AssetProjectCacheConfiguration assetProjectCacheConfiguration, CancellationToken cancellationToken)
        {
            return GetConfiguredAsync(m_DataSource, m_DefaultCacheConfiguration, Descriptor, assetProjectCacheConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        public async Task RefreshAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.HasCachingRequirements)
            {
                var data = await m_DataSource.GetProjectAsync(Descriptor, cancellationToken);
                this.MapFrom(data);
            }
        }

        /// <inheritdoc />
        public async Task<AssetProjectProperties> GetPropertiesAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.CacheProperties)
            {
                return Properties;
            }

            var data = await m_DataSource.GetProjectAsync(Descriptor, cancellationToken);
            return data.From();
        }

        /// <inheritdoc />
        public async Task<IAsset> GetAssetAsync(AssetId assetId, CancellationToken cancellationToken)
        {
            var filter = new AssetSearchFilter();
            filter.Include().Id.WithValue(assetId.ToString());

            var query = QueryAssets()
                .SelectWhereMatchesFilter(filter)
                .LimitTo(new Range(0, 1))
                .ExecuteAsync(cancellationToken);

            IAsset asset = null;

            var enumerator = query.GetAsyncEnumerator(cancellationToken);
            if (await enumerator.MoveNextAsync())
            {
                asset = enumerator.Current;
            }

            await enumerator.DisposeAsync();

            return asset;
        }

        /// <inheritdoc />
        public Task<IAsset> GetAssetAsync(AssetId assetId, AssetVersion assetVersion, CancellationToken cancellationToken)
        {
            var assetDescriptor = new AssetDescriptor(Descriptor, assetId, assetVersion);
            return AssetEntity.GetConfiguredAsync(m_DataSource, m_DefaultCacheConfiguration, assetDescriptor, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IAsset> GetAssetAsync(AssetId assetId, string label, CancellationToken cancellationToken)
        {
            var fieldsFilter = m_DefaultCacheConfiguration.GetAssetFieldsFilter();
            var data = await m_DataSource.GetAssetAsync(Descriptor, assetId, label, fieldsFilter, cancellationToken);
            return data.From(m_DataSource, m_DefaultCacheConfiguration, Descriptor, fieldsFilter);
        }

        /// <inheritdoc />
        public VersionQueryBuilder QueryAssetVersions(AssetId assetId)
        {
            return new VersionQueryBuilder(m_DataSource, m_DefaultCacheConfiguration, Descriptor, assetId);
        }

        /// <inheritdoc />
        public AssetReferenceQueryBuilder QueryAssetReferences(AssetId assetId)
        {
            return new AssetReferenceQueryBuilder(m_DataSource, Descriptor, assetId);
        }

        /// <inheritdoc />
        public async Task<IAsset> CreateAssetAsync(IAssetCreation assetCreation, CancellationToken cancellationToken)
        {
            var assetDescriptor = await CreateAssetLiteAsync(assetCreation, cancellationToken);
            return await GetAssetAsync(assetDescriptor.AssetId, assetDescriptor.AssetVersion, cancellationToken);
        }

        /// <inheritdoc />
        public Task<AssetDescriptor> CreateAssetLiteAsync(IAssetCreation assetCreation, CancellationToken cancellationToken)
        {
            return m_DataSource.CreateAssetAsync(Descriptor, assetCreation.From(), cancellationToken);
        }

        /// <inheritdoc />
        public AssetQueryBuilder QueryAssets()
        {
            return new AssetQueryBuilder(m_DataSource, m_DefaultCacheConfiguration, Descriptor);
        }

        /// <inheritdoc />
        public GroupAndCountAssetsQueryBuilder GroupAndCountAssets()
        {
            return new GroupAndCountAssetsQueryBuilder(m_DataSource, Descriptor);
        }

        /// <inheritdoc />
        public Task<int> CountAssetsAsync(CancellationToken cancellationToken)
        {
            return m_DataSource.GetAssetCountAsync(Descriptor, cancellationToken);
        }

        /// <inheritdoc />
        public Task LinkAssetsAsync(ProjectDescriptor sourceProjectDescriptor, IEnumerable<AssetId> assetIds, CancellationToken cancellationToken)
        {
            return m_DataSource.LinkAssetsToProjectAsync(sourceProjectDescriptor, Descriptor, assetIds, cancellationToken);
        }

        /// <inheritdoc />
        public Task UnlinkAssetsAsync(IEnumerable<AssetId> assetIds, CancellationToken cancellationToken)
        {
            return m_DataSource.UnlinkAssetsFromProjectAsync(Descriptor, assetIds, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteUnfrozenAssetVersionAsync(AssetId assetId, AssetVersion assetVersion, CancellationToken cancellationToken)
        {
            var assetDescriptor = new AssetDescriptor(Descriptor, assetId, assetVersion);
            return m_DataSource.DeleteUnfrozenAssetVersionAsync(assetDescriptor, cancellationToken);
        }

        /// <inheritdoc />
        public CollectionQueryBuilder QueryCollections()
        {
            return new CollectionQueryBuilder(m_DataSource, m_DefaultCacheConfiguration, Descriptor);
        }

        /// <inheritdoc />
        public Task<int> CountCollectionsAsync(CancellationToken cancellationToken)
        {
            return m_DataSource.GetCollectionCountAsync(Descriptor, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IAssetCollection> GetCollectionAsync(CollectionPath collectionPath, CancellationToken cancellationToken)
        {
            var descriptor = new CollectionDescriptor(Descriptor, collectionPath);
            return AssetCollection.GetConfiguredAsync(m_DataSource, m_DefaultCacheConfiguration, descriptor, null, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IAssetCollection> CreateCollectionAsync(IAssetCollectionCreation assetCollectionCreation, CancellationToken cancellationToken)
        {
            var collectionDescriptor = await CreateCollectionLiteAsync(assetCollectionCreation, cancellationToken);
            return await GetCollectionAsync(collectionDescriptor.Path, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CollectionDescriptor> CreateCollectionLiteAsync(IAssetCollectionCreation assetCollectionCreation, CancellationToken cancellationToken)
        {
            assetCollectionCreation.Validate();

            var creationPath = CollectionPath.CombinePaths(assetCollectionCreation.ParentPath, assetCollectionCreation.Name);

            var collectionPath = await m_DataSource.CreateCollectionAsync(Descriptor, assetCollectionCreation.From(), cancellationToken);
            if (creationPath != collectionPath)
            {
                throw new CreateCollectionFailedException($"Failed to create a collection at path {creationPath}");
            }

            return new CollectionDescriptor(Descriptor, collectionPath);
        }

        /// <inheritdoc />
        public Task DeleteCollectionAsync(CollectionPath collectionPath, CancellationToken cancellationToken)
        {
            return m_DataSource.DeleteCollectionAsync(new CollectionDescriptor(Descriptor, collectionPath), cancellationToken);
        }

        /// <inheritdoc />
        public TransformationQueryBuilder QueryTransformations()
        {
            return new TransformationQueryBuilder(m_DataSource, m_DefaultCacheConfiguration, Descriptor);
        }

        /// <summary>
        /// Returns a project configured with the specified cache configuration.
        /// </summary>
        internal static async Task<IAssetProject> GetConfiguredAsync(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, ProjectDescriptor descriptor, AssetProjectCacheConfiguration? configuration, CancellationToken cancellationToken)
        {
            var project = new AssetProjectEntity(dataSource, defaultCacheConfiguration, descriptor, configuration);

            if (project.CacheConfiguration.HasCachingRequirements)
            {
                await project.RefreshAsync(cancellationToken);
            }

            return project;
        }
    }
}
