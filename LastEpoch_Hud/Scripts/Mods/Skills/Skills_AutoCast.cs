using Il2Cpp;
using MelonLoader;
using Il2CppRewired;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    [RegisterTypeInIl2Cpp]
    public class Skills_AutoCast : MonoBehaviour
    {
        public static Skills_AutoCast instance { get; private set; }
        public Skills_AutoCast(System.IntPtr ptr) : base(ptr) { }

        const int SlotCount = 5;
        // Rewired action IDs for ability bar slots 1-5; IDs 4,5 are skipped in the game's mapping
        static readonly int[] AbilityActionIds = { 1, 2, 3, 6, 7 };

        public struct PlayerSkill
        {
            public Ability ability;
            public bool channeled;
            public bool autocast;
            public bool channel_on;
            public bool channeling_active;
        }

        enum InitState { NeedsUpdate, Ready }

        static InitState _state = InitState.NeedsUpdate;
        static PlayerSkill[] _skills = new PlayerSkill[SlotCount];
        static bool _appFocus = true;
        static PlayerChargeManager _chargeManager;
        static UseAbilityProcessor _useAbilityProcessor;
        static Transform _playerTransform;
        static Player _rewiredPlayer;

        void Awake()
        {
            instance = this;
            _state = InitState.NeedsUpdate;
        }

        // Sets guard to stop casting/channeling when the game loses focus
        void OnApplicationFocus(bool hasFocus)
        {
            _appFocus = hasFocus;
        }

        void Update()
        {
            if (!Scenes.IsGameScene())
            {
                _state = InitState.NeedsUpdate;
                return;
            }

            if (_state == InitState.NeedsUpdate)
                InitializeSkills();

            if (_state != InitState.Ready)
                return;

            CacheRefs();
            if (_useAbilityProcessor == null || _useAbilityProcessor.IsNullOrDestroyed())
                return;

            bool anyActive = false;
            for (int i = 0; i < SlotCount; i++)
            {
                if (_skills[i].ability.IsNullOrDestroyed())
                    continue;

                HandleInput(i);
                if (_skills[i].autocast || _skills[i].channel_on || _skills[i].channeling_active)
                    anyActive = true;
            }

            // We skip raycasting when nothing needs it so we don't call GetTargetPosition unnecessarily
            if (!anyActive)
                return;

            Vector3 targetPos = GetTargetPosition(out Transform hitTransform);

            for (int i = 0; i < SlotCount; i++)
            {
                if (_skills[i].ability.IsNullOrDestroyed())
                    continue;

                HandleAutoCast(i, targetPos, hitTransform);
                HandleChanneling(i, targetPos, hitTransform);
            }
        }

        void InitializeSkills()
        {
            if (Refs_Manager.player_treedata.IsNullOrDestroyed()) return;

            var abilityList = Refs_Manager.player_treedata.playerAbilityList;
            if (abilityList.IsNullOrDestroyed()) return;

            _skills = new PlayerSkill[SlotCount];

            for (int i = 0; i < SlotCount; i++)
            {
                Ability ability = null;
                try { ability = abilityList.getAbility(i); } catch { }
                if (ability.IsNullOrDestroyed()) continue;

                _skills[i] = new PlayerSkill
                {
                    ability = ability,
                    channeled = ability.channelled
                };
            }

            _state = InitState.Ready;
        }

        void CacheRefs()
        {
            if (!_chargeManager.IsNullOrDestroyed() &&
                _useAbilityProcessor != null && !_useAbilityProcessor.IsNullOrDestroyed() &&
                !_playerTransform.IsNullOrDestroyed() &&
                _rewiredPlayer != null && !_rewiredPlayer.IsNullOrDestroyed())
                return;

            if (!Refs_Manager.player_actor.IsNullOrDestroyed())
            {
                if (_chargeManager.IsNullOrDestroyed())
                {
                    try { _chargeManager = Refs_Manager.player_actor.GetComponent<PlayerChargeManager>(); }
                    catch { }
                }
                if (_playerTransform.IsNullOrDestroyed())
                {
                    try { _playerTransform = Refs_Manager.player_actor.transform; }
                    catch { }
                }
            }

            if (!Refs_Manager.epoch_input_manager.IsNullOrDestroyed())
            {
                if (_useAbilityProcessor == null || _useAbilityProcessor.IsNullOrDestroyed())
                {
                    try { _useAbilityProcessor = Refs_Manager.epoch_input_manager.useAbilityProcessor; }
                    catch { }
                }
                if (_rewiredPlayer == null || _rewiredPlayer.IsNullOrDestroyed())
                {
                    try { _rewiredPlayer = Refs_Manager.epoch_input_manager.rewiredPlayer; }
                    catch { }
                }
            }
        }

        void HandleInput(int i)
        {
            if (_rewiredPlayer == null || _rewiredPlayer.IsNullOrDestroyed()) return;
            if (!_rewiredPlayer.GetButtonUp(AbilityActionIds[i])) return;

            bool modifier = IsModifierHeld();

            if (_skills[i].channeled)
            {
                if (modifier)
                    _skills[i].channel_on = !_skills[i].channel_on;
                else if (_skills[i].channel_on)
                    _skills[i].channel_on = false;

                if (!_skills[i].channel_on && _skills[i].channeling_active)
                    _skills[i].channeling_active = false;
            }
            else if (modifier)
            {
                _skills[i].autocast = !_skills[i].autocast;
            }
        }

        static bool IsModifierHeld()
        {
#if KEYBOARD
            return EpochInputManager.CtrlPressed();
#elif WINGAMEPAD
            // LB - only buttons 0-5 (A,B,X,Y,LB,RB) work reliably as KeyCode on Xbox controllers
            return Input.GetKey(KeyCode.Joystick1Button4);
#else
            return false;
#endif
        }

        void HandleAutoCast(int i, Vector3 targetPos, Transform hitTransform)
        {
            if (!_skills[i].autocast || !_appFocus) return;
            if (IsOnCooldown(_skills[i].ability)) return;

            try { _useAbilityProcessor.UseBarAbilityCommand(true, i + 1, -1, targetPos, hitTransform, false); }
            catch { }
        }

        void HandleChanneling(int i, Vector3 targetPos, Transform hitTransform)
        {
            if (!_skills[i].channel_on)
            {
                if (_skills[i].channeling_active)
                {
                    _skills[i].channeling_active = false;
                    try { _useAbilityProcessor.UseBarAbilityCommand(false, i + 1, -1, targetPos, hitTransform, false); }
                    catch { }
                }
                return;
            }

            if (!_skills[i].channeling_active)
            {
                _skills[i].channeling_active = true;
                try { _useAbilityProcessor.UseBarAbilityCommand(true, i + 1, -1, targetPos, hitTransform, false); }
                catch { }
            }
            else if (!_appFocus)
            {
                _skills[i].channeling_active = false;
                _skills[i].channel_on = false;
                try { _useAbilityProcessor.UseBarAbilityCommand(false, i + 1, -1, targetPos, hitTransform, false); }
                catch { }
            }
        }

        bool IsOnCooldown(Ability ability)
        {
            if (_chargeManager.IsNullOrDestroyed()) return false;
            try { return _chargeManager.onCoooldown(ability); }
            catch { return false; }
        }

        // Works for both mouse and gamepad - gamepad uses a virtual cursor that drives the same screen position
        static Vector3 GetTargetPosition(out Transform hitTransform)
        {
            hitTransform = null;
            if (_playerTransform.IsNullOrDestroyed())
                return Vector3.zero;

            try
            {
                Vector3 pos = MouseManager.AbilityUseMousePoint(
                    out hitTransform, out _, false, _playerTransform);
                return pos;
            }
            catch
            {
                return _playerTransform.position;
            }
        }
    }
}
