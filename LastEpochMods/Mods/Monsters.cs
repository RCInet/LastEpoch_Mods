using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Monsters
    {
        public static int MagicMobExpMultiplier = 99999;
        public static int MagicMobExpMultiplierPerLevel = 99999;
        public static int RareMobExpMultiplier = 99999;
        public static int RareMobExpMultiplierPerLevel = 99999;
        public static void ExperienceMultiplier()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                System.Type type = obj.GetActualType();
                if (type == typeof(MonsterRarityMagic))
                {
                    MonsterRarityMagic magic_monsters = obj.TryCast<MonsterRarityMagic>();
                    magic_monsters.baseExperienceMultiplier = MagicMobExpMultiplier;
                    magic_monsters.experienceMultiplierPerLevel = MagicMobExpMultiplierPerLevel;
                }
                else if (type == typeof(MonsterRarityRare))
                {
                    MonsterRarityRare rare_monsters = obj.TryCast<MonsterRarityRare>();
                    rare_monsters.baseExperienceMultiplier = RareMobExpMultiplier;
                    rare_monsters.experienceMultiplierPerLevel = RareMobExpMultiplierPerLevel;
                }
            }
        }
    }
}
