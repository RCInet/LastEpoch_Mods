using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class for manipulating a multi-selection metadata value.
    /// </summary>
    class MultiSelectionMetadata : MetadataValue
    {
        /// <summary>
        /// The list of selected values.
        /// </summary>
        public List<string> SelectedValues { get; set; }

        public MultiSelectionMetadata(params string[] selectedValues)
            : base(MetadataValueType.MultiSelection)
        {
            SelectedValues = selectedValues?.ToList() ?? new List<string>();
        }

        internal MultiSelectionMetadata(MetadataValueType valueType, object value)
            : base(valueType)
        {
            SelectedValues = value switch
            {
                null => new List<string>(),
                string stringValue => ParseValue(stringValue),
                IEnumerable<string> stringEnumerable => stringEnumerable.ToList(),
                ICollection collection => collection.Cast<object>().Select(o => o?.ToString() ?? string.Empty).ToList(),
                _ => new List<string> {value.ToString()}
            };
        }

        /// <inheritdoc />
        internal override object GetValue()
        {
            return SelectedValues;
        }

        static List<string> ParseValue(string stringValue)
        {
            var list = new List<string>();

            var splitValues = stringValue.Split(',');
            foreach (var split in splitValues)
            {
                list.Add(split.Trim());
            }

            return list;
        }
    }
}
