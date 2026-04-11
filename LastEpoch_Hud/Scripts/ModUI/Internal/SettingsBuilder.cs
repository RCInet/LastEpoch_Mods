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

        public void Bind(string panelName, BoolSetting setting, string label = null)
        {
            var toggle = Prefab.Component<Toggle>(panel, panelName, "Toggle_" + prefix + panelName);
            if (toggle == null) return;
            toggle.isOn = setting.Value;
            string name = "Toggle_" + prefix + panelName;
            int toggleId = toggle.GetInstanceID();
            ApplyLabel(panelName, name, label);
            Prefab.BindToggle(toggle, new Action<bool>(v =>
            {
                bool actual = toggle.isOn;
                ModSettings.Trace("Toggle listener fired: name=" + name + " v=" + v + " toggle.isOn=" + actual + " setting.Value=" + setting.Value + " id=" + toggleId);
                setting.Set(actual);
            }));
            ModSettings.Trace("Bind(Bool) " + name + " subscribed, isOn=" + toggle.isOn + " id=" + toggleId);
            BindCount++;
        }

        public void Bind(string panelName, RangeSetting setting, string label = null)
        {
            BindToggle(panelName, v => setting.SetEnabled(v), setting.Enabled, label);

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

        public void Bind(string panelName, FloatSetting setting, string label = null)
        {
            BindToggle(panelName, v => setting.SetEnabled(v), setting.Enabled, label);

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
                string wname = "Toggle_" + prefix + name;
                Prefab.BindToggle(toggle, new Action<bool>(v =>
                {
                    bool actual = toggle.isOn;
                    ModSettings.Trace("Radio listener fired: name=" + wname + " idx=" + idx + " v=" + v + " toggle.isOn=" + actual);
                    setting.Select(idx, actual);
                }));
            }
            ModSettings.Trace("BindRadio " + groupPanel + " subscribed " + setting.OptionNames.Length + " options");
            BindCount++;
        }

        public void BindDropdown(string panelName, DropdownSetting setting, string label = null)
        {
            var dropdown = Prefab.Component<Dropdown>(panel, panelName, "Dropdown_" + prefix + panelName);
            if (dropdown == null) return;

            if (setting.Options.Length > 0)
                ApplyDropdownOptions(dropdown, setting.Options);

            dropdown.value = setting.Value;
            string name = "Dropdown_" + prefix + panelName;

            ApplyLabel(panelName, name, label);

            Prefab.BindDropdown(dropdown, new Action<int>(v =>
            {
                ModSettings.Trace("Dropdown listener fired: name=" + name + " v=" + v);
                setting.Set(v);
            }));
            setting.applyOptions = opts => ApplyDropdownOptions(dropdown, opts);
            ModSettings.Trace("BindDropdown " + name + " subscribed, value=" + dropdown.value + " options=" + setting.Options.Length);
            BindCount++;
        }

        public void BindHeader(string panelName, HeaderSetting setting, string label)
        {
            var obj = Prefab.Child(panel, panelName);
            if (obj == null) return;
            var text = obj.GetComponent<Text>();
            if (text == null) text = obj.GetComponentInChildren<Text>();
            if (text == null) return;
            Prefab.ApplyLabel(text, label ?? setting.Text);
            ModSettings.Trace("BindHeader " + panelName + " label=" + (label ?? setting.Text));
            BindCount++;
        }

        public void BindKeybind(string panelName, KeybindSetting setting, string label, string resetLabel)
        {
            // Sub-panel layout (created in prefab):
            //   panelName/
            //     Label                              (Text -- setting label, e.g. "Modifier Key")
            //     Btn_{prefix}{panelName}            (capture button, child Text shows current binding)
            //     Btn_{prefix}{panelName}_Reset      (reset button, child Text "Reset")
            var subPanel = Prefab.Child(panel, panelName);
            if (subPanel == null) return;

            var captureBtnObj = Prefab.Child(subPanel, "Btn_" + prefix + panelName);
            var resetBtnObj   = Prefab.Child(subPanel, "Btn_" + prefix + panelName + "_Reset");
            if (captureBtnObj == null || resetBtnObj == null) return;

            var captureBtn = captureBtnObj.GetComponent<Button>();
            var resetBtn   = resetBtnObj.GetComponent<Button>();
            if (captureBtn == null || resetBtn == null) return;

            // Row label (left side)
            var labelObj = Prefab.Child(subPanel, "Label");
            if (labelObj != null && !string.IsNullOrEmpty(label))
            {
                var labelText = labelObj.GetComponent<Text>();
                if (labelText != null) Prefab.ApplyLabel(labelText, label);
            }

            // Reset button label -- centralized "Reset" string by default, overridable per setting
            var resetText = resetBtnObj.GetComponentInChildren<Text>();
            if (resetText != null) Prefab.ApplyLabel(resetText, resetLabel ?? KeybindStrings.ResetLabel);

            // Display text inside the capture button. Show default in parentheses when unbound
            // so the player can see what Reset would restore.
            var displayText = captureBtnObj.GetComponentInChildren<Text>();
            if (displayText != null) displayText.text = KeybindFormat.FriendlyWithDefault(setting.Value, setting.DefaultValue);

            setting.Changed += newVal =>
            {
                if (displayText != null) displayText.text = KeybindFormat.FriendlyWithDefault(newVal, setting.DefaultValue);
            };

            string captureBtnName = "Btn_" + prefix + panelName;
            string resetBtnName = captureBtnName + "_Reset";

            Prefab.BindButton(captureBtn, new Action(() =>
            {
                ModSettings.Trace("Keybind capture started: " + captureBtnName);
                if (displayText != null) displayText.text = KeybindStrings.CapturePrompt;
                KeybindCapture.Begin(setting, displayText);
            }));

            Prefab.BindButton(resetBtn, new Action(() =>
            {
                ModSettings.Trace("Keybind reset: " + resetBtnName + " -> " + setting.DefaultValue);
                KeybindCapture.Cancel();
                setting.ResetToDefault();
            }));

            ModSettings.Trace("BindKeybind " + captureBtnName + " subscribed, value=" + setting.Value);
            BindCount++;
        }

        public void BindButton(string key, ActionBinding binding, string label = null)
        {
            var btnObj = Prefab.Child(panel, "Btn_" + prefix + key);
            if (btnObj == null) return;
            var btn = btnObj.GetComponent<Button>();
            if (btn == null) return;
            string name = "Btn_" + prefix + key;

            // Buttons may have a child Text component. Try to find and update it with label.
            if (!string.IsNullOrEmpty(label))
            {
                var textChild = btnObj.GetComponentInChildren<Text>();
                if (textChild != null) Prefab.ApplyLabel(textChild, label);
            }

            Prefab.BindButton(btn, new Action(() =>
            {
                ModSettings.Trace("Button listener fired: name=" + name);
                binding.Invoke();
            }));
            ModSettings.Trace("BindButton " + name + " subscribed");
            BindCount++;
        }

        // Helpers

        private void BindToggle(string panelName, Action<bool> setter, bool initialValue, string label = null)
        {
            var toggle = Prefab.Component<Toggle>(panel, panelName, "Toggle_" + prefix + panelName);
            if (toggle == null) return;
            toggle.isOn = initialValue;
            string name = "Toggle_" + prefix + panelName;
            int toggleId = toggle.GetInstanceID();
            ApplyLabel(panelName, name, label);
            Prefab.BindToggle(toggle, new Action<bool>(v =>
            {
                bool actual = toggle.isOn;
                ModSettings.Trace("Toggle listener fired: name=" + name + " v=" + v + " toggle.isOn=" + actual + " id=" + toggleId);
                setter(actual);
            }));
            ModSettings.Trace("BindToggle " + name + " subscribed, isOn=" + toggle.isOn + " id=" + toggleId);
        }

        // Locates the toggle's Label Text child and applies an English label with locale translation.
        // Quiet no-op when label is null (library caller didn't provide one - use prefab's serialized text).
        private void ApplyLabel(string panelName, string toggleName, string label)
        {
            if (string.IsNullOrEmpty(label)) return;
            var text = Prefab.FindLabel(panel, panelName, toggleName);
            if (text == null) return;
            Prefab.ApplyLabel(text, label);
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
