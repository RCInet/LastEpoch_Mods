using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// An interface that provides all the methods to interact with asset entities.
    /// </summary>
    interface IAssetRepository
    {
        /// <summary>
        /// The caching configuration for the asset repository entities.
        /// </summary>
        AssetRepositoryCacheConfiguration CacheConfiguration => throw new NotImplementedException();

        /// <summary>
        /// Returns a builder to create a query to search an organization's <see cref="IAssetProject"/>.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <returns>An <see cref="AssetProjectQueryBuilder"/>. </returns>
        AssetProjectQueryBuilder QueryAssetProjects(OrganizationId organizationId);

        /// <summary>
        /// Gets an organization's <see cref="IAssetProject"/> for current user.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information for identifying the project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IAssetProject"/>. </returns>
        Task<IAssetProject> GetAssetProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Enables a pre-existing dashboard project in asset manager.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information for identifying the project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IAssetProject"/>. </returns>
        Task<IAssetProject> EnableProjectForAssetManagerAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Enables a pre-existing dashboard project in asset manager.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information for identifying the project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task EnableProjectForAssetManagerLiteAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Creates a new <see cref="IAssetProject"/> in the specified organization.
        /// </summary>
        /// <param name="organizationId">The organization to create the project in. </param>
        /// <param name="projectCreation">The object containing the necessary information to create a new project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the new <see cref="IAssetProject"/>. </returns>
        Task<IAssetProject> CreateAssetProjectAsync(OrganizationId organizationId, IAssetProjectCreation projectCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new project in the specified organization.
        /// </summary>
        /// <param name="organizationId">The organization to create the project in. </param>
        /// <param name="projectCreation">The object containing the necessary information to create a new project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the new project's <see cref="ProjectDescriptor"/>. </returns>
        Task<ProjectDescriptor> CreateAssetProjectLiteAsync(OrganizationId organizationId, IAssetProjectCreation projectCreation, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Gets an <see cref="IAssetCollection"/>.
        /// </summary>
        /// <param name="collectionDescriptor">The object containing the necessary information for identifying the collection. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IAssetCollection"/></returns>
        Task<IAssetCollection> GetAssetCollectionAsync(CollectionDescriptor collectionDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a builder which can query for assets in a set of projects.
        /// </summary>
        /// <param name="projectDescriptors">The projects to search. They must all belong to the same organization. </param>
        /// <returns>An <see cref="AssetQueryBuilder"/>. </returns>
        AssetQueryBuilder QueryAssets(IEnumerable<ProjectDescriptor> projectDescriptors);

        /// <summary>
        /// Returns a builder which can query for assets in an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization.</param>
        /// <returns>An <see cref="AssetQueryBuilder"/>. </returns>
        AssetQueryBuilder QueryAssets(OrganizationId organizationId) => throw new NotImplementedException();

        /// <summary>
        /// Returns a builder which can query the asset count for a set of projects.
        /// </summary>
        /// <param name="projectDescriptors">The projects to search. They must all belong to the same organization. </param>
        /// <returns>An <see cref="GroupAndCountAssetsQueryBuilder"/>. </returns>
        GroupAndCountAssetsQueryBuilder GroupAndCountAssets(IEnumerable<ProjectDescriptor> projectDescriptors);

        /// <summary>
        /// Returns a builder which can query the asset count for an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization.</param>
        /// <returns>An <see cref="GroupAndCountAssetsQueryBuilder"/>. </returns>
        GroupAndCountAssetsQueryBuilder GroupAndCountAssets(OrganizationId organizationId) => throw new NotImplementedException();

        /// <summary>
        /// Retrieves an <see cref="IAsset"/> by its id and version.
        /// </summary>
        /// <param name="assetDescriptor">The descriptor containing identifiers for the asset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IAsset"/>. </returns>
        Task<IAsset> GetAssetAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an <see cref="IAsset"/> by its id and label.
        /// </summary>
        /// <param name="projectDescriptor">The project the asset belongs to. </param>
        /// <param name="assetId">The id of the asset. </param>
        /// <param name="label">The label associated to the asset version. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IAsset"/>. </returns>
        Task<IAsset> GetAssetAsync(ProjectDescriptor projectDescriptor, AssetId assetId, string label, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Retrieves an <see cref="IDataset"/> from an asset version.
        /// </summary>
        /// <param name="datasetDescriptor">The descriptor containing identifiers for the dataset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IDataset"/>. </returns>
        Task<IDataset> GetDatasetAsync(DatasetDescriptor datasetDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an <see cref="ITransformation"/> from a dataset.
        /// </summary>
        /// <param name="transformationDescriptor">The descriptor containing identifiers for the transformation. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="ITransformation"/>. </returns>
        Task<ITransformation> GetTransformationAsync(TransformationDescriptor transformationDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an <see cref="IFile"/> from a dataset.
        /// </summary>
        /// <param name="fileDescriptor">The descriptor containing identifiers for the file. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IFile"/>. </returns>
        Task<IFile> GetFileAsync(FileDescriptor fileDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a builder to create a query to search an organization's <see cref="IFieldDefinition"/>.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <returns>A <see cref="FieldDefinitionQueryBuilder"/>. </returns>
        FieldDefinitionQueryBuilder QueryFieldDefinitions(OrganizationId organizationId);

        /// <summary>
        /// Retrieves an <see cref="IFieldDefinition"/>.
        /// </summary>
        /// <param name="fieldDefinitionDescriptor">The descriptor containing the indentifiers for the field definition. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IFieldDefinition"/>. </returns>
        Task<IFieldDefinition> GetFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new field definition within an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization in which to add the field definitino. </param>
        /// <param name="fieldDefinitionCreation">The object containing the necessary information to create a field definition.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the newly created <see cref="IFieldDefinition"/>. </returns>
        Task<IFieldDefinition> CreateFieldDefinitionAsync(OrganizationId organizationId, IFieldDefinitionCreation fieldDefinitionCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new field definition within an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization in which to add the field definitino. </param>
        /// <param name="fieldDefinitionCreation">The object containing the necessary information to create a field definition.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the newly created field definition's <see cref="FieldDefinitionDescriptor"/>. </returns>
        Task<FieldDefinitionDescriptor> CreateFieldDefinitionLiteAsync(OrganizationId organizationId, IFieldDefinitionCreation fieldDefinitionCreation, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Deletes an <see cref="IFieldDefinition"/>.
        /// </summary>
        /// <param name="fieldDefinitionDescriptor">The descriptor containing the indentifiers for the field definition. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task DeleteFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new label within an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization in which to add the label. </param>
        /// <param name="labelCreation">The object containing the necessary information to create a label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="ILabel"/>. </returns>
        Task<ILabel> CreateLabelAsync(OrganizationId organizationId, ILabelCreation labelCreation, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Creates a new label within an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization in which to add the label. </param>
        /// <param name="labelCreation">The object containing the necessary information to create a label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the new label's <see cref="LabelDescriptor"/>. </returns>
        Task<LabelDescriptor> CreateLabelLiteAsync(OrganizationId organizationId, ILabelCreation labelCreation, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Returns a builder to create a query to search an organization's labels.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <returns>A <see cref="LabelQueryBuilder"/>. </returns>
        LabelQueryBuilder QueryLabels(OrganizationId organizationId) => throw new NotImplementedException();

        /// <summary>
        /// Retrieves a label by name.
        /// </summary>
        /// <param name="labelDescriptor">The descriptor containing the indentifiers for the label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="ILabel"/>. </returns>
        Task<ILabel> GetLabelAsync(LabelDescriptor labelDescriptor, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Returns a builder to create a query to search an organization's <see cref="IStatusFlow"/>.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <returns>A <see cref="StatusFlowQueryBuilder"/>. </returns>
        StatusFlowQueryBuilder QueryStatusFlows(OrganizationId organizationId) => throw new NotImplementedException();

        /// <summary>
        /// Implement this method to get an <see cref="AssetDescriptor"/> given a serialized json of asset identifiers.
        /// </summary>
        /// <param name="jsonSerialization">The serialization of an asset's identifiers. Accepts the result of <see cref="IAsset.SerializeIdentifiers"/>. </param>
        /// <returns>An <see cref="AssetDescriptor"/>. </returns>
        [Obsolete("Use Common.AssetDescriptor.FromJson instead.")]
        AssetDescriptor DeserializeAssetIdentifiers(string jsonSerialization);

        /// <summary>
        /// Retrieves an <see cref="IAsset"/> from a serialized JSON string. The <see cref="IAssetRepository"/> is responsible for injecting the necessary dependencies into the asset.
        /// </summary>
        /// <remarks>To serialize an asset use <see cref="IAsset.Serialize"/>. </remarks>
        /// <param name="jsonSerialization">The serialized JSON string of an asset. </param>
        /// <returns>An <see cref="IAsset"/>. </returns>
        [Obsolete("IAsset serialization is no longer supported.")]
        IAsset DeserializeAsset(string jsonSerialization);
    }
}
