using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IFileBaseData : IMetadataInfo
    {
        [DataMember(Name = "description")]
        string Description { get; }

        [DataMember(Name = "tags")]
        IEnumerable<string> Tags { get; }
    }
}
