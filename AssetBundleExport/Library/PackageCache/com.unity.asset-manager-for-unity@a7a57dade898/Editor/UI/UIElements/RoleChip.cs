using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class RoleChip : Chip
    {
        readonly IPageManager m_PageManager;
        readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;
        readonly IPermissionsManager m_PermissionsManager;
        readonly ILinksProxy m_LinksProxy;

        bool m_IsShown;

        static readonly string k_DocumentationPage = "org-project-roles";

        internal RoleChip(IPageManager pageManager, IProjectOrganizationProvider projectOrganizationProvider,
            IPermissionsManager permissionsManager, ILinksProxy linksProxy)
            : base(string.Empty)
        {
            m_PageManager = pageManager;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_PermissionsManager = permissionsManager;
            m_LinksProxy = linksProxy;

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        void OnClicked(ClickEvent evt)
        {
            m_LinksProxy.OpenAssetManagerDocumentationPage(k_DocumentationPage);
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            if (m_LinksProxy.CanOpenAssetManagerDocumentation)
            {
                RegisterCallback<ClickEvent>(OnClicked);
                pickingMode = PickingMode.Position;
            }
            else
            {
                pickingMode = PickingMode.Ignore;
            }

            m_PageManager.ActivePageChanged += OnActivePageChanged;
            m_ProjectOrganizationProvider.ProjectSelectionChanged += OnProjectSelectionChanged;
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;

            TaskUtils.TrackException(SetProjectRole(m_ProjectOrganizationProvider.SelectedOrganization?.Id,
                m_ProjectOrganizationProvider.SelectedProject?.Id));
        }

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            UnregisterCallback<ClickEvent>(OnClicked);

            m_PageManager.ActivePageChanged -= OnActivePageChanged;
            m_ProjectOrganizationProvider.ProjectSelectionChanged -= OnProjectSelectionChanged;
            m_ProjectOrganizationProvider.OrganizationChanged -= OnOrganizationChanged;
        }

        async void OnProjectSelectionChanged(ProjectInfo projectInfo, CollectionInfo _)
        {
            if (projectInfo != null)
            {
                await SetProjectRole(m_ProjectOrganizationProvider.SelectedOrganization?.Id, projectInfo.Id);
            }
        }

        void OnOrganizationChanged(OrganizationInfo organization)
        {
            UIElementsUtils.SetDisplay(this, m_IsShown && organization != null && organization.ProjectInfos.Any());
        }

        void OnActivePageChanged(IPage page)
        {
            TaskUtils.TrackException(SetProjectRole(m_ProjectOrganizationProvider.SelectedOrganization?.Id,
                m_ProjectOrganizationProvider.SelectedProject?.Id));
        }

        async Task SetProjectRole(string organizationId, string projectId)
        {
            m_IsShown = false;
            UIElementsUtils.Hide(this);
            if (string.IsNullOrEmpty(projectId))
                return;

            var role = await m_PermissionsManager.GetRoleAsync(organizationId, projectId);
            m_Label.text = role.ToString();

            var isVisible = m_PageManager.ActivePage is CollectionPage or UploadPage;
            m_IsShown = isVisible && role != Role.None;
            UIElementsUtils.SetDisplay(this, m_IsShown);
        }
    }
}
