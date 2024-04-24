using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    [RegisterTypeInIl2Cpp]
    public class Character_PermanentBuffs : MonoBehaviour
    {
        public static Character_PermanentBuffs instance { get; private set; }
        public Character_PermanentBuffs(System.IntPtr ptr) : base(ptr) { }        
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

        void Awake()
        {
            instance = this;
            SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
        }
        void Update()
        {
            if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!force_disable))
            {
                if ((Buff_Enable) && (!Running)) { StartBuffs(); }
                else if (Running)
                {
                    System.TimeSpan elaspedTime = System.DateTime.Now - StartTime;
                    System.Double seconds = elaspedTime.TotalSeconds;
                    if (seconds > (Buff_Duration - 1)) { Running = false; }
                    else
                    {
                        if ((PauseMenu) && (!HudIsOpen()))
                        {
                            Buff_Enable = InitBuffs();
                            Running = false;
                            if (!Buff_Enable) { RemoveBuffs(); }
                        }
                        PauseMenu = HudIsOpen();
                    }
                }
                else
                {
                    if ((PauseMenu) && (!HudIsOpen())) { Buff_Enable = InitBuffs(); }
                    PauseMenu = HudIsOpen();
                }
            }
            else
            {
                Running = false;
                Buff_Enable = false;
                RemoveBuffs();
                if (force_disable) { this.gameObject.active = false; }
            }
        }        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!Save_Manager.instance.IsNullOrDestroyed()) { Buff_Enable = InitBuffs(); }
            else { Buff_Enable = false; }
            Running = false;
        }
        bool HudIsOpen()
        {
            if (!Hud_Manager.hud_object.IsNullOrDestroyed()) { return Hud_Manager.hud_object.active; }
            else { return false; }
        }        
          
        public void Enable()
        {
            if (!Save_Manager.instance.IsNullOrDestroyed())
            {
                if (Save_Manager.instance.data.Character.PermanentBuffs.Enable_Mod)
                {
                    force_disable = false;
                    this.gameObject.active = true;
                }
                else { Disable(); }
            }
        }
        public void Disable()
        {
            force_disable = true;
        }

        private bool force_disable = false;
        private bool Buff_Enable = false;
        private bool Running = false;
        private bool Starting = false;
        private System.Collections.Generic.List<PermanentBuff> Buffs;
        private System.DateTime StartTime;
        private readonly float Buff_Duration = 255f;
        private bool PauseMenu = false;

        private bool InitBuffs()
        {
            bool result = false;
            Buffs = new System.Collections.Generic.List<PermanentBuff>
            {
                new PermanentBuff
                {
                    Name = "MoveSpeed_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.MoveSpeed_Buff_Value,
                    Propertie = SP.Movespeed,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_MoveSpeed_Buff
                },
                new PermanentBuff
                {
                    Name = "CastSpeed_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.CastSpeed_Buff_Value,
                    Propertie = SP.CastSpeed,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_CastSpeed_Buff
                },
                new PermanentBuff
                {
                    Name = "AttackSpeed_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.AttackSpeed_Buff_Value,
                    Propertie = SP.AttackSpeed,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_AttackSpeed_Buff
                },
                new PermanentBuff
                {
                    Name = "Damage_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.Damage_Buff_Value,
                    Propertie = SP.Damage,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Damage_Buff
                },
                new PermanentBuff
                {
                    Name = "ManaRegen_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.ManaRegen_Buff_Value,
                    Propertie = SP.ManaRegen,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_ManaRegen_Buff
                },
                new PermanentBuff
                {
                    Name = "HealthRegen_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.HealthRegen_Buff_Value,
                    Propertie = SP.HealthRegen,
                    Type = Buff_Type.Increase,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_HealthRegen_Buff
                },
                new PermanentBuff
                {
                    Name = "CriticalChance_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.CriticalChance_Buff_Value,
                    Propertie = SP.CriticalChance,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_CriticalChance_Buff
                },
                new PermanentBuff
                {
                    Name = "CriticalMultiplier_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.CriticalMultiplier_Buff_Value,
                    Propertie = SP.CriticalMultiplier,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_CriticalMultiplier_Buff
                },
                new PermanentBuff
                {
                    Name = "Strength_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.Str_Buff_Value,
                    Propertie = SP.Strength,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Str_Buff
                },
                new PermanentBuff
                {
                    Name = "Intelligence_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.Int_Buff_Value,
                    Propertie = SP.Intelligence,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Int_Buff
                },
                new PermanentBuff
                {
                    Name = "Dexterity_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.Dex_Buff_Value,
                    Propertie = SP.Dexterity,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Dex_Buff
                },
                new PermanentBuff
                {
                    Name = "Attunement_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.Att_Buff_Value,
                    Propertie = SP.Attunement,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Att_Buff
                },
                new PermanentBuff
                {
                    Name = "Vitality_Buff",
                    Value = Save_Manager.instance.data.Character.PermanentBuffs.Vit_Buff_Value,
                    Propertie = SP.Vitality,
                    Type = Buff_Type.Add,
                    Toggle = Save_Manager.instance.data.Character.PermanentBuffs.Enable_Vit_Buff
                },
            };

            foreach (PermanentBuff p in Buffs)
            {
                if (p.Toggle) { result = true; break; }
            }
            
            return result;
        }
        private void StartBuffs()
        {
            if (!Starting)
            {
                Starting = true;
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {
                    System.Collections.Generic.List<string> player_buffs = new System.Collections.Generic.List<string>();
                    foreach (Buff player_buff in Refs_Manager.player_actor.statBuffs.buffs)
                    {
                        player_buffs.Add(player_buff.name);
                    }
                    foreach (PermanentBuff permanent_buff in Buffs)
                    {
                        if (player_buffs.Contains(permanent_buff.Name)) { Refs_Manager.player_actor.statBuffs.removeBuffsWithName(permanent_buff.Name); }
                        if (permanent_buff.Toggle)
                        {
                            float add = 0;
                            float increase = 0;
                            if (permanent_buff.Type == Buff_Type.Add) { add = permanent_buff.Value; }
                            else { increase = permanent_buff.Value; }
                            Refs_Manager.player_actor.statBuffs.addBuff(Buff_Duration, permanent_buff.Propertie, add, increase, null, AT.None, 0, permanent_buff.Name);
                        }
                    }
                    StartTime = System.DateTime.Now;
                    Running = true;
                }
                Starting = false;
            }
        }
        private void RemoveBuffs()
        {
            if (!Refs_Manager.player_actor.IsNullOrDestroyed())
            {
                System.Collections.Generic.List<string> player_buffs = new System.Collections.Generic.List<string>();
                foreach (Buff player_buff in Refs_Manager.player_actor.statBuffs.buffs)
                {
                    player_buffs.Add(player_buff.name);
                }
                foreach (PermanentBuff permanent_buff in Buffs)
                {
                    if (player_buffs.Contains(permanent_buff.Name)) { Refs_Manager.player_actor.statBuffs.removeBuffsWithName(permanent_buff.Name); }
                }
            }
        }
    }
}
