using Newtonsoft.Json;
using System.IO;

namespace Database.Skills
{
    public class Load
    {
        public static bool Json()
        {
            bool result = false;
            string skills_path = System.IO.Directory.GetCurrentDirectory() + Get.Path;
            if (File.Exists(skills_path))
            {
                string JsonString = File.ReadAllText(skills_path);
                Database.Data.skills = JsonConvert.DeserializeObject<Skills.Json.Root>(JsonString);
                result = true;
            }

            return result;
        }
    }
}
