using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IFieldDefinition
    {
        /// <summary>
        /// The descriptor for the field.
        /// </summary>
        FieldDefinitionDescriptor Descriptor { get; }

        /// <summary>
        /// The type of the field.
        /// </summary>
        [Obsolete("Use FieldDefinitionProperties.Type instead.")]
        FieldDefinitionType Type { get; }

        /// <summary>
        /// Whether the field is deleted.
        /// </summary>
        [Obsolete("Use FieldDefinitionProperties.IsDeleted instead.")]
        bool IsDeleted { get; }

        /// <summary>
        /// The display name for the field.
        /// </summary>
        [Obsolete("Use FieldDefinitionProperties.DisplayName instead.")]
        string DisplayName { get; }

        /// <summary>
        /// The creation and update information of the field.
        /// </summary>
        [Obsolete("Use FieldDefinitionProperties.AuthoringInfo instead.")]
        AuthoringInfo AuthoringInfo { get; }

        /// <summary>
        /// The originator of the field.
        /// </summary>
        [Obsolete("Use FieldDefinitionProperties.Origin instead.")]
        FieldDefinitionOrigin Origin => throw new NotImplementedException();

        /// <summary>
        /// The caching configuration for the field definition.
        /// </summary>
        FieldDefinitionCacheConfiguration CacheConfiguration => throw new NotImplementedException();

        /// <summary>
        /// Returns a field definition configured with the specified caching configuration.
        /// </summary>
        /// <param name="fieldDefinitionCacheConfiguration">The caching configuration for the field definition. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IFieldDefinition"/> with cached values specified by the caching configurations. </returns>
        Task<IFieldDefinition> WithCacheConfigurationAsync(FieldDefinitionCacheConfiguration fieldDefinitionCacheConfiguration, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Refreshes the field to retrieve the latest values.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RefreshAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the properties of the field definition.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the <see cref="FieldDefinitionProperties"/> of the field definition. </returns>
        Task<FieldDefinitionProperties> GetPropertiesAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Syncronizes local changes to the field definition to the data source.
        /// </summary>
        /// <param name="definitionUpdate">The object containing the information to update. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task UpdateAsync(IFieldDefinitionUpdate definitionUpdate, CancellationToken cancellationToken);

    }
}
