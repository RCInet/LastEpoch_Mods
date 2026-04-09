using HarmonyLib;
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
        static UsingAbilityPlayer _abilityState;

        // Guards our own UseBarAbilityCommand calls from being suppressed by the Harmony patch
        static bool _fromAutocast;

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
                if (_state == InitState.Ready)
                    ClearAllToggles();
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

        // Zone transitions (monolith exit, loading screens) should stop all active casts
        void ClearAllToggles()
        {
            for (int i = 0; i < SlotCount; i++)
            {
                _skills[i].autocast = false;
                _skills[i].channel_on = false;
                _skills[i].channeling_active = false;
            }
        }

        // IL2CPP refs can become null mid-frame when the engine destroys objects; re-acquire as needed
        void CacheRefs()
        {
            if (!_chargeManager.IsNullOrDestroyed() &&
                _useAbilityProcessor != null && !_useAbilityProcessor.IsNullOrDestroyed() &&
                !_playerTransform.IsNullOrDestroyed() &&
                _rewiredPlayer != null && !_rewiredPlayer.IsNullOrDestroyed() &&
                !_abilityState.IsNullOrDestroyed())
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
                if (_abilityState.IsNullOrDestroyed())
                {
                    try { _abilityState = Refs_Manager.player_actor.GetComponent<UsingAbilityPlayer>(); }
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
            }
            else if (modifier)
            {
                _skills[i].autocast = !_skills[i].autocast;
            }
        }

        // Rewired GamepadTemplate element ID for d-pad left
        const int DpadLeftElementId = 22;

        static bool IsModifierHeld()
        {
#if KEYBOARD
            return EpochInputManager.CtrlPressed();
#elif WINGAMEPAD
            // Read d-pad left through Rewired's controller API
            // Rewired exposes d-pad as buttons
            if (_rewiredPlayer == null || _rewiredPlayer.IsNullOrDestroyed()) return false;
            try
            {
                var joystick = _rewiredPlayer.controllers.GetLastActiveController(ControllerType.Joystick);
                return joystick != null && joystick.GetButtonById(DpadLeftElementId);
            }
            catch { return false; }
#else
            return false;
#endif
        }

        static void StartAbility(int slot, Vector3 targetPos, Transform hitTransform)
        {
            _fromAutocast = true;
            try { _useAbilityProcessor.UseBarAbilityCommand(true, slot + 1, -1, targetPos, hitTransform, false); }
            catch { }
            finally { _fromAutocast = false; }
        }

        // UseBarAbilityCommand(false) does NOT stop channeling - it only affects buffering.
        // Offline path: UsingAbilityPlayer.stopUsingAbility stops the active channel.
        static void StopChanneling()
        {
            if (_abilityState.IsNullOrDestroyed()) return;
            try { _abilityState.stopUsingAbility(true); }
            catch { }
        }

        void HandleAutoCast(int i, Vector3 targetPos, Transform hitTransform)
        {
            if (!_skills[i].autocast || !_appFocus) return;
            if (IsOnCooldown(_skills[i].ability)) return;

            StartAbility(i, targetPos, hitTransform);
        }

        void HandleChanneling(int i, Vector3 targetPos, Transform hitTransform)
        {
            if (!_skills[i].channel_on)
            {
                if (_skills[i].channeling_active)
                {
                    _skills[i].channeling_active = false;
                    StopChanneling();
                }
                return;
            }

            if (!_appFocus)
            {
                _skills[i].channeling_active = false;
                _skills[i].channel_on = false;
                StopChanneling();
                return;
            }

            // Wait out cooldown without disabling - resume channeling when ready
            if (IsOnCooldown(_skills[i].ability))
                return;

            if (!_skills[i].channeling_active)
            {
                // Start channeling once - the game sustains it internally
                _skills[i].channeling_active = true;
                StartAbility(i, targetPos, hitTransform);
            }
            else
            {
                // Update aim direction while channeling (e.g. Warpath steering)
                try { _abilityState.TryUpdateTargetLocationIfChannelling(targetPos); }
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

        // Block the game's native ability activation when modifier is held,
        // so CTRL+ability only toggles autocast without firing the ability
        [HarmonyPatch(typeof(UseAbilityProcessor), nameof(UseAbilityProcessor.UseBarAbilityCommand))]
        public class UseBarAbilityCommand_Patch
        {
            [HarmonyPrefix]
            static bool Prefix()
            {
                if (_fromAutocast) return true;
                return !IsModifierHeld();
            }
        }
    }
}
