using System.Collections.Generic;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Tracks every (Text, canonical English label) pair that ModUI has set, so language
    // changes can re-translate from the original key instead of from the current displayed
    // text.
    internal static class LocaleRegistry
    {
        private static readonly List<(Text Text, string EnglishLabel)> entries = new();
        private static System.Collections.Generic.Dictionary<string, string> lastDict;

        public static void Register(Text text, string englishLabel)
        {
            if (text == null || string.IsNullOrEmpty(englishLabel)) return;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Text == text)
                {
                    entries[i] = (text, englishLabel);
                    return;
                }
            }
            entries.Add((text, englishLabel));
        }

        // Polled from SaveManager.Update each frame.
        public static void TickIfLocaleChanged()
        {
            var current = Locales.current_dictionary;
            if (ReferenceEquals(current, lastDict)) return;
            lastDict = current;
            ReapplyAll();
        }

        private static void ReapplyAll()
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                var entry = entries[i];
                if (entry.Text == null || entry.Text.IsNullOrDestroyed())
                {
                    entries.RemoveAt(i);
                    continue;
                }
                Prefab.ApplyLabel(entry.Text, entry.EnglishLabel);
            }
        }
    }
}
