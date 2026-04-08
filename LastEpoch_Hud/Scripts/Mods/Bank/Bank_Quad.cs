using HarmonyLib;
using Il2Cpp;
using Il2CppLE.Data;
using Il2CppTMPro;
using MelonLoader;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.Mods.Bank
{
    [RegisterTypeInIl2Cpp]
    public class Bank_Quad : MonoBehaviour
    {
        public Bank_Quad(System.IntPtr ptr) : base(ptr) { }
        public static Bank_Quad instance { get; private set; }

        public static int character_index = -1;
        public static int backup_active_tab = -1;
        public static Vector2Int default_size = new Vector2Int(12, 17);
        public static Sprite default_grid = null;
        public static Vector2Int quad_size = new Vector2Int(24, 34);
        public static Sprite quad_grid = null;
        //public static UIPanel stash_panel = null;
        public static GameObject stash_panel = null;
        public static StashItemContainer stash_item_container = null;
        public static StashItemContainerUI stash_item_container_ui = null;
        public static Image stash_grid_image = null;
        public static System.Collections.Generic.List<bool[]> occupied_slots = null;
        public static ItemContainersManager item_contenairs_manager = null;

        //configure
        public static ConfigureTabUI configure_tab_ui = null;
        public static GameObject QuadStash_obj = null;
        public static string toggle_str = "Quad Stash";
        public static string toggle_explain_str = "ReOpen this tab to take effect";
        public static Toggle configure_stash_toggle = null;
        public static TextMeshProUGUI configure_stash_toggle_title = null;
        public static TextMeshProUGUI configure_stash_toggle_explanation = null;
        public static string configure_stash_name_backup = "";
        public static bool open_configure = false;

        void Awake()
        {
            instance = this;

        }
        void Update()
        {
            if (Scenes.IsGameScene())
            {
                Get.Refs();
                UpdateUI();
            }
            //else { stash_panel = null; }
        }

        public static void UpdateUI()
        {
            if ((!stash_panel.IsNullOrDestroyed()) && (!stash_item_container.IsNullOrDestroyed()))
            {
                if (stash_panel.active)
                //if (stash_panel.isOpen)
                {
                    if (backup_active_tab != stash_item_container.CurrentlyActiveTab) //Tab Changed
                    {
                        backup_active_tab = stash_item_container.CurrentlyActiveTab;
                        if (!stash_grid_image.IsNullOrDestroyed())
                        {
                            if (Get.IsQuadStash()) { stash_grid_image.sprite = quad_grid; }
                            else { stash_grid_image.sprite = default_grid; }
                        }
                    }
                }
                else { backup_active_tab = -1; }
            }
            if (!configure_tab_ui.IsNullOrDestroyed())
            {
                if ((open_configure) && (configure_tab_ui.gameObject.active)) //DoOnce
                {
                    open_configure = false;
                    configure_stash_name_backup = configure_tab_ui.nameInputTMP.text; //set backup name
                    if (!configure_stash_toggle_title.IsNullOrDestroyed())
                    {
                        if (configure_stash_toggle_title.text != toggle_str)
                        {
                            configure_stash_toggle_title.text = toggle_str;
                        }
                    }
                    if (!configure_stash_toggle_explanation.IsNullOrDestroyed())
                    {
                        if (configure_stash_toggle_explanation.text != toggle_explain_str)
                        {
                            configure_stash_toggle_explanation.text = toggle_explain_str;
                        }
                    }
                    if (!configure_stash_toggle.IsNullOrDestroyed())
                    {
                        configure_stash_toggle.isOn = Save.Data.UserTabs.names.Contains(configure_tab_ui.nameInputTMP.text);
                    }
                }
            }
        }

        public class Get
        {
            public static void Refs()
            {
                if (!Refs_Manager.stash_panel_ui.IsNullOrDestroyed())
                {
                    if (stash_panel.IsNullOrDestroyed())
                    {
                        stash_panel = Refs_Manager.stash_panel_ui.gameObject;
                    }
                    if ((!stash_panel.IsNullOrDestroyed()) && ((stash_grid_image.IsNullOrDestroyed()) || (default_grid.IsNullOrDestroyed())))
                    {
                        GameObject left_obj = Functions.GetChild(Refs_Manager.stash_panel_ui.gameObject, "left-container");
                        if (!left_obj.IsNullOrDestroyed())
                        {
                            if (stash_grid_image.IsNullOrDestroyed())
                            {
                                GameObject grid_obj = Functions.GetChild(left_obj, "grid-img");
                                if (!grid_obj.IsNullOrDestroyed()) { stash_grid_image = grid_obj.GetComponent<Image>(); }
                            }
                            if ((!stash_grid_image.IsNullOrDestroyed()) && (default_grid.IsNullOrDestroyed()))
                            {
                                default_grid = stash_grid_image.activeSprite;
                                Object.DontDestroyOnLoad(default_grid);
                            }
                        }
                    }
                    if ((!Hud_Manager.asset_bundle.IsNullOrDestroyed()) && (quad_grid.IsNullOrDestroyed()))
                    {
                        foreach (string name in Hud_Manager.asset_bundle.GetAllAssetNames())
                        {
                            if (name.Contains("/quadstash/"))
                            {
                                if ((Functions.Check_Texture(name)) && (name.Contains("quad_grid")))
                                {
                                    Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                                    quad_grid = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                    Object.DontDestroyOnLoad(quad_grid);
                                }
                            }
                        }
                    }
                }
            }
            public static string ActiveTabName()
            {
                string r = "";
                try
                {
                    if (!stash_item_container.IsNullOrDestroyed())
                    {
                        int index = stash_item_container.CurrentlyActiveTab;
                        for (int i = 0; i < stash_item_container.LinkedStash.Tabs.Count; i++)
                        {
                            if (stash_item_container.LinkedStash.Tabs[i].TabId == index)
                            {
                                r = stash_item_container.LinkedStash.Tabs[i].DisplayName;
                                break;
                            }
                        }
                    }
                }
                catch { }

                return r;
            }
            public static string ActiveTabName(int index)
            {
                string r = "";
                try
                {
                    if (!stash_item_container.IsNullOrDestroyed())
                    {
                        for (int i = 0; i < stash_item_container.LinkedStash.Tabs.Count; i++)
                        {
                            if (stash_item_container.LinkedStash.Tabs[i].TabId == index)
                            {
                                r = stash_item_container.LinkedStash.Tabs[i].DisplayName;
                                break;
                            }
                        }
                    }
                }
                catch { }

                return r;
            }
            public static bool IsQuadStash()
            {
                bool r = false;
                try
                {
                    string tab_name = ActiveTabName();
                    if (!Save.Data.UserTabs.IsNullOrDestroyed())
                    {
                        if ((tab_name != "") && (Save.Data.UserTabs.names.Contains(tab_name))) { r = true; }
                    }
                }
                catch { }

                return r;
            }
            public static bool IsQuadStash(int index)
            {
                bool r = false;
                try
                {
                    string tab_name = ActiveTabName(index);
                    if (!Save.Data.UserTabs.IsNullOrDestroyed())
                    {
                        if ((tab_name != "") && (Save.Data.UserTabs.names.Contains(tab_name))) { r = true; }
                    }
                }
                catch { }

                return r;
            }
            public static System.Collections.Generic.List<int> SlotsPosition(Vector2Int slot_position, Vector2Int item_size)
            {
                System.Collections.Generic.List<int> positions = new System.Collections.Generic.List<int>();
                int base_position = slot_position.x + (slot_position.y * quad_size.x);
                for (int y = 0; y < item_size.y; y++)
                {
                    for (int x = 0; x < item_size.x; x++)
                    {
                        positions.Add(base_position + x + (y * quad_size.x));
                    }
                }

                return positions;
            }            
        }
        public class Save
        {
            public class Data
            {
                public static readonly string base_path = Directory.GetCurrentDirectory() + @"\Mods\LastEpoch_Hud\QuadStashs\";
                public static string filename = "QuadStashs.json";
                public static string path = "";
                public static Structures.tabs UserTabs = new Structures.tabs();

                public class Structures
                {
                    public struct tabs
                    {
                        public System.Collections.Generic.List<string> names;
                    }
                }

                public static void DefaultConfig()
                {
                    Main.logger_instance.Msg("QuadStashs : Make DefaultConfig");

                    UserTabs = new Structures.tabs
                    {
                        names = new System.Collections.Generic.List<string>()
                    };
                }
                public static void Load()
                {
                    Main.logger_instance.Msg("QuadStashs : Try to Load : " + Data.path + Data.filename);

                    if (!File.Exists(Data.path + filename))
                    {
                        DefaultConfig();
                        Save();
                    }
                    if (File.Exists(Data.path + filename))
                    {
                        try
                        {
                            Data.UserTabs = JsonConvert.DeserializeObject<Structures.tabs>(File.ReadAllText(Data.path + filename));
                            Main.logger_instance.Msg("QuadStashs : Loaded");
                        }
                        catch { Main.logger_instance.Error("QuadStashs : Error loading file : " + Data.path + filename); }
                    }
                }
                public static void Save()
                {
                    Main.logger_instance.Msg("QuadStashs : Save : " + Data.path + Data.filename);
                    string jsonString = JsonConvert.SerializeObject(Data.UserTabs, Newtonsoft.Json.Formatting.Indented);
                    if (!Directory.Exists(Data.path)) { Directory.CreateDirectory(Data.path); }
                    if (File.Exists(Data.path + Data.filename)) { File.Delete(Data.path + Data.filename); }
                    File.WriteAllText(Data.path + Data.filename, jsonString);
                }
            }
        }
        public class Hooks
        {
            [HarmonyPatch(typeof(LocalCharacterSlots), "LoadCharacterByData")]
            public class LocalCharacterSlots_LoadCharacterByData
            {
                [HarmonyPrefix]
                static void Prefix(ref LocalCharacterSlots __instance, Il2CppLE.Data.CharacterData __0)
                {
                    Cycle cycle = __0.Cycle;
                    string solo_char_name = "";
                    StashType stashType = StashType.Softcore;

                    if (__0.SoloChallenge)
                    {
                        solo_char_name = __0.CharacterName;
                        Save.Data.path = Save.Data.base_path + cycle.ToString() + @"\" + solo_char_name + @"\";
                    }
                    else
                    {
                        if (__0.Hardcore) { stashType = StashType.Hardcore; }
                        else { stashType = StashType.Softcore; }
                        Save.Data.path = Save.Data.base_path + cycle.ToString() + @"\" + stashType.ToString() + @"\";
                    }
                    Save.Data.Load();
                }
            }

            [HarmonyPatch(typeof(StashItemContainerUI), "Awake")]
            public class StashItemContainerUI_Awake
            {
                [HarmonyPostfix]
                static void Postfix(ref StashItemContainerUI __instance)
                {
                    stash_item_container_ui = __instance;
                }
            }

            [HarmonyPatch(typeof(ItemContainersManager), "Awake")]
            public class ItemContainersManager_Awake
            {
                [HarmonyPrefix]
                static void Prefix(ref ItemContainersManager __instance)
                {
                    item_contenairs_manager = __instance;
                    occupied_slots = new System.Collections.Generic.List<bool[]>();
                    stash_item_container = null;
                    backup_active_tab = -1;
                }
            }

            [HarmonyPatch(typeof(TabbedItemContainer), "AddNewTab")]
            public class TabbedItemContainer_AddNewTab
            {
                [HarmonyPrefix]
                static void Prefix(ref TabbedItemContainer __instance, ref ItemContainer __0)
                {
                    if (stash_item_container.IsNullOrDestroyed()) { stash_item_container = __instance.TryCast<StashItemContainer>(); }
                    if (!stash_item_container.IsNullOrDestroyed())
                    {
                        int index = stash_item_container.containers.Count;
                        if (Get.IsQuadStash(index)) { __0.size = quad_size; }
                        else { __0.size = default_size; }
                        bool[] occupied = new bool[(quad_size.x * quad_size.y)];
                        for (int i = 0; i < occupied.Length; i++) { occupied[i] = false; }
                        occupied_slots.Add(occupied);
                    }
                }
            }

            [HarmonyPatch(typeof(ItemContainerUIWithSearch), "Tabbed_OnActiveTabChanged")]
            public class ItemContainerUIWithSearch_Tabbed_OnActiveTabChanged
            {
                [HarmonyPrefix]
                static void Prefix(ref ItemContainerUIWithSearch __instance, ref TabbedItemContainer __0)
                {
                    if (stash_item_container.IsNullOrDestroyed()) { stash_item_container = __0.TryCast<StashItemContainer>(); }
                    if (!stash_item_container_ui.IsNullOrDestroyed())
                    {
                        stash_item_container_ui.slotSize = stash_item_container_ui.CalculateSlotSize();
                        stash_item_container_ui.ReDrawWholeContainer();
                    }
                    __instance.ReDrawWholeContainer();
                }
            }

            [HarmonyPatch(typeof(ItemContainer), "CheckSlotsOccupied")]
            public class ItemContainer_CheckSlotsOccupied
            {
                [HarmonyPrefix]
                static bool Prefix(ref ItemContainer __instance, ref bool __result, Vector2Int __0, Vector2Int __1)
                {
                    bool r = true;                    
                    if (__instance.id == ContainerID.STASH)
                    {
                        if (!stash_item_container.IsNullOrDestroyed())
                        {
                            bool occupied = false;
                            bool found = false;
                            int index = 0;
                            foreach (ItemContainer item_contenair in stash_item_container.containers)
                            {
                                if (item_contenair == __instance)
                                {
                                    found = true;
                                    break;
                                }
                                index++;
                            }
                            if (found)
                            {
                                if (index < occupied_slots.Count)
                                {
                                    foreach (int slot_index in Get.SlotsPosition(__0, __1))
                                    {
                                        if (slot_index < occupied_slots[index].Length)
                                        {                                            
                                            if (occupied_slots[index][slot_index])
                                            {
                                                occupied = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Main.logger_instance.Error("CheckSlotsOccupied : slot_index = " + slot_index + " / occupied_slots[" + index + "].Length = " + occupied_slots[index].Length);
                                            break;
                                        }
                                    }                                    
                                }
                                else { Main.logger_instance.Error("CheckSlotsOccupied : Index  = " + index + " /  occupied_slots.Count = " + occupied_slots.Count); }
                            }
                            __result = occupied;
                            r = false;
                        }
                        else { Main.logger_instance.Error("ItemContainer.CheckSlotsOccupied() stash_item_container : is null"); }
                    }

                    return r;
                }
            }

            [HarmonyPatch(typeof(ItemContainer), "SetSlotsOccupied")]
            public class ItemContainer_SetSlotsOccupied
            {
                [HarmonyPrefix]
                static bool Prefix(ref ItemContainer __instance, bool __result, Vector2Int __0, Vector2Int __1, bool __2)
                {
                    bool r = true;
                    if (__instance.id == ContainerID.STASH)
                    {
                        if (!stash_item_container.IsNullOrDestroyed())
                        {
                            bool found = false;
                            int index = 0;
                            foreach (ItemContainer item_contenair in stash_item_container.containers)
                            {
                                if (item_contenair == __instance)
                                {
                                    found = true;
                                    break;
                                }
                                index++;
                            }

                            if (found)
                            {
                                if (index < occupied_slots.Count)
                                {
                                    foreach (int slot_index in Get.SlotsPosition(__0, __1))
                                    {
                                        if (slot_index < occupied_slots[index].Length)
                                        {
                                            occupied_slots[index][slot_index] = __2;
                                        }
                                        else
                                        {
                                            Main.logger_instance.Error("SetSlotsOccupied : slot_index = " + slot_index + " / occupied_slots[" + index + "].Length = " + occupied_slots[index].Length);
                                            break;
                                        }
                                    }
                                }
                                else { Main.logger_instance.Error("SetSlotsOccupied : Index  = " + index + " /  occupied_slots.Count = " + occupied_slots.Count); }
                            }
                            r = false;
                        }
                    }

                    return r;
                }
            }

            [HarmonyPatch(typeof(ItemContainer), "GetItemsInArea")]
            public class ItemContainer_GetItemsInArea
            {
                [HarmonyPrefix]
                static bool Prefix(ItemContainer __instance, ref Il2CppSystem.Collections.Generic.HashSet<ItemContainerEntry> __result, Vector2Int __0, Vector2Int __1)
                {
                    bool r = true;
                    if ((__instance.id == ContainerID.STASH) && (__instance.size == quad_size))
                    {
                        Il2CppSystem.Collections.Generic.List<int> area_positions = new Il2CppSystem.Collections.Generic.List<int>();
                        foreach (int slot_index in Get.SlotsPosition(__0, __1)) { area_positions.Add(slot_index); }
                        Il2CppSystem.Collections.Generic.HashSet<ItemContainerEntry> item_list = new Il2CppSystem.Collections.Generic.HashSet<ItemContainerEntry>();
                        foreach (ItemContainerEntry item_container_entry in __instance.content)
                        {
                            System.Collections.Generic.List<int> item_positions = Get.SlotsPosition(item_container_entry.Position, item_container_entry.size);
                            foreach (int slot_position in item_positions)
                            {
                                if (area_positions.Contains(slot_position)) { item_list.Add(item_container_entry); }
                            }
                        }
                        __result = item_list;
                        r = false;
                    }

                    return r;
                }
            }

            [HarmonyPatch(typeof(ConfigureTabUI), "OnEnable")]
            public class ConfigureTabUI_OnEnable
            {
                [HarmonyPrepare]
                static bool Prepare()
                {
                    bool found = AccessTools.DeclaredMethod(typeof(ConfigureTabUI), "OnEnable") != null;
                    if (!found)
                        MelonLogger.Warning("ConfigureTabUI.OnEnable not found, skipping Quad Stash config UI patch");
                    return found;
                }

                [HarmonyPrefix]
                static void Prefix(ref ConfigureTabUI __instance)
                {
                    GameObject content = __instance.contents.gameObject;
                    if (!content.IsNullOrDestroyed())
                    {
                        QuadStash_obj = Functions.GetChild(content, "quad_stash_row");
                        if (QuadStash_obj.IsNullOrDestroyed())
                        {
                            GameObject priority_section = Functions.GetChild(content, "Priority Section");
                            GameObject priority = __instance.stashPriorityUI.gameObject;
                            if ((!priority_section.IsNullOrDestroyed()) && (!priority.IsNullOrDestroyed()))
                            {
                                RectTransform priority_section_rect_transf = priority_section.GetComponent<RectTransform>();
                                float row_H = priority_section_rect_transf.rect.height;
                                float margin = 20 * priority_section_rect_transf.lossyScale.x; //Fix Hight resolution scaling

                                GameObject row = Functions.GetChild(priority, "Explanation Row");
                                QuadStash_obj = UnityEngine.Object.Instantiate(row, new Vector3(row.transform.position.x, (row.transform.position.y - row_H - margin), row.transform.position.z), Quaternion.identity);
                                QuadStash_obj.name = "quad_stash_row";
                                QuadStash_obj.transform.SetParent(content.transform);

                                GameObject toogle_obj = Functions.GetChild(QuadStash_obj, "Priority Toggle");
                                if (!toogle_obj.IsNullOrDestroyed())
                                {
                                    configure_stash_toggle = toogle_obj.GetComponent<Toggle>();
                                    if (!configure_stash_toggle.IsNullOrDestroyed()) { configure_stash_toggle.onValueChanged = new Toggle.ToggleEvent(); }
                                }
                                GameObject text_obj = Functions.GetChild(QuadStash_obj, "OptionText");
                                if (!text_obj.IsNullOrDestroyed())
                                {
                                    GameObject title_obj = Functions.GetChild(text_obj, "Title");
                                    if (!title_obj.IsNullOrDestroyed())
                                    {
                                        configure_stash_toggle_title = title_obj.GetComponent<TextMeshProUGUI>();
                                    }
                                    GameObject explanation_obj = Functions.GetChild(text_obj, "Explanation");
                                    if (!explanation_obj.IsNullOrDestroyed())
                                    {
                                        configure_stash_toggle_explanation = explanation_obj.GetComponent<TextMeshProUGUI>();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(ConfigureTabUI), "OnModalOpen")]
            //[HarmonyPatch(typeof(ConfigureTabUI), "SetSelections")]
            public class ConfigureTabUI_SetSelections
            {
                [HarmonyPostfix]
                static void Postfix(ref ConfigureTabUI __instance) //, int __0, int __1, string __2, int __3, int __4, Il2CppSystem.Collections.Generic.List<string> __5, Il2CppSystem.Collections.Generic.List<int> __6, bool __7, StashPriority __8)
                {
                    configure_tab_ui = __instance;
                    open_configure = true;
                }
            }

            [HarmonyPatch(typeof(ConfigureTabUI), "ConfirmConfigure")]
            public class ConfigureTabUI_ConfirmConfigure
            {
                [HarmonyPrefix]
                static void Prefix(ref ConfigureTabUI __instance)
                {
                    if ((!Save.Data.UserTabs.IsNullOrDestroyed()) && (!configure_stash_toggle.IsNullOrDestroyed()))
                    {
                        bool save = false;
                        bool update_containers = false;
                        if (!configure_stash_toggle.isOn)
                        {
                            if (Save.Data.UserTabs.names.Contains(configure_stash_name_backup)) //remove tab name
                            {
                                System.Collections.Generic.List<string> new_names = new System.Collections.Generic.List<string>();
                                foreach (string s in Save.Data.UserTabs.names)
                                {
                                    if (s != configure_stash_name_backup) { new_names.Add(s); }
                                }
                                Save.Data.UserTabs.names = new_names;
                                save = true;
                                update_containers = true;                                
                            }
                        }
                        else
                        {
                            if (__instance.nameInputTMP.text != configure_stash_name_backup) //update tab name
                            {
                                bool updated = false;
                                for (int i = 0; i < Save.Data.UserTabs.names.Count; i++)
                                {
                                    if (Save.Data.UserTabs.names[i] == configure_stash_name_backup)
                                    {
                                        Save.Data.UserTabs.names[i] = __instance.nameInputTMP.text;
                                        updated = true;
                                    }
                                }
                                if (!updated)
                                {
                                    Save.Data.UserTabs.names.Add(__instance.nameInputTMP.text);
                                    update_containers = true;
                                }
                                save = true;
                            }
                            else if (!Save.Data.UserTabs.names.Contains(__instance.nameInputTMP.text)) //add tab name
                            {
                                Save.Data.UserTabs.names.Add(__instance.nameInputTMP.text);
                                save = true;
                                update_containers = true;                                
                            }
                        }
                        if (save) { Save.Data.Save(); }
                        if ((update_containers) && (!stash_item_container.IsNullOrDestroyed()))
                        {
                            int i = 0;
                            foreach (ItemContainer item_container in stash_item_container.containers)
                            {
                                if (Get.IsQuadStash(i)) { item_container.size = quad_size; }
                                else { item_container.size = default_size; }
                                i++;
                            }
                        }
                    }
                }
            }
        }
    }
}
