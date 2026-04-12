using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Difficulty
{
    public class ZoneScaling
    {
        internal static int _lastOriginalZoneLevel = -1;
        private static bool _isOverriding = false;
        private static int _xpBlockCount = 0;

        private static int GetPlayerLevel()
        {
            try
            {
                if (!Refs_Manager.player_data.IsNullOrDestroyed())
                    return Refs_Manager.player_data.Level;
            }
            catch { }
            return 0;
        }

        private static int GetZoneLevel()
        {
            try
            {
                return ZoneInfoManager.ZoneLevel;
            }
            catch
            {
                return 0;
            }
        }

        [HarmonyPatch(typeof(ZoneInfoManager), "SetZoneLevel")]
        public class SetZoneLevelPatch
        {
            [HarmonyPostfix]
            static void Postfix(int level, bool alwaysUpdateClient)
            {
                try
                {
                    if (_isOverriding)
                        return;
                    if (!Scenes.IsGameScene())
                        return;
                    if (!ModSettings.Difficulty.ScaleZoneToPlayer.Value)
                        return;

                    _lastOriginalZoneLevel = level;
                    int playerLevel = GetPlayerLevel();
                    if (playerLevel <= 0)
                        return;
                    if (playerLevel <= level)
                        return;

                    _isOverriding = true;
                    try
                    {
                        ZoneInfoManager.SetZoneLevel(playerLevel, true);
                    }
                    finally
                    {
                        _isOverriding = false;
                    }
                }
                catch { }
            }
        }

        [HarmonyPatch(typeof(ExperienceTracker), "GainExpFromEnemyOrMote")]
        public class XpCapFromEnemyPatch
        {
            [HarmonyPrefix]
            static bool Prefix(ExperienceTracker __instance, ref long expAmount)
            {
                try
                {
                    return HandleXpCap(__instance, ref expAmount);
                }
                catch
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(ExperienceTracker), "GainExpDirect")]
        public class XpCapDirectPatch
        {
            [HarmonyPrefix]
            static bool Prefix(ExperienceTracker __instance, ref long expAmount)
            {
                try
                {
                    return HandleXpCap(__instance, ref expAmount);
                }
                catch
                {
                    return true;
                }
            }
        }

        private static bool HandleXpCap(ExperienceTracker tracker, ref long expAmount)
        {
            if (!Scenes.IsGameScene())
                return true;
            if (!ModSettings.Difficulty.CapLevelToZone.Value)
                return true;

            int capLevel = _lastOriginalZoneLevel > 0 ? _lastOriginalZoneLevel : GetZoneLevel();
            if (capLevel <= 0)
                return true;
            if (tracker.CurrentLevel < capLevel)
                return true;

            long currentXp = tracker.CurrentExperience;
            long nextLevelXp = tracker.NextLevelExperience;

            if (currentXp + expAmount < nextLevelXp)
                return true;

            long maxGain = nextLevelXp - currentXp - 1;
            if (maxGain <= 0)
            {
                _xpBlockCount++;
                return false;
            }

            expAmount = maxGain;
            return true;
        }
    }
}
