using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct CreatedProjectDto
    {
        [DataMember(Name = "projectId")]
        public string Id { get; set; }
    }
}
