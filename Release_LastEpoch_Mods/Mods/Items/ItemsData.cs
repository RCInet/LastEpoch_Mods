using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class ItemsData
    {
        public static System.Collections.Generic.List<int> GetEligibles_Single_Affixes(ItemDataUnpacked item_data)
        {
            System.Collections.Generic.List<int> eligibles = new System.Collections.Generic.List<int>();
            foreach (AffixList.SingleAffix aff in AffixList.instance.singleAffixes)
            {
                int item_base = item_data.itemType;
                ItemList.ClassRequirement item_class_req = item_data.classReq;
                if (AffixList.instance.CheckAffixWithItemType(item_base, item_class_req, aff.affixId))
                {
                    eligibles.Add(aff.affixId);
                }
            }                        

            return eligibles;
        }
        public static System.Collections.Generic.List<int> GetEligibles_Multi_Affixes(ItemDataUnpacked item_data)
        {
            System.Collections.Generic.List<int> eligibles = new System.Collections.Generic.List<int>();
            foreach (AffixList.MultiAffix aff in AffixList.instance.multiAffixes)
            {
                int item_base = item_data.itemType;
                ItemList.ClassRequirement item_class_req = item_data.classReq;
                if (AffixList.instance.CheckAffixWithItemType(item_base, item_class_req, aff.affixId))
                {
                    eligibles.Add(aff.affixId);
                }
            }

            return eligibles;
        }
        public static ItemAffix RandomAffix(System.Collections.Generic.List<int> eligibles_single, System.Collections.Generic.List<int> eligibles_multi, bool seal)
        {
            int random_affix_type = UnityEngine.Random.RandomRangeInt(0, 2); //Single or Multi
            int affix_id = UnityEngine.Random.RandomRangeInt(0, 150);
            if (random_affix_type == 0)
            {
                if (eligibles_single.Count > 0)
                {
                    int temp = UnityEngine.Random.RandomRangeInt(0, eligibles_single.Count);
                    if (temp < eligibles_single.Count) { affix_id = eligibles_single[temp]; }
                }
            }
            else
            {
                if (eligibles_multi.Count > 0)
                {
                    int temp = UnityEngine.Random.RandomRangeInt(0, eligibles_multi.Count);
                    if (temp < eligibles_multi.Count) { affix_id = eligibles_multi[temp]; }
                }
            }

            ItemAffix affix = new ItemAffix
            {
                affixName = "",
                affixId = (byte)affix_id,
                affixTier = (byte)UnityEngine.Random.RandomRangeInt(0, 8),
                affixRoll = (byte)UnityEngine.Random.Range(0f, 255f),
                affixType = 0,
                affixCountForSetProps = 0,
                affixTitle = "",
                isSealedAffix = seal,
                specialAffixType = 0,
                titleType = 0
            };

            return affix;
        }
        
        public static System.Collections.Generic.List<ushort> GetEligibles_Unique(ItemDataUnpacked item_data_unpacked)
        {
            System.Collections.Generic.List<ushort> eligibles = new System.Collections.Generic.List<ushort>();
            ItemData item_data = item_data_unpacked.TryCast<ItemData>();
            foreach (UniqueList.Entry unique in UniqueList.instance.uniques)
            {
                if ((unique.baseType == item_data.itemType) && (unique.subTypes[0] == item_data.subType))
                {
                    eligibles.Add(unique.uniqueID);
                }
            }

            return eligibles;
        }

        [HarmonyPatch(typeof(ItemList), "GetItemMaximumAffixes")]
        public class ItemList_GetItemMaximumAffixes
        {
            [HarmonyPostfix]
            static void Postfix(ItemList __instance, ref int __result, ref int __0)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (!ForceDrop.ForceDrop.drop.generating_item)
                    {
                        if (__0 < 25)
                        {
                            int max = 4;
                            if (Save_Manager.Data.UserData.Items.ItemData.Min_affixs > max) { max = Save_Manager.Data.UserData.Items.ItemData.Min_affixs; }
                            __result = max;
                        }
                        else if ((__0 < 34) && (__result > 2))
                        {
                            __result = 2;
                        }                        
                    }
                }
            }
        } 
            
        [HarmonyPatch(typeof(GenerateItems), "RollRarity")]
        public class GenerateItems_RollRarity
        {
            [HarmonyPrefix]
            static bool Prefix(ref byte __result, ref int __0)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (!ForceDrop.ForceDrop.drop.generating_item)
                    {
                        //Main.logger_instance.Msg("GenerateItems:RollRarity");
                        if (Save_Manager.Data.UserData.Items.ItemData.Enable_Rarity)
                        {
                            byte result = Save_Manager.Data.UserData.Items.ItemData.Roll_Rarity;
                            if (result < 0) { result = 0; }
                            //else if (result == 9) { result = 7; }
                            else if ((result > 4) && (result < 7)) { result = 4; }
                            __result = result;
                            return false;
                        }
                        else { return true; }
                    }
                    else { return true; }
                }
                else { return true; }
            }
        }

        //Seal and Affix (Tier and Value)
        [HarmonyPatch(typeof(GenerateItems), "DropItemAtPoint")]
        public class GenerateItems_DropItemAtPoint
        {
            [HarmonyPrefix]
            static void Prefix(GenerateItems __instance, ref ItemDataUnpacked __0, UnityEngine.Vector3 __1, int __2)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (!ForceDrop.ForceDrop.drop.generating_item)
                    {
                        ItemData item_data = __0.TryCast<ItemData>();

                        //Item.(item_data.itemType);

                        int item_type = System.Convert.ToInt32(item_data.itemType);
                        //Main.logger_instance.Msg("item_type = " + item_type + " : " + __0.itemType);
                        if ((!item_data.isUnique()) && (!item_data.isSet()) && (item_data.itemType < 34))
                        {
                            bool Idol = false;
                            if ((item_type > 24) && (item_type < 34)) { Idol = true; }                            
                            int nb_seals = 0;
                            int nb_affixs = 0;
                            foreach (ItemAffix aff in __0.affixes)
                            {
                                if (!aff.isSealedAffix) { nb_affixs++; }
                                else { nb_seals++; }
                            }
                            if (Save_Manager.Data.UserData.Items.ItemData.Force_Seal)
                            {
                                int seal_tier = UnityEngine.Random.RandomRangeInt(0, 7);
                                __0.AddRandomSealedAffix(seal_tier);
                            }
                            //int nb_affixes_wanted = Save_Manager.Data.UserData.Items.ItemData.Min_affixs;
                            int nb_affixes_wanted = UnityEngine.Random.RandomRangeInt(Save_Manager.Data.UserData.Items.ItemData.Min_affixs, Save_Manager.Data.UserData.Items.ItemData.Max_affixs + 1);

                            if ((Idol) && (nb_affixs > 2)) { nb_affixes_wanted = 2; }
                            System.Collections.Generic.List<int> eligibles_single_affixes = new System.Collections.Generic.List<int>();
                            System.Collections.Generic.List<int> eligibles_multi_affixes = new System.Collections.Generic.List<int>();
                            if (nb_affixs < nb_affixes_wanted)
                            {
                                eligibles_single_affixes = GetEligibles_Single_Affixes(__0);
                                eligibles_multi_affixes = GetEligibles_Multi_Affixes(__0);
                            }
                            for (int i = nb_affixs; i < nb_affixes_wanted; i++)
                            {
                                __0.affixes.Add(RandomAffix(eligibles_single_affixes, eligibles_multi_affixes, false));
                                __0.RefreshIDAndValues();
                            }

                            nb_seals = 0;
                            nb_affixs = 0;
                            foreach (ItemAffix aff in __0.affixes)
                            {
                                if (!aff.isSealedAffix) { nb_affixs++; }
                                else { nb_seals++; }
                            }
                            //Main.logger_instance.Msg("Contain : " + nb_seals + " Seal and " + nb_affixs + " Affixes");
                              
                            foreach (ItemAffix aff in __0.affixes)
                            {
                                if (!aff.isSealedAffix)
                                {
                                    if ((!Idol) && (Save_Manager.Data.UserData.Items.ItemData.Enable_AffixsTier))
                                    {
                                        aff.affixTier = (byte)(Save_Manager.Data.UserData.Items.ItemData.Roll_AffixTier - 1);
                                    }
                                    if (Save_Manager.Data.UserData.Items.ItemData.Enable_AffixsValue)
                                    {
                                        aff.affixRoll = Save_Manager.Data.UserData.Items.ItemData.Roll_AffixValue;
                                    }
                                }
                                else
                                {
                                    if ((!Idol) && (Save_Manager.Data.UserData.Items.ItemData.Enable_SealTier))
                                    {
                                        aff.affixTier = (byte)(Save_Manager.Data.UserData.Items.ItemData.Roll_SealTier - 1);
                                    }
                                    if (Save_Manager.Data.UserData.Items.ItemData.Enable_SealValue)
                                    {
                                        aff.affixRoll = Save_Manager.Data.UserData.Items.ItemData.Roll_SealValue;
                                    }
                                }
                            }                                                       
                            if ((__0.rarity < nb_affixs) && (!item_data.isUniqueSetOrLegendary()))
                            {
                                byte item_rarity = 0;
                                if (nb_affixs > 4) { item_rarity = 4; }
                                else { item_rarity = (byte)nb_affixs; }
                                __0.rarity = item_rarity;
                            }
                            if (__0.sockets < nb_affixs) { __0.sockets = (byte)nb_affixs; }                            
                            if (item_data.isUniqueSetOrLegendary()) //Legendary
                            {
                                System.Collections.Generic.List<ushort> unique_ids = GetEligibles_Unique(__0);
                                if (unique_ids.Count > 0)
                                {
                                    int index = UnityEngine.Random.RandomRangeInt(0, unique_ids.Count);
                                    __0.uniqueID = unique_ids[index];
                                    UniqueList.Entry unique = UniqueList.getUnique(__0.uniqueID);
                                    if (unique != null)
                                    {
                                        //character level
                                        float corruption = UnityEngine.Random.Range(0f, 255f);
                                        if (unique.legendaryType == UniqueList.LegendaryType.LegendaryPotential)
                                        {
                                            int min = UnityEngine.Random.RandomRangeInt(0, 5);                                            
                                            float multiplier = UnityEngine.Random.Range(0f, 255f);
                                            bool out_b = false;
                                            __0.rollLegendaryPotential(unique, min, 100, corruption, multiplier, out out_b);
                                        }
                                        else
                                        {
                                            int min = UnityEngine.Random.RandomRangeInt(0, 29);
                                            __0.RollWeaversWill(unique, min, 100, corruption);
                                        }
                                    }
                                }
                                else //No Unique found = Set rarity to base item
                                {
                                    byte item_rarity = 0;
                                    if (nb_affixs > 4) { item_rarity = 4; }
                                    else { item_rarity = (byte)nb_affixs; }
                                    __0.rarity = item_rarity;
                                }
                            }
                            __0.RefreshIDAndValues();
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(ItemData), "randomiseImplicitRolls")]
        public class ItemData_randomiseImplicitRolls
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemData __instance)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (!ForceDrop.ForceDrop.drop.generating_item)
                    {
                        //Main.logger_instance.Msg("ItemData:randomiseImplicitRolls");
                        if (Save_Manager.Data.UserData.Items.ItemData.Enable_Implicit)
                        {
                            for (int z = 0; z < __instance.implicitRolls.Count; z++)
                            {
                                __instance.implicitRolls[z] = (byte)Save_Manager.Data.UserData.Items.ItemData.Roll_Implicit;
                            }
                        }
                        __instance.RefreshIDAndValues();
                    }                    
                }
            }
        }

        [HarmonyPatch(typeof(GenerateItems), "rollForgingPotential")]
        public class GenerateItems_rollForgingPotential
        {
            [HarmonyPostfix]
            static void Postfix(ref int __result, ref ItemDataUnpacked __0, int __1, bool __2, GenerateItems.VendorType __3)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (!ForceDrop.ForceDrop.drop.generating_item)
                    {
                        //Main.logger_instance.Msg("ItemData:rollForgingPotential");
                        if (Save_Manager.Data.UserData.Items.ItemData.Enable_ForgingPotencial)
                        {
                            __0.forgingPotential = (byte)Save_Manager.Data.UserData.Items.ItemData.Roll_ForgingPotencial;
                            __0.RefreshIDAndValues();
                            __result = __0.forgingPotential;
                        }
                    }                    
                }
            }
        }

        [HarmonyPatch(typeof(ItemData), "AddRandomSealedAffix")]
        public class AddRandomSealedAffix
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemData __instance, int __0)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (!ForceDrop.ForceDrop.drop.generating_item)
                    {
                        //Main.logger_instance.Msg("ItemData:AddRandomSealedAffix : Tier = " + __0);
                    }                    
                }
            }
        }

        [HarmonyPatch(typeof(ItemData), "randomiseUniqueRolls")]
        public class randomiseUniqueRolls
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemData __instance)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (!ForceDrop.ForceDrop.drop.generating_item)
                    {
                        //Main.logger_instance.Msg("ItemData:randomiseUniqueRolls");
                        if (Save_Manager.Data.UserData.Items.ItemData.Enable_UniqueMod)
                        {
                            for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                            {
                                __instance.uniqueRolls[k] = Save_Manager.Data.UserData.Items.ItemData.Roll_UniqueMod;
                            }
                            __instance.RefreshIDAndValues();
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ItemData), "rollLegendaryPotential")]
        public class rollLegendaryPotential
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemData __instance, ref int __result, UniqueList.Entry __0, int __1, int __2)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (!ForceDrop.ForceDrop.drop.generating_item)
                    {
                        //Main.logger_instance.Msg("ItemData:rollLegendaryPotential");
                        if (Save_Manager.Data.UserData.Items.ItemData.Enable_LegendayPotencial)
                        {
                            __instance.legendaryPotential = (byte)Save_Manager.Data.UserData.Items.ItemData.Roll_Legendary_Potencial;
                            __instance.RefreshIDAndValues();
                            __result = __instance.legendaryPotential;
                        }
                    }                    
                }
            }
        }

        [HarmonyPatch(typeof(ItemData), "RollWeaversWill")]
        public class RollWeaversWill
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemData __instance, ref int __result, UniqueList.Entry __0, int __1, int __2)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (!ForceDrop.ForceDrop.drop.generating_item)
                    {
                        //Main.logger_instance.Msg("ItemData:RollWeaversWill");
                        if (Save_Manager.Data.UserData.Items.ItemData.Enable_WeaverWill)
                        {
                            __instance.weaversWill = (byte)Save_Manager.Data.UserData.Items.ItemData.Roll_Weaver_Will;
                            __instance.RefreshIDAndValues();
                            __result = __instance.weaversWill;
                        }
                    }
                }
            }
        }

        //Craft Implicits
        [HarmonyPatch(typeof(ItemData), "ReRollImplicitRolls", new System.Type[] { })]
        public class ItemData_ReRollImplicitRolls
        {
            [HarmonyPrefix]
            static bool Prefix(ref ItemData __instance)
            {
                if (Scenes_Manager.GameScene())
                {
                    //Main.logger_instance.Msg("ItemData:ReRollImplicitRolls");
                    if (Save_Manager.Data.UserData.Items.ItemData.Enable_Implicit)
                    {
                        if (Save_Manager.Data.UserData.Items.ItemData.Enable_Implicit)
                        {
                            for (int z = 0; z < __instance.implicitRolls.Count; z++)
                            {
                                __instance.implicitRolls[z] = (byte)Save_Manager.Data.UserData.Items.ItemData.Roll_Implicit;
                            }
                        }
                        __instance.RefreshIDAndValues();
                        return false;
                    }
                    else { return true; }
                }
                else { return true; }
            }
        }

        //Craft Affixs
        [HarmonyPatch(typeof(ItemData), "ReRollAffixRolls")]
        public class ReRollAffixRolls
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemData __instance, bool __0)
            {
                if (Scenes_Manager.GameScene())
                {
                    //Main.logger_instance.Msg("ItemData:ReRollAffixRolls");
                    foreach (ItemAffix aff in __instance.affixes)
                    {
                        if (Save_Manager.Data.UserData.Items.ItemData.Enable_AffixsValue)
                        {
                            aff.affixRoll = Save_Manager.Data.UserData.Items.ItemData.Roll_AffixValue;
                        }
                    }
                    __instance.RefreshIDAndValues();
                }
            }
        }
    }
}
