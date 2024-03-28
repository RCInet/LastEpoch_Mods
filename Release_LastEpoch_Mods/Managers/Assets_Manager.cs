using System.IO;
using UnityEngine;

namespace LastEpochMods.Managers
{
    public class Assets_Manager
    {
        private static string AssetsPath = Application.dataPath + "/../Mods/LastEpochMods/Assets";
        public static AssetBundle asset_bundle;
        
        public static void OnInitializeMelon()
        {
            Main.logger_instance.Msg("Initialize Assets Manager");
            string bundle_name = "lastepochmods.asset";
            if ((Directory.Exists(AssetsPath)) && (File.Exists(Path.Combine(AssetsPath, bundle_name))))
            {
                asset_bundle = LoadAssetBundle(bundle_name);
                if (asset_bundle == null) { Main.logger_instance.Error("Asset Bundle Not Loaded"); }
            }
            else { Main.logger_instance.Error(bundle_name + " Not Found in Assets directory"); }
        }

        public class Headhunter
        {
            public static bool loaded = false;
            public static Sprite icon;
            public static TextAsset json;
        }
        public class PauseMenu
        {
            public static GameObject Prefab = null;
            public static GameObject Hud = null;

            public static Texture2D PauseMenu_Bottom;
            public static Texture2D PauseMenu_Menu;
        }
        public class Skins
        {
            public static Sprite Armor;
            public static Sprite Boots;
            public static Sprite Gloves;
            public static Sprite Helmet;
            public static Sprite Shield;
            public static Sprite Weapons;
        }
        public static class Extensions
        {
            public static readonly string jpg = ".jpg";
            public static readonly string png = ".png";
            public static readonly string json = ".json";
            public static readonly string prefab = ".prefab";            
        }

        public static void Load_Headhunter()
        {
            if (asset_bundle != null)
            {
                foreach (string name in asset_bundle.GetAllAssetNames())
                {
                    if (name.Contains("/headhunter/"))
                    {
                        if ((Check_Texture(name)) && (name.Contains("icon")))
                        {
                            Texture2D texture = asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                            Headhunter.icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                        }
                        else if ((Check_Json(name)) && (name.Contains("hh_buffs")))
                        {
                            Headhunter.json = asset_bundle.LoadAsset(name).TryCast<TextAsset>();
                        }
                    }
                }
                Headhunter.loaded = true;
            }
            else { Main.logger_instance.Error("Asset Bundle not Loaded : path = " + Path.Combine(AssetsPath, "lastepochmods.asset")); }
        }
        public static void Load_Skins()
        {
            if (asset_bundle != null)
            {
                //Main.logger_instance.Msg("Initialize Assets for Skins");
                foreach (string name in asset_bundle.GetAllAssetNames())
                {
                    if ((Check_Texture(name)) && (name.Contains("/skins/")))
                    {
                        Texture2D texture = asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                        Sprite picture = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                        if (name.Contains("armor")) { Skins.Armor = picture; }
                        else if (name.Contains("boots")) { Skins.Boots = picture; }
                        else if (name.Contains("gloves")) { Skins.Gloves = picture; }
                        else if (name.Contains("helmet")) { Skins.Helmet = picture; }
                        else if (name.Contains("shield")) { Skins.Shield = picture; }
                        else if (name.Contains("weapon")) { Skins.Weapons = picture; }
                    }
                }
            }
            else { Main.logger_instance.Error("Asset Bundle not Loaded : path = " + Path.Combine(AssetsPath, "lastepochmods.asset")); }
        }
        public static void Load_PauseMenu()
        {
            if (asset_bundle != null)
            {
                foreach (string name in asset_bundle.GetAllAssetNames())
                {
                    if (name.Contains("/pausemenu/"))
                    {
                        if (Check_Texture(name))
                        {
                            if (name.Contains("_bottom")) { PauseMenu.PauseMenu_Bottom = asset_bundle.LoadAsset(name).TryCast<Texture2D>(); }
                            else if (name.Contains("_menu")) { PauseMenu.PauseMenu_Menu = asset_bundle.LoadAsset(name).TryCast<Texture2D>(); }
                        }
                        else if (Check_Prefab(name))
                        {
                            if ((name.Contains("pausemenu.prefab")) && (PauseMenu.Prefab.IsNullOrDestroyed()))
                            {
                                if (PauseMenu.Prefab.IsNullOrDestroyed())
                                {
                                    PauseMenu.Prefab = asset_bundle.LoadAsset(name).TryCast<GameObject>();
                                    PauseMenu.Prefab.AddComponent<UIMouseListener>(); //Block Mouse
                                    PauseMenu.Prefab.active = false; //Hide
                                    Main.logger_instance.Msg("Prefab Created");
                                }
                                if ((!PauseMenu.Prefab.IsNullOrDestroyed()) && (PauseMenu.Hud.IsNullOrDestroyed()))
                                {
                                    PauseMenu.Hud = Object.Instantiate(PauseMenu.Prefab, Vector3.zero, Quaternion.identity);
                                }
                            }
                        }
                    }
                }
            }
            else { Main.logger_instance.Error("Asset Bundle not Loaded : path = " + Path.Combine(AssetsPath, "lastepochmods.asset")); }
        }

        private static bool Check_Texture(string name)
        {
            if ((name.Substring(name.Length - Extensions.jpg.Length, Extensions.jpg.Length).ToLower() == Extensions.jpg) ||
                        (name.Substring(name.Length - Extensions.png.Length, Extensions.png.Length).ToLower() == Extensions.png))
            {
                return true;
            }
            else { return false; }
        }
        private static bool Check_Json(string name)
        {
            if (name.Substring(name.Length - Extensions.json.Length, Extensions.json.Length).ToLower() == Extensions.json)                        
            {
                return true;
            }
            else { return false; }
        }
        private static bool Check_Prefab(string name)
        {
            if (name.Substring(name.Length - Extensions.prefab.Length, Extensions.prefab.Length).ToLower() == Extensions.prefab)
            {
                return true;
            }
            else { return false; }
        }        


        /*private static void LoadAssets()
        {
            if (asset_bundle != null )
            {
                int i = 0;
                foreach (string name in asset_bundle.GetAllAssetNames())
                {
                    Main.logger_instance.Msg( i + " : " + name);
                    //var asset = asset_bundle.LoadAsset(name);
                    string jpg = ".jpg";
                    string png = ".png";
                    string prefab = ".prefab";

                    if ((name.Substring(name.Length - jpg.Length, jpg.Length).ToLower() == jpg) ||
                        (name.Substring(name.Length - png.Length, png.Length).ToLower() == png))
                    {
                        Texture2D texture = asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                        Sprite picture = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                        
                        if ((name.Contains("/headhunter/")) && (name.Contains("icon"))) { Sprites.Headhunter.icon = picture; }
                        else if (name.Contains("/pausemenu/"))
                        {
                            if (name.Contains("_bottom")) { Sprites.PauseMenu.PauseMenu_Bottom = picture; }
                            else if (name.Contains("_menu")) { Sprites.PauseMenu.PauseMenu_Menu = picture; }
                            //else if (name.Contains("menu")) { Sprites.PauseMenu.Menu = picture; }
                        }
                        else if (name.Contains("/skins/"))
                        {
                            Main.logger_instance.Msg("Skins");
                            if (name.Contains("armor"))
                            {
                                Main.logger_instance.Msg("Armor");
                                Sprites.Skins.Armor = picture;
                            }
                            else if (name.Contains("boots")) { Sprites.Skins.Boots = picture; }
                            else if (name.Contains("gloves")) { Sprites.Skins.Gloves = picture; }
                            else if (name.Contains("helmet")) { Sprites.Skins.Helmet = picture; }
                            else if (name.Contains("shield")) { Sprites.Skins.Shield = picture; }
                            else if (name.Contains("weapon")) { Sprites.Skins.Weapons = picture; }
                        }
                    }
                    else if (name.Substring(name.Length - prefab.Length, prefab.Length).ToLower() == prefab)
                    {

                    }
                    i++;
                }
            }
            else { Main.logger_instance.Error("Asset Bundle not Loaded : path = " + Path.Combine(AssetsPath, "lastepochmods.asset")); }
        }*/

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
