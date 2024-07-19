using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    [RegisterTypeInIl2Cpp]
    public class Character_PotionReplenishment : MonoBehaviour
    {
        public static Character_PotionReplenishment instance { get; private set; }
        public Character_PotionReplenishment(System.IntPtr ptr) : base(ptr) { }

        public bool mod_enable = false; //Set to true if you want this mod to run

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) && (!Refs_Manager.health_potion.IsNullOrDestroyed()))
            {
                if (Save_Manager.instance.data.modsNotInHud.Enable_PotionResplenishment)
                {
                    if (Refs_Manager.health_potion.currentCharges < Refs_Manager.health_potion.maxCharges)
                    {
                        Refs_Manager.health_potion.currentCharges = Refs_Manager.health_potion.maxCharges;
                    }
                }
            }
        }
    }
}
