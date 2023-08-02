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
                if (!Ui.ForceDrop.drop.generating_item)
                {
                    if (Config.Data.mods_config.items.Enable_RollLegendayPotencial)
                    {
                        __result = Config.Data.mods_config.items.Roll_Legendary_Potencial;
                    }
                }
                else { __result = Ui.ForceDrop.legenday_potencial.value; }
            }
        }

        [HarmonyPatch(typeof(ItemData), "RollWeaversWill")]
        public class RollWeaversWill
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemData __instance, ref int __result, ref UniqueList.Entry __0, ref int __1, ref int __2)
            {
                if (!Ui.ForceDrop.drop.generating_item)
                {
                    if (Config.Data.mods_config.items.Enable_RollWeaverWill)
                    {
                        __result = Config.Data.mods_config.items.Roll_Weaver_Will;
                    }
                }
                else { __result = Ui.ForceDrop.weaver_wil.value; }
            }
        }
               
        [HarmonyPatch(typeof(ItemData), "randomiseImplicitRolls")]
        public class randomiseImplicitRolls
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemData __instance)
            {
                if (!Ui.ForceDrop.drop.generating_item)
                {
                    if (Config.Data.mods_config.items.Enable_RollImplicit)
                    {
                        for (int i = 0; i < __instance.implicitRolls.Count; i++)
                        {
                            __instance.implicitRolls[i] = (byte)Config.Data.mods_config.items.Roll_Implicit;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < __instance.implicitRolls.Count; i++)
                    {
                        __instance.implicitRolls[i] = (byte)Ui.ForceDrop.implicits.value;
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
                if (!Ui.ForceDrop.drop.generating_item)
                {
                    if (Config.Data.mods_config.items.Enable_UniqueMod)
                    {
                        for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                        {
                            __instance.uniqueRolls[k] = Config.Data.mods_config.items.Roll_UniqueMod;
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                    {
                        __instance.uniqueRolls[k] = (byte)Ui.ForceDrop.unique_mods.value;
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
                if (!Ui.ForceDrop.drop.generating_item)
                {
                    foreach (ItemAffix affix in __instance.affixes)
                    {
                        if (affix.isSealedAffix)
                        {
                            if (Config.Data.mods_config.items.Enable_SealValue)
                            {
                                affix.affixRoll = Config.Data.mods_config.items.Roll_SealValue;
                            }
                        }
                    }
                }
                else if (Ui.ForceDrop.affixs.seal.add)
                {
                    int i = 0;
                    foreach (ItemAffix affix in __instance.affixes)
                    {
                        bool seal = affix.isSealedAffix;
                        if ((Ui.ForceDrop.rarity.dropdown_index == 3) && (i == 4)) { seal = true; }
                        if (seal)
                        {
                            if (Ui.ForceDrop.affixs.seal.override_value)
                            {
                                affix.affixRoll = (byte)Ui.ForceDrop.affixs.seal.values;
                            }
                            if (Ui.ForceDrop.affixs.seal.override_affix)
                            {
                                string affix_name = "";
                                if ((Ui.ForceDrop.type.is_idol) && (Ui.ForceDrop.affixs.seal.idol_dropdown_index > -1))
                                {
                                    affix_name = Ui.ForceDrop.affixs.seal.idol_dropdown_list[Ui.ForceDrop.affixs.seal.idol_dropdown_index];
                                }
                                else if (Ui.ForceDrop.affixs.seal.dropdown_index > -1)
                                {
                                    affix_name = Ui.ForceDrop.affixs.seal.dropdown_list[Ui.ForceDrop.affixs.seal.dropdown_index];
                                }
                                if (affix_name != "")
                                {
                                    affix.affixId = (ushort)Ui.ForceDrop.affixs.GetIdFromName(affix_name);
                                    affix.affixName = affix_name;
                                }
                            }
                        }
                        i++;
                    }
                }
            }
        }                
    }
}
