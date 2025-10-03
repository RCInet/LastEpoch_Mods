using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The request for searching for aggregations.
    /// </summary>
    [DataContract]
    class SearchAndAggregateRequestParameters
    {
        public SearchAndAggregateRequestParameters(object aggregateBy)
        {
            AggregateBy = aggregateBy;
        }

        /// <summary>
        /// Parameter filter of SearchAndAggregateRequest
        /// </summary>
        [DataMember(Name = "filter", EmitDefaultValue = false)]
        public SearchRequestFilter Filter { get; set; }

        /// <summary>
        /// The field that can be used in the aggregation.
        /// </summary>
        [DataMember(Name = "aggregateBy", EmitDefaultValue = false)]
        public object AggregateBy { get; }

        /// <summary>
        /// The maximum number of items to be returned.
        /// </summary>
        [DataMember(Name = "maximumNumberOfItems", EmitDefaultValue = false)]
        public int? MaximumNumberOfItems { get; set; } = int.MaxValue;
    }
}
