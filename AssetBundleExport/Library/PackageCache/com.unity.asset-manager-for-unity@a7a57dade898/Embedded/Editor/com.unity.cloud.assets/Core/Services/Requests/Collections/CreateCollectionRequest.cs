using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Builds an API request which creates a collection.
    /// </summary>
    class CreateCollectionRequest : ProjectRequest
    {
        /// <summary>
        /// Returns a collection.
        /// </summary>
        IAssetCollectionData AssetCollection { get; }

        /// <summary>
        /// Initializes and returns an API request for posting collection.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="assetCollectionDto">The collection</param>
        public CreateCollectionRequest(ProjectId projectId, IAssetCollectionData assetCollectionDto)
            : base(projectId)
        {
            AssetCollection = assetCollectionDto;

            m_RequestUrl += "/collections";
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithConverters(AssetCollection, IsolatedSerialization.CollectionPathConverter);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
