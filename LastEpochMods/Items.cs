using System;

namespace LastEpochMods
{
    public class Items
    {
        public static System.Collections.Generic.List<string> GetItemImplicits(Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> item_implicits)
        {
            System.Collections.Generic.List<string> list_implicits = new System.Collections.Generic.List<string>();
            foreach (ItemList.EquipmentImplicit item_implicit in item_implicits)
            {
                string min_value = Convert.ToString(item_implicit.implicitValue);
                string max_value = Convert.ToString(item_implicit.implicitMaxValue);
                if ((item_implicit.implicitValue >= 0) && (item_implicit.implicitValue <= 1) &&
                    (item_implicit.implicitMaxValue >= 0) && (item_implicit.implicitMaxValue <= 1))
                {
                    min_value = Convert.ToString(item_implicit.implicitValue * 100) + " %";
                    max_value = Convert.ToString(item_implicit.implicitMaxValue * 100) + " %";
                }
                string value = min_value;
                if (item_implicit.implicitValue != item_implicit.implicitMaxValue)
                {
                    value = "(" + min_value + " to " + max_value + ")";
                }
                string implicit_type = item_implicit.type.ToString().ToLower();
                string implicit_tags = "";
                if (item_implicit.tags.ToString() != "None") { implicit_tags = " " + item_implicit.tags.ToString(); }
                string implicit_string = implicit_type + " " + value + implicit_tags + " " + item_implicit.property.ToString().ToLower();
                if (implicit_type == "added")
                {
                    implicit_string = "+" + value + implicit_tags + " " + item_implicit.property.ToString();
                }
                else if (implicit_type == "increased")
                {
                    implicit_string = value + " " + implicit_type + implicit_tags + " " + item_implicit.property.ToString();
                }
                list_implicits.Add(implicit_string);
            }

            return list_implicits;
        }
        public static System.Collections.Generic.List<string> GetUniqueMods(Il2CppSystem.Collections.Generic.List<UniqueItemMod> unique_mods)
        {
            System.Collections.Generic.List<string> list_unique_affixs = new System.Collections.Generic.List<string>();
            foreach (UniqueItemMod mod in unique_mods)
            {
                string min_value = Convert.ToString(mod.value);
                string max_value = Convert.ToString(mod.maxValue);
                if ((mod.value >= 0) && (mod.value <= 1) &&
                    (mod.maxValue >= 0) && (mod.maxValue <= 1))
                {
                    min_value = Convert.ToString(mod.value * 100) + " %";
                    max_value = Convert.ToString(mod.maxValue * 100) + " %";
                }
                string value = min_value;
                if (mod.value != mod.maxValue)
                {
                    value = "(" + min_value + " to " + max_value + ")";
                }
                string mod_type = mod.type.ToString().ToLower();
                string mod_tag = "";
                if (mod.tags.ToString() != "None") { mod_tag = " " + mod.tags.ToString(); }
                string mod_string = mod_type + " " + value + mod_tag + " " + mod.property.ToString().ToLower();
                if (mod_type == "added")
                {
                    mod_string = "+" + value + mod_tag + " " + mod.property.ToString();
                }
                else if (mod_type == "increased")
                {
                    mod_string = value + " " + mod_type + mod_tag + " " + mod.property.ToString();
                }
                list_unique_affixs.Add(mod_string);
            }

            return list_unique_affixs;
        }
    }
}
