using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IStatusData
    {
        [DataMember(Name = "id")]
        string Id { get; }

        [DataMember(Name = "name")]
        string Name { get; }

        [DataMember(Name = "description")]
        string Description { get; }

        [DataMember(Name = "canBeSkipped")]
        bool CanBeSkipped { get; }

        [DataMember(Name = "sortingOrder")]
        int SortingOrder { get; }

        [DataMember(Name = "inPredicate")]
        StatusPredicateData InPredicate { get; }

        [DataMember(Name = "outPredicate")]
        StatusPredicateData OutPredicate { get; }
    }
}
