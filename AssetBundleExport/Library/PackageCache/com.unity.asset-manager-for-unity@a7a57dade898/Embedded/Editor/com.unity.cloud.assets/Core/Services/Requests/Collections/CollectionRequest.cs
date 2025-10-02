using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Builds an API request that references a collection by path.
    /// </summary>
    class CollectionRequest : ProjectRequest
    {
        readonly IAssetCollectionData m_Data;

        /// <summary>
        /// Initializes and returns an API request for a collection.
        /// </summary>
        /// <param name="projectId">ID of the project. </param>
        /// <param name="collectionPath">The path to the collection. </param>
        /// <param name="data">The object containing the data of the collection. </param>
        public CollectionRequest(ProjectId projectId, CollectionPath collectionPath, IAssetCollectionData data = null)
            : base(projectId)
        {
            m_RequestUrl += $"/collections/{Uri.EscapeDataString(collectionPath)}";

            m_Data = data;
        }

        public override HttpContent ConstructBody()
        {
            if (m_Data == null)
            {
                return base.ConstructBody();
            }

            var body = IsolatedSerialization.SerializeWithConverters(m_Data, IsolatedSerialization.CollectionPathConverter);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
