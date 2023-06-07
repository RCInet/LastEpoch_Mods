using MelonLoader;
using System.Linq;
using UnityEngine;

namespace LastEpochMods
{
    public class Main : MelonMod
    {
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Scenes.CurrentName = sceneName;
            //LoggerInstance.Msg("Scene : " + sceneName);
        }
        public override void OnLateUpdate()
        {
            if (Scenes.CurrentName != "") //Wait UnityEngine Initialized before
            {
                if (Input.GetKeyDown(KeyCode.F9)) //Launch after Scene Change
                {
                    if (!Scenes.MenuNames.Contains(Scenes.CurrentName))
                    {
                        //ItemDrop
                        Mods.Scene_Mods.Enable_DeathItemDrop_goldMultiplier = true;
                        Mods.Scene_Mods.DeathItemDrop_goldMultiplier = 99;
                        Mods.Scene_Mods.Enable_DeathItemDrop_ItemMultiplier = true;
                        Mods.Scene_Mods.DeathItemDrop_ItemMultiplier = 2;
                        Mods.Scene_Mods.Enable_DeathItemDrop_Experience = true;
                        Mods.Scene_Mods.DeathItemDrop_Experience = 99999;
                        //MonsterRarityMagic
                        Mods.Scene_Mods.Enable_MonsterRarityMagic_BaseExpMultiplier = true;
                        Mods.Scene_Mods.MonsterRarityMagic_BaseExpMultiplier = 99999;
                        Mods.Scene_Mods.Enable_MonsterRarityMagic_ExpMultiplierPerLevel = true;
                        Mods.Scene_Mods.MonsterRarityMagic_ExpMultiplierPerLevel = 99999;
                        //MonsterRarityRare
                        Mods.Scene_Mods.Enable_MonsterRarityRare_BaseExpMultiplier = true;
                        Mods.Scene_Mods.MonsterRarityRare_BaseExpMultiplier = 99999;
                        Mods.Scene_Mods.Enable_MonsterRarityRare_xpMultiplierPerLevel = true;
                        Mods.Scene_Mods.MonsterRarityRare_xpMultiplierPerLevel = 99999;
                        //SpawnerPlacementRoom
                        Mods.Scene_Mods.Enable_SpawnerPlacementRoom_intendedSpawnerDensity = true;
                        Mods.Scene_Mods.SpawnerPlacementRoom_intendedSpawnerDensity = 10;
                        //SpawnerPlacementManager
                        Mods.Scene_Mods.Enable_SpawnerPlacementManager_defaultSpawnerDensity = false;
                        Mods.Scene_Mods.SpawnerPlacementManager_defaultSpawnerDensity = 10;
                        Mods.Scene_Mods.Enable_SpawnerPlacementManager_alwaysRollSpawnerDensity = false;
                        Mods.Scene_Mods.SpawnerPlacementManager_alwaysRollSpawnerDensity = false;
                        Mods.Scene_Mods.Launch();
                        LoggerInstance.Msg("Scene Mods Launch");
                    }
                    else { LoggerInstance.Msg("Lauch a character before doing this"); }
                }
                else if (Input.GetKeyDown(KeyCode.F10)) //Items_Mods (Launch at menu)
                {
                    //Basic
                    Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForAll = false;
                    Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForUndropableOnly = true; //Lock Dropable Item if UnlockDropForAll is set to false
                    Mods.Items_Mods.Basic.EquipmentItem_RemoveClassReq = true;
                    Mods.Items_Mods.Basic.EquipmentItem_EditLevelReq = true;
                    Mods.Items_Mods.Basic.EquipmentItem_SetLevelReq = 0;
                    Mods.Items_Mods.Basic.Launch();
                    //Unique Mods
                    Mods.Items_Mods.Unique.Uniques_Mods = new System.Collections.Generic.List<Mods.Items_Mods.Unique.unique_mod>
                    {
                        new Mods.Items_Mods.Unique.unique_mod { id = 111, mods = Mods.UniqueMods.CustomMods_0() }
                        //Add more unique here
                        //new Mods.Items_Mods.Unique.unique_mod { id = UniqueId, mods = CustomMod }
                    };                    
                    Mods.Items_Mods.Unique.UniqueList_Entry_Enable_UniqueMods = true;
                    //Unique
                    Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForAll = false;
                    Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForUndropableOnly = true; //Lock Dropable Item if UnlockDropForAll is set to false
                    Mods.Items_Mods.Unique.Enable_LegendaryPotentialLevelMod = true;
                    Mods.Items_Mods.Unique.UniqueList_Entry_LegendaryPotentialLevel = 0;
                    Mods.Items_Mods.Unique.Launch();
                    LoggerInstance.Msg("Items Mods Launch");

                    if (!Scenes.MenuNames.Contains(Scenes.CurrentName))
                    { LoggerInstance.Msg("Go Back to Menu for actualize items Mods"); }
                }
                else if (Input.GetKeyDown(KeyCode.F11)) //Generate Database for Save Editor //Launch in game
                {
                    if (!Scenes.MenuNames.Contains(Scenes.CurrentName)) { Mods.LastEpochSaveEditor.GenerateDatabase(this); }
                    else { LoggerInstance.Msg("Lauch a character before doing this"); }
                }
            }
        }
    }
}
