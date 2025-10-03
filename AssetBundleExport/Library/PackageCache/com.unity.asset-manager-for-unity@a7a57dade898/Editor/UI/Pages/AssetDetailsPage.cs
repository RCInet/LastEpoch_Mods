using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using ImportSettings = Unity.AssetManager.Editor.ImportSettings;

namespace Unity.AssetManager.UI.Editor
{
    interface IPageComponent
    {
        void OnSelection(BaseAssetData assetData);
        void RefreshUI(BaseAssetData assetData, bool isLoading = false);
        void RefreshButtons(UIEnabledStates enabled, BaseAssetData assetData, BaseOperation operationInProgress);
    }

    class AssetDetailsPage : SelectionInspectorPage
    {
        readonly IEnumerable<IPageComponent> m_PageComponents;

        VisualElement m_NoFilesWarningBox;
        VisualElement m_SameFileNamesWarningBox;
        VisualElement m_NoDependenciesBox;
        VisualElement m_FilesFoldoutContainer;
        FilesFoldout[] m_FilesFoldouts;

        BaseAssetData m_SelectedAssetData;

        BaseAssetData SelectedAssetData
        {
            get => m_SelectedAssetData;
            set
            {
                if (m_SelectedAssetData == value)
                    return;

                if (m_SelectedAssetData != null)
                {
                    m_SelectedAssetData.AssetDataChanged -= OnAssetDataEvent;
                }

                m_SelectedAssetData = value;

                if (m_SelectedAssetData != null)
                {
                    m_SelectedAssetData.AssetDataChanged += OnAssetDataEvent;
                }
            }
        }

        BaseAssetData m_PreviouslySelectedAssetData;

        readonly Action<IEnumerable<AssetPreview.IStatus>> PreviewStatusUpdated;
        readonly Action<AssetDataAttributeCollection> AssetDataAttributesUpdated;
        readonly Action<Texture2D> PreviewImageUpdated;
        readonly Action<BaseAssetData, bool> PropertiesUpdated;
        readonly Action<BaseAssetData, bool> LinkedProjectsUpdated;
        readonly Action<string> SetFileCount;
        readonly Action<string> SetFileSize;
        readonly Action<string> SetPrimaryExtension;
        readonly Action<BaseAssetData> AssetDependenciesUpdated;
        Action<Type, List<string>> ApplyFilter;

        public AssetDetailsPage(IAssetImporter assetImporter, IAssetOperationManager assetOperationManager,
            IStateManager stateManager, IPageManager pageManager, IAssetDataManager assetDataManager,
            IAssetDatabaseProxy assetDatabaseProxy, IProjectOrganizationProvider projectOrganizationProvider,
            ILinksProxy linksProxy, IUnityConnectProxy unityConnectProxy, IProjectIconDownloader projectIconDownloader,
            IPermissionsManager permissionsManager, IDialogManager dialogManager)
            : base(assetImporter, assetOperationManager, stateManager, pageManager, assetDataManager,
                assetDatabaseProxy, projectOrganizationProvider, linksProxy, unityConnectProxy, projectIconDownloader,
                permissionsManager, dialogManager)
        {
            BuildUxmlDocument();

            var header = new AssetDetailsHeader(this);
            header.OpenDashboard += LinkToDashboard;
            header.CanOpenDashboard += () => m_LinksProxy.CanOpenAssetManagerDashboard;

            var footer = new AssetDetailsFooter(this, m_DialogManager);
            footer.CancelOperation += CancelOrClearImport;
            footer.ImportAsset += ImportAssetAsync;
            footer.HighlightAsset += ShowInProjectBrowser;
            footer.RemoveAsset += RemoveFromProject;
            footer.RemoveOnlySelectedAsset += RemoveOnlyAssetFromProject;
            footer.StopTracking += StopTracking;
            footer.StopTrackingOnlySelected += StopTrackingOnlyAsset;
            PreviewStatusUpdated += footer.UpdatePreviewStatus;

            var detailsTab = new AssetDetailsTab(m_ScrollView.contentContainer, IsAnyFilterActive, m_PageManager, m_StateManager);
            detailsTab.CreateProjectChip += CreateProjectChip;
            detailsTab.CreateUserChip += CreateUserChip;
            detailsTab.ApplyFilter += OnFilterModified;
            PreviewStatusUpdated += detailsTab.UpdatePreviewStatus;
            AssetDataAttributesUpdated += detailsTab.UpdateStatusWarning;
            PreviewImageUpdated += detailsTab.SetPreviewImage;
            PropertiesUpdated += detailsTab.RefreshUI;
            LinkedProjectsUpdated += detailsTab.RefreshUI;
            SetFileCount += detailsTab.SetFileCount;
            SetFileSize += detailsTab.SetFileSize;
            SetPrimaryExtension += detailsTab.SetPrimaryExtension;
            AssetDependenciesUpdated += detailsTab.UpdateDependencyComponent;

            var versionsTab = new AssetVersionsTab(m_ScrollView.contentContainer, m_DialogManager);
            versionsTab.CreateUserChip += CreateUserChip;
            versionsTab.ApplyFilter += OnFilterModified;
            versionsTab.ImportAsset += ImportAssetAsync;

            var tabBar = new AssetDetailsPageTabs(this, footer.ButtonsContainer,
                new AssetTab[]
                {
                    detailsTab,
                    versionsTab
                });

            m_PageComponents = new IPageComponent[]
            {
                header,
                footer,
                tabBar,
                detailsTab,
                versionsTab,
            };

            RefreshUI();
        }

        public override bool IsVisible(int selectedAssetCount)
        {
            return selectedAssetCount == 1;
        }

        protected sealed override void BuildUxmlDocument()
        {
            var treeAsset = UIElementsUtils.LoadUXML("DetailsPageContainer");
            treeAsset.CloneTree(this);

            m_ScrollView = this.Q<ScrollView>("details-page-scrollview");

            // Upload metadata container
            m_UploadMetadataContainer = m_ScrollView.Q<VisualElement>("upload-metadata-container");
            m_UploadMetadataContainer.Add(new UploadMetadataContainer(m_PageManager, m_AssetDataManager, m_ProjectOrganizationProvider, m_LinksProxy));
            RefreshUploadMetadataContainer(); // Hide the container by default

            m_FilesFoldoutContainer = m_ScrollView.Q("files-container");

            m_CloseButton = this.Q<Button>("closeButton");

            m_NoFilesWarningBox = this.Q<VisualElement>("no-files-warning-box");
            m_NoFilesWarningBox.Q<Label>().text = L10n.Tr(Constants.NoFilesText);

            m_SameFileNamesWarningBox = this.Q<VisualElement>("same-files-warning-box");
            m_SameFileNamesWarningBox.Q<Label>().text = L10n.Tr(Constants.SameFileNamesText);

            m_ScrollView.viewDataKey = "details-page-scrollview";

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);

            // We need to manually refresh once to make sure the UI is updated when the window is opened.
            SelectedAssetData = m_PageManager.ActivePage?.SelectedAssets.Count > 1 ? null : m_AssetDataManager.GetAssetData(m_PageManager.ActivePage?.LastSelectedAssetId);
        }

        protected override void OnAttachToPanel(AttachToPanelEvent evt)
        {
            base.OnAttachToPanel(evt);

            m_CloseButton.clicked += OnCloseButton;

            ApplyFilter += OnFilterModified;
        }

        protected override void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            base.OnDetachFromPanel(evt);

            m_CloseButton.clicked -= OnCloseButton;

            ApplyFilter -= OnFilterModified;
        }

        protected override void OnOperationProgress(AssetDataOperation operation)
        {
            if (operation is not ImportOperation and not IndefiniteOperation) // Only import operation are displayed in the details page
                return;

            if (!UIElementsUtils.IsDisplayed(this)
                || !operation.Identifier.Equals(SelectedAssetData?.Identifier))
                return;

            RefreshButtons(SelectedAssetData, operation);
        }

        protected override void OnOperationFinished(AssetDataOperation operation)
        {
            if (operation is not ImportOperation and not IndefiniteOperation)
                return;

            if (!UIElementsUtils.IsDisplayed(this)
                || !operation.Identifier.Equals(SelectedAssetData?.Identifier)
                || operation.Status == OperationStatus.None)
                return;

            if (m_PreviouslySelectedAssetData != null &&
                operation.Status is OperationStatus.Cancelled or OperationStatus.Error)
            {
                SelectedAssetData = m_PreviouslySelectedAssetData;
            }

            RefreshUI();
        }

        protected override void OnImportedAssetInfoChanged(AssetChangeArgs args)
        {
            if (!UIElementsUtils.IsDisplayed(this)
                || !args.Added.Concat(args.Updated).Any(a => a.Equals(SelectedAssetData?.Identifier))
                || args.Removed.Any(a => a.Equals(SelectedAssetData?.Identifier)))
                return;

            // In case of an import, force a full refresh of the displayed information
            TaskUtils.TrackException(SelectAssetDataAsync(new List<BaseAssetData> { SelectedAssetData }));
        }

        void OnFilterModified(Type filterType, List<string> filterValues)
        {
            TaskUtils.TrackException(m_PageManager.PageFilterStrategy.ApplyFilter(filterType, filterValues));
        }

        void OnFilterModified(IEnumerable<string> filterValue)
        {
            m_PageManager.PageFilterStrategy.AddSearchFilter(filterValue);
        }

        void LinkToDashboard()
        {
            m_LinksProxy.OpenAssetManagerDashboard(SelectedAssetData?.Identifier);
        }

        protected override void OnCloudServicesReachabilityChanged(bool cloudServiceReachable)
        {
            RefreshUI();
        }

        void CancelOrClearImport()
        {
            var operation = m_AssetOperationManager.GetAssetOperation(m_PageManager.ActivePage.LastSelectedAssetId);
            if (operation == null)
                return;

            if (operation.Status == OperationStatus.InProgress)
            {
                m_AssetImporter.CancelImport(m_PageManager.ActivePage.LastSelectedAssetId, true);
            }
            else
            {
                m_AssetOperationManager.ClearFinishedOperations();
            }
        }

        async void ImportAssetAsync(ImportTrigger importTrigger, string importDestination, IEnumerable<BaseAssetData> assetsToImport = null)
        {
            m_PreviouslySelectedAssetData = SelectedAssetData;
            try
            {
                var settings = new ImportSettings
                {
                    DestinationPathOverride = importDestination,
                    Type = assetsToImport == null ? ImportOperation.ImportType.UpdateToLatest : ImportOperation.ImportType.Import
                };

                // If assets have been targeted for import, we use the first asset as the selected asset
                SelectedAssetData = assetsToImport?.FirstOrDefault() ?? SelectedAssetData;

                // If no assets have been targeted for import, we use the selected asset from the details page
                assetsToImport ??= m_PageManager.ActivePage.SelectedAssets.Select(x => m_AssetDataManager.GetAssetData(x));

                var importResult = await m_AssetImporter.StartImportAsync(importTrigger, assetsToImport.ToList(), settings);
                SelectedAssetData = importResult.Assets?.FirstOrDefault() ?? m_PreviouslySelectedAssetData;

                if (importResult.Assets == null || !importResult.Assets.Any())
                {
                    RefreshUI();
                }
                else
                {
                    await SelectedAssetData.RefreshVersionsAsync();
                    RefreshButtons(SelectedAssetData,
                        m_AssetImporter.GetImportOperation(SelectedAssetData?.Identifier));
                }
            }
            catch (Exception)
            {
                SelectedAssetData = m_PreviouslySelectedAssetData;
                RefreshUI();
                throw;
            }
        }

        void ShowInProjectBrowser()
        {
            m_AssetImporter.ShowInProject(SelectedAssetData?.Identifier);

            AnalyticsSender.SendEvent(new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.Show));
        }

        bool RemoveFromProject()
        {
            AnalyticsSender.SendEvent(new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.Remove));

            try
            {
                var removeAssets = m_AssetDataManager.FindExclusiveDependencies(new List<AssetIdentifier> {SelectedAssetData?.Identifier});
                return m_AssetImporter.RemoveImports(removeAssets.ToList(), true);
            }
            catch (Exception)
            {
                RefreshButtons(SelectedAssetData, m_AssetImporter.GetImportOperation(SelectedAssetData?.Identifier));
                throw;
            }
        }

        bool RemoveOnlyAssetFromProject()
        {
            AnalyticsSender.SendEvent(new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.RemoveSelected));

            try
            {
                return m_AssetImporter.RemoveImport(SelectedAssetData?.Identifier, true);
            }
            catch (Exception)
            {
                RefreshButtons(SelectedAssetData, m_AssetImporter.GetImportOperation(SelectedAssetData?.Identifier));
                throw;
            }
        }

        bool StopTracking()
        {
            AnalyticsSender.SendEvent(new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.StopTracking));

            try
            {
                var removeAssets = m_AssetDataManager.FindExclusiveDependencies(new List<AssetIdentifier> {SelectedAssetData?.Identifier});
                m_AssetImporter.StopTrackingAssets(removeAssets.ToList());
                return true;
            }
            catch (Exception)
            {
                RefreshButtons(SelectedAssetData, m_AssetImporter.GetImportOperation(SelectedAssetData?.Identifier));
                throw;
            }
        }

        bool StopTrackingOnlyAsset()
        {
            AnalyticsSender.SendEvent(new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.StopTrackingSelected));

            try
            {
                m_AssetImporter.StopTrackingAssets(new List<AssetIdentifier> { SelectedAssetData?.Identifier });
                return true;
            }
            catch (Exception)
            {
                RefreshButtons(SelectedAssetData, m_AssetImporter.GetImportOperation(SelectedAssetData?.Identifier));
                throw;
            }
        }

        void OnAssetDataEvent(BaseAssetData assetData, AssetDataEventType eventType)
        {
            if (assetData != SelectedAssetData)
                return;

            switch (eventType)
            {
                case AssetDataEventType.FilesChanged:
                case AssetDataEventType.PrimaryFileChanged:  // Intentional fallthrough
                    RefreshSourceFilesInformationUI(SelectedAssetData);
                    break;
                case AssetDataEventType.ThumbnailChanged:
                    PreviewImageUpdated?.Invoke(SelectedAssetData.Thumbnail);
                    break;
                case AssetDataEventType.AssetDataAttributesChanged:
                    PreviewStatusUpdated?.Invoke(SelectedAssetData.AssetDataAttributeCollection.GetOverallStatus());
                    AssetDataAttributesUpdated?.Invoke(SelectedAssetData.AssetDataAttributeCollection);
                    RefreshButtons(SelectedAssetData);
                    break;
                case AssetDataEventType.PropertiesChanged:
                    PropertiesUpdated?.Invoke(SelectedAssetData, false);
                    CountFiles(GetFiles(SelectedAssetData));
                    break;
                case AssetDataEventType.LinkedProjectsChanged:
                    LinkedProjectsUpdated?.Invoke(SelectedAssetData, false);
                    CountFiles(GetFiles(SelectedAssetData));
                    break;
                case AssetDataEventType.DependenciesChanged:
                    AssetDependenciesUpdated?.Invoke(SelectedAssetData);
                    break;
            }
        }

        void RefreshUI(bool isLoading = false)
        {
            if (SelectedAssetData == null)
                return;

            foreach (var component in m_PageComponents)
            {
                component.RefreshUI(SelectedAssetData, isLoading);
            }

            PreviewStatusUpdated?.Invoke(AssetDataStatus.GetOverallStatus(SelectedAssetData.AssetDataAttributeCollection));

            foreach (var foldout in m_FilesFoldouts ?? Array.Empty<FilesFoldout>())
            {
                foldout.RefreshFoldoutStyleBasedOnExpansionStatus();
            }

            RefreshSourceFilesInformationUI(SelectedAssetData);
            RefreshUploadMetadataContainer();

            if (m_PageManager.ActivePage is not UploadPage)
            {
                var operation = m_AssetOperationManager.GetAssetOperation(SelectedAssetData.Identifier);
                RefreshButtons(SelectedAssetData, operation);
            }
        }

        protected override async Task SelectAssetDataAsync(IReadOnlyCollection<BaseAssetData> assetData)
        {
            if (assetData == null || assetData.Count > 1)
            {
                SelectedAssetData = null;
                return;
            }

            SelectedAssetData = assetData.FirstOrDefault();

            if (SelectedAssetData == null)
                return;

            var requiresLoading = SelectedAssetData is not UploadAssetData && !m_AssetDataManager.IsInProject(SelectedAssetData.Identifier);

            foreach (var component in m_PageComponents)
            {
                component.OnSelection(SelectedAssetData);
            }

            RefreshUI(requiresLoading);
            RefreshScrollView();

            var tasks = new List<Task>
            {
                SelectedAssetData.GetThumbnailAsync(),
                SelectedAssetData.RefreshAssetDataAttributesAsync(),
                SelectedAssetData.RefreshLinkedProjectsAsync()
            };
            if (requiresLoading)
            {
                tasks.Add(SelectedAssetData.RefreshPropertiesAsync());
                tasks.Add(SelectedAssetData.ResolveDatasetsAsync());
                tasks.Add(SelectedAssetData.RefreshDependenciesAsync());
            }

            await TaskUtils.WaitForTasksWithHandleExceptions(tasks);
        }

        async Task<UserChip> CreateUserChip(string userId, Type searchFilterType)
        {
            UserChip userChip = null;

            if (userId is "System" or "Service Account")
            {
                var userInfo = new UserInfo { Name = L10n.Tr(Constants.ServiceAccountText) };
                userChip = RefreshUserChip(userInfo, null);
            }
            else
            {
                var userInfos = await m_ProjectOrganizationProvider.SelectedOrganization.GetUserInfosAsync();
                var userInfo = userInfos.Find(ui => ui.UserId == userId);
                userChip = RefreshUserChip(userInfo, searchFilterType);
            }

            return userChip;
        }

        UserChip RefreshUserChip(UserInfo userInfo, Type searchFilterType)
        {
            if (userInfo == null || string.IsNullOrEmpty(userInfo.Name))
            {
                return null;
            }

            var userChip = new UserChip(userInfo);

            if (searchFilterType != null)
            {
                userChip.RegisterCallback<ClickEvent>(_ => ApplyFilter?.Invoke(searchFilterType, new List<string>{userInfo.Name}));
            }

            return userChip;
        }

        ProjectChip CreateProjectChip(string projectId)
        {
            var projectInfo = m_ProjectOrganizationProvider.SelectedOrganization?.ProjectInfos.Find(p => p.Id == projectId);

            if (projectInfo == null)
            {
                return null;
            }

            var projectChip = new ProjectChip(projectInfo);
            projectChip.ProjectChipClickAction += p => { m_ProjectOrganizationProvider.SelectProject(p.Id); };

            m_ProjectIconDownloader.DownloadIcon(projectInfo.Id, (id, icon) =>
            {
                if (id == projectInfo.Id)
                {
                    projectChip.SetIcon(icon);
                }
            });

            return projectChip;
        }

        protected override void OnAssetDataChanged(AssetChangeArgs args)
        {
            if (!UIElementsUtils.IsDisplayed(this)
                || !args.Added.Concat(args.Removed).Concat(args.Updated).Any(a => a.Equals(SelectedAssetData?.Identifier)))
                return;

            RefreshButtons(SelectedAssetData, m_AssetImporter.GetImportOperation(SelectedAssetData?.Identifier));
        }

        void RefreshSourceFilesInformationUI(BaseAssetData assetData)
        {
            SetPrimaryExtension?.Invoke(assetData.PrimaryExtension);

            m_FilesFoldoutContainer.Clear();

            var foldouts = new List<FilesFoldout>();
            foreach (var dataset in assetData.Datasets)
            {
                if (CreateFileFoldout(dataset, out var filesFoldout))
                {
                    foldouts.Add(filesFoldout);

                    var datasetFiles = dataset.Files.Where(f =>
                    {
                        if (string.IsNullOrEmpty(f?.Path))
                            return false;

                        return !AssetDataDependencyHelper.IsASystemFile(Path.GetExtension(f.Path));
                    });

                    if (datasetFiles.Any())
                    {
                        filesFoldout.Populate(assetData, datasetFiles);
                    }
                    else
                    {
                        filesFoldout.Clear();
                    }

                    filesFoldout.StopPopulating();
                }
            }

            m_FilesFoldouts = foldouts.ToArray();

            var files = GetFiles(assetData);

            CountFiles(files);

            UIElementsUtils.SetDisplay(m_NoFilesWarningBox, !files.Any());
            UIElementsUtils.SetDisplay(m_SameFileNamesWarningBox, HasCaseInsensitiveMatch(assetData.GetFiles()?.Select(f => f.Path)));
        }

        bool CreateFileFoldout(AssetDataset assetDataset, out FilesFoldout filesFoldout)
        {
            filesFoldout = null;

            if (assetDataset.CanBeImported)
            {
                filesFoldout = new FilesFoldout(m_FilesFoldoutContainer, assetDataset.Name, assetDataset.IsSourceControlled, m_AssetDatabaseProxy)
                {
                    Expanded = m_StateManager.GetFilesFoldoutValue(assetDataset.Name)
                };

                filesFoldout.RegisterValueChangedCallback(value =>
                {
                    m_StateManager.SetFilesFoldoutValue(assetDataset.Name, value);
                    RefreshScrollView();
                });
            }

            return filesFoldout != null;
        }

        static List<BaseAssetDataFile> GetFiles(BaseAssetData assetData)
        {
            return assetData.GetFiles()?.Where(f =>
            {
                if (string.IsNullOrEmpty(f?.Path))
                    return false;

                return !AssetDataDependencyHelper.IsASystemFile(Path.GetExtension(f.Path));
            }).ToList();
        }

        void CountFiles(List<BaseAssetDataFile> files)
        {
            if (files != null && files.Any())
            {
                long totalFileSize = 0;
                var totalFilesCount = 0;
                var incompleteFilesCount = 0;

                var assetFileSize = files.Sum(i => i.FileSize);
                totalFileSize += assetFileSize;
                totalFilesCount += files.Count;
                incompleteFilesCount += files.Count(f => !f.Available);

                SetFileSize?.Invoke(Utilities.BytesToReadableString(totalFileSize));
                SetFileCount?.Invoke(incompleteFilesCount > 0 ? $"{totalFilesCount} [{incompleteFilesCount} incomplete]" : totalFilesCount.ToString());
            }
            else
            {
                SetFileSize?.Invoke(Utilities.BytesToReadableString(0));
                SetFileCount?.Invoke("0");
            }
        }

        void RefreshButtons(BaseAssetData assetData)
        {
            if (m_PageManager.ActivePage is not UploadPage)
            {
                var operation = m_AssetOperationManager.GetAssetOperation(assetData.Identifier);
                RefreshButtons(assetData, operation);
            }
        }

        void RefreshButtons(BaseAssetData assetData, BaseOperation importOperation)
        {
            var status = AssetDataStatus.GetStatusOfImport(assetData?.AssetDataAttributeCollection);
            var enabled = UIEnabledStates.CanImport.GetFlag(assetData is AssetData);
            enabled |= UIEnabledStates.InProject.GetFlag(m_AssetDataManager.IsInProject(assetData?.Identifier));
            enabled |= UIEnabledStates.ServicesReachable.GetFlag(m_UnityConnectProxy.AreCloudServicesReachable);
            enabled |= UIEnabledStates.ValidStatus.GetFlag(status == null || !string.IsNullOrEmpty(status.ActionText));
            enabled |= UIEnabledStates.IsImporting.GetFlag(importOperation?.Status == OperationStatus.InProgress);
            enabled |= UIEnabledStates.HasPermissions.GetFlag(false);

            var files = assetData?.GetFiles()?.ToList();
            if (files != null && files.Any()) // has files
            {
                if (!HasCaseInsensitiveMatch(files.Select(f => f.Path)) // files have unique names
                    && files.All(file => file.Available)) // files are all available
                {
                    enabled |= UIEnabledStates.CanImport;
                }
                else
                {
                    enabled &= ~UIEnabledStates.CanImport;
                }
            }

            foreach (var component in m_PageComponents)
            {
                component.RefreshButtons(enabled, assetData, importOperation);
            }

            TaskUtils.TrackException(RefreshButtonsAsync(assetData, importOperation, enabled));
        }

        async Task RefreshButtonsAsync(BaseAssetData assetData, BaseOperation importOperation, UIEnabledStates enabled)
        {
            var hasPermissions = await m_PermissionsManager.CheckPermissionAsync(assetData?.Identifier.OrganizationId, assetData?.Identifier.ProjectId, Constants.ImportPermission);
            enabled |= UIEnabledStates.HasPermissions.GetFlag(hasPermissions);

            foreach (var component in m_PageComponents)
            {
                component.RefreshButtons(enabled, assetData, importOperation);
            }
        }

        static bool HasCaseInsensitiveMatch(IEnumerable<string> files)
        {
            if (files == null)
                return false;

            var seenFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            return files.Any(file => !seenFiles.Add(file));
        }

        bool IsAnyFilterActive()
        {
            var pageFiltersStrategy = m_PageManager?.PageFilterStrategy;
            return pageFiltersStrategy?.SelectedFilters?.Count > 0 || pageFiltersStrategy?.SearchFilters?.Count > 0;
        }
    }
}
