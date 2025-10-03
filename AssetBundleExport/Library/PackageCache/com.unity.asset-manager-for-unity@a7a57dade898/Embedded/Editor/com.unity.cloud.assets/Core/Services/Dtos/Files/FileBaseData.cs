using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class FileBaseData : IFileBaseData
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }

        /// <inheritdoc />
        public Dictionary<string, object> Metadata { get; set; }

        /// <inheritdoc />
        public Dictionary<string, object> SystemMetadata { get; set; }
    }
}
