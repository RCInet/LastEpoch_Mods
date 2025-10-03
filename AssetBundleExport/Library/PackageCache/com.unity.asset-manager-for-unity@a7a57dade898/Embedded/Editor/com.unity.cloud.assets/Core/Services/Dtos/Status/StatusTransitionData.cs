using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class StatusTransitionData : IStatusTransitionData
    {
        public string Id { get; set; }
        public string FromStatusId { get; set; }
        public string ToStatusId { get; set; }
        public StatusPredicateData ThroughPredicate { get; set; }
    }
}
