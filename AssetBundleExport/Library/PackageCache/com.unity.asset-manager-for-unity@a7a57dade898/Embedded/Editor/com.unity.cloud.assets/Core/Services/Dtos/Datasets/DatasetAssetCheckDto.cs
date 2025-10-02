using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct DatasetAssetCheckDto
    {
        [DataMember(Name = "datasetVersionId")]
        public string DatasetVersionId { get; set; }

        [DataMember(Name = "isPreview")]
        public bool IsPreview { get; set; }
    }
}
