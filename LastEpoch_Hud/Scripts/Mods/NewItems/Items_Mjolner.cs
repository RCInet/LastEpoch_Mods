//______________________________________________________________________//
//https://discord.com/channels/1366160878579351756/1372660677491036272
//https://github.com/zakt4n

using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.Mods.Localization;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpoch_Hud.Scripts.Mods.NewItems
{
    [RegisterTypeInIl2Cpp]
    public class Items_Mjolner : MonoBehaviour
    {
        public static Items_Mjolner instance { get; private set; }
        public Items_Mjolner(System.IntPtr ptr) : base(ptr) { }

        private bool InGame = false;

        void Awake()
        {
            instance = this;
            SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
        }
        void Update()
        {
            Icon.Update();
            Unique.Update();
            Events.Update();
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Scenes.IsGameScene())
            {
                Trigger.Initialize_SocketedSkills();
                if (!InGame) { Events.Reset(); }
                InGame = true;
            }
            else if (InGame) { InGame = false; }
        }
               
        private class Basic
        {
            internal static readonly byte base_type = 7;  //Mace
            internal static readonly int base_id = 10;    //Rune hammer
        }
        public class Unique
        {
            internal static readonly ushort unique_id = 501;
            internal static void Update()
            {
                if (LastEpoch_Hud.Locales.current != LastEpoch_Hud.Locales.Selected.Unknow)
                {
                    if (!AddedToUniqueList) { AddToUniqueList(); }
                    else if (!AddedToDictionary) { AddToDictionary(); }
                }
            }
            public static void Update_LegendaryType(bool weaverwill)
            {
                UniqueList.Entry item = UniqueList.getUnique(unique_id);
                if (!item.IsNullOrDestroyed())
                {
                    if (weaverwill) { item.legendaryType = UniqueList.LegendaryType.WeaversWill; }
                    else { item.legendaryType = UniqueList.LegendaryType.LegendaryPotential; }
                }
            }

            private static bool AddedToUniqueList = false;
            private static bool AddedToDictionary = false;
            private static UniqueList.Entry Item()
            {
                string name = Locales.Get_UniqueName();

                UniqueList.LegendaryType legendaryType = UniqueList.LegendaryType.LegendaryPotential;
                if (Save_Manager.instance.data.NewItems.Mjolner.WeaverWill) { legendaryType = UniqueList.LegendaryType.WeaversWill; }

                Il2CppSystem.Collections.Generic.List<byte> subtypes = new Il2CppSystem.Collections.Generic.List<byte>();
                byte r = (byte)Basic.base_id;
                subtypes.Add(r);

                Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();
                mods.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Damage,
                    tags = AT.Lightning,
                    type = BaseStats.ModType.INCREASED,
                    maxValue = 1.0f,
                    value = 0.8f
                });
                mods.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Damage,
                    tags = AT.Physical,
                    type = BaseStats.ModType.INCREASED,
                    maxValue = 1.2f,
                    value = 0.8f
                });

                Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> tooltip_description = new Il2CppSystem.Collections.Generic.List<ItemTooltipDescription>();
                tooltip_description.Add(new ItemTooltipDescription { description = Locales.Get_UniqueDescription() });

                Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> entries = new Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry>();
                entries.Add(new UniqueModDisplayListEntry(0));
                entries.Add(new UniqueModDisplayListEntry(1));
                entries.Add(new UniqueModDisplayListEntry(128));

                UniqueList.Entry item = new UniqueList.Entry
                {
                    name = name,
                    displayName = name,
                    uniqueID = unique_id,
                    isSetItem = false,
                    setID = 0,
                    overrideLevelRequirement = false,
                    levelRequirement = 78,
                    legendaryType = legendaryType,
                    overrideEffectiveLevelForLegendaryPotential = true,
                    effectiveLevelForLegendaryPotential = 60,
                    canDropRandomly = true,
                    rerollChance = 1,
                    itemModelType = UniqueList.ItemModelType.Unique,
                    subTypeForIM = 0,
                    baseType = Basic.base_type,
                    subTypes = subtypes,
                    mods = mods,
                    tooltipDescriptions = tooltip_description,
                    loreText = Locales.Get_UniqueLore(),
                    tooltipEntries = entries,
                    oldSubTypeID = 0,
                    oldUniqueID = 0
                };

                return item;
            }
            private static void AddToUniqueList()
            {
                if (!AddedToUniqueList && !Refs_Manager.unique_list.IsNullOrDestroyed())
                {
                    try
                    {
                        UniqueList.getUnique(0); //force initialize uniquelist
                        Refs_Manager.unique_list.uniques.Add(Item());
                        AddedToUniqueList = true;
                    }
                    catch { Main.logger_instance?.Error("Mjolner Unique List Error"); }
                }
            }
            private static void AddToDictionary()
            {
                if (AddedToUniqueList && !AddedToDictionary && !Refs_Manager.unique_list.IsNullOrDestroyed())
                {
                    try
                    {
                        UniqueList.Entry item = null;
                        if (Refs_Manager.unique_list.uniques.Count > 1)
                        {
                            foreach (UniqueList.Entry unique in Refs_Manager.unique_list.uniques)
                            {
                                if (unique.uniqueID == unique_id && unique.name == Locales.Get_UniqueName())
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
                    catch { Main.logger_instance?.Error("Mjolner Unique Dictionary Error"); }
                }
            }
        }
        public class Icon
        {
            internal static void Update()
            {
                if (sprite.IsNullOrDestroyed()) { Get_UniqueIcon(); }
            }

            public static Sprite sprite = null;
            private static bool loading = false;
            private static void Get_UniqueIcon()
            {
                if (!loading && !Hud_Manager.asset_bundle.IsNullOrDestroyed())
                {
                    loading = true;
                    foreach (string name in Hud_Manager.asset_bundle.GetAllAssetNames())
                    {
                        if (name.Contains("mjolner.png"))
                        {
                            Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                            break;
                        }
                    }
                    loading = false;
                }
            }

            private class Hooks
            {
                [HarmonyPatch(typeof(InventoryItemUI), "SetImageSpritesAndColours")]
                private class InventoryItemUI_SetImageSpritesAndColours
                {
                    [HarmonyPostfix]
                    static void Postfix(ref InventoryItemUI __instance)
                    {
                        if (__instance.EntryRef.data.getAsUnpacked().FullName == Locales.Get_UniqueName() && !sprite.IsNullOrDestroyed())
                        {
                            __instance.contentImage.sprite = sprite;
                        }
                    }
                }

                [HarmonyPatch(typeof(UITooltipItem), "GetItemSprite")]
                private class UITooltipItem_GetItemSprite
                {
                    [HarmonyPostfix]
                    static void Postfix(ref Sprite __result, ItemData __0)
                    {
                        if (__0.getAsUnpacked().FullName == Locales.Get_UniqueName() && !sprite.IsNullOrDestroyed())
                        {
                            __result = sprite;
                        }
                    }
                }
            }
        }
        public class Locales
        {
            internal static string Get_UniqueName()
            {
                string name = "";
                switch (LastEpoch_Hud.Locales.current)
                {
                    case LastEpoch_Hud.Locales.Selected.English: { name = "Mjölner"; break; }
                    case LastEpoch_Hud.Locales.Selected.French: { name = "Mjölner"; break; }
                    case LastEpoch_Hud.Locales.Selected.German: { name = "Mjölner"; break; }
                    case LastEpoch_Hud.Locales.Selected.Russian: { name = "Мьёльнир"; break; }
                    case LastEpoch_Hud.Locales.Selected.Portuguese: { name = "Mjölner"; break; }
                    case LastEpoch_Hud.Locales.Selected.Korean: { name = "Mjölner"; break; }
                    case LastEpoch_Hud.Locales.Selected.Polish: { name = "Mjölner"; break; }
                    case LastEpoch_Hud.Locales.Selected.Chinese: { name = "Mjölner"; break; }
                    case LastEpoch_Hud.Locales.Selected.Spanish: { name = "Mjölner"; break; }
                }

                return name;
            }
            internal static string Get_UniqueDescription()
            {
                string description = "";
                int str_requirement = Save_Manager.instance.data.NewItems.Mjolner.StrRequirement;
                int int_requirement = Save_Manager.instance.data.NewItems.Mjolner.IntRequirement;
                int display_min_chance = (int)(Save_Manager.instance.data.NewItems.Mjolner.MinTriggerChance * 100f);
                int display_max_chance = (int)(Save_Manager.instance.data.NewItems.Mjolner.MaxTriggerChance * 100f);
                double cooldown = Save_Manager.instance.data.NewItems.Mjolner.SocketedCooldown; // / 1000);
                string skill_0 = Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_0;
                string skill_1 = Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_1;
                string skill_2 = Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_2;
                switch (LastEpoch_Hud.Locales.current)
                {
                    case LastEpoch_Hud.Locales.Selected.English: { description = "If you have at least " + str_requirement + " Strength and " + int_requirement + " Intelligence, " + display_min_chance + " to " + display_max_chance + "% chance to Trigger " + skill_0 + ", " + skill_1 + " and " + skill_2 + " on Hit, with a " + cooldown + " second Cooldown"; break; }
                    case LastEpoch_Hud.Locales.Selected.French: { description = "Si voud avez au moins " + str_requirement + " de Force et " + int_requirement + " d'Intelligence, " + display_min_chance + " à " + display_max_chance + "% de chances de Déclencher " + skill_0 + ", " + skill_1 + " et " + skill_2 + " à l'impact, avec un temps de recharge de " + cooldown + " seconde(s)"; break; }
                    case LastEpoch_Hud.Locales.Selected.Korean: { description = "If you have at least " + str_requirement + " Strength and " + int_requirement + " Intelligence, " + display_min_chance + " to " + display_max_chance + "% chance to Trigger " + skill_0 + ", " + skill_1 + " and " + skill_2 + " on Hit, with a " + cooldown + " second Cooldown"; break; }
                    case LastEpoch_Hud.Locales.Selected.German: { description = "If you have at least " + str_requirement + " Strength and " + int_requirement + " Intelligence, " + display_min_chance + " to " + display_max_chance + "% chance to Trigger " + skill_0 + ", " + skill_1 + " and " + skill_2 + " on Hit, with a " + cooldown + " second Cooldown"; break; }
                    case LastEpoch_Hud.Locales.Selected.Russian: { description = "If you have at least " + str_requirement + " Strength and " + int_requirement + " Intelligence, " + display_min_chance + " to " + display_max_chance + "% chance to Trigger " + skill_0 + ", " + skill_1 + " and " + skill_2 + " on Hit, with a " + cooldown + " second Cooldown"; break; }
                    case LastEpoch_Hud.Locales.Selected.Polish: { description = "If you have at least " + str_requirement + " Strength and " + int_requirement + " Intelligence, " + display_min_chance + " to " + display_max_chance + "% chance to Trigger " + skill_0 + ", " + skill_1 + " and " + skill_2 + " on Hit, with a " + cooldown + " second Cooldown"; break; }
                    case LastEpoch_Hud.Locales.Selected.Portuguese: { description = "If you have at least " + str_requirement + " Strength and " + int_requirement + " Intelligence, " + display_min_chance + " to " + display_max_chance + "% chance to Trigger " + skill_0 + ", " + skill_1 + " and " + skill_2 + " on Hit, with a " + cooldown + " second Cooldown"; break; }
                    case LastEpoch_Hud.Locales.Selected.Chinese: { description = "If you have at least " + str_requirement + " Strength and " + int_requirement + " Intelligence, " + display_min_chance + " to " + display_max_chance + "% chance to Trigger " + skill_0 + ", " + skill_1 + " and " + skill_2 + " on Hit, with a " + cooldown + " second Cooldown"; break; }
                    case LastEpoch_Hud.Locales.Selected.Spanish: { description = "If you have at least " + str_requirement + " Strength and " + int_requirement + " Intelligence, " + display_min_chance + " to " + display_max_chance + "% chance to Trigger " + skill_0 + ", " + skill_1 + " and " + skill_2 + " on Hit, with a " + cooldown + " second Cooldown"; break; }
                }

                return description;
            }
            internal static string Get_UniqueLore()
            {
                string lore = "";
                switch (LastEpoch_Hud.Locales.current)
                {
                    case LastEpoch_Hud.Locales.Selected.English: { lore = "Look the storm in the eye and you will have its respect."; break; }
                    case LastEpoch_Hud.Locales.Selected.French: { lore = "Entrez dans l'œil de la tempête et vous gagnerez son respect."; break; }
                    case LastEpoch_Hud.Locales.Selected.German: { lore = "Blickt dem Sturm ins Auge,\r\nund sein Respekt ist Euch gewiss."; break; }
                    case LastEpoch_Hud.Locales.Selected.Korean: { lore = "Look the storm in the eye and you will have its respect."; break; }
                    case LastEpoch_Hud.Locales.Selected.Russian: { lore = "Look the storm in the eye and you will have its respect."; break; }
                    case LastEpoch_Hud.Locales.Selected.Polish: { lore = "Look the storm in the eye and you will have its respect."; break; }
                    case LastEpoch_Hud.Locales.Selected.Portuguese: { lore = "Encare o olho da tempestade, e ela te respeitará."; break; }
                    case LastEpoch_Hud.Locales.Selected.Chinese: { lore = "Look the storm in the eye and you will have its respect."; break; }
                    case LastEpoch_Hud.Locales.Selected.Spanish: { lore = "Look the storm in the eye and you will have its respect."; break; }
                }

                return lore;
            }

            private class Keys
            {
                internal static string unique_name = "Unique_Name_" + Unique.unique_id;
                internal static string unique_description = "Unique_Tooltip_0_" + Unique.unique_id;
                internal static string unique_lore = "Unique_Lore_" + Unique.unique_id;
            }
            public static void RegisterLocales()
            {
                LocalizationOverride.Register(Keys.unique_name, Get_UniqueName);
                LocalizationOverride.Register(Keys.unique_description, Get_UniqueDescription);
                LocalizationOverride.Register(Keys.unique_lore, Get_UniqueLore);
            }
        }
        public class Trigger
        {
            public static void Initialize_SocketedSkills()
            {
                if (!Initializing)
                {
                    Initializing = true;
                    Abilities = new Ability[3];
                    Times = new System.DateTime[3];
                    if (!Refs_Manager.ability_manager.IsNullOrDestroyed())
                    {
                        int i = 0;
                        foreach (Ability ability in Refs_Manager.ability_manager.abilities)
                        {
                            if (!ability.IsNullOrDestroyed() && i < 3)
                            {
                                if (ability.abilityName == Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_0 ||
                                    ability.abilityName == Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_1 ||
                                    ability.abilityName == Save_Manager.instance.data.NewItems.Mjolner.SockectedSkill_2)
                                {
                                    Abilities[i] = ability;
                                    Times[i] = System.DateTime.Now;
                                    i++;
                                }
                            }
                        }
                    }
                    Initializing = false;
                }
            }
            internal static void SocketedSkills(Actor hitActor)
            {
                if (!hitActor.IsNullOrDestroyed() && !trigger)
                {
                    trigger = true;
                    for (int i = 0; i < Abilities.Length; i++)
                    {
                        if (!Abilities[i].IsNullOrDestroyed() && i < Times.Length)
                        {
                            bool run = false;
                            double cd = Save_Manager.instance.data.NewItems.Mjolner.SocketedCooldown;
                            if (cd < 1) { cd = 1; }
                            if ((System.DateTime.Now - Times[i]).TotalSeconds > cd) { run = true; }
                            if (run)
                            {
                                float item_roll = Random.Range(Save_Manager.instance.data.NewItems.Mjolner.MinTriggerChance, Save_Manager.instance.data.NewItems.Mjolner.MaxTriggerChance);
                                float roll_percent = Random.Range(0f, 1f);
                                if (roll_percent <= item_roll)
                                {
                                    float backup_manacost = Abilities[i].manaCost;
                                    Abilities[i].manaCost = 0; //Remove ManaCost
                                    Abilities[i].castAtTargetFromConstructorAfterDelay(Refs_Manager.player_actor.abilityObjectConstructor, Vector3.zero, hitActor.position(), 0, UseType.Indirect);
                                    Abilities[i].manaCost = backup_manacost; //Reset ManaCost
                                }
                                Times[i] = System.DateTime.Now;
                            }
                        }
                    }
                    trigger = false;
                }
            }            

            public static Ability[] Abilities = null;
            private static System.DateTime[] Times = null;
            private static bool Initializing = false;
            private static bool trigger = false;
        }
        private class Events
        {
            internal static void Update()
            {
                if (!OnHitEvent_Initialized) { Init_OnHitEvent(); }
            }
            internal static void Reset()
            {
                OnHitEvent_Initialized = false;
            }

            private static bool OnHitEvent_Initialized = false;
            private static void Init_OnHitEvent()
            {
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {
                    if (!Refs_Manager.player_actor.gameObject.IsNullOrDestroyed())
                    {
                        AbilityEventListener listener = Refs_Manager.player_actor.gameObject.GetComponent<AbilityEventListener>();
                        if (!listener.IsNullOrDestroyed())
                        {
                            listener.add_onHitEvent(OnHitAction);
                            OnHitEvent_Initialized = true;
                        }
                    }
                }
            }            
            private static readonly System.Action<Ability, Actor> OnHitAction = new System.Action<Ability, Actor>(OnHit);
            private static void OnHit(Ability ability, Actor hitActor)
            {
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {
                    if (Refs_Manager.player_actor.itemContainersManager.hasUniqueEquipped(Unique.unique_id)
                        && Refs_Manager.player_actor.stats.GetAttributeValue(CoreAttribute.Attribute.Strength) >= Save_Manager.instance.data.NewItems.Mjolner.StrRequirement
                        && Refs_Manager.player_actor.stats.GetAttributeValue(CoreAttribute.Attribute.Intelligence) >= Save_Manager.instance.data.NewItems.Mjolner.IntRequirement)
                    {
                        Trigger.SocketedSkills(hitActor);
                    }
                }
            }
        }
    }
}