using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Spawner
    {
        public static int Density = 999;
        public static void IncreaseDensity()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                System.Type type = obj.GetActualType();
                if (type == typeof(SpawnerPlacementRoom))
                {
                    SpawnerPlacementRoom room = obj.TryCast<SpawnerPlacementRoom>();
                    room.intendedSpawnerDensity = Density;
                }
                else if (type == typeof(SpawnerPlacementManager))
                {
                    SpawnerPlacementManager spawner_manager = obj.TryCast<SpawnerPlacementManager>();
                    spawner_manager.defaultSpawnerDensity = (int)Density;
                    spawner_manager.alwaysRollSpawnerDensity = false;
                }
            }            
        }
    }
}
