using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This object contains all the information about an updated asset.
    /// </summary>
    [DataContract]
    class AssetBaseData : IAssetBaseData
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }

        /// <inheritdoc />
        public string Type { get; set; }

        /// <inheritdoc />
        public Dictionary<string, object> Metadata { get; set; }

        /// <inheritdoc />
        public Dictionary<string, object> SystemMetadata { get; set; }
    }
}
