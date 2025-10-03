using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class AssetLabelRequest : ProjectRequest
    {
        public AssetLabelRequest(ProjectId projectId, AssetId assetId, int offset, int limit)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/labels";

            AddParamToQuery("Offset", offset.ToString());
            AddParamToQuery("Limit", limit.ToString());
        }
    }
}
