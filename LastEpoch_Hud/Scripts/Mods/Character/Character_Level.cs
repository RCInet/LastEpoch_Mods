namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_Level
    {
        private static readonly int max_level = 100;
        public static void LevelUpOnce()
        {
            if (!Refs_Manager.exp_tracker.IsNullOrDestroyed())
            {
                if (CanLevelUp())
                {
                    Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                    Refs_Manager.exp_tracker.LevelUp(true);
                }
                else { Main.logger_instance.Msg("Hud Manager : Character already max level"); }
            }
        }
        public static void LevelUpToLevel(int level)
        {
            if (!Refs_Manager.exp_tracker.IsNullOrDestroyed())
            {
                if (CanLevelUp())
                {
                    Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                    if (level > max_level) { level = max_level; }
                    for (int i = Refs_Manager.exp_tracker.CurrentLevel; i < level; i++) { Refs_Manager.exp_tracker.LevelUp(true); }
                }
                else { Main.logger_instance.Msg("Hud Manager : Character already max level"); }
            }
        }
        public static void LevelUptoMax()
        {
            if (!Refs_Manager.exp_tracker.IsNullOrDestroyed())
            {
                if (CanLevelUp())
                {
                    Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                    for (int i = Refs_Manager.exp_tracker.CurrentLevel; i < max_level; i++) { Refs_Manager.exp_tracker.LevelUp(true); }
                }
                else { Main.logger_instance.Msg("Hud Manager : Character already max level"); }
            }
        }

        private static bool CanLevelUp()
        {
            bool result = true;
            if (!Refs_Manager.exp_tracker.IsNullOrDestroyed())
            {
                if (Refs_Manager.exp_tracker.CurrentLevel == max_level) { result = false; }
            }

            return result;
        }
    }
}
