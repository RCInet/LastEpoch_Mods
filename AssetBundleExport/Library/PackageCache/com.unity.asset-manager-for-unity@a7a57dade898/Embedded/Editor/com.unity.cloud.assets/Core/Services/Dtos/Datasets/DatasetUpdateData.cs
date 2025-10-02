using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class DatasetUpdateData : DatasetBaseData, IDatasetUpdateData
    {
        /// <inheritdoc />
        public IEnumerable<string> FileOrder { get; set; }
    }
}
