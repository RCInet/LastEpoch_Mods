using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    class FieldDefinitionEntity : IFieldDefinition
    {
        private protected readonly IAssetDataSource m_DataSource;
        private protected readonly AssetRepositoryCacheConfiguration m_DefaultCacheConfiguration;

        /// <inheritdoc/>
        public FieldDefinitionDescriptor Descriptor { get; }

        /// <inheritdoc/>
        public FieldDefinitionType Type => Properties.Type;

        /// <inheritdoc/>
        public bool IsDeleted => Properties.IsDeleted;

        /// <inheritdoc/>
        public string DisplayName => Properties.DisplayName;

        /// <inheritdoc/>
        public AuthoringInfo AuthoringInfo => Properties.AuthoringInfo;

        /// <inheritdoc/>
        public FieldDefinitionOrigin Origin => Properties.Origin;

        /// <inheritdoc/>
        public FieldDefinitionCacheConfiguration CacheConfiguration { get; }

        internal FieldDefinitionProperties Properties { get; set; }

        internal FieldDefinitionEntity(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, FieldDefinitionDescriptor descriptor, FieldDefinitionCacheConfiguration? localConfiguration = null)
        {
            m_DataSource = dataSource;
            m_DefaultCacheConfiguration = defaultCacheConfiguration;
            Descriptor = descriptor;

            CacheConfiguration = localConfiguration ?? new FieldDefinitionCacheConfiguration(m_DefaultCacheConfiguration);
        }

        /// <inheritdoc/>
        public Task<IFieldDefinition> WithCacheConfigurationAsync(FieldDefinitionCacheConfiguration fieldDefinitionCacheConfiguration, CancellationToken cancellationToken)
        {
            return GetConfiguredAsync(m_DataSource, m_DefaultCacheConfiguration, Descriptor, fieldDefinitionCacheConfiguration, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.HasCachingRequirements)
            {
                var data = await m_DataSource.GetFieldDefinitionAsync(Descriptor, cancellationToken);
                this.MapFrom(data);
            }
        }

        /// <inheritdoc/>
        public async Task<FieldDefinitionProperties> GetPropertiesAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.CacheProperties)
            {
                return Properties;
            }

            var data = await m_DataSource.GetFieldDefinitionAsync(Descriptor, cancellationToken);
            return data.From();
        }

        /// <inheritdoc/>
        public Task UpdateAsync(IFieldDefinitionUpdate definitionUpdate, CancellationToken cancellationToken)
        {
            return m_DataSource.UpdateFieldDefinitionAsync(Descriptor, definitionUpdate.From(), cancellationToken);
        }

        /// <summary>
        /// Returns the field definition rebuilt as a selection field definition.
        /// </summary>
        internal SelectionFieldDefinitionEntity AsSelectionFieldDefinitionEntity()
        {
            if (CacheConfiguration.CacheProperties && Properties.Type != FieldDefinitionType.Selection)
            {
                throw new InvalidCastException("Field definition is not a selection field definition.");
            }

            return new SelectionFieldDefinitionEntity(m_DataSource, m_DefaultCacheConfiguration, Descriptor, CacheConfiguration)
            {
                Properties = Properties,
            };
        }

        /// <summary>
        /// Returns a field definition configured with the specified caching configuration.
        /// </summary>
        internal static async Task<IFieldDefinition> GetConfiguredAsync(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, FieldDefinitionDescriptor descriptor, FieldDefinitionCacheConfiguration? fieldDefinitionCacheConfiguration, CancellationToken cancellationToken)
        {
            var configuration = fieldDefinitionCacheConfiguration ?? new FieldDefinitionCacheConfiguration(defaultCacheConfiguration);

            // If the field definition has caching requirements, allow the data to determine the type of field definition.
            if (configuration.HasCachingRequirements)
            {
                var data = await dataSource.GetFieldDefinitionAsync(descriptor, cancellationToken);
                return data.From(dataSource, defaultCacheConfiguration, descriptor, configuration);
            }

            // When there are no caching requirements, return the basic field definition entity, it will be safe to cast to a selection field definition if needed.
            return new FieldDefinitionEntity(dataSource, defaultCacheConfiguration, descriptor, configuration);
        }
    }
}
