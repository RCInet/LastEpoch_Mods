using Newtonsoft.Json;
using System.IO;

namespace Database.Items.Keys
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
                Data.Keys = JsonConvert.DeserializeObject<Json.Keys>(JsonString);
                result = true;
            }

            return result;
        }
    }
}
