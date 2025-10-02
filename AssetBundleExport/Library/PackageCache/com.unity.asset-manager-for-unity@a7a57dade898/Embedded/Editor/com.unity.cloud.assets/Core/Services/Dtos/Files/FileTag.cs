using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct FileTag
    {
        [DataMember(Name = "tag")]
        public string Tag { get; set; }

        [DataMember(Name = "confidence")]
        public float Confidence { get; set; }
    }

    [DataContract]
    struct FileTags
    {
        [DataMember(Name = "tags")]
        public FileTag[] Tags { get; set; }
    }
}
