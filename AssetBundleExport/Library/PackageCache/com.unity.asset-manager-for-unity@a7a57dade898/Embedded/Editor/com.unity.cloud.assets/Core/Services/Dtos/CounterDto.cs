using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class CounterDto
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }
    }
}
