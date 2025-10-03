using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class AddFileReferenceRequest : FileRequest
    {
        [DataMember(Name = "targetDatasetId")]
        DatasetId m_DatasetId;

        public AddFileReferenceRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, DatasetId targetDatasetId)
            : base(projectId, assetId, assetVersion, datasetId, filePath)
        {
            m_RequestUrl += "/reference";
            m_DatasetId = targetDatasetId;
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithConverters(this, IsolatedSerialization.DatasetIdConverter);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
