using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IDatasetUpdate : IDatasetInfo
    {
        /// <inheritdoc cref="DatasetProperties.FileOrder"/>
        IReadOnlyList<string> FileOrder { get; }
    }
}
