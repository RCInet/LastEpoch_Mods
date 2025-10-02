using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class SubmitVersionRequest : AssetRequest
    {
        [DataMember(Name = "changeLog")]
        readonly string m_ChangeLog;

        [DataMember(Name = "forceFreeze")]
        readonly bool? m_ForceFreeze;

        public SubmitVersionRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string changeLog, bool? forceFreeze = default)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/submit";

            m_ChangeLog = changeLog;
            m_ForceFreeze = forceFreeze;
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(this, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
