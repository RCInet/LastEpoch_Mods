using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Interface for exposing metadata data source operations.
    /// </summary>
    interface IMetadataDataSource
    {
        /// <summary>
        /// Retunrs metadata objects for the given keys.
        /// </summary>
        /// <param name="keys">The keys of the requested metadata. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a dictionary of metadata values. </returns>
        Task<Dictionary<string, MetadataObject>> GetAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Adds or updates the specified fields in the metadata dictionary.
        /// </summary>
        /// <param name="properties">The properties to add or update. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task AddOrUpdateAsync(Dictionary<string, object> properties, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the specified fields from the metadata dictionary.
        /// </summary>
        /// <param name="keys">The keys to remove. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken);
    }
}
