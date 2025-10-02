using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct AssetVersionDto
    {
        [DataMember(Name = "assetVersion")]
        public string Version { get; set; }
    }
}
