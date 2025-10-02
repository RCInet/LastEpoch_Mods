using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.AssetsEmbedded
{
    interface ITransformationCreation
    {
        /// <summary>
        /// The type of workflow to execute.
        /// </summary>
        WorkflowType WorkflowType { get; }

        /// <summary>
        /// The name of the custom workflow to execute.
        /// </summary>
        string CustomWorkflowName => null;

        /// <summary>
        /// The input file paths to process (if any).
        /// </summary>
        string[] InputFilePaths { get; }

        /// <summary>
        /// Any additional parameters to pass to the workflow.
        /// </summary>
        /// <returns>A set of paramters to pass to the request. </returns>
        [Obsolete("Use GetExtraParameters() instead.")]
        Dictionary<string, string> GetParameters() => new();

        /// <summary>
        /// Any additional parameters to pass to the workflow.
        /// </summary>
        /// <returns>A set of paramters to pass to the request. </returns>
#pragma warning disable 618 // Obsolete warning - default maintains backwards compatibility
        Dictionary<string, object> GetExtraParameters() => GetParameters()?.ToDictionary(x => x.Key, x => (object) x.Value);
#pragma warning restore 618 // Obsolete warning
    }
}
