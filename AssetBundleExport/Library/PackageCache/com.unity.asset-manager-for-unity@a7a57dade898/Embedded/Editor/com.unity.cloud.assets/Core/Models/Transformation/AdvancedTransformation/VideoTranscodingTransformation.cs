using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Transcode videos so that they can be scrubbed at high fidelity in the web viewer.
    /// </summary>
    sealed class VideoTranscodingTransformation : ITransformationCreation
    {
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
        /// Whether to create a thumbnail image.
        /// </summary>
        public bool? CreateThumbnail { get; set; }

        /// <inheritdoc />
        public WorkflowType WorkflowType => WorkflowType.Transcode_Video;

        /// <inheritdoc />
        public string[] InputFilePaths { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> GetParameters()
        {
            var parameters = new Dictionary<string, string>
            {
                {"outputFolder", OutputFolder},
                {"outputPrefix", OutputPrefix},
                {"outputSuffix", OutputSuffix},
                {"createThumbnail", TransformationUtilities.GetValue(CreateThumbnail)}
            };

            return parameters;
        }
    }
}
