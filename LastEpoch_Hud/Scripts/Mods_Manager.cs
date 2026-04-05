using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts
{
    [RegisterTypeInIl2Cpp]
    public class Mods_Manager : MonoBehaviour
    {
        public Mods_Manager(System.IntPtr ptr) : base(ptr) { }
        public static Mods_Manager instance { get; private set; }

        GameObject cosmetics_obj = null;
        GameObject damager_meter_obj = null;
        GameObject chat_obj = null;

        GameObject bank_quad_obj = null;

        GameObject character_autopotion_obj = null;
        GameObject character_bank_from_anywhere = null;
        GameObject character_blessings_obj = null;
        GameObject character_godmode_obj = null;
        GameObject character_lowlife_obj = null;
        GameObject character_masteries_obj = null;
        GameObject character_permanentbuffs_obj = null;
        GameObject character_potionreplenishment_obj = null;
        GameObject character_safetp_obj = null;
        GameObject character_twohanded_with_shield_obj = null;

        GameObject craft_max_tier_obj = null;

        GameObject items_autopickup_obj = null;        
        GameObject items_autosell_timer_obj = null;
        GameObject items_nbsocket_obj = null;

        GameObject minimap_icons_obj = null;

        GameObject monoliths_complete_objectives_obj = null;

        GameObject newitems_headhunter_obj = null;
        GameObject newitems_mjolner_obj = null;
        GameObject newitems_sandsofsilk_obj = null;
        GameObject newitems_essentiasanguis_obj = null;
        GameObject newitems_heralds_obj = null;
        GameObject newitems_arakaali_obj = null;

        GameObject spawn_timebeast_obj = null;
                
        GameObject summon_collider_obj = null;
        GameObject summon_forever_obj = null;
        GameObject summon_godmode_obj = null;

        GameObject maxroll_import_obj = null;

        GameObject fix_lowfps_obj = null;
        GameObject teleport_to_scene_obj = null;
        GameObject skills_autocast = null;

        bool initialized = false;
        bool enable = false;
        public bool change = false;

        void Awake()
        {
            instance = this;
            Main.logger_instance?.Msg("Mods Manager : Initialize");
            Il2CppSystem.Collections.Generic.List<GameObject> Mods_Objects = new Il2CppSystem.Collections.Generic.List<GameObject>();

            //Fix
            fix_lowfps_obj = Object.Instantiate(new GameObject { name = "Mod_FixLowFPS" }, Vector3.zero, Quaternion.identity);
            fix_lowfps_obj.AddComponent<Mods.Fixs.Fix_LowFPS>();
            Mods_Objects.Add(fix_lowfps_obj);

            //UI
            cosmetics_obj = Object.Instantiate(new GameObject { name = "Mod_Cosmetics" }, Vector3.zero, Quaternion.identity);
            cosmetics_obj.AddComponent<Mods.Cosmetics.Cosmetics_Offline>();
            Mods_Objects.Add(cosmetics_obj);

            damager_meter_obj = Object.Instantiate(new GameObject { name = "Mod_Damager_Meter" }, Vector3.zero, Quaternion.identity);
            damager_meter_obj.AddComponent<Mods.UI.DamageMeter>();
            Mods_Objects.Add(damager_meter_obj);

            chat_obj = Object.Instantiate(new GameObject { name = "Mod_Remove_Chat" }, Vector3.zero, Quaternion.identity);
            chat_obj.AddComponent<Mods.Chat.Chat_Remove>();
            Mods_Objects.Add(chat_obj);

            //Teleport
            teleport_to_scene_obj = Object.Instantiate(new GameObject { name = "Mod_Teleport_To_Scene" }, Vector3.zero, Quaternion.identity);
            teleport_to_scene_obj.AddComponent<Mods.Teleport.Teleport_ToScene>();
            Mods_Objects.Add(teleport_to_scene_obj);

            //Bank Il2cpp Mods
            bank_quad_obj = Object.Instantiate(new GameObject { name = "Mod_Bank_Quad" }, Vector3.zero, Quaternion.identity);
            bank_quad_obj.AddComponent<Mods.Bank.Bank_Quad>();
            Mods_Objects.Add(bank_quad_obj);

            //Characters Il2cpp Mods
            character_autopotion_obj = Object.Instantiate(new GameObject { name = "Mod_Character_AutoPotion" }, Vector3.zero, Quaternion.identity);
            character_autopotion_obj.active = false;
            character_autopotion_obj.AddComponent<Mods.Character.Character_AutoPotions>();
            Mods_Objects.Add(character_autopotion_obj);

            character_bank_from_anywhere = Object.Instantiate(new GameObject { name = "Mod_Character_Bank_Anywhere" }, Vector3.zero, Quaternion.identity);
            character_bank_from_anywhere.AddComponent<Mods.Character.Character_Bank_Anywhere>();
            Mods_Objects.Add(character_bank_from_anywhere);

            character_blessings_obj = Object.Instantiate(new GameObject { name = "Mod_Character_Blessings" }, Vector3.zero, Quaternion.identity);
            character_blessings_obj.AddComponent<Mods.Character.Character_Blessings>();
            Mods_Objects.Add(character_blessings_obj);

            character_godmode_obj = Object.Instantiate(new GameObject { name = "Mod_Character_GodMode" }, Vector3.zero, Quaternion.identity);
            character_godmode_obj.active = false;
            character_godmode_obj.AddComponent<Mods.Character.Character_GodMode>();
            Mods_Objects.Add(character_godmode_obj);

            character_lowlife_obj = Object.Instantiate(new GameObject { name = "Mod_Character_LowLife" }, Vector3.zero, Quaternion.identity);
            character_lowlife_obj.active = false;
            character_lowlife_obj.AddComponent<Mods.Character.Character_LowLife>();
            Mods_Objects.Add(character_lowlife_obj);

            character_masteries_obj = Object.Instantiate(new GameObject { name = "Mod_Character_Masteries" }, Vector3.zero, Quaternion.identity);
            character_masteries_obj.AddComponent<Mods.Character.Character_Masteries>();
            Mods_Objects.Add(character_masteries_obj);
            
            character_permanentbuffs_obj = Object.Instantiate(new GameObject { name = "Mod_Character_PermanentBuffs" }, Vector3.zero, Quaternion.identity);
            character_permanentbuffs_obj.AddComponent<Mods.Character.Character_PermanentBuffs>();
            Mods_Objects.Add(character_permanentbuffs_obj);

            character_potionreplenishment_obj = Object.Instantiate(new GameObject { name = "Mod_Character_PotionReplenishment" }, Vector3.zero, Quaternion.identity);
            character_potionreplenishment_obj.AddComponent<Mods.Character.Character_PotionReplenishment>();
            Mods_Objects.Add(character_potionreplenishment_obj);

            character_safetp_obj = Object.Instantiate(new GameObject { name = "Mod_Character_SafeTp" }, Vector3.zero, Quaternion.identity);
            character_safetp_obj.AddComponent<Mods.Character.Character_TpSafe>();
            Mods_Objects.Add(character_safetp_obj);

            character_twohanded_with_shield_obj = Object.Instantiate(new GameObject { name = "Mod_Character_TwoHanded_with_Shield" }, Vector3.zero, Quaternion.identity);
            character_twohanded_with_shield_obj.AddComponent<Mods.Character.Character_TwoHandedShield>();
            Mods_Objects.Add(character_twohanded_with_shield_obj);

            //Craft Il2cpp Mods
            craft_max_tier_obj = Object.Instantiate(new GameObject { name = "Mod_Craft_MaxTier" }, Vector3.zero, Quaternion.identity);
            craft_max_tier_obj.AddComponent<Mods.Craft.Craft_MaxTier>();
            Mods_Objects.Add(craft_max_tier_obj);

            //Items Il2cpp Mods
            items_autopickup_obj = Object.Instantiate(new GameObject { name = "Mod_Items_AutoPickupItems" }, Vector3.zero, Quaternion.identity);
            items_autopickup_obj.AddComponent<Mods.Items.Items_AutoPickup_Items>();
            Mods_Objects.Add(items_autopickup_obj);

            items_autosell_timer_obj = Object.Instantiate(new GameObject { name = "Mod_Items_AutoStore_All10Sec" }, Vector3.zero, Quaternion.identity);
            items_autosell_timer_obj.active = false;
            items_autosell_timer_obj.AddComponent<Mods.Items.Items_AutoStore_WithTimer>();
            Mods_Objects.Add(items_autosell_timer_obj);

            items_nbsocket_obj = Object.Instantiate(new GameObject { name = "Mod_Items_NbSocket" }, Vector3.zero, Quaternion.identity);
            items_nbsocket_obj.AddComponent<Mods.Items.Items_SocketsNb>();
            Mods_Objects.Add(items_nbsocket_obj);

            //Skills
            skills_autocast = Object.Instantiate(new GameObject { name = "Mod_Skills_AutoCast" }, Vector3.zero, Quaternion.identity);
            skills_autocast.AddComponent<Mods.Skills.Skills_AutoCast>();
            Mods_Objects.Add(skills_autocast);

            //Minimap Il2cpp Mods
            minimap_icons_obj = Object.Instantiate(new GameObject { name = "Mod_Minimap_Icons" }, Vector3.zero, Quaternion.identity);
            minimap_icons_obj.AddComponent<Mods.Minimap.Minimap_Icons>();
            Mods_Objects.Add(minimap_icons_obj);

            //Monolith Il2cpp Mods
            monoliths_complete_objectives_obj = Object.Instantiate(new GameObject { name = "Mod_Monoliths_Complete_Objectives" }, Vector3.zero, Quaternion.identity);
            monoliths_complete_objectives_obj.AddComponent<Mods.Monoliths.Monoliths_CompleteObjective>();
            Mods_Objects.Add(monoliths_complete_objectives_obj);

            //NewItems Il2cpp Mods
            newitems_essentiasanguis_obj = Object.Instantiate(new GameObject { name = "Mod_NewItems_EssentiaSanguis" }, Vector3.zero, Quaternion.identity);
            newitems_essentiasanguis_obj.AddComponent<Mods.NewItems.Items_EssentiaSanguis>();
            Mods_Objects.Add(newitems_essentiasanguis_obj);

            newitems_headhunter_obj = Object.Instantiate(new GameObject { name = "Mod_NewItems_Headhunter" }, Vector3.zero, Quaternion.identity);
            newitems_headhunter_obj.AddComponent<Mods.NewItems.Items_HeadHunter>();
            Mods_Objects.Add(newitems_headhunter_obj);

            newitems_heralds_obj = Object.Instantiate(new GameObject { name = "Mod_NewItems_Heralds" }, Vector3.zero, Quaternion.identity);
            newitems_heralds_obj.AddComponent<Mods.NewItems.Items_Heralds>();
            Mods_Objects.Add(newitems_heralds_obj);

            newitems_mjolner_obj = Object.Instantiate(new GameObject { name = "Mod_NewItems_Mjolner" }, Vector3.zero, Quaternion.identity);
            newitems_mjolner_obj.AddComponent<Mods.NewItems.Items_Mjolner>();
            Mods_Objects.Add(newitems_mjolner_obj);

            newitems_sandsofsilk_obj = Object.Instantiate(new GameObject { name = "Mod_NewItems_SandsOfSilk" }, Vector3.zero, Quaternion.identity);
            newitems_sandsofsilk_obj.AddComponent<Mods.NewItems.Items_SandsOfSilk>();
            Mods_Objects.Add(newitems_sandsofsilk_obj);

            newitems_arakaali_obj = Object.Instantiate(new GameObject { name = "Mod_NewItems_ArakaalisFang" }, Vector3.zero, Quaternion.identity);
            newitems_arakaali_obj.AddComponent<Mods.NewItems.Items_ArakaalisFang>();
            Mods_Objects.Add(newitems_arakaali_obj);

            //Spawn Il2cpp Mods
            spawn_timebeast_obj = Object.Instantiate(new GameObject { name = "Mod_Spawn_TimeBeast" }, Vector3.zero, Quaternion.identity);
            spawn_timebeast_obj.AddComponent<Mods.Spawn.TimeBeast>();
            Mods_Objects.Add(spawn_timebeast_obj);

            //Summon Il2cpp Mods
            summon_collider_obj = Object.Instantiate(new GameObject { name = "Mod_Summon_Collider" }, Vector3.zero, Quaternion.identity);
            summon_collider_obj.AddComponent<Mods.Summon.Summon_Collider>();
            Mods_Objects.Add(summon_collider_obj);

            summon_forever_obj = Object.Instantiate(new GameObject { name = "Mod_Summon_Forever" }, Vector3.zero, Quaternion.identity);
            summon_forever_obj.AddComponent<Mods.Summon.Summon_Forever>();
            Mods_Objects.Add(summon_forever_obj);

            summon_godmode_obj = Object.Instantiate(new GameObject { name = "Mod_Summon_GodMode" }, Vector3.zero, Quaternion.identity);
            summon_godmode_obj.AddComponent<Mods.Summon.Summon_GodMode>();
            Mods_Objects.Add(summon_godmode_obj);

            maxroll_import_obj = Object.Instantiate(new GameObject { name = "Mod_Maxroll_Import" }, Vector3.zero, Quaternion.identity);
            maxroll_import_obj.AddComponent<Mods.Maxroll.Maxroll_import>();
            Mods_Objects.Add(maxroll_import_obj);

            foreach (GameObject mod in Mods_Objects) { Object.DontDestroyOnLoad(mod); }
            Mods_Objects.Clear();

            initialized = true;
            Main.logger_instance?.Msg("Mods Manager : Mods initialized");
        }
        void Update() //This function will be removed soon (when i have time to do it ^^)
        {
            if (!Save_Manager.instance.IsNullOrDestroyed())
            {
                if (Save_Manager.instance.initialized)
                {
                    if ((!enable) || (change))
                    {
                        change = false;
                        Enable();
                    }
                }
            }
        }
        void Enable() //This function will be removed soon (when i have time to do it ^^)
        {
            if (initialized)
            {
                enable = true;
                character_godmode_obj.SetActive(Save_Manager.instance.data.Character.Cheats.Enable_GodMode);
                character_lowlife_obj.SetActive(Save_Manager.instance.data.Character.Cheats.Enable_LowLife);
                character_autopotion_obj.SetActive(Save_Manager.instance.data.Character.Cheats.Enable_AutoPot);
                items_autosell_timer_obj.SetActive(Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_Timer);
                Mods.Items.Items_Update.Reqs(); //Used to update item req
            }
        }
    }
}
