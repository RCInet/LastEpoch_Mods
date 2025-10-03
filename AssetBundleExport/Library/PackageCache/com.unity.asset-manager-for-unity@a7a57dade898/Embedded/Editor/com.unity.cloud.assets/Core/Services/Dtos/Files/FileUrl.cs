using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct FileUrl
    {
        [DataMember(Name = "datasetId")]
        public string DatasetId { get; set; }

        [DataMember(Name = "filePath")]
        public string Path { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
