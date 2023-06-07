using MelonLoader;
using System.Linq;
using UnityEngine;

namespace LastEpochMods
{
    public class Main : MelonMod
    {
        private void LoadItemsMods() //Items_Mods (AutoLoad after Unity Explorer Init and Scene Change)
        {
            //Basic
            Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForAll = true;
            Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForUndropableOnly = true; //Lock Dropable Item if UnlockDropForAll is set to false
            Mods.Items_Mods.Basic.EquipmentItem_RemoveClassReq = true;
            Mods.Items_Mods.Basic.EquipmentItem_EditLevelReq = true;
            Mods.Items_Mods.Basic.EquipmentItem_SetLevelReq = 0;
            Mods.Items_Mods.Basic.Launch();
            //Unique Mods
            Mods.UniqueMods.Uniques_Mods = new System.Collections.Generic.List<Mods.UniqueMods.unique_mod>
            {
                new Mods.UniqueMods.unique_mod { id = 111, mods = Mods.UniqueMods.CustomMods_0() }
                //Add more unique here
                //new Mods.Items_Mods.Unique.unique_mod { id = UniqueId, mods = CustomMod }
            };
            Mods.UniqueMods.Enable_UniqueMods = true;
            //Unique
            Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForAll = true;
            Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForUndropableOnly = true; //Lock Dropable Item if UnlockDropForAll is set to false
            Mods.Items_Mods.Unique.Enable_LegendaryPotentialLevelMod = true;
            Mods.Items_Mods.Unique.UniqueList_Entry_LegendaryPotentialLevel = 0;
            Mods.Items_Mods.Unique.Launch();
            LoggerInstance.Msg("Items Mods Loaded");
        }
        private void LoadSceneMods() //Scene_Mods (AutoLoad on Scene Change)
        {            
            if ((Scenes.CurrentName != "") && (!Scenes.MenuNames.Contains(Scenes.CurrentName)))
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
                LoggerInstance.Msg("Scene Mods Loaded");
            }
        }        

        private bool UnityExplorerLoaded = false;
        private bool ItemsModsLoaded = false;
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Scenes.CurrentName = sceneName;
            if (UnityExplorerLoaded)
            {
                if ((Scenes.CurrentName != "") && (!Scenes.MenuNames.Contains(Scenes.CurrentName)))
                { LoggerInstance.Msg("New Scene : " + sceneName); }
                LoadItemsMods();
                LoadSceneMods();                
            }
        }
        public override void OnLateUpdate()
        {
            if (!UnityExplorerLoaded) 
            {
                if (UnityExplorer.ObjectExplorer.SceneHandler.SelectedScene != null) { UnityExplorerLoaded = true; }                
            }
            else
            {
                if (!ItemsModsLoaded) { ItemsModsLoaded = true; LoadItemsMods(); }
                if (Input.GetKeyDown(KeyCode.F11)) { Mods.LastEpochSaveEditor.GenerateDatabase(this); }                
            }
        }
    }
}
