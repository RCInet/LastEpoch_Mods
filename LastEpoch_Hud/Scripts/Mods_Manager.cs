using MelonLoader;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace LastEpoch_Hud.Scripts
{
    [RegisterTypeInIl2Cpp]
    public class Mods_Manager : MonoBehaviour
    {
        public Mods_Manager(System.IntPtr ptr) : base(ptr) { }
        public static Mods_Manager instance { get; private set; }        
        GameObject character_autopotion_obj = null;
        GameObject character_blessings_obj = null;
        GameObject character_godmode_obj = null;
        GameObject character_lowlife_obj = null;
        GameObject character_masteries_obj = null;
        GameObject character_permanentbuffs_obj = null;
        GameObject items_autosell_timer_obj = null;
        GameObject items_headhunter_obj = null;
        bool initialized = false;

        void Awake()
        {
            instance = this;
            Main.logger_instance.Msg("Mods Manager : Initialize");
            Il2CppSystem.Collections.Generic.List<GameObject> Mods_Objects = new Il2CppSystem.Collections.Generic.List<GameObject>();

            character_autopotion_obj = Object.Instantiate(new GameObject { name = "Mod_Character_AutoPotion" }, Vector3.zero, Quaternion.identity);
            character_autopotion_obj.active = false;
            character_autopotion_obj.AddComponent<Mods.Character.Character_AutoPotions>();
            Mods_Objects.Add(character_autopotion_obj);

            character_blessings_obj = Object.Instantiate(new GameObject { name = "Mod_Character_Blessings" }, Vector3.zero, Quaternion.identity);
            character_blessings_obj.active = false;
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
            character_masteries_obj.active = false;
            character_masteries_obj.AddComponent<Mods.Character.Character_Masteries>();
            Mods_Objects.Add(character_masteries_obj);

            character_permanentbuffs_obj = Object.Instantiate(new GameObject { name = "Mod_Character_Buffs" }, Vector3.zero, Quaternion.identity);
            character_permanentbuffs_obj.active = false;
            character_permanentbuffs_obj.AddComponent<Mods.Character.Character_PermanentBuffs>();
            Mods_Objects.Add(character_permanentbuffs_obj);

            items_autosell_timer_obj = Object.Instantiate(new GameObject { name = "Mod_Items_AutoStore_All10Sec" }, Vector3.zero, Quaternion.identity);
            items_autosell_timer_obj.active = false;
            items_autosell_timer_obj.AddComponent<Mods.Items.Items_AutoStore_WithTimer>();
            Mods_Objects.Add(items_autosell_timer_obj);

            items_headhunter_obj = Object.Instantiate(new GameObject { name = "Mod_Items_Headhunter" }, Vector3.zero, Quaternion.identity);
            items_headhunter_obj.active = false;
            items_headhunter_obj.AddComponent<Mods.Items.Items_HeadHunter>();
            Mods_Objects.Add(items_headhunter_obj);

            foreach (GameObject mod in Mods_Objects) { Object.DontDestroyOnLoad(mod); }
            Mods_Objects.Clear();

            initialized = true;
            Main.logger_instance.Msg("Mods Manager : Mods initialized");
        }
        void Enable()
        {
            if (initialized)
            {
                character_godmode_obj.active = Save_Manager.instance.data.Character.Cheats.Enable_GodMode;
                character_lowlife_obj.active = Save_Manager.instance.data.Character.Cheats.Enable_LowLife;
                //character_blessings_obj.active = Save_Manager.instance.data.Character.Cheats.Enable_CanChooseBlessing;
                character_blessings_obj.active = true;
                //character_idols_obj.GetComponent<Mods.Character.Character_UnlockAllIdols>().SetActive();
                character_autopotion_obj.active = Save_Manager.instance.data.Character.Cheats.Enable_AutoPot;
                items_autosell_timer_obj.active = Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_All10Sec;
                character_masteries_obj.active = true;
                character_permanentbuffs_obj.GetComponent<Mods.Character.Character_PermanentBuffs>().Enable();

                Mods.Items.Items_Update.Reqs();

                items_headhunter_obj.active = true;
            }
        }
        void Disable()
        {
            if (initialized)
            {
                if (Main.debug) { Main.logger_instance.Msg("Mods Manager : Disable all mods"); }
                character_godmode_obj.active = false;
                character_lowlife_obj.active = false;
                character_blessings_obj.active = false;
                character_autopotion_obj.active = false;
                items_autosell_timer_obj.active = false;
                character_masteries_obj.active = false;
                character_permanentbuffs_obj.GetComponent<Mods.Character.Character_PermanentBuffs>().Disable();
                items_headhunter_obj.active = false;
            }
        }

        public void SetActive(bool online)
        {
            if (!online) { Enable(); }
            else { Disable(); }
        }
        public void ForceUpdate() //Cast by SaveManager when data changed
        {
            SetActive(Refs_Manager.online);
        }
    }
}
