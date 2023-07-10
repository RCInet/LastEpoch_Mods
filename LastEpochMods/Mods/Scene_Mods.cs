using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Scene_Mods
    {
        public static void Launch()
        {            
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                System.Type type = obj.GetActualType();
                if (type == typeof(SpawnerPlacementManager))
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
                    break;
                }
            }
        }
    }
}
