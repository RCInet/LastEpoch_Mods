using System;

namespace Unity.Cloud.AssetsEmbedded
{
    class FieldDefinitionData : FieldDefinitionCreateData, IFieldDefinitionData
    {
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
        public string Origin { get; set; }
    }
}
