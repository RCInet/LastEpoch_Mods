using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    static class AssetDataDependencyHelper
    {
        const char k_AssetIdAssetVersionSeparator = '_';

        const string k_DependencyExtension = ".am4u_dep";
        const string k_GuidExtension = ".am4u_guid";

        [Obsolete("Only used for backwards compatibility with system file references.")]
        static void DecodeDependencySystemFilename(string filename, out string assetId, out string assetVersion)
        {
            assetId = string.Empty;
            assetVersion = string.Empty;

            if (Path.GetExtension(filename) != k_DependencyExtension)
                return;

            var filenameParts = Path.GetFileNameWithoutExtension(filename)
                .Split(k_AssetIdAssetVersionSeparator, StringSplitOptions.RemoveEmptyEntries);

            assetId = filenameParts[0];
            assetVersion = filenameParts.Length >= 2 ? filenameParts[1] : string.Empty;
        }

        static bool IsAGuidSystemFile(string filename)
        {
            // am4u_guid files are deprecated, but we still need this code to hide them in previously generated cloud assets.
            return filename.ToLower().EndsWith(k_GuidExtension);
        }

        static bool IsADependencySystemFile(string filename)
        {
            return filename.ToLower().EndsWith(k_DependencyExtension);
        }

        public static bool IsASystemFile(string filename)
        {
            return IsAGuidSystemFile(filename) || IsADependencySystemFile(filename);
        }

        public static async IAsyncEnumerable<AssetIdentifier> LoadDependenciesAsync(BaseAssetData assetData,
            [EnumeratorCancellation] CancellationToken token)
        {
            var assetsProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();

            // Flag for backwards compatibility - if the asset has no dependencies, check for system file dependencies
            var hasDependencies = false;

            await foreach (var dependency in assetsProvider.GetDependenciesAsync(assetData.Identifier, Range.All, token))
            {
                hasDependencies = true;
                yield return dependency.TargetAssetIdentifier;
            }

            if (!hasDependencies)
            {
#pragma warning disable 618 // Maintain for backwards compatibility with old assets that have dependencies stored in the asset itself
                var systemFileDependencies =
                    LoadSystemFileDependenciesAsync(assetsProvider, assetData, token);
                await foreach (var dependency in systemFileDependencies)
                {
                    yield return dependency;
                }
#pragma warning restore 618
            }
        }

        [Obsolete("Only used for backwards compatibility with system file references.")]
        static async IAsyncEnumerable<AssetIdentifier> LoadSystemFileDependenciesAsync(IAssetsProvider assetsProvider,
            BaseAssetData assetData, [EnumeratorCancellation] CancellationToken token)
        {
            var files = await GetFilesAsync(assetData, token);

            var dependencies = files.Where(f => IsADependencySystemFile(f.Path)).ToList();

            if (!dependencies.Any())
            {
                yield break;
            }

            foreach (var dependency in dependencies)
            {
                DecodeDependencySystemFilename(dependency.Path, out var assetId, out var assetVersion);

                var assetIdentifier = assetData.Identifier
                    .WithAssetId(assetId)
                    .WithVersion(assetVersion);

                // This will only occur with legacy dependencies that have no version defined.
                // When the version fails to parse, try to fetch the latest version from the provider
                if (string.IsNullOrEmpty(assetVersion))
                {
                    Utilities.DevLogWarning("Using legacy dependencies, consider updating the asset for better performance.");
                    assetVersion = await assetsProvider.GetLatestAssetVersionLiteAsync(assetIdentifier, token);
                    assetIdentifier = assetIdentifier.WithVersion(assetVersion);
                }

                // IN CASE no version is found, skip the dependency
                if (string.IsNullOrEmpty(assetVersion))
                {
                    Debug.LogError("No version found for dependency with asset id: " + assetId);
                    continue;
                }

                yield return assetIdentifier;
            }
        }

        public static BaseAssetData GetAssetAssociatedWithGuid(string assetGuid, string organizationId, string projectId = null)
        {
            Utilities.DevAssert(!string.IsNullOrEmpty(assetGuid));
            Utilities.DevAssert(!string.IsNullOrEmpty(organizationId));

            // First, check if the guid is associated with an asset that was already imported
            var assetDataManager = ServicesContainer.instance.Resolve<IAssetDataManager>();
            var importedAssetInfos = assetDataManager.GetImportedAssetInfosFromFileGuid(assetGuid);

            // Because we cannot yet know which cloud asset on the cloud contains which guid,
            // we can only rely on what the user has already imported.
            if (importedAssetInfos == null)
                return null; // If the user has not imported the asset, we cannot recycle it.

            var matches = new List<ImportedAssetInfo>();
            foreach (var importedAssetInfo in importedAssetInfos)
            {
                if (importedAssetInfo.FileInfos.Exists(
                        x => DependencyUtils.GetValidAssetDependencyGuids(x.Guid, true).Contains(assetGuid)))
                {
                    // This check is considered a hack because we assume any additional file that was added to a main asset is necessarily a dependency.
                    // This is to avoid having a dependency recycling the cloud asset of a parent cloud asset when uploaded using Embedded dependencies mode.
                    // This hack doesn't solve the case when multiple assets - that are not dependencies of each other - are uploaded into the same cloud asset.
                    // AMECO-3378 is supposed to find a solution to fix the issue and remove this hack.
                    continue;
                }

                var importedAssetData = importedAssetInfo.AssetData;

                // Imported asset must match current project and still exist in the provider for it to be recycled
                if (importedAssetData == null || importedAssetData.Identifier.OrganizationId != organizationId
                    || projectId != null && importedAssetData.Identifier.ProjectId != projectId)
                {
                    continue;
                }

                matches.Add(importedAssetInfo);
            }

            // we prioritize the most recent asset for the matches
            matches = matches.OrderByDescending(a => a.AssetData.Created ?? DateTime.MinValue).ToList();

            // Prioritize any imported asset where the guid is associated with a source dataset.
            // For context, in the example where optimize and convert files have been imported and then uploaded as separate assets,
            // we want to target the imported asset that contains those files in the source dataset and NOT the original optimize and convert dataset.
            foreach (var importedAssetInfo in matches)
            {
                var importedFileInfo = importedAssetInfo.FileInfos.FirstOrDefault(f => f.Guid == assetGuid);
                if (importedFileInfo != null && !string.IsNullOrEmpty(importedFileInfo.DatasetId))
                {
                    var dataset = importedAssetInfo.AssetData.Datasets.FirstOrDefault(d => d.Id == importedFileInfo.DatasetId);
                    if (dataset is {IsSource: true})
                    {
                        return importedAssetInfo.AssetData;
                    }
                }
            }

            // It is possible that multiple assets contain the same guid, this use case will be addressed in the future
            return matches.FirstOrDefault()?.AssetData;
        }

        static async Task<List<BaseAssetDataFile>> GetFilesAsync(BaseAssetData assetData, CancellationToken token)
        {
            var files = assetData.GetFiles();
            if (!files.Any() && assetData is AssetData ad)
            {
                await ad.ResolveDatasetsAsync(token);
                files = assetData.GetFiles();
            }

            return files.ToList();
        }
    }
}
