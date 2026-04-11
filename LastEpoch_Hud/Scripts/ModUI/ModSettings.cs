using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Central registry of all mod settings.
    // Each section is a SettingsGroup that handles serialization AND UI binding.
    // Adding a setting = one line here. No other files to edit.
    public static class ModSettings
    {
        public static bool Dirty { get; private set; }

        public static void MarkDirty()
        {
            Trace("MarkDirty called");
            Dirty = true;
        }
        public static void ClearDirty() => Dirty = false;

        private static readonly List<SettingsGroup> allGroups = new();
        public static IReadOnlyList<SettingsGroup> AllGroups => allGroups;
        public static void RegisterGroup(SettingsGroup group) => allGroups.Add(group);

        // Tracing helper. No-op when Debug.Enabled is false
        internal static void Trace(string msg)
        {
            if (Debug.Enabled.Value) Main.logger_instance?.Msg("[ModUI] " + msg);
        }

        // Auto-initialize all nested section classes so their SettingsGroups register.
        // Without this, C# defers static nested class init until first access.
        static ModSettings()
        {
            foreach (var type in typeof(ModSettings).GetNestedTypes())
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }

        //
        // META / DIAGNOSTICS (no GUI -- edit SaveModUI.json manually to toggle)
        //

        public static class Debug
        {
            // No .Content() call -- this group persists to JSON but has no widgets to bind.
            public static readonly SettingsGroup Group = new SettingsGroup("Debug");

            // Gates all ModSettings.Trace(...) output. Default false = silent for players.
            public static readonly BoolSetting Enabled = Group.Bool("Enabled");
        }

        //
        // CHARACTER TAB
        //

        public static class Weaver
        {
            public static readonly SettingsGroup Group = new SettingsGroup("Weaver")
                .Content("Character_Content")
                .Viewport("Character_Factions", "Character_Factions_Content")
                .Prefix("Weaver_");

            public static readonly FloatSetting TreePoints = Group.Float("TreePoints", defaultValue: 50f);
            public static readonly BoolSetting  FreeRespec = Group.Bool("FreeRespec");
        }

        //
        // ITEMS TAB
        //

        // public static class ItemsDrop
        // {
        //     public static readonly SettingsGroup Group = new SettingsGroup("ItemsDrop")
        //         .Tab("Items", "Btn_Menu_Items")
        //         .Content("Items_Content")
        //         .Viewport("Items_Drop", "Items_Data_Content")
        //         .Prefix("Items_Drop_");

        //     public static readonly RadioSetting ForceRarity    = Group.Radio("ForceRarity", "Force", "ForceUnique", "ForceSet", "ForceLegendary");
        //     public static readonly RangeSetting Implicits      = Group.Range("Implicits", 0, 255, format: DisplayFormat.Percent);
        //     public static readonly RangeSetting ForginPotencial= Group.Range("ForginPotencial", 0, 255);
        //     public static readonly BoolSetting  ForceSeal      = Group.Bool("ForceSeal");
        //     public static readonly RangeSetting SealTier       = Group.Range("SealTier", 0, 6, format: DisplayFormat.Tier);
        //     public static readonly RangeSetting SealValue      = Group.Range("SealValue", 0, 255, format: DisplayFormat.Percent);
        //     public static readonly RangeSetting AffixCount     = Group.Range("AffixCount", 0, 6, defaultMax: 4, panel: "NbAffixes");
        //     public static readonly RangeSetting IdolAffixCount = Group.Range("IdolAffixCount", 0, 4, defaultMax: 2);
        //     public static readonly RangeSetting AffixTiers     = Group.Range("AffixTiers", 0, 6, format: DisplayFormat.Tier, panel: "AffixesTiers");
        //     public static readonly RangeSetting AffixValues    = Group.Range("AffixValues", 0, 255, format: DisplayFormat.Percent, panel: "AffixesValues");
        //     public static readonly RangeSetting UniqueMods     = Group.Range("UniqueMods", 0, 255, format: DisplayFormat.Percent);
        //     public static readonly RangeSetting LegendaryPotencial = Group.Range("LegendaryPotencial", 0, 255, defaultMax: 4);
        //     public static readonly RangeSetting WeaverWill     = Group.Range("WeaverWill", 0, 28);
        // }

        // public static class ItemsPickup
        // {
        //     public static readonly SettingsGroup Group = new SettingsGroup("ItemsPickup")
        //         .Content("Items_Content")
        //         .Viewport("Items_Pickup", "Items_Pickup_Content")
        //         .Prefix("Items_Pickup_");

        //     public static readonly BoolSetting AutoPickupGold        = Group.Bool("AutoPickupGold", panel: "AutoPickup_Gold");
        //     public static readonly BoolSetting AutoPickupKeys        = Group.Bool("AutoPickupKeys", panel: "AutoPickup_Keys");
        //     public static readonly BoolSetting AutoPickupPotions     = Group.Bool("AutoPickupPotions", panel: "AutoPickup_Pots");
        //     public static readonly BoolSetting AutoPickupXpTome      = Group.Bool("AutoPickupXpTome", panel: "AutoPickup_XpTome");
        //     public static readonly BoolSetting AutoPickupFavorTome   = Group.Bool("AutoPickupFavorTome", panel: "AutoPickup_FavorTome");
        //     public static readonly BoolSetting AutoPickupMemoryAmber = Group.Bool("AutoPickupMemoryAmber", panel: "AutoPickup_MemoryAmber");
        //     public static readonly BoolSetting AutoPickupWovenEchoes = Group.Bool("AutoPickupWovenEchoes", panel: "AutoPickup_WovenEchoes");
        //     public static readonly BoolSetting AutoPickupMaterials   = Group.Bool("AutoPickupMaterials", panel: "AutoPickup_Materials");
        //     public static readonly BoolSetting AutoPickupFromFilter  = Group.Bool("AutoPickupFromFilter", panel: "AutoPickup_Filters");

        //     public static readonly BoolSetting  AutoStoreOnDrop          = Group.Bool("AutoStoreOnDrop", panel: "AutoStore_OnDrop");
        //     public static readonly BoolSetting  AutoStoreOnInventoryOpen = Group.Bool("AutoStoreOnInventoryOpen", panel: "AutoStore_OnInventoryOpen");
        //     public static readonly FloatSetting AutoStoreTimer           = Group.Float("AutoStoreTimer", format: DisplayFormat.Seconds, panel: "AutoStore_Timer");

        //     public static readonly BoolSetting AutoSellFromFilter        = Group.Bool("AutoSellFromFilter", panel: "AutoSell_FromFilter");
        //     public static readonly BoolSetting AutoShatterFromFilter     = Group.Bool("AutoShatterFromFilter", panel: "AutoShatter_FromFilter");
        //     public static readonly BoolSetting AutoShatterUseRune        = Group.Bool("AutoShatterUseRune", panel: "AutoShatter_Rune");
        //     public static readonly FloatSetting AutoShatterChance         = Group.Float("AutoShatterChance", format: DisplayFormat.Percent, panel: "AutoShatter_Chance");
        //     public static readonly FloatSetting AutoShatterAffixChance    = Group.Float("AutoShatterAffixChance", format: DisplayFormat.Percent, panel: "AutoShatter_AffixChance");
        //     public static readonly FloatSetting AutoShatterQuantityChance = Group.Float("AutoShatterQuantityChance", format: DisplayFormat.Percent, panel: "AutoShatter_QuantityChance");

        //     public static readonly BoolSetting RangePickup               = Group.Bool("RangePickup", panel: "Range_Pickup");
        //     public static readonly BoolSetting HideMaterialsNotifications= Group.Bool("HideMaterialsNotifications", panel: "Hide_Notifications");
        // }


        // public static class ItemsRequirements
        // {
        //     // No prefix -- the toggles are named Toggle_RemoveReq_Class directly
        //     public static readonly SettingsGroup Group = new SettingsGroup("ItemsRequirements")
        //         .Content("Items_Content")
        //         .Viewport("Items_Pickup", "Items_Req_Content");

        //     public static readonly BoolSetting Class = Group.Bool("Class", panel: "RemoveReq_Class");
        //     public static readonly BoolSetting Level = Group.Bool("Level", panel: "RemoveReq_Level");
        //     public static readonly BoolSetting Set   = Group.Bool("Set", panel: "RemoveReq_Set");
        // }


        // public static class ItemsCrafting
        // {
        //     // Master enable toggle lives in Items_Craft > Title, outside the viewport.
        //     // Use OnBind to wire it since it doesn't fit the standard panel layout.
        //     public static readonly BoolSetting EnableMod = new BoolSetting();

        //     public static readonly SettingsGroup Group = new SettingsGroup("ItemsCrafting")
        //         .Content("Items_Content")
        //         .Viewport("Items_Craft", "Items_Craft_Content")
        //         .Prefix("Items_Craft_")
        //         .OnBind((contentObj, _) =>
        //         {
        //             var toggle = Prefab.ToggleInTitle(contentObj, "Items_Craft", "Toggle_Items_Craft_Enable", activate: true);
        //             if (toggle == null) return;
        //             toggle.isOn = EnableMod.Value;
        //             Prefab.BindToggle(toggle, new System.Action<bool>(v => EnableMod.Set(v)));
        //         });

        //     public static readonly FloatSetting ForginPotencial = Group.Float("ForginPotencial");
        //     public static readonly FloatSetting Implicit0       = Group.Float("Implicit0", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting Implicit1       = Group.Float("Implicit1", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting Implicit2       = Group.Float("Implicit2", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting SealTier        = Group.Float("SealTier", format: DisplayFormat.Tier);
        //     public static readonly FloatSetting SealValue       = Group.Float("SealValue", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting AffixTier0      = Group.Float("AffixTier0", format: DisplayFormat.Tier);
        //     public static readonly FloatSetting AffixValue0     = Group.Float("AffixValue0", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting AffixTier1      = Group.Float("AffixTier1", format: DisplayFormat.Tier);
        //     public static readonly FloatSetting AffixValue1     = Group.Float("AffixValue1", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting AffixTier2      = Group.Float("AffixTier2", format: DisplayFormat.Tier);
        //     public static readonly FloatSetting AffixValue2     = Group.Float("AffixValue2", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting AffixTier3      = Group.Float("AffixTier3", format: DisplayFormat.Tier);
        //     public static readonly FloatSetting AffixValue3     = Group.Float("AffixValue3", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting UniqueMod0      = Group.Float("UniqueMod0", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting UniqueMod1      = Group.Float("UniqueMod1", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting UniqueMod2      = Group.Float("UniqueMod2", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting UniqueMod3      = Group.Float("UniqueMod3", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting UniqueMod4      = Group.Float("UniqueMod4", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting UniqueMod5      = Group.Float("UniqueMod5", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting UniqueMod6      = Group.Float("UniqueMod6", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting UniqueMod7      = Group.Float("UniqueMod7", format: DisplayFormat.Percent);
        //     public static readonly FloatSetting LegendaryPotencial = Group.Float("LegendaryPotencial");
        //     public static readonly FloatSetting WeaverWill      = Group.Float("WeaverWill");
        // }


        // public static class ItemsForceDrop
        // {
        //     // Drop button fires an event -- mod feature code subscribes to execute the drop.
        //     public static event System.Action OnDrop;

        //     public static readonly SettingsGroup Group = new SettingsGroup("ItemsForceDrop")
        //         .Content("Items_Content")
        //         .Viewport("Items_Pickup", "Items_ForceDrop_Content")
        //         .Prefix("Items_ForceDrop_")
        //         .OnBind((contentObj, _) =>
        //         {
        //             var pickupPanel = Prefab.Child(contentObj, "Items_Pickup");
        //             if (pickupPanel == null) return;
        //             var btnObj = Prefab.Child(pickupPanel, "Btn_Items_ForceDrop_Drop");
        //             if (btnObj == null) return;
        //             var btn = btnObj.GetComponent<UnityEngine.UI.Button>();
        //             if (btn == null) return;
        //             Prefab.BindButton(btn, new System.Action(() => OnDrop?.Invoke()));
        //         });

        //     public static readonly DropdownSetting Type     = Group.Dropdown("Type", panel: "Type");
        //     public static readonly DropdownSetting Rarity   = Group.Dropdown("Rarity", panel: "Rarity");
        //     public static readonly DropdownSetting Item     = Group.Dropdown("Item", panel: "Item");
        //     public static readonly FloatSetting Quantity = Group.Float("Quantity", panel: "Quantity");
        // }

        //
        // SCENES TAB
        //

    //     public static class ScenesCamera
    //     {
    //         public static readonly BoolSetting EnableMod = new BoolSetting();

    //         public static readonly SettingsGroup Group = new SettingsGroup("ScenesCamera")
    //             .Tab("Scenes", "Btn_Menu_Scenes")
    //             .Content("Scenes_Content")
    //             .Viewport("Camera", "Scenes_Camera_Content")
    //             .Prefix("Scenes_Camera_")
    //             .OnBind((contentObj, _) =>
    //             {
    //                 var toggle = Prefab.ToggleInTitle(contentObj, "Camera", "Toggle_Scenes_Camera_Enable");
    //                 if (toggle == null) return;
    //                 toggle.isOn = EnableMod.Value;
    //                 Prefab.BindToggle(toggle, new System.Action<bool>(v => EnableMod.Set(v)));
    //             });

    //         public static readonly ActionBinding Reset = Group.Button("Reset");
    //         public static readonly ActionBinding Set   = Group.Button("Set");

    //         public static readonly FloatSetting ZoomMinimum     = Group.Float("ZoomMinimum");
    //         public static readonly FloatSetting ZoomPerScroll   = Group.Float("ZoomPerScroll");
    //         public static readonly FloatSetting ZoomSpeed       = Group.Float("ZoomSpeed");
    //         public static readonly FloatSetting DefaultRotation = Group.Float("DefaultRotation");
    //         public static readonly FloatSetting OffsetMinimum   = Group.Float("OffsetMinimum");
    //         public static readonly FloatSetting OffsetMaximum   = Group.Float("OffsetMaximum");
    //         public static readonly FloatSetting AngleMinimum    = Group.Float("AngleMinimum");
    //         public static readonly FloatSetting AngleMaximum    = Group.Float("AngleMaximum");
    //         public static readonly BoolSetting  LoadOnStart     = Group.Bool("LoadOnStart");
    //     }

    //     public static class ScenesDungeons
    //     {
    //         public static readonly SettingsGroup Group = new SettingsGroup("ScenesDungeons")
    //             .Content("Scenes_Content")
    //             .Viewport("Center", "Scenes_Dungeons_Content")
    //             .Prefix("Scenes_Dungeons_");

    //         public static readonly BoolSetting EnterWithoutKey = Group.Bool("EnterWithoutKey");
    //     }

    //     public static class ScenesMinimap
    //     {
    //         public static readonly SettingsGroup Group = new SettingsGroup("ScenesMinimap")
    //             .Content("Scenes_Content")
    //             .Viewport("Center", "Scenes_Minimap_Content")
    //             .Prefix("Scenes_Minimap_");

    //         public static readonly BoolSetting MaxZoomOut       = Group.Bool("MaxZoomOut");
    //         public static readonly BoolSetting RemoveFogOfWar   = Group.Bool("RemoveFogOfWar");
    //     }

    //     public static class ScenesMonoliths
    //     {
    //         public static readonly SettingsGroup Group = new SettingsGroup("ScenesMonoliths")
    //             .Content("Scenes_Content")
    //             .Viewport("Monoliths", "Scenes_Monoliths_Content")
    //             .Prefix("Scenes_Monoliths_");

    //         public static readonly FloatSetting MaxStability    = Group.Float("MaxStability");
    //         public static readonly FloatSetting MobsDensity     = Group.Float("MobsDensity", format: DisplayFormat.Percent);
    //         public static readonly FloatSetting MobsDefeatOnStart = Group.Float("MobsDefeatOnStart", format: DisplayFormat.Percent);
    //         public static readonly FloatSetting BlessingSlots   = Group.Float("BlessingSlots");
    //         public static readonly BoolSetting  MaxStabilityOnStart = Group.Bool("MaxStabilityOnStart");
    //         public static readonly BoolSetting  MaxStabilityOnStabilityChanged = Group.Bool("MaxStabilityOnStabilityChanged");
    //         public static readonly BoolSetting  ObjectiveReveal = Group.Bool("ObjectiveReveal");
    //         public static readonly BoolSetting  CompleteObjective = Group.Bool("CompleteObjective");
    //         public static readonly BoolSetting  NoLostWhenDie   = Group.Bool("NoLostWhenDie");
    //         public static readonly BoolSetting  Islands         = Group.Bool("Islands", panel: "EnableIslands");
    //     }

    //     public static class ScenesHarbringers
    //     {
    //         public static readonly SettingsGroup Group = new SettingsGroup("ScenesHarbringers");

    //         public static readonly BoolSetting AltarWithoutKey = Group.Bool("AltarWithoutKey");
    //     }
    // }
    }
}
