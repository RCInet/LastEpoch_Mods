using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Drop_Affixs
    {
        private static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if ((!Save_Manager.instance.data.IsNullOrDestroyed()) &&
                    ((Save_Manager.instance.data.Items.Drop.Enable_ForceUnique) ||
                    (Save_Manager.instance.data.Items.Drop.Enable_ForceSet) ||
                    (Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary) ||
                    (Save_Manager.instance.data.Items.Drop.AffixCount_Min != 0) ||
                    (Save_Manager.instance.data.Items.Drop.AffixCount_Max != 4) ||
                    (Save_Manager.instance.data.Items.Drop.Enable_AffixTiers) ||
                    (Save_Manager.instance.data.Items.Drop.Enable_AffixValues) ||
                    (Save_Manager.instance.data.Items.Drop.Enable_ForceSeal) ||
                    (Save_Manager.instance.data.Items.Drop.Enable_SealTier) ||
                    (Save_Manager.instance.data.Items.Drop.Enable_SealValue)))
                {
                    return true;
                }
                else { return false; }
            }
            else { return false; }
        }
        
        [HarmonyPatch(typeof(GenerateItems), "DropItemAtPoint")]
        public class GenerateItems_DropItemAtPoint
        {
            [HarmonyPrefix]
            static void Prefix(GenerateItems __instance, ref ItemDataUnpacked __0, ref UnityEngine.Vector3 __1, int __2)
            {
                if ((CanRun()) && (__0.itemType < 100))
                {
                    if (__0.itemType < 25)
                    {
                        if ((!__0.isUnique()) && (!__0.isSet())) //Base item and Legendary
                        {
                            /*int nb_affixs = 0;
                            if ((Save_Manager.instance.data.Items.Drop.Enable_ForceSeal) ||
                                (Save_Manager.instance.data.Items.Drop.AffixCount_Max > 4) ||
                                (Save_Manager.instance.data.Items.Drop.AffixCount_Min > 0))
                            {
                                int nb_seals = 0;
                                foreach (ItemAffix aff in __0.affixes)
                                {
                                    if (!aff.isSealedAffix) { nb_affixs++; }
                                    else { nb_seals++; }
                                }
                                if ((Save_Manager.instance.data.Items.Drop.Enable_ForceSeal) && (nb_seals == 0))
                                {
                                    __0.AddRandomSealedAffix(UnityEngine.Random.Range(min_tier, max_tier));
                                }
                                int nb_affixes_wanted = UnityEngine.Random.RandomRangeInt((int)Save_Manager.instance.data.Items.Drop.AffixCount_Min, (int)Save_Manager.instance.data.Items.Drop.AffixCount_Max + 1);
                                if (nb_affixs < nb_affixes_wanted)
                                {
                                    eligibles_single_affixes = new System.Collections.Generic.List<int>();
                                    eligibles_single_affixes = GetEligibles_Single_Affixes(__0);
                                    eligibles_multi_affixes = new System.Collections.Generic.List<int>();
                                    eligibles_multi_affixes = GetEligibles_Multi_Affixes(__0);
                                    for (int i = nb_affixs; i < nb_affixes_wanted; i++)
                                    {
                                        __0.affixes.Add(RandomAffix(eligibles_single_affixes, eligibles_multi_affixes, false));
                                        __0.RefreshIDAndValues();
                                    }
                                    eligibles_single_affixes.Clear();
                                    eligibles_multi_affixes.Clear();
                                }
                            }*/
                            if ((Save_Manager.instance.data.Items.Drop.Enable_AffixTiers) ||
                                (Save_Manager.instance.data.Items.Drop.Enable_AffixValues) ||
                                (Save_Manager.instance.data.Items.Drop.Enable_SealTier) ||
                                (Save_Manager.instance.data.Items.Drop.Enable_SealValue))
                            {
                                foreach (ItemAffix aff in __0.affixes)
                                {
                                    if (!aff.isSealedAffix)
                                    {
                                        if (Save_Manager.instance.data.Items.Drop.Enable_AffixTiers)
                                        {
                                            byte roll = 0;
                                            if (Save_Manager.instance.data.Items.Drop.AffixTiers_Min == Save_Manager.instance.data.Items.Drop.AffixTiers_Max) { roll = (byte)Save_Manager.instance.data.Items.Drop.AffixTiers_Max; }
                                            else { roll = (byte)Random.RandomRange(Save_Manager.instance.data.Items.Drop.AffixTiers_Min, Save_Manager.instance.data.Items.Drop.AffixTiers_Max); }
                                            aff.affixTier = roll;
                                        }
                                        if (Save_Manager.instance.data.Items.Drop.Enable_AffixValues)
                                        {
                                            byte roll = 0;
                                            if (Save_Manager.instance.data.Items.Drop.AffixValues_Min == Save_Manager.instance.data.Items.Drop.AffixValues_Max) { roll = (byte)Save_Manager.instance.data.Items.Drop.AffixValues_Max; }
                                            else { roll = (byte)Random.RandomRange(Save_Manager.instance.data.Items.Drop.AffixValues_Min, Save_Manager.instance.data.Items.Drop.AffixValues_Max); }
                                            aff.affixRoll = roll;
                                        }
                                    }
                                    else
                                    {
                                        if (Save_Manager.instance.data.Items.Drop.Enable_SealTier)
                                        {
                                            byte roll = 0;
                                            if (Save_Manager.instance.data.Items.Drop.SealTier_Min == Save_Manager.instance.data.Items.Drop.SealTier_Max) { roll = (byte)Save_Manager.instance.data.Items.Drop.SealTier_Max; }
                                            else { roll = (byte)Random.RandomRange(Save_Manager.instance.data.Items.Drop.SealTier_Min, Save_Manager.instance.data.Items.Drop.SealTier_Max); }
                                            aff.affixTier = roll;
                                        }
                                        if (Save_Manager.instance.data.Items.Drop.Enable_SealValue)
                                        {
                                            byte roll = 0;
                                            if (Save_Manager.instance.data.Items.Drop.SealValue_Min == Save_Manager.instance.data.Items.Drop.SealValue_Max) { roll = (byte)Save_Manager.instance.data.Items.Drop.SealValue_Max; }
                                            else { roll = (byte)Random.RandomRange(Save_Manager.instance.data.Items.Drop.SealValue_Min, Save_Manager.instance.data.Items.Drop.SealValue_Max); }
                                            aff.affixRoll = roll;
                                        }
                                    }
                                }
                            }
                            /*if (__0.isUniqueSetOrLegendary()) //Legendary
                            {
                                eligibles_unique = new System.Collections.Generic.List<ushort>();
                                eligibles_unique = GetEligibles_Unique(__0);
                                if (eligibles_unique.Count > 0)
                                {
                                    int index = UnityEngine.Random.RandomRangeInt(0, eligibles_unique.Count);
                                    __0.uniqueID = eligibles_unique[index];
                                    UniqueList.Entry unique = UniqueList.getUnique(__0.uniqueID);
                                    if (unique != null)
                                    {
                                        int character_level = 100;
                                        float corruption = UnityEngine.Random.Range(0f, 255f);
                                        if (unique.legendaryType == UniqueList.LegendaryType.LegendaryPotential)
                                        {
                                            int min = UnityEngine.Random.Range(0, 5);
                                            float multiplier = UnityEngine.Random.Range(0f, 255f);
                                            bool out_b = false;
                                            __0.rollLegendaryPotential(unique, min, character_level, corruption, multiplier, out out_b);
                                        }
                                        else
                                        {
                                            int min = UnityEngine.Random.RandomRangeInt(0, 29);
                                            __0.RollWeaversWill(unique, min, character_level, corruption);
                                        }
                                        //Fix subtype
                                        __0.subType = unique.subTypes[0];
                                    }
                                    eligibles_unique.Clear();
                                }
                                else //No Unique found = Set rarity to base item
                                {
                                    byte item_rarity = (byte)nb_affixs;
                                    if (nb_affixs > 4) { item_rarity = 4; }
                                    __0.rarity = item_rarity;
                                }
                            }*/
                            __0.RefreshIDAndValues();
                            //if (legendary) { UniqueList.} //repair unique
                        }
                    }
                    else if ((__0.itemType > 24) && (__0.itemType < 34)) //Idols
                    {
                        if (Save_Manager.instance.data.Items.Drop.Enable_AffixValues)
                        {
                            foreach (ItemAffix aff in __0.affixes)
                            {
                                byte roll = 0;
                                if (Save_Manager.instance.data.Items.Drop.AffixValues_Min == Save_Manager.instance.data.Items.Drop.AffixValues_Max) { roll = (byte)Save_Manager.instance.data.Items.Drop.AffixValues_Max; }
                                else { roll = (byte)Random.RandomRange(Save_Manager.instance.data.Items.Drop.AffixValues_Min, Save_Manager.instance.data.Items.Drop.AffixValues_Max); }
                                aff.affixRoll = roll;
                            }
                            __0.RefreshIDAndValues();
                        }
                    }
                    else //New Items (have to check this)
                    {
                        if (Save_Manager.instance.data.Items.Drop.Enable_AffixValues)
                        {
                            foreach (ItemAffix aff in __0.affixes)
                            {
                                byte roll = 0;
                                if (Save_Manager.instance.data.Items.Drop.AffixValues_Min == Save_Manager.instance.data.Items.Drop.AffixValues_Max) { roll = (byte)Save_Manager.instance.data.Items.Drop.AffixValues_Max; }
                                else { roll = (byte)Random.RandomRange(Save_Manager.instance.data.Items.Drop.AffixValues_Min, Save_Manager.instance.data.Items.Drop.AffixValues_Max); }
                                aff.affixRoll = roll;
                            }
                            __0.RefreshIDAndValues();
                        }
                    }
                }
            }
        }

        /*private static System.Collections.Generic.List<int> eligibles_single_affixes = new System.Collections.Generic.List<int>();
        private static System.Collections.Generic.List<int> eligibles_multi_affixes = new System.Collections.Generic.List<int>();
        private static System.Collections.Generic.List<ushort> eligibles_unique = new System.Collections.Generic.List<ushort>();
        private static int min_tier = 1;
        private static int max_tier = 8;
        private static ItemAffix RandomAffix(System.Collections.Generic.List<int> eligibles_single, System.Collections.Generic.List<int> eligibles_multi, bool seal)
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
                affixTier = (byte)UnityEngine.Random.RandomRangeInt(0, 7),
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
        private static System.Collections.Generic.List<int> GetEligibles_Single_Affixes(ItemDataUnpacked item_data)
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
        private static System.Collections.Generic.List<int> GetEligibles_Multi_Affixes(ItemDataUnpacked item_data)
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
        private static System.Collections.Generic.List<ushort> GetEligibles_Unique(ItemDataUnpacked item_data_unpacked)
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
        }*/
        
    }
}
