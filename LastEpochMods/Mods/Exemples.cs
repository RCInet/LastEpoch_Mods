using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Exemples
    {
        #region character
        public static void Launch_ExempleBuffCharacter()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((obj.name == "MainPlayer(Clone)") && (obj.GetActualType() == typeof(StatBuffs)))
                {
                    float duration = 255;
                    SP propertie = SP.Intelligence;
                    float added_value = 255;
                    float increase_value = 255;
                    Il2CppSystem.Collections.Generic.List<float> more_values = null;
                    AT tag = AT.Buff;
                    byte special_tag = 0;

                    obj.TryCast<StatBuffs>().addBuff(duration, propertie, added_value, increase_value, more_values, tag, special_tag);
                }
            }
        }
        #endregion
        #region Affixs
        public static bool Enable_Edit_Affixs_Rolls = false;
        private static void EditAffixRollsByTier(int affix_id, int tier, int min, int max)
        {
            if (Enable_Edit_Affixs_Rolls)
            {
                AffixList affix_list = AffixList.get();
                if (affix_list != null)
                {
                    bool found = false;
                    foreach (AffixList.SingleAffix s_affix in affix_list.singleAffixes)
                    {
                        if (s_affix.affixId == affix_id)
                        {
                            if (((tier - 1) > -1) && (s_affix.tiers.Count > (tier - 1)))
                            {
                                s_affix.tiers[(tier - 1)].maxRoll = max;
                                s_affix.tiers[(tier - 1)].minRoll = min;
                            }
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        foreach (AffixList.MultiAffix m_affix in affix_list.multiAffixes)
                        {
                            if (m_affix.affixId == affix_id)
                            {
                                if (((tier - 1) > -1) && (m_affix.tiers.Count > (tier - 1)))
                                {
                                    m_affix.tiers[(tier - 1)].maxRoll = max;
                                    m_affix.tiers[(tier - 1)].minRoll = min;
                                }
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region Unique
        public struct unique_mod
        {
            public int id;
            public Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods;
        }
        public static System.Collections.Generic.List<unique_mod> Uniques_Mods = new System.Collections.Generic.List<unique_mod>();
        public static bool Enable_UniqueMods = false;

        //Exemple Custom Mods
        public static Il2CppSystem.Collections.Generic.List<UniqueItemMod> CustomMods_0()
        {
            Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.AttackSpeed,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.CriticalChance,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.CriticalMultiplier,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.Damage,
                tags = AT.Physical
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Strength,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Intelligence,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Dexterity,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.IncreasedDropRate,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.IncreasedExperience,
                tags = AT.None
            });

            return mods;
        }
        public static Il2CppSystem.Collections.Generic.List<UniqueItemMod> CustomMods_1()
        {
            //Create new Array of UniqueItemMod
            Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();
            //Add UniqueItemMod to Array
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.AttackSpeed,
                tags = AT.None
            });
            //Add More

            return mods;
        }

        public struct ability_mod
        {
            public ushort unique_id;
            public int skill_index;
        }
        public static System.Collections.Generic.List<ability_mod> Ability_Mods = new System.Collections.Generic.List<ability_mod>();
        public static bool Enable_AbilityMods = true;
        #endregion
    }
}
