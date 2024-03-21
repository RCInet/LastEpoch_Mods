using LastEpochMods.Managers;

namespace LastEpochMods
{
    public class Main : MelonLoader.MelonMod
    {
        public static MelonLoader.MelonLogger.Instance logger_instance = null;

        public override void OnInitializeMelon()
        {
            logger_instance = LoggerInstance;
            Save_Manager.Load.Init();
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Scenes_Manager.CurrentName = sceneName;
            Mods_Managers.OnSceneWasLoaded(sceneName);
        }
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            GUI_Manager.OnSceneWasInitialized(sceneName);
            Mods_Managers.OnSceneWasInitialized(sceneName);
        }
        public override void OnLateUpdate()
        {
            Save_Manager.Load.Update();
            GUI_Manager.Update();
            Mods_Managers.Update();
        }
        public override void OnGUI()
        {
            GUI_Manager.UpdateGUI();
        }
    }
}
