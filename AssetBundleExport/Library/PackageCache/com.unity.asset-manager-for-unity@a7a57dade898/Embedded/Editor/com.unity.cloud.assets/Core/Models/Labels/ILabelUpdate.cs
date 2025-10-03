using System.Drawing;

namespace Unity.Cloud.AssetsEmbedded
{
    interface ILabelUpdate
    {
        /// <inheritdoc cref="ILabel.Description"/>
        string Description { get; }

        /// <inheritdoc cref="ILabel.DisplayColor"/>
        Color? DisplayColor { get; }
    }
}
