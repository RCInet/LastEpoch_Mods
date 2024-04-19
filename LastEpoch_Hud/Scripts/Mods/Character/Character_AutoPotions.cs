using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    [RegisterTypeInIl2Cpp]
    public class Character_AutoPotions : MonoBehaviour
    {
        public static Character_AutoPotions instance { get; private set; }
        public Character_AutoPotions(System.IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            instance = this;
            if (Main.debug) { Main.logger_instance.Msg("Character_AutoPotions : Awake"); }
        }
        bool CanRun()
        {
            if ((Scenes.IsGameScene()) &&
                (!Refs_Manager.player_health.IsNullOrDestroyed()) &&
                (!Refs_Manager.health_potion.IsNullOrDestroyed()) &&
                (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_AutoPot;
                }
                else { return false; }
            }
            else { return false; }
        }
        void Update()
        {
            if (CanRun())
            {
                int player_health_percent = (int)(Refs_Manager.player_health.currentHealth / Refs_Manager.player_health.maxHealth * 100);
                int auto_pot_percent = (int)(Save_Manager.instance.data.Character.Cheats.autoPot / 255 * 100);
                if (player_health_percent < auto_pot_percent) { Refs_Manager.health_potion.UsePotion(); }
            }
        }
    }
}
