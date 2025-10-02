using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class CheckDatasetBelongsToAssetRequest : DatasetRequest
    {
        public CheckDatasetBelongsToAssetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId)
            : base(projectId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += "/check";
        }
    }
}
