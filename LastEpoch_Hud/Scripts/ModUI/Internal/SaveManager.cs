using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Persists settings to SaveModUI.json. Groups handle their own serialization.
    [RegisterTypeInIl2Cpp]
    public class SaveManager : MonoBehaviour
    {
        public SaveManager(System.IntPtr ptr) : base(ptr) { }
        public static SaveManager instance;
        public volatile bool initialized;

        private static readonly string basePath = Directory.GetCurrentDirectory() + @"\Mods\" + Main.mod_name + @"\";
        private const string filename = "SaveModUI.json";
        private const float SaveInterval = 1f;
        private float saveTimer;

        void Awake() { instance = this; }

        void Start()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                try { Load(); }
                catch (System.Exception ex) { Main.logger_instance?.Error("ModUI SaveManager: Load failed: " + ex.Message); }
            });
        }

        void Update()
        {
            LocaleRegistry.TickIfLocaleChanged();
            if (KeybindCapture.Active) KeybindCapture.Tick();
            if (!initialized) return;
            if (!ModSettings.Dirty) return;
            // Time.unscaledDeltaTime: advances even when the game is paused (HUD open = timeScale=0)
            saveTimer += Time.unscaledDeltaTime;
            if (saveTimer < SaveInterval) return;
            saveTimer = 0f;
            ModSettings.ClearDirty();
            ModSettings.Trace("SaveManager.Update flushing (debounce hit)");
            Save();
        }

        private void Load()
        {
            string fullPath = basePath + filename;
            bool needsRewrite = false;

            if (File.Exists(fullPath))
            {
                Main.logger_instance?.Msg("ModUI SaveManager: Loading " + fullPath);
                try
                {
                    var root = JObject.Parse(File.ReadAllText(fullPath));
                    foreach (var group in ModSettings.AllGroups)
                        group.Load(root);

                    // If any registered group is missing from the on-disk file, rewrite
                    // so a new section (e.g. Debug) shows up on disk without waiting for
                    // the player to interact with one of its settings.
                    foreach (var group in ModSettings.AllGroups)
                    {
                        if (root[group.Name] == null) { needsRewrite = true; break; }
                    }
                }
                catch
                {
                    Main.logger_instance?.Warning("ModUI SaveManager: Error parsing save file, using defaults");
                    needsRewrite = true;
                }
            }
            else
            {
                Main.logger_instance?.Msg("ModUI SaveManager: No save file found, using defaults");
                needsRewrite = true;
            }

            Main.logger_instance?.Msg("ModUI SaveManager: Initialized with " + ModSettings.AllGroups.Count + " group(s)");
            initialized = true;

            if (needsRewrite)
            {
                Save();
                Main.logger_instance?.Msg("ModUI SaveManager: Wrote fresh schema to " + filename);
            }
        }

        private void Save()
        {
            var root = new JObject();
            foreach (var group in ModSettings.AllGroups)
                group.Save(root);

            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            File.WriteAllText(basePath + filename, root.ToString(Formatting.Indented));
            ModSettings.Trace("SaveManager.Save wrote " + filename);
        }

        public static void BindHud(GameObject hud_object)
        {
            BindRoot(BindingRoots.HudRootName, hud_object, RootKind.HudWithTabs);
        }

        public static void BindRoot(string name, GameObject root, RootKind kind = RootKind.Generic)
        {
            if (instance == null || root.IsNullOrDestroyed()) return;
            if (string.IsNullOrEmpty(name)) return;

            if (BindingRoots.IsSameRoot(name, root))
            {
                Main.logger_instance?.Msg("ModUI SaveManager: BindRoot " + name + " skipped (same root already bound)");
                return;
            }

            LocaleRegistry.SweepDead();
            BindingRoots.Register(name, root, kind);

            if (kind == RootKind.HudWithTabs)
                BindHudInternal(root);
            else
                BindGenericRoot(name, root);

            Main.logger_instance?.Msg("ModUI SaveManager: BindRoot " + name + " (kind=" + kind + ", groups=" + ModSettings.AllGroups.Count + ")");
        }

        private static void BindHudInternal(GameObject hud_object)
        {
            var contentRoot = Functions.GetChild(hud_object, "Content");
            var menu = Functions.GetChild(hud_object, "Menu");
            var menuContent = menu.IsNullOrDestroyed() ? null : Functions.GetChild(menu, "Content");
            if (contentRoot.IsNullOrDestroyed() || menuContent.IsNullOrDestroyed())
            {
                Main.logger_instance?.Warning("ModUI SaveManager: BindHudInternal couldn't resolve Content or Menu/Content");
                return;
            }

            TabManager.Init(contentRoot, menuContent);
            BindNonTabGroups(contentRoot);
        }

        private static void BindNonTabGroups(GameObject contentRoot)
        {
            foreach (var group in ModSettings.AllGroups)
            {
                if (group.RootName != BindingRoots.HudRootName) continue;
                if (group.HasTab || group.ContentObjectName == null) continue;
                try
                {
                    var contentObj = Prefab.Child(contentRoot, group.ContentObjectName);
                    if (contentObj != null) group.ResolveAndBind(contentObj);
                }
                catch (System.Exception ex)
                {
                    Main.logger_instance?.Error("ModUI SaveManager: Failed to bind group '" + group.Name + "': " + ex.Message);
                }
            }
        }

        private static void BindGenericRoot(string rootName, GameObject root)
        {
            foreach (var group in ModSettings.AllGroups)
            {
                if (group.RootName != rootName) continue;
                try
                {
                    var contentObj = group.ContentObjectName != null
                        ? Prefab.Child(root, group.ContentObjectName)
                        : root;
                    if (contentObj != null) group.ResolveAndBind(contentObj);
                    else Main.logger_instance?.Warning("ModUI SaveManager: BindGenericRoot '" + rootName + "' missing content '" + group.ContentObjectName + "' for group '" + group.Name + "'");
                }
                catch (System.Exception ex)
                {
                    Main.logger_instance?.Error("ModUI SaveManager: Failed to bind group '" + group.Name + "' to root '" + rootName + "': " + ex.Message);
                }
            }
        }
    }
}
