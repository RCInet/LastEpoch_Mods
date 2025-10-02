using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class AssetManagerUserSettingsProvider : SettingsProvider
    {
        const int k_MinCacheSizeValueGB = 2;
        const int k_MaxCacheSizeValueGB = 200;

        const string k_SettingsProviderPath = "Preferences/Asset Manager";
        const string k_MainDarkUssName = "MainDark";
        const string k_MainLightUssName = "MainLight";

        const string k_AssetManagerTitle = "titleLabel";

        const string k_ImportSettingsFoldout = "importSettingsFoldout";
        const string k_ImportLocationPath = "importLocationPath";
        const string k_ImportLocationErrorBox = "importLocationErrorBox";
        const string k_ImportLocationDropdown = "importLocationDropdown";
        const string k_ImportDefaultLocationLabel = "importSettingsDefaultLocationLabel";
        const string k_ImportCreateSubFolderLabel = "importSettingsCreateSubfolderLabel";
        const string k_ImportKeepHigherVersionLabel = "importSettingsKeepHigherVersionLabel";
        const string k_SubfolderCreationToggle = "subfolderCreationToggle";
        const string k_KeeHigherVersionToggle = "keepHigherVersionToggle";

        const string k_CacheSettingsFoldout = "cacheSettingsFoldout";
        const string k_CacheLocationDropdown = "cacheLocationDropdown";
        const string k_AssetManagerCachePath = "assetManagerCachePath";
        const string k_DisabledErrorBox = "disabledErrorBox";
        const string k_CleanCache = "cleanCache";
        const string k_CacheSizeOnDisk = "cacheSizeOnDisk";
        const string k_MaxCacheSize = "maxCacheSize";
        const string k_RefreshButton = "refresh";
        const string k_ClearExtraCache = "clearExtraCache";
        const string k_LocationLabel = "cacheManagementLocationLabel";
        const string k_MaxSizeLabel = "cacheManagementMaxSizeLabel";
        const string k_SizeLabel = "cacheManagementSizeLabel";

        const string k_UploadSettingsFoldout = "uploadSettingsFoldout";
        const string k_TagsCreationUploadLabel = "tagsCreationUploadLabel";
        const string k_TagsCreationUploadToggle = "tagsCreationUploadToggle";
        const string k_TagsCreationUploadConfidenceLabel = "tagsCreationUploadConfidenceLabel";
        const string k_TagsCreationUploadConfidenceValue = "tagsCreationUploadConfidenceValue";
        const string k_UploadDependenciesWithLatestLabelLabel = "uploadDependenciesWithLatestLabel";
        const string k_UploadDependenciesWithLatestToggle = "uploadDependenciesWithLatestToggle";
        const string k_UploadDependenciesHelpBox = "uploadDependenciesHelpBox";
        const string k_DisableReimportModalLabel = "disableReimportModalLabel";
        const string k_DisableReimportModalToggle = "disableReimportModalToggle";

        const string k_ProjectWindowSettingsFoldout = "projectWindowSettingsFoldout";
        const string k_ProjectWindowShowIconOverlayLabel = "projectWindowShowIconOverlayLabel";
        const string k_ProjectWindowShowIconOverlayToggle = "projectWindowShowIconOverlayToggle";
        const string k_ProjectWindowIconOverlayPositionLabel = "projectWindowIconOverlayPositionLabel";
        const string k_ProjectWindowIconOverlayPositionDropdown = "projectWindowIconOverlayPositionDropdown";
        const string k_ProjectWindowIconOverlayDisplayTypeLabel = "projectWindowIconOverlayDisplayTypeLabel";
        const string k_ProjectWindowIconOverlayDisplayTypeToggle = "projectWindowIconOverlayDisplayTypeToggle";

        const string k_DebugSettingsFoldout = "debugSettingsFoldout";
        const string k_DebugLogsLabel = "debugLogsLabel";
        const string k_DebugLogsToggle = "debugLogsToggle";

        static readonly string[] k_ProjectWindowIconOverlayPositionOptions =
        {
            L10n.Tr("Top Right"),
            L10n.Tr("Top Left"),
            L10n.Tr("Bottom Left"),
            L10n.Tr("Bottom Right"),
        };

        static Dictionary<CacheValidationResultError, string> cacheValidationErrorMessages =
            new()
            {
                { CacheValidationResultError.InvalidPath, "The specified path contains invalid characters" },
                { CacheValidationResultError.DirectoryNotFound, "The specified path is invalid" },
                {
                    CacheValidationResultError.PathTooLong,
                    "The specified path exceeds the system-defined maximum length"
                },
                { CacheValidationResultError.CannotWriteToDirectory, "Could not write to directory" }
            };

        readonly ICachePathHelper m_CachePathHelper;
        readonly ICacheEvictionManager m_CacheEvictionManager;
        readonly IIOProxy m_IOProxy;
        readonly ISettingsManager m_SettingsManager;
        readonly IApplicationProxy m_ApplicationProxy;
        readonly IEditorUtilityProxy m_EditorUtilityProxy;
        Label m_ImportLocationPathLabel;
        Label m_AssetManagerCachePathLabel;
        Label m_CacheSizeOnDisk;
        Button m_ClearExtraCacheButton;

        HelpBox m_CacheErrorLabel;
        HelpBox m_ImportLocationErrorLabel;

        AssetManagerUserSettingsProvider(ICachePathHelper cachePathHelper, ICacheEvictionManager cacheEvictionManager, ISettingsManager settingsManager,
            IIOProxy ioProxy, IApplicationProxy applicationProxy, IEditorUtilityProxy editorUtilityProxy,
            string path, IEnumerable<string> keywords = null)
            : base(path, SettingsScope.User, keywords)
        {
            m_CachePathHelper = cachePathHelper;
            m_CacheEvictionManager = cacheEvictionManager;
            m_SettingsManager = settingsManager;
            m_IOProxy = ioProxy;
            m_ApplicationProxy = applicationProxy;
            m_EditorUtilityProxy = editorUtilityProxy;
        }

        /// <summary>
        /// Initializes all the UI elements
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="rootElement">the root visual element</param>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            UIElementsUtils.LoadUXML("AssetManagerUserSettings").CloneTree(rootElement);
            UIElementsUtils.LoadCommonStyleSheet(rootElement);
            UIElementsUtils.LoadCustomStyleSheet(rootElement,
                EditorGUIUtility.isProSkin ? k_MainDarkUssName : k_MainLightUssName);

            var assetManagerTitle = rootElement.Q<Label>(k_AssetManagerTitle);
            assetManagerTitle.text = L10n.Tr(Constants.AssetManagerTitle);

            // setup the import settings foldout
            var importSettingsFoldout = rootElement.Q<Foldout>(k_ImportSettingsFoldout);
            importSettingsFoldout.text = L10n.Tr(Constants.ImportSettingsTitle);

            // setup the default import location
            var importDefaultLocationLabel = rootElement.Q<Label>(k_ImportDefaultLocationLabel);
            importDefaultLocationLabel.text = L10n.Tr(Constants.ImportDefaultLocation);
            m_ImportLocationPathLabel = rootElement.Q<Label>(k_ImportLocationPath);
            SetPathLabelTextAndTooltip(m_ImportLocationPathLabel, m_SettingsManager.DefaultImportLocation, true);

            m_ImportLocationErrorLabel = rootElement.Q<HelpBox>(k_ImportLocationErrorBox);
            UIElementsUtils.Hide(m_ImportLocationErrorLabel);

            // setup the creation subfolder label
            var importCreateSubFolderLabel = rootElement.Q<Label>(k_ImportCreateSubFolderLabel);
            importCreateSubFolderLabel.text = L10n.Tr(Constants.ImportCreateSubfolders);
            importCreateSubFolderLabel.tooltip = L10n.Tr(Constants.ImportCreateSubfoldersTooltip);

            var cacheSettingsFoldout = rootElement.Q<Foldout>(k_CacheSettingsFoldout);
            cacheSettingsFoldout.text = L10n.Tr(Constants.CacheSettingsTitle);

            // setup the cache location
            var locationLabel = rootElement.Q<Label>(k_LocationLabel);
            locationLabel.text = L10n.Tr(Constants.CacheLocation);
            var maxSizeLabel = rootElement.Q<Label>(k_MaxSizeLabel);
            maxSizeLabel.text = L10n.Tr(Constants.CacheMaxSize);
            var sizeLabel = rootElement.Q<Label>(k_SizeLabel);
            sizeLabel.text = L10n.Tr(Constants.CacheSize);
            m_AssetManagerCachePathLabel = rootElement.Q<Label>(k_AssetManagerCachePath);
            SetPathLabelTextAndTooltip(m_AssetManagerCachePathLabel, m_SettingsManager.BaseCacheLocation, false);

            // setup extra cache clean up
            m_ClearExtraCacheButton = rootElement.Q<Button>(k_ClearExtraCache);
            m_ClearExtraCacheButton.text = L10n.Tr(Constants.ClearExtraCache);
            m_ClearExtraCacheButton.clicked += ClearExtraCache;
            SetClearExtraCacheButton();

            // setup the refresh button
            var refreshButton = rootElement.Q<Button>(k_RefreshButton);
            refreshButton.text = L10n.Tr(Constants.CacheRefresh);
            refreshButton.clicked += RefreshCacheSizeOnDiskLabel;
            m_CacheErrorLabel = rootElement.Q<HelpBox>(k_DisabledErrorBox);
            UIElementsUtils.Hide(m_CacheErrorLabel);
            var cleanCacheButton = rootElement.Q<Button>(k_CleanCache);
            cleanCacheButton.text = L10n.Tr(Constants.CleanCache);
            cleanCacheButton.clicked += CleanCache;
            m_CacheSizeOnDisk = rootElement.Q<Label>(k_CacheSizeOnDisk);
            try
            {
                m_CacheSizeOnDisk.text =
                    $"{Utilities.BytesToReadableString(m_IOProxy.GetDirectorySizeBytes(m_SettingsManager.BaseCacheLocation))}";
            }
            catch (UnauthorizedAccessException)
            {
                ResetCacheLocationToDefault();
                RefreshCacheSizeOnDiskLabel();
            }

            // setup subfolder creation toggle
            var createSubFolderToggle = rootElement.Q<Toggle>(k_SubfolderCreationToggle);
            createSubFolderToggle.value = m_SettingsManager.IsSubfolderCreationEnabled;
            createSubFolderToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                m_SettingsManager.SetIsSubfolderCreationEnabled(evt.newValue);
            });

            // setup the disable reimport modal label and toggle
            var disableReuploadModalLabel = rootElement.Q<Label>(k_DisableReimportModalLabel);
            disableReuploadModalLabel.text = L10n.Tr(Constants.DisableReimportModalLabel);
            disableReuploadModalLabel.tooltip = L10n.Tr(Constants.DisableReimportModalToolTip);
            disableReuploadModalLabel.SetEnabled(m_SettingsManager.IsKeepHigherVersionEnabled);

            var disableReuploadModalToggle = rootElement.Q<Toggle>(k_DisableReimportModalToggle);
            disableReuploadModalToggle.value = m_SettingsManager.IsReimportModalDisabled;
            disableReuploadModalToggle.tooltip = L10n.Tr(Constants.DisableReimportModalToolTip);
            disableReuploadModalToggle.SetEnabled(m_SettingsManager.IsKeepHigherVersionEnabled);
            disableReuploadModalToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                m_SettingsManager.SetDisableReimportModal(evt.newValue);
            });

            // setup the keep higher version label
            var importKeepHigherVersionLabel = rootElement.Q<Label>(k_ImportKeepHigherVersionLabel);
            importKeepHigherVersionLabel.text = L10n.Tr(Constants.ImportKeepHigherVersion);
            importKeepHigherVersionLabel.tooltip = L10n.Tr(Constants.ImportKeepHigherVersionTooltip);

            // setup keep higher version toggle
            var keepHigherVersionToggle = rootElement.Q<Toggle>(k_KeeHigherVersionToggle);
            keepHigherVersionToggle.value = m_SettingsManager.IsKeepHigherVersionEnabled;
            keepHigherVersionToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                m_SettingsManager.SetIsKeepHigherVersionEnabled(evt.newValue);

                if (evt.newValue)
                {
                    disableReuploadModalLabel.SetEnabled(true);
                    disableReuploadModalToggle.SetEnabled(true);
                }
                else
                {
                    disableReuploadModalLabel.SetEnabled(false);
                    disableReuploadModalToggle.SetEnabled(false);
                    disableReuploadModalToggle.SetValueWithoutNotify(false);
                    m_SettingsManager.SetDisableReimportModal(false);
                }
            });

            var uploadSettingsFoldout = rootElement.Q<Foldout>(k_UploadSettingsFoldout);
            uploadSettingsFoldout.text = L10n.Tr(Constants.UploadSettingsTitle);

            // Setup tags creation confidence threshold
            var tagsCreationConfidenceLabel = rootElement.Q<Label>(k_TagsCreationUploadConfidenceLabel);
            tagsCreationConfidenceLabel.text = L10n.Tr(Constants.TagsCreationConfidenceLevel);
            tagsCreationConfidenceLabel.tooltip = L10n.Tr(Constants.TagsCreationConfidenceLevelTooltip);
            tagsCreationConfidenceLabel.SetEnabled(m_SettingsManager.IsTagsCreationUploadEnabled);
            var tagsCreationConfidenceValue = rootElement.Q<SliderInt>(k_TagsCreationUploadConfidenceValue);
            tagsCreationConfidenceValue.SetEnabled(m_SettingsManager.IsTagsCreationUploadEnabled);
            tagsCreationConfidenceValue.value = m_SettingsManager.TagsConfidenceThresholdPercent;
            tagsCreationConfidenceValue.RegisterValueChangedCallback(evt =>
            {
                m_SettingsManager.SetTagsCreationConfidenceThresholdPercent(evt.newValue);
            });

            // Setup tags creation on upload toggle
            var tagsCreationUploadLabel = rootElement.Q<Label>(k_TagsCreationUploadLabel);
            tagsCreationUploadLabel.text = L10n.Tr(Constants.TagsCreation);
            var tagsCreationUpload = rootElement.Q<Toggle>(k_TagsCreationUploadToggle);
            tagsCreationUpload.value = m_SettingsManager.IsTagsCreationUploadEnabled;
            tagsCreationUpload.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                m_SettingsManager.SetIsTagsCreationUploadEnabled(evt.newValue);

                tagsCreationConfidenceValue.SetEnabled(evt.newValue);
                tagsCreationConfidenceLabel.SetEnabled(evt.newValue);
            });

            // Setup upload dependencies with latest toggle
            var uploadDependenciesWithLatestLabelLabel = rootElement.Q<Label>(k_UploadDependenciesWithLatestLabelLabel);
            uploadDependenciesWithLatestLabelLabel.text = L10n.Tr(Constants.UploadDependenciesUsingLatestLabel);
            var uploadDependenciesWithLatestToggle = rootElement.Q<Toggle>(k_UploadDependenciesWithLatestToggle);
            uploadDependenciesWithLatestToggle.tooltip= L10n.Tr(Constants.UploadDependenciesUsingLatestTooltip);
            uploadDependenciesWithLatestToggle.value = m_SettingsManager.IsUploadDependenciesUsingLatestLabel;
            uploadDependenciesWithLatestToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                m_SettingsManager.SetUploadDependenciesUsingLatestLabel(evt.newValue);
            });

            var uploadDependenciesHelpBox = rootElement.Q<HelpBox>(k_UploadDependenciesHelpBox);
            uploadDependenciesHelpBox.text = L10n.Tr(Constants.UploadDependenciesUsingLatestHelpText);

            SetupCacheLocationToolbarButton(rootElement);
            SetupImportLocationToolbarButton(rootElement);
            SetupCacheSize(rootElement);

            var projectWindowSettingsFoldout = rootElement.Q<Foldout>(k_ProjectWindowSettingsFoldout);
            projectWindowSettingsFoldout.text = L10n.Tr(Constants.ProjectWindowSettingsTitle);

            // Setup project window icon overlay position dropdown
            var projectWindowIconOverlayPositionLabel = rootElement.Q<Label>(k_ProjectWindowIconOverlayPositionLabel);
            projectWindowIconOverlayPositionLabel.text = L10n.Tr(Constants.ProjectWindowIconOverlayPositionLabel);
            projectWindowIconOverlayPositionLabel.SetEnabled(m_SettingsManager.IsProjectWindowIconOverlayEnabled);

            var projectWindowIconOverlayPositionDropdown = rootElement.Q<DropdownField>(k_ProjectWindowIconOverlayPositionDropdown);

            // Remove the default label from the dropdown
            var defaultDropdownLabel = projectWindowIconOverlayPositionDropdown.Q<Label>();
            defaultDropdownLabel.style.display = DisplayStyle.None;

            projectWindowIconOverlayPositionDropdown.tooltip = L10n.Tr(Constants.ProjectWindowIconOverlayPositionTooltip);
            projectWindowIconOverlayPositionDropdown.choices = k_ProjectWindowIconOverlayPositionOptions.ToList();
            projectWindowIconOverlayPositionDropdown.value = k_ProjectWindowIconOverlayPositionOptions[m_SettingsManager.ProjectWindowIconOverlayPosition];
            projectWindowIconOverlayPositionDropdown.RegisterValueChangedCallback(evt =>
            {
                m_SettingsManager.SetProjectWindowIconOverlayPosition(Array.IndexOf(k_ProjectWindowIconOverlayPositionOptions, evt.newValue));
            });

            projectWindowIconOverlayPositionDropdown.SetEnabled(m_SettingsManager.IsProjectWindowIconOverlayEnabled);

            // Setup project window icon overlay display type toggle
            var projectWindowIconOverlayDisplayTypeLabel = rootElement.Q<Label>(k_ProjectWindowIconOverlayDisplayTypeLabel);
            projectWindowIconOverlayDisplayTypeLabel.text = L10n.Tr(Constants.ProjectWindowIconOverlayDisplayTypeLabel);
            projectWindowIconOverlayDisplayTypeLabel.SetEnabled(m_SettingsManager.IsProjectWindowIconOverlayEnabled);

            var projectWindowIconOverlayDisplayTypeToggle = rootElement.Q<Toggle>(k_ProjectWindowIconOverlayDisplayTypeToggle);
            projectWindowIconOverlayDisplayTypeToggle.tooltip = L10n.Tr(Constants.ProjectWindowIconOverlayDisplayTypeTooltip);
            projectWindowIconOverlayDisplayTypeToggle.value = m_SettingsManager.DisplayDetailedProjectWindowIconOverlay;
            projectWindowIconOverlayDisplayTypeToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                m_SettingsManager.SetProjectWindowIconOverlayDisplayType(evt.newValue);
            });
            projectWindowIconOverlayDisplayTypeToggle.SetEnabled(m_SettingsManager.IsProjectWindowIconOverlayEnabled);

            // Setup project window icon overlay toggle
            var projectWindowIconOverlayLabel = rootElement.Q<Label>(k_ProjectWindowShowIconOverlayLabel);
            projectWindowIconOverlayLabel.text = L10n.Tr(Constants.ProjectWindowIconOverlayToggleLabel);
            var projectWindowShowIconOverlayToggle = rootElement.Q<Toggle>(k_ProjectWindowShowIconOverlayToggle);
            projectWindowShowIconOverlayToggle.tooltip = L10n.Tr(Constants.ProjectWindowIconOverlayToggleTooltip);
            projectWindowShowIconOverlayToggle.value = m_SettingsManager.IsProjectWindowIconOverlayEnabled;
            projectWindowShowIconOverlayToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                m_SettingsManager.SetProjectWindowIconOverlayEnabled(evt.newValue);

                projectWindowIconOverlayPositionLabel.SetEnabled(evt.newValue);
                projectWindowIconOverlayPositionDropdown.SetEnabled(evt.newValue);
                projectWindowIconOverlayDisplayTypeLabel.SetEnabled(evt.newValue);
                projectWindowIconOverlayDisplayTypeToggle.SetEnabled(evt.newValue);
            });

            var debugSettingsFoldout = rootElement.Q<Foldout>(k_DebugSettingsFoldout);
            debugSettingsFoldout.text = L10n.Tr(Constants.DebugSettingsTitle);

            var debugLogsLabel = rootElement.Q<Label>(k_DebugLogsLabel);
            debugLogsLabel.text = L10n.Tr(Constants.DebugLogsLabel);
            var debugLogsToggle = rootElement.Q<Toggle>(k_DebugLogsToggle);
            debugLogsToggle.tooltip = L10n.Tr(Constants.DebugLogsTooltip);
            debugLogsToggle.value = m_SettingsManager.IsDebugLogsEnabled;
            debugLogsToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                m_SettingsManager.SetDebugLogsEnabled(evt.newValue);
            });

        }

        void SetupCacheSize(VisualElement rootElement)
        {
            var cacheSize = rootElement.Q<SliderInt>(k_MaxCacheSize);
            cacheSize.highValue = k_MaxCacheSizeValueGB;
            cacheSize.lowValue = k_MinCacheSizeValueGB;
            cacheSize.value = m_SettingsManager.MaxCacheSizeGb;
            cacheSize.RegisterValueChangedCallback(evt => { m_SettingsManager.SetMaxCacheSize(evt.newValue); });
            cacheSize.RegisterCallback<MouseLeaveEvent>(_ => SetClearExtraCacheButton());
        }

        void SetClearExtraCacheButton()
        {
            var cacheSizeBytes = m_IOProxy.GetDirectorySizeBytes(m_SettingsManager.BaseCacheLocation);
            if (cacheSizeBytes == 0)
            {
                m_ClearExtraCacheButton.SetEnabled(false);
            }

            var cacheSizeGb = ByteSizeConverter.ConvertBytesToGb(cacheSizeBytes);
            m_ClearExtraCacheButton.SetEnabled(cacheSizeGb >= m_SettingsManager.MaxCacheSizeGb);
        }

        void SetupCacheLocationToolbarButton(VisualElement rootElement)
        {
            var cacheLocationDropDown = rootElement.Q<ToolbarMenu>(k_CacheLocationDropdown);

            cacheLocationDropDown.menu.AppendAction(GetShowInExplorerLabel(),
                a => { m_EditorUtilityProxy.RevealInFinder(m_SettingsManager.BaseCacheLocation); });
            cacheLocationDropDown.menu.AppendAction(L10n.Tr(Constants.ChangeLocationLabel), a =>
            {
                var cacheLocation =
                    m_EditorUtilityProxy.OpenFolderPanel(L10n.Tr(Constants.CacheLocationTitle), m_SettingsManager.BaseCacheLocation,
                        string.Empty);

                // the user clicked cancel
                if (string.IsNullOrEmpty(cacheLocation))
                {
                    return;
                }

                UpdateCachePath(cacheLocation);
            });
            cacheLocationDropDown.menu.AppendAction(L10n.Tr(Constants.ResetDefaultLocation),
                a => { ResetCacheLocationToDefault(); });
        }

        void SetupImportLocationToolbarButton(VisualElement rootElement)
        {
            var importLocationDropdown = rootElement.Q<ToolbarMenu>(k_ImportLocationDropdown);
            importLocationDropdown.menu.AppendAction(GetShowInExplorerLabel(),
                a => { m_EditorUtilityProxy.RevealInFinder(m_SettingsManager.DefaultImportLocation); });
            importLocationDropdown.menu.AppendAction(L10n.Tr(Constants.ChangeLocationLabel), a =>
            {
                var importLocation = m_EditorUtilityProxy.OpenFolderPanel(L10n.Tr(Constants.ImportLocationTitle),
                    m_SettingsManager.DefaultImportLocation, string.Empty);

                // the user clicked cancel
                if (string.IsNullOrEmpty(importLocation))
                {
                    return;
                }


                UpdateDefaultImportPath(importLocation);

            });
            importLocationDropdown.menu.AppendAction(L10n.Tr(Constants.ResetDefaultLocation),
                a => { ResetImportLocationToDefault(); });
        }

        string GetShowInExplorerLabel()
        {
            return m_ApplicationProxy.Platform == RuntimePlatform.OSXEditor ? L10n.Tr(Constants.RevealInFinder) : L10n.Tr(Constants.ShowInExplorerLabel);
        }

        void CleanCache()
        {
            var result = true;
            try
            {
                result = m_IOProxy.DeleteAllFilesAndFoldersFromDirectory(m_SettingsManager.BaseCacheLocation);
                RefreshCacheSizeOnDiskLabel();
            }
            catch (UnauthorizedAccessException)
            {
                result = false;
            }

            if (!result)
            {
                SetCacheErrorLabel(L10n.Tr(Constants.AccessError));
            }
        }

        void RefreshCacheSizeOnDiskLabel()
        {
            m_CacheSizeOnDisk.text =
                $"{Utilities.BytesToReadableString(m_IOProxy.GetDirectorySizeBytes(m_SettingsManager.BaseCacheLocation))}";
        }

        void ClearExtraCache()
        {
            m_CacheEvictionManager.OnCheckEvictConditions(string.Empty);
        }

        void SetCacheErrorLabel(string message)
        {
            UIElementsUtils.Show(m_CacheErrorLabel);
            m_CacheErrorLabel.text = message;
        }

        void SetImportLocationErrorLabel(string message)
        {
            UIElementsUtils.Show(m_ImportLocationErrorLabel);
            m_ImportLocationErrorLabel.text = message;
        }

        /// <summary>
        /// Resets cache to default location depending on the operating system
        /// </summary>
        void ResetCacheLocationToDefault()
        {
            var defaultLocation = m_SettingsManager.ResetCacheLocation();
            SetPathLabelTextAndTooltip(m_AssetManagerCachePathLabel, defaultLocation, false);
            RefreshCacheSizeOnDiskLabel();
        }

        void ResetImportLocationToDefault()
        {
            var defaultLocation = m_SettingsManager.ResetImportLocation();
            SetPathLabelTextAndTooltip(m_ImportLocationPathLabel, defaultLocation, true);
            UIElementsUtils.Hide(m_ImportLocationErrorLabel);
        }

        /// <summary>
        /// Validates the new cache location and updates the settings
        /// if the location is invalid (ex. doesn't exist) an error label will be shown
        /// </summary>
        /// <param name="cacheLocation">the new cache location</param>
        void UpdateCachePath(string cacheLocation)
        {
            var validationResult = m_CachePathHelper.EnsureBaseCacheLocation(cacheLocation);

            if (!validationResult.Success)
            {
                SetCacheErrorLabel(CreateUpdateCacheErrorMessage(validationResult.ErrorType, cacheLocation));
                return;
            }

            UIElementsUtils.Hide(m_CacheErrorLabel);
            m_SettingsManager.SetCacheLocation(cacheLocation);
            SetPathLabelTextAndTooltip(m_AssetManagerCachePathLabel, cacheLocation, false);
            RefreshCacheSizeOnDiskLabel();
        }

        void UpdateDefaultImportPath(string importLocation)
        {
            if (!m_IOProxy.DirectoryExists(importLocation))
            {
                SetImportLocationErrorLabel(L10n.Tr(Constants.DirectoryDoesNotExistError));
                return;
            }

            if(!importLocation.StartsWith(m_ApplicationProxy.DataPath))
            {
                SetImportLocationErrorLabel(L10n.Tr(Constants.ImportDefaultLocationError));
                return;
            }

            UIElementsUtils.Hide(m_ImportLocationErrorLabel);
            m_SettingsManager.DefaultImportLocation = importLocation;
            SetPathLabelTextAndTooltip(m_ImportLocationPathLabel, importLocation, true);
        }

        static string CreateUpdateCacheErrorMessage(CacheValidationResultError errorType, string path)
        {
            return cacheValidationErrorMessages.TryGetValue(errorType, out var message)
                ? $"{message}. Reverting to default path.\nPath: {path}"
                : null;
        }

        static void SetPathLabelTextAndTooltip(TextElement label, string path, bool isRelativePath)
        {
            string finalPath;
            if (isRelativePath && !string.IsNullOrEmpty(path))
            {
                finalPath = Utilities.GetPathRelativeToAssetsFolderIncludeAssets(path);
            }
            else
            {
                finalPath = path;
            }

            label.text = label.tooltip = Utilities.NormalizePathSeparators(finalPath);
        }

        [SettingsProvider]
        public static SettingsProvider CreateUserSettingsProvider()
        {
            var container = ServicesContainer.instance;
            return new AssetManagerUserSettingsProvider(container.Resolve<ICachePathHelper>(),
                container.Resolve<ICacheEvictionManager>(),
                container.Resolve<ISettingsManager>(),
                container.Resolve<IIOProxy>(),
                container.Resolve<IApplicationProxy>(),
                container.Resolve<IEditorUtilityProxy>(),
                k_SettingsProviderPath, new List<string>());
        }
    }
}
