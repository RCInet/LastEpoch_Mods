using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class CreateDatasetRequest : AssetRequest
    {
        IDatasetBaseData DatasetData { get; }

        public CreateDatasetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, IDatasetBaseData datasetInfo)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/datasets";

            DatasetData = datasetInfo;
        }

        /// <inheritdoc/>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithConverters(DatasetData, IsolatedSerialization.DatasetIdConverter);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
