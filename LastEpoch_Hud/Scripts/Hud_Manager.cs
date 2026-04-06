using HarmonyLib;
using Il2Cpp;
using Il2CppLE.Data;
using Il2CppLE.Tools;
using Il2CppLE.UI.Bazaar;
using Il2CppNetworking.Multiplayer.Interactables.Portals;
using Il2CppOperationResult;
using Il2CppRewired.Components; //Gamepad
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using LastEpoch_Hud.Scripts.Mods.Maxroll;
using LastEpoch_Hud.Scripts.Mods.NewItems;
using MelonLoader;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices; //Gamepad
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts
{
    [RegisterTypeInIl2Cpp]
    public class Hud_Manager : MonoBehaviour
    {
        public Hud_Manager(System.IntPtr ptr) : base(ptr) { }
        public static Hud_Manager instance;

        public static AssetBundle asset_bundle;
        public static GameObject hud_object = null;
        public bool data_initialized = false;

        private string asset_path = Application.dataPath + "/../Mods/" + Main.mod_name + "/Assets";
        private static Canvas game_canvas = null;
        public static GameObject game_pause_menu = null;
        private static Canvas hud_canvas = null;
        private readonly string asset_bundle_name = "lastepochmods"; //Name of asset file
        private bool hud_initializing = false;
        private bool data_initializing = false;

        private bool updating = false;        
        public static bool enable = false; //Used to wait loading (Fix_PlayerLoopHelper)        

#if WINGAMEPAD
        public static PlayerMouse virtual_mouse = null;
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

#endif
#if KEYBOARD
        private bool exit = false;
#endif

        void Awake()
        {
            instance = this;
            enable = true;
            AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(asset_path, asset_bundle_name));
            asset_bundle = bundleLoadRequest.assetBundle;
            if (asset_bundle == null) { Main.logger_instance.Error("AssetBundle Error"); }
            else { Object.DontDestroyOnLoad(asset_bundle); }            
        }
        void Update()
        {
            if (!asset_bundle.IsNullOrDestroyed())
            {
                Update_Hud_Scale();
                Update_Refs();
                Update_Locale();

                if (!hud_object.IsNullOrDestroyed())
                {
                    if ((!data_initialized) && (!data_initializing)) { Init_UserData(); } //set once
                    if ((IsPauseOpen()) && (!updating))
                    {
                        updating = true;
                        Update_Hud_Content();
                        hud_object.active = true;
                        Content.Set_Active();

                        if (!Refs_Manager.epoch_input_manager.IsNullOrDestroyed())
                        {
#if KEYBOARD
                            if (!Refs_Manager.epoch_input_manager.forceDisableInput) { Refs_Manager.epoch_input_manager.forceDisableInput = true; }
                        }
                        if (Input.GetKeyDown(KeyCode.Escape)) { exit = true; }
                        if (!Hud_Base.Btn_Resume.IsNullOrDestroyed())
                        {
                            if ((Input.GetKeyUp(KeyCode.Escape)) && (exit))
                            {
                                Hud_Base.Btn_Resume.onClick.Invoke();
                                exit = false;
                            }
                        }
#endif
#if WINGAMEPAD
                            if (Refs_Manager.epoch_input_manager.forceDisableInput) { Refs_Manager.epoch_input_manager.forceDisableInput = false; }
                            if (virtual_mouse.IsNullOrDestroyed()) { virtual_mouse = Refs_Manager.epoch_input_manager.virtualMouse; }
                        }
                        if (Content.OdlForceDrop.enable)
                        {
                            VirtualKeyboard.instance.MoveTo(Content.OdlForceDrop.center_content_1, Content.OdlForceDrop.shards_filter_name);
                        }
                        if ((Input.GetKeyDown(KeyCode.Joystick1Button0)) && (!virtual_mouse.IsNullOrDestroyed())) //A
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)virtual_mouse.screenPosition.x, (uint)virtual_mouse.screenPosition.y, 0, 0);
                            }
                        }
#endif
                        updating = false;
                    }
                    else if (!updating)
                    {
                        updating = true;
                        if (hud_object.active) { hud_object.active = false; }
                        if (!Refs_Manager.epoch_input_manager.IsNullOrDestroyed())
                        {
                            if (Refs_Manager.epoch_input_manager.forceDisableInput) { Refs_Manager.epoch_input_manager.forceDisableInput = false; }
                        }
                        Content.Character.need_update = true;
                        updating = false;
                    }
                }
            }
        }
        void Init_Hud()
        {
            hud_initializing = true;
            if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Load hud object in assets"); }
            if (!asset_bundle.IsNullOrDestroyed())
            {
                string asset_name = "";
                foreach (string name in asset_bundle.GetAllAssetNames())
                {
                    if ((Functions.Check_Prefab(name)) && (name.Contains("/hud/")) && (name.Contains("hud.prefab")))
                    {
                        if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Hud prefab found"); }
                        asset_name = name;
                        break;
                    }
                }
                if (asset_name != "")
                {
                    UnityEngine.Object obj = asset_bundle.LoadAsset(asset_name);
                    GameObject prefab_object = obj.TryCast<GameObject>();             
                    if (!prefab_object.IsNullOrDestroyed())
                    {
                        prefab_object.active = false; //Hide
                        prefab_object.AddComponent<UIMouseListener>(); //Block Mouse
                        prefab_object.AddComponent<WindowFocusManager>();

                        //Instantiate
                        if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Instantiate hud prefab"); }
                        hud_object = Object.Instantiate(prefab_object, Vector3.zero, Quaternion.identity);
                        Object.DontDestroyOnLoad(hud_object);

                        if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Initialize hud refs"); }
                        Hud_Menu.Set_Events();

                        Content.content_obj = Functions.GetChild(hud_object, "Content");
                        Content.Character.Get_Refs();
                        Content.Character.Set_Events();
                        Content.Character.Set_Active(false);

                        Content.Items.Get_Refs();
                        Content.Items.Set_Events();
                        Content.Items.Set_Active(false);

                        Content.Scenes.Get_Refs();
                        Content.Scenes.Set_Events();
                        Content.Scenes.Set_Active(false);

                        Content.Skills.Get_Refs();
                        Content.Skills.Set_Events();
                        Content.Skills.Set_Active(false);

                        Content.OdlForceDrop.Get_Refs();
                        Content.OdlForceDrop.Init_BeastDropdown();
                        Content.OdlForceDrop.Set_Events();
                        Content.OdlForceDrop.Set_Active(false);

                        Content.NewItems.Get_Refs();
                        Content.NewItems.Init_Dropdowns();
                        Content.NewItems.Set_Events();
                        Content.NewItems.Set_Active(false);

                        Content.Maxroll.Get_Refs();
                        Content.Maxroll.Set_Events();
                        Content.Maxroll.Set_Active(false);
                    }
                    else { Main.logger_instance.Error("Hud Manager : Hud Prefab not found"); }
                }

                //Shard prefab
                asset_name = "";
                foreach (string name in asset_bundle.GetAllAssetNames())
                {
                    if ((Functions.Check_Prefab(name)) && (name.Contains("/hud/")) && (name.Contains("mod_shard.prefab")))
                    {
                        asset_name = name;
                        break;
                    }
                }
                if (asset_name != "")
                {
                    GameObject shard_prefab = asset_bundle.LoadAsset(asset_name).TryCast<GameObject>();
                    if (!shard_prefab.IsNullOrDestroyed())
                    {
                        if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Instantiate shard prefab"); }
                        Content.OdlForceDrop.shard_prefab = Object.Instantiate(shard_prefab, Vector3.zero, Quaternion.identity);
                        Object.DontDestroyOnLoad(Content.OdlForceDrop.shard_prefab);
                    }
                    else { Main.logger_instance.Error("Hud Manager : Shard Prefab not found"); }
                }
                else { Main.logger_instance.Error("Hud Manager : Shard Prefab name not found"); }
            }

            hud_initializing = false;
        }
        void Init_UserData()
        {
            data_initializing = true;
            if (!Save_Manager.instance.IsNullOrDestroyed())
            {
                if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Initialize user config"); }
                bool character = Content.Character.Init_UserData();
                bool items = Content.Items.Init_UserData();
                bool scenes = Content.Scenes.Init_UserData();
                bool skills = Content.Skills.Init_UserData();
                bool new_items = Content.NewItems.Init_Data();
                if ((character) && (items) && (scenes) && (skills) && (new_items))
                {
                    if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Initialized"); }
                    data_initialized = true;
                }
            }
            data_initializing = false;
        }
        void Update_Refs()
        {
            if ((hud_canvas.IsNullOrDestroyed()) && (!hud_object.IsNullOrDestroyed())) { hud_canvas = hud_object.GetComponent<Canvas>(); }
            if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
            {
                if ((game_canvas.IsNullOrDestroyed()) && (Refs_Manager.game_uibase.canvases.Count > 0)) { game_canvas = Refs_Manager.game_uibase.canvases[0]; }
                if (Scenes.IsGameScene() && ((game_pause_menu.IsNullOrDestroyed()) || (Hud_Base.Default_PauseMenu_Btns.IsNullOrDestroyed()))) { Hud_Base.Get_DefaultPauseMenu(); }
                if ((!Hud_Base.initiliazed_events) && (!game_pause_menu.IsNullOrDestroyed()) && (!Hud_Base.Default_PauseMenu_Btns.IsNullOrDestroyed())) { Hud_Base.Set_Events(); }
                if (Hud_Base.Get_DefaultPauseMenu_Open()) { Hud_Base.Toogle_DefaultPauseMenu(false); }
            }
            if (!(asset_bundle.IsNullOrDestroyed()) && (hud_object.IsNullOrDestroyed()) && (!hud_initializing)) { Init_Hud(); }
        }
        void Update_Hud_Scale()
        {
            if ((!Refs_Manager.game_uibase.IsNullOrDestroyed()) && (!game_canvas.IsNullOrDestroyed()) && (!hud_canvas.IsNullOrDestroyed()))
            {
                if (hud_canvas.scaleFactor != game_canvas.scaleFactor) { hud_canvas.scaleFactor = game_canvas.scaleFactor; }
            }
        }
        void Update_Locale()
        {
            if ((Locales.update) && (!hud_object.IsNullOrDestroyed()))
            {
                Locales.update = false;
                /*if (Locales.debug_text)
                {
                    Locales.debug_json = new System.Collections.Generic.List<string>();
                    Locales.debug_json.Add("{");
                }*/

                //need to make a function to remove all this trash
                foreach (GameObject level_0_go in Functions.GetAllChild(hud_object))
                {
                    foreach (GameObject level_1_go in Functions.GetAllChild(level_0_go))
                    {
                        ReplaceText(level_1_go);
                        foreach (GameObject level_2_go in Functions.GetAllChild(level_1_go))
                        {
                            ReplaceText(level_2_go);
                            foreach (GameObject level_3_go in Functions.GetAllChild(level_2_go))
                            {
                                ReplaceText(level_3_go);
                                foreach (GameObject level_4_go in Functions.GetAllChild(level_3_go))
                                {
                                    ReplaceText(level_4_go);
                                    foreach (GameObject level_5_go in Functions.GetAllChild(level_4_go))
                                    {
                                        ReplaceText(level_5_go);
                                        foreach (GameObject level_6_go in Functions.GetAllChild(level_5_go))
                                        {
                                            ReplaceText(level_6_go);
                                            foreach (GameObject level_7_go in Functions.GetAllChild(level_6_go))
                                            {
                                                ReplaceText(level_7_go);
                                                foreach (GameObject level_8_go in Functions.GetAllChild(level_7_go))
                                                {
                                                    ReplaceText(level_8_go);

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                /*if (Locales.debug_text)
                {
                    Locales.debug_json.Add("}");
                    string json = "";
                    foreach (string s in Locales.debug_json)
                    {
                        json += s;
                    }

                    Main.logger_instance.Msg("Copy to " + Locales.dictionnary_filename + Extensions.json);
                    Main.logger_instance.Msg(json);
                }*/
            }
        }
        void ReplaceText(GameObject go)
        {
            try
            {
                Text label = go.GetComponent<Text>();
                if (!label.IsNullOrDestroyed())
                {
                    bool ignore = false;
                    foreach (char c in label.text)
                    {
                        if (Locales.igrone_str.Contains(c)) { ignore = true; break; }
                    }
                    if (!ignore)
                    {
                        /*if (Locales.debug_text)
                        {
                            //string s = "\"" + label.text + "\": \"\", ";
                            string s = "\"" + label.text + "\": \"" + label.text + "\", "; //generate default en.json
                            if (!Locales.debug_json.Contains(s)) { Locales.debug_json.Add(s); }
                        }*/

                        if (Locales.current_dictionary != null)
                        {
                            if (Locales.current_dictionary.ContainsKey(label.text)) { label.text = Locales.current_dictionary[label.text]; }
                            //else { Main.logger_instance.Error(label.text + ", not found in dictionnary"); }
                        }
                    }
                }
            }
            catch
            {
                //Not a textbox
            }
        }
        void Update_Hud_Content()
        {
            if ((Content.Character.enable) && (Content.Character.need_update)) { Content.Character.Update_PlayerData(); }            
            if ((Content.Character.enable) && (Content.Character.controls_initialized)) { Content.Character.UpdateVisuals(); }
            if ((Content.Items.enable) && (Content.Items.controls_initialized))
            {
                Content.Items.UpdateVisuals();
                if (!Content.Items.ForceDrop.Type_Initialized) { Content.Items.ForceDrop.InitForcedrop(); }
            }            
            if ((Content.Scenes.enable) && (Content.Scenes.controls_initialized)) { Content.Scenes.UpdateVisuals(); }
            if ((Content.Skills.enable) && (Content.Skills.controls_initialized)) { Content.Skills.UpdateVisuals(); }
            if ((Content.OdlForceDrop.enable) && (Content.OdlForceDrop.initialized))
            {
                if (!Content.OdlForceDrop.Type_Initialized) { Content.OdlForceDrop.InitForcedrop(); }
                else
                {
                    Content.OdlForceDrop.implicits.active = Content.OdlForceDrop.implicits_enable;
                    Content.OdlForceDrop.implicits_border.active = Content.OdlForceDrop.implicits_enable;
                    if (!Content.OdlForceDrop.implicits_enable) { Content.OdlForceDrop.implicits_roll = false; }                    
                    Content.OdlForceDrop.implicit_0.active = Content.OdlForceDrop.implicits_roll;
                    Content.OdlForceDrop.implicit_1.active = Content.OdlForceDrop.implicits_roll;
                    Content.OdlForceDrop.implicit_2.active = Content.OdlForceDrop.implicits_roll;

                    Content.OdlForceDrop.forgin_potencial.active = Content.OdlForceDrop.forgin_potencial_enable;
                    Content.OdlForceDrop.forgin_potencial_border.active = Content.OdlForceDrop.forgin_potencial_enable;
                    if (!Content.OdlForceDrop.forgin_potencial_enable) { Content.OdlForceDrop.forgin_potencial_roll = false; }
                    Content.OdlForceDrop.forgin_potencial_value.active = Content.OdlForceDrop.forgin_potencial_roll;

                    Content.OdlForceDrop.seal.active = Content.OdlForceDrop.seal_enable;
                    Content.OdlForceDrop.seal_border.active = Content.OdlForceDrop.seal_enable;
                    if (!Content.OdlForceDrop.seal_enable) { Content.OdlForceDrop.seal_roll = false; }
                    Content.OdlForceDrop.seal_shard.active = Content.OdlForceDrop.seal_roll;
                    Content.OdlForceDrop.seal_tier.active = Content.OdlForceDrop.seal_roll;
                    Content.OdlForceDrop.seal_value.active = Content.OdlForceDrop.seal_roll;
                    if (Content.OdlForceDrop.seal_roll)
                    {
                        if (Content.OdlForceDrop.seal_id == -1) { Content.OdlForceDrop.seal_name = Content.OdlForceDrop.select_affix; }
                        if (!Content.OdlForceDrop.seal_select_text.IsNullOrDestroyed())
                        {
                            Content.OdlForceDrop.seal_select_text.text = Content.OdlForceDrop.seal_name;
                        }
                        else { Main.logger_instance.Error("seal_select_text NULLLLL"); }                            
                    }
                    else { Content.OdlForceDrop.seal_id = -1; }
                    
                    Content.OdlForceDrop.affixs.active = Content.OdlForceDrop.affixs_enable;
                    Content.OdlForceDrop.affixs_border.active = Content.OdlForceDrop.affixs_enable;
                    if (!Content.OdlForceDrop.affixs_enable) { Content.OdlForceDrop.affixs_roll = false; }
                    Content.OdlForceDrop.affixs_numbers.active = Content.OdlForceDrop.affixs_roll;
                    if ((Content.OdlForceDrop.affixs_numbers.active) && (!Content.OdlForceDrop.affixs_numbers_text.IsNullOrDestroyed()) && (!Content.OdlForceDrop.affixs_numbers_slider.IsNullOrDestroyed()))
                    {
                        Content.OdlForceDrop.affixs_numbers_text.text = System.Convert.ToInt32(Content.OdlForceDrop.affixs_numbers_slider.value).ToString();
                    }
                    if (Content.OdlForceDrop.affixs_roll)
                    {
                        if (Content.OdlForceDrop.affixs_numbers_slider.value > 0) { Content.OdlForceDrop.affix_0.active = true; }
                        else { Content.OdlForceDrop.affix_0.active = false; Content.OdlForceDrop.affix_0_id = -1; }
                        if (Content.OdlForceDrop.affixs_numbers_slider.value > 1) { Content.OdlForceDrop.affix_1.active = true; }
                        else { Content.OdlForceDrop.affix_1.active = false; Content.OdlForceDrop.affix_1_id = -1; }
                        if (Content.OdlForceDrop.affixs_numbers_slider.value > 2) { Content.OdlForceDrop.affix_2.active = true; }
                        else { Content.OdlForceDrop.affix_2.active = false; Content.OdlForceDrop.affix_2_id = -1; }
                        if (Content.OdlForceDrop.affixs_numbers_slider.value > 3) { Content.OdlForceDrop.affix_3.active = true; }
                        else { Content.OdlForceDrop.affix_3.active = false; Content.OdlForceDrop.affix_3_id = -1; }
                        if (Content.OdlForceDrop.affixs_numbers_slider.value > 4) { Content.OdlForceDrop.affix_4.active = true; }
                        else { Content.OdlForceDrop.affix_4.active = false; Content.OdlForceDrop.affix_4_id = -1; }
                        if (Content.OdlForceDrop.affixs_numbers_slider.value > 5) { Content.OdlForceDrop.affix_5.active = true; }
                        else { Content.OdlForceDrop.affix_5.active = false; Content.OdlForceDrop.affix_5_id = -1; }

                        if (Content.OdlForceDrop.affix_0_id == -1) { Content.OdlForceDrop.affix_0_name = Content.OdlForceDrop.select_affix; }
                        if (Content.OdlForceDrop.affix_1_id == -1) { Content.OdlForceDrop.affix_1_name = Content.OdlForceDrop.select_affix; }
                        if (Content.OdlForceDrop.affix_2_id == -1) { Content.OdlForceDrop.affix_2_name = Content.OdlForceDrop.select_affix; }
                        if (Content.OdlForceDrop.affix_3_id == -1) { Content.OdlForceDrop.affix_3_name = Content.OdlForceDrop.select_affix; }
                        if (Content.OdlForceDrop.affix_4_id == -1) { Content.OdlForceDrop.affix_4_name = Content.OdlForceDrop.select_affix; }
                        if (Content.OdlForceDrop.affix_5_id == -1) { Content.OdlForceDrop.affix_5_name = Content.OdlForceDrop.select_affix; }

                        if (!Content.OdlForceDrop.affix_0_select_text.IsNullOrDestroyed())
                        {
                            Content.OdlForceDrop.affix_0_select_text.text = Content.OdlForceDrop.affix_0_name;
                        }
                        else { Main.logger_instance.Error("affix_0_select_text NULLLLL"); }
                        if (!Content.OdlForceDrop.affix_1_select_text.IsNullOrDestroyed())
                        {
                            Content.OdlForceDrop.affix_1_select_text.text = Content.OdlForceDrop.affix_1_name;
                        }
                        else { Main.logger_instance.Error("affix_1_select_text NULLLLL"); }
                        if (!Content.OdlForceDrop.affix_2_select_text.IsNullOrDestroyed())
                        {
                            Content.OdlForceDrop.affix_2_select_text.text = Content.OdlForceDrop.affix_2_name;
                        }
                        else { Main.logger_instance.Error("affix_2_select_text NULLLLL"); }
                        if (!Content.OdlForceDrop.affix_3_select_text.IsNullOrDestroyed())
                        {
                            Content.OdlForceDrop.affix_3_select_text.text = Content.OdlForceDrop.affix_3_name;
                        }
                        else { Main.logger_instance.Error("affix_3_select_text NULLLLL"); }
                        if (!Content.OdlForceDrop.affix_4_select_text.IsNullOrDestroyed())
                        {
                            Content.OdlForceDrop.affix_4_select_text.text = Content.OdlForceDrop.affix_4_name;
                        }
                        else { Main.logger_instance.Error("affix_4_select_text NULLLLL"); }
                        if (!Content.OdlForceDrop.affix_5_select_text.IsNullOrDestroyed())
                        {
                            Content.OdlForceDrop.affix_5_select_text.text = Content.OdlForceDrop.affix_5_name;
                        }
                        else { Main.logger_instance.Error("affix_5_select_text NULLLLL"); }
                    }
                    else
                    {
                        Content.OdlForceDrop.affix_0.active = false;
                        Content.OdlForceDrop.affix_0_id = -1;
                        Content.OdlForceDrop.affix_1.active = false;
                        Content.OdlForceDrop.affix_1_id = -1;
                        Content.OdlForceDrop.affix_2.active = false;
                        Content.OdlForceDrop.affix_2_id = -1;
                        Content.OdlForceDrop.affix_3.active = false;
                        Content.OdlForceDrop.affix_3_id = -1;
                        Content.OdlForceDrop.affix_4.active = false;
                        Content.OdlForceDrop.affix_4_id = -1;
                        Content.OdlForceDrop.affix_5.active = false;
                        Content.OdlForceDrop.affix_5_id = -1;
                    }
                    
                    Content.OdlForceDrop.unique_mods.active = Content.OdlForceDrop.unique_mods_enable;
                    Content.OdlForceDrop.unique_mods_border.active = Content.OdlForceDrop.unique_mods_enable;
                    if (!Content.OdlForceDrop.unique_mods_enable)
                    {
                        Content.OdlForceDrop.unique_mods_roll_0 = false;
                        Content.OdlForceDrop.unique_mods_roll_1 = false;
                        Content.OdlForceDrop.unique_mods_roll_2 = false;
                        Content.OdlForceDrop.unique_mods_roll_3 = false;
                        Content.OdlForceDrop.unique_mods_roll_4 = false;
                        Content.OdlForceDrop.unique_mods_roll_5 = false;
                        Content.OdlForceDrop.unique_mods_roll_6 = false;
                        Content.OdlForceDrop.unique_mods_roll_7 = false;

                        Content.OdlForceDrop.nb_evolution.active = false;
                        Content.OdlForceDrop.beast_evolution_border.active = false;
                        Content.OdlForceDrop.beast_evolution_0_enable = false;
                        Content.OdlForceDrop.beast_evolution_1_enable = false;
                        Content.OdlForceDrop.beast_evolution_2_enable = false;
                        Content.OdlForceDrop.beast_evolution_3_enable = false;
                        Content.OdlForceDrop.beast_evolution_4_enable = false;
                        Content.OdlForceDrop.beast_evolution_5_enable = false;
                        Content.OdlForceDrop.beast_evolution_6_enable = false;
                    }
                    else if (Content.OdlForceDrop.item_unique_id == 444)
                    {
                        //Content.OdlForceDrop.unique_mods_roll_0 = true;
                        Content.OdlForceDrop.unique_mods_roll_1 = false;
                        Content.OdlForceDrop.unique_mods_roll_2 = false;
                        Content.OdlForceDrop.unique_mods_roll_3 = false;
                        Content.OdlForceDrop.unique_mods_roll_4 = false;
                        Content.OdlForceDrop.unique_mods_roll_5 = false;
                        Content.OdlForceDrop.unique_mods_roll_6 = false;
                        Content.OdlForceDrop.unique_mods_roll_7 = false;

                        Content.OdlForceDrop.nb_evolution.active = true;
                        Content.OdlForceDrop.beast_evolution_border.active = true;
                        Content.OdlForceDrop.beast_evolution_0_enable = true;
                        Content.OdlForceDrop.beast_evolution_1_enable = true;
                        Content.OdlForceDrop.beast_evolution_2_enable = true;
                        Content.OdlForceDrop.beast_evolution_3_enable = true;
                        Content.OdlForceDrop.beast_evolution_4_enable = true;
                        Content.OdlForceDrop.beast_evolution_5_enable = true;
                        Content.OdlForceDrop.beast_evolution_6_enable = true;
                    }
                    else
                    {
                        if (Content.OdlForceDrop.unique_mods_dropdown.value == 0)
                        {
                            Content.OdlForceDrop.unique_mods_roll_0 = false;
                            Content.OdlForceDrop.unique_mods_roll_1 = false;
                            Content.OdlForceDrop.unique_mods_roll_2 = false;
                            Content.OdlForceDrop.unique_mods_roll_3 = false;
                            Content.OdlForceDrop.unique_mods_roll_4 = false;
                            Content.OdlForceDrop.unique_mods_roll_5 = false;
                            Content.OdlForceDrop.unique_mods_roll_6 = false;
                            Content.OdlForceDrop.unique_mods_roll_7 = false;
                        }
                        else
                        {
                            Content.OdlForceDrop.unique_mods_roll_0 = true;
                            Content.OdlForceDrop.unique_mods_roll_1 = true;
                            Content.OdlForceDrop.unique_mods_roll_2 = true;
                            Content.OdlForceDrop.unique_mods_roll_3 = true;
                            Content.OdlForceDrop.unique_mods_roll_4 = true;
                            Content.OdlForceDrop.unique_mods_roll_5 = true;
                            Content.OdlForceDrop.unique_mods_roll_6 = true;
                            Content.OdlForceDrop.unique_mods_roll_7 = true;
                        }

                        Content.OdlForceDrop.nb_evolution.active = false;
                        Content.OdlForceDrop.beast_evolution_border.active = false;
                        Content.OdlForceDrop.beast_evolution_0_enable = false;
                        Content.OdlForceDrop.beast_evolution_1_enable = false;
                        Content.OdlForceDrop.beast_evolution_2_enable = false;
                        Content.OdlForceDrop.beast_evolution_3_enable = false;
                        Content.OdlForceDrop.beast_evolution_4_enable = false;
                        Content.OdlForceDrop.beast_evolution_5_enable = false;
                        Content.OdlForceDrop.beast_evolution_6_enable = false;
                    }
                    
                    Content.OdlForceDrop.unique_mod_0.active = Content.OdlForceDrop.unique_mods_roll_0;
                    Content.OdlForceDrop.unique_mod_1.active = Content.OdlForceDrop.unique_mods_roll_1;
                    Content.OdlForceDrop.unique_mod_2.active = Content.OdlForceDrop.unique_mods_roll_2;
                    Content.OdlForceDrop.unique_mod_3.active = Content.OdlForceDrop.unique_mods_roll_3;
                    Content.OdlForceDrop.unique_mod_4.active = Content.OdlForceDrop.unique_mods_roll_4;
                    Content.OdlForceDrop.unique_mod_5.active = Content.OdlForceDrop.unique_mods_roll_5;
                    Content.OdlForceDrop.unique_mod_6.active = Content.OdlForceDrop.unique_mods_roll_6;
                    Content.OdlForceDrop.unique_mod_7.active = Content.OdlForceDrop.unique_mods_roll_7;

                    Content.OdlForceDrop.beast_evolution_0.active = Content.OdlForceDrop.beast_evolution_0_enable;
                    Content.OdlForceDrop.beast_evolution_1.active = Content.OdlForceDrop.beast_evolution_1_enable;
                    Content.OdlForceDrop.beast_evolution_2.active = Content.OdlForceDrop.beast_evolution_2_enable;
                    Content.OdlForceDrop.beast_evolution_3.active = Content.OdlForceDrop.beast_evolution_3_enable;
                    Content.OdlForceDrop.beast_evolution_4.active = Content.OdlForceDrop.beast_evolution_4_enable;
                    Content.OdlForceDrop.beast_evolution_5.active = Content.OdlForceDrop.beast_evolution_5_enable;
                    Content.OdlForceDrop.beast_evolution_6.active = Content.OdlForceDrop.beast_evolution_6_enable;

                    if (Content.OdlForceDrop.beast_evolution_0_enable)
                    {
                        if (Content.OdlForceDrop.beast_evolution_0_dropdown.value == 0) { Content.OdlForceDrop.beast_evolution_select_0_enable = false; }
                        else { Content.OdlForceDrop.beast_evolution_select_0_enable = true; }
                    }
                    else { Content.OdlForceDrop.beast_evolution_select_0_enable = false; }
                    if (Content.OdlForceDrop.beast_evolution_1_enable)
                    {
                        if (Content.OdlForceDrop.beast_evolution_1_dropdown.value == 0) { Content.OdlForceDrop.beast_evolution_select_1_enable = false; }
                        else { Content.OdlForceDrop.beast_evolution_select_1_enable = true; }
                    }
                    else { Content.OdlForceDrop.beast_evolution_select_1_enable = false; }
                    if (Content.OdlForceDrop.beast_evolution_2_enable)
                    {
                        if (Content.OdlForceDrop.beast_evolution_2_dropdown.value == 0) { Content.OdlForceDrop.beast_evolution_select_2_enable = false; }
                        else { Content.OdlForceDrop.beast_evolution_select_2_enable = true; }
                    }
                    else { Content.OdlForceDrop.beast_evolution_select_2_enable = false; }
                    if (Content.OdlForceDrop.beast_evolution_3_enable)
                    {
                        if (Content.OdlForceDrop.beast_evolution_3_dropdown.value == 0) { Content.OdlForceDrop.beast_evolution_select_3_enable = false; }
                        else { Content.OdlForceDrop.beast_evolution_select_3_enable = true; }
                    }
                    else { Content.OdlForceDrop.beast_evolution_select_3_enable = false; }
                    if (Content.OdlForceDrop.beast_evolution_4_enable)
                    {
                        if (Content.OdlForceDrop.beast_evolution_4_dropdown.value == 0) { Content.OdlForceDrop.beast_evolution_select_4_enable = false; }
                        else { Content.OdlForceDrop.beast_evolution_select_4_enable = true; }
                    }
                    else { Content.OdlForceDrop.beast_evolution_select_4_enable = false; }
                    if (Content.OdlForceDrop.beast_evolution_5_enable)
                    {
                        if (Content.OdlForceDrop.beast_evolution_5_dropdown.value == 0) { Content.OdlForceDrop.beast_evolution_select_5_enable = false; }
                        else { Content.OdlForceDrop.beast_evolution_select_5_enable = true; }
                    }
                    else { Content.OdlForceDrop.beast_evolution_select_5_enable = false; }
                    if (Content.OdlForceDrop.beast_evolution_6_enable)
                    {
                        if (Content.OdlForceDrop.beast_evolution_6_dropdown.value == 0) { Content.OdlForceDrop.beast_evolution_select_6_enable = false; }
                        else { Content.OdlForceDrop.beast_evolution_select_6_enable = true; }
                    }
                    else { Content.OdlForceDrop.beast_evolution_select_6_enable = false; }

                    Content.OdlForceDrop.beast_evolution_select_0.active = Content.OdlForceDrop.beast_evolution_select_0_enable;
                    Content.OdlForceDrop.beast_evolution_select_1.active = Content.OdlForceDrop.beast_evolution_select_1_enable;
                    Content.OdlForceDrop.beast_evolution_select_2.active = Content.OdlForceDrop.beast_evolution_select_2_enable;
                    Content.OdlForceDrop.beast_evolution_select_3.active = Content.OdlForceDrop.beast_evolution_select_3_enable;
                    Content.OdlForceDrop.beast_evolution_select_4.active = Content.OdlForceDrop.beast_evolution_select_4_enable;
                    Content.OdlForceDrop.beast_evolution_select_5.active = Content.OdlForceDrop.beast_evolution_select_5_enable;
                    Content.OdlForceDrop.beast_evolution_select_6.active = Content.OdlForceDrop.beast_evolution_select_6_enable;

                    Content.OdlForceDrop.legenday_potencial.active = Content.OdlForceDrop.legenday_potencial_enable;
                    Content.OdlForceDrop.legenday_potencial_border.active = Content.OdlForceDrop.legenday_potencial_enable;
                    if (!Content.OdlForceDrop.legenday_potencial_enable) { Content.OdlForceDrop.legenday_potencial_roll = false; }
                    Content.OdlForceDrop.legenday_potencial_value.active = Content.OdlForceDrop.legenday_potencial_roll;

                    Content.OdlForceDrop.weaver_will.active = Content.OdlForceDrop.weaver_will_enable;
                    Content.OdlForceDrop.weaver_will_border.active = Content.OdlForceDrop.weaver_will_enable;
                    if (!Content.OdlForceDrop.weaver_will_enable) { Content.OdlForceDrop.weaver_will_roll = false; }
                    Content.OdlForceDrop.weaver_will_value.active = Content.OdlForceDrop.weaver_will_roll;

                    Content.OdlForceDrop.corrupted.active = Content.OdlForceDrop.corrupted_enable;
                    Content.OdlForceDrop.corrupted_border.active = Content.OdlForceDrop.corrupted_enable;

                    Content.OdlForceDrop.quantity.active = Content.OdlForceDrop.quantity_enable;
                    Content.OdlForceDrop.quantity_border.active = Content.OdlForceDrop.quantity_enable;
                    Content.OdlForceDrop.quantity_text.text = "";
                    if ((!Content.OdlForceDrop.quantity_text.IsNullOrDestroyed()) && (!Content.OdlForceDrop.forcedrop_quantity_slider.IsNullOrDestroyed()))
                    {
                        Content.OdlForceDrop.quantity_text.text = System.Convert.ToInt32(Content.OdlForceDrop.forcedrop_quantity_slider.value).ToString();
                    }
                    else { Main.logger_instance.Error("affix_5_select_text NULLLLL"); }
                }
            }
        }
        
        public static bool IsPauseOpen()
        {
            if (!game_pause_menu.IsNullOrDestroyed()) { return game_pause_menu.active; }
            else { return false; }
        }

        public class Hooks
        {
            //Select Shards
            [HarmonyPatch(typeof(Button), "Press")]
            public class Button_Press
            {
                [HarmonyPostfix]
                static void Postfix(ref Button __instance)
                {
                    if (Content.OdlForceDrop.enable)
                    {
                        if (__instance.name.Contains(Content.OdlForceDrop.shard_btn_name))
                        {
                            try
                            {
                                int i = System.Convert.ToInt32(__instance.name.Split('_')[1]);
                                GameObject shard_name_object = Functions.GetChild(__instance.gameObject, "shard_name");
                                if (!shard_name_object.IsNullOrDestroyed())
                                {
                                    GameObject text = Functions.GetChild(shard_name_object, "Text");
                                    if (!text.IsNullOrDestroyed())
                                    {
                                        Text shard_name = text.GetComponent<Text>();
                                        Content.OdlForceDrop.SelectShard(i, shard_name.text);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            }

            //All Hooks have to be replace by Unity Actions
            [HarmonyPatch(typeof(Toggle), "OnPointerClick")]
            public class Toggle_OnPointerClick
            {
                [HarmonyPostfix]
                static void Postfix(ref Toggle __instance, UnityEngine.EventSystems.PointerEventData __0)
                {
                    if ((!hud_object.IsNullOrDestroyed()) && (!Save_Manager.instance.IsNullOrDestroyed()) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
                    {
                        if (hud_object.active) //&& (!Save_Manager.instance.data.IsNullOrDestroyed()))
                        {
                            if (__instance.name.Contains("Toggle_Character_"))
                            {
                                switch (__instance.name)
                                {
                                    case "Toggle_Character_Data_Died": { if ((!Refs_Manager.player_data.IsNullOrDestroyed()) && (!Content.Character.Data.died_toggle.IsNullOrDestroyed())) { Refs_Manager.player_data.Died = Content.Character.Data.died_toggle.isOn; } break; }
                                    case "Toggle_Character_Data_Hardcore": { if ((!Refs_Manager.player_data.IsNullOrDestroyed()) && (!Content.Character.Data.hardcore_toggle.IsNullOrDestroyed())) { Refs_Manager.player_data.Hardcore = Content.Character.Data.hardcore_toggle.isOn; } break; }
                                    case "Toggle_Character_Data_Masochist": { if ((!Refs_Manager.player_data.IsNullOrDestroyed()) && (!Content.Character.Data.masochist_toggle.IsNullOrDestroyed())) { Refs_Manager.player_data.Masochist = Content.Character.Data.masochist_toggle.isOn; } break; }
                                    case "Toggle_Character_Data_Portal": { if ((!Refs_Manager.player_data.IsNullOrDestroyed()) && (!Content.Character.Data.portal_toggle.IsNullOrDestroyed())) { Refs_Manager.player_data.PortalUnlocked = Content.Character.Data.portal_toggle.isOn; } break; }
                                    case "Toggle_Character_Data_SoloChallenge": { if ((!Refs_Manager.player_data.IsNullOrDestroyed()) && (!Content.Character.Data.solo_toggle.IsNullOrDestroyed())) { Refs_Manager.player_data.SoloChallenge = Content.Character.Data.solo_toggle.isOn; } break; }

                                    case "Toggle_Character_Buffs_Enable": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_Mod = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_MoveSpeed": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_MoveSpeed_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_Damage": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_Damage_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_AttackSpeed": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_AttackSpeed_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_CastingSpeed": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_CastSpeed_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_CriticalChance": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_CriticalChance_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_CriticalMultiplier": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_CriticalMultiplier_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_HealthRegen": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_HealthRegen_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_ManaRegen": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_ManaRegen_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_Strenght": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_Str_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_Intelligence": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_Int_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_Dexterity": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_Dex_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_Vitality": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_Vit_Buff = __instance.isOn; break; }
                                    case "Toggle_Character_Buffs_Attunement": { Save_Manager.instance.data.Character.PermanentBuffs.Enable_Att_Buff = __instance.isOn; break; }
                                }
                            }
                            else if (__instance.name.Contains("Toggle_Items_"))
                            {
                                switch (__instance.name)
                                {
                                    case "Toggle_Items_Drop_ForceUnique":
                                        {
                                            if (__instance.isOn)
                                            {
                                                Save_Manager.instance.data.Items.Drop.Enable_ForceSet = false;
                                                Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary = false;
                                                Content.Items.Drop.force_set_toggle.isOn = false;
                                                Content.Items.Drop.force_legendary_toggle.isOn = false;
                                            }
                                            Save_Manager.instance.data.Items.Drop.Enable_ForceUnique = __instance.isOn;
                                            break;
                                        }
                                    case "Toggle_Items_Drop_ForceSet":
                                        {
                                            if (__instance.isOn)
                                            {
                                                Save_Manager.instance.data.Items.Drop.Enable_ForceUnique = false;
                                                Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary = false;
                                                Content.Items.Drop.force_unique_toggle.isOn = false;
                                                Content.Items.Drop.force_legendary_toggle.isOn = false;
                                            }
                                            Save_Manager.instance.data.Items.Drop.Enable_ForceSet = __instance.isOn;
                                            break;
                                        }
                                    case "Toggle_Items_Drop_ForceLegendary":
                                        {
                                            if (__instance.isOn)
                                            {
                                                Save_Manager.instance.data.Items.Drop.Enable_ForceUnique = false;
                                                Save_Manager.instance.data.Items.Drop.Enable_ForceSet = false;
                                                Content.Items.Drop.force_set_toggle.isOn = false;
                                                Content.Items.Drop.force_unique_toggle.isOn = false;
                                            }
                                            Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary = __instance.isOn;
                                            break;
                                        }
                                    case "Toggle_Items_Drop_Implicits": { Save_Manager.instance.data.Items.Drop.Enable_Implicits = __instance.isOn; break; }
                                    case "Toggle_Items_Drop_ForginPotencial": { Save_Manager.instance.data.Items.Drop.Enable_ForginPotencial = __instance.isOn; break; }
                                    case "Toggle_Items_Drop_ForceSeal": { Save_Manager.instance.data.Items.Drop.Enable_ForceSeal = __instance.isOn; break; }
                                    case "Toggle_Items_Drop_SealTier": { Save_Manager.instance.data.Items.Drop.Enable_SealTier = __instance.isOn; break; }
                                    case "Toggle_Items_Drop_SealValue": { Save_Manager.instance.data.Items.Drop.Enable_SealValue = __instance.isOn; break; }
                                    case "Toggle_Items_Drop_NbAffixes": { Save_Manager.instance.data.Items.Drop.Enable_AffixCount = __instance.isOn; break; }
                                    case "Toggle_Items_Drop_AffixesTiers": { Save_Manager.instance.data.Items.Drop.Enable_AffixTiers = __instance.isOn; break; }
                                    case "Toggle_Items_Drop_AffixesValues": { Save_Manager.instance.data.Items.Drop.Enable_AffixValues = __instance.isOn; break; }
                                    case "Toggle_Items_Drop_UniqueMods": { Save_Manager.instance.data.Items.Drop.Enable_UniqueMods = __instance.isOn; break; }
                                    case "Toggle_Items_Drop_LegendaryPotencial": { Save_Manager.instance.data.Items.Drop.Enable_LegendaryPotencial = __instance.isOn; break; }
                                    case "Toggle_Items_Drop_WeaverWill": { Save_Manager.instance.data.Items.Drop.Enable_WeaverWill = __instance.isOn; break; }

                                    case "Toggle_Items_Pickup_AutoPickup_Gold": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Gold = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoPickup_Keys": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Keys = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoPickup_Pots": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Potions = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoPickup_XpTome": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_XpTome = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoPickup_FavorTome": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_FavorTome = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoPickup_MemoryAmber": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_MemoryAmber = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoPickup_WovenEchoes": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_WovenEchoes = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoPickup_Materials": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Materials = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoPickup_Filters": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_FromFilter = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoStore_OnDrop": { Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_OnDrop = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoStore_OnInventoryOpen": { Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_OnInventoryOpen = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoStore_Timer": { Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_Timer = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoSell_FromFilter": { Save_Manager.instance.data.Items.Pickup.Enable_AutoSell_FromFilter = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoShatter_FromFilter": { Save_Manager.instance.data.Items.Pickup.Enable_AutoShatter_FromFilter = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoShatter_Rune": { Save_Manager.instance.data.Items.Pickup.Enable_AutoShatter_UseRune = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_Range_Pickup": { Save_Manager.instance.data.Items.Pickup.Enable_RangePickup = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_Hide_Notifications": { Save_Manager.instance.data.Items.Pickup.Enable_HideMaterialsNotifications = __instance.isOn; break; }

                                    case "Toggle_Items_Craft_Enable": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Mod = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_ForginPotencial": { Save_Manager.instance.data.Items.CraftingSlot.Enable_ForginPotencial = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_Implicit0": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Implicit_0 = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_Implicit1": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Implicit_1 = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_Implicit2": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Implicit_2 = __instance.isOn; break; }

                                    case "Toggle_Items_Craft_SealTier": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Seal_Tier = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_SealValue": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Seal_Value = __instance.isOn; break; }

                                    case "Toggle_Items_Craft_AffixTier0": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_0_Tier = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_AffixTier1": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_1_Tier = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_AffixTier2": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_2_Tier = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_AffixTier3": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_3_Tier = __instance.isOn; break; }

                                    case "Toggle_Items_Craft_AffixValue0": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_0_Value = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_AffixValue1": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_1_Value = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_AffixValue2": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_2_Value = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_AffixValue3": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_3_Value = __instance.isOn; break; }

                                    case "Toggle_Items_Craft_UniqueMod0": { Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_0 = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_UniqueMod1": { Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_1 = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_UniqueMod2": { Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_2 = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_UniqueMod3": { Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_3 = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_UniqueMod4": { Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_4 = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_UniqueMod5": { Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_5 = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_UniqueMod6": { Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_6 = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_UniqueMod7": { Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_7 = __instance.isOn; break; }

                                    case "Toggle_Items_Craft_LegendaryPotencial": { Save_Manager.instance.data.Items.CraftingSlot.Enable_LegendaryPotencial = __instance.isOn; break; }
                                    case "Toggle_Items_Craft_WeaverWill": { Save_Manager.instance.data.Items.CraftingSlot.Enable_WeaverWill = __instance.isOn; break; }
                                }
                            }
                            else if (__instance.name.Contains("Toggle_Scenes_"))
                            {
                                switch (__instance.name)
                                {
                                    case "Toggle_Scenes_Camera_Enable": { Save_Manager.instance.data.Scenes.Camera.Enable_Mod = __instance.isOn; break; }
                                    case "Toggle_Scenes_Camera_ZoomMinimum": { Save_Manager.instance.data.Scenes.Camera.Enable_ZoomMinimum = __instance.isOn; break; }
                                    case "Toggle_Scenes_Camera_ZoomPerScroll": { Save_Manager.instance.data.Scenes.Camera.Enable_ZoomPerScroll = __instance.isOn; break; }
                                    case "Toggle_Scenes_Camera_ZoomSpeed": { Save_Manager.instance.data.Scenes.Camera.Enable_ZoomSpeed = __instance.isOn; break; }
                                    case "Toggle_Scenes_Camera_DefaultRotation": { Save_Manager.instance.data.Scenes.Camera.Enable_DefaultRotation = __instance.isOn; break; }
                                    case "Toggle_Scenes_Camera_OffsetMinimum": { Save_Manager.instance.data.Scenes.Camera.Enable_OffsetMinimum = __instance.isOn; break; }
                                    case "Toggle_Scenes_Camera_OffsetMaximum": { Save_Manager.instance.data.Scenes.Camera.Enable_OffsetMaximum = __instance.isOn; break; }
                                    case "Toggle_Scenes_Camera_AngleMinimum": { Save_Manager.instance.data.Scenes.Camera.Enable_AngleMinimum = __instance.isOn; break; }
                                    case "Toggle_Scenes_Camera_AngleMaximum": { Save_Manager.instance.data.Scenes.Camera.Enable_AngleMaximum = __instance.isOn; break; }
                                    case "Toggle_Scenes_Camera_LoadOnStart": { Save_Manager.instance.data.Scenes.Camera.Enable_LoadOnStart = __instance.isOn; break; }

                                    case "Toggle_Scenes_Dungeons_EnterWithoutKey": { Save_Manager.instance.data.Scenes.Dungeons.Enable_EnterWithoutKey = __instance.isOn; break; }

                                    case "Toggle_Scenes_Minimap_MaxZoomOut": { Save_Manager.instance.data.Scenes.Minimap.Enable_MaxZoomOut = __instance.isOn; break; }
                                    case "Toggle_Scenes_Minimap_RemoveFogOfWar": { Save_Manager.instance.data.Scenes.Minimap.Enable_RemoveFogOfWar = __instance.isOn; break; }

                                    case "Toggle_Scenes_Monoliths_MaxStability": { Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStability = __instance.isOn; break; }
                                    case "Toggle_Scenes_Monoliths_MobsDensity": { Save_Manager.instance.data.Scenes.Monoliths.Enable_MobsDensity = __instance.isOn; break; }
                                    case "Toggle_Scenes_Monoliths_MobsDefeatOnStart": { Save_Manager.instance.data.Scenes.Monoliths.Enable_MobsDefeatOnStart = __instance.isOn; break; }
                                    case "Toggle_Scenes_Monoliths_BlessingSlots": { Save_Manager.instance.data.Scenes.Monoliths.Enable_BlessingSlots = __instance.isOn; break; }
                                    case "Toggle_Scenes_Monoliths_MaxStabilityOnStart": { Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStabilityOnStart = __instance.isOn; break; }
                                    case "Toggle_Scenes_Monoliths_MaxStabilityOnStabilityChanged": { Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStabilityOnStabilityChanged = __instance.isOn; break; }
                                    case "Toggle_Scenes_Monoliths_ObjectiveReveal": { Save_Manager.instance.data.Scenes.Monoliths.Enable_ObjectiveReveal = __instance.isOn; break; }
                                    case "Toggle_Scenes_Monoliths_CompleteObjective": { Save_Manager.instance.data.Scenes.Monoliths.Enable_CompleteObjective = __instance.isOn; break; }
                                    case "Toggle_Scenes_Monoliths_NoLostWhenDie": { Save_Manager.instance.data.Scenes.Monoliths.Enable_NoLostWhenDie = __instance.isOn; break; }
                                }
                            }
                            else
                            {
                                switch (__instance.name)
                                {
                                    //Skills
                                    case "Toggle_RemoveManaCost": { Save_Manager.instance.data.Skills.Enable_RemoveManaCost = __instance.isOn; break; }
                                    case "Toggle_RemoveChannelCost": { Save_Manager.instance.data.Skills.Enable_RemoveChannelCost = __instance.isOn; break; }
                                    case "Toggle_ManaRegenWhenChanneling": { Save_Manager.instance.data.Skills.Enable_NoManaRegenWhileChanneling = __instance.isOn; break; }
                                    case "Toggle_DontStopWhenOOM": { Save_Manager.instance.data.Skills.Enable_StopWhenOutOfMana = __instance.isOn; break; }
                                    case "Toggle_NoCooldown": { Save_Manager.instance.data.Skills.Enable_RemoveCooldown = __instance.isOn; break; }
                                    case "Toggle_UnlockAllSkills": { Save_Manager.instance.data.Skills.Enable_AllSkills = __instance.isOn; break; }
                                    case "Toggle_RemoveNodeRequirements": { Save_Manager.instance.data.Skills.Disable_NodeRequirement = __instance.isOn; break; }
                                    case "Toggle_SpecializationSlots": { Save_Manager.instance.data.Skills.Enable_SpecializationSlots = __instance.isOn; break; }
                                    case "Toggle_SkillLevel": { Save_Manager.instance.data.Skills.Enable_SkillLevel = __instance.isOn; break; }
                                    case "Toggle_PassivePoints": { Save_Manager.instance.data.Skills.Enable_PassivePoints = __instance.isOn; break; }
                                    case "Toggle_NoTarget": { Save_Manager.instance.data.Skills.MovementSkills.Enable_NoTarget = __instance.isOn; break; }
                                    case "Toggle_ImmuneDuringMovement": { Save_Manager.instance.data.Skills.MovementSkills.Enable_ImmuneDuringMovement = __instance.isOn; break; }
                                    case "Toggle_DisableSimplePath": { Save_Manager.instance.data.Skills.MovementSkills.Disable_SimplePath = __instance.isOn; break; }
                                    //Companions
                                    case "Toggle_MaximumCompanions": { Save_Manager.instance.data.Skills.Companion.Enable_Limit = __instance.isOn; break; }
                                    case "Toggle_Wolf_SummonToMax": { Save_Manager.instance.data.Skills.Companion.Wolf.Enable_SummonMax = __instance.isOn; break; }
                                    case "Toggle_Wolf_SummonLimit": { Save_Manager.instance.data.Skills.Companion.Wolf.Enable_SummonLimit = __instance.isOn; break; }
                                    case "Toggle_Wolf_StunImmunity": { Save_Manager.instance.data.Skills.Companion.Wolf.Enable_StunImmunity = __instance.isOn; break; }
                                    case "Toggle_Scorpions_SummonLimit": { Save_Manager.instance.data.Skills.Companion.Scorpion.Enable_BabyQuantity = __instance.isOn; break; }
                                    //Minions
                                    case "Toggle_Skeleteon_SummonQuantityFromPassive": { Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsFromPassives = __instance.isOn; break; }
                                    case "Toggle_Skeleteon_SummonQuantityFromSkillTree": { Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsFromSkillTree = __instance.isOn; break; }
                                    case "Toggle_Skeleteon_SummonQuantityPerCast": { Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsPerCast = __instance.isOn; break; }
                                    case "Toggle_Skeleteon_ChanceToResummonOnDeath": { Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_chanceToResummonOnDeath = __instance.isOn; break; }
                                    case "Toggle_Skeleton_ForceArcher": { Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceArcher = __instance.isOn; break; }
                                    case "Toggle_Skeleton_ForceBrawler": { Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceBrawler = __instance.isOn; break; }
                                    case "Toggle_Skeleton_ForceWarrior": { Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceWarrior = __instance.isOn; break; }

                                    case "Toggle_Wraiths_SummonMax": { Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_additionalMaxWraiths = __instance.isOn; break; }
                                    case "Toggle_Wraiths_Delayed": { Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_delayedWraiths = __instance.isOn; break; }
                                    case "Toggle_Wraiths_CastSpeed": { Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_increasedCastSpeed = __instance.isOn; break; }
                                    case "Toggle_Wraiths_DisableLimitTo2": { Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_limitedTo2Wraiths = __instance.isOn; break; }
                                    case "Toggle_Wraiths_DisableDecay": { Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_wraithsDoNotDecay = __instance.isOn; break; }

                                    case "Toggle_Mages_SummonQuantityFromPassive": { Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromPassives = __instance.isOn; break; }
                                    case "Toggle_Mages_SummonQuantityFromSkillTree": { Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromSkillTree = __instance.isOn; break; }
                                    case "Toggle_Mages_SummonQuantityFromItems": { Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromItems = __instance.isOn; break; }
                                    case "Toggle_Mages_SummonPerCast": { Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsPerCast = __instance.isOn; break; }
                                    case "Toggle_Mages_ChanceForExtraPorjectiles": { Save_Manager.instance.data.Skills.Minions.Mages.Enable_chanceForTwoExtraProjectiles = __instance.isOn; break; }
                                    case "Toggle_Mages_ForceCryomancer": { Save_Manager.instance.data.Skills.Minions.Mages.Enable_forceCryomancer = __instance.isOn; break; }
                                    case "Toggle_Mages_ForceDeathKnight": { Save_Manager.instance.data.Skills.Minions.Mages.Enable_forceDeathKnight = __instance.isOn; break; }
                                    case "Toggle_Mages_ForcePyromancer": { Save_Manager.instance.data.Skills.Minions.Mages.Enable_forcePyromancer = __instance.isOn; break; }

                                    case "Toggle_BoneGolem_GolemPerSkeletons": { Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_addedGolemsPer4Skeletons = __instance.isOn; break; }
                                    case "Toggle_BoneGolem_SelfResurectChance": { Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_selfResurrectChance = __instance.isOn; break; }
                                    case "Toggle_BoneGolem_IncreaseFireAura": { Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_increasedFireAuraArea = __instance.isOn; break; }
                                    case "Toggle_BoneGolem_IncreaseArmorAura": { Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_undeadArmorAura = __instance.isOn; break; }
                                    case "Toggle_BoneGolem_IncreaseMoveSpeedAura": { Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_undeadMovespeedAura = __instance.isOn; break; }
                                    case "Toggle_BoneGolem_IncreaseMoveSpeed": { Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_increasedMoveSpeed = __instance.isOn; break; }
                                    case "Toggle_BoneGolem_Twins": { Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_twins = __instance.isOn; break; }
                                    case "Toggle_BoneGolem_Slam": { Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_hasSlamAttack = __instance.isOn; break; }

                                    case "Toggle_VolatileZombies_ChanceOnMinionDeath": { Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastFromMinionDeath = __instance.isOn; break; }
                                    case "Toggle_VolatileZombies_InfernalShadeChance": { Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastInfernalShadeOnDeath = __instance.isOn; break; }
                                    case "Toggle_VolatileZombies_MarrowShardsChance": { Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastMarrowShardsOnDeath = __instance.isOn; break; }

                                    case "Toggle_DreadShades_Duration": { Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Duration = __instance.isOn; break; }
                                    case "Toggle_DreadShades_Max": { Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Max = __instance.isOn; break; }
                                    case "Toggle_DreadShades_Decay": { Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_ReduceDecay = __instance.isOn; break; }
                                    case "Toggle_DreadShades_Radius": { Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Radius = __instance.isOn; break; }
                                    case "Toggle_DreadShades_DisableLimit": { Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_DisableLimit = __instance.isOn; break; }
                                    case "Toggle_DreadShades_DisableHealthDrain": { Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_DisableHealthDrain = __instance.isOn; break; }
                                }
                            }
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(Slider), "set_value")]
            public class Slider_set_value
            {
                [HarmonyPostfix]
                static void Postfix(ref Slider __instance, float __0)
                {
                    if (!(hud_object.IsNullOrDestroyed()) && (!Save_Manager.instance.IsNullOrDestroyed()))
                    {
                        if ((hud_object.active) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
                        {
                            if (__instance.name.Contains("Slider_Character_"))
                            {
                                switch (__instance.name)
                                {
                                    case "Slider_Character_Cheats_AutoPotions":
                                        {
                                            Save_Manager.instance.data.Character.Cheats.autoPot = __0;
                                            //Content.Character.Cheats.autopot_text.text = (int)((Save_Manager.instance.data.Character.Cheats.autoPot / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Cheats_DensityMultiplier":
                                        {
                                            Save_Manager.instance.data.Character.Cheats.DensityMultiplier = __0;
                                            //Content.Character.Cheats.density_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.DensityMultiplier);
                                            break;
                                        }
                                    case "Slider_Character_Cheats_ExperienceMultiplier":
                                        {
                                            Save_Manager.instance.data.Character.Cheats.ExperienceMultiplier = __0;
                                            //Content.Character.Cheats.experience_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.ExperienceMultiplier);
                                            break;
                                        }
                                    case "Slider_Character_Cheats_AbilityMultiplier":
                                        {
                                            Save_Manager.instance.data.Character.Cheats.AbilityMultiplier = __0;
                                            //Content.Character.Cheats.ability_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.AbilityMultiplier);
                                            break;
                                        }
                                    case "Slider_Character_Cheats_FavorMultiplier":
                                        {
                                            Save_Manager.instance.data.Character.Cheats.FavorMultiplier = __0;
                                            //Content.Character.Cheats.favor_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.FavorMultiplier);
                                            break;
                                        }
                                    case "Slider_Character_Cheats_MemoryAmberMultiplier":
                                        {
                                            Save_Manager.instance.data.Character.Cheats.MemoryAmberMultiplier = (uint)__0;
                                            //Content.Character.Cheats.favor_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.FavorMultiplier);
                                            break;
                                        }
                                    case "Slider_Character_Cheats_ItemDropMultiplier":
                                        {
                                            Save_Manager.instance.data.Character.Cheats.ItemDropMultiplier = __0;
                                            //Content.Character.Cheats.itemdropmultiplier_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.ItemDropMultiplier);
                                            break;
                                        }
                                    case "Slider_Character_Cheats_ItemDropChance":
                                        {
                                            Save_Manager.instance.data.Character.Cheats.ItemDropChance = __0;
                                            //Content.Character.Cheats.itemdropchance_text.text = "+ " + (int)((Save_Manager.instance.data.Character.Cheats.ItemDropChance / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Cheats_GoldDropMultiplier":
                                        {
                                            Save_Manager.instance.data.Character.Cheats.GoldDropMultiplier = __0;
                                            //Content.Character.Cheats.golddropmultiplier_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.GoldDropMultiplier);
                                            break;
                                        }
                                    case "Slider_Character_Cheats_GoldDropChance":
                                        {
                                            Save_Manager.instance.data.Character.Cheats.GoldDropChance = __0;
                                            //Content.Character.Cheats.golddropchance_text.text = "+ " + (int)((Save_Manager.instance.data.Character.Cheats.GoldDropChance / 255) * 100) + " %";
                                            break;
                                        }
                                    //Data
                                    case "Slider_Character_Data_Deaths":
                                        {
                                            if (!Refs_Manager.player_data.IsNullOrDestroyed())
                                            {
                                                Refs_Manager.player_data.Deaths = (int)__0;
                                            }

                                            //Content.Character.Data.deaths_text.text = ((int)__0).ToString();
                                            break;
                                        }
                                    case "Slider_Character_Data_LanternLuminance":
                                        {
                                            if (!Refs_Manager.player_data.IsNullOrDestroyed())
                                            {
                                                Refs_Manager.player_data.LanternLuminance = (int)__0;
                                            }
                                            //Content.Character.Data.lantern_text.text = ((int)__0).ToString();
                                            break;
                                        }
                                    case "Slider_Character_Data_SoulEmbers":
                                        {
                                            if (!Refs_Manager.player_data.IsNullOrDestroyed())
                                            {
                                                Refs_Manager.player_data.SoulEmbers = (int)__0;
                                            }
                                            //Content.Character.Data.soul_text.text = ((int)__0).ToString();
                                            break;
                                        }
                                    //Buffs
                                    case "Slider_Character_Buffs_MoveSpeed":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.MoveSpeed_Buff_Value = __0;
                                            //Content.Character.Buffs.movespeed_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.MoveSpeed_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_Damage":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.Damage_Buff_Value = __0;
                                            //Content.Character.Buffs.damage_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.Damage_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_AttackSpeed":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.AttackSpeed_Buff_Value = __0;
                                            //Content.Character.Buffs.attackspeed_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.AttackSpeed_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_CastingSpeed":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.CastSpeed_Buff_Value = __0;
                                            //Content.Character.Buffs.castingspeed_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.CastSpeed_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_CriticalChance":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.CriticalChance_Buff_Value = __0;
                                            //Content.Character.Buffs.criticalchance_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.CriticalChance_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_CriticalMultiplier":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.CriticalMultiplier_Buff_Value = __0;
                                            //Content.Character.Buffs.criticalmultiplier_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.CriticalMultiplier_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_HealthRegen":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.HealthRegen_Buff_Value = __0;
                                            //Content.Character.Buffs.healthregen_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.HealthRegen_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_ManaRegen":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.ManaRegen_Buff_Value = __0;
                                            //Content.Character.Buffs.manaregen_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.ManaRegen_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_Strenght":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.Str_Buff_Value = __0;
                                            //Content.Character.Buffs.str_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.Str_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_Intelligence":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.Int_Buff_Value = __0;
                                            //Content.Character.Buffs.int_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.Int_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_Dexterity":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.Dex_Buff_Value = __0;
                                            //Content.Character.Buffs.dex_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.Dex_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_Vitality":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.Vit_Buff_Value = __0;
                                            //Content.Character.Buffs.vit_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.Vit_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                    case "Slider_Character_Buffs_Attunement":
                                        {
                                            Save_Manager.instance.data.Character.PermanentBuffs.Att_Buff_Value = __0;
                                            //Content.Character.Buffs.att_text.text = "+ " + (int)((Save_Manager.instance.data.Character.PermanentBuffs.Att_Buff_Value / 255) * 100) + " %";
                                            break;
                                        }
                                }
                            }
                            else if (__instance.name.Contains("Slider_Items_"))
                            {
                                switch (__instance.name)
                                {
                                    case "Slider_Items_Drop_Implicits_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.Implicits_Min = __0;
                                            if (__0 > Save_Manager.instance.data.Items.Drop.Implicits_Max) { Content.Items.Drop.implicits_slider_max.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_Implicits_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.Implicits_Max = __0;
                                            if (__0 < Save_Manager.instance.data.Items.Drop.Implicits_Min) { Content.Items.Drop.implicits_slider_min.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_ForginPotencial_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.ForginPotencial_Min = __0;
                                            if (__0 > Save_Manager.instance.data.Items.Drop.ForginPotencial_Max) { Content.Items.Drop.forgin_potencial_slider_max.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_ForginPotencial_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.ForginPotencial_Max = __0;
                                            if (__0 < Save_Manager.instance.data.Items.Drop.ForginPotencial_Min) { Content.Items.Drop.forgin_potencial_slider_min.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_SealTier_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.SealTier_Min = __0;
                                            if (__0 > Save_Manager.instance.data.Items.Drop.SealTier_Max) { Content.Items.Drop.seal_tier_slider_max.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_SealTier_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.SealTier_Max = __0;
                                            if (__0 < Save_Manager.instance.data.Items.Drop.SealTier_Min) { Content.Items.Drop.seal_tier_slider_min.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_SealValue_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.SealValue_Min = __0;
                                            if (__0 > Save_Manager.instance.data.Items.Drop.SealValue_Max) { Content.Items.Drop.seal_value_slider_max.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_SealValue_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.SealValue_Max = __0;
                                            if (__0 < Save_Manager.instance.data.Items.Drop.SealValue_Min) { Content.Items.Drop.seal_value_slider_min.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_NbAffixes_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixCount_Min = __0;
                                            if (__0 > Save_Manager.instance.data.Items.Drop.AffixCount_Max) { Content.Items.Drop.affix_count_slider_max.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_NbAffixes_Max":
                                        {
                                            if (Save_Manager.instance.data.Items.Drop.AffixCount_Max != __0) { Save_Manager.instance.data.Items.Drop.AffixCount_Max = __0; }
                                            if (__0 < Save_Manager.instance.data.Items.Drop.AffixCount_Min) { Content.Items.Drop.affix_count_slider_min.value = __0; }

                                            break;
                                        }
                                    case "Slider_Items_Drop_AffixesTiers_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixTiers_Min = __0;
                                            if (__0 > Save_Manager.instance.data.Items.Drop.AffixTiers_Max)
                                            { Content.Items.Drop.affix_tiers_slider_max.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_AffixesTiers_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixTiers_Max = __0;
                                            if (__0 < Save_Manager.instance.data.Items.Drop.AffixTiers_Min)
                                            { Content.Items.Drop.affix_tiers_slider_min.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_AffixesValues_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixValues_Min = __0;
                                            if (__0 > Save_Manager.instance.data.Items.Drop.AffixValues_Max)
                                            { Content.Items.Drop.affix_values_slider_max.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_AffixesValues_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixValues_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.AffixValues_Max < Save_Manager.instance.data.Items.Drop.AffixValues_Min)
                                            { Content.Items.Drop.affix_values_slider_min.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_UniqueMods_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.UniqueMods_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.UniqueMods_Min > Save_Manager.instance.data.Items.Drop.UniqueMods_Max)
                                            { Content.Items.Drop.unique_mods_slider_max.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_UniqueMods_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.UniqueMods_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.UniqueMods_Max < Save_Manager.instance.data.Items.Drop.UniqueMods_Min)
                                            { Content.Items.Drop.unique_mods_slider_min.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_LegendaryPotencial_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min > Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max)
                                            { Content.Items.Drop.legendary_potencial_slider_max.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_LegendaryPotencial_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max = __0;
                                            if (__0 < Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min) { Content.Items.Drop.legendary_potencial_slider_min.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_WeaverWill_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.WeaverWill_Min = __0;
                                            if (__0 > Save_Manager.instance.data.Items.Drop.WeaverWill_Max) { Content.Items.Drop.weaver_will_slider_max.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_WeaverWill_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.WeaverWill_Max = __0;
                                            if (__0 < Save_Manager.instance.data.Items.Drop.WeaverWill_Min) { Content.Items.Drop.weaver_will_slider_min.value = __0; }
                                            break;
                                        }
                                    case "Slider_Items_Pickup_AutoShatter_Chance": { Save_Manager.instance.data.Items.Pickup.AutoShatter_Chance = (int)__0; break; }
                                    case "Slider_Items_Pickup_AutoShatter_AffixChance": { Save_Manager.instance.data.Items.Pickup.AutoShatter_Affix_Chance = (int)__0; break; }
                                    case "Slider_Items_Pickup_AutoShatter_QuantityChance": { Save_Manager.instance.data.Items.Pickup.AutoShatter_Quantity_Chance = (int)__0; break; }
                                    //Craft
                                    case "Slider_Items_Craft_ForginPotencial": { Save_Manager.instance.data.Items.CraftingSlot.ForginPotencial = __0; break; }
                                    case "Slider_Items_Craft_Implicit0": { Save_Manager.instance.data.Items.CraftingSlot.Implicit_0 = __0; break; }
                                    case "Slider_Items_Craft_Implicit1": { Save_Manager.instance.data.Items.CraftingSlot.Implicit_1 = __0; break; }
                                    case "Slider_Items_Craft_Implicit2": { Save_Manager.instance.data.Items.CraftingSlot.Implicit_2 = __0; break; }

                                    case "Slider_Items_Craft_SealTier": { Save_Manager.instance.data.Items.CraftingSlot.Seal_Tier = (int)__0; break; }
                                    case "Slider_Items_Craft_SealValue": { Save_Manager.instance.data.Items.CraftingSlot.Seal_Value = __0; break; }

                                    case "Slider_Items_Craft_AffixTier0": { Save_Manager.instance.data.Items.CraftingSlot.Affix_0_Tier = (int)__0; break; }
                                    case "Slider_Items_Craft_AffixTier1": { Save_Manager.instance.data.Items.CraftingSlot.Affix_1_Tier = (int)__0; break; }
                                    case "Slider_Items_Craft_AffixTier2": { Save_Manager.instance.data.Items.CraftingSlot.Affix_2_Tier = (int)__0; break; }
                                    case "Slider_Items_Craft_AffixTier3": { Save_Manager.instance.data.Items.CraftingSlot.Affix_3_Tier = (int)__0; break; }
                                    case "Slider_Items_Craft_AffixValue0": { Save_Manager.instance.data.Items.CraftingSlot.Affix_0_Value = __0; break; }
                                    case "Slider_Items_Craft_AffixValue1": { Save_Manager.instance.data.Items.CraftingSlot.Affix_1_Value = __0; break; }
                                    case "Slider_Items_Craft_AffixValue2": { Save_Manager.instance.data.Items.CraftingSlot.Affix_2_Value = __0; break; }
                                    case "Slider_Items_Craft_AffixValue3": { Save_Manager.instance.data.Items.CraftingSlot.Affix_3_Value = __0; break; }

                                    case "Slider_Items_Craft_UniqueMod0": { Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_0 = __0; break; }
                                    case "Slider_Items_Craft_UniqueMod1": { Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_1 = __0; break; }
                                    case "Slider_Items_Craft_UniqueMod2": { Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_2 = __0; break; }
                                    case "Slider_Items_Craft_UniqueMod3": { Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_3 = __0; break; }
                                    case "Slider_Items_Craft_UniqueMod4": { Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_4 = __0; break; }
                                    case "Slider_Items_Craft_UniqueMod5": { Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_5 = __0; break; }
                                    case "Slider_Items_Craft_UniqueMod6": { Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_6 = __0; break; }
                                    case "Slider_Items_Craft_UniqueMod7": { Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_7 = __0; break; }

                                    case "Slider_Items_Craft_LegendaryPotencial": { Save_Manager.instance.data.Items.CraftingSlot.LegendaryPotencial = (int)__0; break; }
                                    case "Slider_Items_Craft_WeaverWill": { Save_Manager.instance.data.Items.CraftingSlot.WeaverWill = (int)__0; break; }
                                    case "Slider_Items_Pickup_AutoStore_Timer": { Save_Manager.instance.data.Items.Pickup.AutoStore_Timer = (int)__0; break; }
                                }
                            }
                            else if (__instance.name.Contains("Slider_Scenes_"))
                            {
                                switch (__instance.name)
                                {
                                    case "Slider_Scenes_Camera_ZoomMinimum": { Save_Manager.instance.data.Scenes.Camera.ZoomMinimum = __0; break; }
                                    case "Slider_Scenes_Camera_ZoomPerScroll": { Save_Manager.instance.data.Scenes.Camera.ZoomPerScroll = __0; break; }
                                    case "Slider_Scenes_Camera_ZoomSpeed": { Save_Manager.instance.data.Scenes.Camera.ZoomSpeed = __0; break; }
                                    case "Slider_Scenes_Camera_DefaultRotation": { Save_Manager.instance.data.Scenes.Camera.DefaultRotation = __0; break; }
                                    case "Slider_Scenes_Camera_OffsetMinimum": { Save_Manager.instance.data.Scenes.Camera.OffsetMinimum = __0; break; }
                                    case "Slider_Scenes_Camera_OffsetMaximum": { Save_Manager.instance.data.Scenes.Camera.OffsetMaximum = __0; break; }
                                    case "Slider_Scenes_Camera_AngleMinimum": { Save_Manager.instance.data.Scenes.Camera.AngleMinimum = __0; break; }
                                    case "Slider_Scenes_Camera_AngleMaximum": { Save_Manager.instance.data.Scenes.Camera.AngleMaximum = __0; break; }

                                    case "Slider_Scenes_Monoliths_MaxStability": { Save_Manager.instance.data.Scenes.Monoliths.MaxStability = __0; break; }
                                    case "Slider_Scenes_Monoliths_MobsDensity": { Save_Manager.instance.data.Scenes.Monoliths.MobsDensity = __0; break; }
                                    case "Slider_Scenes_Monoliths_MobsDefeatOnStart": { Save_Manager.instance.data.Scenes.Monoliths.MobsDefeatOnStart = __0; break; }
                                    case "Slider_Scenes_Monoliths_BlessingSlots": { Save_Manager.instance.data.Scenes.Monoliths.BlessingSlots = (int)__0; break; }
                                }
                            }
                            else
                            {
                                switch (__instance.name)
                                {
                                    case "Slider_SpecializationSlots": { Save_Manager.instance.data.Skills.SpecializationSlots = __0; break; }
                                    case "Slider_SkillLevel": { Save_Manager.instance.data.Skills.SkillLevel = __0; break; }
                                    case "Slider_PassivePoints": { Save_Manager.instance.data.Skills.PassivePoints = __0; break; }

                                    case "Slider_MaximumCompanions": { Save_Manager.instance.data.Skills.Companion.Limit = (int)__0; break; }
                                    case "Slider_Wolf_SummonLimit": { Save_Manager.instance.data.Skills.Companion.Wolf.SummonLimit = (int)__0; break; }
                                    case "Slider_Scorpions_SummonLimit": { Save_Manager.instance.data.Skills.Companion.Scorpion.BabyQuantity = (int)__0; break; }

                                    case "Slider_Skeleteon_SummonQuantityFromPassive": { Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsFromPassives = (int)__0; break; }
                                    case "Slider_Skeleteon_SummonQuantityFromSkillTree": { Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsFromSkillTree = (int)__0; break; }
                                    case "Slider_Skeleteon_SummonQuantityPerCast": { Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsPerCast = (int)__0; break; }
                                    case "Slider_Skeleteon_ChanceToResummonOnDeath": { Save_Manager.instance.data.Skills.Minions.Skeletons.chanceToResummonOnDeath = (int)__0; break; }

                                    case "Slider_Wraiths_SummonMax": { Save_Manager.instance.data.Skills.Minions.Wraiths.additionalMaxWraiths = (int)__0; break; }
                                    case "Slider_Wraiths_Delayed": { Save_Manager.instance.data.Skills.Minions.Wraiths.delayedWraiths = (int)__0; break; }
                                    case "Slider_Wraiths_CastSpeed": { Save_Manager.instance.data.Skills.Minions.Wraiths.increasedCastSpeed = (int)__0; break; }

                                    case "Slider_Mages_SummonQuantityFromPassive": { Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromPassives = (int)__0; break; }
                                    case "Slider_Mages_SummonQuantityFromSkillTree": { Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromSkillTree = (int)__0; break; }
                                    case "Slider_Mages_SummonQuantityFromItems": { Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromItems = (int)__0; break; }
                                    case "Slider_Mages_SummonPerCast": { Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsPerCast = (int)__0; break; }
                                    case "Slider_Mages_ChanceForExtraPorjectiles": { Save_Manager.instance.data.Skills.Minions.Mages.chanceForTwoExtraProjectiles = (int)__0; break; }

                                    case "Slider_BoneGolem_GolemPerSkeletons": { Save_Manager.instance.data.Skills.Minions.BoneGolems.addedGolemsPer4Skeletons = (int)__0; break; }
                                    case "Slider_BoneGolem_SelfResurectChance": { Save_Manager.instance.data.Skills.Minions.BoneGolems.selfResurrectChance = (int)__0; break; }
                                    case "Slider_BoneGolem_IncreaseFireAura": { Save_Manager.instance.data.Skills.Minions.BoneGolems.increasedFireAuraArea = (int)__0; break; }
                                    case "Slider_BoneGolem_IncreaseArmorAura": { Save_Manager.instance.data.Skills.Minions.BoneGolems.undeadArmorAura = (int)__0; break; }
                                    case "Slider_BoneGolem_IncreaseMoveSpeedAura": { Save_Manager.instance.data.Skills.Minions.BoneGolems.undeadMovespeedAura = (int)__0; break; }
                                    case "Slider_BoneGolem_IncreaseMoveSpeed": { Save_Manager.instance.data.Skills.Minions.BoneGolems.increasedMoveSpeed = (int)__0; break; }

                                    case "Slider_VolatileZombies_ChanceOnMinionDeath": { Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastFromMinionDeath = (int)__0; break; }
                                    case "Slider_VolatileZombies_InfernalShadeChance": { Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastInfernalShadeOnDeath = (int)__0; break; }
                                    case "Slider_VolatileZombies_MarrowShardsChance": { Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastMarrowShardsOnDeath = (int)__0; break; }

                                    case "Slider_DreadShades_Duration": { Save_Manager.instance.data.Skills.Minions.DreadShades.Duration = (int)__0; break; }
                                    case "Slider_DreadShades_Max": { Save_Manager.instance.data.Skills.Minions.DreadShades.max = (int)__0; break; }
                                    case "Slider_DreadShades_Decay": { Save_Manager.instance.data.Skills.Minions.DreadShades.decay = (int)__0; break; }
                                    case "Slider_DreadShades_Radius": { Save_Manager.instance.data.Skills.Minions.DreadShades.radius = (int)__0; break; }
                                }
                            }
                        }
                    }
                }
            }
        }
        public class Events
        {
            public static void Set_Base_Button_Event(GameObject base_obj, string child, string btn_name, UnityEngine.Events.UnityAction action)
            {
                if (!base_obj.IsNullOrDestroyed())
                {
                    GameObject go = Functions.GetChild(base_obj, child);
                    if (!go.IsNullOrDestroyed())
                    {
                        GameObject btn_obj = Functions.GetChild(go, btn_name);
                        if (!btn_obj.IsNullOrDestroyed())
                        {
                            Button btn = btn_obj.GetComponent<Button>();
                            if (!btn.IsNullOrDestroyed())
                            {
                                btn.onClick = new Button.ButtonClickedEvent();
                                btn.onClick.AddListener(action);
                            }
                            else { Main.logger_instance.Error("Set_Base_Button_Event Can't found button"); }
                        }
                        else { Main.logger_instance.Error("Set_Base_Button_Event Can't found GameObject button " + btn_name); }
                    }
                    else { Main.logger_instance.Error("Set_Base_Button_Event Can't found " + child + " in base_obj"); }
                }
                else { Main.logger_instance.Error("Set_Base_Button_Event base_obj is null"); }
            }
            public static void Set_Button_Event(Button btn, UnityEngine.Events.UnityAction action)
            {
                btn.onClick = new Button.ButtonClickedEvent();
                btn.onClick.AddListener(action);
            }
            public static void Set_Slider_Event(Slider slider, UnityEngine.Events.UnityAction<float> action)
            {
                slider.onValueChanged = new Slider.SliderEvent();
                slider.onValueChanged.AddListener(action);
            }
            public static void Set_Toggle_Event(Toggle toggle, UnityEngine.Events.UnityAction<bool> action)
            {
                toggle.onValueChanged = new Toggle.ToggleEvent();
                toggle.onValueChanged.AddListener(action);
            }
            public static void Set_DropDown_Event(Dropdown dropdown, UnityEngine.Events.UnityAction<int> action)
            {
                dropdown.onValueChanged = new Dropdown.DropdownEvent();
                dropdown.onValueChanged.AddListener(action);
            }
        }
        public class Hud_Base
        {
            public static bool Initialized = false;
            public static bool Initializing = false;
            public static bool initiliazed_events = false;
            public static GameObject Default_PauseMenu_Btns = null;
            public static Button Btn_Resume;
            public static Button Btn_Settings;
            public static Button Btn_GameGuide;
            public static Button Btn_LeaveGame;
            public static Button Btn_ExitDesktop;
            public static GameObject ChapterInfo = null;
            public static GameObject Menu_Fade_Background = null;
            public static GameObject Chapter_Fade_Background = null;

            public static bool Get_DefaultPauseMenu()
            {
                bool result = false;

                foreach (Il2CppLE.UI.PanelSystem.MainMenuPanel obj in Resources.FindObjectsOfTypeAll<Il2CppLE.UI.PanelSystem.MainMenuPanel>())
                {
                    if (obj.name.Contains("Clone"))
                    {
                        game_pause_menu = obj.gameObject;
                        break;
                    }
                }
                if (!game_pause_menu.IsNullOrDestroyed())
                {
                    Default_PauseMenu_Btns = Functions.GetChild(game_pause_menu, "Menu");
                    Get_Refs();
                    result = true;
                }

                return result;
            }
            public static void Set_ChapterInfo(bool show)
            {
                if ((!Refs_Manager.game_uibase.IsNullOrDestroyed()) && (!game_pause_menu.IsNullOrDestroyed()))
                {
                    if (ChapterInfo.IsNullOrDestroyed()) { ChapterInfo = Functions.GetChild(game_pause_menu, "ChapterInfo"); }
                    if (!ChapterInfo.IsNullOrDestroyed()) { ChapterInfo.active = show; }

                    if (Menu_Fade_Background.IsNullOrDestroyed()) { Menu_Fade_Background = Functions.GetChild(game_pause_menu, "Menu_Fade_Background"); }
                    if (!Menu_Fade_Background.IsNullOrDestroyed()) { Menu_Fade_Background.active = show; }

                    if (Chapter_Fade_Background.IsNullOrDestroyed()) { Chapter_Fade_Background = Functions.GetChild(game_pause_menu, "Chapter_Fade_Background"); }
                    if (!Chapter_Fade_Background.IsNullOrDestroyed()) { Chapter_Fade_Background.active = show; }
                }
            }                        
            public static bool Get_DefaultPauseMenu_Open()
            {
                if (!Default_PauseMenu_Btns.IsNullOrDestroyed())
                {
                     return Default_PauseMenu_Btns.active;
                }
                else { return false; }
            }
            public static void Toogle_DefaultPauseMenu(bool show)
            {
                if (!Default_PauseMenu_Btns.IsNullOrDestroyed())
                {
                    Default_PauseMenu_Btns.active = show;
                }
            }
            
            public static void Get_Refs()
            {
                if (!Default_PauseMenu_Btns.IsNullOrDestroyed())
                {
                    GameObject Btns = Functions.GetChild(Default_PauseMenu_Btns, "Buttons");
                    if (!Btns.IsNullOrDestroyed())
                    {
                        Hud_Base.Btn_Resume = Functions.GetChild(Btns, "Resume Button").GetComponent<Button>();
                        Hud_Base.Btn_Settings = Functions.GetChild(Btns, "Settings Button").GetComponent<Button>();
                        Hud_Base.Btn_GameGuide = Functions.GetChild(Btns, "Game Guide Button").GetComponent<Button>();
                        Hud_Base.Btn_LeaveGame = Functions.GetChild(Btns, "Character Select Button").GetComponent<Button>();
                        Hud_Base.Btn_ExitDesktop = Functions.GetChild(Btns, "Exit Button").GetComponent<Button>();
                    }
                }
            }            
            public static void Set_Events()
            {
                if ((!Default_PauseMenu_Btns.IsNullOrDestroyed()) && (!hud_object.IsNullOrDestroyed()))
                {
                    GameObject base_obj = Functions.GetChild(hud_object, "Base");
                    if (!base_obj.IsNullOrDestroyed())
                    {
                        Events.Set_Base_Button_Event(base_obj, "Content", "Btn_Base_Resume", Resume_OnClick_Action);
                        Events.Set_Base_Button_Event(base_obj, "Content", "Btn_Base_Settings", Settings_OnClick_Action);
                        Events.Set_Base_Button_Event(base_obj, "Content", "Btn_Base_GameGuide", GameGuide_OnClick_Action);
                        Events.Set_Base_Button_Event(base_obj, "Content", "Btn_Base_LeaveGame", LeaveGame_OnClick_Action);
                        Events.Set_Base_Button_Event(base_obj, "Content", "Btn_Base_ExitDesktop", ExitDesktop_OnClick_Action);
                        initiliazed_events = true;
                    }
                }
            }

            private static readonly System.Action Resume_OnClick_Action = new System.Action(Resume_Click);
            public static void Resume_Click()
            {
                if (!Btn_Resume.IsNullOrDestroyed()) { Btn_Resume.onClick.Invoke(); }
            }

            private static readonly System.Action Settings_OnClick_Action = new System.Action(Settings_Click);
            public static void Settings_Click()
            {
                if (!Btn_Settings.IsNullOrDestroyed()) { Btn_Settings.onClick.Invoke(); }
            }

            private static readonly System.Action GameGuide_OnClick_Action = new System.Action(GameGuide_Click);
            public static void GameGuide_Click()
            {
                if (!Btn_GameGuide.IsNullOrDestroyed()) { Btn_GameGuide.onClick.Invoke(); }
            }

            private static readonly System.Action LeaveGame_OnClick_Action = new System.Action(LeaveGame_Click);
            public static void LeaveGame_Click()
            {
                if (Btn_LeaveGame is  not null)
                {
                    Content.Close_AllContent();
                    Btn_LeaveGame.onClick.Invoke();
                }
            }

            private static readonly System.Action ExitDesktop_OnClick_Action = new System.Action(ExitDesktop_Click);
            public static void ExitDesktop_Click()
            {
                if (!Btn_ExitDesktop.IsNullOrDestroyed())
                {
                    Content.Close_AllContent();
                    Btn_ExitDesktop.onClick.Invoke();
                }
            }
        }
        public class Hud_Menu
        {
            public static void Set_Events()
            {
                if (!hud_object.IsNullOrDestroyed())
                {
                    GameObject menu = Functions.GetChild(hud_object, "Menu");
                    if (!menu.IsNullOrDestroyed())
                    {
                        Events.Set_Base_Button_Event(menu, "Content", "Btn_Menu_Character", Character_OnClick_Action);
                        Events.Set_Base_Button_Event(menu, "Content", "Btn_Menu_Items", Items_OnClick_Action);
                        Events.Set_Base_Button_Event(menu, "Content", "Btn_Menu_Scenes", Scenes_OnClick_Action);
                        Events.Set_Base_Button_Event(menu, "Content", "Btn_Menu_TreeSkills", Skills_OnClick_Action);
                        Events.Set_Base_Button_Event(menu, "Content", "Btn_Menu_ForceDrop", OldForceDrop_OnClick_Action);
                        Events.Set_Base_Button_Event(menu, "Content", "Btn_Menu_NewItems", NewItems_OnClick_Action);
                        Events.Set_Base_Button_Event(menu, "Content", "Btn_Menu_Maxroll", Maxroll_OnClick_Action);
                    }
                }
            }
            
            private static readonly System.Action Character_OnClick_Action = new System.Action(Character_Click);
            public static void Character_Click()
            {
                Content.Items.Set_Active(false);
                Content.Scenes.Set_Active(false);
                Content.Skills.Set_Active(false);
                Content.OdlForceDrop.Set_Active(false);
                Content.NewItems.Set_Active(false);
                Content.Maxroll.Set_Active(false);
                Content.Character.Toggle_Active();          
            }

            private static readonly System.Action Items_OnClick_Action = new System.Action(Items_Click);
            public static void Items_Click()
            {
                Content.Character.Set_Active(false);
                Content.Scenes.Set_Active(false);
                Content.Skills.Set_Active(false);
                Content.OdlForceDrop.Set_Active(false);
                Content.NewItems.Set_Active(false);
                Content.Maxroll.Set_Active(false);
                Content.Items.Toggle_Active();
            }

            private static readonly System.Action Scenes_OnClick_Action = new System.Action(Scenes_Click);
            public static void Scenes_Click()
            {
                Content.Character.Set_Active(false);
                Content.Items.Set_Active(false);
                Content.Skills.Set_Active(false);
                Content.OdlForceDrop.Set_Active(false);
                Content.NewItems.Set_Active(false);
                Content.Maxroll.Set_Active(false);
                Content.Scenes.Toggle_Active();
            }

            private static readonly System.Action Skills_OnClick_Action = new System.Action(Skills_Click);
            public static void Skills_Click()
            {
                Content.Character.Set_Active(false);
                Content.Items.Set_Active(false);
                Content.Scenes.Set_Active(false);
                Content.OdlForceDrop.Set_Active(false);
                Content.NewItems.Set_Active(false);
                Content.Maxroll.Set_Active(false);
                Content.Skills.Toggle_Active();
            }

            private static readonly System.Action OldForceDrop_OnClick_Action = new System.Action(OldForceDrop_Click);
            public static void OldForceDrop_Click()
            {
                Content.Character.Set_Active(false);
                Content.Items.Set_Active(false);
                Content.Scenes.Set_Active(false);
                Content.Skills.Set_Active(false);
                Content.NewItems.Set_Active(false);
                Content.Maxroll.Set_Active(false);
                Content.OdlForceDrop.Toggle_Active();
            }

            private static readonly System.Action NewItems_OnClick_Action = new System.Action(NewItems_Click);
            public static void NewItems_Click()
            {
                Content.Character.Set_Active(false);
                Content.Items.Set_Active(false);
                Content.Scenes.Set_Active(false);
                Content.Skills.Set_Active(false);
                Content.OdlForceDrop.Set_Active(false);
                Content.Maxroll.Set_Active(false);
                Content.NewItems.Toggle_Active();
            }

            private static readonly System.Action Maxroll_OnClick_Action = new System.Action(Maxroll_Click);
            public static void Maxroll_Click()
            {
                Content.Character.Set_Active(false);
                Content.Items.Set_Active(false);
                Content.Scenes.Set_Active(false);
                Content.Skills.Set_Active(false);
                Content.OdlForceDrop.Set_Active(false);
                Content.NewItems.Set_Active(false);
                Content.Maxroll.Toggle_Active();
            }
        }
        public class Content
        {
            public static GameObject content_obj = null;
            public static void Set_Active()
            {
                if (!content_obj.IsNullOrDestroyed())
                {
                    bool show = false;
                    if ((Character.enable) || (Items.enable) || (Scenes.enable) || (Skills.enable) ||
                        (OdlForceDrop.enable) || (NewItems.enable) || (Maxroll.enable)) { show = true; }
                    if (content_obj.active != show) { content_obj.active = show; }
                }
            }
            public static void Close_AllContent()
            {
                Character.enable = false;
                Items.enable = false;
                Scenes.enable = false;
                Skills.enable = false;
                OdlForceDrop.enable = false;
                NewItems.enable = false;
                Maxroll.enable = false;
            }

            public class Character
            {
                public static GameObject content_obj = null;
                public static bool controls_initialized = false;
                public static bool enable = false;
                public static bool need_update = true;

                public static void Get_Refs()
                {
                    if (!Content.content_obj.IsNullOrDestroyed())
                    {
                        content_obj = Functions.GetChild(Content.content_obj, "Character_Content");
                        if (!content_obj.IsNullOrDestroyed())
                        {
                            GameObject character_cheats_content = Functions.GetViewportContent(content_obj, "Character_Cheats", "Character_Cheats_Content");
                            if (!character_cheats_content.IsNullOrDestroyed())
                            {
                                Cheats.godmode_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "GodMode", "Toggle_Character_Cheats_GodMode");
                                Cheats.lowlife_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "ForceLowLife", "Toggle_Character_Cheats_LowLife");
                                Cheats.allow_choosing_blessing = Functions.Get_ToggleInPanel(character_cheats_content, "AllowChoosingBlessings", "Toggle_Character_Cheats_AllowChooseBlessings");
                                Cheats.unlock_all_idols = Functions.Get_ToggleInPanel(character_cheats_content, "UnlockAllIdolsSlots", "Toggle_Character_Cheats_UnlockAllIdols");

                                Cheats.autoPot_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "AutoPotions", "Toggle_Character_Cheats_AutoPotions");
                                Cheats.autopot_text = Functions.Get_TextInToggle(character_cheats_content, "AutoPotions", "Toggle_Character_Cheats_AutoPotions", "Value");
                                Cheats.autopot_slider = Functions.Get_SliderInPanel(character_cheats_content, "AutoPotions", "Slider_Character_Cheats_AutoPotions");

                                Cheats.density_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "DensityMultiplier", "Toggle_Character_Cheats_DensityMultiplier");
                                Cheats.density_text = Functions.Get_TextInToggle(character_cheats_content, "DensityMultiplier", "Toggle_Character_Cheats_DensityMultiplier", "Value");
                                Cheats.density_slider = Functions.Get_SliderInPanel(character_cheats_content, "DensityMultiplier", "Slider_Character_Cheats_DensityMultiplier");

                                Cheats.experience_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "ExperienceMultiplier", "Toggle_Character_Cheats_ExperienceMultiplier");
                                Cheats.experience_text = Functions.Get_TextInToggle(character_cheats_content, "ExperienceMultiplier", "Toggle_Character_Cheats_ExperienceMultiplier", "Value");
                                Cheats.experience_slider = Functions.Get_SliderInPanel(character_cheats_content, "ExperienceMultiplier", "Slider_Character_Cheats_ExperienceMultiplier");

                                Cheats.ability_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "AbilityMultiplier", "Toggle_Character_Cheats_AbilityMultiplier");
                                Cheats.ability_text = Functions.Get_TextInToggle(character_cheats_content, "AbilityMultiplier", "Toggle_Character_Cheats_AbilityMultiplier", "Value");
                                Cheats.ability_slider = Functions.Get_SliderInPanel(character_cheats_content, "AbilityMultiplier", "Slider_Character_Cheats_AbilityMultiplier");

                                Cheats.favor_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "FavorMultiplier", "Toggle_Character_Cheats_FavorMultiplier");
                                Cheats.favor_text = Functions.Get_TextInToggle(character_cheats_content, "FavorMultiplier", "Toggle_Character_Cheats_FavorMultiplier", "Value");
                                Cheats.favor_slider = Functions.Get_SliderInPanel(character_cheats_content, "FavorMultiplier", "Slider_Character_Cheats_FavorMultiplier");

                                Cheats.memoryamber_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "MemoryAmberMultiplier", "Toggle_Character_Cheats_MemoryAmberMultiplier");
                                Cheats.memoryamber_text = Functions.Get_TextInToggle(character_cheats_content, "MemoryAmberMultiplier", "Toggle_Character_Cheats_MemoryAmberMultiplier", "Value");
                                Cheats.memoryamber_slider = Functions.Get_SliderInPanel(character_cheats_content, "MemoryAmberMultiplier", "Slider_Character_Cheats_MemoryAmberMultiplier");

                                Cheats.itemdropmultiplier_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "ItemDropMultiplier", "Toggle_Character_Cheats_ItemDropMultiplier");
                                Cheats.itemdropmultiplier_text = Functions.Get_TextInToggle(character_cheats_content, "ItemDropMultiplier", "Toggle_Character_Cheats_ItemDropMultiplier", "Value");
                                Cheats.itemdropmultiplier_slider = Functions.Get_SliderInPanel(character_cheats_content, "ItemDropMultiplier", "Slider_Character_Cheats_ItemDropMultiplier");

                                Cheats.itemdropchance_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "ItemDropChance", "Toggle_Character_Cheats_ItemDropChance");
                                Cheats.itemdropchance_text = Functions.Get_TextInToggle(character_cheats_content, "ItemDropChance", "Toggle_Character_Cheats_ItemDropChance", "Value");
                                Cheats.itemdropchance_slider = Functions.Get_SliderInPanel(character_cheats_content, "ItemDropChance", "Slider_Character_Cheats_ItemDropChance");

                                Cheats.golddropmultiplier_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "GoldDropMultiplier", "Toggle_Character_Cheats_GoldDropMultiplier");
                                Cheats.golddropmultiplier_text = Functions.Get_TextInToggle(character_cheats_content, "GoldDropMultiplier", "Toggle_Character_Cheats_GoldDropMultiplier", "Value");
                                Cheats.golddropmultiplier_slider = Functions.Get_SliderInPanel(character_cheats_content, "GoldDropMultiplier", "Slider_Character_Cheats_GoldDropMultiplier");

                                Cheats.golddropchance_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "GoldDropChance", "Toggle_Character_Cheats_GoldDropChance");
                                Cheats.golddropchance_text = Functions.Get_TextInToggle(character_cheats_content, "GoldDropChance", "Toggle_Character_Cheats_GoldDropChance", "Value");
                                Cheats.golddropchance_slider = Functions.Get_SliderInPanel(character_cheats_content, "GoldDropChance", "Slider_Character_Cheats_GoldDropChance");

                                Cheats.twohanded_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "TwoHandeWithShield", "Toggle_Character_Cheats_TwoHandeWithShield");

                                Cheats.waypoints_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "WaypointsUnlock", "Toggle_Character_Cheats_UnlockAllWaypoints");

                                Cheats.level_once_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_LevelOnce").GetComponent<Button>();
                                Cheats.level_max_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_LevelToMax").GetComponent<Button>();
                                Cheats.complete_quest_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_CompleteQuest").GetComponent<Button>();
                                Cheats.masterie_buttons = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_Masterie").GetComponent<Button>();
                                Cheats.masterie_text = Functions.Get_TextInButton(character_cheats_content, "Btn_Character_Cheats_Masterie", "Label");
                                Cheats.add_runes_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_AddRunes").GetComponent<Button>();
                                Cheats.add_glyphs_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_AddGlyphs").GetComponent<Button>();
                                Cheats.add_shards_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_AddAffixs").GetComponent<Button>();
                                Cheats.add_ancien_bone_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_AddAncienBone").GetComponent<Button>();
                                Cheats.discover_blessings_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_DicoverAllBlessings").GetComponent<Button>();
                            }
                            else { Main.logger_instance.Error("Hud Manager : character_cheats_content is null"); }

                            //Data
                            GameObject character_data_content = Functions.GetViewportContent(content_obj, "Character_Data", "Character_Data_Content");
                            if (!character_data_content.IsNullOrDestroyed())
                            {
                                Data.class_dropdown = Functions.Get_DopboxInPanel(character_data_content, "Classe", "Dropdown_Character_Data_Classes", new System.Action<int>((_) => { if ((!Refs_Manager.player_data.IsNullOrDestroyed()) && (!Data.class_dropdown.IsNullOrDestroyed())) { Refs_Manager.player_data.CharacterClass = Data.class_dropdown.value; } }));

                                Data.died_toggle = Functions.Get_ToggleInPanel(character_data_content, "Died", "Toggle_Character_Data_Died");
                                Data.hardcore_toggle = Functions.Get_ToggleInPanel(character_data_content, "Hardcore", "Toggle_Character_Data_Hardcore");
                                Data.masochist_toggle = Functions.Get_ToggleInPanel(character_data_content, "Masochist", "Toggle_Character_Data_Masochist");
                                Data.portal_toggle = Functions.Get_ToggleInPanel(character_data_content, "Portal Unlocked", "Toggle_Character_Data_Portal");
                                Data.solo_toggle = Functions.Get_ToggleInPanel(character_data_content, "SoloChallenge", "Toggle_Character_Data_SoloChallenge");

                                Data.deaths_text = Functions.Get_TextInButton(character_data_content, "Deaths", "Value");
                                Data.deaths_slider = Functions.Get_SliderInPanel(character_data_content, "Deaths", "Slider_Character_Data_Deaths");

                                Data.lantern_text = Functions.Get_TextInButton(character_data_content, "LanternLuminance", "Value");
                                Data.lantern_slider = Functions.Get_SliderInPanel(character_data_content, "LanternLuminance", "Slider_Character_Data_LanternLuminance");

                                Data.soul_text = Functions.Get_TextInButton(character_data_content, "Soul Embers", "Value");
                                Data.soul_slider = Functions.Get_SliderInPanel(character_data_content, "Soul Embers", "Slider_Character_Data_SoulEmbers");

                                Data.monolith_stability_basic_go = Functions.GetChild(character_data_content, "Monolith_Stability_Basic");
                                Data.monolith_stability_basic_go.active = false;
                                Data.monolith_stability_basic_text = Functions.Get_TextInButton(character_data_content, "Monolith_Stability_Basic", "Value");
                                Data.monolith_stability_basic_slider = Functions.Get_SliderInPanel(character_data_content, "Monolith_Stability_Basic", "Slider_Basic_Stability");

                                Data.monolith_stability_empower_go = Functions.GetChild(character_data_content, "Monolith_Stability_Empower");
                                Data.monolith_stability_empower_go.active = false;
                                Data.monolith_stability_empower_text = Functions.Get_TextInButton(character_data_content, "Monolith_Stability_Empower", "Value");
                                Data.monolith_stability_empower_slider = Functions.Get_SliderInPanel(character_data_content, "Monolith_Stability_Empower", "Slider_Empower_Stability");

                                Data.monolith_corruption_go = Functions.GetChild(character_data_content, "Monolith_Corruption");
                                Data.monolith_corruption_go.active = false;
                                Data.monolith_corruption_text = Functions.Get_TextInButton(character_data_content, "Monolith_Corruption", "Value");
                                Data.monolith_corruption_slider = Functions.Get_SliderInPanel(character_data_content, "Monolith_Corruption", "Slider_Empower_Corruption");

                                Data.monolith_gaze_go = Functions.GetChild(character_data_content, "Monolith_Gaze");
                                Data.monolith_gaze_go.active = false;
                                Data.monolith_gaze_text = Functions.Get_TextInButton(character_data_content, "Monolith_Gaze", "Value");
                                Data.monolith_gaze_slider = Functions.Get_SliderInPanel(character_data_content, "Monolith_Gaze", "Slider");

                                Data.monolith_dropdown = Functions.Get_DopboxInPanel(character_data_content, "Monoliths", "Dropdown", new System.Action<int>((_) => { Update_Monoliths_Data(); }));
                                Data.monolith_dropdown.options = new List<Dropdown.OptionData>();
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "Select" });
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "Fall_Of_The_Outcast" });
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "The_Stolen_Lance" });
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "The_Black_Sun" });
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "Blood_Frost_And_Death" });
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "Ending_The_Storm" });
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "Fall_Of_The_Empire" });
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "Reign_Of_Dragon" });
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "The_Last_Ruins" });
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "The_Age_Of_Winter" });
                                Data.monolith_dropdown.options.Add(new Dropdown.OptionData { text = "Spirits_Of_Fire" });
                                Data.monolith_dropdown.m_CurrentIndex = 0;
                            }
                            else { Main.logger_instance.Error("Hud Manager : character_data_content is null"); }
                            
                            GameObject char_data = Functions.GetChild(content_obj, "Character_Data");
                            if (!char_data.IsNullOrDestroyed())
                            {
                                GameObject panel_save = Functions.GetChild(char_data, "Panel");
                                if (!panel_save.IsNullOrDestroyed())
                                {
                                    Data.save_button = Functions.GetChild(panel_save, "Btn_Character_Data_Save").GetComponent<Button>();
                                }
                            }

                            //Faction Tracker
                            GameObject character_faction_content = Functions.GetViewportContent(content_obj, "Character_Factions", "Character_Factions_Content");
                            if (!character_faction_content.IsNullOrDestroyed())
                            {
                                Faction_Tracker.factions_dropdown = Functions.Get_DopboxInPanel(character_faction_content, "Factions_Dropdown", "Dropdown", new System.Action<int>((_) => { Update_Faction_Data(); }));
                                Faction_Tracker.factions_go = Functions.GetChild(character_faction_content, "Factions_Values");
                                if (!Faction_Tracker.factions_go.IsNullOrDestroyed()) { Faction_Tracker.factions_go.active = false; }

                                Faction_Tracker.factions_favor_text = Functions.Get_TextInButton(character_faction_content, "Factions_Values", "FavorValue");
                                Faction_Tracker.factions_favor_slider = Functions.Get_SliderInPanel(character_faction_content, "Factions_Values", "Slider_Character_Factions_Favor");
                                Faction_Tracker.factions_gain_favor_button = Functions.GetChild(Faction_Tracker.factions_go, "Btn_Character_Factions_Gain_Favor").GetComponent<Button>();
                                //Faction_Tracker.factions_set_favor_button = Functions.GetChild(Faction_Tracker.factions_go, "Btn_Character_Factions_Set_Favor").GetComponent<Button>();
                                
                                Faction_Tracker.factions_rank_text = Functions.Get_TextInButton(character_faction_content, "Factions_Values", "RankValue");
                                Faction_Tracker.factions_rank_slider = Functions.Get_SliderInPanel(character_faction_content, "Factions_Values", "Slider_Character_Factions_Rank");
                                Faction_Tracker.factions_set_rank_button = Functions.GetChild(Faction_Tracker.factions_go, "Btn_Character_Factions_Set_Rang").GetComponent<Button>();

                                Faction_Tracker.factions_reputation_text = Functions.Get_TextInButton(character_faction_content, "Factions_Values", "ReputationValue");
                                Faction_Tracker.factions_reputation_slider = Functions.Get_SliderInPanel(character_faction_content, "Factions_Values", "Slider_Character_Factions_Reputation");
                                Faction_Tracker.factions_set_reputation_button = Functions.GetChild(Faction_Tracker.factions_go, "Btn_Character_Factions_Set_Reputation").GetComponent<Button>();
                            }

                            //Buffs
                            Buffs.enable_mod = Functions.Get_ToggleInLabel(content_obj, "Character_Buffs", "Toggle_Character_Buffs_Enable");
                            GameObject character_buffs_content = Functions.GetViewportContent(content_obj, "Character_Buffs", "Character_Buffs_Content");
                            if (!character_buffs_content.IsNullOrDestroyed())
                            {
                                //Movespeed
                                Buffs.movespeed_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "MoveSpeed", "Toggle_Character_Buffs_MoveSpeed");
                                Buffs.movespeed_text = Functions.Get_TextInToggle(character_buffs_content, "MoveSpeed", "Toggle_Character_Buffs_MoveSpeed", "Value");
                                Buffs.movespeed_slider = Functions.Get_SliderInPanel(character_buffs_content, "MoveSpeed", "Slider_Character_Buffs_MoveSpeed");

                                //Damage
                                Buffs.damage_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "Damage", "Toggle_Character_Buffs_Damage");
                                Buffs.damage_text = Functions.Get_TextInToggle(character_buffs_content, "Damage", "Toggle_Character_Buffs_Damage", "Value");
                                Buffs.damage_slider = Functions.Get_SliderInPanel(character_buffs_content, "Damage", "Slider_Character_Buffs_Damage");

                                //AttackSpeed
                                Buffs.attackspeed_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "AttackSpeed", "Toggle_Character_Buffs_AttackSpeed");
                                Buffs.attackspeed_text = Functions.Get_TextInToggle(character_buffs_content, "AttackSpeed", "Toggle_Character_Buffs_AttackSpeed", "Value");
                                Buffs.attackspeed_slider = Functions.Get_SliderInPanel(character_buffs_content, "AttackSpeed", "Slider_Character_Buffs_AttackSpeed");

                                //CastingSpeed
                                Buffs.castingspeed_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "CastingSpeed", "Toggle_Character_Buffs_CastingSpeed");
                                Buffs.castingspeed_text = Functions.Get_TextInToggle(character_buffs_content, "CastingSpeed", "Toggle_Character_Buffs_CastingSpeed", "Value");
                                Buffs.castingspeed_slider = Functions.Get_SliderInPanel(character_buffs_content, "CastingSpeed", "Slider_Character_Buffs_CastingSpeed");

                                //CriticalChance
                                Buffs.criticalchance_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "CriticalChance", "Toggle_Character_Buffs_CriticalChance");
                                Buffs.criticalchance_text = Functions.Get_TextInToggle(character_buffs_content, "CriticalChance", "Toggle_Character_Buffs_CriticalChance", "Value");
                                Buffs.criticalchance_slider = Functions.Get_SliderInPanel(character_buffs_content, "CriticalChance", "Slider_Character_Buffs_CriticalChance");

                                //CriticalMultiplier
                                Buffs.criticalmultiplier_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "CriticalMultiplier", "Toggle_Character_Buffs_CriticalMultiplier");
                                Buffs.criticalmultiplier_text = Functions.Get_TextInToggle(character_buffs_content, "CriticalMultiplier", "Toggle_Character_Buffs_CriticalMultiplier", "Value");
                                Buffs.criticalmultiplier_slider = Functions.Get_SliderInPanel(character_buffs_content, "CriticalMultiplier", "Slider_Character_Buffs_CriticalMultiplier");

                                //HealthRegen
                                Buffs.healthregen_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "HealthRegen", "Toggle_Character_Buffs_HealthRegen");
                                Buffs.healthregen_text = Functions.Get_TextInToggle(character_buffs_content, "HealthRegen", "Toggle_Character_Buffs_HealthRegen", "Value");
                                Buffs.healthregen_slider = Functions.Get_SliderInPanel(character_buffs_content, "HealthRegen", "Slider_Character_Buffs_HealthRegen");

                                //ManaRegen
                                Buffs.manaregen_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "ManaRegen", "Toggle_Character_Buffs_ManaRegen");
                                Buffs.manaregen_text = Functions.Get_TextInToggle(character_buffs_content, "ManaRegen", "Toggle_Character_Buffs_ManaRegen", "Value");
                                Buffs.manaregen_slider = Functions.Get_SliderInPanel(character_buffs_content, "ManaRegen", "Slider_Character_Buffs_ManaRegen");

                                //Strenght
                                Buffs.str_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "Strenght", "Toggle_Character_Buffs_Strenght");
                                Buffs.str_text = Functions.Get_TextInToggle(character_buffs_content, "Strenght", "Toggle_Character_Buffs_Strenght", "Value");
                                Buffs.str_slider = Functions.Get_SliderInPanel(character_buffs_content, "Strenght", "Slider_Character_Buffs_Strenght");

                                //Intelligence
                                Buffs.int_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "Intelligence", "Toggle_Character_Buffs_Intelligence");
                                Buffs.int_text = Functions.Get_TextInToggle(character_buffs_content, "Intelligence", "Toggle_Character_Buffs_Intelligence", "Value");
                                Buffs.int_slider = Functions.Get_SliderInPanel(character_buffs_content, "Intelligence", "Slider_Character_Buffs_Intelligence");

                                //Dexterity
                                Buffs.dex_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "Dexterity", "Toggle_Character_Buffs_Dexterity");
                                Buffs.dex_text = Functions.Get_TextInToggle(character_buffs_content, "Dexterity", "Toggle_Character_Buffs_Dexterity", "Value");
                                Buffs.dex_slider = Functions.Get_SliderInPanel(character_buffs_content, "Dexterity", "Slider_Character_Buffs_Dexterity");

                                //Vitality
                                Buffs.vit_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "Vitality", "Toggle_Character_Buffs_Vitality");
                                Buffs.vit_text = Functions.Get_TextInToggle(character_buffs_content, "Vitality", "Toggle_Character_Buffs_Vitality", "Value");
                                Buffs.vit_slider = Functions.Get_SliderInPanel(character_buffs_content, "Vitality", "Slider_Character_Buffs_Vitality");

                                //Attunement
                                Buffs.att_toggle = Functions.Get_ToggleInPanel(character_buffs_content, "Attunement", "Toggle_Character_Buffs_Attunement");
                                Buffs.att_text = Functions.Get_TextInToggle(character_buffs_content, "Attunement", "Toggle_Character_Buffs_Attunement", "Value");
                                Buffs.att_slider = Functions.Get_SliderInPanel(character_buffs_content, "Attunement", "Slider_Character_Buffs_Attunement");
                            }
                            else { Main.logger_instance.Error("Hud Manager : character_buffs_content is null"); }
                        }
                        else { Main.logger_instance.Error("Hud Manager : Character_Content is null"); }
                    }
                }
                public static void Set_Events()
                {
                    if (!Cheats.godmode_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.godmode_toggle, Cheats.Godmode_Toggle_Action);
                    }
                    if (!Cheats.lowlife_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.lowlife_toggle, Cheats.Lowlife_Toggle_Action);
                    }
                    if (!Cheats.allow_choosing_blessing.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.allow_choosing_blessing, Cheats.AllowChooseBlessings_Toggle_Action);
                    }
                    if (!Cheats.unlock_all_idols.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.unlock_all_idols, Cheats.UnlockAllIdols_Toggle_Action);
                    }
                    if (!Cheats.autoPot_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.autoPot_toggle, Cheats.AutoPot_Toggle_Action);
                    }
                    if (!Cheats.density_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.density_toggle, Cheats.Density_Toggle_Action);
                    }
                    if (!Cheats.experience_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.experience_toggle, Cheats.Experience_Toggle_Action);
                    }
                    if (!Cheats.ability_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.ability_toggle, Cheats.Ability_Toggle_Action);
                    }
                    if (!Cheats.favor_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.favor_toggle, Cheats.Favor_Toggle_Action);
                    }
                    if (!Cheats.memoryamber_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.memoryamber_toggle, Cheats.memoryamber_Toggle_Action);
                    }
                    if (!Cheats.itemdropmultiplier_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.itemdropmultiplier_toggle, Cheats.ItemDropMulti_Toggle_Action);
                    }
                    if (!Cheats.itemdropchance_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.itemdropchance_toggle, Cheats.ItemDropChance_Toggle_Action);
                    }
                    if (!Cheats.golddropmultiplier_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.golddropmultiplier_toggle, Cheats.GoldMulti_Toggle_Action);
                    }
                    if (!Cheats.golddropchance_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.golddropchance_toggle, Cheats.GoldChance_Toggle_Action);
                    }
                    if (!Cheats.twohanded_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.twohanded_toggle, Cheats.TwoHanded_Toggle_Action);
                    }
                    if (!Cheats.waypoints_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Cheats.waypoints_toggle, Cheats.Waypoints_Toggle_Action);
                    }
                    if (!Cheats.level_once_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Cheats.level_once_button, Cheats.LevelUpOnce_OnClick_Action);
                    }
                    if (!Cheats.level_max_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Cheats.level_max_button, Cheats.LevelUpMax_OnClick_Action);
                    }
                    if (!Cheats.complete_quest_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Cheats.complete_quest_button, Cheats.CompleteQuest_OnClick_Action);
                    }
                    if (!Cheats.masterie_buttons.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Cheats.masterie_buttons, Cheats.Masteries_OnClick_Action);
                    }
                    if (!Cheats.add_runes_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Cheats.add_runes_button, Cheats.AddRunes_OnClick_Action);
                    }
                    if (!Cheats.add_glyphs_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Cheats.add_glyphs_button, Cheats.AddGlyphs_OnClick_Action);
                    }
                    if (!Cheats.add_shards_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Cheats.add_shards_button, Cheats.AddAffixs_OnClick_Action);
                    }
                    if (!Cheats.discover_blessings_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Cheats.discover_blessings_button, Cheats.DiscoverAllBlessings_OnClick_Action);
                    }
                    if (!Cheats.add_ancien_bone_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Cheats.add_ancien_bone_button, Cheats.AddAncienBone_OnClick_Action);
                    }
                    if (!Data.monolith_stability_basic_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Data.monolith_stability_basic_slider, Data.monolith_stability_basic_slider_Action);
                    }
                    if (!Data.monolith_stability_empower_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Data.monolith_stability_empower_slider, Data.monolith_stability_empower_slider_Action);
                    }
                    if (!Data.monolith_corruption_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Data.monolith_corruption_slider, Data.monolith_corruption_slider_Action);
                    }
                    if (!Data.monolith_gaze_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Data.monolith_gaze_slider, Data.monolith_gaze_slider_Action);
                    }
                    
                    if (!Data.save_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Data.save_button, Data.Save_OnClick_Action);
                    }

                    if (!Faction_Tracker.factions_favor_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Faction_Tracker.factions_favor_slider, Faction_Tracker.factions_favor_slider_Action);
                    }
                    if (!Faction_Tracker.factions_gain_favor_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Faction_Tracker.factions_gain_favor_button, Faction_Tracker.factions_gain_favor_OnClick_Action);
                    }
                    if (!Faction_Tracker.factions_set_favor_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Faction_Tracker.factions_set_favor_button, Faction_Tracker.factions_set_favor_OnClick_Action);
                    }
                    if (!Faction_Tracker.factions_rank_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Faction_Tracker.factions_rank_slider, Faction_Tracker.factions_rank_slider_Action);
                    }
                    if (!Faction_Tracker.factions_set_rank_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Faction_Tracker.factions_set_rank_button, Faction_Tracker.factions_set_rank_OnClick_Action);
                    }
                    if (!Faction_Tracker.factions_reputation_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Faction_Tracker.factions_reputation_slider, Faction_Tracker.factions_reputation_slider_Action);
                    }
                    if (!Faction_Tracker.factions_set_reputation_button.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(Faction_Tracker.factions_set_reputation_button, Faction_Tracker.factions_set_reputation_OnClick_Action);
                    }
                }
                public static void Set_Active(bool show)
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static void Toggle_Active()
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        bool show = !content_obj.active;
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static bool Init_UserData()
                {
                    bool result = false;
                    if (!Save_Manager.instance.IsNullOrDestroyed())
                    {
                        if (Save_Manager.instance.initialized)
                        {
                            //Content.Character.Cheats
                            if (!Cheats.godmode_toggle.IsNullOrDestroyed())
                            {
                                Cheats.godmode_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_GodMode;
                            }
                            if (!Cheats.lowlife_toggle.IsNullOrDestroyed())
                            {
                                Cheats.lowlife_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_LowLife;
                            }
                            if (!Cheats.allow_choosing_blessing.IsNullOrDestroyed())
                            {
                                Cheats.allow_choosing_blessing.isOn = Save_Manager.instance.data.Character.Cheats.Enable_CanChooseBlessing;
                            }
                            if (!Cheats.unlock_all_idols.IsNullOrDestroyed())
                            {
                                Cheats.unlock_all_idols.isOn = Save_Manager.instance.data.Character.Cheats.Enable_UnlockAllIdolsSlots;
                            }
                            if (!Cheats.autoPot_toggle.IsNullOrDestroyed())
                            {
                                Cheats.autoPot_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_AutoPot;
                            }
                            if (!Cheats.autopot_slider.IsNullOrDestroyed())
                            {
                                Cheats.autopot_slider.value = Save_Manager.instance.data.Character.Cheats.autoPot;
                            }
                            if (!Cheats.density_toggle.IsNullOrDestroyed())
                            {
                                Cheats.density_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_DensityMultiplier;
                            }
                            if (!Cheats.density_slider.IsNullOrDestroyed())
                            {
                                Cheats.density_slider.value = Save_Manager.instance.data.Character.Cheats.DensityMultiplier;
                            }
                            if (!Cheats.experience_toggle.IsNullOrDestroyed())
                            {
                                Cheats.experience_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_ExperienceMultiplier;
                            }
                            if (!Cheats.experience_slider.IsNullOrDestroyed())
                            {
                                Cheats.experience_slider.value = Save_Manager.instance.data.Character.Cheats.ExperienceMultiplier;
                            }
                            if (!Cheats.ability_toggle.IsNullOrDestroyed())
                            {
                                Cheats.ability_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_AbilityMultiplier;
                            }
                            if (!Cheats.ability_slider.IsNullOrDestroyed())
                            {
                                Cheats.ability_slider.value = Save_Manager.instance.data.Character.Cheats.AbilityMultiplier;
                            }
                            if (!Cheats.favor_toggle.IsNullOrDestroyed())
                            {
                                Cheats.favor_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_FavorMultiplier;
                            }
                            if (!Cheats.favor_slider.IsNullOrDestroyed())
                            {
                                Cheats.favor_slider.value = Save_Manager.instance.data.Character.Cheats.FavorMultiplier;
                            }
                            if (!Cheats.memoryamber_toggle.IsNullOrDestroyed())
                            {
                                Cheats.memoryamber_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_MemoryAmberMultiplier;
                            }
                            if (!Cheats.memoryamber_slider.IsNullOrDestroyed())
                            {
                                Cheats.memoryamber_slider.value = Save_Manager.instance.data.Character.Cheats.MemoryAmberMultiplier;
                            }
                            if (!Cheats.itemdropmultiplier_toggle.IsNullOrDestroyed())
                            {
                                Cheats.itemdropmultiplier_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_ItemDropMultiplier;
                            }
                            if (!Cheats.itemdropmultiplier_slider.IsNullOrDestroyed())
                            {
                                Cheats.itemdropmultiplier_slider.value = Save_Manager.instance.data.Character.Cheats.ItemDropMultiplier;
                            }
                            if (!Cheats.itemdropchance_toggle.IsNullOrDestroyed())
                            {
                                Cheats.itemdropchance_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_ItemDropChance;
                            }
                            if (!Cheats.itemdropchance_slider.IsNullOrDestroyed())
                            {
                                Cheats.itemdropchance_slider.value = Save_Manager.instance.data.Character.Cheats.ItemDropChance;
                            }
                            if (!Cheats.golddropmultiplier_toggle.IsNullOrDestroyed())
                            {
                                Cheats.golddropmultiplier_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_GoldDropMultiplier;
                            }
                            if (!Cheats.golddropmultiplier_slider.IsNullOrDestroyed())
                            {
                                Cheats.golddropmultiplier_slider.value = Save_Manager.instance.data.Character.Cheats.GoldDropMultiplier;
                            }
                            if (!Cheats.golddropchance_toggle.IsNullOrDestroyed())
                            {
                                Cheats.golddropchance_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_GoldDropChance;
                            }
                            if (!Cheats.golddropchance_slider.IsNullOrDestroyed())
                            {
                                Cheats.golddropchance_slider.value = Save_Manager.instance.data.Character.Cheats.GoldDropChance;
                            }
                            if (!Cheats.waypoints_toggle.IsNullOrDestroyed())
                            {
                                Cheats.waypoints_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_WaypointsUnlock;
                            }
                            //Content.Character.Buffs
                            if (!Buffs.enable_mod.IsNullOrDestroyed())
                            {
                                Buffs.enable_mod.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Mod;
                            }
                            if (!Buffs.movespeed_toggle.IsNullOrDestroyed())
                            {
                                Buffs.movespeed_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_MoveSpeed_Buff;
                            }
                            if (!Buffs.movespeed_slider.IsNullOrDestroyed())
                            {
                                Buffs.movespeed_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.MoveSpeed_Buff_Value;
                            }
                            if (!Buffs.damage_toggle.IsNullOrDestroyed())
                            {
                                Buffs.damage_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Damage_Buff;
                            }
                            if (!Buffs.damage_slider.IsNullOrDestroyed())
                            {
                                Buffs.damage_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Damage_Buff_Value;
                            }
                            if (!Buffs.attackspeed_toggle.IsNullOrDestroyed())
                            {
                                Buffs.attackspeed_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_AttackSpeed_Buff;
                            }
                            if (!Buffs.attackspeed_slider.IsNullOrDestroyed())
                            {
                                Buffs.attackspeed_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.AttackSpeed_Buff_Value;
                            }
                            if (!Buffs.castingspeed_toggle.IsNullOrDestroyed())
                            {
                                Buffs.castingspeed_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_CastSpeed_Buff;
                            }
                            if (!Buffs.castingspeed_slider.IsNullOrDestroyed())
                            {
                                Buffs.castingspeed_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.CastSpeed_Buff_Value;
                            }
                            if (!Buffs.criticalchance_toggle.IsNullOrDestroyed())
                            {
                                Buffs.criticalchance_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_CriticalChance_Buff;
                            }
                            if (!Buffs.criticalchance_slider.IsNullOrDestroyed())
                            {
                                Buffs.criticalchance_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.CriticalChance_Buff_Value;
                            }
                            if (!Buffs.criticalmultiplier_toggle.IsNullOrDestroyed())
                            {
                                Buffs.criticalmultiplier_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_CriticalMultiplier_Buff;
                            }
                            if (!Buffs.criticalmultiplier_slider.IsNullOrDestroyed())
                            {
                                Buffs.criticalmultiplier_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.CriticalMultiplier_Buff_Value;
                            }
                            if (!Buffs.healthregen_toggle.IsNullOrDestroyed())
                            {
                                Buffs.healthregen_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_HealthRegen_Buff;
                            }
                            if (!Buffs.healthregen_slider.IsNullOrDestroyed())
                            {
                                Buffs.healthregen_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.HealthRegen_Buff_Value;
                            }
                            if (!Buffs.manaregen_toggle.IsNullOrDestroyed())
                            {
                                Buffs.manaregen_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_ManaRegen_Buff;
                            }
                            if (!Buffs.manaregen_slider.IsNullOrDestroyed())
                            {
                                Buffs.manaregen_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.ManaRegen_Buff_Value;
                            }
                            if (!Buffs.str_toggle.IsNullOrDestroyed())
                            {
                                Buffs.str_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Str_Buff;
                            }
                            if (!Buffs.str_slider.IsNullOrDestroyed())
                            {
                                Buffs.str_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Str_Buff_Value;
                            }
                            if (!Buffs.int_toggle.IsNullOrDestroyed())
                            {
                                Buffs.int_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Int_Buff;
                            }
                            if (!Buffs.int_slider.IsNullOrDestroyed())
                            {
                                Buffs.int_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Int_Buff_Value;
                            }
                            if (!Buffs.dex_toggle.IsNullOrDestroyed())
                            {
                                Buffs.dex_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Dex_Buff;
                            }
                            if (!Buffs.dex_slider.IsNullOrDestroyed())
                            {
                                Buffs.dex_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Dex_Buff_Value;
                            }
                            if (!Buffs.vit_toggle.IsNullOrDestroyed())
                            {
                                Buffs.vit_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Vit_Buff;
                            }
                            if (!Buffs.vit_slider.IsNullOrDestroyed())
                            {
                                Buffs.vit_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Vit_Buff_Value;
                            }
                            if (!Buffs.att_toggle.IsNullOrDestroyed())
                            {
                                Buffs.att_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Att_Buff;
                            }
                            if (!Buffs.att_slider.IsNullOrDestroyed())
                            {
                                Buffs.att_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Att_Buff_Value;
                            }
                            controls_initialized = true;
                            result = true;
                        }
                    }

                    return result;
                }
                public static void Update_PlayerData()
                {
                    need_update = false;
                    if ((!Refs_Manager.player_treedata.IsNullOrDestroyed()) && (!Refs_Manager.character_class_list.IsNullOrDestroyed()) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
                    {
                        Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
                        foreach (CharacterClass char_class in Refs_Manager.character_class_list.classes)
                        {
                            options.Add(new Dropdown.OptionData { text = char_class.className });
                        }
                        if (!Data.class_dropdown.IsNullOrDestroyed())
                        {
                            Data.class_dropdown.options = options;
                            Data.class_dropdown.value = Refs_Manager.player_data.CharacterClass;
                        }
                        if (!Data.died_toggle.IsNullOrDestroyed())
                        {
                            Data.died_toggle.isOn = Refs_Manager.player_data.Died;
                        }
                        if (!Data.deaths_slider.IsNullOrDestroyed())
                        {
                            Data.deaths_slider.value = Refs_Manager.player_data.Deaths;
                        }
                        if (!Data.deaths_text.IsNullOrDestroyed())
                        {
                            Data.deaths_text.text = Refs_Manager.player_data.Deaths.ToString();
                        }
                        if (!Data.hardcore_toggle.IsNullOrDestroyed())
                        {
                            Data.hardcore_toggle.isOn = Refs_Manager.player_data.Hardcore;
                        }
                        if (!Data.masochist_toggle.IsNullOrDestroyed())
                        {
                            Data.masochist_toggle.isOn = Refs_Manager.player_data.Masochist;
                        }
                        if (!Data.portal_toggle.IsNullOrDestroyed())
                        {
                            Data.portal_toggle.isOn = Refs_Manager.player_data.PortalUnlocked;
                        }
                        if (!Data.solo_toggle.IsNullOrDestroyed())
                        {
                            Data.solo_toggle.isOn = Refs_Manager.player_data.SoloChallenge;
                        }
                        if (!Data.lantern_slider.IsNullOrDestroyed())
                        {
                            Data.lantern_slider.value = Refs_Manager.player_data.LanternLuminance;
                        }
                        if (!Data.lantern_text.IsNullOrDestroyed())
                        {
                            Data.lantern_text.text = Refs_Manager.player_data.LanternLuminance.ToString();
                        }
                        if (!Data.soul_slider.IsNullOrDestroyed())
                        {
                            Data.soul_slider.value = Refs_Manager.player_data.SoulEmbers;
                        }
                        if (!Data.soul_text.IsNullOrDestroyed())
                        {
                            Data.soul_text.text = Refs_Manager.player_data.SoulEmbers.ToString();
                        }
                    }
                }
                public static void Update_Monoliths_Data()
                {                    
                    if ((!Refs_Manager.player_data.IsNullOrDestroyed()) && (!Data.monolith_dropdown.IsNullOrDestroyed())
                        && (!Data.monolith_stability_basic_go.IsNullOrDestroyed())
                        && (!Data.monolith_stability_basic_slider.IsNullOrDestroyed())
                        && (!Data.monolith_stability_basic_text.IsNullOrDestroyed())
                        && (!Data.monolith_stability_empower_go.IsNullOrDestroyed())
                        && (!Data.monolith_stability_empower_slider.IsNullOrDestroyed())
                        && (!Data.monolith_stability_empower_text.IsNullOrDestroyed())
                        && (!Data.monolith_corruption_go.IsNullOrDestroyed())
                        && (!Data.monolith_corruption_slider.IsNullOrDestroyed())
                        && (!Data.monolith_corruption_text.IsNullOrDestroyed())
                        && (!Data.monolith_gaze_go.IsNullOrDestroyed())
                        && (!Data.monolith_gaze_slider.IsNullOrDestroyed())
                        && (!Data.monolith_gaze_text.IsNullOrDestroyed()))
                    {                        
                        int index = Data.monolith_dropdown.value;
                        if (index < 1)
                        {
                            int value = -1;
                            Data.monolith_stability_basic_go.active = false;
                            Data.monolith_stability_basic_slider.value = value;
                            Data.monolith_stability_basic_text.text = value.ToString();

                            Data.monolith_stability_empower_go.active = false;
                            Data.monolith_stability_empower_slider.value = value;
                            Data.monolith_stability_empower_text.text = value.ToString();

                            Data.monolith_corruption_go.active = false;
                            Data.monolith_corruption_slider.value = value;
                            Data.monolith_corruption_text.text = value.ToString();

                            Data.monolith_gaze_go.active = false;
                            Data.monolith_gaze_slider.value = value;
                            Data.monolith_gaze_text.text = value.ToString();
                        }
                        else
                        {
                            SavedMonolithRun basic = null;
                            SavedMonolithRun empower = null;
                            foreach (SavedMonolithRun run in Refs_Manager.player_data.MonolithRuns)
                            {
                                if (run.TimelineID == index)
                                {
                                    if (run.DifficultyIndex == 0) { basic = run; }
                                    else { empower = run; }
                                }
                            }

                            if (!basic.IsNullOrDestroyed())
                            {
                                Data.monolith_stability_basic_go.active = true;
                                int value = basic.Stability;
                                Data.monolith_stability_basic_slider.value = value;
                                Data.monolith_stability_basic_text.text = value.ToString();
                            }
                            else
                            {
                                Data.monolith_stability_basic_go.active = false;
                                int value = -1;
                                Data.monolith_stability_basic_slider.value = value;
                                Data.monolith_stability_basic_text.text = value.ToString();
                            }

                            if (!empower.IsNullOrDestroyed())
                            {
                                Data.monolith_stability_empower_go.active = true;
                                int value = empower.Stability;
                                Data.monolith_stability_empower_slider.value = value;
                                Data.monolith_stability_empower_text.text = value.ToString();
                                if (!empower.SavedEchoWeb.IsNullOrDestroyed())
                                {
                                    Data.monolith_corruption_go.active = true;
                                    int value2 = empower.SavedEchoWeb.Corruption;
                                    Data.monolith_corruption_slider.value = value2;
                                    Data.monolith_corruption_text.text = value2.ToString();

                                    Data.monolith_gaze_go.active = true;
                                    int value3 = empower.SavedEchoWeb.GazeOfOrobyss;
                                    Data.monolith_gaze_slider.value = value3;
                                    Data.monolith_gaze_text.text = value3.ToString();
                                }
                                else
                                {
                                    Data.monolith_corruption_go.active = false;
                                    int value2 = -1;
                                    Data.monolith_corruption_slider.value = value2;
                                    Data.monolith_corruption_text.text = value2.ToString();

                                    Data.monolith_gaze_go.active = false;
                                    int value3 = -1;
                                    Data.monolith_gaze_slider.value = value3;
                                    Data.monolith_gaze_text.text = value3.ToString();
                                }
                            }
                            else
                            {
                                Data.monolith_stability_empower_go.active = false;
                                int value = -1;
                                Data.monolith_stability_empower_slider.value = value;
                                Data.monolith_stability_empower_text.text = value.ToString();
                            }
                        }
                    }
                }
                public static void Update_Faction_Data()
                {
                    Faction_Tracker.factions_favor_text.text = Faction_Tracker.factions_favor_slider.value.ToString();
                    Faction_Tracker.factions_rank_text.text = Faction_Tracker.factions_rank_slider.value.ToString();
                    Faction_Tracker.factions_reputation_text.text = Faction_Tracker.factions_reputation_slider.value.ToString();
                    if (Faction_Tracker.factions_dropdown.value > 0) { Faction_Tracker.factions_go.active = true; }
                    else { Faction_Tracker.factions_go.active = false; }
                    Faction_Tracker.selected_faction = Faction_Tracker.GetFaction();
                }
                public static void UpdateVisuals()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (controls_initialized))
                    {
                        if (Save_Manager.instance.initialized)
                        {
                            if (!Cheats.autopot_text.IsNullOrDestroyed())
                            {
                                Cheats.autopot_text.text = (int)((Save_Manager.instance.data.Character.Cheats.autoPot / 255) * 100) + " %";
                            }
                            if (!Cheats.density_text.IsNullOrDestroyed())
                            {
                                Cheats.density_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.DensityMultiplier);
                            }
                            if (!Cheats.experience_text.IsNullOrDestroyed())
                            {
                                Cheats.experience_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.ExperienceMultiplier);
                            }
                            if (!Cheats.ability_text.IsNullOrDestroyed())
                            {
                                Cheats.ability_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.AbilityMultiplier);
                            }
                            if (!Cheats.favor_text.IsNullOrDestroyed())
                            {
                                Cheats.favor_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.FavorMultiplier);
                            }
                            if (!Cheats.memoryamber_text.IsNullOrDestroyed())
                            {
                                Cheats.memoryamber_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.MemoryAmberMultiplier);
                            }
                            if (!Cheats.itemdropmultiplier_text.IsNullOrDestroyed())
                            {
                                Cheats.itemdropmultiplier_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.ItemDropMultiplier);
                            }
                            if (!Cheats.itemdropchance_text.IsNullOrDestroyed())
                            {
                                Cheats.itemdropchance_text.text = "+ " + (int)((Save_Manager.instance.data.Character.Cheats.ItemDropChance / 255) * 100) + " %";
                            }
                            if (!Cheats.golddropmultiplier_text.IsNullOrDestroyed())
                            {
                                Cheats.golddropmultiplier_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.GoldDropMultiplier);
                            }
                            if (!Cheats.golddropchance_text.IsNullOrDestroyed())
                            {
                                Cheats.golddropchance_text.text = "+ " + (int)((Save_Manager.instance.data.Character.Cheats.GoldDropChance / 255) * 100) + " %";
                            }

                            if (!Buffs.movespeed_text.IsNullOrDestroyed())
                            {
                                Buffs.movespeed_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.MoveSpeed_Buff_Value * 100) + " %";
                            }
                            if (!Buffs.damage_text.IsNullOrDestroyed())
                            {
                                Buffs.damage_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Damage_Buff_Value * 100) + " %";
                            }
                            if (!Buffs.attackspeed_text.IsNullOrDestroyed())
                            {
                                Buffs.attackspeed_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.AttackSpeed_Buff_Value * 100) + " %";
                            }
                            if (!Buffs.castingspeed_text.IsNullOrDestroyed())
                            {
                                Buffs.castingspeed_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.CastSpeed_Buff_Value * 100) + " %";
                            }

                            int crit_chance = 0;
                            if (Save_Manager.instance.data.Character.PermanentBuffs.CriticalChance_Buff_Value > 0)
                            {
                                crit_chance = (int)(Save_Manager.instance.data.Character.PermanentBuffs.CriticalChance_Buff_Value * 100) + 1;
                            }
                            if (!Buffs.criticalchance_text.IsNullOrDestroyed())
                            {
                                Buffs.criticalchance_text.text = "+ " + crit_chance + " %";
                            }
                            if (!Buffs.criticalmultiplier_text.IsNullOrDestroyed())
                            {
                                Buffs.criticalmultiplier_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.CriticalMultiplier_Buff_Value * 100) + " %";
                            }
                            if (!Buffs.healthregen_text.IsNullOrDestroyed())
                            {
                                Buffs.healthregen_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.HealthRegen_Buff_Value * 100) + " %";
                            }
                            if (!Buffs.manaregen_text.IsNullOrDestroyed())
                            {
                                Buffs.manaregen_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.ManaRegen_Buff_Value * 100) + " %";
                            }
                            if (!Buffs.str_text.IsNullOrDestroyed())
                            {
                                Buffs.str_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Str_Buff_Value) + " %";
                            }
                            if (!Buffs.int_text.IsNullOrDestroyed())
                            {
                                Buffs.int_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Int_Buff_Value) + " %";
                            }
                            if (!Buffs.dex_text.IsNullOrDestroyed())
                            {
                                Buffs.dex_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Dex_Buff_Value) + " %";
                            }
                            if (!Buffs.vit_text.IsNullOrDestroyed())
                            {
                                Buffs.vit_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Vit_Buff_Value) + " %";
                            }
                            if (!Buffs.att_text.IsNullOrDestroyed())
                            {
                                Buffs.att_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Att_Buff_Value) + " %";
                            }
                        }
                    }
                }

                public class Cheats
                {
                    // BUG: For some reason the game always return true in action delegates
                    public static Toggle godmode_toggle = null;
                    public static readonly System.Action<bool> Godmode_Toggle_Action = new System.Action<bool>(Set_Godmode_Enable);
                    private static void Set_Godmode_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!godmode_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_GodMode = godmode_toggle.isOn;
                        }                        
                    }

                    public static Toggle lowlife_toggle = null;
                    public static readonly System.Action<bool> Lowlife_Toggle_Action = new System.Action<bool>(Set_Lowlife_Enable);
                    private static void Set_Lowlife_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!lowlife_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_LowLife = lowlife_toggle.isOn;
                        }
                    }

                    public static Toggle allow_choosing_blessing = null;
                    public static readonly System.Action<bool> AllowChooseBlessings_Toggle_Action = new System.Action<bool>(Set_AllowChooseBlessings_Enable);
                    private static void Set_AllowChooseBlessings_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!allow_choosing_blessing.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_CanChooseBlessing = allow_choosing_blessing.isOn;
                        }
                    }

                    public static Toggle unlock_all_idols = null;
                    public static readonly System.Action<bool> UnlockAllIdols_Toggle_Action = new System.Action<bool>(Set_UnlockAllIdols_Enable);
                    private static void Set_UnlockAllIdols_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!unlock_all_idols.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_UnlockAllIdolsSlots = unlock_all_idols.isOn;
                            Mods.Character.Character_UnlockAllIdols.Update();
                        }
                    }

                    public static Toggle autoPot_toggle = null;
                    public static readonly System.Action<bool> AutoPot_Toggle_Action = new System.Action<bool>(Set_AutoPot_Enable);
                    private static void Set_AutoPot_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!autoPot_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_AutoPot = autoPot_toggle.isOn;
                        }
                    }
                    public static Text autopot_text = null;
                    public static Slider autopot_slider = null;
                                        
                    public static Toggle density_toggle = null;
                    public static readonly System.Action<bool> Density_Toggle_Action = new System.Action<bool>(Set_Density_Enable);
                    private static void Set_Density_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!density_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_DensityMultiplier = density_toggle.isOn;
                        }
                    }
                    public static Text density_text = null;
                    public static Slider density_slider = null;
                    
                    public static Toggle experience_toggle = null;
                    public static readonly System.Action<bool> Experience_Toggle_Action = new System.Action<bool>(Set_Experience_Enable);
                    private static void Set_Experience_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!experience_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_ExperienceMultiplier = experience_toggle.isOn;
                        }
                    }
                    public static Text experience_text = null;
                    public static Slider experience_slider = null;
                    
                    public static Toggle ability_toggle = null;
                    public static readonly System.Action<bool> Ability_Toggle_Action = new System.Action<bool>(Set_Ability_Enable);
                    private static void Set_Ability_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!ability_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_AbilityMultiplier = ability_toggle.isOn;
                        }
                    }
                    public static Text ability_text = null;
                    public static Slider ability_slider = null;
                    
                    public static Toggle favor_toggle = null;
                    public static readonly System.Action<bool> Favor_Toggle_Action = new System.Action<bool>(Set_Favor_Enable);
                    private static void Set_Favor_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!favor_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_FavorMultiplier = favor_toggle.isOn;
                        }
                    }
                    public static Text favor_text = null;
                    public static Slider favor_slider = null;

                    public static Toggle memoryamber_toggle = null;
                    public static readonly System.Action<bool> memoryamber_Toggle_Action = new System.Action<bool>(Set_memoryamber_Enable);
                    private static void Set_memoryamber_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!favor_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_MemoryAmberMultiplier = memoryamber_toggle.isOn;
                        }
                    }
                    public static Text memoryamber_text = null;
                    public static Slider memoryamber_slider = null;

                    public static Toggle itemdropmultiplier_toggle = null;
                    public static readonly System.Action<bool> ItemDropMulti_Toggle_Action = new System.Action<bool>(Set_ItemDropMulti_Enable);
                    private static void Set_ItemDropMulti_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!itemdropmultiplier_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_ItemDropMultiplier = itemdropmultiplier_toggle.isOn;
                        }
                    }
                    public static Text itemdropmultiplier_text = null;
                    public static Slider itemdropmultiplier_slider = null;
                    
                    public static Toggle itemdropchance_toggle = null;
                    public static readonly System.Action<bool> ItemDropChance_Toggle_Action = new System.Action<bool>(Set_ItemDropChance_Enable);
                    private static void Set_ItemDropChance_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!itemdropchance_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_ItemDropChance = itemdropchance_toggle.isOn;
                        }
                    }
                    public static Text itemdropchance_text = null;
                    public static Slider itemdropchance_slider = null;
                    
                    public static Toggle golddropmultiplier_toggle = null;
                    public static readonly System.Action<bool> GoldMulti_Toggle_Action = new System.Action<bool>(Set_GoldMulti_Enable);
                    private static void Set_GoldMulti_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!golddropmultiplier_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_GoldDropMultiplier = golddropmultiplier_toggle.isOn;
                        }
                    }
                    public static Text golddropmultiplier_text = null;
                    public static Slider golddropmultiplier_slider = null;
                    
                    public static Toggle golddropchance_toggle = null;
                    public static readonly System.Action<bool> GoldChance_Toggle_Action = new System.Action<bool>(Set_GoldChance_Enable);
                    private static void Set_GoldChance_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!golddropchance_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_GoldDropChance = golddropchance_toggle.isOn;
                        }
                    }
                    public static Text golddropchance_text = null;
                    public static Slider golddropchance_slider = null;

                    public static Toggle twohanded_toggle = null;
                    public static readonly System.Action<bool> TwoHanded_Toggle_Action = new System.Action<bool>(Set_TwoHandedWithShield_Enable);
                    private static void Set_TwoHandedWithShield_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!twohanded_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_TwoHandedWithShield = twohanded_toggle.isOn;
                        }
                    }

                    public static Toggle waypoints_toggle = null;
                    public static readonly System.Action<bool> Waypoints_Toggle_Action = new System.Action<bool>(Set_Waypoints_Enable);
                    private static void Set_Waypoints_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!waypoints_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Character.Cheats.Enable_WaypointsUnlock = waypoints_toggle.isOn;
                        }
                    }

                    public static Button level_once_button = null;
                    public static readonly System.Action LevelUpOnce_OnClick_Action = new System.Action(LevelUpOnce_Click);
                    public static void LevelUpOnce_Click()
                    {
                        Mods.Character.Character_Level.LevelUpOnce();
                    }

                    public static Button level_max_button = null;
                    public static readonly System.Action LevelUpMax_OnClick_Action = new System.Action(LevelUpMax_Click);
                    public static void LevelUpMax_Click()
                    {
                        Mods.Character.Character_Level.LevelUptoMax();
                    }

                    public static Button complete_quest_button = null;
                    public static readonly System.Action CompleteQuest_OnClick_Action = new System.Action(CompleteQuest_Click);
                    public static void CompleteQuest_Click()
                    {
                        Mods.Character.Character_MainQuest.Complete();
                    }

                    public static Button masterie_buttons = null;
                    public static readonly System.Action Masteries_OnClick_Action = new System.Action(Masteries_Click);
                    public static void Masteries_Click()
                    {
                        Mods.Character.Character_Masteries.ResetChooseMasterie();
                    }
                    public static Text masterie_text = null;

                    public static Button add_runes_button = null;
                    public static readonly System.Action AddRunes_OnClick_Action = new System.Action(AddRunes_Click);
                    public static void AddRunes_Click()
                    {
                        Mods.Character.Character_Materials.GetAllRunesX99();
                    }

                    public static Button add_glyphs_button = null;
                    public static readonly System.Action AddGlyphs_OnClick_Action = new System.Action(AddGlyphs_Click);
                    public static void AddGlyphs_Click()
                    {
                        Mods.Character.Character_Materials.GetAllGlyphsX99();
                    }

                    public static Button add_shards_button = null;
                    public static readonly System.Action AddAffixs_OnClick_Action = new System.Action(AddAffixs_Click);
                    public static void AddAffixs_Click()
                    {
                        Mods.Character.Character_Materials.GetAllShardsX10();
                    }

                    public static Button discover_blessings_button = null;
                    public static readonly System.Action DiscoverAllBlessings_OnClick_Action = new System.Action(DiscoverAllBlessings_Click);
                    public static void DiscoverAllBlessings_Click()
                    {
                        Mods.Character.Character_Blessings.DiscoverAllBlessings();
                    }

                    public static Button add_ancien_bone_button = null;
                    public static readonly System.Action AddAncienBone_OnClick_Action = new System.Action(AddAncienBones_Click);
                    public static void AddAncienBones_Click()
                    {
                        Mods.Character.Character_Materials.GetAddAncienBonesX10000();
                    }
                }
                public class Data
                {
                    public static Dropdown class_dropdown = null;
                    public static Toggle died_toggle = null;
                    public static Toggle hardcore_toggle = null;
                    public static Toggle masochist_toggle = null;
                    public static Toggle portal_toggle = null;
                    public static Toggle solo_toggle = null;
                    public static Text deaths_text = null;
                    public static Slider deaths_slider = null;
                    public static Text lantern_text = null;
                    public static Slider lantern_slider = null;
                    public static Text soul_text = null;
                    public static Slider soul_slider = null;

                    public static Dropdown monolith_dropdown = null;
                    public static GameObject monolith_stability_basic_go = null;
                    public static Text monolith_stability_basic_text = null;
                    public static Slider monolith_stability_basic_slider = null;
                    public static readonly System.Action<float> monolith_stability_basic_slider_Action = new System.Action<float>(Set_monolith_stability_basic);
                    public static void Set_monolith_stability_basic(float f)
                    {
                        if ((!monolith_stability_basic_slider.IsNullOrDestroyed()) && (!monolith_stability_basic_text.IsNullOrDestroyed()) && (!monolith_dropdown.IsNullOrDestroyed()))
                        {
                            int result = System.Convert.ToInt32(monolith_stability_basic_slider.value);
                            int index = monolith_dropdown.value;
                            if (!Refs_Manager.player_data.IsNullOrDestroyed())
                            {
                                foreach (SavedMonolithRun run in Refs_Manager.player_data.MonolithRuns)
                                {
                                    if ((run.TimelineID == index) && (run.DifficultyIndex == 0))
                                    {
                                        if (run.Stability != result) { run.Stability = result; }
                                        break;
                                    }
                                }
                            }
                            monolith_stability_basic_text.text = result.ToString();
                        }
                    }

                    public static GameObject monolith_stability_empower_go = null;
                    public static Text monolith_stability_empower_text = null;
                    public static Slider monolith_stability_empower_slider = null;
                    public static readonly System.Action<float> monolith_stability_empower_slider_Action = new System.Action<float>(Set_monolith_stability_empower);
                    public static void Set_monolith_stability_empower(float f)
                    {
                        if ((!monolith_stability_empower_slider.IsNullOrDestroyed()) && (!monolith_stability_empower_text.IsNullOrDestroyed()) && (!monolith_dropdown.IsNullOrDestroyed()))
                        {
                            int result = System.Convert.ToInt32(monolith_stability_empower_slider.value);
                            int index = monolith_dropdown.value;
                            if (!Refs_Manager.player_data.IsNullOrDestroyed())
                            {
                                foreach (SavedMonolithRun run in Refs_Manager.player_data.MonolithRuns)
                                {
                                    if ((run.TimelineID == index) && (run.DifficultyIndex == 1))
                                    {
                                        if (run.Stability != result) { run.Stability = result; }
                                        break;
                                    }
                                }
                            }
                            monolith_stability_empower_text.text = result.ToString();
                        }
                    }

                    public static GameObject monolith_corruption_go = null;
                    public static Text monolith_corruption_text = null;
                    public static Slider monolith_corruption_slider = null;
                    public static readonly System.Action<float> monolith_corruption_slider_Action = new System.Action<float>(Set_monolith_corruption_empower);
                    public static void Set_monolith_corruption_empower(float f)
                    {
                        if ((!monolith_corruption_slider.IsNullOrDestroyed()) && (!monolith_corruption_text.IsNullOrDestroyed()) && (!monolith_dropdown.IsNullOrDestroyed()))
                        {
                            int result = System.Convert.ToInt32(monolith_corruption_slider.value);
                            int index = monolith_dropdown.value;
                            if (!Refs_Manager.player_data.IsNullOrDestroyed())
                            {
                                foreach (SavedMonolithRun run in Refs_Manager.player_data.MonolithRuns)
                                {
                                    if ((run.TimelineID == index) && (run.DifficultyIndex == 1))
                                    {
                                        if (!run.SavedEchoWeb.IsNullOrDestroyed())
                                        {
                                            if (run.SavedEchoWeb.Corruption != result) { run.SavedEchoWeb.Corruption = result; }
                                        }
                                        break;
                                    }
                                }
                            }
                            monolith_corruption_text.text = result.ToString();
                        }
                    }

                    public static GameObject monolith_gaze_go = null;
                    public static Text monolith_gaze_text = null;
                    public static Slider monolith_gaze_slider = null;
                    public static readonly System.Action<float> monolith_gaze_slider_Action = new System.Action<float>(Set_monolith_gaze_empower);
                    public static void Set_monolith_gaze_empower(float f)
                    {
                        if ((!monolith_gaze_slider.IsNullOrDestroyed()) && (!monolith_gaze_text.IsNullOrDestroyed()) && (!monolith_dropdown.IsNullOrDestroyed()))
                        {
                            int result = System.Convert.ToInt32(monolith_gaze_slider.value);
                            int index = monolith_dropdown.value;
                            if (!Refs_Manager.player_data.IsNullOrDestroyed())
                            {
                                foreach (SavedMonolithRun run in Refs_Manager.player_data.MonolithRuns)
                                {
                                    if ((run.TimelineID == index) && (run.DifficultyIndex == 1))
                                    {
                                        if (!run.SavedEchoWeb.IsNullOrDestroyed())
                                        {
                                            if (run.SavedEchoWeb.GazeOfOrobyss != result) { run.SavedEchoWeb.GazeOfOrobyss = result; }
                                        }
                                        break;
                                    }
                                }
                            }
                            monolith_gaze_text.text = result.ToString();
                        }
                    }
                                        
                    public static Button save_button = null;
                    public static readonly System.Action Save_OnClick_Action = new System.Action(Save_Click);
                    public static void Save_Click()
                    {
                        Main.logger_instance.Msg("Save Character Data");
                        if (!Refs_Manager.player_data.IsNullOrDestroyed()) { Refs_Manager.player_data.SaveData(); }
                    }
                }
                public class Faction_Tracker
                {
                    public static Il2CppLE.Factions.Faction selected_faction = null;
                    public static Il2CppLE.Factions.Faction GetFaction()
                    {
                        Il2CppLE.Factions.Faction faction = null;
                        if (!Refs_Manager.faction_tracker.IsNullOrDestroyed())
                        {                            
                            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<Il2CppLE.Factions.FactionID, Il2CppLE.Factions.Faction> values in Refs_Manager.faction_tracker.factions)
                            {
                                if (((values.Key == Il2CppLE.Factions.FactionID.CircleOfFortune) && (factions_dropdown.value == 1)) ||
                                    ((values.Key == Il2CppLE.Factions.FactionID.MerchantsGuild) && (factions_dropdown.value == 2)) ||
                                    ((values.Key == Il2CppLE.Factions.FactionID.ForgottenKnights) && (factions_dropdown.value == 3)) ||
                                    ((values.Key == Il2CppLE.Factions.FactionID.TheWeaver) && (factions_dropdown.value == 4)))
                                {
                                    faction = values.Value;
                                }
                            }
                        }

                        return faction;
                    }
                                        
                    public static Dropdown factions_dropdown = null;
                    public static GameObject factions_go = null;
                    public static Text factions_favor_text = null;
                    public static Slider factions_favor_slider = null;
                    public static readonly System.Action<float> factions_favor_slider_Action = new System.Action<float>(factions_favor);
                    public static void factions_favor(float f)
                    {
                        int result = System.Convert.ToInt32(factions_favor_slider.value);
                        if (!factions_favor_text.IsNullOrDestroyed()) { factions_favor_text.text = result.ToString(); }
                    }
                    public static Button factions_gain_favor_button = null;
                    public static readonly System.Action factions_gain_favor_OnClick_Action = new System.Action(factions_gain_favor_Click);
                    public static void factions_gain_favor_Click()
                    {
                        if ((!factions_favor_slider.IsNullOrDestroyed()) && (!selected_faction.IsNullOrDestroyed()))
                        {
                            if (!selected_faction.hasEverJoined) { selected_faction.Join(true); }
                            if (!selected_faction.IsMember) { selected_faction.IsMember = true; }

                            int added_favor = Il2CppSystem.Convert.ToInt32(factions_favor_slider.value);
                            int max_favor = 999999;
                            int max_added_favor = max_favor - selected_faction.Favor;
                            if (added_favor > max_added_favor) { added_favor = max_added_favor; }

                            selected_faction.GainFavor(added_favor, false);
                            selected_faction.SaveAndSync(false);
                        }
                        else { Main.logger_instance.Error("Faction not found"); }
                    }
                    public static Button factions_set_favor_button = null;
                    public static readonly System.Action factions_set_favor_OnClick_Action = new System.Action(factions_set_favor_Click);
                    public static void factions_set_favor_Click()
                    {
                        if ((!factions_favor_slider.IsNullOrDestroyed()) && (!selected_faction.IsNullOrDestroyed()))
                        {
                            if (!selected_faction.hasEverJoined) { selected_faction.Join(true); }
                            if (!selected_faction.IsMember) { selected_faction.IsMember = true; }

                            int added_favor = Il2CppSystem.Convert.ToInt32(factions_favor_slider.value);
                            int max_favor = 999999;
                            int max_added_favor = max_favor - selected_faction.Favor;
                            if (added_favor > max_added_favor) { added_favor = max_added_favor; }

                            selected_faction.Favor = added_favor;
                            selected_faction.SaveAndSync(false);
                        }
                        else { Main.logger_instance.Error("Faction not found"); }
                    }

                    public static Text factions_rank_text = null;
                    public static Slider factions_rank_slider = null;
                    public static readonly System.Action<float> factions_rank_slider_Action = new System.Action<float>(factions_rank);
                    public static void factions_rank(float f)
                    {
                        int result = System.Convert.ToInt32(factions_rank_slider.value);
                        if (!factions_rank_text.IsNullOrDestroyed()){ factions_rank_text.text = result.ToString(); }
                    }
                    public static Button factions_set_rank_button = null;
                    public static readonly System.Action factions_set_rank_OnClick_Action = new System.Action(factions_set_rank_Click);
                    public static void factions_set_rank_Click()
                    {
                        if ((!factions_rank_slider.IsNullOrDestroyed()) && (!selected_faction.IsNullOrDestroyed()))
                        {
                            if (!selected_faction.hasEverJoined) { selected_faction.Join(true); }
                            if (!selected_faction.IsMember) { selected_faction.IsMember = true; }

                            int max_rank = 12;
                            if ((factions_dropdown.value == 3) || (factions_dropdown.value == 4)) { max_rank = 10; }
                            int rank = Il2CppSystem.Convert.ToInt32(factions_rank_slider.value);
                            if (rank > max_rank) { rank = max_rank; }

                            selected_faction.Rank = rank;
                            selected_faction.SaveAndSync(false);
                        }
                        else { Main.logger_instance.Error("Faction not found"); }
                    }

                    public static Text factions_reputation_text = null;
                    public static Slider factions_reputation_slider = null;
                    public static readonly System.Action<float> factions_reputation_slider_Action = new System.Action<float>(factions_reputation);
                    public static void factions_reputation(float f)
                    {
                        int result = System.Convert.ToInt32(factions_reputation_slider.value);
                        if (!factions_reputation_text.IsNullOrDestroyed()) { factions_reputation_text.text = result.ToString(); }
                    }
                    public static Button factions_set_reputation_button = null;
                    public static readonly System.Action factions_set_reputation_OnClick_Action = new System.Action(factions_set_reputation_Click);
                    public static void factions_set_reputation_Click()
                    {
                        if ((!factions_reputation_slider.IsNullOrDestroyed()) && (!selected_faction.IsNullOrDestroyed()))
                        {
                            if (!selected_faction.hasEverJoined) { selected_faction.Join(true); }
                            if (!selected_faction.IsMember) { selected_faction.IsMember = true; }

                            int favor = Il2CppSystem.Convert.ToInt32(factions_reputation_slider.value);
                            int total_reputation = selected_faction.TotalReputation;
                            if (favor > total_reputation) { favor = total_reputation; }

                            selected_faction.Reputation = favor;
                            selected_faction.SaveAndSync(false);
                        }
                        else { Main.logger_instance.Error("Faction not found"); }
                    }
                }
                public class Buffs
                {
                    public static Toggle enable_mod = null;

                    public static Toggle movespeed_toggle = null;
                    public static Text movespeed_text = null;
                    public static Slider movespeed_slider = null;

                    public static Toggle damage_toggle = null;
                    public static Text damage_text = null;
                    public static Slider damage_slider = null;

                    public static Toggle attackspeed_toggle = null;
                    public static Text attackspeed_text = null;
                    public static Slider attackspeed_slider = null;

                    public static Toggle castingspeed_toggle = null;
                    public static Text castingspeed_text = null;
                    public static Slider castingspeed_slider = null;

                    public static Toggle criticalchance_toggle = null;
                    public static Text criticalchance_text = null;
                    public static Slider criticalchance_slider = null;

                    public static Toggle criticalmultiplier_toggle = null;
                    public static Text criticalmultiplier_text = null;
                    public static Slider criticalmultiplier_slider = null;

                    public static Toggle healthregen_toggle = null;
                    public static Text healthregen_text = null;
                    public static Slider healthregen_slider = null;

                    public static Toggle manaregen_toggle = null;
                    public static Text manaregen_text = null;
                    public static Slider manaregen_slider = null;

                    public static Toggle str_toggle = null;
                    public static Text str_text = null;
                    public static Slider str_slider = null;

                    public static Toggle int_toggle = null;
                    public static Text int_text = null;
                    public static Slider int_slider = null;

                    public static Toggle dex_toggle = null;
                    public static Text dex_text = null;
                    public static Slider dex_slider = null;

                    public static Toggle vit_toggle = null;
                    public static Text vit_text = null;
                    public static Slider vit_slider = null;

                    public static Toggle att_toggle = null;
                    public static Text att_text = null;
                    public static Slider att_slider = null;
                }
            }
            public class Items
            {
                public static GameObject content_obj = null;
                public static bool controls_initialized = false;
                public static bool enable = false;

                public static void Get_Refs()
                {
                    if (!Content.content_obj.IsNullOrDestroyed())
                    {
                        content_obj = Functions.GetChild(Content.content_obj, "Items_Content");
                        if (!content_obj.IsNullOrDestroyed())
                        {
                            GameObject items_drop_content = Functions.GetViewportContent(content_obj, "Items_Drop", "Items_Data_Content");
                            if (!items_drop_content.IsNullOrDestroyed())
                            {
                                GameObject force = Functions.GetChild(items_drop_content, "Force");
                                if (!force.IsNullOrDestroyed())
                                {
                                    Drop.force_unique_toggle = Functions.Get_ToggleInPanel(force, "ForceUnique", "Toggle_Items_Drop_ForceUnique");
                                    Drop.force_set_toggle = Functions.Get_ToggleInPanel(force, "ForceSet", "Toggle_Items_Drop_ForceSet");
                                    Drop.force_legendary_toggle = Functions.Get_ToggleInPanel(force, "ForceLegendary", "Toggle_Items_Drop_ForceLegendary");
                                }
                                Drop.implicits_toggle = Functions.Get_ToggleInPanel(items_drop_content, "Implicits", "Toggle_Items_Drop_Implicits");
                                Drop.implicits_text = Functions.Get_TextInToggle(items_drop_content, "Implicits", "Toggle_Items_Drop_Implicits", "Value");
                                Drop.implicits_slider_min = Functions.Get_SliderInPanel(items_drop_content, "Implicits", "Slider_Items_Drop_Implicits_Min");
                                Drop.implicits_slider_max = Functions.Get_SliderInPanel(items_drop_content, "Implicits", "Slider_Items_Drop_Implicits_Max");

                                Drop.forgin_potencial_toggle = Functions.Get_ToggleInPanel(items_drop_content, "ForginPotencial", "Toggle_Items_Drop_ForginPotencial");
                                Drop.forgin_potencial_text = Functions.Get_TextInToggle(items_drop_content, "ForginPotencial", "Toggle_Items_Drop_ForginPotencial", "Value");
                                Drop.forgin_potencial_slider_min = Functions.Get_SliderInPanel(items_drop_content, "ForginPotencial", "Slider_Items_Drop_ForginPotencial_Min");
                                Drop.forgin_potencial_slider_max = Functions.Get_SliderInPanel(items_drop_content, "ForginPotencial", "Slider_Items_Drop_ForginPotencial_Max");

                                Drop.force_seal_toggle = Functions.Get_ToggleInPanel(items_drop_content, "ForceSeal", "Toggle_Items_Drop_ForceSeal");

                                Drop.seal_tier_toggle = Functions.Get_ToggleInPanel(items_drop_content, "SealTier", "Toggle_Items_Drop_SealTier");
                                Drop.seal_tier_text = Functions.Get_TextInToggle(items_drop_content, "SealTier", "Toggle_Items_Drop_SealTier", "Value");
                                Drop.seal_tier_slider_min = Functions.Get_SliderInPanel(items_drop_content, "SealTier", "Slider_Items_Drop_SealTier_Min");
                                Drop.seal_tier_slider_max = Functions.Get_SliderInPanel(items_drop_content, "SealTier", "Slider_Items_Drop_SealTier_Max");

                                Drop.seal_value_toggle = Functions.Get_ToggleInPanel(items_drop_content, "SealValue", "Toggle_Items_Drop_SealValue");
                                Drop.seal_value_text = Functions.Get_TextInToggle(items_drop_content, "SealValue", "Toggle_Items_Drop_SealValue", "Value");
                                Drop.seal_value_slider_min = Functions.Get_SliderInPanel(items_drop_content, "SealValue", "Slider_Items_Drop_SealValue_Min");
                                Drop.seal_value_slider_max = Functions.Get_SliderInPanel(items_drop_content, "SealValue", "Slider_Items_Drop_SealValue_Max");

                                Drop.affix_count_toggle = Functions.Get_ToggleInPanel(items_drop_content, "NbAffixes", "Toggle_Items_Drop_NbAffixes");
                                Drop.affix_count_text = Functions.Get_TextInToggle(items_drop_content, "NbAffixes", "Toggle_Items_Drop_NbAffixes", "Value");
                                Drop.affix_count_slider_min = Functions.Get_SliderInPanel(items_drop_content, "NbAffixes", "Slider_Items_Drop_NbAffixes_Min");
                                Drop.affix_count_slider_min.maxValue = 6;
                                Drop.affix_count_slider_max = Functions.Get_SliderInPanel(items_drop_content, "NbAffixes", "Slider_Items_Drop_NbAffixes_Max");
                                Drop.affix_count_slider_max.maxValue = 6;

                                Drop.affix_tiers_toggle = Functions.Get_ToggleInPanel(items_drop_content, "AffixesTiers", "Toggle_Items_Drop_AffixesTiers");
                                Drop.affix_tiers_text = Functions.Get_TextInToggle(items_drop_content, "AffixesTiers", "Toggle_Items_Drop_AffixesTiers", "Value");
                                Drop.affix_tiers_slider_min = Functions.Get_SliderInPanel(items_drop_content, "AffixesTiers", "Slider_Items_Drop_AffixesTiers_Min");
                                Drop.affix_tiers_slider_max = Functions.Get_SliderInPanel(items_drop_content, "AffixesTiers", "Slider_Items_Drop_AffixesTiers_Max");

                                Drop.affix_values_toggle = Functions.Get_ToggleInPanel(items_drop_content, "AffixesValues", "Toggle_Items_Drop_AffixesValues");
                                Drop.affix_values_text = Functions.Get_TextInToggle(items_drop_content, "AffixesValues", "Toggle_Items_Drop_AffixesValues", "Value");
                                Drop.affix_values_slider_min = Functions.Get_SliderInPanel(items_drop_content, "AffixesValues", "Slider_Items_Drop_AffixesValues_Min");
                                Drop.affix_values_slider_max = Functions.Get_SliderInPanel(items_drop_content, "AffixesValues", "Slider_Items_Drop_AffixesValues_Max");

                                Drop.unique_mods_toggle = Functions.Get_ToggleInPanel(items_drop_content, "UniqueMods", "Toggle_Items_Drop_UniqueMods");
                                Drop.unique_mods_text = Functions.Get_TextInToggle(items_drop_content, "UniqueMods", "Toggle_Items_Drop_UniqueMods", "Value");
                                Drop.unique_mods_slider_min = Functions.Get_SliderInPanel(items_drop_content, "UniqueMods", "Slider_Items_Drop_UniqueMods_Min");
                                Drop.unique_mods_slider_max = Functions.Get_SliderInPanel(items_drop_content, "UniqueMods", "Slider_Items_Drop_UniqueMods_Max");

                                Drop.legendary_potencial_toggle = Functions.Get_ToggleInPanel(items_drop_content, "LegendaryPotencial", "Toggle_Items_Drop_LegendaryPotencial");
                                Drop.legendary_potencial_text = Functions.Get_TextInToggle(items_drop_content, "LegendaryPotencial", "Toggle_Items_Drop_LegendaryPotencial", "Value");
                                Drop.legendary_potencial_slider_min = Functions.Get_SliderInPanel(items_drop_content, "LegendaryPotencial", "Slider_Items_Drop_LegendaryPotencial_Min");
                                Drop.legendary_potencial_slider_max = Functions.Get_SliderInPanel(items_drop_content, "LegendaryPotencial", "Slider_Items_Drop_LegendaryPotencial_Max");

                                Drop.weaver_will_toggle = Functions.Get_ToggleInPanel(items_drop_content, "WeaverWill", "Toggle_Items_Drop_WeaverWill");
                                Drop.weaver_will_text = Functions.Get_TextInToggle(items_drop_content, "WeaverWill", "Toggle_Items_Drop_WeaverWill", "Value");
                                Drop.weaver_will_slider_min = Functions.Get_SliderInPanel(items_drop_content, "WeaverWill", "Slider_Items_Drop_WeaverWill_Min");
                                Drop.weaver_will_slider_max = Functions.Get_SliderInPanel(items_drop_content, "WeaverWill", "Slider_Items_Drop_WeaverWill_Max");
                            }

                            GameObject items_pickup_content = Functions.GetViewportContent(content_obj, "Items_Pickup", "Items_Pickup_Content");
                            if (!items_pickup_content.IsNullOrDestroyed())
                            {
                                Pickup.autopickup_gold_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_Gold", "Toggle_Items_Pickup_AutoPickup_Gold");
                                Pickup.autopickup_keys_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_Keys", "Toggle_Items_Pickup_AutoPickup_Keys");
                                Pickup.autopickup_potions_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_Pots", "Toggle_Items_Pickup_AutoPickup_Pots");
                                Pickup.autopickup_xptome_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_XpTome", "Toggle_Items_Pickup_AutoPickup_XpTome");
                                Pickup.autopickup_favortome_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_FavorTome", "Toggle_Items_Pickup_AutoPickup_FavorTome");
                                Pickup.autopickup_memoryamber_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_MemoryAmber", "Toggle_Items_Pickup_AutoPickup_MemoryAmber");
                                Pickup.autopickup_wovenechoes_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_WovenEchoes", "Toggle_Items_Pickup_AutoPickup_WovenEchoes");
                                Pickup.autopickup_materials_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_Materials", "Toggle_Items_Pickup_AutoPickup_Materials");
                                Pickup.autopickup_fromfilter_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_Filters", "Toggle_Items_Pickup_AutoPickup_Filters");

                                Pickup.autostore_materials_ondrop_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoStore_OnDrop", "Toggle_Items_Pickup_AutoStore_OnDrop");
                                Pickup.autostore_materials_oninventoryopen_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoStore_OnInventoryOpen", "Toggle_Items_Pickup_AutoStore_OnInventoryOpen");
                                Pickup.autostore_materials_Timer_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoStore_Timer", "Toggle_Items_Pickup_AutoStore_Timer");                                
                                Pickup.autostore_materials_Timer_text = Functions.Get_TextInToggle(items_pickup_content, "AutoStore_Timer", "Toggle_Items_Pickup_AutoStore_Timer", "Label");
                                Pickup.autostore_materials_Timer_slider = Functions.Get_SliderInPanel(items_pickup_content, "AutoStore_Timer", "Slider_Items_Pickup_AutoStore_Timer");

                                Pickup.autosell_hide_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoSell_FromFilter", "Toggle_Items_Pickup_AutoSell_FromFilter");
                                
                                Pickup.autoshatter_hide_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoShatter_FromFilter", "Toggle_Items_Pickup_AutoShatter_FromFilter");
                                Pickup.autoshatter_rune_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoShatter_Rune", "Toggle_Items_Pickup_AutoShatter_Rune");
                                Pickup.autoshatter_chance_text = Functions.Get_TextInPanel(items_pickup_content, "AutoShatter_Chance", "Value");
                                Pickup.autoshatter_chance_slider = Functions.Get_SliderInPanel(items_pickup_content, "AutoShatter_Chance", "Slider_Items_Pickup_AutoShatter_Chance");
                                Pickup.autoshatter_affix_chance_text = Functions.Get_TextInPanel(items_pickup_content, "AutoShatter_AffixChance", "Value");
                                Pickup.autoshatter_affix_chance_slider = Functions.Get_SliderInPanel(items_pickup_content, "AutoShatter_AffixChance", "Slider_Items_Pickup_AutoShatter_AffixChance");
                                Pickup.autoshatter_quantity_chance_text = Functions.Get_TextInPanel(items_pickup_content, "AutoShatter_QuantityChance", "Value");
                                Pickup.autoshatter_quantity_chance_slider = Functions.Get_SliderInPanel(items_pickup_content, "AutoShatter_QuantityChance", "Slider_Items_Pickup_AutoShatter_QuantityChance");

                                Pickup.range_pickup_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "Range_Pickup", "Toggle_Items_Pickup_Range_Pickup");
                                Pickup.hide_materials_notifications_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "Hide_Notifications", "Toggle_Items_Pickup_Hide_Notifications");
                            }

                            GameObject items_req_content = Functions.GetViewportContent(content_obj, "Items_Pickup", "Items_Req_Content");
                            if (!items_req_content.IsNullOrDestroyed())
                            {
                                Requirements.class_req_toggle = Functions.Get_ToggleInPanel(items_req_content, "RemoveReq_Class", "Toggle_RemoveReq_Class");
                                Requirements.level_req_toggle = Functions.Get_ToggleInPanel(items_req_content, "RemoveReq_Level", "Toggle_RemoveReq_Level");
                                Requirements.set_req_toggle = Functions.Get_ToggleInPanel(items_req_content, "RemoveReq_Set", "Toggle_RemoveReq_Set");
                            }
                            else { Main.logger_instance.Error("Requirements"); }

                            GameObject items_forcedrop_content = Functions.GetViewportContent(content_obj, "Items_Pickup", "Items_ForceDrop_Content");
                            if (!items_forcedrop_content.IsNullOrDestroyed())
                            {
                                ForceDrop.forcedrop_type_dropdown = Functions.Get_DopboxInPanel(items_forcedrop_content, "Type", "Dropdown_Items_ForceDrop_Type", new System.Action<int>((_) => { Content.Items.ForceDrop.SelectType(); }));
                                ForceDrop.forcedrop_rarity_dropdown = Functions.Get_DopboxInPanel(items_forcedrop_content, "Rarity", "Dropdown_Items_ForceDrop_Rarity", new System.Action<int>((_) => { Content.Items.ForceDrop.SelectRarity(); }));
                                ForceDrop.forcedrop_items_dropdown = Functions.Get_DopboxInPanel(items_forcedrop_content, "Item", "Dropdown_Items_ForceDrop_Item", new System.Action<int>((_) => { Content.Items.ForceDrop.SelectItem(); }));
                                ForceDrop.forcedrop_quantity_text = Functions.Get_TextInButton(items_forcedrop_content, "Quantity", "Value");
                                ForceDrop.forcedrop_quantity_slider = Functions.Get_SliderInPanel(items_forcedrop_content, "Quantity", "Slider_Items_ForceDrop_Quantity");
                                GameObject new_obj = Functions.GetChild(content_obj, "Items_Pickup");
                                if (!new_obj.IsNullOrDestroyed())
                                {
                                    ForceDrop.forcedrop_drop_button = Functions.Get_ButtonInPanel(new_obj, "Btn_Items_ForceDrop_Drop");
                                }
                            }
                            else { Main.logger_instance.Error("Forcedrop"); }

                            CraftingSlot.enable_mod = Functions.Get_ToggleInLabel(content_obj, "Items_Craft", "Toggle_Items_Craft_Enable", makeSureItsActive: true);
                            GameObject items_craft_content = Functions.GetViewportContent(content_obj, "Items_Craft", "Items_Craft_Content");
                            if (!items_craft_content.IsNullOrDestroyed())
                            {
                                CraftingSlot.forgin_potencial_toggle = Functions.Get_ToggleInPanel(items_craft_content, "ForginPotencial", "Toggle_Items_Craft_ForginPotencial");
                                CraftingSlot.forgin_potencial_text = Functions.Get_TextInToggle(items_craft_content, "ForginPotencial", "Toggle_Items_Craft_ForginPotencial", "Value");
                                CraftingSlot.forgin_potencial_slider = Functions.Get_SliderInPanel(items_craft_content, "ForginPotencial", "Slider_Items_Craft_ForginPotencial");

                                CraftingSlot.implicit_0_toggle = Functions.Get_ToggleInPanel(items_craft_content, "Implicit0", "Toggle_Items_Craft_Implicit0");
                                CraftingSlot.implicit_0_text = Functions.Get_TextInToggle(items_craft_content, "Implicit0", "Toggle_Items_Craft_Implicit0", "Value");
                                CraftingSlot.implicit_0_slider = Functions.Get_SliderInPanel(items_craft_content, "Implicit0", "Slider_Items_Craft_Implicit0");

                                CraftingSlot.implicit_1_toggle = Functions.Get_ToggleInPanel(items_craft_content, "Implicit1", "Toggle_Items_Craft_Implicit1");
                                CraftingSlot.implicit_1_text = Functions.Get_TextInToggle(items_craft_content, "Implicit1", "Toggle_Items_Craft_Implicit1", "Value");
                                CraftingSlot.implicit_1_slider = Functions.Get_SliderInPanel(items_craft_content, "Implicit1", "Slider_Items_Craft_Implicit1");

                                CraftingSlot.implicit_2_toggle = Functions.Get_ToggleInPanel(items_craft_content, "Implicit2", "Toggle_Items_Craft_Implicit2");
                                CraftingSlot.implicit_2_text = Functions.Get_TextInToggle(items_craft_content, "Implicit2", "Toggle_Items_Craft_Implicit2", "Value");
                                CraftingSlot.implicit_2_slider = Functions.Get_SliderInPanel(items_craft_content, "Implicit2", "Slider_Items_Craft_Implicit2");

                                CraftingSlot.seal_tier_toggle = Functions.Get_ToggleInPanel(items_craft_content, "SealTier", "Toggle_Items_Craft_SealTier");
                                CraftingSlot.seal_tier_text = Functions.Get_TextInToggle(items_craft_content, "SealTier", "Toggle_Items_Craft_SealTier", "Value");
                                CraftingSlot.seal_tier_slider = Functions.Get_SliderInPanel(items_craft_content, "SealTier", "Slider_Items_Craft_SealTier");

                                CraftingSlot.seal_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "SealValue", "Toggle_Items_Craft_SealValue");
                                CraftingSlot.seal_value_text = Functions.Get_TextInToggle(items_craft_content, "SealValue", "Toggle_Items_Craft_SealValue", "Value");
                                CraftingSlot.seal_value_slider = Functions.Get_SliderInPanel(items_craft_content, "SealValue", "Slider_Items_Craft_SealValue");

                                CraftingSlot.affix_0_tier_toggle = Functions.Get_ToggleInPanel(items_craft_content, "AffixTier0", "Toggle_Items_Craft_AffixTier0");
                                CraftingSlot.affix_0_tier_text = Functions.Get_TextInToggle(items_craft_content, "AffixTier0", "Toggle_Items_Craft_AffixTier0", "Value");
                                CraftingSlot.affix_0_tier_slider = Functions.Get_SliderInPanel(items_craft_content, "AffixTier0", "Slider_Items_Craft_AffixTier0");

                                CraftingSlot.affix_0_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "AffixValue0", "Toggle_Items_Craft_AffixValue0");
                                CraftingSlot.affix_0_value_text = Functions.Get_TextInToggle(items_craft_content, "AffixValue0", "Toggle_Items_Craft_AffixValue0", "Value");
                                CraftingSlot.affix_0_value_slider = Functions.Get_SliderInPanel(items_craft_content, "AffixValue0", "Slider_Items_Craft_AffixValue0");

                                CraftingSlot.affix_1_tier_toggle = Functions.Get_ToggleInPanel(items_craft_content, "AffixTier1", "Toggle_Items_Craft_AffixTier1");
                                CraftingSlot.affix_1_tier_text = Functions.Get_TextInToggle(items_craft_content, "AffixTier1", "Toggle_Items_Craft_AffixTier1", "Value");
                                CraftingSlot.affix_1_tier_slider = Functions.Get_SliderInPanel(items_craft_content, "AffixTier1", "Slider_Items_Craft_AffixTier1");

                                CraftingSlot.affix_1_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "AffixValue1", "Toggle_Items_Craft_AffixValue1");
                                CraftingSlot.affix_1_value_text = Functions.Get_TextInToggle(items_craft_content, "AffixValue1", "Toggle_Items_Craft_AffixValue1", "Value");
                                CraftingSlot.affix_1_value_slider = Functions.Get_SliderInPanel(items_craft_content, "AffixValue1", "Slider_Items_Craft_AffixValue1");

                                CraftingSlot.affix_2_tier_toggle = Functions.Get_ToggleInPanel(items_craft_content, "AffixTier2", "Toggle_Items_Craft_AffixTier2");
                                CraftingSlot.affix_2_tier_text = Functions.Get_TextInToggle(items_craft_content, "AffixTier2", "Toggle_Items_Craft_AffixTier2", "Value");
                                CraftingSlot.affix_2_tier_slider = Functions.Get_SliderInPanel(items_craft_content, "AffixTier2", "Slider_Items_Craft_AffixTier2");

                                CraftingSlot.affix_2_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "AffixValue2", "Toggle_Items_Craft_AffixValue2");
                                CraftingSlot.affix_2_value_text = Functions.Get_TextInToggle(items_craft_content, "AffixValue2", "Toggle_Items_Craft_AffixValue2", "Value");
                                CraftingSlot.affix_2_value_slider = Functions.Get_SliderInPanel(items_craft_content, "AffixValue2", "Slider_Items_Craft_AffixValue2");

                                CraftingSlot.affix_3_tier_toggle = Functions.Get_ToggleInPanel(items_craft_content, "AffixTier3", "Toggle_Items_Craft_AffixTier3");
                                CraftingSlot.affix_3_tier_text = Functions.Get_TextInToggle(items_craft_content, "AffixTier3", "Toggle_Items_Craft_AffixTier3", "Value");
                                CraftingSlot.affix_3_tier_slider = Functions.Get_SliderInPanel(items_craft_content, "AffixTier3", "Slider_Items_Craft_AffixTier3");

                                CraftingSlot.affix_3_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "AffixValue3", "Toggle_Items_Craft_AffixValue3");
                                CraftingSlot.affix_3_value_text = Functions.Get_TextInToggle(items_craft_content, "AffixValue3", "Toggle_Items_Craft_AffixValue3", "Value");
                                CraftingSlot.affix_3_value_slider = Functions.Get_SliderInPanel(items_craft_content, "AffixValue3", "Slider_Items_Craft_AffixValue3");

                                CraftingSlot.uniquemod_0_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "UniqueMod0", "Toggle_Items_Craft_UniqueMod0");
                                CraftingSlot.uniquemod_0_value_text = Functions.Get_TextInToggle(items_craft_content, "UniqueMod0", "Toggle_Items_Craft_UniqueMod0", "Value");
                                CraftingSlot.uniquemod_0_value_slider = Functions.Get_SliderInPanel(items_craft_content, "UniqueMod0", "Slider_Items_Craft_UniqueMod0");

                                CraftingSlot.uniquemod_1_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "UniqueMod1", "Toggle_Items_Craft_UniqueMod1");
                                CraftingSlot.uniquemod_1_value_text = Functions.Get_TextInToggle(items_craft_content, "UniqueMod1", "Toggle_Items_Craft_UniqueMod1", "Value");
                                CraftingSlot.uniquemod_1_value_slider = Functions.Get_SliderInPanel(items_craft_content, "UniqueMod1", "Slider_Items_Craft_UniqueMod1");

                                CraftingSlot.uniquemod_2_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "UniqueMod2", "Toggle_Items_Craft_UniqueMod2");
                                CraftingSlot.uniquemod_2_value_text = Functions.Get_TextInToggle(items_craft_content, "UniqueMod2", "Toggle_Items_Craft_UniqueMod2", "Value");
                                CraftingSlot.uniquemod_2_value_slider = Functions.Get_SliderInPanel(items_craft_content, "UniqueMod2", "Slider_Items_Craft_UniqueMod2");

                                CraftingSlot.uniquemod_3_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "UniqueMod3", "Toggle_Items_Craft_UniqueMod3");
                                CraftingSlot.uniquemod_3_value_text = Functions.Get_TextInToggle(items_craft_content, "UniqueMod3", "Toggle_Items_Craft_UniqueMod3", "Value");
                                CraftingSlot.uniquemod_3_value_slider = Functions.Get_SliderInPanel(items_craft_content, "UniqueMod3", "Slider_Items_Craft_UniqueMod3");

                                CraftingSlot.uniquemod_4_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "UniqueMod4", "Toggle_Items_Craft_UniqueMod4");
                                CraftingSlot.uniquemod_4_value_text = Functions.Get_TextInToggle(items_craft_content, "UniqueMod4", "Toggle_Items_Craft_UniqueMod4", "Value");
                                CraftingSlot.uniquemod_4_value_slider = Functions.Get_SliderInPanel(items_craft_content, "UniqueMod4", "Slider_Items_Craft_UniqueMod4");

                                CraftingSlot.uniquemod_5_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "UniqueMod5", "Toggle_Items_Craft_UniqueMod5");
                                CraftingSlot.uniquemod_5_value_text = Functions.Get_TextInToggle(items_craft_content, "UniqueMod5", "Toggle_Items_Craft_UniqueMod5", "Value");
                                CraftingSlot.uniquemod_5_value_slider = Functions.Get_SliderInPanel(items_craft_content, "UniqueMod5", "Slider_Items_Craft_UniqueMod5");

                                CraftingSlot.uniquemod_6_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "UniqueMod6", "Toggle_Items_Craft_UniqueMod6");
                                CraftingSlot.uniquemod_6_value_text = Functions.Get_TextInToggle(items_craft_content, "UniqueMod6", "Toggle_Items_Craft_UniqueMod6", "Value");
                                CraftingSlot.uniquemod_6_value_slider = Functions.Get_SliderInPanel(items_craft_content, "UniqueMod6", "Slider_Items_Craft_UniqueMod6");

                                CraftingSlot.uniquemod_7_value_toggle = Functions.Get_ToggleInPanel(items_craft_content, "UniqueMod7", "Toggle_Items_Craft_UniqueMod7");
                                CraftingSlot.uniquemod_7_value_text = Functions.Get_TextInToggle(items_craft_content, "UniqueMod7", "Toggle_Items_Craft_UniqueMod7", "Value");
                                CraftingSlot.uniquemod_7_value_slider = Functions.Get_SliderInPanel(items_craft_content, "UniqueMod7", "Slider_Items_Craft_UniqueMod7");

                                CraftingSlot.legendary_potencial_toggle = Functions.Get_ToggleInPanel(items_craft_content, "LegendaryPotencial", "Toggle_Items_Craft_LegendaryPotencial");
                                CraftingSlot.legendary_potencial_text = Functions.Get_TextInToggle(items_craft_content, "LegendaryPotencial", "Toggle_Items_Craft_LegendaryPotencial", "Value");
                                CraftingSlot.legendary_potencial_slider = Functions.Get_SliderInPanel(items_craft_content, "LegendaryPotencial", "Slider_Items_Craft_LegendaryPotencial");

                                CraftingSlot.weaver_will_toggle = Functions.Get_ToggleInPanel(items_craft_content, "WeaverWill", "Toggle_Items_Craft_WeaverWill");
                                CraftingSlot.weaver_will_text = Functions.Get_TextInToggle(items_craft_content, "WeaverWill", "Toggle_Items_Craft_WeaverWill", "Value");
                                CraftingSlot.weaver_will_slider = Functions.Get_SliderInPanel(items_craft_content, "WeaverWill", "Slider_Items_Craft_WeaverWill");
                            }
                        }
                    }
                }
                public static void Set_Events()
                {
                    Events.Set_Toggle_Event(Requirements.level_req_toggle, Requirements.Level_Toggle_Action);
                    Events.Set_Toggle_Event(Requirements.class_req_toggle, Requirements.Class_Toggle_Action);
                    Events.Set_Toggle_Event(Requirements.set_req_toggle, Requirements.Set_Toggle_Action);

                    Events.Set_Button_Event(ForceDrop.forcedrop_drop_button, ForceDrop.Drop_OnClick_Action);
                }
                public static void Set_Active(bool show)
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static void Toggle_Active()
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        bool show = !content_obj.active;
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static bool Init_UserData()
                {
                    bool result = false;
                    if (!Save_Manager.instance.IsNullOrDestroyed())
                    {
                        if ((Save_Manager.instance.initialized) && (!Save_Manager.instance.data.IsNullOrDestroyed()))
                        {
                            //Drop
                            Drop.force_unique_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_ForceUnique;
                            Drop.force_set_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_ForceSet;
                            Drop.force_legendary_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary;

                            Drop.implicits_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_Implicits;
                            Drop.implicits_slider_min.value = Save_Manager.instance.data.Items.Drop.Implicits_Min;
                            Drop.implicits_slider_max.value = Save_Manager.instance.data.Items.Drop.Implicits_Max;

                            Drop.forgin_potencial_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_ForginPotencial;
                            Drop.forgin_potencial_slider_min.value = Save_Manager.instance.data.Items.Drop.ForginPotencial_Min;
                            Drop.forgin_potencial_slider_max.value = Save_Manager.instance.data.Items.Drop.ForginPotencial_Max;

                            Drop.force_seal_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_ForceSeal;

                            Drop.seal_tier_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_SealTier;
                            Drop.seal_tier_slider_min.value = Save_Manager.instance.data.Items.Drop.SealTier_Min;
                            Drop.seal_tier_slider_max.value = Save_Manager.instance.data.Items.Drop.SealTier_Max;

                            Drop.seal_value_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_SealValue;
                            Drop.seal_value_slider_min.value = Save_Manager.instance.data.Items.Drop.SealValue_Min;
                            Drop.seal_value_slider_max.value = Save_Manager.instance.data.Items.Drop.SealValue_Max;

                            Drop.affix_count_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_AffixCount;
                            Drop.affix_count_slider_min.value = Save_Manager.instance.data.Items.Drop.AffixCount_Min;
                            Drop.affix_count_slider_max.value = Save_Manager.instance.data.Items.Drop.AffixCount_Max;

                            Drop.affix_tiers_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_AffixTiers;
                            Drop.affix_tiers_slider_min.value = Save_Manager.instance.data.Items.Drop.AffixTiers_Min;
                            Drop.affix_tiers_slider_max.value = Save_Manager.instance.data.Items.Drop.AffixTiers_Max;

                            Drop.affix_values_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_AffixValues;
                            Drop.affix_values_slider_min.value = Save_Manager.instance.data.Items.Drop.AffixValues_Min;
                            Drop.affix_values_slider_max.value = Save_Manager.instance.data.Items.Drop.AffixValues_Max;

                            Drop.unique_mods_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_UniqueMods;
                            Drop.unique_mods_slider_min.value = Save_Manager.instance.data.Items.Drop.UniqueMods_Min;
                            Drop.unique_mods_slider_max.value = Save_Manager.instance.data.Items.Drop.UniqueMods_Max;

                            Drop.legendary_potencial_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary;
                            Drop.legendary_potencial_slider_min.value = Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min;
                            Drop.legendary_potencial_slider_max.value = Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max;

                            Drop.weaver_will_toggle.isOn = Save_Manager.instance.data.Items.Drop.Enable_WeaverWill;
                            Drop.weaver_will_slider_min.value = Save_Manager.instance.data.Items.Drop.WeaverWill_Min;
                            Drop.weaver_will_slider_max.value = Save_Manager.instance.data.Items.Drop.WeaverWill_Max;
                            
                            //Pickup
                            Pickup.autopickup_gold_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Gold;
                            Pickup.autopickup_keys_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Keys;
                            Pickup.autopickup_potions_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Potions;
                            Pickup.autopickup_xptome_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_XpTome;
                            Pickup.autopickup_favortome_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_FavorTome;
                            Pickup.autopickup_memoryamber_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_MemoryAmber;
                            Pickup.autopickup_wovenechoes_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_WovenEchoes;
                            Pickup.autopickup_materials_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Materials;
                            Pickup.autopickup_fromfilter_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_FromFilter;

                            Pickup.autostore_materials_ondrop_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_OnDrop;
                            Pickup.autostore_materials_oninventoryopen_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_OnInventoryOpen;
                            Pickup.autostore_materials_Timer_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_Timer;
                            Pickup.autostore_materials_Timer_slider.value = Save_Manager.instance.data.Items.Pickup.AutoStore_Timer;

                            Pickup.autosell_hide_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoSell_FromFilter;
                            Pickup.autoshatter_hide_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoShatter_FromFilter;
                            Pickup.autoshatter_rune_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoShatter_UseRune;
                            Pickup.autoshatter_chance_slider.value = Save_Manager.instance.data.Items.Pickup.AutoShatter_Chance;
                            Pickup.autoshatter_affix_chance_slider.value = Save_Manager.instance.data.Items.Pickup.AutoShatter_Affix_Chance;
                            Pickup.autoshatter_quantity_chance_slider.value = Save_Manager.instance.data.Items.Pickup.AutoShatter_Quantity_Chance;

                            Pickup.range_pickup_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_RangePickup;
                            Pickup.hide_materials_notifications_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_HideMaterialsNotifications;

                            //Requirements
                            Requirements.class_req_toggle.isOn = Save_Manager.instance.data.Items.Req.classe;
                            Requirements.level_req_toggle.isOn = Save_Manager.instance.data.Items.Req.level;
                            Requirements.set_req_toggle.isOn = Save_Manager.instance.data.Items.Req.set;

                            //Craft
                            CraftingSlot.enable_mod.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Mod;
                            CraftingSlot.forgin_potencial_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_ForginPotencial;
                            CraftingSlot.forgin_potencial_slider.value = Save_Manager.instance.data.Items.CraftingSlot.ForginPotencial;

                            CraftingSlot.implicit_0_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Implicit_0;
                            CraftingSlot.implicit_0_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Implicit_0;

                            CraftingSlot.implicit_1_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Implicit_1;
                            CraftingSlot.implicit_1_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Implicit_1;

                            CraftingSlot.implicit_2_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Implicit_2;
                            CraftingSlot.implicit_2_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Implicit_2;
                            
                            CraftingSlot.seal_tier_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Seal_Tier;
                            CraftingSlot.seal_tier_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Seal_Tier;

                            CraftingSlot.seal_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Seal_Value;
                            CraftingSlot.seal_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Seal_Value;

                            CraftingSlot.affix_0_tier_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_0_Tier;
                            CraftingSlot.affix_0_tier_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Affix_0_Tier;

                            CraftingSlot.affix_0_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_0_Value;
                            CraftingSlot.affix_0_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Affix_0_Value;

                            CraftingSlot.affix_1_tier_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_1_Tier;
                            CraftingSlot.affix_1_tier_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Affix_1_Tier;

                            CraftingSlot.affix_1_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_1_Value;
                            CraftingSlot.affix_1_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Affix_1_Value;

                            CraftingSlot.affix_2_tier_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_2_Tier;
                            CraftingSlot.affix_2_tier_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Affix_2_Tier;

                            CraftingSlot.affix_2_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_2_Value;
                            CraftingSlot.affix_2_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Affix_2_Value;

                            CraftingSlot.affix_3_tier_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_3_Tier;
                            CraftingSlot.affix_3_tier_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Affix_3_Tier;

                            CraftingSlot.affix_3_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_3_Value;
                            CraftingSlot.affix_3_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.Affix_3_Value;

                            CraftingSlot.uniquemod_0_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_0;
                            CraftingSlot.uniquemod_0_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_0;

                            CraftingSlot.uniquemod_1_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_1;
                            CraftingSlot.uniquemod_1_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_1;
                            
                            CraftingSlot.uniquemod_2_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_2;
                            CraftingSlot.uniquemod_2_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_2;

                            CraftingSlot.uniquemod_3_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_3;
                            CraftingSlot.uniquemod_3_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_3;

                            CraftingSlot.uniquemod_4_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_4;
                            CraftingSlot.uniquemod_4_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_4;

                            CraftingSlot.uniquemod_5_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_5;
                            CraftingSlot.uniquemod_5_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_5;

                            CraftingSlot.uniquemod_6_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_6;
                            CraftingSlot.uniquemod_6_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_6;

                            CraftingSlot.uniquemod_7_value_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_7;
                            CraftingSlot.uniquemod_7_value_slider.value = Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_7;

                            CraftingSlot.legendary_potencial_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_LegendaryPotencial;
                            CraftingSlot.legendary_potencial_slider.value = Save_Manager.instance.data.Items.CraftingSlot.LegendaryPotencial;

                            CraftingSlot.weaver_will_toggle.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_WeaverWill;
                            CraftingSlot.weaver_will_slider.value = Save_Manager.instance.data.Items.CraftingSlot.WeaverWill;

                            controls_initialized = true;
                            result = true;
                        }
                    }

                    return result;
                }                
                public static void UpdateVisuals()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (controls_initialized))
                    {
                        if ((Save_Manager.instance.initialized) && (!Save_Manager.instance.data.IsNullOrDestroyed()))
                        {
                            //Values
                            Drop.forgin_potencial_text.text = (int)(Save_Manager.instance.data.Items.Drop.ForginPotencial_Min) + " to " + (int)(Save_Manager.instance.data.Items.Drop.ForginPotencial_Max);
                            Drop.affix_count_text.text = (int)(Save_Manager.instance.data.Items.Drop.AffixCount_Min) + " to " + (int)(Save_Manager.instance.data.Items.Drop.AffixCount_Max);                            
                            Drop.legendary_potencial_text.text = (int)(Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min) + " to " + (int)(Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max);
                            Drop.weaver_will_text.text = (int)(Save_Manager.instance.data.Items.Drop.WeaverWill_Min) + " to " + (int)(Save_Manager.instance.data.Items.Drop.WeaverWill_Max);
                            ForceDrop.forcedrop_quantity_text.text = "" + (int)(ForceDrop.forcedrop_quantity_slider.value);
                            CraftingSlot.forgin_potencial_text.text = "" + (int)(Save_Manager.instance.data.Items.CraftingSlot.ForginPotencial);
                            CraftingSlot.legendary_potencial_text.text = "" + (int)(Save_Manager.instance.data.Items.CraftingSlot.LegendaryPotencial);
                            CraftingSlot.weaver_will_text.text = "" + (int)(Save_Manager.instance.data.Items.CraftingSlot.WeaverWill);
                            Pickup.autostore_materials_Timer_text.text = "All " + (int)Save_Manager.instance.data.Items.Pickup.AutoStore_Timer + " sec";
                            Pickup.autoshatter_chance_text.text = "" + (int)(Save_Manager.instance.data.Items.Pickup.AutoShatter_Chance) + " %";
                            Pickup.autoshatter_affix_chance_text.text = "" + (int)(Save_Manager.instance.data.Items.Pickup.AutoShatter_Affix_Chance) + " %";
                            Pickup.autoshatter_quantity_chance_text.text = "" + (int)(Save_Manager.instance.data.Items.Pickup.AutoShatter_Quantity_Chance) + " %";

                            //Tiers
                            Drop.seal_tier_text.text = ((int)(Save_Manager.instance.data.Items.Drop.SealTier_Min) + 1) + " to " + ((int)(Save_Manager.instance.data.Items.Drop.SealTier_Max) + 1);
                            Drop.affix_tiers_text.text = ((int)(Save_Manager.instance.data.Items.Drop.AffixTiers_Min) + 1) + " to " + ((int)(Save_Manager.instance.data.Items.Drop.AffixTiers_Max) + 1);
                            CraftingSlot.seal_tier_text.text = "" + ((int)(Save_Manager.instance.data.Items.CraftingSlot.Seal_Tier) + 1);
                            CraftingSlot.affix_0_tier_text.text = "" + ((int)(Save_Manager.instance.data.Items.CraftingSlot.Affix_0_Tier) + 1);
                            CraftingSlot.affix_1_tier_text.text = "" + ((int)(Save_Manager.instance.data.Items.CraftingSlot.Affix_1_Tier) + 1);
                            CraftingSlot.affix_2_tier_text.text = "" + ((int)(Save_Manager.instance.data.Items.CraftingSlot.Affix_2_Tier) + 1);
                            CraftingSlot.affix_3_tier_text.text = "" + ((int)(Save_Manager.instance.data.Items.CraftingSlot.Affix_3_Tier) + 1);

                            //%
                            Drop.implicits_text.text = (int)((Save_Manager.instance.data.Items.Drop.Implicits_Min / 255) * 100) + " to " + (int)((Save_Manager.instance.data.Items.Drop.Implicits_Max / 255) * 100) + " %";
                            Drop.seal_value_text.text = (int)((Save_Manager.instance.data.Items.Drop.SealValue_Min / 255) * 100) + " to " + (int)((Save_Manager.instance.data.Items.Drop.SealValue_Max / 255) * 100) + " %";
                            Drop.affix_values_text.text = (int)((Save_Manager.instance.data.Items.Drop.AffixValues_Min / 255) * 100) + " to " + (int)((Save_Manager.instance.data.Items.Drop.AffixValues_Max / 255) * 100) + " %";
                            Drop.unique_mods_text.text = (int)((Save_Manager.instance.data.Items.Drop.UniqueMods_Min / 255) * 100) + " to " + (int)((Save_Manager.instance.data.Items.Drop.UniqueMods_Max / 255) * 100) + " %";
                            CraftingSlot.implicit_0_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.Implicit_0 / 255) * 100) + " %";
                            CraftingSlot.implicit_1_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.Implicit_1 / 255) * 100) + " %";
                            CraftingSlot.implicit_2_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.Implicit_2 / 255) * 100) + " %";
                            CraftingSlot.seal_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.Seal_Value / 255) * 100) + " %";
                            CraftingSlot.affix_0_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.Affix_0_Value / 255) * 100) + " %";
                            CraftingSlot.affix_1_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.Affix_1_Value / 255) * 100) + " %";                            
                            CraftingSlot.affix_2_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.Affix_2_Value / 255) * 100) + " %";                            
                            CraftingSlot.affix_3_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.Affix_3_Value / 255) * 100) + " %";
                            CraftingSlot.uniquemod_0_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_0 / 255) * 100) + " %";
                            CraftingSlot.uniquemod_1_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_1 / 255) * 100) + " %";
                            CraftingSlot.uniquemod_2_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_2 / 255) * 100) + " %";
                            CraftingSlot.uniquemod_3_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_3 / 255) * 100) + " %";
                            CraftingSlot.uniquemod_4_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_4 / 255) * 100) + " %";
                            CraftingSlot.uniquemod_5_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_5 / 255) * 100) + " %";
                            CraftingSlot.uniquemod_6_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_6 / 255) * 100) + " %";
                            CraftingSlot.uniquemod_7_value_text.text = (int)((Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_7 / 255) * 100) + " %";
                        }
                    }
                }

                public class Drop
                {
                    public static Toggle force_unique_toggle = null;
                    public static Toggle force_set_toggle = null;
                    public static Toggle force_legendary_toggle = null;

                    public static Toggle implicits_toggle = null;
                    public static Text implicits_text = null;
                    public static Slider implicits_slider_min = null;
                    public static Slider implicits_slider_max = null;

                    public static Toggle forgin_potencial_toggle = null;
                    public static Text forgin_potencial_text = null;
                    public static Slider forgin_potencial_slider_min = null;
                    public static Slider forgin_potencial_slider_max = null;

                    public static Toggle force_seal_toggle = null;

                    public static Toggle seal_tier_toggle = null;
                    public static Text seal_tier_text = null;
                    public static Slider seal_tier_slider_min = null;
                    public static Slider seal_tier_slider_max = null;

                    public static Toggle seal_value_toggle = null;
                    public static Text seal_value_text = null;
                    public static Slider seal_value_slider_min = null;
                    public static Slider seal_value_slider_max = null;

                    public static Toggle affix_count_toggle = null;
                    public static Text affix_count_text = null;
                    public static Slider affix_count_slider_min = null;
                    public static Slider affix_count_slider_max = null;

                    public static Toggle affix_tiers_toggle = null;
                    public static Text affix_tiers_text = null;
                    public static Slider affix_tiers_slider_min = null;
                    public static Slider affix_tiers_slider_max = null;

                    public static Toggle affix_values_toggle = null;
                    public static Text affix_values_text = null;
                    public static Slider affix_values_slider_min = null;
                    public static Slider affix_values_slider_max = null;

                    public static Toggle unique_mods_toggle = null;
                    public static Text unique_mods_text = null;
                    public static Slider unique_mods_slider_min = null;
                    public static Slider unique_mods_slider_max = null;

                    public static Toggle legendary_potencial_toggle = null;
                    public static Text legendary_potencial_text = null;
                    public static Slider legendary_potencial_slider_min = null;
                    public static Slider legendary_potencial_slider_max = null;

                    public static Toggle weaver_will_toggle = null;
                    public static Text weaver_will_text = null;
                    public static Slider weaver_will_slider_min = null;
                    public static Slider weaver_will_slider_max = null;
                }
                public class Pickup
                {
                    public static Toggle autopickup_gold_toggle = null;
                    public static Toggle autopickup_keys_toggle = null;
                    public static Toggle autopickup_potions_toggle = null;
                    public static Toggle autopickup_xptome_toggle = null;
                    public static Toggle autopickup_favortome_toggle = null;
                    public static Toggle autopickup_memoryamber_toggle = null;
                    public static Toggle autopickup_wovenechoes_toggle = null;
                    public static Toggle autopickup_materials_toggle = null;
                    public static Toggle autopickup_fromfilter_toggle = null;
                    public static Toggle autostore_materials_ondrop_toggle = null;
                    public static Toggle autostore_materials_oninventoryopen_toggle = null;
                    public static Toggle autostore_materials_Timer_toggle = null;
                    public static Text autostore_materials_Timer_text = null;
                    public static Slider autostore_materials_Timer_slider = null;                    
                    public static Toggle autosell_hide_toggle = null;
                    public static Toggle autoshatter_hide_toggle = null;
                    public static Toggle autoshatter_rune_toggle = null;
                    public static Text autoshatter_chance_text = null;
                    public static Slider autoshatter_chance_slider = null;
                    public static Text autoshatter_affix_chance_text = null;
                    public static Slider autoshatter_affix_chance_slider = null;
                    public static Text autoshatter_quantity_chance_text = null;
                    public static Slider autoshatter_quantity_chance_slider = null;

                    public static Toggle range_pickup_toggle = null;
                    public static Toggle hide_materials_notifications_toggle = null;
                }
                public class Requirements
                {
                    // BUG: For some reason the game always return true in action delegates
                    public static Toggle class_req_toggle = null;
                    public static readonly System.Action<bool> Class_Toggle_Action = new System.Action<bool>(Class_Enable);
                    private static void Class_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!class_req_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Items.Req.classe = class_req_toggle.isOn;
                        }
                        //Items_Req_Class.Enable();
                    }
                    
                    public static Toggle level_req_toggle = null;
                    public static readonly System.Action<bool> Level_Toggle_Action = new System.Action<bool>(Level_Enable);
                    private static void Level_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!level_req_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Items.Req.level = level_req_toggle.isOn;
                        }
                    }

                    public static Toggle set_req_toggle = null;
                    public static readonly System.Action<bool> Set_Toggle_Action = new System.Action<bool>(Set_Enable);
                    private static void Set_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!set_req_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Items.Req.set = set_req_toggle.isOn;
                            //Items_Req_Set.Enable();
                        }
                    }
                }
                public class ForceDrop
                {
                    public static Dropdown forcedrop_type_dropdown = null;
                    public static Dropdown forcedrop_rarity_dropdown = null;
                    public static Dropdown forcedrop_items_dropdown = null;
                    public static Text forcedrop_quantity_text = null;
                    public static Slider forcedrop_quantity_slider = null;
                    public static Button forcedrop_drop_button = null;
                    public static int item_type = -1;                    
                    public static int item_rarity = -1;
                    public static int item_subtype = -1;
                    public static int item_unique_id = -1;
                    public static bool btn_enable = false;
                    public static bool Type_Initialized = false;
                    public static bool Initializing_type = false;

                    public static void InitForcedrop()
                    {
                        if ((enable) && (LastEpoch_Hud.Scenes.IsGameScene()) &&
                            (!Type_Initialized) &&
                            (!Initializing_type) &&
                            (!Refs_Manager.item_list.IsNullOrDestroyed()) &&
                            (!forcedrop_type_dropdown.IsNullOrDestroyed()) &&
                            (!forcedrop_rarity_dropdown.IsNullOrDestroyed()) &&
                            (!forcedrop_items_dropdown.IsNullOrDestroyed()))
                        {
                            Initializing_type = true;
                            forcedrop_type_dropdown.ClearOptions();
                            Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
                            options.Add(new Dropdown.OptionData { text = "Select" });
                            foreach (ItemList.BaseEquipmentItem item in ItemList.get().EquippableItems)
                            {
                                options.Add(new Dropdown.OptionData { text = item.BaseTypeName });
                            }
                            foreach (ItemList.BaseNonEquipmentItem item in ItemList.get().nonEquippableItems)
                            {
                                options.Add(new Dropdown.OptionData { text = item.BaseTypeName });
                            }
                            forcedrop_type_dropdown.options = options;
                            forcedrop_type_dropdown.value = 0;

                            forcedrop_rarity_dropdown.ClearOptions();
                            forcedrop_rarity_dropdown.enabled = false;

                            forcedrop_items_dropdown.ClearOptions();
                            forcedrop_items_dropdown.enabled = false;

                            //forcedrop_drop_button.enabled = false;

                            Initializing_type = false;
                            Type_Initialized = true;
                        }
                    }
                    public static void SelectType()
                    {
                        if ((Type_Initialized) && (!forcedrop_type_dropdown.IsNullOrDestroyed()))
                        {
                            int index = forcedrop_type_dropdown.value;
                            if (index < forcedrop_type_dropdown.options.Count)
                            {
                                string type_str = forcedrop_type_dropdown.options[forcedrop_type_dropdown.value].text;
                                //Main.logger_instance.Msg("Select : Type = " + type_str);
                                item_type = -1;
                                bool found = false;
                                foreach (ItemList.BaseEquipmentItem item in ItemList.get().EquippableItems)
                                {
                                    if (item.BaseTypeName == type_str)
                                    {
                                        item_type = item.baseTypeID;
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    foreach (ItemList.BaseNonEquipmentItem item in ItemList.get().nonEquippableItems)
                                    {
                                        if (item.BaseTypeName == type_str)
                                        {
                                            item_type = item.baseTypeID;
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                                if (!found) { item_type = -1; }
                                UpdateRarity();
                                UpdateItems();
                                UpdateButton();
                            }
                        }
                    }
                    public static void UpdateRarity()
                    {
                        if ((enable) && (LastEpoch_Hud.Scenes.IsGameScene()) &&
                            (!Refs_Manager.item_list.IsNullOrDestroyed()) &&
                            (Type_Initialized) &&
                            (!forcedrop_type_dropdown.IsNullOrDestroyed()) &&
                            (!forcedrop_rarity_dropdown.IsNullOrDestroyed()) &&
                            (!forcedrop_items_dropdown.IsNullOrDestroyed()))
                        {
                            forcedrop_rarity_dropdown.ClearOptions();
                            Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
                            options.Add(new Dropdown.OptionData { text = "Select" });
                            if ((forcedrop_type_dropdown.value > 0) && (item_type > -1))
                            {
                                bool has_unique = false;
                                bool has_set = false;
                                if (UniqueList.instance.IsNullOrDestroyed()) { UniqueList.getUnique(0); }
                                if (!UniqueList.instance.IsNullOrDestroyed())
                                {
                                    foreach (UniqueList.Entry unique in UniqueList.instance.uniques)
                                    {
                                        if (unique.baseType == item_type)
                                        {
                                            if (unique.isSetItem) { has_set = true; }
                                            else { has_unique = true; }
                                        }
                                    }
                                }
                                options.Add(new Dropdown.OptionData { text = "Base Item" });
                                if (has_unique) { options.Add(new Dropdown.OptionData { text = "Unique" }); }
                                if (has_set) { options.Add(new Dropdown.OptionData { text = "Set" }); }
                                forcedrop_rarity_dropdown.enabled = true;
                            }
                            else { forcedrop_rarity_dropdown.enabled = false; }
                            forcedrop_rarity_dropdown.options = options;
                            forcedrop_rarity_dropdown.value = 0;
                            item_rarity = -1;
                        }
                    }
                    public static void SelectRarity()
                    {
                        if ((Type_Initialized) && (!forcedrop_rarity_dropdown.IsNullOrDestroyed()))
                        {
                            int index = forcedrop_rarity_dropdown.value;
                            if (index < forcedrop_rarity_dropdown.options.Count)
                            {
                                string rarity_str = forcedrop_rarity_dropdown.options[index].text;
                                item_rarity = -1;
                                if (rarity_str == "Base Item") { item_rarity = 0; }
                                else if (rarity_str == "Unique") { item_rarity = 7; }
                                else if (rarity_str == "Set") { item_rarity = 8; }
                                //Main.logger_instance.Msg("Select : Rarity = " + rarity_str);
                                UpdateItems();
                                UpdateButton();
                            }
                        }
                    }
                    public static void UpdateItems()
                    {
                        if ((enable) && (LastEpoch_Hud.Scenes.IsGameScene()) &&
                            (!Refs_Manager.item_list.IsNullOrDestroyed()) &&
                            (Type_Initialized) &&
                            //(!forcedrop_type_dropdown.IsNullOrDestroyed()) &&
                            //(!forcedrop_rarity_dropdown.IsNullOrDestroyed()) &&
                            (!forcedrop_items_dropdown.IsNullOrDestroyed()))
                        {
                            //Main.logger_instance.Msg("Update Items : Type = " + item_type + ", Rarity = " + item_rarity);
                            forcedrop_items_dropdown.ClearOptions();

                            Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
                            options.Add(new Dropdown.OptionData { text = "Select" });
                            if ((item_type > -1) && (item_rarity > -1))
                            {
                                if (item_rarity == 0)
                                {
                                    bool type_found = false;
                                    foreach (ItemList.BaseEquipmentItem item_t in ItemList.get().EquippableItems)
                                    {
                                        if (item_t.baseTypeID == item_type)
                                        {
                                            foreach (ItemList.EquipmentItem item in item_t.subItems)
                                            {
                                                string name = item.displayName;
                                                if (name == "" ) { name =  item.name; }
                                                options.Add(new Dropdown.OptionData { text = name });
                                            }
                                            type_found = true;
                                        }
                                    }
                                    if (!type_found)
                                    {
                                        foreach (ItemList.BaseNonEquipmentItem item_t in ItemList.get().nonEquippableItems)
                                        {
                                            if (item_t.baseTypeID == item_type)
                                            {
                                                foreach (ItemList.NonEquipmentItem item in item_t.subItems)
                                                {
                                                    string name = item.displayName;
                                                    if (name == "") { name = item.name; }
                                                    options.Add(new Dropdown.OptionData { text = name });
                                                }

                                                type_found = true;
                                            }
                                        }
                                    }
                                }
                                else if ((item_rarity == 7) || (item_rarity == 8))
                                {
                                    if (UniqueList.instance.IsNullOrDestroyed()) { UniqueList.getUnique(0); }
                                    if (!UniqueList.instance.IsNullOrDestroyed())
                                    {
                                        foreach (UniqueList.Entry unique in UniqueList.instance.uniques)
                                        {
                                            if ((unique.baseType == item_type) &&
                                                (((item_rarity == 7) && (!unique.isSetItem)) ||
                                                ((item_rarity == 8) && (unique.isSetItem))))
                                            {
                                                string name = unique.displayName;
                                                if ((name == "") || (name == "Pearls of the Swine") || (name == "Scales of Eterra")) { name = unique.name; } // if item's displayName is "Pearls of the Swine", use unique.name instead of unique.displayName
                                                options.Add(new Dropdown.OptionData { text = name });
                                            }
                                        }
                                    }
                                }
                                forcedrop_items_dropdown.enabled = true;
                            }
                            else { forcedrop_items_dropdown.enabled = false; }
                            forcedrop_items_dropdown.options = options;
                            forcedrop_items_dropdown.value = 0;
                        }
                    }
                    public static void SelectItem()
                    {
                        if ((Type_Initialized) && (!forcedrop_items_dropdown.IsNullOrDestroyed()))
                        {
                            int index = forcedrop_items_dropdown.value;
                            if (index < forcedrop_items_dropdown.options.Count)
                            {
                                string item_str = forcedrop_items_dropdown.options[forcedrop_items_dropdown.value].text;
                                //Main.logger_instance.Msg("Select : Item = " + item_str);

                                item_subtype = -1;
                                item_unique_id = 0;
                                bool item_found = false;
                                if (item_rarity == 0)
                                {
                                    foreach (ItemList.BaseEquipmentItem item_t in ItemList.get().EquippableItems)
                                    {
                                        if (item_t.baseTypeID == item_type)
                                        {
                                            foreach (ItemList.EquipmentItem item in item_t.subItems)
                                            {
                                                if ((item_str == item.displayName) || (item_str == item.name))
                                                {
                                                    item_subtype = item.subTypeID;
                                                    item_found = true;
                                                    break;
                                                }                                                
                                            }
                                        }
                                    }
                                    if (!item_found)
                                    {                                        
                                        foreach (ItemList.BaseNonEquipmentItem item_t in ItemList.get().nonEquippableItems)
                                        {
                                            if (item_t.baseTypeID == item_type)
                                            {
                                                foreach (ItemList.NonEquipmentItem item in item_t.subItems)
                                                {
                                                    if ((item_str == item.displayName) || (item_str == item.name))
                                                    {
                                                        item_subtype = item.subTypeID;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if ((item_rarity == 7) || (item_rarity == 8))
                                {
                                    if (UniqueList.instance.IsNullOrDestroyed()) { UniqueList.getUnique(0); }
                                    if (!UniqueList.instance.IsNullOrDestroyed())
                                    {
                                        foreach (UniqueList.Entry unique in UniqueList.instance.uniques)
                                        {
                                            if ((item_str == unique.displayName) || (item_str == unique.name))
                                            {
                                                item_subtype = unique.subTypes[0]; //need to be fix here
                                                item_unique_id = unique.uniqueID;
                                                break;
                                            }
                                        }
                                    }
                                }
                                UpdateButton();
                            }
                        }
                    }
                    public static void UpdateButton()
                    {
                        if ((item_type > -1) && (item_rarity > -1) && (item_subtype > -1)) { btn_enable = true; }
                        else { btn_enable = false; }
                    }

                    public static readonly System.Action Drop_OnClick_Action = new System.Action(Drop);
                    public static void Drop()
                    {
                        if ((btn_enable) && (!forcedrop_quantity_slider.IsNullOrDestroyed()))
                        {
                            if ((!Refs_Manager.ground_item_manager.IsNullOrDestroyed()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
                            {
                                for (int i = 0; i < forcedrop_quantity_slider.value; i++)
                                {
                                    ItemDataUnpacked item = new ItemDataUnpacked
                                    {
                                        LvlReq = 0,
                                        classReq = ItemList.ClassRequirement.Any,
                                        itemType = (byte)item_type,
                                        subType = (ushort)item_subtype,
                                        rarity = (byte)item_rarity,
                                        forgingPotential = (byte)0,
                                        sockets = (byte)0,
                                        uniqueID = (ushort)item_unique_id,
                                        legendaryPotential = (byte)0,
                                        weaversWill = (byte)0,
                                        hasSealedRegularAffix = false
                                    };

                                    //Random
                                    if (item.itemType < 100)
                                    {
                                        for (int k = 0; k < item.implicitRolls.Count; k++) { item.implicitRolls[k] = (byte)Random.RandomRange(0f, 255f); }
                                        if (!item.isUniqueSetOrLegendary()) { item.forgingPotential = (byte)Random.RandomRange(0f, 255f); }
                                        UniqueList.LegendaryType legendary_type = UniqueList.LegendaryType.LegendaryPotential;
                                        if (item.isUniqueSetOrLegendary())
                                        {                                            
                                            for (int k = 0; k < item.uniqueRolls.Count; k++) { item.uniqueRolls[k] = (byte)Random.RandomRange(0f, 255f); }                                            
                                            legendary_type = UniqueList.getUnique((ushort)item_unique_id).legendaryType;
                                            if (legendary_type == UniqueList.LegendaryType.WeaversWill) { item.weaversWill = (byte)Random.RandomRange(0f, 28f); }
                                            else if (item.isUnique()) { item.legendaryPotential = (byte)Random.RandomRange(0f, 4f); }
                                        }
                                    }
                                    item.RefreshIDAndValues();
                                    Refs_Manager.ground_item_manager.dropItemForPlayer(Refs_Manager.player_actor, item.TryCast<ItemData>(), Refs_Manager.player_actor.position(), false);
                                }
                            }
                        }
                    }
                }
                public class CraftingSlot
                {
                    public static Toggle enable_mod = null;

                    public static Toggle forgin_potencial_toggle = null;
                    public static Text forgin_potencial_text = null;
                    public static Slider forgin_potencial_slider = null;

                    public static Toggle implicit_0_toggle = null;
                    public static Text implicit_0_text = null;
                    public static Slider implicit_0_slider = null;

                    public static Toggle implicit_1_toggle = null;
                    public static Text implicit_1_text = null;
                    public static Slider implicit_1_slider = null;

                    public static Toggle implicit_2_toggle = null;
                    public static Text implicit_2_text = null;
                    public static Slider implicit_2_slider = null;

                    public static Toggle seal_tier_toggle = null;
                    public static Text seal_tier_text = null;
                    public static Slider seal_tier_slider = null;

                    public static Toggle seal_value_toggle = null;
                    public static Text seal_value_text = null;
                    public static Slider seal_value_slider = null;

                    public static Toggle affix_0_tier_toggle = null;
                    public static Text affix_0_tier_text = null;
                    public static Slider affix_0_tier_slider = null;

                    public static Toggle affix_0_value_toggle = null;
                    public static Text affix_0_value_text = null;
                    public static Slider affix_0_value_slider = null;

                    public static Toggle affix_1_tier_toggle = null;
                    public static Text affix_1_tier_text = null;
                    public static Slider affix_1_tier_slider = null;

                    public static Toggle affix_1_value_toggle = null;
                    public static Text affix_1_value_text = null;
                    public static Slider affix_1_value_slider = null;

                    public static Toggle affix_2_tier_toggle = null;
                    public static Text affix_2_tier_text = null;
                    public static Slider affix_2_tier_slider = null;

                    public static Toggle affix_2_value_toggle = null;
                    public static Text affix_2_value_text = null;
                    public static Slider affix_2_value_slider = null;

                    public static Toggle affix_3_tier_toggle = null;
                    public static Text affix_3_tier_text = null;
                    public static Slider affix_3_tier_slider = null;

                    public static Toggle affix_3_value_toggle = null;
                    public static Text affix_3_value_text = null;
                    public static Slider affix_3_value_slider = null;

                    public static Toggle uniquemod_0_value_toggle = null;
                    public static Text uniquemod_0_value_text = null;
                    public static Slider uniquemod_0_value_slider = null;

                    public static Toggle uniquemod_1_value_toggle = null;
                    public static Text uniquemod_1_value_text = null;
                    public static Slider uniquemod_1_value_slider = null;

                    public static Toggle uniquemod_2_value_toggle = null;
                    public static Text uniquemod_2_value_text = null;
                    public static Slider uniquemod_2_value_slider = null;

                    public static Toggle uniquemod_3_value_toggle = null;
                    public static Text uniquemod_3_value_text = null;
                    public static Slider uniquemod_3_value_slider = null;

                    public static Toggle uniquemod_4_value_toggle = null;
                    public static Text uniquemod_4_value_text = null;
                    public static Slider uniquemod_4_value_slider = null;

                    public static Toggle uniquemod_5_value_toggle = null;
                    public static Text uniquemod_5_value_text = null;
                    public static Slider uniquemod_5_value_slider = null;

                    public static Toggle uniquemod_6_value_toggle = null;
                    public static Text uniquemod_6_value_text = null;
                    public static Slider uniquemod_6_value_slider = null;

                    public static Toggle uniquemod_7_value_toggle = null;
                    public static Text uniquemod_7_value_text = null;
                    public static Slider uniquemod_7_value_slider = null;

                    public static Toggle legendary_potencial_toggle = null;
                    public static Text legendary_potencial_text = null;
                    public static Slider legendary_potencial_slider = null;

                    public static Toggle weaver_will_toggle = null;
                    public static Text weaver_will_text = null;
                    public static Slider weaver_will_slider = null;
                }
            }
            public class Scenes
            {
                public static GameObject content_obj = null;
                public static bool controls_initialized = false;
                public static bool enable = false;

                public static void Get_Refs()
                {
                    if (!Content.content_obj.IsNullOrDestroyed())
                    {
                        content_obj = Functions.GetChild(Content.content_obj, "Scenes_Content");
                        if (!content_obj.IsNullOrDestroyed())
                        {
                            Camera.enable_mod = Functions.Get_ToggleInLabel(content_obj, "Camera", "Toggle_Scenes_Camera_Enable");

                            GameObject scene_camera_content = Functions.GetViewportContent(content_obj, "Camera", "Scenes_Camera_Content");
                            if (!scene_camera_content.IsNullOrDestroyed())
                            {
                                Camera.zoom_minimum_toggle = Functions.Get_ToggleInPanel(scene_camera_content, "ZoomMinimum", "Toggle_Scenes_Camera_ZoomMinimum");
                                Camera.zoom_minimum_text = Functions.Get_TextInToggle(scene_camera_content, "ZoomMinimum", "Toggle_Scenes_Camera_ZoomMinimum", "Value");
                                Camera.zoom_minimum_slider = Functions.Get_SliderInPanel(scene_camera_content, "ZoomMinimum", "Slider_Scenes_Camera_ZoomMinimum");

                                Camera.zoom_per_scroll_toggle = Functions.Get_ToggleInPanel(scene_camera_content, "ZoomPerScroll", "Toggle_Scenes_Camera_ZoomPerScroll");
                                Camera.zoom_per_scroll_text = Functions.Get_TextInToggle(scene_camera_content, "ZoomPerScroll", "Toggle_Scenes_Camera_ZoomPerScroll", "Value");
                                Camera.zoom_per_scroll_slider = Functions.Get_SliderInPanel(scene_camera_content, "ZoomPerScroll", "Slider_Scenes_Camera_ZoomPerScroll");

                                Camera.zoom_speed_toggle = Functions.Get_ToggleInPanel(scene_camera_content, "ZoomSpeed", "Toggle_Scenes_Camera_ZoomSpeed");
                                Camera.zoom_speed_text = Functions.Get_TextInToggle(scene_camera_content, "ZoomSpeed", "Toggle_Scenes_Camera_ZoomSpeed", "Value");
                                Camera.zoom_speed_slider = Functions.Get_SliderInPanel(scene_camera_content, "ZoomSpeed", "Slider_Scenes_Camera_ZoomSpeed");

                                Camera.default_rotation_toggle = Functions.Get_ToggleInPanel(scene_camera_content, "DefaultRotation", "Toggle_Scenes_Camera_DefaultRotation");
                                Camera.default_rotation_text = Functions.Get_TextInToggle(scene_camera_content, "DefaultRotation", "Toggle_Scenes_Camera_DefaultRotation", "Value");
                                Camera.default_rotation_slider = Functions.Get_SliderInPanel(scene_camera_content, "DefaultRotation", "Slider_Scenes_Camera_DefaultRotation");

                                Camera.offset_minimum_toggle = Functions.Get_ToggleInPanel(scene_camera_content, "OffsetMinimum", "Toggle_Scenes_Camera_OffsetMinimum");
                                Camera.offset_minimum_text = Functions.Get_TextInToggle(scene_camera_content, "OffsetMinimum", "Toggle_Scenes_Camera_OffsetMinimum", "Value");
                                Camera.offset_minimum_slider = Functions.Get_SliderInPanel(scene_camera_content, "OffsetMinimum", "Slider_Scenes_Camera_OffsetMinimum");

                                Camera.offset_maximum_toggle = Functions.Get_ToggleInPanel(scene_camera_content, "OffsetMaximum", "Toggle_Scenes_Camera_OffsetMaximum");
                                Camera.offset_maximum_text = Functions.Get_TextInToggle(scene_camera_content, "OffsetMaximum", "Toggle_Scenes_Camera_OffsetMaximum", "Value");
                                Camera.offset_maximum_slider = Functions.Get_SliderInPanel(scene_camera_content, "OffsetMaximum", "Slider_Scenes_Camera_OffsetMaximum");

                                Camera.angle_minimum_toggle = Functions.Get_ToggleInPanel(scene_camera_content, "AngleMinimum", "Toggle_Scenes_Camera_AngleMinimum");
                                Camera.angle_minimum_text = Functions.Get_TextInToggle(scene_camera_content, "AngleMinimum", "Toggle_Scenes_Camera_AngleMinimum", "Value");
                                Camera.angle_minimum_slider = Functions.Get_SliderInPanel(scene_camera_content, "AngleMinimum", "Slider_Scenes_Camera_AngleMinimum");

                                Camera.angle_maximum_toggle = Functions.Get_ToggleInPanel(scene_camera_content, "AngleMaximum", "Toggle_Scenes_Camera_AngleMaximum");
                                Camera.angle_maximum_text = Functions.Get_TextInToggle(scene_camera_content, "AngleMaximum", "Toggle_Scenes_Camera_AngleMaximum", "Value");
                                Camera.angle_maximum_slider = Functions.Get_SliderInPanel(scene_camera_content, "AngleMaximum", "Slider_Scenes_Camera_AngleMaximum");

                                Camera.zoom_load_on_start_toggle = Functions.Get_ToggleInPanel(scene_camera_content, "LoadOnStart", "Toggle_Scenes_Camera_LoadOnStart");

                                Camera.reset_button = Functions.GetChild(scene_camera_content, "Btn_Scenes_Camera_Reset").GetComponent<Button>();
                                Camera.set_button = Functions.GetChild(scene_camera_content, "Btn_Scenes_Camera_Set").GetComponent<Button>();
                            }
                            GameObject scene_dungeons_content = Functions.GetViewportContent(content_obj, "Center", "Scenes_Dungeons_Content");
                            if (!scene_dungeons_content.IsNullOrDestroyed())
                            {
                                Dungeons.enter_without_key_toggle = Functions.Get_ToggleInPanel(scene_dungeons_content, "EnterWithoutKey", "Toggle_Scenes_Dungeons_EnterWithoutKey");

                                Teleport.scene_dropdown = Functions.GetChild(scene_dungeons_content, "Teleport_Dropdown").GetComponent<Dropdown>();
                                Teleport.scene_button = Functions.GetChild(scene_dungeons_content, "Teleport_Btn").GetComponent<Button>();
                                Teleport.Init();
                            }
                            GameObject scene_minimap_content = Functions.GetViewportContent(content_obj, "Center", "Scenes_Minimap_Content");
                            if (!scene_minimap_content.IsNullOrDestroyed())
                            {
                                Minimap.max_zoom_out_toggle = Functions.Get_ToggleInPanel(scene_minimap_content, "MaxZoomOut", "Toggle_Scenes_Minimap_MaxZoomOut");
                                Minimap.remove_fog_of_war_toggle = Functions.Get_ToggleInPanel(scene_minimap_content, "RemoveFogOfWar", "Toggle_Scenes_Minimap_RemoveFogOfWar");
                            }
                            GameObject scene_monoliths_content = Functions.GetViewportContent(content_obj, "Monoliths", "Scenes_Monoliths_Content");
                            if (!scene_monoliths_content.IsNullOrDestroyed())
                            {
                                Monoliths.max_stability_toggle = Functions.Get_ToggleInPanel(scene_monoliths_content, "MaxStability", "Toggle_Scenes_Monoliths_MaxStability");
                                Monoliths.max_stability_text = Functions.Get_TextInToggle(scene_monoliths_content, "MaxStability", "Toggle_Scenes_Monoliths_MaxStability", "Value");
                                Monoliths.max_stability_slider = Functions.Get_SliderInPanel(scene_monoliths_content, "MaxStability", "Slider_Scenes_Monoliths_MaxStability");

                                Monoliths.mob_density_toggle = Functions.Get_ToggleInPanel(scene_monoliths_content, "MobsDensity", "Toggle_Scenes_Monoliths_MobsDensity");
                                Monoliths.mob_density_text = Functions.Get_TextInToggle(scene_monoliths_content, "MobsDensity", "Toggle_Scenes_Monoliths_MobsDensity", "Value");
                                Monoliths.mob_density_slider = Functions.Get_SliderInPanel(scene_monoliths_content, "MobsDensity", "Slider_Scenes_Monoliths_MobsDensity");

                                Monoliths.mob_defeat_toggle = Functions.Get_ToggleInPanel(scene_monoliths_content, "MobsDefeatOnStart", "Toggle_Scenes_Monoliths_MobsDefeatOnStart");
                                Monoliths.mob_defeat_text = Functions.Get_TextInToggle(scene_monoliths_content, "MobsDefeatOnStart", "Toggle_Scenes_Monoliths_MobsDefeatOnStart", "Value");
                                Monoliths.mob_defeat_slider = Functions.Get_SliderInPanel(scene_monoliths_content, "MobsDefeatOnStart", "Slider_Scenes_Monoliths_MobsDefeatOnStart");

                                Monoliths.blessing_slot_toggle = Functions.Get_ToggleInPanel(scene_monoliths_content, "BlessingSlots", "Toggle_Scenes_Monoliths_BlessingSlots");
                                Monoliths.blessing_slot_text = Functions.Get_TextInToggle(scene_monoliths_content, "BlessingSlots", "Toggle_Scenes_Monoliths_BlessingSlots", "Value");
                                Monoliths.blessing_slot_slider = Functions.Get_SliderInPanel(scene_monoliths_content, "BlessingSlots", "Slider_Scenes_Monoliths_BlessingSlots");

                                Monoliths.max_stability_on_start_toggle = Functions.Get_ToggleInPanel(scene_monoliths_content, "MaxStabilityOnStart", "Toggle_Scenes_Monoliths_MaxStabilityOnStart");
                                Monoliths.max_stability_on_stability_changed_toggle = Functions.Get_ToggleInPanel(scene_monoliths_content, "MaxStabilityOnStabilityChanged", "Toggle_Scenes_Monoliths_MaxStabilityOnStabilityChanged");
                                Monoliths.objective_reveal_toggle = Functions.Get_ToggleInPanel(scene_monoliths_content, "ObjectiveReveal", "Toggle_Scenes_Monoliths_ObjectiveReveal");
                                Monoliths.complete_objective_toggle = Functions.Get_ToggleInPanel(scene_monoliths_content, "CompleteObjective", "Toggle_Scenes_Monoliths_CompleteObjective");
                                Monoliths.no_lost_when_die_toggle = Functions.Get_ToggleInPanel(scene_monoliths_content, "NoLostWhenDie", "Toggle_Scenes_Monoliths_NoLostWhenDie");
                            }
                        }
                    }
                }
                public static void Set_Events()
                {
                    Events.Set_Button_Event(Camera.reset_button, Camera.Reset_OnClick_Action);
                    Events.Set_Button_Event(Camera.set_button, Camera.Set_OnClick_Action);
                    Events.Set_Button_Event(Teleport.scene_button, Teleport.Scene_OnClick_Action);
                }
                public static void Set_Active(bool show)
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static void Toggle_Active()
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        bool show = !content_obj.active;
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static bool Init_UserData()
                {
                    bool result = false;
                    if (!Save_Manager.instance.IsNullOrDestroyed())
                    {
                        if ((Save_Manager.instance.initialized) && (!Save_Manager.instance.data.IsNullOrDestroyed()))
                        {
                            Camera.enable_mod.isOn = Save_Manager.instance.data.Scenes.Camera.Enable_Mod;

                            Camera.zoom_minimum_toggle.isOn = Save_Manager.instance.data.Scenes.Camera.Enable_ZoomMinimum;
                            Camera.zoom_minimum_slider.value = Save_Manager.instance.data.Scenes.Camera.ZoomMinimum;

                            Camera.zoom_per_scroll_toggle.isOn = Save_Manager.instance.data.Scenes.Camera.Enable_ZoomPerScroll;
                            Camera.zoom_per_scroll_slider.value = Save_Manager.instance.data.Scenes.Camera.ZoomPerScroll;

                            Camera.zoom_speed_toggle.isOn = Save_Manager.instance.data.Scenes.Camera.Enable_ZoomSpeed;
                            Camera.zoom_speed_slider.value = Save_Manager.instance.data.Scenes.Camera.ZoomSpeed;

                            Camera.default_rotation_toggle.isOn = Save_Manager.instance.data.Scenes.Camera.Enable_DefaultRotation;
                            Camera.default_rotation_slider.value = Save_Manager.instance.data.Scenes.Camera.DefaultRotation;

                            Camera.offset_minimum_toggle.isOn = Save_Manager.instance.data.Scenes.Camera.Enable_OffsetMinimum;
                            Camera.offset_minimum_slider.value = Save_Manager.instance.data.Scenes.Camera.OffsetMinimum;

                            Camera.offset_maximum_toggle.isOn = Save_Manager.instance.data.Scenes.Camera.Enable_OffsetMaximum;
                            Camera.offset_maximum_slider.value = Save_Manager.instance.data.Scenes.Camera.OffsetMaximum;

                            Camera.angle_minimum_toggle.isOn = Save_Manager.instance.data.Scenes.Camera.Enable_AngleMinimum;
                            Camera.angle_minimum_slider.value = Save_Manager.instance.data.Scenes.Camera.AngleMinimum;

                            Camera.angle_maximum_toggle.isOn = Save_Manager.instance.data.Scenes.Camera.Enable_AngleMaximum;
                            Camera.angle_maximum_slider.value = Save_Manager.instance.data.Scenes.Camera.AngleMaximum;

                            Camera.zoom_load_on_start_toggle.isOn = Save_Manager.instance.data.Scenes.Camera.Enable_LoadOnStart;

                            Dungeons.enter_without_key_toggle.isOn = Save_Manager.instance.data.Scenes.Dungeons.Enable_EnterWithoutKey;

                            Minimap.max_zoom_out_toggle.isOn = Save_Manager.instance.data.Scenes.Minimap.Enable_MaxZoomOut;
                            Minimap.remove_fog_of_war_toggle.isOn = Save_Manager.instance.data.Scenes.Minimap.Enable_RemoveFogOfWar;

                            Monoliths.max_stability_toggle.isOn = Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStability;
                            Monoliths.max_stability_slider.value = Save_Manager.instance.data.Scenes.Monoliths.MaxStability;

                            Monoliths.mob_density_toggle.isOn = Save_Manager.instance.data.Scenes.Monoliths.Enable_MobsDensity;
                            Monoliths.mob_density_slider.value = Save_Manager.instance.data.Scenes.Monoliths.MobsDensity;

                            Monoliths.mob_defeat_toggle.isOn = Save_Manager.instance.data.Scenes.Monoliths.Enable_MobsDefeatOnStart;
                            Monoliths.mob_defeat_slider.value = Save_Manager.instance.data.Scenes.Monoliths.MobsDefeatOnStart;

                            Monoliths.blessing_slot_toggle.isOn = Save_Manager.instance.data.Scenes.Monoliths.Enable_BlessingSlots;
                            Monoliths.blessing_slot_slider.value = Save_Manager.instance.data.Scenes.Monoliths.BlessingSlots;

                            Monoliths.max_stability_on_start_toggle.isOn = Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStabilityOnStart;
                            Monoliths.max_stability_on_stability_changed_toggle.isOn = Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStabilityOnStabilityChanged;
                            Monoliths.objective_reveal_toggle.isOn = Save_Manager.instance.data.Scenes.Monoliths.Enable_ObjectiveReveal;
                            Monoliths.complete_objective_toggle.isOn = Save_Manager.instance.data.Scenes.Monoliths.Enable_CompleteObjective;
                            Monoliths.no_lost_when_die_toggle.isOn = Save_Manager.instance.data.Scenes.Monoliths.Enable_NoLostWhenDie;

                            controls_initialized = true;
                            result = true;
                        }
                    }
                    
                    return result;
                }
                public static void UpdateVisuals()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (controls_initialized))
                    {
                        if ((Save_Manager.instance.initialized) && (!Save_Manager.instance.data.IsNullOrDestroyed()))
                        {
                            Camera.zoom_minimum_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Camera.ZoomMinimum);
                            Camera.zoom_per_scroll_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Camera.ZoomPerScroll);
                            Camera.zoom_speed_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Camera.ZoomSpeed);
                            Camera.default_rotation_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Camera.DefaultRotation);
                            Camera.offset_minimum_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Camera.OffsetMinimum);
                            Camera.offset_maximum_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Camera.OffsetMaximum);
                            Camera.angle_minimum_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Camera.AngleMinimum);
                            Camera.angle_maximum_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Camera.AngleMaximum);
                            Monoliths.max_stability_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Monoliths.MaxStability);
                            Monoliths.mob_density_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Monoliths.MobsDensity);
                            Monoliths.mob_defeat_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Monoliths.MobsDefeatOnStart);
                            Monoliths.blessing_slot_text.text = "" + (int)(Save_Manager.instance.data.Scenes.Monoliths.BlessingSlots);
                        }
                    }
                }

                public class Camera
                {
                    public static Toggle enable_mod = null;

                    public static Toggle zoom_minimum_toggle = null;
                    public static Text zoom_minimum_text = null;
                    public static Slider zoom_minimum_slider = null;

                    public static Toggle zoom_per_scroll_toggle = null;
                    public static Text zoom_per_scroll_text = null;
                    public static Slider zoom_per_scroll_slider = null;

                    public static Toggle zoom_speed_toggle = null;
                    public static Text zoom_speed_text = null;
                    public static Slider zoom_speed_slider = null;

                    public static Toggle default_rotation_toggle = null;
                    public static Text default_rotation_text = null;
                    public static Slider default_rotation_slider = null;

                    public static Toggle offset_minimum_toggle = null;
                    public static Text offset_minimum_text = null;
                    public static Slider offset_minimum_slider = null;

                    public static Toggle offset_maximum_toggle = null;
                    public static Text offset_maximum_text = null;
                    public static Slider offset_maximum_slider = null;

                    public static Toggle angle_minimum_toggle = null;
                    public static Text angle_minimum_text = null;
                    public static Slider angle_minimum_slider = null;

                    public static Toggle angle_maximum_toggle = null;
                    public static Text angle_maximum_text = null;
                    public static Slider angle_maximum_slider = null;

                    public static Toggle zoom_load_on_start_toggle = null;

                    public static Button reset_button = null;
                    public static Button set_button = null;

                    public static readonly System.Action Reset_OnClick_Action = new System.Action(Reset);
                    public static void Reset()
                    {
                        Mods.Camera.Camera_Override.ResetToDefault();
                    }
                    public static readonly System.Action Set_OnClick_Action = new System.Action(Set);
                    public static void Set()
                    {
                        Mods.Camera.Camera_Override.Set();
                    }
                }
                public class Minimap
                {
                    public static Toggle max_zoom_out_toggle = null;
                    public static Toggle remove_fog_of_war_toggle = null;
                }
                public class Dungeons
                {
                    public static Toggle enter_without_key_toggle = null;
                }
                public class Teleport
                {
                    public static Dropdown scene_dropdown = null;
                    public static Button scene_button = null;
                    public static readonly System.Action Scene_OnClick_Action = new System.Action(Scene_Teleport);

                    public static void Init()
                    {
                        scene_dropdown.options.Clear();
                        scene_dropdown.options.Add(new Dropdown.OptionData("Select"));

                        Mods.Teleport.Teleport_ToScene.scene_names.Clear();
                        Mods.Teleport.Teleport_ToScene.scene_names.Add("");
                        foreach (SceneDetails scene_detail in SceneList.instance.sceneDetailsCollection)
                        {
                            if ((scene_detail.Name != "PersistentUI") &&
                                (scene_detail.Name != "CharacterSelectScene") &&
                                (scene_detail.Name != "Login") &&
                                (scene_detail.Name != "PersistentUI") &&
                                (scene_detail.Name != "MonolithHub") &&
                                (scene_detail.Name != "A_Reward") &&
                                (scene_detail.Name != "Mastery") &&
                                (scene_detail.Name != "Neutral") &&
                                (scene_detail.Name != "PCG_Dev") &&
                                (!scene_detail.Name.Contains("PCG")) &&
                                (!scene_detail.Name.Contains("Arena")) &&
                                (scene_detail.LocalizedName != scene_detail.Name) && //New zones
                                (!Mods.Teleport.Teleport_ToScene.scene_names.Contains(scene_detail.Name)))
                            {
                                string option_name = "";
                                if (scene_detail.Name != scene_detail.LocalizedName)
                                {
                                    option_name = scene_detail.Name + " : " + scene_detail.LocalizedName;
                                }
                                else { option_name = scene_detail.Name; }
                                if (option_name != "")
                                {
                                    //Main.logger_instance.Warning("add scene = " + scene_detail.Name);
                                    Mods.Teleport.Teleport_ToScene.scene_names.Add(scene_detail.Name);
                                    scene_dropdown.options.Add(new Dropdown.OptionData(option_name));
                                }
                            }
                        }
                    }
                    public static void Scene_Teleport()
                    {
                        Mods.Teleport.Teleport_ToScene.StartTp(scene_dropdown.value);
                    }
                }
                public class Monoliths
                {
                    public static Toggle max_stability_toggle = null;
                    public static Text max_stability_text = null;
                    public static Slider max_stability_slider = null;

                    public static Toggle mob_density_toggle = null;
                    public static Text mob_density_text = null;
                    public static Slider mob_density_slider = null;

                    public static Toggle mob_defeat_toggle = null;
                    public static Text mob_defeat_text = null;
                    public static Slider mob_defeat_slider = null;

                    public static Toggle blessing_slot_toggle = null;
                    public static Text blessing_slot_text = null;
                    public static Slider blessing_slot_slider = null;

                    public static Toggle max_stability_on_start_toggle = null;
                    public static Toggle max_stability_on_stability_changed_toggle = null;
                    public static Toggle objective_reveal_toggle = null;
                    public static Toggle complete_objective_toggle = null;
                    public static Toggle no_lost_when_die_toggle = null;
                }
            }            
            public class Skills
            {
                public static GameObject content_obj = null;
                public static bool controls_initialized = false;
                public static bool enable = false;

                public static void Get_Refs()
                {
                    if (!Content.content_obj.IsNullOrDestroyed())
                    {
                        content_obj = Functions.GetChild(Content.content_obj, "Skill_Tree_Content");
                        if (!content_obj.IsNullOrDestroyed())
                        {
                            GameObject skills_content = Functions.GetViewportContent(content_obj, "Left", "Skills_Content");
                            if (!skills_content.IsNullOrDestroyed())
                            {
                                SkillTree.enable_remove_mana_cost_toggle = Functions.Get_ToggleInPanel(skills_content, "RemoveManaCost", "Toggle_RemoveManaCost");
                                SkillTree.enable_remove_channel_cost_toggle = Functions.Get_ToggleInPanel(skills_content, "RemoveChannelCost", "Toggle_RemoveChannelCost");
                                SkillTree.enable_mana_regen_when_channeling_toggle = Functions.Get_ToggleInPanel(skills_content, "ManaRegenWhenChanneling", "Toggle_ManaRegenWhenChanneling");
                                SkillTree.enable_dont_stop_oom_toggle = Functions.Get_ToggleInPanel(skills_content, "DontStopWhenOOM", "Toggle_DontStopWhenOOM");
                                SkillTree.enable_no_cooldown_toggle = Functions.Get_ToggleInPanel(skills_content, "NoCooldown", "Toggle_NoCooldown");
                                SkillTree.enable_unlock_all_skills_toggle = Functions.Get_ToggleInPanel(skills_content, "UnlockAllSkills", "Toggle_UnlockAllSkills");
                                SkillTree.enable_remove_node_req_toggle = Functions.Get_ToggleInPanel(skills_content, "RemoveNodeRequirements", "Toggle_RemoveNodeRequirements");

                                SkillTree.enable_specialization_slots_toggle = Functions.Get_ToggleInPanel(skills_content, "SpecializationSlots", "Toggle_SpecializationSlots");
                                SkillTree.specialization_slots_text = Functions.Get_TextInToggle(skills_content, "SpecializationSlots", "Toggle_SpecializationSlots", "Value");
                                SkillTree.specialization_slots_slider = Functions.Get_SliderInPanel(skills_content, "SpecializationSlots", "Slider_SpecializationSlots");

                                SkillTree.enable_skill_level_toggle = Functions.Get_ToggleInPanel(skills_content, "SkillLevel", "Toggle_SkillLevel");
                                SkillTree.skill_level_text = Functions.Get_TextInToggle(skills_content, "SkillLevel", "Toggle_SkillLevel", "Value");
                                SkillTree.skill_level_slider = Functions.Get_SliderInPanel(skills_content, "SkillLevel", "Slider_SkillLevel");

                                SkillTree.enable_passive_points_toggle = Functions.Get_ToggleInPanel(skills_content, "PassivePoints", "Toggle_PassivePoints");
                                SkillTree.passive_points_text = Functions.Get_TextInToggle(skills_content, "PassivePoints", "Toggle_PassivePoints", "Value");
                                SkillTree.passive_points_slider = Functions.Get_SliderInPanel(skills_content, "PassivePoints", "Slider_PassivePoints");

                                SkillTree.enable_movement_no_target_toggle = Functions.Get_ToggleInPanel(skills_content, "NoTarget", "Toggle_NoTarget");
                                SkillTree.enable_movement_immune_toggle = Functions.Get_ToggleInPanel(skills_content, "ImmuneDuringMovement", "Toggle_ImmuneDuringMovement");
                                SkillTree.enable_movement_simple_path_toggle = Functions.Get_ToggleInPanel(skills_content, "DisableSimplePath", "Toggle_DisableSimplePath");

                                SkillTree.enable_summon_godmode_toggle = Functions.Get_ToggleInPanel(skills_content, "SummonGodMode", "Toggle");
                                SkillTree.enable_summon_forever_toggle = Functions.Get_ToggleInPanel(skills_content, "SummonForever", "Toggle");
                                SkillTree.enable_summon_dontcollide_toggle = Functions.Get_ToggleInPanel(skills_content, "SummonDontCollide", "Toggle");
                            }
                            else { Main.logger_instance.Error("Skills content is null"); }

                            GameObject companions_content = Functions.GetViewportContent(content_obj, "Center", "Companions_Content");
                            if (!companions_content.IsNullOrDestroyed())
                            {
                                Companions.enable_maximum_companions_toggle = Functions.Get_ToggleInPanel(companions_content, "MaximumCompanions", "Toggle_MaximumCompanions");
                                Companions.maximum_companions_text = Functions.Get_TextInToggle(companions_content, "MaximumCompanions", "Toggle_MaximumCompanions", "Value");
                                Companions.maximum_companions_slider = Functions.Get_SliderInPanel(companions_content, "MaximumCompanions", "Slider_MaximumCompanions");

                                //wolf
                                Companions.enable_wolf_summon_maximum_toggle = Functions.Get_ToggleInPanel(companions_content, "Wolf_SummonToMax", "Toggle_Wolf_SummonToMax");

                                Companions.enable_wolf_summon_limit_toggle = Functions.Get_ToggleInPanel(companions_content, "Wolf_SummonLimit", "Toggle_Wolf_SummonLimit");
                                Companions.wolf_summon_limit_text = Functions.Get_TextInToggle(companions_content, "Wolf_SummonLimit", "Toggle_Wolf_SummonLimit", "Value");
                                Companions.wolf_summon_limit_slider = Functions.Get_SliderInPanel(companions_content, "Wolf_SummonLimit", "Slider_Wolf_SummonLimit");

                                Companions.enable_wolf_stun_immunity_toggle = Functions.Get_ToggleInPanel(companions_content, "Wolf_StunImmunity", "Toggle_Wolf_StunImmunity");

                                //Scorpion
                                Companions.enable_scorpion_summon_limit_toggle = Functions.Get_ToggleInPanel(companions_content, "Scorpions_SummonLimit", "Toggle_Scorpions_SummonLimit");
                                Companions.scorpion_summon_limit_text = Functions.Get_TextInToggle(companions_content, "Scorpions_SummonLimit", "Toggle_Scorpions_SummonLimit", "Value");
                                Companions.scorpion_summon_limit_slider = Functions.Get_SliderInPanel(companions_content, "Scorpions_SummonLimit", "Slider_Scorpions_SummonLimit");
                            }
                            else { Main.logger_instance.Error("Companions content is null"); }

                            GameObject minions_content = Functions.GetViewportContent(content_obj, "Right", "Minions_Content");
                            if (!minions_content.IsNullOrDestroyed())
                            {
                                //Skeletons
                                Minions.enable_skeleton_passive_summon_toggle = Functions.Get_ToggleInPanel(minions_content, "Skeleteon_SummonQuantityFromPassive", "Toggle_Skeleteon_SummonQuantityFromPassive");
                                Minions.skeleton_passive_summon_text = Functions.Get_TextInToggle(minions_content, "Skeleteon_SummonQuantityFromPassive", "Toggle_Skeleteon_SummonQuantityFromPassive", "Value");
                                Minions.skeleton_passive_summon_slider = Functions.Get_SliderInPanel(minions_content, "Skeleteon_SummonQuantityFromPassive", "Slider_Skeleteon_SummonQuantityFromPassive");

                                Minions.enable_skeleton_skilltree_summon_toggle = Functions.Get_ToggleInPanel(minions_content, "Skeleteon_SummonQuantityFromSkillTree", "Toggle_Skeleteon_SummonQuantityFromSkillTree");
                                Minions.skeleton_skilltree_summon_text = Functions.Get_TextInToggle(minions_content, "Skeleteon_SummonQuantityFromSkillTree", "Toggle_Skeleteon_SummonQuantityFromSkillTree", "Value");
                                Minions.skeleton_skilltree_summon_slider = Functions.Get_SliderInPanel(minions_content, "Skeleteon_SummonQuantityFromSkillTree", "Slider_Skeleteon_SummonQuantityFromSkillTree");

                                Minions.enable_skeleton_quantity_per_cast_toggle = Functions.Get_ToggleInPanel(minions_content, "Skeleteon_SummonQuantityPerCast", "Toggle_Skeleteon_SummonQuantityPerCast");
                                Minions.skeleton_quantity_per_cast_text = Functions.Get_TextInToggle(minions_content, "Skeleteon_SummonQuantityPerCast", "Toggle_Skeleteon_SummonQuantityPerCast", "Value");
                                Minions.skeleton_quantity_per_cast_slider = Functions.Get_SliderInPanel(minions_content, "Skeleteon_SummonQuantityPerCast", "Slider_Skeleteon_SummonQuantityPerCast");

                                Minions.enable_skeleton_resummon_on_death_toggle = Functions.Get_ToggleInPanel(minions_content, "Skeleteon_ChanceToResummonOnDeath", "Toggle_Skeleteon_ChanceToResummonOnDeath");
                                Minions.skeleton_resummon_on_death_text = Functions.Get_TextInToggle(minions_content, "Skeleteon_ChanceToResummonOnDeath", "Toggle_Skeleteon_ChanceToResummonOnDeath", "Value");
                                Minions.skeleton_resummon_on_death_slider = Functions.Get_SliderInPanel(minions_content, "Skeleteon_ChanceToResummonOnDeath", "Slider_Skeleteon_ChanceToResummonOnDeath");

                                Minions.enable_skeleton_force_archer_toggle = Functions.Get_ToggleInPanel(minions_content, "Skeleton_ForceArcher", "Toggle_Skeleton_ForceArcher");
                                Minions.enable_skeleton_force_brawler_toggle = Functions.Get_ToggleInPanel(minions_content, "Skeleton_ForceBrawler", "Toggle_Skeleton_ForceBrawler");
                                Minions.enable_skeleton_force_warrior_toggle = Functions.Get_ToggleInPanel(minions_content, "Skeleton_ForceWarrior", "Toggle_Skeleton_ForceWarrior");

                                //Wraiths
                                Minions.enable_wraith_summon_limit_toggle = Functions.Get_ToggleInPanel(minions_content, "Wraiths_SummonMax", "Toggle_Wraiths_SummonMax");
                                Minions.wraith_summon_limit_text = Functions.Get_TextInToggle(minions_content, "Wraiths_SummonMax", "Toggle_Wraiths_SummonMax", "Value");
                                Minions.wraith_summon_limit_slider = Functions.Get_SliderInPanel(minions_content, "Wraiths_SummonMax", "Slider_Wraiths_SummonMax");

                                Minions.enable_wraith_delay_toggle = Functions.Get_ToggleInPanel(minions_content, "Wraiths_Delayed", "Toggle_Wraiths_Delayed");
                                Minions.wraith_delay_text = Functions.Get_TextInToggle(minions_content, "Wraiths_Delayed", "Toggle_Wraiths_Delayed", "Value");
                                Minions.wraith_delay_slider = Functions.Get_SliderInPanel(minions_content, "Wraiths_Delayed", "Slider_Wraiths_Delayed");

                                Minions.enable_wraith_cast_speed_toggle = Functions.Get_ToggleInPanel(minions_content, "Wraiths_CastSpeed", "Toggle_Wraiths_CastSpeed");
                                Minions.wraith_cast_speed_text = Functions.Get_TextInToggle(minions_content, "Wraiths_CastSpeed", "Toggle_Wraiths_CastSpeed", "Value");
                                Minions.wraith_cast_speed_slider = Functions.Get_SliderInPanel(minions_content, "Wraiths_CastSpeed", "Slider_Wraiths_CastSpeed");

                                Minions.enable_wraith_no_limit_toggle = Functions.Get_ToggleInPanel(minions_content, "Wraiths_DisableLimitTo2", "Toggle_Wraiths_DisableLimitTo2");
                                Minions.enable_wraith_no_decay_toggle = Functions.Get_ToggleInPanel(minions_content, "Wraiths_DisableDecay", "Toggle_Wraiths_DisableDecay");

                                //Mages
                                Minions.enable_mage_passive_summon_toggle = Functions.Get_ToggleInPanel(minions_content, "Mages_SummonQuantityFromPassive", "Toggle_Mages_SummonQuantityFromPassive");
                                Minions.mage_passive_summon_text = Functions.Get_TextInToggle(minions_content, "Mages_SummonQuantityFromPassive", "Toggle_Mages_SummonQuantityFromPassive", "Value");
                                Minions.mage_passive_summon_slider = Functions.Get_SliderInPanel(minions_content, "Mages_SummonQuantityFromPassive", "Slider_Mages_SummonQuantityFromPassive");

                                Minions.enable_mage_skilltree_summon_toggle = Functions.Get_ToggleInPanel(minions_content, "Mages_SummonQuantityFromSkillTree", "Toggle_Mages_SummonQuantityFromSkillTree");
                                Minions.mage_skilltree_summon_text = Functions.Get_TextInToggle(minions_content, "Mages_SummonQuantityFromSkillTree", "Toggle_Mages_SummonQuantityFromSkillTree", "Value");
                                Minions.mage_skilltree_summon_slider = Functions.Get_SliderInPanel(minions_content, "Mages_SummonQuantityFromSkillTree", "Slider_Mages_SummonQuantityFromSkillTree");

                                Minions.enable_mage_items_summon_toggle = Functions.Get_ToggleInPanel(minions_content, "Mages_SummonQuantityFromItems", "Toggle_Mages_SummonQuantityFromItems");
                                Minions.mage_items_summon_text = Functions.Get_TextInToggle(minions_content, "Mages_SummonQuantityFromItems", "Toggle_Mages_SummonQuantityFromItems", "Value");
                                Minions.mage_items_summon_slider = Functions.Get_SliderInPanel(minions_content, "Mages_SummonQuantityFromItems", "Slider_Mages_SummonQuantityFromItems");

                                Minions.enable_mage_per_cast_summon_toggle = Functions.Get_ToggleInPanel(minions_content, "Mages_SummonPerCast", "Toggle_Mages_SummonPerCast");
                                Minions.mage_per_cast_summon_text = Functions.Get_TextInToggle(minions_content, "Mages_SummonPerCast", "Toggle_Mages_SummonPerCast", "Value");
                                Minions.mage_per_cast_summon_slider = Functions.Get_SliderInPanel(minions_content, "Mages_SummonPerCast", "Slider_Mages_SummonPerCast");

                                Minions.enable_mage_projectile_chance_toggle = Functions.Get_ToggleInPanel(minions_content, "Mages_ChanceForExtraPorjectiles", "Toggle_Mages_ChanceForExtraPorjectiles");
                                Minions.mage_projectile_chance_text = Functions.Get_TextInToggle(minions_content, "Mages_ChanceForExtraPorjectiles", "Toggle_Mages_ChanceForExtraPorjectiles", "Value");
                                Minions.mage_projectile_chance_slider = Functions.Get_SliderInPanel(minions_content, "Mages_ChanceForExtraPorjectiles", "Slider_Mages_ChanceForExtraPorjectiles");

                                Minions.enable_mage_force_cryomancer_toggle = Functions.Get_ToggleInPanel(minions_content, "Mages_ForceCryomancer", "Toggle_Mages_ForceCryomancer");
                                Minions.enable_mage_force_deathknight_toggle = Functions.Get_ToggleInPanel(minions_content, "Mages_ForceDeathKnight", "Toggle_Mages_ForceDeathKnight");
                                Minions.enable_mage_force_pyromancer_toggle = Functions.Get_ToggleInPanel(minions_content, "Mages_ForcePyromancer", "Toggle_Mages_ForcePyromancer");

                                //Bone Golem
                                Minions.enable_bonegolem_per_skeleton_toggle = Functions.Get_ToggleInPanel(minions_content, "BoneGolem_GolemPerSkeletons", "Toggle_BoneGolem_GolemPerSkeletons");
                                Minions.bonegolem_per_skeleton_text = Functions.Get_TextInToggle(minions_content, "BoneGolem_GolemPerSkeletons", "Toggle_BoneGolem_GolemPerSkeletons", "Value");
                                Minions.bonegolem_per_skeleton_slider = Functions.Get_SliderInPanel(minions_content, "BoneGolem_GolemPerSkeletons", "Slider_BoneGolem_GolemPerSkeletons");

                                Minions.enable_bonegolem_resurect_chance_toggle = Functions.Get_ToggleInPanel(minions_content, "BoneGolem_SelfResurectChance", "Toggle_BoneGolem_SelfResurectChance");
                                Minions.bonegolem_resurect_chance_text = Functions.Get_TextInToggle(minions_content, "BoneGolem_SelfResurectChance", "Toggle_BoneGolem_SelfResurectChance", "Value");
                                Minions.bonegolem_resurect_chance_slider = Functions.Get_SliderInPanel(minions_content, "BoneGolem_SelfResurectChance", "Slider_BoneGolem_SelfResurectChance");

                                Minions.enable_bonegolem_fire_aura_toggle = Functions.Get_ToggleInPanel(minions_content, "BoneGolem_IncreaseFireAura", "Toggle_BoneGolem_IncreaseFireAura");
                                Minions.bonegolem_fire_aura_text = Functions.Get_TextInToggle(minions_content, "BoneGolem_IncreaseFireAura", "Toggle_BoneGolem_IncreaseFireAura", "Value");
                                Minions.bonegolem_fire_aura_slider = Functions.Get_SliderInPanel(minions_content, "BoneGolem_IncreaseFireAura", "Slider_BoneGolem_IncreaseFireAura");

                                Minions.enable_bonegolem_armor_aura_toggle = Functions.Get_ToggleInPanel(minions_content, "BoneGolem_IncreaseArmorAura", "Toggle_BoneGolem_IncreaseArmorAura");
                                Minions.bonegolem_armor_aura_text = Functions.Get_TextInToggle(minions_content, "BoneGolem_IncreaseArmorAura", "Toggle_BoneGolem_IncreaseArmorAura", "Value");
                                Minions.bonegolem_armor_aura_slider = Functions.Get_SliderInPanel(minions_content, "BoneGolem_IncreaseArmorAura", "Slider_BoneGolem_IncreaseArmorAura");

                                Minions.enable_bonegolem_movespeed_aura_toggle = Functions.Get_ToggleInPanel(minions_content, "BoneGolem_IncreaseMoveSpeedAura", "Toggle_BoneGolem_IncreaseMoveSpeedAura");
                                Minions.bonegolem_movespeed_aura_text = Functions.Get_TextInToggle(minions_content, "BoneGolem_IncreaseMoveSpeedAura", "Toggle_BoneGolem_IncreaseMoveSpeedAura", "Value");
                                Minions.bonegolem_movespeed_aura_slider = Functions.Get_SliderInPanel(minions_content, "BoneGolem_IncreaseMoveSpeedAura", "Slider_BoneGolem_IncreaseMoveSpeedAura");

                                Minions.enable_bonegolem_move_speed_toggle = Functions.Get_ToggleInPanel(minions_content, "BoneGolem_IncreaseMoveSpeed", "Toggle_BoneGolem_IncreaseMoveSpeed");
                                Minions.bonegolem_move_speed_text = Functions.Get_TextInToggle(minions_content, "BoneGolem_IncreaseMoveSpeed", "Toggle_BoneGolem_IncreaseMoveSpeed", "Value");
                                Minions.bonegolem_move_speed_slider = Functions.Get_SliderInPanel(minions_content, "BoneGolem_IncreaseMoveSpeed", "Slider_BoneGolem_IncreaseMoveSpeed");

                                Minions.enable_bonegolem_twins_toggle = Functions.Get_ToggleInPanel(minions_content, "BoneGolem_Twins", "Toggle_BoneGolem_Twins");
                                Minions.enable_bonegolem_slam_toggle = Functions.Get_ToggleInPanel(minions_content, "BoneGolem_Slam", "Toggle_BoneGolem_Slam");

                                //Volatile Zombies
                                Minions.enable_volatilezombie_cast_on_death_toggle = Functions.Get_ToggleInPanel(minions_content, "VolatileZombies_ChanceOnMinionDeath", "Toggle_VolatileZombies_ChanceOnMinionDeath");
                                Minions.volatilezombie_cast_on_death_text = Functions.Get_TextInToggle(minions_content, "VolatileZombies_ChanceOnMinionDeath", "Toggle_VolatileZombies_ChanceOnMinionDeath", "Value");
                                Minions.volatilezombie_cast_on_death_slider = Functions.Get_SliderInPanel(minions_content, "VolatileZombies_ChanceOnMinionDeath", "Slider_VolatileZombies_ChanceOnMinionDeath");

                                Minions.enable_volatilezombie_infernal_shade_toggle = Functions.Get_ToggleInPanel(minions_content, "VolatileZombies_InfernalShadeChance", "Toggle_VolatileZombies_InfernalShadeChance");
                                Minions.volatilezombie_infernal_shade_text = Functions.Get_TextInToggle(minions_content, "VolatileZombies_InfernalShadeChance", "Toggle_VolatileZombies_InfernalShadeChance", "Value");
                                Minions.volatilezombie_infernal_shade_slider = Functions.Get_SliderInPanel(minions_content, "VolatileZombies_InfernalShadeChance", "Slider_VolatileZombies_InfernalShadeChance");

                                Minions.enable_volatilezombie_marrow_shards_toggle = Functions.Get_ToggleInPanel(minions_content, "VolatileZombies_MarrowShardsChance", "Toggle_VolatileZombies_MarrowShardsChance");
                                Minions.volatilezombie_marrow_shards_text = Functions.Get_TextInToggle(minions_content, "VolatileZombies_MarrowShardsChance", "Toggle_VolatileZombies_MarrowShardsChance", "Value");
                                Minions.volatilezombie_marrow_shards_slider = Functions.Get_SliderInPanel(minions_content, "VolatileZombies_MarrowShardsChance", "Slider_VolatileZombies_MarrowShardsChance");

                                //Dreadshades
                                Minions.enable_dreadShades_max_toggle = Functions.Get_ToggleInPanel(minions_content, "DreadShades_Max", "Toggle_DreadShades_Max");
                                Minions.dreadShades_max_text = Functions.Get_TextInToggle(minions_content, "DreadShades_Max", "Toggle_DreadShades_Max", "Value");
                                Minions.dreadShades_max_slider = Functions.Get_SliderInPanel(minions_content, "DreadShades_Max", "Slider_DreadShades_Max");

                                Minions.enable_dreadShades_duration_toggle = Functions.Get_ToggleInPanel(minions_content, "DreadShades_Duration", "Toggle_DreadShades_Duration");
                                Minions.dreadShades_duration_text = Functions.Get_TextInToggle(minions_content, "DreadShades_Duration", "Toggle_DreadShades_Duration", "Value");
                                Minions.dreadShades_duration_slider = Functions.Get_SliderInPanel(minions_content, "DreadShades_Duration", "Slider_DreadShades_Duration");

                                Minions.enable_dreadShades_decay_toggle = Functions.Get_ToggleInPanel(minions_content, "DreadShades_Decay", "Toggle_DreadShades_Decay");
                                Minions.dreadShades_decay_text = Functions.Get_TextInToggle(minions_content, "DreadShades_Decay", "Toggle_DreadShades_Decay", "Value");
                                Minions.dreadShades_decay_slider = Functions.Get_SliderInPanel(minions_content, "DreadShades_Decay", "Slider_DreadShades_Decay");

                                Minions.enable_dreadShades_radius_toggle = Functions.Get_ToggleInPanel(minions_content, "DreadShades_Radius", "Toggle_DreadShades_Radius");
                                Minions.dreadShades_radius_text = Functions.Get_TextInToggle(minions_content, "DreadShades_Radius", "Toggle_DreadShades_Radius", "Value");
                                Minions.dreadShades_radius_slider = Functions.Get_SliderInPanel(minions_content, "DreadShades_Radius", "Slider_DreadShades_Radius");

                                Minions.enable_dreadShades_summon_limit_toggle = Functions.Get_ToggleInPanel(minions_content, "DreadShades_DisableLimit", "Toggle_DreadShades_DisableLimit");
                                Minions.enable_dreadShades_health_drain_toggle = Functions.Get_ToggleInPanel(minions_content, "DreadShades_DisableHealthDrain", "Toggle_DreadShades_DisableHealthDrain");
                            }
                            else { Main.logger_instance.Error("Minions content is null"); }
                        }
                        else { Main.logger_instance.Error("Skill Tree content is null"); }
                    }
                }
                public static void Set_Events()
                {
                    if (!SkillTree.enable_summon_godmode_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(SkillTree.enable_summon_godmode_toggle, SkillTree.Summon_Godmode_Toggle_Action);
                    }
                    if (!SkillTree.enable_summon_forever_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(SkillTree.enable_summon_forever_toggle, SkillTree.Summon_Forever_Toggle_Action);
                    }
                    if (!SkillTree.enable_summon_dontcollide_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(SkillTree.enable_summon_dontcollide_toggle, SkillTree.Summon_DontCollide_Toggle_Action);
                    }
                }
                public static void Set_Active(bool show)
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static void Toggle_Active()
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        bool show = !content_obj.active;
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static bool Init_UserData()
                {
                    bool result = false;
                    if (!Save_Manager.instance.IsNullOrDestroyed())
                    {
                        if ((Save_Manager.instance.initialized) && (!Save_Manager.instance.data.IsNullOrDestroyed()))
                        {
                            SkillTree.enable_remove_mana_cost_toggle.isOn = Save_Manager.instance.data.Skills.Enable_RemoveManaCost;
                            SkillTree.enable_remove_channel_cost_toggle.isOn = Save_Manager.instance.data.Skills.Enable_RemoveChannelCost;
                            SkillTree.enable_mana_regen_when_channeling_toggle.isOn = Save_Manager.instance.data.Skills.Enable_NoManaRegenWhileChanneling;
                            SkillTree.enable_dont_stop_oom_toggle.isOn = Save_Manager.instance.data.Skills.Enable_StopWhenOutOfMana;
                            SkillTree.enable_no_cooldown_toggle.isOn = Save_Manager.instance.data.Skills.Enable_RemoveCooldown;
                            SkillTree.enable_unlock_all_skills_toggle.isOn = Save_Manager.instance.data.Skills.Enable_AllSkills;
                            SkillTree.enable_remove_node_req_toggle.isOn = Save_Manager.instance.data.Skills.Disable_NodeRequirement;

                            SkillTree.enable_specialization_slots_toggle.isOn = Save_Manager.instance.data.Skills.Enable_SpecializationSlots;
                            SkillTree.specialization_slots_slider.value = Save_Manager.instance.data.Skills.SpecializationSlots;

                            SkillTree.enable_skill_level_toggle.isOn = Save_Manager.instance.data.Skills.Enable_SkillLevel;
                            SkillTree.skill_level_slider.value = Save_Manager.instance.data.Skills.SkillLevel;

                            SkillTree.enable_passive_points_toggle.isOn = Save_Manager.instance.data.Skills.Enable_PassivePoints;
                            SkillTree.passive_points_slider.value = Save_Manager.instance.data.Skills.PassivePoints;

                            SkillTree.enable_movement_no_target_toggle.isOn = Save_Manager.instance.data.Skills.MovementSkills.Enable_NoTarget;
                            SkillTree.enable_movement_immune_toggle.isOn = Save_Manager.instance.data.Skills.MovementSkills.Enable_ImmuneDuringMovement;
                            SkillTree.enable_movement_simple_path_toggle.isOn = Save_Manager.instance.data.Skills.MovementSkills.Disable_SimplePath;
                            
                            SkillTree.enable_summon_godmode_toggle.isOn = Save_Manager.instance.data.Summon.Enable_GodMode;
                            SkillTree.enable_summon_forever_toggle.isOn = Save_Manager.instance.data.Summon.Enable_Forever;
                            SkillTree.enable_summon_dontcollide_toggle.isOn = Save_Manager.instance.data.Summon.Enable_DontCollide;
                            //Companions
                            Companions.enable_maximum_companions_toggle.isOn = Save_Manager.instance.data.Skills.Companion.Enable_Limit;
                            Companions.maximum_companions_slider.value = Save_Manager.instance.data.Skills.Companion.Limit;

                            Companions.enable_wolf_summon_maximum_toggle.isOn = Save_Manager.instance.data.Skills.Companion.Wolf.Enable_SummonMax;

                            Companions.enable_wolf_summon_limit_toggle.isOn = Save_Manager.instance.data.Skills.Companion.Wolf.Enable_SummonLimit;
                            Companions.wolf_summon_limit_slider.value = Save_Manager.instance.data.Skills.Companion.Wolf.SummonLimit;

                            Companions.enable_wolf_stun_immunity_toggle.isOn = Save_Manager.instance.data.Skills.Companion.Wolf.Enable_StunImmunity;

                            Companions.enable_scorpion_summon_limit_toggle.isOn = Save_Manager.instance.data.Skills.Companion.Scorpion.Enable_BabyQuantity;
                            Companions.scorpion_summon_limit_slider.value = Save_Manager.instance.data.Skills.Companion.Scorpion.BabyQuantity;

                            //Skeletons
                            Minions.enable_skeleton_passive_summon_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsFromPassives;
                            Minions.skeleton_passive_summon_slider.value = Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsFromPassives;

                            Minions.enable_skeleton_skilltree_summon_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsFromSkillTree;
                            Minions.skeleton_skilltree_summon_slider.value = Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsFromSkillTree;

                            Minions.enable_skeleton_quantity_per_cast_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_additionalSkeletonsPerCast;
                            Minions.skeleton_quantity_per_cast_slider.value = Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsPerCast;

                            Minions.enable_skeleton_resummon_on_death_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_chanceToResummonOnDeath;
                            Minions.skeleton_resummon_on_death_slider.value = Save_Manager.instance.data.Skills.Minions.Skeletons.chanceToResummonOnDeath;

                            Minions.enable_skeleton_force_archer_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceArcher;
                            Minions.enable_skeleton_force_brawler_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceBrawler;
                            Minions.enable_skeleton_force_warrior_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Skeletons.Enable_forceWarrior;

                            //Wraiths
                            Minions.enable_wraith_summon_limit_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_additionalMaxWraiths;
                            Minions.wraith_summon_limit_slider.value = Save_Manager.instance.data.Skills.Minions.Wraiths.additionalMaxWraiths;

                            Minions.enable_wraith_delay_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_delayedWraiths;
                            Minions.wraith_delay_slider.value = Save_Manager.instance.data.Skills.Minions.Wraiths.delayedWraiths;

                            Minions.enable_wraith_cast_speed_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_increasedCastSpeed;
                            Minions.wraith_cast_speed_slider.value = Save_Manager.instance.data.Skills.Minions.Wraiths.increasedCastSpeed;

                            Minions.enable_wraith_no_decay_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_wraithsDoNotDecay;
                            Minions.enable_wraith_no_limit_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Wraiths.Enable_limitedTo2Wraiths;

                            //Mage
                            Minions.enable_mage_passive_summon_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromPassives;
                            Minions.mage_passive_summon_slider.value = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromPassives;

                            Minions.enable_mage_skilltree_summon_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromSkillTree;
                            Minions.mage_skilltree_summon_slider.value = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromSkillTree;

                            Minions.enable_mage_items_summon_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsFromItems;
                            Minions.mage_items_summon_slider.value = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromItems;

                            Minions.enable_mage_per_cast_summon_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Mages.Enable_additionalSkeletonsPerCast;
                            Minions.mage_per_cast_summon_slider.value = Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsPerCast;

                            Minions.enable_mage_projectile_chance_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Mages.Enable_chanceForTwoExtraProjectiles;
                            Minions.mage_projectile_chance_slider.value = Save_Manager.instance.data.Skills.Minions.Mages.chanceForTwoExtraProjectiles;

                            Minions.enable_mage_force_cryomancer_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Mages.Enable_forceCryomancer;
                            Minions.enable_mage_force_deathknight_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Mages.Enable_forceDeathKnight;
                            Minions.enable_mage_force_pyromancer_toggle.isOn = Save_Manager.instance.data.Skills.Minions.Mages.Enable_forcePyromancer;

                            //Bone Golem
                            Minions.enable_bonegolem_per_skeleton_toggle.isOn = Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_addedGolemsPer4Skeletons;
                            Minions.bonegolem_per_skeleton_slider.value = Save_Manager.instance.data.Skills.Minions.BoneGolems.addedGolemsPer4Skeletons;

                            Minions.enable_bonegolem_resurect_chance_toggle.isOn = Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_selfResurrectChance;
                            Minions.bonegolem_resurect_chance_slider.value = Save_Manager.instance.data.Skills.Minions.BoneGolems.selfResurrectChance;

                            Minions.enable_bonegolem_fire_aura_toggle.isOn = Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_increasedFireAuraArea;
                            Minions.bonegolem_fire_aura_slider.value = Save_Manager.instance.data.Skills.Minions.BoneGolems.increasedFireAuraArea;

                            Minions.enable_bonegolem_armor_aura_toggle.isOn = Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_undeadArmorAura;
                            Minions.bonegolem_armor_aura_slider.value = Save_Manager.instance.data.Skills.Minions.BoneGolems.undeadArmorAura;

                            Minions.enable_bonegolem_movespeed_aura_toggle.isOn = Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_undeadMovespeedAura;
                            Minions.bonegolem_movespeed_aura_slider.value = Save_Manager.instance.data.Skills.Minions.BoneGolems.undeadMovespeedAura;

                            Minions.enable_bonegolem_move_speed_toggle.isOn = Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_increasedMoveSpeed;
                            Minions.bonegolem_move_speed_slider.value = Save_Manager.instance.data.Skills.Minions.BoneGolems.increasedMoveSpeed;

                            Minions.enable_bonegolem_twins_toggle.isOn = Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_twins;
                            Minions.enable_bonegolem_slam_toggle.isOn = Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_hasSlamAttack;

                            //Volatile Zombies
                            Minions.enable_volatilezombie_cast_on_death_toggle.isOn = Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastFromMinionDeath;
                            Minions.volatilezombie_cast_on_death_slider.value = Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastFromMinionDeath;

                            Minions.enable_volatilezombie_infernal_shade_toggle.isOn = Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastInfernalShadeOnDeath;
                            Minions.volatilezombie_infernal_shade_slider.value = Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastInfernalShadeOnDeath;

                            Minions.enable_volatilezombie_marrow_shards_toggle.isOn = Save_Manager.instance.data.Skills.Minions.VolatileZombies.Enable_chanceToCastMarrowShardsOnDeath;
                            Minions.volatilezombie_marrow_shards_slider.value = Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastMarrowShardsOnDeath;

                            //Dreadshades
                            Minions.enable_dreadShades_duration_toggle.isOn = Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Duration;
                            Minions.dreadShades_duration_slider.value = Save_Manager.instance.data.Skills.Minions.DreadShades.Duration;

                            Minions.enable_dreadShades_max_toggle.isOn = Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Max;
                            Minions.dreadShades_max_slider.value = Save_Manager.instance.data.Skills.Minions.DreadShades.max;

                            Minions.enable_dreadShades_decay_toggle.isOn = Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_ReduceDecay;
                            Minions.dreadShades_decay_slider.value = Save_Manager.instance.data.Skills.Minions.DreadShades.decay;

                            Minions.enable_dreadShades_radius_toggle.isOn = Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_Radius;
                            Minions.dreadShades_radius_slider.value = Save_Manager.instance.data.Skills.Minions.DreadShades.radius;

                            Minions.enable_dreadShades_summon_limit_toggle.isOn = Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_DisableLimit;
                            Minions.enable_dreadShades_health_drain_toggle.isOn = Save_Manager.instance.data.Skills.Minions.DreadShades.Enable_DisableHealthDrain;

                            controls_initialized = true;
                            result = true;
                        }
                    }

                    return result;
                }
                public static void UpdateVisuals()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (controls_initialized))
                    {
                        if ((Save_Manager.instance.initialized) && (!Save_Manager.instance.data.IsNullOrDestroyed()))
                        {
                            SkillTree.specialization_slots_text.text = "" + (int)Save_Manager.instance.data.Skills.SpecializationSlots;
                            SkillTree.skill_level_text.text = "" + (int)Save_Manager.instance.data.Skills.SkillLevel;
                            SkillTree.passive_points_text.text = "" + (int)Save_Manager.instance.data.Skills.PassivePoints;

                            Companions.maximum_companions_text.text = "" + (int)Save_Manager.instance.data.Skills.Companion.Limit;
                            Companions.wolf_summon_limit_text.text = "" + (int)Save_Manager.instance.data.Skills.Companion.Wolf.SummonLimit;
                            Companions.scorpion_summon_limit_text.text = "" + (int)Save_Manager.instance.data.Skills.Companion.Scorpion.BabyQuantity;

                            Minions.skeleton_passive_summon_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsFromPassives;
                            Minions.skeleton_skilltree_summon_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsFromSkillTree;
                            Minions.skeleton_quantity_per_cast_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Skeletons.additionalSkeletonsPerCast;
                            Minions.skeleton_resummon_on_death_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Skeletons.chanceToResummonOnDeath;

                            Minions.wraith_summon_limit_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Wraiths.additionalMaxWraiths;
                            Minions.wraith_delay_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Wraiths.delayedWraiths;
                            Minions.wraith_cast_speed_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Wraiths.increasedCastSpeed;

                            Minions.mage_passive_summon_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromPassives;
                            Minions.mage_skilltree_summon_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromSkillTree;
                            Minions.mage_items_summon_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsFromItems;
                            Minions.mage_per_cast_summon_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Mages.additionalSkeletonsPerCast;
                            Minions.mage_projectile_chance_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.Mages.chanceForTwoExtraProjectiles;

                            Minions.bonegolem_per_skeleton_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.BoneGolems.addedGolemsPer4Skeletons;
                            Minions.bonegolem_resurect_chance_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.BoneGolems.selfResurrectChance;
                            Minions.bonegolem_fire_aura_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.BoneGolems.increasedFireAuraArea;
                            Minions.bonegolem_armor_aura_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.BoneGolems.undeadArmorAura;
                            Minions.bonegolem_movespeed_aura_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.BoneGolems.undeadMovespeedAura;
                            Minions.bonegolem_move_speed_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.BoneGolems.increasedMoveSpeed;

                            Minions.volatilezombie_cast_on_death_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastFromMinionDeath;
                            Minions.volatilezombie_infernal_shade_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastInfernalShadeOnDeath;
                            Minions.volatilezombie_marrow_shards_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.VolatileZombies.chanceToCastMarrowShardsOnDeath;

                            Minions.dreadShades_duration_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.DreadShades.Duration;
                            Minions.dreadShades_max_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.DreadShades.max;
                            Minions.dreadShades_decay_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.DreadShades.decay;
                            Minions.dreadShades_radius_text.text = "" + (int)Save_Manager.instance.data.Skills.Minions.DreadShades.radius;
                        }
                    }
                }

                public class SkillTree
                {
                    public static Toggle enable_remove_mana_cost_toggle = null;
                    public static Toggle enable_remove_channel_cost_toggle = null;
                    public static Toggle enable_mana_regen_when_channeling_toggle = null;
                    public static Toggle enable_dont_stop_oom_toggle = null;
                    public static Toggle enable_no_cooldown_toggle = null;
                    public static Toggle enable_unlock_all_skills_toggle = null;
                    public static Toggle enable_remove_node_req_toggle = null;

                    public static Toggle enable_specialization_slots_toggle = null;
                    public static Text specialization_slots_text = null;
                    public static Slider specialization_slots_slider = null;

                    public static Toggle enable_skill_level_toggle = null;
                    public static Text skill_level_text = null;
                    public static Slider skill_level_slider = null;

                    public static Toggle enable_passive_points_toggle = null;
                    public static Text passive_points_text = null;
                    public static Slider passive_points_slider = null;

                    public static Toggle enable_movement_no_target_toggle = null;
                    public static Toggle enable_movement_immune_toggle = null;
                    public static Toggle enable_movement_simple_path_toggle = null;

                    public static Toggle enable_summon_godmode_toggle = null;
                    public static readonly System.Action<bool> Summon_Godmode_Toggle_Action = new System.Action<bool>(Set_Summon_Godmode_Enable);
                    private static void Set_Summon_Godmode_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!enable_summon_godmode_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Summon.Enable_GodMode = enable_summon_godmode_toggle.isOn;
                        }
                    }
                    public static Toggle enable_summon_forever_toggle = null;
                    public static readonly System.Action<bool> Summon_Forever_Toggle_Action = new System.Action<bool>(Set_Summon_Forever_Enable);
                    private static void Set_Summon_Forever_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!enable_summon_forever_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Summon.Enable_Forever = enable_summon_forever_toggle.isOn;
                        }
                    }
                    public static Toggle enable_summon_dontcollide_toggle = null;
                    public static readonly System.Action<bool> Summon_DontCollide_Toggle_Action = new System.Action<bool>(Set_Summon_DontCollide_Enable);
                    private static void Set_Summon_DontCollide_Enable(bool enable)
                    {
                        if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!enable_summon_dontcollide_toggle.IsNullOrDestroyed()))
                        {
                            Save_Manager.instance.data.Summon.Enable_DontCollide = enable_summon_dontcollide_toggle.isOn;
                        }
                    }
                }
                public class Companions
                {
                    public static Toggle enable_maximum_companions_toggle = null;
                    public static Text maximum_companions_text = null;
                    public static Slider maximum_companions_slider = null;

                    //wolf
                    public static Toggle enable_wolf_summon_maximum_toggle = null;

                    public static Toggle enable_wolf_summon_limit_toggle = null;
                    public static Text wolf_summon_limit_text = null;
                    public static Slider wolf_summon_limit_slider = null;

                    public static Toggle enable_wolf_stun_immunity_toggle = null;

                    //Scorpions
                    public static Toggle enable_scorpion_summon_limit_toggle = null;
                    public static Text scorpion_summon_limit_text = null;
                    public static Slider scorpion_summon_limit_slider = null;
                }
                public class Minions
                {
                    //Skeletons
                    public static Toggle enable_skeleton_passive_summon_toggle = null;
                    public static Text skeleton_passive_summon_text = null;
                    public static Slider skeleton_passive_summon_slider = null;

                    public static Toggle enable_skeleton_skilltree_summon_toggle = null;
                    public static Text skeleton_skilltree_summon_text = null;
                    public static Slider skeleton_skilltree_summon_slider = null;

                    public static Toggle enable_skeleton_quantity_per_cast_toggle = null;
                    public static Text skeleton_quantity_per_cast_text = null;
                    public static Slider skeleton_quantity_per_cast_slider = null;

                    public static Toggle enable_skeleton_resummon_on_death_toggle = null;
                    public static Text skeleton_resummon_on_death_text = null;
                    public static Slider skeleton_resummon_on_death_slider = null;

                    public static Toggle enable_skeleton_force_archer_toggle = null;
                    public static Toggle enable_skeleton_force_brawler_toggle = null;
                    public static Toggle enable_skeleton_force_warrior_toggle = null;

                    //Wraiths
                    public static Toggle enable_wraith_summon_limit_toggle = null;
                    public static Text wraith_summon_limit_text = null;
                    public static Slider wraith_summon_limit_slider = null;

                    public static Toggle enable_wraith_delay_toggle = null;
                    public static Text wraith_delay_text = null;
                    public static Slider wraith_delay_slider = null;

                    public static Toggle enable_wraith_cast_speed_toggle = null;
                    public static Text wraith_cast_speed_text = null;
                    public static Slider wraith_cast_speed_slider = null;

                    public static Toggle enable_wraith_no_limit_toggle = null;
                    public static Toggle enable_wraith_no_decay_toggle = null;

                    //Mage
                    public static Toggle enable_mage_passive_summon_toggle = null;
                    public static Text mage_passive_summon_text = null;
                    public static Slider mage_passive_summon_slider = null;

                    public static Toggle enable_mage_items_summon_toggle = null;
                    public static Text mage_items_summon_text = null;
                    public static Slider mage_items_summon_slider = null;

                    public static Toggle enable_mage_skilltree_summon_toggle = null;
                    public static Text mage_skilltree_summon_text = null;
                    public static Slider mage_skilltree_summon_slider = null;

                    public static Toggle enable_mage_per_cast_summon_toggle = null;
                    public static Text mage_per_cast_summon_text = null;
                    public static Slider mage_per_cast_summon_slider = null;

                    public static Toggle enable_mage_projectile_chance_toggle = null;
                    public static Text mage_projectile_chance_text = null;
                    public static Slider mage_projectile_chance_slider = null;

                    public static Toggle enable_mage_force_cryomancer_toggle = null;
                    public static Toggle enable_mage_force_deathknight_toggle = null;
                    public static Toggle enable_mage_force_pyromancer_toggle = null;

                    //Bone Golem
                    public static Toggle enable_bonegolem_per_skeleton_toggle = null;
                    public static Text bonegolem_per_skeleton_text = null;
                    public static Slider bonegolem_per_skeleton_slider = null;

                    public static Toggle enable_bonegolem_resurect_chance_toggle = null;
                    public static Text bonegolem_resurect_chance_text = null;
                    public static Slider bonegolem_resurect_chance_slider = null;

                    public static Toggle enable_bonegolem_fire_aura_toggle = null;
                    public static Text bonegolem_fire_aura_text = null;
                    public static Slider bonegolem_fire_aura_slider = null;

                    public static Toggle enable_bonegolem_armor_aura_toggle = null;
                    public static Text bonegolem_armor_aura_text = null;
                    public static Slider bonegolem_armor_aura_slider = null;

                    public static Toggle enable_bonegolem_movespeed_aura_toggle = null;
                    public static Text bonegolem_movespeed_aura_text = null;
                    public static Slider bonegolem_movespeed_aura_slider = null;

                    public static Toggle enable_bonegolem_move_speed_toggle = null;
                    public static Text bonegolem_move_speed_text = null;
                    public static Slider bonegolem_move_speed_slider = null;

                    public static Toggle enable_bonegolem_twins_toggle = null;
                    public static Toggle enable_bonegolem_slam_toggle = null;

                    //Volatile Zombies
                    public static Toggle enable_volatilezombie_cast_on_death_toggle = null;
                    public static Text volatilezombie_cast_on_death_text = null;
                    public static Slider volatilezombie_cast_on_death_slider = null;

                    public static Toggle enable_volatilezombie_infernal_shade_toggle = null;
                    public static Text volatilezombie_infernal_shade_text = null;
                    public static Slider volatilezombie_infernal_shade_slider = null;

                    public static Toggle enable_volatilezombie_marrow_shards_toggle = null;
                    public static Text volatilezombie_marrow_shards_text = null;
                    public static Slider volatilezombie_marrow_shards_slider = null;

                    //DreadShades
                    public static Toggle enable_dreadShades_duration_toggle = null;
                    public static Text dreadShades_duration_text = null;
                    public static Slider dreadShades_duration_slider = null;

                    public static Toggle enable_dreadShades_max_toggle = null;
                    public static Text dreadShades_max_text = null;
                    public static Slider dreadShades_max_slider = null;

                    public static Toggle enable_dreadShades_decay_toggle = null;
                    public static Text dreadShades_decay_text = null;
                    public static Slider dreadShades_decay_slider = null;

                    public static Toggle enable_dreadShades_radius_toggle = null;
                    public static Text dreadShades_radius_text = null;
                    public static Slider dreadShades_radius_slider = null;

                    public static Toggle enable_dreadShades_summon_limit_toggle = null;
                    public static Toggle enable_dreadShades_health_drain_toggle = null;
                }
            }
            public class OdlForceDrop
            {
                public static bool initialized = false;
                public static bool enable = false;

                public static GameObject content_obj = null;
                public static GameObject left_base_content = null;
                public static GameObject center_content_1 = null; //Used for Keyboard
                public static GameObject center_content = null; //viewport

                //Type
                public static int type_size = 24;
                public static Dropdown type_dropdown = null;
                public static int item_type = -1;
                public static EquipmentType item_equipmenttype = EquipmentType.ARCTUS_LENS;
                public static bool Type_Initialized = false;
                public static bool Initializing_type = false;

                //Rarity
                public static int rarity_size = 24;
                public static Dropdown rarity_dropdown = null;
                public static int item_rarity = -1;

                //Items
                public static int items_size = 24;
                public static Dropdown items_dropdown = null;
                public static int item_subtype = -1;
                public static int item_unique_id = -1;

                //Implicits
                public static bool implicits_enable = false;
                public static int implicits_size = 24;
                public static bool implicits_roll = false;
                public static int implicits_roll_size = 44;
                public static GameObject implicits = null;
                public static GameObject implicits_border = null;
                public static Dropdown implicits_dropdown = null;

                public static GameObject implicit_0 = null;
                public static Text implicit_0_Text = null;
                public static Slider implicit_0_slider = null;
                public static readonly System.Action<float> implicit_0_Action = new System.Action<float>(SetImplicit_0);

                public static GameObject implicit_1 = null;
                public static Text implicit_1_Text = null;
                public static Slider implicit_1_slider = null;
                public static readonly System.Action<float> implicit_1_Action = new System.Action<float>(SetImplicit_1);

                public static GameObject implicit_2 = null;
                public static Text implicit_2_Text = null;
                public static Slider implicit_2_slider = null;
                public static readonly System.Action<float> implicit_2_Action = new System.Action<float>(SetImplicit_2);

                //Forgin potencial
                public static bool forgin_potencial_enable = false;
                public static int forgin_potencial_size = 24;
                public static bool forgin_potencial_roll = false;
                public static int forgin_potencial_roll_size = 42;
                public static GameObject forgin_potencial = null;
                public static GameObject forgin_potencial_border = null;
                public static Dropdown forgin_potencial_dropdown = null;

                public static GameObject forgin_potencial_value = null;
                public static Text forgin_potencial_text = null;
                public static Slider forgin_potencial_slider = null;
                public static readonly System.Action<float> forgin_potencial_Action = new System.Action<float>(SetForginPotencial);

                public static string select_affix = "Select Affix";

                //Seal
                public static bool seal_enable = false;
                public static int seal_id = -1;
                public static string seal_name = "";
                public static bool seal_roll = false;
                public static GameObject seal = null;
                public static GameObject seal_border = null;
                public static Dropdown seal_dropdown = null;

                public static GameObject seal_shard = null;
                public static Button seal_select_btn = null;
                public static Text seal_select_text = null;
                public static readonly System.Action Seal_OnClick_Action = new System.Action(SelectSeal);

                public static GameObject seal_tier = null;
                public static Text seal_tier_text = null;
                public static Slider seal_tier_slider = null;
                public static readonly System.Action<float> seal_tier_Action = new System.Action<float>(SetSealTier);

                public static GameObject seal_value = null;
                public static Text seal_value_text = null;
                public static Slider seal_value_slider = null;
                public static readonly System.Action<float> seal_value_Action = new System.Action<float>(SetSealValue);

                //Affix
                public static bool affixs_enable = false;
                public static bool affixs_roll = false;
                public static GameObject affixs = null;
                public static GameObject affixs_border = null;
                public static Dropdown affixs_dropdown = null;

                public static GameObject affixs_numbers = null;
                public static Text affixs_numbers_text = null;
                public static Slider affixs_numbers_slider = null;

                public static bool affix_0_enable = false;
                public static int affix_0_id = -1;
                public static string affix_0_name = "";
                public static Text affix_0_select_text = null;
                public static GameObject affix_0 = null;
                public static Button affix_0_button = null;
                public static readonly System.Action affix_0_OnClick_Action = new System.Action(SelectAffix_0);
                public static Text affix_0_tier_text = null;
                public static Slider affix_0_tier_slider = null;
                public static readonly System.Action<float> affix_0_tier_Action = new System.Action<float>(SetAffix_0_Tier);
                public static Text affix_0_value_text = null;
                public static Toggle affix_0_random_toggle = null;
                public static readonly System.Action<bool> Affix_0_RandomRoll_Toggle_Action = new System.Action<bool>(Set_Affix_0_RandomRoll_Enable);
                public static Slider affix_0_value_slider = null;
                public static readonly System.Action<float> affix_0_value_Action = new System.Action<float>(SetAffix_0_Value);

                public static bool affix_1_enable = false;
                public static int affix_1_id = -1;
                public static string affix_1_name = "";
                public static Text affix_1_select_text = null;
                public static GameObject affix_1 = null;
                public static Button affix_1_button = null;
                public static readonly System.Action affix_1_OnClick_Action = new System.Action(SelectAffix_1);
                public static Text affix_1_tier_text = null;
                public static Slider affix_1_tier_slider = null;
                public static readonly System.Action<float> affix_1_tier_Action = new System.Action<float>(SetAffix_1_Tier);
                public static Text affix_1_value_text = null;
                public static Toggle affix_1_random_toggle = null;
                public static readonly System.Action<bool> Affix_1_RandomRoll_Toggle_Action = new System.Action<bool>(Set_Affix_1_RandomRoll_Enable);
                public static Slider affix_1_value_slider = null;
                public static readonly System.Action<float> affix_1_value_Action = new System.Action<float>(SetAffix_1_Value);

                public static bool affix_2_enable = false;
                public static int affix_2_id = -1;
                public static string affix_2_name = "";
                public static Text affix_2_select_text = null;
                public static GameObject affix_2 = null;
                public static Button affix_2_button = null;
                public static readonly System.Action affix_2_OnClick_Action = new System.Action(SelectAffix_2);
                public static Text affix_2_tier_text = null;
                public static Slider affix_2_tier_slider = null;
                public static readonly System.Action<float> affix_2_tier_Action = new System.Action<float>(SetAffix_2_Tier);
                public static Text affix_2_value_text = null;
                public static Toggle affix_2_random_toggle = null;
                public static readonly System.Action<bool> Affix_2_RandomRoll_Toggle_Action = new System.Action<bool>(Set_Affix_2_RandomRoll_Enable);
                public static Slider affix_2_value_slider = null;
                public static readonly System.Action<float> affix_2_value_Action = new System.Action<float>(SetAffix_2_Value);

                public static bool affix_3_enable = false;
                public static int affix_3_id = -1;
                public static string affix_3_name = "";
                public static Text affix_3_select_text = null;
                public static GameObject affix_3 = null;
                public static Button affix_3_button = null;
                public static readonly System.Action affix_3_OnClick_Action = new System.Action(SelectAffix_3);
                public static Text affix_3_tier_text = null;
                public static Slider affix_3_tier_slider = null;
                public static readonly System.Action<float> affix_3_tier_Action = new System.Action<float>(SetAffix_3_Tier);
                public static Text affix_3_value_text = null;
                public static Toggle affix_3_random_toggle = null;
                public static readonly System.Action<bool> Affix_3_RandomRoll_Toggle_Action = new System.Action<bool>(Set_Affix_3_RandomRoll_Enable);
                public static Slider affix_3_value_slider = null;
                public static readonly System.Action<float> affix_3_value_Action = new System.Action<float>(SetAffix_3_Value);

                public static bool affix_4_enable = false;
                public static int affix_4_id = -1;
                public static string affix_4_name = "";
                public static Text affix_4_select_text = null;
                public static GameObject affix_4 = null;
                public static Button affix_4_button = null;
                public static readonly System.Action affix_4_OnClick_Action = new System.Action(SelectAffix_4);
                public static Text affix_4_tier_text = null;
                public static Slider affix_4_tier_slider = null;
                public static readonly System.Action<float> affix_4_tier_Action = new System.Action<float>(SetAffix_4_Tier);
                public static Text affix_4_value_text = null;
                public static Toggle affix_4_random_toggle = null;
                public static readonly System.Action<bool> Affix_4_RandomRoll_Toggle_Action = new System.Action<bool>(Set_Affix_4_RandomRoll_Enable);
                public static Slider affix_4_value_slider = null;
                public static readonly System.Action<float> affix_4_value_Action = new System.Action<float>(SetAffix_4_Value);

                public static bool affix_5_enable = false;
                public static int affix_5_id = -1;
                public static string affix_5_name = "";
                public static Text affix_5_select_text = null;
                public static GameObject affix_5 = null;
                public static Button affix_5_button = null;
                public static readonly System.Action affix_5_OnClick_Action = new System.Action(SelectAffix_5);
                public static Text affix_5_tier_text = null;
                public static Slider affix_5_tier_slider = null;
                public static readonly System.Action<float> affix_5_tier_Action = new System.Action<float>(SetAffix_5_Tier);
                public static Text affix_5_value_text = null;
                public static Toggle affix_5_random_toggle = null;
                public static readonly System.Action<bool> Affix_5_RandomRoll_Toggle_Action = new System.Action<bool>(Set_Affix_5_RandomRoll_Enable);
                public static Slider affix_5_value_slider = null;
                public static readonly System.Action<float> affix_5_value_Action = new System.Action<float>(SetAffix_5_Value);

                //Unique mods
                public static bool unique_mods_enable = false;
                public static GameObject unique_mods = null;
                public static GameObject unique_mods_border = null;
                public static Dropdown unique_mods_dropdown = null;

                public static bool unique_mods_roll_0 = false;
                public static GameObject unique_mod_0 = null;
                public static Text unique_mod_0_Text = null;
                public static Slider unique_mod_0_slider = null;
                public static readonly System.Action<float> unique_mod_0_Action = new System.Action<float>(SetUniqueMod_0);

                public static bool unique_mods_roll_1 = false;
                public static GameObject unique_mod_1 = null;
                public static Text unique_mod_1_Text = null;
                public static Slider unique_mod_1_slider = null;
                public static readonly System.Action<float> unique_mod_1_Action = new System.Action<float>(SetUniqueMod_1);

                public static bool unique_mods_roll_2 = false;
                public static GameObject unique_mod_2 = null;
                public static Text unique_mod_2_Text = null;
                public static Slider unique_mod_2_slider = null;
                public static readonly System.Action<float> unique_mod_2_Action = new System.Action<float>(SetUniqueMod_2);

                public static bool unique_mods_roll_3 = false;
                public static GameObject unique_mod_3 = null;
                public static Text unique_mod_3_Text = null;
                public static Slider unique_mod_3_slider = null;
                public static readonly System.Action<float> unique_mod_3_Action = new System.Action<float>(SetUniqueMod_3);

                public static bool unique_mods_roll_4 = false;
                public static GameObject unique_mod_4 = null;
                public static Text unique_mod_4_Text = null;
                public static Slider unique_mod_4_slider = null;
                public static readonly System.Action<float> unique_mod_4_Action = new System.Action<float>(SetUniqueMod_4);

                public static bool unique_mods_roll_5 = false;
                public static GameObject unique_mod_5 = null;
                public static Text unique_mod_5_Text = null;
                public static Slider unique_mod_5_slider = null;
                public static readonly System.Action<float> unique_mod_5_Action = new System.Action<float>(SetUniqueMod_5);

                public static bool unique_mods_roll_6 = false;
                public static GameObject unique_mod_6 = null;
                public static Text unique_mod_6_Text = null;
                public static Slider unique_mod_6_slider = null;
                public static readonly System.Action<float> unique_mod_6_Action = new System.Action<float>(SetUniqueMod_6);

                public static bool unique_mods_roll_7 = false;
                public static GameObject unique_mod_7 = null;
                public static Text unique_mod_7_Text = null;
                public static Slider unique_mod_7_slider = null;
                public static readonly System.Action<float> unique_mod_7_Action = new System.Action<float>(SetUniqueMod_7);

                //Beast Evolutions
                public static GameObject beast_evolution_border = null;
                public static int evo_count = 0;

                public static GameObject nb_evolution = null;
                public static Text nb_evolution_Text = null;
                public static Slider nb_evolution_slider = null;
                public static readonly System.Action<float> nb_evolution_Action = new System.Action<float>(SetNbEvolution);

                public static bool beast_evolution_0_enable = false;
                public static GameObject beast_evolution_0 = null;
                public static Dropdown beast_evolution_0_dropdown = null;
                public static int beast_evolution_0_select = 0;
                public static bool beast_evolution_select_0_enable = false;
                public static GameObject beast_evolution_select_0 = null;
                public static Dropdown beast_evolution_0_select_dropdown = null;

                public static bool beast_evolution_1_enable = false;
                public static GameObject beast_evolution_1 = null;
                public static Dropdown beast_evolution_1_dropdown = null;
                public static int beast_evolution_1_select = 0;
                public static bool beast_evolution_select_1_enable = false;
                public static GameObject beast_evolution_select_1 = null;
                public static Dropdown beast_evolution_1_select_dropdown = null;

                public static bool beast_evolution_2_enable = false;
                public static GameObject beast_evolution_2 = null;
                public static Dropdown beast_evolution_2_dropdown = null;
                public static int beast_evolution_2_select = 0;
                public static bool beast_evolution_select_2_enable = false;
                public static GameObject beast_evolution_select_2 = null;
                public static Dropdown beast_evolution_2_select_dropdown = null;

                public static bool beast_evolution_3_enable = false;
                public static GameObject beast_evolution_3 = null;
                public static Dropdown beast_evolution_3_dropdown = null;
                public static int beast_evolution_3_select = 0;
                public static bool beast_evolution_select_3_enable = false;
                public static GameObject beast_evolution_select_3 = null;
                public static Dropdown beast_evolution_3_select_dropdown = null;

                public static bool beast_evolution_4_enable = false;
                public static GameObject beast_evolution_4 = null;
                public static Dropdown beast_evolution_4_dropdown = null;
                public static int beast_evolution_4_select = 0;
                public static bool beast_evolution_select_4_enable = false;
                public static GameObject beast_evolution_select_4 = null;
                public static Dropdown beast_evolution_4_select_dropdown = null;

                public static bool beast_evolution_5_enable = false;
                public static GameObject beast_evolution_5 = null;
                public static Dropdown beast_evolution_5_dropdown = null;
                public static int beast_evolution_5_select = 0;
                public static bool beast_evolution_select_5_enable = false;
                public static GameObject beast_evolution_select_5 = null;
                public static Dropdown beast_evolution_5_select_dropdown = null;

                public static bool beast_evolution_6_enable = false;
                public static GameObject beast_evolution_6 = null;
                public static Dropdown beast_evolution_6_dropdown = null;
                public static int beast_evolution_6_select = 0;
                public static bool beast_evolution_select_6_enable = false;
                public static GameObject beast_evolution_select_6 = null;
                public static Dropdown beast_evolution_6_select_dropdown = null;

                //Legendary Potencial
                public static UniqueList.LegendaryType item_legendary_type = UniqueList.LegendaryType.LegendaryPotential;

                public static bool legenday_potencial_enable = false;
                public static bool legenday_potencial_roll = false;
                public static GameObject legenday_potencial = null;
                public static GameObject legenday_potencial_border = null;
                public static Dropdown legenday_potencial_dropdown = null;
                public static GameObject legenday_potencial_value = null;
                public static Text legenday_potencial_Text = null;
                public static Slider legenday_potencial_slider = null;
                public static readonly System.Action<float> legenday_potencial_Action = new System.Action<float>(SetLegendayPotencial);

                //Weaver will
                public static bool weaver_will_enable = false;
                public static bool weaver_will_roll = false;
                public static GameObject weaver_will = null;
                public static GameObject weaver_will_border = null;
                public static Dropdown weaver_will_dropdown = null;
                public static GameObject weaver_will_value = null;
                public static Text weaver_will_Text = null;
                public static Slider weaver_will_slider = null;
                public static readonly System.Action<float> weaver_will_Action = new System.Action<float>(SetWeaverWill);

                //Corrupted
                public static bool corrupted_enable = false;
                public static GameObject corrupted = null;
                public static Toggle toggle_corrupted = null;
                public static GameObject corrupted_border = null;

                //Quantity
                public static bool quantity_enable = false;
                public static GameObject quantity = null;
                public static GameObject quantity_border = null;
                public static Text quantity_text = null;
                public static Slider forcedrop_quantity_slider = null;

                //Drop button
                public static Button forcedrop_drop_button = null;
                public static bool btn_enable = false;
                public static readonly System.Action Drop_OnClick_Action = new System.Action(Drop);

                //Shards Filters
                public static GameObject shard_filters = null;
                public static Dropdown shards_filter_type = null;
                public static Dropdown shards_filter_class = null;
                //public static InputField shards_filter_name = null;
                public static TMP_InputField shards_filter_name = null;
                public static Button shards_filters_button = null;
                public static readonly System.Action Resfresh_OnClick_Action = new System.Action(InitializeShardsView);

                //Shards View
                public static readonly UnityEngine.Color color_red = new UnityEngine.Color(0.8980392f, 0.2705882f, 0f, 1f); //!naturally             
                public static readonly UnityEngine.Color color_yellow = new UnityEngine.Color(1f, 0.9607843f, 0.6078432f, 1f); //prefix
                public static readonly UnityEngine.Color color_blue = new UnityEngine.Color(0f, 0.8784314f, 1f, 1f); //suffix / idol
                public static readonly UnityEngine.Color color_green = new UnityEngine.Color(0.07058824f, 0.8980392f, 0f, 1f); //special

                public static GameObject shard_prefab = null;
                public static readonly string shard_btn_name = "ShardBtn_";
                public static bool shard_initialized = false;
                public static bool shard_seal = false;
                public static int shard_number = -1;
                public static int shard_id = -1;

                public static void Get_Refs()
                {
                    if (!Content.content_obj.IsNullOrDestroyed())
                    {
                        bool error = false;
                        content_obj = Functions.GetChild(Content.content_obj, "Old_ForceDrop_Content");
                        if (!content_obj.IsNullOrDestroyed())
                        {
                            GameObject left_obj = Functions.GetChild(content_obj, "Left");
                            if (!left_obj.IsNullOrDestroyed())
                            {
                                //Item
                                left_base_content = Functions.GetViewportContent(left_obj, "Content", "Item");
                                if (!left_base_content.IsNullOrDestroyed())
                                {
                                    type_dropdown = Functions.Get_DopboxInPanel(left_base_content, "Type", "Dropdown_Items_ForceDrop_Type", new System.Action<int>((_) => { SelectType(); }));
                                    if (type_dropdown.IsNullOrDestroyed()) { error = true; Main.logger_instance.Error("Error type_dropdown not found"); }

                                    rarity_dropdown = Functions.Get_DopboxInPanel(left_base_content, "Rarity", "Dropdown_Items_ForceDrop_Rarity", new System.Action<int>((_) => { SelectRarity(); }));
                                    if (rarity_dropdown.IsNullOrDestroyed()) { error = true; Main.logger_instance.Error("Error rarity_dropdown not found"); }

                                    items_dropdown = Functions.Get_DopboxInPanel(left_base_content, "Item", "Dropdown_Items_ForceDrop_Item", new System.Action<int>((_) => { SelectItem(); }));
                                    if (items_dropdown.IsNullOrDestroyed()) { error = true; Main.logger_instance.Error("Error items_dropdown not found"); }

                                    implicits = Functions.GetChild(left_base_content, "EnableImplicits");
                                    implicits_border = Functions.GetChild(left_base_content, "ImplicitsBorder");
                                    implicits_dropdown = Functions.Get_DopboxInPanel(left_base_content, "EnableImplicits", "Dropdown", new System.Action<int>((_) => { EnableImplicits(); }));
                                    if (implicits_dropdown.IsNullOrDestroyed()) { error = true; Main.logger_instance.Error("Error implicits_dropdown not found"); }

                                    implicit_0 = Functions.GetChild(left_base_content, "Implicit_0");
                                    implicit_0_Text = Functions.Get_TextInPanel(left_base_content, "Implicit_0", "Value");
                                    implicit_0_slider = Functions.Get_SliderInPanel(left_base_content, "Implicit_0", "Slider");

                                    implicit_1 = Functions.GetChild(left_base_content, "Implicit_1");
                                    implicit_1_Text = Functions.Get_TextInPanel(left_base_content, "Implicit_1", "Value");
                                    implicit_1_slider = Functions.Get_SliderInPanel(left_base_content, "Implicit_1", "Slider");

                                    implicit_2 = Functions.GetChild(left_base_content, "Implicit_2");
                                    implicit_2_Text = Functions.Get_TextInPanel(left_base_content, "Implicit_2", "Value");
                                    implicit_2_slider = Functions.Get_SliderInPanel(left_base_content, "Implicit_2", "Slider");

                                    forgin_potencial = Functions.GetChild(left_base_content, "EnableForginPotencial");
                                    forgin_potencial_border = Functions.GetChild(left_base_content, "ForginPotencialBorder");
                                    forgin_potencial_dropdown = Functions.Get_DopboxInPanel(left_base_content, "EnableForginPotencial", "Dropdown", new System.Action<int>((_) => { EnableForginPotencial(); }));
                                    forgin_potencial_value = Functions.GetChild(left_base_content, "ForginPotencial");
                                    forgin_potencial_text = Functions.Get_TextInPanel(left_base_content, "ForginPotencial", "Value");
                                    forgin_potencial_slider = Functions.Get_SliderInPanel(left_base_content, "ForginPotencial", "Slider");

                                    seal = Functions.GetChild(left_base_content, "EnableSeal");
                                    seal_border = Functions.GetChild(left_base_content, "SealBorder");
                                    seal_dropdown = Functions.Get_DopboxInPanel(left_base_content, "EnableSeal", "Dropdown", new System.Action<int>((_) => { EnableSeal(); }));
                                    seal_shard = Functions.GetChild(left_base_content, "SelectSeal");
                                    seal_select_btn = Functions.Get_ButtonInPanel(seal_shard, "Button");
                                    seal_select_text = Functions.Get_TextInButton(seal_shard, "Button", "Text");
                                    seal_tier = Functions.GetChild(left_base_content, "SealTier");
                                    seal_tier_text = Functions.Get_TextInPanel(left_base_content, "SealTier", "Value");
                                    seal_tier_slider = Functions.Get_SliderInPanel(left_base_content, "SealTier", "Slider");
                                    seal_value = Functions.GetChild(left_base_content, "SealValue");
                                    seal_value_text = Functions.Get_TextInPanel(left_base_content, "SealValue", "Value");
                                    seal_value_slider = Functions.Get_SliderInPanel(left_base_content, "SealValue", "Slider");

                                    affixs = Functions.GetChild(left_base_content, "EnableAffixs");
                                    if (affixs.IsNullOrDestroyed()) { Main.logger_instance.Error("affixs is null"); }
                                    affixs_border = Functions.GetChild(left_base_content, "AffixsBorder");
                                    if (affixs_border.IsNullOrDestroyed()) { Main.logger_instance.Error("seal_border is null"); }
                                    affixs_dropdown = Functions.Get_DopboxInPanel(left_base_content, "EnableAffixs", "Dropdown", new System.Action<int>((_) => { EnableAffixs(); }));
                                    if (affixs_dropdown.IsNullOrDestroyed()) { Main.logger_instance.Error("affixs_dropdown is null"); }
                                    affixs_numbers = Functions.GetChild(left_base_content, "AffixsNb");
                                    if (affixs_numbers.IsNullOrDestroyed()) { Main.logger_instance.Error("affixs_numbers is null"); }
                                    affixs_numbers_text = Functions.Get_TextInPanel(left_base_content, "AffixsNb", "Value");
                                    if (affixs_numbers_text.IsNullOrDestroyed()) { Main.logger_instance.Error("affixs_numbers_text is null"); }
                                    affixs_numbers_slider = Functions.Get_SliderInPanel(left_base_content, "AffixsNb", "Slider");
                                    if (affixs_numbers_slider.IsNullOrDestroyed()) { Main.logger_instance.Error("affixs_numbers_slider is null"); }

                                    affix_0 = Functions.GetChild(left_base_content, "Affix_0");
                                    affix_0_button = Functions.Get_ButtonInPanel(affix_0, "Button");
                                    affix_0_select_text = Functions.Get_TextInButton(affix_0, "Button", "Text");
                                    affix_0_tier_text = Functions.Get_TextInPanel(affix_0, "Tier", "Value");
                                    affix_0_tier_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_0", "TierSlider");
                                    affix_0_value_text = Functions.Get_TextInPanel(affix_0, "Roll", "Value");
                                    affix_0_random_toggle = Functions.Get_ToggleInPanel(affix_0, "RandomRoll", "Toggle_Random");
                                    affix_0_value_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_0", "ValueSlider");

                                    affix_1 = Functions.GetChild(left_base_content, "Affix_1");
                                    affix_1_button = Functions.Get_ButtonInPanel(affix_1, "Button");
                                    affix_1_select_text = Functions.Get_TextInButton(affix_1, "Button", "Text");
                                    affix_1_tier_text = Functions.Get_TextInPanel(affix_1, "Tier", "Value");
                                    affix_1_tier_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_1", "TierSlider");
                                    affix_1_value_text = Functions.Get_TextInPanel(affix_1, "Roll", "Value");
                                    affix_1_random_toggle = Functions.Get_ToggleInPanel(affix_1, "RandomRoll", "Toggle_Random");
                                    affix_1_value_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_1", "ValueSlider");

                                    affix_2 = Functions.GetChild(left_base_content, "Affix_2");
                                    affix_2_button = Functions.Get_ButtonInPanel(affix_2, "Button");
                                    affix_2_select_text = Functions.Get_TextInButton(affix_2, "Button", "Text");
                                    affix_2_tier_text = Functions.Get_TextInPanel(affix_2, "Tier", "Value");
                                    affix_2_tier_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_2", "TierSlider");
                                    affix_2_value_text = Functions.Get_TextInPanel(affix_2, "Roll", "Value");
                                    affix_2_random_toggle = Functions.Get_ToggleInPanel(affix_2, "RandomRoll", "Toggle_Random");
                                    affix_2_value_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_2", "ValueSlider");

                                    affix_3 = Functions.GetChild(left_base_content, "Affix_3");
                                    affix_3_button = Functions.Get_ButtonInPanel(affix_3, "Button");
                                    affix_3_select_text = Functions.Get_TextInButton(affix_3, "Button", "Text");
                                    affix_3_tier_text = Functions.Get_TextInPanel(affix_3, "Tier", "Value");
                                    affix_3_tier_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_3", "TierSlider");
                                    affix_3_value_text = Functions.Get_TextInPanel(affix_3, "Roll", "Value");
                                    affix_3_random_toggle = Functions.Get_ToggleInPanel(affix_3, "RandomRoll", "Toggle_Random");
                                    affix_3_value_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_3", "ValueSlider");

                                    affix_4 = Functions.GetChild(left_base_content, "Affix_4");
                                    affix_4_button = Functions.Get_ButtonInPanel(affix_4, "Button");
                                    affix_4_select_text = Functions.Get_TextInButton(affix_4, "Button", "Text");
                                    affix_4_tier_text = Functions.Get_TextInPanel(affix_4, "Tier", "Value");
                                    affix_4_tier_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_4", "TierSlider");
                                    affix_4_value_text = Functions.Get_TextInPanel(affix_4, "Roll", "Value");
                                    affix_4_random_toggle = Functions.Get_ToggleInPanel(affix_4, "RandomRoll", "Toggle_Random");
                                    affix_4_value_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_4", "ValueSlider");

                                    affix_5 = Functions.GetChild(left_base_content, "Affix_5");
                                    affix_5_button = Functions.Get_ButtonInPanel(affix_5, "Button");
                                    affix_5_select_text = Functions.Get_TextInButton(affix_5, "Button", "Text");
                                    affix_5_tier_text = Functions.Get_TextInPanel(affix_5, "Tier", "Value");
                                    affix_5_tier_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_5", "TierSlider");
                                    affix_5_value_text = Functions.Get_TextInPanel(affix_5, "Roll", "Value");
                                    affix_5_random_toggle = Functions.Get_ToggleInPanel(affix_5, "RandomRoll", "Toggle_Random");
                                    affix_5_value_slider = Functions.Get_SliderInPanel(left_base_content, "Affix_5", "ValueSlider");

                                    unique_mods = Functions.GetChild(left_base_content, "EnableUniqueMods");
                                    unique_mods_border = Functions.GetChild(left_base_content, "UniqueModsBorder");
                                    unique_mods_dropdown = Functions.Get_DopboxInPanel(left_base_content, "EnableUniqueMods", "Dropdown", new System.Action<int>((_) => { EnableUniqueMods(); }));
                                    unique_mod_0 = Functions.GetChild(left_base_content, "UniqueMod_0");
                                    unique_mod_0_Text = Functions.Get_TextInPanel(left_base_content, "UniqueMod_0", "Value");
                                    unique_mod_0_slider = Functions.Get_SliderInPanel(left_base_content, "UniqueMod_0", "Slider");
                                    unique_mod_1 = Functions.GetChild(left_base_content, "UniqueMod_1");
                                    unique_mod_1_Text = Functions.Get_TextInPanel(left_base_content, "UniqueMod_1", "Value");
                                    unique_mod_1_slider = Functions.Get_SliderInPanel(left_base_content, "UniqueMod_1", "Slider");
                                    unique_mod_2 = Functions.GetChild(left_base_content, "UniqueMod_2");
                                    unique_mod_2_Text = Functions.Get_TextInPanel(left_base_content, "UniqueMod_2", "Value");
                                    unique_mod_2_slider = Functions.Get_SliderInPanel(left_base_content, "UniqueMod_2", "Slider");
                                    unique_mod_3 = Functions.GetChild(left_base_content, "UniqueMod_3");
                                    unique_mod_3_Text = Functions.Get_TextInPanel(left_base_content, "UniqueMod_3", "Value");
                                    unique_mod_3_slider = Functions.Get_SliderInPanel(left_base_content, "UniqueMod_3", "Slider");
                                    unique_mod_4 = Functions.GetChild(left_base_content, "UniqueMod_4");
                                    unique_mod_4_Text = Functions.Get_TextInPanel(left_base_content, "UniqueMod_4", "Value");
                                    unique_mod_4_slider = Functions.Get_SliderInPanel(left_base_content, "UniqueMod_4", "Slider");
                                    unique_mod_5 = Functions.GetChild(left_base_content, "UniqueMod_5");
                                    unique_mod_5_Text = Functions.Get_TextInPanel(left_base_content, "UniqueMod_5", "Value");
                                    unique_mod_5_slider = Functions.Get_SliderInPanel(left_base_content, "UniqueMod_5", "Slider");
                                    unique_mod_6 = Functions.GetChild(left_base_content, "UniqueMod_6");
                                    unique_mod_6_Text = Functions.Get_TextInPanel(left_base_content, "UniqueMod_6", "Value");
                                    unique_mod_6_slider = Functions.Get_SliderInPanel(left_base_content, "UniqueMod_6", "Slider");
                                    unique_mod_7 = Functions.GetChild(left_base_content, "UniqueMod_7");
                                    unique_mod_7_Text = Functions.Get_TextInPanel(left_base_content, "UniqueMod_7", "Value");
                                    unique_mod_7_slider = Functions.Get_SliderInPanel(left_base_content, "UniqueMod_7", "Slider");

                                    nb_evolution = Functions.GetChild(left_base_content, "Nb_Evo");
                                    nb_evolution_Text = Functions.Get_TextInPanel(left_base_content, "Nb_Evo", "Value");
                                    nb_evolution_slider = Functions.Get_SliderInPanel(left_base_content, "Nb_Evo", "Slider");

                                    beast_evolution_border = Functions.GetChild(left_base_content, "BeastEvolBorder");
                                    beast_evolution_0 = Functions.GetChild(left_base_content, "Enable_BeastEvo_0");
                                    beast_evolution_0_dropdown = Functions.Get_DopboxInPanel(left_base_content, "Enable_BeastEvo_0", "Dropdown", new System.Action<int>((_) => { EnableBeastEvolution_0(); }));
                                    beast_evolution_select_0 = Functions.GetChild(left_base_content, "BeastEvo_0");
                                    beast_evolution_0_select_dropdown = Functions.Get_DopboxInPanel(left_base_content, "BeastEvo_0", "Dropdown", new System.Action<int>((_) => { SelectBeastEvolution_0(); }));
                                    beast_evolution_1 = Functions.GetChild(left_base_content, "Enable_BeastEvo_1");
                                    beast_evolution_1_dropdown = Functions.Get_DopboxInPanel(left_base_content, "Enable_BeastEvo_1", "Dropdown", new System.Action<int>((_) => { EnableBeastEvolution_1(); }));
                                    beast_evolution_select_1 = Functions.GetChild(left_base_content, "BeastEvo_1");
                                    beast_evolution_1_select_dropdown = Functions.Get_DopboxInPanel(left_base_content, "BeastEvo_1", "Dropdown", new System.Action<int>((_) => { SelectBeastEvolution_1(); }));
                                    beast_evolution_2 = Functions.GetChild(left_base_content, "Enable_BeastEvo_2");
                                    beast_evolution_2_dropdown = Functions.Get_DopboxInPanel(left_base_content, "Enable_BeastEvo_2", "Dropdown", new System.Action<int>((_) => { EnableBeastEvolution_2(); }));
                                    beast_evolution_select_2 = Functions.GetChild(left_base_content, "BeastEvo_2");
                                    beast_evolution_2_select_dropdown = Functions.Get_DopboxInPanel(left_base_content, "BeastEvo_2", "Dropdown", new System.Action<int>((_) => { SelectBeastEvolution_2(); }));
                                    beast_evolution_3 = Functions.GetChild(left_base_content, "Enable_BeastEvo_3");
                                    beast_evolution_3_dropdown = Functions.Get_DopboxInPanel(left_base_content, "Enable_BeastEvo_3", "Dropdown", new System.Action<int>((_) => { EnableBeastEvolution_3(); }));
                                    beast_evolution_select_3 = Functions.GetChild(left_base_content, "BeastEvo_3");
                                    beast_evolution_3_select_dropdown = Functions.Get_DopboxInPanel(left_base_content, "BeastEvo_3", "Dropdown", new System.Action<int>((_) => { SelectBeastEvolution_3(); }));
                                    beast_evolution_4 = Functions.GetChild(left_base_content, "Enable_BeastEvo_4");
                                    beast_evolution_4_dropdown = Functions.Get_DopboxInPanel(left_base_content, "Enable_BeastEvo_4", "Dropdown", new System.Action<int>((_) => { EnableBeastEvolution_4(); }));
                                    beast_evolution_select_4 = Functions.GetChild(left_base_content, "BeastEvo_4");
                                    beast_evolution_4_select_dropdown = Functions.Get_DopboxInPanel(left_base_content, "BeastEvo_4", "Dropdown", new System.Action<int>((_) => { SelectBeastEvolution_4(); }));
                                    beast_evolution_5 = Functions.GetChild(left_base_content, "Enable_BeastEvo_5");
                                    beast_evolution_5_dropdown = Functions.Get_DopboxInPanel(left_base_content, "Enable_BeastEvo_5", "Dropdown", new System.Action<int>((_) => { EnableBeastEvolution_5(); }));
                                    beast_evolution_select_5 = Functions.GetChild(left_base_content, "BeastEvo_5");
                                    beast_evolution_5_select_dropdown = Functions.Get_DopboxInPanel(left_base_content, "BeastEvo_5", "Dropdown", new System.Action<int>((_) => { SelectBeastEvolution_5(); }));
                                    beast_evolution_6 = Functions.GetChild(left_base_content, "Enable_BeastEvo_6");
                                    beast_evolution_6_dropdown = Functions.Get_DopboxInPanel(left_base_content, "Enable_BeastEvo_6", "Dropdown", new System.Action<int>((_) => { EnableBeastEvolution_6(); }));
                                    beast_evolution_select_6 = Functions.GetChild(left_base_content, "BeastEvo_6");
                                    beast_evolution_6_select_dropdown = Functions.Get_DopboxInPanel(left_base_content, "BeastEvo_6", "Dropdown", new System.Action<int>((_) => { SelectBeastEvolution_6(); }));

                                    legenday_potencial = Functions.GetChild(left_base_content, "EnableLegendaryPotencial");
                                    legenday_potencial_border = Functions.GetChild(left_base_content, "LegendaryPotencialBorder");
                                    legenday_potencial_dropdown = Functions.Get_DopboxInPanel(left_base_content, "EnableLegendaryPotencial", "Dropdown", new System.Action<int>((_) => { EnableLegendaryPotencial(); }));
                                    legenday_potencial_value = Functions.GetChild(left_base_content, "LegendaryPotencial");
                                    legenday_potencial_Text = Functions.Get_TextInPanel(left_base_content, "LegendaryPotencial", "Value");
                                    legenday_potencial_slider = Functions.Get_SliderInPanel(left_base_content, "LegendaryPotencial", "Slider");

                                    weaver_will = Functions.GetChild(left_base_content, "EnableWeaverWill");
                                    weaver_will_border = Functions.GetChild(left_base_content, "WeaverWillBorder");
                                    weaver_will_dropdown = Functions.Get_DopboxInPanel(left_base_content, "EnableWeaverWill", "Dropdown", new System.Action<int>((_) => { EnableWeaverWill(); }));
                                    weaver_will_value = Functions.GetChild(left_base_content, "WeaverWill");
                                    weaver_will_Text = Functions.Get_TextInPanel(left_base_content, "WeaverWill", "Value");
                                    weaver_will_slider = Functions.Get_SliderInPanel(left_base_content, "WeaverWill", "Slider");

                                    corrupted = Functions.GetChild(left_base_content, "Corrupted");
                                    toggle_corrupted = Functions.GetChild(corrupted, "Toggle").GetComponent<Toggle>();
                                    corrupted_border = Functions.GetChild(left_base_content, "CorruptedBorder");

                                    quantity = Functions.GetChild(left_base_content, "Quantity");
                                    quantity_border = Functions.GetChild(left_base_content, "QuantityBorder");
                                    forcedrop_quantity_slider = Functions.Get_SliderInPanel(left_base_content, "Quantity", "Slider_Items_ForceDrop_Quantity");
                                    if (forcedrop_quantity_slider.IsNullOrDestroyed()) { error = true; Main.logger_instance.Error("Error forcedrop_quantity_slider not found"); }
                                    quantity_text = Functions.Get_TextInPanel(left_base_content, "Quantity", "Value");
                                }
                                else { error = true; Main.logger_instance.Error("left_content not found"); }

                                //Drop button
                                GameObject left_content = Functions.GetChild(left_obj, "Content");
                                if (!left_content.IsNullOrDestroyed())
                                {
                                    GameObject new_obj = Functions.GetChild(left_content, "Btn");
                                    if (!new_obj.IsNullOrDestroyed())
                                    {
                                        forcedrop_drop_button = Functions.Get_ButtonInPanel(new_obj, "Btn_Drop");
                                        if (forcedrop_drop_button.IsNullOrDestroyed()) { error = true; Main.logger_instance.Error("Error forcedrop_drop_button not found"); }
                                    }
                                    else { error = true; Main.logger_instance.Error("left Btn panel not found"); }
                                }
                                else { error = true; Main.logger_instance.Error("left_content not found"); }
                            }
                            else { error = true; Main.logger_instance.Error("left_obj not found"); }                            

                            //Shards filters
                            GameObject center = Functions.GetChild(content_obj, "Center");
                            if (!center.IsNullOrDestroyed())
                            {
                                center_content_1 = Functions.GetChild(center, "Content");
                                if (!center_content_1.IsNullOrDestroyed())
                                {
                                    shard_filters = Functions.GetChild(center_content_1, "Filters");
                                    if (!shard_filters.IsNullOrDestroyed())
                                    {
                                        GameObject line_0 = Functions.GetChild(shard_filters, "Line_0");
                                        if (!line_0.IsNullOrDestroyed())
                                        {
                                            shards_filter_type = Functions.Get_DopboxInPanel(line_0, "Type", "Dropdown", new System.Action<int>((value) => { }));
                                            shards_filter_class = Functions.Get_DopboxInPanel(line_0, "Class", "Dropdown", new System.Action<int>((_) => { }));
                                        }
                                        else { error = true; Main.logger_instance.Error("line_0 not found"); }

                                        GameObject line_1 = Functions.GetChild(shard_filters, "Line_1");
                                        if (!line_1.IsNullOrDestroyed())
                                        {
                                            GameObject name = Functions.GetChild(line_1, "Name");
                                            if (!name.IsNullOrDestroyed())
                                            {
                                                GameObject g = Functions.GetChild(name, "InputField");
                                                if (!g.IsNullOrDestroyed()) { shards_filter_name = g.GetComponent<TMP_InputField>(); }
                                                //if (!g.IsNullOrDestroyed()) { shards_filter_name = g.GetComponent<InputField>(); }
                                                else { error = true; Main.logger_instance.Error("g_name not found"); }
                                            }
                                            else { error = true; Main.logger_instance.Error("name not found"); }

                                            GameObject refresh = Functions.GetChild(line_1, "Refresh");
                                            if (!refresh.IsNullOrDestroyed())
                                            {
                                                GameObject g = Functions.GetChild(refresh, "Button");
                                                if (!g.IsNullOrDestroyed()) { shards_filters_button = g.GetComponent<Button>(); }
                                                else { error = true; Main.logger_instance.Error("g_refresh not found"); }
                                            }
                                            else { error = true; Main.logger_instance.Error("refresh not found"); }
                                        }
                                        else { error = true; Main.logger_instance.Error("line_1 not found"); }
                                    }
                                    else { error = true; Main.logger_instance.Error("shard_filters not found"); }
                                }
                                //Shards
                                center_content = Functions.GetViewportContent(center, "Content", "Content");
                                if (!center_content.IsNullOrDestroyed())
                                {
                                    
                                }
                                else { error = true; Main.logger_instance.Error("center_content not found"); }
                            }
                            else { error = true; Main.logger_instance.Error("center not found"); }

                            //Shards
                            /*center_content = Functions.GetViewportContent(content_obj, "Center", "Content");
                            if (!center_content.IsNullOrDestroyed())
                            {

                            }
                            else { error = true; Main.logger_instance.Error("center_content not found"); }*/

                            //Drop button
                            /*GameObject left_obj = Functions.GetChild(content_obj, "Left");
                            if (!left_obj.IsNullOrDestroyed())
                            {
                                GameObject new_obj = Functions.GetChild(left_obj, "Btn");
                                if (!new_obj.IsNullOrDestroyed())
                                {
                                    forcedrop_drop_button = Functions.Get_ButtonInPanel(new_obj, "Btn_Drop");
                                    if (forcedrop_drop_button.IsNullOrDestroyed()) { error = true; Main.logger_instance.Error("Error forcedrop_drop_button not found"); }
                                }
                                else { error = true; Main.logger_instance.Error("left Btn panel not found"); }
                            }
                            else { error = true; Main.logger_instance.Error("left_obj not found"); }*/
                        }
                        else { error = true; Main.logger_instance.Error("content_obj is null"); }

                        if (!error) { initialized = true; }
                    }
                }
                public static void Init_BeastDropdown()
                {
                    TimeBeastData time_beast_data = null;
                    foreach (TimeBeastData data in Resources.FindObjectsOfTypeAll<TimeBeastData>())
                    {
                        time_beast_data = data;
                        break;
                    }
                    if (!time_beast_data.IsNullOrDestroyed())
                    {
                        System.Collections.Generic.List<string> evos = new System.Collections.Generic.List<string>();
                        foreach (TimeBeastData.AdaptationData adaptation in time_beast_data.adaptationData) { evos.Add(adaptation.displayName); }
                        evo_count = evos.Count;
                        Dropdown[] beast_dropdowns =
                        {
                            beast_evolution_0_select_dropdown,
                            beast_evolution_1_select_dropdown,
                            beast_evolution_2_select_dropdown,
                            beast_evolution_3_select_dropdown,
                            beast_evolution_4_select_dropdown,
                            beast_evolution_5_select_dropdown,
                            beast_evolution_6_select_dropdown
                        };
                        foreach (Dropdown dropdown in beast_dropdowns)
                        {
                            dropdown.options.Clear();
                            foreach (string s in evos) { dropdown.options.Add(new Dropdown.OptionData(s)); }
                        }
                    }
                    else { Main.logger_instance.Error("TimeBeastData not found"); }
                }
                public static void Set_Events()
                {
                    if (!forcedrop_drop_button.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(implicit_0_slider, implicit_0_Action);
                        Events.Set_Slider_Event(implicit_1_slider, implicit_1_Action);
                        Events.Set_Slider_Event(implicit_2_slider, implicit_2_Action);
                        Events.Set_Slider_Event(forgin_potencial_slider, forgin_potencial_Action);
                        Events.Set_Button_Event(seal_select_btn, Seal_OnClick_Action);
                        Events.Set_Slider_Event(seal_tier_slider, seal_tier_Action);
                        Events.Set_Slider_Event(seal_value_slider, seal_value_Action);                                                
                        Events.Set_Button_Event(affix_0_button, affix_0_OnClick_Action);
                        Events.Set_Slider_Event(affix_0_tier_slider, affix_0_tier_Action);
                        Events.Set_Toggle_Event(affix_0_random_toggle, Affix_0_RandomRoll_Toggle_Action);
                        Events.Set_Slider_Event(affix_0_value_slider, affix_0_value_Action);
                        Events.Set_Button_Event(affix_1_button, affix_1_OnClick_Action);
                        Events.Set_Slider_Event(affix_1_tier_slider, affix_1_tier_Action);
                        Events.Set_Toggle_Event(affix_1_random_toggle, Affix_1_RandomRoll_Toggle_Action);
                        Events.Set_Slider_Event(affix_1_value_slider, affix_1_value_Action);
                        Events.Set_Button_Event(affix_2_button, affix_2_OnClick_Action);
                        Events.Set_Slider_Event(affix_2_tier_slider, affix_2_tier_Action);
                        Events.Set_Toggle_Event(affix_2_random_toggle, Affix_2_RandomRoll_Toggle_Action);
                        Events.Set_Slider_Event(affix_2_value_slider, affix_2_value_Action);
                        Events.Set_Button_Event(affix_3_button, affix_3_OnClick_Action);
                        Events.Set_Slider_Event(affix_3_tier_slider, affix_3_tier_Action);
                        Events.Set_Toggle_Event(affix_3_random_toggle, Affix_3_RandomRoll_Toggle_Action);
                        Events.Set_Slider_Event(affix_3_value_slider, affix_3_value_Action);
                        Events.Set_Button_Event(affix_4_button, affix_4_OnClick_Action);
                        Events.Set_Slider_Event(affix_4_tier_slider, affix_4_tier_Action);
                        Events.Set_Toggle_Event(affix_4_random_toggle, Affix_4_RandomRoll_Toggle_Action);
                        Events.Set_Slider_Event(affix_4_value_slider, affix_4_value_Action);
                        Events.Set_Button_Event(affix_5_button, affix_5_OnClick_Action);
                        Events.Set_Slider_Event(affix_5_tier_slider, affix_5_tier_Action);
                        Events.Set_Toggle_Event(affix_5_random_toggle, Affix_5_RandomRoll_Toggle_Action);
                        Events.Set_Slider_Event(affix_5_value_slider, affix_5_value_Action);
                        Events.Set_Slider_Event(unique_mod_0_slider, unique_mod_0_Action);
                        Events.Set_Slider_Event(unique_mod_1_slider, unique_mod_1_Action);
                        Events.Set_Slider_Event(unique_mod_2_slider, unique_mod_2_Action);
                        Events.Set_Slider_Event(unique_mod_3_slider, unique_mod_3_Action);
                        Events.Set_Slider_Event(unique_mod_4_slider, unique_mod_4_Action);
                        Events.Set_Slider_Event(unique_mod_5_slider, unique_mod_5_Action);
                        Events.Set_Slider_Event(unique_mod_6_slider, unique_mod_6_Action);
                        Events.Set_Slider_Event(unique_mod_7_slider, unique_mod_7_Action);
                        Events.Set_Slider_Event(nb_evolution_slider, nb_evolution_Action);
                        Events.Set_Slider_Event(legenday_potencial_slider, legenday_potencial_Action);
                        Events.Set_Slider_Event(weaver_will_slider, weaver_will_Action);

                        Events.Set_Button_Event(shards_filters_button, Resfresh_OnClick_Action);

                        Events.Set_Button_Event(forcedrop_drop_button, Drop_OnClick_Action);
                    }
                }
                public static void Set_Active(bool show)
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static void Toggle_Active()
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        bool show = !content_obj.active;
                        content_obj.active = show;
                        enable = show;
                    }
                }

                public static void InitForcedrop()
                {
                    if ((!Type_Initialized) && (!Initializing_type))
                    {
                        Initializing_type = true;
                        type_dropdown.ClearOptions();
                        Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
                        options.Add(new Dropdown.OptionData { text = "Select" });
                        foreach (ItemList.BaseEquipmentItem item in ItemList.get().EquippableItems)
                        {
                            options.Add(new Dropdown.OptionData { text = item.BaseTypeName });
                        }
                        foreach (ItemList.BaseNonEquipmentItem item in ItemList.get().nonEquippableItems)
                        {
                            options.Add(new Dropdown.OptionData { text = item.BaseTypeName });
                        }
                        type_dropdown.options = options;
                        type_dropdown.value = 0;

                        rarity_dropdown.ClearOptions();
                        rarity_dropdown.enabled = false;

                        items_dropdown.ClearOptions();
                        items_dropdown.enabled = false;

                        //forcedrop_drop_button.enabled = false;

                        Type_Initialized = true;
                        Initializing_type = false;
                    }
                }
                public static void SelectType()
                {
                    if (Type_Initialized)
                    {
                        int index = type_dropdown.value;
                        if (index < type_dropdown.options.Count)
                        {
                            string type_str = type_dropdown.options[type_dropdown.value].text;
                            item_type = -1;
                            bool found = false;
                            foreach (ItemList.BaseEquipmentItem item in ItemList.get().EquippableItems)
                            {
                                if (item.BaseTypeName == type_str)
                                {
                                    item_type = item.baseTypeID;
                                    item_equipmenttype = item.type;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                foreach (ItemList.BaseNonEquipmentItem item in ItemList.get().nonEquippableItems)
                                {
                                    if (item.BaseTypeName == type_str)
                                    {
                                        item_type = item.baseTypeID;
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (!found) { item_type = -1; }
                            UpdateRarity();
                            UpdateItems();
                            shard_initialized = false; //Reset shards
                            //UpdateUI();
                        }
                    }
                    UpdateUI();
                }
                public static void UpdateRarity()
                {
                    if ((enable) && (LastEpoch_Hud.Scenes.IsGameScene()) &&
                        (!Refs_Manager.item_list.IsNullOrDestroyed()) &&
                        (Type_Initialized) &&
                        (!type_dropdown.IsNullOrDestroyed()) &&
                        (!rarity_dropdown.IsNullOrDestroyed()) &&
                        (!items_dropdown.IsNullOrDestroyed()))
                    {
                        rarity_dropdown.ClearOptions();
                        Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
                        options.Add(new Dropdown.OptionData { text = "Select" });
                        if ((type_dropdown.value > 0) && (item_type > -1))
                        {
                            bool has_unique = false;
                            bool has_set = false;
                            if (UniqueList.instance.IsNullOrDestroyed()) { UniqueList.getUnique(0); }
                            if (!UniqueList.instance.IsNullOrDestroyed())
                            {
                                foreach (UniqueList.Entry unique in UniqueList.instance.uniques)
                                {
                                    if (unique.baseType == item_type)
                                    {
                                        if (unique.isSetItem) { has_set = true; }
                                        else { has_unique = true; }
                                    }
                                }
                            }
                            options.Add(new Dropdown.OptionData { text = "Base Item" });
                            if (has_unique) { options.Add(new Dropdown.OptionData { text = "Unique" }); }
                            if (has_set) { options.Add(new Dropdown.OptionData { text = "Set" }); }
                            rarity_dropdown.enabled = true;
                        }
                        else { rarity_dropdown.enabled = false; }
                        rarity_dropdown.options = options;
                        rarity_dropdown.value = 0;
                        item_rarity = -1;
                    }
                }
                public static void SelectRarity()
                {
                    if (Type_Initialized)
                    {
                        int index = rarity_dropdown.value;
                        if (index < rarity_dropdown.options.Count)
                        {
                            string rarity_str = rarity_dropdown.options[index].text;
                            item_rarity = -1;
                            if (rarity_str == "Base Item") { item_rarity = 0; }
                            else if (rarity_str == "Unique") { item_rarity = 7; }
                            else if (rarity_str == "Set") { item_rarity = 8; }
                            UpdateItems();
                            //UpdateUI();
                        }
                    }
                    UpdateUI();
                }
                public static void UpdateItems()
                {
                    if ((enable) && (LastEpoch_Hud.Scenes.IsGameScene()) &&
                        (!Refs_Manager.item_list.IsNullOrDestroyed()) &&
                        (Type_Initialized) &&
                        //(!forcedrop_type_dropdown.IsNullOrDestroyed()) &&
                        //(!forcedrop_rarity_dropdown.IsNullOrDestroyed()) &&
                        (!items_dropdown.IsNullOrDestroyed()))
                    {
                        //Main.logger_instance.Msg("Update Items : Type = " + item_type + ", Rarity = " + item_rarity);
                        items_dropdown.ClearOptions();

                        Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
                        options.Add(new Dropdown.OptionData { text = "Select" });
                        if ((item_type > -1) && (item_rarity > -1))
                        {
                            if (item_rarity == 0)
                            {
                                bool type_found = false;
                                foreach (ItemList.BaseEquipmentItem item_t in ItemList.get().EquippableItems)
                                {
                                    if (item_t.baseTypeID == item_type)
                                    {
                                        foreach (ItemList.EquipmentItem item in item_t.subItems)
                                        {
                                            string name = item.displayName;
                                            if (name == "") { name = item.name; }
                                            options.Add(new Dropdown.OptionData { text = name });
                                        }
                                        type_found = true;
                                    }
                                }
                                if (!type_found)
                                {
                                    foreach (ItemList.BaseNonEquipmentItem item_t in ItemList.get().nonEquippableItems)
                                    {
                                        if (item_t.baseTypeID == item_type)
                                        {
                                            foreach (ItemList.NonEquipmentItem item in item_t.subItems)
                                            {
                                                string name = item.displayName;
                                                if (name == "") { name = item.name; }
                                                options.Add(new Dropdown.OptionData { text = name });
                                            }

                                            type_found = true;
                                        }
                                    }
                                }
                            }
                            else if ((item_rarity == 7) || (item_rarity == 8))
                            {
                                if (UniqueList.instance.IsNullOrDestroyed()) { UniqueList.getUnique(0); }
                                if (!UniqueList.instance.IsNullOrDestroyed())
                                {
                                    foreach (UniqueList.Entry unique in UniqueList.instance.uniques)
                                    {
                                        if ((unique.baseType == item_type) &&
                                            (((item_rarity == 7) && (!unique.isSetItem)) ||
                                            ((item_rarity == 8) && (unique.isSetItem))))
                                        {
                                            string name = unique.displayName;
                                            if ((name == "") || (name == "Pearls of the Swine") || (name == "Scales of Eterra")) { name = unique.name; } // if item's displayName is "Pearls of the Swine", use unique.name instead of unique.displayName
                                            options.Add(new Dropdown.OptionData { text = name });
                                        }
                                    }
                                }
                            }
                            items_dropdown.enabled = true;
                        }
                        else { items_dropdown.enabled = false; }
                        items_dropdown.options = options;
                        items_dropdown.value = 0;
                    }
                }
                public static void SelectItem()
                {
                    if (Type_Initialized)
                    {
                        int index = items_dropdown.value;
                        if (index < items_dropdown.options.Count)
                        {
                            string item_str = items_dropdown.options[items_dropdown.value].text;
                            //Main.logger_instance.Msg("Select : Item = " + item_str);

                            item_subtype = -1;
                            item_unique_id = 0;

                            bool item_found = false;
                            if (item_rarity == 0)
                            {
                                foreach (ItemList.BaseEquipmentItem item_t in ItemList.get().EquippableItems)
                                {
                                    if (item_t.baseTypeID == item_type)
                                    {
                                        foreach (ItemList.EquipmentItem item in item_t.subItems)
                                        {
                                            if ((item_str == item.displayName) || (item_str == item.name))
                                            {
                                                item_subtype = item.subTypeID;
                                                item_found = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (!item_found)
                                {
                                    foreach (ItemList.BaseNonEquipmentItem item_t in ItemList.get().nonEquippableItems)
                                    {
                                        if (item_t.baseTypeID == item_type)
                                        {
                                            foreach (ItemList.NonEquipmentItem item in item_t.subItems)
                                            {
                                                if ((item_str == item.displayName) || (item_str == item.name))
                                                {
                                                    item_subtype = item.subTypeID;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if ((item_rarity == 7) || (item_rarity == 8))
                            {
                                if (UniqueList.instance.IsNullOrDestroyed()) { UniqueList.getUnique(0); }
                                if (!UniqueList.instance.IsNullOrDestroyed())
                                {
                                    foreach (UniqueList.Entry unique in UniqueList.instance.uniques)
                                    {
                                        if ((item_str == unique.displayName) || (item_str == unique.name))
                                        {
                                            item_subtype = unique.subTypes[0]; //need to be fix here
                                            item_unique_id = unique.uniqueID;
                                            item_legendary_type = unique.legendaryType;
                                            break;
                                        }
                                    }
                                }
                            }
                            UpdateUI();
                        }
                    }
                }
                public static void EnableImplicits()
                {
                    int index = implicits_dropdown.value;
                    if (index < implicits_dropdown.options.Count)
                    {
                        if (index == 1) { implicits_roll = true; }
                        else { implicits_roll = false; }
                    }
                }
                public static void SetImplicit_0(float f)
                {
                    int result = System.Convert.ToInt32((implicit_0_slider.value / 255) * 100);
                    implicit_0_Text.text = result.ToString() + " %";
                }
                public static void SetImplicit_1(float f)
                {
                    int result = System.Convert.ToInt32((implicit_1_slider.value / 255) * 100);
                    implicit_1_Text.text = result.ToString() + " %";
                }
                public static void SetImplicit_2(float f)
                {
                    int result = System.Convert.ToInt32((implicit_2_slider.value / 255) * 100);
                    implicit_2_Text.text = result.ToString() + " %";
                }
                public static void EnableForginPotencial()
                {
                    int index = forgin_potencial_dropdown.value;
                    if (index < forgin_potencial_dropdown.options.Count)
                    {
                        if (index == 1) { forgin_potencial_roll = true; }
                        else { forgin_potencial_roll = false; }
                    }
                }
                public static void SetForginPotencial(float f)
                {
                    forgin_potencial_text.text = System.Convert.ToInt32(forgin_potencial_slider.value).ToString();
                }
                public static void EnableSeal()
                {
                    int index = seal_dropdown.value;
                    if (index < seal_dropdown.options.Count)
                    {
                        if (index == 1) { seal_roll = true; }
                        else { seal_roll = false; }
                    }
                }
                public static void SelectSeal()
                {
                    SetShardsView(0, true);
                }
                public static void SetSealTier(float f)
                {
                    int t = System.Convert.ToInt32(seal_tier_slider.value) + 1;
                    seal_tier_text.text = t.ToString();
                }
                public static void SetSealValue(float f)
                {
                    int result = System.Convert.ToInt32((seal_value_slider.value / 255) * 100);
                    seal_value_text.text = result.ToString() + " %";
                }
                public static void EnableAffixs()
                {
                    int index = affixs_dropdown.value;
                    if (index < affixs_dropdown.options.Count)
                    {
                        if (index == 1) { affixs_roll = true; }
                        else { affixs_roll = false; }
                    }
                }
                public static void SelectAffix_0()
                {
                    SetShardsView(0, false);
                }
                public static void SetAffix_0_Tier(float f)
                {
                    int t = System.Convert.ToInt32(affix_0_tier_slider.value) + 1;
                    affix_0_tier_text.text = t.ToString();
                }
                private static void Set_Affix_0_RandomRoll_Enable(bool enable)
                {
                    affix_0_value_slider.interactable = !affix_0_random_toggle.isOn;
                    if (affix_0_random_toggle.isOn) { affix_0_value_text.text = "Random"; }
                    else
                    {
                        int result = System.Convert.ToInt32((affix_0_value_slider.value / 255) * 100);
                        affix_0_value_text.text = result.ToString() + " %";
                    }
                }
                public static void SetAffix_0_Value(float f)
                {
                    int result = System.Convert.ToInt32((affix_0_value_slider.value / 255) * 100);
                    affix_0_value_text.text = result.ToString() + " %";
                }
                public static void SelectAffix_1()
                {
                    SetShardsView(1, false);
                }
                public static void SetAffix_1_Tier(float f)
                {
                    int t = System.Convert.ToInt32(affix_1_tier_slider.value) + 1;
                    affix_1_tier_text.text = t.ToString();
                }
                private static void Set_Affix_1_RandomRoll_Enable(bool enable)
                {
                    affix_1_value_slider.interactable = !affix_1_random_toggle.isOn;
                    if (affix_1_random_toggle.isOn) { affix_1_value_text.text = "Random"; }
                    else
                    {
                        int result = System.Convert.ToInt32((affix_1_value_slider.value / 255) * 100);
                        affix_1_value_text.text = result.ToString() + " %";
                    }
                }
                public static void SetAffix_1_Value(float f)
                {
                    int result = System.Convert.ToInt32((affix_1_value_slider.value / 255) * 100);
                    affix_1_value_text.text = result.ToString() + " %";
                }
                public static void SelectAffix_2()
                {
                    SetShardsView(2, false);
                }
                public static void SetAffix_2_Tier(float f)
                {
                    int t = System.Convert.ToInt32(affix_2_tier_slider.value) + 1;
                    affix_2_tier_text.text = t.ToString();
                }
                private static void Set_Affix_2_RandomRoll_Enable(bool enable)
                {
                    affix_2_value_slider.interactable = !affix_2_random_toggle.isOn;
                    if (affix_2_random_toggle.isOn) { affix_2_value_text.text = "Random"; }
                    else
                    {
                        int result = System.Convert.ToInt32((affix_2_value_slider.value / 255) * 100);
                        affix_2_value_text.text = result.ToString() + " %";
                    }
                }
                public static void SetAffix_2_Value(float f)
                {
                    int result = System.Convert.ToInt32((affix_2_value_slider.value / 255) * 100);
                    affix_2_value_text.text = result.ToString() + " %";
                }
                public static void SelectAffix_3()
                {
                    SetShardsView(3, false);
                }
                public static void SetAffix_3_Tier(float f)
                {
                    int t = System.Convert.ToInt32(affix_3_tier_slider.value) + 1;
                    affix_3_tier_text.text = t.ToString();
                }
                private static void Set_Affix_3_RandomRoll_Enable(bool enable)
                {
                    affix_3_value_slider.interactable = !affix_3_random_toggle.isOn;
                    if (affix_3_random_toggle.isOn) { affix_3_value_text.text = "Random"; }
                    else
                    {
                        int result = System.Convert.ToInt32((affix_3_value_slider.value / 255) * 100);
                        affix_3_value_text.text = result.ToString() + " %";
                    }
                }
                public static void SetAffix_3_Value(float f)
                {
                    int result = System.Convert.ToInt32((affix_3_value_slider.value / 255) * 100);
                    affix_3_value_text.text = result.ToString() + " %";
                }
                public static void SelectAffix_4()
                {
                    SetShardsView(4, false);
                }
                public static void SetAffix_4_Tier(float f)
                {
                    int t = System.Convert.ToInt32(affix_4_tier_slider.value) + 1;
                    affix_4_tier_text.text = t.ToString();
                }
                private static void Set_Affix_4_RandomRoll_Enable(bool enable)
                {
                    affix_4_value_slider.interactable = !affix_4_random_toggle.isOn;
                    if (affix_4_random_toggle.isOn) { affix_4_value_text.text = "Random"; }
                    else
                    {
                        int result = System.Convert.ToInt32((affix_4_value_slider.value / 255) * 100);
                        affix_4_value_text.text = result.ToString() + " %";
                    }
                }
                public static void SetAffix_4_Value(float f)
                {
                    int result = System.Convert.ToInt32((affix_4_value_slider.value / 255) * 100);
                    affix_4_value_text.text = result.ToString() + " %";
                }
                public static void SelectAffix_5()
                {
                    SetShardsView(5, false);
                }
                public static void SetAffix_5_Tier(float f)
                {
                    int t = System.Convert.ToInt32(affix_5_tier_slider.value) + 1;
                    affix_5_tier_text.text = t.ToString();
                }
                private static void Set_Affix_5_RandomRoll_Enable(bool enable)
                {
                    affix_5_value_slider.interactable = !affix_5_random_toggle.isOn;
                    if (affix_5_random_toggle.isOn) { affix_5_value_text.text = "Random"; }
                    else
                    {
                        int result = System.Convert.ToInt32((affix_5_value_slider.value / 255) * 100);
                        affix_5_value_text.text = result.ToString() + " %";
                    }
                }
                public static void SetAffix_5_Value(float f)
                {
                    int result = System.Convert.ToInt32((affix_5_value_slider.value / 255) * 100);
                    affix_5_value_text.text = result.ToString() + " %";
                }                
                public static void EnableUniqueMods()
                {
                    int index = unique_mods_dropdown.value;
                    if (index < unique_mods_dropdown.options.Count)
                    {
                        if (index == 1)
                        {
                            if (item_unique_id != 444)
                            {
                                unique_mods_roll_0 = true;
                                unique_mods_roll_1 = true;
                                unique_mods_roll_2 = true;
                                unique_mods_roll_3 = true;
                                unique_mods_roll_4 = true;
                                unique_mods_roll_5 = true;
                                unique_mods_roll_6 = true;
                                unique_mods_roll_7 = true;

                                beast_evolution_0_enable = false;
                                beast_evolution_1_enable = false;
                                beast_evolution_2_enable = false;
                                beast_evolution_3_enable = false;
                                beast_evolution_4_enable = false;
                                beast_evolution_5_enable = false;

                                beast_evolution_select_0_enable = false;
                                beast_evolution_select_1_enable = false;
                                beast_evolution_select_2_enable = false;
                                beast_evolution_select_3_enable = false;
                                beast_evolution_select_4_enable = false;
                                beast_evolution_select_5_enable = false;
                            }
                            else
                            {
                                unique_mods_roll_0 = true;
                                unique_mods_roll_1 = false;
                                unique_mods_roll_2 = true;
                                unique_mods_roll_3 = false;
                                unique_mods_roll_4 = false;
                                unique_mods_roll_5 = false;
                                unique_mods_roll_6 = false;
                                unique_mods_roll_7 = false;

                                beast_evolution_0_enable = true;
                                beast_evolution_1_enable = true;
                                beast_evolution_2_enable = true;
                                beast_evolution_3_enable = true;
                                beast_evolution_4_enable = true;
                                beast_evolution_5_enable = true;

                                EnableBeastEvolution_0();
                                EnableBeastEvolution_1();
                                EnableBeastEvolution_2();
                                EnableBeastEvolution_3();
                                EnableBeastEvolution_4();
                                EnableBeastEvolution_5();
                            }
                        }
                        else
                        {
                            unique_mods_roll_0 = false;
                            unique_mods_roll_1 = false;
                            unique_mods_roll_2 = false;
                            unique_mods_roll_3 = false;
                            unique_mods_roll_4 = false;
                            unique_mods_roll_5 = false;

                            beast_evolution_0_enable = false;
                            beast_evolution_1_enable = false;
                            beast_evolution_2_enable = false;
                            beast_evolution_3_enable = false;
                            beast_evolution_4_enable = false;
                            beast_evolution_5_enable = false;

                            beast_evolution_select_0_enable = false;
                            beast_evolution_select_1_enable = false;
                            beast_evolution_select_2_enable = false;
                            beast_evolution_select_3_enable = false;
                            beast_evolution_select_4_enable = false;
                            beast_evolution_select_5_enable = false;
                        }
                    }
                }
                public static void SetUniqueMod_0(float f)
                {
                    int result = System.Convert.ToInt32((unique_mod_0_slider.value / 255) * 100);
                    unique_mod_0_Text.text = result.ToString() + " %";
                }
                public static void SetUniqueMod_1(float f)
                {
                    int result = System.Convert.ToInt32((unique_mod_1_slider.value / 255) * 100);
                    unique_mod_1_Text.text = result.ToString() + " %";
                }
                public static void SetUniqueMod_2(float f)
                {
                    int result = System.Convert.ToInt32((unique_mod_2_slider.value / 255) * 100);
                    unique_mod_2_Text.text = result.ToString() + " %";
                }
                public static void SetUniqueMod_3(float f)
                {
                    int result = System.Convert.ToInt32((unique_mod_3_slider.value / 255) * 100);
                    unique_mod_3_Text.text = result.ToString() + " %";
                }
                public static void SetUniqueMod_4(float f)
                {
                    int result = System.Convert.ToInt32((unique_mod_4_slider.value / 255) * 100);
                    unique_mod_4_Text.text = result.ToString() + " %";
                }
                public static void SetUniqueMod_5(float f)
                {
                    int result = System.Convert.ToInt32((unique_mod_5_slider.value / 255) * 100);
                    unique_mod_5_Text.text = result.ToString() + " %";
                }
                public static void SetUniqueMod_6(float f)
                {
                    int result = System.Convert.ToInt32((unique_mod_6_slider.value / 255) * 100);
                    unique_mod_6_Text.text = result.ToString() + " %";
                }
                public static void SetUniqueMod_7(float f)
                {
                    int result = System.Convert.ToInt32((unique_mod_7_slider.value / 255) * 100);
                    unique_mod_7_Text.text = result.ToString() + " %";
                }

                public static void SetNbEvolution(float f)
                {
                    int result = System.Convert.ToInt32(nb_evolution_slider.value);
                    nb_evolution_Text.text = result.ToString();
                }
                public static void EnableBeastEvolution_0()
                {
                    int index = beast_evolution_0_dropdown.value;
                    if (index < beast_evolution_0_dropdown.options.Count)
                    {
                        if (index == 1) { beast_evolution_0_enable = true; }
                        else { beast_evolution_0_enable = false; }
                    }
                }
                public static void SelectBeastEvolution_0()
                {
                    int index = beast_evolution_0_select_dropdown.value;
                    if (index < beast_evolution_0_select_dropdown.options.Count)
                    {
                        beast_evolution_0_select = index;
                    }
                }
                public static void EnableBeastEvolution_1()
                {
                    int index = beast_evolution_1_dropdown.value;
                    if (index < beast_evolution_1_dropdown.options.Count)
                    {
                        if (index == 1) { beast_evolution_1_enable = true; }
                        else { beast_evolution_1_enable = false; }
                    }
                }
                public static void SelectBeastEvolution_1()
                {
                    int index = beast_evolution_1_select_dropdown.value;
                    if (index < beast_evolution_1_select_dropdown.options.Count)
                    {
                        beast_evolution_1_select = index;
                    }
                }
                public static void EnableBeastEvolution_2()
                {
                    int index = beast_evolution_2_dropdown.value;
                    if (index < beast_evolution_2_dropdown.options.Count)
                    {
                        if (index == 1) { beast_evolution_2_enable = true; }
                        else { beast_evolution_2_enable = false; }
                    }
                }
                public static void SelectBeastEvolution_2()
                {
                    int index = beast_evolution_2_select_dropdown.value;
                    if (index < beast_evolution_2_select_dropdown.options.Count)
                    {
                        beast_evolution_2_select = index;
                    }
                }
                public static void EnableBeastEvolution_3()
                {
                    int index = beast_evolution_3_dropdown.value;
                    if (index < beast_evolution_3_dropdown.options.Count)
                    {
                        if (index == 1) { beast_evolution_3_enable = true; }
                        else { beast_evolution_3_enable = false; }
                    }
                }
                public static void SelectBeastEvolution_3()
                {
                    int index = beast_evolution_3_select_dropdown.value;
                    if (index < beast_evolution_3_select_dropdown.options.Count)
                    {
                        beast_evolution_3_select = index;
                    }
                }
                public static void EnableBeastEvolution_4()
                {
                    int index = beast_evolution_4_dropdown.value;
                    if (index < beast_evolution_4_dropdown.options.Count)
                    {
                        if (index == 1) { beast_evolution_4_enable = true; }
                        else { beast_evolution_4_enable = false; }
                    }
                }
                public static void SelectBeastEvolution_4()
                {
                    int index = beast_evolution_4_select_dropdown.value;
                    if (index < beast_evolution_4_select_dropdown.options.Count)
                    {
                        beast_evolution_4_select = index;
                    }
                }
                public static void EnableBeastEvolution_5()
                {
                    int index = beast_evolution_5_dropdown.value;
                    if (index < beast_evolution_5_dropdown.options.Count)
                    {
                        if (index == 1) { beast_evolution_5_enable = true; }
                        else { beast_evolution_5_enable = false; }
                    }
                }
                public static void SelectBeastEvolution_5()
                {
                    int index = beast_evolution_5_select_dropdown.value;
                    if (index < beast_evolution_5_select_dropdown.options.Count)
                    {
                        beast_evolution_5_select = index;
                    }
                }
                public static void EnableBeastEvolution_6()
                {
                    int index = beast_evolution_6_dropdown.value;
                    if (index < beast_evolution_6_dropdown.options.Count)
                    {
                        if (index == 1) { beast_evolution_6_enable = true; }
                        else { beast_evolution_6_enable = false; }
                    }
                }
                public static void SelectBeastEvolution_6()
                {
                    int index = beast_evolution_6_select_dropdown.value;
                    if (index < beast_evolution_6_select_dropdown.options.Count)
                    {
                        beast_evolution_6_select = index;
                    }
                }

                public static void EnableLegendaryPotencial()
                {
                    int index = legenday_potencial_dropdown.value;
                    if (index < legenday_potencial_dropdown.options.Count)
                    {
                        if (index == 1) { legenday_potencial_roll = true; }
                        else { legenday_potencial_roll = false; }
                    }
                }
                public static void SetLegendayPotencial(float f)
                {
                    legenday_potencial_Text.text = System.Convert.ToInt32(legenday_potencial_slider.value).ToString();
                }
                public static void EnableWeaverWill()
                {
                    int index = weaver_will_dropdown.value;
                    if (index < weaver_will_dropdown.options.Count)
                    {
                        if (index == 1) { weaver_will_roll = true; }
                        else { weaver_will_roll = false; }
                    }
                }
                public static void SetWeaverWill(float f)
                {
                    weaver_will_Text.text = System.Convert.ToInt32(weaver_will_slider.value).ToString();
                }

                public static void SetShardsView(int affix_number, bool seal)
                {
                    shard_seal = seal;
                    shard_number = affix_number;
                    if (!shard_initialized) { InitializeShardsView(); }                    
                }
                public static void InitializeShardsView()
                {
                    RemoveShardsInView();
                    bool filter_by_type = false;                    
                    AffixList.AffixType wanted_type = AffixList.AffixType.PREFIX;
                    if (shards_filter_type.value > 0)
                    {
                        filter_by_type = true;
                        if (shards_filter_type.value == 1) { wanted_type = AffixList.AffixType.PREFIX; }
                        else if (shards_filter_type.value == 2) { wanted_type = AffixList.AffixType.SUFFIX; }
                    }
                    bool filter_by_class = false;
                    AffixList.ClassSpecificity wanted_class = AffixList.ClassSpecificity.None;
                    if (shards_filter_class.value > 0)
                    {
                        filter_by_class = true;
                        if (shards_filter_class.value == 1) { wanted_class = AffixList.ClassSpecificity.NonSpecific; }
                        else if (shards_filter_class.value == 2) { wanted_class = AffixList.ClassSpecificity.Primalist; }
                        else if (shards_filter_class.value == 3) { wanted_class = AffixList.ClassSpecificity.Mage; }
                        else if (shards_filter_class.value == 4) { wanted_class = AffixList.ClassSpecificity.Sentinel; }
                        else if (shards_filter_class.value == 5) { wanted_class = AffixList.ClassSpecificity.Acolyte; }
                        else if (shards_filter_class.value == 6) { wanted_class = AffixList.ClassSpecificity.Rogue; }                        
                    }
                    bool filter_by_name = false;
                    string wanted_name = "";
                    if (shards_filter_name.text != "")
                    {
                        filter_by_name = true;
                        wanted_name = shards_filter_name.text;
                    }
                    bool item_idol = false;
                    if (((item_type > 24) && (item_type < 34)) || (item_type == 41)) { item_idol = true; }
                    foreach (AffixList.SingleAffix affix in AffixList.instance.singleAffixes)
                    {
                        bool affix_idol = false;
                        if (affix.affixName.Contains("Idol ")) { affix_idol = true; }

                        if (((item_idol && affix_idol) || (!item_idol && !affix_idol)) &&
                            (((filter_by_name) && (affix.affixName.ToLower().Contains(wanted_name.ToLower()))) || (!filter_by_name)) &&
                            (((filter_by_type) && (affix.type == wanted_type)) || (!filter_by_type)) &&
                            (((filter_by_class) && (affix.classSpecificity == wanted_class)) || (!filter_by_class))
                            )
                        {
                            bool naturally = false;
                            if (affix.canRollOn.Contains(item_equipmenttype)) { naturally = true; }
                            bool corrupted = false;
                            if (affix.displayCategory == AffixList.AffixDisplayCategory.CORRUPTED) { corrupted = true; }
                            AddShardInView(affix.affixId, affix.affixName, affix.type, affix_idol, naturally, corrupted);
                        }
                    }
                    foreach (AffixList.MultiAffix affix in AffixList.instance.multiAffixes)
                    {
                        bool affix_idol = false;
                        if (affix.affixName.Contains("Idol ")) { affix_idol = true; }

                        if (((item_idol && affix_idol) || (!item_idol && !affix_idol)) &&
                            (((filter_by_name) && (affix.affixName.ToLower().Contains(wanted_name.ToLower()))) || (!filter_by_name)) &&
                            (((filter_by_type) && (affix.type == wanted_type)) || (!filter_by_type)) &&
                            (((filter_by_class) && (affix.classSpecificity == wanted_class)) || (!filter_by_class))
                            )
                        {
                            bool naturally = false;
                            if (affix.canRollOn.Contains(item_equipmenttype)) { naturally = true; }
                            bool corrupted = false;
                            if (affix.displayCategory == AffixList.AffixDisplayCategory.CORRUPTED) { corrupted = true; }
                            AddShardInView(affix.affixId, affix.affixName, affix.type, affix_idol, naturally, corrupted);
                        }
                    }
                    shard_initialized = true;
                }
                public static void RemoveShardsInView()
                {
                    foreach (GameObject go in Functions.GetAllChild(center_content))
                    {
                        Destroy(go);
                    }                        
                }
                public static void AddShardInView(int id, string name, AffixList.AffixType affix_type, bool idol, bool naturally, bool corrupted)
                {
                    GameObject g = Object.Instantiate(shard_prefab, Vector3.zero, Quaternion.identity);
                    g.transform.SetParent(center_content.transform);
                    GameObject shard_btn_object = Functions.GetChild(g, "shard_btn");
                    Button shard_btn = shard_btn_object.GetComponent<Button>();
                    shard_btn.name = shard_btn_name + id;
                    UnityEngine.Color color_id = color_red;
                    if (naturally) { color_id = color_green; }
                    UnityEngine.Color color_name = color_blue;
                    if (!idol)
                    {
                        if (corrupted) { color_name = color_green; }
                        else if (affix_type == AffixList.AffixType.PREFIX) { color_name = color_yellow; }
                        else if (affix_type == AffixList.AffixType.SPECIAL) { color_name = color_green; }
                    }
                    GameObject shard_id_object = Functions.GetChild(shard_btn_object, "shard_id");
                    if (!shard_id_object.IsNullOrDestroyed())
                    {
                        GameObject text = Functions.GetChild(shard_id_object, "Text");
                        if (!text.IsNullOrDestroyed())
                        {
                            Text shard_id = text.GetComponent<Text>();
                            shard_id.text = id.ToString();
                            shard_id.color = color_id;
                        }
                    }
                    GameObject shard_name_object = Functions.GetChild(shard_btn_object, "shard_name");
                    if (!shard_name_object.IsNullOrDestroyed())
                    {
                        GameObject text = Functions.GetChild(shard_name_object, "Text");
                        if (!text.IsNullOrDestroyed())
                        {
                            Text shard_name = text.GetComponent<Text>();
                            shard_name.text = name.ToString();
                            shard_name.color = color_name;
                        }
                    }
                }
                public static void SelectShard(int id, string name)
                {
                    if (shard_seal) { seal_id = id; seal_name = name; }
                    else
                    {
                        if (shard_number == 0) { affix_0_id = id; affix_0_name = name; }
                        else if (shard_number == 1) { affix_1_id = id; affix_1_name = name; }
                        else if (shard_number == 2) { affix_2_id = id; affix_2_name = name; }
                        else if (shard_number == 3) { affix_3_id = id; affix_3_name = name; }
                        else if (shard_number == 4) { affix_4_id = id; affix_4_name = name; }
                        else if (shard_number == 5) { affix_5_id = id; affix_5_name = name; }
                    }
                }
                public static ItemAffix MakeAffix(int id, byte tier, byte roll, bool seal)
                {
                    //ItemAffix a = new ItemAffix();
                    //a.IsSealedCorrupted

                    ItemAffix new_affix = null;
                    if (id > -1)
                    {
                        bool found = false;
                        foreach (AffixList.SingleAffix affix in AffixList.instance.singleAffixes)
                        {
                            if (id == affix.affixId)
                            {
                                new_affix = new ItemAffix
                                {
                                    affixId = (ushort)affix.affixId,
                                    affixName = affix.affixName,
                                    affixTitle = affix.affixTitle,
                                    affixType = affix.type,
                                    //isSealedAffix = seal,
                                    affixTier = tier,
                                    affixRoll = roll                                    
                                };
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            foreach (AffixList.MultiAffix affix in AffixList.instance.multiAffixes)
                            {
                                if (id == affix.affixId)
                                {
                                    new_affix = new ItemAffix
                                    {
                                        affixId = (ushort)affix.affixId,
                                        affixName = affix.affixName,
                                        affixTitle = affix.affixTitle,
                                        affixType = affix.type,
                                        //isSealedAffix = seal,
                                        affixTier = tier,
                                        affixRoll = roll
                                    };
                                    break;
                                }
                            }
                        }
                    }

                    return new_affix;
                }

                public static void UpdateUI()
                {
                    implicits_enable = false;
                    seal_enable = false;
                    forgin_potencial_enable = false;
                    affixs_enable = false;
                    unique_mods_enable = false;
                    legenday_potencial_enable = false;
                    weaver_will_enable = false;
                    corrupted_enable = false;
                    quantity_enable = false;
                    btn_enable = false;

                    if ((type_dropdown.value > 0) && (rarity_dropdown.value > 0) && (items_dropdown.value > 0))
                    {
                        implicits_enable = true;
                        if (item_type < 100) { seal_enable = true; affixs_enable = true; }
                        if ((item_type < 100) && (item_rarity < 7)) { forgin_potencial_enable = true; }
                        if (item_rarity > 6)
                        {
                            unique_mods_enable = true;
                            if (item_legendary_type == UniqueList.LegendaryType.LegendaryPotential) { legenday_potencial_enable = true; }
                            weaver_will_enable = !legenday_potencial_enable;
                        }
                        corrupted_enable = true;
                        quantity_enable = true;
                        btn_enable = true;
                    }
                    else //Reset all dropdown
                    {
                        implicits_dropdown.value = 0;
                        forgin_potencial_dropdown.value = 0;
                        seal_dropdown.value = 0;
                        affixs_dropdown.value = 0;
                        unique_mods_dropdown.value = 0;
                        legenday_potencial_dropdown.value = 0;
                        weaver_will_dropdown.value = 0;
                    }
                }
                public static void Drop()
                {
                    if ((btn_enable) && (!Refs_Manager.ground_item_manager.IsNullOrDestroyed()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
                    {
                        for (int i = 0; i < forcedrop_quantity_slider.value; i++)
                        {
                            //Rarity
                            byte ra = (byte)item_rarity;

                            //Forgin potencial
                            byte fg = 0;
                            if ((item_type < 100) && (ra < 7))
                            {                                
                                if (forgin_potencial_roll) { fg = (byte)forgin_potencial_slider.value; }
                                else { fg = (byte)Random.RandomRange(0f, 255f); } //Random
                            }

                            //Affixes
                            bool sa = false; //Seal
                            byte an = 0; //Affix numbers
                            List<ItemAffix> af = new List<ItemAffix>(); //Affixes
                            if (seal_roll)
                            {
                                if (seal_id > -1)
                                {
                                    ItemAffix affix = MakeAffix(seal_id, (byte)seal_tier_slider.value, (byte)seal_value_slider.value, true);
                                    if (!affix.IsNullOrDestroyed())
                                    {
                                        sa = true;
                                        an = 1; //Set affix number
                                        af.Add(affix);
                                    }
                                    else { Main.logger_instance.Error("Seal is null"); }
                                }                                
                            }
                            if (affixs_roll)
                            {
                                System.Collections.Generic.List<ItemAffix> new_affixes = new System.Collections.Generic.List<ItemAffix>();
                                if (affix_0_id > -1)
                                {
                                    if (affix_0_random_toggle.isOn)
                                    {
                                        new_affixes.Add(MakeAffix(affix_0_id, (byte)affix_0_tier_slider.value, (byte)Random.Range(0f, 255f), false));
                                    }
                                    else
                                    {
                                        new_affixes.Add(MakeAffix(affix_0_id, (byte)affix_0_tier_slider.value, (byte)affix_0_value_slider.value, false));
                                    }
                                }
                                if (affix_1_id > -1)
                                {
                                    if (affix_1_random_toggle.isOn)
                                    {
                                        new_affixes.Add(MakeAffix(affix_1_id, (byte)affix_1_tier_slider.value, (byte)Random.Range(0f, 255f), false));
                                    }
                                    else
                                    {
                                        new_affixes.Add(MakeAffix(affix_1_id, (byte)affix_1_tier_slider.value, (byte)affix_1_value_slider.value, false));
                                    }                                        
                                }
                                if (affix_2_id > -1)
                                {
                                    if (affix_2_random_toggle.isOn)
                                    {
                                        new_affixes.Add(MakeAffix(affix_2_id, (byte)affix_2_tier_slider.value, (byte)Random.Range(0f, 255f), false));
                                    }
                                    else
                                    {
                                        new_affixes.Add(MakeAffix(affix_2_id, (byte)affix_2_tier_slider.value, (byte)affix_2_value_slider.value, false));
                                    }                                        
                                }
                                if (affix_3_id > -1)
                                {
                                    if (affix_3_random_toggle.isOn)
                                    {
                                        new_affixes.Add(MakeAffix(affix_3_id, (byte)affix_3_tier_slider.value, (byte)Random.Range(0f, 255f), false));
                                    }
                                    else
                                    {
                                        new_affixes.Add(MakeAffix(affix_3_id, (byte)affix_3_tier_slider.value, (byte)affix_3_value_slider.value, false));
                                    }                                        
                                }
                                if (affix_4_id > -1)
                                {
                                    if (affix_4_random_toggle.isOn)
                                    {
                                        new_affixes.Add(MakeAffix(affix_4_id, (byte)affix_4_tier_slider.value, (byte)Random.Range(0f, 255f), false));
                                    }
                                    else
                                    {
                                        new_affixes.Add(MakeAffix(affix_4_id, (byte)affix_4_tier_slider.value, (byte)affix_4_value_slider.value, false));
                                    }                                        
                                }
                                if (affix_5_id > -1)
                                {
                                    if (affix_5_random_toggle.isOn)
                                    {
                                        new_affixes.Add(MakeAffix(affix_5_id, (byte)affix_5_tier_slider.value, (byte)Random.Range(0f, 255f), false));
                                    }
                                    else
                                    {
                                        new_affixes.Add(MakeAffix(affix_5_id, (byte)affix_5_tier_slider.value, (byte)affix_5_value_slider.value, false));
                                    }                                        
                                }
                                
                                byte new_count = 0;
                                foreach (ItemAffix a in new_affixes)
                                {
                                    af.Add(a);
                                    new_count++;
                                }
                                new_affixes.Clear();
                                an += new_count; //Set affix number
                                if (ra < 7) { ra = new_count; } //Set rarity to affix numbers for base item only
                                //else if (an > 0) { ra = 9; }//Set rarity to legendary if seal or affix
                                else if (new_count > 0) { ra = 9; } //Set rarity to legendary if affix only
                            }

                            //Unique
                            byte lp = 0; //Legendary potencial
                            byte ww = 0; //Weaver will
                            if (ra > 6)                            
                            {
                                if (item_legendary_type == UniqueList.LegendaryType.LegendaryPotential)
                                {                                    
                                    if (legenday_potencial_roll)
                                    {
                                        lp = (byte)legenday_potencial_slider.value;
                                    }
                                    else { lp = (byte)Random.RandomRange(0f, 4f); } //Random
                                }
                                else
                                {                                    
                                    if (weaver_will_roll)
                                    {
                                        ww = (byte)weaver_will_slider.value;
                                    }
                                    else { ww = (byte)Random.RandomRange(0f, 28f); } //Random
                                }                                
                            }

                            //Create item
                            ItemDataUnpacked item = new ItemDataUnpacked
                            {
                                LvlReq = 0,
                                classReq = ItemList.ClassRequirement.Any,
                                itemType = (byte)item_type,
                                subType = (ushort)item_subtype,
                                rarity = (byte)ra,                                
                                forgingPotential = fg,
                                hasSealedRegularAffix = sa,
                                sockets = (byte)an,
                                affixes = af,
                                uniqueID = (ushort)item_unique_id,
                                legendaryPotential = lp,
                                weaversWill = ww,
                                corrupted = toggle_corrupted.isOn
                            };

                            //Set Implicits
                            if (implicits_roll)
                            {
                                item.implicitRolls[0] = (byte)implicit_0_slider.value;
                                item.implicitRolls[1] = (byte)implicit_1_slider.value;
                                item.implicitRolls[2] = (byte)implicit_2_slider.value;
                            }
                            else //Random
                            {
                                for (int k = 0; k < item.implicitRolls.Count; k++)
                                {
                                    item.implicitRolls[k] = (byte)Random.RandomRange(0f, 255f);
                                }
                            }

                            //Set Unique mods
                            if (item.isUniqueSetOrLegendary())
                            {
                                if (item_unique_id != 444)
                                {
                                    if (unique_mods_dropdown.value == 1)
                                    {
                                        item.uniqueRolls[0] = (byte)unique_mod_0_slider.value;
                                        item.uniqueRolls[1] = (byte)unique_mod_1_slider.value;
                                        item.uniqueRolls[2] = (byte)unique_mod_2_slider.value;
                                        item.uniqueRolls[3] = (byte)unique_mod_3_slider.value;
                                        item.uniqueRolls[4] = (byte)unique_mod_4_slider.value;
                                        item.uniqueRolls[5] = (byte)unique_mod_5_slider.value;
                                        item.uniqueRolls[6] = (byte)unique_mod_6_slider.value;
                                        item.uniqueRolls[7] = (byte)unique_mod_7_slider.value;
                                    }
                                    else
                                    {
                                        for (int k = 0; k < item.uniqueRolls.Count; k++)
                                        {
                                            item.uniqueRolls[k] = (byte)Random.RandomRange(0f, 255f);
                                        }
                                    }
                                }
                                else
                                {
                                    if (unique_mods_dropdown.value == 1) { item.uniqueRolls[0] = (byte)unique_mod_0_slider.value; }
                                    else { item.uniqueRolls[0] = (byte)Random.RandomRange(0f, 255f); }
                                    if (beast_evolution_0_dropdown.value == 0) { item.uniqueRolls[1] = (byte)Random.RandomRangeInt(1, evo_count); }
                                    else { item.uniqueRolls[1] = (byte)beast_evolution_0_select_dropdown.value; }                                    
                                    if (beast_evolution_1_dropdown.value == 0) { item.uniqueRolls[2] = (byte)Random.RandomRangeInt(1, evo_count); }
                                    else { item.uniqueRolls[2] = (byte)beast_evolution_1_select_dropdown.value; }                                    
                                    if (beast_evolution_2_dropdown.value == 0) { item.uniqueRolls[3] = (byte)Random.RandomRangeInt(1, evo_count); }
                                    else { item.uniqueRolls[3] = (byte)beast_evolution_2_select_dropdown.value; }                                    
                                    if (beast_evolution_3_dropdown.value == 0) { item.uniqueRolls[4] = (byte)Random.RandomRangeInt(1, evo_count); }
                                    else { item.uniqueRolls[4] = (byte)beast_evolution_3_select_dropdown.value; }                                    
                                    if (beast_evolution_4_dropdown.value == 0) { item.uniqueRolls[5] = (byte)Random.RandomRangeInt(1, evo_count); }
                                    else { item.uniqueRolls[5] = (byte)beast_evolution_4_select_dropdown.value; }                                    
                                    if (beast_evolution_5_dropdown.value == 0) { item.uniqueRolls[6] = (byte)Random.RandomRangeInt(1, evo_count); }
                                    else { item.uniqueRolls[6] = (byte)beast_evolution_5_select_dropdown.value; }
                                    if (beast_evolution_6_dropdown.value == 0) { item.uniqueRolls[7] = (byte)Random.RandomRangeInt(1, evo_count); }
                                    else { item.uniqueRolls[7] = (byte)beast_evolution_6_select_dropdown.value; }

                                    int unique_roll = item.uniqueRolls[0];
                                    int m = 0;
                                    if (unique_roll > 8) { m = unique_roll / 8; }
                                    int nb_evo = unique_roll - (8 * m);
                                    int nb_evolutions = (int)nb_evolution_slider.value;
                                    if (nb_evo != nb_evolutions) { item.uniqueRolls[0] = (byte)((m * 8) + nb_evolutions); }
                                }
                            }
                            item.RefreshIDAndValues(); //Refresh item for implicits and unique mods
                            
                            Refs_Manager.ground_item_manager.dropItemForPlayer(Refs_Manager.player_actor, item.TryCast<ItemData>(), Refs_Manager.player_actor.position(), false);
                        }
                    }
                }
            }
            public class NewItems
            {
                public static GameObject content_obj = null;
                public static bool enable = false;

                //Headhunter
                public static Text Headhunter_MinGeneratedBuff_text = null;
                public static Slider Headhunter_MinGeneratedBuff_slider = null;
                public static readonly System.Action<float> Headhunter_MinGeneratedBuff_slider_Action = new System.Action<float>(Set_Headhunter_MinGeneratedBuff);
                public static void Set_Headhunter_MinGeneratedBuff(float f)
                {
                    if ((!Headhunter_MinGeneratedBuff_slider.IsNullOrDestroyed()) && (!Headhunter_MinGeneratedBuff_text.IsNullOrDestroyed()))
                    {
                        float result = Headhunter_MinGeneratedBuff_slider.value;
                        Save_Manager.instance.data.NewItems.Headhunter.MinGenerated = (int)result;
                        if (Save_Manager.instance.data.NewItems.Headhunter.MinGenerated > Save_Manager.instance.data.NewItems.Headhunter.MaxGenerated)
                        {
                            Headhunter_MaxGeneratedBuff_slider.value = Save_Manager.instance.data.NewItems.Headhunter.MinGenerated;
                        }                        
                        Headhunter_MinGeneratedBuff_text.text = result.ToString();
                    }
                }
                public static Text Headhunter_MaxGeneratedBuff_text = null;
                public static Slider Headhunter_MaxGeneratedBuff_slider = null;
                public static readonly System.Action<float> Headhunter_MaxGeneratedBuff_slider_Action = new System.Action<float>(Set_Headhunter_MaxGeneratedBuff);
                public static void Set_Headhunter_MaxGeneratedBuff(float f)
                {
                    if ((!Headhunter_MaxGeneratedBuff_slider.IsNullOrDestroyed()) && (!Headhunter_MaxGeneratedBuff_text.IsNullOrDestroyed()))
                    {
                        float result = Headhunter_MaxGeneratedBuff_slider.value;
                        Save_Manager.instance.data.NewItems.Headhunter.MaxGenerated = (int)result;
                        if (Save_Manager.instance.data.NewItems.Headhunter.MaxGenerated < Save_Manager.instance.data.NewItems.Headhunter.MinGenerated)
                        {
                            Headhunter_MinGeneratedBuff_slider.value = Save_Manager.instance.data.NewItems.Headhunter.MaxGenerated;
                        }
                        Headhunter_MaxGeneratedBuff_text.text = result.ToString();                    
                    }
                }
                public static Text Headhunter_BuffDuration_text = null;
                public static Slider Headhunter_BuffDuration_slider = null;
                public static readonly System.Action<float> Headhunter_BuffDuration_slider_Action = new System.Action<float>(Set_Headhunter_BuffDuration);
                public static void Set_Headhunter_BuffDuration(float f)
                {
                    if ((!Headhunter_BuffDuration_slider.IsNullOrDestroyed()) && (!Headhunter_BuffDuration_text.IsNullOrDestroyed()))
                    {
                        float result = Headhunter_BuffDuration_slider.value;
                        Save_Manager.instance.data.NewItems.Headhunter.BuffDuration = result;
                        Headhunter_BuffDuration_text.text = result.ToString() + " sec";
                    }
                }
                public static Text Headhunter_BuffStack_text = null;
                public static Slider Headhunter_BuffStack_slider = null;
                public static readonly System.Action<float> Headhunter_BuffStack_slider_Action = new System.Action<float>(Set_Headhunter_BuffStack);
                public static void Set_Headhunter_BuffStack(float f)
                {
                    if ((!Headhunter_BuffStack_slider.IsNullOrDestroyed()) && (!Headhunter_BuffStack_text.IsNullOrDestroyed()))
                    {
                        float result = Headhunter_BuffStack_slider.value;
                        Save_Manager.instance.data.NewItems.Headhunter.Stack = result;
                        Headhunter_BuffStack_text.text = result.ToString();
                    }
                }
                public static Text Headhunter_Add_text = null;
                public static Slider Headhunter_Add_slider = null;
                public static readonly System.Action<float> Headhunter_Add_slider_Action = new System.Action<float>(Set_Headhunter_Add);
                public static void Set_Headhunter_Add(float f)
                {
                    if ((!Headhunter_Add_slider.IsNullOrDestroyed()) && (!Headhunter_Add_text.IsNullOrDestroyed()))
                    {
                        float result = Headhunter_Add_slider.value;
                        Save_Manager.instance.data.NewItems.Headhunter.AddValue = result;
                        Headhunter_Add_text.text = "+ " + result.ToString();
                    }
                }
                public static Text Headhunter_Increase_text = null;
                public static Slider Headhunter_Increase_slider = null;
                public static readonly System.Action<float> Headhunter_Increase_slider_Action = new System.Action<float>(Set_Headhunter_Increase);
                public static void Set_Headhunter_Increase(float f)
                {
                    if ((!Headhunter_Increase_slider.IsNullOrDestroyed()) && (!Headhunter_Increase_text.IsNullOrDestroyed()))
                    {
                        float result = Headhunter_Increase_slider.value;
                        Save_Manager.instance.data.NewItems.Headhunter.IncreasedValue = result;
                        Headhunter_Increase_text.text = "+ " + ((int)(result * 100)).ToString() + " %";
                    }
                }
                public static Dropdown Headhunter_LegendaryType_dropdown = null;
                private static void Set_Headhunter_LegendaryType()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Headhunter_LegendaryType_dropdown.IsNullOrDestroyed()))
                    {
                        bool weaverwill = false;
                        if (Headhunter_LegendaryType_dropdown.value == 1) { weaverwill = true; }
                        Save_Manager.instance.data.NewItems.Headhunter.WeaverWill = weaverwill;
                        if (IsPauseOpen()) { Items_HeadHunter.Unique.Update_LegendaryType(); }
                    }
                }

                //Mjolnir
                public static Text Mjolnir_StrReq_text = null;
                public static Slider Mjolnir_StrReq_slider = null;
                public static readonly System.Action<float> Mjolnir_StrReq_slider_Action = new System.Action<float>(Set_Mjolnir_StrReq);
                public static void Set_Mjolnir_StrReq(float f)
                {
                    if ((!Mjolnir_StrReq_slider.IsNullOrDestroyed()) && (!Mjolnir_StrReq_text.IsNullOrDestroyed()))
                    {
                        float result = Mjolnir_StrReq_slider.value;
                        Save_Manager.instance.data.NewItems.Mjolner.StrRequirement = (int)result;
                        Mjolnir_StrReq_text.text = result.ToString();
                    }
                }
                public static Text Mjolnir_IntReq_text = null;
                public static Slider Mjolnir_IntReq_slider = null;
                public static readonly System.Action<float> Mjolnir_IntReq_slider_Action = new System.Action<float>(Set_Mjolnir_IntReq);
                public static void Set_Mjolnir_IntReq(float f)
                {
                    if ((!Mjolnir_IntReq_slider.IsNullOrDestroyed()) && (!Mjolnir_IntReq_text.IsNullOrDestroyed()))
                    {
                        float result = Mjolnir_IntReq_slider.value;
                        Save_Manager.instance.data.NewItems.Mjolner.IntRequirement = (int)result;
                        Mjolnir_IntReq_text.text = result.ToString();
                    }
                }
                public static Text Mjolnir_MinTriggerChance_text = null;
                public static Slider Mjolnir_MinTriggerChance_slider = null;
                public static readonly System.Action<float> Mjolnir_MinTriggerChance_slider_Action = new System.Action<float>(Set_Mjolnir_MinTriggerChance);
                public static void Set_Mjolnir_MinTriggerChance(float f)
                {
                    if ((!Mjolnir_MinTriggerChance_slider.IsNullOrDestroyed()) && (!Mjolnir_MinTriggerChance_text.IsNullOrDestroyed()))
                    {
                        if (Mjolnir_MinTriggerChance_slider.value > Mjolnir_MaxTriggerChance_slider.value)
                        {
                            Mjolnir_MaxTriggerChance_slider.value = Mjolnir_MinTriggerChance_slider.value;
                        }
                        float result = Mjolnir_MinTriggerChance_slider.value;
                        Save_Manager.instance.data.NewItems.Mjolner.MinTriggerChance = (result / 100);
                        Mjolnir_MinTriggerChance_text.text = result.ToString() + " %";
                    }
                }
                public static Text Mjolnir_MaxTriggerChance_text = null;
                public static Slider Mjolnir_MaxTriggerChance_slider = null;
                public static readonly System.Action<float> Mjolnir_MaxTriggerChance_slider_Action = new System.Action<float>(Set_Mjolnir_MaxTriggerChance);
                public static void Set_Mjolnir_MaxTriggerChance(float f)
                {
                    if ((!Mjolnir_MaxTriggerChance_slider.IsNullOrDestroyed()) && (!Mjolnir_MaxTriggerChance_text.IsNullOrDestroyed()))
                    {
                        if (Mjolnir_MaxTriggerChance_slider.value < Mjolnir_MinTriggerChance_slider.value)
                        {
                            Mjolnir_MinTriggerChance_slider.value = Mjolnir_MaxTriggerChance_slider.value;
                        }
                        float result = Mjolnir_MaxTriggerChance_slider.value;
                        Save_Manager.instance.data.NewItems.Mjolner.MaxTriggerChance = (result / 100);
                        Mjolnir_MaxTriggerChance_text.text = result.ToString() + " %";
                    }
                }
                public static Text Mjolnir_TriggerCooldown_text = null;
                public static Slider Mjolnir_TriggerCooldown_slider = null;
                public static readonly System.Action<float> Mjolnir_TriggerCooldown_slider_Action = new System.Action<float>(Set_Mjolnir_TriggerCooldown);
                public static void Set_Mjolnir_TriggerCooldown(float f)
                {
                    if ((!Mjolnir_TriggerCooldown_slider.IsNullOrDestroyed()) && (!Mjolnir_TriggerCooldown_text.IsNullOrDestroyed()))
                    {
                        float result = Mjolnir_TriggerCooldown_slider.value;
                        Save_Manager.instance.data.NewItems.Mjolner.SocketedCooldown = result;
                        Mjolnir_TriggerCooldown_text.text = result.ToString() + " sec";
                    }
                }
                public static Dropdown Mjolnir_Socket0_dropdown = null;
                private static void Set_Mjolnir_Socket0()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Mjolnir_Socket0_dropdown.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_0 = Mjolnir_Socket0_dropdown.options[Mjolnir_Socket0_dropdown.value].text;
                        if (IsPauseOpen()) { Items_Mjolner.Trigger.Initialize_SocketedSkills(); }
                    }
                }
                public static Dropdown Mjolnir_Socket1_dropdown = null;
                private static void Set_Mjolnir_Socket1()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Mjolnir_Socket1_dropdown.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_1 = Mjolnir_Socket1_dropdown.options[Mjolnir_Socket1_dropdown.value].text;
                        if (IsPauseOpen()) { Items_Mjolner.Trigger.Initialize_SocketedSkills(); }
                    }
                }
                public static Dropdown Mjolnir_Socket2_dropdown = null;
                private static void Set_Mjolnir_Socket2()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Mjolnir_Socket2_dropdown.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_2 = Mjolnir_Socket2_dropdown.options[Mjolnir_Socket2_dropdown.value].text;
                        if (IsPauseOpen()) { Items_Mjolner.Trigger.Initialize_SocketedSkills(); }
                    }
                }
                public static Dropdown Mjolnir_LegendaryType_dropdown = null;
                private static void Set_Mjolnir_LegendaryType()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Mjolnir_LegendaryType_dropdown.IsNullOrDestroyed()))
                    {
                        bool weaverwill = false;
                        if (Mjolnir_LegendaryType_dropdown.value == 1) { weaverwill = true; }
                        Save_Manager.instance.data.NewItems.Mjolner.WeaverWill = weaverwill;
                        if (IsPauseOpen()) { Items_Mjolner.Unique.Update_LegendaryType(weaverwill); }
                    }
                }

                //Herald of Ice
                public static Dropdown Herald_of_Ice_VFX_dropdown = null;
                private static void Set_Herald_of_Ice_VFX()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Ice_VFX_dropdown.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.HeraldOfIce.VFX = Herald_of_Ice_VFX_dropdown.options[Herald_of_Ice_VFX_dropdown.value].text;
                        Object.Destroy(Items_Heralds.Uniques.Ice.ability);
                        Object.Destroy(Items_Heralds.Uniques.Ice.prefab_obj);
                    }
                }
                public static Toggle Herald_of_Ice_Radius_toggle = null;
                public static readonly System.Action<bool> Herald_of_Ice_Radius_Toggle_Action = new System.Action<bool>(Set_Herald_of_Ice_Radius_Enable);
                private static void Set_Herald_of_Ice_Radius_Enable(bool enable)
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Ice_Radius_toggle.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.HeraldOfIce.Enable_Radius = Herald_of_Ice_Radius_toggle.isOn;
                        Object.Destroy(Items_Heralds.Uniques.Ice.prefab_obj);
                    }
                }
                public static Text Herald_of_Ice_Radius_text = null;
                public static Slider Herald_of_Ice_Radius_slider = null;
                public static readonly System.Action<float> Herald_of_Ice_Radius_slider_Action = new System.Action<float>(Set_Herald_of_Ice_Radius);
                public static void Set_Herald_of_Ice_Radius(float f)
                {
                    if ((!Herald_of_Ice_Radius_slider.IsNullOrDestroyed()) && (!Herald_of_Ice_Radius_text.IsNullOrDestroyed()))
                    {
                        float result = Herald_of_Ice_Radius_slider.value;
                        Save_Manager.instance.data.NewItems.HeraldOfIce.Radius = result;              
                        Herald_of_Ice_Radius_text.text = ((int)(result * 100)).ToString() + " %";
                        Object.Destroy(Items_Heralds.Uniques.Ice.prefab_obj);
                    }
                }
                public static Dropdown Herald_of_Ice_LegendaryType_dropdown = null;
                private static void Set_Herald_of_Ice_LegendaryType()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Ice_LegendaryType_dropdown.IsNullOrDestroyed()))
                    {
                        bool weaverwill = false;
                        if (Herald_of_Ice_LegendaryType_dropdown.value == 1) { weaverwill = true; }
                        Save_Manager.instance.data.NewItems.HeraldOfIce.WeaverWill = weaverwill;
                        if (IsPauseOpen()) { Items_Heralds.Uniques.Ice.Update_LegendaryType(weaverwill); }
                    }
                }

                //Herald of Ash
                public static Dropdown Herald_of_Fire_VFX_dropdown = null;
                private static void Set_Herald_of_Fire_VFX()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Fire_VFX_dropdown.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.HeraldOfFire.VFX = Herald_of_Fire_VFX_dropdown.options[Herald_of_Fire_VFX_dropdown.value].text;
                        Object.Destroy(Items_Heralds.Uniques.Fire.ability);
                        Object.Destroy(Items_Heralds.Uniques.Fire.prefab_obj);
                    }
                }
                public static Toggle Herald_of_Fire_Radius_toggle = null;
                public static readonly System.Action<bool> Herald_of_Fire_Radius_Toggle_Action = new System.Action<bool>(Set_Herald_of_Fire_Radius_Enable);
                private static void Set_Herald_of_Fire_Radius_Enable(bool enable)
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Fire_Radius_toggle.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.HeraldOfFire.Enable_Radius = Herald_of_Fire_Radius_toggle.isOn;
                        Object.Destroy(Items_Heralds.Uniques.Fire.prefab_obj);
                    }
                }
                public static Text Herald_of_Fire_Radius_text = null;
                public static Slider Herald_of_Fire_Radius_slider = null;
                public static readonly System.Action<float> Herald_of_Fire_Radius_slider_Action = new System.Action<float>(Set_Herald_of_Fire_Radius);
                public static void Set_Herald_of_Fire_Radius(float f)
                {
                    if ((!Herald_of_Fire_Radius_slider.IsNullOrDestroyed()) && (!Herald_of_Fire_Radius_text.IsNullOrDestroyed()))
                    {
                        float result = Herald_of_Fire_Radius_slider.value;
                        Save_Manager.instance.data.NewItems.HeraldOfFire.Radius = result;
                        Herald_of_Fire_Radius_text.text = ((int)(result * 100)).ToString() + " %";
                        Object.Destroy(Items_Heralds.Uniques.Fire.prefab_obj);
                    }
                }
                public static Dropdown Herald_of_Fire_LegendaryType_dropdown = null;
                private static void Set_Herald_of_Fire_LegendaryType()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Fire_LegendaryType_dropdown.IsNullOrDestroyed()))
                    {
                        bool weaverwill = false;
                        if (Herald_of_Fire_LegendaryType_dropdown.value == 1) { weaverwill = true; }
                        Save_Manager.instance.data.NewItems.HeraldOfFire.WeaverWill = weaverwill;
                        if (IsPauseOpen()) { Items_Heralds.Uniques.Fire.Update_LegendaryType(weaverwill); }
                    }
                }

                //Herald of Thunder
                public static Dropdown Herald_of_Thunder_VFX_dropdown = null;
                private static void Set_Herald_of_Thunder_VFX()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Thunder_VFX_dropdown.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.HeraldOfThunder.VFX = Herald_of_Thunder_VFX_dropdown.options[Herald_of_Thunder_VFX_dropdown.value].text;
                        Object.Destroy(Items_Heralds.Uniques.Lightning.ability);
                        Object.Destroy(Items_Heralds.Uniques.Lightning.prefab_obj);
                    }
                }
                public static Toggle Herald_of_Thunder_Radius_toggle = null;
                public static readonly System.Action<bool> Herald_of_Thunder_Radius_Toggle_Action = new System.Action<bool>(Set_Herald_of_Thunder_Radius_Enable);
                private static void Set_Herald_of_Thunder_Radius_Enable(bool enable)
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Thunder_Radius_toggle.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.HeraldOfThunder.Enable_Radius = Herald_of_Thunder_Radius_toggle.isOn;
                        Object.Destroy(Items_Heralds.Uniques.Lightning.prefab_obj);
                    }
                }
                public static Text Herald_of_Thunder_Radius_text = null;
                public static Slider Herald_of_Thunder_Radius_slider = null;
                public static readonly System.Action<float> Herald_of_Thunder_Radius_slider_Action = new System.Action<float>(Set_Herald_of_Thunder_Radius);
                public static void Set_Herald_of_Thunder_Radius(float f)
                {
                    if ((!Herald_of_Thunder_Radius_slider.IsNullOrDestroyed()) && (!Herald_of_Thunder_Radius_text.IsNullOrDestroyed()))
                    {
                        float result = Herald_of_Thunder_Radius_slider.value;
                        Save_Manager.instance.data.NewItems.HeraldOfThunder.Radius = result;
                        Herald_of_Thunder_Radius_text.text = ((int)(result * 100)).ToString() + " %";
                        Object.Destroy(Items_Heralds.Uniques.Lightning.prefab_obj);
                    }
                }
                public static Dropdown Herald_of_Thunder_LegendaryType_dropdown = null;
                private static void Set_Herald_of_Thunder_LegendaryType()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Thunder_LegendaryType_dropdown.IsNullOrDestroyed()))
                    {
                        bool weaverwill = false;
                        if (Herald_of_Thunder_LegendaryType_dropdown.value == 1) { weaverwill = true; }
                        Save_Manager.instance.data.NewItems.HeraldOfThunder.WeaverWill = weaverwill;
                        if (IsPauseOpen()) { Items_Heralds.Uniques.Lightning.Update_LegendaryType(weaverwill); }
                    }
                }

                //Herald of Agony
                public static Dropdown Herald_of_Agony_VFX_dropdown = null;
                private static void Set_Herald_of_Agony_VFX()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Agony_VFX_dropdown.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.HeraldOfAgony.VFX = Herald_of_Agony_VFX_dropdown.options[Herald_of_Agony_VFX_dropdown.value].text;
                        Object.Destroy(Items_Heralds.Uniques.Poison.ability);
                        Object.Destroy(Items_Heralds.Uniques.Poison.prefab_obj);
                    }
                }
                public static Toggle Herald_of_Agony_Radius_toggle = null;
                public static readonly System.Action<bool> Herald_of_Agony_Radius_Toggle_Action = new System.Action<bool>(Set_Herald_of_Agony_Radius_Enable);
                private static void Set_Herald_of_Agony_Radius_Enable(bool enable)
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Agony_Radius_toggle.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.HeraldOfAgony.Enable_Radius = Herald_of_Agony_Radius_toggle.isOn;
                        Object.Destroy(Items_Heralds.Uniques.Poison.prefab_obj);
                    }
                }
                public static Text Herald_of_Agony_Radius_text = null;
                public static Slider Herald_of_Agony_Radius_slider = null;
                public static readonly System.Action<float> Herald_of_Agony_Radius_slider_Action = new System.Action<float>(Set_Herald_of_Agony_Radius);
                public static void Set_Herald_of_Agony_Radius(float f)
                {
                    if ((!Herald_of_Agony_Radius_slider.IsNullOrDestroyed()) && (!Herald_of_Agony_Radius_text.IsNullOrDestroyed()))
                    {
                        float result = Herald_of_Agony_Radius_slider.value;
                        Save_Manager.instance.data.NewItems.HeraldOfAgony.Radius = result;
                        Herald_of_Agony_Radius_text.text = ((int)(result * 100)).ToString() + " %";
                        Object.Destroy(Items_Heralds.Uniques.Poison.prefab_obj);
                    }
                }
                public static Dropdown Herald_of_Agony_LegendaryType_dropdown = null;
                private static void Set_Herald_of_Agony_LegendaryType()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Agony_LegendaryType_dropdown.IsNullOrDestroyed()))
                    {
                        bool weaverwill = false;
                        if (Herald_of_Agony_LegendaryType_dropdown.value == 1) { weaverwill = true; }
                        Save_Manager.instance.data.NewItems.HeraldOfAgony.WeaverWill = weaverwill;
                        if (IsPauseOpen()) { Items_Heralds.Uniques.Poison.Update_LegendaryType(weaverwill); }
                    }
                }

                //Herald of Purity
                public static Dropdown Herald_of_Purity_VFX_dropdown = null;
                private static void Set_Herald_of_Purity_VFX()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Purity_VFX_dropdown.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.HeraldOfPurity.VFX = Herald_of_Purity_VFX_dropdown.options[Herald_of_Purity_VFX_dropdown.value].text;
                        Object.Destroy(Items_Heralds.Uniques.Physical.ability);
                        Object.Destroy(Items_Heralds.Uniques.Physical.prefab_obj);
                    }
                }
                public static Toggle Herald_of_Purity_Radius_toggle = null;
                public static readonly System.Action<bool> Herald_of_Purity_Radius_Toggle_Action = new System.Action<bool>(Set_Herald_of_Purity_Radius_Enable);
                private static void Set_Herald_of_Purity_Radius_Enable(bool enable)
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Purity_Radius_toggle.IsNullOrDestroyed()))
                    {
                        Save_Manager.instance.data.NewItems.HeraldOfPurity.Enable_Radius = Herald_of_Purity_Radius_toggle.isOn;
                        Object.Destroy(Items_Heralds.Uniques.Physical.prefab_obj);
                    }
                }
                public static Text Herald_of_Purity_Radius_text = null;
                public static Slider Herald_of_Purity_Radius_slider = null;
                public static readonly System.Action<float> Herald_of_Purity_Radius_slider_Action = new System.Action<float>(Set_Herald_of_Purity_Radius);
                public static void Set_Herald_of_Purity_Radius(float f)
                {
                    if ((!Herald_of_Purity_Radius_slider.IsNullOrDestroyed()) && (!Herald_of_Purity_Radius_text.IsNullOrDestroyed()))
                    {
                        float result = Herald_of_Purity_Radius_slider.value;
                        Save_Manager.instance.data.NewItems.HeraldOfPurity.Radius = result;
                        Herald_of_Purity_Radius_text.text = ((int)(result * 100)).ToString() + " %";
                        Object.Destroy(Items_Heralds.Uniques.Physical.prefab_obj);
                    }
                }
                public static Dropdown Herald_of_Purity_LegendaryType_dropdown = null;
                private static void Set_Herald_of_Purity_LegendaryType()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Herald_of_Purity_LegendaryType_dropdown.IsNullOrDestroyed()))
                    {
                        bool weaverwill = false;
                        if (Herald_of_Purity_LegendaryType_dropdown.value == 1) { weaverwill = true; }
                        Save_Manager.instance.data.NewItems.HeraldOfPurity.WeaverWill = weaverwill;
                        if (IsPauseOpen()) { Items_Heralds.Uniques.Physical.Update_LegendaryType(weaverwill); }
                    }
                }

                public static void Get_Refs()
                {
                    content_obj = Functions.GetChild(Content.content_obj, "NewItems_Content");
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        GameObject left = Functions.GetViewportContent(content_obj, "Left", "Content");
                        if (!left.IsNullOrDestroyed())
                        {
                            GameObject headhunter = Functions.GetChild(left, "Headhunter");
                            if (!headhunter.IsNullOrDestroyed())
                            {
                                Headhunter_MinGeneratedBuff_text = Functions.Get_TextInToggle(left, "Headhunter", "MinBuffText", "Value");
                                Headhunter_MinGeneratedBuff_slider = Functions.Get_SliderInPanel(left, "Headhunter", "Slider_MinBuff");
                                Headhunter_MaxGeneratedBuff_text = Functions.Get_TextInToggle(left, "Headhunter", "MaxBuffText", "Value");
                                Headhunter_MaxGeneratedBuff_slider = Functions.Get_SliderInPanel(left, "Headhunter", "Slider_MaxBuff");
                                Headhunter_BuffDuration_text = Functions.Get_TextInToggle(left, "Headhunter", "BuffDurationText", "Value");
                                Headhunter_BuffDuration_slider = Functions.Get_SliderInPanel(left, "Headhunter", "Slider_BuffDuration");
                                Headhunter_BuffStack_text = Functions.Get_TextInToggle(left, "Headhunter", "BuffStack", "Value");
                                Headhunter_BuffStack_slider = Functions.Get_SliderInPanel(left, "Headhunter", "Slider_BuffStack");
                                Headhunter_Add_text = Functions.Get_TextInToggle(left, "Headhunter", "AddText", "Value");
                                Headhunter_Add_slider = Functions.Get_SliderInPanel(left, "Headhunter", "Slider_Add");
                                Headhunter_Increase_text = Functions.Get_TextInToggle(left, "Headhunter", "IncreaseText", "Value");
                                Headhunter_Increase_slider = Functions.Get_SliderInPanel(left, "Headhunter", "Slider_Increase");
                                Headhunter_LegendaryType_dropdown = Functions.Get_DopboxInPanel(headhunter, "Dropdown_LegendaryType", "Dropdown", new System.Action<int>((_) => { Set_Headhunter_LegendaryType(); }));
                            }
                            GameObject mjolnir = Functions.GetChild(left, "Mjolnir");
                            if (!mjolnir.IsNullOrDestroyed())
                            {
                                Mjolnir_StrReq_text = Functions.Get_TextInToggle(left, "Mjolnir", "StrReqText", "Value");
                                Mjolnir_StrReq_slider = Functions.Get_SliderInPanel(left, "Mjolnir", "Slider_StrReq");
                                Mjolnir_IntReq_text = Functions.Get_TextInToggle(left, "Mjolnir", "IntReqText", "Value");
                                Mjolnir_IntReq_slider = Functions.Get_SliderInPanel(left, "Mjolnir", "Slider_IntReq");
                                Mjolnir_MinTriggerChance_text = Functions.Get_TextInToggle(left, "Mjolnir", "MinTriggerChanceText", "Value");
                                Mjolnir_MinTriggerChance_slider = Functions.Get_SliderInPanel(left, "Mjolnir", "Slider_MinTriggerChance");
                                Mjolnir_MaxTriggerChance_text = Functions.Get_TextInToggle(left, "Mjolnir", "MaxTriggerChanceText", "Value");
                                Mjolnir_MaxTriggerChance_slider = Functions.Get_SliderInPanel(left, "Mjolnir", "Slider_MaxTriggerChance");
                                Mjolnir_TriggerCooldown_text = Functions.Get_TextInToggle(left, "Mjolnir", "SockectedCooldownText", "Value");
                                Mjolnir_TriggerCooldown_slider = Functions.Get_SliderInPanel(left, "Mjolnir", "Slider_SockectedCooldown(1)");
                                Mjolnir_Socket0_dropdown = Functions.Get_DopboxInPanel(mjolnir, "Dropdown_Socket1", "Dropdown", new System.Action<int>((_) => { Set_Mjolnir_Socket0(); }));
                                Mjolnir_Socket1_dropdown = Functions.Get_DopboxInPanel(mjolnir, "Dropdown_Socket2", "Dropdown", new System.Action<int>((_) => { Set_Mjolnir_Socket1(); }));
                                Mjolnir_Socket2_dropdown = Functions.Get_DopboxInPanel(mjolnir, "Dropdown_Socket3", "Dropdown", new System.Action<int>((_) => { Set_Mjolnir_Socket2(); }));
                                Mjolnir_LegendaryType_dropdown = Functions.Get_DopboxInPanel(mjolnir, "Dropdown_LegendaryType", "Dropdown", new System.Action<int>((_) => { Set_Mjolnir_LegendaryType(); }));
                            }
                            GameObject herald_of_ice = Functions.GetChild(left, "Herald_of_Ice");
                            if (!herald_of_ice.IsNullOrDestroyed())
                            {
                                Herald_of_Ice_VFX_dropdown = Functions.Get_DopboxInPanel(herald_of_ice, "VFX", "Dropdown", new System.Action<int>((_) => { Set_Herald_of_Ice_VFX(); }));
                                Herald_of_Ice_Radius_toggle = Functions.Get_ToggleInPanel(left, "Herald_of_Ice", "Toggle");
                                Herald_of_Ice_Radius_text = Functions.Get_TextInToggle(left, "Herald_of_Ice", "Toggle", "Value");
                                Herald_of_Ice_Radius_slider = Functions.Get_SliderInPanel(left, "Herald_of_Ice", "Slider");
                                Herald_of_Ice_LegendaryType_dropdown = Functions.Get_DopboxInPanel(herald_of_ice, "Dropdown_LegendaryType", "Dropdown", new System.Action<int>((_) => { Set_Herald_of_Ice_LegendaryType(); }));
                            }
                            GameObject herald_of_fire = Functions.GetChild(left, "Herald_of_Fire");
                            if (!herald_of_fire.IsNullOrDestroyed())
                            {
                                Herald_of_Fire_VFX_dropdown = Functions.Get_DopboxInPanel(herald_of_fire, "VFX", "Dropdown", new System.Action<int>((_) => { Set_Herald_of_Fire_VFX(); }));
                                Herald_of_Fire_Radius_toggle = Functions.Get_ToggleInPanel(left, "Herald_of_Fire", "Toggle");
                                Herald_of_Fire_Radius_text = Functions.Get_TextInToggle(left, "Herald_of_Fire", "Toggle", "Value");
                                Herald_of_Fire_Radius_slider = Functions.Get_SliderInPanel(left, "Herald_of_Fire", "Slider");
                                Herald_of_Fire_LegendaryType_dropdown = Functions.Get_DopboxInPanel(herald_of_fire, "Dropdown_LegendaryType", "Dropdown", new System.Action<int>((_) => { Set_Herald_of_Fire_LegendaryType(); }));
                            }
                            GameObject herald_of_thunder = Functions.GetChild(left, "Herald_of_Thunder");
                            if (!herald_of_thunder.IsNullOrDestroyed())
                            {
                                Herald_of_Thunder_VFX_dropdown = Functions.Get_DopboxInPanel(herald_of_thunder, "VFX", "Dropdown", new System.Action<int>((_) => { Set_Herald_of_Thunder_VFX(); }));
                                Herald_of_Thunder_Radius_toggle = Functions.Get_ToggleInPanel(left, "Herald_of_Thunder", "Toggle");
                                Herald_of_Thunder_Radius_text = Functions.Get_TextInToggle(left, "Herald_of_Thunder", "Toggle", "Value");
                                Herald_of_Thunder_Radius_slider = Functions.Get_SliderInPanel(left, "Herald_of_Thunder", "Slider");
                                Herald_of_Thunder_LegendaryType_dropdown = Functions.Get_DopboxInPanel(herald_of_thunder, "Dropdown_LegendaryType", "Dropdown", new System.Action<int>((_) => { Set_Herald_of_Thunder_LegendaryType(); }));
                            }
                            GameObject herald_of_agony = Functions.GetChild(left, "Herald_of_Agony");
                            if (!herald_of_agony.IsNullOrDestroyed())
                            {
                                Herald_of_Agony_VFX_dropdown = Functions.Get_DopboxInPanel(herald_of_agony, "VFX", "Dropdown", new System.Action<int>((_) => { Set_Herald_of_Agony_VFX(); }));
                                Herald_of_Agony_Radius_toggle = Functions.Get_ToggleInPanel(left, "Herald_of_Agony", "Toggle");
                                Herald_of_Agony_Radius_text = Functions.Get_TextInToggle(left, "Herald_of_Agony", "Toggle", "Value");
                                Herald_of_Agony_Radius_slider = Functions.Get_SliderInPanel(left, "Herald_of_Agony", "Slider");
                                Herald_of_Agony_LegendaryType_dropdown = Functions.Get_DopboxInPanel(herald_of_agony, "Dropdown_LegendaryType", "Dropdown", new System.Action<int>((_) => { Set_Herald_of_Agony_LegendaryType(); }));
                            }
                            GameObject herald_of_purity = Functions.GetChild(left, "Herald_of_Purity");
                            if (!herald_of_agony.IsNullOrDestroyed())
                            {
                                Herald_of_Purity_VFX_dropdown = Functions.Get_DopboxInPanel(herald_of_purity, "VFX", "Dropdown", new System.Action<int>((_) => { Set_Herald_of_Purity_VFX(); }));
                                Herald_of_Purity_Radius_toggle = Functions.Get_ToggleInPanel(left, "Herald_of_Purity", "Toggle");
                                Herald_of_Purity_Radius_text = Functions.Get_TextInToggle(left, "Herald_of_Purity", "Toggle", "Value");
                                Herald_of_Purity_Radius_slider = Functions.Get_SliderInPanel(left, "Herald_of_Purity", "Slider");
                                Herald_of_Purity_LegendaryType_dropdown = Functions.Get_DopboxInPanel(herald_of_purity, "Dropdown_LegendaryType", "Dropdown", new System.Action<int>((_) => { Set_Herald_of_Purity_LegendaryType(); }));
                            }
                        }
                        GameObject center = Functions.GetViewportContent(content_obj, "Center", "Content");
                        if (!center.IsNullOrDestroyed())
                        {

                        }
                        GameObject right = Functions.GetViewportContent(content_obj, "R", "Content");
                        if (!right.IsNullOrDestroyed())
                        {

                        }
                    }
                }
                public static void Init_Dropdowns()
                {
                    GetLegendaryType(Headhunter_LegendaryType_dropdown);

                    GetAbility(Mjolnir_Socket0_dropdown, "", true, false);
                    GetAbility(Mjolnir_Socket1_dropdown, "", true, false);
                    GetAbility(Mjolnir_Socket2_dropdown, "", true, false);
                    GetLegendaryType(Mjolnir_LegendaryType_dropdown);

                    GetAbility(Herald_of_Ice_VFX_dropdown, "Cold, Spell", false, true);
                    GetLegendaryType(Herald_of_Ice_LegendaryType_dropdown);

                    GetAbility(Herald_of_Fire_VFX_dropdown, "Fire, Spell", false, true);
                    GetLegendaryType(Herald_of_Fire_LegendaryType_dropdown);

                    GetAbility(Herald_of_Thunder_VFX_dropdown, "Lightning, Spell", false, true);
                    GetLegendaryType(Herald_of_Thunder_LegendaryType_dropdown);

                    GetAbility(Herald_of_Agony_VFX_dropdown, "Poison, Spell", false, true);
                    GetLegendaryType(Herald_of_Agony_LegendaryType_dropdown);

                    GetAbility(Herald_of_Purity_VFX_dropdown, "Physical, Spell", false, true);
                    GetLegendaryType(Herald_of_Agony_LegendaryType_dropdown);
                }
                public static void GetLegendaryType(Dropdown dropdown)
                {
                    if (!dropdown.IsNullOrDestroyed())
                    {
                        dropdown.options.Clear();
                        dropdown.options.Add(new Dropdown.OptionData(UniqueList.LegendaryType.LegendaryPotential.ToString()));
                        dropdown.options.Add(new Dropdown.OptionData(UniqueList.LegendaryType.WeaversWill.ToString()));
                    }
                }
                public static void GetAbility(Dropdown dropdown, string tags, bool mjolnir, bool herald)
                {
                    if (!dropdown.IsNullOrDestroyed())
                    {
                        dropdown.options.Clear();
                        System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>(); //Don't duplicate abilities
                        foreach (Ability ab in Resources.FindObjectsOfTypeAll<Ability>())
                        {
                            if ((mjolnir) && (ab.tags.HasFlag(AT.Lightning)) && (ab.tags.HasFlag(AT.Spell)))
                            {
                                if (!names.Contains(ab.abilityName)) { names.Add(ab.abilityName); }
                            }
                            else if ((herald) && (ab.tags.ToString() == tags))
                            {
                                if (!ab.abilityPrefab.IsNullOrDestroyed())
                                {
                                    bool contain_collider = false;
                                    SphereCollider collider = ab.abilityPrefab.GetComponent<UnityEngine.SphereCollider>();
                                    if (!collider.IsNullOrDestroyed()) { contain_collider = true; }
                                    bool contain_vfx_ondeath = false;
                                    CreateVfxOnDeath vfx_on_death = ab.abilityPrefab.GetComponent<CreateVfxOnDeath>();
                                    if (!vfx_on_death.IsNullOrDestroyed()) { contain_vfx_ondeath = true; }
                                    if ((contain_collider) && (contain_vfx_ondeath))
                                    {
                                        if (!names.Contains(ab.name)) { names.Add(ab.name); }
                                    }
                                }
                            }
                        }
                        names.Sort();
                        foreach (string name in names) { dropdown.options.Add(new Dropdown.OptionData(name)); }
                    }
                }
                public static void Set_Events()
                {
                    if (!Headhunter_MinGeneratedBuff_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Headhunter_MinGeneratedBuff_slider, Headhunter_MinGeneratedBuff_slider_Action);
                    }
                    if (!Headhunter_MaxGeneratedBuff_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Headhunter_MaxGeneratedBuff_slider, Headhunter_MaxGeneratedBuff_slider_Action);
                    }
                    if (!Headhunter_BuffDuration_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Headhunter_BuffDuration_slider, Headhunter_BuffDuration_slider_Action);
                    }
                    if (!Headhunter_BuffStack_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Headhunter_BuffStack_slider, Headhunter_BuffStack_slider_Action);
                    }
                    if (!Headhunter_Add_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Headhunter_Add_slider, Headhunter_Add_slider_Action);
                    }
                    if (!Headhunter_Increase_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Headhunter_Increase_slider, Headhunter_Increase_slider_Action);
                    }
                    if (!Mjolnir_StrReq_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Mjolnir_StrReq_slider, Mjolnir_StrReq_slider_Action);
                    }
                    if (!Mjolnir_IntReq_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Mjolnir_IntReq_slider, Mjolnir_IntReq_slider_Action);
                    }
                    if (!Mjolnir_MinTriggerChance_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Mjolnir_MinTriggerChance_slider, Mjolnir_MinTriggerChance_slider_Action);
                    }
                    if (!Mjolnir_MaxTriggerChance_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Mjolnir_MaxTriggerChance_slider, Mjolnir_MaxTriggerChance_slider_Action);
                    }
                    if (!Mjolnir_TriggerCooldown_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Mjolnir_TriggerCooldown_slider, Mjolnir_TriggerCooldown_slider_Action);
                    }
                    if (!Herald_of_Ice_Radius_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Herald_of_Ice_Radius_toggle, Herald_of_Ice_Radius_Toggle_Action);
                    }
                    if (!Herald_of_Ice_Radius_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Herald_of_Ice_Radius_slider, Herald_of_Ice_Radius_slider_Action);
                    }
                    if (!Herald_of_Fire_Radius_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Herald_of_Fire_Radius_toggle, Herald_of_Fire_Radius_Toggle_Action);
                    }
                    if (!Herald_of_Fire_Radius_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Herald_of_Fire_Radius_slider, Herald_of_Fire_Radius_slider_Action);
                    }
                    if (!Herald_of_Thunder_Radius_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Herald_of_Thunder_Radius_toggle, Herald_of_Thunder_Radius_Toggle_Action);
                    }
                    if (!Herald_of_Thunder_Radius_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Herald_of_Thunder_Radius_slider, Herald_of_Thunder_Radius_slider_Action);
                    }
                    if (!Herald_of_Agony_Radius_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Herald_of_Agony_Radius_toggle, Herald_of_Agony_Radius_Toggle_Action);
                    }
                    if (!Herald_of_Agony_Radius_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Herald_of_Agony_Radius_slider, Herald_of_Agony_Radius_slider_Action);
                    }
                    if (!Herald_of_Purity_Radius_toggle.IsNullOrDestroyed())
                    {
                        Events.Set_Toggle_Event(Herald_of_Purity_Radius_toggle, Herald_of_Purity_Radius_Toggle_Action);
                    }
                    if (!Herald_of_Purity_Radius_slider.IsNullOrDestroyed())
                    {
                        Events.Set_Slider_Event(Herald_of_Purity_Radius_slider, Herald_of_Purity_Radius_slider_Action);
                    }
                }
                public static void Set_Active(bool show)
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static void Toggle_Active()
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        bool show = !content_obj.active;
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static bool Init_Data()
                {
                    bool result = false;
                    if (!Save_Manager.instance.IsNullOrDestroyed())
                    {
                        if (Save_Manager.instance.initialized)
                        {
                            if (!Headhunter_MinGeneratedBuff_slider.IsNullOrDestroyed())
                            {
                                Headhunter_MinGeneratedBuff_slider.value = Save_Manager.instance.data.NewItems.Headhunter.MinGenerated;
                            }
                            if (!Headhunter_MaxGeneratedBuff_slider.IsNullOrDestroyed())
                            {
                                Headhunter_MaxGeneratedBuff_slider.value = Save_Manager.instance.data.NewItems.Headhunter.MaxGenerated;
                            }
                            if (!Headhunter_BuffDuration_slider.IsNullOrDestroyed())
                            {
                                Headhunter_BuffDuration_slider.value = Save_Manager.instance.data.NewItems.Headhunter.BuffDuration;
                            }
                            if (!Headhunter_BuffStack_slider.IsNullOrDestroyed())
                            {
                                Headhunter_BuffStack_slider.value = Save_Manager.instance.data.NewItems.Headhunter.Stack;
                            }
                            if (!Headhunter_Add_slider.IsNullOrDestroyed())
                            {
                                Headhunter_Add_slider.value = Save_Manager.instance.data.NewItems.Headhunter.AddValue;
                            }
                            if (!Headhunter_Increase_slider.IsNullOrDestroyed())
                            {
                                Headhunter_Increase_slider.value = Save_Manager.instance.data.NewItems.Headhunter.IncreasedValue;
                            }
                            if (!Headhunter_LegendaryType_dropdown.IsNullOrDestroyed())
                            {
                                int index = 0;
                                if (Save_Manager.instance.data.NewItems.Headhunter.WeaverWill) { index = 1; }
                                Headhunter_LegendaryType_dropdown.value = index;
                            }

                            if (!Mjolnir_StrReq_slider.IsNullOrDestroyed())
                            {
                                Mjolnir_StrReq_slider.value = Save_Manager.instance.data.NewItems.Mjolner.StrRequirement;
                            }
                            if (!Mjolnir_IntReq_slider.IsNullOrDestroyed())
                            {
                                Mjolnir_IntReq_slider.value = Save_Manager.instance.data.NewItems.Mjolner.IntRequirement;
                            }
                            if (!Mjolnir_MinTriggerChance_slider.IsNullOrDestroyed())
                            {
                                Mjolnir_MinTriggerChance_slider.value = (Save_Manager.instance.data.NewItems.Mjolner.MinTriggerChance * 100);
                            }
                            if (!Mjolnir_MaxTriggerChance_slider.IsNullOrDestroyed())
                            {
                                Mjolnir_MaxTriggerChance_slider.value = (Save_Manager.instance.data.NewItems.Mjolner.MaxTriggerChance * 100);
                            }
                            if (!Mjolnir_TriggerCooldown_slider.IsNullOrDestroyed())
                            {
                                Mjolnir_TriggerCooldown_slider.value = (float)Save_Manager.instance.data.NewItems.Mjolner.SocketedCooldown;
                            }
                            if (!Mjolnir_Socket0_dropdown.IsNullOrDestroyed())
                            {
                                int index = 0;
                                if (Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_0 != "")
                                {
                                    bool found = false;
                                    foreach (Dropdown.OptionData options in Mjolnir_Socket0_dropdown.options)
                                    {
                                        if (options.text == Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_0) { break; }
                                        found = true;
                                        index++;
                                    }
                                    if (!found) { index = 0; }
                                }
                                Mjolnir_Socket0_dropdown.value = index;
                            }
                            if (!Mjolnir_Socket1_dropdown.IsNullOrDestroyed())
                            {
                                int index = 0;
                                if (Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_1 != "")
                                {
                                    bool found = false;
                                    foreach (Dropdown.OptionData options in Mjolnir_Socket1_dropdown.options)
                                    {
                                        if (options.text == Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_1) { break; }
                                        found = true;
                                        index++;
                                    }
                                    if (!found) { index = 0; }
                                }
                                Mjolnir_Socket1_dropdown.value = index;
                            }
                            if (!Mjolnir_Socket2_dropdown.IsNullOrDestroyed())
                            {
                                int index = 0;
                                if (Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_2 != "")
                                {
                                    bool found = false;
                                    foreach (Dropdown.OptionData options in Mjolnir_Socket2_dropdown.options)
                                    {
                                        if (options.text == Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_2) { break; }
                                        found = true;
                                        index++;
                                    }
                                    if (!found) { index = 0; }
                                }
                                Mjolnir_Socket2_dropdown.value = index;
                            }
                            if (!Mjolnir_LegendaryType_dropdown.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.NewItems.Mjolner.WeaverWill) { Mjolnir_LegendaryType_dropdown.value = 1; }
                                else { Mjolnir_LegendaryType_dropdown.value = 0; }
                            }

                            if (!Herald_of_Ice_VFX_dropdown.IsNullOrDestroyed())
                            {
                                int index = 0;
                                if (Save_Manager.instance.data.NewItems.HeraldOfIce.VFX != "")
                                {
                                    foreach (Dropdown.OptionData options in Herald_of_Ice_VFX_dropdown.options)
                                    {
                                        if (options.text == Save_Manager.instance.data.NewItems.HeraldOfIce.VFX) { break; }
                                        index++;
                                    }
                                }
                                Herald_of_Ice_VFX_dropdown.value = index;
                            }
                            if (!Herald_of_Ice_Radius_toggle.IsNullOrDestroyed())
                            {
                                Herald_of_Ice_Radius_toggle.isOn = Save_Manager.instance.data.NewItems.HeraldOfIce.Enable_Radius;
                            }
                            if (!Herald_of_Ice_Radius_slider.IsNullOrDestroyed())
                            {
                                Herald_of_Ice_Radius_slider.value = Save_Manager.instance.data.NewItems.HeraldOfIce.Radius;
                            }
                            if (!Herald_of_Ice_LegendaryType_dropdown.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.NewItems.HeraldOfIce.WeaverWill) { Herald_of_Ice_LegendaryType_dropdown.value = 1; }
                                else { Herald_of_Ice_LegendaryType_dropdown.value = 0; }
                            }

                            if (!Herald_of_Fire_VFX_dropdown.IsNullOrDestroyed())
                            {
                                int index = 0;
                                if (Save_Manager.instance.data.NewItems.HeraldOfFire.VFX != "")
                                {
                                    foreach (Dropdown.OptionData options in Herald_of_Fire_VFX_dropdown.options)
                                    {
                                        if (options.text == Save_Manager.instance.data.NewItems.HeraldOfFire.VFX) { break; }
                                        index++;
                                    }
                                }
                                Herald_of_Fire_VFX_dropdown.value = index;
                            }
                            if (!Herald_of_Fire_Radius_toggle.IsNullOrDestroyed())
                            {
                                Herald_of_Fire_Radius_toggle.isOn = Save_Manager.instance.data.NewItems.HeraldOfFire.Enable_Radius;
                            }
                            if (!Herald_of_Fire_Radius_slider.IsNullOrDestroyed())
                            {
                                Herald_of_Fire_Radius_slider.value = Save_Manager.instance.data.NewItems.HeraldOfFire.Radius;
                            }
                            if (!Herald_of_Fire_LegendaryType_dropdown.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.NewItems.HeraldOfFire.WeaverWill) { Herald_of_Fire_LegendaryType_dropdown.value = 1; }
                                else { Herald_of_Fire_LegendaryType_dropdown.value = 0; }
                            }

                            if (!Herald_of_Thunder_VFX_dropdown.IsNullOrDestroyed())
                            {
                                int index = 0;
                                if (Save_Manager.instance.data.NewItems.HeraldOfThunder.VFX != "")
                                {
                                    foreach (Dropdown.OptionData options in Herald_of_Thunder_VFX_dropdown.options)
                                    {
                                        if (options.text == Save_Manager.instance.data.NewItems.HeraldOfThunder.VFX) { break; }
                                        index++;
                                    }
                                }
                                Herald_of_Thunder_VFX_dropdown.value = index;
                            }
                            if (!Herald_of_Thunder_Radius_toggle.IsNullOrDestroyed())
                            {
                                Herald_of_Thunder_Radius_toggle.isOn = Save_Manager.instance.data.NewItems.HeraldOfThunder.Enable_Radius;
                            }
                            if (!Herald_of_Thunder_Radius_slider.IsNullOrDestroyed())
                            {
                                Herald_of_Thunder_Radius_slider.value = Save_Manager.instance.data.NewItems.HeraldOfThunder.Radius;
                            }
                            if (!Herald_of_Thunder_LegendaryType_dropdown.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.NewItems.HeraldOfThunder.WeaverWill) { Herald_of_Thunder_LegendaryType_dropdown.value = 1; }
                                else { Herald_of_Thunder_LegendaryType_dropdown.value = 0; }
                            }

                            if (!Herald_of_Agony_VFX_dropdown.IsNullOrDestroyed())
                            {
                                int index = 0;
                                if (Save_Manager.instance.data.NewItems.HeraldOfAgony.VFX != "")
                                {
                                    foreach (Dropdown.OptionData options in Herald_of_Agony_VFX_dropdown.options)
                                    {
                                        if (options.text == Save_Manager.instance.data.NewItems.HeraldOfAgony.VFX) { break; }
                                        index++;
                                    }
                                }
                                Herald_of_Agony_VFX_dropdown.value = index;
                            }
                            if (!Herald_of_Agony_Radius_toggle.IsNullOrDestroyed())
                            {
                                Herald_of_Agony_Radius_toggle.isOn = Save_Manager.instance.data.NewItems.HeraldOfAgony.Enable_Radius;
                            }
                            if (!Herald_of_Agony_Radius_slider.IsNullOrDestroyed())
                            {
                                Herald_of_Agony_Radius_slider.value = Save_Manager.instance.data.NewItems.HeraldOfAgony.Radius;
                            }
                            if (!Herald_of_Agony_LegendaryType_dropdown.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.NewItems.HeraldOfAgony.WeaverWill) { Herald_of_Agony_LegendaryType_dropdown.value = 1; }
                                else { Herald_of_Agony_LegendaryType_dropdown.value = 0; }
                            }

                            if (!Herald_of_Purity_VFX_dropdown.IsNullOrDestroyed())
                            {
                                int index = 0;
                                if (Save_Manager.instance.data.NewItems.HeraldOfPurity.VFX != "")
                                {
                                    foreach (Dropdown.OptionData options in Herald_of_Purity_VFX_dropdown.options)
                                    {
                                        if (options.text == Save_Manager.instance.data.NewItems.HeraldOfPurity.VFX) { break; }
                                        index++;
                                    }
                                }
                                Herald_of_Purity_VFX_dropdown.value = index;
                            }
                            if (!Herald_of_Purity_Radius_toggle.IsNullOrDestroyed())
                            {
                                Herald_of_Purity_Radius_toggle.isOn = Save_Manager.instance.data.NewItems.HeraldOfPurity.Enable_Radius;
                            }
                            if (!Herald_of_Purity_Radius_slider.IsNullOrDestroyed())
                            {
                                Herald_of_Purity_Radius_slider.value = Save_Manager.instance.data.NewItems.HeraldOfPurity.Radius;
                            }
                            if (!Herald_of_Purity_LegendaryType_dropdown.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.NewItems.HeraldOfPurity.WeaverWill) { Herald_of_Purity_LegendaryType_dropdown.value = 1; }
                                else { Herald_of_Purity_LegendaryType_dropdown.value = 0; }
                            }

                            result = true;
                        }
                    }
                    
                    return result;
                }
            }
            public class Maxroll
            {
                public static GameObject content_obj = null;
                public static bool enable = false;
                public static bool show = false;
                public static bool loading = false;
                public static bool update = false;

                public static GameObject profile_text_obj = null;
                public static GameObject profile_dropdown_obj = null;
                public static Dropdown profile_dropdown = null;
                public static void Update_Profile()
                {
                    if (!loading)
                    {
                        Maxroll_import.Data.selected_profile = profile_dropdown.value;
                        update = true;
                        show = false;
                    }
                }
                public static TMP_InputField url_field = null;
                public static Button clear_url_btn = null;
                public static readonly System.Action clear_url_OnClick_Action = new System.Action(clear_url_Click);
                public static void clear_url_Click()
                {
                    if (!url_field.IsNullOrDestroyed()) { url_field.text = ""; }
                    //Hide();
                    Maxroll_import.Data.root = null;
                    Maxroll_import.Data.data = null;                    
                }
                public static Button refresh_btn = null;
                public static readonly System.Action refresh_OnClick_Action = new System.Action(refresh_Click);
                public static async void refresh_Click()
                {
                    if (!url_field.IsNullOrDestroyed()) { await Maxroll_import.Data.Load(url_field.text); }
                }
                public static GameObject _3_obj = null;
                public static Text build_name_text = null;
                public static Text autor_name_text = null;
                public static GameObject youtube_obj = null;
                public static Button youtube_btn = null;
                public static readonly System.Action youtube_OnClick_Action = new System.Action(youtube_Click);
                public static void youtube_Click()
                {
                    if (youtube_url != "") { Application.OpenURL(youtube_url); }
                }
                public static string youtube_url = "";
                public static GameObject twitch_obj = null;
                public static Button twitch_btn = null;
                public static readonly System.Action twitch_OnClick_Action = new System.Action(twitch_Click);
                public static void twitch_Click()
                {
                    if (twitch_url != "") { Application.OpenURL(twitch_url); }
                }
                public static string twitch_url = "";
                public static GameObject l_content_obj = null;
                public static GameObject r_content_obj = null;
                public static Text classe_text = null;
                public static Button classe_btn = null;
                public static readonly System.Action classe_OnClick_Action = new System.Action(classe_Click);
                public static void classe_Click()
                {
                    if ((!Refs_Manager.character_class_list.IsNullOrDestroyed()) && (!classe_text.IsNullOrDestroyed()) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
                    {
                        bool found = false;
                        int i = 0;
                        foreach (CharacterClass char_class in Refs_Manager.character_class_list.classes)
                        {
                            if (char_class.className == classe_text.text) { found = true; break; }
                            i++;
                        }
                        if (found)
                        {
                            Refs_Manager.player_data.CharacterClass = i;
                            Refs_Manager.player_data.SaveData();
                            update = true;
                            show = false;
                        }                        
                    }
                }
                public static Text level_text = null;
                public static Button level_btn = null;
                public static readonly System.Action level_OnClick_Action = new System.Action(level_Click);
                public static void level_Click()
                {
                    try
                    {
                        int level = System.Convert.ToInt32(level_text.text);
                        Mods.Character.Character_Level.LevelUpToLevel(level);
                    }
                    catch { }
                }
                public static Text items_text = null;
                public static Button items_btn = null;
                public static readonly System.Action items_OnClick_Action = new System.Action(items_Click);
                public static void items_Click()
                {
                    Maxroll_import.Data.Load_Equipments();
                }
                public static Text idols_text = null;
                public static Button idols_btn = null;
                public static readonly System.Action idols_OnClick_Action = new System.Action(idols_Click);
                public static void idols_Click()
                {
                    Maxroll_import.Data.Load_Idols();
                }
                public static Text blessings_text = null;
                public static Button blessings_btn = null;
                public static readonly System.Action blessings_OnClick_Action = new System.Action(blessings_Click);
                public static void blessings_Click()
                {
                    Maxroll_import.Data.Load_Blessings();
                }
                public static Text passives_text = null;
                public static Button passives_btn = null;
                public static readonly System.Action passives_OnClick_Action = new System.Action(passives_Click);
                public static void passives_Click()
                {
                    Maxroll_import.Data.Load_Passives();
                }
                public static Text weavertree_text = null;
                public static Button weavertree_btn = null;
                public static readonly System.Action weavertree_OnClick_Action = new System.Action(weavertree_Click);
                public static void weavertree_Click()
                {
                    Maxroll_import.Data.Load_WeaverTree();
                }
                public static Image mainskill_image = null;
                public static Text mainskill_text = null;
                public static Image activeskill_0_image = null;
                public static Image activeskill_1_image = null;
                public static Image activeskill_2_image = null;
                public static Image activeskill_3_image = null;
                public static Image activeskill_4_image = null;
                public static Button activeskills_btn = null;
                public static readonly System.Action activeskills_OnClick_Action = new System.Action(activeskills_Click);
                public static void activeskills_Click()
                {
                    Maxroll_import.Data.Load_ActiveSkills();
                }
                public static Button skilltrees_btn = null;
                public static readonly System.Action skilltrees_OnClick_Action = new System.Action(skilltrees_Click);
                public static void skilltrees_Click()
                {
                    Maxroll_import.Data.Load_SkillTrees();
                }
                public static Text skill_0_text = null;
                public static Image skill_0_image = null;
                public static Dropdown skill_0_dropdown = null;
                public static Button skill_0_btn = null;
                public static readonly System.Action skill_0_OnClick_Action = new System.Action(skill_0_Click);
                public static void skill_0_Click()
                {
                    Maxroll_import.Data.Load_SkillTree(0, skill_0_dropdown.value);
                }
                public static Text skill_1_text = null;
                public static Image skill_1_image = null;
                public static Dropdown skill_1_dropdown = null;
                public static Button skill_1_btn = null;
                public static readonly System.Action skill_1_OnClick_Action = new System.Action(skill_1_Click);
                public static void skill_1_Click()
                {
                    Maxroll_import.Data.Load_SkillTree(1, skill_1_dropdown.value);
                }
                public static Text skill_2_text = null;
                public static Image skill_2_image = null;
                public static Dropdown skill_2_dropdown = null;
                public static Button skill_2_btn = null;
                public static readonly System.Action skill_2_OnClick_Action = new System.Action(skill_2_Click);
                public static void skill_2_Click()
                {
                    Maxroll_import.Data.Load_SkillTree(2, skill_2_dropdown.value);
                }
                public static Text skill_3_text = null;
                public static Image skill_3_image = null;
                public static Dropdown skill_3_dropdown = null;
                public static Button skill_3_btn = null;
                public static readonly System.Action skill_3_OnClick_Action = new System.Action(skill_3_Click);
                public static void skill_3_Click()
                {
                    Maxroll_import.Data.Load_SkillTree(3, skill_3_dropdown.value);
                }
                public static Text skill_4_text = null;
                public static Image skill_4_image = null;
                public static Dropdown skill_4_dropdown = null;
                public static Button skill_4_btn = null;
                public static readonly System.Action skill_4_OnClick_Action = new System.Action(skill_4_Click);
                public static void skill_4_Click()
                {
                    Maxroll_import.Data.Load_SkillTree(4, skill_4_dropdown.value);
                }
                
                public static void Get_Refs()
                {
                    content_obj = Functions.GetChild(Content.content_obj, "Maxroll_Content");
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        GameObject top_obj = Functions.GetChild(content_obj, "Top");
                        if (!top_obj.IsNullOrDestroyed())
                        {
                            GameObject top_content_obj = Functions.GetChild(top_obj, "Content");
                            if (!top_content_obj.IsNullOrDestroyed())
                            {
                                GameObject _0_obj = Functions.GetChild(top_content_obj, "0");
                                if (!_0_obj.IsNullOrDestroyed())
                                {
                                    GameObject profile_obj = Functions.GetChild(_0_obj, "Profile");
                                    if (!profile_obj.IsNullOrDestroyed())
                                    {
                                        profile_text_obj = Functions.GetChild(profile_obj, "Text");
                                        if (!profile_text_obj.IsNullOrDestroyed())
                                        {

                                        }
                                    }
                                }
                                GameObject _1_obj = Functions.GetChild(top_content_obj, "1");
                                if (!_1_obj.IsNullOrDestroyed())
                                {
                                    GameObject url_obj = Functions.GetChild(_1_obj, "Url");
                                    if (!url_obj.IsNullOrDestroyed())
                                    {
                                        GameObject inputfield_obj = Functions.GetChild(url_obj, "InputField");
                                        if (!inputfield_obj.IsNullOrDestroyed())
                                        {
                                            url_field = inputfield_obj.GetComponent<TMP_InputField>();
                                        }
                                        GameObject btn_obj = Functions.GetChild(url_obj, "Button");
                                        if (!btn_obj.IsNullOrDestroyed())
                                        {
                                            clear_url_btn = btn_obj.GetComponent<Button>();
                                        }
                                    }
                                    GameObject profile_obj = Functions.GetChild(_1_obj, "Profile");
                                    if (!profile_obj.IsNullOrDestroyed())
                                    {                                        
                                        profile_dropdown_obj = Functions.GetChild(profile_obj, "Dropdown");
                                        if (!profile_dropdown_obj.IsNullOrDestroyed())
                                        {
                                            profile_dropdown = profile_dropdown_obj.GetComponent<Dropdown>();
                                            profile_dropdown.onValueChanged = new Dropdown.DropdownEvent();
                                            profile_dropdown.onValueChanged.AddListener(new System.Action<int>((_) => { Update_Profile(); }));
                                        }
                                    }
                                }
                                GameObject _2_obj = Functions.GetChild(top_content_obj, "2");
                                if (!_2_obj.IsNullOrDestroyed())
                                {
                                    GameObject refresh_obj = Functions.GetChild(_2_obj, "Refresh");
                                    if (!refresh_obj.IsNullOrDestroyed())
                                    {
                                        GameObject btn_obj = Functions.GetChild(refresh_obj, "Button");
                                        if (!btn_obj.IsNullOrDestroyed())
                                        {
                                            refresh_btn = btn_obj.GetComponent<Button>();
                                        }
                                    }
                                }
                                _3_obj = Functions.GetChild(top_content_obj, "3");
                                if (!_3_obj.IsNullOrDestroyed())
                                {
                                    GameObject buildname_obj = Functions.GetChild(_3_obj, "BuildName");
                                    if (!buildname_obj.IsNullOrDestroyed())
                                    {
                                        GameObject buildname_text_obj = Functions.GetChild(buildname_obj, "Text");
                                        if (!buildname_text_obj.IsNullOrDestroyed())
                                        {
                                            build_name_text = buildname_text_obj.GetComponent<Text>();
                                        }
                                    }
                                    GameObject copyright_obj = Functions.GetChild(_3_obj, "Copyright");
                                    if (!copyright_obj.IsNullOrDestroyed())
                                    {
                                        GameObject madeby_obj = Functions.GetChild(copyright_obj, "MadeBy");
                                        if (!madeby_obj.IsNullOrDestroyed())
                                        {
                                            GameObject username_obj = Functions.GetChild(madeby_obj, "Username");
                                            if (!username_obj.IsNullOrDestroyed())
                                            {
                                                GameObject autorname_text_obj = Functions.GetChild(username_obj, "Text");
                                                if (!autorname_text_obj.IsNullOrDestroyed())
                                                {
                                                    autor_name_text = autorname_text_obj.GetComponent<Text>();
                                                }
                                            }
                                            GameObject social_obj = Functions.GetChild(madeby_obj, "Social");
                                            if (!social_obj.IsNullOrDestroyed())
                                            {
                                                youtube_obj = Functions.GetChild(social_obj, "Youtube");
                                                if (!youtube_obj.IsNullOrDestroyed())
                                                {
                                                    youtube_btn = youtube_obj.GetComponent<Button>();
                                                }
                                                twitch_obj = Functions.GetChild(social_obj, "Twitch");
                                                if (!twitch_obj.IsNullOrDestroyed())
                                                {
                                                    twitch_btn = twitch_obj.GetComponent<Button>();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        GameObject center_obj = Functions.GetChild(content_obj, "Center");
                        if (!center_obj.IsNullOrDestroyed())
                        {
                            GameObject l_obj = Functions.GetChild(center_obj, "L");
                            if (!l_obj.IsNullOrDestroyed())
                            {
                                GameObject c_obj = Functions.GetChild(l_obj, "Content");
                                if (!c_obj.IsNullOrDestroyed())
                                {
                                    GameObject v_obj = Functions.GetChild(c_obj, "Viewport");
                                    if (!v_obj.IsNullOrDestroyed())
                                    {
                                        l_content_obj = Functions.GetChild(v_obj, "Content");
                                        if (!l_content_obj.IsNullOrDestroyed())
                                        {
                                            GameObject classe_obj = Functions.GetChild(l_content_obj, "Classe");
                                            if (!classe_obj.IsNullOrDestroyed())
                                            {
                                                GameObject value_obj = Functions.GetChild(classe_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        classe_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(classe_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        classe_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                            GameObject level_obj = Functions.GetChild(l_content_obj, "Level");
                                            if (!level_obj.IsNullOrDestroyed())
                                            {
                                                GameObject value_obj = Functions.GetChild(level_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        level_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(level_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        level_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                            GameObject items_obj = Functions.GetChild(l_content_obj, "Items");
                                            if (!items_obj.IsNullOrDestroyed())
                                            {
                                                GameObject value_obj = Functions.GetChild(items_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        items_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(items_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        items_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                            GameObject idols_obj = Functions.GetChild(l_content_obj, "Idols");
                                            if (!idols_obj.IsNullOrDestroyed())
                                            {
                                                GameObject value_obj = Functions.GetChild(idols_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        idols_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(idols_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        idols_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                            GameObject blessings_obj = Functions.GetChild(l_content_obj, "Blessings");
                                            if (!blessings_obj.IsNullOrDestroyed())
                                            {
                                                GameObject value_obj = Functions.GetChild(blessings_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        blessings_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(blessings_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        blessings_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                            GameObject passives_obj = Functions.GetChild(l_content_obj, "Passives");
                                            if (!passives_obj.IsNullOrDestroyed())
                                            {
                                                GameObject value_obj = Functions.GetChild(passives_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        passives_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(passives_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        passives_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                            GameObject weavertree_obj = Functions.GetChild(l_content_obj, "WeaverTree");
                                            if (!weavertree_obj.IsNullOrDestroyed())
                                            {
                                                GameObject value_obj = Functions.GetChild(weavertree_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        weavertree_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(weavertree_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        weavertree_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            GameObject r_obj = Functions.GetChild(center_obj, "R");
                            if (!r_obj.IsNullOrDestroyed())
                            {
                                GameObject c_obj = Functions.GetChild(r_obj, "Content");
                                if (!c_obj.IsNullOrDestroyed())
                                {
                                    GameObject v_obj = Functions.GetChild(c_obj, "Viewport");
                                    if (!v_obj.IsNullOrDestroyed())
                                    {
                                        r_content_obj = Functions.GetChild(v_obj, "Content");
                                        if (!r_content_obj.IsNullOrDestroyed())
                                        {
                                            GameObject mainskill_obj = Functions.GetChild(r_content_obj, "MainSkill");
                                            if (!mainskill_obj.IsNullOrDestroyed())
                                            {
                                                GameObject icon_obj = Functions.GetChild(mainskill_obj, "Icon");
                                                if (!icon_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject image_obj = Functions.GetChild(icon_obj, "Value");
                                                    if (!image_obj.IsNullOrDestroyed())
                                                    {
                                                        mainskill_image = image_obj.GetComponent<Image>();
                                                    }
                                                }
                                                GameObject value_obj = Functions.GetChild(mainskill_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        mainskill_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                            }
                                            GameObject activeskills_obj = Functions.GetChild(r_content_obj, "ActiveSkills");
                                            if (!activeskills_obj.IsNullOrDestroyed())
                                            {
                                                GameObject icons_obj = Functions.GetChild(activeskills_obj, "Icons");
                                                if (!icons_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject values_obj = Functions.GetChild(icons_obj, "Values");
                                                    if (!values_obj.IsNullOrDestroyed())
                                                    {
                                                        GameObject _0_obj = Functions.GetChild(values_obj, "0");
                                                        if (!_0_obj.IsNullOrDestroyed())
                                                        {
                                                            activeskill_0_image = _0_obj.GetComponent<Image>();
                                                        }
                                                        GameObject _1_obj = Functions.GetChild(values_obj, "1");
                                                        if (!_1_obj.IsNullOrDestroyed())
                                                        {
                                                            activeskill_1_image = _1_obj.GetComponent<Image>();
                                                        }
                                                        GameObject _2_obj = Functions.GetChild(values_obj, "2");
                                                        if (!_2_obj.IsNullOrDestroyed())
                                                        {
                                                            activeskill_2_image = _2_obj.GetComponent<Image>();
                                                        }
                                                        GameObject _3_obj = Functions.GetChild(values_obj, "3");
                                                        if (!_3_obj.IsNullOrDestroyed())
                                                        {
                                                            activeskill_3_image = _3_obj.GetComponent<Image>();
                                                        }
                                                        GameObject _4_obj = Functions.GetChild(values_obj, "4");
                                                        if (!_4_obj.IsNullOrDestroyed())
                                                        {
                                                            activeskill_4_image = _4_obj.GetComponent<Image>();
                                                        }
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(activeskills_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        activeskills_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                            GameObject skilltrees_obj = Functions.GetChild(r_content_obj, "SkillTrees");
                                            if (!skilltrees_obj.IsNullOrDestroyed())
                                            {
                                                GameObject btn_obj = Functions.GetChild(skilltrees_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        skilltrees_btn = button_obj.GetComponent<Button>();
                                                    }
                                                    else { Main.logger_instance.Error("skilltrees_btn Not found"); }
                                                }
                                            }
                                            GameObject skill_0_obj = Functions.GetChild(r_content_obj, "Skill_0");
                                            if (!skill_0_obj.IsNullOrDestroyed())
                                            {
                                                GameObject icon_obj = Functions.GetChild(skill_0_obj, "Icon");
                                                if (!icon_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject image_obj = Functions.GetChild(icon_obj, "Value");
                                                    if (!image_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_0_image = image_obj.GetComponent<Image>();
                                                    }
                                                }
                                                GameObject value_obj = Functions.GetChild(skill_0_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_0_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject index_obj = Functions.GetChild(skill_0_obj, "IndexValue");
                                                if (!index_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject dropdown_obj = Functions.GetChild(index_obj, "Dropdown");
                                                    if (!dropdown_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_0_dropdown = dropdown_obj.GetComponent<Dropdown>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(skill_0_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_0_btn = button_obj.GetComponent<Button>();
                                                    }
                                                    else { Main.logger_instance.Error("skill_0_btn Not found"); }
                                                }
                                            }
                                            GameObject skill_1_obj = Functions.GetChild(r_content_obj, "Skill_1");
                                            if (!skill_1_obj.IsNullOrDestroyed())
                                            {
                                                GameObject icon_obj = Functions.GetChild(skill_1_obj, "Icon");
                                                if (!icon_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject image_obj = Functions.GetChild(icon_obj, "Value");
                                                    if (!image_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_1_image = image_obj.GetComponent<Image>();
                                                    }
                                                }
                                                GameObject value_obj = Functions.GetChild(skill_1_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_1_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject index_obj = Functions.GetChild(skill_1_obj, "IndexValue");
                                                if (!index_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject dropdown_obj = Functions.GetChild(index_obj, "Dropdown");
                                                    if (!dropdown_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_1_dropdown = dropdown_obj.GetComponent<Dropdown>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(skill_1_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_1_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                            GameObject skill_2_obj = Functions.GetChild(r_content_obj, "Skill_2");
                                            if (!skill_2_obj.IsNullOrDestroyed())
                                            {
                                                GameObject icon_obj = Functions.GetChild(skill_2_obj, "Icon");
                                                if (!icon_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject image_obj = Functions.GetChild(icon_obj, "Value");
                                                    if (!image_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_2_image = image_obj.GetComponent<Image>();
                                                    }
                                                }
                                                GameObject value_obj = Functions.GetChild(skill_2_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_2_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject index_obj = Functions.GetChild(skill_2_obj, "IndexValue");
                                                if (!index_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject dropdown_obj = Functions.GetChild(index_obj, "Dropdown");
                                                    if (!dropdown_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_2_dropdown = dropdown_obj.GetComponent<Dropdown>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(skill_2_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_2_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                            GameObject skill_3_obj = Functions.GetChild(r_content_obj, "Skill_3");
                                            if (!skill_3_obj.IsNullOrDestroyed())
                                            {
                                                GameObject icon_obj = Functions.GetChild(skill_3_obj, "Icon");
                                                if (!icon_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject image_obj = Functions.GetChild(icon_obj, "Value");
                                                    if (!image_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_3_image = image_obj.GetComponent<Image>();
                                                    }
                                                }
                                                GameObject value_obj = Functions.GetChild(skill_3_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_3_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject index_obj = Functions.GetChild(skill_3_obj, "IndexValue");
                                                if (!index_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject dropdown_obj = Functions.GetChild(index_obj, "Dropdown");
                                                    if (!dropdown_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_3_dropdown = dropdown_obj.GetComponent<Dropdown>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(skill_3_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_3_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                            GameObject skill_4_obj = Functions.GetChild(r_content_obj, "Skill_4");
                                            if (!skill_4_obj.IsNullOrDestroyed())
                                            {
                                                GameObject icon_obj = Functions.GetChild(skill_4_obj, "Icon");
                                                if (!icon_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject image_obj = Functions.GetChild(icon_obj, "Value");
                                                    if (!image_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_4_image = image_obj.GetComponent<Image>();
                                                    }
                                                }
                                                GameObject value_obj = Functions.GetChild(skill_4_obj, "Value");
                                                if (!value_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject text_obj = Functions.GetChild(value_obj, "Text");
                                                    if (!text_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_4_text = text_obj.GetComponent<Text>();
                                                    }
                                                }
                                                GameObject index_obj = Functions.GetChild(skill_4_obj, "IndexValue");
                                                if (!index_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject dropdown_obj = Functions.GetChild(index_obj, "Dropdown");
                                                    if (!dropdown_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_4_dropdown = dropdown_obj.GetComponent<Dropdown>();
                                                    }
                                                }
                                                GameObject btn_obj = Functions.GetChild(skill_4_obj, "Btn");
                                                if (!btn_obj.IsNullOrDestroyed())
                                                {
                                                    GameObject button_obj = Functions.GetChild(btn_obj, "Button");
                                                    if (!button_obj.IsNullOrDestroyed())
                                                    {
                                                        skill_4_btn = button_obj.GetComponent<Button>();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                public static void Set_Events()
                {
                    if (!clear_url_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(clear_url_btn, clear_url_OnClick_Action);
                    }
                    if (!refresh_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(refresh_btn, refresh_OnClick_Action);
                    }
                    if (!youtube_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(youtube_btn, youtube_OnClick_Action);
                    }
                    if (!twitch_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(twitch_btn, twitch_OnClick_Action);
                    }
                    if (!classe_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(classe_btn, classe_OnClick_Action);
                    }
                    if (!level_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(level_btn, level_OnClick_Action);
                    }
                    if (!items_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(items_btn, items_OnClick_Action);
                    }
                    if (!idols_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(idols_btn, idols_OnClick_Action);
                    }
                    if (!blessings_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(blessings_btn, blessings_OnClick_Action);
                    }

                    if (!passives_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(passives_btn, passives_OnClick_Action);
                    }

                    if (!weavertree_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(weavertree_btn, weavertree_OnClick_Action);
                    }

                    if (!activeskills_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(activeskills_btn, activeskills_OnClick_Action);
                    }

                    if (!skilltrees_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(skilltrees_btn, skilltrees_OnClick_Action);
                    }

                    if (!skill_0_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(skill_0_btn, skill_0_OnClick_Action);
                    }

                    if (!skill_1_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(skill_1_btn, skill_1_OnClick_Action);
                    }

                    if (!skill_2_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(skill_2_btn, skill_2_OnClick_Action);
                    }
                    if (!skill_3_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(skill_3_btn, skill_3_OnClick_Action);
                    }
                    if (!skill_4_btn.IsNullOrDestroyed())
                    {
                        Events.Set_Button_Event(skill_4_btn, skill_4_OnClick_Action);
                    }
                }
                public static void Set_Active(bool show)
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static void Toggle_Active()
                {
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        bool show = !content_obj.active;
                        content_obj.active = show;
                        enable = show;
                    }
                }
                public static void Hide()
                {
                    if (!profile_text_obj.IsNullOrDestroyed()) { profile_text_obj.active = false; }
                    if (!profile_dropdown_obj.IsNullOrDestroyed()) { profile_dropdown_obj.active = false; }
                    if (!_3_obj.IsNullOrDestroyed()) { _3_obj.active = false; }
                    if (!l_content_obj.IsNullOrDestroyed()) { l_content_obj.active = false; }
                    if (!r_content_obj.IsNullOrDestroyed()) { r_content_obj.active = false; }
                    show = false;
                }
                public static void Show()
                {
                    show = true;
                    loading = true;
                    //set profile_dropdown
                    if (!profile_text_obj.IsNullOrDestroyed()) { profile_text_obj.active = true; }
                    if (!profile_dropdown.IsNullOrDestroyed())
                    {
                        if (!update)
                        {
                            profile_dropdown.options.Clear();
                            foreach (string s in Maxroll_import.Data.profile_names)
                            {
                                profile_dropdown.options.Add(new Dropdown.OptionData { text = s });
                            }
                        }
                        update = false;
                        profile_dropdown.value = Maxroll_import.Data.selected_profile;
                    }
                    if (!profile_dropdown_obj.IsNullOrDestroyed()) { profile_dropdown_obj.active = true; }
                    //Set Copyright
                    if (!build_name_text.IsNullOrDestroyed()) { build_name_text.text = Maxroll_import.Data.build_name; }
                    if (!autor_name_text.IsNullOrDestroyed()) { autor_name_text.text = Maxroll_import.Data.autor_name; }
                    if (!youtube_obj.IsNullOrDestroyed()) { youtube_obj.active = Maxroll_import.Data.youtube; }
                    youtube_url = Maxroll_import.Data.youtube_url;
                    if (!twitch_obj.IsNullOrDestroyed()) { twitch_obj.active = Maxroll_import.Data.twitch; }
                    twitch_url = Maxroll_import.Data.twitch_url;
                    if (!_3_obj.IsNullOrDestroyed()) { _3_obj.active = true; }
                    //Set l_content
                    string class_name = Maxroll_import.Data.class_name;
                    if (!classe_text.IsNullOrDestroyed()) { classe_text.text = class_name; }
                    bool IsClass = false;
                    if ((!classe_btn.IsNullOrDestroyed()) && (!Refs_Manager.character_class_list.IsNullOrDestroyed()) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
                    {
                        bool active = true;
                        int i = 0;
                        foreach (CharacterClass char_class in Refs_Manager.character_class_list.classes)
                        {
                            if (i == Refs_Manager.player_data.CharacterClass)
                            {
                                if (char_class.className == class_name) { active = false; }
                                break;
                            }
                            i++;
                        }
                        classe_btn.gameObject.active = active;
                        IsClass = !active;
                    }
                    int character_level = Maxroll_import.Data.character_level;
                    if (!level_text.IsNullOrDestroyed()) { level_text.text = character_level.ToString(); }
                    if ((!level_btn.IsNullOrDestroyed()) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
                    {
                        bool active = true;
                        if (Refs_Manager.player_data.Level >= character_level) { active = false; }
                        level_btn.gameObject.active = active;
                    }
                    int nb_items = Maxroll_import.Data.nb_items;
                    if (!items_text.IsNullOrDestroyed()) { items_text.text = "Count = " + nb_items; }
                    if (!items_btn.IsNullOrDestroyed())
                    {
                        bool active = false;
                        if (nb_items > 0) { active = true; }
                        items_btn.gameObject.active = active;
                    }
                    int nb_idols = Maxroll_import.Data.nb_idols;
                    if (!idols_text.IsNullOrDestroyed()) { idols_text.text = "Count = " + nb_idols; }
                    if (!idols_btn.IsNullOrDestroyed())
                    {
                        bool active = false;
                        if (nb_idols > 0) { active = true; }
                        idols_btn.gameObject.active = active;
                    }
                    int nb_blessings = Maxroll_import.Data.nb_blessings;
                    if (!blessings_text.IsNullOrDestroyed()) { blessings_text.text = "Count = " + nb_blessings; }
                    if (!blessings_btn.IsNullOrDestroyed())
                    {
                        bool active = false;
                        if (nb_blessings > 0) { active = true; }
                        blessings_btn.gameObject.active = active;
                    }
                    int nb_passives = Maxroll_import.Data.nb_passives;
                    if (!passives_text.IsNullOrDestroyed()) { passives_text.text = nb_passives + " points"; }
                    if (!passives_btn.IsNullOrDestroyed())
                    {
                        bool active = false;
                        if (nb_passives > 0) { active = true; }
                        passives_btn.gameObject.active = active;
                    }
                    int nb_weavertree = Maxroll_import.Data.nb_weavertree;
                    if (!weavertree_text.IsNullOrDestroyed()) { weavertree_text.text = nb_weavertree + " points"; }
                    if (!weavertree_btn.IsNullOrDestroyed())
                    {
                        bool active = false;
                        if (nb_weavertree > 0) { active = true; }
                        weavertree_btn.gameObject.active = active;
                    }
                    if (!l_content_obj.IsNullOrDestroyed()) { l_content_obj.active = true; }
                    //Set r_content
                    if (!mainskill_text.IsNullOrDestroyed()) { mainskill_text.text = Maxroll_import.Data.mainskill_name; }
                    if (!mainskill_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.mainskill_icon.IsNullOrDestroyed())
                        {
                            mainskill_image.gameObject.active = true;
                            mainskill_image.sprite = Maxroll_import.Data.mainskill_icon;
                        }
                        else { mainskill_image.gameObject.active = false; }
                    }
                    if (!activeskill_0_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.active_icons[0].IsNullOrDestroyed())
                        {
                            activeskill_0_image.gameObject.active = true;
                            activeskill_0_image.sprite = Maxroll_import.Data.active_icons[0];
                        }
                        else { activeskill_0_image.gameObject.active = false; }                            
                    }
                    if (!activeskill_1_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.active_icons[1].IsNullOrDestroyed())
                        {
                            activeskill_1_image.gameObject.active = true;
                            activeskill_1_image.sprite = Maxroll_import.Data.active_icons[1];
                        }
                        else { activeskill_1_image.gameObject.active = false; }
                    }
                    if (!activeskill_2_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.active_icons[2].IsNullOrDestroyed())
                        {
                            activeskill_2_image.gameObject.active = true;
                            activeskill_2_image.sprite = Maxroll_import.Data.active_icons[2];
                        }
                        else { activeskill_2_image.gameObject.active = false; }
                    }
                    if (!activeskill_3_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.active_icons[3].IsNullOrDestroyed())
                        {
                            activeskill_3_image.gameObject.active = true;
                            activeskill_3_image.sprite = Maxroll_import.Data.active_icons[3];
                        }
                        else { activeskill_3_image.gameObject.active = false; }
                    }
                    if (!activeskill_4_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.active_icons[4].IsNullOrDestroyed())
                        {
                            activeskill_4_image.gameObject.active = true;
                            activeskill_4_image.sprite = Maxroll_import.Data.active_icons[4];
                        }
                        else { activeskill_4_image.gameObject.active = false; }
                    }
                    if (!activeskills_btn.IsNullOrDestroyed())
                    {
                        if (IsClass) { activeskills_btn.gameObject.active = true; }
                        else { activeskills_btn.gameObject.active = false; }
                    }
                    if (skilltrees_btn.IsNullOrDestroyed())
                    {
                        if (IsClass) { skilltrees_btn.gameObject.active = true; }
                        else { skilltrees_btn.gameObject.active = false; }
                    }
                    if (skill_0_btn.IsNullOrDestroyed())
                    {
                        if (IsClass) { skill_0_btn.gameObject.active = true; }
                        else { skill_0_btn.gameObject.active = false; }
                    }
                    if (!skill_0_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.specialized_icons[0].IsNullOrDestroyed())
                        {
                            skill_0_image.gameObject.active = true;
                            skill_0_image.sprite = Maxroll_import.Data.specialized_icons[0];
                        }
                        else { skill_0_image.gameObject.active = false; }
                    }
                    if (!skill_0_text.IsNullOrDestroyed())
                    {
                        skill_0_text.text = Maxroll_import.Data.specialized_names[0];                        
                        if (!skill_0_btn.IsNotNullOrDestroyed())
                        {
                            bool active = false;
                            if (skill_0_text.text != "") { active = true; }
                            skill_0_btn.gameObject.active = active;
                        }
                    }
                    if (skill_1_btn.IsNullOrDestroyed())
                    {
                        if (IsClass) { skill_1_btn.gameObject.active = true; }
                        else { skill_1_btn.gameObject.active = false; }
                    }
                    if (!skill_1_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.specialized_icons[1].IsNullOrDestroyed())
                        {
                            skill_1_image.gameObject.active = true;
                            skill_1_image.sprite = Maxroll_import.Data.specialized_icons[1];
                        }
                        else { skill_1_image.gameObject.active = false; }
                    }
                    if (!skill_1_text.IsNullOrDestroyed())
                    {
                        skill_1_text.text = Maxroll_import.Data.specialized_names[1];
                        if (!skill_1_btn.IsNotNullOrDestroyed())
                        {
                            bool active = false;
                            if (skill_1_text.text != "") { active = true; }
                            skill_1_btn.gameObject.active = active;
                        }
                    }
                    if (skill_2_btn.IsNullOrDestroyed())
                    {
                        if (IsClass) { skill_2_btn.gameObject.active = true; }
                        else { skill_2_btn.gameObject.active = false; }
                    }
                    if (!skill_2_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.specialized_icons[2].IsNullOrDestroyed())
                        {
                            skill_2_image.gameObject.active = true;
                            skill_2_image.sprite = Maxroll_import.Data.specialized_icons[2];
                        }
                        else { skill_2_image.gameObject.active = false; }
                    }
                    if (!skill_2_text.IsNullOrDestroyed())
                    {
                        skill_2_text.text = Maxroll_import.Data.specialized_names[2];
                        if (!skill_2_btn.IsNotNullOrDestroyed())
                        {
                            bool active = false;
                            if (skill_2_text.text != "") { active = true; }
                            skill_2_btn.gameObject.active = active;
                        }
                    }
                    if (skill_3_btn.IsNullOrDestroyed())
                    {
                        if (IsClass) { skill_3_btn.gameObject.active = true; }
                        else { skill_3_btn.gameObject.active = false; }
                    }
                    if (!skill_3_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.specialized_icons[3].IsNullOrDestroyed())
                        {
                            skill_3_image.gameObject.active = true;
                            skill_3_image.sprite = Maxroll_import.Data.specialized_icons[3];
                        }
                        else { skill_3_image.gameObject.active = false; }
                    }
                    if (!skill_3_text.IsNullOrDestroyed())
                    {
                        skill_3_text.text = Maxroll_import.Data.specialized_names[3];
                        if (!skill_3_btn.IsNotNullOrDestroyed())
                        {
                            bool active = false;
                            if (skill_3_text.text != "") { active = true; }
                            skill_3_btn.gameObject.active = active;
                        }
                    }
                    if (skill_4_btn.IsNullOrDestroyed())
                    {
                        if (IsClass) { skill_4_btn.gameObject.active = true; }
                        else { skill_4_btn.gameObject.active = false; }
                    }
                    if (!skill_4_image.IsNullOrDestroyed())
                    {
                        if (!Maxroll_import.Data.specialized_icons[4].IsNullOrDestroyed())
                        {
                            skill_4_image.gameObject.active = true;
                            skill_4_image.sprite = Maxroll_import.Data.specialized_icons[4];
                        }
                        else { skill_4_image.gameObject.active = false; }
                    }
                    if (!skill_4_text.IsNullOrDestroyed())
                    {
                        skill_4_text.text = Maxroll_import.Data.specialized_names[4];
                        if (!skill_0_btn.IsNotNullOrDestroyed())
                        {
                            bool active = false;
                            if (skill_4_text.text != "") { active = true; }
                            skill_4_btn.gameObject.active = active;
                        }
                    }
                    if (!r_content_obj.IsNullOrDestroyed()) { r_content_obj.active = true; }
                    loading = false;
                }
            }
        }
    }
}