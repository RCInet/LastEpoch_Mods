using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class ModifyFieldDefinitionSelectionRequest : FieldDefinitionRequest
    {
        public ModifyFieldDefinitionSelectionRequest(OrganizationId organizationId, string fieldKey, IEnumerable<string> values)
            : base(organizationId, fieldKey)
        {
            m_RequestUrl += "/accepted-values";

            AddParamToQuery("values", values);
        }

        public override HttpContent ConstructBody()
        {
            return new StringContent("", Encoding.UTF8, "application/json");
        }
    }
}
