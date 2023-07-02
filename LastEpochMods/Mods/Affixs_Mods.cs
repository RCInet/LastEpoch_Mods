using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Affixs_Mods
    {
        private static AffixList affix_list = null;
        //private static bool affixs_found = false;
        /*private static void InitAffixsList()
        {
            if (!affixs_found)
            {
                try
                {
                    affix_list = AffixList.get();
                    if (affix_list != null)
                    {
                        affixs_found = true;
                        Main.logger_instance.Msg("Affixs item list found : " + affix_list.name);
                    }
                }
                catch (System.Exception ex)
                {
                    Main.logger_instance.Msg("Error Affixs item list");
                }
            }
        }*/

        public static bool Enable_Affixs_Multiplier = true;
        public static int Affixs_Multiplier = 10;

        public static bool Enable_Edit_Affixs_Rolls = false;
        private static void MultiplyAffixsRolls(int mutiplier)
        {
            if (Enable_Affixs_Multiplier)
            {
                //if (!affixs_found) { InitAffixsList(); }

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
        private static void EditAffixRollsByTier(int affix_id, int tier, int min, int max)
        {
            if (Enable_Edit_Affixs_Rolls)
            {
                //if (!affixs_found) { InitAffixsList(); }
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
        public static void Launch()
        {
            Mods.Affixs_Mods.MultiplyAffixsRolls(Affixs_Multiplier);
            //Mods.Affixs_Mods.EditAffixRollsByTier(100, 7, 100, 999); //Edit Affixs Exemple
        }
    }
}
