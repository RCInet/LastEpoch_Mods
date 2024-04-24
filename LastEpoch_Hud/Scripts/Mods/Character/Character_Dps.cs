using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_Dps
    {
        public static float current_health = -1;
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
                        }
                    }
                }                
            }

            [HarmonyPostfix]
            static void Postfix(ref Actor __0)
            {
                if ((Scenes.IsGameScene()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
                {
                    if (__0 != Refs_Manager.player_actor)
                    {
                        if ((!__0.gameObject.GetComponent<UnitHealth>().IsNullOrDestroyed()) && (current_health > -1))
                        {
                            float health = __0.gameObject.GetComponent<UnitHealth>().currentHealth;
                            Main.logger_instance.Msg("Player Apply " + (current_health - health) + " Damage");
                        }
                    }
                }
            }
        }
    }
}
