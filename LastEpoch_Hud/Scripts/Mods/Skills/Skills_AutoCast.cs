using Desktop.Robot;
using Il2Cpp;
using Il2CppPixelCrushers.DialogueSystem;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    [RegisterTypeInIl2Cpp]
    public class Skills_AutoCast : MonoBehaviour
    {
        public static Skills_AutoCast instance { get; private set; }
        public Skills_AutoCast(System.IntPtr ptr) : base(ptr) { }

        public static bool need_update = false;
        public static bool updating = false;
        public static PlayerSkill[] player_skills = new PlayerSkill[5];
        public static bool[] autocast = { false, false, false, false, false };
        public static bool[] channel_on_off = { false, false, false, false, false };
        public static bool app_focus = true;

        void Awake()
        {
            instance = this;
            need_update = true;
        }
        void Update()
        {
            if (Scenes.IsGameScene())
            {
                if ((need_update) && (!updating))
                {
                    updating = true;
                    need_update = false;
                    player_skills = new PlayerSkill[5];

                    GameObject ability_0 = null;
                    GameObject ability_1 = null;
                    GameObject ability_2 = null;
                    GameObject ability_3 = null;
                    GameObject ability_4 = null;
                    //if (!Refs_Manager.player_data.IsNullOrDestroyed()) { character_name = Refs_Manager.player_data.CharacterName; }
                    if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
                    {
                        Canvas canvas_bottom = null;
                        foreach (Canvas canvas in Refs_Manager.game_uibase.canvases)
                        {
                            if (canvas.name == "Canvas (bottom screen UI)")
                            {
                                canvas_bottom = canvas;
                                break;
                            }
                        }
                        if (!canvas_bottom.IsNullOrDestroyed())
                        {
                            GameObject holder = Functions.GetChild(canvas_bottom.gameObject, "Bottom Screen UI Holder");
                            if (!holder.IsNullOrDestroyed())
                            {
                                GameObject screen_ui = Functions.GetChild(holder, "Bottom Screen UI(Clone)");
                                if (!screen_ui.IsNullOrDestroyed())
                                {
                                    GameObject actionBar = Functions.GetChild(screen_ui, "actionBar");
                                    if (!actionBar.IsNullOrDestroyed())
                                    {
                                        GameObject actionBar_middle = Functions.GetChild(actionBar, "ActionBarMiddle");
                                        if (!actionBar_middle.IsNullOrDestroyed())
                                        {
                                            GameObject input_abilities = Functions.GetChild(actionBar_middle, "Inputs_Abilities");
                                            if (!input_abilities.IsNullOrDestroyed())
                                            {
                                                ability_0 = Functions.GetChild(input_abilities, "ActionBarAbility");
                                                ability_1 = Functions.GetChild(input_abilities, "ActionBarAbility (1)");
                                                ability_2 = Functions.GetChild(input_abilities, "ActionBarAbility (2)");
                                                ability_3 = Functions.GetChild(input_abilities, "ActionBarAbility (3)");
                                                ability_4 = Functions.GetChild(input_abilities, "ActionBarAbility (4)");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!ability_0.IsNullOrDestroyed())
                    {
                        Sprite sprite = GetIcon(ability_0);
                        string name = GetName(sprite);
                        player_skills[0] = new PlayerSkill
                        {
                            go = ability_0,
                            ability = GetAbility(sprite),
                            key = GetKeyBind(ability_0),
                            channeled = GetIsChanneled(name),
                            instant = GetIsInstant(name),
                            channel_with_robot = false
                        };
                    }
                    if (!ability_1.IsNullOrDestroyed())
                    {
                        Sprite sprite = GetIcon(ability_1);
                        string name = GetName(sprite);
                        player_skills[1] = new PlayerSkill
                        {
                            go = ability_1,
                            ability = GetAbility(sprite),
                            key = GetKeyBind(ability_1),
                            channeled = GetIsChanneled(name),
                            instant = GetIsInstant(name),
                            channel_with_robot = false
                        };
                    }
                    if (!ability_2.IsNullOrDestroyed())
                    {
                        Sprite sprite = GetIcon(ability_2);
                        string name = GetName(sprite);
                        player_skills[2] = new PlayerSkill
                        {
                            go = ability_2,
                            ability = GetAbility(sprite),
                            key = GetKeyBind(ability_2),
                            channeled = GetIsChanneled(name),
                            instant = GetIsInstant(name),
                            channel_with_robot = false
                        };
                    }
                    if (!ability_3.IsNullOrDestroyed())
                    {
                        Sprite sprite = GetIcon(ability_3);
                        string name = GetName(sprite);
                        player_skills[3] = new PlayerSkill
                        {
                            go = ability_3,
                            ability = GetAbility(sprite),
                            key = GetKeyBind(ability_3),
                            channeled = GetIsChanneled(name),
                            instant = GetIsInstant(name),
                            channel_with_robot = false
                        };
                    }
                    if (!ability_4.IsNullOrDestroyed())
                    {
                        Sprite sprite = GetIcon(ability_4);
                        string name = GetName(sprite);
                        player_skills[4] = new PlayerSkill
                        {
                            go = ability_4,
                            ability = GetAbility(sprite),
                            key = GetKeyBind(ability_4),
                            channeled = GetIsChanneled(name),
                            instant = GetIsInstant(name),
                            channel_with_robot = false
                        };
                    }
                    updating = false;
                }
                if (!need_update)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (player_skills[i].key != KeyCode.None)
                        {
#if KEYBOARD
                            Event e = Event.current;
                            if ((e.type == EventType.KeyUp) && (e.keyCode == player_skills[i].key))
                            {
                                if (player_skills[i].channeled)
                                {
                                    bool enable = channel_on_off[i];
                                    if (e.control) { channel_on_off[i] = !enable; }
                                    else if (channel_on_off[i]) { channel_on_off[i] = false; }
                                    if ((!channel_on_off[i]) && (player_skills[i].channel_with_robot)) { player_skills[i].channel_with_robot = false; }
                                }
                                else if (e.control)
                                {
                                    bool enable = autocast[i];
                                    autocast[i] = !enable;
                                }
                            }
#endif
                            if ((autocast[i]) &&                                        //Autocast On
                            (!GetIsOnCooldown(player_skills[i].go)) &&                  //Not on cooldown
                            (!GetIsOutOfMana(player_skills[i].go)) &&                   //Not out of mana
                            (app_focus))                                                //Application is focused
                            {
                                Robot robot = new Robot();
                                Key robot_key = GetRobotKey(player_skills[i].key);
                                if (robot_key != Key.Pause) { robot.KeyPress(robot_key); }
                            }
                            if (channel_on_off[i])
                            {
                                if (!player_skills[i].channel_with_robot)                   //DoOnce
                                {
                                    Robot robot = new Robot();
                                    Key robot_key = GetRobotKey(player_skills[i].key);
                                    if (robot_key != Key.Pause)
                                    {
                                        player_skills[i].channel_with_robot = true;
                                        robot.KeyDown(robot_key);
                                    }
                                }
                                else if (!app_focus)
                                {
                                    Robot robot = new Robot();
                                    Key robot_key = GetRobotKey(player_skills[i].key);
                                    if (robot_key != Key.Pause)
                                    {
                                        player_skills[i].channel_with_robot = false;
                                        robot.KeyUp(robot_key);
                                    }
                                }
                            }
                            else { player_skills[i].channel_with_robot = false; }
                        }
                    }
                }
            }
            else { need_update = true; }
        }
        void OnApplicationFocus(bool hasFocus)
        {
            app_focus = hasFocus;
        }

        public struct PlayerSkill
        {
            public GameObject go;
            public Ability ability;
            public KeyCode key;
            public bool channeled;
            public bool instant;
            public bool channel_with_robot;
        }
        Sprite GetIcon(GameObject go)
        {
            Sprite result = null;
            if (!go.IsNullOrDestroyed())
            {
                GameObject icon = Functions.GetChild(go, "AbilityIcon1");
                if (!icon.IsNullOrDestroyed())
                {
                    GameObject sprite = Functions.GetChild(icon, "Sprite");
                    if (!sprite.IsNullOrDestroyed())
                    {
                        if (sprite.active)
                        {
                            Image sprite_image = sprite.GetComponent<Image>();
                            result = sprite_image.sprite;
                        }
                    }
                }
            }
            return result;
        }
        bool GetIsOnCooldown(GameObject go)
        {
            bool result = false;
            if (!go.IsNullOrDestroyed())
            {
                GameObject icon = Functions.GetChild(go, "AbilityIcon1");
                if (!icon.IsNullOrDestroyed())
                {
                    GameObject cooldown = Functions.GetChild(icon, "CooldownBar");
                    if (!cooldown.IsNullOrDestroyed())
                    {
                        if (cooldown.activeSelf)
                        {
                            Image cooldown_image = cooldown.GetComponent<Image>();
                            if (!cooldown_image.IsNullOrDestroyed())
                            {
                                if (cooldown_image.fillAmount > 0f) { result = true; }
                            }
                        }
                    }
                }
            }
            return result;
        }
        bool GetIsOutOfMana(GameObject go)
        {
            bool result = false;
            if (!go.IsNullOrDestroyed())
            {
                GameObject icon = Functions.GetChild(go, "AbilityIcon1");
                if (!icon.IsNullOrDestroyed())
                {
                    GameObject mana = Functions.GetChild(icon, "OutOfMana");
                    if (!mana.IsNullOrDestroyed())
                    {
                        if (mana.activeSelf) { result = true; }
                    }
                }
            }
            return result;
        }
        KeyCode GetKeyBind(GameObject go)
        {
            KeyCode result = KeyCode.None;
            if (!go.IsNullOrDestroyed())
            {
                GameObject input = Functions.GetChild(go, "Ability Input Character");
                if (!input.IsNullOrDestroyed())
                {
                    Il2CppTMPro.TextMeshProUGUI keybind_text = input.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
                    if (!keybind_text.IsNullOrDestroyed())
                    {
                        switch (keybind_text.text)
                        {
                            case "A": result = KeyCode.A; break;
                            case "B": result = KeyCode.B; break;
                            case "C": result = KeyCode.C; break;
                            case "D": result = KeyCode.D; break;
                            case "E": result = KeyCode.E; break;
                            case "F": result = KeyCode.F; break;
                            case "G": result = KeyCode.G; break;
                            case "H": result = KeyCode.H; break;
                            case "I": result = KeyCode.I; break;
                            case "J": result = KeyCode.J; break;
                            case "K": result = KeyCode.K; break;
                            case "L": result = KeyCode.L; break;
                            case "M": result = KeyCode.M; break;
                            case "N": result = KeyCode.N; break;
                            case "O": result = KeyCode.O; break;
                            case "P": result = KeyCode.P; break;
                            case "Q": result = KeyCode.Q; break;
                            case "R": result = KeyCode.R; break;
                            case "S": result = KeyCode.S; break;
                            case "T": result = KeyCode.T; break;
                            case "U": result = KeyCode.U; break;
                            case "V": result = KeyCode.V; break;
                            case "W": result = KeyCode.W; break;
                            case "X": result = KeyCode.X; break;
                            case "Y": result = KeyCode.Y; break;
                            case "Z": result = KeyCode.Z; break;
                        }
                    }
                }
            }
            return result;
        }
        string GetName(Sprite sprite)
        {
            string result = "";
            if (!sprite.IsNullOrDestroyed())
            {
                try
                {
                    foreach (Ability ability in Refs_Manager.player_treedata.playerAbilityList.equippedAbilities)
                    {
                        if (!ability.IsNullOrDestroyed())
                        {
                            if (sprite == ability.abilitySprite)
                            {
                                result = ability.abilityName;
                                break;
                            }
                        }
                    }
                }
                catch { }
            }
            return result;
        }
        Ability GetAbility(Sprite sprite)
        {
            Ability result = null;
            if (!sprite.IsNullOrDestroyed())
            {
                try
                {
                    foreach (Ability ability in Refs_Manager.player_treedata.playerAbilityList.equippedAbilities)
                    {
                        if (!ability.IsNullOrDestroyed())
                        {
                            if (sprite == ability.abilitySprite)
                            {
                                result = ability;
                                break;
                            }
                        }
                    }
                }
                catch { }
            }
            return result;
        }
        bool GetIsChanneled(string name)
        {
            bool result = false;
            if (name != "")
            {
                try
                {
                    foreach (Ability ability in Refs_Manager.player_treedata.playerAbilityList.equippedAbilities)
                    {
                        if (!ability.IsNullOrDestroyed())
                        {
                            if (name == ability.abilityName)
                            {
                                result = ability.channelled;
                                break;
                            }
                        }
                    }
                }
                catch { }
            }
            return result;
        }
        bool GetIsInstant(string name)
        {
            bool result = false;
            if (name != "")
            {
                try
                {
                    foreach (Ability ability in Refs_Manager.player_treedata.playerAbilityList.equippedAbilities)
                    {
                        if (!ability.IsNullOrDestroyed())
                        {
                            if (name == ability.abilityName)
                            {
                                result = ability.instantCastForPlayer;
                                break;
                            }
                        }
                    }
                }
                catch { }
            }
            return result;
        }
        Key GetRobotKey(KeyCode key)
        {
            Key result = Key.Pause;
            switch (key)
            {
                case KeyCode.A:
                    result = Key.A;
                    break;
                case KeyCode.B:
                    result = Key.B;
                    break;
                case KeyCode.C:
                    result = Key.C;
                    break;
                case KeyCode.D:
                    result = Key.D;
                    break;
                case KeyCode.E:
                    result = Key.E;
                    break;
                case KeyCode.F:
                    result = Key.F;
                    break;
                case KeyCode.G:
                    result = Key.G;
                    break;
                case KeyCode.H:
                    result = Key.H;
                    break;
                case KeyCode.I:
                    result = Key.I;
                    break;
                case KeyCode.J:
                    result = Key.J;
                    break;
                case KeyCode.K:
                    result = Key.K;
                    break;
                case KeyCode.L:
                    result = Key.L;
                    break;
                case KeyCode.M:
                    result = Key.M;
                    break;
                case KeyCode.N:
                    result = Key.N;
                    break;
                case KeyCode.O:
                    result = Key.O;
                    break;
                case KeyCode.P:
                    result = Key.P;
                    break;
                case KeyCode.Q:
                    result = Key.Q;
                    break;
                case KeyCode.R:
                    result = Key.R;
                    break;
                case KeyCode.S:
                    result = Key.S;
                    break;
                case KeyCode.T:
                    result = Key.T;
                    break;
                case KeyCode.U:
                    result = Key.U;
                    break;
                case KeyCode.V:
                    result = Key.V;
                    break;
                case KeyCode.W:
                    result = Key.W;
                    break;
                case KeyCode.X:
                    result = Key.X;
                    break;
                case KeyCode.Y:
                    result = Key.Y;
                    break;
                case KeyCode.Z:
                    result = Key.Z;
                    break;
            }

            return result;
        }

        /*[HarmonyPatch(typeof(AbilityBarIcon), "Pressed")]
        public class AbilityBarIcon_Pressed
        {
            [HarmonyPostfix]
            static void Postfix(ref AbilityBarIcon __instance)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (player_skills[i].channeled)
                    {
                        AbilityBarIcon abicon = Functions.GetChild(player_skills[i].go, "AbilityIcon1").GetComponent<AbilityBarIcon>();
                        if (!abicon.IsNullOrDestroyed())
                        {
                            if (abicon == __instance)
                            {
                                bool enable = channel_started[i];
                                channel_started[i] = !enable;
                                Main.logger_instance.Msg("channel_started[" + i + "] set to " + channel_started[i]);
                                break;
                            }
                        }
                    }
                }
            }
        }*/

        /*[HarmonyPatch(typeof(CharacterMutator), "OnStartedUsingAbility")]
        public class CharacterMutator_OnStartedUsingAbility
        {
            [HarmonyPostfix]
            static void Postfix(ref AbilityInfo __0, Ability __1)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (player_skills[i].ability == __1)
                    {
                        player_skills[i].abilityInfo = __0;
                        if (player_skills[i].channeled)
                        {
                            bool enable = channel_started[i];
                            channel_started[i] = !enable;
                            Main.logger_instance.Msg("channel_started[" + i + "] set to " + channel_started[i]);
                        }
                        break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CharacterMutator), "OnStoppedUsingAbility")]
        public class CharacterMutator_OnStoppedUsingAbility
        {
            [HarmonyPrefix]
            static bool Prefix(ref AbilityInfo __0)
            {
                bool result = true;
                for (int i = 0; i < 5; i++)
                {
                    if (player_skills[i].abilityInfo == __0)
                    {
                        if (channel_started[i])
                        {
                            result = false;
                            Main.logger_instance.Msg("alter OnStoppedUsingAbility");
                        }
                        break;
                    }
                }

                return result;
            }
        }*/
    }
}
