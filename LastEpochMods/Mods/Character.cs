using Il2CppSystem.Linq.Expressions;
using LastEpochMods.Hooks;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Character
    {
        private static ExperienceTracker ExpTracker = null;
        private static CharacterDataTracker DataTracker = null;
        private static LocalTreeData TreeData = null;
        private static BaseHealth Health = null;
        
        private static void GetExpTracker()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(ExperienceTracker)))
            {
                if (obj.name == "MainPlayer(Clone)")
                {
                    ExpTracker = obj.TryCast<ExperienceTracker>();
                    break;
                }
            }
        }
        private static void GetDataTracker()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(CharacterDataTracker)))
            {
                if (obj.name == "MainPlayer(Clone)")
                {
                    DataTracker = obj.TryCast<CharacterDataTracker>();
                    break;
                }
            }
        }
        private static void GetLocalTreeData()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(LocalTreeData)))
            {
                if (obj.name == "MainPlayer(Clone)")
                {
                    TreeData = obj.TryCast<LocalTreeData>();
                    break;
                }
            }
        }
        private static void GetBaseHealth()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(BaseHealth)))
            {
                if (obj.name == "MainPlayer(Clone)")
                {
                    Health = obj.TryCast<BaseHealth>();
                    break;
                }
            }
        }

        public static void LevelUp()
        {
            if (ExpTracker == null) { GetExpTracker(); }
            if (ExpTracker != null) { ExpTracker.LevelUp(true); }            
        }
        public static bool GetIsMastered()
        {
            bool result = false;
            if (TreeData == null) { GetLocalTreeData(); }
            if (TreeData != null)
            {
                if (TreeData.chosenMastery > 0) { result = true; }
            }

            return result;
        }
        public static void ResetMasterie()
        {
            if (DataTracker == null) { GetDataTracker(); }
            if (DataTracker != null)
            {
                DataTracker.charData.ChosenMastery = 0;
                DataTracker.charData.SaveData();
            }
            else { Main.logger_instance.Msg("Error DataTracker is null"); }
            if (TreeData == null) { GetLocalTreeData(); }
            if (TreeData != null) { TreeData.chosenMastery = 0; }
            else { Main.logger_instance.Msg("Error TreeData is null"); }
            UI.MasterieUI.Show();
        }        
        public static void GodMode()
        {
            if (Health == null) { GetBaseHealth(); }
            if (Health != null)
            {
                bool result = false;
                if (Config.Data.mods_config.character.characterstats.Enable_GodMode) { result = true; }
                Health.damageable = result;
                Health.canDie = result;
                Config.Data.mods_config.character.characterstats.Enable_GodMode = !Config.Data.mods_config.character.characterstats.Enable_GodMode;
            }
        }
    }
}
