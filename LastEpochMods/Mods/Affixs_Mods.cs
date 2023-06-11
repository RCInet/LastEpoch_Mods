using LastEpochMods.Db.Json;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Affixs_Mods
    {
        public static void MultiplyAffixsRolls(int mutiplier)
        {
            UnityEngine.Object obj = Functions.GetObject("MasterAffixesList");
            System.Type type = obj.GetActualType();
            if (type == typeof(AffixList))
            {
                AffixList affix_list = obj.TryCast<AffixList>();
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
        public static void EditAffixRollsByTier(int affix_id, int tier, int min, int max)
        {
            UnityEngine.Object obj = Functions.GetObject("MasterAffixesList");
            System.Type type = obj.GetActualType();
            if (type == typeof(AffixList))
            {
                AffixList affix_list = obj.TryCast<AffixList>();
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
}
