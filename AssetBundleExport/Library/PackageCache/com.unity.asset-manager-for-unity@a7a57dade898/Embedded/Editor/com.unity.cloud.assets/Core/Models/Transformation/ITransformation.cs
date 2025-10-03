using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    interface ITransformation
    {
        /// <summary>
        /// The descriptor of the transformation.
        /// </summary>
        TransformationDescriptor Descriptor { get; }

        /// <summary>
        /// The ID of the Dataset on which the transformation is applied
        /// </summary>
        [Obsolete("Use Descriptor.DatasetId instead.")]
        DatasetId InputDatasetId => Descriptor.DatasetId;

        /// <summary>
        /// The ID of the Dataset that will be created by the transformation if any
        /// </summary>
        [Obsolete("Use TransformationProperties.OutputDatasetId instead.")]
        DatasetId OutputDatasetId { get; }

        /// <summary>
        /// The ID of the Dataset that will be linked to the transformation if any
        /// </summary>
        [Obsolete("Use TransformationProperties.LinkDatasetId instead.")]
        DatasetId LinkDatasetId { get; }

        /// <summary>
        /// The files on which the transformation is applied
        /// </summary>
        [Obsolete("Use TransformationProperties.InputFilePaths instead.")]
        IEnumerable<string> InputFiles { get; }

        /// <summary>
        /// The type of transformation
        /// </summary>
        [Obsolete("Use TransformationProperties.WorkflowType instead.")]
        WorkflowType WorkflowType { get; }

        /// <summary>
        /// The name of the workflow
        /// </summary>
        [Obsolete("Use TransformationProperties.WorkflowName instead.")]
        string WorkflowName => WorkflowType.ToJsonValue();

        /// <summary>
        /// The status of the transformation
        /// </summary>
        [Obsolete("Use TransformationProperties.StatusName instead.")]
        TransformationStatus Status { get; }

        /// <summary>
        /// If the transformation failed, this will contain the associated error message
        /// </summary>
        [Obsolete("Use TransformationProperties.ErrorMessage instead.")]
        string ErrorMessage { get; }

        /// <summary>
        /// The progress of the transformation. This is a value between 0 and 100.
        /// </summary>
        [Obsolete("Use TransformationProperties.Progress instead.")]
        int Progress => 0;

        /// <summary>
        /// The datetime at which the transformation was created
        /// </summary>
        [Obsolete("Use TransformationProperties.Created instead.")]
        DateTime CreatedOn { get; }

        /// <summary>
        /// The datetime at which the transformation was last updated
        /// </summary>
        [Obsolete("Use TransformationProperties.Updated instead.")]
        DateTime UpdatedAt { get; }

        /// <summary>
        /// The datetime at which the transformation was started
        /// </summary>
        [Obsolete("Use TransformationProperties.Started instead.")]
        DateTime StartedAt { get; }

        /// <summary>
        /// The user ID of the user who started the transformation
        /// </summary>
        [Obsolete("Use TransformationProperties.UserId instead.")]
        UserId UserId => throw new NotImplementedException();

        /// <summary>
        /// The job ID of the transformation
        /// </summary>
        [Obsolete("Use TransformationProperties.JobId instead.")]
        string JobId { get; }

        /// <summary>
        /// The caching configuration for the transformation.
        /// </summary>
        TransformationCacheConfiguration CacheConfiguration => throw new NotImplementedException();

        /// <summary>
        /// Returns a transformation configured with the specified caching configuration.
        /// </summary>
        /// <param name="transformationCacheConfiguration">The caching configuration for the transformation. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="ITransformation"/> with cached values specified by the caching configurations. </returns>
        Task<ITransformation> WithCacheConfigurationAsync(TransformationCacheConfiguration transformationCacheConfiguration, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Refreshes the transformation properties.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task RefreshAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the properties of the transformation.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the <see cref="TransformationProperties"/> of the transformation. </returns>
        Task<TransformationProperties> GetPropertiesAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Cancels the transformation.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task TerminateAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
