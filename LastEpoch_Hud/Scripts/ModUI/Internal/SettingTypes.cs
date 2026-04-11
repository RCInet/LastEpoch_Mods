using System;

namespace LastEpoch_Hud.Scripts.ModUI
{
    public enum DisplayFormat
    {
        Raw,        // "50 to 100"
        Tier,       // "(min+1) to (max+1)" -- game uses 0-indexed, display is 1-indexed
        Percent,    // "75 %" -- displayed as percentage relative to the setting's max limit
        Seconds     // "5 sec"
    }

    public static class DisplayFormatHelper
    {
        public static string Format(float val, DisplayFormat fmt, float maxLimit = 0)
        {
            return fmt switch
            {
                DisplayFormat.Tier => ((int)val + 1).ToString(),
                DisplayFormat.Percent => FormatPercent(val, maxLimit),
                DisplayFormat.Seconds => (int)val + " sec",
                _ => ((int)val).ToString()
            };
        }

        private static string FormatPercent(float val, float maxLimit)
        {
            if (maxLimit > 0 && maxLimit != 100)
                return (int)(val / maxLimit * 100) + " %";
            return (int)val + " %";
        }
    }

    // On/off toggle.
    public class BoolSetting
    {
        public bool Value { get; internal set; }

        public event Action<bool> Changed;

        public BoolSetting(bool defaultValue = false)
        {
            Value = defaultValue;
        }

        public void Set(bool val)
        {
            ModSettings.Trace("BoolSetting.Set: before=" + Value + " requested=" + val);
            if (Value == val) return;
            Value = val;
            ModSettings.MarkDirty();
            Changed?.Invoke(val);
        }
    }

    // Toggle + single float value (e.g., "Density Multiplier: 2.5x").
    public class FloatSetting
    {
        public bool Enabled { get; internal set; }
        public float Value { get; internal set; }
        public DisplayFormat Format { get; }

        public event Action Changed;

        public FloatSetting(float defaultValue = 0f, bool defaultEnabled = false, DisplayFormat format = DisplayFormat.Raw)
        {
            Value = defaultValue;
            Enabled = defaultEnabled;
            Format = format;
        }

        public void SetEnabled(bool val)
        {
            ModSettings.Trace("FloatSetting.SetEnabled: before=" + Enabled + " requested=" + val);
            if (Enabled == val) return;
            Enabled = val;
            ModSettings.MarkDirty();
            Changed?.Invoke();
        }

        public void SetValue(float val)
        {
            ModSettings.Trace("FloatSetting.SetValue: before=" + Value + " requested=" + val);
            if (Value == val) return;
            Value = val;
            ModSettings.MarkDirty();
            Changed?.Invoke();
        }

        public string FormatValue(float maxLimit = 0) => DisplayFormatHelper.Format(Value, Format, maxLimit);
    }

    // Toggle + min/max range (e.g., "Implicits: 20% to 80%").
    public class RangeSetting
    {
        public bool Enabled { get; internal set; }
        public float Min { get; internal set; }
        public float Max { get; internal set; }
        public float MinLimit { get; }
        public float MaxLimit { get; }
        public DisplayFormat Format { get; }

        public event Action Changed;

        public RangeSetting(
            float minLimit = 0, float maxLimit = 255,
            float? defaultMin = null, float? defaultMax = null,
            bool defaultEnabled = false,
            DisplayFormat format = DisplayFormat.Raw)
        {
            MinLimit = minLimit;
            MaxLimit = maxLimit;
            Min = defaultMin ?? minLimit;
            Max = defaultMax ?? maxLimit;
            Enabled = defaultEnabled;
            Format = format;
        }

        public void SetEnabled(bool val)
        {
            ModSettings.Trace("RangeSetting.SetEnabled: before=" + Enabled + " requested=" + val);
            if (Enabled == val) return;
            Enabled = val;
            ModSettings.MarkDirty();
            Changed?.Invoke();
        }

        public void SetMin(float val)
        {
            ModSettings.Trace("RangeSetting.SetMin: before=" + Min + " requested=" + val);
            if (Min == val) return;
            Min = val;
            if (Min > Max) Max = Min;
            ModSettings.MarkDirty();
            Changed?.Invoke();
        }

        public void SetMax(float val)
        {
            ModSettings.Trace("RangeSetting.SetMax: before=" + Max + " requested=" + val);
            if (Max == val) return;
            Max = val;
            if (Max < Min) Min = Max;
            ModSettings.MarkDirty();
            Changed?.Invoke();
        }

        public string FormatRange()
        {
            return DisplayFormatHelper.Format(Min, Format, MaxLimit) + " to " + DisplayFormatHelper.Format(Max, Format, MaxLimit);
        }
    }

    // Dropdown selection -- stores selected index, provides option labels.
    public class DropdownSetting
    {
        public int Value { get; internal set; }
        public string[] Options { get; private set; }

        public event Action<int> Changed;
        internal Action<string[]> applyOptions;

        public DropdownSetting(int defaultValue = 0, string[] options = null)
        {
            Value = defaultValue;
            Options = options ?? Array.Empty<string>();
        }

        public string SelectedText => (Value >= 0 && Value < Options.Length) ? Options[Value] : null;

        public void Set(int val)
        {
            ModSettings.Trace("DropdownSetting.Set: before=" + Value + " requested=" + val);
            if (Value == val) return;
            Value = val;
            ModSettings.MarkDirty();
            Changed?.Invoke(val);
        }

        public void SetOptions(string[] options, int selectedIndex = 0)
        {
            ModSettings.Trace("DropdownSetting.SetOptions: count=" + (options?.Length ?? 0) + " selectedIndex=" + selectedIndex);
            Options = options ?? Array.Empty<string>();
            Value = selectedIndex;
            applyOptions?.Invoke(Options);
        }
    }

    // Mutually exclusive options (e.g., ForceUnique / ForceSet / ForceLegendary).
    public class RadioSetting
    {
        public readonly string[] OptionNames;
        private readonly bool[] values;

        public event Action Changed;

        public RadioSetting(params string[] names)
        {
            OptionNames = names;
            values = new bool[names.Length];
        }

        public bool IsSelected(int index) => values[index];

        public void Select(int index, bool value)
        {
            ModSettings.Trace("RadioSetting.Select: index=" + index + " name=" + OptionNames[index] + " value=" + value);
            if (value)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = (i == index);
            }
            else
            {
                values[index] = false;
            }
            ModSettings.MarkDirty();
            Changed?.Invoke();
        }

        internal bool GetValue(int index) => values[index];
        internal void SetValue(int index, bool value) { values[index] = value; }
    }

    // Button action (e.g., "Drop Item", "Reset Camera"). No persisted value.
    public class ActionBinding
    {
        public event Action Clicked;

        internal void Invoke()
        {
            ModSettings.Trace("ActionBinding.Invoke");
            Clicked?.Invoke();
        }
    }

    // Static localized text row (section header). No persisted value, no events.
    // Lets ModSettings.cs declare a header without an OnBind escape hatch.
    public class HeaderSetting
    {
        public string Text { get; internal set; }

        public HeaderSetting(string text) { Text = text ?? ""; }
        public void SetText(string text) { Text = text ?? ""; }
    }

    // Single rebindable input (keyboard key OR gamepad button). Value is a tagged string:
    //   "kb:LeftControl"  -- UnityEngine.KeyCode.ToString()
    //   "gp:A"            -- IGamepadTemplate accessor name from KeybindRewired
    //   ""                -- unbound
    public class KeybindSetting
    {
        public string Value { get; internal set; }
        public string DefaultValue { get; }

        public event Action<string> Changed;

        public KeybindSetting(string defaultValue = "")
        {
            DefaultValue = defaultValue ?? "";
            Value = DefaultValue;
        }

        public void Set(string newBinding)
        {
            ModSettings.Trace("KeybindSetting.Set: before=" + Value + " requested=" + newBinding);
            string val = newBinding ?? "";
            if (Value == val) return;
            Value = val;
            ModSettings.MarkDirty();
            Changed?.Invoke(val);
        }

        public void ResetToDefault() => Set(DefaultValue);
    }
}
