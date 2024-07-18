using System.Linq;

namespace LastEpochMods.Managers
{
    public class Scenes_Manager
    {
        public static string CurrentName = "";
        public static string[] MenuNames = { "ClientSplash", "PersistentUI", "Login", "CharacterSelectScene" };
        public static bool GameScene()
        {
            if ((CurrentName != "") && (!MenuNames.Contains(CurrentName))) { return true; }
            else { return false; }
        }
    }
}
