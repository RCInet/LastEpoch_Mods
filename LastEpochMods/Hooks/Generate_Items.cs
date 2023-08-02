using HarmonyLib;
using static AffixList;
using static LastEpochMods.Ui.ForceDrop;

namespace LastEpochMods.Hooks
{
    public class Generate_Items
    {
        [HarmonyPatch(typeof(GenerateItems), "RollRarity")]
        public class RollRarity
        {
            [HarmonyPostfix]
            static void Postfix(ref byte __result, ref int __0)
            {
                if (!Ui.ForceDrop.drop.generating_item)
                {
                    if (Config.Data.mods_config.items.Enable_Rarity)
                    {
                        byte result = Config.Data.mods_config.items.GenerateItem_Rarity;
                        if (result < 0) { result = 0; }
                        else if (result > 9) { result = 9; }
                        else if ((result > 4) && (result < 7)) { result = 4; }                        
                        __result = result;
                    }                    
                }
            }
        }               

        [HarmonyPatch(typeof(GenerateItems), "RollAffixes")]
        public class RollAffixes
        {
            [HarmonyPostfix]
            static void Postfix(GenerateItems __instance, int __result, ref ItemDataUnpacked __0, int __1, bool __2, bool __3, ref Il2CppSystem.Boolean __4)
            {
                if (!Ui.ForceDrop.drop.generating_item)
                {                    
                    if (Config.Data.mods_config.items.Enable_SealTier)
                    {
                        int tier_result = System.Convert.ToInt32(Config.Data.mods_config.items.Roll_SealTier) - 1;
                        __0.AddRandomSealedAffix(tier_result);
                    }
                    foreach (ItemAffix aff in __0.affixes)
                    {
                        if (!aff.isSealedAffix)
                        {
                            if (Config.Data.mods_config.items.Enable_AffixsTier)
                            {
                                int tier_result = System.Convert.ToInt32(Config.Data.mods_config.items.Roll_AffixTier) - 1;
                                aff.affixTier = (byte)tier_result;
                            }
                            if (Config.Data.mods_config.items.Enable_AffixsValue)
                            {
                                aff.affixRoll = Config.Data.mods_config.items.Roll_AffixValue;
                            }
                        }
                    }
                }
                else
                {                    
                    if (Ui.ForceDrop.affixs.seal.add)
                    {
                        __0.AddRandomSealedAffix(Ui.ForceDrop.affixs.seal.tier - 1);
                    }
                    int i = 0;
                    foreach (ItemAffix aff in __0.affixes)
                    {
                        if (!aff.isSealedAffix)
                        {
                            if (Ui.ForceDrop.affixs.override_tier)
                            {
                                aff.affixTier = (byte)(Ui.ForceDrop.affixs.affix_tier - 1);
                            }
                            if (Ui.ForceDrop.affixs.override_value)
                            {
                                aff.affixRoll = (byte)Ui.ForceDrop.affixs.affix_values;
                            }
                            bool idol = false;
                            if ((__0.itemType > 24) && (__0.itemType < 34)) { idol = true; }
                            System.Collections.Generic.List<int> list = Ui.ForceDrop.affixs.GetIdList(idol);
                            if (i < list.Count)
                            {
                                int value = list[i];
                                if (value > -1)
                                {
                                    aff.affixId = (ushort)value;
                                    aff.affixName = Ui.ForceDrop.affixs.GetNameFromId(value);
                                }
                            }
                            i++;
                        }                        
                    }
                }                
            }
        }

        [HarmonyPatch(typeof(GenerateItems), "rollForgingPotential")]
        public class rollForgingPotential
        {
            [HarmonyPostfix]
            static void Postfix(ref int __result, ItemDataUnpacked __0, int __1, bool __2, GenerateItems.VendorType __3)
            {
                if (!Ui.ForceDrop.drop.generating_item)
                {
                    if (Config.Data.mods_config.items.Enable_ForgingPotencial)
                    {
                        __result = Config.Data.mods_config.items.Roll_ForgingPotencial;
                    }
                }
                else { __result = Ui.ForceDrop.forgin_potencial.value; }
            }
        }                 
    }
}
