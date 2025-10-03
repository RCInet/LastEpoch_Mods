using System;
using UnityEditor;

namespace Unity.AssetManager.Upload.Editor
{
    enum UploadAssetMode
    {
        SkipIdentical, // Upload a new version of asset and skips any existing asset on the cloud that are identical
        ForceNewVersion, // Always force a new version on the asset
        ForceNewAsset, // Uploads new assets and potentially duplicates without checking for existing matches
    }

    enum UploadDependencyMode
    {
        Ignore, // Do not add any dependencies
        Separate, // Add dependencies as separate assets
        Embedded, // Add dependencies as files in the parent asset
    }

    enum UploadFilePathMode
    {
        Full, // Keep the path relative to the project Assets folder
        Compact, // Reduce files nesting by removing common path parts
        Flatten, // Flatten all files to the root of the asset and rename them in case of collision
    }

    [Serializable]
    class UploadSettings
    {
        static readonly string k_UploadModePrefKey = "com.unity.asset-manager-for-unity.reupload-mode";
        static readonly string k_UploadDependencyModePrefKey = "com.unity.asset-manager-for-unity.upload-dependency-mode";
        static readonly string k_UploadFlattenFilePathsPrefKey = "com.unity.asset-manager-for-unity.upload-flatten-file-paths";

        static readonly UploadAssetMode k_DefaultUploadAssetMode = UploadAssetMode.SkipIdentical;
        static readonly UploadDependencyMode k_DefaultUploadDependencyMode = UploadDependencyMode.Separate;
        static readonly UploadFilePathMode k_DefaultUploadFilePathMode = UploadFilePathMode.Full;

        public string OrganizationId;
        public string ProjectId;
        public string CollectionPath;

        public UploadAssetMode UploadMode
        {
            get => SavedUploadMode;
            set => SavedUploadMode = value;
        }

        public UploadDependencyMode DependencyMode
        {
            get => SavedDependencyMode;
            set => SavedDependencyMode = value;
        }

        public UploadFilePathMode FilePathMode
        {
            get => SavedFilePathMode;
            set => SavedFilePathMode = value;
        }

        UploadAssetMode SavedUploadMode
        {
            set => EditorPrefs.SetInt(k_UploadModePrefKey, (int)value);
            get => (UploadAssetMode)EditorPrefs.GetInt(k_UploadModePrefKey, (int)k_DefaultUploadAssetMode);
        }

        UploadDependencyMode SavedDependencyMode
        {
            set => EditorPrefs.SetInt(k_UploadDependencyModePrefKey, (int)value);
            get => (UploadDependencyMode)EditorPrefs.GetInt(k_UploadDependencyModePrefKey, (int)k_DefaultUploadDependencyMode);
        }

        UploadFilePathMode SavedFilePathMode
        {
            set => EditorPrefs.SetInt(k_UploadFlattenFilePathsPrefKey, (int)value);
            get => (UploadFilePathMode)EditorPrefs.GetInt(k_UploadFlattenFilePathsPrefKey, (int)k_DefaultUploadFilePathMode);
        }

        public void ResetToDefault()
        {
            SavedUploadMode = k_DefaultUploadAssetMode;
            SavedDependencyMode = k_DefaultUploadDependencyMode;
            SavedFilePathMode = k_DefaultUploadFilePathMode;
        }

        public static string GetUploadModeTooltip(UploadAssetMode mode)
        {
            return mode switch
            {
                UploadAssetMode.ForceNewAsset => L10n.Tr("Uploads new assets and potentially duplicates without checking for existing matches"),

                UploadAssetMode.SkipIdentical => L10n.Tr("For existing cloud Assets, a new version will be created only if local changes are detected"),

                UploadAssetMode.ForceNewVersion => L10n.Tr("For existing cloud Assets, a new version will always be created regardless of local changes"),

                _ => null
            };
        }

        public static string GetDependencyModeTooltip(UploadDependencyMode mode)
        {
            return mode switch
            {
                UploadDependencyMode.Ignore => L10n.Tr("Dependencies will be ignored and not uploaded"),

                UploadDependencyMode.Separate => L10n.Tr("Dependencies will be uploaded as separate Cloud Asset and the parent Cloud Asset will have a reference to them"),

                UploadDependencyMode.Embedded => L10n.Tr("Dependencies will have be added as files in the parent Cloud Asset"),

                _ => null
            };
        }

        public static string GetFilePathModeTooltip(UploadFilePathMode mode)
        {
            return mode switch
            {
                UploadFilePathMode.Full => L10n.Tr("Keep the original path relative to the project's Assets folder"),

                UploadFilePathMode.Compact => L10n.Tr("Reduce files nesting by removing common path parts"),

                UploadFilePathMode.Flatten => L10n.Tr("Flatten all files to the root of the asset and rename them in case of collision"),

                _ => null
            };
        }
    }
}
