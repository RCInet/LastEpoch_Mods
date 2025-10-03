using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IMetadataInfo
    {
        /// <summary>
        /// The user metadata.
        /// </summary>
        [DataMember(Name = "metadata")]
        Dictionary<string, object> Metadata { get; }

        /// <summary>
        /// The system metadata.
        /// </summary>
        [DataMember(Name = "systemMetadata")]
        Dictionary<string, object> SystemMetadata { get; }
    }
}
