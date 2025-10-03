using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// SearchAndAggregateRequest
    /// Aggregations of assets that match a criteria by a defined field.
    /// </summary>
    class SearchAndAggregateRequest : ProjectRequest
    {
        /// <summary>Accessor for searchAndAggregateRequestParameter </summary>
        SearchAndAggregateRequestParameters Parameters { get; }

        /// <summary>
        /// SearchAndAggregate Request Object.
        /// Aggregations of assets that match a criteria by a defined field.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="parameters">The request containing the read filter and the field to be used in the aggregation..</param>
        public SearchAndAggregateRequest(ProjectId projectId, SearchAndAggregateRequestParameters parameters = default)
            : base(projectId)
        {
            m_RequestUrl += "/assets/aggregations/search";

            Parameters = parameters;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithDefaultConverters(Parameters);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
