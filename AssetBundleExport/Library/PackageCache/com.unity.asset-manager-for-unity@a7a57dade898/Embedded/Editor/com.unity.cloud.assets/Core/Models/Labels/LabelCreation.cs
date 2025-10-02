using System.Drawing;

namespace Unity.Cloud.AssetsEmbedded
{
    class LabelCreation : ILabelCreation
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public Color DisplayColor { get; set; } = Color.White;
    }
}
