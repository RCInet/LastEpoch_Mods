namespace LastEpochMods.OnSceneChanged
{
    public class Spawner_Placement_Manager
    {
        public static void Init()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(SpawnerPlacementManager))) //UnityEngine.Object
            {
                if ((Config.Data.mods_config.scene.Enable_SpawnerPlacementManager_defaultSpawnerDensity) |
                        (Config.Data.mods_config.scene.Enable_SpawnerPlacementManager_IncreaseExperience))
                {
                    SpawnerPlacementManager spawner_manager = obj.TryCast<SpawnerPlacementManager>();
                    if (Config.Data.mods_config.scene.Enable_SpawnerPlacementManager_defaultSpawnerDensity)
                    {
                        spawner_manager.defaultSpawnerDensity = Config.Data.mods_config.scene.SpawnerPlacementManager_defaultSpawnerDensity;
                        spawner_manager.alwaysRollSpawnerDensity = false;
                    }
                    if (Config.Data.mods_config.scene.Enable_SpawnerPlacementManager_IncreaseExperience)
                    {
                        spawner_manager.increasedExperience = Config.Data.mods_config.scene.SpawnerPlacementManager_IncreaseExperience;
                    }
                }
            }
        }        
    }
}
