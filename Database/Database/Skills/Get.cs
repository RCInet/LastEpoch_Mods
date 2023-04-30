using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Database.Skills
{
    public class Get
    {
        public static string Path = @"\Database\Skills\Skills.json";
        public static string[] MasteriesName(int main_class)
        {
            string[] result = new string[5];
            if (main_class == 0) { result = new string[] { "Druid", "Shaman", "BeastMaster" }; }
            if (main_class == 1) { result = new string[] { "Sorcerer", "SpellBlade" }; }
            if (main_class == 2) { result = new string[] { "ForgeGuard", "VoidKnight", "Paladin" }; }
            if (main_class == 3) { result = new string[] { "Necromancer", "Lich" }; }
            if (main_class == 4) { result = new string[] { "Marksman", "BladeDancer" }; }

            return result;
        }
        public static string[] ClassesName()
        {
            string[] result = new string[] { "Primalist", "Mage", "Sentinel", "Acolyte", "Rogue" };

            return result;
        }
        public static List<List<Json.Base>> Skills(bool filter, int classe, int masterie)
        {
            List<List<Json.Base>> result = new List<List<Json.Base>>();

            List<List<Json.Base>[]> skills_list = new List<List<Json.Base>[]>();
            skills_list.Add(new List<Json.Base>[]
                {
                    Database.Data.skills.Primalist.Base,
                    Database.Data.skills.Primalist.Druid,
                    Database.Data.skills.Primalist.Shaman,
                    Database.Data.skills.Primalist.BeastMaster
                });
            skills_list.Add(new List<Json.Base>[]
                {
                    Database.Data.skills.Mage.Base,
                    Database.Data.skills.Mage.Sorcerer,
                    Database.Data.skills.Mage.SpellBlade
                });
            skills_list.Add(new List<Json.Base>[]
                {
                    Database.Data.skills.Sentinel.Base,
                    Database.Data.skills.Sentinel.ForgeGuard,
                    Database.Data.skills.Sentinel.VoidKnight,
                    Database.Data.skills.Sentinel.Paladin
                });
            skills_list.Add(new List<Json.Base>[]
                {
                    Database.Data.skills.Acolyte.Base,
                    Database.Data.skills.Acolyte.Necromancer,
                    Database.Data.skills.Acolyte.Lich
                });
            skills_list.Add(new List<Json.Base>[]
                {
                    Database.Data.skills.Rogue.Base,
                    Database.Data.skills.Rogue.Marksman,
                    Database.Data.skills.Rogue.BladeDancer
                });

            int j = 0;
            foreach (string s in ClassesName())
            {
                if ((!filter) | (classe == j))
                {
                    int i = 0;
                    foreach (List<Json.Base> b in skills_list[j])
                    {
                        if ((!filter) | (masterie == i)) { result.Add(b); }
                        i++;
                    }
                }
                j++;
            }            
            
            return result;
        }
        public static List<string> Names(bool filter, int classe, int masterie)
        {
            List<string> result = new List<string>();
            foreach (var t in Skills(filter, classe, masterie))
            {
                foreach (Skills.Json.Base skill in t)
                {
                    result.Add(skill.Name);
                }                
            }

            return result;
        }
        public static string Name(string id)
        {
            bool found = false;
            string result = "";
            foreach (var t in Skills(false, 0, 0))
            {
                foreach (Skills.Json.Base skill_base in t)
                {
                    if (id == skill_base.Id) { result = skill_base.Name; found = true; break; }
                }
                if (found) { break; }
            }

            return result;
        }
        public static string TreeId(string name)
        {
            bool found = false;
            string result = "";
            foreach (var t in Skills(false, 0, 0))
            {
                foreach (Skills.Json.Base skill_base in t)
                {
                    if (name == skill_base.Name) { result = skill_base.Id; found = true; break; }
                }
                if (found) { break; }
            }

            return result;
        }
        
        public static Bitmap Icon(string tree_id)
        {
            Bitmap result = null;
            List<Json.Base>[] a = new List<Json.Base>[]
            {
                Database.Data.skills.Primalist.Base,
                Database.Data.skills.Primalist.Druid,
                Database.Data.skills.Primalist.Shaman,
                Database.Data.skills.Primalist.BeastMaster,
                Database.Data.skills.Mage.Base,
                    Database.Data.skills.Mage.Sorcerer,
                    Database.Data.skills.Mage.SpellBlade,
                Database.Data.skills.Sentinel.Base,
                    Database.Data.skills.Sentinel.ForgeGuard,
                    Database.Data.skills.Sentinel.VoidKnight,
                    Database.Data.skills.Sentinel.Paladin,
                Database.Data.skills.Acolyte.Base,
                    Database.Data.skills.Acolyte.Necromancer,
                    Database.Data.skills.Acolyte.Lich,
                Database.Data.skills.Rogue.Base,
                    Database.Data.skills.Rogue.Marksman,
                    Database.Data.skills.Rogue.BladeDancer
            };
            string[] paths = new string[]
            { @"Primalist\", @"Primalist\Druid\", @"Primalist\Shaman\", @"Primalist\BeastMaster\",
                @"Mage\",  @"Mage\Sorcerer\", @"Mage\SpellBlade\",
            @"Sentinel\", @"Sentinel\ForgeGuard\", @"Sentinel\VoidKnight\", @"Sentinel\Paladin\",
            @"Acolyte\", @"Acolyte\Necromancer\", @"Acolyte\Lich\",
            @"Rogue\", @"Rogue\Marksman\", @"Rogue\BladeDancer\"};

            bool found = false;
            int i = 0;
            string path = System.IO.Directory.GetCurrentDirectory() + @"\Database\Skills\";
            foreach (var b in a)
            {
                foreach (var t in b)
                {
                    if (tree_id == t.Id) { path += paths[i] + t.Name + ".png"; found = true; break; }
                }
                if (found) { break; }
                i++;
            }
            
            if (File.Exists(path)) { result = new Bitmap(path); }

            return result;
        }
    }
}
