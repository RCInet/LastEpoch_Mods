using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct PageTokenDto
    {
        [DataMember(Name = "next")]
        public string Token { get; set; }
    }
}
