using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class GridTool : VisualElement
    {
        protected readonly IPageManager m_PageManager;
        protected readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;

        protected virtual VisualElement Container => this;

        internal GridTool(IPageManager pageManager, IProjectOrganizationProvider projectOrganizationProvider)
        {
            m_PageManager = pageManager;
            m_ProjectOrganizationProvider = projectOrganizationProvider;

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        protected virtual void OnAttachToPanel(AttachToPanelEvent evt)
        {
            m_PageManager.ActivePageChanged += OnActivePageChanged;
            m_PageManager.LoadingStatusChanged += OnPageManagerLoadingStatusChanged;
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;

            InitDisplay(m_PageManager.ActivePage);
        }

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            m_PageManager.ActivePageChanged -= OnActivePageChanged;
            m_PageManager.LoadingStatusChanged -= OnPageManagerLoadingStatusChanged;
            m_ProjectOrganizationProvider.OrganizationChanged -= OnOrganizationChanged;
        }

        protected virtual void OnActivePageChanged(IPage page)
        {
            InitDisplay(page);
        }

        protected virtual void InitDisplay(IPage page)
        {
            UIElementsUtils.SetDisplay(Container, IsDisplayed(page) && m_ProjectOrganizationProvider.SelectedOrganization != null);
        }

        void OnPageManagerLoadingStatusChanged(IPage page, bool isLoading)
        {
            if (!m_PageManager.IsActivePage(page))
                return;

            if (!isLoading)
            {
                InitDisplay(page);
            }
        }

        void OnOrganizationChanged(OrganizationInfo organization)
        {
            UIElementsUtils.SetDisplay(Container, organization != null && organization.ProjectInfos.Any() && IsDisplayed(m_PageManager.ActivePage));
        }

        protected virtual bool IsDisplayed(IPage page)
        {
            return true;
        }
    }
}
