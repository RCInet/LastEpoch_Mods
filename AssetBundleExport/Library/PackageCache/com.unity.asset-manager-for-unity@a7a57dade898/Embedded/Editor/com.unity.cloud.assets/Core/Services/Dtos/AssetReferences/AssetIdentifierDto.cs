using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct AssetIdentifierDto
    {
        [DataMember(Name = "assetId")]
        public string Id { get; set; }

        [DataMember(Name = "assetVersion")]
        public string Version { get; set; }

        [DataMember(Name = "labelName")]
        public string Label { get; set; }
    }
}
