using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Cosmetic_Item_Object
    {
        [HarmonyPatch(typeof(CosmeticItemObject), "OnPointerClick")]
        public class CosmeticItemClick
        {
            [HarmonyPrefix]
            static bool Prefix(ref CosmeticItemObject __instance, ref UnityEngine.EventSystems.PointerEventData __0)
            {
                if (__instance != null)
                {
                    if (__0.pointerPress != null)
                    {
                        if (__0.pointerPress == true)
                        {
                            __instance.currentSelection.gameObject.active = true;
                            Mods.Cosmetics.AddCosmeticToSlot(__instance);
                        }
                    }
                    else { Main.logger_instance.Msg("Event is null"); }
                }
                else { Main.logger_instance.Msg("Item is null"); }

                return false;
            }
        }
    }
}
