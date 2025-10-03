using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class AssignLabelRequest : AssetRequest
    {
        [DataMember(Name = "labelNames")]
        readonly string[] m_Labels;

        public AssignLabelRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, bool assign, IEnumerable<string> labels)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/labels/{(assign ? "assign" : "unassign")}";

            m_Labels = labels.ToArray();
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(this, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
