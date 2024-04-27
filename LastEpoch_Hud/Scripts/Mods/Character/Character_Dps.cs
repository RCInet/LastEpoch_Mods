using HarmonyLib;
using MelonLoader;
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

        public static bool ShowDebug = false;
        public static bool Initialized = false;

        void Awake()
        {
            instance = this;            
            SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
        }
        void Update()
        {
            if (!Initialized) { CombatLog.Init(); }            
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Initialized)
            {
                CombatLog.Add_Line("--------------------");
                CombatLog.Add_Line("Scene changed to " + scene.name);
                CombatLog.Add_Line("--------------------");
                if (ShowDebug) { Main.logger_instance.Msg("CombatLog : Scene changed to " + scene.name); }
            }
            else { Main.logger_instance.Error("Scene changed : CombatLog : Not Initialized"); }
        }

        public static void Player_OnDamageTaken(float health_diff, bool Hit, bool Critical, bool Kill, string AbilityName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (Kill) { sb.Append("Player died : "); }
            else { sb.Append("Player take "); }
            sb.Append(health_diff + " damage ");
            if (Critical) { sb.Append("(critical) "); }
            if (!Hit) { sb.Append("(not a hit) "); }            
            sb.Append("from " + AbilityName);

            if (ShowDebug) { Main.logger_instance.Msg(sb.ToString()); }
            CombatLog.Add_Line(sb.ToString());
        }
        public static void Enemy_OnDamageTaken(Actor enemy, float health_diff, bool Hit, bool Critical, bool Kill, string AbilityName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(enemy.name);
            if (Kill) { sb.Append(" died : "); }
            else { sb.Append(" take "); }
            sb.Append(health_diff + " damage ");
            if (Critical) { sb.Append("(critical) "); }
            if (!Hit) { sb.Append("(not a hit) "); }
            sb.Append("from " + AbilityName);

            if (ShowDebug) { Main.logger_instance.Msg(sb.ToString()); }
            CombatLog.Add_Line(sb.ToString());
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

        public class Damage
        {
            public static float current_health = -1;
            public static Actor current_target = null;
            public static string current_ability_name = "";
            public static bool Hit = false;
            public static bool Critical = false;
            public static bool Kill = false;
            
            [HarmonyPatch(typeof(DamageStatsHolder), "applyDamage", new System.Type[] { typeof(Actor) })]
            public class DamageStatsHolder_applyDamage
            {
                [HarmonyPrefix]
                static void Prefix(ref DamageStatsHolder __instance, ref Actor __0)
                {
                    current_health = -1;
                    current_target = __0; //null;
                    current_ability_name = __instance.getAbilityName(); //"";
                    Hit = false;
                    Critical = false;
                    Kill = false;

                    if ((Scenes.IsGameScene()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
                    {
                        if (__0 == Refs_Manager.player_actor)
                        {
                            if (!__0.gameObject.GetComponent<PlayerHealth>().IsNullOrDestroyed())
                            {
                                current_health = __0.gameObject.GetComponent<PlayerHealth>().currentHealth;
                            }
                            else { Main.logger_instance.Error("DamageStatsHolder:applyDamage Prefix : Can't get player health from target : " + __0.name); }
                        }
                        else
                        {
                            if (!__0.gameObject.GetComponent<UnitHealth>().IsNullOrDestroyed())
                            {
                                current_health = __0.gameObject.GetComponent<UnitHealth>().currentHealth;
                            }
                            else { Main.logger_instance.Error("DamageStatsHolder:applyDamage Prefix : Can't get enenmy health from target : " + __0.name); }
                        }
                    }
                }

                [HarmonyPostfix]
                static void Postfix(ref DamageStatsHolder __instance, ref Actor __0)
                {
                    if ((Scenes.IsGameScene()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()) &&
                        (current_target == __0) && (current_ability_name == __instance.getAbilityName()) &&
                        (current_health > -1))
                    {
                        if (__0 == Refs_Manager.player_actor)
                        {
                            if (!__0.gameObject.GetComponent<PlayerHealth>().IsNullOrDestroyed())
                            {
                                float health = __0.gameObject.GetComponent<PlayerHealth>().currentHealth;
                                float health_diff = (current_health - health);
                                Player_OnDamageTaken(health_diff, Hit, Critical, Kill, current_ability_name);
                            }
                            else { Main.logger_instance.Error("DamageStatsHolder:applyDamage Postfix : Can't get player health from target : " + __0.name); }
                        }
                        else if (__0 != Refs_Manager.player_actor)
                        {
                            if (!__0.gameObject.GetComponent<UnitHealth>().IsNullOrDestroyed())
                            {
                                float health = __0.gameObject.GetComponent<UnitHealth>().currentHealth;
                                float health_diff = (current_health - health);
                                Enemy_OnDamageTaken(__0, health_diff, Hit, Critical, Kill, current_ability_name);
                            }
                            /*else if () //Minions
                            {

                            }*/
                            else { Main.logger_instance.Error("DamageStatsHolder:applyDamage Postfix : Can't get enenmy health from target : " + __0.name); }
                        }
                        else
                        {
                            Main.logger_instance.Error("DamageStatsHolder:applyDamage Postfix : current_target = " + __0.name +
                                ", hit = " + Hit +
                                ", critical = " + Critical +
                                ", kill = " + Kill +
                                ", ability = " + current_ability_name +
                                ", __instance.ability = " + __instance.getAbilityName() +
                                ", Health = " + current_health);
                        }
                    }
                    current_target = null;
                    current_ability_name = "";
                }
            }

            [HarmonyPatch(typeof(AbilityEventListener), "HitEvent")]
            public class AbilityEventListener_HitEvent
            {
                [HarmonyPostfix]
                static void Postfix(ref Ability __0, ref Actor __1) //, AT __2)
                {
                    //Main.logger_instance.Msg("Postfix AbilityEventListener:HitEvent Ability = " + __0.abilityName + ", Actor = " + __1.name);
                    if ((__0.abilityName == current_ability_name) && (__1 == current_target))
                    {
                        Hit = true;
                    }
                }
            }

            [HarmonyPatch(typeof(AbilityEventListener), "CritEvent")]
            public class AbilityEventListener_CritEvent
            {
                [HarmonyPostfix]
                static void Postfix(ref Ability __0, ref Actor __1)
                {
                    //Main.logger_instance.Msg("Postfix AbilityEventListener:CritEvent Ability = " + __0.abilityName + ", Actor = " + __1.name);
                    if ((__0.abilityName == current_ability_name) && (__1 == current_target))
                    { Critical = true; }
                }
            }

            [HarmonyPatch(typeof(AbilityEventListener), "KillEvent")]
            public class AbilityEventListener_KillEvent
            {
                [HarmonyPostfix]
                static void Postfix(ref Ability __0, ref Actor __1)
                {
                    //Main.logger_instance.Msg("Postfix AbilityEventListener:KillEvent Ability = " + __0.abilityName + ", Actor = " + __1.name);
                    if ((__0.abilityName == current_ability_name) && (__1 == current_target))
                    { Kill = true; }
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
                [HarmonyPostfix]
                static void Postfix(ref Ability __0, ref Actor __1)
                {
                    if ((Scenes.IsGameScene()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
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
                    if (File.Exists(path + filename))
                    {
                        File.WriteAllText(path + filename, string.Empty);
                        Initialized = true;

                        Add_Line("--------------------");
                        Add_Line("Session Start");
                        Add_Line("--------------------");
                        
                        if (ShowDebug) { Main.logger_instance.Msg("CombatLog : Session Start"); }                                              
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
                else if (!Initialized) { Main.logger_instance.Error("CombatLog : Not Initialized"); }
                else if (!File.Exists(path + filename)) { Main.logger_instance.Error("CombatLog : File not found"); }
            }
        }
    }
}
