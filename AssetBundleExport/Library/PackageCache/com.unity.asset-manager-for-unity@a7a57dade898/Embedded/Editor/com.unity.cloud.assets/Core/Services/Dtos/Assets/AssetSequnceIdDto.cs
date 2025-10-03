using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct VersionNumberDto
    {
        [DataMember(Name = "versionNumber")]
        public int? VersionNumber { get; set; }
    }
}
