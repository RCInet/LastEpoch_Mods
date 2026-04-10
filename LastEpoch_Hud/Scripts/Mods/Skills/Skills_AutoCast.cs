using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using Il2CppRewired;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    [RegisterTypeInIl2Cpp]
    public class Skills_AutoCast : MonoBehaviour
    {
        public static Skills_AutoCast instance { get; private set; }
        public Skills_AutoCast(System.IntPtr ptr) : base(ptr) { }

        const int SlotCount = 5;
        const int HumanFormKey = -1;
        // Rewired action IDs for ability bar slots 1-5; IDs 4,5 are skipped in the game's mapping
        static readonly int[] AbilityActionIds = { 1, 2, 3, 6, 7 };
        const float PressThrottleSeconds = 0.1f;
        public struct PlayerSkill
        {
            public Ability ability;
            public bool channeled;
            public bool autocastOn;
            public bool held;
            public float nextPressTime;
        }

        enum InitState { NeedsUpdate, Ready }

        static InitState state = InitState.NeedsUpdate;
        static PlayerSkill[] skills = new PlayerSkill[SlotCount];
        static bool appFocus = true;
        static PlayerChargeManager chargeManager;
        static UseAbilityProcessor useAbilityProcessor;
        static Transform playerTransform;
        static Player rewiredPlayer;
        static UsingAbilityPlayer abilityState;
        static int lastFormKey = HumanFormKey;
        static readonly Dictionary<int, bool[]> formStates = new Dictionary<int, bool[]>();
        static string lastSceneName = "";
        static bool fromAutocast;

        void Awake()
        {
            instance = this;
            state = InitState.NeedsUpdate;
        }

        void OnApplicationFocus(bool hasFocus)
        {
            appFocus = hasFocus;
        }

        void Update()
        {
            if (!Scenes.IsGameScene())
            {
                if (state == InitState.Ready)
                    ClearAllToggles();
                ResetCachedRefs();
                state = InitState.NeedsUpdate;
                lastSceneName = "";
                return;
            }

            if (Scenes.SceneName != lastSceneName)
            {
                lastSceneName = Scenes.SceneName;
                ClearAllToggles();
                state = InitState.NeedsUpdate;
            }

            if (state == InitState.NeedsUpdate)
                InitializeSkills();

            if (state != InitState.Ready)
                return;

            CacheRefs();
            if (useAbilityProcessor == null || useAbilityProcessor.IsNullOrDestroyed())
                return;

            CheckFormChange();
            if (state != InitState.Ready)
                return;

            bool anyActive = false;
            bool anyHeld = false;
            for (int i = 0; i < SlotCount; i++)
            {
                if (skills[i].ability.IsNullOrDestroyed())
                    continue;

                HandleInput(i);
                if (skills[i].autocastOn) anyActive = true;
                if (skills[i].held) anyHeld = true;
            }

            if (!anyActive && !anyHeld)
                return;

            Vector3 targetPos = GetTargetPosition(out Transform hitTransform);

            int manualHoldIndex = SlotCount;
            if (rewiredPlayer != null && !rewiredPlayer.IsNullOrDestroyed())
            {
                for (int i = 0; i < SlotCount; i++)
                {
                    if (skills[i].ability.IsNullOrDestroyed()) continue;
                    if (rewiredPlayer.GetButton(AbilityActionIds[i])) { manualHoldIndex = i; break; }
                }
            }

            bool firedRisingEdge = false;
            for (int i = 0; i < SlotCount; i++)
            {
                if (skills[i].ability.IsNullOrDestroyed())
                    continue;

                HandleAutocastSlot(i, manualHoldIndex, targetPos, hitTransform, ref firedRisingEdge);
            }

            UpdateIconShine();
        }

        void InitializeSkills()
        {
            if (Refs_Manager.player_treedata.IsNullOrDestroyed()) return;

            var abilityList = Refs_Manager.player_treedata.playerAbilityList;
            if (abilityList.IsNullOrDestroyed()) return;

            skills = new PlayerSkill[SlotCount];

            for (int i = 0; i < SlotCount; i++)
            {
                Ability ability = null;
                try { ability = abilityList.getAbility(i); } catch { }
                if (ability.IsNullOrDestroyed()) continue;

                skills[i] = new PlayerSkill
                {
                    ability = ability,
                    channeled = ability.channelled
                };
            }

            RestoreFormState(lastFormKey);
            state = InitState.Ready;
        }

        static void ClearAllToggles()
        {
            formStates.Clear();
            lastFormKey = HumanFormKey;
            for (int i = 0; i < SlotCount; i++)
            {
                skills[i].autocastOn = false;
                skills[i].held = false;
                skills[i].nextPressTime = 0f;
            }
        }

        // Form swaps the ability bar (Werebear/Spriggan/Swarmblade/Reaper).
        // Saves the old form's toggles, re-inits, and restores the new form's saved toggles.
        void CheckFormChange()
        {
            if (Refs_Manager.player_treedata.IsNullOrDestroyed()) return;
            var abilityList = Refs_Manager.player_treedata.playerAbilityList;
            if (abilityList.IsNullOrDestroyed()) return;

            int currentKey = GetCurrentFormKey(abilityList);
            if (currentKey == lastFormKey) return;

            for (int i = 0; i < SlotCount; i++)
            {
                if (skills[i].held && skills[i].channeled)
                    StopChannel(i);
            }

            SaveFormState(lastFormKey);
            lastFormKey = currentKey;
            state = InitState.NeedsUpdate;
            InitializeSkills();
        }

        static int GetCurrentFormKey(PlayerAbilityList abilityList)
        {
            try
            {
                var id = abilityList.TransformAbilityId;
                if (id.HasValue) return id.Value;
            }
            catch { }
            return HumanFormKey;
        }

        static void SaveFormState(int key)
        {
            if (!formStates.TryGetValue(key, out var saved))
            {
                saved = new bool[SlotCount];
                formStates[key] = saved;
            }
            for (int i = 0; i < SlotCount; i++)
                saved[i] = skills[i].autocastOn;
        }

        static void RestoreFormState(int key)
        {
            if (!formStates.TryGetValue(key, out var saved)) return;
            for (int i = 0; i < SlotCount; i++)
                skills[i].autocastOn = saved[i];
        }

        static void ResetCachedRefs()
        {
            chargeManager = null;
            useAbilityProcessor = null;
            playerTransform = null;
            rewiredPlayer = null;
            abilityState = null;
        }

        void CacheRefs()
        {
            if (!chargeManager.IsNullOrDestroyed() &&
                useAbilityProcessor != null && !useAbilityProcessor.IsNullOrDestroyed() &&
                !playerTransform.IsNullOrDestroyed() &&
                rewiredPlayer != null && !rewiredPlayer.IsNullOrDestroyed() &&
                !abilityState.IsNullOrDestroyed())
                return;

            if (!Refs_Manager.player_actor.IsNullOrDestroyed())
            {
                if (chargeManager.IsNullOrDestroyed())
                {
                    try { chargeManager = Refs_Manager.player_actor.GetComponent<PlayerChargeManager>(); }
                    catch { }
                }
                if (playerTransform.IsNullOrDestroyed())
                {
                    try { playerTransform = Refs_Manager.player_actor.transform; }
                    catch { }
                }
                if (abilityState.IsNullOrDestroyed())
                {
                    try { abilityState = Refs_Manager.player_actor.GetComponent<UsingAbilityPlayer>(); }
                    catch { }
                }
            }

            if (!Refs_Manager.epoch_input_manager.IsNullOrDestroyed())
            {
                if (useAbilityProcessor == null || useAbilityProcessor.IsNullOrDestroyed())
                {
                    try { useAbilityProcessor = Refs_Manager.epoch_input_manager.useAbilityProcessor; }
                    catch { }
                }
                if (rewiredPlayer == null || rewiredPlayer.IsNullOrDestroyed())
                {
                    try { rewiredPlayer = Refs_Manager.epoch_input_manager.rewiredPlayer; }
                    catch { }
                }
            }
        }

        void HandleInput(int i)
        {
            if (rewiredPlayer == null || rewiredPlayer.IsNullOrDestroyed()) return;
            if (!rewiredPlayer.GetButtonDown(AbilityActionIds[i])) return;
            if (!IsModifierHeld()) return;

            skills[i].autocastOn = !skills[i].autocastOn;
        }

        static bool IsModifierHeld()
        {
#if KEYBOARD
            return EpochInputManager.CtrlPressed();
#elif WINGAMEPAD
            if (rewiredPlayer == null || rewiredPlayer.IsNullOrDestroyed()) return false;
            try
            {
                var joystick = rewiredPlayer.controllers.GetLastActiveController(ControllerType.Joystick);
                if (joystick == null) return false;

                var template = joystick.GetTemplate<GamepadTemplate>();
                if (template == null) return false;

                var dpad = template.Cast<IGamepadTemplate>().dPad;
                if (dpad == null) return false;

                return dpad.left.value;
            }
            catch { return false; }
#else
            return false;
#endif
        }

        static void SendBarCommand(int slot, bool keyDown, Vector3 targetPos, Transform hitTransform)
        {
            fromAutocast = true;
            try { useAbilityProcessor.UseBarAbilityCommand(keyDown, slot + 1, -1, targetPos, hitTransform, false); }
            catch { }
            finally { fromAutocast = false; }
        }


        static void StopChannel(int slot)
        {
            try
            {
                if (useAbilityProcessor != null && !useAbilityProcessor.IsNullOrDestroyed())
                {
                    var state = useAbilityProcessor.PlayerAbilityState;
                    if (state != null)
                    {
                        var clientState = state.TryCast<ClientUsingAbilityState>();
                        if (clientState != null)
                        {
                            clientState.AttemptToStopAbility(slot + 1, true);
                            return;
                        }
                    }
                }
            }
            catch { }
            if (abilityState.IsNullOrDestroyed()) return;
            try { abilityState.stopUsingAbility(true); }
            catch { }
        }

        bool IsOnCooldown(Ability ability)
        {
            if (chargeManager.IsNullOrDestroyed()) return false;
            try { return chargeManager.onCoooldown(ability); }
            catch { return false; }
        }

        void HandleAutocastSlot(int i, int manualHoldIndex, Vector3 targetPos, Transform hitTransform, ref bool firedRisingEdge)
        {
            ref var s = ref skills[i];

            if (!s.autocastOn)
            {
                if (s.held)
                {
                    s.held = false;
                    if (s.channeled)
                        StopChannel(i);
                }
                return;
            }

            if (!appFocus) return;

            if (i >= manualHoldIndex) return;

            bool channelling = s.channeled && !abilityState.IsNullOrDestroyed() && abilityState.channelling;
            bool onCd = IsOnCooldown(s.ability);
            bool throttled = Time.time < s.nextPressTime;
            bool readyToPress = !channelling && !onCd && !throttled;

            if (readyToPress)
            {
                if (firedRisingEdge)
                    return;

                SendBarCommand(i, true, targetPos, hitTransform);
                firedRisingEdge = true;
                s.held = true;
                s.nextPressTime = Time.time + PressThrottleSeconds;
            }
            else
            {
                SendBarCommand(i, false, targetPos, hitTransform);
            }

            if (channelling)
            {
                try { abilityState.TryUpdateTargetLocationIfChannelling(targetPos); }
                catch { }
            }
        }

        // Works for both mouse and gamepad (virtual cursor drives same screen position)
        static Vector3 GetTargetPosition(out Transform hitTransform)
        {
            hitTransform = null;
            if (playerTransform.IsNullOrDestroyed())
                return Vector3.zero;

            try
            {
                Vector3 pos = MouseManager.AbilityUseMousePoint(
                    out hitTransform, out _, false, playerTransform);
                return pos;
            }
            catch
            {
                return playerTransform.position;
            }
        }

        static void UpdateIconShine()
        {
            var icons = AbilityBarIcon.all;
            if (icons == null) return;

            float t = (Mathf.Sin(Time.time * 5f) + 1f) * 0.5f;
            float scale = Mathf.Lerp(0.92f, 1.07f, t);
            float green = Mathf.Lerp(1f, 0.75f, t);
            float blue = Mathf.Lerp(1f, 0.4f, t);
            var tint = new Color(1f, green, blue, 1f);

            for (int i = 0; i < icons.Count; i++)
            {
                var barIcon = icons[i];
                if (barIcon.IsNullOrDestroyed()) continue;

                int slot = barIcon.abilityNumber - 1;
                bool active = slot >= 0 && slot < SlotCount && skills[slot].autocastOn;

                barIcon.transform.localScale = active
                    ? new Vector3(scale, scale, 1f)
                    : Vector3.one;
                barIcon.icon.color = active ? tint : Color.white;
            }
        }

        [HarmonyPatch(typeof(UseAbilityProcessor), nameof(UseAbilityProcessor.UseBarAbilityCommand))]
        public class UseBarAbilityCommand_Patch
        {
            [HarmonyPrefix]
            static bool Prefix()
            {
                if (fromAutocast) return true;
                return !IsModifierHeld();
            }
        }

        [HarmonyPatch(typeof(PlayerVoiceEventManager), nameof(PlayerVoiceEventManager.playVoiceLine))]
        public class PlayVoiceLine_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(PlayerVoiceEventManager.VoiceLineEvent voiceLineEvent)
            {
                if (!fromAutocast) return true;
                return voiceLineEvent != PlayerVoiceEventManager.VoiceLineEvent.CannotUseSkill;
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "initialise")]
        public class MonolithZoneInit_Patch
        {
            [HarmonyPostfix]
            static void Postfix()
            {
                ClearAllToggles();
                state = InitState.NeedsUpdate;
            }
        }

        [HarmonyPatch(typeof(MonolithRunsManager), nameof(MonolithRunsManager.onRestZoneEnteredAfterEchoCompleted))]
        public class MonolithRestEnter_Patch
        {
            [HarmonyPostfix]
            static void Postfix()
            {
                ClearAllToggles();
                state = InitState.NeedsUpdate;
            }
        }
    }
}
