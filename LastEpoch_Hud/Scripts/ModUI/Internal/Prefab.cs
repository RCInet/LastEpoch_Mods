using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Prefab interaction: element lookups, event binding, and hierarchy traversal.
    // IL2CPP: Never replace event objects (slider.onValueChanged = new SliderEvent()).
    // Native code holds a pointer to the event. Replacing it disconnects the callback chain.
    public static class Prefab
    {
        public static GameObject Child(GameObject parent, string name)
        {
            if (parent == null) return null;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;
                if (child.name == name) return child;
            }
            return null;
        }

        public static T Component<T>(GameObject parent, string panelName, string childName) where T : Component
        {
            var panel = Child(parent, panelName);
            if (panel == null) return null;
            var obj = Child(panel, childName);
            if (obj == null) return null;
            return obj.GetComponent<T>();
        }

        // Walk a slash-separated child path: "Panel/Title/OnOff" -> root.Panel.Title.OnOff
        public static GameObject ChildPath(GameObject parent, string path)
        {
            if (parent == null || string.IsNullOrEmpty(path)) return null;
            var current = parent;
            foreach (var segment in path.Split('/'))
            {
                if (string.IsNullOrEmpty(segment)) continue;
                current = Child(current, segment);
                if (current == null) return null;
            }
            return current;
        }

        public static T ComponentAtPath<T>(GameObject parent, string path) where T : Component
        {
            var go = ChildPath(parent, path);
            return go == null ? null : go.GetComponent<T>();
        }

        // Navigate: parent -> panel -> content -> "Viewport" -> "Content"
        public static GameObject ViewportContent(GameObject parent, string panelName, string contentName)
        {
            var panel = Child(parent, panelName);
            if (panel == null) return null;
            var content = Child(panel, contentName);
            if (content == null) return null;
            var viewport = Child(content, "Viewport");
            if (viewport == null) return null;
            return Child(viewport, "Content");
        }

        // Find text label in multiple locations (prefab layouts vary).
        public static Text FindText(GameObject parent, string panelName, string toggleName)
        {
            var panel = Child(parent, panelName);
            if (panel == null) return null;

            var toggle = Child(panel, toggleName);
            if (toggle != null)
            {
                var text = Child(toggle, "Value");
                if (text != null) return text.GetComponent<Text>();
                text = Child(toggle, "Label");
                if (text != null) return text.GetComponent<Text>();
            }

            var directText = Child(panel, "Value");
            return directText != null ? directText.GetComponent<Text>() : null;
        }

        // Finds the user-visible "Label" Text child of a toggle (NOT the "Value" numeric display).
        // Used by the setting label application path -- we want to update the setting's name label,
        // not the displayed numeric value of a slider.
        public static Text FindLabel(GameObject parent, string panelName, string toggleName)
        {
            var panel = Child(parent, panelName);
            if (panel == null) return null;
            var toggle = Child(panel, toggleName);
            if (toggle == null) return null;
            var label = Child(toggle, "Label");
            return label != null ? label.GetComponent<Text>() : null;
        }

        // Applies a label string to a Text component, translating via the mod's active locale
        // dictionary when an entry exists. Falls back to the English input when the dictionary
        // is unloaded or the key is missing.
        public static void ApplyLabel(Text text, string englishLabel)
        {
            if (text == null || string.IsNullOrEmpty(englishLabel)) return;
            LocaleRegistry.Register(text, englishLabel);
            var dict = Locales.current_dictionary;
            if (dict != null && dict.TryGetValue(englishLabel, out var translated) && !string.IsNullOrEmpty(translated))
                text.text = translated;
            else
                text.text = englishLabel;
        }

        // Navigate: parent -> panel -> "Title" -> toggleName (for master toggles)
        public static Toggle ToggleInTitle(GameObject parent, string panelName, string toggleName, bool activate = false)
        {
            var panel = Child(parent, panelName);
            if (panel == null) return null;
            var title = Child(panel, "Title");
            if (title == null) return null;
            var obj = Child(title, toggleName);
            if (obj == null) return null;
            if (activate) obj.SetActive(true);
            return obj.GetComponent<Toggle>();
        }

        // Event binding -- AddListener on existing events, never create new ones in IL2CPP.

        public static void BindButton(Button btn, UnityAction action)
        {
            btn.onClick.AddListener(action);
        }

        public static void BindToggle(Toggle toggle, UnityAction<bool> action)
        {
            toggle.onValueChanged.AddListener(action);
        }

        public static void BindDropdown(Dropdown dropdown, UnityAction<int> action)
        {
            dropdown.onValueChanged.AddListener(action);
        }

        // Sliders use SliderHook (Harmony) -- AddListener doesn't fire in IL2CPP.

        public static void ForEachDescendant(GameObject root, Action<GameObject> visitor)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                var child = root.transform.GetChild(i).gameObject;
                visitor(child);
                ForEachDescendant(child, visitor);
            }
        }

        public static void ReplaceAllText(GameObject root)
        {
            if (Locales.current_dictionary == null) return;
            ForEachDescendant(root, go =>
            {
                var text = go.GetComponent<Text>();
                if (text == null) return;
                if (text.text.Length == 0) return;
                if (Array.IndexOf(Locales.igrone_str, text.text[0]) >= 0) return;
                if (Locales.current_dictionary.ContainsKey(text.text))
                    text.text = Locales.current_dictionary[text.text];
            });
        }
    }
}
