namespace Unity.AssetManager.UI.Editor
{
    static class Constants
    {
        public const string AllAssetsFolderName = "All Assets";
        public const string Continue = "Continue";
        public const string Cancel = "Cancel";
        public const string Processing = "Processing...";
        public const string ComparingAssetsWithCloud = "Comparing assets with Cloud...";

        public const string CategoriesScrollViewUssName = "categories-scrollView";

        public const int DefaultPageSize = 50;

        // Sidebar
        public const string SidebarProjectsText = "Projects";
        public const string SidebarSavedViewsText = "Saved Views";
        public const string SidebarRenameSavedView = "Rename saved view";
        public const string SidebarDeleteSavedView = "Delete saved view";

        // Filter
        public const string NoSelectionsText = "Empty";
        public const string UpToDate = "Up to date";
        public const string Outdated = "Outdated";
        public const string Deleted = "Deleted (on cloud)";
        public const string PrimaryMetadata = "PRIMARY METADATA";
        public const string CustomMetadata = "CUSTOM METADATA";
        public const string FromText = "From";
        public const string ToText = "To";
        public const string Clear = "Clear";
        public const string Apply = "Apply";
        public const string EnterText = "Enter text :";
        public const string EnterNumberText = "Enter number :";
        public const string EnterUrlText = "Enter hyperlink label :";

        // Saved View Controls
        public const string ClearFilter = "Clear";
        public const string ClearFilterTooltip = "Clear the current saved view and all applied search filters";
        public const string SaveFilter = "Save";
        public const string SaveCurrentFilterTooltip = "Save the current view and all currently applied search filters";
        public const string SaveFilterDropdownTooltip = "More save options";
        public const string SaveFilterAsNew = "Save as new view";
        public const string SaveCurrentFilter = "Save current view";


        // Sort
        public const string Sort = "Sort by:";

        // Tabs
        public const string AssetsTabLabel = "Assets";
        public const string InProjectTabLabel = "In Project";
        public const string UploadTabLabel = "Upload";

        // In Project
        public const string InProjectTitle = "Locally Imported Assets";

        // Update all button
        public const string UpdateAllButtonTooltip = "Update all assets to Latest.";

        // Upload
        public const string IgnoreAll = "Ignore All";
        public const string IncludeAll = "Include All";
        public const string RemoveAll = "Remove All";
        public const string IgnoreAsset = "Ignore Asset";
        public const string IgnoreSelectedAssets = "Ignore Selected Assets";
        public const string RemoveAsset = "Remove Asset";
        public const string IncludeAsset = "Include Asset";
        public const string IncludeAllScripts = "Include All Scripts";
        public const string IgnoreAssetToolTip = "This asset is ignored and will not\nbe uploaded to the Asset Manager.";
        public const string IgnoreToggleTooltip = "Uncheck to ignore asset";
        public const string IncludeToggleTooltip = "Check to include asset";
        public const string IgnoreDependenciesDialogTitle = "Warning";
        public const string IgnoreDependenciesDialogMessage = "You are trying to upload assets without their dependencies. This might break other assets that depend on them.\nAre you sure you want to proceed?";
        public const string UploadNoAssetsMessage = "Drag and drop assets from the Project window\n\nor\n\nIn the Project window, right-click on a file or folder and select Upload to Asset Manager";
        public const string CancelUploadActionText = "Cancel Upload";
        public const string ClearAllActionText = "Clear All";
        public const string UploadActionText = "Upload Assets";
        public const string UploadingText = "Uploading...";
        public const string UploadNoEntitlementMessage = "You can't upload this asset without an assigned seat. \nContact your Organization Owner to assign you a seat.";
        public const string UploadNoPermissionTooltip = "You don’t have permissions to upload this asset. \nSee your role from the project settings page on \nthe Asset Manager dashboard.";
        public const string UploadCloudServicesNotReachableTooltip = "Cloud services are not reachable";
        public const string UploadAllIgnoredTooltip = "All assets are ignored";
        public const string UploadNoProjectSelectedTooltip = "Select a project to upload assets";
        public const string UploadNoAssetsTooltip = "No assets to upload";
        public const string UploadWaitStatusTooltip = "Waiting for assets status...";
        public const string UploadAssetsTooltip = "Upload assets to cloud";
        public const string UploadAssetsExistsTooltip = "All assets already exist in the cloud";
        public const string UploadAssetsNotModifiedTooltip = "All assets are unchanged";
        public const string UploadOutsideProjectTooltip = "One of more assets contain files outside the Assets folder";
        public const string DirtyAssetsDialogTitle = "Warning";
        public const string DirtyAssetsDialogMessage = "You are trying to upload assets that have unsaved changes.\nAre you sure you want to proceed?";
        public const string DirtyAssetsDialogOk = "Save and Continue";
        public const string DirtyAssetsDialogCancel = Cancel;
        public const string UploadSettings = "Upload Settings";
        public const string UploadMode = "Reupload mode";
        public const string Dependencies = "Dependencies";
        public const string FilePaths = "File paths";
        public const string UploadSettingsReset = "Reset to default";
        public const string ScalingIssuesMessage = "Uploading {0}+ assets may reach scaling issues. Try to upload less than {0} assets at the time for better results.";
        public const int ScalingIssuesThreshold = 100;
        public const string UnexpectedFieldDefinitionType = "Unexpected field definition type was encountered.";
        public const string UrlLabel = "URL";
        public const string InvalidUrlFormat = "URL format is invalid";
        public const string HyperlinkLabel = "Hyperlink Label";
        public const string DateLabel = "Date (YYYY/MM/DD)";
        public const string TimeLabel = "Time (hh:mm)";
        public const string InvalidYearLabel = "Year must be between 1 and 9999";
        public const string UnexpectedTimestampFormat = "Unexpected timestamp format encountered.";
        public const string NoProjectSelected = "No project selected";
        public const string UploadMetadata = "Upload Metadata";
        public const string AddCustomField = "Add custom field";
        public const string MetadataPartialEditing = "Metadata that are only on some of the selected assets cannot be multi-edited.";
        public const string NoMatchingFields = "No matching metadata field. To add custom fields, go to the <a>dashboard</a> and create a new metadata field.";
        public const string NoMatchingFields_WithoutLink = "No matching metadata field.";

        // Preview Status
        public const string ImportedText = "Asset is imported from Asset Manager";
        public const string UpToDateText = "Asset is up to date relative to Asset Manager";
        public const string OutOfDateText = "Asset is outdated relative to Asset Manager";
        public const string StatusErrorText = "Asset was deleted from Asset Manager or is not accessible";

        // Upload Status
        public const string LinkedText = "This asset is a dependency of another asset";
        public const string UploadAddText = "This asset does not exist on the cloud and will be added";
        public const string UploadSkipText = "This asset already exists on the cloud and will not be uploaded";
        public const string UploadNewVersionText = "This asset will add a new version in its cloud version";
        public const string UploadDuplicateText = "This asset already exists on the cloud but a new cloud asset will be uploaded";
        public const string UploadOutsideText = "This asset is outside the Assets folder and cannot be uploaded";
        public const string UploadSourceControlledText = "This asset is source controlled and cannot be re-uploaded";

        // AssetDetailsView Asset info
        public const string AssetIdText = "Asset Id";
        public const string VersionText = "Ver. ";
        public const string PendingVersionText = "Pending";
        public const string NewVersionText = "New Ver.";
        public const string FromVersionText = "From Ver.";
        public const string DashboardLinkTooltip = "Open asset in the dashboard";
        public const string TotalFilesText = "Total Files";
        public const string FilesSizeText = "Files Size";
        public const string LoadingText = "Loading...";
        public const string ChangeLogText = "What changed?";
        public const string NoChangeLogText = "No change log provided.";
        public const string CreatedFromText = "Created From";
        public const string CreatedByText = "Created by";
        public const string UploadDateText = "Upload date";
        public const string LastEditByText = "Last edit by";
        public const string LastModifiedText = "Last modified";
        public const string DateText = "Date";
        public const string TagsText = "Tags";
        public const string ProjectText = "Project";
        public const string ProjectsText = "Projects";
        public const string AssetTypeText = "Asset Type";
        public const string StatusText = "Status";
        public const string SourceFilesText = "Source Files";
        public const string NoFilesText = "No files were found in this asset.";
        public const string SameFileNamesText = "Files of the same name were found in this asset. Unity does not support files with the same name.";
        public const string DependenciesText = "Dependencies";
        public const string NoDependenciesText = "This asset has no dependencies";
        public const string ServiceAccountText = "Service Account";
        public const string LatestTagText = "Latest";
        public const string ImportedTagText = "Imported";
        public const string VCSChipTooltip = "Those files are version controlled";

        // AssetDetailsView Asset status
        public const string AssetDraftStatus = "Draft";
        public const string AssetInReviewStatus = "InReview";
        public const string AssetApprovedStatus = "Approved";
        public const string AssetRejectedStatus = "Rejected";
        public const string AssetPublishedStatus = "Published";
        public const string AssetWithdrawnStatus = "Withdrawn";

        // AssetDetailsView actions text
        public const string ImportActionText = "Import";
        public const string ImportToActionText = "Import To";
        public const string ImportButtonTooltip = "Imports all associated files of the Cloud Asset into your Unity project";
        public const string ImportButtonDropdownTooltip = "Additional import options";
        public const string UpdateToLatestActionText = "Update To Latest";
        public const string ReimportActionText = "Reimport";
        public const string ReimportButtonTooltip = "Reimports all associated files of the Cloud Asset into your Unity project";
        public const string ImportLocationTitle = "Choose import location";
        public const string RemoveFromProjectActionText = "Remove From Project";
        public const string RemoveAllFromProjectActionText = "Remove All From Local Project";
        public const string RemoveAssetOnlyText = "Remove Asset (Ignore dependencies)";
        public const string RemoveAssetsOnlyText = "Remove Assets (Ignore dependencies)";
        public const string StopTrackingAssetOnlyActionText = "Stop Tracking Asset (Ignore dependencies)";
        public const string StopTrackingAssetsOnlyActionText = "Stop Tracking Assets (Ignore dependencies)";
        public const string StopTrackingAssetActionText = "Stop Tracking Asset And Exclusive Dependencies";
        public const string StopTrackingAssetsActionText = "Stop Tracking Assets And Exclusive Dependencies";
        public const string UntrackAssetActionText = "Stop Tracking Asset";
        public const string UntrackAssetsActionText = "Stop Tracking Assets";
        public const string ClearImportActionText = "Clear All Finished Imports";
        public const string ShowInProjectActionText = "Show In Project";
        public const string ShowInProjectButtonToolTip = "Pings the Asset in an active Project window";
        public const string ShowInProjectButtonDisabledToolTip = "This Asset has not yet been imported";
        public const string ShowInDashboardActionText = "Show In Dashboard";
        public const string AssetsSelectedTitle = "Assets Selected";
        public const string ImportingText = "Importing";
        public const string ImportAllSelectedActionText = "Import All Selected";
        public const string RemoveFromProjectAllSelectedActionText = "Remove All Selected From Project";
        public const string RemoveAllFromProjectToolTip = "Remove all selected assets and their exclusive dependencies from your Unity project";
        public const string RemoveFromProjectButtonToolTip = "Removes the asset and its exlcusive dependencies from your Unity project";
        public const string RemoveFromProjectButtonDisabledToolTip = "There is nothing to remove from the project.";
        public const string ImportButtonDisabledToolTip = "There is nothing to import.";
        public const string ImportNoPermissionMessage = "You don’t have permissions to import this asset. \nSee your role from the project settings page on \nthe Asset Manager dashboard.";
        public const string FilteredAssetOutdatedWarning = "Asset is not up to date. Update your asset to see the latest asset details.";

        // Grid View
        public const string EmptyCollectionsText = "This collection has no assets, use the Asset Manager dashboard to link your assets to a collection.";
        public const string EmptyInProjectText = "Your imported assets will be shown here.";
        public const string EmptyProjectText = "The selected project is empty. To add assets, right click on any asset in project window and select upload to asset manager.";
        public const string EmptyAllAssetsText = "The selected organization is empty. To add assets, right click on any asset in project window and select upload to asset manager.";
        public const string NoResultsText = "No results found";
        public const string NoResultsForSearchText = " for search: ";
        public const string NoResultsForFiltersText = " with filters: ";

        public const string UpdateAllText = "Update All";
        public const string UpdateAllToLatestActionText = "Update All To Latest";
        public const string UpdateSelectedToLatestActionText = "Update Selected To Latest";
        public const string UpdateProjectToLatestActionText = "Update All Imported Assets From This Project";
        public const string UpdateCollectionToLatestActionText = "Update All Imported Assets From This Collection";

        // Permissions
        public const string ImportPermission = "amc.assets.download";

        // Reimport Window
        public const string ReimportWindowConflictsTitle = "Some files have conflicts. To complete import, choose how to resolve them.";
        public const string ReimportWindowConflictsWarning = "Warning: replacing files will overwrite local copies.";
        public const string ReimportWindowDependentsTitle = "The following dependent assets have updates:";
        public const string ReimportWindowUpwardDependenciesTitle = "The following assets have dependencies on updated assets. They might also be affected:";
        public const string ReimportWindowCancel = Cancel;
        public const string ReimportWindowImport = "Import";
        public const string ReimportWindowSkip = "Skip";
        public const string ReimportWindowUpdate = "Update";
        public const string ReimportWindowReimport = "Reimport";
        public const string ReimportWindowReplace = "Replace";
        public const string Conflicts = "conflicts";

        // Asset Manager Settings
        public const string AssetManagerTitle = "Asset Manager";

        public const string ImportSettingsTitle = "Import Settings";
        public const string ImportDefaultLocation = "Default import location";
        public const string ImportDefaultLocationError = "Please select an import location path relative to the project's Assets folder.";
        public const string ImportCreateSubfolders = "Create subfolder on import";
        public const string ImportCreateSubfoldersTooltip = "Enabling this option will automatically generate a folder named after the cloud asset and import files within it";
        public const string ImportKeepHigherVersion = "Avoid rolling back versions of dependencies";
        public const string ImportKeepHigherVersionTooltip = "Enabling this option will keep by default the higher version of a dependency when importing";

        public const string CacheSettingsTitle = "Cache Settings";
        public const string CacheLocationTitle = "Cache location";
        public const string AccessError = "Some folders or files could not be accessed.";
        public const string DirectoryDoesNotExistError = "This directory does not exist";
        public const string RevealInFinder = "Reveal in Finder";
        public const string ShowInExplorerLabel = "Show in Explorer";
        public const string ChangeLocationLabel = "Change Location";
        public const string ResetDefaultLocation = "Reset to Default Location";
        public const string CacheLocation = "Cache Location";
        public const string CacheMaxSize = "Maximum cache size (GB)";
        public const string CacheSize = "Cache size is ";
        public const string CacheRefresh = "Refresh";
        public const string CleanCache = "Clean cache";
        public const string ClearExtraCache = "Clear extra cache";

        public const string UploadSettingsTitle = "Upload Settings";
        public const string TagsCreation = "Generate tags automatically based on preview image";
        public const string TagsCreationConfidenceLevel = "Confidence level for automatic tags generation";
        public const string TagsCreationConfidenceLevelTooltip = "The higher the value, the more accurate the tags will be.";
        public const string UploadDependenciesUsingLatestLabel = "Pin dependencies to 'Latest' version label during upload";
        public const string UploadDependenciesUsingLatestTooltip = "Enable this option to map asset dependencies to their latest versions during upload, ensuring the latest versions are used during import. ";
        public const string UploadDependenciesUsingLatestHelpText = "This setting only applies when dependencies are set to \"Separate\". It doesn't affect uploads with dependencies set to \"Embedded\" or \"Ignore\".";

        public const string DisableReimportModalLabel = "Disable the reimport modal";
        public const string DisableReimportModalToolTip = "When enabled, no conflict resolution modals will be shown when importing and \"replace\" will be applied to everything. \"Avoid rolling back versions of dependencies\" must be enabled to use this setting.";

        public const string ProjectWindowSettingsTitle = "Project Window Settings";
        public const string ProjectWindowIconOverlayToggleLabel = "Enable icon overlay in the project window";
        public const string ProjectWindowIconOverlayToggleTooltip = "Enable or disable the icon overlay in the project window. The icon overlay shows the status of Asset Manager assets relative to the cloud.";
        public const string ProjectWindowIconOverlayPositionLabel = "Icon overlay position";
        public const string ProjectWindowIconOverlayPositionTooltip = "Choose the position of the icon overlay on the asset icon";
        public const string ProjectWindowIconOverlayDisplayTypeLabel = "Display detailed icon overlay";
        public const string ProjectWindowIconOverlayDisplayTypeTooltip = "Enabling the detailed overlay will show the status of the Asset Manager asset relative to the cloud. Else only the import status is shown.";

        public const string DebugSettingsTitle = "Debug Settings";
        public const string DebugLogsLabel = "Enable debug logs";
        public const string DebugLogsTooltip = "Enable detailed logs for debugging purposes.";

        // Collection
        public const string CollectionCreate = "Create new collection";
        public const string CollectionDefaultName = "New collection";
        public const string CollectionRename = "Rename";
        public const string CollectionDelete = "Delete";
        public const string CollectionDeleteMessage = "Deleting a collection will not delete assets linked to it, they will still be visible from the project view.";
        public const string CollectionDeleteTitle = "Warning";
        public const string CollectionDeleteOk = "Delete";
        public const string CollectionDeleteCancel = Cancel;
    }
}
