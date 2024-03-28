using LastEpochMods.Managers;

namespace LastEpochMods
{
    public class Main : MelonLoader.MelonMod
    {
        public static MelonLoader.MelonLogger.Instance logger_instance = null;
        public override void OnInitializeMelon()
        {
            logger_instance = LoggerInstance;
            Assets_Manager.OnInitializeMelon();
            Save_Manager.OnInitializeMelon();            
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Scenes_Manager.CurrentName = sceneName;
            Mods_Managers.OnSceneWasLoaded(sceneName);
        }
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            GUI_Manager.OnSceneWasInitialized(sceneName);
            Mods_Managers.OnSceneWasInitialized();
        }
        public override void OnLateUpdate()
        {
            QuickUpdate();            
            if (!Running) { SlowUpdate(); }
            else
            {
                System.TimeSpan elaspedTime = System.DateTime.Now - StartTime;
                System.Double seconds = elaspedTime.TotalSeconds;
                if (seconds > (Duration)) { Running = false; }
            }
        }
        public override void OnGUI()
        {
            GUI_Manager.UpdateGUI();
            Memory_Manager.OnGUI();
        }
        
        private static bool Running = false;
        private static System.DateTime StartTime;
        private static readonly float Duration = 1f;
        private static void QuickUpdate()
        {
            Mods_Managers.QuickUpdate();
            //Memory_Manager.QuickUpdate(); //F5 to see memory
        }
        private static void SlowUpdate()
        {
            StartTime = System.DateTime.Now;
            Running = true;

            GUI_Manager.SlowUpdate();
            Mods_Managers.SlowUpdate();
        }
    }
}
