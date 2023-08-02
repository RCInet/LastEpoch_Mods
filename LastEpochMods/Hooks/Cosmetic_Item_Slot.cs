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
                Mods.Cosmetics.ItemSlot.selected_slot = __instance;
            }
        }
        
        [HarmonyPatch(typeof(CosmeticItemSlot), "SetCosmetic")]
        public class SetCosmetic
        {
            [HarmonyPostfix]
            static void Postfix(ref CosmeticItemSlot __instance, ref LE.Services.Cosmetics.Cosmetic __0)
            {
                //Main.logger_instance.Msg("CosmeticItemSlot : SetCosmetic");
            }
        }
    }
}
