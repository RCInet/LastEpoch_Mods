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
                    Mods.ItemsReq.Remove(this); //Remove Items Level and Class Req

                    Mods.ItemsDrop.OnlyUndropablesBasic = true; //False = unlock all / True = Only Undropable
                    Mods.ItemsDrop.UnlockForAllBasic(this); //Basic Edit Drop

                    Mods.ItemsDrop.OnlyUndropablesUnique = true; //False = unlock all / True = Only Undropable
                    Mods.ItemsDrop.UnlockForAllUniques(this); //Uniques Edit Drop                    
                    
                    /*Edit Unique Mods Here*/
                    Mods.UniqueMods.EditUniqueMods(111, Mods.UniqueMods.CustomMods_0(), this); //Wover Flesh : UniqueId = 111
                    //Mods.UniqueMods.EditUniqueMods(UniqueId, CustomMod, this);
                    //Mods.UniqueMods.EditUniqueMods(UniqueId, CustomMod, this);
                    //Mods.UniqueMods.EditUniqueMods(UniqueId, CustomMod, this);
                    
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
