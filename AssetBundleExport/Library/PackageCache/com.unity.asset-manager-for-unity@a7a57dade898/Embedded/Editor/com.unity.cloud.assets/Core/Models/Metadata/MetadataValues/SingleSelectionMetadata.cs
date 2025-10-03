using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class for manipulating a single selection metadata value.
    /// </summary>
    sealed class SingleSelectionMetadata : MetadataValue
    {
        /// <summary>
        /// The text value of a metadata field.
        /// </summary>
        public string SelectedValue { get; set; }

        public SingleSelectionMetadata(string selectedValue = default)
            : base(MetadataValueType.SingleSelection)
        {
            SelectedValue = selectedValue ?? string.Empty;
        }

        internal SingleSelectionMetadata(MetadataValueType valueType, object value)
            : base(valueType)
        {
            SelectedValue = value?.ToString() ?? string.Empty;
        }

        /// <inheritdoc />
        internal override object GetValue()
        {
            return SelectedValue ?? string.Empty;
        }
    }
}
