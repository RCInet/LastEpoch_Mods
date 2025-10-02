using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Optimize and convert generic file formats.
    /// </summary>
    sealed class OptimizeAndConvertFreeTransformation : ITransformationCreation
    {
        /// <inheritdoc />
        public WorkflowType WorkflowType => WorkflowType.Optimize_Convert_Free;

        /// <summary>
        /// Specifies the output file name. The default value is <c>output</c>.
        /// </summary>
        /// <remarks>Do not provide an extension for the file name. </remarks>
        public string OutputFilename { get; set; }

        /// <summary>
        /// The ouput file extension list. The default value is <c>["glb"]</c>.
        /// </summary>
        public string[] ExportFormats { get; set; }

        /// <inheritdoc />
        public string[] InputFilePaths { get; set; }

        /// <inheritdoc />
        public Dictionary<string, object> GetExtraParameters()
        {
            return new Dictionary<string, object>
            {
                {"outputFileName", OutputFilename},
                {"exportFormats", ExportFormats},
            };
        }
    }
}
