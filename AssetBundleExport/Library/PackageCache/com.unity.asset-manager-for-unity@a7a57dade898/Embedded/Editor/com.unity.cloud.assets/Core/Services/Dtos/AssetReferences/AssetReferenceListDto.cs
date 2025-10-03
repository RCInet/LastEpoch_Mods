using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class AssetReferenceListDto
    {
        [DataMember(Name = "results")]
        public AssetReferenceData[] AssetReferences { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }
    }
}
