using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// AcrossProjectsSearchAndAggregateRequest
    /// Aggregations of assets across projects that match a criteria by a defined field.
    /// </summary>
    class AcrossProjectsSearchAndAggregateRequest : OrganizationRequest
    {
        readonly SearchAndAggregateRequestParameters m_Parameters;

        /// <summary>
        /// Search Request Object.
        /// Search assets based on criteria.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="parameters">The search asset request criteria.</param>
        public AcrossProjectsSearchAndAggregateRequest(OrganizationId organizationId, SearchAndAggregateRequestParameters parameters = default)
            : base(organizationId)
        {
            m_RequestUrl += $"/assets/aggregations/search";

            m_Parameters = parameters;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithDefaultConverters(m_Parameters);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
