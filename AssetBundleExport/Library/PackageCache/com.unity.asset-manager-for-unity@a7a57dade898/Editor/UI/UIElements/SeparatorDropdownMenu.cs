using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class DropdownMenuItem
    {
        public string Text { get; }
        public bool IsSeparator { get; }

        public DropdownMenuItem(string text, bool isSeparator = false)
        {
            Text = text;
            IsSeparator = isSeparator;
        }
    }

    class SeparatorDropdownMenu : VisualElement
    {
        const string k_BaseField = "unity-base-field";
        const string k_BaseFieldNoLabel = k_BaseField + "--no-label";
        const string k_BaseFieldInput = k_BaseField + "__input";
        const string k_BaseFieldLabel = k_BaseField + "__label";

        const string k_BasePopupField = "unity-base-popup-field";
        const string k_BasePopupFieldInput = k_BasePopupField + "__input";
        const string k_BasePopupFieldText = k_BasePopupField + "__text";
        const string k_BasePopupFieldArrow = k_BasePopupField + "__arrow";

        const string k_PopupField = "unity-popup-field";
        const string k_PopupFieldInput = k_PopupField + "__input";

        Button m_DropdownButton;
        VisualElement m_Arrow;
        Label m_ValueLabel;
        GenericDropdownMenu m_GenericDropdownMenu;
        string m_Value;
        bool m_DropdownEnabled;

        string m_Tooltip;

        public List<DropdownMenuItem> choices { get; set; }

        public string value
        {
            get => m_Value;
            set
            {
                if (m_Value != value)
                {
                    m_Value = value;
                    m_ValueLabel.text = value ?? "";
                    OnValueChanged?.Invoke(m_Value);
                }
            }
        }

        public event Action<string> OnValueChanged;

        public SeparatorDropdownMenu(string label = null, List<DropdownMenuItem> choices = null, string tooltip = null)
        {
            this.choices = choices ?? new List<DropdownMenuItem>();
            m_Tooltip = tooltip;

            InitializeUI(label);
        }

        void InitializeUI(string label = null)
        {
            AddToClassList(k_BaseField);
            AddToClassList(k_BasePopupField);
            AddToClassList(k_PopupField);

            if (!string.IsNullOrEmpty(label))
            {
                var labelElement = new Label(label);
                labelElement.AddToClassList(k_BaseFieldLabel);
                Add(labelElement);
            }
            else
            {
                AddToClassList(k_BaseFieldNoLabel);
            }

            m_DropdownButton = new Button();
            m_DropdownButton.ClearClassList();
            m_DropdownButton.AddToClassList(k_BaseFieldInput);
            m_DropdownButton.AddToClassList(k_BasePopupFieldInput);
            m_DropdownButton.AddToClassList(k_PopupFieldInput);
            m_DropdownButton.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);

            m_ValueLabel = new Label();
            m_ValueLabel.ClearClassList();
            m_ValueLabel.AddToClassList(k_BasePopupFieldText);
            m_ValueLabel.displayTooltipWhenElided = false;
            m_DropdownButton.Add(m_ValueLabel);

            m_Arrow = new VisualElement();
            m_Arrow.ClearClassList();
            m_Arrow.AddToClassList(k_BasePopupFieldArrow);
            m_DropdownButton.Add(m_Arrow);

            Add(m_DropdownButton);

            UpdateValueLabel();
            EnableTooltip(true);
            SetDropdownEnabled(true);
        }

        public void SetDropdownEnabled(bool enabled)
        {
            m_Arrow.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
            m_DropdownEnabled = enabled;
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if (!m_DropdownEnabled || evt.button != (int) MouseButton.LeftMouse)
                return;

            ToggleMenu();
            evt.StopImmediatePropagation();
        }

        void OnGlobalPointerDown(PointerDownEvent evt)
        {
            if (m_GenericDropdownMenu != null && !m_GenericDropdownMenu.contentContainer.worldBound.Contains(evt.position))
            {
                HideMenu();
                evt.StopPropagation();
            }
        }

        void ToggleMenu()
        {
            if (IsMenuOpenAndFocused())
                HideMenu();
            else
                ShowMenu();
        }

        void ShowMenu()
        {
            m_GenericDropdownMenu = new GenericDropdownMenu();
            foreach (var item in choices)
            {
                if (item.IsSeparator)
                {
                    m_GenericDropdownMenu.AddSeparator(string.Empty);
                }
                else
                {
                    var captured = item.Text;
                    m_GenericDropdownMenu.AddItem(captured, captured == value, () =>
                    {
                        value = captured;
                    });
                }
            }

            panel?.visualTree.RegisterCallback<PointerDownEvent>(OnGlobalPointerDown, TrickleDown.TrickleDown);

            m_GenericDropdownMenu.DropDown(m_DropdownButton.worldBound, m_DropdownButton, false);
            EnableTooltip(false);
        }

        void HideMenu()
        {
            var hideMethod = typeof(GenericDropdownMenu).GetMethod("Hide", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (hideMethod != null)
            {
                hideMethod.Invoke(m_GenericDropdownMenu, new object[] { false });
                m_GenericDropdownMenu = null;
            }

            panel?.visualTree.UnregisterCallback<PointerDownEvent>(OnGlobalPointerDown, TrickleDown.TrickleDown);
            EnableTooltip(true);
        }

        bool IsMenuOpenAndFocused()
        {
            return m_GenericDropdownMenu != null && m_GenericDropdownMenu.contentContainer != null &&
                m_GenericDropdownMenu.contentContainer.focusController != null &&
                m_GenericDropdownMenu.contentContainer.focusController.focusedElement ==
                m_GenericDropdownMenu.contentContainer;
        }

        void EnableTooltip(bool enable)
        {
            m_DropdownButton.tooltip = enable ? m_Tooltip : null;
        }

        void UpdateValueLabel()
        {
            m_ValueLabel.text = value ?? "";
        }

        public void RegisterValueChangedCallback(Action<string> callback)
        {
            OnValueChanged += callback;
        }

        public void UnregisterValueChangedCallback(Action<string> callback)
        {
            OnValueChanged -= callback;
        }

        public void SetValueWithoutNotify(string newValue)
        {
            m_Value = newValue;
            m_ValueLabel.text = newValue ?? "";
        }
    }
}
