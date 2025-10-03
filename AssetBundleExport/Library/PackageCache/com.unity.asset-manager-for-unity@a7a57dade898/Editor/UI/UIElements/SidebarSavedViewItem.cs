using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class SidebarSavedViewItem : VisualElement
    {
        internal delegate bool IsRenameValidDelegate(string filterId, string filterName);

        SavedAssetSearchFilter m_SavedAssetSearchFilter;

        RadioButton m_RadioButton;
        Label m_Label;
        TextField m_TextField;

        bool m_IsSelected;

        IsRenameValidDelegate m_IsRenameValid;

        public event Action<SavedAssetSearchFilter, string> RenameFilter;
        public event Action<SavedAssetSearchFilter> DeleteFilter;
        public event Action<SidebarSavedViewItem> ItemClicked;

        public string FilterId => m_SavedAssetSearchFilter.FilterId;

        public SidebarSavedViewItem(SavedAssetSearchFilter savedAssetSearchFilter, IsRenameValidDelegate isRenameValid)
        {
            m_SavedAssetSearchFilter = savedAssetSearchFilter;
            m_IsRenameValid = isRenameValid;

            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.AddToClassList("sidebar-item-container");
            Add(container);

            m_RadioButton = new RadioButton
            {
                focusable = false
            };
            m_RadioButton.RegisterValueChangedCallback(OnRadioButtonValueChanged);

            container.Add(m_RadioButton);

            m_Label = new Label(m_SavedAssetSearchFilter.FilterName);
            container.Add(m_Label);

            m_TextField = new TextField();
            m_TextField.selectAllOnFocus = false;
            m_TextField.selectAllOnMouseUp = false;
            m_TextField.AddToClassList("sidebar-text-field");
            container.Add(m_TextField);
            UIElementsUtils.Hide(m_TextField);

            var contextMenu = new SavedViewContextMenu(this);
            var contextualMenuManipulator = new ContextualMenuManipulator(contextMenu.SetupContextMenuEntries);
            this.AddManipulator(contextualMenuManipulator);

            RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
        }

        public void SetSelected(bool isSelected)
        {
            m_IsSelected = isSelected;
            m_RadioButton.SetValueWithoutNotify(isSelected);
        }

        public void StartRenaming()
        {
            UIElementsUtils.Hide(m_Label);
            UIElementsUtils.Show(m_TextField);
            m_TextField.value = m_Label.text;
            m_TextField.Focus();
            m_TextField.SelectAll();

            m_TextField.RegisterCallback<FocusOutEvent>(Rename);
        }

        void Rename(FocusOutEvent evt)
        {
            UIElementsUtils.Hide(m_TextField);
            UIElementsUtils.Show(m_Label);
            m_TextField.UnregisterCallback<FocusOutEvent>(Rename);

            var filterName = m_TextField.value.Trim();

            if (m_Label.text == filterName)
                return;

            if (string.IsNullOrWhiteSpace(m_TextField.value))
                return;

            if (!m_IsRenameValid(m_SavedAssetSearchFilter.FilterId, filterName))
            {
                Debug.LogWarning($"Saved view name already exists: {filterName}");
                return;
            }

            m_Label.text = filterName;
            RenameFilter?.Invoke(m_SavedAssetSearchFilter, filterName);
        }

        public void Delete()
        {
            DeleteFilter?.Invoke(m_SavedAssetSearchFilter);
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
                return;

            ItemClicked?.Invoke(this);
        }

        void OnRadioButtonValueChanged(ChangeEvent<bool> evt)
        {
            // The radio button itself is visual, the selection logic is controlled by this entire element's
            // pointer-down event. Thus maintain the value of the radio button to the selection state of this item
            SetSelected(m_IsSelected);
        }
    }
}
