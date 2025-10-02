using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IMetadataContainer
    {
        /// <summary>
        /// Returns a <see cref="MetadataQueryBuilder"/> for filtering and fetching metadata.
        /// </summary>
        /// <returns>A <see cref="MetadataQueryBuilder"/> for defining and executing queries. </returns>
        MetadataQueryBuilder Query();

        /// <summary>
        /// Adds or updates the specified fields in the metadata dictionary.
        /// </summary>
        /// <param name="metadataObjects">A collection of metadata values to add or update. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result.</returns>
        /// <exception cref="ArgumentException">If the type of a dictionary value is not recognized as valid metadata type. </exception>
        /// <exception cref="InvalidArgumentException">If this version of the asset is frozen, because it cannot be modified. </exception>
        /// <remarks>Can only be called if the version of the asset is unfrozen. </remarks>
        Task AddOrUpdateAsync(IReadOnlyDictionary<string, MetadataValue> metadataObjects, CancellationToken cancellationToken);

        /// <summary>
        /// Adds or updates the specified field in the metadata dictionary.
        /// </summary>
        /// <param name="key">The <see cref="FieldDefinitionDescriptor.FieldKey"/> of a corresponding <see cref="IFieldDefinition"/>. </param>
        /// <param name="metadataValue">The value of the field. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result.</returns>
        /// <exception cref="ArgumentException">If <paramref name="metadataValue"/> type is not a valid metadata type. </exception>
        /// <exception cref="InvalidArgumentException">If this version of the asset is frozen, because it cannot be modified. </exception>
        /// <remarks>Can only be called if the version of the asset is unfrozen. </remarks>
        Task AddOrUpdateAsync(string key, MetadataValue metadataValue, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the specified fields from the metadata dictionary.
        /// </summary>
        /// <param name="keys">The keys to remove from this dictionary. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result.</returns>
        /// <exception cref="InvalidArgumentException">If this version of the asset is frozen, because it cannot be modified. </exception>
        /// <remarks>Can only be called if the version of the asset is unfrozen. </remarks>
        Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken);
    }
}
