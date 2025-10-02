using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class AssetStatusRequest : AssetRequest
    {
        AssetStatusRequest(ProjectId projectId, AssetId assetId, AssetVersion version, string action = null)
            : base(projectId, assetId, version)
        {
            m_RequestUrl += $"/status";

            if (!string.IsNullOrEmpty(action))
            {
                m_RequestUrl += $"/{action}";
            }
        }

        public static ApiRequest GetCurrentStatusRequest(ProjectId projectId, AssetId assetId, AssetVersion version)
        {
            return new AssetStatusRequest(projectId, assetId, version);
        }

        public static ApiRequest GetReachableStatusRequest(ProjectId projectId, AssetId assetId, AssetVersion version)
        {
            return new AssetStatusRequest(projectId, assetId, version, "reachable");
        }
    }
}
