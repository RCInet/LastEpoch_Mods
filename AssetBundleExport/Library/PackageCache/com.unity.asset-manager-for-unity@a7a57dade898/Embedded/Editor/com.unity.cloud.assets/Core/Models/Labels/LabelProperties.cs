using System;
using System.Drawing;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The properties of an <see cref="ILabel"/>.
    /// </summary>
    struct LabelProperties
    {
        /// <summary>
        /// The description of the label.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Whether the label is a system label.
        /// </summary>
        public bool IsSystemLabel { get; internal set; }

        /// <summary>
        /// Whether the label can be manually assigned to an asset.
        /// </summary>
        public bool IsAssignable { get; internal set; }

        /// <summary>
        /// The authoring information for the label.
        /// </summary>
        public AuthoringInfo AuthoringInfo { get; internal set; }

        /// <summary>
        /// The color of the label.
        /// </summary>
        public Color? DisplayColor { get; internal set; }
    }
}
