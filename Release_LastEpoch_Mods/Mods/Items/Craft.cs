using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class Craft
    {
        //Add Forgin Potencial
        [HarmonyPatch(typeof(CraftingManager), "OnMainItemChange")]
        public class CraftingManager_OnMainItemChange
        {
            [HarmonyPrefix]
            static void Postfix(ref CraftingManager __instance, ref Il2CppSystem.Object __0, ItemContainerEntryHandler __1)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (Save_Manager.Data.UserData.Items.Craft.Enable_AddForginpotencial)
                    {
                        try
                        {
                            __0.TryCast<OneItemContainer>().content.data.forgingPotential = Save_Manager.Data.UserData.Items.Craft.AddForginpotencial;
                        }
                        catch { }
                    }
                    //Edit item data here (work fine)
                    //__0.TryCast<OneItemContainer>().content.data.implicitRolls[0] = 255;
                }
            }
        }

        //NoForgingPotentialCost
        [HarmonyPatch(typeof(CraftingManager), "Forge")]
        public class CraftingManager_Forge
        {
            [HarmonyPrefix]
            static bool Prefix(ref CraftingManager __instance)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (Save_Manager.Data.UserData.Items.Craft.NoForgingPotentialCost) { __instance.debugNoForgingPotentialCost = true; }
                    else { __instance.debugNoForgingPotentialCost = false; }
                }

                return true;
            }
        }

        //NoCpabilityCheck
        [HarmonyPatch(typeof(CraftingManager), "CheckForgeCapability")]
        public class CheckForgeCapability
        {
            [HarmonyPrefix]
            static void Prefix(CraftingManager __instance, bool __result, ref System.String __0, ref System.Boolean __1)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (Save_Manager.Data.UserData.Items.Craft.DontChekCapability)
                    {
                        if (__0 == "No Forging Potential") { __0 = "Upgrade Affix"; }
                        __1 = false; //No errors
                    }
                }
            }

            [HarmonyPostfix]
            static void Postfix(CraftingManager __instance, ref bool __result, ref System.String __0, ref System.Boolean __1)
            {
                if (Scenes_Manager.GameScene())
                {
                    if ((__0 != "Add a Modifier Item") && (__0 != "Can't Forge Uniques"))
                    {
                        if (Save_Manager.Data.UserData.Items.Craft.DontChekCapability)
                        {
                            __result = true;
                        }
                    }
                }
            }
        }
    }
}
