using System;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct CreatedAssetDto
    {
        [DataMember(Name = "assetId")]
        public AssetId AssetId { get; set; }

        [DataMember(Name = "assetVersion")]
        public AssetVersion AssetVersion { get; set; }

        [DataMember(Name = "datasets")]
        public DatasetData[] Datasets { get; set; }
    }
}
