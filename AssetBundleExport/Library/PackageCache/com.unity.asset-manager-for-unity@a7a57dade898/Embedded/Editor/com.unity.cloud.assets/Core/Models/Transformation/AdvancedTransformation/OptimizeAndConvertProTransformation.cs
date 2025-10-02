using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Optimize and convert all supported 3D file formats.
    /// Only available in the Unity Pro and Enterprise/Industry Tiers.
    /// </summary>
    sealed class OptimizeAndConvertProTransformation : ITransformationCreation
    {
        public enum OptimizeStrategy
        {
            /// <summary>
            /// Optimize the model using a ratio.
            /// </summary>
            Ratio,
            /// <summary>
            /// Optimize the model using a polygon target number.
            /// </summary>
            TriangleCount,
        }

        /// <inheritdoc />
        public WorkflowType WorkflowType => WorkflowType.Optimize_Convert_Pro;

        /// <summary>
        /// Specifies the output file name. The default value is <c>output</c>.
        /// </summary>
        /// <remarks>Do not provide an extension for the file name. </remarks>
        public string OutputFilename { get; set; }

        /// <summary>
        /// The ouput file extension list. The default value is <c>["glb"]</c>.
        /// </summary>
        public string[] ExportFormats { get; set; }

        /// <summary>
        /// The optimization strategy to use. The default value is <c>OptimizeStrategy.Ratio</c>.
        /// </summary>
        public OptimizeStrategy? Strategy { get; set; }

        /// <summary>
        /// For target for processing the file. The default value is <c>100</c>.
        /// </summary>
        public int? StrategyTarget { get; set; }

        /// <summary>
        /// Whether to perform a scene optimization by merging some components. The default value is <c>false</c>.
        /// </summary>
        public bool? MergeOptimization { get; set; }

        /// <summary>
        /// Whether to perform a scene optimization by deleting unnecessary components. The default value is <c>false</c>.
        /// </summary>
        public bool? MeshCleaning { get; set; }

        /// <inheritdoc />
        public string[] InputFilePaths { get; set; }

        /// <inheritdoc />
        public Dictionary<string, object> GetExtraParameters()
        {
            return new Dictionary<string, object>
            {
                {"outputFileName", OutputFilename},
                {"exportFormats", ExportFormats},
                {"strategy", GetStrategy(Strategy)},
                {"target", StrategyTarget},
                {"mergeOptimization", MergeOptimization},
                {"meshCleaning", MeshCleaning},
            };
        }

        static string GetStrategy(OptimizeStrategy? strategy)
        {
            if (strategy.HasValue)
            {
                return strategy switch
                {
                    OptimizeStrategy.TriangleCount => "triangleCount",
                    OptimizeStrategy.Ratio => "ratio",
                    _ => null
                };
            }

            return null;
        }
    }
}
