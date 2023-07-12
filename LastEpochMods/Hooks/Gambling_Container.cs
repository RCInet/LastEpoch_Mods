using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Gambling_Container
    {
        [HarmonyPatch(typeof(GamblingContainer), "getItemLevelForPopulating")]
        public class getItemLevelForPopulating
        {
            [HarmonyPostfix]
            static void Postfix(GamblingContainer __instance, ref int __result)
            {
                __result = 100;
            }
        }

        /*[HarmonyPatch(typeof(GamblingContainer), "MoveItemTo")]
        public class MoveItemTo
        {
            [HarmonyPostfix]
            static void Postfix(GamblingContainer __instance, bool __result, ref ItemContainerEntry __0, int __1, IItemContainer __2, Il2CppSystem.Nullable<UnityEngine.Vector2Int> __3, Context __4)
            {
                ItemData item_data = __0.data;
                ItemDataUnpacked item_data_unpacked = item_data.getAsUnpacked();
                if (Config.Data.mods_config.items.Enable_Rarity) { item_data_unpacked.rarity = Config.Data.mods_config.items.GenerateItem_Rarity; }
                if (Config.Data.mods_config.items.Enable_RollImplicit)
                {  
                    //Basic Magic Rare
                    for (int j = 0; j < item_data_unpacked.implicitRolls.Count; j++)
                    {
                        item_data_unpacked.implicitRolls[j] = Config.Data.mods_config.items.Roll_Implicit;
                    }
                    //Unique Set
                    for (int i = 0; i < item_data.implicitRolls.Count; i++)
                    {
                        item_data.implicitRolls[i] = (byte)Config.Data.mods_config.items.Roll_Implicit;
                    }
                }
                if (Config.Data.mods_config.items.Enable_ForgingPotencial)
                {
                    item_data_unpacked.forgingPotential = (byte)Config.Data.mods_config.items.Roll_ForgingPotencial;
                }
                if ((Config.Data.mods_config.items.Enable_AffixsTier) | (Config.Data.mods_config.items.Enable_AffixsValue))
                {
                    int tier_result = System.Convert.ToInt32(Config.Data.mods_config.items.Roll_AffixTier) - 1;
                    for (int i = 0; i < item_data_unpacked.affixes.Count; i++)
                    {
                        if (Config.Data.mods_config.items.Enable_AffixsTier) { item_data_unpacked.affixes[i].affixTier = (byte)tier_result; }
                        if (Config.Data.mods_config.items.Enable_AffixsValue) { item_data_unpacked.affixes[i].affixRoll = Config.Data.mods_config.items.Roll_AffixValue; }
                    }                    
                }
                if (Config.Data.mods_config.items.Enable_UniqueMod)
                {
                    for (int k = 0; k < item_data.uniqueRolls.Count; k++)
                    {
                        item_data.uniqueRolls[k] = Config.Data.mods_config.items.Roll_UniqueMod;
                    }
                }
                if (Config.Data.mods_config.items.Enable_RollLegendayPotencial)
                {
                    item_data.legendaryPotential = (byte)Config.Data.mods_config.items.Roll_Legendary_Potencial;
                }
                if (Config.Data.mods_config.items.Enable_RollWeaverWill)
                {
                    item_data.weaversWill = (byte)Config.Data.mods_config.items.Roll_Weaver_Will;
                }
            }
        }*/
    }
}
