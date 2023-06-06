using UnityEngine;
using UniverseLib;
using static LastEpochMods.Mods.LastEpochSaveEditor;

namespace LastEpochMods
{
    public class Functions
    {
        public static UnityEngine.Object GetObject(string name)
        {
            UnityEngine.Object objet = new UnityEngine.Object();
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((name != "") && (obj.name.Contains(name)))
                {
                    System.Type type = obj.GetActualType();
                    if (type != typeof(TextAsset))
                    {
                        objet = obj;
                        break;
                    }
                }
            }
            return objet;
        }
        public static UnityEngine.Texture2D GetTexture2D(string name)
        {
            UnityEngine.Texture2D picture = null;
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((name != "") && (obj.name.Contains(name)))
                {
                    System.Type type = obj.GetActualType();
                    if (type == typeof(UnityEngine.Texture2D))
                    {
                        picture = obj.TryCast<UnityEngine.Texture2D>();
                        break;
                    }
                }
            }

            return picture;
        }
        /*
        public static void SaveSkillIconFromSpriteSheet(Ability ability, string icon_fullpath)
        {
            int spritesheet_w = ability.abilitySprite.texture.width;
            int spritesheet_h = ability.abilitySprite.texture.height;
            int position_x = System.Convert.ToInt32(ability.abilitySprite.textureRect.position.x);
            int position_y = System.Convert.ToInt32(ability.abilitySprite.textureRect.position.y);
            int icon_size_x = System.Convert.ToInt32(ability.abilitySprite.textureRect.width);
            int icon_size_y = System.Convert.ToInt32(ability.abilitySprite.textureRect.height);
            int x = System.Convert.ToInt32(ability.abilitySprite.textureRect.position.x);
            int y = spritesheet_h - position_y - icon_size_y;
            UnityEngine.Texture2D SpriteSheet = ability.abilitySprite.texture;
            Rect rectangle = new Rect
            {
                position = new Vector2(x, y),
                width = icon_size_x,
                height = icon_size_y                
            };
            UnityEngine.Texture2D Icon = UniverseLib.Runtime.TextureHelper.CopyTexture(ability.abilitySprite.texture, rectangle);
            UniverseLib.Runtime.TextureHelper.SaveTextureAsPNG(Icon, icon_fullpath);
        }
        public static void SaveSpriteSheetToPng(Texture2D texture, string path, string name)
        {
            UnityEngine.Texture2D SpriteSheet = texture;
            if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
            if (!System.IO.File.Exists(path + name + "_SpriteSheet.png"))
            { UniverseLib.Runtime.TextureHelper.SaveTextureAsPNG(SpriteSheet, path + name + "_SpriteSheet.png"); }
        }
        public static Mods.LastEpochSaveEditor.Skill_structure AbilityToSkill(Ability ability)
        {
            Mods.LastEpochSaveEditor.Skill_structure skill = new Skill_structure
            {
                Name = ability.abilityName,
                Id = ability.playerAbilityID,
                Icon = new Skill_Icon_Structure
                {
                    PositionX = System.Convert.ToInt32(ability.abilitySprite.textureRect.x),
                    PositionY = System.Convert.ToInt32(ability.abilitySprite.textureRect.y),
                    SizeX = System.Convert.ToInt32(ability.abilitySprite.textureRect.width),
                    SizeY = System.Convert.ToInt32(ability.abilitySprite.textureRect.height)
                }
            };

            return skill;
        }*/
    }
}
