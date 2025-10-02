using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IStatusTransitionData
    {
        [DataMember(Name = "id")]
        string Id { get; }

        [DataMember(Name = "fromStatusId")]
        string FromStatusId { get; }

        [DataMember(Name = "toStatusId")]
        string ToStatusId { get; }

        [DataMember(Name = "throughPredicate")]
        StatusPredicateData ThroughPredicate { get; }
    }
}
