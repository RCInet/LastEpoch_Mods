using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct AggregationsDto
    {
        [DataMember(Name = "aggregations")]
        public AggregateDto[] Aggregations { get; set; }
    }

    struct AggregateDto
    {
        [DataMember(Name = "value")]
        public object Value { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        public AggregateDto(string value, int count)
        {
            Value = value;
            Count = count;
        }
    }
}
