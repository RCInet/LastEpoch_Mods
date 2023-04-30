using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Materials.Runes
{
    class Load
    {
        public static bool Json()
        {
            bool result = false;
            string path = System.IO.Directory.GetCurrentDirectory() + Get.Path + Get.Filename;
            if (File.Exists(path))
            {
                string JsonString = File.ReadAllText(path);
                Data.Runes = JsonConvert.DeserializeObject<Json.Runes>(JsonString);
                result = true;
            }

            return result;
        }
    }
}
