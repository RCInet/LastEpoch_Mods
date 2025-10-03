using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class PaginationDto
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }
    }
}
