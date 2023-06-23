using ItemFiltering;
using LastEpochMods.Mods;
using MelonLoader;
using System.Linq;
using UMA.AssetBundles;
using UnityEngine;
using UnityExplorer.UI;
using UniverseLib;

namespace LastEpochMods
{
    public class Main : MelonMod
    {
        public static MelonLoader.MelonLogger.Instance logger_instance = null;
        public static bool UniverseLibLoaded = false;
        public override void OnInitializeMelon()
        {
            logger_instance = LoggerInstance;
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Scenes.CurrentName = sceneName;
            if (UniverseLibLoaded)
            {
                if ((Scenes.CurrentName != "") && (!Scenes.MenuNames.Contains(Scenes.CurrentName)))
                {
                    Mods.Affixs_Mods.Launch(); //Affixs Mods
                    Mods.Items_Mods.Basic.Launch(); //Basic Items Mods                    
                    Mods.UniqueMods.Enable_UniqueMods = true; //Edit Unique Mods
                    Mods.UniqueMods.Uniques_Mods = new System.Collections.Generic.List<Mods.UniqueMods.unique_mod>
                    {
                        new Mods.UniqueMods.unique_mod { id = 111, mods = Mods.UniqueMods.CustomMods_0() }
                        //Add more unique here
                        //new Mods.Items_Mods.Unique.unique_mod { id = UniqueId, mods = CustomMod }
                    };
                    Mods.Items_Mods.Unique.Launch(); //unique Items Mods                    
                    Mods.Scene_Mods.Launch();
                    Mods.Character_Mods.Launch();
                }
            }
        }
        public override void OnLateUpdate()
        {
            if ((!UniverseLibLoaded) && (UniverseLib.Universe.CurrentGlobalState == UniverseLib.Universe.GlobalState.SetupCompleted))
            {
                UniverseLibLoaded = true;
                LoggerInstance.Msg("Game Loaded");
            }
            if (UniverseLibLoaded)
            {
                if (Input.GetKeyDown(KeyCode.F1)) { Ui.Menu.isMenuOpen = !Ui.Menu.isMenuOpen; }
                if (!Scenes.MenuNames.Contains(Scenes.CurrentName)) //In game
                {
                    if (Input.GetKeyDown(KeyCode.F9)) { Character_Mods.Launch_ExempleBuffCharacter(); }
                    else if (Input.GetKeyDown(KeyCode.F10)) { Character_Mods.Launch_LevelUp(); }
                    //else if (Input.GetKeyDown(KeyCode.F12)) { Ability_mods.LoadSkillsHelper(); }
                }
            }
        }
        public override void OnGUI()
        {
            if (Ui.Menu.isMenuOpen) { Ui.Menu.Update(); }            
        }        
    }
}
