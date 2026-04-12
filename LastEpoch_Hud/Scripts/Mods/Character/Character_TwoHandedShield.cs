using Il2CppRewired;
using LastEpoch_Hud.Scripts.ModUI;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    [RegisterTypeInIl2Cpp]
    public class Character_TwoHandedShield : MonoBehaviour
    {
        public Character_TwoHandedShield(System.IntPtr ptr)
            : base(ptr) { }

        public static Character_TwoHandedShield instance { get; private set; }

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            if (
                !Refs_Manager.character_mutator.IsNullOrDestroyed()
                && ModSettings.Cheats.TwoHandeWithShield.Value
            )
            {
                Refs_Manager.character_mutator.twohandersAllowedWithShieldBaseTypes = Il2Cpp
                    .Item
                    .TwoHandersAllowedWithShieldSetID
                    .ForgeGuard;
            }
        }
    }
}
