using UnityEngine;
using UniverseLib;
using static ItemList;

namespace LastEpochMods.Mods
{
    public class Character
    {
        public static void Launch_LevelUp()
        {            
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(ExperienceTracker)))
            {
                if (obj.name == "MainPlayer(Clone)")
                {
                    obj.TryCast<ExperienceTracker>().LevelUp(true);
                    break;
                }
            }
        }        
    }
}
