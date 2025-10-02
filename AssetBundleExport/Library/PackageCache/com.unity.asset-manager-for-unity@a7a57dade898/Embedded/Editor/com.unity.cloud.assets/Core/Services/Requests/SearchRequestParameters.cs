using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The request to read the assets.
    /// </summary>
    [DataContract]
    class SearchRequestParameters
    {
        /// <summary>
        /// The request to read the assets.
        /// </summary>
        /// <param name="includeFields">The fields to be returned.</param>
        public SearchRequestParameters(FieldsFilter includeFields = default)
        {
            includeFields?.Parse(AddIncludeField);
        }

        /// <summary>
        /// Parameter filter of SearchRequest
        /// </summary>
        [DataMember(Name = "filter", EmitDefaultValue = false)]
        public ISearchRequestFilter Filter { get; set; }

        /// <summary>
        /// The fields to be returned.
        /// </summary>
        [DataMember(Name = "includeFields", EmitDefaultValue = false)]
        public List<string> IncludeFields { get; } = new();

        /// <summary>
        /// Parameter pagination of SearchRequest
        /// </summary>
        [DataMember(Name = "pagination", EmitDefaultValue = false)]
        public SearchRequestPagination Pagination { get; set; }

        public Range PaginationRange { get; set; } = Range.All;

        void AddIncludeField(string field)
        {
            IncludeFields.Add(field);
        }
    }
}
