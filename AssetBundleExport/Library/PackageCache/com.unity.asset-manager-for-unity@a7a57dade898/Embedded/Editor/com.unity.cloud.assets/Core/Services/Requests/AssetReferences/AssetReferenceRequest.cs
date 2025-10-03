using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class AssetReferenceRequest : ProjectRequest
    {
        readonly IAssetReferenceRequestBody m_Data;

        public AssetReferenceRequest(ProjectId projectId, AssetId assetId, IAssetReferenceRequestBody data = null)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/references";

            m_Data = data;
        }

        public AssetReferenceRequest(ProjectId projectId, AssetId assetId, AssetVersion? assetVersion, string context, int? offset = null, int? limit = null)
            : this(projectId, assetId)
        {
            AddParamToQuery("AssetVersion", assetVersion?.ToString());
            AddParamToQuery("Context", context);
            AddParamToQuery("offset", offset?.ToString());
            AddParamToQuery("limit", limit?.ToString());
        }

        public AssetReferenceRequest(ProjectId projectId, AssetId assetId, string referenceId)
            : this(projectId, assetId)
        {
            m_RequestUrl += $"/{referenceId}";
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
