using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class SidebarProjectContent : Foldout
    {
        readonly IPageManager m_PageManager;
        readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;
        readonly IStateManager m_StateManager;
        readonly IMessageManager m_MessageManager;
        readonly IUnityConnectProxy m_UnityConnectProxy;

        readonly Dictionary<string, SideBarCollectionFoldout> m_SideBarProjectFoldouts = new ();

        VisualElement m_NoProjectSelectedContainer;

        public VisualElement NoProjectSelectedContainer => m_NoProjectSelectedContainer;

        public SidebarProjectContent(IUnityConnectProxy unityConnectProxy, IProjectOrganizationProvider projectOrganizationProvider, IPageManager pageManager,
            IStateManager stateManager, IMessageManager messageManager)
        {
            var toggle = this.Q<Toggle>();
            toggle.text = Constants.SidebarProjectsText;
            toggle.AddToClassList("SidebarContentTitle");

            m_UnityConnectProxy = unityConnectProxy;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_PageManager = pageManager;
            m_StateManager = stateManager;
            m_MessageManager = messageManager;

            InitializeLayout();
        }

        void InitializeLayout()
        {
            m_NoProjectSelectedContainer = new VisualElement();
            m_NoProjectSelectedContainer.AddToClassList("NoProjectSelected");
            m_NoProjectSelectedContainer.Add(new Label {text = L10n.Tr("No project selected")});

            Add(m_NoProjectSelectedContainer);
        }

        public void ProjectInfoChanged(ProjectInfo projectInfo)
        {
            TryAddCollections(projectInfo);
        }

        public void Refresh()
        {
            if (m_ProjectOrganizationProvider.SelectedOrganization != null)
            {
                var orderedProjectInfos =
                    m_ProjectOrganizationProvider.SelectedOrganization.ProjectInfos.OrderBy(p => p.Name);
                RebuildProjectList(orderedProjectInfos.ToList());
            }
            else
            {
                RebuildProjectList(null);
            }
        }

        void RebuildProjectList(List<ProjectInfo> projectInfos)
        {
            foreach (var foldout in m_SideBarProjectFoldouts.Values)
                Remove(foldout);

            m_SideBarProjectFoldouts.Clear();

            if (projectInfos?.Any() != true)
                return;

            foreach (var projectInfo in projectInfos)
            {
                var projectFoldout = new SideBarCollectionFoldout(m_UnityConnectProxy, m_PageManager,
                    m_StateManager, m_MessageManager, m_ProjectOrganizationProvider, projectInfo.Name,
                    projectInfo.Id, null);
                Add(projectFoldout);
                m_SideBarProjectFoldouts[projectInfo.Id] = projectFoldout;

                TryAddCollections(projectInfo);
            }
        }

        void TryAddCollections(ProjectInfo projectInfo)
        {
            // Clean up any existing collection foldouts for the project
            if (!m_SideBarProjectFoldouts.TryGetValue(projectInfo.Id, out var projectFoldout))
                return;

            projectFoldout.Clear();
            projectFoldout.ChangeBackToChildlessFolder();

            if (projectInfo.CollectionInfos == null)
                return;

            if (!projectInfo.CollectionInfos.Any())
                return;

            projectFoldout.value = false;
            var orderedCollectionInfos = projectInfo.CollectionInfos.OrderBy(c => c.GetFullPath());
            foreach (var collection in orderedCollectionInfos)
            {
                CreateFoldoutForParentsThenItself(collection, projectInfo, projectFoldout);
            }
        }

        void CreateFoldoutForParentsThenItself(CollectionInfo collectionInfo, ProjectInfo projectInfo,
            SideBarFoldout projectFoldout)
        {
            if (GetCollectionFoldout(projectInfo, collectionInfo.GetFullPath()) != null)
                return;

            var collectionFoldout =
                CreateSideBarCollectionFoldout(collectionInfo.Name, projectInfo, collectionInfo.GetFullPath());

            SideBarFoldout parentFoldout = null;
            if (!string.IsNullOrEmpty(collectionInfo.ParentPath))
            {
                var parentCollection = projectInfo.GetCollection(collectionInfo.ParentPath);
                CreateFoldoutForParentsThenItself(parentCollection, projectInfo, projectFoldout);

                parentFoldout = GetCollectionFoldout(projectInfo, parentCollection.GetFullPath());
                Utilities.DevAssert(parentFoldout != null);
            }

            var immediateParent = parentFoldout ?? projectFoldout;
            immediateParent.AddFoldout(collectionFoldout);
        }

        SideBarCollectionFoldout CreateSideBarCollectionFoldout(string foldoutName, ProjectInfo projectInfo,
            string collectionPath)
        {
            return new SideBarCollectionFoldout(m_UnityConnectProxy, m_PageManager, m_StateManager,
                m_MessageManager, m_ProjectOrganizationProvider,
                foldoutName, projectInfo.Id, collectionPath);
        }

        SideBarFoldout GetCollectionFoldout(ProjectInfo projectInfo, string collectionPath)
        {
            var collectionId = SideBarCollectionFoldout.GetCollectionId(projectInfo.Id, collectionPath);
            return this.Q<SideBarFoldout>(collectionId);
        }
    }
}
