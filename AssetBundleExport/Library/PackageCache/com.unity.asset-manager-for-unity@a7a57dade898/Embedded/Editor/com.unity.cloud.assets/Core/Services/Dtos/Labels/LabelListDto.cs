using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class LabelListDto
    {
        [DataMember(Name = "results")]
        public LabelData[] Labels { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }
    }
}
