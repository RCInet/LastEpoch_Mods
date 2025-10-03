using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class TerminateTransformationRequest : ProjectRequest
    {
        public TerminateTransformationRequest(ProjectId projectId, TransformationId transformationId)
            : base(projectId)
        {
            m_RequestUrl += $"/transformations/{transformationId}/termination";
        }
    }
}
