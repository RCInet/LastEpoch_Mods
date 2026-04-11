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

                    // If any registered group is missing from the on-disk file,
                    // schedule a rewrite so newly-declared sections (e.g. Debug)
                    // auto-appear without requiring a user interaction.
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

        // Called once from Hud_Manager.Init_Hud after the legacy prefab is instantiated.
        // Binds every registered SettingsGroup to the shared hud_object.
        public static void BindHud(GameObject hud_object)
        {
            if (instance == null || hud_object.IsNullOrDestroyed()) return;

            var contentRoot = Functions.GetChild(hud_object, "Content");
            var menu = Functions.GetChild(hud_object, "Menu");
            var menuContent = menu.IsNullOrDestroyed() ? null : Functions.GetChild(menu, "Content");
            if (contentRoot.IsNullOrDestroyed() || menuContent.IsNullOrDestroyed())
            {
                Main.logger_instance?.Warning("ModUI SaveManager: BindHud couldn't resolve Content or Menu/Content");
                return;
            }

            TabManager.Init(contentRoot, menuContent);
            BindNonTabGroups(contentRoot);
            Main.logger_instance?.Msg("ModUI SaveManager: BindHud complete (groups=" + ModSettings.AllGroups.Count + ")");
        }

        private static void BindNonTabGroups(GameObject contentRoot)
        {
            foreach (var group in ModSettings.AllGroups)
            {
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
    }
}
