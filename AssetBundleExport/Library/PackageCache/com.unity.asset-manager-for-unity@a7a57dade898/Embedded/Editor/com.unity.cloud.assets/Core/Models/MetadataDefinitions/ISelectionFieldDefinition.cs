using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    interface ISelectionFieldDefinition : IFieldDefinition
    {
        /// <summary>
        /// The accepted values of the field.
        /// <remarks>This is only required for field definitions of type <see cref="FieldDefinitionType.Selection"/>.</remarks>
        /// </summary>
        [Obsolete("Use SelectionFieldDefinitionProperties.AcceptedValues instead.")]
        IEnumerable<string> AcceptedValues { get; }

        /// <summary>
        /// Whether the field can have multiple values.
        /// <remarks>This is only requred for field definitions of type <see cref="FieldDefinitionType.Selection"/>.</remarks>
        /// </summary>
        [Obsolete("Use SelectionFieldDefinitionProperties.Multiselection instead.")]
        bool Multiselection { get; }

        /// <summary>
        /// Returns a field definition configured with the specified caching configuration.
        /// </summary>
        /// <param name="fieldDefinitionCacheConfiguration">The caching configuration for the field definition. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="ISelectionFieldDefinition"/> with cached values specified by the caching configurations. </returns>
        new Task<ISelectionFieldDefinition> WithCacheConfigurationAsync(FieldDefinitionCacheConfiguration fieldDefinitionCacheConfiguration, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Returns the properties of the field definition.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the <see cref="FieldDefinitionProperties"/> of the field definition. </returns>
        new Task<SelectionFieldDefinitionProperties> GetPropertiesAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Sets the parameter list as the accepted values of the field.
        /// </summary>
        /// <param name="acceptedValues">An enumeration of accepted values. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task SetSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken);

        /// <summary>
        /// Appends the parameter list to the accepted values of the field.
        /// </summary>
        /// <param name="acceptedValues">An enumeration of accepted values. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task AddSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the parameter list from the accepted values of the field.
        /// </summary>
        /// <param name="acceptedValues">An enumeration of accepted values. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task RemoveSelectionValuesAsync(IEnumerable<string> acceptedValues, CancellationToken cancellationToken);
    }
}
