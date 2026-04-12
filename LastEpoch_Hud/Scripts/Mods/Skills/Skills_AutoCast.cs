using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using Il2CppRewired;
using LastEpoch_Hud.Scripts.ModUI;
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
        // Per-form autocast intent. Survives scene changes and form swaps, so the
        // player's setup for every form they've visited comes back on resume.
        static readonly Dictionary<int, bool[]> autocastByForm = new Dictionary<int, bool[]>();
        static bool isSuspended;
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
                ClearAllToggles();
                ResetCachedRefs();
                state = InitState.NeedsUpdate;
                lastSceneName = "";
                return;
            }

            if (Scenes.SceneName != lastSceneName)
            {
                lastSceneName = Scenes.SceneName;
                if (PauseOnZoneChangeOn)
                    SuspendAllToggles();
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

            // Flipping the gate on while already in a non-combat zone (or any state
            // where live firing shouldn't happen under the current gate) suspends now.
            if (!isSuspended && GateOn && InNonCombatZone())
                SuspendAllToggles();

            TryResume();

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

            UpdateIconShine();

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

            ApplyFormStateToSkills(lastFormKey);
            state = InitState.Ready;
        }

        static void ClearAllToggles()
        {
            autocastByForm.Clear();
            isSuspended = false;
            lastFormKey = HumanFormKey;
            for (int i = 0; i < SlotCount; i++)
            {
                skills[i].autocastOn = false;
                skills[i].held = false;
                skills[i].nextPressTime = 0f;
            }
        }

        // Stops firing and flips the mod into suspended mode. autocastByForm is untouched
        // so per-form intent survives until a clean press resumes it.
        static void SuspendAllToggles()
        {
            isSuspended = true;
            for (int i = 0; i < SlotCount; i++)
            {
                if (skills[i].held && skills[i].channeled)
                    StopChannel(i);

                skills[i].autocastOn = false;
                skills[i].held = false;
                skills[i].nextPressTime = 0f;
            }
        }

        static void ResumeNow()
        {
            isSuspended = false;
            ApplyFormStateToSkills(lastFormKey);
        }

        static bool[] GetOrCreateFormState(int formKey)
        {
            if (!autocastByForm.TryGetValue(formKey, out var arr))
            {
                arr = new bool[SlotCount];
                autocastByForm[formKey] = arr;
            }
            return arr;
        }

        static void ApplyFormStateToSkills(int formKey)
        {
            if (isSuspended) return;
            if (!autocastByForm.TryGetValue(formKey, out var stored)) return;
            for (int i = 0; i < SlotCount; i++)
                if (!skills[i].ability.IsNullOrDestroyed())
                    skills[i].autocastOn = stored[i];
        }

        static bool GateOn => ModSettings.SkillsAutoCast.DisableInNonCombatZone.Value;
        static bool PauseOnZoneChangeOn => ModSettings.SkillsAutoCast.PauseOnZoneChange.Value;

        static bool InNonCombatZone()
        {
            if (string.IsNullOrEmpty(Scenes.SceneName)) return false;
            try { return SceneList.IsNonCombatZone(Scenes.SceneName); }
            catch { return false; }
        }

        // PauseOnZoneChange on: wait for a clean (no modifier) press before resuming.
        // PauseOnZoneChange off: auto-resume as soon as the non-combat gate clears.
        // Either way, stay suspended while the non-combat gate is blocking us.
        static void TryResume()
        {
            if (!isSuspended) return;
            if (GateOn && InNonCombatZone()) return;

            if (!PauseOnZoneChangeOn)
            {
                ResumeNow();
                return;
            }

            if (rewiredPlayer == null || rewiredPlayer.IsNullOrDestroyed()) return;
            if (IsModifierHeld()) return;

            for (int i = 0; i < SlotCount; i++)
            {
                if (!rewiredPlayer.GetButtonDown(AbilityActionIds[i])) continue;
                ResumeNow();
                return;
            }
        }

        // Form swap rebuilds the ability bar (Werebear/Spriggan/Swarmblade/Reaper). The new
        // form's stored intent is mirrored back into skills[] by InitializeSkills.
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

            var arr = GetOrCreateFormState(lastFormKey);
            arr[i] = !arr[i];

            // Suspended clicks only stage intent into autocastByForm; UpdateIconShine still
            // paints the slot via its armed check so the player sees the pending state.
            if (!isSuspended && !skills[i].ability.IsNullOrDestroyed())
                skills[i].autocastOn = arr[i];
        }

        static bool IsModifierHeld()
        {
            return KeybindMatcher.IsHeld(ModSettings.SkillsAutoCast.ModifierKey.Value);
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
            float liveScale = Mathf.Lerp(0.92f, 1.07f, t);
            var liveTint = new Color(1f, Mathf.Lerp(1f, 0.75f, t), Mathf.Lerp(1f, 0.4f, t), 1f);
            var armedTint = new Color(0.55f, 0.75f, 1f, Mathf.Lerp(0.55f, 0.85f, t));

            bool[] armedArr = null;
            if (isSuspended)
                autocastByForm.TryGetValue(lastFormKey, out armedArr);

            for (int i = 0; i < icons.Count; i++)
            {
                var barIcon = icons[i];
                if (barIcon.IsNullOrDestroyed()) continue;

                int slot = barIcon.abilityNumber - 1;
                if (slot < 0 || slot >= SlotCount)
                {
                    barIcon.transform.localScale = Vector3.one;
                    barIcon.icon.color = Color.white;
                    continue;
                }

                bool live = skills[slot].autocastOn;
                bool armed = !live && armedArr != null && armedArr[slot];

                if (live)
                {
                    barIcon.transform.localScale = new Vector3(liveScale, liveScale, 1f);
                    barIcon.icon.color = liveTint;
                }
                else if (armed)
                {
                    barIcon.transform.localScale = Vector3.one;
                    barIcon.icon.color = armedTint;
                }
                else
                {
                    barIcon.transform.localScale = Vector3.one;
                    barIcon.icon.color = Color.white;
                }
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
                if (PauseOnZoneChangeOn)
                    SuspendAllToggles();
                state = InitState.NeedsUpdate;
            }
        }

        [HarmonyPatch(typeof(MonolithRunsManager), nameof(MonolithRunsManager.onRestZoneEnteredAfterEchoCompleted))]
        public class MonolithRestEnter_Patch
        {
            [HarmonyPostfix]
            static void Postfix()
            {
                if (PauseOnZoneChangeOn)
                    SuspendAllToggles();
                state = InitState.NeedsUpdate;
            }
        }
    }
}
