using Newtonsoft.Json;
using System.IO;

namespace Database.Items
{
    public class Load
    {
        public static bool Json()
        {
            bool result = false;
            Data.Items = new Items.Json.Items[255];
            foreach (Get.Type_Structure type_structure in Get.TypesArray)
            {
                if (type_structure.Path != "")
                {
                    string path = System.IO.Directory.GetCurrentDirectory() + type_structure.Path;
                    string filename = type_structure.Name.Replace(' ', '_');
                    if (File.Exists(path + filename))
                    {
                        string JsonString = File.ReadAllText(path + filename);
                        if (type_structure.Id < 101)
                        { Data.Items[type_structure.Id] = JsonConvert.DeserializeObject<Json.Items>(JsonString); }
                    }
                }
            }

            return result;
        }
    }
}
