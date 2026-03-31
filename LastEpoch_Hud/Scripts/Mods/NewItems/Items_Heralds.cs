using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpoch_Hud.Scripts.Mods.NewItems
{
    [RegisterTypeInIl2Cpp]
    public class Items_Heralds : MonoBehaviour
    {
        public static Items_Heralds instance { get; private set; }
        public Items_Heralds(System.IntPtr ptr) : base(ptr) { }

        bool InGame = false;

        void Awake()
        {
            instance = this;
            SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
        }
        void Update()
        {
            if (Uniques.Ice.Icon.IsNullOrDestroyed() ||
                Uniques.Fire.Icon.IsNullOrDestroyed() ||
                Uniques.Lightning.Icon.IsNullOrDestroyed() ||
                Uniques.Poison.Icon.IsNullOrDestroyed() ||
                Uniques.Physical.Icon.IsNullOrDestroyed())
            { Assets.Loaded = false; }
            if (!Assets.Loaded) { Assets.Load(); }
            if (Locales.current != Locales.Selected.Unknow)
            {
                if (!Basic.AddedToBasicList) { Basic.AddToBasicList(); }
                if (!Uniques.AddedToUniqueList) { Uniques.AddToUniqueList(); }
                if (Uniques.AddedToUniqueList && !Uniques.AddedToDictionary) { Uniques.AddToDictionary(); }
            }
            if (!Events.OnKillEvent_Initialized) { Events.Init_OnKillEvent(); }
            if (!Events.OnMinionKillEvent_Initialized) { Events.Init_OnMinionKillEvent(); }
            if (Scenes.IsGameScene())
            {
                if (Uniques.Ice.prefab_obj.IsNullOrDestroyed()) { Uniques.Ice.GetPrefab(); }
                if (Uniques.Ice.ability.IsNullOrDestroyed()) { Uniques.Ice.GetAbility(); }
                if (Uniques.Fire.prefab_obj.IsNullOrDestroyed()) { Uniques.Fire.GetPrefab(); }
                if (Uniques.Fire.ability.IsNullOrDestroyed()) { Uniques.Fire.GetAbility(); }
                if (Uniques.Lightning.prefab_obj.IsNullOrDestroyed()) { Uniques.Lightning.GetPrefab(); }
                if (Uniques.Lightning.ability.IsNullOrDestroyed()) { Uniques.Lightning.GetAbility(); }
                if (Uniques.Poison.prefab_obj.IsNullOrDestroyed()) { Uniques.Poison.GetPrefab(); }
                if (Uniques.Poison.ability.IsNullOrDestroyed()) { Uniques.Poison.GetAbility(); }
                if (Uniques.Physical.prefab_obj.IsNullOrDestroyed()) { Uniques.Physical.GetPrefab(); }
                if (Uniques.Physical.ability.IsNullOrDestroyed()) { Uniques.Physical.GetAbility(); }
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
                            if (name.Contains("/heraldofice/"))
                            {
                                if (Functions.Check_Texture(name) && name.Contains("icon") && Uniques.Ice.Icon.IsNullOrDestroyed())
                                {
                                    Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                                    Uniques.Ice.Icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                }
                            }
                            if (name.Contains("/heraldofash/"))
                            {
                                if (Functions.Check_Texture(name) && name.Contains("icon") && Uniques.Fire.Icon.IsNullOrDestroyed())
                                {
                                    Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                                    Uniques.Fire.Icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                }
                            }
                            if (name.Contains("/heraldofthunder/"))
                            {
                                if (Functions.Check_Texture(name) && name.Contains("icon") && Uniques.Lightning.Icon.IsNullOrDestroyed())
                                {
                                    Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                                    Uniques.Lightning.Icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                }
                            }
                            if (name.Contains("/heraldofagony/"))
                            {
                                if (Functions.Check_Texture(name) && name.Contains("icon") && Uniques.Poison.Icon.IsNullOrDestroyed())
                                {
                                    Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                                    Uniques.Poison.Icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                }
                            }
                            if (name.Contains("/heraldofpurity/"))
                            {
                                if (Functions.Check_Texture(name) && name.Contains("icon") && Uniques.Physical.Icon.IsNullOrDestroyed())
                                {
                                    Texture2D texture = Hud_Manager.asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                                    Uniques.Physical.Icon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                }
                            }
                        }
                        if (!Uniques.Ice.Icon.IsNullOrDestroyed() &&
                            !Uniques.Fire.Icon.IsNullOrDestroyed() &&
                            !Uniques.Lightning.Icon.IsNullOrDestroyed() &&
                            !Uniques.Poison.Icon.IsNullOrDestroyed() &&
                            !Uniques.Poison.Icon.IsNullOrDestroyed())
                        {
                            Loaded = true;
                        }
                        else { Loaded = false; }
                    }
                    catch { Main.logger_instance?.Error("Heralds Asset Error"); }
                    loading = false;
                }
            }
        }
        public class Basic
        {
            public static bool AddedToBasicList = false;
            public static readonly byte base_type = 25; //Small Idol
            public static readonly int base_id = 3;
            public static ItemList.EquipmentItem Item()
            {
                ItemList.EquipmentItem item = new ItemList.EquipmentItem
                {
                    classRequirement = ItemList.ClassRequirement.None,
                    implicits = implicits(),
                    subClassRequirement = ItemList.SubClassRequirement.None,
                    cannotDrop = true,
                    itemTags = ItemLocationTag.None,
                    levelRequirement = 15,
                    name = Languagues.Get_Subtype_Name(),
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
            public static Il2CppSystem.Collections.Generic.List<byte> SubType()
            {
                Il2CppSystem.Collections.Generic.List<byte> result = new Il2CppSystem.Collections.Generic.List<byte>();
                byte r = (byte)base_id;
                result.Add(r);

                return result;
            }
            private static Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> implicits()
            {
                Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> implicits = new Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit>();

                return implicits;
            }
        }
        public class Uniques
        {
            public static bool AddedToUniqueList = false;
            public static bool AddedToDictionary = false;
                        
            public static void AddToUniqueList()
            {
                if (!AddedToUniqueList && !Refs_Manager.unique_list.IsNullOrDestroyed())
                {
                    try
                    {
                        UniqueList.getUnique(0);
                        Refs_Manager.unique_list.uniques.Add(Ice.Item());
                        Refs_Manager.unique_list.uniques.Add(Fire.Item());
                        Refs_Manager.unique_list.uniques.Add(Lightning.Item());
                        Refs_Manager.unique_list.uniques.Add(Poison.Item());
                        Refs_Manager.unique_list.uniques.Add(Physical.Item());
                        AddedToUniqueList = true;
                    }
                    catch { Main.logger_instance?.Error("Herald of Ice Unique List Error"); }
                }
            }
            public static void AddToDictionary()
            {
                if (AddedToUniqueList && !AddedToDictionary && !Refs_Manager.unique_list.IsNullOrDestroyed())
                {
                    try
                    {
                        UniqueList.Entry ice_item = null;
                        UniqueList.Entry fire_item = null;
                        UniqueList.Entry ligtning_item = null;
                        UniqueList.Entry poison_item = null;
                        UniqueList.Entry physical_item = null;
                        if (Refs_Manager.unique_list.uniques.Count > 1)
                        {
                            foreach (UniqueList.Entry unique in Refs_Manager.unique_list.uniques)
                            {
                                if (unique.uniqueID == Ice.unique_id && unique.name == Ice.Get_Unique_Name()) { ice_item = unique; }
                                else if (unique.uniqueID == Fire.unique_id && unique.name == Fire.Get_Unique_Name()) { fire_item = unique; }
                                else if (unique.uniqueID == Lightning.unique_id && unique.name == Lightning.Get_Unique_Name()) { ligtning_item = unique; }
                                else if (unique.uniqueID == Poison.unique_id && unique.name == Poison.Get_Unique_Name()) { poison_item = unique; }
                                else if (unique.uniqueID == Physical.unique_id && unique.name == Physical.Get_Unique_Name()) { physical_item = unique; }
                            }
                        }
                        if (!ice_item.IsNullOrDestroyed() &&
                            !fire_item.IsNullOrDestroyed() &&
                            !ligtning_item.IsNullOrDestroyed() &&
                            !poison_item.IsNullOrDestroyed() &&
                            !physical_item.IsNullOrDestroyed())
                        {
                            Refs_Manager.unique_list.entryDictionary.Add(Ice.unique_id, ice_item);
                            Refs_Manager.unique_list.entryDictionary.Add(Fire.unique_id, fire_item);
                            Refs_Manager.unique_list.entryDictionary.Add(Lightning.unique_id, ligtning_item);
                            Refs_Manager.unique_list.entryDictionary.Add(Poison.unique_id, poison_item);
                            Refs_Manager.unique_list.entryDictionary.Add(Physical.unique_id, physical_item);
                            AddedToDictionary = true;
                        }
                    }
                    catch { Main.logger_instance?.Error("Herald of Ice Unique Dictionary Error"); }
                }
            }
            public static void SetDamage(GameObject prefab_obj, AT element, int target_max_health)
            {
                DamageEnemyOnHit damageEnemyOnHit = prefab_obj.GetComponent<DamageEnemyOnHit>();
                if (!damageEnemyOnHit.IsNullOrDestroyed())
                {
                    DamageStatsHolder.BaseDamageStats base_damage_stats = damageEnemyOnHit.baseDamageStats;
                    if (!base_damage_stats.IsNullOrDestroyed())
                    {
                        base_damage_stats.addedDamageScaling = 2f;
                        base_damage_stats.critChance = 0.05f;
                        base_damage_stats.critMultiplier = 2f;
                        //base_damage_stats.cullPercent = cull_percent;
                        base_damage_stats.convertAllAddedDamage = false;
                        int index = -1;
                        if (element == AT.Physical) { index = 0; }
                        else if (element == AT.Fire) { index = 1; }
                        else if (element == AT.Cold) { index = 2; }
                        else if (element == AT.Lightning) { index = 3; }
                        else if (element == AT.Necrotic) { index = 4; }
                        else if (element == AT.Void) { index = 5; }
                        else if (element == AT.Poison) { index = 6; }
                        if (index > -1)
                        {
                            for (int i = 0; i < base_damage_stats.damage.Count; i++)
                            {
                                if (i == index) { base_damage_stats.damage[i] = target_max_health / 20; }
                                else { base_damage_stats.damage[i] = 0f; }
                            }
                        }
                    }
                }
            }

            public class Ice
            {
                public static bool Initialize_ability = false;
                public static Ability ability = null;
                public static bool Initialize_prefab = false;
                public static GameObject prefab_obj = null;
                public static Sprite Icon = null;
                public static readonly ushort unique_id = 504;
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
                        levelRequirement = 15,
                        legendaryType = UniqueList.LegendaryType.LegendaryPotential,
                        overrideEffectiveLevelForLegendaryPotential = true,
                        effectiveLevelForLegendaryPotential = 0,
                        canDropRandomly = true,
                        rerollChance = 1,
                        itemModelType = UniqueList.ItemModelType.Unique,
                        subTypeForIM = 0,
                        baseType = Basic.base_type,
                        subTypes = Basic.SubType(),
                        mods = Mods(),
                        tooltipDescriptions = TooltipDescription(),
                        loreText = Get_Unique_Lore(),
                        tooltipEntries = TooltipEntries(),
                        oldSubTypeID = 0,
                        oldUniqueID = 0
                    };

                    return item;
                }

                public static void GetAbility()
                {
                    if (!Initialize_ability)
                    {
                        Initialize_ability = true;
                        if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                        {
                            foreach (Ability ab in Resources.FindObjectsOfTypeAll<Ability>())
                            {
                                if (ab.name == Save_Manager.instance.data.NewItems.HeraldOfIce.VFX)
                                {
                                    ability = new Ability
                                    {
                                        name = "Herald of Ice",
                                        abilityName = "Herald of Ice",
                                        abilitySprite = Icon,
                                        abilityObjectRotation = Ability.AbilityObjectRotation.FacingTarget,
                                        abilityObjectType = Ability.AbilityObjectType.Default,
                                        animation = AbilityAnimation.Cast,
                                        attachCastingVFXToCaster = true,
                                        attributeScaling = new Il2CppSystem.Collections.Generic.List<Ability.AttributeScaling>(),
                                        baseMovementAnimationLength = 1f,
                                        castingVFXPositioning = CastingVFXPositioning.Default,
                                        description = "",
                                        manaCost = 0f,
                                        moveOrAttackFallback = Ability.MoveOrAttackFallback.Wait,
                                        speedMultiplier = 1f,
                                        speedScaler = SP.CastSpeed,
                                        tags = ab.tags,
                                        useDelay = 0.4f,
                                        useDuration = 0.75f
                                    };
                                    break;
                                }
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
                            if (ab.name == Save_Manager.instance.data.NewItems.HeraldOfIce.VFX)
                            {
                                prefab_obj = Instantiate(ab.abilityPrefab, Vector3.zero, Quaternion.identity);
                                prefab_obj.active = false;
                                prefab_obj.name = "Herald of Ice prefab";
                                SphereCollider collider = prefab_obj.GetComponent<SphereCollider>();
                                if (!collider.IsNullOrDestroyed() && Save_Manager.instance.data.NewItems.HeraldOfIce.Enable_Radius)
                                { collider.radius = Save_Manager.instance.data.NewItems.HeraldOfIce.Radius; }
                                CreateVfxOnDeath vfx_on_death = prefab_obj.GetComponent<CreateVfxOnDeath>();
                                if (!vfx_on_death.IsNullOrDestroyed() && Save_Manager.instance.data.NewItems.HeraldOfIce.Enable_Radius)
                                { vfx_on_death.increasedRadius = Save_Manager.instance.data.NewItems.HeraldOfIce.Radius; }                                
                                break;
                            }
                        }
                        Initialize_prefab = false;
                    }
                }
                public static string Get_Unique_Name()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Ice.UniqueName.en; break; }
                        case Locales.Selected.French: { result = Languagues.Ice.UniqueName.en; break; }
                        case Locales.Selected.German: { result = Languagues.Ice.UniqueName.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Ice.UniqueName.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Ice.UniqueName.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Ice.UniqueName.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Ice.UniqueName.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Ice.UniqueName.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Ice.UniqueName.en; break; }
                    }

                    return result;
                }
                public static string Get_Unique_Description()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Ice.UniqueDescription.en; break; }
                        case Locales.Selected.French: { result = Languagues.Ice.UniqueDescription.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Ice.UniqueDescription.en; break; }
                        case Locales.Selected.German: { result = Languagues.Ice.UniqueDescription.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Ice.UniqueDescription.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Ice.UniqueDescription.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Ice.UniqueDescription.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Ice.UniqueDescription.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Ice.UniqueDescription.en; break; }
                    }

                    return result;
                }
                public static string Get_Unique_Lore()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Ice.Lore.en; break; }
                        case Locales.Selected.French: { result = Languagues.Ice.Lore.en; break; }
                        case Locales.Selected.German: { result = Languagues.Ice.Lore.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Ice.Lore.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Ice.Lore.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Ice.Lore.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Ice.Lore.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Ice.Lore.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Ice.Lore.en; break; }
                    }

                    return result;
                }
                public static bool Equipped()
                {
                    bool r = false;
                    foreach (ItemContainerEntry entry in Refs_Manager.player_actor.itemContainersManager.idols.content)
                    {
                        if (entry.data.uniqueID == unique_id) { r = true; break; }
                    }

                    return r;
                }                
                public static void Launch(GameObject actor, Actor target)
                {
                    if (!ability.IsNullOrDestroyed() && (!target.IsNullOrDestroyed()) && (!prefab_obj.IsNullOrDestroyed()))
                    {
                        SetDamage(prefab_obj, AT.Cold, target.health.maxHealth);
                        ability.abilityPrefab = Instantiate(prefab_obj, Vector3.zero, Quaternion.identity);
                        if (!ability.abilityPrefab.IsNullOrDestroyed())
                        {
                            ability.abilityPrefab.active = true;
                            ability.CastAfterDelay(actor.GetComponent<AbilityObjectConstructor>(), target.position(), target.position(), 0f);
                        }
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

                private static Il2CppSystem.Collections.Generic.List<UniqueItemMod> Mods()
                {
                    Il2CppSystem.Collections.Generic.List<UniqueItemMod> result = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();

                    return result;
                }
                private static Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> TooltipEntries()
                {
                    Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> result = new Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry>();
                    result.Add(new UniqueModDisplayListEntry(128));

                    return result;
                }
                private static Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> TooltipDescription()
                {
                    Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> result = new Il2CppSystem.Collections.Generic.List<ItemTooltipDescription>();
                    result.Add(new ItemTooltipDescription { description = Get_Unique_Description() });

                    return result;
                }
            }
            public class Fire
            {                
                public static bool Initialize_ability = false;
                public static Ability ability = null;
                public static bool Initialize_prefab = false;
                public static GameObject prefab_obj = null;
                public static Sprite Icon = null;
                public static readonly ushort unique_id = 505;
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
                        levelRequirement = 15,
                        legendaryType = UniqueList.LegendaryType.LegendaryPotential,
                        overrideEffectiveLevelForLegendaryPotential = true,
                        effectiveLevelForLegendaryPotential = 0,
                        canDropRandomly = true,
                        rerollChance = 1,
                        itemModelType = UniqueList.ItemModelType.Unique,
                        subTypeForIM = 0,
                        baseType = Basic.base_type,
                        subTypes = Basic.SubType(),
                        mods = Mods(),
                        tooltipDescriptions = TooltipDescription(),
                        loreText = Get_Unique_Lore(),
                        tooltipEntries = TooltipEntries(),
                        oldSubTypeID = 0,
                        oldUniqueID = 0
                    };

                    return item;
                }

                public static void GetAbility()
                {
                    if (!Initialize_ability)
                    {
                        Initialize_ability = true;
                        if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                        {
                            foreach (Ability ab in Resources.FindObjectsOfTypeAll<Ability>())
                            {
                                if (ab.name == Save_Manager.instance.data.NewItems.HeraldOfFire.VFX)
                                {
                                    ability = new Ability
                                    {
                                        name = "Herald of Ash",
                                        abilityName = "Herald of Ash",
                                        abilitySprite = Icon,
                                        abilityObjectRotation = Ability.AbilityObjectRotation.FacingTarget,
                                        abilityObjectType = Ability.AbilityObjectType.Default,
                                        animation = AbilityAnimation.Cast,
                                        attachCastingVFXToCaster = true,
                                        attributeScaling = new Il2CppSystem.Collections.Generic.List<Ability.AttributeScaling>(),
                                        baseMovementAnimationLength = 1f,
                                        castingVFXPositioning = CastingVFXPositioning.Default,
                                        description = "",
                                        manaCost = 0f,
                                        moveOrAttackFallback = Ability.MoveOrAttackFallback.Wait,
                                        speedMultiplier = 1f,
                                        speedScaler = SP.CastSpeed,
                                        tags = ab.tags,
                                        useDelay = 0.4f,
                                        useDuration = 0.75f
                                    };
                                    break;
                                }
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
                            if (ab.name == Save_Manager.instance.data.NewItems.HeraldOfFire.VFX)
                            {
                                prefab_obj = Instantiate(ab.abilityPrefab, Vector3.zero, Quaternion.identity);
                                prefab_obj.active = false;
                                prefab_obj.name = "Herald of Ash prefab";
                                SphereCollider collider = prefab_obj.GetComponent<SphereCollider>();
                                if (!collider.IsNullOrDestroyed() && Save_Manager.instance.data.NewItems.HeraldOfFire.Enable_Radius)
                                { collider.radius = Save_Manager.instance.data.NewItems.HeraldOfFire.Radius; }
                                CreateVfxOnDeath vfx_on_death = prefab_obj.GetComponent<CreateVfxOnDeath>();
                                if (!vfx_on_death.IsNullOrDestroyed() && Save_Manager.instance.data.NewItems.HeraldOfFire.Enable_Radius)
                                { vfx_on_death.increasedRadius = Save_Manager.instance.data.NewItems.HeraldOfFire.Radius; }
                                break;
                            }
                        }
                        Initialize_prefab = false;
                    }
                }
                public static string Get_Unique_Name()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Fire.UniqueName.en; break; }
                        case Locales.Selected.French: { result = Languagues.Fire.UniqueName.en; break; }
                        case Locales.Selected.German: { result = Languagues.Fire.UniqueName.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Fire.UniqueName.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Fire.UniqueName.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Fire.UniqueName.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Fire.UniqueName.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Fire.UniqueName.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Fire.UniqueName.en; break; }
                    }

                    return result;
                }
                public static string Get_Unique_Description()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Fire.UniqueDescription.en; break; }
                        case Locales.Selected.French: { result = Languagues.Fire.UniqueDescription.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Fire.UniqueDescription.en; break; }
                        case Locales.Selected.German: { result = Languagues.Fire.UniqueDescription.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Fire.UniqueDescription.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Fire.UniqueDescription.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Fire.UniqueDescription.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Fire.UniqueDescription.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Fire.UniqueDescription.en; break; }
                    }

                    return result;
                }
                public static string Get_Unique_Lore()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Fire.Lore.en; break; }
                        case Locales.Selected.French: { result = Languagues.Fire.Lore.en; break; }
                        case Locales.Selected.German: { result = Languagues.Fire.Lore.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Fire.Lore.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Fire.Lore.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Fire.Lore.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Fire.Lore.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Fire.Lore.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Fire.Lore.en; break; }
                    }

                    return result;
                }
                public static bool Equipped()
                {
                    bool r = false;
                    foreach (ItemContainerEntry entry in Refs_Manager.player_actor.itemContainersManager.idols.content)
                    {
                        if (entry.data.uniqueID == unique_id) { r = true; break; }
                    }

                    return r;
                }
                public static void Launch(GameObject actor, Actor target)
                {
                    if (!ability.IsNullOrDestroyed() && (!target.IsNullOrDestroyed()) && (!prefab_obj.IsNullOrDestroyed()))
                    {
                        SetDamage(prefab_obj, AT.Fire, target.health.maxHealth);
                        ability.abilityPrefab = Instantiate(prefab_obj, Vector3.zero, Quaternion.identity);
                        if (!ability.abilityPrefab.IsNullOrDestroyed())
                        {
                            ability.abilityPrefab.active = true;
                            ability.CastAfterDelay(actor.GetComponent<AbilityObjectConstructor>(), target.position(), target.position(), 0f);
                        }
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

                private static Il2CppSystem.Collections.Generic.List<UniqueItemMod> Mods()
                {
                    Il2CppSystem.Collections.Generic.List<UniqueItemMod> result = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();

                    return result;
                }
                private static Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> TooltipEntries()
                {
                    Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> result = new Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry>();
                    result.Add(new UniqueModDisplayListEntry(128));

                    return result;
                }
                private static Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> TooltipDescription()
                {
                    Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> result = new Il2CppSystem.Collections.Generic.List<ItemTooltipDescription>();
                    result.Add(new ItemTooltipDescription { description = Get_Unique_Description() });

                    return result;
                }
            }
            public class Lightning
            {
                public static bool Initialize_ability = false;
                public static Ability ability = null;
                public static bool Initialize_prefab = false;
                public static GameObject prefab_obj = null;
                public static Sprite Icon = null;
                public static readonly ushort unique_id = 506;
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
                        levelRequirement = 15,
                        legendaryType = UniqueList.LegendaryType.LegendaryPotential,
                        overrideEffectiveLevelForLegendaryPotential = true,
                        effectiveLevelForLegendaryPotential = 0,
                        canDropRandomly = true,
                        rerollChance = 1,
                        itemModelType = UniqueList.ItemModelType.Unique,
                        subTypeForIM = 0,
                        baseType = Basic.base_type,
                        subTypes = Basic.SubType(),
                        mods = Mods(),
                        tooltipDescriptions = TooltipDescription(),
                        loreText = Get_Unique_Lore(),
                        tooltipEntries = TooltipEntries(),
                        oldSubTypeID = 0,
                        oldUniqueID = 0
                    };

                    return item;
                }

                public static void GetAbility()
                {
                    if (!Initialize_ability)
                    {
                        Initialize_ability = true;
                        if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                        {
                            foreach (Ability ab in Resources.FindObjectsOfTypeAll<Ability>())
                            {
                                if (ab.name == Save_Manager.instance.data.NewItems.HeraldOfThunder.VFX)
                                {
                                    ability = new Ability
                                    {
                                        name = "Herald of Thunder",
                                        abilityName = "Herald of Thunder",
                                        abilitySprite = Icon,
                                        abilityObjectRotation = Ability.AbilityObjectRotation.FacingTarget,
                                        abilityObjectType = Ability.AbilityObjectType.Default,
                                        animation = AbilityAnimation.Cast,
                                        attachCastingVFXToCaster = true,
                                        attributeScaling = new Il2CppSystem.Collections.Generic.List<Ability.AttributeScaling>(),
                                        baseMovementAnimationLength = 1f,
                                        castingVFXPositioning = CastingVFXPositioning.Default,
                                        description = "",
                                        manaCost = 0f,
                                        moveOrAttackFallback = Ability.MoveOrAttackFallback.Wait,
                                        speedMultiplier = 1f,
                                        speedScaler = SP.CastSpeed,
                                        tags = ab.tags,
                                        useDelay = 0.4f,
                                        useDuration = 0.75f
                                    };
                                    break;
                                }
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
                            if (ab.name == Save_Manager.instance.data.NewItems.HeraldOfThunder.VFX)
                            {
                                prefab_obj = Instantiate(ab.abilityPrefab, Vector3.zero, Quaternion.identity);
                                prefab_obj.active = false;
                                prefab_obj.name = "Herald of Thunder prefab";
                                SphereCollider collider = prefab_obj.GetComponent<SphereCollider>();
                                if (!collider.IsNullOrDestroyed() && Save_Manager.instance.data.NewItems.HeraldOfThunder.Enable_Radius)
                                { collider.radius = Save_Manager.instance.data.NewItems.HeraldOfThunder.Radius; }
                                CreateVfxOnDeath vfx_on_death = prefab_obj.GetComponent<CreateVfxOnDeath>();
                                if(!vfx_on_death.IsNullOrDestroyed() && Save_Manager.instance.data.NewItems.HeraldOfThunder.Enable_Radius)
                                { vfx_on_death.increasedRadius = Save_Manager.instance.data.NewItems.HeraldOfThunder.Radius; }
                                break;
                            }
                        }
                        Initialize_prefab = false;
                    }
                }
                public static string Get_Unique_Name()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Lightning.UniqueName.en; break; }
                        case Locales.Selected.French: { result = Languagues.Lightning.UniqueName.en; break; }
                        case Locales.Selected.German: { result = Languagues.Lightning.UniqueName.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Lightning.UniqueName.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Lightning.UniqueName.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Lightning.UniqueName.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Lightning.UniqueName.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Lightning.UniqueName.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Lightning.UniqueName.en; break; }
                    }

                    return result;
                }
                public static string Get_Unique_Description()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Lightning.UniqueDescription.en; break; }
                        case Locales.Selected.French: { result = Languagues.Lightning.UniqueDescription.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Lightning.UniqueDescription.en; break; }
                        case Locales.Selected.German: { result = Languagues.Lightning.UniqueDescription.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Lightning.UniqueDescription.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Lightning.UniqueDescription.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Lightning.UniqueDescription.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Lightning.UniqueDescription.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Lightning.UniqueDescription.en; break; }
                    }

                    return result;
                }
                public static string Get_Unique_Lore()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Lightning.Lore.en; break; }
                        case Locales.Selected.French: { result = Languagues.Lightning.Lore.en; break; }
                        case Locales.Selected.German: { result = Languagues.Lightning.Lore.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Lightning.Lore.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Lightning.Lore.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Lightning.Lore.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Lightning.Lore.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Lightning.Lore.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Lightning.Lore.en; break; }
                    }

                    return result;
                }
                public static bool Equipped()
                {
                    bool r = false;
                    foreach (ItemContainerEntry entry in Refs_Manager.player_actor.itemContainersManager.idols.content)
                    {
                        if (entry.data.uniqueID == unique_id) { r = true; break; }
                    }

                    return r;
                }
                public static void Launch(GameObject actor, Actor target)
                {
                    if (!ability.IsNullOrDestroyed() && (!target.IsNullOrDestroyed()) && (!prefab_obj.IsNullOrDestroyed()))
                    {
                        SetDamage(prefab_obj, AT.Lightning, target.health.maxHealth);
                        ability.abilityPrefab = Instantiate(prefab_obj, Vector3.zero, Quaternion.identity);
                        if (!ability.abilityPrefab.IsNullOrDestroyed())
                        {
                            ability.abilityPrefab.active = true;
                            ability.CastAfterDelay(actor.GetComponent<AbilityObjectConstructor>(), target.position(), target.position(), 0f);
                        }
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

                private static Il2CppSystem.Collections.Generic.List<UniqueItemMod> Mods()
                {
                    Il2CppSystem.Collections.Generic.List<UniqueItemMod> result = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();

                    return result;
                }
                private static Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> TooltipEntries()
                {
                    Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> result = new Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry>();
                    result.Add(new UniqueModDisplayListEntry(128));

                    return result;
                }
                private static Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> TooltipDescription()
                {
                    Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> result = new Il2CppSystem.Collections.Generic.List<ItemTooltipDescription>();
                    result.Add(new ItemTooltipDescription { description = Get_Unique_Description() });

                    return result;
                }
            }
            public class Poison
            {                
                public static bool Initialize_ability = false;
                public static Ability ability = null;
                public static bool Initialize_prefab = false;
                public static GameObject prefab_obj = null;
                public static Sprite Icon = null;
                public static readonly ushort unique_id = 507;
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
                        levelRequirement = 15,
                        legendaryType = UniqueList.LegendaryType.LegendaryPotential,
                        overrideEffectiveLevelForLegendaryPotential = true,
                        effectiveLevelForLegendaryPotential = 0,
                        canDropRandomly = true,
                        rerollChance = 1,
                        itemModelType = UniqueList.ItemModelType.Unique,
                        subTypeForIM = 0,
                        baseType = Basic.base_type,
                        subTypes = Basic.SubType(),
                        mods = Mods(),
                        tooltipDescriptions = TooltipDescription(),
                        loreText = Get_Unique_Lore(),
                        tooltipEntries = TooltipEntries(),
                        oldSubTypeID = 0,
                        oldUniqueID = 0
                    };

                    return item;
                }

                public static void GetAbility()
                {
                    if (!Initialize_ability)
                    {
                        Initialize_ability = true;
                        if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                        {
                            foreach (Ability ab in Resources.FindObjectsOfTypeAll<Ability>())
                            {
                                if (ab.name == Save_Manager.instance.data.NewItems.HeraldOfAgony.VFX)
                                {
                                    ability = new Ability
                                    {
                                        name = "Herald of Agony",
                                        abilityName = "Herald of Agony",
                                        abilitySprite = Icon,
                                        abilityObjectRotation = Ability.AbilityObjectRotation.FacingTarget,
                                        abilityObjectType = Ability.AbilityObjectType.Default,
                                        animation = AbilityAnimation.Cast,
                                        attachCastingVFXToCaster = true,
                                        attributeScaling = new Il2CppSystem.Collections.Generic.List<Ability.AttributeScaling>(),
                                        baseMovementAnimationLength = 1f,
                                        castingVFXPositioning = CastingVFXPositioning.Default,
                                        description = "",
                                        manaCost = 0f,
                                        moveOrAttackFallback = Ability.MoveOrAttackFallback.Wait,
                                        speedMultiplier = 1f,
                                        speedScaler = SP.CastSpeed,
                                        tags = ab.tags,
                                        useDelay = 0.4f,
                                        useDuration = 0.75f
                                    };
                                    break;
                                }
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
                            if (ab.name == Save_Manager.instance.data.NewItems.HeraldOfAgony.VFX)
                            {
                                prefab_obj = Instantiate(ab.abilityPrefab, Vector3.zero, Quaternion.identity);
                                prefab_obj.active = false;
                                prefab_obj.name = "Herald of Agony prefab";
                                SphereCollider collider = prefab_obj.GetComponent<SphereCollider>();
                                if (!collider.IsNullOrDestroyed() && Save_Manager.instance.data.NewItems.HeraldOfAgony.Enable_Radius)
                                { collider.radius = Save_Manager.instance.data.NewItems.HeraldOfAgony.Radius; }
                                CreateVfxOnDeath vfx_on_death = prefab_obj.GetComponent<CreateVfxOnDeath>();
                                if (!vfx_on_death.IsNullOrDestroyed() && Save_Manager.instance.data.NewItems.HeraldOfAgony.Enable_Radius)
                                { vfx_on_death.increasedRadius = Save_Manager.instance.data.NewItems.HeraldOfAgony.Radius; }
                                break;
                            }
                        }
                        Initialize_prefab = false;
                    }
                }
                public static string Get_Unique_Name()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Poison.UniqueName.en; break; }
                        case Locales.Selected.French: { result = Languagues.Poison.UniqueName.en; break; }
                        case Locales.Selected.German: { result = Languagues.Poison.UniqueName.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Poison.UniqueName.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Poison.UniqueName.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Poison.UniqueName.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Poison.UniqueName.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Poison.UniqueName.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Poison.UniqueName.en; break; }
                    }

                    return result;
                }
                public static string Get_Unique_Description()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Poison.UniqueDescription.en; break; }
                        case Locales.Selected.French: { result = Languagues.Poison.UniqueDescription.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Poison.UniqueDescription.en; break; }
                        case Locales.Selected.German: { result = Languagues.Poison.UniqueDescription.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Poison.UniqueDescription.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Poison.UniqueDescription.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Poison.UniqueDescription.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Poison.UniqueDescription.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Poison.UniqueDescription.en; break; }
                    }

                    return result;
                }
                public static string Get_Unique_Lore()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Poison.Lore.en; break; }
                        case Locales.Selected.French: { result = Languagues.Poison.Lore.en; break; }
                        case Locales.Selected.German: { result = Languagues.Poison.Lore.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Poison.Lore.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Poison.Lore.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Poison.Lore.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Poison.Lore.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Poison.Lore.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Poison.Lore.en; break; }
                    }

                    return result;
                }
                public static bool Equipped()
                {
                    bool r = false;
                    foreach (ItemContainerEntry entry in Refs_Manager.player_actor.itemContainersManager.idols.content)
                    {
                        if (entry.data.uniqueID == unique_id) { r = true; break; }
                    }

                    return r;
                }
                public static void Launch(GameObject actor, Actor target)
                {
                    if (!ability.IsNullOrDestroyed() && (!target.IsNullOrDestroyed()) && (!prefab_obj.IsNullOrDestroyed()))
                    {
                        SetDamage(prefab_obj, AT.Poison, target.health.maxHealth);
                        ability.abilityPrefab = Instantiate(prefab_obj, Vector3.zero, Quaternion.identity);
                        if (!ability.abilityPrefab.IsNullOrDestroyed())
                        {
                            ability.abilityPrefab.active = true;
                            ability.CastAfterDelay(actor.GetComponent<AbilityObjectConstructor>(), target.position(), target.position(), 0f);
                        }
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

                private static Il2CppSystem.Collections.Generic.List<UniqueItemMod> Mods()
                {
                    Il2CppSystem.Collections.Generic.List<UniqueItemMod> result = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();

                    return result;
                }
                private static Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> TooltipEntries()
                {
                    Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> result = new Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry>();
                    result.Add(new UniqueModDisplayListEntry(128));

                    return result;
                }
                private static Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> TooltipDescription()
                {
                    Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> result = new Il2CppSystem.Collections.Generic.List<ItemTooltipDescription>();
                    result.Add(new ItemTooltipDescription { description = Get_Unique_Description() });

                    return result;
                }
            }
            public class Physical
            {
                public static bool Initialize_ability = false;
                public static Ability ability = null;
                public static bool Initialize_prefab = false;
                public static GameObject prefab_obj = null;
                public static Sprite Icon = null;
                public static readonly ushort unique_id = 509;
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
                        levelRequirement = 15,
                        legendaryType = UniqueList.LegendaryType.LegendaryPotential,
                        overrideEffectiveLevelForLegendaryPotential = true,
                        effectiveLevelForLegendaryPotential = 0,
                        canDropRandomly = true,
                        rerollChance = 1,
                        itemModelType = UniqueList.ItemModelType.Unique,
                        subTypeForIM = 0,
                        baseType = Basic.base_type,
                        subTypes = Basic.SubType(),
                        mods = Mods(),
                        tooltipDescriptions = TooltipDescription(),
                        loreText = Get_Unique_Lore(),
                        tooltipEntries = TooltipEntries(),
                        oldSubTypeID = 0,
                        oldUniqueID = 0
                    };

                    return item;
                }

                public static void GetAbility()
                {
                    if (!Initialize_ability)
                    {
                        Initialize_ability = true;
                        if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                        {
                            foreach (Ability ab in Resources.FindObjectsOfTypeAll<Ability>())
                            {
                                if (ab.name == Save_Manager.instance.data.NewItems.HeraldOfPurity.VFX)
                                {
                                    ability = new Ability
                                    {
                                        name = "Herald of Purity",
                                        abilityName = "Herald of Purity",
                                        abilitySprite = Icon,
                                        abilityObjectRotation = Ability.AbilityObjectRotation.FacingTarget,
                                        abilityObjectType = Ability.AbilityObjectType.Default,
                                        animation = AbilityAnimation.Cast,
                                        attachCastingVFXToCaster = true,
                                        attributeScaling = new Il2CppSystem.Collections.Generic.List<Ability.AttributeScaling>(),
                                        baseMovementAnimationLength = 1f,
                                        castingVFXPositioning = CastingVFXPositioning.Default,
                                        description = "",
                                        manaCost = 0f,
                                        moveOrAttackFallback = Ability.MoveOrAttackFallback.Wait,
                                        speedMultiplier = 1f,
                                        speedScaler = SP.CastSpeed,
                                        tags = ab.tags,
                                        useDelay = 0.4f,
                                        useDuration = 0.75f
                                    };
                                    break;
                                }
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
                            if (ab.name == Save_Manager.instance.data.NewItems.HeraldOfPurity.VFX)
                            {
                                prefab_obj = Instantiate(ab.abilityPrefab, Vector3.zero, Quaternion.identity);
                                prefab_obj.active = false;
                                prefab_obj.name = "Herald of Purity prefab";
                                SphereCollider collider = prefab_obj.GetComponent<SphereCollider>();
                                if (!collider.IsNullOrDestroyed() && Save_Manager.instance.data.NewItems.HeraldOfPurity.Enable_Radius)
                                { collider.radius = Save_Manager.instance.data.NewItems.HeraldOfPurity.Radius; }
                                CreateVfxOnDeath vfx_on_death = prefab_obj.GetComponent<CreateVfxOnDeath>();
                                if (!vfx_on_death.IsNullOrDestroyed() && Save_Manager.instance.data.NewItems.HeraldOfPurity.Enable_Radius)
                                { vfx_on_death.increasedRadius = Save_Manager.instance.data.NewItems.HeraldOfPurity.Radius; }
                                break;
                            }
                        }
                        Initialize_prefab = false;
                    }
                }
                public static string Get_Unique_Name()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Physical.UniqueName.en; break; }
                        case Locales.Selected.French: { result = Languagues.Physical.UniqueName.en; break; }
                        case Locales.Selected.German: { result = Languagues.Physical.UniqueName.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Physical.UniqueName.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Physical.UniqueName.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Physical.UniqueName.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Physical.UniqueName.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Physical.UniqueName.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Physical.UniqueName.en; break; }
                    }

                    return result;
                }
                public static string Get_Unique_Description()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Physical.UniqueDescription.en; break; }
                        case Locales.Selected.French: { result = Languagues.Physical.UniqueDescription.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Physical.UniqueDescription.en; break; }
                        case Locales.Selected.German: { result = Languagues.Physical.UniqueDescription.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Physical.UniqueDescription.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Physical.UniqueDescription.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Physical.UniqueDescription.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Physical.UniqueDescription.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Physical.UniqueDescription.en; break; }
                    }

                    return result;
                }
                public static string Get_Unique_Lore()
                {
                    string result = "";
                    switch (Locales.current)
                    {
                        case Locales.Selected.English: { result = Languagues.Physical.Lore.en; break; }
                        case Locales.Selected.French: { result = Languagues.Physical.Lore.en; break; }
                        case Locales.Selected.German: { result = Languagues.Physical.Lore.en; break; }
                        case Locales.Selected.Korean: { result = Languagues.Physical.Lore.en; break; }
                        case Locales.Selected.Russian: { result = Languagues.Physical.Lore.en; break; }
                        case Locales.Selected.Polish: { result = Languagues.Physical.Lore.en; break; }
                        case Locales.Selected.Portuguese: { result = Languagues.Physical.Lore.en; break; }
                        case Locales.Selected.Chinese: { result = Languagues.Physical.Lore.en; break; }
                        case Locales.Selected.Spanish: { result = Languagues.Physical.Lore.en; break; }
                    }

                    return result;
                }
                public static bool Equipped()
                {
                    bool r = false;
                    foreach (ItemContainerEntry entry in Refs_Manager.player_actor.itemContainersManager.idols.content)
                    {
                        if (entry.data.uniqueID == unique_id) { r = true; break; }
                    }

                    return r;
                }
                public static void Launch(GameObject actor, Actor target)
                {
                    if (!ability.IsNullOrDestroyed() && (!target.IsNullOrDestroyed()) && (!prefab_obj.IsNullOrDestroyed()))
                    {
                        SetDamage(prefab_obj, AT.Physical, target.health.maxHealth);
                        ability.abilityPrefab = Instantiate(prefab_obj, Vector3.zero, Quaternion.identity);
                        if (!ability.abilityPrefab.IsNullOrDestroyed())
                        {
                            ability.abilityPrefab.active = true;
                            ability.CastAfterDelay(actor.GetComponent<AbilityObjectConstructor>(), target.position(), target.position(), 0f);
                        }
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

                private static Il2CppSystem.Collections.Generic.List<UniqueItemMod> Mods()
                {
                    Il2CppSystem.Collections.Generic.List<UniqueItemMod> result = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();

                    return result;
                }
                private static Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> TooltipEntries()
                {
                    Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> result = new Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry>();
                    result.Add(new UniqueModDisplayListEntry(128));

                    return result;
                }
                private static Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> TooltipDescription()
                {
                    Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> result = new Il2CppSystem.Collections.Generic.List<ItemTooltipDescription>();
                    result.Add(new ItemTooltipDescription { description = Get_Unique_Description() });

                    return result;
                }
            }

            [HarmonyPatch(typeof(InventoryItemUI), "SetImageSpritesAndColours")]
            public class InventoryItemUI_SetImageSpritesAndColours
            {
                [HarmonyPostfix]
                static void Postfix(ref InventoryItemUI __instance)
                {
                    if (__instance.EntryRef.data.getAsUnpacked().FullName == Ice.Get_Unique_Name() && !Ice.Icon.IsNullOrDestroyed())
                    {
                        __instance.contentImage.sprite = Ice.Icon;
                    }
                    else if (__instance.EntryRef.data.getAsUnpacked().FullName == Fire.Get_Unique_Name() && !Fire.Icon.IsNullOrDestroyed())
                    {
                        __instance.contentImage.sprite = Fire.Icon;
                    }
                    else if (__instance.EntryRef.data.getAsUnpacked().FullName == Lightning.Get_Unique_Name() && !Lightning.Icon.IsNullOrDestroyed())
                    {
                        __instance.contentImage.sprite = Lightning.Icon;
                    }
                    else if (__instance.EntryRef.data.getAsUnpacked().FullName == Poison.Get_Unique_Name() && !Poison.Icon.IsNullOrDestroyed())
                    {
                        __instance.contentImage.sprite = Poison.Icon;
                    }
                    else if (__instance.EntryRef.data.getAsUnpacked().FullName == Physical.Get_Unique_Name() && !Physical.Icon.IsNullOrDestroyed())
                    {
                        __instance.contentImage.sprite = Physical.Icon;
                    }
                }
            }

            [HarmonyPatch(typeof(UITooltipItem), "GetItemSprite")]
            public class UITooltipItem_GetItemSprite
            {
                [HarmonyPostfix]
                static void Postfix(ref Sprite __result, ItemData __0)
                {
                    if (__0.getAsUnpacked().FullName == Ice.Get_Unique_Name() && !Ice.Icon.IsNullOrDestroyed())
                    {
                        __result = Ice.Icon;
                    }
                    else if (__0.getAsUnpacked().FullName == Fire.Get_Unique_Name() && !Fire.Icon.IsNullOrDestroyed())
                    {
                        __result = Fire.Icon;
                    }
                    else if (__0.getAsUnpacked().FullName == Lightning.Get_Unique_Name() && !Lightning.Icon.IsNullOrDestroyed())
                    {
                        __result = Lightning.Icon;
                    }
                    else if (__0.getAsUnpacked().FullName == Poison.Get_Unique_Name() && !Poison.Icon.IsNullOrDestroyed())
                    {
                        __result = Poison.Icon;
                    }
                    else if (__0.getAsUnpacked().FullName == Physical.Get_Unique_Name() && !Physical.Icon.IsNullOrDestroyed())
                    {
                        __result = Physical.Icon;
                    }
                }
            }
        }
        public class Languagues
        {
            public static string basic_subtype_name_key = "Item_SubType_Name_" + Basic.base_type + "_" + Basic.base_id;
            public static string Get_Subtype_Name()
            {
                string result = "";
                switch (Locales.current)
                {
                    case Locales.Selected.English: { result = SubType.en; break; }
                    case Locales.Selected.French: { result = SubType.en; break; }
                    case Locales.Selected.German: { result = SubType.en; break; }
                    case Locales.Selected.Russian: { result = SubType.en; break; }
                    case Locales.Selected.Portuguese: { result = SubType.en; break; }
                    case Locales.Selected.Korean: { result = SubType.en; break; }
                    case Locales.Selected.Polish: { result = SubType.en; break; }
                    case Locales.Selected.Chinese: { result = SubType.en; break; }
                    case Locales.Selected.Spanish: { result = SubType.en; break; }
                }

                return result;
            }
            public class SubType
            {
                public static string en = "Herald Idol";
                //Add all languages here
            }
            public class Ice
            {
                public static string unique_name_key = "Unique_Name_" + Uniques.Ice.unique_id;
                public static string unique_description_key = "Unique_Tooltip_0_" + Uniques.Ice.unique_id;
                public static string unique_lore_key = "Unique_Lore_" + Uniques.Ice.unique_id;

                public class UniqueName
                {
                    public static string en = "Herald of Ice";
                    //Add all languages here
                }
                public class UniqueDescription
                {
                    public static string en = "Grants a buff, when you or your minions Kill a monster with cold damage, this item will cause them to explode and deal 5% of their Life as cold damage to enemies near them";
                    //Add all languages here
                }
                public class Lore
                {
                    public static readonly string en = "";
                    //Add all languages here
                }
            }
            public class Fire
            {
                public static string unique_name_key = "Unique_Name_" + Uniques.Fire.unique_id;
                public static string unique_description_key = "Unique_Tooltip_0_" + Uniques.Fire.unique_id;
                public static string unique_lore_key = "Unique_Lore_" + Uniques.Fire.unique_id;

                public class UniqueName
                {
                    public static string en = "Herald of Ash";
                    //Add all languages here
                }
                public class UniqueDescription
                {
                    public static string en = "Grants a buff, when you or your minions Kill a monster with fire damage, this item will cause them to explode and deal 5% of their Life as fire damage to enemies near them";
                    //Add all languages here
                }
                public class Lore
                {
                    public static readonly string en = "";
                    //Add all languages here
                }
            }
            public class Lightning
            {
                public static string unique_name_key = "Unique_Name_" + Uniques.Lightning.unique_id;
                public static string unique_description_key = "Unique_Tooltip_0_" + Uniques.Lightning.unique_id;
                public static string unique_lore_key = "Unique_Lore_" + Uniques.Lightning.unique_id;

                public class UniqueName
                {
                    public static string en = "Herald of Thunder";
                    //Add all languages here
                }
                public class UniqueDescription
                {
                    public static string en = "Grants a buff, when you or your minions Kill a monster with lightning damage, this item will cause them to explode and deal 5% of their Life as lightning damage to enemies near them";
                    //Add all languages here
                }
                public class Lore
                {
                    public static readonly string en = "";
                    //Add all languages here
                }
            }
            public class Poison
            {
                public static string unique_name_key = "Unique_Name_" + Uniques.Poison.unique_id;
                public static string unique_description_key = "Unique_Tooltip_0_" + Uniques.Poison.unique_id;
                public static string unique_lore_key = "Unique_Lore_" + Uniques.Poison.unique_id;

                public class UniqueName
                {
                    public static string en = "Herald of Agony";
                    //Add all languages here
                }
                public class UniqueDescription
                {
                    public static string en = "Grants a buff, when you or your minions Kill a monster with poison damage, this item will cause them to explode and deal 5% of their Life as poison damage to enemies near them";
                    //Add all languages here
                }
                public class Lore
                {
                    public static readonly string en = "";
                    //Add all languages here
                }
            }
            public class Physical
            {
                public static string unique_name_key = "Unique_Name_" + Uniques.Physical.unique_id;
                public static string unique_description_key = "Unique_Tooltip_0_" + Uniques.Physical.unique_id;
                public static string unique_lore_key = "Unique_Lore_" + Uniques.Physical.unique_id;

                public class UniqueName
                {
                    public static string en = "Herald of Purity";
                    //Add all languages here
                }
                public class UniqueDescription
                {
                    public static string en = "Grants a buff, when you or your minions Kill a monster with physical damage, this item will cause them to explode and deal 5% of their Life as physical damage to enemies near them";
                    //Add all languages here
                }
                public class Lore
                {
                    public static readonly string en = "";
                    //Add all languages here
                }
            }

            [HarmonyPatch(typeof(Localization), "TryGetText")]
            public class Localization_TryGetText
            {
                [HarmonyPrefix]
                static bool Prefix(ref bool __result, string __0) //, Il2CppSystem.String __1)
                {
                    bool result = true;
                    if (__0 == basic_subtype_name_key ||
                        __0 == Ice.unique_name_key || __0 == Ice.unique_description_key || __0 == Ice.unique_lore_key ||
                        __0 == Fire.unique_name_key || __0 == Fire.unique_description_key || __0 == Fire.unique_lore_key ||
                        __0 == Lightning.unique_name_key || __0 == Lightning.unique_description_key || __0 == Lightning.unique_lore_key ||
                        __0 == Poison.unique_name_key || __0 == Poison.unique_description_key || __0 == Poison.unique_lore_key ||
                        __0 == Physical.unique_name_key || __0 == Physical.unique_description_key || __0 == Physical.unique_lore_key)
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
                        __result = Get_Subtype_Name();
                        result = false;
                    }
                    //Ice
                    else if (__0 == Ice.unique_name_key)
                    {
                        __result = Uniques.Ice.Get_Unique_Name();
                        result = false;
                    }
                    else if (__0 == Ice.unique_description_key)
                    {
                        string description = Uniques.Ice.Get_Unique_Description();
                        if (description != "")
                        {
                            __result = description;
                            result = false;
                        }
                    }
                    else if (__0 == Ice.unique_lore_key)
                    {
                        string lore = Uniques.Ice.Get_Unique_Lore();
                        if (lore != "")
                        {
                            __result = lore;
                            result = false;
                        }
                    }
                    //Fire
                    else if (__0 == Fire.unique_name_key)
                    {
                        __result = Uniques.Fire.Get_Unique_Name();
                        result = false;
                    }
                    else if (__0 == Fire.unique_description_key)
                    {
                        string description = Uniques.Fire.Get_Unique_Description();
                        if (description != "")
                        {
                            __result = description;
                            result = false;
                        }
                    }
                    else if (__0 == Fire.unique_lore_key)
                    {
                        string lore = Uniques.Fire.Get_Unique_Lore();
                        if (lore != "")
                        {
                            __result = lore;
                            result = false;
                        }
                    }
                    //Lightning
                    else if (__0 == Lightning.unique_name_key)
                    {
                        __result = Uniques.Lightning.Get_Unique_Name();
                        result = false;
                    }
                    else if (__0 == Lightning.unique_description_key)
                    {
                        string description = Uniques.Lightning.Get_Unique_Description();
                        if (description != "")
                        {
                            __result = description;
                            result = false;
                        }
                    }
                    else if (__0 == Fire.unique_lore_key)
                    {
                        string lore = Uniques.Lightning.Get_Unique_Lore();
                        if (lore != "")
                        {
                            __result = lore;
                            result = false;
                        }
                    }
                    //Poison
                    else if (__0 == Poison.unique_name_key)
                    {
                        __result = Uniques.Poison.Get_Unique_Name();
                        result = false;
                    }
                    else if (__0 == Poison.unique_description_key)
                    {
                        string description = Uniques.Poison.Get_Unique_Description();
                        if (description != "")
                        {
                            __result = description;
                            result = false;
                        }
                    }
                    else if (__0 == Poison.unique_lore_key)
                    {
                        string lore = Uniques.Poison.Get_Unique_Lore();
                        if (lore != "")
                        {
                            __result = lore;
                            result = false;
                        }
                    }
                    //Physical
                    else if (__0 == Physical.unique_name_key)
                    {
                        __result = Uniques.Physical.Get_Unique_Name();
                        result = false;
                    }
                    else if (__0 == Physical.unique_description_key)
                    {
                        string description = Uniques.Physical.Get_Unique_Description();
                        if (description != "")
                        {
                            __result = description;
                            result = false;
                        }
                    }
                    else if (__0 == Physical.unique_lore_key)
                    {
                        string lore = Uniques.Physical.Get_Unique_Lore();
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
                //Main.logger_instance.Warning("OnKill : " + ability.abilityName);

                bool ice = true;
                bool fire = true;
                bool lightning = true;
                bool poison = true;
                bool physical = true;

                bool force_ice = false;
                bool force_fire = false;
                bool force_lightning = false;
                bool force_poison = false;
                bool force_physical = false;

                //AbilityMutator damage convertion
                if (!Refs_Manager.player_treedata.IsNullOrDestroyed())
                {
                    foreach (LocalTreeData.SkillTreeData skill_tree_data in Refs_Manager.player_treedata.specialisedSkillTrees)
                    {
                        if (skill_tree_data.ability.abilityName == ability.abilityName)
                        {
                            if (!skill_tree_data.mutator.IsNullOrDestroyed())
                            {
                                Il2CppSystem.Type il2cpp_type = null;
                                try { il2cpp_type = skill_tree_data.mutator.GetIl2CppType(); }
                                catch { Main.logger_instance?.Error("Can't get Mutator type"); }

                                if (!il2cpp_type.IsNullOrDestroyed())
                                {
                                    //Add skills here (Fix conversion)
                                    if (il2cpp_type.ToString() == "WarpathMutator")
                                    {
                                        WarpathMutator m = skill_tree_data.mutator.TryCast<WarpathMutator>();
                                        if (!m.IsNullOrDestroyed())
                                        {
                                            if (m.fireConversion)
                                            {
                                                physical = false;
                                                force_fire = true;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }

                //Items damage convertion                
                if (!Refs_Manager.player_actor.IsNullOrDestroyed())
                {                    
                    if (ability.abilityName == "Aura Of Decay")     //Aura of decay and Ash Wake (convert poison to fire)
                    {
                        try //in case we don't have any boots equiped
                        {
                            if (Refs_Manager.player_actor.itemContainersManager.equipment.boots.content.data.uniqueID == 461)
                            {
                                poison = false;
                                force_fire = true;
                            }
                        }
                        catch { }
                    }
                }

                if ((!Refs_Manager.player_actor.IsNullOrDestroyed()) && (!ability.IsNullOrDestroyed()) && (!killedActor.IsNullOrDestroyed()))
                {
                    if ((Uniques.Ice.Equipped()) && (ice) && ((ability.tags.HasFlag(AT.Cold)) || (force_ice))) { Uniques.Ice.Launch(Refs_Manager.player_actor.gameObject, killedActor); }
                    if ((Uniques.Fire.Equipped()) && (fire) && ((ability.tags.HasFlag(AT.Fire)) || (force_fire))) { Uniques.Fire.Launch(Refs_Manager.player_actor.gameObject, killedActor); }
                    if ((Uniques.Lightning.Equipped()) && (lightning) && ((ability.tags.HasFlag(AT.Lightning)) || (force_lightning))) { Uniques.Lightning.Launch(Refs_Manager.player_actor.gameObject, killedActor); }
                    if ((Uniques.Poison.Equipped()) && (poison) && ((ability.tags.HasFlag(AT.Poison)) || (force_poison))) { Uniques.Poison.Launch(Refs_Manager.player_actor.gameObject, killedActor); }
                    if ((Uniques.Physical.Equipped()) && (physical) && ((ability.tags.HasFlag(AT.Physical)) || (force_physical))) { Uniques.Physical.Launch(Refs_Manager.player_actor.gameObject, killedActor); }
                }
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
                if ((!summon.IsNullOrDestroyed()) && (!ability.IsNullOrDestroyed()) && (!killedActor.IsNullOrDestroyed()))
                {
                    if ((Uniques.Ice.Equipped()) && (ability.tags.HasFlag(AT.Cold))) { Uniques.Ice.Launch(summon.gameObject, killedActor); }
                    if ((Uniques.Fire.Equipped()) && (ability.tags.HasFlag(AT.Fire))) { Uniques.Fire.Launch(summon.gameObject, killedActor); }
                    if ((Uniques.Lightning.Equipped()) && (ability.tags.HasFlag(AT.Lightning))) { Uniques.Lightning.Launch(summon.gameObject, killedActor); }
                    if ((Uniques.Poison.Equipped()) && (ability.tags.HasFlag(AT.Poison))) { Uniques.Poison.Launch(summon.gameObject, killedActor); }
                    if ((Uniques.Physical.Equipped()) && (ability.tags.HasFlag(AT.Physical))) { Uniques.Physical.Launch(summon.gameObject, killedActor); }
                }
            }
        }        
    }
}
