using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Spawn
{
    [RegisterTypeInIl2Cpp]
    public class TimeBeast : MonoBehaviour
    {
        public TimeBeast(System.IntPtr ptr) : base(ptr) { }
        public static TimeBeast instance { get; private set; }

        public static Ability beast_ability = null;

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                //if (Input.GetKeyDown(Save_Manager.instance.data.KeyBinds.SpawnMysteriousRift)) { SpawnMysteriousRift(); }
                //if (Input.GetKeyDown(Save_Manager.instance.data.KeyBinds.SpawnRiftBeast)) { SpawnRiftBeast(); }
                if (Input.GetKeyDown(Save_Manager.instance.data.KeyBinds.SummonBeast)) { SummonBeast(); }
                /*if (Input.GetKeyDown(KeyCode.F6)) //Debug Show all evos
                {
                    TimeBeastData time_beast_data = null;
                    foreach (TimeBeastData data in Resources.FindObjectsOfTypeAll<TimeBeastData>())
                    {
                        time_beast_data = data;
                        break;
                    }
                    if (!time_beast_data.IsNullOrDestroyed())
                    {
                        Main.logger_instance.Msg("Evos");
                        int i = 0;
                        foreach (TimeBeastData.AdaptationData adaptation in time_beast_data.adaptationData)
                        {
                            Main.logger_instance.Msg(i + " : " + adaptation.displayName);
                            i++;
                        }
                    }
                    else { Main.logger_instance.Error("TimeBeastData not found"); }
                }*/
                //BeastForever();
                //BeastGodMode();
            }
        }

        public static Ability GetBeastAbility()
        {
            Ability result = null;
            foreach (Ability ability in Resources.FindObjectsOfTypeAll<Ability>())
            {
                if (ability.abilityName == "Summon Rift Beast")
                {
                    result = ability;
                    break;
                }
            }

            return result;
        }
        /*public static void SpawnMysteriousRift()
        {
            if (!Refs_Manager.player_actor.IsNullOrDestroyed())
            {
                RandomEncounterManager.DestroyAllTimeBeastRifts();
                RandomEncounterManager.PlaceTimeBeastRift(Refs_Manager.player_actor.position(), Refs_Manager.player_actor);
            }
        }*/
        /*public static void SpawnRiftBeast()
        {
            if (!Refs_Manager.player_actor.IsNullOrDestroyed())
            {
                RandomEncounterManager.DestroyAllTimeBeastEggs();
                RandomEncounterManager.PlaceTimeBeastEggs(Refs_Manager.player_actor.position(), Refs_Manager.player_actor, null);
            }
        }*/
        public static void SummonBeast()
        {
            if ((!Refs_Manager.using_ability_player.IsNullOrDestroyed()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                if (beast_ability.IsNullOrDestroyed()) { beast_ability = GetBeastAbility(); }
                if (!beast_ability.IsNullOrDestroyed())
                {
                    Refs_Manager.using_ability_player.InitialiseAbilityUse(beast_ability, Refs_Manager.player_actor.position(), true);
                }
            }
        }
        /*public static void BeastForever() //Cannot unsummon
        {
            if (!Refs_Manager.summon_tracker.IsNullOrDestroyed())
            {
                foreach (Summoned summoned in Refs_Manager.summon_tracker.summons)
                {
                    if (summoned.name == "Time Beast Summoned(Clone)")
                    {
                        UnsummonAfterDelay unsummon = summoned.gameObject.GetComponent<UnsummonAfterDelay>();
                        if (!unsummon.IsNullOrDestroyed()) { Destroy(unsummon); }
                    }
                }
            }
        }*/
        /*public static void BeastGodMode()
        {
            if ((!Refs_Manager.summon_tracker.IsNullOrDestroyed()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (Save_Manager.instance.data.Character.Cheats.Enable_GodMode)
                {
                    foreach (Summoned summoned in Refs_Manager.summon_tracker.summons)
                    {
                        if (summoned.name == "Time Beast Summoned(Clone)")
                        {
                            UnitHealth unit_health = summoned.gameObject.GetComponent<UnitHealth>();
                            if (!unit_health.IsNullOrDestroyed())
                            {
                                BaseHealth base_health = unit_health.TryCast<BaseHealth>();
                                if (!base_health.IsNullOrDestroyed())
                                {
                                    if (base_health.damageable) { base_health.damageable = false; }
                                }
                            }
                        }
                    }
                }
            }
        }*/
    }
}
