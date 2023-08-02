namespace LastEpochMods.OnSceneChanged
{
    public class Set_Bonuses_List
    {
        private static Il2CppSystem.Collections.Generic.List<SetBonusesList.Entry> backup = null;
        public static void Init()
        {
            if (backup == null) { backup = SetBonusesList.get().entries; }
            if (backup != null)
            {
                if (Config.Data.mods_config.items.Remove_set_req)
                {
                    foreach (SetBonusesList.Entry set_bonuses in SetBonusesList.get().entries)
                    {
                        foreach (SetBonus set_bonus in set_bonuses.mods)
                        {
                            set_bonus.setRequirement = 0;                            
                        }                        
                    }
                }
                else if (SetBonusesList.get().entries != backup)
                {
                    SetBonusesList.get().entries = backup;
                }
            }
        }
    }
}
