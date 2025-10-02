using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class AssignAssetStatusFlowRequest : AssetRequest
    {
        public AssignAssetStatusFlowRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string statusFlowId)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/statusflows/{statusFlowId}/assign";
        }
    }
}
