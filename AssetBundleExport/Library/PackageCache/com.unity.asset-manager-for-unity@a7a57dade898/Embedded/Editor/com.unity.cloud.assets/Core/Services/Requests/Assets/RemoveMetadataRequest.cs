using System;
using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class RemoveMetadataRequest : AssetRequest
    {
        public RemoveMetadataRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string from, IEnumerable<string> keys)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/fields";
            AddParamToQuery(from, keys);
        }

        public RemoveMetadataRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string from, IEnumerable<string> keys)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/datasets/{datasetId}/fields";
            AddParamToQuery(from, keys);
        }

        public RemoveMetadataRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, string from, IEnumerable<string> keys)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/datasets/{datasetId}/files/{Uri.EscapeDataString(filePath)}/fields";
            AddParamToQuery(from, keys);
        }
    }
}
