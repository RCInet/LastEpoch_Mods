using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IDatasetCreation : IDatasetInfo
    {
        /// <inheritdoc cref="IDataset.Metadata"/>
        Dictionary<string, MetadataValue> Metadata { get; }
    }
}
