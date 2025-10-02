using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    ///  The across projects search and aggregate request parameters.
    /// </summary>
    [DataContract]
    class AcrossProjectsSearchAndAggregateRequestParameters : SearchAndAggregateRequestParameters
    {
        public AcrossProjectsSearchAndAggregateRequestParameters(IEnumerable<ProjectId> projectIds, object aggregateBy)
            : base(aggregateBy)
        {
            ProjectIds = projectIds.ToArray();
        }

        /// <summary>
        /// Parameter project ids of AcrossProjectsSearchAndAggregateRequest
        /// </summary>
        [DataMember(Name = "projectIds", EmitDefaultValue = false)]
        public ProjectId[] ProjectIds { get; }
    }
}
