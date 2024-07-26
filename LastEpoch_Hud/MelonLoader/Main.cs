using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud
{
    public class Main : MelonLoader.MelonMod
    {
        public static MelonLoader.MelonLogger.Instance logger_instance = null;
        public const string company_name = "Eleventh Hour Games";
        public const string game_name = "Last Epoch";
        public const string mod_name = "LastEpoch_Hud";
        public const string mod_version = "4.0.8"; //LastEpoch 1.1.3
        public static bool debug = true;

        public override void OnInitializeMelon()
        {
            logger_instance = LoggerInstance;
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Scenes.SceneName = sceneName;
        }
        public override void OnLateUpdate()
        {
            if ((!Base.Initializing) && (!Base.Initialized)) { Base.Init(); }
        }
        public override void OnApplicationQuit()
        {
            Caching.ClearCache();
        }
    }
    public class Locales
    {
        public enum Selected
        {
            Unknow,
            English,
            French,
            Korean,
            German,
            Russian,
            Polish,
            Portuguese,
            Chinese,
            Spanish
        }

        public static Selected current = Selected.Unknow;

        [HarmonyPatch(typeof(Localization), "get_Locale")]
        public class Localization_get_Locale
        {
            [HarmonyPostfix]
            static void Postfix(string __result)
            {
                Selected backup = current;
                current = Selected.Unknow;
                switch (__result)
                {
                    case "English (en)": { current = Selected.English; break; }
                    case "French (fr)": { current = Selected.French; break; }
                    case "Korean (ko)": { current = Selected.Korean; break; }
                    case "German (Germany) (de-DE)": { current = Selected.German; break; }
                    case "Russian (ru)": { current = Selected.Russian; break; }
                    case "Polish (pl)": { current = Selected.Polish; break; }
                    case "Portuguese (pt)": { current = Selected.Portuguese; break; }
                    case "Chinese (Simplified) (zh)": { current = Selected.Chinese; break; }
                    case "Spanish (Spain) (es-ES)": { current = Selected.Spanish; break; }
                }
                if (current != backup)
                {
                    if (backup == Selected.Unknow) { Main.logger_instance.Msg("Locale initialized to " + current.ToString()); }
                    else { Main.logger_instance.Msg("Locale change to " + current.ToString()); }
                    
                    //Here to Update mods text
                }
            }
        }
    }
    public class Base
    {
        public static readonly string base_object_name = "BaseHud";
        public static bool Initialized = false;
        public static bool Initializing = false;

        public static void Init()
        {
            Initializing = true;
            GameObject base_object = Object.Instantiate(new GameObject(name: base_object_name), Vector3.zero, Quaternion.identity);
            Object.DontDestroyOnLoad(base_object);
            base_object.AddComponent<Scripts.Refs_Manager>();
            base_object.AddComponent<Scripts.Save_Manager>();           
            base_object.AddComponent<Scripts.Hud_Manager>();          
            base_object.AddComponent<Scripts.Mods_Manager>();
            Initialized = true;
            Initializing = false;
        }
    }
    public class Scenes
    {
        public static string SceneName = "";
        private static string[] SceneMenuNames = { "ClientSplash", "PersistentUI", "Login", "CharacterSelectScene" };
        
        public static bool IsGameScene()
        {
            if ((SceneName != "") && (!SceneMenuNames.Contains(SceneName))) { return true; }
            else { return false; }
        }
        public static bool IsCharacterSelection()
        {
            if ((SceneName != "") && (!SceneMenuNames.Contains(SceneMenuNames[3]))) { return true; }
            else { return false; }
        }
    }
    public class Extensions
    {
        public static readonly string jpg = ".jpg";
        public static readonly string png = ".png";
        public static readonly string json = ".json";
        public static readonly string prefab = ".prefab";
    }
    public static class Functions
    {
        public static bool IsNullOrDestroyed(this object obj)
        {
            try
            {
                if (obj == null) { return true; }
                else if (obj is Object unityObj && !unityObj) { return true; }
                return false;
            }
            catch { return true; }
        }
        public static GameObject GetChild(GameObject obj, string name)
        {
            GameObject result = null;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                string obj_name = obj.transform.GetChild(i).gameObject.name;
                if (obj_name == name)
                {
                    result = obj.transform.GetChild(i).gameObject;
                    break;
                }
            }

            return result;
        }
        public static GameObject GetViewportContent(GameObject obj, string panel_name, string panel_content_name)
        {
            GameObject result = null;
            GameObject panel = GetChild(obj, panel_name);
            if (!panel.IsNullOrDestroyed())
            {
                GameObject content = GetChild(panel, panel_content_name);
                if (!content.IsNullOrDestroyed())
                {
                    GameObject viewport = GetChild(content, "Viewport");
                    if (!viewport.IsNullOrDestroyed()) { result = GetChild(viewport, "Content"); }
                }
            }

            return result;
        }
        public static Toggle Get_ToggleInPanel(GameObject obj, string panel_name, string obj_name)
        {            
            Toggle result = null;
            GameObject panel = GetChild(obj, panel_name);
            if (!panel.IsNullOrDestroyed()) { result = Functions.GetChild(panel, obj_name).GetComponent<Toggle>(); }
            
            return result;
        }
        public static Slider Get_SliderInPanel(GameObject obj, string panel_name, string obj_name)
        {
            Slider result = null;
            GameObject panel = GetChild(obj, panel_name);
            if (!panel.IsNullOrDestroyed()) { result = Functions.GetChild(panel, obj_name).GetComponent<Slider>(); }

            return result;
        }
        public static Button Get_ButtonInPanel(GameObject obj, string obj_name)
        {
            Button result = null;
            GameObject panel = GetChild(obj, obj_name);
            if (!panel.IsNullOrDestroyed()) { result = panel.GetComponent<Button>(); }

            return result;
        }
        public static Text Get_TextInToggle(GameObject obj, string panel_name, string toggle_name, string obj_name)
        {
            Text result = null;
            GameObject panel = GetChild(obj, panel_name);
            if (!panel.IsNullOrDestroyed())
            {
                GameObject toogle = GetChild(panel, toggle_name);
                if (!toogle.IsNullOrDestroyed())
                {
                    result = GetChild(toogle, obj_name).GetComponent<Text>();
                }                
            }

            return result;
        }
        public static Text Get_TextInButton(GameObject obj, string button_name, string text_name)
        {
            Text result = null;
            GameObject button = GetChild(obj, button_name);
            if (!button.IsNullOrDestroyed())
            {
                result = GetChild(button, text_name).GetComponent<Text>();
            }

            return result;
        }
        public static Dropdown Get_DopboxInPanel(GameObject obj, string panel_name, string dropdown_name)
        {
            Dropdown result = null;
            GameObject panel = GetChild(obj, panel_name);
            if (!panel.IsNullOrDestroyed())
            {
                GameObject dropdown = GetChild(panel, dropdown_name);
                if (!dropdown.IsNullOrDestroyed())
                {
                    result = dropdown.GetComponent<Dropdown>();
                }
            }

            return result;
        }
        public static Toggle Get_ToggleInLabel(GameObject obj, string panel_name, string obj_name)
        {
            Toggle result = null;
            GameObject panel = GetChild(obj, panel_name);
            if (!panel.IsNullOrDestroyed())
            {
                GameObject label = GetChild(panel, "Title");
                if (!label.IsNullOrDestroyed()) { result = Functions.GetChild(label, obj_name).GetComponent<Toggle>(); }                
            }

            return result;
        }
        public static bool Check_Texture(string name)
        {
            if ((name.Substring(name.Length - Extensions.jpg.Length, Extensions.jpg.Length).ToLower() == Extensions.jpg) ||
                        (name.Substring(name.Length - Extensions.png.Length, Extensions.png.Length).ToLower() == Extensions.png))
            {
                return true;
            }
            else { return false; }
        }
        public static bool Check_Json(string name)
        {
            if (name.Substring(name.Length - Extensions.json.Length, Extensions.json.Length).ToLower() == Extensions.json)
            {
                return true;
            }
            else { return false; }
        }
        public static bool Check_Prefab(string name)
        {
            if (name.Substring(name.Length - Extensions.prefab.Length, Extensions.prefab.Length).ToLower() == Extensions.prefab)
            {
                return true;
            }
            else { return false; }
        }        
    }
}
