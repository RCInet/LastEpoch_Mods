using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    [RegisterTypeInIl2Cpp]
    public class Character_LowLife : MonoBehaviour
    {
        public static Character_LowLife instance { get; private set; }
        public Character_LowLife(System.IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            instance = this;
            if (Main.debug) { Main.logger_instance.Msg("Character_LowLife : Awake"); }
        }
        bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_LowLife;
                }
                else { return false; }
            }
            else { return false; }
        }
        void Update()
        {
            if (!Refs_Manager.player_stats.IsNullOrDestroyed())
            {
                if (CanRun())
                {
                    if (!Refs_Manager.player_stats.atLowHealth) { Refs_Manager.player_stats.atLowHealth = true; }
                }
                /*else
                {
                    if (Hud_Manager.player_stats.atLowHealth) { Hud_Manager.player_stats.atLowHealth = false; }
                }*/
            }
        }
    }
}
