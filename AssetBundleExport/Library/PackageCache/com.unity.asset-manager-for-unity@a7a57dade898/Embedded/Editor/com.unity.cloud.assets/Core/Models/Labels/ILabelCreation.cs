using System.Drawing;

namespace Unity.Cloud.AssetsEmbedded
{
    interface ILabelCreation
    {
        /// <inheritdoc cref="ILabel.Name"/>
        string Name { get; }

        /// <inheritdoc cref="ILabel.Description"/>
        string Description { get; }

        /// <inheritdoc cref="ILabel.DisplayColor"/>
        Color DisplayColor { get; }
    }
}
