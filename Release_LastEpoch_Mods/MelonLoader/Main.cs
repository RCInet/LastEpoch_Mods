using LastEpochMods.Managers;
using System.Runtime;

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
            KeyBinds();
            Memory_Manager.Update();
            if (!Running) { DoUpdate(); }
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

        private static void KeyBinds()
        {
            if (UnityEngine.Input.GetKeyDown(Save_Manager.Data.UserData.KeyBinds.HeadhunterBuffs))
            {
                Save_Manager.Data.UserData.Items.Headhunter.showui = !Save_Manager.Data.UserData.Items.Headhunter.showui;
            }
            if (UnityEngine.Input.GetKeyDown(Save_Manager.Data.UserData.KeyBinds.BankStashs))
            {
                Mods.Items.Bank.OpenClose();
            }
            /*if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F5))
                {
                    Mods.Scenes.Monoliths.RevealIslands();
                }*/
            /*if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F6))
            {
                Mods.Scenes.Monoliths.ConnectIslands();
            }*/
        }

        private static bool Running = false;
        private static System.DateTime StartTime;
        private static readonly float Duration = 1f;
        private static void DoUpdate()
        {
            StartTime = System.DateTime.Now;
            Running = true;            
            Save_Manager.Load.Update();
            GUI_Manager.Update();
            Mods_Managers.Update();
        }

        
    }
}
