using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LastEpochMods.Config
{
    public class Save
    {
        public static void Mods()
        {
            string jsonString = JsonConvert.SerializeObject(Data.mods_config);
            if (!Directory.Exists(Data.path)) { Directory.CreateDirectory(Data.path); }
            if (File.Exists(Data.path + Data.filename)) { File.Delete(Data.path + Data.filename); }
            File.WriteAllText(Data.path + Data.filename, jsonString);
        }
    }
}
