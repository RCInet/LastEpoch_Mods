using LastEpochMods.Mods;
using MelonLoader;
using System.Linq;
using UnityEngine;

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
                    Mods.Affixs_Mods.Launch();
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
                    if (Input.GetKeyDown(KeyCode.F9))
                    {
                        InventoryPanelUI.instance.cosmeticPanel.active = true;
                        //Character_Mods.Launch_ExempleBuffCharacter();
                    }
                    else if (Input.GetKeyDown(KeyCode.F10)) { Character_Mods.Launch_LevelUp(); }                    
                    else if (Input.GetKeyDown(KeyCode.F11))
                    {
                        //Mods.Items_Mods.Rolls.Enable_Items_Rolls = !Mods.Items_Mods.Rolls.Enable_Items_Rolls;
                        //LoggerInstance.Msg(Mods.Items_Mods.Rolls.Enable_Items_Rolls + " Items Roll");
                    }
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
