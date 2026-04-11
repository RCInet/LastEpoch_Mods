using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Tab creation, menu button wiring, and mutual-exclusion switching.
    public static class TabManager
    {
        private static readonly List<Tab> tabs = new();
        private static readonly Dictionary<string, Tab> tabById = new();
        private static Tab activeTab;

        public static string ActiveTabId => activeTab?.Id;

        public static void Init(GameObject contentRoot, GameObject menuContent)
        {
            Clear();

            foreach (var group in ModSettings.AllGroups)
            {
                if (!group.HasTab) continue;
                var tab = new Tab(group);
                tabs.Add(tab);
                tabById[tab.Id] = tab;
            }

            foreach (var tab in tabs)
            {
                try { tab.Init(contentRoot); }
                catch (Exception ex) { Main.logger_instance?.Error("TabManager: Failed to init tab '" + tab.Id + "': " + ex.Message); }
                tab.SetActive(false);
            }

            WireMenuButtons(menuContent);
            Main.logger_instance?.Msg("TabManager: Created " + tabs.Count + " tab(s)");
        }

        public static void ActivateTab(string tabId)
        {
            if (!tabById.TryGetValue(tabId, out var tab)) return;

            if (activeTab == tab)
            {
                tab.SetActive(false);
                activeTab = null;
                return;
            }

            DeactivateAll();
            tab.SetActive(true);
            activeTab = tab;
        }

        public static void DeactivateAll()
        {
            foreach (var tab in tabs) tab.SetActive(false);
            activeTab = null;
        }

        public static void Clear()
        {
            tabs.Clear();
            tabById.Clear();
            activeTab = null;
            SliderHook.Clear();
        }

        private static void WireMenuButtons(GameObject menuContent)
        {
            int wired = 0;
            foreach (var tab in tabs)
            {
                var btnObj = Prefab.Child(menuContent, tab.MenuButtonName);
                if (btnObj == null)
                {
                    Main.logger_instance?.Warning("TabManager: Menu button '" + tab.MenuButtonName + "' not found for tab '" + tab.Id + "'");
                    continue;
                }
                var btn = btnObj.GetComponent<Button>();
                if (btn == null) continue;
                Prefab.BindButton(btn, new Action(() => ActivateTab(tab.Id)));
                wired++;
            }
            Main.logger_instance?.Msg("TabManager: Wired " + wired + "/" + tabs.Count + " menu button(s)");
        }

        private class Tab
        {
            private readonly SettingsGroup group;
            private GameObject contentObj;

            public Tab(SettingsGroup group) { this.group = group; }
            public string Id => group.TabId;
            public string MenuButtonName => group.MenuButtonName;

            public void Init(GameObject contentRoot)
            {
                contentObj = Prefab.Child(contentRoot, group.ContentObjectName);
                if (contentObj == null) return;
                group.ResolveAndBind(contentObj);
            }

            public void SetActive(bool active)
            {
                if (contentObj != null) contentObj.SetActive(active);
            }
        }
    }
}
