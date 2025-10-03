using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Unity.AssetManager.UI.Editor
{
    class UploadAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // If an asset was modified, we need to refresh the upload page so any changes are reflected if those assets are being prepared for uploaded.
            var pageManager = ServicesContainer.instance.Resolve<IPageManager>();
            if (pageManager?.ActivePage is UploadPage uploadPage)
            {
                uploadPage.RefreshSelection(importedAssets, deletedAssets);
            }
        }
    }

    // We need the context to be both static and serializable.
    // A ScriptableSingleton is a good way to achieve this.
    class UploadContextScriptableObject : ScriptableSingleton<UploadContextScriptableObject>
    {
        [SerializeField]
        public UploadStaging UploadStaging = new();
    }

    static partial class UssStyle
    {
        public const string UploadPageSettingsPanel = "upload-page-settings-panel";
        public const string UploadPageCustomSection = "upload-page-custom-section";
        public const string UploadPageActionSection = "upload-page-action-section";
        public const string UploadPageAllActionsSection = "upload-page-all-actions-section";
        public const string UploadPageUploadButton = "upload-page-upload-button";
        public const string UploadPageResetButton = "upload-page-reset-button";
    }

    [Serializable]
    class UploadPage : BasePage
    {
        static readonly float k_UploadSettingPanelWidth = 280f;
        static readonly string k_UploadSettingsOpenedKey = "com.unity.asset-manager-for-unity.upload-settings-panel-opened";

        static readonly HelpBoxMessage k_ScalingIssuesMessage = new(string.Format(L10n.Tr(Constants.ScalingIssuesMessage),
            Constants.ScalingIssuesThreshold), messageType:HelpBoxMessageType.Warning);

        [SerializeReference]
        IUploadManager m_UploadManager;

        [SerializeReference]
        IAssetOperationManager m_AssetOperationManager;

        [SerializeReference]
        ISettingsManager m_SettingsManager;

        bool SavedUploadSettingsPanelOpened
        {
            get => EditorPrefs.GetBool(k_UploadSettingsOpenedKey, true);
            set => EditorPrefs.SetBool(k_UploadSettingsOpenedKey, value);
        }

        UploadStaging m_UploadStaging = UploadContextScriptableObject.instance.UploadStaging;
        public UploadStaging UploadStaging => m_UploadStaging;

        IReadOnlyCollection<AssetIdentifier> m_SelectionToRestore;

        Button m_UploadAssetsButton;
        Button m_ClearUploadButton;

        public override bool DisplaySearchBar => false;
        public override bool DisplayBreadcrumbs => true;
        public override bool DisplayFilters => false;
        public override bool DisplayFooter => false;
        public override bool DisplaySavedViewControls => false;
        public override bool DisplaySort => false;
        public override bool DisplayUploadMetadata => true;
        public override bool DisplayUpdateAllButton => false;
        public override string DefaultProjectName => L10n.Tr(Constants.NoProjectSelected);

        public UploadPage(IAssetDataManager assetDataManager, IAssetsProvider assetsProvider,
            IProjectOrganizationProvider projectOrganizationProvider, IMessageManager messageManager,
            IPageManager pageManager, IDialogManager dialogManager)
            : base(assetDataManager, assetsProvider, projectOrganizationProvider, messageManager, pageManager, dialogManager)
        {
            m_UploadManager = ServicesContainer.instance.Resolve<IUploadManager>();
            m_AssetOperationManager = ServicesContainer.instance.Resolve<IAssetOperationManager>();
            m_SettingsManager = ServicesContainer.instance.Resolve<ISettingsManager>();
        }

        [MenuItem("Assets/Upload to Asset Manager", false, 21)]
        static void UploadToAssetManagerMenuItem()
        {
            var windowHook = new AssetManagerWindowHook();
            windowHook.OrganizationLoaded += LoadUploadPage;
            windowHook.OpenAssetManagerWindow();
        }

        [MenuItem("Assets/Upload to Asset Manager", true, 21)]
        static bool UploadToAssetManagerMenuItemValidation()
        {
            var uploadManager = ServicesContainer.instance.Resolve<IUploadManager>();

            return Selection.assetGUIDs is { Length: > 0 } &&
                   Selection.activeObject != null &&
                   !uploadManager.IsUploading;
        }

        static void LoadUploadPage()
        {
            AssetManagerWindow.Instance.Focus();

            var provider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();
            if (string.IsNullOrEmpty(provider.SelectedOrganization?.Id))
                return;

            var pageManager = ServicesContainer.instance.Resolve<IPageManager>();

            if (pageManager.ActivePage is not UploadPage)
            {
                pageManager.SetActivePage<UploadPage>();
            }

            var uploadPage = pageManager.ActivePage as UploadPage;
            uploadPage?.AddAssets(Selection.assetGUIDs);
        }

        public override void OnActivated()
        {
            base.OnActivated();

            if (m_UploadManager.IsUploading)
            {
                m_ProjectOrganizationProvider.SelectProject(m_UploadStaging.ProjectId, m_UploadStaging.CollectionPath);
            }
            else
            {
                m_UploadStaging.SetOrganizationInfo(m_ProjectOrganizationProvider.SelectedOrganization);
                m_UploadStaging.SetProjectId(m_ProjectOrganizationProvider.SelectedProject?.Id);
                m_UploadStaging.SetCollectionPath(m_ProjectOrganizationProvider.SelectedCollection?.GetFullPath());

                var status = m_UploadStaging.StagingStatus;
                if (status != null)
                {
                    if (status.TargetOrganizationId != m_ProjectOrganizationProvider.SelectedOrganization?.Id ||
                        status.TargetProjectId != m_ProjectOrganizationProvider.SelectedProject?.Id)
                    {
                        RefreshStagingStatus();
                    }
                }
                else
                {
                    RefreshStagingStatus();
                }
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            m_UploadManager.UploadEnded += OnUploadEnded;

            m_UploadStaging.UploadAssetEntriesChanged += UpdateButtonsState;
            m_UploadStaging.StagingStatusChanged += OnStagingStatusChanged;

            m_UploadStaging.RebuildAssetList(m_AssetDataManager);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            m_UploadManager.UploadEnded -= OnUploadEnded;

            m_UploadStaging.UploadAssetEntriesChanged -= UpdateButtonsState;
            m_UploadStaging.StagingStatusChanged -= OnStagingStatusChanged;
        }

        protected override VisualElement CreateCustomUISection()
        {
            var root = new VisualElement();
            root.AddToClassList(UssStyle.UploadPageCustomSection);

            var actions = new VisualElement();
            actions.AddToClassList(UssStyle.UploadPageAllActionsSection);

            var actionsSection = new VisualElement();
            actionsSection.AddToClassList(UssStyle.UploadPageActionSection);

            actions.Add(actionsSection);

            root.Add(actions);

            m_ClearUploadButton = new Button(() =>
            {
                if (m_UploadManager.IsUploading)
                {
                    m_UploadManager.CancelUpload();
                }
                else
                {
                    m_UploadStaging.Clear();
                    Reload();
                    m_AssetOperationManager.ClearFinishedOperations();
                }

                UpdateButtonsState();
            });

            actionsSection.Add(m_ClearUploadButton);

            m_UploadAssetsButton = new Button(UploadAssets);
            m_UploadAssetsButton.AddToClassList(UssStyle.UploadPageUploadButton);
            actionsSection.Add(m_UploadAssetsButton);

            var settingsPanel = CreateSettingsPanel();
            settingsPanel.style.width = k_UploadSettingPanelWidth;
            root.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (evt.newRect.width < k_UploadSettingPanelWidth + settingsPanel.resolvedStyle.marginRight)
                {
                    settingsPanel.style.width = evt.newRect.width - settingsPanel.resolvedStyle.marginRight;
                }
                else
                {
                    settingsPanel.style.width = k_UploadSettingPanelWidth;
                }
            });

            root.Add(settingsPanel);

            OnStagingStatusChanged();

            return root;
        }

        void OnUploadEnded(UploadEndedStatus status)
        {
            if (status == UploadEndedStatus.Success)
            {
                m_UploadStaging.Clear();
                GoBackToCollectionPage();
            }
            else
            {
                RefreshStagingStatus();
            }

            UpdateButtonsState();
        }

        void OnStagingStatusChanged()
        {
            ManageHelpBoxMessages();
            UpdateButtonsState();
        }

        public override void ToggleAsset(AssetIdentifier assetIdentifier, bool checkState)
        {
            if (SelectedAssets.Contains(assetIdentifier))
            {
                var selectedAssets = m_AssetDataManager.GetAssetsData(SelectedAssets);
                foreach (var selectedAssetIdentifier in selectedAssets.Cast<UploadAssetData>().Where(x => x.CanBeIgnored).Select(x => x.Identifier))
                {
                    m_UploadStaging.SetIgnore(selectedAssetIdentifier, !checkState);
                }
            }
            else
            {
                m_UploadStaging.SetIgnore(assetIdentifier, !checkState);
            }

            RefreshStagingStatus();
        }

        public void AddAssets(List<Object> objects)
        {
            var assetDatabaseProxy = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();
            AddAssets(objects.Select(assetDatabaseProxy.GetAssetPath).Select(assetDatabaseProxy.AssetPathToGuid));
        }

        public void RemoveAsset(UploadAssetData uploadAssetData)
        {
            m_UploadStaging.RemoveFromSelection(uploadAssetData.Guid);
            m_AssetOperationManager.ClearOperation(new TrackedAssetIdentifier(uploadAssetData.Identifier));
            Reload();
        }

        public void RefreshSelection(IEnumerable<string> importedAssets, IEnumerable<string> deletedAssets)
        {
            if (m_UploadStaging.IsEmpty())
                return;

            var dirty = false;
            var assetDatabaseProxy = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();

            // Make sure to remove any deleted assets from the selection
            foreach (var assetPath in deletedAssets)
            {
                var guid = assetDatabaseProxy.AssetPathToGuid(assetPath);

                if (!m_UploadStaging.RemoveFromSelection(guid))
                    continue;

                dirty = true;
            }

            // If any of the imported assets are part of the selection, mark the page as dirty
            if (!dirty && importedAssets.Select(assetDatabaseProxy.AssetPathToGuid).Any(guid => m_UploadStaging.IsSelected(guid)))
            {
                dirty = true;
            }

            if (dirty)
            {
                Reload();
            }
        }

        void AddAssets(IEnumerable<string> assetGuids)
        {
            if (m_UploadManager.IsUploading)
            {
                Debug.LogError("You cannot add assets during upload.");
                return;
            }

            foreach (var assetGuid in assetGuids)
            {
                m_UploadStaging.AddToSelection(assetGuid);
            }

            Reload();
        }

        void Reload()
        {
            m_SelectionToRestore = SelectedAssets.ToList();

            Utilities.DevLog("Analysing Selection for upload to cloud...");

            m_DialogManager?.DisplayProgressBar("Analysing Assets For Upload...", 0f);

            var assetDatabase = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();

            m_UploadStaging.GenerateUploadAssetData((guid, progress) =>
            {
                try
                {
                    var assetName = Path.GetFileName(assetDatabase.GuidToAssetPath(guid));
                    m_DialogManager?.DisplayProgressBar("Analysing Assets For Upload...", progress * 0.5f, assetName);
                }
                catch (Exception e)
                {
                    Utilities.DevLogException(e);
                }
            });

            Clear(true);

            RefreshStagingStatus(0.5f);
        }

        void RefreshStagingStatus(float startProgress = 0f)
        {
            TaskUtils.TrackException(m_UploadStaging.RefreshStatusAsync(checkWithCloud: true, (info, progress) =>
            {
                if (progress >= 1f)
                {
                    m_DialogManager?.ClearProgressBar();
                }
                else
                {
                    m_DialogManager?.DisplayProgressBar("Resolving asset statuses", progress * (1f - startProgress) + startProgress, info);
                }
            }, default));
        }

        public void RefreshLocalStatus()
        {
            TaskUtils.TrackException(m_UploadStaging.RefreshStatusAsync(checkWithCloud: false, (info, progress) =>
            {
                if (progress >= 1f)
                {
                    m_DialogManager?.ClearProgressBar();
                }
                else
                {
                    m_DialogManager?.DisplayProgressBar("Resolving asset statuses", progress, info);
                }
            }, default));
        }

        protected internal override async IAsyncEnumerable<BaseAssetData> LoadMoreAssets(
            [EnumeratorCancellation] CancellationToken token)
        {
            // Sort the result before displaying it
            foreach (var assetData in m_UploadStaging.UploadAssets
                         .OrderBy(assetData => ((UploadAssetData)assetData).IsDependency)
                         .ThenBy(a => a.PrimaryExtension))
            {
                yield return assetData;
            }

            m_CanLoadMoreItems = false;

            await Task.CompletedTask; // Remove warning about async
        }

        VisualElement CreateSettingsPanel()
        {
            var settingsPanel = new VisualElement();
            settingsPanel.AddToClassList(UssStyle.UploadPageSettingsPanel);

            var foldout = new Foldout
            {
                text = L10n.Tr(Constants.UploadSettings),
                value = SavedUploadSettingsPanelOpened
            };
            foldout.RegisterValueChangedCallback(evt => SavedUploadSettingsPanelOpened = evt.newValue);
            settingsPanel.Add(foldout);

            var toggle = foldout.Q<Toggle>();
            toggle.focusable = false;

            // Upload Mode
            var uploadModeDropdown = CreateEnumDropdown(L10n.Tr(Constants.UploadMode), m_UploadStaging.UploadMode,
                mode =>
                {
                    m_UploadStaging.UploadMode = mode;
                    Reload();
                }, UploadSettings.GetUploadModeTooltip);

            foldout.Add(uploadModeDropdown);

            // Dependency Mode
            var dependencyModeDropdown = CreateEnumDropdown(L10n.Tr(Constants.Dependencies), m_UploadStaging.DependencyMode,
                mode =>
                {
                    m_UploadStaging.DependencyMode = mode;
                    Reload();
                }, UploadSettings.GetDependencyModeTooltip);

            foldout.Add(dependencyModeDropdown);

            // File Paths Mode
            var filePathModeDropdown = CreateEnumDropdown(L10n.Tr(Constants.FilePaths), m_UploadStaging.FilePathMode,
                mode =>
                {
                    m_UploadStaging.FilePathMode = mode;
                    Reload();
                }, UploadSettings.GetFilePathModeTooltip);

            foldout.Add(filePathModeDropdown);

            // Reset Button
            var resetButton = new Button(() =>
            {
                m_UploadStaging.ResetDefaultSettings();
                uploadModeDropdown.index = (int)m_UploadStaging.UploadMode;
                dependencyModeDropdown.index = (int)m_UploadStaging.DependencyMode;
                filePathModeDropdown.index = (int)m_UploadStaging.FilePathMode;
            })
            {
                text = L10n.Tr(Constants.UploadSettingsReset)
            };

            resetButton.AddToClassList(UssStyle.UploadPageResetButton);
            foldout.Add(resetButton);

            return settingsPanel;
        }

        DropdownField CreateEnumDropdown<TEnum>(string name, TEnum defaultValue, Action<TEnum> onValueChanged, Func<TEnum, string> tooltipProvider) where TEnum : Enum
        {
            var dropDown = new DropdownField(name)
            {
                choices = Enum.GetNames(typeof(TEnum)).Select(ObjectNames.NicifyVariableName).ToList(),
                index = (int)Enum.ToObject(typeof(TEnum), defaultValue),
                tooltip = tooltipProvider.Invoke(defaultValue)
            };

            dropDown.RegisterValueChangedCallback(v =>
            {
                var value = (TEnum)Enum.ToObject(typeof(TEnum), dropDown.index);
                dropDown.tooltip = tooltipProvider.Invoke(value);
                onValueChanged?.Invoke(value);
            });

            return dropDown;
        }

        protected override void OnLoadMoreSuccessCallBack()
        {
            if (m_ProjectOrganizationProvider.SelectedProject == null)
            {
                // If no project is selected (coming from AllAssets or AM window never was opened), show select project message
                SetPageMessage(MissingSelectedProjectMessage);
            }
            else if (!AssetList.Any() || m_UploadStaging.UploadAssets.Count == 0)
            {
                SetPageMessage(new Message(Constants.UploadNoAssetsMessage));
            }
            else
            {
                m_MessageManager.ClearGridViewMessage();
            }

            if (m_SelectionToRestore != null)
            {
                // Some selected asset might not exist anymore in this page, so we remove them.
                var parsedSelection = m_SelectionToRestore.Where(identifier => AssetList.FirstOrDefault(assetData => assetData.Identifier == identifier) != null);
                m_SelectionToRestore = null;

                SelectAssets(parsedSelection);
            }
        }

        void ManageHelpBoxMessages()
        {
            if (m_UploadStaging.StagingStatus == null)
                return;

            if (m_UploadStaging.StagingStatus.ReadyAssetCount > Constants.ScalingIssuesThreshold)
            {
                m_MessageManager.SetHelpBoxMessage(k_ScalingIssuesMessage);
            }
            else
            {
                m_MessageManager.ClearHelpBoxMessage();
            }
        }

        protected override void OnProjectSelectionChanged(ProjectInfo projectInfo, CollectionInfo collectionInfo)
        {
            // Stay in upload page
            m_UploadStaging.SetProjectId(projectInfo?.Id);
            m_UploadStaging.SetCollectionPath(collectionInfo?.GetFullPath());

            // Changing the project or organization require to recompute the UploadAssetData because re-upload might change from one project to another
            Reload();
        }

        public void UploadAssets()
        {
            var status = m_UploadStaging.StagingStatus;
            Utilities.DevAssert(status != null);

            if (status == null || status.ReadyAssetCount == 0)
            {
                Debug.LogError("No assets to upload");
                return;
            }

            var hasIgnoredDependencies = status.ManuallyIgnoredDependencyCount > 0;
            if (hasIgnoredDependencies)
            {
                var userWantsToUploadWithMissingDependencies = ServicesContainer.instance.Resolve<IEditorUtilityProxy>()
                    .DisplayDialog(L10n.Tr(Constants.IgnoreDependenciesDialogTitle),
                        L10n.Tr(Constants.IgnoreDependenciesDialogMessage), L10n.Tr(Constants.Continue), L10n.Tr(Constants.Cancel));

                if (!userWantsToUploadWithMissingDependencies)
                    return;
            }

            // Do a better sanity check than checking the dirty status
            var hasDirtyAssets = m_UploadStaging.HasDirtyAssets();
            if (hasDirtyAssets)
            {
                var userWantsToUploadWithDirtyAssets = ServicesContainer.instance.Resolve<IEditorUtilityProxy>()
                    .DisplayDialog(L10n.Tr(Constants.DirtyAssetsDialogTitle),
                        L10n.Tr(Constants.DirtyAssetsDialogMessage),
                        L10n.Tr(Constants.DirtyAssetsDialogOk),
                        L10n.Tr(Constants.DirtyAssetsDialogCancel));

                if (!userWantsToUploadWithDirtyAssets)
                    return;

                m_UploadStaging.SaveDirtyAssets();
            }

            Utilities.DevLog("Uploading assets...");
            TaskUtils.TrackException(UploadAssetEntriesAsync());
        }

        async Task UploadAssetEntriesAsync()
        {
            var uploadEntries = m_UploadStaging.GenerateUploadAssets();

            var task = m_UploadManager.UploadAsync(uploadEntries);

            UpdateButtonsState();

            SendUploadAnalytics(uploadEntries);

            try
            {
                await task;
            }
            catch (Exception e)
            {
                // All Exception are logged in the IUploaderManager
                // If it happens to be a ServiceClientException, we provide visual feedback to user

                var serviceExceptionInfo = ServiceExceptionHelper.GetServiceExceptionInfo(e);

                if (serviceExceptionInfo != null)
                {
                    DisplayErrorMessageFromServiceException(serviceExceptionInfo);
                }
            }
        }

        void DisplayErrorMessageFromServiceException(ServiceExceptionInfo serviceExceptionInfo)
        {
            var expDetail = string.IsNullOrEmpty(serviceExceptionInfo.Detail) ? serviceExceptionInfo.Message : serviceExceptionInfo.Detail;
            var uploadErrorMessage = $"Upload operation failed : [{serviceExceptionInfo.StatusCode}] {expDetail}";

            m_MessageManager.SetHelpBoxMessage(new HelpBoxMessage(uploadErrorMessage,
                RecommendedAction.OpenAssetManagerDocumentationPage, messageType:HelpBoxMessageType.Error));
        }

        void SendUploadAnalytics(IReadOnlyCollection<IUploadAsset> uploadEntries)
        {
            try
            {
                var uploadSettings = new UploadEvent.UploadSettings
                {
                    UploadMode = m_UploadStaging.UploadMode.ToString(),
                    DependencyMode = m_UploadStaging.DependencyMode.ToString(),
                    FilePathMode = m_UploadStaging.FilePathMode.ToString(),
                    UseCollection = !string.IsNullOrEmpty(m_UploadStaging.CollectionPath),
                    UseLatestDependencies = m_SettingsManager.IsUploadDependenciesUsingLatestLabel
                };

                var uploadEvent = UploadEvent.CreateFromUploadData(uploadEntries, uploadSettings);
                AnalyticsSender.SendEvent(uploadEvent);
            }
            catch (Exception e)
            {
                Utilities.DevLogException(e);
            }
        }

        void UpdateButtonsState()
        {
            if (m_ClearUploadButton == null || m_UploadAssetsButton == null)
                return;

            if (m_UploadManager.IsUploading)
            {
                DisableUIComponent(UIComponents.Inspector | UIComponents.ProjectSideBar);
                m_ClearUploadButton.SetEnabled(true);
                m_ClearUploadButton.text = L10n.Tr(Constants.CancelUploadActionText);
                m_UploadAssetsButton.SetEnabled(false);
                m_UploadAssetsButton.text = L10n.Tr(Constants.UploadingText);
                return;
            }

            EnableUIComponent(UIComponents.Inspector | UIComponents.ProjectSideBar);

            m_ClearUploadButton.text = L10n.Tr(Constants.ClearAllActionText);
            m_ClearUploadButton.SetEnabled(m_UploadStaging.UploadAssets.Count > 0);

            m_UploadAssetsButton.SetEnabled(false);
            m_UploadAssetsButton.text = L10n.Tr(Constants.UploadActionText);

            if (m_UploadStaging.UploadAssets.Count == 0)
            {
                m_UploadAssetsButton.tooltip = L10n.Tr(Constants.UploadNoAssetsTooltip);
                return;
            }

            if (m_UploadStaging.StagingStatus == null)
            {
                m_UploadAssetsButton.text = L10n.Tr(Constants.Processing);
                m_UploadAssetsButton.tooltip = "Preparing upload...";
                return;
            }

            var areCloudServicesReachable = ServicesContainer.instance.Resolve<IUnityConnectProxy>().AreCloudServicesReachable;
            if (!areCloudServicesReachable)
            {
                m_UploadAssetsButton.tooltip = L10n.Tr(Constants.UploadCloudServicesNotReachableTooltip);
            }
            else
            {
                m_UploadAssetsButton.tooltip = L10n.Tr(Constants.UploadWaitStatusTooltip);
                TaskUtils.TrackException(UpdateUploadButtonAsync());
            }
        }

        async Task UpdateUploadButtonAsync()
        {
            var status = m_UploadStaging.StagingStatus;

            if (status == null)
                return;

            string tooltip;

            var hasUploadPermission = await m_UploadStaging.CheckPermissionToUploadAsync();
            if (!hasUploadPermission)
            {
                tooltip = L10n.Tr(Constants.UploadNoPermissionTooltip);
            }
            else if (string.IsNullOrEmpty(m_UploadStaging.ProjectId))
            {
                tooltip = L10n.Tr(Constants.UploadNoProjectSelectedTooltip);
            }
            else if (status.ReadyAssetCount == 0)
            {
                tooltip = L10n.Tr(Constants.UploadNoAssetsTooltip);
            }
            else if (status.HasFilesOutsideProject)
            {
                tooltip = L10n.Tr(Constants.UploadOutsideProjectTooltip);
            }
            else
            {
                m_UploadAssetsButton.SetEnabled(true);

                // Add all the information inside status to the tooltip
                var assetStr = status.ReadyAssetCount > 1 ? "Assets" : "Asset";

                tooltip = $"Ready to upload {status.ReadyAssetCount} {assetStr} (out of {status.TotalAssetCount})";
                tooltip += $"\nAdded: {status.AddedAssetCount}, Updated: {status.UpdatedAssetCount}, Skipped: {status.SkippedAssetCount}";

                if (status.IgnoredAssetCount > 0)
                {
                    tooltip += $"\nIgnored: {status.IgnoredAssetCount}";
                }

                var sizeStr = Utilities.BytesToReadableString(status.TotalSize);

                tooltip += $"\n\nTotal Files (including .meta): {status.TotalFileCount}";
                tooltip += $"\nTotal Files Size: {sizeStr}";

                m_UploadAssetsButton.text = $"Upload {status.ReadyAssetCount} {assetStr}";
            }

            m_UploadAssetsButton.tooltip = tooltip;
        }

        void GoBackToCollectionPage()
        {
            if (m_PageManager == null)
                return;

            if (m_PageManager.ActivePage != this)
                return;

            m_ProjectOrganizationProvider.SelectProject(m_UploadStaging.ProjectId, m_UploadStaging.CollectionPath);
            m_PageManager.SetActivePage<CollectionPage>();
        }

        public void SetIncludeAllScripts(UploadAssetData uploadAssetData, bool include)
        {
            m_UploadStaging.SetIncludeAllScripts(uploadAssetData, include);
            Reload(); // Recalculate the upload asset data
        }

        public void AddMetadata(IEnumerable<(UploadAssetData, IMetadata)> toAddOrReplace)
        {
            foreach (var addOrReplace in toAddOrReplace)
            {
                m_UploadStaging.AddMetadata(addOrReplace.Item1.Identifier, addOrReplace.Item2);
            }

            RefreshLocalStatus();
        }

        public void RemoveMetadata(IEnumerable<UploadAssetData> uploadAssets, string fieldKey)
        {
            foreach (var uploadAssetData in uploadAssets)
            {
                m_UploadStaging.RemoveMetadata(uploadAssetData.Identifier, fieldKey);
            }

            RefreshLocalStatus();
        }
    }
}
