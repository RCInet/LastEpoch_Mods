using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class FieldDefinitionRequest : OrganizationRequest
    {
        readonly IFieldDefinitionBaseData m_Data;

        public FieldDefinitionRequest(OrganizationId organizationId, string fieldKey, IFieldDefinitionBaseData data = null)
            : base(organizationId)
        {
            m_RequestUrl += $"/templates/fields/{Uri.EscapeDataString(fieldKey)}";

            m_Data = data;
        }

        /// <summary>
        /// Provides a helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            if (m_Data == null)
            {
                return base.ConstructBody();
            }

            var body = IsolatedSerialization.Serialize(m_Data, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
