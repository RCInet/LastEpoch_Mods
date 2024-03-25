using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class Craft
    {
        public static ItemData current_item;

        [HarmonyPatch(typeof(CraftingManager), "OnMainItemChange")]
        public class CraftingManager_OnMainItemChange
        {
            [HarmonyPostfix]
            static void Postfix(ref CraftingManager __instance, ref Il2CppSystem.Object __0, ItemContainerEntryHandler __1)
            {
                if (Scenes_Manager.GameScene())
                {
                    try { current_item = __0.TryCast<OneItemContainer>().content.data; }
                    catch { current_item = null; }

                    if (Save_Manager.Data.UserData.Items.Craft.NoForgingPotentialCost) { __instance.debugNoForgingPotentialCost = true; }
                    else { __instance.debugNoForgingPotentialCost = false; }

                    if ((Save_Manager.Data.UserData.Items.Craft.Enable_AddForginpotencial) && (!current_item.IsNullOrDestroyed()))
                    {
                        current_item.forgingPotential = Save_Manager.Data.UserData.Items.Craft.AddForginpotencial;
                        current_item.RefreshIDAndValues();
                    }

                    if (!current_item.IsNullOrDestroyed())
                    {
                        int i = 0;
                        foreach (Save_Manager.Data.Craft_Implicits impl in Save_Manager.Data.UserData.Items.Craft.Implicits)
                        {
                            if (i < current_item.implicitRolls.Count)
                            {
                                if (impl.Enable_Implicit) { current_item.implicitRolls[i] = Save_Manager.Data.UserData.Items.Craft.Implicits[i].Implicit; }
                            }
                            else { break; }
                            i++;
                        }

                        i = 0;
                        foreach (Save_Manager.Data.Craft_Affixs affix in Save_Manager.Data.UserData.Items.Craft.Affix)
                        {
                            if (i < current_item.affixes.Count)
                            {
                                if (affix.Enable_Affix_Tier) { current_item.affixes[i].affixTier = (byte)(Save_Manager.Data.UserData.Items.Craft.Affix[i].Tier - 1); }
                                if (affix.Enable_Affix_Value) { current_item.affixes[i].affixRoll = Save_Manager.Data.UserData.Items.Craft.Affix[i].Value; }
                            }
                            else { break; }
                            i++;
                        }
                        current_item.RefreshIDAndValues();
                    }
                    /*
                    int i = 0;
                    System.Reflection.PropertyInfo lucky = null;
                    System.Reflection.PropertyInfo critical = null;
                    foreach (System.Reflection.PropertyInfo info in typeof(CraftingManager).GetProperties())
                    {
                        Main.logger_instance.Msg("Propertie[" + i + "] = " + info.Name);
                        if (info.Name == "chanceForLuckyRollWhenRollingForgingPotentialCost") { lucky = info; }
                        if (info.Name == "criticalSuccessChance") { critical = info; }
                        i++;
                    }
                    if (Save_Manager.Data.UserData.Items.Craft.Enable_LuckyRollChance)
                    {
                        if (lucky != null)
                        {
                            System.Type value_type = lucky.GetType();
                            System.Object obj = lucky.GetValue(__instance);
                            System.Type obj_type = obj.GetType();
                            float roll_chance = Save_Manager.Data.UserData.Items.Craft.LuckyRollChance / 255;
                            Main.logger_instance.Msg("lucky : Value = " + obj + " to " + roll_chance);
                            //lucky.SetValue(__instance, System.Convert.ToSingle(roll_chance));
                        }
                    }
                    if (Save_Manager.Data.UserData.Items.Craft.Enable_CritChance)
                    {
                        if (critical != null)
                        {
                            System.Type value_type = critical.GetType();
                            System.Object obj = critical.GetValue(__instance);
                            System.Type obj_type = obj.GetType();
                            float roll_chance = Save_Manager.Data.UserData.Items.Craft.CritChance / 255;
                            Main.logger_instance.Msg("Critical : Value = " + obj + " to " + roll_chance);
                            //critical.SetValue(__instance, System.Convert.ToSingle(roll_chance));
                        }
                    }*/
                }
            }
        }

        //NoForgingPotentialCost
        /*[HarmonyPatch(typeof(CraftingManager), "Forge")]
        public class CraftingManager_Forge
        {
            [HarmonyPrefix]
            static void Prefix(ref CraftingManager __instance)
            {
                if (Scenes_Manager.GameScene())
                {
                    


                    if (Save_Manager.Data.UserData.Items.Craft.NoForgingPotentialCost) { __instance.debugNoForgingPotentialCost = true; }
                    else { __instance.debugNoForgingPotentialCost = false; }
                }
            }
            [HarmonyPostfix]
            static void Postfix(ref CraftingManager __instance)
            {
                if ((Scenes_Manager.GameScene()) && (!current_item.IsNullOrDestroyed()))
                {
                    
                }
                else { Main.logger_instance.Error("current_item is null"); }
            }
        }*/

        //NoCpabilityCheck
        /*[HarmonyPatch(typeof(CraftingManager), "CheckForgeCapability")]
        public class CheckForgeCapability
        {
            [HarmonyPrefix]
            static void Prefix(CraftingManager __instance, ref bool __result, ref System.String __0, System.Boolean __1)
            {
                Main.logger_instance.Msg("Forge Prefix : " + __0 + ", Error = " + __1 + ", Result = " + __result);
                if ((Scenes_Manager.GameScene()) && (Save_Manager.Data.UserData.Items.Craft.DontChekCapability))
                {
                    __result = true;
                    return false;
                    //if (Save_Manager.Data.UserData.Items.Craft.DontChekCapability)
                    //{
                        //if (__0 == "No Forging Potential") { __0 = "Upgrade Affix"; }
                        //__1 = false; //No errors                        
                    //}                    
                }
                else { return true; }
            }

            [HarmonyPostfix]
            static void Postfix(CraftingManager __instance, ref bool __result, ref System.String __0, ref System.Boolean __1)
            {
                if ((Scenes_Manager.GameScene()) && (Save_Manager.Data.UserData.Items.Craft.DontChekCapability))
                {
                    Main.logger_instance.Msg("Forge PostFix : " + __0 + ", Error = " + __1 + ", Result = " + __result);
                    if ((__0 == "Can't Forge Uniques") && (current_item.rarity == 9))
                    {
                        __0 = "Legendary Craft";
                        __1 = false;
                        __result = true;
                    }
                    if ((__0 == "No Forging Potential") && (Save_Manager.Data.UserData.Items.Craft.NoForgingPotentialCost))
                    {
                        __0 = "No Cost";
                        __1 = false;
                        __result = true;
                    }
                    Main.logger_instance.Msg("Fix : " + __0 + ", Error = " + __1 + ", Result = " + __result);
                }
            }
        }*/
    }
}
