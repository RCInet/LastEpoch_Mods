using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class CreateFieldDefinitionRequest : OrganizationRequest
    {
        readonly IFieldDefinitionBaseData m_Data;

        public CreateFieldDefinitionRequest(OrganizationId organizationId, IFieldDefinitionBaseData data)
            : base(organizationId)
        {
            m_RequestUrl += "/templates/fields";

            m_Data = data;
        }

        /// <inheritdoc/>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithConverters(m_Data);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
