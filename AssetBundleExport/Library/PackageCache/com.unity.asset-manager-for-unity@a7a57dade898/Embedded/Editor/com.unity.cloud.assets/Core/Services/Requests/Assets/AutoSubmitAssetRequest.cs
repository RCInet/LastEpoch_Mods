using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class AutoSubmitAssetRequest : AssetRequest
    {
        [DataMember(Name = "changeLog")]
        string m_Changelog;

        public AutoSubmitAssetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string changelog)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += "/autosubmit";
            m_Changelog = changelog;
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(this, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }

        public static AutoSubmitAssetRequest GetDisableRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion)
        {
            var request = new AutoSubmitAssetRequest(projectId, assetId, assetVersion, null);
            request.m_RequestUrl += "/disable";

            return request;
        }
    }
}
