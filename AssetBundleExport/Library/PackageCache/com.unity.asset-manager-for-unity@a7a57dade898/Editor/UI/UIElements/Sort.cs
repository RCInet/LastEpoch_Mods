using System;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Unity.AssetManager.UI.Editor
{
    class Sort : GridTool
    {
        const string k_UssClassName = "unity-sort";
        const string k_DropdownUssClassName = k_UssClassName + "-dropdown";
        const string k_OrderUssClassName = k_UssClassName + "-order";
        const string k_AscendingUssClassName = k_OrderUssClassName + "--ascending";
        const string k_DescendingUssClassName = k_OrderUssClassName + "--descending";

        readonly DropdownField m_DropdownField;
        readonly Button m_OrderButton;
        readonly ISavedAssetSearchFilterManager m_SavedAssetSearchFilterManager;

        public Sort(IPageManager pageManager, IProjectOrganizationProvider projectOrganizationProvider, ISavedAssetSearchFilterManager savedAssetSearchFilterManager)
            : base(pageManager, projectOrganizationProvider)
        {
            m_SavedAssetSearchFilterManager = savedAssetSearchFilterManager;
            m_SavedAssetSearchFilterManager.FilterSelected += OnFilterSelected;

            AddToClassList(k_UssClassName);

            m_DropdownField = new DropdownField(L10n.Tr(Constants.Sort));
            if (m_PageManager.ActivePage != null)
            {
                SetupSortField(m_PageManager.ActivePage);
            }
            m_DropdownField.AddToClassList(k_DropdownUssClassName);

            m_DropdownField.RegisterValueChangedCallback(ev =>
            {
                OnValueChanged(m_PageManager.ActivePage.SortOptions[ev.newValue], m_PageManager.SortingOrder);
            });
            Add(m_DropdownField);

            m_OrderButton = new Button();
            m_OrderButton.AddToClassList(k_OrderUssClassName);
            SetupOrderButtonField();
            m_OrderButton.clicked += () =>
            {
                var sortingOrder = m_PageManager.SortingOrder == SortingOrder.Ascending ? SortingOrder.Descending : SortingOrder.Ascending;
                m_OrderButton.RemoveFromClassList(sortingOrder == SortingOrder.Ascending ? k_DescendingUssClassName : k_AscendingUssClassName);
                m_OrderButton.AddToClassList(sortingOrder == SortingOrder.Ascending ? k_AscendingUssClassName : k_DescendingUssClassName);
                OnValueChanged(m_PageManager.ActivePage.SortOptions[m_DropdownField.value], sortingOrder);
            };
            Add(m_OrderButton);
        }

        void OnValueChanged(SortField sortField, SortingOrder sortingOrder)
        {
            m_PageManager.SetSortValues(sortField, sortingOrder);

            AnalyticsSender.SendEvent(new SortEvent(sortField.ToString(), sortingOrder == SortingOrder.Ascending));
        }

        protected override void OnActivePageChanged(IPage page)
        {
            base.OnActivePageChanged(page);
            SetupSortField(page);
            SetupOrderButtonField();
        }

        protected override bool IsDisplayed(IPage page)
        {
            if (page is BasePage basePage)
            {
                return basePage.DisplaySort;
            }

            return base.IsDisplayed(page);
        }

        void SetupSortField(IPage page)
        {
            m_DropdownField.choices = page.SortOptions.Keys.ToList();
            var sortField = m_PageManager.SortField;
            var sortFieldValue = BasePage.s_SortFieldsLabelMap[sortField];

            m_DropdownField.SetValueWithoutNotify(sortFieldValue);
        }

        void SetupOrderButtonField()
        {
            var addToClass = m_PageManager.SortingOrder == SortingOrder.Ascending
                ? k_AscendingUssClassName
                : k_DescendingUssClassName;

            m_OrderButton.RemoveFromClassList(k_AscendingUssClassName);
            m_OrderButton.RemoveFromClassList(k_DescendingUssClassName);
            m_OrderButton.AddToClassList(addToClass);
        }

        void OnFilterSelected(SavedAssetSearchFilter filter, bool applyFilter)
        {
            if (filter == null)
                return;

            if (applyFilter)
            {
                var sortFieldLabel = BasePage.s_SortFieldsLabelMap[filter.SortingOptions.SortField];
                m_DropdownField.SetValueWithoutNotify(sortFieldLabel);

                m_OrderButton.RemoveFromClassList(k_AscendingUssClassName);
                m_OrderButton.RemoveFromClassList(k_DescendingUssClassName);
                m_OrderButton.AddToClassList(filter.SortingOptions.SortingOrder == SortingOrder.Ascending ? k_AscendingUssClassName : k_DescendingUssClassName);

                OnValueChanged(filter.SortingOptions.SortField, filter.SortingOptions.SortingOrder);
            }
        }
    }
}
