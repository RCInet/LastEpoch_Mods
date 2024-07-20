using HarmonyLib;
using MelonLoader;
using System.IO;
using UnityEngine;
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
        private static GameObject game_pause_menu = null;
        private static Canvas hud_canvas = null;
        private readonly string asset_bundle_name = "lastepochmods"; //Name of asset file
        private bool hud_initializing = false;
        private bool asset_bundle_initializing = false;
        private bool data_initializing = false;

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            Update_Hud_Scale();
            Update_Refs();

            if (!hud_object.IsNullOrDestroyed())
            {
                if ((!data_initialized) && (!data_initializing)) { Init_UserData(); } //set once
                if (IsPauseOpen())
                {
                    Update_Hud_Content();
                    hud_object.active = true;
                    Content.Set_Active();
                }
                else
                {
                    hud_object.active = false;
                    Content.Character.need_update = true;
                }
            }            
        }

        void Init_AssetsBundle()
        {
            asset_bundle_initializing = true;
            if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Initialize assets bundle"); }
            if ((Directory.Exists(asset_path)) && (File.Exists(Path.Combine(asset_path, asset_bundle_name))))
            {
                asset_bundle = AssetBundle.LoadFromFile(Path.Combine(asset_path, asset_bundle_name));
                Object.DontDestroyOnLoad(asset_bundle);
            }
            else { Main.logger_instance.Error("Hud Manager : " + asset_bundle_name + " Not Found in Assets directory"); }
            asset_bundle_initializing = false;
        }
        void Init_Hud()
        {
            hud_initializing = true;
            if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Load hud object in assets"); }
            string asset_name = "";
            foreach (string name in asset_bundle.GetAllAssetNames())
            {
                if ((Functions.Check_Prefab(name)) && (name.Contains("/hud/")) && (name.Contains("hud.prefab")))
                {
                    asset_name = name;
                    break;
                }
            }
            if (asset_name != "")
            {
                GameObject prefab_object = asset_bundle.LoadAsset(asset_name).TryCast<GameObject>();                
                if (!prefab_object.IsNullOrDestroyed())
                {
                    prefab_object.active = false; //Hide
                    prefab_object.AddComponent<UIMouseListener>(); //Block Mouse
                    prefab_object.AddComponent<WindowFocusManager>();
                    
                    //Instantiate
                    if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Instantiate hud prefab"); }
                    hud_object = Object.Instantiate(prefab_object, Vector3.zero, Quaternion.identity);
                    Object.DontDestroyOnLoad(hud_object);
                    Init_Hud_Refs();
                }
                else { Main.logger_instance.Error("Hud Manager : Prefab not found"); }
            }
            hud_initializing = false;
        }
        void Init_Hud_Refs()
        {
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
            Content.Skills.Set_Active(false);

            //Content.Headhunter.Get_Refs();
            Content.Headhunter.Set_Active(false);
        }
        void Init_UserData()
        {
            data_initializing = true;
            if ((Save_Manager.instance.initialized) && (!Save_Manager.instance.data.IsNullOrDestroyed()))
            {
                if (Main.debug) { Main.logger_instance.Msg("Hud Manager : Initialize user config"); }
                bool character = Content.Character.Init_UserData();
                bool items = Content.Items.Init_UserData();
                bool scenes = Content.Scenes.Init_UserData();
                bool skills = Content.Skills.Init_UserData();
                bool headhunter = Content.Headhunter.Init_Data();
                if ((character) && (items) && (scenes) && (skills)) // && (headhunter))
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
            if ((asset_bundle.IsNullOrDestroyed()) && (!asset_bundle_initializing)) { Init_AssetsBundle(); }
            if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
            {
                if ((game_canvas.IsNullOrDestroyed()) && (Refs_Manager.game_uibase.canvases.Count > 0)) { game_canvas = Refs_Manager.game_uibase.canvases[0]; }
                if ((game_pause_menu.IsNullOrDestroyed()) || (Hud_Base.Default_PauseMenu_Btns.IsNullOrDestroyed())) { Hud_Base.Get_DefaultPauseMenu(); }
                if (Hud_Base.Get_DefaultPauseMenu_Open()) { Hud_Base.Toogle_DefaultPauseMenu(false); }
            }
            if ((!asset_bundle.IsNullOrDestroyed()) && (hud_object.IsNullOrDestroyed()) && (!hud_initializing)) { Init_Hud(); }
        }
        void Update_Hud_Scale()
        {
            if ((!Refs_Manager.game_uibase.IsNullOrDestroyed()) && (!game_canvas.IsNullOrDestroyed()) && (!hud_canvas.IsNullOrDestroyed()))
            {
                if (hud_canvas.scaleFactor != game_canvas.scaleFactor) { hud_canvas.scaleFactor = game_canvas.scaleFactor; }
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
        }
        bool IsPauseOpen()
        {
            if (!game_pause_menu.IsNullOrDestroyed()) { return game_pause_menu.active; }
            else { return false; }
        }
        
        public class Hooks
        {
            //All Hooks have to be replace by Unity Actions
            [HarmonyPatch(typeof(Toggle), "OnPointerClick")]
            public class Toggle_OnPointerClick
            {
                [HarmonyPostfix]
                static void Postfix(ref Toggle __instance, UnityEngine.EventSystems.PointerEventData __0)
                {
                    if ((!hud_object.IsNullOrDestroyed()) && (!Save_Manager.instance.IsNullOrDestroyed()) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
                    {
                        if ((hud_object.active) && (!Save_Manager.instance.data.IsNullOrDestroyed()))
                        {
                            if (__instance.name.Contains("Toggle_Character_"))
                            {
                                switch (__instance.name)
                                {
                                    case "Toggle_Character_Data_Died": { if (!Refs_Manager.player_data.IsNullOrDestroyed()) { Refs_Manager.player_data.Died = Content.Character.Data.died_toggle.isOn; } break; }
                                    case "Toggle_Character_Data_Hardcore": { if (!Refs_Manager.player_data.IsNullOrDestroyed()) { Refs_Manager.player_data.Hardcore = Content.Character.Data.hardcore_toggle.isOn; } break; }
                                    case "Toggle_Character_Data_Masochist": { if (!Refs_Manager.player_data.IsNullOrDestroyed()) { Refs_Manager.player_data.Masochist = Content.Character.Data.masochist_toggle.isOn; } break; }
                                    case "Toggle_Character_Data_Portal": { if (!Refs_Manager.player_data.IsNullOrDestroyed()) { Refs_Manager.player_data.PortalUnlocked = Content.Character.Data.portal_toggle.isOn; } break; }
                                    case "Toggle_Character_Data_SoloChallenge": { if (!Refs_Manager.player_data.IsNullOrDestroyed()) { Refs_Manager.player_data.SoloChallenge = Content.Character.Data.solo_toggle.isOn; } break; }

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
                                    case "Toggle_Items_Pickup_AutoPickup_Materials": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Materials = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoPickup_Filters": { Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_FromFilter = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoStore_OnDrop": { Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_OnDrop = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoStore_OnInventoryOpen": { Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_OnInventoryOpen = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoStore_10sec": { Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_All10Sec = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_AutoSell_All_Hide": { Save_Manager.instance.data.Items.Pickup.Enable_AutoSell_Hide = __instance.isOn; break; }
                                    
                                    case "Toggle_Items_Pickup_Range_Pickup": { Save_Manager.instance.data.Items.Pickup.Enable_RangePickup = __instance.isOn; break; }
                                    case "Toggle_Items_Pickup_Hide_Notifications": { Save_Manager.instance.data.Items.Pickup.Enable_HideMaterialsNotifications = __instance.isOn; break; }

                                    //case "Toggle_Items_Craft_Enable": { Save_Manager.instance.data.Items.CraftingSlot.Enable_Mod = __instance.isOn; break; }
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
                    if ((!hud_object.IsNullOrDestroyed()) && (!Save_Manager.instance.IsNullOrDestroyed()))
                    {
                        if ((hud_object.active) && (!Save_Manager.instance.data.IsNullOrDestroyed()) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
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
                                            if (Save_Manager.instance.data.Items.Drop.Implicits_Min > Save_Manager.instance.data.Items.Drop.Implicits_Max)
                                            { Content.Items.Drop.implicits_slider_max.value = Save_Manager.instance.data.Items.Drop.Implicits_Min; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_Implicits_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.Implicits_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.Implicits_Max < Save_Manager.instance.data.Items.Drop.Implicits_Min)
                                            { Content.Items.Drop.implicits_slider_min.value = Save_Manager.instance.data.Items.Drop.Implicits_Max; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_ForginPotencial_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.ForginPotencial_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.ForginPotencial_Min > Save_Manager.instance.data.Items.Drop.ForginPotencial_Max)
                                            { Content.Items.Drop.forgin_potencial_slider_max.value = Save_Manager.instance.data.Items.Drop.ForginPotencial_Min; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_ForginPotencial_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.ForginPotencial_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.ForginPotencial_Max < Save_Manager.instance.data.Items.Drop.ForginPotencial_Min)
                                            { Content.Items.Drop.forgin_potencial_slider_min.value = Save_Manager.instance.data.Items.Drop.ForginPotencial_Max; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_SealTier_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.SealTier_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.SealTier_Min > Save_Manager.instance.data.Items.Drop.SealTier_Max)
                                            { Content.Items.Drop.seal_tier_slider_max.value = Save_Manager.instance.data.Items.Drop.SealTier_Min; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_SealTier_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.SealTier_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.SealTier_Max < Save_Manager.instance.data.Items.Drop.SealTier_Min)
                                            { Content.Items.Drop.seal_tier_slider_min.value = Save_Manager.instance.data.Items.Drop.SealTier_Max; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_SealValue_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.SealValue_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.SealValue_Min > Save_Manager.instance.data.Items.Drop.SealValue_Max)
                                            { Content.Items.Drop.seal_value_slider_max.value = Save_Manager.instance.data.Items.Drop.SealValue_Min; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_SealValue_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.SealValue_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.SealValue_Max < Save_Manager.instance.data.Items.Drop.SealValue_Min)
                                            { Content.Items.Drop.seal_value_slider_min.value = Save_Manager.instance.data.Items.Drop.SealValue_Max; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_NbAffixes_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixCount_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.AffixCount_Min > Save_Manager.instance.data.Items.Drop.AffixCount_Max)
                                            { Content.Items.Drop.affix_count_slider_max.value = Save_Manager.instance.data.Items.Drop.AffixCount_Min; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_NbAffixes_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixCount_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.AffixCount_Max < Save_Manager.instance.data.Items.Drop.AffixCount_Min)
                                            { Content.Items.Drop.affix_count_slider_min.value = Save_Manager.instance.data.Items.Drop.AffixCount_Max; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_AffixesTiers_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixTiers_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.AffixTiers_Min > Save_Manager.instance.data.Items.Drop.AffixTiers_Max)
                                            { Content.Items.Drop.affix_tiers_slider_max.value = Save_Manager.instance.data.Items.Drop.AffixTiers_Min; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_AffixesTiers_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixTiers_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.AffixTiers_Max < Save_Manager.instance.data.Items.Drop.AffixTiers_Min)
                                            { Content.Items.Drop.affix_tiers_slider_min.value = Save_Manager.instance.data.Items.Drop.AffixTiers_Max; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_AffixesValues_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixValues_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.AffixValues_Min > Save_Manager.instance.data.Items.Drop.AffixValues_Max)
                                            { Content.Items.Drop.affix_values_slider_max.value = Save_Manager.instance.data.Items.Drop.AffixValues_Min; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_AffixesValues_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.AffixValues_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.AffixValues_Max < Save_Manager.instance.data.Items.Drop.AffixValues_Min)
                                            { Content.Items.Drop.affix_values_slider_min.value = Save_Manager.instance.data.Items.Drop.AffixValues_Max; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_UniqueMods_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.UniqueMods_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.UniqueMods_Min > Save_Manager.instance.data.Items.Drop.UniqueMods_Max)
                                            { Content.Items.Drop.unique_mods_slider_max.value = Save_Manager.instance.data.Items.Drop.UniqueMods_Min; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_UniqueMods_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.UniqueMods_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.UniqueMods_Max < Save_Manager.instance.data.Items.Drop.UniqueMods_Min)
                                            { Content.Items.Drop.unique_mods_slider_min.value = Save_Manager.instance.data.Items.Drop.UniqueMods_Max; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_LegendaryPotencial_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min > Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max)
                                            { Content.Items.Drop.legendary_potencial_slider_max.value = Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_LegendaryPotencial_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max < Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min)
                                            { Content.Items.Drop.legendary_potencial_slider_min.value = Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_WeaverWill_Min":
                                        {
                                            Save_Manager.instance.data.Items.Drop.WeaverWill_Min = __0;
                                            if (Save_Manager.instance.data.Items.Drop.WeaverWill_Min > Save_Manager.instance.data.Items.Drop.WeaverWill_Max)
                                            { Content.Items.Drop.weaver_will_slider_max.value = Save_Manager.instance.data.Items.Drop.WeaverWill_Min; }
                                            break;
                                        }
                                    case "Slider_Items_Drop_WeaverWill_Max":
                                        {
                                            Save_Manager.instance.data.Items.Drop.WeaverWill_Max = __0;
                                            if (Save_Manager.instance.data.Items.Drop.WeaverWill_Max < Save_Manager.instance.data.Items.Drop.WeaverWill_Min)
                                            { Content.Items.Drop.weaver_will_slider_min.value = Save_Manager.instance.data.Items.Drop.WeaverWill_Max; }
                                            break;
                                        }
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

            [HarmonyPatch(typeof(Dropdown), "OnSelectItem")]
            public class Dropdown_OnSelectItem
            {
                [HarmonyPostfix]
                static void Postfix(ref Dropdown __instance, Toggle __0)
                {
                    if (!hud_object.IsNullOrDestroyed())
                    {
                        if ((hud_object.active) && (!__instance.IsNullOrDestroyed()) &&
                            (!Refs_Manager.player_data.IsNullOrDestroyed()))
                        {
                            switch (__instance.name)
                            {
                                case "Dropdown_Character_Data_Classes": { if (!Refs_Manager.player_data.IsNullOrDestroyed()) { Refs_Manager.player_data.CharacterClass = __instance.value; } break; }
                                case "Dropdown_Items_ForceDrop_Type": { Content.Items.ForceDrop.SelectType(); break; }
                                case "Dropdown_Items_ForceDrop_Rarity": { Content.Items.ForceDrop.SelectRarity(); break; }
                                case "Dropdown_Items_ForceDrop_Item": { Content.Items.ForceDrop.SelectItem(); break; }
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
                    }
                }
            }
            public static void Set_Button_Event(Button btn, UnityEngine.Events.UnityAction action)
            {
                btn.onClick = new Button.ButtonClickedEvent();
                btn.onClick.AddListener(action);
            }
            public static void Set_Toggle_Event(Toggle toggle, UnityEngine.Events.UnityAction<bool> action)
            {
                toggle.onValueChanged = new Toggle.ToggleEvent();
                toggle.onValueChanged.AddListener(action);
            }
        }
        public class Hud_Base
        {
            public static bool Initialized = false;
            public static bool Initializing = false;
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
                if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
                {
                    GameObject Draw_over_login_canvas = Functions.GetChild(UIBase.instance.gameObject, "Draw Over Login Canvas");
                    if (!Draw_over_login_canvas.IsNullOrDestroyed()) { game_pause_menu = Functions.GetChild(Draw_over_login_canvas, "Menu"); }
                    if (!game_pause_menu.IsNullOrDestroyed())
                    {
                        Default_PauseMenu_Btns = Functions.GetChild(game_pause_menu, "Menu Image");
                        Get_Refs();
                        Set_Events();
                    }   
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
                        Hud_Base.Btn_Resume = Functions.GetChild(Btns, "ResumeButton (1)").GetComponent<Button>();
                        Hud_Base.Btn_Settings = Functions.GetChild(Btns, "SettingsButton").GetComponent<Button>();
                        Hud_Base.Btn_GameGuide = Functions.GetChild(Btns, "GameButton").GetComponent<Button>();
                        Hud_Base.Btn_LeaveGame = Functions.GetChild(Btns, "ExitToCharacterSelectButton").GetComponent<Button>();
                        Hud_Base.Btn_ExitDesktop = Functions.GetChild(Btns, "ExitGameButton").GetComponent<Button>();
                    }
                }
            }            
            public static void Set_Events()
            {
                Events.Set_Base_Button_Event(hud_object, "Base", "Btn_Base_Resume", Resume_OnClick_Action);
                Events.Set_Base_Button_Event(hud_object, "Base", "Btn_Base_Settings", Settings_OnClick_Action);
                Events.Set_Base_Button_Event(hud_object, "Base", "Btn_Base_GameGuide", GameGuide_OnClick_Action);
                Events.Set_Base_Button_Event(hud_object, "Base", "Btn_Base_LeaveGame", LeaveGame_OnClick_Action);
                Events.Set_Base_Button_Event(hud_object, "Base", "Btn_Base_ExitDesktop", ExitDesktop_OnClick_Action);
            }

            private static readonly System.Action Resume_OnClick_Action = new System.Action(Resume_Click);
            public static void Resume_Click()
            {
                if ((!hud_object.gameObject.IsNullOrDestroyed()) && (!Btn_Resume.IsNullOrDestroyed()))
                {
                    Btn_Resume.onClick.Invoke();
                }
            }

            private static readonly System.Action Settings_OnClick_Action = new System.Action(Settings_Click);
            public static void Settings_Click()
            {
                if ((!hud_object.IsNullOrDestroyed()) && (!Btn_Settings.IsNullOrDestroyed()))
                {
                    Btn_Settings.onClick.Invoke();
                }
            }

            private static readonly System.Action GameGuide_OnClick_Action = new System.Action(GameGuide_Click);
            public static void GameGuide_Click()
            {
                if ((!hud_object.IsNullOrDestroyed()) && (!Btn_GameGuide.IsNullOrDestroyed()))
                {
                    Btn_GameGuide.onClick.Invoke();
                }
            }

            private static readonly System.Action LeaveGame_OnClick_Action = new System.Action(LeaveGame_Click);
            public static void LeaveGame_Click()
            {
                if ((!hud_object.IsNullOrDestroyed()) && (!Btn_LeaveGame.IsNullOrDestroyed()))
                {
                    Content.Close_AllContent();
                    Btn_LeaveGame.onClick.Invoke();
                }
            }

            private static readonly System.Action ExitDesktop_OnClick_Action = new System.Action(ExitDesktop_Click);
            public static void ExitDesktop_Click()
            {
                if ((!hud_object.IsNullOrDestroyed()) && (!Btn_ExitDesktop.IsNullOrDestroyed()))
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
                Events.Set_Base_Button_Event(hud_object, "Menu", "Btn_Menu_Character", Character_OnClick_Action);
                Events.Set_Base_Button_Event(hud_object, "Menu", "Btn_Menu_Items", Items_OnClick_Action);
                Events.Set_Base_Button_Event(hud_object, "Menu", "Btn_Menu_Scenes", Scenes_OnClick_Action);
                Events.Set_Base_Button_Event(hud_object, "Menu", "Btn_Menu_TreeSkills", Skills_OnClick_Action);
                Events.Set_Base_Button_Event(hud_object, "Menu", "Btn_Menu_Headhunter", Headhunter_OnClick_Action);
            }
            
            private static readonly System.Action Character_OnClick_Action = new System.Action(Character_Click);
            public static void Character_Click()
            {
                Content.Items.Set_Active(false);
                Content.Scenes.Set_Active(false);
                Content.Skills.Set_Active(false);
                Content.Headhunter.Set_Active(false);
                Content.Character.Toggle_Active();          
            }

            private static readonly System.Action Items_OnClick_Action = new System.Action(Items_Click);
            public static void Items_Click()
            {
                Content.Character.Set_Active(false);
                Content.Scenes.Set_Active(false);
                Content.Skills.Set_Active(false);
                Content.Headhunter.Set_Active(false);
                Content.Items.Toggle_Active();
            }

            private static readonly System.Action Scenes_OnClick_Action = new System.Action(Scenes_Click);
            public static void Scenes_Click()
            {
                Content.Character.Set_Active(false);
                Content.Items.Set_Active(false);
                Content.Skills.Set_Active(false);
                Content.Headhunter.Set_Active(false);
                Content.Scenes.Toggle_Active();
            }

            private static readonly System.Action Skills_OnClick_Action = new System.Action(Skills_Click);
            public static void Skills_Click()
            {
                Content.Character.Set_Active(false);
                Content.Items.Set_Active(false);
                Content.Scenes.Set_Active(false);             
                Content.Headhunter.Set_Active(false);
                Content.Skills.Toggle_Active();
            }

            private static readonly System.Action Headhunter_OnClick_Action = new System.Action(Headhunter_Click);
            public static void Headhunter_Click()
            {
                Content.Character.Set_Active(false);
                Content.Items.Set_Active(false);
                Content.Scenes.Set_Active(false);
                Content.Skills.Set_Active(false);                
                Content.Headhunter.Toggle_Active();
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
                    if ((Character.enable) || (Items.enable) || (Scenes.enable) || (Skills.enable))
                    { show = true; }
                    if (content_obj.active != show) { content_obj.active = show; }
                }
            }
            public static void Close_AllContent()
            {
                Character.enable = false;
                Items.enable = false;
            }

            public class Character
            {
                public static GameObject content_obj = null;
                public static bool controls_initialized = false;
                public static bool enable = false;
                public static bool need_update = true;

                public static void Get_Refs()
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

                            Cheats.waypoints_toggle = Functions.Get_ToggleInPanel(character_cheats_content, "WaypointsUnlock", "Toggle_Character_Cheats_UnlockAllWaypoints");

                            Cheats.level_once_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_LevelOnce").GetComponent<Button>();
                            Cheats.level_max_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_LevelToMax").GetComponent<Button>();
                            Cheats.complete_quest_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_CompleteQuest").GetComponent<Button>();
                            Cheats.masterie_buttons = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_Masterie").GetComponent<Button>();
                            Cheats.masterie_text = Functions.Get_TextInButton(character_cheats_content, "Btn_Character_Cheats_Masterie", "Label");
                            Cheats.add_runes_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_AddRunes").GetComponent<Button>();
                            Cheats.add_glyphs_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_AddGlyphs").GetComponent<Button>();
                            Cheats.add_shards_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_AddAffixs").GetComponent<Button>();
                            Cheats.discover_blessings_button = Functions.GetChild(character_cheats_content, "Btn_Character_Cheats_DicoverAllBlessings").GetComponent<Button>();
                        }
                        else { Main.logger_instance.Error("Hud Manager : character_cheats_content is null"); }

                        //Data
                        GameObject character_data_content = Functions.GetViewportContent(content_obj, "Character_Data", "Character_Data_Content");
                        if (!character_data_content.IsNullOrDestroyed())
                        {
                            Data.class_dropdown = Functions.Get_DopboxInPanel(character_data_content, "Classe", "Dropdown_Character_Data_Classes");
                            
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
                public static void Set_Events()
                {
                    Events.Set_Toggle_Event(Cheats.godmode_toggle, Cheats.Godmode_Toggle_Action);
                    Events.Set_Toggle_Event(Cheats.lowlife_toggle, Cheats.Lowlife_Toggle_Action);
                    Events.Set_Toggle_Event(Cheats.allow_choosing_blessing, Cheats.AllowChooseBlessings_Toggle_Action);
                    Events.Set_Toggle_Event(Cheats.unlock_all_idols, Cheats.UnlockAllIdols_Toggle_Action);                    
                    Events.Set_Toggle_Event(Cheats.autoPot_toggle, Cheats.AutoPot_Toggle_Action);

                    Events.Set_Toggle_Event(Cheats.density_toggle, Cheats.Density_Toggle_Action);

                    Events.Set_Toggle_Event(Cheats.experience_toggle, Cheats.Experience_Toggle_Action);

                    Events.Set_Toggle_Event(Cheats.ability_toggle, Cheats.Ability_Toggle_Action);

                    Events.Set_Toggle_Event(Cheats.favor_toggle, Cheats.Favor_Toggle_Action);

                    Events.Set_Toggle_Event(Cheats.itemdropmultiplier_toggle, Cheats.ItemDropMulti_Toggle_Action);

                    Events.Set_Toggle_Event(Cheats.itemdropchance_toggle, Cheats.ItemDropChance_Toggle_Action);

                    Events.Set_Toggle_Event(Cheats.golddropmultiplier_toggle, Cheats.GoldMulti_Toggle_Action);
                    
                    Events.Set_Toggle_Event(Cheats.golddropchance_toggle, Cheats.GoldChance_Toggle_Action);
                    
                    Events.Set_Toggle_Event(Cheats.waypoints_toggle, Cheats.Waypoints_Toggle_Action);
                    Events.Set_Button_Event(Cheats.level_once_button, Cheats.LevelUpOnce_OnClick_Action);
                    Events.Set_Button_Event(Cheats.level_max_button, Cheats.LevelUpMax_OnClick_Action);                    
                    Events.Set_Button_Event(Cheats.complete_quest_button, Cheats.CompleteQuest_OnClick_Action);
                    Events.Set_Button_Event(Cheats.masterie_buttons, Cheats.Masteries_OnClick_Action);
                    Events.Set_Button_Event(Cheats.add_runes_button, Cheats.AddRunes_OnClick_Action);
                    Events.Set_Button_Event(Cheats.add_glyphs_button, Cheats.AddGlyphs_OnClick_Action);
                    Events.Set_Button_Event(Cheats.add_shards_button, Cheats.AddAffixs_OnClick_Action);
                    Events.Set_Button_Event(Cheats.discover_blessings_button, Cheats.DiscoverAllBlessings_OnClick_Action);

                    Events.Set_Button_Event(Data.save_button, Data.Save_OnClick_Action);
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
                            //Content.Character.Cheats
                            Cheats.godmode_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_GodMode;
                            Cheats.lowlife_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_LowLife;
                            Cheats.allow_choosing_blessing.isOn = Save_Manager.instance.data.Character.Cheats.Enable_CanChooseBlessing;
                            Cheats.unlock_all_idols.isOn = Save_Manager.instance.data.Character.Cheats.Enable_UnlockAllIdolsSlots;
                            
                            Cheats.autoPot_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_AutoPot;
                            Cheats.autopot_slider.value = Save_Manager.instance.data.Character.Cheats.autoPot;

                            Cheats.density_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_DensityMultiplier;
                            Cheats.density_slider.value = Save_Manager.instance.data.Character.Cheats.DensityMultiplier;

                            Cheats.experience_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_ExperienceMultiplier;
                            Cheats.experience_slider.value = Save_Manager.instance.data.Character.Cheats.ExperienceMultiplier;

                            Cheats.ability_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_AbilityMultiplier;
                            Cheats.ability_slider.value = Save_Manager.instance.data.Character.Cheats.AbilityMultiplier;

                            Cheats.favor_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_FavorMultiplier;
                            Cheats.favor_slider.value = Save_Manager.instance.data.Character.Cheats.FavorMultiplier;

                            Cheats.itemdropmultiplier_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_ItemDropMultiplier;
                            Cheats.itemdropmultiplier_slider.value = Save_Manager.instance.data.Character.Cheats.ItemDropMultiplier;

                            Cheats.itemdropchance_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_ItemDropChance;
                            Cheats.itemdropchance_slider.value = Save_Manager.instance.data.Character.Cheats.ItemDropChance;

                            Cheats.golddropmultiplier_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_GoldDropMultiplier;
                            Cheats.golddropmultiplier_slider.value = Save_Manager.instance.data.Character.Cheats.GoldDropMultiplier;

                            Cheats.golddropchance_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_GoldDropChance;
                            Cheats.golddropchance_slider.value = Save_Manager.instance.data.Character.Cheats.GoldDropChance;

                            Cheats.waypoints_toggle.isOn = Save_Manager.instance.data.Character.Cheats.Enable_WaypointsUnlock;

                            //Content.Character.Buffs
                            Buffs.enable_mod.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Mod;

                            Buffs.movespeed_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_MoveSpeed_Buff;
                            Buffs.movespeed_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.MoveSpeed_Buff_Value;

                            Buffs.damage_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Damage_Buff;
                            Buffs.damage_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Damage_Buff_Value;

                            Buffs.attackspeed_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_AttackSpeed_Buff;
                            Buffs.attackspeed_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.AttackSpeed_Buff_Value;

                            Buffs.castingspeed_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_CastSpeed_Buff;
                            Buffs.castingspeed_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.CastSpeed_Buff_Value;

                            Buffs.criticalchance_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_CriticalChance_Buff;
                            Buffs.criticalchance_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.CriticalChance_Buff_Value;

                            Buffs.criticalmultiplier_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_CriticalMultiplier_Buff;
                            Buffs.criticalmultiplier_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.CriticalMultiplier_Buff_Value;

                            Buffs.healthregen_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_HealthRegen_Buff;
                            Buffs.healthregen_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.HealthRegen_Buff_Value;

                            Buffs.manaregen_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_ManaRegen_Buff;
                            Buffs.manaregen_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.ManaRegen_Buff_Value;

                            Buffs.str_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Str_Buff;
                            Buffs.str_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Str_Buff_Value;

                            Buffs.int_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Int_Buff;
                            Buffs.int_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Int_Buff_Value;

                            Buffs.dex_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Dex_Buff;
                            Buffs.dex_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Dex_Buff_Value;

                            Buffs.vit_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Vit_Buff;
                            Buffs.vit_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Vit_Buff_Value;

                            Buffs.att_toggle.isOn = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Att_Buff;
                            Buffs.att_slider.value = Save_Manager.instance.data.Character.PermanentBuffs.Att_Buff_Value;

                            controls_initialized = true;
                            result = true;
                        }
                    }

                    return result;
                }
                public static void Update_PlayerData()
                {
                    need_update = false;
                    if ((!Refs_Manager.player_treedata.IsNullOrDestroyed()) &&
                        (!Refs_Manager.character_class_list.IsNullOrDestroyed()) &&
                        (!Refs_Manager.player_data.IsNullOrDestroyed()))
                    {
                        Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> options = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
                        foreach (CharacterClass char_class in Refs_Manager.character_class_list.classes)
                        {
                            options.Add(new Dropdown.OptionData { text = char_class.className });
                        }
                        Data.class_dropdown.options = options;

                        Data.class_dropdown.value = Refs_Manager.player_data.CharacterClass;
                        Data.died_toggle.isOn = Refs_Manager.player_data.Died;
                        Data.deaths_slider.value = Refs_Manager.player_data.Deaths;
                        Data.deaths_text.text = Refs_Manager.player_data.Deaths.ToString();
                        Data.hardcore_toggle.isOn = Refs_Manager.player_data.Hardcore;
                        Data.masochist_toggle.isOn = Refs_Manager.player_data.Masochist;
                        Data.portal_toggle.isOn = Refs_Manager.player_data.PortalUnlocked;
                        Data.solo_toggle.isOn = Refs_Manager.player_data.SoloChallenge;
                        Data.lantern_slider.value = Refs_Manager.player_data.LanternLuminance;
                        Data.lantern_text.text = Refs_Manager.player_data.LanternLuminance.ToString();
                        Data.soul_slider.value = Refs_Manager.player_data.SoulEmbers;
                        Data.soul_text.text = Refs_Manager.player_data.SoulEmbers.ToString();
                    }
                }
                public static void UpdateVisuals()
                {
                    if ((!Save_Manager.instance.IsNullOrDestroyed()) && (controls_initialized))
                    {
                        if ((Save_Manager.instance.initialized) && (!Save_Manager.instance.data.IsNullOrDestroyed()))
                        {
                            Cheats.autopot_text.text = (int)((Save_Manager.instance.data.Character.Cheats.autoPot / 255) * 100) + " %";
                            Cheats.density_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.DensityMultiplier);
                            Cheats.experience_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.ExperienceMultiplier);
                            Cheats.ability_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.AbilityMultiplier);
                            Cheats.favor_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.FavorMultiplier);
                            Cheats.itemdropmultiplier_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.ItemDropMultiplier);
                            Cheats.itemdropchance_text.text = "+ " + (int)((Save_Manager.instance.data.Character.Cheats.ItemDropChance / 255) * 100) + " %";
                            Cheats.golddropmultiplier_text.text = "x " + (int)(Save_Manager.instance.data.Character.Cheats.GoldDropMultiplier);
                            Cheats.golddropchance_text.text = "+ " + (int)((Save_Manager.instance.data.Character.Cheats.GoldDropChance / 255) * 100) + " %";
                            
                            Buffs.movespeed_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.MoveSpeed_Buff_Value * 100) + " %";
                            Buffs.damage_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Damage_Buff_Value * 100) + " %";
                            Buffs.attackspeed_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.AttackSpeed_Buff_Value * 100) + " %";
                            Buffs.castingspeed_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.CastSpeed_Buff_Value * 100) + " %";
                            int crit_chance = 0;
                            if (Save_Manager.instance.data.Character.PermanentBuffs.CriticalChance_Buff_Value > 0)
                            {
                                crit_chance = (int)(Save_Manager.instance.data.Character.PermanentBuffs.CriticalChance_Buff_Value * 100) + 1;
                            }
                            Buffs.criticalchance_text.text = "+ " + crit_chance + " %";
                            Buffs.criticalmultiplier_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.CriticalMultiplier_Buff_Value * 100) + " %";
                            Buffs.healthregen_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.HealthRegen_Buff_Value * 100) + " %";
                            Buffs.manaregen_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.ManaRegen_Buff_Value * 100) + " %";
                            Buffs.str_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Str_Buff_Value) + " %";
                            Buffs.int_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Int_Buff_Value) + " %";
                            Buffs.dex_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Dex_Buff_Value) + " %";
                            Buffs.vit_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Vit_Buff_Value) + " %";
                            Buffs.att_text.text = "+ " + (int)(Save_Manager.instance.data.Character.PermanentBuffs.Att_Buff_Value) + " %";
                        }
                    }
                }

                public class Cheats
                {
                    public static Toggle godmode_toggle = null;
                    public static readonly System.Action<bool> Godmode_Toggle_Action = new System.Action<bool>(Set_Godmode_Enable);
                    private static void Set_Godmode_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_GodMode = enable;
                    }

                    public static Toggle lowlife_toggle = null;
                    public static readonly System.Action<bool> Lowlife_Toggle_Action = new System.Action<bool>(Set_Lowlife_Enable);
                    private static void Set_Lowlife_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_LowLife = enable;
                    }

                    public static Toggle allow_choosing_blessing = null;
                    public static readonly System.Action<bool> AllowChooseBlessings_Toggle_Action = new System.Action<bool>(Set_AllowChooseBlessings_Enable);
                    private static void Set_AllowChooseBlessings_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_CanChooseBlessing = enable;
                    }

                    public static Toggle unlock_all_idols = null;
                    public static readonly System.Action<bool> UnlockAllIdols_Toggle_Action = new System.Action<bool>(Set_UnlockAllIdols_Enable);
                    private static void Set_UnlockAllIdols_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_UnlockAllIdolsSlots = enable;
                        Mods.Character.Character_UnlockAllIdols.Update();
                    }

                    public static Toggle autoPot_toggle = null;
                    public static readonly System.Action<bool> AutoPot_Toggle_Action = new System.Action<bool>(Set_AutoPot_Enable);
                    private static void Set_AutoPot_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_AutoPot = enable;
                    }
                    public static Text autopot_text = null;
                    public static Slider autopot_slider = null;
                                        
                    public static Toggle density_toggle = null;
                    public static readonly System.Action<bool> Density_Toggle_Action = new System.Action<bool>(Set_Density_Enable);
                    private static void Set_Density_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_DensityMultiplier = enable;
                    }
                    public static Text density_text = null;
                    public static Slider density_slider = null;
                    
                    public static Toggle experience_toggle = null;
                    public static readonly System.Action<bool> Experience_Toggle_Action = new System.Action<bool>(Set_Experience_Enable);
                    private static void Set_Experience_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_ExperienceMultiplier = enable;
                    }
                    public static Text experience_text = null;
                    public static Slider experience_slider = null;
                    
                    public static Toggle ability_toggle = null;
                    public static readonly System.Action<bool> Ability_Toggle_Action = new System.Action<bool>(Set_Ability_Enable);
                    private static void Set_Ability_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_AbilityMultiplier = enable;
                    }
                    public static Text ability_text = null;
                    public static Slider ability_slider = null;
                    
                    public static Toggle favor_toggle = null;
                    public static readonly System.Action<bool> Favor_Toggle_Action = new System.Action<bool>(Set_Favor_Enable);
                    private static void Set_Favor_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_FavorMultiplier = enable;
                    }
                    public static Text favor_text = null;
                    public static Slider favor_slider = null;
                    
                    public static Toggle itemdropmultiplier_toggle = null;
                    public static readonly System.Action<bool> ItemDropMulti_Toggle_Action = new System.Action<bool>(Set_ItemDropMulti_Enable);
                    private static void Set_ItemDropMulti_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_ItemDropMultiplier = enable;
                    }
                    public static Text itemdropmultiplier_text = null;
                    public static Slider itemdropmultiplier_slider = null;
                    
                    public static Toggle itemdropchance_toggle = null;
                    public static readonly System.Action<bool> ItemDropChance_Toggle_Action = new System.Action<bool>(Set_ItemDropChance_Enable);
                    private static void Set_ItemDropChance_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_ItemDropChance = enable;
                    }
                    public static Text itemdropchance_text = null;
                    public static Slider itemdropchance_slider = null;
                    
                    public static Toggle golddropmultiplier_toggle = null;
                    public static readonly System.Action<bool> GoldMulti_Toggle_Action = new System.Action<bool>(Set_GoldMulti_Enable);
                    private static void Set_GoldMulti_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_GoldDropMultiplier = enable;
                    }
                    public static Text golddropmultiplier_text = null;
                    public static Slider golddropmultiplier_slider = null;
                    
                    public static Toggle golddropchance_toggle = null;
                    public static readonly System.Action<bool> GoldChance_Toggle_Action = new System.Action<bool>(Set_GoldChance_Enable);
                    private static void Set_GoldChance_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_GoldDropChance = enable;
                    }
                    public static Text golddropchance_text = null;
                    public static Slider golddropchance_slider = null;

                    public static Toggle waypoints_toggle = null;
                    public static readonly System.Action<bool> Waypoints_Toggle_Action = new System.Action<bool>(Set_Waypoints_Enable);
                    private static void Set_Waypoints_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Character.Cheats.Enable_WaypointsUnlock = enable;
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
                    public static Button save_button = null;

                    public static readonly System.Action Save_OnClick_Action = new System.Action(Save_Click);
                    public static void Save_Click()
                    {
                        if (!Refs_Manager.player_data.IsNullOrDestroyed()) { Refs_Manager.player_data.SaveData(); }
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
                    content_obj = Functions.GetChild(Content.content_obj, "Items_Content");
                    if (!content_obj.IsNullOrDestroyed())
                    {
                        GameObject items_drop_content = Functions.GetViewportContent(content_obj, "Items_Drop", "Items_Data_Content");
                        if (!items_drop_content.IsNullOrDestroyed())
                        {
                            Drop.force_unique_toggle = Functions.Get_ToggleInPanel(items_drop_content, "ForceUnique", "Toggle_Items_Drop_ForceUnique");
                            Drop.force_set_toggle = Functions.Get_ToggleInPanel(items_drop_content, "ForceSet", "Toggle_Items_Drop_ForceSet");
                            Drop.force_legendary_toggle = Functions.Get_ToggleInPanel(items_drop_content, "ForceLegendary", "Toggle_Items_Drop_ForceLegendary");

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
                            Pickup.autopickup_materials_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_Materials", "Toggle_Items_Pickup_AutoPickup_Materials");
                            Pickup.autopickup_fromfilter_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoPickup_Filters", "Toggle_Items_Pickup_AutoPickup_Filters");
                            
                            Pickup.autostore_materials_ondrop_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoStore_OnDrop", "Toggle_Items_Pickup_AutoStore_OnDrop");
                            Pickup.autostore_materials_oninventoryopen_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoStore_OnInventoryOpen", "Toggle_Items_Pickup_AutoStore_OnInventoryOpen");
                            Pickup.autostore_materials_all10sec_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoStore_10sec", "Toggle_Items_Pickup_AutoStore_10sec");
                            
                            Pickup.autosell_hide_toggle = Functions.Get_ToggleInPanel(items_pickup_content, "AutoSell_All_Hide", "Toggle_Items_Pickup_AutoSell_All_Hide");                            
                            
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
                            ForceDrop.forcedrop_type_dropdown = Functions.Get_DopboxInPanel(items_forcedrop_content, "Type", "Dropdown_Items_ForceDrop_Type");
                            ForceDrop.forcedrop_rarity_dropdown = Functions.Get_DopboxInPanel(items_forcedrop_content, "Rarity", "Dropdown_Items_ForceDrop_Rarity");
                            ForceDrop.forcedrop_items_dropdown = Functions.Get_DopboxInPanel(items_forcedrop_content, "Item", "Dropdown_Items_ForceDrop_Item");                            
                            ForceDrop.forcedrop_quantity_text = Functions.Get_TextInButton(items_forcedrop_content, "Quantity", "Value");
                            ForceDrop.forcedrop_quantity_slider = Functions.Get_SliderInPanel(items_forcedrop_content, "Quantity", "Slider_Items_ForceDrop_Quantity");
                            GameObject new_obj = Functions.GetChild(content_obj, "Items_Pickup");
                            if (!new_obj.IsNullOrDestroyed())
                            {
                                ForceDrop.forcedrop_drop_button = Functions.Get_ButtonInPanel(new_obj, "Btn_Items_ForceDrop_Drop");
                            }
                        }
                        else { Main.logger_instance.Error("Forcedrop"); }
                        
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
                            Pickup.autopickup_materials_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Materials;
                            Pickup.autopickup_fromfilter_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_FromFilter;

                            Pickup.autostore_materials_ondrop_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_OnDrop;
                            Pickup.autostore_materials_oninventoryopen_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_OnInventoryOpen;
                            Pickup.autostore_materials_all10sec_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_All10Sec;

                            Pickup.autosell_hide_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_AutoSell_Hide;                            

                            Pickup.range_pickup_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_RangePickup;
                            Pickup.hide_materials_notifications_toggle.isOn = Save_Manager.instance.data.Items.Pickup.Enable_HideMaterialsNotifications;

                            //Requirements
                            Requirements.class_req_toggle.isOn = Save_Manager.instance.data.Items.Req.classe;
                            Requirements.level_req_toggle.isOn = Save_Manager.instance.data.Items.Req.level;
                            Requirements.set_req_toggle.isOn = Save_Manager.instance.data.Items.Req.set;

                            //Craft
                            //CraftingSlot.enable_mod.isOn = Save_Manager.instance.data.Items.CraftingSlot.Enable_Mod;
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
                    public static Toggle autopickup_materials_toggle = null;
                    public static Toggle autopickup_fromfilter_toggle = null;
                    public static Toggle autostore_materials_ondrop_toggle = null;
                    public static Toggle autostore_materials_oninventoryopen_toggle = null;
                    public static Toggle autostore_materials_all10sec_toggle = null;
                    public static Toggle autosell_hide_toggle = null;
                    public static Toggle range_pickup_toggle = null;
                    public static Toggle hide_materials_notifications_toggle = null;
                }
                public class Requirements
                {
                    public static Toggle class_req_toggle = null;
                    public static readonly System.Action<bool> Class_Toggle_Action = new System.Action<bool>(Class_Enable);
                    private static void Class_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Items.Req.classe = enable;
                        //Items_Req_Class.Enable();
                    }
                    
                    public static Toggle level_req_toggle = null;
                    public static readonly System.Action<bool> Level_Toggle_Action = new System.Action<bool>(Level_Enable);
                    private static void Level_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Items.Req.level = enable;
                    }

                    public static Toggle set_req_toggle = null;
                    public static readonly System.Action<bool> Set_Toggle_Action = new System.Action<bool>(Set_Enable);
                    private static void Set_Enable(bool enable)
                    {
                        Save_Manager.instance.data.Items.Req.set = enable;
                        //Items_Req_Set.Enable();
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
                        if (Type_Initialized)
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
                        if (Type_Initialized)
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
                                                if (name == "") { name = unique.name; }
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
                        if (Type_Initialized)
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
                        if (btn_enable)
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
                                        hasSealedAffix = false
                                    };

                                    //Random
                                    if (item.itemType < 100)
                                    {
                                        for (int k = 0; k < item.implicitRolls.Count; k++) { item.implicitRolls[k] = (byte)Random.RandomRange(0f, 255f); }
                                        if (!item.isUniqueSetOrLegendary()) { item.forgingPotential = (byte)Random.RandomRange(0f, 255f); }
                                        UniqueList.LegendaryType legendary_type = UniqueList.LegendaryType.LegendaryPotential;
                                        if (item.isUniqueSetOrLegendary())
                                        {
                                            legendary_type = UniqueList.getUnique((ushort)item_unique_id).legendaryType;
                                            for (int k = 0; k < item.uniqueRolls.Count; k++) { item.uniqueRolls[k] = (byte)Random.RandomRange(0f, 255f); }
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
                public static void Set_Events()
                {
                    Events.Set_Button_Event(Camera.reset_button, Camera.Reset_OnClick_Action);
                    Events.Set_Button_Event(Camera.set_button, Camera.Set_OnClick_Action);
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
            public class Headhunter
            {
                public static GameObject content_obj = null;
                public static bool enable = false;

                public static void Get_Refs()
                {

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



                    return result;
                }
                public static void UpdateVisuals()
                {
                    if (!Save_Manager.instance.IsNullOrDestroyed())
                    {
                        if ((Save_Manager.instance.initialized) && (!Save_Manager.instance.data.IsNullOrDestroyed()))
                        {

                        }
                    }
                }
            }
        }
    }
}