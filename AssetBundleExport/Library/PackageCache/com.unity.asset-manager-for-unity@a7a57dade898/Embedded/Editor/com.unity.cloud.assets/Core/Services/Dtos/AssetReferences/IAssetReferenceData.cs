using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IAssetReferenceData
    {
        [DataMember(Name = "referenceId")]
        public string ReferenceId { get; set; }

        [DataMember(Name = "isValid")]
        public bool IsValid { get; set; }

        [DataMember(Name = "source")]
        public AssetIdentifierDto Source { get; set; }

        [DataMember(Name = "target")]
        public AssetIdentifierDto Target { get; set; }
    }
}
