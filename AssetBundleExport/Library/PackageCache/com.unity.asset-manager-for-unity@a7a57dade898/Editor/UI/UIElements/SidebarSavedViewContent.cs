using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class SidebarSavedViewContent : Foldout
    {
        readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;
        readonly IPageManager m_PageManager;
        readonly ISavedAssetSearchFilterManager m_SavedSearchFilterManager;

        Dictionary<string, SidebarSavedViewItem> m_SidebarSavedViewItems = new ();

        Button m_SaveCurrentFilterButton;
        Toggle m_SavedViewsToggle;

        public SidebarSavedViewContent(IProjectOrganizationProvider projectOrganizationProvider, IPageManager pageManager,
            ISavedAssetSearchFilterManager savedSearchFilterManager)
        {
            m_SavedViewsToggle = this.Q<Toggle>();
            m_SavedViewsToggle.text = Constants.SidebarSavedViewsText;
            m_SavedViewsToggle.AddToClassList("SidebarContentTitle");

            m_SaveCurrentFilterButton = new Button(OnSaveCurrentFilterClicked)
            {
                text = L10n.Tr("Save Current Filter"),
                name = "SaveCurrentFilterButton"
            };
            Add(m_SaveCurrentFilterButton);
            UIElementsUtils.Hide(m_SaveCurrentFilterButton);

            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;

            m_PageManager = pageManager;
            m_PageManager.ActivePageChanged += OnActivePageChanged;
            OnActivePageChanged(m_PageManager.ActivePage);

            m_SavedSearchFilterManager = savedSearchFilterManager;
            m_SavedSearchFilterManager.FilterSelected += OnFilterSelected;
            m_SavedSearchFilterManager.FilterAdded += OnFilterAdded;
            m_SavedSearchFilterManager.FilterDeleted += OnFilterDeleted;

            RebuildSavedViewItemsList();
        }

        void OnOrganizationChanged(OrganizationInfo _)
        {
            RebuildSavedViewItemsList();
        }

        void OnActivePageChanged(IPage page)
        {
            var displayed = page is BasePage {DisplaySavedViewControls: true} basePage;
            UIElementsUtils.SetDisplay(this, displayed);
        }

        void OnFilterSelected(SavedAssetSearchFilter filter, bool _)
        {
            foreach (var savedViewItem in m_SidebarSavedViewItems.Values)
                savedViewItem.SetSelected(savedViewItem.FilterId == filter?.FilterId);

            m_SavedViewsToggle.value = true;
        }

        void OnFilterAdded(SavedAssetSearchFilter filter)
        {
            RebuildSavedViewItemsList();

            // Enter rename mode for the newly added filter
            if (m_SidebarSavedViewItems.TryGetValue(filter.FilterId, out var sidebarSavedViewItem))
                sidebarSavedViewItem.StartRenaming();

            m_SavedViewsToggle.value = true;
        }

        void OnFilterDeleted(SavedAssetSearchFilter _)
        {
            RebuildSavedViewItemsList();
        }

        void RebuildSavedViewItemsList()
        {
            foreach (var item in m_SidebarSavedViewItems.Values)
            {
                item.ItemClicked -= OnItemClicked;
                item.RenameFilter -= OnItemRename;
                item.DeleteFilter -= OnItemDelete;
                Remove(item);
            }

            m_SidebarSavedViewItems.Clear();

            var searchFilters = m_SavedSearchFilterManager.Filters.Where(f => f.OrganizationId == m_ProjectOrganizationProvider.SelectedOrganization?.Id);

            if (!searchFilters.Any())
                UIElementsUtils.Show(m_SaveCurrentFilterButton);
            else
                UIElementsUtils.Hide(m_SaveCurrentFilterButton);

            foreach (var savedAssetSearchFilter in searchFilters)
            {
                var sidebarSavedViewItem = new SidebarSavedViewItem(savedAssetSearchFilter, IsRenameValid);
                sidebarSavedViewItem.ItemClicked += OnItemClicked;
                sidebarSavedViewItem.RenameFilter += OnItemRename;
                sidebarSavedViewItem.DeleteFilter += OnItemDelete;

                Utilities.DevLog("Filter added to sidebar: " + savedAssetSearchFilter.FilterId + " - " + savedAssetSearchFilter.FilterName);

                if (!m_SidebarSavedViewItems.TryAdd(savedAssetSearchFilter.FilterId, sidebarSavedViewItem))
                    Utilities.DevLogError("Duplicate filter ID found in sidebar: " + savedAssetSearchFilter.FilterId);

                sidebarSavedViewItem.SetSelected(sidebarSavedViewItem.FilterId == m_SavedSearchFilterManager.SelectedFilter?.FilterId);
                Add(sidebarSavedViewItem);
            }
        }

        bool IsRenameValid(string targetFilterId, string newName)
        {
            var any = m_SavedSearchFilterManager.Filters.Any(f => f.FilterName == newName && f.FilterId != targetFilterId);
            return !any;
        }

        void OnItemRename(SavedAssetSearchFilter savedAssetSearchFilter, string newName)
        {
            if (IsRenameValid(savedAssetSearchFilter.FilterId, newName))
            {
                savedAssetSearchFilter.SetFilterName(newName);
                m_SavedSearchFilterManager.RenameFilter(savedAssetSearchFilter, newName, false);
            }
        }

        void OnItemDelete(SavedAssetSearchFilter savedAssetSearchFilter)
        {
            m_SavedSearchFilterManager.DeleteFilter(savedAssetSearchFilter);
        }

        void OnItemClicked(SidebarSavedViewItem item)
        {
            var filterId = item.FilterId;
            if (m_SavedSearchFilterManager.SelectedFilter?.FilterId == filterId)
            {
                m_SavedSearchFilterManager.ClearSelectedFilter();
                item.SetSelected(false);
            }
            else
            {
                m_SavedSearchFilterManager.SelectFilter(item.FilterId);
                item.SetSelected(true);
            }
        }

        void OnSaveCurrentFilterClicked()
        {
            var sortingOptions = new SortingOptions(m_PageManager.SortField, m_PageManager.SortingOrder);
            m_SavedSearchFilterManager.SaveNewFilter("New saved view", m_PageManager.PageFilterStrategy.AssetSearchFilter, sortingOptions);
        }
    }
}
