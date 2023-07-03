using LastEpochMods.Mods;
using MelonLoader;
using System.Linq;
using UMA.CharacterSystem.Examples;
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
        public void UniverseLib_OnInitialized()
        {
            UniverseLibLoaded = true;
            LoggerInstance.Msg("Mods init completed");
        }
        public void UniverseLib_LogHandler(string message, LogType type)
        {
            LoggerInstance.Msg("UniverseLib : " + message);
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
            if (!Config.Data.mods_config_loaded) { Config.Data.mods_config_loaded = true; Config.Load.Mods(); }
            if (!UniverseLibLoaded)
            {                
                if (UniverseLib.Universe.CurrentGlobalState == UniverseLib.Universe.GlobalState.SetupCompleted)
                {
                    UniverseLibLoaded = true;
                    LoggerInstance.Msg("UnityExplorer found");
                    LoggerInstance.Msg("Mods init completed");
                }
                else if (UniverseLib.Universe.CurrentGlobalState == UniverseLib.Universe.GlobalState.WaitingToSetup)
                {
                    float startupDelay = 1f;
                    UniverseLib.Config.UniverseLibConfig config = new UniverseLib.Config.UniverseLibConfig()
                    {
                        Disable_EventSystem_Override = false,
                        Force_Unlock_Mouse = true,
                        Unhollowed_Modules_Folder = System.IO.Directory.GetCurrentDirectory() + @"\MelonLoader\"
                    };
                    UniverseLib.Universe.Init(startupDelay, UniverseLib_OnInitialized, UniverseLib_LogHandler, config);
                }
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
            Ui.Menu.Update();
            //if (Ui.Menu.isMenuOpen) { Ui.Menu.Update(); }            
        }        
    }
}
