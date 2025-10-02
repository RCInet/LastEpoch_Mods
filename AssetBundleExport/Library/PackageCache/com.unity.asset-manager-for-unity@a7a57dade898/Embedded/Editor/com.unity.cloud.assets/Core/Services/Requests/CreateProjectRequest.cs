using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Base class for api requests on assets.
    /// </summary>
    class CreateProjectRequest : OrganizationRequest
    {
        IProjectBaseData Data { get; }

        /// <summary>
        /// AssetRequest Request Object.
        /// </summary>
        /// <param name="organizationId">The organization id. </param>
        /// <param name="data">The object containting the necessary information to create a project. </param>
        public CreateProjectRequest(OrganizationId organizationId, IProjectBaseData data)
            : base(organizationId)
        {
            Data = data;

            m_RequestUrl += "/projects";
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(Data, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
