using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class ProjectContextMenu: ContextMenu
    {
        readonly IUnityConnectProxy m_UnityConnectProxy;
        readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;
        readonly IPageManager m_PageManager;
        readonly IStateManager m_StateManager;
        readonly IMessageManager m_MessageManager;
        readonly string m_ProjectId;

        VisualElement m_Target;

        public ProjectContextMenu(string projectId, IUnityConnectProxy unityConnectProxy,
            IProjectOrganizationProvider projectOrganizationProvider, IPageManager pageManager,
            IStateManager stateManager, IMessageManager messageManager)
        {
            m_ProjectId = projectId;
            m_UnityConnectProxy = unityConnectProxy;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_PageManager = pageManager;
            m_StateManager = stateManager;
            m_MessageManager = messageManager;
        }

        public override void SetupContextMenuEntries(ContextualMenuPopulateEvent evt)
        {
            // Check the target to avoid adding the same menu entries multiple times add don't know which one is called
            if (evt.target == evt.currentTarget)
            {
                m_Target = (VisualElement)evt.target;
                AddMenuEntry(evt, Constants.CollectionCreate,
                    m_UnityConnectProxy.AreCloudServicesReachable && !string.IsNullOrEmpty(m_ProjectId),
                    (_) =>
                    {
                        CreateCollection();
                    });
            }
        }

        void CreateCollection()
        {
            var name = Constants.CollectionDefaultName;

            var projectInfo = m_ProjectOrganizationProvider.GetProject(m_ProjectId);

            if (projectInfo?.CollectionInfos != null)
            {
                var index = 1;
                while (projectInfo.CollectionInfos.Any(c => c.GetFullPath() == $"{name}"))
                {
                    name = $"{Constants.CollectionDefaultName} ({index++})";
                }
            }

            var newFoldout = new SideBarCollectionFoldout(m_UnityConnectProxy, m_PageManager, m_StateManager,
                m_MessageManager, m_ProjectOrganizationProvider, name, m_ProjectId, string.Empty);
            m_Target.Add(newFoldout);
            newFoldout.StartNaming();
        }
    }
}
