using System;
using System.Collections.Generic;
using System.Drawing;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Produce four 2d images from the X, Y, Z, and Isometric-front-left directions for previewing purposes.
    /// </summary>
    sealed class ThumbnailGeneratorTransformation : ITransformationCreation
    {
        /// <summary>
        /// Whether to use the custom environment lighting map.
        /// </summary>
        public bool? UseEnvironmentLight { get; set; }

        /// <summary>
        /// The HDR file to use for custom environment lighting.
        /// </summary>
        public string CustomEnvironmentLight { get; set; }

        /// <summary>
        /// The environment map rotation angle.
        /// </summary>
        public float? EnvironmentLightRotation { get; set; }

        /// <summary>
        /// The resolution of the generated thumbnail.
        /// </summary>
        public string Resolution { get; set; }

        /// <summary>
        /// Which views to generate.
        /// </summary>
        /// <remarks>Available options are: <c>front</c> <c>left</c> <c>top</c> <c>isoFrontLeftTop</c></remarks>
        public string Views { get; set; }

        /// <summary>
        /// The output folder.
        /// </summary>
        public string OutputFolder { get; set; }

        /// <summary>
        /// The prefix text to use for naming the artifacts. The default value is <c>preview</c>.
        /// </summary>
        public string OutputPrefix { get; set; }

        /// <summary>
        /// The suffix text to use for naming the artifacts.
        /// </summary>
        public string OutputSuffix { get; set; }

        /// <summary>
        /// The extension of the output artifacts. The default value is <c>png</c>.
        /// </summary>
        public string OutputFileExtension { get; set; }

        /// <summary>
        /// The background color.
        /// </summary>
        public Color? BackgroundColor { get; set; }

        /// <summary>
        /// Whether to force convert Z up to Y up.
        /// </summary>
        public bool? ForceConvertZUpToYUp { get; set; }

        /// <inheritdoc />
        public WorkflowType WorkflowType => WorkflowType.Thumbnail_Generation;

        /// <inheritdoc />
        public string[] InputFilePaths { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> GetParameters()
        {
            var parameters = new Dictionary<string, string>
            {
                {"useEnvLight", TransformationUtilities.GetValue(UseEnvironmentLight)},
                {"customEnvLight", CustomEnvironmentLight},
                {"envLightRotation", EnvironmentLightRotation?.ToString()},
                {"resolution", Resolution},
                {"views", Views},
                {"outputFolder", OutputFolder},
                {"outputPrefix", OutputPrefix},
                {"outputSuffix", OutputSuffix},
                {"outputFileExt", OutputFileExtension},
                {"forceConvertZUpToYUp", TransformationUtilities.GetValue(ForceConvertZUpToYUp)}
            };

            if (BackgroundColor.HasValue)
            {
                parameters.Add("backgroundColor", $"{BackgroundColor.Value.R} {BackgroundColor.Value.G} {BackgroundColor.Value.B} {BackgroundColor.Value.A}");
            }

            return parameters;
        }
    }
}
