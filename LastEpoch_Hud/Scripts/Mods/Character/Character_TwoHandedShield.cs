using HarmonyLib;
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

        // Override the 2H-weapon vs shield compatibility check
        [HarmonyPatch(
            typeof(Il2Cpp.OffhandItemContainer),
            nameof(Il2Cpp.OffhandItemContainer.IncompatibleDueTo2hWeapon)
        )]
        public class OffhandItemContainer_IncompatibleDueTo2hWeapon
        {
            [HarmonyPrefix]
            static bool Prefix(ref bool __result)
            {
                if (ModSettings.Cheats.TwoHandeWithShield.Value)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        private const int SHIELD_BASE_TYPE = (int)Il2Cpp.EquipmentType.SHIELD;
        private static int lastMainHandType = -1;
        private static int lastOffHandType = -1;

        private static bool IsNonMeleeTwoHander(int itemType)
        {
            return itemType == (int)Il2Cpp.EquipmentType.TWO_HANDED_STAFF
                || itemType == (int)Il2Cpp.EquipmentType.BOW
                || itemType == (int)Il2Cpp.EquipmentType.CROSSBOW;
        }

        [HarmonyPatch(
            typeof(Il2Cpp.EquipmentVisualsManager),
            nameof(Il2Cpp.EquipmentVisualsManager.EquipWeapon)
        )]
        public class EquipmentVisualsManager_EquipWeapon
        {
            [HarmonyPrefix]
            static bool Prefix(int itemType, Il2Cpp.IMSlotType slotType)
            {
                if (!ModSettings.Cheats.TwoHandeWithShield.Value)
                    return true;

                if (
                    slotType == Il2Cpp.IMSlotType.OffHand
                    && itemType == SHIELD_BASE_TYPE
                    && IsNonMeleeTwoHander(lastMainHandType)
                )
                {
                    lastOffHandType = itemType;
                    return false;
                }

                return true;
            }

            [HarmonyPostfix]
            static void Postfix(
                Il2Cpp.EquipmentVisualsManager __instance,
                int itemType,
                Il2Cpp.IMSlotType slotType
            )
            {
                if (slotType == Il2Cpp.IMSlotType.MainHand)
                    lastMainHandType = itemType;
                else if (slotType == Il2Cpp.IMSlotType.OffHand)
                    lastOffHandType = itemType;

                if (!ModSettings.Cheats.TwoHandeWithShield.Value)
                    return;

                if (
                    slotType == Il2Cpp.IMSlotType.MainHand
                    && IsNonMeleeTwoHander(itemType)
                    && lastOffHandType == SHIELD_BASE_TYPE
                )
                {
                    __instance.RemoveWeapon(true, false);
                }
            }
        }

        [HarmonyPatch(
            typeof(Il2Cpp.EquipmentVisualsManager),
            nameof(Il2Cpp.EquipmentVisualsManager.RemoveWeapon)
        )]
        public class EquipmentVisualsManager_RemoveWeapon
        {
            [HarmonyPostfix]
            static void Postfix(bool offHand, bool clearData)
            {
                if (!clearData)
                    return;
                if (offHand)
                    lastOffHandType = -1;
                else
                    lastMainHandType = -1;
            }
        }
    }
}
