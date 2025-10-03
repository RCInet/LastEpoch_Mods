using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class CreateAssetReferenceRequestBody : IAssetReferenceRequestBody
    {
        [DataMember(Name = "assetVersion")]
        public string AssetVersion { get; set; }

        [DataMember(Name = "target")]
        public AssetIdentifierDto Target { get; set; }
    }
}
