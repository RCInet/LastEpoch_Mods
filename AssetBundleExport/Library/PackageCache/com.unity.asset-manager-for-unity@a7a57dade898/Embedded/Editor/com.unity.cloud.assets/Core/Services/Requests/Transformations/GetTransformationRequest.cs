using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a get transformation URL request.
    /// </summary>
    class GetTransformationRequest : DatasetRequest
    {
        public GetTransformationRequest(TransformationId transformationId, ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId)
            : base(projectId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += $"/transformations/{transformationId}";
        }
    }
}
