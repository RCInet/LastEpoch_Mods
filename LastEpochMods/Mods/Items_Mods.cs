using HarmonyLib;

namespace LastEpochMods.Mods
{
    public class Items_Mods
    {
        public class Drop_Mods
        {
            public class Drop_Bonuses
            {
                [HarmonyPatch(typeof(ItemDropBonuses), "getIncreasedRarityDropRate")]
                public class Ability_Cooldown_Prefix
                {
                    [HarmonyPrefix]
                    static bool Prefix(ref ItemDropBonuses __instance, float __result, float __0)
                    {
                        if (Config.Data.mods_config.items.Enable_increase_equipment_droprate)
                        {
                            for (int i = 0; i < __instance.increasedEquipmentDroprates.Count; i++)
                            {
                                __instance.increasedEquipmentDroprates[i] = Config.Data.mods_config.items.increase_equipment_droprate;
                            }
                        }
                        if (Config.Data.mods_config.items.Enable_increase_equipmentshards_droprate)
                        {
                            for (int z = 0; z < __instance.increasedEquipmentShardDroprates.Count; z++)
                            {
                                __instance.increasedEquipmentShardDroprates[z] = Config.Data.mods_config.items.increase_equipmentshards_droprate;
                            }
                        }
                        if (Config.Data.mods_config.items.Enable_increase_uniques_droprate)
                        {
                            __instance.increasedUniqueDropRate = Config.Data.mods_config.items.increase_uniques_droprate;
                        }

                        return true;
                    }
                }
            }
            public class GoldMultiplier
            {
                [HarmonyPatch(typeof(DeathItemDrop), "Start")]
                public class Items
                {
                    [HarmonyPostfix]
                    static void Postfix(ref DeathItemDrop __instance)
                    {
                        if (Config.Data.mods_config.items.Enable_DeathItemDrop_goldMultiplier)
                        {
                            __instance.overrideBaseGoldDropChance = true;
                            __instance.goldDropChance = 1; //100%
                            __instance.goldMultiplier = Config.Data.mods_config.items.DeathItemDrop_goldMultiplier;
                        }
                    }
                }
            }
            public class ItemMultiplier
            {
                [HarmonyPatch(typeof(DeathItemDrop), "Start")]
                public class Items
                {
                    [HarmonyPostfix]
                    static void Postfix(ref DeathItemDrop __instance)
                    {
                        if (Config.Data.mods_config.items.Enable_DeathItemDrop_ItemMultiplier)
                        {
                            __instance.overrideBaseItemDropChance = true;
                            __instance.itemDropChance = 1; //100%
                            __instance.itemMultiplier = Config.Data.mods_config.items.DeathItemDrop_ItemMultiplier;
                        }                        
                    }
                }
            }
            public class ExperienceMultiplier
            {
                [HarmonyPatch(typeof(DeathItemDrop), "Start")]
                public class Items
                {
                    [HarmonyPostfix]
                    static void Postfix(ref DeathItemDrop __instance)
                    {
                        if (Config.Data.mods_config.items.Enable_DeathItemDrop_Experience)
                        {
                            __instance.experience = Config.Data.mods_config.items.DeathItemDrop_Experience;
                        }
                    }
                }
            }
            public class AdditionalRare
            {
                [HarmonyPatch(typeof(DeathItemDrop), "Start")]
                public class Items
                {
                    [HarmonyPostfix]
                    static void Postfix(ref DeathItemDrop __instance)
                    {
                        if (Config.Data.mods_config.items.Enable_DeathItemDrop_AdditionalRare)
                        {
                            __instance.guaranteedAdditionalRare = Config.Data.mods_config.items.DeathItemDrop_AdditionalRare;
                        }                        
                    }
                }
            }
            public class Rarity
            {
                [HarmonyPatch(typeof(GenerateItems), "RollRarity")]
                public class RollRarity
                {
                    [HarmonyPostfix]
                    static void Postfix(ref byte __result, int __0)
                    {
                        if (Config.Data.mods_config.items.Enable_Rarity) { __result = Config.Data.mods_config.items.GenerateItem_Rarity; }                        
                    }
                }

                [HarmonyPatch(typeof(GenerateItems), "initialiseRandomItemData")]
                public class initialiseRandomItemData
                {
                    [HarmonyPostfix]
                    static void Postfix(GenerateItems __instance, ref ItemDataUnpacked __result, bool __0, int __1, bool __2, ItemLocationTag __3, int __4, int __5, int __6, int __7, int __8, bool __9, int __10)
                    {
                        if (Config.Data.mods_config.items.Enable_Rarity) { __result.rarity = Config.Data.mods_config.items.GenerateItem_Rarity; }
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
                        if (Config.Data.mods_config.items.Enable_RollImplicit)
                        {                            
                            for (int j = 0; j < __0.implicitRolls.Count; j++) //Work only for basic item
                            {
                                __0.implicitRolls[j] = Config.Data.mods_config.items.Roll_Implicit;
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
                        if (Config.Data.mods_config.items.Enable_RollImplicit)
                        {
                            for (int i = 0; i < __instance.implicitRolls.Count; i++)
                            {
                                __instance.implicitRolls[i] = (byte)Config.Data.mods_config.items.Roll_Implicit;
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
                        if (Config.Data.mods_config.items.Enable_RollImplicit)
                        {
                            for (int i = 0; i < __instance.implicitRolls.Count; i++)
                            {
                                __instance.implicitRolls[i] = (byte)Config.Data.mods_config.items.Roll_Implicit;
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
                        if (Config.Data.mods_config.items.Enable_ForgingPotencial) { __0.forgingPotential = Config.Data.mods_config.items.Roll_ForgingPotencial; }                        
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
                        if ((Config.Data.mods_config.items.Enable_AffixsTier) | (Config.Data.mods_config.items.Enable_AffixsValue))
                        {
                            int tier_result = System.Convert.ToInt32(Config.Data.mods_config.items.Roll_AffixTier) - 1;
                            for (int i = 0; i < __0.affixes.Count; i++)
                            {
                                if (Config.Data.mods_config.items.Enable_AffixsTier) { __0.affixes[i].affixTier = (byte)tier_result; }
                                if (Config.Data.mods_config.items.Enable_AffixsValue) { __0.affixes[i].affixRoll = Config.Data.mods_config.items.Roll_AffixValue; }
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
                        if (Config.Data.mods_config.items.Enable_UniqueMod)
                        {
                            for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                            {
                                __instance.uniqueRolls[k] = Config.Data.mods_config.items.Roll_UniqueMod;
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
                        if (Config.Data.mods_config.items.Enable_UniqueMod)
                        {
                            for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                            {
                                __instance.uniqueRolls[k] = Config.Data.mods_config.items.Roll_UniqueMod;
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
                        if (Config.Data.mods_config.items.Enable_RollWeaverWill) { __result = Config.Data.mods_config.items.Roll_Weaver_Will; }
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
                        if (Config.Data.mods_config.items.Enable_RollLegendayPotencial) { __result = Config.Data.mods_config.items.Roll_Legendary_Potencial; }
                    }
                }
                #endregion
            }
        }
        public class AutoLoot
        {
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
                        if ((Config.Data.mods_config.auto_loot.AutoPickup_Key) && (Item.isKey(__1.itemType))) { pickup = true; }
                        else if ((Config.Data.mods_config.auto_loot.AutoPickup_Craft) && (ItemList.isCraftingItem(__1.itemType))) { pickup = true; }
                        else if ((Config.Data.mods_config.auto_loot.AutoPickup_UniqueAndSet) && (Item.rarityIsUniqueSetOrLegendary(__1.rarity))) { pickup = true; }
                        
                        if (pickup)
                        {
                            __instance.pickupItem(__0, item_id);
                            if ((Config.Data.mods_config.auto_loot.AutoStore_Materials) && (ItemList.isCraftingItem(__1.itemType)))
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
                    if ((__instance != null) && (Config.Data.mods_config.auto_loot.AutoPickup_Gold))
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
                    if ((__instance != null) && (Config.Data.mods_config.auto_loot.AutoPickup_XpTome))
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
                    if ((__instance != null) && (Config.Data.mods_config.auto_loot.AutoPickup_Pots))
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
            #region RemoveLevelAndClass
            [HarmonyPatch(typeof(ItemData), "CalculateLevelAndClassRequirement")]
            public class AllItems
            {
                [HarmonyPostfix]
                static void CalculateLevelAndClassRequirement(ItemData __instance, ref int __result, ref ItemList.ClassRequirement __0, ref ItemList.SubClassRequirement __1)
                {
                    if (Config.Data.mods_config.items.Remove_LevelReq) { __result = 0; }
                    if (Config.Data.mods_config.items.Remove_ClassReq) { __0 = ItemList.ClassRequirement.None; }
                    if (Config.Data.mods_config.items.Remove_SubClassReq) { __1 = ItemList.SubClassRequirement.None; }
                }
            }
            #endregion
        }
    }
}
