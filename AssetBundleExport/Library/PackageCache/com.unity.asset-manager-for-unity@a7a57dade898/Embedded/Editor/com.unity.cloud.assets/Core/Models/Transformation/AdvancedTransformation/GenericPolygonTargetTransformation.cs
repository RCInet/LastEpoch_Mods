using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    [Obsolete("Use OptimizeAndConvertFreeTransformation or OptimizeAndConvertProTransformation instead.")]
sealed class GenericPolygonTargetTransformation : ITransformationCreation
    {
        /// <summary>
        /// The output folder.
        /// </summary>
        public string OutputFolder { get; set; }

        /// <summary>
        /// The extension of the output artifacts. The default value is <c>glb</c>.
        /// </summary>
        public string OutputFileExtension { get; set; }

        /// <summary>
        /// Specifies the filename to output.
        /// </summary>
        /// <remarks>Do not provide an extension for the file name. </remarks>
        public string OutputFilename { get; set; }

        /// <summary>
        /// The targeted polygon count of the 3d model exported. Defaults to 100000.
        /// </summary>
        public int PolygonCountTarget { get; set; }

        /// <summary>
        /// Whether structure optimization is enabled (reduce draw calls, but combined objects won't move separately).
        /// </summary>
        public bool? MergeObjects { get; set; }

        /// <summary>
        /// Whether to optimize the scene by deleting unnecessary objects.
        /// </summary>
        public bool? DeleteObjects { get; set; }

        /// <inheritdoc />
        public WorkflowType WorkflowType => WorkflowType.Generic_Polygon_Target;

        /// <inheritdoc />
        public string[] InputFilePaths { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> GetParameters()
        {
            var parameters = new Dictionary<string, string>
            {
                {"outputFolder", OutputFolder},
                {"outputFileExt", OutputFileExtension},
                {"outputFilenameNoExtension", OutputFilename},
                {"polygonCountTarget", PolygonCountTarget.ToString()},
                {"mergeObjects", TransformationUtilities.GetValue(MergeObjects)},
                {"deleteObjects", TransformationUtilities.GetValue(DeleteObjects)},
            };

            return parameters;
        }
    }
}
