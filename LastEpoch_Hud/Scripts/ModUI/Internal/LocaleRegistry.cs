using System.Collections.Generic;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Tracks (Text, canonical English label) pairs. On language change we re-translate
    // from the canonical key instead of the current Text content (which has already been
    // translated once and so is no longer a key in any locale dictionary).
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

        public static void TickIfLocaleChanged()
        {
            var current = Locales.current_dictionary;
            if (ReferenceEquals(current, lastDict)) return;
            lastDict = current;
            ReapplyAll();
        }

        public static void SweepDead()
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                var t = entries[i].Text;
                if (t == null || t.IsNullOrDestroyed())
                    entries.RemoveAt(i);
            }
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
