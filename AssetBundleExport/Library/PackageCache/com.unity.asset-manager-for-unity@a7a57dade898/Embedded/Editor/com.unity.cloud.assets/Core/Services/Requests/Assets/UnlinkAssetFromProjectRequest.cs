using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents an unlink asset from project request.
    /// </summary>
    [DataContract]
    class UnlinkAssetFromProjectRequest : ProjectRequest
    {
        [DataMember(Name = "assetIds")]
        AssetId[] m_AssetIds;

        /// <summary>
        /// Unlink an Asset from a Project Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        public UnlinkAssetFromProjectRequest(ProjectId projectId, AssetId assetId)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/unlink";
        }

        /// <summary>
        /// Link an Asset to a Project Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetIds">The ids of the assets.</param>
        public UnlinkAssetFromProjectRequest(ProjectId projectId, IEnumerable<AssetId> assetIds)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/unlink";

            m_AssetIds = assetIds.ToArray();
        }

        public override HttpContent ConstructBody()
        {
            if (m_AssetIds == null)
            {
                return base.ConstructBody();
            }

            var body = IsolatedSerialization.SerializeWithDefaultConverters(this);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
