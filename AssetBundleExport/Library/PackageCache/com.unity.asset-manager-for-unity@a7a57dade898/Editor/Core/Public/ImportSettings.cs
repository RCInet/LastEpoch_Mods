using System;

namespace Unity.AssetManager.Editor
{
    /// <summary>
    /// Defines how to override the import preferences.
    /// </summary>
    public enum ConflictResolutionOverride
    {
        /// <summary>
        /// The default behavior is to use the settings defined in the Editor Preferences.
        /// </summary>
        None = 0,
        /// <summary>
        /// Imports dependency versions as defined by the importing asset.
        /// Displays the import modal with conflict resolution options.
        /// </summary>
        AllowAssetVersionRollbackAndShowConflictResolver = 1,
        /// <summary>
        /// When gathering dependencies, maintains the highest imported version.
        /// Displays the import modal with conflict resolution options.
        /// </summary>
        PreventAssetVersionRollbackAndShowConflictResolver = 2,
        /// <summary>
        /// When gathering dependencies, maintains the highest imported version.
        /// Skips the import modal and replaces conflicting assets with imported ones.
        /// </summary>
        PreventAssetVersionRollbackAndReplaceAll = 3,
    }

    /// <summary>
    /// Additional settings for importing assets.
    /// </summary>
    [Serializable]
    public struct ImportSettings
    {
        /// <summary>
        /// Determines how to override import preferences.
        /// </summary>
        public ConflictResolutionOverride ConflictResolutionOverride { get; set; }

        /// <summary>
        /// A path relative to the project's ./Assets folder into which the assets will be imported.
        /// </summary>
        public string DestinationPathOverride { get; set; }

        internal Core.Editor.ImportOperation.ImportType Type { get; set; }
    }
}
