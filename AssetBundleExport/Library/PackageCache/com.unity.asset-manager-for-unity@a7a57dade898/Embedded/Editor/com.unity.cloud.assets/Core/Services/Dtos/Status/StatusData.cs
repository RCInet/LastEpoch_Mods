using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class StatusData : IStatusData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanBeSkipped { get; set; }
        public int SortingOrder { get; set; }
        public StatusPredicateData InPredicate { get; set; }
        public StatusPredicateData OutPredicate { get; set; }
    }
}
