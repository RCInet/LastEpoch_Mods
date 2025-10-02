using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    class DatasetCreation : DatasetInfo, IDatasetCreation
    {
        /// <inheritdoc/>
        public Dictionary<string, MetadataValue> Metadata { get; set; }

        public DatasetCreation(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "The name of the dataset cannot be null or empty.");
            }
            Name = name;
        }
    }
}
