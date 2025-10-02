using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    interface ITransformationData
    {
        /// <summary>
        /// The ID of the transformation.
        /// </summary>
        [DataMember(Name = "id")]
        TransformationId Id { get; }

        /// <summary>
        /// The user id.
        /// </summary>
        [DataMember(Name = "userId")]
        UserId UserId { get; }

        /// <summary>
        /// The ID of the asset containing the dataset on which the transformation is applied.
        /// </summary>
        [DataMember(Name = "assetId")]
        AssetId AssetId { get; }

        /// <summary>
        /// The version of the asset containing the dataset on which the transformation is applied.
        /// </summary>
        [DataMember(Name = "assetVersion")]
        AssetVersion AssetVersion { get; }

        /// <summary>
        /// The dataset ID on which the transformation is applied.
        /// </summary>
        [DataMember(Name = "inputDatasetId")]
        DatasetId InputDatasetId {get;}

        /// <summary>
        /// The dataset ID that will be created by the transformation if any.
        /// </summary>
        [DataMember(Name = "outputDatasetId")]
        DatasetId OutputDatasetId { get; }

        /// <summary>
        /// The dataset ID that will be linked by the transformation if any.
        /// </summary>
        [DataMember(Name = "linkDatasetId")]
        DatasetId LinkDatasetId { get; }

        /// <summary>
        /// The files on which the transformation is applied if any were specified.
        /// </summary>
        [DataMember(Name = "inputFiles")]
        IEnumerable<string> InputFiles { get; }

        /// <summary>
        /// The type of workflow used by the transformation.
        /// </summary>
        [DataMember(Name = "workflowType")]
        string WorkflowType { get; }

        /// <summary>
        /// The status of the transformation.
        /// </summary>
        [DataMember(Name = "status")]
        TransformationStatus Status { get; }

        /// <summary>
        /// If the transformation failed, this will contain the associated error message.
        /// </summary>
        [DataMember(Name = "ErrorMessage")]
        string ErrorMessage { get; }

        /// <summary>
        /// Transformation progress status. In percent.
        /// </summary>
        [DataMember(Name = "progress")]
        int Progress { get; }

        /// <summary>
        /// The datetime at which the transformation was created.
        /// </summary>
        [DataMember(Name = "createdOn")]
        DateTime CreatedOn { get; }

        /// <summary>
        /// The datetime at which the transformation was last updated.
        /// </summary>
        [DataMember(Name = "updatedAt")]
        DateTime UpdatedAt { get; }

        /// <summary>
        /// The datetime at which the transformation was started.
        /// </summary>
        [DataMember(Name = "startedAt")]
        DateTime StartedAt { get; }

        /// <summary>
        /// The job ID of the transformation
        /// </summary>
        [DataMember(Name = "jobId")]
        string JobId { get; }
    }
}
