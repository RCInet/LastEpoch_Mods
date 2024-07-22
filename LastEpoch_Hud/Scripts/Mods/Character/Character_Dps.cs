using HarmonyLib;
using MelonLoader;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    [RegisterTypeInIl2Cpp]
    public class Character_Dps : MonoBehaviour
    {
        public static Character_Dps instance { get; private set; }
        public Character_Dps(System.IntPtr ptr) : base(ptr) { }

        public static bool Mod_Enable = false; //Set true if you want this mod to work
        public static bool ShowDebug = false;
        public static bool Initialized = false;

        void Awake()
        {
            instance = this;            
            SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
        }
        void Update()
        {
            if ((CanRun()) && (Initialized))
            {
                CombatLog.Init();
            }                       
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Initialized)
            {
                CombatLog.Add_Line("--------------------");
                CombatLog.Add_Line("Scene changed to " + scene.name);
                if (ShowDebug) { Main.logger_instance.Msg("CombatLog : Scene changed to " + scene.name); }
            }
        }
        private static bool CanRun()
        {
            if (!Save_Manager.instance.IsNullOrDestroyed())
            {
                if (Save_Manager.instance.data.modsNotInHud.Enable_CombatLog) { return true; }
                else { return false; }
            }
            else { return false; }
        }          
        
        public static void Player_OnHealth(float health_diff, string AbilityName)
        {
            string s = "Player health for " + health_diff + " from " + AbilityName;
            if (ShowDebug) { Main.logger_instance.Msg(s); }
            CombatLog.Add_Line(s);
        }
        public static void Enemy_OnHealth(Actor enemy, float health_diff, string AbilityName)
        {
            string s = enemy.name + " health for = " + health_diff + " with " + AbilityName;
            if (ShowDebug) { Main.logger_instance.Msg(s); }
            CombatLog.Add_Line(s);
        }

        private static float Get_Health(ref Actor actor)
        {
            float result = -1;
            if (actor == Refs_Manager.player_actor)
            {
                if (!actor.gameObject.GetComponent<PlayerHealth>().IsNullOrDestroyed())
                {
                    result = actor.gameObject.GetComponent<PlayerHealth>().currentHealth;
                }
                else { Main.logger_instance.Error("DamageStatsHolder:applyDamage Prefix : Can't get player health from target : " + actor.name); }
            }
            else
            {
                if (!actor.gameObject.GetComponent<UnitHealth>().IsNullOrDestroyed())
                {
                    result = actor.gameObject.GetComponent<UnitHealth>().currentHealth;
                }
                else { Main.logger_instance.Error("DamageStatsHolder:applyDamage Prefix : Can't get (enenmy or summoned) health from target : " + actor.name); }
            }

            return result;
        }
        private static bool Get_IsSummoned(Actor actor)
        {
            bool result = false;
            if (!Refs_Manager.player_actor.IsNullOrDestroyed())
            {
                SummonTracker summon_tracker = Refs_Manager.player_actor.gameObject.GetComponent<SummonTracker>();
                if (!summon_tracker.IsNullOrDestroyed())
                {
                    foreach (Summoned summon in summon_tracker.summons)
                    {
                        if (summon.actor == actor)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }
        private static string Get_ActorType(Actor actor)
        {
            string result = "";
            if (!Refs_Manager.player_actor.IsNullOrDestroyed())
            {
                if (actor == Refs_Manager.player_actor) { result = "Player"; }
                else
                {
                    if (Get_IsSummoned(actor)) { result = "Summon"; }
                    else { result = "Enemy"; }
                }
            }

            return result;
        }

        public class Damage
        {
            public static bool Hit = false;
            public static bool Critical = false;
            public static bool Kill = false;
            public static Actor current_source_actor = null;
            public static Ability current_source_ability = null;
            public static Actor current_target_actor = null;
            public static float previous_health = -1;
            
            [HarmonyPatch(typeof(DamageStatsHolder), "applyDamage", new System.Type[] { typeof(Actor) })]
            public class DamageStatsHolder_applyDamage
            {
                [HarmonyPrefix]
                static void Prefix(ref DamageStatsHolder __instance, ref Actor __0)
                {
                    if (CanRun())
                    {
                        Hit = false;
                        Critical = false;
                        Kill = false;
                        previous_health = -1;
                        current_source_actor = null;
                        current_source_ability = null;
                        current_target_actor = null;
                        if ((Scenes.IsGameScene()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
                        {
                            current_source_ability = __instance.GetDamageSourceInfo().Item2;
                            current_source_actor = __instance.GetDamageSourceInfo().Item3;
                            current_target_actor = __0;
                            previous_health = Get_Health(ref __0);
                        }
                    }
                }
                
                [HarmonyPostfix]
                static void Postfix(ref DamageStatsHolder __instance, ref Actor __0)
                {
                    if ((Scenes.IsGameScene()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()) && (CanRun()))
                    {
                        Il2CppSystem.ValueTuple<Ailment, Ability, Actor, IrregularDamageSourceID> source = __instance.GetDamageSourceInfo();
                        Ailment source_ailment = source.Item1;
                        Ability source_ability = source.Item2;
                        Actor source_actor = source.Item3;
                        //IrregularDamageSourceID source_irregularDamageSourceID = source.Item4;

                        if ((current_source_ability == source_ability) &&
                            (current_source_actor == source_actor) &&
                            (current_target_actor == __0))
                        {
                            CombatLog.Add_Line("--------------------");
                            CombatLog.Add_Line("Source : " + Get_ActorType(source_actor));
                            CombatLog.Add_Line("Actor = " + source_actor.name);
                            CombatLog.Add_Line("Ability = " + source_ability.abilityName);
                            if (!source_ailment.IsNullOrDestroyed())
                            {
                                CombatLog.Add_Line("Ailment = " + source_ailment.name + ", " + source_ailment.displayName);
                            }
                            CombatLog.Add_Line("Damage modifier = " + __instance.getDamageModifier());
                            if (!__instance.damageStats.IsNullOrDestroyed())
                            {
                                CombatLog.Add_Line("Critical chance = " + (__instance.damageStats.critChance * 100) + " %");
                                CombatLog.Add_Line("Critical multiplier = " + (__instance.damageStats.critMultiplier * 100));
                                CombatLog.Add_Line("Cull percent = " + __instance.damageStats.cullPercent);
                                CombatLog.Add_Line("Freeze rate = " + __instance.damageStats.freezeRate);
                                CombatLog.Add_Line("Increased stun chance = " + __instance.damageStats.increasedStunChance);
                                CombatLog.Add_Line("Increased stun duration = " + __instance.damageStats.increasedStunDuration);
                                int i = 0;
                                foreach (float dmg in __instance.damageStats.damage)
                                {
                                    CombatLog.Add_Line("Damage[" + i + "] = " + dmg);
                                    i++;
                                }
                                i = 0;
                                if (!__instance.damageStats.penetration.IsNullOrDestroyed())
                                {
                                    foreach (float pen in __instance.damageStats.penetration.penetration)
                                    {
                                        CombatLog.Add_Line("Penetration[" + i + "] = " + pen);
                                        i++;
                                    }
                                }
                            }
                            CombatLog.Add_Line("");
                            CombatLog.Add_Line("Target : " + Get_ActorType(__0));
                            CombatLog.Add_Line("Actor = " + __0.name);                            
                            float health_diff = (previous_health - Get_Health(ref __0));
                            string damage_done = "Damage = " + health_diff;
                            if (Hit) { damage_done += " (Hit)"; }
                            if (Critical) { damage_done += " (Critical)"; }
                            if (Kill) { damage_done += " (Kill)"; }
                            CombatLog.Add_Line(damage_done);
                            float overkill_damage = (__instance.getTotalDamage() - health_diff);
                            if (overkill_damage > 0) { CombatLog.Add_Line("Overkill damage = " + overkill_damage); }
                            //else if (overkill_damage < 0) { Main.logger_instance.Error("Overkill damage = " + overkill_damage); }                            
                        }
                    }
                }
            }
            
            [HarmonyPatch(typeof(AbilityEventListener), "HitEvent")]
            public class AbilityEventListener_HitEvent
            {
                [HarmonyPostfix]
                static void Postfix(ref Ability __0, ref Actor __1) //, AT __2)
                {
                    if (CanRun())
                    {
                        if ((__0 == current_source_ability) && (__1 == current_target_actor))
                        {
                            Hit = true;
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
                    if (CanRun())
                    {
                        if ((__0 == current_source_ability) && (__1 == current_target_actor))
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
                static void Postfix(ref Ability __0, ref Actor __1)
                {
                    if (CanRun())
                    {
                        if ((__0 == current_source_ability) && (__1 == current_target_actor))
                        {
                            Kill = true;
                        }
                    }
                }
            }            
        }
        public class Health
        {
            public static float current_health = -1;

            [HarmonyPatch(typeof(AbilityEventListener), "HealEvent")]
            public class AbilityEventListener_HealEvent
            {
                [HarmonyPrefix]
                static void Prefix(ref Ability __0, ref Actor __1)
                {
                    if (CanRun())
                    {
                        current_health = -1;
                        if ((Scenes.IsGameScene()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
                        {
                            if (__1 == Refs_Manager.player_actor)
                            {
                                if (!__1.gameObject.GetComponent<PlayerHealth>().IsNullOrDestroyed())
                                {
                                    current_health = __1.gameObject.GetComponent<PlayerHealth>().currentHealth;
                                }
                                else { Main.logger_instance.Error("AbilityEventListener:HealEvent Prefix : Can't get player health from target : " + __1.name); }
                            }
                            else
                            {
                                if (!__1.gameObject.GetComponent<UnitHealth>().IsNullOrDestroyed())
                                {
                                    current_health = __1.gameObject.GetComponent<UnitHealth>().currentHealth;
                                }
                                else { Main.logger_instance.Error("AbilityEventListener:HealEvent Prefix : Can't get enenmy health from target : " + __1.name); }
                            }
                        }
                    }
                }
                [HarmonyPostfix]
                static void Postfix(ref Ability __0, ref Actor __1)
                {
                    if ((Scenes.IsGameScene()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()) && (CanRun()))
                    {
                        float health_diff = 0;
                        if (__1 == Refs_Manager.player_actor)
                        {
                            if (!__1.gameObject.GetComponent<PlayerHealth>().IsNullOrDestroyed())
                            {
                                float health = __1.gameObject.GetComponent<PlayerHealth>().currentHealth;
                                health_diff = (current_health - health);
                                Player_OnHealth(health_diff, __0.abilityName);
                            }
                            else { Main.logger_instance.Error("AbilityEventListener:HealEvent Postfix : Can't get player health from target : " + __1.name); }
                        }
                        else
                        {
                            if (!__1.gameObject.GetComponent<UnitHealth>().IsNullOrDestroyed())
                            {
                                float health = __1.gameObject.GetComponent<UnitHealth>().currentHealth;
                                health_diff = (current_health - health);
                                Enemy_OnHealth(__1, health_diff, __0.abilityName);
                            }
                            else { Main.logger_instance.Error("AbilityEventListener:HealEvent Postfix : Can't get enenmy health from target : " + __1.name); }
                        }
                    }
                }
            }
        }
        public class CombatLog
        {
            public static string path = Directory.GetCurrentDirectory() + @"\Mods\" + Main.mod_name + @"\Logs\";
            public static string filename = "Combatlog.txt";
            public static System.Collections.Generic.List<string> log = new System.Collections.Generic.List<string>();
            public static bool initializing = false;
            public static int lines = 0;

            public static void Init()
            {
                if (!initializing)
                {
                    initializing = true;
                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                    if (!File.Exists(path + filename)) { File.Create(path + filename); }
                    
                    try
                    {
                        File.WriteAllText(path + filename, string.Empty);
                        Initialized = true;

                        Add_Line("--------------------");
                        Add_Line("Session Start");

                        if (ShowDebug) { Main.logger_instance.Msg("CombatLog : Session Start"); }

                    }
                    catch (Exception e)
                    {
                        if (ShowDebug) { Main.logger_instance.Error(e.ToString()); }
                    }
                    initializing = false;
                }
            }
            public static void Add_Line(string s)
            {
                if ((File.Exists(path + filename)) && (Initialized))
                {
                    string text = "";
                    if (lines > 0) { text += System.Environment.NewLine; }
                    text += System.DateTime.Now.ToString() + " : " + s;
                    File.AppendAllText(path + filename, text);
                    lines++;
                }
            }
        }
    }
}
