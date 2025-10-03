using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    class SelectionFieldDefinitionCreation : ISelectionFieldDefinitionCreation
    {
        /// <inheritdoc/>
        public string Key { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string DisplayName { get; set; } = string.Empty;

        /// <inheritdoc/>
        public FieldDefinitionType Type => FieldDefinitionType.Selection;

        /// <inheritdoc/>
        public bool Multiselection { get; set; }

        /// <inheritdoc/>
        public List<string> AcceptedValues { get; set; }
    }
}
