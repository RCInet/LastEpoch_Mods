using System;

namespace Unity.Cloud.AssetsEmbedded
{
    class FieldDefinitionUpdate : IFieldDefinitionUpdate
    {
        /// <inheritdoc/>
        public string DisplayName { get; set; } = string.Empty;

        public FieldDefinitionUpdate() { }

        [Obsolete("Use the default constructor instead.")]
        public FieldDefinitionUpdate(IFieldDefinition fieldDefinition)
        {
            DisplayName = fieldDefinition.DisplayName;
        }
    }
}
