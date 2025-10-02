using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class LabelData : LabelBaseData, ILabelData
    {
        /// <inheritdoc/>
        public bool IsSystemLabel { get; set; }

        /// <inheritdoc/>
        public bool IsUserAssignable { get; set; }

        /// <inheritdoc />
        public DateTime? Created { get; set; }

        /// <inheritdoc />
        public string CreatedBy { get; set; }

        /// <inheritdoc />
        public DateTime? Updated { get; set; }

        /// <inheritdoc />
        public string UpdatedBy { get; set; }
    }
}
