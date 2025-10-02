using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class LabelRequest : OrganizationRequest
    {
        readonly ILabelBaseData m_Data;

        public LabelRequest(OrganizationId organizationId, string labelName, ILabelBaseData data = null)
            : base(organizationId)
        {
            m_RequestUrl += $"/labels/{Uri.EscapeDataString(labelName)}";

            m_Data = data;
        }

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
