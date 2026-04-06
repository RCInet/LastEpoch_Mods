using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    [RegisterTypeInIl2Cpp]
    public class Skills_AutoCast : MonoBehaviour
    {
        public static Skills_AutoCast instance { get; private set; }
        public Skills_AutoCast(System.IntPtr ptr) : base(ptr) { }

        const int SlotCount = 5;

        enum InitState { NeedsUpdate, Ready }

        static InitState _state = InitState.NeedsUpdate;
        static PlayerSkill[] _skills = new PlayerSkill[SlotCount];
        static bool _appFocus = true;
        static PlayerChargeManager _chargeManager;
        static UseAbilityProcessor _useAbilityProcessor;
        static Transform _playerTransform;

        void Awake()
        {
            instance = this;
            _state = InitState.NeedsUpdate;
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

        void OnApplicationFocus(bool hasFocus)
        {
            _appFocus = hasFocus;
        }

        public struct PlayerSkill
        {
            public Ability ability;
            public KeyCode key;
            public bool channeled;
            public bool autocast;
            public bool channel_on;
            public bool channeling_active;
        }

        void InitializeSkills()
        {
            _skills = new PlayerSkill[SlotCount];

            if (Refs_Manager.player_treedata.IsNullOrDestroyed()) { _state = InitState.Ready; return; }

            var abilityList = Refs_Manager.player_treedata.playerAbilityList;
            if (abilityList.IsNullOrDestroyed()) { _state = InitState.Ready; return; }

            var actionBarSlots = FindActionBarSlots();

            for (int i = 0; i < SlotCount; i++)
            {
                Ability ability = null;
                try { ability = abilityList.getAbility(i); } catch { }
                if (ability.IsNullOrDestroyed()) continue;

                KeyCode key = KeyCode.None;
                if (actionBarSlots != null && !actionBarSlots[i].IsNullOrDestroyed())
                    key = GetKeyBind(actionBarSlots[i]);

                _skills[i] = new PlayerSkill
                {
                    ability = ability,
                    key = key,
                    channeled = ability.channelled
                };
            }

            _state = InitState.Ready;
        }

        void CacheRefs()
        {
            if (!_chargeManager.IsNullOrDestroyed() &&
                !(_useAbilityProcessor == null || _useAbilityProcessor.IsNullOrDestroyed()) &&
                !_playerTransform.IsNullOrDestroyed())
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

            if (_useAbilityProcessor == null || _useAbilityProcessor.IsNullOrDestroyed())
            {
                if (!Refs_Manager.epoch_input_manager.IsNullOrDestroyed())
                {
                    try { _useAbilityProcessor = Refs_Manager.epoch_input_manager.useAbilityProcessor; }
                    catch { }
                }
            }
        }

        void HandleInput(int i)
        {
#if KEYBOARD
            if (_skills[i].key == KeyCode.None) return;
            if (!Input.GetKeyUp(_skills[i].key)) return;

            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (_skills[i].channeled)
            {
                if (ctrl)
                    _skills[i].channel_on = !_skills[i].channel_on;
                else if (_skills[i].channel_on)
                    _skills[i].channel_on = false;

                if (!_skills[i].channel_on && _skills[i].channeling_active)
                    _skills[i].channeling_active = false;
            }
            else if (ctrl)
            {
                _skills[i].autocast = !_skills[i].autocast;
            }
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

        GameObject[] FindActionBarSlots()
        {
            if (Refs_Manager.game_uibase.IsNullOrDestroyed()) return null;

            Canvas canvas = FindBottomCanvas();
            if (canvas.IsNullOrDestroyed()) return null;

            GameObject inputAbilities = NavigateTo(canvas.gameObject,
                "Bottom Screen UI Holder", "Bottom Screen UI(Clone)",
                "actionBar", "ActionBarMiddle", "Inputs_Abilities");
            if (inputAbilities.IsNullOrDestroyed()) return null;

            return new[]
            {
                Functions.GetChild(inputAbilities, "ActionBarAbility"),
                Functions.GetChild(inputAbilities, "ActionBarAbility (1)"),
                Functions.GetChild(inputAbilities, "ActionBarAbility (2)"),
                Functions.GetChild(inputAbilities, "ActionBarAbility (3)"),
                Functions.GetChild(inputAbilities, "ActionBarAbility (4)")
            };
        }

        Canvas FindBottomCanvas()
        {
            foreach (Canvas canvas in Refs_Manager.game_uibase.canvases)
            {
                if (canvas.name == "Canvas (bottom screen UI)")
                    return canvas;
            }
            return null;
        }

        GameObject NavigateTo(GameObject root, params string[] path)
        {
            GameObject current = root;
            foreach (string name in path)
            {
                current = Functions.GetChild(current, name);
                if (current.IsNullOrDestroyed()) return null;
            }
            return current;
        }

        KeyCode GetKeyBind(GameObject go)
        {
            if (go.IsNullOrDestroyed()) return KeyCode.None;

            GameObject input = Functions.GetChild(go, "Ability Input Character");
            if (input.IsNullOrDestroyed()) return KeyCode.None;

            var keybindText = input.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
            if (keybindText.IsNullOrDestroyed()) return KeyCode.None;

            string text = keybindText.text;
            if (text.Length == 1 && char.IsLetter(text[0]) &&
                System.Enum.TryParse<KeyCode>(text.ToUpper(), out var key))
                return key;

            return KeyCode.None;
        }
    }
}
