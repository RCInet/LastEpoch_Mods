using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpoch_Hud.Scripts.Mods.NewItems
{
    [RegisterTypeInIl2Cpp]
    public class Items_HeadHunter : MonoBehaviour
    {
        public static Items_HeadHunter instance { get; private set; }
        public Items_HeadHunter(System.IntPtr ptr) : base(ptr) { }

        public static bool Initialized = false;
        bool InGame = false;

        void Awake()
        {
            instance = this;
            SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
        }        
        void Update()
        {
            if (!Assets.Loaded()) { Assets.Load(); }
            else if (!Initialized) { Initialized = Config.LoadConfig(); }
            if (Locales.current != Locales.Selected.Unknow)
            {
                if (!Basic.AddedToBasicList) { Basic.AddToBasicList(); }
                if (!Unique.AddedToUniqueList) { Unique.AddToUniqueList(); }
                else if (!Unique.AddedToDictionary) { Unique.AddToDictionary(); }
            }
            if (!Events.OnKillEvent_Initialized) { Events.Init_OnKillEvent(); }
            if (!Events.OnMinionKillEvent_Initialized) { Events.Init_OnMinionKillEvent(); }
            if (!UI.buffs_prefab.IsNullOrDestroyed() && UI.buffs_obj.IsNullOrDestroyed()) { UI.Init(); }
            if (Unique.IsEquipped()) { UI.UpdateBuff(); }
        }        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Scenes.IsGameScene())
            {
                if (!InGame)
                {
                    Events.OnKillEvent_Initialized = false;
                    Events.OnMinionKillEvent_Initialized = false;
                }
                InGame = true;
            }
            else if (InGame) { InGame = false; }
        }

        public class Assets
        {
            private static bool loading = false;

            public static bool Loaded()
            {
                bool result = false;
                if (!Unique.Icon.IsNullOrDestroyed() &&
                    !Config.json.IsNullOrDestroyed() &&
                    !UI.buffs_prefab.IsNullOrDestroyed() &&
                    !UI.buffs_prefab.IsNullOrDestroyed())
                {
                    result = true;
                }

                return result;
            }
            public static void Load()
            {
                if (!Hud_Manager.asset_bundle.IsNullOrDestroyed() && !loading)
                {
                    loading = true;
                    foreach (string name in Hud_Manager.asset_bundle.GetAllAssetNames())
                    {
                        if (name.Contains("/headhunter/"))
                        {
                            if (Functions.Check_Texture(name) && name.Contains("icon") && Unique.Icon.IsNullOrDestroyed())
                            {
                                Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                                Unique.Icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                            }
                            else if (Functions.Check_Json(name) && name.Contains("hh_buffs") && Config.json.IsNullOrDestroyed())
                            {
                                Config.json = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<TextAsset>();
                            }
                            else if (Functions.Check_Prefab(name) && name.Contains("buffs.prefab"))
                            {
                                UI.buffs_prefab = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<GameObject>();
                            }
                            else if (Functions.Check_Prefab(name) && name.Contains("buff.prefab"))
                            {
                                UI.buff_prefab = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<GameObject>();
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
            public static GameObject buffs_prefab = null;
            public static GameObject buffs_obj = null;
            public static GameObject buff_prefab = null;

            public static void Init()
            {
                if (!Initializing && !Refs_Manager.game_uibase.IsNullOrDestroyed())
                {
                    Initializing = true;
                    buffs_obj = Instantiate(buffs_prefab, Vector3.zero, Quaternion.identity);
                    DontDestroyOnLoad(buffs_obj);
                    buffs_obj.transform.SetParent(Refs_Manager.game_uibase.transform);
                    MoveEnemyHealthBar(55f);
                    Initializing = false;
                }
            }
            public static void MoveEnemyHealthBar(float down)
            {
                GameObject canvas_enemy_health_bar = Functions.GetChild(Refs_Manager.game_uibase.gameObject, "Canvas (enemy health bar)");
                if (!canvas_enemy_health_bar.IsNullOrDestroyed())
                {
                    GameObject enemy_health_bar = Functions.GetChild(canvas_enemy_health_bar, "EnemyHealthBar");
                    if (!enemy_health_bar.IsNullOrDestroyed())
                    {
                        float scale = enemy_health_bar.GetComponent<Transform>().lossyScale.y;
                        GameObject normal_enemy = Functions.GetChild(enemy_health_bar, "normalEnemy");
                        if (!normal_enemy.IsNullOrDestroyed())
                        {
                            Vector3 position = normal_enemy.GetComponent<Transform>().position;
                            normal_enemy.GetComponent<Transform>().position = new Vector3(position.x, (position.y - (down * scale)), position.z);
                        }
                        GameObject rare_enemy = Functions.GetChild(enemy_health_bar, "rareEnemy");
                        if (!rare_enemy.IsNullOrDestroyed())
                        {
                            Vector3 position = rare_enemy.GetComponent<Transform>().position;
                            rare_enemy.GetComponent<Transform>().position = new Vector3(position.x, (position.y - (down * scale)), position.z);
                        }
                        GameObject boss_enemy = Functions.GetChild(enemy_health_bar, "bossEnemy");
                        if (!boss_enemy.IsNullOrDestroyed())
                        {
                            Vector3 position = boss_enemy.GetComponent<Transform>().position;
                            boss_enemy.GetComponent<Transform>().position = new Vector3(position.x, (position.y - (down * scale)), position.z);
                        }
                        GameObject friendly_unit = Functions.GetChild(enemy_health_bar, "friendlyUnit");
                        if (!friendly_unit.IsNullOrDestroyed())
                        {
                            Vector3 position = friendly_unit.GetComponent<Transform>().position;
                            friendly_unit.GetComponent<Transform>().position = new Vector3(position.x, (position.y - (down * scale)), position.z);
                        }
                    }
                }
            }
            public static void AddBuff(string buff_name, int stack, SP property)
            {
                RemoveBuff(buff_name);
                if (!buffs_obj.IsNullOrDestroyed())
                {
                    GameObject buffs_content = Functions.GetChild(buffs_obj, "Panel");
                    if (!buffs_content.IsNullOrDestroyed())
                    {
                        GameObject obj = Instantiate(buff_prefab, Vector3.zero, Quaternion.identity);
                        obj.name = buff_name;
                        obj.transform.SetParent(buffs_content.transform);
                        GameObject panel_icon_obj = Functions.GetChild(obj, "Panel_Icon");
                        if (!panel_icon_obj.IsNullOrDestroyed())
                        {
                            GameObject icon_obj = Functions.GetChild(panel_icon_obj, "Icon");
                            if (!icon_obj.IsNullOrDestroyed())
                            {
                                Sprite icon = GetBuffIcon(property);
                                UnityEngine.UI.Image img = icon_obj.GetComponent<UnityEngine.UI.Image>();
                                if (!img.IsNullOrDestroyed() && !icon.IsNullOrDestroyed()) { img.sprite = icon; }
                            }
                        }
                        GameObject stack_obj = Functions.GetChild(obj, "Panel_Stack");
                        if (!stack_obj.IsNullOrDestroyed())
                        {
                            GameObject text_obj = Functions.GetChild(stack_obj, "Text (Legacy)");
                            if (!text_obj.IsNullOrDestroyed())
                            {
                                UnityEngine.UI.Text text = text_obj.GetComponent<UnityEngine.UI.Text>();
                                if (!text.IsNullOrDestroyed()) { text.text = stack.ToString(); }
                            }
                        }
                    }
                }
            }
            public static void RemoveBuff(string buff_name)
            {
                if (!buffs_obj.IsNullOrDestroyed())
                {
                    GameObject buffs_content = Functions.GetChild(buffs_obj, "Panel");
                    if (!buffs_content.IsNullOrDestroyed())
                    {
                        foreach (GameObject obj in Functions.GetAllChild(buffs_content))
                        {
                            if (obj.name == buff_name)
                            {
                                Destroy(obj);
                                //break;
                            }
                        }
                    }
                }
            }
            public static void UpdateBuff()
            {
                if (!buffs_obj.IsNullOrDestroyed())
                {
                    GameObject buffs_content = Functions.GetChild(buffs_obj, "Panel");
                    if (!buffs_content.IsNullOrDestroyed())
                    {
                        foreach (GameObject obj in Functions.GetAllChild(buffs_content))
                        {
                            bool found = false;
                            foreach (Buff buff in PlayerFinder.getPlayerActor().statBuffs.buffs)
                            {
                                if (obj.name == buff.name)
                                {
                                    found = true;
                                    GameObject icon_obj = Functions.GetChild(obj, "Panel_Icon");
                                    if (!icon_obj.IsNullOrDestroyed())
                                    {
                                        GameObject timer_obj = Functions.GetChild(icon_obj, "Timer");
                                        if (!timer_obj.IsNullOrDestroyed())
                                        {
                                            UnityEngine.UI.Image img = timer_obj.GetComponent<UnityEngine.UI.Image>();
                                            if (!img.IsNullOrDestroyed()) { img.fillAmount = buff.remainingDuration / Save_Manager.instance.data.NewItems.Headhunter.BuffDuration; }
                                        }
                                        GameObject timer__text_obj = Functions.GetChild(icon_obj, "Timer_Text");
                                        if (!timer__text_obj.IsNullOrDestroyed())
                                        {
                                            UnityEngine.UI.Text text = timer__text_obj.GetComponent<UnityEngine.UI.Text>();
                                            if (!text.IsNullOrDestroyed()) { text.text = buff.remainingDuration.ToString("0.#"); }
                                        }
                                    }
                                    break;
                                }
                            }
                            if (!found) { Destroy(obj); }
                        }
                    }
                }
            }
            public static Sprite GetBuffIcon(SP stat)
            {
                Sprite result = null;
                if (!buffs_obj.IsNullOrDestroyed())
                {
                    GameObject go = Functions.GetChild(buffs_obj, stat.ToString());
                    if (!go.IsNullOrDestroyed())
                    {
                        UnityEngine.UI.Image img = go.GetComponent<UnityEngine.UI.Image>();
                        if (!img.IsNullOrDestroyed()) { result = img.sprite; }
                    }
                }
                
                return result;
            }              
        }
        public class Basic
        {
            public static bool AddedToBasicList = false;
            public static readonly byte base_type = 2; //Belt
            public static readonly int base_id = 13;
            public static ItemList.EquipmentItem Item()
            {
                ItemList.EquipmentItem item = new ItemList.EquipmentItem
                {
                    classRequirement = ItemList.ClassRequirement.None,
                    implicits = implicits(),
                    subClassRequirement = ItemList.SubClassRequirement.None,
                    cannotDrop = true,
                    itemTags = ItemLocationTag.None,
                    levelRequirement = 40,
                    name = Get_Subtype_Name(),
                    subTypeID = base_id
                };

                return item;
            }
            
            public static void AddToBasicList()
            {
                if (!Refs_Manager.item_list.IsNullOrDestroyed())
                {
                    Refs_Manager.item_list.EquippableItems[base_type].subItems.Add(Item());
                    AddedToBasicList = true;
                }
            }
            public static string Get_Subtype_Name()
            {
                string result = "";
                switch (Locales.current)
                {
                    case Locales.Selected.English: { result = HHLocales.SubType.en; break; }
                    case Locales.Selected.French: { result = HHLocales.SubType.fr; break; }
                    case Locales.Selected.German: { result = HHLocales.SubType.de; break; }
                    case Locales.Selected.Russian: { result = HHLocales.SubType.ru; break; }
                    case Locales.Selected.Portuguese: { result = HHLocales.SubType.pt; break; }

                    case Locales.Selected.Korean: { result = HHLocales.SubType.en; break; }                    
                    case Locales.Selected.Polish: { result = HHLocales.SubType.en; break; }                    
                    case Locales.Selected.Chinese: { result = HHLocales.SubType.en; break; }
                    case Locales.Selected.Spanish: { result = HHLocales.SubType.en; break; }
                }

                return result;
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
            public static bool AddedToUniqueList = false;
            public static bool AddedToDictionary = false;
            public static Sprite Icon = null;
            public static readonly ushort unique_id = 500;
            public static UniqueList.Entry Item()
            {
                UniqueList.Entry item = new UniqueList.Entry
                {
                    name = Get_Unique_Name(),
                    displayName = Get_Unique_Name(),
                    uniqueID = unique_id,
                    isSetItem = false,
                    setID = 0,
                    overrideLevelRequirement = true,
                    levelRequirement = 40,
                    legendaryType = LegendaryType(),
                    overrideEffectiveLevelForLegendaryPotential = true,
                    effectiveLevelForLegendaryPotential = 0,
                    canDropRandomly = true,
                    rerollChance = 1,
                    itemModelType = UniqueList.ItemModelType.Unique,
                    subTypeForIM = 0,
                    baseType = Basic.base_type,
                    subTypes = SubType(),
                    mods = Mods(),
                    tooltipDescriptions = TooltipDescription(),
                    loreText = Get_Unique_Lore(),
                    tooltipEntries = TooltipEntries(),
                    oldSubTypeID = 0,
                    oldUniqueID = 0
                };

                return item;
            }
            
            public static void AddToUniqueList()
            {
                if (!Refs_Manager.unique_list.IsNullOrDestroyed())
                {
                    UniqueList.getUnique(0);
                    Refs_Manager.unique_list.uniques.Add(Item());
                    AddedToUniqueList = true;
                }
            }      
            public static void AddToDictionary()
            {
                if (!Refs_Manager.unique_list.IsNullOrDestroyed())
                {
                    UniqueList.Entry item = null;
                    if (Refs_Manager.unique_list.uniques.Count > 1)
                    {
                        foreach (UniqueList.Entry unique in Refs_Manager.unique_list.uniques)
                        {
                            if (unique.uniqueID == unique_id && unique.name == Get_Unique_Name())
                            {
                                item = unique;
                                break;
                            }
                        }
                    }
                    if (!item.IsNullOrDestroyed())
                    {
                        Refs_Manager.unique_list.entryDictionary.Add(unique_id, item);
                        AddedToDictionary = true;
                    }
                }
            }
            public static string Get_Unique_Name()
            {
                string result = "";
                switch (Locales.current)
                {
                    case Locales.Selected.English: { result = HHLocales.UniqueName.en; break; }
                    case Locales.Selected.French: { result = HHLocales.UniqueName.fr; break; }
                    case Locales.Selected.German: { result = HHLocales.UniqueName.de; break; }
                    case Locales.Selected.Russian: { result = HHLocales.UniqueName.ru; break; }
                    case Locales.Selected.Portuguese: { result = HHLocales.UniqueName.pt; break; }

                    case Locales.Selected.Korean: { result = HHLocales.UniqueName.en; break; }                    
                    case Locales.Selected.Polish: { result = HHLocales.UniqueName.en; break; }                    
                    case Locales.Selected.Chinese: { result = HHLocales.UniqueName.en; break; }
                    case Locales.Selected.Spanish: { result = HHLocales.UniqueName.en; break; }
                }
                
                return result;
            }
            public static string Get_Unique_Description()
            {
                string result = "";
                switch (Locales.current)
                {
                    case Locales.Selected.English: { result = HHLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.French: { result = HHLocales.UniqueDescription.fr(); break; }
                    
                    case Locales.Selected.Korean: { result = HHLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.German: { result = HHLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.Russian: { result = HHLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.Polish: { result = HHLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.Portuguese: { result = HHLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.Chinese: { result = HHLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.Spanish: { result = HHLocales.UniqueDescription.en(); break; }
                }

                return result;
            }
            public static string Get_Unique_Lore()
            {
                string result = "";
                switch (Locales.current)
                {
                    case Locales.Selected.English: { result = HHLocales.Lore.en; break; }
                    case Locales.Selected.French: { result = HHLocales.Lore.fr; break; }
                    case Locales.Selected.German: { result = HHLocales.Lore.de; break; }

                    case Locales.Selected.Korean: { result = HHLocales.Lore.en; break; }                    
                    case Locales.Selected.Russian: { result = HHLocales.Lore.en; break; }
                    case Locales.Selected.Polish: { result = HHLocales.Lore.en; break; }
                    case Locales.Selected.Portuguese: { result = HHLocales.Lore.en; break; }
                    case Locales.Selected.Chinese: { result = HHLocales.Lore.en; break; }
                    case Locales.Selected.Spanish: { result = HHLocales.Lore.en; break; }
                }

                return result;
            }
            public static bool IsEquipped()
            {
                bool result = false;
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {
                    if (Refs_Manager.player_actor.itemContainersManager.hasUniqueEquipped(unique_id)) { result = true; }
                }

                return result;
            }
            public static void Update_LegendaryType()
            {
                UniqueList.Entry item = UniqueList.getUnique(unique_id);
                if (!item.IsNullOrDestroyed())
                {
                    item.legendaryType = LegendaryType();
                }
            }

            private static Il2CppSystem.Collections.Generic.List<byte> SubType()
            {
                Il2CppSystem.Collections.Generic.List<byte> result = new Il2CppSystem.Collections.Generic.List<byte>();
                result.Add((byte)Basic.base_id);

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
                    value = 40,
                    hideInTooltip = false
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Dexterity,
                    tags = AT.None,
                    type = BaseStats.ModType.ADDED,
                    maxValue = 55,
                    value = 40,
                    hideInTooltip = false
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Health,
                    tags = AT.None,
                    type = BaseStats.ModType.ADDED,
                    maxValue = 60,
                    value = 50,
                    hideInTooltip = false
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Damage,
                    tags = AT.None,
                    type = BaseStats.ModType.INCREASED,
                    maxValue = 0.3f,
                    value = 0.2f,
                    hideInTooltip = false
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
                result.Add(new ItemTooltipDescription { description = Get_Unique_Description() });
                
                return result;
            }
            private static UniqueList.LegendaryType LegendaryType()
            {
                UniqueList.LegendaryType legendaryType = UniqueList.LegendaryType.LegendaryPotential;
                if (Save_Manager.instance.data.NewItems.Headhunter.WeaverWill) { legendaryType = UniqueList.LegendaryType.WeaversWill; }

                return legendaryType;
            }
        
            [HarmonyPatch(typeof(InventoryItemUI), "SetImageSpritesAndColours")]
            public class InventoryItemUI_SetImageSpritesAndColours
            {
                [HarmonyPostfix]
                static void Postfix(ref InventoryItemUI __instance)
                {
                    if (__instance.EntryRef.data.getAsUnpacked().FullName == Get_Unique_Name() && !Icon.IsNullOrDestroyed())
                    {
                        __instance.contentImage.sprite = Icon;
                    }
                }
            }

            [HarmonyPatch(typeof(UITooltipItem), "GetItemSprite")]
            public class UITooltipItem_GetItemSprite
            {
                [HarmonyPostfix]
                static void Postfix(ref Sprite __result, ItemData __0)
                {
                    if (__0.getAsUnpacked().FullName == Get_Unique_Name() && !Icon.IsNullOrDestroyed())
                    {
                        __result = Icon;
                    }
                }
            }
        }
        public class Config
        {
            public static TextAsset json = null;
            public static System.Collections.Generic.List<RandomBuffs.HH_Buff> HH_Buff_Config = new System.Collections.Generic.List<RandomBuffs.HH_Buff>();
            public static bool LoadConfig()
            {
                bool result = false;
                if (!json.IsNullOrDestroyed())
                {
                    HH_Buff_Config = JsonConvert.DeserializeObject<System.Collections.Generic.List<RandomBuffs.HH_Buff>>(json.text);
                    RandomBuffs.Generate_HH_BuffsList();
                    result = true;
                }
                else { Main.logger_instance.Error("json is null"); }
                
                return result;
            }
        }
        public class RandomBuffs
        {
            public struct HH_Buff
            {
                public string property;
                public bool added;
                public bool increased;
            }
            public static string hh_buff = "HHBuff";
            public static void GenerateBuffs()
            {
                int NbBuff = Random.Range(Save_Manager.instance.data.NewItems.Headhunter.MinGenerated, Save_Manager.instance.data.NewItems.Headhunter.MaxGenerated + 1);
                for (int i = 0; i < NbBuff; i++)
                {
                    Buff random_buff = Generate_Random_HHBuff();
                    int max_stack = (int)Save_Manager.instance.data.NewItems.Headhunter.Stack;
                    if (max_stack == 0)
                    {
                        max_stack = 1;
                        Save_Manager.instance.data.NewItems.Headhunter.Stack = 1;
                    }
                    if (random_buff != null)
                    {
                        //player
                        UpdateBuff(PlayerFinder.getPlayerActor(), random_buff, max_stack);
                        
                        //Summons
                        if (!Refs_Manager.summon_tracker.IsNullOrDestroyed())
                        {
                            foreach (Summoned summon in Refs_Manager.summon_tracker.summons)
                            {
                                UpdateBuff(summon.actor, random_buff, max_stack);
                            }
                        }
                    }
                }
            }
            public static void UpdateBuff(Actor actor, Buff random_buff, int max_stack)
            {
                if (!actor.IsNullOrDestroyed())
                {
                    float old_value = 0;
                    string BuffToRemove = "";
                    int stack = 1;
                    foreach (Buff buff in actor.statBuffs.buffs)
                    {
                        if (buff.name.Contains(random_buff.name))
                        {
                            BuffToRemove = buff.name;
                            float new_value = 0;
                            if (!GetIsIncrease(buff))
                            {
                                old_value = buff.stat.addedValue;
                                if (old_value < Save_Manager.instance.data.NewItems.Headhunter.AddValue * max_stack)
                                {
                                    new_value = old_value + Save_Manager.instance.data.NewItems.Headhunter.AddValue;
                                    stack = (int)(new_value / Save_Manager.instance.data.NewItems.Headhunter.AddValue);
                                }
                                else
                                {
                                    new_value = Save_Manager.instance.data.NewItems.Headhunter.AddValue * max_stack;
                                    stack = max_stack;
                                }
                                random_buff.stat.addedValue = new_value;
                            }
                            else
                            {
                                old_value = buff.stat.increasedValue;
                                if (old_value < Save_Manager.instance.data.NewItems.Headhunter.IncreasedValue * max_stack)
                                {
                                    new_value = old_value + Save_Manager.instance.data.NewItems.Headhunter.IncreasedValue;
                                    stack = (int)(new_value / Save_Manager.instance.data.NewItems.Headhunter.IncreasedValue);
                                }
                                else
                                {
                                    new_value = Save_Manager.instance.data.NewItems.Headhunter.IncreasedValue * max_stack;
                                    stack = max_stack;
                                }
                                random_buff.stat.increasedValue = new_value;
                            }
                            break;
                        }
                    }
                    if (BuffToRemove != "") { actor.statBuffs.removeBuffsWithName(BuffToRemove); }
                    actor.statBuffs.addBuff(random_buff.remainingDuration, random_buff.stat.property,
                        random_buff.stat.addedValue, random_buff.stat.increasedValue, random_buff.stat.moreValues,
                        random_buff.stat.tags, random_buff.name);
                    //actor.statBuffs.addBuff(random_buff.remainingDuration, random_buff.stat.property,
                    //      random_buff.stat.addedValue, random_buff.stat.increasedValue, random_buff.stat.moreValues,
                    //      random_buff.stat.tags, random_buff.stat.specialTag, random_buff.name);
                    if (actor == PlayerFinder.getPlayerActor()) { UI.AddBuff(random_buff.name, stack, random_buff.stat.property); }
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

                if (HH_Buff_Add.Count > 0 && HH_Buff_Increased.Count > 0)
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
                        addedValue = Save_Manager.instance.data.NewItems.Headhunter.AddValue;
                    }
                    else
                    {
                        int random = Random.Range(0, HH_Buff_Increased.Count);
                        if (random > HH_Buff_Increased.Count) { random = HH_Buff_Increased.Count - 1; }
                        string property_name = HH_Buff_Increased[random].property;
                        property = GetPropertyFromName(property_name);
                        name += "Increased ";
                        increasedValue = Save_Manager.instance.data.NewItems.Headhunter.IncreasedValue;
                    }
                    if (property != SP.None)
                    {
                        name += property.ToString();
                        result = new Buff
                        {
                            name = name,
                            remainingDuration = Save_Manager.instance.data.NewItems.Headhunter.BuffDuration,
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
        public class HHLocales
        {
            private static string basic_subtype_name_key = "Item_SubType_Name_" + Basic.base_type + "_" + Basic.base_id;
            private static string unique_name_key = "Unique_Name_" + Unique.unique_id;
            private static string unique_description_key = "Unique_Tooltip_0_" + Unique.unique_id;
            private static string unique_lore_key = "Unique_Lore_" + Unique.unique_id;

            public class SubType
            {
                public static string en = "HH Leather belt";
                public static string fr = "HH Ceinture en cuir";
                public static string de = "HH Ledergürtel";
                public static string ru = "HH Ремень";
                public static string pt = "Cinto de Couro";
                //Add all languages here
            }
            public class UniqueName
            {
                public static string en = "Headhunter";
                public static string fr = "Chasseur de têtes";
                public static string de = "Kopfjäger";
                public static string ru = "Охотник за головами";
                public static string pt = "Caçador de Cabeças";
                //Add all languages here
            }
            public class UniqueDescription
            {
                public static string en()
                {
                    return "When you or your minions Kill a monster, you gain " + Save_Manager.instance.data.NewItems.Headhunter.MinGenerated +
                    " to " + Save_Manager.instance.data.NewItems.Headhunter.MaxGenerated + " random Modifiers for " +
                    Save_Manager.instance.data.NewItems.Headhunter.BuffDuration + " seconds";
                }
                public static string fr()
                {
                    return "Lorsque vous ou vos serviteurs tuez un monstre, vous gagnez " + Save_Manager.instance.data.NewItems.Headhunter.MinGenerated +
                    " à " + Save_Manager.instance.data.NewItems.Headhunter.MaxGenerated + " modificateurs aléatoires pendant " +
                    Save_Manager.instance.data.NewItems.Headhunter.BuffDuration + " secondes.";
                }
                //Add all languages here
            }
            public class Lore
            {
                public static readonly string en = "A man's soul rules from a cavern of bone, learns and\r\njudges through flesh-born windows. The heart is meat.\r\nThe head is where the Man is.\"\r\n- Lavianga, Advisor to Kaom";
                public static readonly string fr = "L'âme d'un homme règne depuis une caverne d'os,\r\napprend et juge à travers des fenêtres plantées dans la chair.\r\nLe cœur est un morceau de viande. La tête est le siège de l'homme.\r\n- Lavianga, conseiller de Kaom";
                public static readonly string de = "Die Seele eines Mannes regiert\r\naus einer Höhle aus Knochen,\r\nlernt und urteilt aus Fenstern,\r\ngeboren aus Fleisch. Das Herz ist Fleisch.\r\nDer Kopf ist dort, wo der Mann ist.\r\n– Lavianga, Berater von Kaom";
                //Add all languages here
            }

            [HarmonyPatch(typeof(Localization), "TryGetText")]
            public class Localization_TryGetText
            {
                [HarmonyPrefix]
                static bool Prefix(ref bool __result, string __0) //, Il2CppSystem.String __1)
                {
                    bool result = true;
                    if (__0 == basic_subtype_name_key || __0 == unique_name_key ||
                        __0 == unique_description_key || __0 == unique_lore_key)
                    {
                        __result = true;
                        result = false;
                    }

                    return result;
                }
            }

            [HarmonyPatch(typeof(Localization), "GetText")]
            public class Localization_GetText
            {
                [HarmonyPrefix]
                static bool Prefix(ref string __result, string __0)
                {
                    bool result = true;
                    if (__0 == basic_subtype_name_key)
                    {
                        __result = Basic.Get_Subtype_Name();
                        result = false;
                    }
                    else if (__0 == unique_name_key)
                    {
                        __result = Unique.Get_Unique_Name();
                        result = false;
                    }
                    else if (__0 == unique_description_key)
                    {
                        string description = Unique.Get_Unique_Description();
                        if (description != "")
                        {
                            __result = description;
                            result = false;
                        }
                    }
                    else if (__0 == unique_lore_key)
                    {
                        string lore = Unique.Get_Unique_Lore();
                        if (lore != "")
                        {
                            __result = lore;
                            result = false;
                        }
                    }

                    return result;
                }
            }
        }
        public class Events
        {
            public static bool OnKillEvent_Initialized = false;
            public static void Init_OnKillEvent()
            {
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {
                    if (!Refs_Manager.player_actor.gameObject.IsNullOrDestroyed())
                    {
                        AbilityEventListener listener = Refs_Manager.player_actor.gameObject.GetComponent<AbilityEventListener>();
                        if (!listener.IsNullOrDestroyed())
                        {
                            listener.add_onKillEvent(OnKillAction);
                            OnKillEvent_Initialized = true;
                        }
                    }
                }
            }
            private static readonly System.Action<Ability, Actor> OnKillAction = new System.Action<Ability, Actor>(OnKill);
            private static void OnKill(Ability ability, Actor killedActor)
            {
                if (Unique.IsEquipped()) { RandomBuffs.GenerateBuffs(); }
            }

            public static bool OnMinionKillEvent_Initialized = false;
            public static void Init_OnMinionKillEvent()
            {
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {
                    if (!Refs_Manager.player_actor.gameObject.IsNullOrDestroyed())
                    {
                        SummonTracker listener = Refs_Manager.player_actor.gameObject.GetComponent<SummonTracker>();
                        if (!listener.IsNullOrDestroyed())
                        {
                            listener.add_minionKillEvent(OnMinionKillAction);
                            OnMinionKillEvent_Initialized = true;
                        }
                    }
                }
            }
            private static readonly System.Action<Summoned, Ability, Actor> OnMinionKillAction = new System.Action<Summoned, Ability, Actor>(OnMinionKill);
            private static void OnMinionKill(Summoned summon, Ability ability, Actor killedActor)
            {
                if (Unique.IsEquipped()) { RandomBuffs.GenerateBuffs(); }
            }
        }        
    }
}
