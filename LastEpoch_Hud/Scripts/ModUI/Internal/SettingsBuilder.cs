using System;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Binds prefab UI elements to settings by naming convention.
    // Toggles/Buttons/Dropdowns: AddListener. Sliders: Harmony hook (SliderHook).
    public class SettingsBuilder
    {
        private readonly GameObject panel;
        private readonly string prefix;

        public int BindCount { get; private set; }

        public SettingsBuilder(GameObject panel, string prefix)
        {
            this.panel = panel;
            this.prefix = prefix;
        }

        public void Bind(string panelName, BoolSetting setting)
        {
            var toggle = Prefab.Component<Toggle>(panel, panelName, "Toggle_" + prefix + panelName);
            if (toggle == null) return;
            toggle.isOn = setting.Value;
            Prefab.BindToggle(toggle, new Action<bool>(v => setting.Set(v)));
            BindCount++;
        }

        public void Bind(string panelName, RangeSetting setting)
        {
            BindToggle(panelName, v => setting.SetEnabled(v), setting.Enabled);

            var text = Prefab.FindText(panel, panelName, "Toggle_" + prefix + panelName);
            var sMin = Prefab.Component<Slider>(panel, panelName, "Slider_" + prefix + panelName + "_Min");
            var sMax = Prefab.Component<Slider>(panel, panelName, "Slider_" + prefix + panelName + "_Max");
            if (sMin == null && sMax == null) return;

            ConfigureSlider(sMin, setting.MinLimit, setting.MaxLimit, setting.Min);
            ConfigureSlider(sMax, setting.MinLimit, setting.MaxLimit, setting.Max);

            if (sMin != null)
            {
                SliderHook.Register(sMin.name, v =>
                {
                    setting.SetMin(v);
                    if (sMax != null && sMax.value < setting.Min) sMax.value = setting.Min;
                    UpdateText(text, setting.FormatRange());
                });
            }
            if (sMax != null)
            {
                SliderHook.Register(sMax.name, v =>
                {
                    setting.SetMax(v);
                    if (sMin != null && sMin.value > setting.Max) sMin.value = setting.Max;
                    UpdateText(text, setting.FormatRange());
                });
            }
            UpdateText(text, setting.FormatRange());
            BindCount++;
        }

        public void Bind(string panelName, FloatSetting setting)
        {
            BindToggle(panelName, v => setting.SetEnabled(v), setting.Enabled);

            var text = Prefab.FindText(panel, panelName, "Toggle_" + prefix + panelName);
            var slider = Prefab.Component<Slider>(panel, panelName, "Slider_" + prefix + panelName);
            if (slider == null) return;

            float maxLimit = slider.maxValue;
            slider.value = setting.Value;
            SliderHook.Register(slider.name, v =>
            {
                setting.SetValue(v);
                UpdateText(text, setting.FormatValue(maxLimit));
            });
            UpdateText(text, setting.FormatValue(maxLimit));
            BindCount++;
        }

        public void BindRadio(string groupPanel, RadioSetting setting)
        {
            var group = Prefab.Child(panel, groupPanel);
            if (group == null) return;

            for (int i = 0; i < setting.OptionNames.Length; i++)
            {
                int idx = i;
                string name = setting.OptionNames[idx];
                var toggle = Prefab.Component<Toggle>(group, name, "Toggle_" + prefix + name);
                if (toggle == null) continue;
                toggle.isOn = setting.IsSelected(idx);
                Prefab.BindToggle(toggle, new Action<bool>(v => setting.Select(idx, v)));
            }
            BindCount++;
        }

        public void BindDropdown(string panelName, DropdownSetting setting)
        {
            var dropdown = Prefab.Component<Dropdown>(panel, panelName, "Dropdown_" + prefix + panelName);
            if (dropdown == null) return;

            if (setting.Options.Length > 0)
                ApplyDropdownOptions(dropdown, setting.Options);

            dropdown.value = setting.Value;
            Prefab.BindDropdown(dropdown, new Action<int>(v => setting.Set(v)));
            setting.applyOptions = opts => ApplyDropdownOptions(dropdown, opts);
            BindCount++;
        }

        public void BindButton(string key, ActionBinding binding)
        {
            var btnObj = Prefab.Child(panel, "Btn_" + prefix + key);
            if (btnObj == null) return;
            var btn = btnObj.GetComponent<Button>();
            if (btn == null) return;
            Prefab.BindButton(btn, new Action(() => binding.Invoke()));
            BindCount++;
        }

        // Helpers

        private void BindToggle(string panelName, Action<bool> setter, bool initialValue)
        {
            var toggle = Prefab.Component<Toggle>(panel, panelName, "Toggle_" + prefix + panelName);
            if (toggle == null) return;
            toggle.isOn = initialValue;
            Prefab.BindToggle(toggle, new Action<bool>(v => setter(v)));
        }

        private static void ConfigureSlider(Slider slider, float min, float max, float value)
        {
            if (slider == null) return;
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;
        }

        private static void ApplyDropdownOptions(Dropdown dropdown, string[] options)
        {
            dropdown.ClearOptions();
            var list = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
            foreach (var opt in options)
                list.Add(new Dropdown.OptionData { text = opt });
            dropdown.options = list;
        }

        private static void UpdateText(Text text, string value)
        {
            if (text != null) text.text = value;
        }
    }
}
