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
            LoggerInstance.Msg("Scene : " + sceneName);
        }
        public override void OnLateUpdate()
        {
            if (Scenes.CurrentName != "") //Wait UnityEngine Initialized before
            {
                if (Input.GetKeyDown(KeyCode.F9)) //Launch after Scene Change
                {
                    if (!Scenes.MenuNames.Contains(Scenes.CurrentName))
                    {
                        Mods.Spawner.Density = 999; //Untested
                        //Mods.Spawner.IncreaseDensity(); //Comment to Disable
                        Mods.ItemsDrop.goldMultiplier = 9999;
                        Mods.ItemsDrop.ItemMultiplier = 5;
                        Mods.ItemsDrop.Experience = 99999999; //Untested //should be xp on death
                        Mods.ItemsDrop.EditMonstersDeathDrop(); //Comment to Disable
                        Mods.Monsters.MagicMobExpMultiplier = 99999999; //Untested
                        Mods.Monsters.MagicMobExpMultiplierPerLevel = 999; //Untested
                        Mods.Monsters.RareMobExpMultiplier = 99999999; //Untested
                        Mods.Monsters.RareMobExpMultiplierPerLevel = 999; //Untested
                        Mods.Monsters.ExperienceMultiplier(); //Comment to Disable
                    }
                    else { LoggerInstance.Msg("Lauch a character before doing this"); }
                }
                else if (Input.GetKeyDown(KeyCode.F10)) //Items Mods (Launch at menu)
                {
                    //Level and Class Req
                    Mods.ItemsReq.Remove(this); //Comment to Disable
                    //Basic Drop
                    Mods.ItemsDrop.OnlyUndropablesBasic = true; //False = unlock all / True = Only Undropable
                    Mods.ItemsDrop.UnlockForAllBasic(this); //Comment to Disable
                    //Unique Drop
                    Mods.ItemsDrop.OnlyUndropablesUnique = true; //False = unlock all / True = Only Undropable
                    Mods.ItemsDrop.UnlockForAllUniques(this); //Comment to Disable                    
                    //Drop Level minimum for Legenday Potencial
                    //Mods.ItemsDrop.SetLevelReqForLegendaryPotencial(0); //Untested //Comment to Disable
                    //Unique Mods
                    Mods.UniqueMods.Edit(111, Mods.UniqueMods.CustomMods_0(), this); //Wover Flesh : UniqueId = 111
                    //Mods.UniqueMods.Edit(UniqueId, CustomMod, this);
                    //Mods.UniqueMods.Edit(UniqueId, CustomMod, this);
                    //Mods.UniqueMods.Edit(UniqueId, CustomMod, this);
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
