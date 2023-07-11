using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastEpochMods.OnSceneChanged
{
    public class Affix_List
    {
        public static void Init()
        {
            if (Config.Data.mods_config.affixs.Enable_Affixs_Multiplier)
            {
                AffixList affix_list = AffixList.get();
                if (affix_list != null)
                {
                    int multiplier = Config.Data.mods_config.affixs.Affixs_Multiplier;
                    foreach (AffixList.SingleAffix s_affix in affix_list.singleAffixes)
                    {
                        Il2CppSystem.Collections.Generic.List<AffixList.Tier> tiers = s_affix.tiers;
                        foreach (AffixList.Tier tier in tiers)
                        {
                            tier.maxRoll = multiplier * tier.maxRoll;
                            tier.minRoll = multiplier * tier.minRoll;
                        }
                    }
                    foreach (AffixList.MultiAffix m_affix in affix_list.multiAffixes)
                    {
                        Il2CppSystem.Collections.Generic.List<AffixList.Tier> tiers = m_affix.tiers;
                        foreach (AffixList.Tier tier in tiers)
                        {
                            tier.maxRoll = multiplier * tier.maxRoll;
                            tier.minRoll = multiplier * tier.minRoll;
                        }
                    }
                }
            }
        }
    }
}
