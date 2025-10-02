using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial class AssetDataSource
    {
        /// <inheritdoc/>
        public async Task<TransformationId> StartTransformationAsync(DatasetDescriptor datasetDescriptor, string workflowType, string[] inputFiles, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new StartTransformationRequest(workflowType, inputFiles, parameters, datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var startedTransformationResponse = IsolatedSerialization.DeserializeWithConverters<StartedTransformationDto>(jsonContent, IsolatedSerialization.TransformationIdConverter);

            return startedTransformationResponse.TransformationId;
        }

        /// <inheritdoc/>
        public async Task<ITransformationData> GetTransformationAsync(TransformationDescriptor transformationDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GetTransformationRequest(transformationDescriptor.TransformationId,
                transformationDescriptor.ProjectId, transformationDescriptor.AssetId,
                transformationDescriptor.AssetVersion, transformationDescriptor.DatasetId);

            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<TransformationData>(jsonContent);
        }

        /// <inheritdoc/>
        public async Task<ITransformationData[]> GetTransformationsAsync(ProjectDescriptor projectDescriptor, TransformationSearchData searchData, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new SearchTransformationRequest(projectDescriptor.ProjectId, searchData);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var transformations = IsolatedSerialization.DeserializeWithDefaultConverters<TransformationData[]>(jsonContent);
            return transformations.Select(x => (ITransformationData)x).ToArray();
        }

        /// <inheritdoc/>
        public async Task TerminateTransformationAsync(ProjectDescriptor projectDescriptor, TransformationId transformationId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new TerminateTransformationRequest(projectDescriptor.ProjectId, transformationId);
            await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }
    }
}
