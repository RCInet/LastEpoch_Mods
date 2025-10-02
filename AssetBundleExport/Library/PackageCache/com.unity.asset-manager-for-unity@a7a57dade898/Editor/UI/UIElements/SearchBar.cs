using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class SearchBar : VisualElement
    {
        const string k_SearchTerms = "Search";
        const string k_SearchBarAssetName = "SearchBar";
        const string k_SearchCancelUssName = "search-clear";
        const string k_SearchFieldElementName = "inputSearch";
        const string k_PlaceholderClass = k_SearchBarAssetName + "__placeholder";
        const int k_MaxLength = 1024;

        readonly IPageManager m_PageManager;
        readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;
        readonly IMessageManager m_MessageManager;

        readonly VisualElement m_TextInput;

        bool m_Focused;

        Button m_ClearAllButton;
        Button m_RefreshButton;
        VisualElement m_SearchChipsContainer;
        TextField m_SearchTextField;
        ToolbarSearchField m_ToolbarSearchField;

        IPageFilterStrategy PageFilterStrategy => m_PageManager?.PageFilterStrategy;

        public SearchBar(IPageManager pageManager, IProjectOrganizationProvider projectOrganizationProvider,
            IMessageManager messageManager)
        {
            m_PageManager = pageManager;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_MessageManager = messageManager;

            var windowContent = UIElementsUtils.LoadUXML(k_SearchBarAssetName);
            windowContent.CloneTree(this);

            m_ToolbarSearchField = UIElementsUtils.SetupToolbarSearchField(k_SearchFieldElementName, OnSearchbarValueChanged, this);
            m_SearchTextField = m_ToolbarSearchField.Q<TextField>();
            m_SearchTextField.maxLength = k_MaxLength;
            m_SearchTextField.isDelayed = true;

            m_ClearAllButton = m_ToolbarSearchField.Q<Button>("unity-cancel");
            if (m_ClearAllButton != null)
            {
                m_ClearAllButton.name = k_SearchCancelUssName;
            }

            m_SearchChipsContainer = new VisualElement();
            m_SearchChipsContainer.AddToClassList("search-chip-container");
            m_ToolbarSearchField.Insert(1, m_SearchChipsContainer);

            m_TextInput = m_ToolbarSearchField.Q<TextField>().Q("unity-text-input");

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);

            m_SearchTextField.AddToClassList(k_PlaceholderClass);
            ShowSearchTermsTextIfNeeded();
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            m_SearchTextField.RegisterCallback<KeyDownEvent>(OnKeyDown);
            m_SearchTextField.RegisterCallback<FocusOutEvent>(OnFocusOut);
            m_SearchTextField.RegisterCallback<FocusInEvent>(OnFocusIn);

            if (m_ClearAllButton != null)
            {
                m_ClearAllButton.clicked += OnSearchCancelClick;
            }

            m_PageManager.ActivePageChanged += OnActivePageChanged;
            m_PageManager.SearchFiltersChanged += OnPageSearchFiltersChanged;
            m_PageManager.LoadingStatusChanged += OnPageManagerLoadingStatusChanged;
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;
            m_MessageManager.GridViewMessageSet += OnGridViewMessageSet;
            m_MessageManager.GridViewMessageCleared += OnGridViewMessageCleared;

            var activePage = m_PageManager.ActivePage;
            Refresh(activePage);
            InitDisplay(activePage);
        }

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            m_SearchTextField.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            m_SearchTextField.UnregisterCallback<FocusOutEvent>(OnFocusOut);
            m_SearchTextField.UnregisterCallback<FocusInEvent>(OnFocusIn);

            if (m_ClearAllButton != null)
            {
                m_ClearAllButton.clicked -= OnSearchCancelClick;
            }

            m_PageManager.ActivePageChanged -= OnActivePageChanged;
            m_PageManager.SearchFiltersChanged -= OnPageSearchFiltersChanged;
            m_PageManager.LoadingStatusChanged -= OnPageManagerLoadingStatusChanged;
            m_ProjectOrganizationProvider.OrganizationChanged -= OnOrganizationChanged;
            m_MessageManager.GridViewMessageSet -= OnGridViewMessageSet;
            m_MessageManager.GridViewMessageCleared -= OnGridViewMessageCleared;
        }

        void OnPageSearchFiltersChanged(IPage page, IEnumerable<string> searchFilters)
        {
            if (m_PageManager.IsActivePage(page))
            {
                Refresh(page);
            }
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

        void OnActivePageChanged(IPage page)
        {
            Refresh(page);
            InitDisplay(page);
        }

        void OnFocusIn(FocusInEvent evt)
        {
            m_Focused = true;
            if (m_SearchTextField.text == k_SearchTerms)
            {
                m_SearchTextField.SetValueWithoutNotify("");
                m_SearchTextField.RemoveFromClassList(k_PlaceholderClass);
            }
        }

        void OnFocusOut(FocusOutEvent evt)
        {
            m_Focused = false;
            if (m_SearchChipsContainer.childCount == 0 && string.IsNullOrWhiteSpace(m_SearchTextField.text))
            {
                m_SearchTextField.AddToClassList(k_PlaceholderClass);
                ShowSearchTermsTextIfNeeded();
            }
        }

        void OnGridViewMessageSet(Message _)
        {
            Refresh(m_PageManager.ActivePage);
        }

        void OnGridViewMessageCleared()
        {
            Refresh(m_PageManager.ActivePage);
        }

        void InitDisplay(IPage page)
        {
            var display = m_ProjectOrganizationProvider.SelectedOrganization != null;
            UIElementsUtils.SetDisplay(m_RefreshButton,  display);
            UIElementsUtils.SetDisplay(m_ToolbarSearchField, display);
        }

        void Refresh(IPage page)
        {
            if (page == null)
                return;

            m_SearchChipsContainer.Clear();
            m_ToolbarSearchField.SetValueWithoutNotify(string.Empty);
            foreach (var filter in PageFilterStrategy.SearchFilters)
            {
                m_SearchChipsContainer.Add(new SearchFilterChip(filter, DismissSearchFilter));
            }

            ShowSearchTermsTextIfNeeded();
            m_ClearAllButton.visible = PageFilterStrategy.SearchFilters.Any();
        }

        void OnSearchCancelClick()
        {
            PageFilterStrategy.ClearSearchFilters();
            ShowSearchTermsTextIfNeeded();
        }

        void OnSearchbarValueChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == k_SearchTerms)
                return;

            var searchFilters = evt.newValue.Split(" ").Where(s => !string.IsNullOrEmpty(s));
            if (searchFilters.Any())
            {
                AnalyticsSender.SendEvent(new SearchAttemptEvent(searchFilters.Count()));
                PageFilterStrategy.AddSearchFilter(searchFilters);
            }

            if (m_Focused)
            {
                SetKeyboardFocusOnSearchField();
            }
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Escape:
                    m_ToolbarSearchField.value = "";
                    PageFilterStrategy.ClearSearchFilters();
                    SetKeyboardFocusOnSearchField();
                    return;
                case KeyCode.Backspace when string.IsNullOrWhiteSpace(m_SearchTextField.text):
                    PopSearchChip();
                    SetKeyboardFocusOnSearchField();
                    break;
            }
        }

        void SetKeyboardFocusOnSearchField()
        {
            m_TextInput.Focus();
        }

        void PopSearchChip()
        {
            if (!PageFilterStrategy.SearchFilters.Any())
                return;

            PageFilterStrategy.RemoveSearchFilter(PageFilterStrategy.SearchFilters.Last());
            if (!PageFilterStrategy.SearchFilters.Any())
            {
                ShowSearchTermsTextIfNeeded();
            }
        }

        void DismissSearchFilter(string searchFilter)
        {
            if (!PageFilterStrategy.SearchFilters.Contains(searchFilter))
                return;

            PageFilterStrategy.RemoveSearchFilter(searchFilter);
            SetKeyboardFocusOnSearchField();
        }

        void ShowSearchTermsTextIfNeeded()
        {
            if (m_SearchChipsContainer.childCount == 0 && !m_Focused)
            {
                m_SearchTextField.SetValueWithoutNotify(k_SearchTerms);
            }
        }

        void OnOrganizationChanged(OrganizationInfo organizationInfo)
        {
            Refresh(m_PageManager.ActivePage);
        }
    }
}
