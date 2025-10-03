using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial interface IAssetDataSource
    {
        /// <summary>
        /// Retrieves the field definitions in an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <param name="pagination">An object containing the necessary information return a range of field definitions.</param>
        /// <param name="queryParameters">Optional query parameters.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is an async enumeration of field definitions. </returns>
        IAsyncEnumerable<IFieldDefinitionData> ListFieldDefinitionsAsync(OrganizationId organizationId, PaginationData pagination, Dictionary<string, string> queryParameters, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the specified field definition.
        /// </summary>
        /// <param name="fieldDefinitionDescriptor">The descriptor that identifies the field defintion. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is an <see cref="IFieldDefinitionData"/></returns>
        Task<IFieldDefinitionData> GetFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new metadata field definition in an organization.
        /// </summary>
        /// <param name="organizationId">The organization id. </param>
        /// <param name="fieldCreation">The object containing the necessary information to create a field definition. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a newly created field. </returns>
        Task<FieldDefinitionDescriptor> CreateFieldDefinitionAsync(OrganizationId organizationId, IFieldDefinitionCreateData fieldCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the specified field definition.
        /// </summary>
        /// <param name="fieldDefinitionDescriptor">The descriptor that identifies the field defintion. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task DeleteFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the specified field definition.
        /// </summary>
        /// <param name="fieldDefinitionDescriptor">The descriptor that identifies the field defintion. </param>
        /// <param name="fieldUpdate">The object containing the necessary information to update a field definition. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UpdateFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, IFieldDefinitionBaseData fieldUpdate, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the accepted values of the specified field definition.
        /// </summary>
        /// <param name="fieldDefinitionDescriptor">The descriptor that identifies the field defintion. </param>
        /// <param name="acceptedValues">The collection of values to append to the acceptance list. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task AddAcceptedValuesToFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, IEnumerable<string> acceptedValues, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the accepted values of the specified field definition.
        /// </summary>
        /// <param name="fieldDefinitionDescriptor">The descriptor that identifies the field defintion. </param>
        /// <param name="acceptedValues">The collection of values to remove from the acceptance list. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task RemoveAcceptedValuesFromFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, IEnumerable<string> acceptedValues, CancellationToken cancellationToken);
    }
}
