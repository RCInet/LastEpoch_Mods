using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IFileCreateData : IFileBaseData
    {
        [DataMember(Name = "filePath")]
        string Path { get; }

        [DataMember(Name = "fileSize")]
        long SizeBytes { get; }

        [DataMember(Name = "userChecksum")]
        string UserChecksum { get; }
    }
}
