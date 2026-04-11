namespace LastEpoch_Hud.Scripts.ModUI
{
    // Centralized English strings for the keybind UI. Translated once via Locales.current_dictionary
    // (same path as label: parameters). Override per-setting via the resetLabel: parameter on
    // SettingsGroup.Keybind(...) when a specific button needs different wording.
    internal static class KeybindStrings
    {
        public const string ResetLabel = "Reset";
        public const string CapturePrompt = "Press any key…";
        public const string Unbound = "(none)";
        public const string GamepadPrefix = "[GP] ";
    }
}
