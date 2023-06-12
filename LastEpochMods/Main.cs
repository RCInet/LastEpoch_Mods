using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;

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
            //Affixs
            //Mods.Affixs_Mods.MultiplyAffixsRolls(100);
            //Mods.Affixs_Mods.EditAffixRollsByTier(100, 7, 100, 999);
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
                Mods.Scene_Mods.Enable_DeathItemDrop_AdditionalRare = true;
                Mods.Scene_Mods.DeathItemDrop_AdditionalRare = true;
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
                Mods.Scene_Mods.SpawnerPlacementRoom_intendedSpawnerDensity = 9999;
                //SpawnerPlacementManager
                Mods.Scene_Mods.Enable_SpawnerPlacementManager_defaultSpawnerDensity = true;
                Mods.Scene_Mods.SpawnerPlacementManager_defaultSpawnerDensity = 9999;
                Mods.Scene_Mods.Enable_SpawnerPlacementManager_alwaysRollSpawnerDensity = true;
                Mods.Scene_Mods.SpawnerPlacementManager_alwaysRollSpawnerDensity = false;
                Mods.Scene_Mods.Enable_SpawnerPlacementManager_IncreaseExperience = true;
                Mods.Scene_Mods.SpawnerPlacementManager_IncreaseExperience = 99999;
                //SkillsTreeNode
                //Mods.Scene_Mods.Enable_SkillTreeNode_RemoveNodeRequirements = true;

                Mods.Scene_Mods.Launch();
                LoggerInstance.Msg("Scene Mods Loaded");
            }
        }        
        private void LoadCharacter_Mods() //Character_Mods (AutoLoad on Scene Change)
        {
            //ItemDrop
            Mods.Character_Mods.Enable_increase_equipment_droprate = true;
            Mods.Character_Mods.increase_equipment_droprate = 1;
            Mods.Character_Mods.Enable_increase_equipmentshards_droprate = true;
            Mods.Character_Mods.increase_equipmentshards_droprate = 1;
            Mods.Character_Mods.Enable_increase_uniques_droprate = true;
            Mods.Character_Mods.increase_uniques_droprate = 1;
            //Speed Manager
            Mods.Character_Mods.Enable_base_movement_speed = true;
            Mods.Character_Mods.base_movement_speed = 10; //default 4,8
            //Ability List
            Mods.Character_Mods.Enable_channel_cost = true;
            Mods.Character_Mods.channel_cost = 0;
            Mods.Character_Mods.Enable_manaCost = true;
            Mods.Character_Mods.manaCost = 0;
            Mods.Character_Mods.Enable_manaCostPerDistance = true;
            Mods.Character_Mods.manaCostPerDistance = 0;
            Mods.Character_Mods.Enable_minimumManaCost = true;
            Mods.Character_Mods.minimumManaCost = 0;
            Mods.Character_Mods.Enable_noManaRegenWhileChanneling = true;
            Mods.Character_Mods.noManaRegenWhileChanneling = false;
            Mods.Character_Mods.Enable_stopWhenOutOfMana = true;
            Mods.Character_Mods.stopWhenOutOfMana = false;
            //Tree Data
            Mods.Character_Mods.Enable_character_class = true;
            Mods.Character_Mods.character_class = CharacterClassID.Sentinel;
            Mods.Character_Mods.Enable_character_level = true;
            Mods.Character_Mods.character_level = 100;
            Mods.Character_Mods.Enable_choosen_mastery = true;
            Mods.Character_Mods.chosen_mastery = 0;
            Mods.Character_Mods.Enable_number_of_unlocked_slots = true;
            Mods.Character_Mods.number_of_unlocked_slots = 5;
            Mods.Character_Mods.Enable_passiveTree_pointsEarnt = true;
            Mods.Character_Mods.passiveTree_pointsEarnt = 65535;
            Mods.Character_Mods.Enable_skilltree_level = true;
            Mods.Character_Mods.skilltree_level = 255;
            //GoldTracker
            Mods.Character_Mods.Enable_gold_value = true;
            Mods.Character_Mods.gold_value = 99999999;
            //CharacterStats
            Mods.Character_Mods.Enable_attack_rate = true;
            Mods.Character_Mods.attack_rate = 255;
            Mods.Character_Mods.Enable_attributes = true;
            Mods.Character_Mods.attributes_str = 99999999;
            Mods.Character_Mods.attributes_int = 99999999;
            Mods.Character_Mods.attributes_vita = 99999999;
            Mods.Character_Mods.attributes_dext = 99999999;
            Mods.Character_Mods.attributes_atte = 99999999;
            //ExperienceTracker
            Mods.Character_Mods.Enable_NextLevelExperience = true;
            Mods.Character_Mods.NextLevelExperience = 0;
            Mods.Character_Mods.Launch();
        }

        private void LoadSkillsHelper()
        {
            if ((Scenes.CurrentName != "") && (!Scenes.MenuNames.Contains(Scenes.CurrentName)))
            {
                Mods.Skills_Mods.Helper_Skills_Nodes(this);                
            }
            else { LoggerInstance.Msg("Go in game for launching Skills Helper"); }
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
                if (!ItemsModsLoaded) { ItemsModsLoaded = true; LoadItemsMods(); }                
                LoadSceneMods();
                LoadCharacter_Mods();
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
                if (Input.GetKeyDown(KeyCode.F10)) //Launch LevelUp
                {
                    foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
                    {
                        if ((obj.name == "MainPlayer(Clone)") && (obj.GetActualType() == typeof(ExperienceTracker)))
                        {
                            obj.TryCast<ExperienceTracker>().LevelUp(true);
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.F11)) { LoadSkillsHelper(); }
            }
        }
    }
}
