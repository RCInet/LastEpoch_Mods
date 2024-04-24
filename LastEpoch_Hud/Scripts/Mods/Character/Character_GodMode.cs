using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    [RegisterTypeInIl2Cpp]
    public class Character_GodMode : MonoBehaviour
    {
        public static Character_GodMode instance { get; private set; }
        public Character_GodMode(System.IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            instance = this;
        }
        bool CanRun()
        {            
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_GodMode;
                }
                else { return false; }
            }
            else { return false; }
        }
        void Update()
        {
            if (!Refs_Manager.player_health.IsNullOrDestroyed())
            {
                if (CanRun())
                {
                    if (Refs_Manager.player_health.damageable) { Refs_Manager.player_health.damageable = false; }
                    if (Refs_Manager.player_health.canDie) { Refs_Manager.player_health.canDie = false; }
                }
                else
                {
                    if (!Refs_Manager.player_health.damageable) { Refs_Manager.player_health.damageable = true; }
                    if (!Refs_Manager.player_health.canDie) { Refs_Manager.player_health.canDie = true; }
                }                    
            }            
        }
    }
}
