using MelonLoader;
using System.Linq;
using UnityEngine;
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
                    OnSceneChanged.Set_Bonuses_List.Init();
                    OnSceneChanged.Local_Tree_Data.Init();
                    OnSceneChanged.Notifications_.Init();
                    OnSceneChanged.GenerateItem.Init();
                    OnSceneChanged.Ability_Mutator.Init();
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
                if (!Mods.Items.HeadHunter.Initialized) { Mods.Items.HeadHunter.Init(); }
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F1)) { Ui.Menu.isMenuOpen = !Ui.Menu.isMenuOpen; }
                if (Scenes.GameScene())
                {                    
                    if (Input.GetKeyDown(KeyCode.F9)) //Exemple Buff Character
                    {
                        foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(StatBuffs)))
                        {
                            if (obj.name == "MainPlayer(Clone)")
                            {
                                float duration = 255;
                                SP propertie = SP.HealthRegen;
                                float added_value = 0f;
                                float increase_value = 0.3f;
                                Il2CppSystem.Collections.Generic.List<float> more_values = null;
                                AT tag = AT.None;
                                byte special_tag = 0;
                                string name = "default sigils of hope mutator HealthRegen";
                                obj.TryCast<StatBuffs>().addBuff(duration, propertie, added_value, increase_value, more_values, tag, special_tag, name);                                
                                
                                /*Buff buff = new Buff
                                {
                                    name = "default sigils of hope mutator HealthRegen - None 0FalseTrue",
                                    remainingDuration = 255,
                                    stat = new Stats.Stat
                                    {
                                        addedValue = 0,
                                        increasedValue = 0.3f,
                                        moreValues = null,
                                        property = SP.HealthRegen,
                                        specialTag = 0,
                                        tags = AT.None
                                    }
                                };
                                obj.TryCast<StatBuffs>().buffs.Add(buff);*/
                            }
                        }
                    }
                    //if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F9)) { Mods.Character.ResetMasterie(); }
                    //if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F10)) { Mods.Character.Launch_LevelUp(); }
                }
            }
        }
        public override void OnGUI()
        {
            Ui.Menu.Update();           
        }        
    }
}
