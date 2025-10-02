using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    sealed class MultiValueTextField : TextField
    {
        public MultiValueTextField(List<string> values)
        {
            this.InitializeFieldAsMultiValue(values);
        }
    }

    sealed class MultiValueDoubleField : DoubleField
    {
        public MultiValueDoubleField(List<double> values)
        {
            this.InitializeFieldAsMultiValue(values);
        }
    }

    sealed class MultiValueToggle : Toggle
    {
        public MultiValueToggle(List<bool> values)
        {
            this.InitializeFieldAsMultiValue(values);

            this.RegisterValueChangedCallback(evt => OnValueChanged());
        }

        void OnValueChanged()
        {
            showMixedValue = false;
        }
    }

    sealed class MultiValueDropdownField : DropdownField
    {
        public MultiValueDropdownField(List<string> values, List<string> choices)
        {
            this.choices = choices;
            this.InitializeFieldAsMultiValue(values);
        }
    }

    sealed class MultiValueTimestampPicker : TimestampPicker
    {
        public MultiValueTimestampPicker(List<DateTime> values)
        {
            if (values == null || !values.Any())
                return;

            SetTimeUsingMultipleTimestamps(values);
        }
    }

    sealed class MultiValueMultiSelectionPicker : MultiSelectionPicker
    {
        public MultiValueMultiSelectionPicker(string displayName, List<List<string>> values, List<string> options)
        : base(displayName, options)
        {
            if (values == null || !values.Any())
                return;

            foreach (var toggle in Selection)
            {
                if (values.TrueForAll(x => x.Contains(toggle.Key)))
                {
                    toggle.Value.value = true;
                }
                else if (!values.Exists(x => x.Contains(toggle.Key)))
                {
                    toggle.Value.value = false;
                }
                else
                {
                    toggle.Value.showMixedValue = true;
                }
            }
        }
    }

    static class BaseFieldExtensions
    {
        public static void InitializeFieldAsMultiValue<T>(this BaseField<T> field, List<T> values)
        {
            if (values == null || !values.Any())
                return;

            if (values.Exists(x => !x.Equals(values.FirstOrDefault())))
            {
                field.showMixedValue = true;
            }
            else
            {
                field.value = values.FirstOrDefault();
            }
        }
    }
}
