using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The asset read filter.
    /// </summary>
    [DataContract]
    class SearchRequestFilter : ISearchRequestFilter
    {
        /// <summary>
        /// The asset read filter.
        /// </summary>
        /// <param name="includeQuery">The dictionary to include all the entries matching all the criteria.</param>
        /// <param name="excludeQuery">The dictionary to exclude all the entries matching all the criteria.</param>
        /// <param name="anyQuery">The dictionary to Include all entries matching any of the criterias.</param>
        /// <param name="anyQueryMinimumMatch">The minimum any query match amount to be considered a match for return.</param>
        /// <param name="collections">The collection paths.</param>
        public SearchRequestFilter(IReadOnlyDictionary<string, object> includeQuery = default, IReadOnlyDictionary<string, object> excludeQuery = default, IReadOnlyDictionary<string, object> anyQuery = default, int? anyQueryMinimumMatch = default, IEnumerable<CollectionPath> collections = default)
        {
            IncludeQuery = includeQuery;
            ExcludeQuery = excludeQuery;
            AnyQuery = anyQuery;
            AnyQueryMinimumMatch = anyQueryMinimumMatch;

            Collections = collections?.ToArray();
            if (Collections is {Length: 0}) Collections = null;
        }

        /// <summary>
        /// The dictionary to include all the entries matching all the criteria.
        /// </summary>
        [DataMember(Name = "includeQuery", EmitDefaultValue = false)]
        public IReadOnlyDictionary<string, object> IncludeQuery { get; }

        /// <summary>
        /// The dictionary to exclude all the entries matching all the criteria.
        /// </summary>
        [DataMember(Name = "excludeQuery", EmitDefaultValue = false)]
        public IReadOnlyDictionary<string, object> ExcludeQuery { get; }

        /// <summary>
        /// The dictionary to Include all entries matching any of the criterias.
        /// </summary>
        [DataMember(Name = "anyQuery", EmitDefaultValue = false)]
        public IReadOnlyDictionary<string, object> AnyQuery { get; }

        /// <summary>
        /// The minimum any query match amount to be considered a match for return.
        /// </summary>
        [DataMember(Name = "anyQueryMinimumMatch", EmitDefaultValue = false)]
        public int? AnyQueryMinimumMatch { get; }

        /// <summary>
        /// The collection paths.
        /// </summary>
        [DataMember(Name = "collections", EmitDefaultValue = false)]
        public CollectionPath[] Collections { get; }
    }
}
