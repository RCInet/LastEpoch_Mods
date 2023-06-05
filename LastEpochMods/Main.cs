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
        }
        public override void OnLateUpdate()
        {
            if (Scenes.CurrentName != "")
            {
                if (Input.GetKeyDown(KeyCode.F10)) //Wait UnityEngine Initialized before
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
                else if (Input.GetKeyDown(KeyCode.F11))
                {
                    if (!Scenes.MenuNames.Contains(Scenes.CurrentName)) { Mods.LastEpochSaveEditor.GenerateDatabase(this); }
                    else { LoggerInstance.Msg("Lauch a character before doing this"); }
                }
            }
        }
    }
}
