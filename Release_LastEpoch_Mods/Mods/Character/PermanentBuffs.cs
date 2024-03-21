using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Character
{
    public class PermanentBuffs
    {
        public static void Update()
        {
            if (Scenes_Manager.GameScene())
            {
                MoveSpeed.Update();
                AttackSpeed.Update();
                CastingSpeed.Update();
                Damage.Update();
                ManaRegen.Update();
                HealthRegen.Update();
                CriticalChance.Update();
                CriticalMultiplier.Update();
                Strength.Update();
                Intelligence.Update();
                Dexterity.Update();
                Attunement.Update();
                Vitality.Update();
            }
        }
        
        public class MoveSpeed
        {
            private static readonly string BuffName = "MoveSpeed_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.MoveSpeed_Buff_Value);                    
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_MoveSpeed_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.Movespeed, 0f, Save_Manager.Data.UserData.Character.PermanentBuffs.MoveSpeed_Buff_Value, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class CastingSpeed
        {
            private static readonly string BuffName = "CastSpeed_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.CastSpeed_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_CastSpeed_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.CastSpeed, 0f, Save_Manager.Data.UserData.Character.PermanentBuffs.CastSpeed_Buff_Value, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class AttackSpeed
        {
            private static readonly string BuffName = "AttackSpeed_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.AttackSpeed_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_AttackSpeed_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.AttackSpeed, 0f, Save_Manager.Data.UserData.Character.PermanentBuffs.AttackSpeed_Buff_Value, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class Damage
        {
            private static readonly string BuffName = "Damage_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.Damage_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Damage_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.Damage, 0f, Save_Manager.Data.UserData.Character.PermanentBuffs.Damage_Buff_Value, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class ManaRegen
        {
            private static readonly string BuffName = "ManaRegen_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.ManaRegen_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_ManaRegen_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.ManaRegen, 0f, Save_Manager.Data.UserData.Character.PermanentBuffs.ManaRegen_Buff_Value, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class HealthRegen
        {
            private static readonly string BuffName = "HealthRegen_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.HealthRegen_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_HealthRegen_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.HealthRegen, 0f, Save_Manager.Data.UserData.Character.PermanentBuffs.HealthRegen_Buff_Value, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class CriticalChance
        {
            private static readonly string BuffName = "CriticalChance_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.CriticalChance_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_CriticalChance_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.CriticalChance, Save_Manager.Data.UserData.Character.PermanentBuffs.CriticalChance_Buff_Value, 0f, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class CriticalMultiplier
        {
            private static readonly string BuffName = "CriticalMultiplier_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.CriticalMultiplier_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_CriticalMultiplier_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.CriticalMultiplier, Save_Manager.Data.UserData.Character.PermanentBuffs.CriticalMultiplier_Buff_Value, 0f, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class Strength
        {
            private static readonly string BuffName = "Strength_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.Str_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Str_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.Strength, Save_Manager.Data.UserData.Character.PermanentBuffs.Str_Buff_Value, 0f, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class Intelligence
        {
            private static readonly string BuffName = "Intelligence_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.Int_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Int_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.Intelligence, Save_Manager.Data.UserData.Character.PermanentBuffs.Int_Buff_Value, 0f, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class Dexterity
        {
            private static readonly string BuffName = "Dexterity_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.Dex_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Dex_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.Dexterity, Save_Manager.Data.UserData.Character.PermanentBuffs.Dex_Buff_Value, 0f, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class Attunement
        {
            private static readonly string BuffName = "Attunement_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.Att_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Att_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.Attunement, Save_Manager.Data.UserData.Character.PermanentBuffs.Att_Buff_Value, 0f, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }
        public class Vitality
        {
            private static readonly string BuffName = "Vitality_Buff";
            public static void Update()
            {
                if (playerActor.IsNullOrDestroyed()) { playerActor = PlayerFinder.getPlayerActor(); }
                if (!playerActor.IsNullOrDestroyed())
                {
                    BuffState state = GetState(BuffName, Save_Manager.Data.UserData.Character.PermanentBuffs.Vit_Buff_Value);
                    if (Save_Manager.Data.UserData.Character.PermanentBuffs.Enable_Vit_Buff)
                    {
                        if ((state.PlayerAlreadyHaveBuff) && (state.ValueChanged))
                        {
                            playerActor.statBuffs.removeBuffsWithName(BuffName);
                            state.PlayerAlreadyHaveBuff = false;
                        }
                        if (!state.PlayerAlreadyHaveBuff)
                        {
                            playerActor.statBuffs.addBuff(255f, SP.Vitality, Save_Manager.Data.UserData.Character.PermanentBuffs.Vit_Buff_Value, 0f, null, AT.None, 0, BuffName);
                        }
                    }
                    else
                    {
                        if (state.PlayerAlreadyHaveBuff) { playerActor.statBuffs.removeBuffsWithName(BuffName); }
                    }
                }
            }
        }

        public struct BuffState
        {
            public bool PlayerAlreadyHaveBuff;
            public bool ValueChanged;
        }

        private static Actor playerActor = null;
        private static BuffState GetState(string BuffName, float value)
        {
            BuffState state = new BuffState
            {
                PlayerAlreadyHaveBuff = false,
                ValueChanged = false
            };

            foreach (Buff player_buff in playerActor.statBuffs.buffs)
            {
                if (player_buff.name == BuffName)
                {
                    state.PlayerAlreadyHaveBuff = true;
                    if (player_buff.stat.increasedValue != value) { state.ValueChanged = true; }
                    break;
                }
            }

            return state;
        }
    }
}
