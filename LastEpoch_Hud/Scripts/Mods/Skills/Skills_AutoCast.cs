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
        // Rewired action IDs for ability bar slots 1-5; IDs 4,5 are skipped in the game's mapping
        static readonly int[] AbilityActionIds = { 1, 2, 3, 6, 7 };
        static readonly Color autocastTint = new Color(0.6f, 1f, 0.6f, 1f);

        public struct PlayerSkill
        {
            public Ability ability;
            public bool channeled;
            public bool autocastEnabled;
            public bool channelEnabled;
            public bool channelSustaining;
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
        static bool wasTransformed;

        // Prevents the Harmony patch from blocking our own ability calls
        static bool fromAutocast;

        void Awake()
        {
            instance = this;
            state = InitState.NeedsUpdate;
        }

        // Stops casting when alt-tabbed
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
                state = InitState.NeedsUpdate;
                return;
            }

            if (state == InitState.NeedsUpdate)
                InitializeSkills();

            if (state != InitState.Ready)
                return;

            CacheRefs();
            if (useAbilityProcessor == null || useAbilityProcessor.IsNullOrDestroyed())
                return;

            CheckTransformChange();
            if (state != InitState.Ready)
                return;

            bool anyActive = false;
            for (int i = 0; i < SlotCount; i++)
            {
                if (skills[i].ability.IsNullOrDestroyed())
                    continue;

                HandleInput(i);
                if (skills[i].autocastEnabled || skills[i].channelEnabled || skills[i].channelSustaining)
                    anyActive = true;
            }

            // Skip expensive raycast when no slots need it
            if (!anyActive)
                return;

            // Manual input takes priority, defer autocast while player holds any ability key
            bool manualInput = IsPlayerHoldingAbilityKey();

            Vector3 targetPos = GetTargetPosition(out Transform hitTransform);

            for (int i = 0; i < SlotCount; i++)
            {
                if (skills[i].ability.IsNullOrDestroyed())
                    continue;

                if (!manualInput)
                    HandleAutoCast(i, targetPos, hitTransform);
                HandleChanneling(i, targetPos, hitTransform);
            }

            UpdateIconTints();
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

            state = InitState.Ready;
        }

        // Zone transitions clear all toggles
        void ClearAllToggles()
        {
            for (int i = 0; i < SlotCount; i++)
            {
                skills[i].autocastEnabled = false;
                skills[i].channelEnabled = false;
                skills[i].channelSustaining = false;
            }
        }

        // Transform swaps the ability bar, re-init to pick up new abilities
        void CheckTransformChange()
        {
            if (Refs_Manager.player_treedata.IsNullOrDestroyed()) return;
            var abilityList = Refs_Manager.player_treedata.playerAbilityList;
            if (abilityList.IsNullOrDestroyed()) return;

            bool is_transformed = false;
            try { is_transformed = abilityList.isTransformed(); } catch { }

            if (is_transformed == wasTransformed) return;
            wasTransformed = is_transformed;
            ClearAllToggles();
            state = InitState.NeedsUpdate;
        }

        // IL2CPP refs can become null mid-frame, re-acquire as needed
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
            if (!rewiredPlayer.GetButtonUp(AbilityActionIds[i])) return;

            bool modifier = IsModifierHeld();

            if (skills[i].channeled)
            {
                if (modifier)
                    skills[i].channelEnabled = !skills[i].channelEnabled;
                else if (skills[i].channelEnabled)
                    skills[i].channelEnabled = false;
            }
            else if (modifier)
            {
                skills[i].autocastEnabled = !skills[i].autocastEnabled;
            }
        }

        // Rewired GamepadTemplate element ID for d-pad left
        const int DpadLeftElementId = 22;

        static bool IsModifierHeld()
        {
#if KEYBOARD
            return EpochInputManager.CtrlPressed();
#elif WINGAMEPAD
            // D-pad left via Rewired (element ID 22)
            if (rewiredPlayer == null || rewiredPlayer.IsNullOrDestroyed()) return false;
            try
            {
                var joystick = rewiredPlayer.controllers.GetLastActiveController(ControllerType.Joystick);
                return joystick != null && joystick.GetButtonById(DpadLeftElementId);
            }
            catch { return false; }
#else
            return false;
#endif
        }

        static bool IsPlayerHoldingAbilityKey()
        {
            if (rewiredPlayer == null || rewiredPlayer.IsNullOrDestroyed()) return false;
            for (int i = 0; i < SlotCount; i++)
            {
                if (rewiredPlayer.GetButton(AbilityActionIds[i])) return true;
            }
            return false;
        }

        static void StartAbility(int slot, Vector3 targetPos, Transform hitTransform)
        {
            fromAutocast = true;
            try { useAbilityProcessor.UseBarAbilityCommand(true, slot + 1, -1, targetPos, hitTransform, false); }
            catch { }
            finally { fromAutocast = false; }
        }

        // UseBarAbilityCommand(false) doesn't stop channeling, need explicit stop call
        static void StopChanneling()
        {
            if (abilityState.IsNullOrDestroyed()) return;
            try { abilityState.stopUsingAbility(true); }
            catch { }
        }

        void HandleAutoCast(int i, Vector3 targetPos, Transform hitTransform)
        {
            if (!skills[i].autocastEnabled || !appFocus) return;
            if (IsOnCooldown(skills[i].ability)) return;

            StartAbility(i, targetPos, hitTransform);
        }

        void HandleChanneling(int i, Vector3 targetPos, Transform hitTransform)
        {
            if (!skills[i].channelEnabled)
            {
                if (skills[i].channelSustaining)
                {
                    skills[i].channelSustaining = false;
                    StopChanneling();
                }
                return;
            }

            if (!appFocus)
            {
                skills[i].channelSustaining = false;
                skills[i].channelEnabled = false;
                StopChanneling();
                return;
            }

            // Wait out cooldown, resume when ready
            if (IsOnCooldown(skills[i].ability))
                return;

            if (!skills[i].channelSustaining)
            {
                // Start once, game sustains internally
                skills[i].channelSustaining = true;
                StartAbility(i, targetPos, hitTransform);
            }
            else
            {
                // Update aim while channeling (Warpath steering etc.)
                try { abilityState.TryUpdateTargetLocationIfChannelling(targetPos); }
                catch { }
            }
        }

        bool IsOnCooldown(Ability ability)
        {
            if (chargeManager.IsNullOrDestroyed()) return false;
            try { return chargeManager.onCoooldown(ability); }
            catch { return false; }
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

        static void UpdateIconTints()
        {
            var icons = AbilityBarIcon.all;
            if (icons == null) return;

            for (int i = 0; i < icons.Count; i++)
            {
                var barIcon = icons[i];
                if (barIcon.IsNullOrDestroyed()) continue;

                int slot = barIcon.abilityNumber - 1;
                bool active = slot >= 0 && slot < SlotCount &&
                    (skills[slot].autocastEnabled || skills[slot].channelEnabled);

                barIcon.icon.color = active ? autocastTint : Color.white;
            }
        }

        // Suppress native ability fire when modifier held, so it only toggles autocast
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

        // External interruption (stun, manual cast) resets sustaining so channel can restart
        [HarmonyPatch(typeof(UsingAbilityPlayer), nameof(UsingAbilityPlayer.stopUsingAbility))]
        public class StopUsingAbility_Patch
        {
            [HarmonyPostfix]
            static void Postfix()
            {
                if (fromAutocast) return;
                for (int i = 0; i < SlotCount; i++)
                    skills[i].channelSustaining = false;
            }
        }
    }
}
