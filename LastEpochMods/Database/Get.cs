using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastEpochMods.Database
{
    public class Get
    {
        private static string AppPath = System.IO.Directory.GetCurrentDirectory();
        public static string Affixs_Path = AppPath + @"\Mods\Out_LastEpoch\Database\Affixs\";
        public static string Affixs_IconPath = Affixs_Path + @"Icons\";
        public static string Affixs_Filename = "Affixs.json";        
    }
}
