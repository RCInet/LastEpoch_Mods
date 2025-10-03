using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IDatasetInfo
    {
        /// <inheritdoc cref="DatasetProperties.Name"/>
        string Name { get; }
        
        /// <inheritdoc cref="DatasetProperties.Type"/>
        AssetType? Type { get; }

        /// <inheritdoc cref="DatasetProperties.Description"/>
        string Description { get; }

        /// <inheritdoc cref="DatasetProperties.Tags"/>
        List<string> Tags { get; }

        /// <inheritdoc cref="DatasetProperties.IsVisible"/>
        bool? IsVisible { get; }
    }
}
