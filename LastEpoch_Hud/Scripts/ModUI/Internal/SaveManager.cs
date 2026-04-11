using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Persists settings to SaveModUI.json. Groups handle their own serialization.
    // Runs independently of the legacy Save_Manager / Save.json.
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
            if (!initialized) return;
            if (!ModSettings.Dirty) return;
            saveTimer += Time.deltaTime;
            if (saveTimer < SaveInterval) return;
            saveTimer = 0f;
            ModSettings.ClearDirty();
            Save();
        }

        private void Load()
        {
            string fullPath = basePath + filename;
            if (File.Exists(fullPath))
            {
                Main.logger_instance?.Msg("ModUI SaveManager: Loading " + fullPath);
                try
                {
                    var root = JObject.Parse(File.ReadAllText(fullPath));
                    foreach (var group in ModSettings.AllGroups)
                        group.Load(root);
                }
                catch { Main.logger_instance?.Warning("ModUI SaveManager: Error parsing save file, using defaults"); }
            }
            else
            {
                Main.logger_instance?.Msg("ModUI SaveManager: No save file found, using defaults");
            }
            Main.logger_instance?.Msg("ModUI SaveManager: Initialized with " + ModSettings.AllGroups.Count + " group(s)");
            initialized = true;
        }

        private void Save()
        {
            var root = new JObject();
            foreach (var group in ModSettings.AllGroups)
                group.Save(root);

            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            File.WriteAllText(basePath + filename, root.ToString(Formatting.Indented));
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
