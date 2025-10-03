using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class TransformationEntity : ITransformation
    {
        readonly IAssetDataSource m_DataSource;
        readonly AssetRepositoryCacheConfiguration m_DefaultCacheConfiguration;

        /// <inheritdoc />
        public TransformationDescriptor Descriptor { get; }

        /// <inheritdoc />
        public DatasetId OutputDatasetId => Properties.OutputDatasetId;

        /// <inheritdoc />
        public DatasetId LinkDatasetId => Properties.LinkDatasetId;

        /// <inheritdoc />
        public IEnumerable<string> InputFiles => Properties.InputFilePaths;

        /// <inheritdoc />
        public WorkflowType WorkflowType => Properties.WorkflowType;

        /// <inheritdoc />
        public string WorkflowName => Properties.WorkflowName;

        /// <inheritdoc />
        public TransformationStatus Status => Properties.Status;

        /// <inheritdoc />
        public string ErrorMessage => Properties.ErrorMessage;

        /// <inheritdoc />
        public int Progress => Properties.Progress;

        /// <inheritdoc />
        public DateTime CreatedOn => Properties.Created;

        /// <inheritdoc />
        public DateTime UpdatedAt => Properties.Updated;

        /// <inheritdoc />
        public DateTime StartedAt => Properties.Started;

        /// <inheritdoc />
        public UserId UserId => Properties.UserId;

        /// <inheritdoc/>
        public string JobId => Properties.JobId;

        /// <inheritdoc/>
        public TransformationCacheConfiguration CacheConfiguration { get; }

        internal TransformationProperties Properties { get; set; }

        internal TransformationEntity(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, TransformationDescriptor descriptor, TransformationCacheConfiguration? cacheConfigurationOverride = null)
        {
            m_DataSource = dataSource;
            m_DefaultCacheConfiguration = defaultCacheConfiguration;
            Descriptor = descriptor;

            CacheConfiguration = cacheConfigurationOverride ?? new TransformationCacheConfiguration(m_DefaultCacheConfiguration);
        }

        /// <inheritdoc />
        public Task<ITransformation> WithCacheConfigurationAsync(TransformationCacheConfiguration transformationCacheConfiguration, CancellationToken cancellationToken)
        {
            return GetConfiguredAsync(m_DataSource, m_DefaultCacheConfiguration, Descriptor, transformationCacheConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        public async Task RefreshAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.HasCachingRequirements)
            {
                var data = await m_DataSource.GetTransformationAsync(Descriptor, cancellationToken);
                this.MapFrom(data);
            }
        }

        /// <inheritdoc />
        public async Task<TransformationProperties> GetPropertiesAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.CacheProperties)
            {
                return Properties;
            }

            var data = await m_DataSource.GetTransformationAsync(Descriptor, cancellationToken);
            return data.From();
        }

        /// <inheritdoc />
        public async Task TerminateAsync(CancellationToken cancellationToken)
        {
            await m_DataSource.TerminateTransformationAsync(Descriptor.DatasetDescriptor.AssetDescriptor.ProjectDescriptor, Descriptor.TransformationId, cancellationToken);
        }

        /// <summary>
        /// Returns a transformation configured with the specified cache configuration.
        /// </summary>
        internal static async Task<ITransformation> GetConfiguredAsync(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, TransformationDescriptor descriptor, TransformationCacheConfiguration? configuration, CancellationToken cancellationToken)
        {
            var transformation = new TransformationEntity(dataSource, defaultCacheConfiguration, descriptor, configuration);

            if (transformation.CacheConfiguration.HasCachingRequirements)
            {
                await transformation.RefreshAsync(cancellationToken);
            }

            return transformation;
        }
    }
}
