namespace LastEpoch_Hud.Scripts.ModUI
{
    internal static class KeybindFormat
    {
        public static string Friendly(string binding)
        {
            if (string.IsNullOrEmpty(binding)) return KeybindStrings.Unbound;
            if (binding.StartsWith("kb:")) return binding.Substring(3);
            if (binding.StartsWith("gp:")) return KeybindStrings.GamepadPrefix + binding.Substring(3);
            return binding;
        }

        // Used for the capture button display: "LeftControl" when bound,
        // "NONE (default: LeftControl)" when explicitly unbound, so the player can
        // tell at a glance both that nothing is bound AND what Reset would restore.
        public static string FriendlyWithDefault(string binding, string defaultBinding)
        {
            if (!string.IsNullOrEmpty(binding)) return Friendly(binding);
            if (string.IsNullOrEmpty(defaultBinding)) return KeybindStrings.Unbound;
            return KeybindStrings.Unbound + " (default: " + Friendly(defaultBinding) + ")";
        }
    }
}
