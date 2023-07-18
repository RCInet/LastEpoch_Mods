using MelonLoader;
using System.Linq;

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
        public void UniverseLib_LogHandler(string message, UnityEngine.LogType type)
        {
            LoggerInstance.Msg("UniverseLib : " + message);
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Scenes.CurrentName = sceneName;
            if (UniverseLibLoaded)
            {
                if (Scenes.GameScene())
                {                    
                    OnSceneChanged.Spawner_Placement_Manager.Init();
                    OnSceneChanged.Local_Tree_Data.Init();
                    Ui.GenerateItem.Init();
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
                    UniverseLib.Config.UniverseLibConfig config = new UniverseLib.Config.UniverseLibConfig()
                    {
                        Disable_EventSystem_Override = false,
                        Force_Unlock_Mouse = true,
                        Unhollowed_Modules_Folder = System.IO.Directory.GetCurrentDirectory() + @"\MelonLoader\"
                    };
                    UniverseLib.Universe.Init(1f, UniverseLib_OnInitialized, UniverseLib_LogHandler, config);
                }
            }
            else
            {
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F1)) { Ui.Menu.isMenuOpen = !Ui.Menu.isMenuOpen; }
                if (Scenes.GameScene()) //In game
                //if (!Scenes.MenuNames.Contains(Scenes.CurrentName)) 
                {
                    if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F10)) { Mods.Character.Launch_LevelUp(); }
                }
            }
        }
        public override void OnGUI()
        {
            Ui.Menu.Update();           
        }        
    }
}
