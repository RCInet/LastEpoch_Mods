using System.IO;
using UnityEngine;

namespace LastEpochMods.Managers
{
    public class Assets_Manager
    {
        private static string AssetsPath = Application.dataPath + "/../Mods/LastEpochMods/Assets";
        public static AssetBundle asset_bundle;
        
        public static Sprite Headhunter_icon;

        /*public static Sprite Menu;
        public static Sprite PauseMenu_Bottom;
        public static Sprite PauseMenu_Menu;

        public static Sprite Armor;
        public static Sprite Boots;
        public static Sprite Gloves;
        public static Sprite Helmet;
        public static Sprite Shield;
        public static Sprite Weapons;*/

        public static void OnInitializeMelon()
        {
            LoadBundles();
        }

        public static void OnSceneWasInitialized()
        {
            if (Scenes_Manager.CurrentName == Scenes_Manager.MenuNames[3])
            {
                LoadAssets();
            }
        }

        private static void LoadBundles()
        {
            asset_bundle = LoadAssetBundle("lastepochmods.asset");
        }
        private static void LoadAssets()
        {
            if (asset_bundle != null )
            {
                // i = 0;
                foreach (string name in asset_bundle.GetAllAssetNames())
                {
                    //Main.logger_instance.Msg( i + " : " + name);
                    var asset = asset_bundle.LoadAsset(name);
                    if (name.Contains("headhunter_icon"))
                    {
                        Texture2D texture = asset.TryCast<Texture2D>();
                        Headhunter_icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    }
                    //i++;
                }
            }
            else { Main.logger_instance.Error("Asset Bundle not Loaded : path = " + Path.Combine(AssetsPath, "lastepochmods.asset")); }

            //Headhunter
            //Headhunter_icon = LoadTexture(@"Headhunter\Headhunter.png");
            //PauseMenu
            /*Menu = LoadTexture(@"PauseMenu\Menu.jpg");
            PauseMenu_Bottom = LoadTexture(@"PauseMenu\PauseMenu_Bottom.png");
            PauseMenu_Menu = LoadTexture(@"PauseMenu\PauseMenu_Menu.png");
            //Skins
            Armor = LoadTexture(@"Skins\Armor.png");
            Boots = LoadTexture(@"Skins\Boots.png");
            Gloves = LoadTexture(@"Skins\Gloves.png");
            Helmet = LoadTexture(@"Skins\Helmet.png");
            Shield = LoadTexture(@"Skins\Shield.png");
            Weapons = LoadTexture(@"Skins\Weapon.png");*/

            //Main.logger_instance.Msg("Textures Loaded");
        }
        
        /*private static Sprite LoadTexture(string picture)
        {
            Sprite sprite = null;
            //string path = AssetsPath + picture;
            string path = Path.Combine(AssetsPath, picture);
            Main.logger_instance.Msg("LoadTexture : " + path);

            if (File.Exists(path))
            {
                byte[] fileData = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(1, 1);
                ImageConversion.LoadImage(texture, fileData, true);
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }  
            else { Main.logger_instance.Error("Path not found : " + path); }

            return sprite;
        }*/
        private static AssetBundle LoadAssetBundle(string bundle)
        {
            string path = Path.Combine(AssetsPath, bundle);
            if (!File.Exists(path)) { return null; }

            return AssetBundle.LoadFromFile(path);
        }
    }
}
