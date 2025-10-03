using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Inserts assets into a collection request.
    /// </summary>
    [DataContract]
    class ModifyAssetsInCollectionRequest : CollectionRequest
    {
        /// <summary>
        /// The DTO containing the asset to insert.
        /// </summary>
        [DataMember(Name = "assetIds")]
        AssetId[] m_AssetsInCollection;

        /// <summary>
        /// Inserts assets into a collection request.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="collectionPath">The path to the collection</param>
        /// <param name="assets">The assets to insert to the collection</param>
        public ModifyAssetsInCollectionRequest(ProjectId projectId, CollectionPath collectionPath, IEnumerable<AssetId> assets)
            : base(projectId, collectionPath)
        {
            m_AssetsInCollection = assets.ToArray();

            m_RequestUrl += $"/assets";
        }

        /// <summary>
        /// Provides an helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithDefaultConverters(this);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
