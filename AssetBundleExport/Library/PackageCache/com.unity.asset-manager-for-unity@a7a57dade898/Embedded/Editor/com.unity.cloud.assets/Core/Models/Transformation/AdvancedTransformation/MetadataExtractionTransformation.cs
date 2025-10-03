using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Generate a metadata JSON file containing extracted 3D model metadata. The metadata is also added to the asset.
    /// </summary>
    sealed class MetadataExtractionTransformation : ITransformationCreation
    {
        /// <summary>
        /// Specifies the output folder.
        /// </summary>
        public string OutputFolder { get; set; }

        /// <summary>
        /// Specifies the filename to output. The default value is <c>metadata</c>.
        /// </summary>
        public string OutputFilename { get; set; }

        /// <inheritdoc />
        public WorkflowType WorkflowType => WorkflowType.Metadata_Extraction;

        /// <inheritdoc />
        public string[] InputFilePaths { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                {"outputFolder", OutputFolder},
                {"outputFileName", OutputFilename}
            };
        }
    }
}
