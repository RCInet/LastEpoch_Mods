using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Prefab interaction: element lookups, event binding, and hierarchy traversal.
    // Null-safe -- returns null on failure, never throws.
    //
    // IL2CPP: Never replace event objects (slider.onValueChanged = new SliderEvent()).
    // Native code holds a pointer to the event. Replacing it disconnects the callback chain.
    // https://deepwiki.com/BepInEx/Il2CppInterop/5-runtime-interoperability
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

        // Navigate: parent → panel → content → "Viewport" → "Content"
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

        // Navigate: parent → panel → "Title" → toggleName (for master toggles)
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
