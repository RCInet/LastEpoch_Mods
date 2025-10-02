using System.Drawing;

namespace Unity.Cloud.AssetsEmbedded
{
    class LabelUpdate : ILabelUpdate
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public Color? DisplayColor { get; set; }
    }
}
