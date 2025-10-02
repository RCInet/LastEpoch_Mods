using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial interface IAssetDataSource
    {
        /// <summary>
        /// Starts a transformation on the specified dataset.
        /// </summary>
        /// <param name="datasetDescriptor">The object containing the necessary information to identify the dataset on which to start the transformation.</param>
        /// <param name="workflowType">The type of workflow that will be applied in the transformation.</param>
        /// <param name="inputFiles">The files to include in the transformation. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>The ID of the transformation </returns>
        Task<TransformationId> StartTransformationAsync(DatasetDescriptor datasetDescriptor, string workflowType, string[] inputFiles, Dictionary<string, object> parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Get a transformation on the specified dataset.
        /// </summary>
        /// <param name="transformationDescriptor">The object containing the necessary information to identify the transformation.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A DTO of the transformation</returns>
        Task<ITransformationData> GetTransformationAsync(TransformationDescriptor transformationDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Searches for transformations given the search criteria.
        /// </summary>
        /// <param name="projectDescriptor">A project to search in. </param>
        /// <param name="searchData">An object containing search criteria. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is an array of transformation DTOs satisfying the search. </returns>
        Task<ITransformationData[]> GetTransformationsAsync(ProjectDescriptor projectDescriptor, TransformationSearchData searchData, CancellationToken cancellationToken);

        /// <summary>
        /// Cancels a transformation.
        /// </summary>
        /// <param name="projectDescriptor">The project to search in. </param>
        /// <param name="transformationId">The id of the transformation. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result.</returns>
        Task TerminateTransformationAsync(ProjectDescriptor projectDescriptor, TransformationId transformationId, CancellationToken cancellationToken);
    }
}
