using System;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Capture state machine. Polled from SaveManager.Update each frame.
    // First key/button after Begin() commits and exits capture mode.
    internal static class KeybindCapture
    {
        private static KeybindSetting current;
        private static Text displayText;
        private static bool[] gamepadSnapshot;

        public static bool Active => current != null;

        public static void Begin(KeybindSetting setting, Text display)
        {
            current = setting;
            displayText = display;
            gamepadSnapshot = null;
        }

        public static void Cancel()
        {
            if (current != null && displayText != null)
                displayText.text = KeybindFormat.Friendly(current.Value);
            current = null;
            displayText = null;
            gamepadSnapshot = null;
        }

        public static void Tick()
        {
            if (current == null) return;

            if (Input.GetKeyDown(KeyCode.Escape)) { Cancel(); return; }

            if (Input.anyKeyDown && TryCaptureKeyboard()) return;
            TryCaptureGamepad();
        }

        private static bool TryCaptureKeyboard()
        {
            foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
            {
                if (kc == KeyCode.None) continue;
                if (kc == KeyCode.Escape) continue;
                if (kc == KeyCode.Mouse0 || kc == KeyCode.Mouse1) continue;
                if (!Input.GetKeyDown(kc)) continue;
                Commit("kb:" + kc.ToString());
                return true;
            }
            return false;
        }

        private static void TryCaptureGamepad()
        {
            var template = KeybindRewired.ActiveTemplate();
            if (template == null) return;

            int n = KeybindRewired.ButtonCount;

            // First Tick after Begin: snapshot current state. Don't fire on already-held buttons
            // so the user can hold a face button while clicking the capture mouse and not capture it.
            if (gamepadSnapshot == null)
            {
                gamepadSnapshot = new bool[n];
                for (int i = 0; i < n; i++) gamepadSnapshot[i] = KeybindRewired.ButtonHeldAt(template, i);
                return;
            }

            for (int i = 0; i < n; i++)
            {
                bool now = KeybindRewired.ButtonHeldAt(template, i);
                bool was = gamepadSnapshot[i];
                gamepadSnapshot[i] = now;
                if (now && !was)
                {
                    Commit("gp:" + KeybindRewired.ButtonNameAt(i));
                    return;
                }
            }
        }

        private static void Commit(string binding)
        {
            ModSettings.Trace("KeybindCapture committed: " + binding);
            var s = current;
            current = null;
            displayText = null;
            gamepadSnapshot = null;
            s.Set(binding);
        }
    }
}
