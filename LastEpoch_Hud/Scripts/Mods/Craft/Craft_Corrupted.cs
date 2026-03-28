//using HarmonyLib;
//using Il2Cpp;

namespace LastEpoch_Hud.Scripts.Mods.Craft
{
    public class Craft_Corrupted
    {
        //Not finished
        /*public static ItemData item = null;

        [HarmonyPatch(typeof(CraftingManager), "OnMainItemChange")]
        public class CraftingManager_OnMainItemChange
        {
            [HarmonyPostfix]
            static void Postfix(ref CraftingManager __instance, ref Il2CppSystem.Object __0, ref ItemContainerEntryHandler __1)
            {
                if (!__0.IsNullOrDestroyed())
                {
                    item = null;
                    OneItemContainer item_container = __0.TryCast<OneItemContainer>();
                    if (!item_container.IsNullOrDestroyed())
                    {
                        if (!item_container.content.IsNullOrDestroyed())
                        {
                            item = item_container.content.data;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CraftingManager), "OnMainItemRemoved")]
        public class CraftingManager_OnMainItemRemoved
        {
            [HarmonyPostfix]
            static void Postfix(CraftingManager __instance, Il2CppSystem.Object __0, ItemContainerEntryHandler __1)
            {
                item = null;
            }
        }

        [HarmonyPatch(typeof(CraftingManager), "CheckForgeCapability")]
        public class CheckForgeCapability
        {
            [HarmonyPostfix]
            static void Postfix(ref CraftingManager __instance, ref bool __result, ref System.String __0, ref System.Boolean __1, ref System.Boolean __2, ref System.String __3)
            {
                bool r = true;
                if ((__0 == Craft_Locales.item_is_corrupted) && (Save_Manager.instance.data.modsNotInHud.Craft_Corrupted))
                {
                    __0 = "Craft Corrupted";
                    __1 = false;
                    __2 = false;
                    __3 = "";
                    __result = true;
                }
            }
        }

        [HarmonyPatch(typeof(CraftingUpgradeButton), "UpdateButton")]
        public class CraftingUpgradeButton_UpdateButton
        {
            [HarmonyPrefix]
            static void Prefix(ref CraftingUpgradeButton __instance, int __0, ref bool __1)
            {
                if ((Scenes.IsGameScene()) && (!item.IsNullOrDestroyed()) && (__0 > -1))
                {
                    if ((item.corrupted) && (Save_Manager.instance.data.modsNotInHud.Craft_Corrupted)) { __1 = true; }
                }
            }
        }*/
    }
}
