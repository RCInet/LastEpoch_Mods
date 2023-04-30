using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastEpochMods.Src.Icons
{
    public class Get
    {
        private static string AppPath = System.IO.Directory.GetCurrentDirectory();
        public static string ShardsIcon_Path = AppPath + @"\Mods\Src\Icons\Items\";
        public static string OldersShardsIcon_Path = ShardsIcon_Path + @"\Olders\";
        public struct shard_struct
        {
            public string name;
            public string icon_name;
            public string url;
        }
    }
}
