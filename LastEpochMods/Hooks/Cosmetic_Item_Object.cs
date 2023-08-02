using HarmonyLib;
using UnityEngine;
using static LastEpochMods.Mods.Cosmetics;

namespace LastEpochMods.Hooks
{
    public class Cosmetic_Item_Object
    {
        [HarmonyPatch(typeof(CosmeticItemObject), "OnPointerClick")]
        public class OnPointerClick
        {
            [HarmonyPrefix]
            static bool Prefix(ref CosmeticItemObject __instance, UnityEngine.EventSystems.PointerEventData __0)
            {
                if (__instance != null)
                {
                    foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(LE.Services.Cosmetics.Cosmetic)))
                    {
                        if (obj.name == __instance.storedCosmetic.name)
                        {
                            //Mods.Cosmetics.ItemSlot.selected_slot.SetCosmetic(obj.TryCast<LE.Services.Cosmetics.Cosmetic>());
                            Mods.Cosmetics.ItemSlot.selected_slot._storedCosmetic = obj.TryCast<LE.Services.Cosmetics.Cosmetic>();
                            Mods.Cosmetics.ItemSlot.selected_slot.cosmeticDisplayImage = __instance.itemImage;
                            Mods.Cosmetics.ItemSlot.selected_slot.ItemTypeInfoTooltip = __instance.ItemTypeInfoTooltip;
                            break;
                        }
                    }

                    

                    
                }
                else { Main.logger_instance.Msg("CosmeticItemObject is null"); }

                return false;
            }
        }
    }
}
