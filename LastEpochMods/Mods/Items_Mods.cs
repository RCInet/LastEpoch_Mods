using HarmonyLib;

namespace LastEpochMods.Mods
{
    public class Items_Mods
    {
        public class Drop_Mods
        {
            //Drop Rarity
            public static bool Enable_Rarity = true;
            public static byte GenerateItem_Rarity = 7;
            //Implicits
            public static bool Enable_RollImplicit = true;
            public static byte Roll_Implicit = 255;   
            //Affixes
            public static bool Enable_AffixsValue = true;
            public static byte Roll_AffixValue = 255;
            public static bool Enable_AffixsTier = true;
            public static byte Roll_AffixTier = 7;
            //Forging Potencial
            public static bool Enable_ForgingPotencial = true;
            public static byte Roll_ForgingPotencial = 255;
            //Unique Mods
            public static bool Enable_UniqueMod = true;
            public static byte Roll_UniqueMod = 255;
            //Legendat Potencial
            public static bool Enable_RollLegendayPotencial = true;
            public static int Roll_Legendary_Potencial = 4; //0 to 4
            //Weaver Will
            public static bool Enable_RollWeaverWill = true;
            public static int Roll_Weaver_Will = 28; //5 to 28

            public class Rarity
            {
                [HarmonyPatch(typeof(GenerateItems), "RollRarity")]
                public class Items
                {
                    [HarmonyPostfix]
                    static void Postfix(ref byte __result, int __0)
                    {
                        if (Enable_Rarity) { __result = GenerateItem_Rarity; }                        
                    }
                }
            }
            public class Implicits
            {
                [HarmonyPatch(typeof(GenerateItems), "RollAffixes")]
                public class Basic
                {
                    [HarmonyPostfix]
                    static void Postfix(GenerateItems __instance, ref int __result, ref ItemDataUnpacked __0, int __1, bool __2, bool __3, ref Il2CppSystem.Boolean __4)
                    {
                        if (Enable_RollImplicit)
                        {                            
                            for (int j = 0; j < __0.implicitRolls.Count; j++) //Work only for basic item
                            {
                                __0.implicitRolls[j] = Roll_Implicit;
                            }
                        }                        
                    }
                }

                [HarmonyPatch(typeof(ItemData), "rollLegendaryPotential")]
                public class Unique_LegendaryPotencial
                {
                    [HarmonyPostfix]
                    static void rollLegendaryPotential(ref ItemData __instance, ref int __result, ref UniqueList.Entry __0, ref int __1, ref int __2)
                    {
                        if (Enable_RollImplicit)
                        {
                            for (int i = 0; i < __instance.implicitRolls.Count; i++)
                            {
                                __instance.implicitRolls[i] = (byte)Roll_Implicit;
                            }
                        }                        
                    }
                }

                [HarmonyPatch(typeof(ItemData), "RollWeaversWill")]
                public class Unique_WeaverWill
                {
                    [HarmonyPostfix]
                    static void RollWeaversWill(ref ItemData __instance, ref int __result, ref UniqueList.Entry __0, ref int __1, ref int __2)
                    {
                        if (Enable_RollImplicit)
                        {
                            for (int i = 0; i < __instance.implicitRolls.Count; i++)
                            {
                                __instance.implicitRolls[i] = (byte)Roll_Implicit;
                            }
                        }                                              
                    }
                }

            }
            public class ForginPotencial
            {
                [HarmonyPatch(typeof(GenerateItems), "RollAffixes")]
                public class Basic
                {
                    [HarmonyPostfix]
                    static void Postfix(GenerateItems __instance, ref int __result, ref ItemDataUnpacked __0, int __1, bool __2, bool __3, ref Il2CppSystem.Boolean __4)
                    {
                        if (Enable_ForgingPotencial) { __0.forgingPotential = Roll_ForgingPotencial; }                        
                    }
                }
            }
            public class Affixes
            {
                [HarmonyPatch(typeof(GenerateItems), "RollAffixes")]
                public class Items
                {
                    [HarmonyPostfix]
                    static void Postfix(GenerateItems __instance, ref int __result, ref ItemDataUnpacked __0, int __1, bool __2, bool __3, ref Il2CppSystem.Boolean __4)
                    {
                        if ((Enable_AffixsTier) | (Enable_AffixsValue))
                        {
                            int tier_result = System.Convert.ToInt32(Roll_AffixTier) - 1;
                            for (int i = 0; i < __0.affixes.Count; i++)
                            {
                                if (Enable_AffixsTier) { __0.affixes[i].affixTier = (byte)tier_result; }
                                if (Enable_AffixsValue) { __0.affixes[i].affixRoll = Roll_AffixValue; }
                            }
                        }
                    }
                }
            }            
            public class UniqueMods
            {
                [HarmonyPatch(typeof(ItemData), "rollLegendaryPotential")]
                public class Unique_LegendaryPotencial
                {
                    [HarmonyPostfix]
                    static void rollLegendaryPotential(ref ItemData __instance, ref int __result, ref UniqueList.Entry __0, ref int __1, ref int __2)
                    {
                        if (Enable_UniqueMod)
                        {
                            for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                            {
                                __instance.uniqueRolls[k] = Roll_UniqueMod;
                            }
                        }
                    }
                }

                [HarmonyPatch(typeof(ItemData), "RollWeaversWill")]
                public class Unique_WeaverWill
                {
                    [HarmonyPostfix]
                    static void RollWeaversWill(ref ItemData __instance, ref int __result, ref UniqueList.Entry __0, ref int __1, ref int __2)
                    {
                        if (Enable_UniqueMod)
                        {
                            for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                            {
                                __instance.uniqueRolls[k] = Roll_UniqueMod;
                            }
                        }
                    }
                }
            }
            public class WeaverWill
            {
                #region RollValue
                [HarmonyPatch(typeof(ItemData), "RollWeaversWill")]
                public class ItemDataRoll
                {
                    [HarmonyPostfix]
                    static void RollWeaversWill(ref ItemData __instance, ref int __result, ref UniqueList.Entry __0, ref int __1, ref int __2)
                    {
                        if (Enable_RollWeaverWill) { __result = Roll_Weaver_Will; }
                    }
                }
                #endregion
            }
            public class LegendaryPotencial
            {
                #region RollValue
                [HarmonyPatch(typeof(ItemData), "rollLegendaryPotential")]
                public class ItemDataRoll
                {
                    [HarmonyPostfix]
                    static void rollLegendaryPotential(ref ItemData __instance, ref int __result, ref UniqueList.Entry __0, ref int __1, ref int __2)
                    {
                        if (Enable_RollLegendayPotencial) { __result = Roll_Legendary_Potencial; }
                    }
                }
                #endregion
            }
        }
        public class AutoLoot
        {
            public static bool AutoPickup_Key = true;
            public static bool AutoPickup_Craft = true;
            public static bool AutoPickup_UniqueAndSet = false;
            public static bool AutoStore_Materials = true;
            public static bool AutoPickup_Gold = true;
            public static bool AutoPickup_XpTome = true;
            public static bool AutoPickup_Pots = false;

            #region Items
            [HarmonyPatch(typeof(GroundItemManager), "dropItemForPlayer")]
            public class Items
            {
                [HarmonyPostfix]
                static void Postfix(ref GroundItemManager __instance, ref Actor __0, ref ItemData __1, ref UnityEngine.Vector3 __2, ref bool __3)
                {
                    if (__instance != null)
                    {
                        System.UInt32 item_id = __instance.nextItemId - 1;
                        bool pickup = false;
                        if ((AutoPickup_Key) && (Item.isKey(__1.itemType))) { pickup = true; }
                        else if ((AutoPickup_Craft) && (ItemList.isCraftingItem(__1.itemType))) { pickup = true; }
                        else if ((AutoPickup_UniqueAndSet) && (Item.rarityIsUniqueSetOrLegendary(__1.rarity))) { pickup = true; }
                        
                        if (pickup)
                        {
                            __instance.pickupItem(__0, item_id);
                            if ((AutoStore_Materials) && (ItemList.isCraftingItem(__1.itemType)))
                            {
                                InventoryPanelUI.instance.StoreMaterialsButtonPress();
                            }
                        }
                    }
                }
            }
            #endregion
            #region Gold
            [HarmonyPatch(typeof(GroundItemManager), "dropGoldForPlayer")]
            public class Gold
            {
                [HarmonyPostfix]
                static void Postfix(ref GroundItemManager __instance, ref Actor __0, ref int __1, ref UnityEngine.Vector3 __2, ref bool __3)
                {
                    if ((__instance != null) && (AutoPickup_Gold))
                    {
                        System.UInt32 gold_id = __instance.nextGoldId - 1;
                        foreach (GoldPickupInteraction gold_pickup_interaction in __instance.activeGoldPiles)
                        {
                            if (gold_pickup_interaction.id == gold_id)
                            {
                                __instance.pickupGold(__0, gold_id, gold_pickup_interaction);
                                break;
                            }
                        }
                    }
                }
            }
            #endregion            
            #region XpTome
            [HarmonyPatch(typeof(GroundItemManager), "dropXPTomeForPlayer")]
            public class XpTome
            {
                [HarmonyPostfix]
                static void Postfix3(ref GroundItemManager __instance, ref Actor __0, ref int __1, ref UnityEngine.Vector3 __2, ref bool __3)
                {
                    if ((__instance != null) && (AutoPickup_XpTome))
                    {
                        System.UInt32 tome_id = __instance.nextXpTomeId - 1;
                        foreach (PickupExperiencePotionInteraction pick_exp_pot_interaction in __instance.activeXPTomes)
                        {
                            if (pick_exp_pot_interaction.id == tome_id)
                            {
                                __instance.pickupXPTome(__0, tome_id, pick_exp_pot_interaction);
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            #region Pots
            [HarmonyPatch(typeof(GroundItemManager), "dropPotionForPlayer")]
            public class Potions
            {
                [HarmonyPostfix]
                static void Postfix(GroundItemManager __instance, Actor __0, UnityEngine.Vector3 __1, bool __2)
                {
                    if ((__instance != null) && (AutoPickup_Pots))
                    {
                        System.UInt32 pot_id = __instance.nextPotionId - 1;
                        foreach (PotionPickupInteraction pick_pot_interaction in __instance.activePotions)
                        {
                            if (pick_pot_interaction.id == pot_id)
                            {
                                __instance.pickupPotion(__0, pot_id, pick_pot_interaction);
                                break;
                            }
                        }
                    }
                }
            }
            #endregion            
        }
        public class RemoveReq
        {
            public static bool Remove_LevelReq = true;
            public static bool Remove_ClassReq = true;
            public static bool Remove_SubClassReq = true;

            #region RemoveLevelAndClass
            [HarmonyPatch(typeof(ItemData), "CalculateLevelAndClassRequirement")]
            public class AllItems
            {
                [HarmonyPostfix]
                static void CalculateLevelAndClassRequirement(ItemData __instance, ref int __result, ref ItemList.ClassRequirement __0, ref ItemList.SubClassRequirement __1)
                {
                    if (Remove_LevelReq) { __result = 0; }
                    if (Remove_ClassReq) { __0 = ItemList.ClassRequirement.None; }
                    if (Remove_SubClassReq) { __1 = ItemList.SubClassRequirement.None; }
                }
            }
            #endregion
        }
    }
}
