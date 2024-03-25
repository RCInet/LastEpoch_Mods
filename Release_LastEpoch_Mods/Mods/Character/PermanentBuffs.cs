using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Character
{
    public class PermanentBuffs
    {        
        public struct PermanentBuff
        {
            public string Name;
            public bool Toggle;
            public float Value;
            public SP Propertie;
            public Buff_Type Type;
        }
        public enum Buff_Type
        {
            Add,
            Increase
        }

        public static void OnSceneWasLoaded()
        {
            Enable = InitBuffs();
            Running = false;
        }
        public static void Update()
        {
            if ((Enable) && (!Running)) { StartBuffs(); }
            else if (Running)
            {
                System.TimeSpan elaspedTime = System.DateTime.Now - StartTime;
                System.Double seconds = elaspedTime.TotalSeconds;
                if (seconds > (Buff_Duration - 1)) { Running = false; }
                else if (!GUI_Manager.PauseMenu.Refs.PauseMenu.IsNullOrDestroyed())
                {
                    if ((PauseMenu) && (!GUI_Manager.PauseMenu.Refs.PauseMenu.active))
                    {
                        Enable = InitBuffs();
                        Running = false;
                        if (!Enable) { RemoveBuffs(); }
                    }
                    PauseMenu = GUI_Manager.PauseMenu.Refs.PauseMenu.active;
                }
            }
            else if (!GUI_Manager.PauseMenu.Refs.PauseMenu.IsNullOrDestroyed()) //When disable
            {
                if ((PauseMenu) && (!GUI_Manager.PauseMenu.Refs.PauseMenu.active))
                {
                    Enable = InitBuffs();
                }
                PauseMenu = GUI_Manager.PauseMenu.Refs.PauseMenu.active;
            }
        }

        private static bool Enable = false;
        private static bool Running = false;
        private static bool Starting = false;
        private static System.Collections.Generic.List<PermanentBuff> Buffs;
        private static System.DateTime StartTime;
        private static readonly float Buff_Duration = 255f;
        private static bool PauseMenu = false;
        private static Actor playerActor;

        private static bool InitBuffs()
        {
            Buffs = new System.Collections.Generic.List<PermanentBuff>
            {
                new PermanentBuff
                {
                    Name = "MoveSpeed_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.MoveSpeed_Buff_Value,
                    Propertie = SP.Movespeed,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_MoveSpeed_Buff
                },
                new PermanentBuff
                {
                    Name = "CastSpeed_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.CastSpeed_Buff_Value,
                    Propertie = SP.CastSpeed,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_CastSpeed_Buff
                },
                new PermanentBuff
                {
                    Name = "AttackSpeed_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.AttackSpeed_Buff_Value,
                    Propertie = SP.AttackSpeed,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_AttackSpeed_Buff
                },
                new PermanentBuff
                {
                    Name = "Damage_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.Damage_Buff_Value,
                    Propertie = SP.Damage,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Damage_Buff
                },
                new PermanentBuff
                {
                    Name = "ManaRegen_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.ManaRegen_Buff_Value,
                    Propertie = SP.ManaRegen,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_ManaRegen_Buff
                },
                new PermanentBuff
                {
                    Name = "HealthRegen_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.HealthRegen_Buff_Value,
                    Propertie = SP.HealthRegen,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_HealthRegen_Buff
                },
                new PermanentBuff
                {
                    Name = "CriticalChance_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.CriticalChance_Buff_Value,
                    Propertie = SP.CriticalChance,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_CriticalChance_Buff
                },
                new PermanentBuff
                {
                    Name = "CriticalMultiplier_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.CriticalMultiplier_Buff_Value,
                    Propertie = SP.CriticalMultiplier,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_CriticalMultiplier_Buff
                },
                new PermanentBuff
                {
                    Name = "Strength_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.Str_Buff_Value,
                    Propertie = SP.Strength,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Str_Buff
                },
                new PermanentBuff
                {
                    Name = "Intelligence_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.Int_Buff_Value,
                    Propertie = SP.Intelligence,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Int_Buff
                },
                new PermanentBuff
                {
                    Name = "Dexterity_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.Dex_Buff_Value,
                    Propertie = SP.Dexterity,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Dex_Buff
                },
                new PermanentBuff
                {
                    Name = "Attunement_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.Att_Buff_Value,
                    Propertie = SP.Attunement,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Att_Buff
                },
                new PermanentBuff
                {
                    Name = "Vitality_Buff",
                    Value = Save_Manager.Data.UserData.Character.PermanentBuffs.Vit_Buff_Value,
                    Propertie = SP.Vitality,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Vit_Buff
                },
            };
            bool result = false;
            foreach (PermanentBuff p in Buffs)
            {
                if (p.Toggle) { result = true; break; }
            }

            return result;
        }
        private static void StartBuffs()
        {
            if (!Starting)
            {
                Starting = true;
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    System.Collections.Generic.List<string> player_buffs = new System.Collections.Generic.List<string>();
                    foreach (Buff player_buff in playerActor.statBuffs.buffs)
                    {
                        player_buffs.Add(player_buff.name);
                    }
                    foreach (PermanentBuff permanent_buff in Buffs)
                    {
                        if (player_buffs.Contains(permanent_buff.Name)) { playerActor.statBuffs.removeBuffsWithName(permanent_buff.Name); }
                        if (permanent_buff.Toggle)
                        {
                            float add = 0;
                            float increase = 0;
                            if (permanent_buff.Type == Buff_Type.Add) { add = permanent_buff.Value; }
                            else { increase = permanent_buff.Value; }
                            playerActor.statBuffs.addBuff(Buff_Duration, permanent_buff.Propertie, add, increase, null, AT.None, 0, permanent_buff.Name);
                        }
                    }
                    StartTime = System.DateTime.Now;
                    Running = true;
                }
                Starting = false;
            }
        }
        private static void RemoveBuffs()
        {
            if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
            if (!playerActor.IsNullOrDestroyed())
            {
                System.Collections.Generic.List<string> player_buffs = new System.Collections.Generic.List<string>();
                foreach (Buff player_buff in playerActor.statBuffs.buffs)
                {
                    player_buffs.Add(player_buff.name);
                }
                foreach (PermanentBuff permanent_buff in Buffs)
                {
                    if (player_buffs.Contains(permanent_buff.Name)) { playerActor.statBuffs.removeBuffsWithName(permanent_buff.Name); }
                }
            }
        }
    }
}
