using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a get transformation URL request.
    /// </summary>
    [DataContract]
    class StartTransformationRequest : DatasetRequest
    {
        [DataMember(Name = "inputFiles")]
        string[] m_InputFiles;

        [DataMember(Name="extraParameters")]
        Dictionary<string, object> m_ExtraParameters;

        public StartTransformationRequest(string workflowType, IEnumerable<string> inputFiles, Dictionary<string, object> parameters,
            ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId)
            : base(projectId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += $"/transformations/start/{workflowType}";

            m_InputFiles = inputFiles?.ToArray();

            if (parameters is {Count: > 0})
            {
                m_ExtraParameters = new Dictionary<string, object>();
                foreach (var kvp in parameters)
                {
                    if (kvp.Value != null)
                    {
                        m_ExtraParameters.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(this, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
