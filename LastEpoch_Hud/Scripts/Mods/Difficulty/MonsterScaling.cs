using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Difficulty
{
    public class MonsterScaling
    {
        static MonsterScaling()
        {
            ModSettings.Difficulty.Reset.Clicked += ResetAll;
        }

        private static void ResetAll()
        {
            ModSettings.Difficulty.EnemyHealthMult.SetEnabled(false);
            ModSettings.Difficulty.EnemyHealthMult.SetValue(1.0f);
            ModSettings.Difficulty.EnemyDamageMult.SetEnabled(false);
            ModSettings.Difficulty.EnemyDamageMult.SetValue(1.0f);
            ModSettings.Difficulty.EnemySpeedMult.SetEnabled(false);
            ModSettings.Difficulty.EnemySpeedMult.SetValue(1.0f);
            ModSettings.Difficulty.ForceMonsterRarity.Select(0, true);
            ModSettings.Difficulty.ScaleZoneToPlayer.Set(false);
            ModSettings.Difficulty.CapLevelToZone.Set(false);
        }

        private static int GetForceRarity()
        {
            var radio = ModSettings.Difficulty.ForceMonsterRarity;
            if (radio.IsSelected(2))
                return 2; // Rare
            if (radio.IsSelected(1))
                return 1; // Magic
            return 0; // Off
        }

        private static bool IsEnemy(Actor actor)
        {
            if (actor.IsNullOrDestroyed())
                return false;
            if (actor.data.IsNullOrDestroyed())
                return false;
            var t = actor.data.actorType;
            return t == ActorData.Type.Normal
                || t == ActorData.Type.MiniBoss
                || t == ActorData.Type.Boss;
        }

        private static bool ShouldForceRarity(Actor actor)
        {
            if (actor.IsNullOrDestroyed())
                return false;
            if (actor.isPlayerActor)
                return false;
            if (actor.data.IsNullOrDestroyed())
                return false;
            if (actor.data.actorType != ActorData.Type.Normal)
                return false;
            if (actor.health.IsNullOrDestroyed())
                return false;
            if (actor.health.TryCast<UnitHealth>() == null)
                return false;
            return true;
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

        [HarmonyPatch(typeof(UnitHealth), "get_EffectiveHealthMultiplier")]
        public class HealthMultiplierPatch
        {
            [HarmonyPostfix]
            static void Postfix(UnitHealth __instance, ref float __result)
            {
                try
                {
                    if (!Scenes.IsGameScene())
                        return;
                    if (!ModSettings.Difficulty.EnemyHealthMult.Enabled)
                        return;
                    if (!IsEnemy(__instance.actor))
                        return;

                    __result *= ModSettings.Difficulty.EnemyHealthMult.Value;
                }
                catch { }
            }
        }

        [HarmonyPatch(typeof(Actor), "Start")]
        public class ActorStartPatch
        {
            [HarmonyPostfix]
            static void Postfix(Actor __instance)
            {
                try
                {
                    if (!Scenes.IsGameScene())
                        return;
                    if (__instance.isPlayerActor)
                        return;
                    if (!IsEnemy(__instance))
                        return;
                    if (__instance.statBuffs.IsNullOrDestroyed())
                        return;

                    ApplyDamageBuff(__instance);
                    ApplySpeedBuff(__instance);
                    ApplyForceRarity(__instance);
                }
                catch { }
            }
        }

        private static Il2CppSystem.Collections.Generic.List<float> MoreList(float mult)
        {
            var list = new Il2CppSystem.Collections.Generic.List<float>();
            list.Add(mult - 1.0f);
            return list;
        }

        private static void ApplyDamageBuff(Actor actor)
        {
            if (!ModSettings.Difficulty.EnemyDamageMult.Enabled)
                return;
            actor.statBuffs.addBuff(
                float.MaxValue,
                SP.Damage,
                0f,
                0f,
                MoreList(ModSettings.Difficulty.EnemyDamageMult.Value),
                AT.None,
                0,
                0,
                "DifficultyDamageMult"
            );
        }

        private static void ApplySpeedBuff(Actor actor)
        {
            if (!ModSettings.Difficulty.EnemySpeedMult.Enabled)
                return;
            var more = MoreList(ModSettings.Difficulty.EnemySpeedMult.Value);
            actor.statBuffs.addBuff(
                float.MaxValue,
                SP.AttackSpeed,
                0f,
                0f,
                more,
                AT.None,
                0,
                0,
                "DifficultyAttackSpeed"
            );
            actor.statBuffs.addBuff(
                float.MaxValue,
                SP.CastSpeed,
                0f,
                0f,
                more,
                AT.None,
                0,
                0,
                "DifficultyCastSpeed"
            );
            actor.statBuffs.addBuff(
                float.MaxValue,
                SP.Movespeed,
                0f,
                0f,
                more,
                AT.None,
                0,
                0,
                "DifficultyMoveSpeed"
            );
        }

        private static void ApplyForceRarity(Actor actor)
        {
            int force = GetForceRarity();
            if (force <= 0)
                return;
            if (!ShouldForceRarity(actor))
                return;

            Actor.Rarity current = actor.getRarity();
            int level = GetZoneLevel();

            if (force == 1 && current == Actor.Rarity.Normal)
            {
                actor.setRarity(Actor.Rarity.Magic);
                MonsterRarityManager.setRarity(
                    actor.data,
                    actor,
                    SpawnerContext.Default,
                    level,
                    false,
                    1,
                    -1,
                    -1,
                    false
                );
            }
            else if (force >= 2 && current != Actor.Rarity.Rare)
            {
                actor.setRarity(Actor.Rarity.Rare);
                MonsterRarityManager.setRarity(
                    actor.data,
                    actor,
                    SpawnerContext.Default,
                    level,
                    true,
                    1,
                    -1,
                    -1,
                    false
                );
            }
        }

        [HarmonyPatch(typeof(Spawner), "chooseRarity")]
        public class SpawnerRarityPatch
        {
            [HarmonyPostfix]
            static void Postfix(ref Spawner.MonsterRarity __result)
            {
                try
                {
                    int force = GetForceRarity();
                    if (force <= 0)
                        return;

                    if (force == 1 && __result == Spawner.MonsterRarity.Common)
                        __result = Spawner.MonsterRarity.Magic;
                    else if (force >= 2)
                        __result = Spawner.MonsterRarity.Rare;
                }
                catch { }
            }
        }

        [HarmonyPatch(typeof(Actor), "setRarity")]
        public class ActorSetRarityPatch
        {
            [HarmonyPrefix]
            static void Prefix(Actor __instance, ref Actor.Rarity __0)
            {
                try
                {
                    int force = GetForceRarity();
                    if (force <= 0)
                        return;
                    if (!ShouldForceRarity(__instance))
                        return;

                    if (force == 1 && __0 == Actor.Rarity.Normal)
                        __0 = Actor.Rarity.Magic;
                    else if (force >= 2)
                        __0 = Actor.Rarity.Rare;
                }
                catch { }
            }
        }

        [HarmonyPatch(typeof(MonsterRarityManager), "setRarity")]
        public class MonsterRarityManagerPatch
        {
            [HarmonyPrefix]
            static void Prefix(ActorData __0, ref bool __4)
            {
                try
                {
                    int force = GetForceRarity();
                    if (force <= 0)
                        return;
                    if (__0 == null || __0.actorType != ActorData.Type.Normal)
                        return;

                    if (force >= 2)
                        __4 = true;
                }
                catch { }
            }
        }
    }
}
