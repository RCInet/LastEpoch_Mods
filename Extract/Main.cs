using Extract.Json.Skills;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Extract
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        private void Main_Shown(object sender, EventArgs e)
        {
            //Load Skills
            //Load Skills Spritesheet
            //Do work
            //Enjoy
        }
        public static bool LoadSkillsJson()
        {
            bool result = false;
            string skills_path = @"C:\Program Files (x86)\Steam\steamapps\common\Last Epoch\Mods\Out_LastEpoch\Database\Skills\";
            //string skills_path = System.IO.Directory.GetCurrentDirectory() + @"\Out_LastEpoch\Database\Skills\";
            string json_filename = "Skills.json";
            string spritesheet_filename = "Skills_SpriteSheet.png";
            Console.WriteLine("Skill Path : " + skills_path);
            if (File.Exists(skills_path))
            {
                string JsonString = File.ReadAllText(skills_path + json_filename);
                List<Json.Skills.Root> skills = JsonConvert.DeserializeObject<List<Json.Skills.Root>>(JsonString);
                Bitmap sprite_sheet = new Bitmap(skills_path + spritesheet_filename);
                foreach (Json.Skills.Root character_class in skills)
                {
                    string Class_Path = skills_path + character_class.ClassName + @"\";
                    foreach (Json.Skills.DefaultSkill skill in character_class.DefaultSkills)
                    {
                        string icon_filename = skill.Name + ".png";
                        System.Drawing.Bitmap b = new System.Drawing.Bitmap(skill.Icon.SizeX, skill.Icon.SizeY);
                        System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(b);
                        graphics.DrawImage(sprite_sheet, skill.Icon.PositionX, skill.Icon.PositionY);
                        if (System.IO.File.Exists(Class_Path + icon_filename)) { System.IO.File.Delete(Class_Path + icon_filename); }
                        b.Save(Class_Path + icon_filename, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    foreach (Json.Skills.Mastery masterie in character_class.Masteries)
                    {
                        string Mastery_Path = Class_Path;
                        if (masterie.MasteryName != "All") { Mastery_Path = Class_Path + masterie.MasteryName + @"\"; }
                        foreach (Json.Skills.MasterySkill skill in masterie.MasterySkills)
                        {
                            string icon_filename = skill.Name + ".png";
                            System.Drawing.Bitmap b = new System.Drawing.Bitmap(skill.Icon.SizeX, skill.Icon.SizeY);
                            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(b);
                            graphics.DrawImage(sprite_sheet, skill.Icon.PositionX, skill.Icon.PositionY);
                            if (System.IO.File.Exists(Class_Path + icon_filename)) { System.IO.File.Delete(Class_Path + icon_filename); }
                            b.Save(Class_Path + icon_filename, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }

                result = true;
            }

            return result;
        }
    }
}
