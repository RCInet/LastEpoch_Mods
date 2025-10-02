using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial class AssetDataSource
    {
        /// <inheritdoc/>
        public async IAsyncEnumerable<IProjectData> ListProjectsAsync(OrganizationId organizationId, PaginationData pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 99;

            var (offset, length) = await pagination.Range.GetOffsetAndLengthAsync(_cancellationToken =>
            {
                _cancellationToken.ThrowIfCancellationRequested();
                return Task.FromResult(int.MaxValue);
            }, cancellationToken);

            if (length == 0) yield break;

            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));
            var pageNumber = offset / pageSize + 1;

            var startIndex = offset % pageSize;
            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = new ListProjectsRequest(organizationId, pageNumber, pageSize);
                var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                    cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var projectPageDto = IsolatedSerialization.DeserializeWithDefaultConverters<ProjectPageDto>(jsonContent);

                ++pageNumber;

                if (projectPageDto.Projects == null || projectPageDto.Projects.Length == 0) break;

                for (var i = 0; i < projectPageDto.Projects.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (count == 0 && i < startIndex) continue;
                    if (count >= length) break;

                    ++count;
                    yield return projectPageDto.Projects[i];
                }
            } while (count < length);
        }

        /// <inheritdoc/>
        public async Task<IProjectData> GetProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = ProjectRequest.GetProjectRequset(projectDescriptor.ProjectId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<ProjectData>(jsonContent);
        }

        /// <inheritdoc/>
        public Task EnableProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = ProjectRequest.GetEnableProjectRequest(projectDescriptor.ProjectId);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<ProjectDescriptor> CreateProjectAsync(OrganizationId organizationId, IProjectBaseData projectCreation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateProjectRequest(organizationId, projectCreation);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var projectDto = IsolatedSerialization.DeserializeWithDefaultConverters<CreatedProjectDto>(jsonContent);

            return new ProjectDescriptor(organizationId, new ProjectId(projectDto.Id));
        }

        /// <inheritdoc/>
        public async Task<int> GetCollectionCountAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = ProjectRequest.GetCollectionCountRequest(projectDescriptor.ProjectId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<CounterDto>(jsonContent).Count;
        }

        /// <inheritdoc/>
        public async Task<int> GetAssetCountAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = ProjectRequest.GetAssetCountRequest(projectDescriptor.ProjectId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<CounterDto>(jsonContent).Count;
        }

        /// <inheritdoc />
        public Task LinkAssetsToProjectAsync(ProjectDescriptor sourceProject, ProjectDescriptor destinationProject, IEnumerable<AssetId> assetIds, CancellationToken cancellationToken)
        {
            return SplitRequest(assetIds, ids => new LinkAssetToProjectRequest(sourceProject.ProjectId, destinationProject.ProjectId, ids), cancellationToken);
        }

        /// <inheritdoc />
        public Task UnlinkAssetsFromProjectAsync(ProjectDescriptor sourceProject, IEnumerable<AssetId> assetIds, CancellationToken cancellationToken)
        {
            return SplitRequest(assetIds, ids => new UnlinkAssetFromProjectRequest(sourceProject.ProjectId, ids), cancellationToken);
        }

        Task SplitRequest(IEnumerable<AssetId> assetIds, Func<IEnumerable<AssetId>, ApiRequest> buildRequest, CancellationToken cancellationToken)
        {
            const int maxPageSize = 50;

            var assetIdArray = assetIds.ToArray();

            var tasks = new List<Task>();
            for (var i = 0; i * maxPageSize < assetIdArray.Length; ++i)
            {
                var request = buildRequest(assetIdArray.Skip(i * maxPageSize).Take(maxPageSize));
                var task = RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                    ServiceHttpClientOptions.Default(), cancellationToken);
                tasks.Add(task);
            }

            return Task.WhenAll(tasks);
        }
    }
}
