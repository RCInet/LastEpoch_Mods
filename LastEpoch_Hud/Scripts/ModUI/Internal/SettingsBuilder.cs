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

        public void Bind(string panelName, BoolSetting setting, string label = null, string path = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var toggleByPath = Prefab.ComponentAtPath<Toggle>(panel, path);
                if (toggleByPath == null) { ModSettings.Trace("Bind(Bool) path not found: " + path); return; }
                BindToggleComponent(toggleByPath, setting, label);
                BindCount++;
                return;
            }

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

        private static void BindToggleComponent(Toggle toggle, BoolSetting setting, string label)
        {
            toggle.isOn = setting.Value;
            int toggleId = toggle.GetInstanceID();
            if (!string.IsNullOrEmpty(label))
            {
                var text = toggle.GetComponentInChildren<Text>();
                if (text != null) Prefab.ApplyLabel(text, label);
            }
            Prefab.BindToggle(toggle, new Action<bool>(v =>
            {
                bool actual = toggle.isOn;
                ModSettings.Trace("Toggle (path) listener fired: id=" + toggleId + " v=" + v + " actual=" + actual);
                setting.Set(actual);
            }));
            ModSettings.Trace("Bind(Bool) path subscribed id=" + toggleId);
        }

        public void Bind(string panelName, RangeSetting setting, string label = null, RangePaths? paths = null)
        {
            if (paths.HasValue)
            {
                var p = paths.Value;
                var sMin = Prefab.ComponentAtPath<Slider>(panel, p.MinSlider);
                var sMax = Prefab.ComponentAtPath<Slider>(panel, p.MaxSlider);
                if (sMin == null || sMax == null)
                {
                    ModSettings.Trace("Range path sliders not found: min=" + p.MinSlider + " max=" + p.MaxSlider);
                    return;
                }
                var toggle = string.IsNullOrEmpty(p.Toggle) ? null : Prefab.ComponentAtPath<Toggle>(panel, p.Toggle);
                var displayText = sMin.GetComponentInChildren<Text>();
                BindRangeComponents(toggle, sMin, sMax, displayText, setting, label);
                BindCount++;
                return;
            }

            // Convention mode wires the enable toggle via BindToggle, then delegates the
            // slider half to the shared helper with a null toggle (already handled).
            BindToggle(panelName, v => setting.SetEnabled(v), setting.Enabled, label);
            var text = Prefab.FindText(panel, panelName, "Toggle_" + prefix + panelName);
            var convMin = Prefab.Component<Slider>(panel, panelName, "Slider_" + prefix + panelName + "_Min");
            var convMax = Prefab.Component<Slider>(panel, panelName, "Slider_" + prefix + panelName + "_Max");
            if (convMin == null && convMax == null) return;
            BindRangeComponents(null, convMin, convMax, text, setting, label: null);
            BindCount++;
        }

        // When enableToggle is null the setting is always enabled (no on/off control).
        private void BindRangeComponents(Toggle enableToggle, Slider sMin, Slider sMax, Text valueText, RangeSetting setting, string label)
        {
            if (enableToggle != null)
            {
                enableToggle.isOn = setting.Enabled;
                if (!string.IsNullOrEmpty(label))
                {
                    var t = enableToggle.GetComponentInChildren<Text>();
                    if (t != null) Prefab.ApplyLabel(t, label);
                }
                Prefab.BindToggle(enableToggle, new Action<bool>(v => setting.SetEnabled(enableToggle.isOn)));
            }
            else
            {
                if (!setting.Enabled) setting.SetEnabled(true);
            }

            ConfigureSlider(sMin, setting.MinLimit, setting.MaxLimit, setting.Min);
            ConfigureSlider(sMax, setting.MinLimit, setting.MaxLimit, setting.Max);

            if (sMin != null)
            {
                SliderHook.Register(sMin, v =>
                {
                    setting.SetMin(v);
                    if (sMax != null && sMax.value < setting.Min) sMax.value = setting.Min;
                    UpdateText(valueText, setting.FormatRange());
                });
            }
            if (sMax != null)
            {
                SliderHook.Register(sMax, v =>
                {
                    setting.SetMax(v);
                    if (sMin != null && sMin.value > setting.Max) sMin.value = setting.Max;
                    UpdateText(valueText, setting.FormatRange());
                });
            }
            UpdateText(valueText, setting.FormatRange());
        }

        public void Bind(string panelName, FloatSetting setting, string label = null, FloatPaths? paths = null)
        {
            if (paths.HasValue)
            {
                var p = paths.Value;
                var slider = Prefab.ComponentAtPath<Slider>(panel, p.Slider);
                if (slider == null)
                {
                    ModSettings.Trace("Float path slider not found: " + p.Slider);
                    return;
                }
                var toggle = string.IsNullOrEmpty(p.Toggle) ? null : Prefab.ComponentAtPath<Toggle>(panel, p.Toggle);
                var displayText = slider.GetComponentInChildren<Text>();
                BindFloatComponents(toggle, slider, displayText, setting, label);
                BindCount++;
                return;
            }

            BindToggle(panelName, v => setting.SetEnabled(v), setting.Enabled, label);
            var text = Prefab.FindText(panel, panelName, "Toggle_" + prefix + panelName);
            var convSlider = Prefab.Component<Slider>(panel, panelName, "Slider_" + prefix + panelName);
            if (convSlider == null) return;
            BindFloatComponents(null, convSlider, text, setting, label: null);
            BindCount++;
        }

        private void BindFloatComponents(Toggle enableToggle, Slider slider, Text valueText, FloatSetting setting, string label)
        {
            if (enableToggle != null)
            {
                enableToggle.isOn = setting.Enabled;
                if (!string.IsNullOrEmpty(label))
                {
                    var t = enableToggle.GetComponentInChildren<Text>();
                    if (t != null) Prefab.ApplyLabel(t, label);
                }
                Prefab.BindToggle(enableToggle, new Action<bool>(v => setting.SetEnabled(enableToggle.isOn)));
            }
            else
            {
                if (!setting.Enabled) setting.SetEnabled(true);
            }

            float maxLimit = slider.maxValue;
            slider.value = setting.Value;
            SliderHook.Register(slider, v =>
            {
                setting.SetValue(v);
                UpdateText(valueText, setting.FormatValue(maxLimit));
            });
            UpdateText(valueText, setting.FormatValue(maxLimit));
        }

        public void BindRadio(string groupPanel, RadioSetting setting, RadioPaths? paths = null)
        {
            if (paths.HasValue)
            {
                var p = paths.Value;
                if (p.Toggles == null || p.Toggles.Length != setting.OptionNames.Length)
                {
                    Main.logger_instance?.Error("BindRadio path mode: paths.Toggles length (" + (p.Toggles?.Length ?? 0) + ") != optionNames length (" + setting.OptionNames.Length + ")");
                    return;
                }
                var toggles = new Toggle[p.Toggles.Length];
                for (int i = 0; i < p.Toggles.Length; i++)
                {
                    toggles[i] = Prefab.ComponentAtPath<Toggle>(panel, p.Toggles[i]);
                    if (toggles[i] == null)
                    {
                        ModSettings.Trace("Radio path not found: " + p.Toggles[i]);
                        return;
                    }
                }
                BindRadioComponents(toggles, setting);
                BindCount++;
                return;
            }

            var group = Prefab.Child(panel, groupPanel);
            if (group == null) return;

            var convToggles = new Toggle[setting.OptionNames.Length];
            for (int i = 0; i < setting.OptionNames.Length; i++)
            {
                string name = setting.OptionNames[i];
                convToggles[i] = Prefab.Component<Toggle>(group, name, "Toggle_" + prefix + name);
            }
            BindRadioComponents(convToggles, setting);
            ModSettings.Trace("BindRadio " + groupPanel + " subscribed " + setting.OptionNames.Length + " options");
            BindCount++;
        }

        private void BindRadioComponents(Toggle[] toggles, RadioSetting setting)
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                int idx = i;
                var toggle = toggles[idx];
                if (toggle == null) continue;
                toggle.isOn = setting.IsSelected(idx);
                Prefab.BindToggle(toggle, new Action<bool>(v =>
                {
                    bool actual = toggle.isOn;
                    ModSettings.Trace("Radio listener fired: idx=" + idx + " v=" + v + " actual=" + actual);
                    setting.Select(idx, actual);
                }));
            }
        }

        public void BindDropdown(string panelName, DropdownSetting setting, string label = null, string path = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var dropdownByPath = Prefab.ComponentAtPath<Dropdown>(panel, path);
                if (dropdownByPath == null) { ModSettings.Trace("BindDropdown path not found: " + path); return; }
                BindDropdownComponent(dropdownByPath, setting, label);
                BindCount++;
                return;
            }

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

        private static void BindDropdownComponent(Dropdown dropdown, DropdownSetting setting, string label)
        {
            if (setting.Options.Length > 0) ApplyDropdownOptions(dropdown, setting.Options);
            dropdown.value = setting.Value;
            int id = dropdown.GetInstanceID();
            if (!string.IsNullOrEmpty(label))
            {
                var text = dropdown.GetComponentInChildren<Text>();
                if (text != null) Prefab.ApplyLabel(text, label);
            }
            Prefab.BindDropdown(dropdown, new Action<int>(v =>
            {
                ModSettings.Trace("Dropdown (path) listener fired: id=" + id + " v=" + v);
                setting.Set(v);
            }));
            setting.applyOptions = opts => ApplyDropdownOptions(dropdown, opts);
            ModSettings.Trace("BindDropdown path subscribed id=" + id);
        }

        public void BindHeader(string panelName, HeaderSetting setting, string label, string path = null)
        {
            GameObject obj;
            if (!string.IsNullOrEmpty(path))
            {
                obj = Prefab.ChildPath(panel, path);
                if (obj == null) { ModSettings.Trace("BindHeader path not found: " + path); return; }
            }
            else
            {
                obj = Prefab.Child(panel, panelName);
                if (obj == null) return;
            }
            var text = obj.GetComponent<Text>();
            if (text == null) text = obj.GetComponentInChildren<Text>();
            if (text == null) return;
            Prefab.ApplyLabel(text, label ?? setting.Text);
            ModSettings.Trace("BindHeader " + (path ?? panelName) + " label=" + (label ?? setting.Text));
            BindCount++;
        }

        public void BindKeybind(string panelName, KeybindSetting setting, string label, string resetLabel, KeybindPaths? paths = null)
        {
            if (paths.HasValue)
            {
                var p = paths.Value;
                var captureBtn = Prefab.ComponentAtPath<Button>(panel, p.CaptureButton);
                if (captureBtn == null)
                {
                    ModSettings.Trace("Keybind path capture not found: " + p.CaptureButton);
                    return;
                }
                var resetBtn = string.IsNullOrEmpty(p.ResetButton) ? null : Prefab.ComponentAtPath<Button>(panel, p.ResetButton);
                var labelText = string.IsNullOrEmpty(p.Label) ? null : Prefab.ComponentAtPath<Text>(panel, p.Label);
                BindKeybindComponents(captureBtn, resetBtn, labelText, setting, label, resetLabel);
                BindCount++;
                return;
            }

            // Convention layout: panelName/Label + Btn_{prefix}{panelName}[_Reset]
            var subPanel = Prefab.Child(panel, panelName);
            if (subPanel == null) return;

            var captureBtnObj = Prefab.Child(subPanel, "Btn_" + prefix + panelName);
            var resetBtnObj   = Prefab.Child(subPanel, "Btn_" + prefix + panelName + "_Reset");
            if (captureBtnObj == null || resetBtnObj == null) return;

            var convCaptureBtn = captureBtnObj.GetComponent<Button>();
            var convResetBtn   = resetBtnObj.GetComponent<Button>();
            if (convCaptureBtn == null || convResetBtn == null) return;

            var labelObj = Prefab.Child(subPanel, "Label");
            var convLabelText = labelObj?.GetComponent<Text>();

            BindKeybindComponents(convCaptureBtn, convResetBtn, convLabelText, setting, label, resetLabel);
            BindCount++;
        }

        // resetBtn null = no reset UI. rowLabelText null = caller has no separate label control.
        private void BindKeybindComponents(Button captureBtn, Button resetBtn, Text rowLabelText, KeybindSetting setting, string label, string resetLabel)
        {
            if (rowLabelText != null && !string.IsNullOrEmpty(label))
                Prefab.ApplyLabel(rowLabelText, label);

            if (resetBtn != null)
            {
                var resetText = resetBtn.GetComponentInChildren<Text>();
                if (resetText != null) Prefab.ApplyLabel(resetText, resetLabel ?? KeybindStrings.ResetLabel);
            }

            var displayText = captureBtn.GetComponentInChildren<Text>();
            if (displayText != null) displayText.text = KeybindFormat.FriendlyWithDefault(setting.Value, setting.DefaultValue);

            setting.Changed += newVal =>
            {
                if (displayText != null) displayText.text = KeybindFormat.FriendlyWithDefault(newVal, setting.DefaultValue);
            };

            int captureId = captureBtn.GetInstanceID();
            Prefab.BindButton(captureBtn, new Action(() =>
            {
                ModSettings.Trace("Keybind capture started: id=" + captureId);
                if (displayText != null) displayText.text = KeybindStrings.CapturePrompt;
                KeybindCapture.Begin(setting, displayText);
            }));

            if (resetBtn != null)
            {
                Prefab.BindButton(resetBtn, new Action(() =>
                {
                    ModSettings.Trace("Keybind reset id=" + captureId + " -> " + setting.DefaultValue);
                    KeybindCapture.Cancel();
                    setting.ResetToDefault();
                }));
            }

            ModSettings.Trace("BindKeybind id=" + captureId + " subscribed, value=" + setting.Value);
        }

        public void BindButton(string key, ActionBinding binding, string label = null, string path = null)
        {
            Button btn;
            string name;
            if (!string.IsNullOrEmpty(path))
            {
                btn = Prefab.ComponentAtPath<Button>(panel, path);
                if (btn == null) { ModSettings.Trace("BindButton path not found: " + path); return; }
                name = path;
                if (!string.IsNullOrEmpty(label))
                {
                    var textChild = btn.GetComponentInChildren<Text>();
                    if (textChild != null) Prefab.ApplyLabel(textChild, label);
                }
            }
            else
            {
                var btnObj = Prefab.Child(panel, "Btn_" + prefix + key);
                if (btnObj == null) return;
                btn = btnObj.GetComponent<Button>();
                if (btn == null) return;
                name = "Btn_" + prefix + key;

                if (!string.IsNullOrEmpty(label))
                {
                    var textChild = btnObj.GetComponentInChildren<Text>();
                    if (textChild != null) Prefab.ApplyLabel(textChild, label);
                }
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

        // No-op when label is null so the prefab's serialized Text is preserved.
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
