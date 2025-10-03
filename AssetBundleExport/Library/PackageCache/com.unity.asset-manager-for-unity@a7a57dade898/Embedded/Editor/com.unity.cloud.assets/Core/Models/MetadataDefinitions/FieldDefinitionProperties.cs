using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The properties of an <see cref="IFieldDefinition"/>.
    /// </summary>
    struct FieldDefinitionProperties
    {
        /// <summary>
        /// The type of the field.
        /// </summary>
        public FieldDefinitionType Type { get; internal set; }

        /// <summary>
        /// Whether the field is deleted.
        /// </summary>
        public bool IsDeleted { get; internal set; }

        /// <summary>
        /// The display name for the field.
        /// </summary>
        public string DisplayName { get; internal set; }

        /// <summary>
        /// The creation and update information of the field.
        /// </summary>
        public AuthoringInfo AuthoringInfo { get; internal set; }

        /// <summary>
        /// The originator of the field.
        /// </summary>
        public FieldDefinitionOrigin Origin { get; internal set; }

        /// <summary>
        /// The accepted values of the field.
        /// </summary>
        internal IEnumerable<string> AcceptedValues { get; set; }

        /// <summary>
        /// Whether the field can have multiple values.
        /// </summary>
        internal bool Multiselection { get; set; }
    }
}
