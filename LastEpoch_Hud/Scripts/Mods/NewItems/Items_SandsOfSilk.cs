using System;
using HarmonyLib;
using Il2Cpp;
using Il2CppLE.Services.Models.Items;
using Il2CppLE.Services.Visuals;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.NewItems
{
    [RegisterTypeInIl2Cpp]
    public class Items_SandsOfSilk : MonoBehaviour
    {
        public static Items_SandsOfSilk instance { get; private set; }
        public Items_SandsOfSilk(System.IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if (Unique.Icon.IsNullOrDestroyed()) { Assets.Loaded = false; }
            if (!Assets.Loaded) { Assets.Load(); }
            if (Locales.current != Locales.Selected.Unknow && !Basic.AddedToBasicList) { Basic.AddToBasicList(); }
            if (Locales.current != Locales.Selected.Unknow && !Unique.AddedToUniqueList) { Unique.AddToUniqueList(); }
            if (Locales.current != Locales.Selected.Unknow && Unique.AddedToUniqueList && !Unique.AddedToDictionary) { Unique.AddToDictionary(); }            
        }

        public class Assets
        {
            public static bool Loaded = false;
            public static bool loading = false;
            public static void Load()
            {
                if (!Loaded && !Hud_Manager.asset_bundle.IsNullOrDestroyed() && !loading)
                {
                    loading = true;
                    try
                    {
                        foreach (string name in Hud_Manager.asset_bundle.GetAllAssetNames())
                        {
                            if (name.Contains("/sandsofsilk/"))
                            {
                                if (Functions.Check_Texture(name) && name.Contains("icon") && Unique.Icon.IsNullOrDestroyed())
                                {
                                    Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                                    Unique.Icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                }
                            }
                        }
                        if (!Unique.Icon.IsNullOrDestroyed()) { Loaded = true; }
                        else { Loaded = false; }
                    }
                    catch { Main.logger_instance?.Error("Sands Of Silk Asset Error"); }
                    loading = false;
                }
            }
        }
        public class Basic
        {
            public static bool AddedToBasicList = false;
            public static readonly byte base_type = 1; //Body Armor
            public static readonly int base_id = 71;
            public static ItemList.EquipmentItem Item()
            {
                ItemList.EquipmentItem item = new ItemList.EquipmentItem
                {
                    classRequirement = ItemList.ClassRequirement.None,
                    implicits = implicits(),
                    subClassRequirement = ItemList.SubClassRequirement.None,
                    cannotDrop = true,
                    itemTags = ItemLocationTag.None,
                    levelRequirement = 16,
                    name = Get_Subtype_Name(),
                    subTypeID = base_id
                };

                return item;
            }

            public static void AddToBasicList()
            {
                if (!AddedToBasicList && !Refs_Manager.item_list.IsNullOrDestroyed())
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
                    case Locales.Selected.English: { result = SOSLocales.SubType.en; break; }
                    case Locales.Selected.French: { result = SOSLocales.SubType.en; break; }
                    case Locales.Selected.German: { result = SOSLocales.SubType.en; break; }
                    case Locales.Selected.Russian: { result = SOSLocales.SubType.en; break; }
                    case Locales.Selected.Portuguese: { result = SOSLocales.SubType.en; break; }
                    case Locales.Selected.Korean: { result = SOSLocales.SubType.en; break; }
                    case Locales.Selected.Polish: { result = SOSLocales.SubType.en; break; }
                    case Locales.Selected.Chinese: { result = SOSLocales.SubType.en; break; }
                    case Locales.Selected.Spanish: { result = SOSLocales.SubType.en; break; }
                }

                return result;
            }

            private static Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> implicits()
            {
                Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> implicits = new Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit>();
                implicits.Add(new ItemList.EquipmentImplicit
                {
                    implicitMaxValue = 204,
                    implicitValue = 153,
                    property = SP.DodgeRating,
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
            public static readonly ushort unique_id = 502;
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
                    levelRequirement = 16,
                    legendaryType = UniqueList.LegendaryType.LegendaryPotential,
                    overrideEffectiveLevelForLegendaryPotential = true,
                    effectiveLevelForLegendaryPotential = 0,
                    canDropRandomly = true,
                    rerollChance = 1,
                    itemModelType = UniqueList.ItemModelType.Unique,
                    subTypeForIM = 0,
                    baseType = Basic.base_type,
                    subTypes = SubType(),
                    mods = Mods(),
                    //tooltipDescriptions = TooltipDescription(),
                    loreText = Get_Unique_Lore(), //lore,
                    tooltipEntries = TooltipEntries(),
                    oldSubTypeID = 0,
                    oldUniqueID = 0
                };

                return item;
            }

            public static void AddToUniqueList()
            {
                if (!AddedToUniqueList && !Refs_Manager.unique_list.IsNullOrDestroyed())
                {
                    try
                    {
                        UniqueList.getUnique(0); //force initialize uniquelist
                        Refs_Manager.unique_list.uniques.Add(Item());
                        AddedToUniqueList = true;
                    }
                    catch { Main.logger_instance?.Error("Sands of Silks Unique List Error"); }
                }
            }
            public static void AddToDictionary()
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
                    catch { Main.logger_instance?.Error("Sands of Silks Unique Dictionary Error"); }
                }
            }
            public static string Get_Unique_Name()
            {
                string result = "";
                switch (Locales.current)
                {
                    case Locales.Selected.English: { result = SOSLocales.UniqueName.en; break; }
                    case Locales.Selected.French: { result = SOSLocales.UniqueName.en; break; }
                    case Locales.Selected.German: { result = SOSLocales.UniqueName.en; break; }
                    case Locales.Selected.Russian: { result = SOSLocales.UniqueName.en; break; }
                    case Locales.Selected.Portuguese: { result = SOSLocales.UniqueName.en; break; }
                    case Locales.Selected.Korean: { result = SOSLocales.UniqueName.en; break; }
                    case Locales.Selected.Polish: { result = SOSLocales.UniqueName.en; break; }
                    case Locales.Selected.Chinese: { result = SOSLocales.UniqueName.en; break; }
                    case Locales.Selected.Spanish: { result = SOSLocales.UniqueName.en; break; }
                }

                return result;
            }
            public static string Get_Unique_Description()
            {
                string result = "";
                switch (Locales.current)
                {
                    case Locales.Selected.English: { result = SOSLocales.UniqueDescription.en; break; }
                    case Locales.Selected.French: { result = SOSLocales.UniqueDescription.en; break; }
                    case Locales.Selected.Korean: { result = SOSLocales.UniqueDescription.en; break; }
                    case Locales.Selected.German: { result = SOSLocales.UniqueDescription.en; break; }
                    case Locales.Selected.Russian: { result = SOSLocales.UniqueDescription.en; break; }
                    case Locales.Selected.Polish: { result = SOSLocales.UniqueDescription.en; break; }
                    case Locales.Selected.Portuguese: { result = SOSLocales.UniqueDescription.en; break; }
                    case Locales.Selected.Chinese: { result = SOSLocales.UniqueDescription.en; break; }
                    case Locales.Selected.Spanish: { result = SOSLocales.UniqueDescription.en; break; }
                }

                return result;
            }
            public static string Get_Unique_Lore()
            {
                string result = "";
                switch (Locales.current)
                {
                    case Locales.Selected.English: { result = SOSLocales.Lore.en; break; }
                    case Locales.Selected.French: { result = SOSLocales.Lore.en; break; }
                    case Locales.Selected.German: { result = SOSLocales.Lore.en; break; }

                    case Locales.Selected.Korean: { result = SOSLocales.Lore.en; break; }
                    case Locales.Selected.Russian: { result = SOSLocales.Lore.en; break; }
                    case Locales.Selected.Polish: { result = SOSLocales.Lore.en; break; }
                    case Locales.Selected.Portuguese: { result = SOSLocales.Lore.en; break; }
                    case Locales.Selected.Chinese: { result = SOSLocales.Lore.en; break; }
                    case Locales.Selected.Spanish: { result = SOSLocales.Lore.en; break; }
                }

                return result;
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
                    property = SP.DodgeRating,
                    tags = AT.None,
                    type = BaseStats.ModType.INCREASED,
                    maxValue = 1f,
                    value = 0.5f
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Mana,
                    tags = AT.None,
                    type = BaseStats.ModType.ADDED,
                    maxValue = 80,
                    value = 50
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Dexterity,
                    tags = AT.None,
                    type = BaseStats.ModType.ADDED,
                    maxValue = 20,
                    value = 10
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.Intelligence,
                    tags = AT.None,
                    type = BaseStats.ModType.ADDED,
                    maxValue = 20,
                    value = 10
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.FireResistance,
                    tags = AT.None,
                    type = BaseStats.ModType.INCREASED,
                    maxValue = 0.15f,
                    value = 0.1f
                });
                result.Add(new UniqueItemMod
                {
                    canRoll = true,
                    property = SP.IncreasedCooldownRecoverySpeed,
                    tags = AT.None,
                    type = BaseStats.ModType.INCREASED,
                    maxValue = 0.3f,
                    value = 0.15f
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
                result.Add(new UniqueModDisplayListEntry(4));
                result.Add(new UniqueModDisplayListEntry(5));

                return result;
            }
            /*private static Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> TooltipDescription()
            {
                Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> result = new Il2CppSystem.Collections.Generic.List<ItemTooltipDescription>();
                result.Add(new ItemTooltipDescription { description = Get_Unique_Description() });

                return result;
            }*/
            /*private static UniqueList.LegendaryType LegendaryType()
            {
                UniqueList.LegendaryType legendaryType = UniqueList.LegendaryType.LegendaryPotential;
                if (Save_Manager.instance.data.Items.Headhunter.WeaverWill) { legendaryType = UniqueList.LegendaryType.WeaversWill; }

                return legendaryType;
            }*/

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
        public class Visual
        {
            [HarmonyPatch(typeof(ClientVisualsService), "GetItemVisual")]
            public class ClientVisualsService_GetItemVisual
            {
                [HarmonyPrefix]
                static void Prefix(ClientVisualsService __instance, ref ItemVisualKey __0)
                {
                    if (__0.EquipmentType == EquipmentType.BODY_ARMOR &&
                        __0.SubType == Basic.base_id &&
                        __0.UniqueID == Unique.unique_id)
                    {
                        __0.SubType = 0;
                        __0.UniqueID = 7; //The Krestel
                    }
                }

                [HarmonyFinalizer]
                static Exception Finalizer(Exception __exception)
                {
                    if (__exception != null)
                        MelonLogger.Warning($"GetItemVisual threw: {__exception.GetType().Name}");
                    return null;
                }
            }
        }
        public class SOSLocales
        {
            private static string basic_subtype_name_key = "Item_SubType_Name_" + Basic.base_type + "_" + Basic.base_id;
            private static string unique_name_key = "Unique_Name_" + Unique.unique_id;
            private static string unique_description_key = "Unique_Tooltip_0_" + Unique.unique_id;
            private static string unique_lore_key = "Unique_Lore_" + Unique.unique_id;

            public class SubType
            {
                public static string en = "Shrouded Vest";
                //Add all languages here
            }
            public class UniqueName
            {
                public static string en = "Sands of Silk";
                //Add all languages here
            }
            public class UniqueDescription
            {
                public static string en = "The desert is ever flowing.";
                //Add all languages here
            }
            public class Lore
            {
                public static readonly string en = "The desert is ever flowing.";
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
    }
}
