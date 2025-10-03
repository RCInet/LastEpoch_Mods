using System.Collections.Generic;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class SavedViewControls : GridTool
    {
        const string k_UssClassName = "unity-saved-view-controls";

        ISavedAssetSearchFilterManager m_SavedAssetSearchFilterManager;

        public SavedViewControls(IPageManager pageManager, IProjectOrganizationProvider projectOrganizationProvider,
            ISavedAssetSearchFilterManager savedAssetSearchFilterManager)
            : base(pageManager, projectOrganizationProvider)
        {
            m_SavedAssetSearchFilterManager = savedAssetSearchFilterManager;

            AddToClassList(k_UssClassName);

            var clearButton = new Button(ClearCurrentFilter)
            {
                text = L10n.Tr(Constants.ClearFilter),
                tooltip = L10n.Tr(Constants.ClearFilterTooltip)
            };
            clearButton.focusable = false;
            Add(clearButton);

            var saveButton = new SaveViewButton(m_PageManager, m_SavedAssetSearchFilterManager);
            Add(saveButton);
        }

        void ClearCurrentFilter()
        {
            m_SavedAssetSearchFilterManager.ClearSelectedFilter();
        }

        protected override bool IsDisplayed(IPage page)
        {
            if (page is BasePage basePage)
            {
                return basePage.DisplaySavedViewControls;
            }

            return base.IsDisplayed(page);
        }
    }

    class SaveViewButton : VisualElement
    {
        const string k_UssSaveViewButtonsContainer = "unity-save-view-button-container";
        const string k_UssSaveViewButton = "unity-save-view-button";
        const string k_UssSaveViewDropdownButton = "unity-save-view-dropdown-button";

        IPageManager m_PageManager;
        ISavedAssetSearchFilterManager m_SavedAssetSearchFilterManager;

        Button m_SaveButton;

        public SaveViewButton(IPageManager pageManager, ISavedAssetSearchFilterManager savedAssetSearchFilterManager)
        {
            m_PageManager = pageManager;
            m_SavedAssetSearchFilterManager = savedAssetSearchFilterManager;

            AddToClassList(k_UssSaveViewButtonsContainer);

            CreateSaveButton();
            CreateSaveMoreButton();
        }

        void CreateSaveButton()
        {
            var button = new Button(SaveCurrentFilter)
            {
                text = L10n.Tr(Constants.SaveFilter),
                tooltip = L10n.Tr(Constants.SaveCurrentFilterTooltip)
            };
            button.focusable = false;
            button.AddToClassList(k_UssSaveViewButton);

            Add(button);
        }

        void CreateSaveMoreButton()
        {
            var saveDropdown = new DropdownField
            {
                focusable = false
            };
            UIElementsUtils.Hide(saveDropdown.Q(null, "unity-base-popup-field__text"));
            saveDropdown.AddToClassList(k_UssSaveViewDropdownButton);
            saveDropdown.tooltip = L10n.Tr(Constants.SaveFilterDropdownTooltip);

            var saveCurrentChoice = L10n.Tr(Constants.SaveCurrentFilter);
            var saveAsNewChoice = L10n.Tr(Constants.SaveFilterAsNew);
            saveDropdown.choices = new List<string> { saveCurrentChoice, saveAsNewChoice };

            saveDropdown.RegisterValueChangedCallback(value =>
            {
                if (value.newValue == saveCurrentChoice)
                {
                    SaveCurrentFilter();
                }
                else if (value.newValue == saveAsNewChoice)
                {
                    SaveAsNewFilter();
                }

                saveDropdown.value = null;
            });

            Add(saveDropdown);
        }

        void SaveCurrentFilter()
        {
            var currentFilter = m_SavedAssetSearchFilterManager.SelectedFilter;
            if (currentFilter == null)
            {
                SaveAsNewFilter();
                return;
            }

            var sortingOptions = new SortingOptions(m_PageManager.SortField, m_PageManager.SortingOrder);
            m_SavedAssetSearchFilterManager.UpdateFilter(currentFilter, m_PageManager.PageFilterStrategy.AssetSearchFilter, sortingOptions);
        }

        void SaveAsNewFilter()
        {
            var sortingOptions = new SortingOptions(m_PageManager.SortField, m_PageManager.SortingOrder);
            var newFilterName = SavedFilterUtilities.GetValidFilterName(m_SavedAssetSearchFilterManager.Filters, L10n.Tr("New saved view"));
            m_SavedAssetSearchFilterManager.SaveNewFilter(newFilterName, m_PageManager.PageFilterStrategy.AssetSearchFilter, sortingOptions);
        }
    }
}
