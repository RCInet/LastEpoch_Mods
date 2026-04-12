# ModUI Framework

Declarative settings framework for the Last Epoch mod HUD.

## Adding a setting

One line in `ModSettings.cs`:

```csharp
public static readonly BoolSetting ForceSeal = Group.Bool("ForceSeal");
```

This gives you serialization to `SaveModUI.json`, UI binding to the shared `hud.prefab` toggle, and a dirty flag for debounced auto-save. All automatic.

**All factory methods:**

```csharp
Group.Bool("ForceSeal")                                             // Toggle
Group.Bool("LoadOnStart", defaultValue: true)                       // Toggle, on by default
Group.Float("ZoomMinimum")                                          // Toggle + Slider
Group.Float("MobsDensity", format: DisplayFormat.Percent)           // Toggle + Slider, shown as %
Group.Range("Implicits", 0, 255, format: DisplayFormat.Percent)     // Toggle + Min/Max Sliders
Group.Range("AffixTiers", 0, 6, format: DisplayFormat.Tier)         // Tiers display 1-indexed
Group.Range("AffixCount", 0, 6, defaultMax: 4, panel: "NbAffixes") // Custom default + panel override
Group.Dropdown("Type")                                              // Dropdown (options set at runtime)
Group.Dropdown("Monolith", options: new[] { "Select", "Outcast" })  // Dropdown with static options
Group.Radio("ForceRarity", "Force", "ForceUnique", "ForceSet", "ForceLegendary") // Exclusive toggles
Group.Button("Reset")                                               // Clickable action (not saved)
Group.Header("Header", label: "AutoCast")                           // Static localized section header
Group.Keybind("ModifierKey", defaultBinding: "kb:LeftControl", label: "Modifier Key")  // Rebindable input
```

When the prefab panel name differs from the setting key, use `panel:`:
```csharp
public static readonly RangeSetting AffixCount = Group.Range("AffixCount", 0, 6, panel: "NbAffixes");
```

Range defaults min/max to `minLimit`/`maxLimit`. Override with `defaultMin:` / `defaultMax:`.

## Labels & localization

Every factory method takes an optional `label:` (English display text). At bind time it's looked up in `Locales.current_dictionary` and the translated value is written to the widget's `Label` Text. If no translation exists, the English string is used as-is. If `label:` is null, the prefab's serialized Text is left alone.

```csharp
public static readonly FloatSetting TreePoints =
    Group.Float("TreePoints", defaultValue: 50f, label: "Weaver Tree Points");
```

`LocaleRegistry` tracks every `(Text, English label)` pair the library applies. Language switches re-translate from the canonical English key, so labels round trip cleanly across en→fr→en even though `Locales.current_dictionary` is one-way.

**Adding a localized label:**
1. Declare the setting with `label: "English Label"`
2. Add the key to `LastEpoch_Hud/Locales/base.json` (empty value)
3. Add translations to `en.json` / `fr.json` / `zh.json`
4. Rebuild the DLL. Labels live in code, not the prefab, so no AssetBundle rebuild needed


## Reading settings from feature code

```csharp
using LastEpoch_Hud.Scripts.ModUI;

// Direct reads
if (ModSettings.ItemsDrop.ForceSeal.Value) { /* ... */ }
float min = ModSettings.ItemsDrop.Implicits.Min;
bool enabled = ModSettings.ItemsDrop.Implicits.Enabled;

// Radio: check which option is selected
if (ModSettings.ItemsDrop.ForceRarity.IsSelected(0)) { /* ForceUnique */ }

// React to changes
ModSettings.ItemsDrop.ForceRarity.Changed += () => RecalculateDropTable();

// React to button clicks
ModSettings.ScenesCamera.Reset.Clicked += () => ResetCameraValues();
```

Settings have `{ get; internal set; }` -- feature code reads directly but must use `.Set()`, `.SetMin()`, `.SetMax()`, `.SetEnabled()` to write (triggers dirty flag + Changed events).


## Creating a new section

### With its own tab

```csharp
public static class ScenesCamera
{
    public static readonly SettingsGroup Group = new SettingsGroup("ScenesCamera")
        .Tab("Scenes", "Btn_Menu_Scenes")       // creates a HUD tab
        .Content("Scenes_Content")               // prefab content panel
        .Viewport("Camera", "Scenes_Camera_Content") // scrollable area
        .Prefix("Scenes_Camera_");               // prepended for UI element lookups

    public static readonly FloatSetting ZoomMinimum = Group.Float("ZoomMinimum");
    public static readonly ActionBinding Reset      = Group.Button("Reset");
}
```

### Binding to a custom prefab (not the Hud)

Register the prefab as a named root, then declare a group with `.Root("name")` and address widgets by absolute transform path.

```csharp
// Feature code, after instantiating the prefab:
SaveManager.BindRoot("PortalPrompt", portalPromptObj);

// ModSettings.cs:
public static class PortalPromptUI
{
    public static readonly SettingsGroup Group = new SettingsGroup("PortalPrompt").Root("PortalPrompt");

    public static readonly HeaderSetting   Header      = Group.Header("Header", label: "Quick Portal", path: "Title/Header");
    public static readonly DropdownSetting Destination = Group.Dropdown("Destination", path: "Panel/DestinationDropdown", label: "Destination");
    public static readonly BoolSetting     Include     = Group.Bool("Include", path: "Panel/IncludeUncompleted", label: "Include Uncompleted");
    public static readonly ActionBinding   Portal      = Group.Button("Portal", path: "Panel/PortalButton", label: "Portal");
}

// Feature code reads/writes the same as any other setting:
PortalPromptUI.Portal.Clicked += () => TeleportTo(PortalPromptUI.Destination.SelectedText);
```

`path:` is absolute from the registered root, so one group can span multiple sibling sub-trees. Settings persist to `SaveModUI.json` under the group name. `BindRoot` upserts on a new GameObject instance - call it again after re-instantiating. Works on Canvas roots too.

**Compound settings** (`Float`, `Range`, `Keybind`, `Radio`) bind multiple controls per setting and use a typed `paths:` struct:

```csharp
Group.Float("Damage", paths: new FloatPaths(slider: "Settings/Damage/Slider"))
Group.Float("Damage", paths: new FloatPaths(slider: "Settings/Damage/Slider", toggle: "Settings/Damage/Enable"))

Group.Range("Tier", paths: new RangePaths(minSlider: "Tier/Min", maxSlider: "Tier/Max", toggle: "Tier/Enable"))

Group.Keybind("Modifier", paths: new KeybindPaths(captureButton: "Mod/Capture", resetButton: "Mod/Reset", label: "Mod/Label"))
Group.Keybind("Modifier", paths: new KeybindPaths(captureButton: "Mod/Capture"))

Group.RadioWithPaths("Rarity",
    new RadioPaths("Rarity/Common", "Rarity/Magic", "Rarity/Rare", "Rarity/Unique"),
    "Common", "Magic", "Rare", "Unique")
```

Required sub-paths are constructor arguments; optional ones default to `null`. Omit the Float/Range toggle for a slider with no on/off control. Omit the Keybind reset button for a binding the player can rebind but not reset to default. Radio toggle count must match option count (positional pairing).

### Without a tab (sub-panel sharing another group's content)

```csharp
public static class ItemsPickup
{
    public static readonly SettingsGroup Group = new SettingsGroup("ItemsPickup")
        .Content("Items_Content")                    // shares content with ItemsDrop
        .Viewport("Items_Pickup", "Items_Pickup_Content")
        .Prefix("Items_Pickup_");

    public static readonly BoolSetting AutoPickupGold = Group.Bool("AutoPickupGold", panel: "AutoPickup_Gold");
}
```

Groups with `.Tab()` get a menu button and own their content panel. Groups without `.Tab()` are sub-panels bound during HUD init.


## OnBind -- escape hatch for non-standard UI

When a UI element doesn't follow the naming convention (master toggles in title bars, buttons in non-standard locations):

```csharp
.OnBind((contentObj, viewportContent) =>
{
    var toggle = Prefab.ToggleInTitle(contentObj, "Camera", "Toggle_Scenes_Camera_Enable");
    if (toggle == null) return;
    toggle.isOn = EnableMod.Value;
    Prefab.BindToggle(toggle, new Action<bool>(v => EnableMod.Set(v)));
})
```

Runs after all automatic bindings. Use sparingly -- if you're using it for standard toggles or buttons, the naming convention is probably wrong.


## Dropdown options

Dropdowns can have **static options** (known at compile time) or **dynamic options** (populated from game data at runtime).

**Static options:**
```csharp
public static readonly DropdownSetting Monolith = Group.Dropdown("Monolith",
    options: new[] { "Select", "Fall_Of_The_Outcast", "The_Stolen_Lance" });
```

**Dynamic options** (populated when game data is available):
```csharp
public static readonly DropdownSetting Type = Group.Dropdown("Type");

// In feature code, when game data loads:
var names = ItemList.get().EquippableItems.Select(i => i.BaseTypeName).ToArray();
ModSettings.ItemsForceDrop.Type.SetOptions(names);
```

**Cascading dropdowns** (one selection drives another's options):
```csharp
ModSettings.ItemsForceDrop.Type.Changed += _ =>
{
    var rarities = GetRaritiesForType(ModSettings.ItemsForceDrop.Type.SelectedText);
    ModSettings.ItemsForceDrop.Rarity.SetOptions(rarities);
};
```

`SetOptions` resets the selection to index 0 and does not fire `Changed` or mark dirty -- it's a UI reset, not a user action.

`.SelectedText` returns the label of the current selection, or null if no options are set.


## Naming convention

SettingsBuilder finds UI elements by combining `Prefix` + key:

| Element | Name pattern | Example (prefix `Items_Drop_`) |
|---------|-------------|-------------------------------|
| Toggle | `Toggle_{prefix}{key}` | `Toggle_Items_Drop_ForceSeal` |
| Slider | `Slider_{prefix}{key}` | `Slider_Items_Drop_Implicits_Min` |
| Dropdown | `Dropdown_{prefix}{key}` | `Dropdown_Items_ForceDrop_Type` |
| Button | `Btn_{prefix}{key}` | `Btn_Scenes_Camera_Reset` |

UI elements live inside a sub-panel named after the setting key (or `panel:` override). Buttons are direct children of the viewport content.

Text labels: searched as `Value` or `Label` child inside the toggle, then as direct panel child.


## Reference

### Setting types

| Type | Stores | UI | Read | Write |
|------|--------|----|------|-------|
| `BoolSetting` | bool | Toggle | `.Value` | `.Set(bool)` |
| `FloatSetting` | bool + float | Toggle + Slider | `.Enabled`, `.Value` | `.SetEnabled(bool)`, `.SetValue(float)` |
| `RangeSetting` | bool + min/max | Toggle + 2 Sliders | `.Enabled`, `.Min`, `.Max` | `.SetEnabled(bool)`, `.SetMin(float)`, `.SetMax(float)` |
| `DropdownSetting` | int + labels | Dropdown | `.Value`, `.SelectedText` | `.Set(int)`, `.SetOptions(string[])` |
| `RadioSetting` | N bools | N Toggles | `.IsSelected(i)` | `.Select(i, bool)` |
| `ActionBinding` | nothing | Button | -- | subscribe to `.Clicked` |
| `HeaderSetting` | nothing | Static text row | `.Text` | `.SetText(string)` |
| `KeybindSetting` | tagged string | Capture Button + Reset Button | `.Value`, `.DefaultValue` | `.Set(string)`, `.ResetToDefault()` |

All setting types (except `ActionBinding` and `HeaderSetting`) expose a `Changed` event for reactive updates.

## Keybind settings

`KeybindSetting` captures one keyboard key OR gamepad button. Click capture, press any key, first input wins. `Escape` aborts. Reset reverts to default.

```csharp
public static readonly KeybindSetting ModifierKey =
    Group.Keybind("ModifierKey", defaultBinding: "kb:LeftControl", label: "Modifier Key");

if (KeybindMatcher.IsHeld(ModSettings.SkillsAutoCast.ModifierKey.Value)) { ... }
```

**Binding format**: tagged strings. `"kb:LeftControl"` is a KeyCode name. `"gp:A"` is an IGamepadTemplate name: `A`, `B`, `X`, `Y`, `LB`, `RB`, `Back`, `Start`, `Guide`, `LStick`, `RStick`, `DPadUp/Down/Left/Right`.

`resetLabel:` overrides the centralized "Reset" string per setting when one button needs different wording.

**Convention prefab layout** (one Keybind setting):
```
{panelName}/
  Label                            -- setting label Text
  Btn_{prefix}{panelName}          -- capture button (child Text shows current binding)
  Btn_{prefix}{panelName}_Reset    -- reset button (child Text "Reset")
```
For arbitrary prefabs use `paths: new KeybindPaths(...)` instead of the convention layout.

### Display formats

| Format | Example output | Use case |
|--------|---------------|----------|
| `Raw` | `50` | Integer values |
| `Tier` | `3` | 1-indexed display (game stores 0-indexed) |
| `Percent` | `75 %` | Percentage (auto-scales if max != 100) |
| `Seconds` | `5 sec` | Time intervals |

### SettingsGroup fluent API

```csharp
new SettingsGroup("Name")           // JSON key, auto-registers for save/load
    .Root("Hud")                    // Binding root name (default "Hud" for backward compat)
    .Tab("TabId", "Btn_Menu_Name")  // Creates HUD tab (omit for sub-panels and non-Hud roots)
    .Content("Panel_Name")          // Prefab content panel (optional for non-Hud roots)
    .Viewport("Panel", "Content")   // Scrollable viewport (omit for flat layouts)
    .Prefix("Ui_Prefix_")          // Prepended to element names for lookups (convention mode)
    .OnBind((content, viewport) => { }) // Escape hatch for non-standard elements
```

### Binding modes

| Mode | When to use | How |
|---|---|---|
| **Convention** | Hud rows that follow `Toggle_{prefix}{key}` / `Slider_{prefix}{key}` / `Btn_{prefix}{key}` naming | `Group.Bool("MyToggle")` |
| **Path** | Custom prefabs with arbitrary hierarchy (DamageMeter, etc.) | `Group.Bool("MyToggle", path: "Panel/Settings/MyToggle")` |
| **OnBind** | One-off non-standard elements (master toggles in title bars, etc.) | `.OnBind((content, vp) => { ... })` |

### File layout

```
ModUI/
  ModSettings.cs         THE file to edit -- all settings, tabs, and panel config
  Internal/              Framework internals
    SettingTypes.cs         Setting types + ActionBinding + display formatting
    SettingsGroup.cs        Fluent API, factory methods, serialization, UI binding
    SettingsBuilder.cs      Convention-based + path-based prefab binding
    TabManager.cs           Tab switching and menu button wiring (Hud root only)
    Prefab.cs               Null-safe child + path lookups, ChildPath / ComponentAtPath
    SliderHook.cs           Harmony patch for IL2CPP slider events
    SaveManager.cs          SaveModUI.json persistence + BindHud / BindRoot entry points
    BindingRoots.cs         Name-keyed registry of prefab roots ("Hud", custom, ...)
    LocaleRegistry.cs       Tracks (Text, English label) pairs for clean language round-trip
  Keybind/               Keybind setting helpers (rebindable inputs)
    KeybindStrings.cs       Centralized localized strings (Reset, Press any key, etc.)
    KeybindFormat.cs        Friendly display formatting for tagged bindings
    KeybindRewired.cs       Rewired IGamepadTemplate walking + button table
    KeybindCapture.cs       Capture state machine ticked from SaveManager.Update
    KeybindMatcher.cs       Public IsHeld API consumed by feature code
```

### Cheatsheet

| I want to... | Do this |
|---|---|
| Add a toggle/slider/range | `ModSettings.cs` -- one line: `Group.Bool/Float/Range(...)` |
| Add a dropdown | `ModSettings.cs` -- `Group.Dropdown("Key")`, optionally with `options:` |
| Populate dropdown at runtime | `.SetOptions(string[])` from feature code when game data loads |
| Add an action button | `ModSettings.cs` -- `Group.Button("Key")`, subscribe via `.Clicked` |
| Add a new tab | `ModSettings.cs` -- new class with `.Tab()` on its SettingsGroup |
| Add a sub-panel (no tab) | `ModSettings.cs` -- new class with `.Content()` only |
| Read a setting | `ModSettings.Section.Setting.Value` |
| React to changes | `.Changed` event on the setting |
| React to button click | `.Clicked` event on the ActionBinding |
| Wire a non-standard element | `.OnBind()` on the SettingsGroup |
| Override prefab panel name | `panel:` parameter on factory method |
| Add a new display format | `Internal/SettingTypes.cs` |
| Add a new setting type | `Internal/SettingTypes.cs` + `SettingsGroup.cs` + `SettingsBuilder.cs` |
| Add a section header | `ModSettings.cs` -- `Group.Header("Header", label: "Section Name")` |
| Add a rebindable input | `ModSettings.cs` -- `Group.Keybind("Key", defaultBinding: "kb:LeftControl", label: "Modifier")` |
| Read a keybind | `KeybindMatcher.IsHeld(ModSettings.X.Y.Value)` |
| Bind a custom prefab | `SaveManager.BindRoot("MyPrefab", obj)` then `Group.Root("MyPrefab")` in ModSettings |
| Bind a widget by path | `Group.Bool("Key", path: "Panel/Settings/Toggle")` (skips naming convention) |
