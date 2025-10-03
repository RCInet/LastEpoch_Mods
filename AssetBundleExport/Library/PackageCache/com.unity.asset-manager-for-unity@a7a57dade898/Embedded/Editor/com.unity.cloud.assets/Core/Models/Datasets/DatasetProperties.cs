using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The properties of an <see cref="IDataset"/>.
    /// </summary>
    struct DatasetProperties
    {
        /// <summary>
        /// The name of the dataset.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// A description of the dataset.
        /// </summary>
        public string Description { get; internal set; }
        
        /// <summary>
        /// The type of the dataset.
        /// </summary>
        public AssetType Type { get; internal set; }

        /// <summary>
        /// The user tags of the dataset.
        /// </summary>
        public IEnumerable<string> Tags { get; internal set; }

        /// <summary>
        /// The system tags of the dataset.
        /// </summary>
        public IEnumerable<string> SystemTags { get; internal set; }

        /// <summary>
        /// The status of the dataset.
        /// </summary>
        public string StatusName { get; internal set; }

        /// <summary>
        /// The authoring info of the dataset.
        /// </summary>
        public AuthoringInfo AuthoringInfo { get; internal set; }

        /// <summary>
        /// The order of the files in the dataset.
        /// </summary>
        public IEnumerable<string> FileOrder { get; internal set; }

        /// <summary>
        /// Indicates whether the dataset is visible or not.
        /// </summary>
        public bool IsVisible { get; internal set; }
        
        /// <summary>
        /// The name of the workflow which outputs to the dataset.
        /// </summary>
        public string WorkflowName { get; internal set; }
    }
}
