using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Generate_Items
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

        [HarmonyPatch(typeof(GenerateItems), "RollAffixes")]
        public class RollAffixes
        {
            [HarmonyPostfix]
            static void Postfix(GenerateItems __instance, ref int __result, ref ItemDataUnpacked __0, int __1, ref bool __2, ref bool __3, ref Il2CppSystem.Boolean __4)
            {
                if ((Ui.GenerateItem.drop.generating_random_item) && (__0.affixes.Count > Ui.GenerateItem.affixs.nb_affixs))
                {
                    Il2CppSystem.Collections.Generic.List<ItemAffix> new_list = new Il2CppSystem.Collections.Generic.List<ItemAffix>();
                    for (int z = 0; z < Ui.GenerateItem.affixs.nb_affixs; z++) { new_list.Add(__0.affixes[z]); }
                    __0.affixes = new_list;
                }
                for (int i = 0; i < __0.affixes.Count; i++)
                {
                    if (Config.Data.mods_config.items.Enable_AffixsTier)
                    {
                        int tier_result = System.Convert.ToInt32(Config.Data.mods_config.items.Roll_AffixTier) - 1;
                        __0.affixes[i].affixTier = (byte)tier_result;
                    }
                    if (Config.Data.mods_config.items.Enable_AffixsValue) { __0.affixes[i].affixRoll = Config.Data.mods_config.items.Roll_AffixValue; }
                    if (Ui.GenerateItem.drop.generating_random_item)
                    {
                        bool idol = false;
                        if ((__0.itemType > 24) && (__0.itemType < 34)) { idol = true; }
                        System.Collections.Generic.List<int> list = Ui.GenerateItem.affixs.GetIdList(idol);
                        if (i < list.Count)
                        {
                            int value = list[i];
                            if (value > -1)
                            {
                                __0.affixes[i].affixId = (ushort)value;
                                __0.affixes[i].affixName = Ui.GenerateItem.affixs.GetNameFromId(value);                                
                            }
                        }
                    }
                }

                //__2 = true; //Can roll uncraftable Tier
                //__3 = false; //force Exalted
            }
        }

        [HarmonyPatch(typeof(GenerateItems), "rollForgingPotential")]
        public class rollForgingPotential
        {
            [HarmonyPostfix]
            static void Postfix(ref int __result, ItemDataUnpacked __0, int __1, bool __2, GenerateItems.VendorType __3)
            {
                if (Config.Data.mods_config.items.Enable_ForgingPotencial)
                {
                    __result = Config.Data.mods_config.items.Roll_ForgingPotencial;
                }
            }
        }                 
    }
}
