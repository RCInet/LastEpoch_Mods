using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    class ReadOnlyMetadataContainerEntity : IReadOnlyMetadataContainer
    {
        protected readonly IMetadataDataSource m_DataSource;

        internal Dictionary<string, MetadataObject> Properties { get; set; }

        public ReadOnlyMetadataContainerEntity(IMetadataDataSource dataSource)
        {
            m_DataSource = dataSource;
        }

        /// <summary>
        /// Refreshes the metadata dictionary.
        /// </summary>
        /// <param name="keys">The subset of keys to include in the dictionary; if empty or null all keys will be included. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result.</returns>
        public async Task<Dictionary<string, MetadataValue>> GetMetadataAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var keyList = keys?.ToHashSet() ?? new HashSet<string>();

            var properties = Properties ?? await m_DataSource.GetAsync(keyList, cancellationToken);

            if (keyList.Count == 0)
            {
                return properties.ToDictionary(kvp => kvp.Key, kvp => (MetadataValue) kvp.Value);
            }

            return properties.Where(kvp => keyList.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => (MetadataValue) kvp.Value);
        }

        /// <inheritdoc />
        public MetadataQueryBuilder Query()
        {
            return new MetadataQueryBuilder(this);
        }
    }
}
