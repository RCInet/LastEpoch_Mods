using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.Mods.UI
{
    [RegisterTypeInIl2Cpp]
    public class DamageMeter : MonoBehaviour
    {
        public static DamageMeter instance { get; private set; }
        public DamageMeter(System.IntPtr ptr) : base(ptr) { }

        public static GameObject DamageMeter_prefab = null;
        public static GameObject DamageMeter_obj = null;
        public static GameObject DamageMeter_content = null;
        public static TextMeshProUGUI MenuText = null;

        public static Button OnOff_btn = null;
        public static bool On = false;
        public static GameObject Reset_obj = null;
        public static Button Reset_btn = null;

        public static Image OnOff_Image = null;
        public static Sprite On_sprite = null;
        public static Sprite Off_sprite = null;

        public static GameObject Settings_panel = null;
        public static GameObject Settings_obj = null;
        public static Button Settings_btn = null;

        public static GameObject Skill_prefab = null;
        public static System.Collections.Generic.List<Ability> Abilities = new System.Collections.Generic.List<Ability>();
        public static System.Collections.Generic.List<Skill> Skills = new System.Collections.Generic.List<Skill>();
        public static float TotalDamageDeal = 0f;

        //Should be added into UI
        public static bool Show_Percent = true; //set to false to show flat damage
        public static bool Separate_Dot = true; //set to false to not separate DoT and hit damage

        public static int Frames = 30; //Update append every x frames (to avoid too much performance cost)
        public static int Current_Frame = 0;

        void Awake()
        {
            instance = this;
            UI.Reset();
        }
        void Update()
        {
            if (!Assets.Loaded()) { Assets.Load(); }
            else
            {
                if (Scenes.IsGameScene())
                {
                    if ((Abilities.Count == 0) && (On))
                    {
                        foreach (Ability ab in Resources.FindObjectsOfTypeAll<Ability>())
                        {
                            if (!Abilities.Contains(ab)) { Abilities.Add(ab); }
                        }
                        foreach (Ability ab in NewItems.Items_Mjolner.Trigger.Abilities)
                        {
                            if (!Abilities.Contains(ab)) { Abilities.Add(ab); }
                        }
                    }
                    if (!UI.Initialized) { UI.Init(); }
                    else
                    {
                        if (!MenuText.IsNullOrDestroyed()) { MenuText.text = "Damage Meter"; }
                        if (!DamageMeter_obj.IsNullOrDestroyed())
                        {
                            if (Input.GetKeyDown(KeyCode.U)) { DamageMeter_obj.active = !DamageMeter_obj.active; }
                            if (!DamageMeter_obj.active) { On = false; }
                            if (On) { UI.Update(); }
                        }
                        if (!OnOff_Image.IsNullOrDestroyed())
                        {
                            if (On) { OnOff_Image.sprite = On_sprite; }
                            else { OnOff_Image.sprite = Off_sprite; }
                        }
                        if (!Reset_obj.IsNullOrDestroyed())
                        {
                            if (Skills.Count > 0) { Reset_obj.active = true; }
                            else { Reset_obj.active = false; }
                        }
                    }
                }
                else
                {
                    On = false;
                    Abilities = new System.Collections.Generic.List<Ability>();
                    if (!DamageMeter_obj.IsNullOrDestroyed())
                    {
                        if (DamageMeter_obj.active) { DamageMeter_obj.active = false; }
                    }
                    UI.Initialized = false;
                }
            }
        }

        public class Assets
        {
            private static bool loading = false;

            public static bool Loaded()
            {
                bool result = false;
                if ((!DamageMeter_prefab.IsNullOrDestroyed()) && (!Skill_prefab.IsNullOrDestroyed())) { result = true; }

                return result;
            }
            public static void Load()
            {
                if ((!Hud_Manager.asset_bundle.IsNullOrDestroyed()) && (!loading))
                {                    
                    loading = true;
                    foreach (string name in Hud_Manager.asset_bundle.GetAllAssetNames())
                    {
                        if (name.Contains("/damagemeter/"))
                        {
                            if (Functions.Check_Prefab(name) && name.Contains("damagemeter.prefab"))
                            {
                                DamageMeter_prefab = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<GameObject>();
                            }
                            else if (Functions.Check_Prefab(name) && name.Contains("skill.prefab"))
                            {
                                Skill_prefab = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<GameObject>();
                            }
                        }
                    }
                    loading = false;
                }
            }
        }
        public class UI
        {
            public static bool Initializing = false;
            public static bool Initialized = false;

            public static void Init()
            {
                if (!Initializing && !Refs_Manager.game_uibase.IsNullOrDestroyed())
                {
                    Initializing = true;
                    DamageMeter_obj = Instantiate(DamageMeter_prefab, Vector3.zero, Quaternion.identity);
                    DontDestroyOnLoad(DamageMeter_obj);
                    DamageMeter_obj.active = false;
                    DamageMeter_obj.transform.SetParent(Refs_Manager.game_uibase.transform);
                    DamageMeter_obj.AddComponent<UIMouseListener>(); //Block mouse
                    if (!DamageMeter_obj.IsNullOrDestroyed())
                    {
                        GameObject images = Functions.GetChild(DamageMeter_obj, "Images");
                        if (!images.IsNullOrDestroyed())
                        {
                            GameObject On_obj = Functions.GetChild(images, "On");
                            if (!On_obj.IsNullOrDestroyed())
                            {
                                Image On_image = On_obj.GetComponent<Image>();
                                if (!On_image.IsNullOrDestroyed()) { On_sprite = On_image.sprite; }                                
                            }
                            GameObject Off_obj = Functions.GetChild(images, "Off");
                            if (!Off_obj.IsNullOrDestroyed())
                            {
                                Image Off_image = Off_obj.GetComponent<Image>();
                                if (!Off_image.IsNullOrDestroyed()) { Off_sprite = Off_image.sprite; }
                            }
                        }
                        GameObject panel = Functions.GetChild(DamageMeter_obj, "Panel");
                        if (!panel.IsNullOrDestroyed())
                        {
                            DamageMeter_content = Functions.GetChild(panel, "Content");
                            GameObject title = Functions.GetChild(panel, "Title");
                            if (!title.IsNullOrDestroyed())
                            {
                                GameObject on_off = Functions.GetChild(title, "OnOff");
                                if (!on_off.IsNullOrDestroyed())
                                {
                                    OnOff_btn = on_off.GetComponent<Button>();
                                    if (!OnOff_btn.IsNullOrDestroyed()) { Events.Set(OnOff_btn, Events.OnOff_OnClick_Action); }
                                    GameObject image = Functions.GetChild(on_off, "Image");
                                    if (!image.IsNullOrDestroyed())
                                    {
                                        OnOff_Image = image.GetComponent<Image>();
                                        if (!OnOff_Image.IsNullOrDestroyed()) { OnOff_Image.sprite = Off_sprite; }
                                    }
                                }

                                Reset_obj = Functions.GetChild(title, "Reset");
                                if (!Reset_obj.IsNullOrDestroyed())
                                {
                                    Reset_btn = Reset_obj.GetComponent<Button>();
                                    if (!Reset_btn.IsNullOrDestroyed()) { Events.Set(Reset_btn, Events.Reset_OnClick_Action); }
                                }

                                Settings_obj = Functions.GetChild(title, "Settings");
                                if (!Settings_obj.IsNullOrDestroyed())
                                {
                                    Settings_btn = Settings_obj.GetComponent<Button>();
                                    if (!Settings_btn.IsNullOrDestroyed()) { Events.Set(Settings_btn, Events.Settings_OnClick_Action); }
                                }
                            }
                        }
                        Settings_panel = Functions.GetChild(DamageMeter_obj, "SettingsPanel");
                        if (!Settings_panel.IsNullOrDestroyed())
                        {

                        }
                    }
                    //menu
                    if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
                    {
                        GameObject go = Refs_Manager.game_uibase.bottomScreenMenu.gameObject; //BottomScreenMenu
                        if (!go.IsNullOrDestroyed())
                        {
                            GameObject panel = Functions.GetChild(go, "BottomScreenMenuPanel");
                            if (!panel.IsNullOrDestroyed())
                            {
                                BottomScreenMenu bottomScreenMenu = panel.GetComponent<BottomScreenMenu>();
                                if (!bottomScreenMenu.IsNullOrDestroyed())
                                {
                                    bottomScreenMenu.buttonsToDisableInOnline = new Il2CppSystem.Collections.Generic.List<Button>();
                                }

                                GameObject shop = Functions.GetChild(panel, "Shop");
                                if (!shop.IsNullOrDestroyed())
                                {
                                    Button shop_btn = shop.GetComponent<Button>();
                                    Events.Set(shop_btn, Events.ToggleVisibility_OnClick_Action);
                                    shop_btn.interactable = true;

                                    CanvasGroup canvasGroup = shop.GetComponent<CanvasGroup>();
                                    if(!canvasGroup.IsNullOrDestroyed()) { Object.Destroy(canvasGroup); }

                                    GameObject text_obj = Functions.GetChild(shop, "TextMeshPro Text");
                                    if (!text_obj.IsNullOrDestroyed())
                                    {
                                        MenuText = text_obj.GetComponent<TextMeshProUGUI>();                                        
                                    }
                                }
                            }
                        }
                    }
                    UI.Reset();
                    Initialized = true;
                    Initializing = false;
                }
            }                        
            public static void AddSkill(string ability_name, float damage, bool hit, bool crit, bool kill, float overkill, string target_name)
            {
                if (!Skill_prefab.IsNullOrDestroyed())
                {
                    GameObject skill_obj = Instantiate(Skill_prefab, Vector3.zero, Quaternion.identity);
                    skill_obj.active = false;
                    skill_obj.transform.SetParent(DamageMeter_content.transform);
                    Sprite icon = GetIcon(ability_name);
                    System.Collections.Generic.List<float> damages = new System.Collections.Generic.List<float>();
                    damages.Add(damage);
                    System.Collections.Generic.List<bool> crits = new System.Collections.Generic.List<bool>();
                    crits.Add(crit);
                    System.Collections.Generic.List<bool> kills = new System.Collections.Generic.List<bool>();
                    kills.Add(kill);
                    System.Collections.Generic.List<float> overkills = new System.Collections.Generic.List<float>();
                    overkills.Add(overkill);
                    System.Collections.Generic.List<string> target_names = new System.Collections.Generic.List<string>();
                    target_names.Add(target_name);
                    Skills.Add(new Skill
                    {
                        Obj = skill_obj,
                        Icon = icon,
                        AbilityName = ability_name,                        
                        Damages = damages,
                        Dot = !hit,
                        Crits = crits,
                        Kills = kills,
                        Overkills = overkills,
                        TargetName = target_names
                    });
                }
            }
            public static void Update()
            {
                Current_Frame++;
                if (Current_Frame >= Frames)
                {
                    Current_Frame = 0;
                    foreach (Skill skill in Skills)
                    {
                        if (!skill.Obj.IsNullOrDestroyed())
                        {
                            if (((!Separate_Dot) && (!skill.Dot)) || (Separate_Dot))
                            {
                                GameObject infos = Functions.GetChild(skill.Obj, "Infos");
                                //Set icon and SkillName                         
                                if ((!infos.IsNullOrDestroyed()))
                                {
                                    GameObject icon_obj = Functions.GetChild(infos, "Icon");
                                    if ((!icon_obj.IsNullOrDestroyed()))
                                    {
                                        GameObject image_obj = Functions.GetChild(icon_obj, "Image");
                                        if ((!image_obj.IsNullOrDestroyed()))
                                        {
                                            Image icon = image_obj.GetComponent<Image>();
                                            if (!icon.IsNullOrDestroyed()) { icon.sprite = skill.Icon; }
                                        }
                                    }
                                    GameObject skill_name_obj = Functions.GetChild(infos, "SkillName");
                                    if ((!skill_name_obj.IsNullOrDestroyed()))
                                    {
                                        GameObject text_obj = Functions.GetChild(skill_name_obj, "Text");
                                        if ((!text_obj.IsNullOrDestroyed()))
                                        {
                                            Text text = text_obj.GetComponent<Text>();
                                            if (!text.IsNullOrDestroyed())
                                            {
                                                text.text = skill.AbilityName;
                                                if (skill.Dot) { text.text += " (DoT)"; }
                                            }
                                        }
                                    }
                                }
                                //Set damage and slider
                                float damage = 0f;
                                foreach (float f in skill.Damages) { damage += f; }
                                if (!Separate_Dot)
                                {
                                    foreach (Skill s in Skills)
                                    {
                                        if ((s.AbilityName == skill.AbilityName) && (s.Dot))
                                        {
                                            foreach (float f in s.Damages) { damage += f; }
                                            break;
                                        }
                                    }
                                }
                                if ((!infos.IsNullOrDestroyed()))
                                {
                                    GameObject percent_obj = Functions.GetChild(infos, "Percent");
                                    if ((!percent_obj.IsNullOrDestroyed()))
                                    {
                                        GameObject text_obj = Functions.GetChild(percent_obj, "Text");
                                        if ((!text_obj.IsNullOrDestroyed()))
                                        {
                                            Text text = text_obj.GetComponent<Text>();
                                            if (!text.IsNullOrDestroyed())
                                            {
                                                if (Show_Percent)
                                                {
                                                    float damage_percent = ((damage * 100) / TotalDamageDeal);
                                                    text.text = damage_percent.ToString("0.0") + " %";
                                                }
                                                else { text.text = System.Convert.ToInt32(damage).ToString(); }
                                            }
                                        }
                                    }
                                }
                                GameObject skill_bar_obj = Functions.GetChild(skill.Obj, "SkillBar");
                                if (!skill_bar_obj.IsNullOrDestroyed())
                                {
                                    Slider slider = skill_bar_obj.GetComponent<Slider>();
                                    if (!slider.IsNullOrDestroyed()) { slider.value = damage * 100 / TotalDamageDeal; }
                                }
                                if (!skill.Obj.active) { skill.Obj.active = true; }
                            }
                            else if (skill.Obj.active) { skill.Obj.active = false; }
                        }
                    }
                }
            }
            public static Sprite GetIcon(string ability_name)
            {
                Sprite result = null;
                try
                {
                    foreach (Ability ab in Abilities)
                    {
                        if (ab.abilityName == ability_name)
                        {
                            if (!ab.abilitySprite.IsNullOrDestroyed())
                            {
                                result = ab.abilitySprite;
                                break;
                            }
                        }
                    }
                    if (result == null)
                    {
                        foreach (Summoned summoned in Refs_Manager.summon_tracker.summons) //Summon
                        {
                            AbilityList abilitylist = summoned.gameObject.GetComponent<AbilityList>();
                            if (!abilitylist.IsNullOrDestroyed())
                            {
                                foreach (Ability ability in abilitylist.abilities)
                                {
                                    //if (ability.name == obj_name)
                                    if (ability.abilityName == ability_name)
                                    {
                                        CreationReferences creationReferences = summoned.gameObject.GetComponent<CreationReferences>();
                                        if (!creationReferences.IsNullOrDestroyed())
                                        {
                                            if (!creationReferences.thisAbility.IsNullOrDestroyed())
                                            {
                                                result = creationReferences.thisAbility.abilitySprite;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }

                return result;
            }
            public static void Reset()
            {
                Skills = new System.Collections.Generic.List<Skill>();
                TotalDamageDeal = 0f;
                if (!DamageMeter_content.IsNullOrDestroyed())
                {
                    foreach (GameObject go in Functions.GetAllChild(DamageMeter_content)) { Object.Destroy(go); }
                }
            }
        }
        public class Events
        {
            public static void Set(Button btn, UnityEngine.Events.UnityAction action)
            {
                if (!btn.IsNullOrDestroyed())
                {
                    btn.onClick = new Button.ButtonClickedEvent();
                    btn.onClick.AddListener(action);
                }
            }

            public static readonly System.Action ToggleVisibility_OnClick_Action = new System.Action(ToggleVisibility_Click);
            public static void ToggleVisibility_Click()
            {
                DamageMeter_obj.active = !DamageMeter_obj.active;
            }

            public static readonly System.Action OnOff_OnClick_Action = new System.Action(OnOff_Click);
            public static void OnOff_Click()
            {
                On = !On;                
            }

            public static readonly System.Action Reset_OnClick_Action = new System.Action(Reset_Click);
            public static void Reset_Click()
            {
                UI.Reset();                
            }

            public static readonly System.Action Settings_OnClick_Action = new System.Action(Settings_Click);
            public static void Settings_Click()
            {
                Settings_panel.active = !Settings_panel.active;
            }
        }
        public class DamageDeal
        {
            [HarmonyPatch(typeof(AbilityEventListener), "DetailedAbilityEvent")]
            public class AbilityEventListener_DetailedAbilityEvent
            {
                [HarmonyPostfix]
                static void Postfix(DetailedAbilityEvent __0)
                {
                    if ((Scenes.IsGameScene()) && (On))
                    {
                        bool target_is_player = false;
                        if (__0.target == Refs_Manager.player_actor) { target_is_player = true; }
                        bool target_is_summon = false;
                        foreach (Summoned summoned in Refs_Manager.summon_tracker.summons)
                        {
                            if (summoned.actor == __0.target) { target_is_summon = true; break; }
                        }
                        if ((!target_is_player) && (!target_is_summon))
                        {
                            TotalDamageDeal += __0.damageDealt;
                            bool found = false;
                            foreach (Skill skill in Skills)
                            {
                                if ((skill.AbilityName == __0.ability.abilityName) && (skill.Dot == !__0.hit))
                                {
                                    found = true;
                                    skill.Damages.Add(__0.damageDealt);
                                    skill.Crits.Add(__0.crit);
                                    skill.Kills.Add(__0.kill);
                                    skill.Overkills.Add(__0.overkill);
                                    skill.TargetName.Add(__0.target.name);
                                    break;
                                }
                            }
                            if (!found) { UI.AddSkill(__0.ability.abilityName, __0.damageDealt, __0.hit, __0.crit, __0.kill, __0.overkill, __0.target.name); }
                        }
                    }
                }
            }
        }
        public struct Skill
        {
            public GameObject Obj;
            public Sprite Icon;
            public string AbilityName;
            public bool Dot;
            public System.Collections.Generic.List<float> Damages;            
            public System.Collections.Generic.List<bool> Crits;
            public System.Collections.Generic.List<bool> Kills;
            public System.Collections.Generic.List<float> Overkills;
            public System.Collections.Generic.List<string> TargetName;
        }
    }
}
