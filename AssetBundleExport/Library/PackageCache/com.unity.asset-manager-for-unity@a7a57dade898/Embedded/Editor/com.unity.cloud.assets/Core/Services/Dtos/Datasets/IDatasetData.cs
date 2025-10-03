using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IDatasetData : IDatasetUpdateData, IAuthoringData
    {
        [DataMember(Name = "datasetId")]
        DatasetId DatasetId { get; }

        [DataMember(Name = "systemTags")]
        IEnumerable<string> SystemTags { get; }

        [DataMember(Name = "status")]
        string Status { get; }

        [DataMember(Name = "workflowName")]
        string WorkflowName { get; }

        [DataMember(Name = "files")]
        IEnumerable<FileData> Files { get; }
    }
}
