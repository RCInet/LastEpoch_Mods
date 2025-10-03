using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class CreateLabelRequest : OrganizationRequest
    {
        readonly ILabelBaseData m_Data;

        public CreateLabelRequest(OrganizationId organizationId, ILabelBaseData labelData)
            : base(organizationId)
        {
            m_RequestUrl += "/labels";

            m_Data = labelData;
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(m_Data, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
