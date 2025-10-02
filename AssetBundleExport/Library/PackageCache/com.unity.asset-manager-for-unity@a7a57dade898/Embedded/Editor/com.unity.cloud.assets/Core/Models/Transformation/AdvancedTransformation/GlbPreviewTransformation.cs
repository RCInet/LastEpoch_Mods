using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Produce a simplified GLB representation of a 3D model.
    /// </summary>
    sealed class GlbPreviewTransformation : ITransformationCreation
    {
        /// <summary>
        /// Specifies the output file name. The default value is <c>preview</c>.
        /// </summary>
        /// <remarks>Do not provide an extension for the file name. </remarks>
        public string OutputFilename { get; set; }

        /// <summary>
        /// Whether to use KTX2 texture compression.
        /// </summary>
        public bool? UseKtx2Texture { get; set; }

        /// <summary>
        /// Whether to use Draco mesh compression.
        /// </summary>
        public bool? UseDracoCompression { get; set; }

        /// <inheritdoc />
        public WorkflowType WorkflowType => WorkflowType.GLB_Preview;

        /// <inheritdoc />
        public string[] InputFilePaths { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                {"outputFilenameNoExtension", OutputFilename},
                {"useKTX2Texture", TransformationUtilities.GetValue(UseKtx2Texture)},
                {"useDracoCompression", TransformationUtilities.GetValue(UseDracoCompression)}
            };
        }
    }
}
