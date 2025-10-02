using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string MultiSelectionPickerTextStyleClass = "multi-selection-picker-text";
        public const string MultiSelectionPickerEntryStyleClass = "multi-selection-picker-entry";
        public const string MultiSelectionPickerLabelStyleClass = "multi-selection-picker-label";
    }

    class MultiSelectionPicker : Foldout
    {
        readonly Dictionary<string, Toggle> m_Selection = new();

        public Dictionary<string, Toggle> Selection => m_Selection;

        public event Action<string, bool> ValueChanged;

        public MultiSelectionPicker(string displayName, List<string> options)
        {
            AddToClassList(UssStyle.MultiSelectionPickerTextStyleClass);

            text = displayName;

            foreach (var option in options)
            {
                var entry = new VisualElement();
                entry.AddToClassList(UssStyle.MultiSelectionPickerEntryStyleClass);

                var label = new Label(option);
                label.AddToClassList(UssStyle.MultiSelectionPickerLabelStyleClass);
                entry.Add(label);

                var toggle = new Toggle();
                toggle.RegisterValueChangedCallback(evt =>
                {
                    toggle.showMixedValue = false;

                    OnValueChanged(option, evt.newValue);
                });
                entry.Add(toggle);

                m_Selection.Add(option, toggle);

                Add(entry);
            }
        }

        void OnValueChanged(string option, bool toggleValue)
        {
            ValueChanged?.Invoke(option, toggleValue);
        }
    }
}
