using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract(Name = "dataset")]
    class DatasetData : DatasetUpdateData, IDatasetData
    {
        /// <inheritdoc />
        public DatasetId DatasetId { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> SystemTags { get; set; }

        /// <inheritdoc />
        public string Status { get; set; }

        /// <inheritdoc />
        public string CreatedBy { get; set; }

        /// <inheritdoc />
        public DateTime? Created { get; set; }

        /// <inheritdoc />
        public string UpdatedBy { get; set; }

        /// <inheritdoc />
        public DateTime? Updated { get; set; }

        /// <inheritdoc />
        public string WorkflowName { get; set; }

        /// <inheritdoc />
        public IEnumerable<FileData> Files { get; set; }
    }
}
