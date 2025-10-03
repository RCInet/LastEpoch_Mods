using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct AssetReferenceDescriptorDto
    {
        [DataMember(Name = "assetDescriptor")]
        public string AssetDescriptor { get; set; }

        [DataMember(Name = "referenceId")]
        public string ReferenceId { get; set; }
    }
}
