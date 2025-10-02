using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class ReachableStatusesDto
    {
        [DataMember(Name = "statusFlowId")]
        public string StatusFlowId { get; set; }

        [DataMember(Name = "reachableStatuses")]
        public string[] ReachableStatusNames { get; set; }
    }
}
