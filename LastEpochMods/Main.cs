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
                //Wait UnityEngine Initialized before
                if (Input.GetKeyDown(KeyCode.F7)) //Unlock Drop for all Uniques
                {
                    Mods.ItemsDrop.UnlockForAllUniques(this);
                    if (!Scenes.MenuNames.Contains(Scenes.CurrentName))
                    { LoggerInstance.Msg("Go Back to Menu for actualize items Mods"); }
                }
                if (Input.GetKeyDown(KeyCode.F8)) //Unlock Drop for all Basic
                {
                    Mods.ItemsDrop.UnlockForAllBasic(this);
                    if (!Scenes.MenuNames.Contains(Scenes.CurrentName))
                    { LoggerInstance.Msg("Go Back to Menu for actualize items Mods"); }
                }
                else if (Input.GetKeyDown(KeyCode.F9)) //Remove Level and Class Req
                {
                    Mods.ItemsReq.Remove(this);
                    if (!Scenes.MenuNames.Contains(Scenes.CurrentName))
                    { LoggerInstance.Msg("Go Back to Menu for actualize items Mods"); }
                }
                else if (Input.GetKeyDown(KeyCode.F10)) //Edit Unique Mods
                {
                    Mods.UniqueMods.EditUniqueMods(111, Mods.UniqueMods.CustomMods_0(), this); //Wover Flesh : UniqueId = 111
                    //Mods.UniqueMods.EditUniqueMods(UniqueId, CustomMod, this);
                    //Mods.UniqueMods.EditUniqueMods(UniqueId, CustomMod, this);
                    //Mods.UniqueMods.EditUniqueMods(UniqueId, CustomMod, this);
                    if (!Scenes.MenuNames.Contains(Scenes.CurrentName))
                    { LoggerInstance.Msg("Go Back to Menu for actualize items Mods"); }
                }
                else if (Input.GetKeyDown(KeyCode.F11))
                {
                    if (!Scenes.MenuNames.Contains(Scenes.CurrentName)) { Mods.LastEpochSaveEditorDatabase.GenerateDatabase(this); }
                    else { LoggerInstance.Msg("Lauch a character before doing this"); }
                }
            }
        }
    }
}
