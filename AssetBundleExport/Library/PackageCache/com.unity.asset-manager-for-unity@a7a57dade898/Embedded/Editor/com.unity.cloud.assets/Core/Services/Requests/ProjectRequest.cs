using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Base class for api requests on assets.
    /// </summary>
    class ProjectRequest : ApiRequest
    {
        /// <summary>
        /// AssetRequest Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        protected ProjectRequest(ProjectId projectId)
        {
            m_RequestUrl = $"/projects/{projectId}";
        }

        public static ProjectRequest GetProjectRequset(ProjectId projectId)
        {
            var request = new ProjectRequest(projectId);
            request.AddParamToQuery("IncludeFields", "hasCollection");
            return request;
        }

        public static ProjectRequest GetEnableProjectRequest(ProjectId projectId)
        {
            var request = new ProjectRequest(projectId);
            request.m_RequestUrl += "/enable";
            return request;
        }

        public static ProjectRequest GetAssetCountRequest(ProjectId projectId)
        {
            var projectRequest = new ProjectRequest(projectId);
            projectRequest.m_RequestUrl += "/assets/count";
            return projectRequest;
        }

        public static ProjectRequest GetCollectionCountRequest(ProjectId projectId)
        {
            var projectRequest = new ProjectRequest(projectId);
            projectRequest.m_RequestUrl += "/collections-count";
            return projectRequest;
        }
    }
}
