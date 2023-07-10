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
            static void Postfix(GenerateItems __instance, ref int __result, ref ItemDataUnpacked __0, int __1, bool __2, bool __3, ref Il2CppSystem.Boolean __4)
            {
                if (Config.Data.mods_config.items.Enable_RollImplicit)
                {
                    for (int j = 0; j < __0.implicitRolls.Count; j++) //Work only for basic item
                    {
                        __0.implicitRolls[j] = Config.Data.mods_config.items.Roll_Implicit;
                    }
                }
                if (Config.Data.mods_config.items.Enable_ForgingPotencial)
                {
                    __0.forgingPotential = Config.Data.mods_config.items.Roll_ForgingPotencial;
                }
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
}
