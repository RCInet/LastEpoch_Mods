using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The across projects search request parameters.
    /// </summary>
    [DataContract]
    class AcrossProjectsSearchRequestParameters : SearchRequestParameters
    {
        /// <summary>
        /// The across projects search request parameters.
        /// </summary>
        /// <param name="projectIds"></param>
        /// <param name="includeFields">The fields to be returned.</param>
        public AcrossProjectsSearchRequestParameters(IEnumerable<ProjectId> projectIds, FieldsFilter includeFields = default)
            : base(includeFields)
        {
            ProjectIds = projectIds.ToArray();
        }

        /// <summary>
        /// Parameter project ids of AcrossProjectsSearchRequest
        /// </summary>
        [DataMember(Name = "projectIds", EmitDefaultValue = false)]
        public ProjectId[] ProjectIds { get; }
    }
}
