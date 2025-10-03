using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IDatasetUpdateData : IDatasetBaseData
    {
        [DataMember(Name = "filesOrder")]
        IEnumerable<string> FileOrder { get; }
    }
}
