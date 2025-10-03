using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class AssetLabelListDto
    {
        [DataMember(Name = "assetVersionLabels")]
        public AssetLabelsDto[] AssetLabels { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }
    }

    [DataContract]
    class AssetLabelsDto
    {
        [DataMember(Name = "assetVersion")]
        public string AssetVersion { get; set; }

        [DataMember(Name = "labels")]
        public LabelData[] Labels { get; set; }

        [DataMember(Name = "archivedLabels")]
        public LabelData[] ArchivedLabels { get; set; }
    }
}
