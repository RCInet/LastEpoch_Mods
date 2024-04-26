using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_Dps
    {
        //Will be used to create CombatLog or Dps meter

        public static float current_health = -1;
        public static Actor current_target_to_damage = null;
        public static Actor current_target_from_skill = null;
        public static Ability current_ability = null;
        public static bool Critical = false;
        public static bool Kill = false;

        [HarmonyPatch(typeof(DamageStatsHolder), "applyDamage", new System.Type[] { typeof(Actor) })]
        public class DamageStatsHolder_applyDamage
        {
            [HarmonyPrefix]
            static void Prefix(ref Actor __0)
            {
                current_health = -1;
                if ((Scenes.IsGameScene()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
                {
                    if (__0 != Refs_Manager.player_actor)
                    {
                        if (!__0.gameObject.GetComponent<UnitHealth>().IsNullOrDestroyed())
                        {
                            current_health = __0.gameObject.GetComponent<UnitHealth>().currentHealth;
                            current_target_to_damage = __0;
                        }
                    }
                }
            }

            [HarmonyPostfix]
            static void Postfix(ref Actor __0)
            {
                if ((Scenes.IsGameScene()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()) && (!current_target_to_damage.IsNullOrDestroyed()))
                {
                    if (__0 != Refs_Manager.player_actor)
                    {
                        if ((!__0.gameObject.GetComponent<UnitHealth>().IsNullOrDestroyed()) && (current_health > -1))
                        {
                            float health = __0.gameObject.GetComponent<UnitHealth>().currentHealth;
                            float damage_done = (current_health - health);
                            string crit = "";
                            if (Critical) { crit = " (critical)"; }
                            if ((!current_ability.IsNullOrDestroyed()) &&
                                (!current_target_to_damage.IsNullOrDestroyed()) &&
                                (!current_target_from_skill.IsNullOrDestroyed()))
                            {
                                string result = "";
                                if (!Kill) { result += "Player apply " + damage_done + " damage" + crit + " to target " + current_target_to_damage.name + " with " + current_ability.name; }
                                else { result += "Player kill target " + current_target_to_damage.name + " with " + current_ability.name + ", damage = " + damage_done + crit; }
                                
                                if (current_target_to_damage == current_target_from_skill)
                                {
                                    Main.logger_instance.Msg(result);
                                }
                            }
                            else
                            {
                                Main.logger_instance.Msg("Player Apply " + damage_done + " damage" + crit);
                            }
                        }
                    }
                }
                current_target_to_damage = null;
                current_target_from_skill = null;
                current_ability = null;
            }
        }

        [HarmonyPatch(typeof(AbilityEventListener), "HitEvent")]
        public class AbilityEventListener_HitEvent
        {
            [HarmonyPostfix]
            static void Postfix(ref Ability __0, ref Actor __1, AT __2)
            {
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {
                    if (__1 != Refs_Manager.player_actor)
                    {
                        current_ability = __0;
                        current_target_from_skill = __1;
                        Critical = false;
                        Kill = false;
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(AbilityEventListener), "CritEvent")]
        public class AbilityEventListener_CritEvent
        {
            [HarmonyPostfix]
            static void Postfix(ref Ability __0, ref Actor __1)
            {
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {                    
                    if ((__0 == current_ability) && (__1 != current_target_from_skill))
                    {
                        Critical = true;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AbilityEventListener), "KillEvent")]
        public class AbilityEventListener_KillEvent
        {
            [HarmonyPostfix]
            static void Postfix(ref Ability __0,ref Actor __1)
            {
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {
                    if ((__0 == current_ability) && (__1 != current_target_from_skill))
                    {
                        Kill = true;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AbilityEventListener), "HealEvent")]
        public class AbilityEventListener_HealEvent
        {
            [HarmonyPostfix]
            static void Postfix(AbilityEventListener __instance, ref Ability __0, ref Actor __1, bool __2, float __3)
            {
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {
                    if ((__1 == Refs_Manager.player_actor) && (__3 > 0))
                    {
                        string delayed = "";
                        if (__2) { delayed = " (OverTime)"; }
                        Main.logger_instance.Msg("Player heal for " + __3 + " with " + __0.abilityName + delayed);
                    }
                }
            }
        }        

        [HarmonyPatch(typeof(CharacterMutator), "OnAbilityUse")]
        public class OnAbilityUse
        {
            [HarmonyPostfix]
            static void Postfix(CharacterMutator __instance, AbilityInfo __0, ref AbilityMutator __1, float __2, UnityEngine.Vector3 __3, bool __4)
            {
                Main.logger_instance.Msg("Player use ability : " + __1.ability.name);
            }
        }
    }
}
