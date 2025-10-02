using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The properties of an <see cref="IAssetProject"/>.
    /// </summary>
    struct AssetProjectProperties
    {
        /// <summary>
        /// The project name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The date time of creation the project.
        /// </summary>
        public DateTime Created { get; internal set; }

        /// <summary>
        /// The date time of last update the project.
        /// </summary>
        public DateTime Updated { get; internal set; }

        /// <summary>
        /// Additional project metadata.
        /// </summary>
        public Dictionary<string, string> Metadata { get; internal set; }

        /// <summary>
        /// Whether the project has any collections
        /// </summary>
        public bool HasCollection { get; internal set; }
    }
}
