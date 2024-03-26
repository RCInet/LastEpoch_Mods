using LastEpochMods.Managers;
using UnityEngine.Profiling;

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
            //FixMemory();
        }

        const long kCollectAfterAllocating = 8 * 1024 * 1024;
        const long kHighWater = 128 * 1024 * 1024;
        public static long lastFrameMemory = 0;
        public static long nextCollectAt = 0;
        private static void FixMemory()
        {
            long mem = Profiler.GetMonoUsedSizeLong();
            if (mem < lastFrameMemory) { nextCollectAt = mem + kCollectAfterAllocating; }
            if (mem > kHighWater) { System.GC.Collect(0); }
            else if (mem >= nextCollectAt)
            {
                UnityEngine.Scripting.GarbageCollector.CollectIncremental(0);
                lastFrameMemory = mem + kCollectAfterAllocating;
            }
            lastFrameMemory = mem;
        }
    }
}
