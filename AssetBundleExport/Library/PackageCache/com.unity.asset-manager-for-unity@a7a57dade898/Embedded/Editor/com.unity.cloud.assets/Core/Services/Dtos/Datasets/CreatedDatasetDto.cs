using System;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct CreatedDatasetDto
    {
        [DataMember(Name = "datasetId")]
        public DatasetId DatasetId { get; set; }
    }
}
