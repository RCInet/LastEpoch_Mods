using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Item_Data
    {
        [HarmonyPatch(typeof(ItemData), "CalculateLevelAndClassRequirement")]
        public class CalculateLevelAndClassRequirement
        {
            [HarmonyPostfix]
            static void Postfix(ItemData __instance, ref int __result, ref ItemList.ClassRequirement __0, ref ItemList.SubClassRequirement __1)
            {
                if (Config.Data.mods_config.items.Remove_LevelReq) { __result = 0; }
                if (Config.Data.mods_config.items.Remove_ClassReq) { __0 = ItemList.ClassRequirement.None; }
                if (Config.Data.mods_config.items.Remove_SubClassReq) { __1 = ItemList.SubClassRequirement.None; }
            }
        }

        [HarmonyPatch(typeof(ItemData), "rollLegendaryPotential")]
        public class rollLegendaryPotential
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemData __instance, ref int __result, ref UniqueList.Entry __0, ref int __1, ref int __2)
            {
                if (Config.Data.mods_config.items.Enable_RollImplicit)
                {
                    for (int i = 0; i < __instance.implicitRolls.Count; i++)
                    {
                        __instance.implicitRolls[i] = (byte)Config.Data.mods_config.items.Roll_Implicit;
                    }
                }
                if (Config.Data.mods_config.items.Enable_UniqueMod)
                {
                    for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                    {
                        __instance.uniqueRolls[k] = Config.Data.mods_config.items.Roll_UniqueMod;
                    }
                }
                if (Config.Data.mods_config.items.Enable_RollLegendayPotencial)
                {
                    __result = Config.Data.mods_config.items.Roll_Legendary_Potencial;
                }
            }
        }

        [HarmonyPatch(typeof(ItemData), "RollWeaversWill")]
        public class RollWeaversWill
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemData __instance, ref int __result, ref UniqueList.Entry __0, ref int __1, ref int __2)
            {
                if (Config.Data.mods_config.items.Enable_RollImplicit)
                {
                    for (int i = 0; i < __instance.implicitRolls.Count; i++)
                    {
                        __instance.implicitRolls[i] = (byte)Config.Data.mods_config.items.Roll_Implicit;
                    }
                }
                if (Config.Data.mods_config.items.Enable_UniqueMod)
                {
                    for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                    {
                        __instance.uniqueRolls[k] = Config.Data.mods_config.items.Roll_UniqueMod;
                    }
                }
                if (Config.Data.mods_config.items.Enable_RollWeaverWill)
                {
                    __result = Config.Data.mods_config.items.Roll_Weaver_Will;
                }
            }
        }
    }
}
