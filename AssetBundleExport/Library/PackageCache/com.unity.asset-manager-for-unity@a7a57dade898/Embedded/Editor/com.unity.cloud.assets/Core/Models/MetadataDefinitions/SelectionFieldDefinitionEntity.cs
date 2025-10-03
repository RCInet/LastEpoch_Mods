using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    class SelectionFieldDefinitionEntity : FieldDefinitionEntity, ISelectionFieldDefinition
    {
        /// <inheritdoc/>
        public IEnumerable<string> AcceptedValues => Properties.AcceptedValues;

        /// <inheritdoc/>
        public bool Multiselection => Properties.Multiselection;

        internal SelectionFieldDefinitionEntity(IAssetDataSource dataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration, FieldDefinitionDescriptor descriptor, FieldDefinitionCacheConfiguration? localConfiguration = null)
            : base(dataSource, defaultCacheConfiguration, descriptor, localConfiguration) { }

        /// <inheritdoc/>
        public new async Task<ISelectionFieldDefinition> WithCacheConfigurationAsync(FieldDefinitionCacheConfiguration fieldDefinitionCacheConfiguration, CancellationToken cancellationToken)
        {
            // If the field definition has caching requirements, allow the data to determine the type of field definition.
            if (fieldDefinitionCacheConfiguration.HasCachingRequirements)
            {
                var data = await m_DataSource.GetFieldDefinitionAsync(Descriptor, cancellationToken);
                return data.From(m_DataSource, m_DefaultCacheConfiguration, Descriptor, fieldDefinitionCacheConfiguration) as ISelectionFieldDefinition;
            }

            // When there are no caching requirements, return the basic field definition entity, it will be safe to cast to a selection field definition if needed.
            return new SelectionFieldDefinitionEntity(m_DataSource, m_DefaultCacheConfiguration, Descriptor, fieldDefinitionCacheConfiguration);
        }

        /// <inheritdoc/>
        public new async Task<SelectionFieldDefinitionProperties> GetPropertiesAsync(CancellationToken cancellationToken)
        {
            if (CacheConfiguration.CacheProperties)
            {
                return Properties.From();
            }

            var data = await m_DataSource.GetFieldDefinitionAsync(Descriptor, cancellationToken);
            return data.From().From();
        }

        /// <inheritdoc/>
        public Task SetSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken)
        {
            var definitionUpdate = new FieldDefinitionBaseData
            {
                AcceptedValues = acceptedValues.ToArray()
            };
            return m_DataSource.UpdateFieldDefinitionAsync(Descriptor, definitionUpdate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task AddSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken)
        {
            return m_DataSource.AddAcceptedValuesToFieldDefinitionAsync(Descriptor, acceptedValues, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken)
        {
            return m_DataSource.RemoveAcceptedValuesFromFieldDefinitionAsync(Descriptor, acceptedValues, cancellationToken);
        }
    }
}
