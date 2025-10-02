using System;
using System.Collections.Generic;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class SidebarContent : VisualElement
    {
        readonly IPageManager m_PageManager;
        readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;
        readonly IStateManager m_StateManager;
        readonly IMessageManager m_MessageManager;
        readonly IUnityConnectProxy m_UnityConnectProxy;
        readonly ISavedAssetSearchFilterManager m_SavedSearchFilterManager;
        ScrollView m_ScrollContainer;

        SideBarAllAssetsFoldout m_AllAssetsFolder;
        SidebarSavedViewContent m_SidebarSavedViewContent;
        SidebarProjectContent m_SidebarProjectContent;

        public SidebarContent(IUnityConnectProxy unityConnectProxy, IProjectOrganizationProvider projectOrganizationProvider, IPageManager pageManager,
            IStateManager stateManager, IMessageManager messageManager, ISavedAssetSearchFilterManager savedAssetSearchFilterManager)
        {
            m_UnityConnectProxy = unityConnectProxy;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_PageManager = pageManager;
            m_StateManager = stateManager;
            m_MessageManager = messageManager;
            m_SavedSearchFilterManager = savedAssetSearchFilterManager;

            InitializeLayout();

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        void InitializeLayout()
        {
            m_ScrollContainer = new ScrollView
            {
                name = Constants.CategoriesScrollViewUssName,
                mode = ScrollViewMode.Vertical
            };

            m_AllAssetsFolder = new SideBarAllAssetsFoldout(m_UnityConnectProxy, m_PageManager, m_StateManager,
                m_MessageManager, m_ProjectOrganizationProvider, Constants.AllAssetsFolderName);
            m_AllAssetsFolder.AddToClassList("allAssetsFolder");
            m_ScrollContainer.Add(m_AllAssetsFolder);

            m_SidebarSavedViewContent = new SidebarSavedViewContent(m_ProjectOrganizationProvider, m_PageManager, m_SavedSearchFilterManager);
            m_ScrollContainer.Add(m_SidebarSavedViewContent);

            m_SidebarProjectContent = new SidebarProjectContent(m_UnityConnectProxy, m_ProjectOrganizationProvider, m_PageManager,
                m_StateManager, m_MessageManager);
            m_ScrollContainer.Add(m_SidebarProjectContent);

            Add(m_ScrollContainer);
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            m_UnityConnectProxy.CloudServicesReachabilityChanged += OnCloudServicesReachabilityChanged;
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;
            m_ProjectOrganizationProvider.ProjectInfoChanged += OnProjectInfoChanged;

            Refresh();
            ScrollToHeight(m_StateManager.SideBarScrollValue);
        }

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            m_UnityConnectProxy.CloudServicesReachabilityChanged -= OnCloudServicesReachabilityChanged;
            m_ProjectOrganizationProvider.OrganizationChanged -= OnOrganizationChanged;
            m_ProjectOrganizationProvider.ProjectInfoChanged -= OnProjectInfoChanged;

            m_StateManager.SideBarScrollValue = m_ScrollContainer.verticalScroller.value;
            m_StateManager.SideBarWidth = layout.width;
        }

        void OnCloudServicesReachabilityChanged(bool cloudServicesReachable)
        {
            Refresh();
        }

        void OnOrganizationChanged(OrganizationInfo organization)
        {
            Refresh();
        }

        void OnProjectInfoChanged(ProjectInfo projectInfo)
        {
            m_SidebarProjectContent.ProjectInfoChanged(projectInfo);
        }

        void Refresh()
        {
            var showAllAssetsFolder = m_ProjectOrganizationProvider.SelectedOrganization?.ProjectInfos.Count > 1;
            UIElementsUtils.SetDisplay(m_AllAssetsFolder, showAllAssetsFolder);

            m_SidebarProjectContent.Refresh();

            if (!m_UnityConnectProxy.AreCloudServicesReachable)
            {
                UIElementsUtils.Hide(m_ScrollContainer);
                UIElementsUtils.Show(m_SidebarProjectContent.NoProjectSelectedContainer);
                return;
            }

            var projectInfos = m_ProjectOrganizationProvider.SelectedOrganization?.ProjectInfos as IList<ProjectInfo> ??
                Array.Empty<ProjectInfo>();
            if (projectInfos.Count == 0)
            {
                UIElementsUtils.Hide(m_ScrollContainer);
                UIElementsUtils.Show(m_SidebarProjectContent.NoProjectSelectedContainer);
                return;
            }

            UIElementsUtils.Show(m_ScrollContainer);
            UIElementsUtils.Hide(m_SidebarProjectContent.NoProjectSelectedContainer);

            m_ScrollContainer.verticalScroller.value = m_StateManager.SideBarScrollValue;
        }

        void ScrollToHeight(float height)
        {
            if (m_ScrollContainer != null)
            {
                m_ScrollContainer.verticalScroller.value = height;
            }
        }
    }
}
