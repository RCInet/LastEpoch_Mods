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
            [HarmonyPrefix]
            static bool Prefix(ItemList __instance, ref int __result, ref int __0)
            {
                if ((Scenes_Manager.GameScene()) && (!ForceDrop.ForceDrop.drop.generating_item) &&
                    (__0 < 25) && (Save_Manager.Data.UserData.Items.ItemData.Min_affixs > 4))
                {
                    __result = Save_Manager.Data.UserData.Items.ItemData.Min_affixs;
                    return false;
                }
                else { return true; }
            }
        } 
        
        //Need to be replace by ForceUnique ForceSet and ForceLgendary
        [HarmonyPatch(typeof(GenerateItems), "RollRarity")]
        public class GenerateItems_RollRarity
        {
            [HarmonyPrefix]
            static bool Prefix(ref byte __result, int __0)
            {
                if ((Scenes_Manager.GameScene()) && (!ForceDrop.ForceDrop.drop.generating_item))
                {
                    if (Save_Manager.Data.UserData.Items.ItemData.ForceUnique) { __result = 7; return false; }
                    else if (Save_Manager.Data.UserData.Items.ItemData.ForceSet) { __result = 8; return false; }
                    else if (Save_Manager.Data.UserData.Items.ItemData.ForceLegendary) { __result = 9; return false; }
                    else { return true; }
                }
                else { return true; }
            }
        }

        //Seal and Affix (Tier and Value)
        public static System.Collections.Generic.List<int> eligibles_single_affixes = new System.Collections.Generic.List<int>();
        public static System.Collections.Generic.List<int> eligibles_multi_affixes = new System.Collections.Generic.List<int>();
        public static int min_tier = 1;
        public static int max_tier = 8;
        [HarmonyPatch(typeof(GenerateItems), "DropItemAtPoint")]
        public class GenerateItems_DropItemAtPoint
        {
            [HarmonyPrefix]
            static void Prefix(GenerateItems __instance, ref ItemDataUnpacked __0, ref UnityEngine.Vector3 __1, int __2)
            {
                if ((Scenes_Manager.GameScene()) && (!ForceDrop.ForceDrop.drop.generating_item) &&
                    (__0.itemType < 100))
                {
                    if (__0.itemType < 25)
                    {
                        if ((!__0.isUnique()) && (!__0.isSet())) //Base item and Legendary
                        {
                            int nb_affixs = 0;
                            if ((Save_Manager.Data.UserData.Items.ItemData.Force_Seal) ||
                                (Save_Manager.Data.UserData.Items.ItemData.Max_affixs > 4))
                            {
                                int nb_seals = 0;
                                foreach (ItemAffix aff in __0.affixes)
                                {
                                    if (!aff.isSealedAffix) { nb_affixs++; }
                                    else { nb_seals++; }
                                }
                                if ((Save_Manager.Data.UserData.Items.ItemData.Force_Seal) && (nb_seals == 0))
                                {
                                    __0.AddRandomSealedAffix(UnityEngine.Random.Range(min_tier, max_tier));
                                }
                                int nb_affixes_wanted = UnityEngine.Random.RandomRangeInt(Save_Manager.Data.UserData.Items.ItemData.Min_affixs, Save_Manager.Data.UserData.Items.ItemData.Max_affixs + 1);
                                if (nb_affixs < nb_affixes_wanted)
                                {
                                    eligibles_single_affixes = GetEligibles_Single_Affixes(__0);
                                    eligibles_multi_affixes = GetEligibles_Multi_Affixes(__0);
                                    for (int i = nb_affixs; i < nb_affixes_wanted; i++)
                                    {
                                        __0.affixes.Add(RandomAffix(eligibles_single_affixes, eligibles_multi_affixes, false));
                                        __0.RefreshIDAndValues();
                                    }
                                }
                            }
                            if (Save_Manager.Data.UserData.Items.ItemData.Enable_AffixsTier)
                            {
                                foreach (ItemAffix aff in __0.affixes)
                                {
                                    if (!aff.isSealedAffix)
                                    {
                                        aff.affixTier = (byte)(Save_Manager.Data.UserData.Items.ItemData.Roll_AffixTier - 1);
                                    }
                                    else { aff.affixTier = (byte)(Save_Manager.Data.UserData.Items.ItemData.Roll_SealTier - 1); }
                                }
                            }
                            if (Save_Manager.Data.UserData.Items.ItemData.Enable_AffixsValue)
                            {
                                foreach (ItemAffix aff in __0.affixes)
                                {
                                    if (!aff.isSealedAffix)
                                    {
                                        aff.affixRoll = Save_Manager.Data.UserData.Items.ItemData.Roll_AffixValue;
                                    }
                                    else { aff.affixRoll = Save_Manager.Data.UserData.Items.ItemData.Roll_SealValue; }
                                }
                            }
                            if (__0.isUniqueSetOrLegendary()) //Legendary
                            {
                                //legendary = true;
                                System.Collections.Generic.List<ushort> unique_ids = GetEligibles_Unique(__0);
                                if (unique_ids.Count > 0)
                                {
                                    int index = UnityEngine.Random.RandomRangeInt(0, unique_ids.Count);
                                    __0.uniqueID = unique_ids[index];
                                    UniqueList.Entry unique = UniqueList.getUnique(__0.uniqueID);
                                    if (unique != null)
                                    {
                                        //float character level = 
                                        float corruption = UnityEngine.Random.Range(0f, 255f);
                                        if (unique.legendaryType == UniqueList.LegendaryType.LegendaryPotential)
                                        {
                                            int min = UnityEngine.Random.Range(0, 5);
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
                                    byte item_rarity = (byte)nb_affixs;
                                    if (nb_affixs > 4) { item_rarity = 4; }
                                    __0.rarity = item_rarity;
                                }
                            }
                            __0.RefreshIDAndValues();
                            //if (legendary) { UniqueList.} //repair unique
                        }                        
                    }
                    else if ((__0.itemType > 24) && (__0.itemType < 34)) //Idols
                    {
                        if (Save_Manager.Data.UserData.Items.ItemData.Enable_AffixsValue)
                        {
                            foreach (ItemAffix aff in __0.affixes)
                            {
                                aff.affixRoll = Save_Manager.Data.UserData.Items.ItemData.Roll_AffixValue;
                            }
                            __0.RefreshIDAndValues();
                        }
                    }
                    else //New Items (have to check this)
                    {
                        if (Save_Manager.Data.UserData.Items.ItemData.Enable_AffixsValue)
                        {
                            foreach (ItemAffix aff in __0.affixes)
                            {
                                aff.affixRoll = Save_Manager.Data.UserData.Items.ItemData.Roll_AffixValue;
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
            [HarmonyPrefix]
            static bool Prefix(ref ItemData __instance)
            {
                if ((Scenes_Manager.GameScene()) && (!ForceDrop.ForceDrop.drop.generating_item) &&
                    (Save_Manager.Data.UserData.Items.ItemData.Enable_Implicit))
                {
                    for (int z = 0; z < __instance.implicitRolls.Count; z++)
                    {
                        __instance.implicitRolls[z] = Save_Manager.Data.UserData.Items.ItemData.Roll_Implicit;
                    }
                    __instance.RefreshIDAndValues();
                    return false;
                }
                else {  return true; }
            }
        }

        [HarmonyPatch(typeof(GenerateItems), "rollForgingPotential")]
        public class GenerateItems_rollForgingPotential
        {
            [HarmonyPrefix]
            static bool Prefix(ref int __result, ref ItemDataUnpacked __0, int __1, bool __2, GenerateItems.VendorType __3)
            {
                if ((Scenes_Manager.GameScene()) && (!ForceDrop.ForceDrop.drop.generating_item) &&
                    (Save_Manager.Data.UserData.Items.ItemData.Enable_ForgingPotencial))
                {
                    __result = Save_Manager.Data.UserData.Items.ItemData.Roll_ForgingPotencial;
                    return false;
                }
                else { return true; }
            }
        }
          
        [HarmonyPatch(typeof(ItemData), "randomiseUniqueRolls")]
        public class randomiseUniqueRolls
        {
            [HarmonyPrefix]
            static bool Prefix(ref ItemData __instance)
            {
                if ((Scenes_Manager.GameScene()) && (!ForceDrop.ForceDrop.drop.generating_item) &&
                    (Save_Manager.Data.UserData.Items.ItemData.Enable_UniqueMod))
                {
                    for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                    {
                        __instance.uniqueRolls[k] = Save_Manager.Data.UserData.Items.ItemData.Roll_UniqueMod;
                    }
                    __instance.RefreshIDAndValues();
                    return false;
                }
                else { return true; };
            }
        }

        [HarmonyPatch(typeof(ItemData), "rollLegendaryPotential")]
        public class rollLegendaryPotential
        {
            [HarmonyPrefix]
            static bool Prefix(ref ItemData __instance, ref int __result, UniqueList.Entry __0, int __1, int __2)
            {
                if ((Scenes_Manager.GameScene()) && (!ForceDrop.ForceDrop.drop.generating_item) &&
                    (Save_Manager.Data.UserData.Items.ItemData.Enable_LegendayPotencial))
                {
                    __result = Save_Manager.Data.UserData.Items.ItemData.Roll_Legendary_Potencial;
                    return false;
                }
                else { return true; };
            }
        }

        [HarmonyPatch(typeof(ItemData), "RollWeaversWill")]
        public class RollWeaversWill
        {
            [HarmonyPrefix]
            static bool Prefix(ref ItemData __instance, ref int __result, UniqueList.Entry __0, int __1, int __2)
            {
                if (( Scenes_Manager.GameScene()) && (!ForceDrop.ForceDrop.drop.generating_item) &&
                    (Save_Manager.Data.UserData.Items.ItemData.Enable_WeaverWill))
                {
                    __result = Save_Manager.Data.UserData.Items.ItemData.Roll_Weaver_Will;
                    return false;
                }
                else { return true; }
            }
        }

        //Craft Implicits
        [HarmonyPatch(typeof(ItemData), "ReRollImplicitRolls", new System.Type[] { })]
        public class ItemData_ReRollImplicitRolls
        {
            [HarmonyPrefix]
            static bool Prefix(ref ItemData __instance)
            {
                if ((Scenes_Manager.GameScene()) && (!ForceDrop.ForceDrop.drop.generating_item) &&
                    (Save_Manager.Data.UserData.Items.ItemData.Enable_Implicit))
                {
                    for (int z = 0; z < __instance.implicitRolls.Count; z++)
                    {
                        __instance.implicitRolls[z] = Save_Manager.Data.UserData.Items.ItemData.Roll_Implicit;
                    }
                    __instance.RefreshIDAndValues();
                    return false;
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
                if ((Scenes_Manager.GameScene()) && (!ForceDrop.ForceDrop.drop.generating_item) &&
                    (Save_Manager.Data.UserData.Items.ItemData.Enable_AffixsValue))
                {
                    foreach (ItemAffix aff in __instance.affixes)
                    {
                        aff.affixRoll = Save_Manager.Data.UserData.Items.ItemData.Roll_AffixValue;
                    }
                    __instance.RefreshIDAndValues();
                }
            }
        }
    }
}
