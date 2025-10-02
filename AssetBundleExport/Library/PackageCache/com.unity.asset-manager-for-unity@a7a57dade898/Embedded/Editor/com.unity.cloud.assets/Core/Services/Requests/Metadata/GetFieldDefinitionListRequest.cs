using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class GetFieldDefinitionListRequest : OrganizationRequest
    {
        public GetFieldDefinitionListRequest(OrganizationId organizationId, int limit, SortingOrder sortingOrder, string nextToken, Dictionary<string, string> queryParameters = null)
            : base(organizationId)
        {
            m_RequestUrl += "/templates/fields";

            if (queryParameters != null)
            {
                foreach (var param in queryParameters)
                {
                    AddParamToQuery(param.Key, param.Value);
                }
            }

            AddParamToQuery("SortingOrder", sortingOrder.ToString());
            AddParamToQuery("Limit", limit.ToString());
            AddParamToQuery("Next", nextToken);
        }
    }
}
