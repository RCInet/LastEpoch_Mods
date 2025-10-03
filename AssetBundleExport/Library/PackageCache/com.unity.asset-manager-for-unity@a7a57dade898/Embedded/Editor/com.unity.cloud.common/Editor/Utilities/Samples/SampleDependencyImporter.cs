using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Unity.Cloud.CommonEmbedded.Editor
{
    [InitializeOnLoad]
    class SampleDependencyImporter : IPackageManagerExtension
    {
        /// <summary>
        /// An implementation of AssetPostProcessor which will raise an event when a new asset is imported.
        /// </summary>
        class SamplePostprocessor : AssetPostprocessor
        {
            public static event Action<string> OnAssetWillImport;

#pragma warning disable S1144 // Remove the unused private method
            void OnPreprocessAsset()
            {
                OnAssetWillImport?.Invoke(assetPath);
            }
#pragma warning restore S1144
        }

        static SampleDependencyImporter()
        {
            var importer = new SampleDependencyImporter();
            SamplePostprocessor.OnAssetWillImport += importer.LoadOnAssetDependencies;
            PackageManagerExtensions.RegisterExtension(importer);
        }

        const string k_PackageFilterPrefix = "com.unity.cloud.";
        const string k_SampleDependencyFilename = ".sample-dependencies.json";
        PackageInfo m_PackageInfo;
        List<Sample> m_Samples;
        SampleConfiguration m_SampleConfiguration;

        VisualElement IPackageManagerExtension.CreateExtensionUI() => default;
        public void OnPackageAddedOrUpdated(PackageInfo packageInfo) { /* Not required for sample dependency import*/ }
        public void OnPackageRemoved(PackageInfo packageInfo) { /* Not required for sample dependency import*/ }

        /// <summary>
        /// Called when the package selection changes in the Package Manager window.
        /// The dependency importer will track the selected package and its sample configuration.
        /// </summary>
        void IPackageManagerExtension.OnPackageSelectionChange(PackageInfo packageInfo)
        {
            // This method can be called multiple times by different internal systems. Do not assume
            // this is called only once every time the selection changes.
            if (packageInfo != null &&
                packageInfo.name.StartsWith(k_PackageFilterPrefix))
            {
                m_PackageInfo = packageInfo;
                m_Samples = GetSamples(packageInfo);
                TryLoadSampleConfiguration(m_PackageInfo, out m_SampleConfiguration);
            }
            else
            {
                m_PackageInfo = null;
                m_Samples = null;
                m_SampleConfiguration = null;
            }
        }

        /// <summary>
        /// Loads the sample configuration for the specified package, if one is available.
        /// </summary>
        static void TryLoadSampleConfiguration(PackageInfo packageInfo, out SampleConfiguration configuration)
        {
            configuration = null;

            var configurationPath = $"{packageInfo.assetPath}/{k_SampleDependencyFilename}";

            if (!File.Exists(configurationPath))
                return;

            var configurationText = File.ReadAllText(configurationPath);
            configuration = JsonSerialization.Deserialize<SampleConfiguration>(configurationText);
        }

        /// <summary>
        /// Handles loading common asset dependencies if required.
        /// </summary>
        void LoadOnAssetDependencies(string assetPath)
        {
            if (m_SampleConfiguration is null)
                return;

            var newFilesImported = false;
            var imported = new List<string>();

            for (int i = 0; i < m_Samples.Count; ++i)
            {
                // Import dependencies if we are importing the root directory of the sample.
                // Exclude assets imported in StreamingAssets since the path may end with the sample name.
                var isSampleDirectory = assetPath.EndsWith(m_Samples[i].displayName) && !assetPath.StartsWith("Assets/StreamingAssets");
                if (!isSampleDirectory)
                    continue;

                var resolvedPath = m_Samples[i].resolvedPath;

                if (imported.Contains(resolvedPath))
                    continue;

                imported.Add(resolvedPath);

                var sampleEntry = m_SampleConfiguration.GetEntry(m_Samples[i]);
                if (sampleEntry != null)
                {
                    // Import the common asset dependencies
                    newFilesImported |= ImportDependencies(m_PackageInfo, m_SampleConfiguration.SharedAssetDependencies);

                    // Import the sample-specific dependencies
                    newFilesImported |= ImportDependencies(m_PackageInfo, sampleEntry.AssetDependencies);
                }

                CopyStreamingAssets(m_PackageInfo.displayName, m_Samples[i].displayName, m_Samples[i].resolvedPath);
            }

            if (newFilesImported)
                CompilationPipeline.RequestScriptCompilation();
        }

        /// <summary>
        /// Copy the StreamingAssets directory content from the package into the project.
        /// </summary>
        static void CopyStreamingAssets(string packageName, string sampleName, string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            foreach (var streamingAssets in new []{ "StreamingAssets", "StreamingAssets~" })
            {
                var source = Path.GetFullPath($"{path}/{streamingAssets}");
                if (!Directory.Exists(source))
                    continue;

                if (!EditorUtility.DisplayDialog("Copy StreamingAssets", $"Copy StreamingAssets content from {sampleName} to your project StreamingAssets directory?", "Yes", "No"))
                    return;

                var destination = Path.GetFullPath($"{Application.dataPath}/StreamingAssets/{packageName}/{sampleName}");

                CopyDirectory(source, destination);
            }
        }

        /// <summary>
        /// Imports specified dependencies from the package into the project.
        /// </summary>
        static bool ImportDependencies(PackageInfo packageInfo, string[] paths)
        {
            if (paths == null)
                return false;

            var assetsImported = false;
            for (int i = 0; i < paths.Length; ++i)
            {
                var dependencyPath = Path.GetFullPath($"Packages/{packageInfo.name}/Samples~/{paths[i]}");
                if (Directory.Exists(dependencyPath))
                {
                    var samplePath = $"Samples/{packageInfo.displayName}/{packageInfo.version}/{paths[i]}";
                    CopyDirectory(dependencyPath, $"{Application.dataPath}/{samplePath}");
                    AssetDatabase.ImportAsset($"Assets/{samplePath}");
                    assetsImported = true;
                }
            }

            return assetsImported;
        }

        /// <summary>
        /// Returns all samples part of the specified package.
        /// </summary>
        /// <param name="packageInfo"></param>
        /// <returns></returns>
        static List<Sample> GetSamples(PackageInfo packageInfo)
        {
            // Find all samples for the package
            var samples = Sample.FindByPackage(packageInfo.name, packageInfo.version);
            return new List<Sample>(samples);
        }

        /// <summary>
        /// Copies a directory from the source to target path. Overwrites existing directories.
        /// </summary>
        static void CopyDirectory(string sourcePath, string targetPath)
        {
            // Verify source directory
            var source = new DirectoryInfo(sourcePath);
            if (!source.Exists)
                throw new DirectoryNotFoundException($"{sourcePath}  directory not found");

            // Delete pre-existing directory at target path
            var target = new DirectoryInfo(targetPath);
            if (target.Exists)
                target.Delete(true);

            Directory.CreateDirectory(targetPath);

            // Copy all files to target path
            foreach (FileInfo file in source.GetFiles())
            {
                var newFilePath = Path.Combine(targetPath, file.Name);
                file.CopyTo(newFilePath);
            }

            // Recursively copy all subdirectories
            foreach (DirectoryInfo child in source.GetDirectories())
            {
                var newDirectoryPath = Path.Combine(targetPath, child.Name);
                CopyDirectory(child.FullName, newDirectoryPath);
            }
        }
    }
}
