using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.AssetsEmbedded
{
    abstract class DatasetInfo : IDatasetInfo
    {
        /// <inheritdoc />
        public string Name { get; set; }
        
        /// <inheritdoc />
        public AssetType? Type { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public List<string> Tags { get; set; }

        /// <inheritdoc />
        public bool? IsVisible { get; set; }

        protected DatasetInfo() { }

        [Obsolete("Use the default constructor.")]
        protected DatasetInfo(string name)
        {
            Name = name;
        }

        [Obsolete("Use the default constructor.")]
        protected DatasetInfo(IDataset dataset)
            : this(dataset.Name)
        {
            Description = dataset.Description;
            Tags = dataset.Tags?.ToList() ?? new List<string>();
            IsVisible = dataset.IsVisible;
        }
    }
}
