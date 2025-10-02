using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    sealed class MetadataContainerEntity : ReadOnlyMetadataContainerEntity, IMetadataContainer
    {
        public MetadataContainerEntity(IMetadataDataSource dataSource)
            : base(dataSource) { }

        /// <inheritdoc />
        public Task AddOrUpdateAsync(IReadOnlyDictionary<string, MetadataValue> metadataObjects, CancellationToken cancellationToken)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var kvp in metadataObjects)
            {
                var value = kvp.Value.GetValue();

                ValidateMetadataValue(value);

                dictionary.Add(kvp.Key, value);
            }

            return AddOrUpdateAsync(dictionary, cancellationToken);
        }

        /// <inheritdoc />
        public Task AddOrUpdateAsync(string key, MetadataValue metadataValue, CancellationToken cancellationToken)
        {
            var value = metadataValue.GetValue();

            ValidateMetadataValue(value);

            return AddOrUpdateAsync(new Dictionary<string, object> {{key, value}}, cancellationToken);
        }

        async Task AddOrUpdateAsync(Dictionary<string, object> metadataValues, CancellationToken cancellationToken)
        {
            if (metadataValues == null || !metadataValues.Any()) return;

            await m_DataSource.AddOrUpdateAsync(metadataValues, cancellationToken);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var keyHashSet = new HashSet<string>(keys);

            if (!keyHashSet.Any()) return;

            await m_DataSource.RemoveAsync(keyHashSet, cancellationToken);
        }

        static void ValidateMetadataValue(object value)
        {
            switch (value)
            {
                case bool:
                case string:
                case IEnumerable<string>:
                case double or int or float or long or short or byte or sbyte or decimal:
                case DateTime:
                    return;
                default:
                    throw new ArgumentException($"Invalid metadata value type: {value.GetType()}");
            }
        }
    }
}
