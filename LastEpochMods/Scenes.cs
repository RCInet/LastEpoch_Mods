using System.Linq;

namespace LastEpochMods
{
    public class Scenes
    {
        public static string CurrentName = "";
        public static string[] MenuNames = { "PersistentUI", "Login", "CharacterSelectScene" };
        public static bool GameScene()
        {
            if ((Scenes.CurrentName != "") && (!Scenes.MenuNames.Contains(Scenes.CurrentName))) { return true; }
            else { return false; }
        }
    }
}
