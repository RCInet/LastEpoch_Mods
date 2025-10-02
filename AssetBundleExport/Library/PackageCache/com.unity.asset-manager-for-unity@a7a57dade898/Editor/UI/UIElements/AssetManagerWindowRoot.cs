using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    [Flags]
    enum UIComponents
    {
        None = 0,
        Inspector = 1 << 0,
        ProjectSideBar = 1 << 1,
        // Add the other UI components here

        All = Inspector | ProjectSideBar
    }


    class AssetManagerWindowRoot : VisualElement
    {
        const int k_CloudStorageUsageRefreshMs = 30000;
        const int k_SidebarMinWidth = 160;
        const int k_InspectorPanelMaxWidth = 300;
        const int k_InspectorPanelMinWidth = 200;
        const string k_MainDarkUssName = "MainDark";
        const string k_MainLightUssName = "MainLight";
        const string k_InspectorPanelLastWidthPrefKey = "InspectorPanelLastWidth";

        VisualElement m_AssetManagerContainer;
        VisualElement m_SearchContentSplitViewContainer;
        VisualElement m_ContentContainer;
        VisualElement m_SelectionInspectorContainer;
        LoadingScreen m_LoadingScreen;

        LoginPage m_LoginPage;
        AwaitingLoginPage m_AwaitingLoginPage;
        SideBar m_SideBar;
        SearchBar m_SearchBar;
        Breadcrumbs m_Breadcrumbs;
        Filters m_Filters;
        SavedViewControls m_SavedViewControls;
        Sort m_Sort;
        TwoPaneSplitView m_CategoriesSplit;
        TwoPaneSplitView m_InspectorSplit;
        UpdateAllButton m_UpdateAllButton;

        AssetsGridView m_AssetsGridView;
        readonly List<SelectionInspectorPage> m_SelectionInspectorPages = new();
        ActionHelpBox m_ActionHelpBox;

        IVisualElementScheduledItem m_StorageInfoRefreshScheduledItem;

        VisualElement m_CustomizableSection;

        readonly IPageManager m_PageManager;
        readonly IAssetDataManager m_AssetDataManager;
        readonly IAssetImporter m_AssetImporter;
        readonly IAssetOperationManager m_AssetOperationManager;
        readonly IStateManager m_StateManager;
        readonly IUnityConnectProxy m_UnityConnect;
        readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;
        readonly ILinksProxy m_LinksProxy;
        readonly IAssetDatabaseProxy m_AssetDatabaseProxy;
        readonly IProjectIconDownloader m_ProjectIconDownloader;
        readonly IPermissionsManager m_PermissionsManager;
        readonly IUploadManager m_UploadManager;
        readonly IPopupManager m_PopupManager;
        readonly IAssetImportResolver m_AssetImportResolver;
        readonly IMessageManager m_MessageManager;
        readonly IApplicationProxy m_ApplicationProxy;
        readonly IDialogManager m_DialogManager;
        readonly ISettingsManager m_SettingsManager;
        readonly ISavedAssetSearchFilterManager m_SavedSearchFilterManager;

        static int InspectorPanelLastWidth
        {
            get => EditorPrefs.GetInt(k_InspectorPanelLastWidthPrefKey, k_InspectorPanelMaxWidth);
            set => EditorPrefs.SetInt(k_InspectorPanelLastWidthPrefKey, value);
        }

        public AssetManagerWindowRoot(IPageManager pageManager,
            IAssetDataManager assetDataManager,
            IAssetImporter assetImporter,
            IAssetOperationManager assetOperationManager,
            IStateManager stateManager,
            IUnityConnectProxy unityConnect,
            IProjectOrganizationProvider projectOrganizationProvider,
            ILinksProxy linksProxy,
            IAssetDatabaseProxy assetDatabaseProxy,
            IProjectIconDownloader projectIconDownloader,
            IPermissionsManager permissionsManager,
            IUploadManager uploadManager,
            IPopupManager popupManager,
            IAssetImportResolver assetImportResolver,
            IMessageManager messageManager,
            IApplicationProxy applicationProxy,
            IDialogManager dialogManager,
            ISettingsManager settingsManager,
            ISavedAssetSearchFilterManager savedSearchFilterManager)
        {
            m_PageManager = pageManager;
            m_AssetDataManager = assetDataManager;
            m_AssetImporter = assetImporter;
            m_AssetOperationManager = assetOperationManager;
            m_StateManager = stateManager;
            m_UnityConnect = unityConnect;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_LinksProxy = linksProxy;
            m_AssetDatabaseProxy = assetDatabaseProxy;
            m_ProjectIconDownloader = projectIconDownloader;
            m_PermissionsManager = permissionsManager;
            m_UploadManager = uploadManager;
            m_PopupManager = popupManager;
            m_AssetImportResolver = assetImportResolver;
            m_MessageManager = messageManager;
            m_ApplicationProxy = applicationProxy;
            m_DialogManager = dialogManager;
            m_SettingsManager = settingsManager;
            m_SavedSearchFilterManager = savedSearchFilterManager;
        }

        public void OnEnable()
        {
            InitializeLayout();
            RegisterCallbacks();

            // We need to do an initial refresh here to make sure the window is correctly updated after it's opened
            // All following Refresh called will triggered through callbacks
            Refresh();
        }

        public void OnDisable()
        {
            UnregisterCallbacks();
        }

        void InitializeLayout()
        {
            UIElementsUtils.LoadCommonStyleSheet(this);
            UIElementsUtils.LoadCustomStyleSheet(this,
                EditorGUIUtility.isProSkin ? k_MainDarkUssName : k_MainLightUssName);

            m_AwaitingLoginPage  = new AwaitingLoginPage(m_SettingsManager);
            m_AwaitingLoginPage.AddToClassList("SignInPage");
            m_AwaitingLoginPage.StretchToParentSize();
            Add(m_AwaitingLoginPage);

            m_LoginPage = new LoginPage(m_LinksProxy);
            m_LoginPage.AddToClassList("SignInPage");
            m_LoginPage.StretchToParentSize();
            Add(m_LoginPage);

            m_AssetManagerContainer = new VisualElement();
            Add(m_AssetManagerContainer);
            m_AssetManagerContainer.AddToClassList("AssetManagerContainer");
            m_AssetManagerContainer.StretchToParentSize();

            m_PopupManager.CreatePopupContainer(this);

            m_LoadingScreen = new LoadingScreen();
            m_LoadingScreen.AddToClassList("LoadingScreen");
            m_LoadingScreen.SetVisible(false);
            m_AssetManagerContainer.Add(m_LoadingScreen);

            m_InspectorSplit =
                new TwoPaneSplitView(1, k_InspectorPanelMaxWidth, TwoPaneSplitViewOrientation.Horizontal);
            m_CategoriesSplit = new TwoPaneSplitView(0, k_SidebarMinWidth, TwoPaneSplitViewOrientation.Horizontal);

            m_SideBar = new SideBar(m_UnityConnect, m_StateManager, m_PageManager,
                m_MessageManager, m_ProjectOrganizationProvider, m_SavedSearchFilterManager, m_PermissionsManager, m_CategoriesSplit);
            m_SideBar.AddToClassList("SideBarContainer");
            m_CategoriesSplit.Add(m_SideBar);

            m_SearchContentSplitViewContainer = new VisualElement();
            m_SearchContentSplitViewContainer.AddToClassList("SearchContentSplitView");
            m_CategoriesSplit.Add(m_SearchContentSplitViewContainer);
            m_CategoriesSplit.fixedPaneInitialDimension = m_StateManager.SideBarWidth;

            var tabView = new TabView(m_PageManager, m_UnityConnect);
            tabView.AddPage<CollectionPage>(L10n.Tr(Constants.AssetsTabLabel));
            tabView.MergePage<CollectionPage, AllAssetsPage>();
            tabView.AddPage<InProjectPage>(L10n.Tr(Constants.InProjectTabLabel));
            tabView.AddPage<UploadPage>(L10n.Tr(Constants.UploadTabLabel));
            m_SearchContentSplitViewContainer.Add(tabView);

            var actionHelpBoxContainer = new VisualElement();
            actionHelpBoxContainer.AddToClassList("HelpBoxContainer");
            m_ActionHelpBox = new ActionHelpBox(m_UnityConnect, m_ApplicationProxy, m_PageManager,
                m_ProjectOrganizationProvider, m_MessageManager, m_LinksProxy, m_SettingsManager);
            actionHelpBoxContainer.Add(m_ActionHelpBox);
            m_SearchContentSplitViewContainer.Add(actionHelpBoxContainer);

            var storageInfoHelpBoxContainer = new VisualElement();
            storageInfoHelpBoxContainer.AddToClassList("HelpBoxContainer");
            var storageInfoHelpBox = new StorageInfoHelpBox(m_PageManager, m_ProjectOrganizationProvider, m_LinksProxy, m_UnityConnect, m_SettingsManager);
            storageInfoHelpBoxContainer.Add(storageInfoHelpBox);
            m_SearchContentSplitViewContainer.Add(storageInfoHelpBoxContainer);

            // Schedule storage info to be refreshed each 30 seconds
            m_StorageInfoRefreshScheduledItem = storageInfoHelpBox.schedule.Execute(storageInfoHelpBox.RefreshCloudStorageAsync).Every(k_CloudStorageUsageRefreshMs);
            var topContainer = new VisualElement();
            topContainer.AddToClassList("unity-top-container");

            var topLeftContainer = new VisualElement();
            topLeftContainer.AddToClassList("unity-top-left-container");

            var pageTitle = new PageTitle(m_PageManager);
            topLeftContainer.Add(pageTitle);

            m_Breadcrumbs = new Breadcrumbs(m_PageManager, m_ProjectOrganizationProvider);
            topLeftContainer.Add(m_Breadcrumbs);

            var roleChip = new RoleChip(m_PageManager, m_ProjectOrganizationProvider, m_PermissionsManager, m_LinksProxy);
            topLeftContainer.Add(roleChip);

            topContainer.Add(topLeftContainer);

            var topRightContainer = new VisualElement();
            topRightContainer.AddToClassList("unity-top-right-container");

            m_UpdateAllButton= new UpdateAllButton(m_AssetImporter, m_PageManager, m_ProjectOrganizationProvider, m_ApplicationProxy);
            topRightContainer.Add(m_UpdateAllButton);

            topContainer.Add(topRightContainer);

            m_SearchContentSplitViewContainer.Add(topContainer);

            m_SearchBar = new SearchBar(m_PageManager, m_ProjectOrganizationProvider,
                m_MessageManager);
            m_SearchContentSplitViewContainer.Add(m_SearchBar);

            var filtersSortContainer = new VisualElement();
            filtersSortContainer.AddToClassList("unity-filters-sort-container");
            m_SearchContentSplitViewContainer.Add(filtersSortContainer);

            m_Filters = new Filters(m_PageManager, m_ProjectOrganizationProvider, m_ApplicationProxy, m_PopupManager);
            filtersSortContainer.Add(m_Filters);

            var savedViewSortContainer = new VisualElement();
            savedViewSortContainer.AddToClassList("unity-saved-view-sort-container");

            m_SavedViewControls = new SavedViewControls(m_PageManager, m_ProjectOrganizationProvider,
                m_SavedSearchFilterManager);
            savedViewSortContainer.Add(m_SavedViewControls);

            m_Sort = new Sort(m_PageManager, m_ProjectOrganizationProvider, m_SavedSearchFilterManager);
            savedViewSortContainer.Add(m_Sort);
            filtersSortContainer.Add(savedViewSortContainer);

            var content = new VisualElement();
            content.AddToClassList("AssetManagerContentView");
            m_SearchContentSplitViewContainer.Add(content);

            if (!ServicesContainer.instance.Resolve<IContextMenuBuilder>().IsContextMenuRegistered(typeof(AssetData)))
            {
                ServicesContainer.instance.Resolve<IContextMenuBuilder>()
                    .RegisterContextMenu(typeof(AssetData), typeof(CloudAssetContextMenu));
            }

            if (!ServicesContainer.instance.Resolve<IContextMenuBuilder>()
                    .IsContextMenuRegistered(typeof(UploadAssetData)))
            {
                ServicesContainer.instance.Resolve<IContextMenuBuilder>()
                    .RegisterContextMenu(typeof(UploadAssetData), typeof(UploadContextMenu));
            }

            m_AssetsGridView = new AssetsGridView(m_ProjectOrganizationProvider, m_UnityConnect, m_PageManager,
                m_AssetDataManager, m_AssetOperationManager, m_LinksProxy, m_UploadManager, m_AssetImporter,
                m_PermissionsManager, m_MessageManager, m_ApplicationProxy);

            m_SelectionInspectorPages.Add(new AssetDetailsPage(m_AssetImporter, m_AssetOperationManager, m_StateManager,
                m_PageManager, m_AssetDataManager, m_AssetDatabaseProxy, m_ProjectOrganizationProvider, m_LinksProxy,
                m_UnityConnect, m_ProjectIconDownloader, m_PermissionsManager, m_DialogManager));

            m_SelectionInspectorPages.Add(new MultiAssetDetailsPage(m_AssetImporter, m_AssetOperationManager, m_StateManager,
                m_PageManager, m_AssetDataManager, m_AssetDatabaseProxy, m_ProjectOrganizationProvider, m_LinksProxy, m_UnityConnect,
                m_ProjectIconDownloader, m_PermissionsManager, m_DialogManager));

            m_SelectionInspectorContainer = new VisualElement();
            m_SelectionInspectorContainer.AddToClassList("SelectionInspectorContainer");
            foreach (var page in m_SelectionInspectorPages)
            {
                m_SelectionInspectorContainer.Add(page);
            }

            m_ContentContainer = new VisualElement();
            m_ContentContainer.AddToClassList("ContentPanel");
            m_ContentContainer.Add(m_CategoriesSplit);

            m_InspectorSplit.Add(m_ContentContainer);
            m_InspectorSplit.Add(m_SelectionInspectorContainer);

            m_AssetManagerContainer.Add(m_InspectorSplit);

            var activePage = m_PageManager.ActivePage;

            if (activePage == null || activePage.LastSelectedAssetId == null)
            {
                SetInspectorVisibility(null);
                SetUIComponentEnabled(null);
            }
            else
            {
                SetInspectorVisibility(activePage.SelectedAssets);
                SetUIComponentEnabled(activePage);
            }

            content.Add(m_AssetsGridView);

            m_CustomizableSection = new VisualElement();
            content.Add(m_CustomizableSection);

            SetCustomFieldsVisibility(m_PageManager.ActivePage);
        }

        void RegisterCallbacks()
        {
            m_SelectionInspectorContainer.RegisterCallback<GeometryChangedEvent>(OnInspectorResized);

            m_PermissionsManager.AuthenticationStateChanged += OnAuthenticationStateChanged;
            m_PageManager.SelectedAssetChanged += OnSelectedAssetChanged;
            m_PageManager.ActivePageChanged += OnActivePageChanged;
            m_PageManager.UIComponentEnabledChanged += OnUIComponentEnabledChanged;
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;
            m_ProjectOrganizationProvider.LoadingStateChanged += OnLoadingStateChanged;
            m_UnityConnect.CloudServicesReachabilityChanged += OnCloudServicesReachabilityChanged;
        }

        void UnregisterCallbacks()
        {
            m_SelectionInspectorContainer?.UnregisterCallback<GeometryChangedEvent>(OnInspectorResized);

            m_PermissionsManager.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            m_PageManager.SelectedAssetChanged -= OnSelectedAssetChanged;
            m_PageManager.ActivePageChanged -= OnActivePageChanged;
            m_PageManager.UIComponentEnabledChanged -= OnUIComponentEnabledChanged;
            m_ProjectOrganizationProvider.OrganizationChanged -= OnOrganizationChanged;
            m_ProjectOrganizationProvider.LoadingStateChanged -= OnLoadingStateChanged;
            m_UnityConnect.CloudServicesReachabilityChanged -= OnCloudServicesReachabilityChanged;
        }

        static void OnInspectorResized(GeometryChangedEvent evt)
        {
            InspectorPanelLastWidth = (int)(evt.newRect.width < k_InspectorPanelMinWidth
                ? InspectorPanelLastWidth
                : evt.newRect.width);
        }

        void OnCloudServicesReachabilityChanged(bool cloudServicesReachable)
        {
            Refresh();
        }

        void OnAuthenticationStateChanged(AuthenticationState _) => Refresh();

        void OnOrganizationChanged(OrganizationInfo organizationInfo) => Refresh();

        void OnSelectedAssetChanged(IPage page, IEnumerable<AssetIdentifier> assets)
        {
            SetInspectorVisibility(assets);
        }

        void OnLoadingStateChanged(bool isLoading)
        {
            m_LoadingScreen.SetVisible(isLoading);
        }

        void SetInspectorVisibility(IEnumerable<AssetIdentifier> assets)
        {
            var validAssets = assets?.Where(asset => asset.IsIdValid()).ToList();

            if (validAssets is { Count: > 0 })
            {
                m_InspectorSplit.fixedPaneInitialDimension = InspectorPanelLastWidth;
                m_InspectorSplit.UnCollapse();

                //Hide all pages
                foreach (var inspectorPage in m_SelectionInspectorPages)
                {
                    UIElementsUtils.Hide(inspectorPage);
                }

                // Show only the first page that is visible
                var inspectorPageToShow = m_SelectionInspectorPages.Find(page => page.IsVisible(validAssets.Count));
                TaskUtils.TrackException(inspectorPageToShow?.SelectedAsset(validAssets));
                UIElementsUtils.Show(inspectorPageToShow);
            }
            else
            {
                m_InspectorSplit.fixedPaneInitialDimension = 0;
                m_InspectorSplit.CollapseChild(1);

                foreach (var inspectorPage in m_SelectionInspectorPages)
                {
                    TaskUtils.TrackException(inspectorPage?.SelectionCleared());
                }
            }
        }

        void SetCustomFieldsVisibility(IPage page)
        {
            m_CustomizableSection.Clear();

            if (page == null)
                return;

            var basePage = (BasePage)page;

            UIElementsUtils.SetDisplay(m_SearchBar, basePage.DisplaySearchBar);
            UIElementsUtils.SetDisplay(m_Filters, basePage.DisplayFilters);
            UIElementsUtils.SetDisplay(m_SavedViewControls, basePage.DisplaySavedViewControls);
            UIElementsUtils.SetDisplay(m_Sort, basePage.DisplaySort);

            if (basePage.DisplaySideBar)
            {
                m_CategoriesSplit.UnCollapse();
            }
            else
            {
                m_CategoriesSplit.CollapseChild(0);
            }

            var customSection = basePage.GetCustomUISection();

            if (customSection != null)
            {
                m_CustomizableSection.Add(customSection);
            }
        }

        void OnActivePageChanged(IPage page)
        {
            SetInspectorVisibility(page.SelectedAssets);
            SetCustomFieldsVisibility(page);
            SetUIComponentEnabled(page);
        }

        void OnUIComponentEnabledChanged(IPage page, UIComponents uiComponents)
        {
            SetUIComponentEnabled(page);
        }

        void SetUIComponentEnabled(IPage page)
        {
            var uiComponent = page?.EnabledUIComponents ?? UIComponents.All;

            m_SelectionInspectorContainer.SetEnabled(uiComponent.HasFlag(UIComponents.Inspector));
            m_SideBar.SetContentEnabled(uiComponent.HasFlag(UIComponents.ProjectSideBar));
        }

        void Refresh()
        {
            m_LoginPage.Refresh();
            m_UpdateAllButton.Refresh();
            m_LoadingScreen.SetVisible(m_ProjectOrganizationProvider.IsLoading);

            if (!m_UnityConnect.AreCloudServicesReachable)
            {
                UIElementsUtils.Hide(m_LoginPage);
                UIElementsUtils.Hide(m_AwaitingLoginPage);
                UIElementsUtils.Show(m_AssetManagerContainer);

                m_ActionHelpBox.Refresh();
                return;
            }

            m_StorageInfoRefreshScheduledItem.Resume();

            if (m_PermissionsManager.AuthenticationState == AuthenticationState.AwaitingLogin)
            {
                UIElementsUtils.Hide(m_LoginPage);
                UIElementsUtils.Show(m_AwaitingLoginPage);
                UIElementsUtils.Hide(m_AssetManagerContainer);
                return;
            }

            if (m_PermissionsManager.AuthenticationState == AuthenticationState.LoggedOut)
            {
                UIElementsUtils.Show(m_LoginPage);
                UIElementsUtils.Hide(m_AwaitingLoginPage);
                UIElementsUtils.Hide(m_AssetManagerContainer);

                m_PermissionsManager.Reset();

                return;
            }

            if (m_PermissionsManager.AuthenticationState == AuthenticationState.LoggedIn)
            {
                UIElementsUtils.Hide(m_LoginPage);
                UIElementsUtils.Hide(m_AwaitingLoginPage);
                UIElementsUtils.Show(m_AssetManagerContainer);

                m_ActionHelpBox.Refresh();
            }
        }

        public bool CurrentOrganizationIsEmpty()
        {
            return m_ProjectOrganizationProvider.SelectedOrganization?.ProjectInfos?.Count == 0 &&
                !m_ProjectOrganizationProvider.IsLoading;
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            if (m_UnityConnect.AreCloudServicesReachable && m_LinksProxy.CanOpenAssetManagerDashboard)
            {
                var goToDashboard = new GUIContent(L10n.Tr("Go to Dashboard"));
                menu.AddItem(goToDashboard, false, m_LinksProxy.OpenAssetManagerDashboard);
            }

            var projectSettings = new GUIContent(L10n.Tr("Project Settings"));
            menu.AddItem(projectSettings, false, () => m_LinksProxy.OpenProjectSettings(ProjectSettingsMenu.Services));

            var preferences = new GUIContent(L10n.Tr("Preferences"));
            menu.AddItem(preferences, false, m_LinksProxy.OpenPreferences);
        }
    }

    class LoadingScreen : VisualElement
    {
        readonly LoadingIcon m_LoadingIcon;

        public LoadingScreen()
        {
            m_LoadingIcon = new LoadingIcon();
            m_LoadingIcon.AddToClassList("loading-icon");

            var loadingScreenContainer = new VisualElement();
            loadingScreenContainer.Add(m_LoadingIcon);
            loadingScreenContainer.Add(new Label { text = L10n.Tr("Loading...") });

            Add(loadingScreenContainer);
        }

        public void SetVisible(bool visibility)
        {
            if (visibility)
            {
                UIElementsUtils.Show(this);
                m_LoadingIcon.PlayAnimation();
            }
            else
            {
                UIElementsUtils.Hide(this);
                m_LoadingIcon.StopAnimation();
            }
        }
    }
}
