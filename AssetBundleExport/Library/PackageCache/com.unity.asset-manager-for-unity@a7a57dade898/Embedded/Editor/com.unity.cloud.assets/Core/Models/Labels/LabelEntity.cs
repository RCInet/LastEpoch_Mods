using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    class LabelEntity : ILabel
    {
        readonly IAssetDataSource m_AssetDataSource;
        readonly AssetRepositoryCacheConfiguration m_DefaultCacheConfiguration;

        /// <inheritdoc/>
        public LabelDescriptor Descriptor { get; private set; }

        /// <inheritdoc/>
        public string Description => Properties.Description;

        /// <inheritdoc/>
        public bool IsSystemLabel => Properties.IsSystemLabel;

        /// <inheritdoc/>
        public bool IsAssignable => Properties.IsAssignable;

        /// <inheritdoc/>
        public Color DisplayColor => Properties.DisplayColor ?? Color.White;

        /// <inheritdoc/>
        public AuthoringInfo AuthoringInfo => Properties.AuthoringInfo;

        /// <inheritdoc/>
        public LabelCacheConfiguration CacheConfiguration { get; }

        internal LabelProperties Properties { get; set; }

        public LabelEntity(IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, LabelDescriptor descriptor, LabelCacheConfiguration? cacheConfigurationOverride = null)
        {
            m_AssetDataSource = assetDataSource;
            m_DefaultCacheConfiguration = defaultCacheConfiguration;
            Descriptor = descriptor;

            CacheConfiguration = cacheConfigurationOverride ?? new LabelCacheConfiguration(m_DefaultCacheConfiguration);
        }

        /// <inheritdoc/>
        public Task<ILabel> WithCacheConfigurationAsync(LabelCacheConfiguration labelCacheConfiguration, CancellationToken cancellationToken)
        {
            return GetConfiguredAsync(m_AssetDataSource, m_DefaultCacheConfiguration, Descriptor, labelCacheConfiguration, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.HasCachingRequirements)
            {
                var data = await m_AssetDataSource.GetLabelAsync(Descriptor, cancellationToken);
                this.MapFrom(data);
            }
        }

        /// <inheritdoc/>
        public async Task<LabelProperties> GetPropertiesAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.CacheProperties)
            {
                return Properties;
            }

            var data = await m_AssetDataSource.GetLabelAsync(Descriptor, cancellationToken);
            return data.From();
        }

        /// <inheritdoc/>
        public Task UpdateAsync(ILabelUpdate labelUpdate, CancellationToken cancellationToken)
        {
            return m_AssetDataSource.UpdateLabelAsync(Descriptor, labelUpdate.From(), cancellationToken);
        }

        /// <inheritdoc/>
        public async Task RenameAsync(string labelName, CancellationToken cancellationToken)
        {
            var labelUpdate = new LabelBaseData {Name = labelName};
            await m_AssetDataSource.UpdateLabelAsync(Descriptor, labelUpdate, cancellationToken);

            // On success, the descriptor must be modified immediately.
            Descriptor = new LabelDescriptor(Descriptor.OrganizationId, labelUpdate.Name);
        }

        /// <inheritdoc/>
        public Task ArchiveAsync(CancellationToken cancellationToken)
        {
            return m_AssetDataSource.UpdateLabelStatusAsync(Descriptor, true, cancellationToken);
        }

        /// <inheritdoc/>
        public Task UnarchiveAsync(CancellationToken cancellationToken)
        {
            return m_AssetDataSource.UpdateLabelStatusAsync(Descriptor, false, cancellationToken);
        }

        /// <summary>
        /// Returns a label configured with the specified cache configuration.
        /// </summary>
        internal static async Task<ILabel> GetConfiguredAsync(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, LabelDescriptor descriptor, LabelCacheConfiguration? configuration, CancellationToken cancellationToken)
        {
            var label = new LabelEntity(dataSource, defaultCacheConfiguration, descriptor, configuration);

            if (label.CacheConfiguration.HasCachingRequirements)
            {
                await label.RefreshAsync(cancellationToken);
            }

            return label;
        }
    }
}
