using System.Collections.Generic;

namespace LastEpochMods.Db
{
    public class Data
    {
        public static Db.Json.Affixs.Shards DB_Single_Affixs = new Db.Json.Affixs.Shards();
        public static Db.Json.Affixs.Shards DB_Multi_Affixs = new Db.Json.Affixs.Shards();

        public static bool Affixs_init = false;
        public static void InitAffixs()
        {
            Data.DB_Single_Affixs = new Db.Json.Affixs.Shards();
            Data.DB_Single_Affixs.List = new List<Db.Json.Affixs.Affix>();
            Data.DB_Multi_Affixs = new Db.Json.Affixs.Shards();
            Data.DB_Multi_Affixs.List = new List<Db.Json.Affixs.Affix>();
            Affixs_init = true;
        }
        public static Db.Json.Affixs.Affix SingleAffixToShard(AffixList.SingleAffix affix)
        {
            int x = affix.affixId / 256;
            int id = affix.affixId - (x * 256);
            string modifier = affix.modifierType.ToString();
            if (affix.tags.ToString() != "None") { modifier += " " + affix.tags.ToString(); }
            modifier += " " + affix.property.ToString();
            Db.Json.Affixs.Affix shard = new Db.Json.Affixs.Affix
            {
                Single = true,
                Modifier = modifier,
                ModifierList = null,
                DisplayName = affix.affixDisplayName,
                Id = id,
                X = x,
                Name = affix.affixName,
                Title = affix.affixTitle,
                Class = affix.classSpecificity.ToString(),
                DisplayCategory = affix.displayCategory.ToString(),
                Group = affix.group.ToString(),
                Level = affix.levelRequirement,
                RollOn = affix.rollsOn.ToString(),
                Tiers = affix.tiers.Capacity,
                Type = affix.type.ToString().ToLower()
            };

            return shard;
        }
        public static Db.Json.Affixs.Affix MultiAffixToShard(AffixList.MultiAffix affix)
        {
            int x = affix.affixId / 256;
            int id = affix.affixId - (x * 256);
            System.Collections.Generic.List<string> properties_list = new System.Collections.Generic.List<string>();
            Il2CppSystem.Collections.Generic.List<AffixList.AffixProperty> affix_properties_list = affix.affixProperties;
            foreach (AffixList.AffixProperty alp in affix_properties_list)
            {
                string modifier = alp.modifierType.ToString();
                if (alp.tags.ToString() != "None") { modifier += " " + alp.tags.ToString(); }
                modifier += " " + alp.property.ToString();
                properties_list.Add(modifier);
            }
            Db.Json.Affixs.Affix shard = new Db.Json.Affixs.Affix
            {
                Single = false,
                Modifier = "",
                ModifierList = properties_list,
                DisplayName = affix.affixDisplayName,
                Id = id,
                X = x,
                Name = affix.affixName,
                Title = affix.affixTitle,
                Class = affix.classSpecificity.ToString(),
                DisplayCategory = affix.displayCategory.ToString(),
                Group = affix.group.ToString(),
                Level = affix.levelRequirement,
                RollOn = affix.rollsOn.ToString(),
                Tiers = affix.tiers.Capacity,
                Type = affix.type.ToString().ToLower()
            };

            return shard;
        }
    }
}
