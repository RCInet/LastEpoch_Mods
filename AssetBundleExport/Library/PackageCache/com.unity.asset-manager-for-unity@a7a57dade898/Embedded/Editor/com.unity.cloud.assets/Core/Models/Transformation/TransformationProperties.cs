using System;
using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The properties of an <see cref="ITransformation"/>.
    /// </summary>
    struct TransformationProperties
    {
        /// <summary>
        /// The id of the Dataset that will be created by the transformation if any
        /// </summary>
        public DatasetId OutputDatasetId { get; set; }

        /// <summary>
        /// The id of the Dataset that will be linked to the transformation if any
        /// </summary>
        public DatasetId LinkDatasetId { get; internal set; }

        /// <summary>
        /// The files on which the transformation is applied
        /// </summary>
        public IEnumerable<string> InputFilePaths { get; internal set; }

        /// <summary>
        /// The type of transformation
        /// </summary>
        public WorkflowType WorkflowType { get; internal set; }

        /// <summary>
        /// The identifying name of the transformation
        /// </summary>
        public string WorkflowName { get; internal set; }

        /// <summary>
        /// The status of the transformation
        /// </summary>
        public TransformationStatus Status { get; internal set; }

        /// <summary>
        /// If the transformation failed, this will contain the associated error message
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// The progress of the transformation. This is a value between 0 and 100.
        /// </summary>
        public int Progress { get; internal set; }

        /// <summary>
        /// The datetime at which the transformation was created
        /// </summary>
        public DateTime Created { get; internal set; }

        /// <summary>
        /// The datetime at which the transformation was last updated
        /// </summary>
        public DateTime Updated { get; internal set; }

        /// <summary>
        /// The datetime at which the transformation was started
        /// </summary>
        public DateTime Started { get; internal set; }

        /// <summary>
        /// The user id of the user who started the transformation
        /// </summary>
        public UserId UserId { get; internal set; }

        /// <summary>
        /// The job id of the transformation
        /// </summary>
        public string JobId { get; internal set; }
    }
}
