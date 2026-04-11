using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Settings group: handles serialization, UI binding, and tab creation.
    // Factory methods register settings for save/load and bind them to prefab elements.
    public class SettingsGroup
    {
        public string Name { get; }

        // Tab metadata -- set via fluent API, used by TabManager to auto-create HUD tabs
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
        // Use for non-standard elements that don't fit the auto-binding pattern.
        private System.Action<UnityEngine.GameObject, UnityEngine.GameObject> customBind;

        public SettingsGroup OnBind(System.Action<UnityEngine.GameObject, UnityEngine.GameObject> callback)
        {
            customBind = callback;
            return this;
        }

        // Factory methods -- optional 'panel' overrides prefab panel name (defaults to key).
        // Optional 'label' is the English display text -- applied to the widget's Label Text
        // at bind time, translated via Locales.current_dictionary. Leave null to keep whatever
        // label was serialized into the prefab.

        public BoolSetting Bool(string key, bool defaultValue = false, string panel = null, string label = null)
        {
            var s = new BoolSetting(defaultValue);
            entries.Add(new Entry(key, panel ?? key, EntryType.Bool, label: label, boolSetting: s));
            return s;
        }

        public FloatSetting Float(string key, float defaultValue = 0f, bool defaultEnabled = false, DisplayFormat format = DisplayFormat.Raw, string panel = null, string label = null)
        {
            var s = new FloatSetting(defaultValue, defaultEnabled, format);
            entries.Add(new Entry(key, panel ?? key, EntryType.Float, label: label, floatSetting: s));
            return s;
        }

        public RangeSetting Range(string key, float minLimit = 0, float maxLimit = 255, float? defaultMin = null, float? defaultMax = null, bool defaultEnabled = false, DisplayFormat format = DisplayFormat.Raw, string panel = null, string label = null)
        {
            var s = new RangeSetting(minLimit, maxLimit, defaultMin, defaultMax, defaultEnabled, format);
            entries.Add(new Entry(key, panel ?? key, EntryType.Range, label: label, rangeSetting: s));
            return s;
        }

        public DropdownSetting Dropdown(string key, int defaultValue = 0, string[] options = null, string panel = null, string label = null)
        {
            var s = new DropdownSetting(defaultValue, options);
            entries.Add(new Entry(key, panel ?? key, EntryType.Dropdown, label: label, dropdownSetting: s));
            return s;
        }

        public RadioSetting Radio(string key, string radioPanel, params string[] optionNames)
        {
            var s = new RadioSetting(optionNames);
            entries.Add(new Entry(key, radioPanel, EntryType.Radio, radioSetting: s));
            return s;
        }

        public ActionBinding Button(string key, string label = null)
        {
            var b = new ActionBinding();
            entries.Add(new Entry(key, key, EntryType.Button, label: label, actionBinding: b));
            return b;
        }

        // Resolves the viewport hierarchy and binds all settings to their prefab UI elements.
        // Handles both viewport-based and flat content layouts.

        public void ResolveAndBind(UnityEngine.GameObject contentObj)
        {
            var viewportContent = (ViewportPanel != null)
                ? Prefab.ViewportContent(contentObj, ViewportPanel, ViewportContent)
                : contentObj;
            if (viewportContent == null) return;

            BindAll(contentObj, viewportContent, new SettingsBuilder(viewportContent, UiPrefix));
        }

        private void BindAll(UnityEngine.GameObject contentObj, UnityEngine.GameObject viewportContent, SettingsBuilder builder)
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
                        case EntryType.Bool: builder.Bind(e.Panel, e.BoolSetting, e.Label); break;
                        case EntryType.Float: builder.Bind(e.Panel, e.FloatSetting, e.Label); break;
                        case EntryType.Range: builder.Bind(e.Panel, e.RangeSetting, e.Label); break;
                        case EntryType.Dropdown: builder.BindDropdown(e.Panel, e.DropdownSetting, e.Label); break;
                        case EntryType.Radio: builder.BindRadio(e.Panel, e.RadioSetting); break;
                        case EntryType.Button: builder.BindButton(e.Key, e.ActionBinding, e.Label); break;
                    }
                    if (builder.BindCount > before) { bound++; continue; }
                }
                catch (System.Exception ex)
                {
                    Main.logger_instance?.Warning("SettingsGroup " + Name + ": Exception binding '" + e.Key + "': " + ex.Message);
                }
                failed++;
                Main.logger_instance?.Warning("SettingsGroup " + Name + ": Failed to bind '" + e.Key + "' (panel: '" + e.Panel + "')");
            }

            try { customBind?.Invoke(contentObj, viewportContent); }
            catch (System.Exception ex) { Main.logger_instance?.Warning("SettingsGroup " + Name + ": Custom bind failed: " + ex.Message); }

            Main.logger_instance?.Msg("SettingsGroup " + Name + ": Bound " + bound + "/" + (bound + failed) + " settings");
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
            if (root[Name] is not JObject section) return;
            foreach (var e in entries)
                LoadEntry(section, e);
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
                    section[e.Key] = new JObject { ["Enabled"] = e.FloatSetting.Enabled, ["Value"] = e.FloatSetting.Value };
                    break;
                case EntryType.Range:
                    section[e.Key] = new JObject { ["Enabled"] = e.RangeSetting.Enabled, ["Min"] = e.RangeSetting.Min, ["Max"] = e.RangeSetting.Max };
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
            }
        }

        private static void LoadEntry(JObject section, Entry e)
        {
            var token = section[e.Key];
            if (token == null) return;

            switch (e.Type)
            {
                case EntryType.Bool:
                    e.BoolSetting.Value = token.Value<bool>();
                    break;
                case EntryType.Dropdown:
                    e.DropdownSetting.Value = token.Value<int>();
                    break;
                case EntryType.Float:
                    if (token is JObject fo) { e.FloatSetting.Enabled = fo.Value<bool>("Enabled"); e.FloatSetting.Value = fo.Value<float>("Value"); }
                    break;
                case EntryType.Range:
                    if (token is JObject ro) { e.RangeSetting.Enabled = ro.Value<bool>("Enabled"); e.RangeSetting.Min = ro.Value<float>("Min"); e.RangeSetting.Max = ro.Value<float>("Max"); }
                    break;
                case EntryType.Radio:
                    if (token is not JObject jo) break;
                    for (int i = 0; i < e.RadioSetting.OptionNames.Length; i++)
                    {
                        var val = jo[e.RadioSetting.OptionNames[i]];
                        if (val != null) e.RadioSetting.SetValue(i, val.Value<bool>());
                    }
                    break;
            }
        }

        private enum EntryType { Bool, Float, Range, Dropdown, Radio, Button }

        private readonly struct Entry
        {
            public readonly string Key;
            public readonly string Panel;
            public readonly string Label;
            public readonly EntryType Type;
            public readonly BoolSetting BoolSetting;
            public readonly FloatSetting FloatSetting;
            public readonly RangeSetting RangeSetting;
            public readonly DropdownSetting DropdownSetting;
            public readonly RadioSetting RadioSetting;
            public readonly ActionBinding ActionBinding;

            public Entry(string key, string panel, EntryType type, string label = null, BoolSetting boolSetting = null, FloatSetting floatSetting = null, RangeSetting rangeSetting = null, DropdownSetting dropdownSetting = null, RadioSetting radioSetting = null, ActionBinding actionBinding = null)
            {
                Key = key;
                Panel = panel;
                Label = label;
                Type = type;
                BoolSetting = boolSetting;
                FloatSetting = floatSetting;
                RangeSetting = rangeSetting;
                DropdownSetting = dropdownSetting;
                RadioSetting = radioSetting;
                ActionBinding = actionBinding;
            }
        }
    }
}
