using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    static class MetadataExtensions
    {
        /// <summary>
        /// Returns a dictionary of metadata values.
        /// </summary>
        /// <param name="metadataValues">The <see cref="IMetadataContainer"/> to convert to a dictionary. </param>
        /// <returns>A dictionary of metadata values. </returns>
        public static Dictionary<string, object> ToObjectDictionary(this IReadOnlyDictionary<string, MetadataValue> metadataValues)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var entry in metadataValues)
            {
                dictionary.Add(entry.Key, entry.Value.GetValue());
            }

            return dictionary;
        }
    }
}
