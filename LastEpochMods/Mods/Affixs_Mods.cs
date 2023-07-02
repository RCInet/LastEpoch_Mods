using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Affixs_Mods
    {
        private static AffixList affix_list = null;
        public static bool Enable_Affixs_Multiplier = true;
        public static int Affixs_Multiplier = 10;

        public static void Launch()
        {
            Mods.Affixs_Mods.MultiplyAffixsRolls(Affixs_Multiplier);
            //Mods.Affixs_Mods.EditAffixRollsByTier(100, 7, 100, 999); //Edit Affixs Exemple
        }
        private static void MultiplyAffixsRolls(int mutiplier)
        {
            if (Enable_Affixs_Multiplier)
            {
                affix_list = AffixList.get();
                if (affix_list != null)
                {
                    foreach (AffixList.SingleAffix s_affix in affix_list.singleAffixes)
                    {
                        Il2CppSystem.Collections.Generic.List<AffixList.Tier> tiers = s_affix.tiers;
                        foreach (AffixList.Tier tier in tiers)
                        {
                            tier.maxRoll = mutiplier * tier.maxRoll;
                            tier.minRoll = mutiplier * tier.minRoll;
                        }
                    }
                    foreach (AffixList.MultiAffix m_affix in affix_list.multiAffixes)
                    {
                        Il2CppSystem.Collections.Generic.List<AffixList.Tier> tiers = m_affix.tiers;
                        foreach (AffixList.Tier tier in tiers)
                        {
                            tier.maxRoll = mutiplier * tier.maxRoll;
                            tier.minRoll = mutiplier * tier.minRoll;
                        }
                    }
                }
            }
        }

        #region Exemple
        public static bool Enable_Edit_Affixs_Rolls = false;
        private static void EditAffixRollsByTier(int affix_id, int tier, int min, int max)
        {
            if (Enable_Edit_Affixs_Rolls)
            {                
                affix_list = AffixList.get();
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
    }
}
