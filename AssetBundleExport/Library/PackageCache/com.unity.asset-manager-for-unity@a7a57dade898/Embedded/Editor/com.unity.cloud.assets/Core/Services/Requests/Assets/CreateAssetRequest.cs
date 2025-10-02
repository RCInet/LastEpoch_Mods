using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a create asset request.
    /// </summary>
    class CreateAssetRequest : ProjectRequest
    {
        /// <summary>
        /// The asset to create.
        /// </summary>
        IAssetBaseData Asset { get; }

        /// <summary>
        /// Create Asset Request Object.
        /// Create a single asset.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="asset">The asset to create.</param>
        public CreateAssetRequest(ProjectId projectId, IAssetBaseData asset)
            : base(projectId)
        {
            Asset = asset;

            m_RequestUrl += "/assets";
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithDefaultConverters(Asset);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
