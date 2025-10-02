using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial interface IAssetDataSource
    {
        /// <summary>
        /// Retrieves a list of <see cref="IProjectData"/> for an organization for the current user.
        /// </summary>
        /// <param name="organizationId">The organization id. </param>
        /// <param name="pagination">An object containing the necessary information return a range of projects. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an async enumeration of projects. </returns>
        IAsyncEnumerable<IProjectData> ListProjectsAsync(OrganizationId organizationId, PaginationData pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an <see cref="IProjectData"/> for an organization.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a project. </returns>
        Task<IProjectData> GetProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Enables a project for Asset Manager.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task EnableProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new project in an organization.
        /// </summary>
        /// <param name="organizationId">The organization id. </param>
        /// <param name="projectCreation">The object containing the necessary information to create a project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a newly created project. </returns>
        Task<ProjectDescriptor> CreateProjectAsync(OrganizationId organizationId, IProjectBaseData projectCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the number of collections in a project.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is the number of collections in the project. </returns>
        Task<int> GetCollectionCountAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the number of assets in a project.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is the number of assets in the project. </returns>
        Task<int> GetAssetCountAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Adds assets to the project.
        /// </summary>
        /// <param name="sourceProject">The object containing the necessary information to identify the source project.</param>
        /// <param name="destinationProject">The object containing the necessary information to identify the destination project.</param>
        /// <param name="assetIds">The ids of the assets to link.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task LinkAssetsToProjectAsync(ProjectDescriptor sourceProject, ProjectDescriptor destinationProject, IEnumerable<AssetId> assetIds, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the assets from the project.
        /// </summary>
        /// <param name="sourceProject">The object containing the necessary information to identify the source project.</param>
        /// <param name="assetIds">The ids of the assets to unlink.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UnlinkAssetsFromProjectAsync(ProjectDescriptor sourceProject, IEnumerable<AssetId> assetIds, CancellationToken cancellationToken);
    }
}
