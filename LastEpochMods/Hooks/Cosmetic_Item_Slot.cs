using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Cosmetic_Item_Slot
    {
        [HarmonyPatch(typeof(CosmeticItemSlot), "OnPointerClick")]
        public class OnPointerClick
        {
            [HarmonyPostfix]
            static void Postfix(ref CosmeticItemSlot __instance, ref UnityEngine.EventSystems.PointerEventData __0)
            {
                if (__0.pointerPress == true)
                {
                    Mods.Cosmetics.current_slot_name = __instance.gameObject.name;
                }
            }
        }
    }
}
