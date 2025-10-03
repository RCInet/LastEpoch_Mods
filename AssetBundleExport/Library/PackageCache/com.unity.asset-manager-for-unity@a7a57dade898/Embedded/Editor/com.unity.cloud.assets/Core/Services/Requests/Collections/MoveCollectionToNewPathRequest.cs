using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Move a collection to a new path request.
    /// </summary>
    [DataContract]
    class MoveCollectionToNewPathRequest : CollectionRequest
    {
        /// <summary>
        /// The new path to the collection.
        /// </summary>
        [DataMember(Name = "destinationParentPath")]
        string m_DestinationPath;

        /// <summary>
        /// Move a collection request.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="collectionPath">The path to the collection</param>
        /// <param name="destinationPath">The new path to the collection</param>
        public MoveCollectionToNewPathRequest(ProjectId projectId, CollectionPath collectionPath, string destinationPath)
            : base(projectId, collectionPath)
        {
            m_DestinationPath = destinationPath;

            m_RequestUrl += $"/move";
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(this, IsolatedSerialization.defaultSettings);

            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
