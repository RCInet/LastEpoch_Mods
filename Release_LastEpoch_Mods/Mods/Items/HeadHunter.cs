using HarmonyLib;
using LastEpochMods.Managers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpochMods.Mods.Items
{
    public class HeadHunter : MonoBehaviour
    {
        public static bool Initialized = false;
        public static void Init()
        {
            if ((HHTracker.Tracker_Initialized) && (Basic.AddedToBasicList) && (Events.SceneLoaded_Initialized))
            {
                if (Assets_Manager.Headhunter.loaded)
                {
                    Config.LoadConfig();
                    Initialized = true;
                }                
            }
            else
            {
                HHTracker.Init();
                Basic.AddToBasicList();
                Events.Init();
            }
        }
        public static void Init_Assets()
        {
            Assets_Manager.Load_Headhunter();
        }

        public class Basic
        {
            public static readonly byte base_type = 2; //Belt            
            public static readonly string base_name = "HH Leather Belt";
            public static readonly int base_id = 13;
            public static ItemList.EquipmentItem Item()
            {
                ItemList.EquipmentItem item = new ItemList.EquipmentItem
                {
                    classRequirement = ItemList.ClassRequirement.None,
                    implicits = implicits(),
                    subClassRequirement = ItemList.SubClassRequirement.None,
                    cannotDrop = Save_Manager.Data.UserData.Items.Headhunter.base_item_cannotDrop,
                    itemTags = ItemLocationTag.None,
                    levelRequirement = 40,
                    name = base_name,
                    subTypeID = base_id
                };

                return item;
            }
            public static bool AddedToBasicList = false;
            public static void AddToBasicList()
            {
                if (!AddedToBasicList)
                {
                    try
                    {
                        ItemList.instance.EquippableItems[base_type].subItems.Add(Item());
                        AddedToBasicList = true;
                    }
                    catch { }
                }
            }

            private static Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> implicits()
            {
                Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> implicits = new Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit>();
                implicits.Add(new ItemList.EquipmentImplicit
                {
                    implicitMaxValue = 40,
                    implicitValue = 25,
                    property = SP.Health,
                    specialTag = 0,
                    tags = AT.None,
                    type = BaseStats.ModType.ADDED
                });

                return implicits;
            }
        }
        public class Unique
        {
            public static readonly string unique_name = "Headhunter";
            public static readonly ushort unique_id = 500;
            public static readonly string lore = "\"A man's soul rules from a cavern of bone, learns and judges through flesh-born windows. The heart is meat. The head is where the Man is.\"\r\n- Lavianga, Advisor to Kaom";
            public static UniqueList.Entry Item()
            {
                UniqueList.Entry item = new UniqueList.Entry
                {
                    name = unique_name,
                    displayName = unique_name,
                    uniqueID = unique_id,
                    isSetItem = false,
                    setID = 0,
                    overrideLevelRequirement = true,
                    levelRequirement = 40,
                    legendaryType = LegendaryType(),
                    overrideEffectiveLevelForLegendaryPotential = true,
                    effectiveLevelForLegendaryPotential = 0,
                    canDropRandomly = Save_Manager.Data.UserData.Items.Headhunter.unique_item_canDropRandomly,
                    rerollChance = 1,
                    itemModelType = UniqueList.ItemModelType.Unique,
                    subTypeForIM = 0,
                    baseType = Basic.base_type,
                    subTypes = SubType(),
                    mods = Mods(),
                    tooltipDescriptions = TooltipDescription(),
                    loreText = lore,
                    tooltipEntries = TooltipEntries(),
                    oldSubTypeID = 0,
                    oldUniqueID = 0
                };

                return item;
            }
            public static bool AddedToUniqueList = false;
            public static void AddToUniqueList(ref UniqueList unique_list)
            {
                if (!AddedToUniqueList)
                {
                    try
                    {
                        unique_list.uniques.Add(Item());
                        AddedToUniqueList = true;
                    }
                    catch { }
                }
            }
            public static void Update()
            {
                try
                {
                    int index = 0;
                    bool found = false;
                    foreach (UniqueList.Entry item in UniqueList.instance.uniques)
                    {
                        if (item.name == Unique.unique_name)
                        {
                            found = true;
                            break;
                        }
                        index++;
                    }
                    if (found)
                    {
                        UniqueList.Entry hh = Unique.Item();
                        if (UniqueList.instance.uniques[index] != hh) { UniqueList.instance.uniques[index] = hh; }
                    }
                }
                catch { }
            }            

            private static Il2CppSystem.Collections.Generic.List<byte> SubType()
            {
                Il2CppSystem.Collections.Generic.List<byte> result = new Il2CppSystem.Collections.Generic.List<byte>();
                byte r = (byte)Basic.base_id;
                result.Add(r);

                return result;
            }
            private static Il2CppSystem.Collections.Generic.List<UniqueItemMod> Mods()
            {
                Il2CppSystem.Collections.Generic.List<UniqueItemMod> result = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Strength,
                    tags = AT.None,
                    type = BaseStats.ModType.ADDED,
                    maxValue = 55,
                    value = 40
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Dexterity,
                    tags = AT.None,
                    type = BaseStats.ModType.ADDED,
                    maxValue = 55,
                    value = 40
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Health,
                    tags = AT.None,
                    type = BaseStats.ModType.ADDED,
                    maxValue = 60,
                    value = 50
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Damage,
                    tags = AT.None,
                    type = BaseStats.ModType.INCREASED,
                    maxValue = 0.3f,
                    value = 0.2f
                });

                return result;
            }
            private static Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> TooltipEntries()
            {
                Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> result = new Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry>();
                result.Add(new UniqueModDisplayListEntry(0));
                result.Add(new UniqueModDisplayListEntry(1));
                result.Add(new UniqueModDisplayListEntry(2));
                result.Add(new UniqueModDisplayListEntry(3));
                result.Add(new UniqueModDisplayListEntry(128));

                return result;
            }
            private static Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> TooltipDescription()
            {
                Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> result = new Il2CppSystem.Collections.Generic.List<ItemTooltipDescription>();
                string description = "";
                if (!Save_Manager.Data.UserData.Items.Headhunter.random)
                {
                    description = "When you Kill a Rare monster, you gain its Modifiers for " + Steal.BuffDuration + " seconds";
                }
                else
                {
                    description = "When you or your minions Kill a monster, you gain " + Save_Manager.Data.UserData.Items.Headhunter.Min_Generated +
                        " to " + Save_Manager.Data.UserData.Items.Headhunter.Max_Generated + " random Modifiers for " +
                        Save_Manager.Data.UserData.Items.Headhunter.BuffDuration + " seconds";
                }
                result.Add(new ItemTooltipDescription { description = description });

                return result;
            }
            private static UniqueList.LegendaryType LegendaryType()
            {
                UniqueList.LegendaryType legendaryType = UniqueList.LegendaryType.LegendaryPotential;
                if (Save_Manager.Data.UserData.Items.Headhunter.weaverwill)
                {
                    legendaryType = UniqueList.LegendaryType.WeaversWill;
                }

                return legendaryType;
            }

            [HarmonyPatch(typeof(UniqueList), "get")]
            public class UniqueList_Get
            {
                [HarmonyPostfix]
                static void Postfix(ref UniqueList __result)
                {
                    if (__result != null) { AddToUniqueList(ref __result); }
                }
            }
            
            [HarmonyPatch(typeof(InventoryItemUI), "GetSpriteImage")]
            public class InventoryItemUI_GetSpriteImage
            {
                [HarmonyPostfix]
                static void Postfix(ref UnityEngine.Sprite __result, ItemData __0, ItemUIContext __1)
                {
                    if (__0.getAsUnpacked().FullName == unique_name)
                    {
                        __result = Assets_Manager.Headhunter.icon;
                    }
                }
            }

            [HarmonyPatch(typeof(UITooltipItem), "SetItemSprite")]
            public class UITooltipItem_SetItemSprite
            {
                [HarmonyPostfix]
                static void Postfix(ref UnityEngine.Sprite __result, ItemDataUnpacked __0)
                {
                    if (__0.FullName == unique_name)
                    {
                        __result = Assets_Manager.Headhunter.icon;
                    }
                }
            }
        }
        public class Config
        {
            public static System.Collections.Generic.List<RandomBuffs.HH_Buff> HH_Buff_Config = new System.Collections.Generic.List<RandomBuffs.HH_Buff>();
            public static System.Collections.Generic.List<RandomBuffs.HH_Buff> HH_Buff_Config_Backup = new System.Collections.Generic.List<RandomBuffs.HH_Buff>();
            public static void LoadConfig()
            {
                HH_Buff_Config = new System.Collections.Generic.List<RandomBuffs.HH_Buff>();
                HH_Buff_Config_Backup = new System.Collections.Generic.List<RandomBuffs.HH_Buff>();
                if (!System.IO.File.Exists(Save_Manager.path + filename)) { DefaultConfig(); }
                else
                {
                    try
                    {
                        HH_Buff_Config = JsonConvert.DeserializeObject<System.Collections.Generic.List<RandomBuffs.HH_Buff>>(System.IO.File.ReadAllText(Save_Manager.path + filename));
                        HH_Buff_Config_Backup = HH_Buff_Config;
                        RandomBuffs.Generate_HH_BuffsList();
                    }
                    catch { DefaultConfig(); }
                }
            }
            public static void SaveConfig()
            {
                HH_Buff_Config_Backup = HH_Buff_Config; //Use to check if buffs changed                
                string jsonString = JsonConvert.SerializeObject(HH_Buff_Config, Formatting.Indented);
                if (!System.IO.Directory.Exists(Save_Manager.path)) { System.IO.Directory.CreateDirectory(Save_Manager.path); }
                if (System.IO.File.Exists(Save_Manager.path + filename)) { System.IO.File.Delete(Save_Manager.path + filename); }
                System.IO.File.WriteAllText(Save_Manager.path + filename, jsonString);
                RandomBuffs.Generate_HH_BuffsList();
            }

            private static string filename = "hh_buffs.json";
            private static void DefaultConfig()
            {
                HH_Buff_Config = JsonConvert.DeserializeObject<System.Collections.Generic.List<RandomBuffs.HH_Buff>>(Assets_Manager.Headhunter.json.text);
                HH_Buff_Config_Backup = HH_Buff_Config;
                SaveConfig();
            }
            private static void nullConfig()
            {
                HH_Buff_Config = new System.Collections.Generic.List<RandomBuffs.HH_Buff>();
                HH_Buff_Config_Backup = new System.Collections.Generic.List<RandomBuffs.HH_Buff>();
                for (int i = 0; i < System.Enum.GetValues(typeof(SP)).Length; i++)
                {
                    SP value = (SP)i;
                    HH_Buff_Config.Add(new RandomBuffs.HH_Buff
                    {
                        property = value.ToString(),
                        //enable = false,
                        added = false,
                        increased = false,
                        max_added = 0,
                        max_increased = 0
                    });
                }
            }
        }
        public class RandomBuffs
        {
            public struct HH_Buff
            {
                public string property;
                public bool added;
                public bool increased;
                public int max_added;
                public int max_increased;
            }
            public static string hh_buff = "HHBuff";
            public static void GenerateBuffs()
            {
                int NbBuff = Random.Range(Save_Manager.Data.UserData.Items.Headhunter.Min_Generated, Save_Manager.Data.UserData.Items.Headhunter.Max_Generated + 1);
                for (int i = 0; i < NbBuff; i++)
                {
                    Actor playerActor = PlayerFinder.getPlayerActor();
                    Buff random_buff = Generate_Random_HHBuff();
                    int max_value = 255;
                    int max_addvalue = max_value;
                    int max_increasedvalue = max_value;
                    bool found = false;
                    foreach (var p in Config.HH_Buff_Config)
                    {
                        if (random_buff.name.Contains(p.property))
                        {
                            if ((p.max_added == 0) | (p.max_added > max_value) | (p.max_added < 0)) { max_addvalue = max_value; }
                            else { max_addvalue = p.max_added; }
                            if ((p.max_increased == 0) | (p.max_increased > max_value) | (p.max_increased < 0)) { max_increasedvalue = max_value; }
                            else { max_increasedvalue = p.max_increased; }
                            found = true;
                            break;
                        }
                    }
                    if (!found) { LastEpochMods.Main.logger_instance.Msg("Error : Property " + random_buff.name + " Not Found"); }
                    if (random_buff != null)
                    {
                        float old_value = 0;
                        string BuffToRemove = "";
                        foreach (Buff player_buff in playerActor.statBuffs.buffs)
                        {
                            if (player_buff.name.Contains(random_buff.name))
                            {
                                BuffToRemove = player_buff.name;
                                float new_value = 0;
                                if (!GetIsIncrease(player_buff))
                                {
                                    old_value = player_buff.stat.addedValue;
                                    if (old_value < max_addvalue)
                                    {
                                        new_value = old_value + Save_Manager.Data.UserData.Items.Headhunter.addvalue;
                                    }
                                    else { new_value = max_addvalue; }
                                    random_buff.stat.addedValue = new_value;
                                }
                                else
                                {
                                    old_value = player_buff.stat.increasedValue;
                                    if (old_value < max_increasedvalue)
                                    {
                                        new_value = old_value + Save_Manager.Data.UserData.Items.Headhunter.increasedvalue;
                                    }
                                    else { new_value = max_increasedvalue; }
                                    random_buff.stat.increasedValue = new_value;
                                }
                                break;
                            }
                        }
                        if (BuffToRemove != null) { playerActor.statBuffs.removeBuffsWithName(BuffToRemove); }
                        playerActor.statBuffs.addBuff(random_buff.remainingDuration, random_buff.stat.property,
                            random_buff.stat.addedValue, random_buff.stat.increasedValue, random_buff.stat.moreValues,
                            random_buff.stat.tags, random_buff.stat.specialTag, random_buff.name);
                    }
                }
            }
            public static void Generate_HH_BuffsList()
            {
                HH_Buff_Add = new System.Collections.Generic.List<HH_Buff>();
                HH_Buff_Increased = new System.Collections.Generic.List<HH_Buff>();
                for (int i = 0; i < System.Enum.GetValues(typeof(SP)).Length; i++)
                {
                    SP property = (SP)i;
                    foreach (HH_Buff hh_buff in Config.HH_Buff_Config)
                    {
                        if (hh_buff.property == property.ToString())
                        {
                            if (hh_buff.added) { HH_Buff_Add.Add(hh_buff); }
                            if (hh_buff.increased) { HH_Buff_Increased.Add(hh_buff); }
                            break;
                        }
                    }
                }
            }

            private static SP GetPropertyFromName(string name)
            {
                SP result = SP.None;
                for (int i = 0; i < System.Enum.GetValues(typeof(SP)).Length; i++)
                {
                    SP property = (SP)i;
                    if (name == property.ToString())
                    {
                        result = property;
                        break;
                    }
                }

                return result;
            }
            private static Buff Generate_Random_HHBuff()
            {
                Buff result = null;
                byte specialTag = 0;
                AT tags = AT.None;
                float addedValue = 0;
                float increasedValue = 0;
                string name = hh_buff + " : ";
                bool error = false;
                bool addedd = false;
                SP property = SP.None;

                if ((HH_Buff_Add.Count > 0) && (HH_Buff_Increased.Count > 0))
                {
                    int add_increase = Random.Range(0, 2);
                    if (add_increase == 0) { addedd = true; }
                }
                else if (HH_Buff_Add.Count > 0) { addedd = true; }
                else if (HH_Buff_Increased.Count > 0) { }
                else { error = true; }
                if (!error)
                {
                    if (addedd)
                    {
                        int random = Random.Range(0, HH_Buff_Add.Count);
                        if (random > HH_Buff_Add.Count) { random = HH_Buff_Add.Count - 1; }
                        string property_name = HH_Buff_Add[random].property;
                        property = GetPropertyFromName(property_name);
                        name += "Add ";
                        addedValue = Save_Manager.Data.UserData.Items.Headhunter.addvalue;
                    }
                    else
                    {
                        int random = Random.Range(0, HH_Buff_Increased.Count);
                        if (random > HH_Buff_Increased.Count) { random = HH_Buff_Increased.Count - 1; }
                        string property_name = HH_Buff_Increased[random].property;
                        property = GetPropertyFromName(property_name);
                        name += "Increased ";
                        increasedValue = Save_Manager.Data.UserData.Items.Headhunter.increasedvalue;
                    }
                    if (property != SP.None)
                    {
                        name += property.ToString();
                        result = new Buff
                        {
                            name = name,
                            remainingDuration = Save_Manager.Data.UserData.Items.Headhunter.BuffDuration,
                            stat = new Stats.Stat
                            {
                                addedValue = addedValue,
                                increasedValue = increasedValue,
                                moreValues = null,
                                property = property,
                                specialTag = specialTag,
                                tags = tags
                            }
                        };
                    }
                }

                return result;
            }
            private static bool GetIsIncrease(Buff b)
            {
                if (b.name.Contains("Increased")) { return true; }
                else { return false; }
            }
            private static System.Collections.Generic.List<HH_Buff> HH_Buff_Add = new System.Collections.Generic.List<HH_Buff>();
            private static System.Collections.Generic.List<HH_Buff> HH_Buff_Increased = new System.Collections.Generic.List<HH_Buff>();
        }
        public class Steal
        {
            public const float BuffDuration = 20f;
            public static void StealBuffFromMobs(Actor killedActor)
            {
                if ((killedActor.tag == "Enemy") && (killedActor.rarity == Actor.Rarity.Rare) && (!killedActor.data.isBossOrMiniBoss()))
                {
                    Actor playerActor = PlayerFinder.getPlayerActor();
                    foreach (Buff b in killedActor.statBuffs.buffs)
                    {
                        playerActor.statBuffs.addBuff(BuffDuration, b.stat.property, b.stat.addedValue, b.stat.increasedValue,
                            b.stat.moreValues, b.stat.tags, b.stat.specialTag, "HHBuff " + b.stat.property.ToString());
                    }
                }
            }
        }
        public class Events
        {
            public static bool SceneLoaded_Initialized = false;
            public static void Init()
            {
                if (!SceneLoaded_Initialized)
                {
                    try
                    {
                        SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
                        SceneLoaded_Initialized = true;
                    }
                    catch { }
                }
            }
            public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (Scenes_Manager.GameScene())
                {
                    if (!PlayerFinder.getPlayer().GetComponent<HHTracker>())
                    {
                        PlayerFinder.getPlayer().GetComponent<AbilityEventListener>().add_onKillEvent(OnKillAction);
                        PlayerFinder.getPlayer().GetComponent<SummonTracker>().add_minionKillEvent(OnMinionKillAction);
                        PlayerFinder.getPlayer().AddComponent<HHTracker>();
                    }
                }
            }

            private static readonly System.Action<Ability, Actor> OnKillAction = new System.Action<Ability, Actor>(OnKill);
            private static void OnKill(Ability ability, Actor killedActor)
            {
                Actor playerActor = null;
                try { playerActor = PlayerFinder.getPlayerActor(); }
                catch { }
                if (playerActor != null)
                {
                    if (playerActor.itemContainersManager.hasUniqueEquipped(Unique.unique_id))
                    {
                        if (!Save_Manager.Data.UserData.Items.Headhunter.random) { Steal.StealBuffFromMobs(killedActor); }
                        else { RandomBuffs.GenerateBuffs(); }
                    }
                }
            }
            private static readonly System.Action<Summoned, Ability, Actor> OnMinionKillAction = new System.Action<Summoned, Ability, Actor>(OnMinionKill);
            private static void OnMinionKill(Summoned summon, Ability ability, Actor killedActor)
            {
                Actor playerActor = null;
                try { playerActor = PlayerFinder.getPlayerActor(); }
                catch { }
                if (playerActor != null)
                {
                    if (playerActor.itemContainersManager.hasUniqueEquipped(Unique.unique_id))
                    {
                        if (!Save_Manager.Data.UserData.Items.Headhunter.random) { Steal.StealBuffFromMobs(killedActor); }
                        else { RandomBuffs.GenerateBuffs(); }
                    }
                }
            }
        }
        public class Ui
        {
            public static void Update()
            {
                if (Scenes_Manager.GameScene())
                {
                    if ((PlayerFinder.getPlayerActor().itemContainersManager.hasUniqueEquipped(Unique.unique_id)) && (Save_Manager.Data.UserData.Items.Headhunter.random))
                    {
                        if (Save_Manager.Data.UserData.Items.Headhunter.showui)
                        {
                            System.Collections.Generic.List<Buff> hh_buffs = GetAllHHBuffs();
                            if (hh_buffs.Count > 0)
                            {
                                InitTextures();
                                float scrollview_max_h = (hh_buffs.Count * 25) + 5;
                                bool scroll = false;
                                float pos_x = 0;
                                float pos_y = 0;
                                float size_h;
                                if (Screen.height > scrollview_max_h + 10)
                                {
                                    size_h = scrollview_max_h;
                                    pos_y = (Screen.height / 2) - (size_h / 2);
                                }
                                else
                                {
                                    size_h = Screen.height;
                                    scroll = true;
                                }
                                float size_w = 270;
                                float scrollview_w = size_w - 10;
                                if (scroll) { size_w += 20; }
                                GUI.DrawTexture(new Rect(pos_x, pos_y, size_w, size_h), windowBackground);
                                scrollview = GUI.BeginScrollView(new Rect(pos_x, pos_y, size_w, size_h), scrollview, new Rect(0, 0, scrollview_w, scrollview_max_h));

                                float scroll_pos_y = 5;
                                foreach (Buff buff in hh_buffs)
                                {
                                    GUI.DrawTexture(new Rect(pos_x, scroll_pos_y, 265, 20), texture_grey);
                                    GUI.Label(new Rect(pos_x + 5, scroll_pos_y, 60, 20), GetBuffValue(buff), Buff_Style());
                                    GUI.Label(new Rect(pos_x + 65, scroll_pos_y, 150, 20), GetPropertyName(buff), Buff_Style());
                                    GUI.Label(new Rect(pos_x + 220, scroll_pos_y, 40, 20), GetBuffDuration(buff), Buff_Style());
                                    scroll_pos_y += 25;
                                }

                                GUI.EndScrollView();
                            }
                        }
                    }
                }
            }
            public static void HHConfig(float pos_x, float pos_y)
            {
                InitTextures();
                float buff_size_h = 25;
                float scrollview_max_h = (Config.HH_Buff_Config.Count * buff_size_h) + 5;
                bool scroll = false;
                float size_h;
                float max_h = Screen.height - pos_y;
                if (max_h > scrollview_max_h + 10) { size_h = scrollview_max_h; }
                else
                {
                    size_h = max_h;
                    scroll = true;
                }
                float size_w = 435;
                float scrollview_w = size_w - 10;
                if (scroll) { size_w += 20; }
                pos_x = pos_x - size_w;
                GUI.DrawTexture(new Rect(pos_x, pos_y, size_w, size_h), windowBackground);
                scrollview_config = GUI.BeginScrollView(new Rect(pos_x, pos_y, size_w, size_h), scrollview_config, new Rect(0, 0, scrollview_w, scrollview_max_h));

                int index = -1;
                bool btn_add = false;
                bool btn_increase = false;

                float scroll_pos_y = 5;
                int i = 0;
                foreach (RandomBuffs.HH_Buff buff in Config.HH_Buff_Config)
                {
                    float scroll_pos_x = 5;
                    GUI.Label(new Rect(scroll_pos_x, scroll_pos_y, 300, 20), buff.property, Buff_Style());
                    scroll_pos_x += 305;

                    if (GUI.Button(new Rect(scroll_pos_x, scroll_pos_y, 60, 20), "Add", Button_Style(buff.added)))
                    {
                        index = i;
                        btn_add = true;
                    }
                    scroll_pos_x += 65;

                    if (GUI.Button(new Rect(scroll_pos_x, scroll_pos_y, 60, 20), "Increase", Button_Style(buff.increased)))
                    {
                        index = i;
                        btn_increase = true;
                    }
                    scroll_pos_y += 25;
                    scroll_pos_x += 65;
                    i++;
                }
                GUI.EndScrollView();

                if (index > -1)
                {
                    Mods.Items.HeadHunter.RandomBuffs.HH_Buff hh_buff = Config.HH_Buff_Config[index];
                    if (btn_add) { hh_buff.added = !hh_buff.added; }
                    if (btn_increase) { hh_buff.increased = !hh_buff.increased; }
                    Config.HH_Buff_Config[index] = hh_buff;
                    Config.SaveConfig();
                }
            }

            private static Texture2D windowBackground = null;
            private static Texture2D texture_grey = null;
            private static Texture2D texture_green = null;
            private static Vector2 scrollview = Vector2.zero;
            private static Vector2 scrollview_config = Vector2.zero;
            private static void InitTextures()
            {
                if (windowBackground == null) { windowBackground = GUI_Manager.GeneralFunctions.MakeTextureFromColor(Color.black); }
                if (texture_grey == null) { texture_grey = GUI_Manager.GeneralFunctions.MakeTextureFromColor(Color.grey); }
                if (texture_green == null) { texture_green = GUI_Manager.GeneralFunctions.MakeTextureFromColor(Color.green); }
            }
            private static GUIStyle Buff_Style()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.background = texture_grey;
                style.normal.textColor = Color.black;
                style.hover.background = texture_grey;
                style.hover.textColor = Color.black;
                style.focused.background = texture_grey;
                style.focused.textColor = Color.black;
                style.active.background = texture_grey;
                style.active.textColor = Color.black;
                style.alignment = TextAnchor.MiddleLeft;
                style.fontSize = 12;

                return style;
            }
            private static GUIStyle Button_Style(bool select)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (select) { style.normal.background = texture_green; }
                else { style.normal.background = texture_grey; }
                style.normal.textColor = Color.black;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
                style.alignment = TextAnchor.MiddleCenter;

                return style;
            }
            private static System.Collections.Generic.List<Buff> GetAllHHBuffs()
            {
                Actor playerActor = PlayerFinder.getPlayerActor();
                System.Collections.Generic.List<Buff> hh_buffs = new System.Collections.Generic.List<Buff>();
                foreach (Buff player_buff in playerActor.statBuffs.buffs)
                {
                    if (player_buff.name.Contains(RandomBuffs.hh_buff))
                    {
                        bool found = false;
                        int index = 0;
                        foreach (Buff hh_buff in hh_buffs)
                        {
                            if (player_buff.name == hh_buff.name) { found = true; break; }
                            index++;
                        }
                        if (found)
                        {
                            if (hh_buffs[index].stat.addedValue < player_buff.stat.addedValue)
                            {
                                hh_buffs[index].stat.addedValue = player_buff.stat.addedValue;
                            }
                            else if (hh_buffs[index].stat.increasedValue < player_buff.stat.increasedValue)
                            {
                                hh_buffs[index].stat.increasedValue = player_buff.stat.increasedValue;
                            }
                        }
                        else { hh_buffs.Add(player_buff); }
                    }
                }

                return hh_buffs;
            }
            private static string GetPropertyName(Buff buff)
            {
                string property = buff.stat.property.ToString();
                if (property == "ArmourMitigationAppliesToDamageOverTime") { property = "MitigationToDot"; }
                else if (property == "BlockChanceAgainstDistantEnemies") { property = "BlockChanceDistant"; }
                else if (property == "CullPercentFromWeapon") { property = "WeaponCullPercent"; }
                else if (property == "CullPercentFromPassives") { property = "PassivesCullPercent"; }
                else if (property == "DamagePerStackOfAilment") { property = "DamagePerAilment"; }
                else if (property == "DamageTakenFromNearbyEnemies") { property = "DamageTakenFromNearby"; }
                else if (property == "LessBonusDamageTakenFromCrits") { property = "LessDamageTakenFromCrits"; }
                else if (property == "ManaBeforeHealthPercent") { property = "ManaBeforeHealth"; }
                else if (property == "ManaBeforeWardPercent") { property = "ManaBeforeWard"; }
                else if (property == "ManaSpentGainedAsWard") { property = "ManaGainedAsWard"; }
                else if (property == "MaximumHealthGainedAsEnduranceThreshold") { property = "MaxHealthAsEndurance"; }
                else if (property == "MoreFreezeRatePerStackOfChill") { property = "FreezeRatePerChill"; }
                string result_property = "";
                int i = 0;
                foreach (char c in property)
                {
                    if ((System.Char.IsUpper(c)) && (i != 0)) { result_property += " " + c; }
                    else { result_property += c; }
                    i++;
                }
                string final = "";
                if (result_property.Split(' ').Length > 0)
                {
                    foreach (string s in result_property.Split(' '))
                    {
                        if ((final != "") && (s != "") && (s != "Increased")) { final += " "; }
                        if (s == "Resistance") { final += "Res"; }
                        else if (s == "Percent") { final += "%"; }
                        else if ((s != "") && (s != "Increased")) { final += s; }
                    }
                }
                else { final = "null"; }

                return final;
            }
            private static string GetBuffValue(Buff buff)
            {
                string value = "";
                if (buff.stat.addedValue > 0) { value = "+ " + buff.stat.addedValue.ToString(); }
                else { value = "+ " + System.Convert.ToInt32(buff.stat.increasedValue * 100) + " %"; }

                return value;
            }
            private static string GetBuffDuration(Buff buff)
            {
                string result = "null";
                if (buff.remainingDuration > 0)
                {
                    result = System.Convert.ToInt32(buff.remainingDuration) + " sec";
                }
                return result;
            }
        }
    }
    public class HHTracker : MonoBehaviour
    {
        public static bool Tracker_Initialized = false;
        public static void Init()
        {
            if (!Tracker_Initialized)
            {
                try
                {
                    UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<HHTracker>();
                    Tracker_Initialized = true;
                }
                catch { }
            }
        }
        public HHTracker(System.IntPtr ptr) : base(ptr) { }
    }
}