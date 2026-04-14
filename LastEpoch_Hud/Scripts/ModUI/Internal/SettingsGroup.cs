using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Settings group: handles serialization, UI binding, and tab creation.
    // Factory methods register settings for save/load and bind them to prefab elements.
    public class SettingsGroup
    {
        public string Name { get; }

        // Defaults to the Hud; custom prefabs opt in via .Root("name").
        public string RootName { get; private set; } = BindingRoots.HudRootName;

        // Tab metadata -- set via fluent API, consumed by TabManager when the Hud binds
        public string TabId { get; private set; }
        public string ContentObjectName { get; private set; }
        public string MenuButtonName { get; private set; }
        public string UiPrefix { get; private set; } = "";
        public string ViewportPanel { get; private set; }
        public string ViewportContent { get; private set; }

        private readonly List<Entry> entries = new();

        public SettingsGroup(string name)
        {
            Name = name;
            ModSettings.RegisterGroup(this);
        }

        public bool HasTab => TabId != null;

        // Fluent configuration

        public SettingsGroup Root(string rootName)
        {
            RootName = rootName ?? BindingRoots.HudRootName;
            return this;
        }

        public SettingsGroup Tab(string tabId, string menuButton)
        {
            TabId = tabId;
            MenuButtonName = menuButton;
            return this;
        }

        public SettingsGroup Content(string contentObjectName)
        {
            ContentObjectName = contentObjectName;
            return this;
        }

        public SettingsGroup Viewport(string panel, string content)
        {
            ViewportPanel = panel;
            ViewportContent = content;
            return this;
        }

        public SettingsGroup Prefix(string uiPrefix)
        {
            UiPrefix = uiPrefix;
            return this;
        }

        // Custom bind callback. Runs after automatic bindings with access to both
        // the content panel (e.g., Items_Content) and viewport content.
        // Escape hatch for elements that don't fit the convention or path-mode patterns.
        private System.Action<UnityEngine.GameObject, UnityEngine.GameObject> customBind;

        public SettingsGroup OnBind(
            System.Action<UnityEngine.GameObject, UnityEngine.GameObject> callback
        )
        {
            customBind = callback;
            return this;
        }

        // Factory methods -- optional 'panel' overrides prefab panel name (defaults to key).
        // Optional 'label' is the English display text -- applied to the widget's Label Text
        // at bind time, translated via Locales.current_dictionary. Leave null to keep whatever
        // label was serialized into the prefab.

        public BoolSetting Bool(
            string key,
            bool defaultValue = false,
            string panel = null,
            string label = null,
            string path = null
        )
        {
            var s = new BoolSetting(defaultValue);
            entries.Add(
                new Entry(
                    key,
                    panel ?? key,
                    EntryType.Bool,
                    label: label,
                    boolSetting: s,
                    widgetPath: path
                )
            );
            return s;
        }

        public FloatSetting Float(
            string key,
            float defaultValue = 0f,
            bool defaultEnabled = false,
            DisplayFormat format = DisplayFormat.Raw,
            float min = 0f,
            float max = 0f,
            string panel = null,
            string label = null,
            FloatPaths? paths = null
        )
        {
            var s = new FloatSetting(defaultValue, defaultEnabled, format, min, max);
            entries.Add(
                new Entry(
                    key,
                    panel ?? key,
                    EntryType.Float,
                    label: label,
                    floatSetting: s,
                    floatPaths: paths
                )
            );
            return s;
        }

        public RangeSetting Range(
            string key,
            float minLimit = 0,
            float maxLimit = 255,
            float? defaultMin = null,
            float? defaultMax = null,
            bool defaultEnabled = false,
            DisplayFormat format = DisplayFormat.Raw,
            string panel = null,
            string label = null,
            RangePaths? paths = null
        )
        {
            var s = new RangeSetting(
                minLimit,
                maxLimit,
                defaultMin,
                defaultMax,
                defaultEnabled,
                format
            );
            entries.Add(
                new Entry(
                    key,
                    panel ?? key,
                    EntryType.Range,
                    label: label,
                    rangeSetting: s,
                    rangePaths: paths
                )
            );
            return s;
        }

        public DropdownSetting Dropdown(
            string key,
            int defaultValue = 0,
            string[] options = null,
            string panel = null,
            string label = null,
            string path = null
        )
        {
            var s = new DropdownSetting(defaultValue, options);
            entries.Add(
                new Entry(
                    key,
                    panel ?? key,
                    EntryType.Dropdown,
                    label: label,
                    dropdownSetting: s,
                    widgetPath: path
                )
            );
            return s;
        }

        public RadioSetting Radio(string key, string radioPanel, params string[] optionNames)
        {
            var s = new RadioSetting(optionNames);
            entries.Add(new Entry(key, radioPanel, EntryType.Radio, radioSetting: s));
            return s;
        }

        // Separate method because params arrays can't follow optional parameters cleanly.
        public RadioSetting RadioWithPaths(
            string key,
            RadioPaths paths,
            params string[] optionNames
        )
        {
            var s = new RadioSetting(optionNames);
            entries.Add(new Entry(key, key, EntryType.Radio, radioSetting: s, radioPaths: paths));
            return s;
        }

        public ActionBinding Button(string key, string label = null, string path = null)
        {
            var b = new ActionBinding();
            entries.Add(
                new Entry(
                    key,
                    key,
                    EntryType.Button,
                    label: label,
                    actionBinding: b,
                    widgetPath: path
                )
            );
            return b;
        }

        public HeaderSetting Header(
            string key,
            string label,
            string panel = null,
            string path = null
        )
        {
            var s = new HeaderSetting(label);
            entries.Add(
                new Entry(
                    key,
                    panel ?? key,
                    EntryType.Header,
                    label: label,
                    headerSetting: s,
                    widgetPath: path
                )
            );
            return s;
        }

        // resetLabel null = centralized "Reset" string.
        public KeybindSetting Keybind(
            string key,
            string defaultBinding = "",
            string panel = null,
            string label = null,
            string resetLabel = null,
            KeybindPaths? paths = null
        )
        {
            var s = new KeybindSetting(defaultBinding);
            entries.Add(
                new Entry(
                    key,
                    panel ?? key,
                    EntryType.Keybind,
                    label: label,
                    keybindSetting: s,
                    resetLabel: resetLabel,
                    keybindPaths: paths
                )
            );
            return s;
        }

        // Resolves the viewport hierarchy and binds all settings to their prefab UI elements.
        // Handles both viewport-based and flat content layouts.

        public void ResolveAndBind(UnityEngine.GameObject contentObj)
        {
            var viewportContent =
                (ViewportPanel != null)
                    ? Prefab.ViewportContent(contentObj, ViewportPanel, ViewportContent)
                    : contentObj;
            if (viewportContent == null)
                return;

            BindAll(contentObj, viewportContent, new SettingsBuilder(viewportContent, UiPrefix));
        }

        private void BindAll(
            UnityEngine.GameObject contentObj,
            UnityEngine.GameObject viewportContent,
            SettingsBuilder builder
        )
        {
            int bound = 0;
            int failed = 0;
            foreach (var e in entries)
            {
                try
                {
                    int before = builder.BindCount;
                    switch (e.Type)
                    {
                        case EntryType.Bool:
                            builder.Bind(e.Panel, e.BoolSetting, e.Label, e.WidgetPath);
                            break;
                        case EntryType.Float:
                            builder.Bind(e.Panel, e.FloatSetting, e.Label, e.FloatPaths);
                            break;
                        case EntryType.Range:
                            builder.Bind(e.Panel, e.RangeSetting, e.Label, e.RangePaths);
                            break;
                        case EntryType.Dropdown:
                            builder.BindDropdown(e.Panel, e.DropdownSetting, e.Label, e.WidgetPath);
                            break;
                        case EntryType.Radio:
                            builder.BindRadio(e.Panel, e.RadioSetting, e.RadioPaths);
                            break;
                        case EntryType.Button:
                            builder.BindButton(e.Key, e.ActionBinding, e.Label, e.WidgetPath);
                            break;
                        case EntryType.Header:
                            builder.BindHeader(e.Panel, e.HeaderSetting, e.Label, e.WidgetPath);
                            break;
                        case EntryType.Keybind:
                            builder.BindKeybind(
                                e.Panel,
                                e.KeybindSetting,
                                e.Label,
                                e.ResetLabel,
                                e.KeybindPaths
                            );
                            break;
                    }
                    if (builder.BindCount > before)
                    {
                        bound++;
                        continue;
                    }
                }
                catch (System.Exception ex)
                {
                    Main.logger_instance?.Warning(
                        "SettingsGroup "
                            + Name
                            + ": Exception binding '"
                            + e.Key
                            + "': "
                            + ex.Message
                    );
                }
                failed++;
                Main.logger_instance?.Warning(
                    "SettingsGroup "
                        + Name
                        + ": Failed to bind '"
                        + e.Key
                        + "' (panel: '"
                        + e.Panel
                        + "')"
                );
            }

            try
            {
                customBind?.Invoke(contentObj, viewportContent);
            }
            catch (System.Exception ex)
            {
                Main.logger_instance?.Warning(
                    "SettingsGroup " + Name + ": Custom bind failed: " + ex.Message
                );
            }

            Main.logger_instance?.Msg(
                "SettingsGroup " + Name + ": Bound " + bound + "/" + (bound + failed) + " settings"
            );
        }

        // JSON serialization

        public void Save(JObject root)
        {
            var section = new JObject();
            foreach (var e in entries)
                SaveEntry(section, e);
            root[Name] = section;
        }

        public void Load(JObject root)
        {
            if (root[Name] is not JObject section)
                return;
            foreach (var e in entries)
                LoadEntry(section, e);
        }

        public bool HasMissingEntries(JObject root)
        {
            if (root[Name] is not JObject section)
                return true;
            foreach (var e in entries)
            {
                if (section[e.Key] == null)
                    return true;
            }
            return false;
        }

        // Internals

        private static void SaveEntry(JObject section, Entry e)
        {
            switch (e.Type)
            {
                case EntryType.Bool:
                    section[e.Key] = e.BoolSetting.Value;
                    break;
                case EntryType.Float:
                    section[e.Key] = new JObject
                    {
                        ["Enabled"] = e.FloatSetting.Enabled,
                        ["Value"] = e.FloatSetting.Value,
                    };
                    break;
                case EntryType.Range:
                    section[e.Key] = new JObject
                    {
                        ["Enabled"] = e.RangeSetting.Enabled,
                        ["Min"] = e.RangeSetting.Min,
                        ["Max"] = e.RangeSetting.Max,
                    };
                    break;
                case EntryType.Dropdown:
                    section[e.Key] = e.DropdownSetting.Value;
                    break;
                case EntryType.Radio:
                    var radio = new JObject();
                    for (int i = 0; i < e.RadioSetting.OptionNames.Length; i++)
                        radio[e.RadioSetting.OptionNames[i]] = e.RadioSetting.GetValue(i);
                    section[e.Key] = radio;
                    break;
                case EntryType.Keybind:
                    section[e.Key] = e.KeybindSetting.Value;
                    break;
            }
        }

        private static void LoadEntry(JObject section, Entry e)
        {
            var token = section[e.Key];
            if (token == null)
                return;

            switch (e.Type)
            {
                case EntryType.Bool:
                    e.BoolSetting.Value = token.Value<bool>();
                    break;
                case EntryType.Dropdown:
                    int idx = token.Value<int>();
                    int optCount = e.DropdownSetting.Options?.Length ?? 0;
                    if (optCount > 0 && (idx < 0 || idx >= optCount))
                    {
                        WarnInvalid(
                            e.Key,
                            idx + " not in [0," + (optCount - 1) + "], snapped to 0"
                        );
                        idx = 0;
                    }
                    e.DropdownSetting.Value = idx;
                    break;
                case EntryType.Float:
                    if (token is JObject fo)
                    {
                        e.FloatSetting.Enabled = fo.Value<bool>("Enabled");
                        float fv = fo.Value<float>("Value");
                        if (e.FloatSetting.HasLimits)
                        {
                            float clamped = Mathf.Clamp(
                                fv,
                                e.FloatSetting.MinLimit,
                                e.FloatSetting.MaxLimit
                            );
                            if (clamped != fv)
                            {
                                WarnInvalid(
                                    e.Key,
                                    fv
                                        + " out of ["
                                        + e.FloatSetting.MinLimit
                                        + ","
                                        + e.FloatSetting.MaxLimit
                                        + "], clamped to "
                                        + clamped
                                );
                                fv = clamped;
                            }
                        }
                        e.FloatSetting.Value = fv;
                    }
                    break;
                case EntryType.Range:
                    if (token is JObject ro)
                    {
                        e.RangeSetting.Enabled = ro.Value<bool>("Enabled");
                        float rmin = Mathf.Clamp(
                            ro.Value<float>("Min"),
                            e.RangeSetting.MinLimit,
                            e.RangeSetting.MaxLimit
                        );
                        float rmax = Mathf.Clamp(
                            ro.Value<float>("Max"),
                            e.RangeSetting.MinLimit,
                            e.RangeSetting.MaxLimit
                        );
                        if (rmin > rmax)
                        {
                            WarnInvalid(e.Key, "Min(" + rmin + ") > Max(" + rmax + "), swapping");
                            (rmin, rmax) = (rmax, rmin);
                        }
                        e.RangeSetting.Min = rmin;
                        e.RangeSetting.Max = rmax;
                    }
                    break;
                case EntryType.Radio:
                    if (token is not JObject jo)
                        break;
                    for (int i = 0; i < e.RadioSetting.OptionNames.Length; i++)
                    {
                        var val = jo[e.RadioSetting.OptionNames[i]];
                        if (val != null)
                            e.RadioSetting.SetValue(i, val.Value<bool>());
                    }
                    // If nothing is selected after load and a default exists, apply it
                    if (e.RadioSetting.DefaultIndex >= 0)
                    {
                        bool anySelected = false;
                        for (int i = 0; i < e.RadioSetting.OptionNames.Length; i++)
                            if (e.RadioSetting.GetValue(i))
                            {
                                anySelected = true;
                                break;
                            }
                        if (!anySelected)
                            e.RadioSetting.SetValue(e.RadioSetting.DefaultIndex, true);
                    }
                    break;
                case EntryType.Keybind:
                    string kb = token.Value<string>() ?? "";
                    if (!IsValidKeybind(kb))
                    {
                        WarnInvalid(
                            e.Key,
                            "'"
                                + kb
                                + "' is not a valid binding, reset to default '"
                                + e.KeybindSetting.DefaultValue
                                + "'"
                        );
                        kb = e.KeybindSetting.DefaultValue;
                    }
                    e.KeybindSetting.Value = kb;
                    break;
            }
        }

        private static bool IsValidKeybind(string binding)
        {
            if (string.IsNullOrEmpty(binding))
                return true;
            if (binding.StartsWith("kb:"))
                return System.Enum.TryParse<KeyCode>(binding.Substring(3), out _);
            if (binding.StartsWith("gp:"))
                return !string.IsNullOrEmpty(binding.Substring(3));
            return false;
        }

        private static void WarnInvalid(string key, string detail)
        {
            Main.logger_instance?.Warning(
                "ModUI: setting '" + key + "' invalid in SaveModUI.json: " + detail
            );
        }

        private enum EntryType
        {
            Bool,
            Float,
            Range,
            Dropdown,
            Radio,
            Button,
            Header,
            Keybind,
        }

        private readonly struct Entry
        {
            public readonly string Key;
            public readonly string Panel;
            public readonly string Label;
            public readonly string ResetLabel;
            public readonly string WidgetPath;
            public readonly EntryType Type;
            public readonly BoolSetting BoolSetting;
            public readonly FloatSetting FloatSetting;
            public readonly RangeSetting RangeSetting;
            public readonly DropdownSetting DropdownSetting;
            public readonly RadioSetting RadioSetting;
            public readonly ActionBinding ActionBinding;
            public readonly HeaderSetting HeaderSetting;
            public readonly KeybindSetting KeybindSetting;
            public readonly FloatPaths? FloatPaths;
            public readonly RangePaths? RangePaths;
            public readonly KeybindPaths? KeybindPaths;
            public readonly RadioPaths? RadioPaths;

            public Entry(
                string key,
                string panel,
                EntryType type,
                string label = null,
                BoolSetting boolSetting = null,
                FloatSetting floatSetting = null,
                RangeSetting rangeSetting = null,
                DropdownSetting dropdownSetting = null,
                RadioSetting radioSetting = null,
                ActionBinding actionBinding = null,
                HeaderSetting headerSetting = null,
                KeybindSetting keybindSetting = null,
                string resetLabel = null,
                string widgetPath = null,
                FloatPaths? floatPaths = null,
                RangePaths? rangePaths = null,
                KeybindPaths? keybindPaths = null,
                RadioPaths? radioPaths = null
            )
            {
                Key = key;
                Panel = panel;
                Label = label;
                ResetLabel = resetLabel;
                WidgetPath = widgetPath;
                Type = type;
                BoolSetting = boolSetting;
                FloatSetting = floatSetting;
                RangeSetting = rangeSetting;
                DropdownSetting = dropdownSetting;
                RadioSetting = radioSetting;
                ActionBinding = actionBinding;
                HeaderSetting = headerSetting;
                KeybindSetting = keybindSetting;
                FloatPaths = floatPaths;
                RangePaths = rangePaths;
                KeybindPaths = keybindPaths;
                RadioPaths = radioPaths;
            }
        }
    }
}
