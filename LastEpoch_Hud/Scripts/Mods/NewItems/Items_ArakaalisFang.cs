using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpoch_Hud.Scripts.Mods.NewItems
{
    [RegisterTypeInIl2Cpp]
    public class Items_ArakaalisFang : MonoBehaviour
    {
        public static Items_ArakaalisFang instance { get; private set; }
        public Items_ArakaalisFang(System.IntPtr ptr) : base(ptr) { }

        public static bool Initialized = false;
        bool InGame = false;

        public static MinionUI minion_ui = null;
        public static System.Collections.Generic.List<GameObject> spiders = new System.Collections.Generic.List<GameObject>();

        void Awake()
        {
            instance = this;
            spiders = new System.Collections.Generic.List<GameObject>();
            SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
        }
        void Update()
        {
            if (!Assets.Loaded()) { Assets.Load(); }
            if (Locales.current != Locales.Selected.Unknow)
            {
                if (!Basic.AddedToBasicList) { Basic.AddToBasicList(); }
                if (!Unique.AddedToUniqueList) { Unique.AddToUniqueList(); }
                else if (!Unique.AddedToDictionary) { Unique.AddToDictionary(); }
            }
            if (!Events.OnKillEvent_Initialized) { Events.Init_OnKillEvent(); }
            if (!Events.OnMinionKillEvent_Initialized) { Events.Init_OnMinionKillEvent(); }
            if (Scenes.IsGameScene())
            {
                if (RaiseSpider.ability.IsNullOrDestroyed()) { RaiseSpider.GetAbility(); }
                if (RaiseSpider.actor_data.IsNullOrDestroyed()) { RaiseSpider.GetActorData(); }
                else if (RaiseSpider.prefab_obj.IsNullOrDestroyed()) { RaiseSpider.GetPrefab(); }
                RaiseSpider.Update();
            }
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
                if ((!Unique.Icon.IsNullOrDestroyed()) && (!RaiseSpider.Icon.IsNullOrDestroyed())) { result = true; }

                return result;
            }
            public static void Load()
            {                
                if (!Hud_Manager.asset_bundle.IsNullOrDestroyed() && !loading)
                {
                    loading = true;
                    foreach (string name in Hud_Manager.asset_bundle.GetAllAssetNames())
                    {
                        if (name.Contains("/arakaalisfang/"))
                        {
                            if (Functions.Check_Texture(name) && name.Contains("icon") && Unique.Icon.IsNullOrDestroyed())
                            {
                                Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                                Unique.Icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                            }
                            else if (Functions.Check_Texture(name) && name.Contains("minion") && RaiseSpider.Icon.IsNullOrDestroyed())
                            {
                                Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                                RaiseSpider.Icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                            }
                        }
                    }
                    loading = false;
                }
            }
        }        
        public class Basic
        {
            public static bool AddedToBasicList = false;
            public static readonly byte base_type = 6; //Dagger
            public static readonly int base_id = 15;
            public static ItemList.EquipmentItem Item()
            {
                ItemList.EquipmentItem item = new ItemList.EquipmentItem
                {
                    classRequirement = ItemList.ClassRequirement.None,
                    implicits = implicits(),
                    subClassRequirement = ItemList.SubClassRequirement.None,
                    cannotDrop = true,
                    itemTags = ItemLocationTag.None,
                    levelRequirement = 53,
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
                    case Locales.Selected.English: { result = AFLocales.SubType.en; break; }
                    case Locales.Selected.French: { result = AFLocales.SubType.fr; break; }
                    case Locales.Selected.German: { result = AFLocales.SubType.de; break; }
                    case Locales.Selected.Russian: { result = AFLocales.SubType.ru; break; }
                    case Locales.Selected.Portuguese: { result = AFLocales.SubType.pt; break; }

                    case Locales.Selected.Korean: { result = AFLocales.SubType.en; break; }
                    case Locales.Selected.Polish: { result = AFLocales.SubType.en; break; }
                    case Locales.Selected.Chinese: { result = AFLocales.SubType.en; break; }
                    case Locales.Selected.Spanish: { result = AFLocales.SubType.en; break; }
                }

                return result;
            }

            private static Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> implicits()
            {
                Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> implicits = new Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit>();
                implicits.Add(new ItemList.EquipmentImplicit
                {
                    implicitMaxValue = 0.4f,
                    implicitValue = 0.4f,
                    property = SP.CriticalChance,
                    specialTag = 0,
                    tags = AT.None,
                    type = BaseStats.ModType.INCREASED
                });

                return implicits;
            }
        }
        public class Unique
        {
            public static bool AddedToUniqueList = false;
            public static bool AddedToDictionary = false;
            public static Sprite Icon = null;
            public static readonly ushort unique_id = 508;
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
                    levelRequirement = 53,
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
                    case Locales.Selected.English: { result = AFLocales.UniqueName.en; break; }
                    case Locales.Selected.French: { result = AFLocales.UniqueName.fr; break; }
                    case Locales.Selected.German: { result = AFLocales.UniqueName.de; break; }
                    case Locales.Selected.Russian: { result = AFLocales.UniqueName.ru; break; }
                    case Locales.Selected.Portuguese: { result = AFLocales.UniqueName.pt; break; }

                    case Locales.Selected.Korean: { result = AFLocales.UniqueName.en; break; }
                    case Locales.Selected.Polish: { result = AFLocales.UniqueName.en; break; }
                    case Locales.Selected.Chinese: { result = AFLocales.UniqueName.en; break; }
                    case Locales.Selected.Spanish: { result = AFLocales.UniqueName.en; break; }
                }

                return result;
            }
            public static string Get_Unique_Description()
            {
                string result = "";
                switch (Locales.current)
                {
                    case Locales.Selected.English: { result = AFLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.French: { result = AFLocales.UniqueDescription.fr(); break; }

                    case Locales.Selected.Korean: { result = AFLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.German: { result = AFLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.Russian: { result = AFLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.Polish: { result = AFLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.Portuguese: { result = AFLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.Chinese: { result = AFLocales.UniqueDescription.en(); break; }
                    case Locales.Selected.Spanish: { result = AFLocales.UniqueDescription.en(); break; }
                }

                return result;
            }
            public static string Get_Unique_Lore()
            {
                string result = "";
                switch (Locales.current)
                {
                    case Locales.Selected.English: { result = AFLocales.Lore.en; break; }
                    case Locales.Selected.French: { result = AFLocales.Lore.fr; break; }
                    case Locales.Selected.German: { result = AFLocales.Lore.de; break; }

                    case Locales.Selected.Korean: { result = AFLocales.Lore.en; break; }
                    case Locales.Selected.Russian: { result = AFLocales.Lore.en; break; }
                    case Locales.Selected.Polish: { result = AFLocales.Lore.en; break; }
                    case Locales.Selected.Portuguese: { result = AFLocales.Lore.en; break; }
                    case Locales.Selected.Chinese: { result = AFLocales.Lore.en; break; }
                    case Locales.Selected.Spanish: { result = AFLocales.Lore.en; break; }
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
                    property = SP.Damage,
                    tags = AT.Physical,
                    type = BaseStats.ModType.INCREASED,
                    maxValue = 2f,
                    value = 1.7f,
                    hideInTooltip = false
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Damage,
                    tags = AT.Physical,
                    type = BaseStats.ModType.ADDED,
                    maxValue = 30,
                    value = 8,
                    hideInTooltip = false
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Damage,
                    tags = AT.Necrotic,
                    type = BaseStats.ModType.ADDED,
                    maxValue = 59,
                    value = 1,
                    hideInTooltip = false
                });

                return result;
            }
            private static Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> TooltipEntries()
            {
                Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> result = new Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry>();
                result.Add(new UniqueModDisplayListEntry(128));
                result.Add(new UniqueModDisplayListEntry(0));
                result.Add(new UniqueModDisplayListEntry(1));
                result.Add(new UniqueModDisplayListEntry(2));              

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
        public class RaiseSpider
        {
            public static Sprite Icon = null;
            public static int summon_limit = 20;
            public static float max_time = 30;
            public static string actor_name = "Cave Spider";
            public static string actor_data_name = "Arakaali Spider";
            public static ActorData actor_data = null;
            public static bool Initialize_ability = false;
            public static Ability ability = null;
            public static bool Initialize_prefab = false;
            public static GameObject prefab_obj = null;            

            public static void GetActorData()
            {
                foreach (ActorData ad in Resources.FindObjectsOfTypeAll<ActorData>())
                {
                    if (ad.name == actor_name)
                    {
                        actor_data = new ActorData();
                        actor_data.actorName = actor_data_name;
                        actor_data.name = actor_data_name;
                        actor_data.ActorReference = ad.ActorReference;
                        actor_data.actorType = ActorData.Type.Minion;
                        actor_data.enableAnimationSlidingFix = true;
                        actor_data.eTag = ad.eTag;
                        actor_data.eTypes = ad.eTypes;
                        actor_data.id = 999999999; //65903566 //-675722824
                        actor_data.level = 53;
                        actor_data.VisualsReference = ad.VisualsReference;
                        break;
                    }
                }
            }
            public static void GetAbility()
            {
                if (!Initialize_ability)
                {
                    Initialize_ability = true;
                    foreach (Ability ab in Resources.FindObjectsOfTypeAll<Ability>())
                    {
                        if (ab.name == "SummonPyromancer")
                        {
                            ability = new Ability
                            {
                                name = "Raise Spider",
                                abilityName = "Raise Spider",
                                abilitySprite = Icon,
                                abilityObjectRotation = Ability.AbilityObjectRotation.FacingTarget,
                                abilityObjectType = Ability.AbilityObjectType.Default,
                                animation = AbilityAnimation.CastUp,
                                attachCastingVFXToCaster = true,
                                attributeScaling = ab.attributeScaling,
                                backupControllerTargeting = ControllerTargetingID.CastMaxRangeForward,
                                baseMovementAnimationLength = 1f,
                                castingVFXPositioning = CastingVFXPositioning.Default,
                                description = "Summons a spider.",
                                manaCost = 0f,
                                minionTagsDisplay = ab.minionTagsDisplay,
                                minionLimitForActiveMinionCriteria = 20,
                                moveOrAttackFallback = Ability.MoveOrAttackFallback.Wait,
                                abilityPrefab = prefab_obj,
                                speedMultiplier = 1f,
                                speedScaler = SP.CastSpeed,
                                tags = AT.None,
                                useDelay = 0.43f,
                                useDuration = 0.75f
                            };
                            break;
                        }
                    }
                    Initialize_ability = false;
                }
            }
            public static void GetPrefab()
            {
                if (!Initialize_prefab)
                {
                    Initialize_prefab = true;
                    foreach (Ability ab in Resources.FindObjectsOfTypeAll<Ability>())
                    {
                        if (ab.name == "SummonPyromancer")
                        {
                            prefab_obj = Instantiate(ab.abilityPrefab, Vector3.zero, Quaternion.identity);
                            prefab_obj.active = false;
                            prefab_obj.name = "Raise Spider prefab";
                            SummonEntityOnDeath summon = prefab_obj.GetComponent<SummonEntityOnDeath>();
                            if (!summon.IsNullOrDestroyed())
                            {
                                summon.abilitiesThatCountForLimit = new Il2CppSystem.Collections.Generic.List<Ability>();
                                summon.ActorReference = actor_data;
                                summon.limit = 1;
                            }                            
                        }
                    }
                    Initialize_prefab = false;
                }
            }
            public static void Update()
            {
                if (!minion_ui.IsNullOrDestroyed())
                {
                    //Fix Unsummon when time < 0
                    foreach (GameObject go in spiders)
                    {
                        if (!go.IsNullOrDestroyed())
                        {
                            Summoned summoned = go.GetComponent<Summoned>();
                            if (summoned.IsNullOrDestroyed()) { go.AddComponent<Summoned>(); }

                            UnsummonAfterDelay unsummonAfterDelay = go.GetComponent<UnsummonAfterDelay>();
                            if (!unsummonAfterDelay.IsNullOrDestroyed())
                            {
                                if (!unsummonAfterDelay.enabled) { unsummonAfterDelay.enabled = true; }
                            }

                            /*SizeManager sizeManager = go.GetComponent<SizeManager>();
                            if (!sizeManager.IsNullOrDestroyed())
                            {
                                sizeManager.originalScale = new Vector3(0.2f, 0.2f, 0.2f);
                            }
                            else { Main.logger_instance.Error("sizeManager is null"); }*/
                        }
                    }

                    //Icon
                    foreach (MultiMinionCard multi_minion_card in minion_ui.multiMinionCards)
                    {
                        if (multi_minion_card.ability.abilityName == "Raise Spider")
                        {
                            multi_minion_card.image.sprite = Icon;
                        }
                    }
                }
            }
            public static void Summon(GameObject actor, Vector3 target_position)
            {
                if ((!ability.IsNullOrDestroyed()) && (!prefab_obj.IsNullOrDestroyed()))
                {
                    if (Count() < summon_limit)
                    {
                        if (ability.abilityPrefab.IsNullOrDestroyed()) { ability.abilityPrefab = Instantiate(prefab_obj, Vector3.zero, Quaternion.identity); }
                        if (!ability.abilityPrefab.IsNullOrDestroyed())
                        {                            
                            ability.abilityPrefab.active = true;
                            ability.CastAfterDelay(actor.GetComponent<AbilityObjectConstructor>(), target_position, target_position, 0f);
                        }
                    }
                    ResetTimer();
                }
            }
            public static int Count()
            {
                int count = 0;
                foreach (MultiMinionCard multi_minion_card in minion_ui.multiMinionCards)
                {
                    if (multi_minion_card.ability.abilityName == "Raise Spider")
                    {
                        //multi_minion_card.image.sprite = Icon; //already in update
                        count += multi_minion_card.currentMinionCount;
                        //count += multi_minion_card.number;
                    }
                }
                if (count == 0) { spiders = new System.Collections.Generic.List<GameObject>(); }

                return count;
            }
            public static void ResetTimer()
            {
                foreach (GameObject go in spiders)
                {
                    if (!go.IsNullOrDestroyed())
                    {
                        UnsummonAfterDelay unsummonAfterDelay = go.GetComponent<UnsummonAfterDelay>();
                        if (!unsummonAfterDelay.IsNullOrDestroyed())
                        {
                            if (unsummonAfterDelay.timeRemaining > 0) { unsummonAfterDelay.timeRemaining = max_time; }
                        }
                    }
                }
            }
            
            [HarmonyPatch(typeof(Summoned), "initialise")]
            public class Summoned_initialise
            {
                [HarmonyPostfix]
                static void Postfix(ref Summoned __instance)
                {
                    if (__instance.actor.data.name == actor_data_name)
                    {
                        __instance.gameObject.name = actor_data_name + "(Clone)";
                        spiders.Add(__instance.gameObject);

                        Emerging emerging = __instance.gameObject.GetComponent<Emerging>();
                        if (!emerging.IsNullOrDestroyed())
                        {
                            emerging.duration = 0f;
                        }
                        AbilityList ability_list = __instance.gameObject.GetComponent<AbilityList>();
                        if (!ability_list.IsNullOrDestroyed())
                        {
                            foreach (Ability ability in ability_list.abilities)
                            {
                                if (ability.abilitySprite.IsNullOrDestroyed()) { ability.abilitySprite = Icon; }
                            }
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(SummonChangeTracker), "Awake")]
            public class SummonChangeTracker_Awake
            {
                [HarmonyPostfix]
                static void Postfix(ref SummonChangeTracker __instance)
                {
                    Summoned summoned = __instance.gameObject.GetComponent<Summoned>();
                    if (!summoned.IsNullOrDestroyed())
                    {
                        if (summoned.actor.data.name == actor_data_name)
                        {
                            __instance.changeLimitDuration(true, max_time);
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(MinionUI), "Awake")]
            public class MinionUI_Awake
            {
                [HarmonyPostfix]
                static void Postfix(ref MinionUI __instance)
                {
                    if (__instance.name == "ActiveMinions")
                    {                        
                        minion_ui = __instance;
                        Vector2 original_position = __instance.originalPosition;
                        __instance.originalPosition = new Vector2(original_position.x, -20);
                        __instance.MoveUiToDefaultPosition();
                    }
                }
            }
        }
        public class AFLocales
        {
            private static string basic_subtype_name_key = "Item_SubType_Name_" + Basic.base_type + "_" + Basic.base_id;
            private static string unique_name_key = "Unique_Name_" + Unique.unique_id;
            private static string unique_description_key = "Unique_Tooltip_0_" + Unique.unique_id;
            private static string unique_lore_key = "Unique_Lore_" + Unique.unique_id;

            public class SubType
            {
                public static string en = "Rune Dagger";
                public static string fr = "Rune Dagger";
                public static string de = "Rune Dagger";
                public static string ru = "Rune Dagger";
                public static string pt = "Rune Dagger";
                //Add all languages here
            }
            public class UniqueName
            {
                public static string en = "Arakaali's Fang";
                public static string fr = "Arakaali's Fang";
                public static string de = "Arakaali's Fang";
                public static string ru = "Arakaali's Fang";
                public static string pt = "Arakaali's Fang";
                //Add all languages here
            }
            public class UniqueDescription
            {
                public static string en()
                {
                    return "100% chance to Trigger Level 1 Raise Spiders on Kill";
                }
                public static string fr()
                {
                    return "100% chance to Trigger Level 1 Raise Spiders on Kill";
                }
                //Add all languages here
            }
            public class Lore
            {
                public static readonly string en = "All children must eat.";
                public static readonly string fr = "All children must eat.";
                public static readonly string de = "All children must eat.";
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
                if (Unique.IsEquipped()) { RaiseSpider.Summon(Refs_Manager.player_actor.gameObject, Refs_Manager.player_actor.position()); }
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
                if (Unique.IsEquipped()) { RaiseSpider.Summon(Refs_Manager.player_actor.gameObject, Refs_Manager.player_actor.position()); }
            }
        }
    }
}
