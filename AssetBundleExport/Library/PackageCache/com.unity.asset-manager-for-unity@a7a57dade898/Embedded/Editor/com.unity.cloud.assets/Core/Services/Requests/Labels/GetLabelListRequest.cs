using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class GetLabelListRequest : OrganizationRequest
    {
        public GetLabelListRequest(OrganizationId organizationId, int offset, int limit, bool? archived, bool? systemLabels)
            : base(organizationId)
        {
            m_RequestUrl += "/labels";

            var status = string.Empty;
            if (archived.HasValue)
            {
                status = archived.Value ? "archived" : "active";
            }

            AddParamToQuery("IsSystemLabel", systemLabels?.ToString());
            AddParamToQuery("Status", status);
            AddParamToQuery("Offset", offset.ToString());
            AddParamToQuery("Limit", limit.ToString());
        }
    }
}
