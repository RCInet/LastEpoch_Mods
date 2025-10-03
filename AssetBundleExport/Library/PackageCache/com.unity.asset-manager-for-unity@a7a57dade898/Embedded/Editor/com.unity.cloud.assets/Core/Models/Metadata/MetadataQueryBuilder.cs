using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that builds and executes a query to return a set of metadata.
    /// </summary>
    class MetadataQueryBuilder
    {
        readonly ReadOnlyMetadataContainerEntity m_MetadataContainerEntity;

        IEnumerable<string> m_Keys;

        internal MetadataQueryBuilder(ReadOnlyMetadataContainerEntity metadataContainer)
        {
            m_MetadataContainerEntity = metadataContainer;
        }

        /// <summary>
        /// Sets the query to return the metadata for the specified keys.
        /// </summary>
        /// <param name="keys">The collection of desired keys. </param>
        /// <returns>The calling <see cref="MetadataQueryBuilder"/>. </returns>
        public MetadataQueryBuilder SelectWhereKeyEquals(params string[] keys)
        {
            m_Keys = keys;
            return this;
        }

        /// <summary>
        /// Sets the query to return metadata for all keys.
        /// </summary>
        /// <returns>The calling <see cref="MetadataQueryBuilder"/>. </returns>
        public MetadataQueryBuilder SelectAll()
        {
            m_Keys = null;
            return this;
        }

        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>An async enumeration of key value pairs of a string key and <see cref="MetadataValue"/> value. </returns>
        public async IAsyncEnumerable<KeyValuePair<string, MetadataValue>> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var results = await m_MetadataContainerEntity.GetMetadataAsync(m_Keys, cancellationToken);
            foreach (var (key, value) in results)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return new KeyValuePair<string, MetadataValue>(key, value);
            }
        }
    }
}
