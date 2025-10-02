using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial class AssetDataSource
    {
        /// <inheritdoc />
        public async Task<IEnumerable<IAssetCollectionData>> GetAssetCollectionsAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GetAssetCollectionsRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var assetCollectionDtos = DeserializeCollectionPath<AssetCollectionData[]>(jsonContent);

            return assetCollectionDtos ?? Array.Empty<AssetCollectionData>();
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<IAssetCollectionData> ListCollectionsAsync(ProjectDescriptor projectDescriptor, Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GetCollectionListRequest(projectDescriptor.ProjectId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var collectionListDto = DeserializeCollectionPath<AssetCollectionData[]>(jsonContent);
            if (collectionListDto == null || collectionListDto.Length == 0)
            {
                yield break;
            }

            var (start, length) = range.GetValidatedOffsetAndLength(collectionListDto.Length);
            for (var i = start; i < start + length; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return collectionListDto[i];
            }
        }

        /// <inheritdoc/>
        public async Task<IAssetCollectionData> GetCollectionAsync(CollectionDescriptor collectionDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CollectionRequest(collectionDescriptor.ProjectId, collectionDescriptor.Path);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return DeserializeCollectionPath<AssetCollectionData>(jsonContent);
        }

        /// <inheritdoc/>
        public async Task<CollectionPath> CreateCollectionAsync(ProjectDescriptor projectDescriptor, IAssetCollectionData assetCollection, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateCollectionRequest(projectDescriptor.ProjectId, assetCollection);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var pathDto = DeserializeCollectionPath<AssetCollectionPathDto>(jsonContent);

            return pathDto.Path;
        }

        /// <inheritdoc/>
        public Task UpdateCollectionAsync(CollectionDescriptor collectionDescriptor, IAssetCollectionData assetCollection, CancellationToken cancellationToken)
        {
            var request = new CollectionRequest(collectionDescriptor.ProjectId, collectionDescriptor.Path, assetCollection);
            return RateLimitedServiceClient(request, HttpClientExtensions.HttpMethodPatch).PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc/>
        public Task DeleteCollectionAsync(CollectionDescriptor collectionDescriptor, CancellationToken cancellationToken)
        {
            var request = new CollectionRequest(collectionDescriptor.ProjectId, collectionDescriptor.Path);
            return RateLimitedServiceClient(request, HttpMethod.Delete).DeleteAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task AddAssetsToCollectionAsync(CollectionDescriptor collectionDescriptor, IEnumerable<AssetId> assets, CancellationToken cancellationToken)
        {
            var request = new ModifyAssetsInCollectionRequest(collectionDescriptor.ProjectId, collectionDescriptor.Path, assets);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveAssetsFromCollectionAsync(CollectionDescriptor collectionDescriptor, IEnumerable<AssetId> assets, CancellationToken cancellationToken)
        {
            var request = new ModifyAssetsInCollectionRequest(collectionDescriptor.ProjectId, collectionDescriptor.Path, assets);
            return RateLimitedServiceClient(request, HttpClientExtensions.HttpMethodPatch).PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CollectionPath> MoveCollectionToNewPathAsync(CollectionDescriptor collectionDescriptor, CollectionPath newCollectionPath, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new MoveCollectionToNewPathRequest(collectionDescriptor.ProjectId, collectionDescriptor.Path, newCollectionPath);
            var response = await RateLimitedServiceClient(request, HttpClientExtensions.HttpMethodPatch).PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var pathDto = DeserializeCollectionPath<AssetCollectionPathDto>(jsonContent);

            return pathDto.Path;
        }

        static T DeserializeCollectionPath<T>(string json)
        {
            return IsolatedSerialization.DeserializeWithConverters<T>(json, IsolatedSerialization.CollectionPathConverter);
        }
    }
}
