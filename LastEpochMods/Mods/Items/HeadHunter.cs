using LastEpochMods.Ui;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpochMods.Mods.Items
{
    public class HeadHunter
    {
        public static bool Initialized = false;
        public static void Init()
        {
            if (!Refs.Tracker_Initialized)
            {
                try
                {
                    UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<HHTracker>();
                    Refs.Tracker_Initialized = true;
                }
                catch { }
            }
            if (!Refs.BaseItemAdd_Initialized)
            {
                try
                {
                    ItemList.instance.EquippableItems[2].subItems.Add(Basic.Item());
                    Refs.BaseItemAdd_Initialized = true;
                }
                catch { }
            }
            if (!Refs.UniqueItemAdd_Initialized)
            {
                try
                {
                    UniqueList.instance.uniques.Add(Unique.Item());
                    Refs.UniqueItemAdd_Initialized = true;
                }
                catch { }
            }
            if (!Refs.SceneLoaded_Initialized)
            {
                try
                {
                    SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(Events.OnSceneLoaded));
                    Refs.SceneLoaded_Initialized = true;
                }
                catch { }
            }
            if ((Refs.Tracker_Initialized) && (Refs.BaseItemAdd_Initialized) &&
                (Refs.UniqueItemAdd_Initialized) && (Refs.SceneLoaded_Initialized))
            {
                Initialized = true;
            }
        }
                       
        public class Basic
        {
            public static int base_id = 99;
            public static ItemList.EquipmentItem Item()
            {

                ItemList.EquipmentImplicit implicit_0 = new ItemList.EquipmentImplicit
                {
                    implicitMaxValue = 40,
                    implicitValue = 25,
                    property = SP.Health,
                    specialTag = 0,
                    tags = AT.None,
                    type = BaseStats.ModType.ADDED
                };
                Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> implicits = new Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit>();
                implicits.Add(implicit_0);

                ItemList.EquipmentItem item = new ItemList.EquipmentItem();

                item.classRequirement = ItemList.ClassRequirement.None;
                item.implicits = implicits;
                item.subClassRequirement = ItemList.SubClassRequirement.None;
                item.cannotDrop = true;
                item.itemTags = ItemLocationTag.None;
                item.levelRequirement = 40;
                item.name = "Leather Belt";
                item.subTypeID = base_id;

                return item;
            }
        }
        public class Unique
        {
            public static string unique_name = "Headhunter";
            public static ushort unique_id = 513;
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
                    legendaryType = UniqueList.LegendaryType.LegendaryPotential,
                    overrideEffectiveLevelForLegendaryPotential = true,
                    effectiveLevelForLegendaryPotential = 0,
                    canDropRandomly = true,
                    rerollChance = 1,
                    itemModelType = UniqueList.ItemModelType.Unique,
                    subTypeForIM = 0,
                    baseType = 2, //Belt
                    subTypes = SubType(),
                    mods = Mods(),
                    tooltipDescriptions =TooltipDescription(),
                    loreText = "A man's soul rules from a cavern of bone, learns and judges through flesh-born windows. " +
                    "The heart is meat. The head is where the Man is. - Lavianga, Advisor to Kaom",
                    tooltipEntries = TooltipEntries(),
                    oldSubTypeID = 0,
                    oldUniqueID = 0
                };

                return item;
            }
            public static Sprite Icon()
            {
                Sprite sprite = null;
                try
                {
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    Properties.Resources.Headhunter.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    Texture2D icon = new Texture2D(1, 1);
                    ImageConversion.LoadImage(icon, stream.ToArray(), true);
                    sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);
                }
                catch { }

                return sprite;
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

                return result;
            }
            private static Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> TooltipEntries()
            {
                Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> result = new Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry>();
                result.Add(new UniqueModDisplayListEntry(0));
                result.Add(new UniqueModDisplayListEntry(1));
                result.Add(new UniqueModDisplayListEntry(2));
                result.Add(new UniqueModDisplayListEntry(128));

                return result;
            }
            private static Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> TooltipDescription()
            {
                Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> result = new Il2CppSystem.Collections.Generic.List<ItemTooltipDescription>();
                result.Add(new ItemTooltipDescription
                {
                    description = "When you Kill a monster, you gain " + RandomBuffs.Min_Generated + " to " + RandomBuffs.Max_Generated + " random Modifier(s) stackable for " + RandomBuffs.BuffDuration + " seconds"
                });

                return result;
            }
            
        }        
        public class RandomBuffs
        {
            public static int Min_Generated = 1;
            public static int Max_Generated = 5;
            public const float BuffDuration = 20f;
            public static bool Enable_RandomBuffs = true;
            public static void GenerateBuffs()
            {
                int NbBuff = Random.Range(Min_Generated, Max_Generated);
                for (int i = 0; i < NbBuff; i++)
                {
                    Actor playerActor = PlayerFinder.getPlayerActor();
                    Buff random_buff = null;
                    for (int k = 0; k < 99; k++)
                    {
                        Buff temp_buff = Generate_RandomBuff();
                        if ((!temp_buff.name.Contains("Negative")) &&
                            (!temp_buff.name.Contains("PlayerProperty")) &&
                            (!temp_buff.name.Contains("AbilityProperty")) &&
                            (!temp_buff.name.Contains("ChanceToBe")) &&
                            (!temp_buff.name.Contains("DamageTaken")) &&
                            (!temp_buff.name.Contains("Drain")) &&
                            (!temp_buff.name.Contains("None")) &&
                            (!temp_buff.name.Contains("Cost")) &&
                            (!temp_buff.name.Contains("Received")) &&
                            (!temp_buff.name.Contains("Potion")) &&
                            (!temp_buff.name.Contains("PerceivedUnimportanceModifier")) &&
                            (!temp_buff.name.Contains("ManaBeforeHealth")) &&
                            (!temp_buff.name.Contains("Level")) &&
                            (!temp_buff.name.Contains("Penetration")) &&
                            (!temp_buff.name.Contains("ManaEfficiency")) &&
                            (!temp_buff.name.Contains("MaximumHealth")) &&
                            (!temp_buff.name.Contains("Skill")))
                        {
                            random_buff = temp_buff;
                            break;
                        }
                    }
                    if (random_buff != null)
                    {
                        Buff remove_buff = null;
                        bool update = false;
                        foreach (Buff player_buff in playerActor.statBuffs.buffs)
                        {
                            if (random_buff.name == player_buff.name)
                            {
                                float new_value = 0;
                                if (player_buff.name.Contains("Add "))
                                {
                                    new_value = player_buff.stat.addedValue;
                                    if (new_value < (float.MaxValue - 1)) { new_value++; }
                                    else { new_value = float.MaxValue; }
                                    random_buff.stat.addedValue = new_value;
                                }
                                else if (player_buff.name.Contains("Increase "))
                                {
                                    new_value = player_buff.stat.increasedValue;
                                    if (new_value < (float.MaxValue - 1)) { new_value++; }
                                    else { new_value = float.MaxValue; }
                                    random_buff.stat.increasedValue = new_value;
                                }
                                remove_buff = player_buff;
                                update = true;
                                break;
                            }
                        }
                        if (update) { playerActor.statBuffs.removeBuff(remove_buff); }
                        float value = 0;
                        if (random_buff.stat.increasedValue == 0) { value = random_buff.stat.addedValue; }
                        else { value = random_buff.stat.increasedValue; }
                        Main.logger_instance.Msg("Add Buff : " + random_buff.name + ", Value = " + value);
                        playerActor.statBuffs.addBuff(random_buff.remainingDuration, random_buff.stat.property,
                            random_buff.stat.addedValue, random_buff.stat.increasedValue, random_buff.stat.moreValues,
                            random_buff.stat.tags, random_buff.stat.specialTag, random_buff.name);
                    }
                }
            }

            private static Buff Generate_RandomBuff()
            {
                float addedValue = 0;
                float increasedValue = 0;
                Il2CppSystem.Collections.Generic.List<float> moreValues = null;
                int increase = Random.Range(0, 3);
                string name = "Add ";
                if ((increase == 2) || (increase == 3))
                {
                    name = "Increase ";
                    increasedValue = 1;
                }
                else { addedValue = 1; }
                SP property = (SP)Random.Range(0, System.Enum.GetValues(typeof(SP)).Length);
                name += property.ToString();
                byte specialTag = 0;
                AT tags = AT.None;

                Buff result = new Buff
                {
                    name = name,
                    remainingDuration = BuffDuration,
                    stat = new Stats.Stat
                    {
                        addedValue = addedValue,
                        increasedValue = increasedValue,
                        moreValues = moreValues,
                        property = property,
                        specialTag = specialTag,
                        tags = tags
                    }
                };

                return result;
            }
        }        
        public class Steal
        {
            public static bool Enable_Steal = false;
            public static void StealBuffFromMobs(Actor killedActor)
            {
                if ((killedActor.tag == "Enemy") && (killedActor.rarity == Actor.Rarity.Rare) && (!killedActor.data.isBossOrMiniBoss()))
                {
                    Actor playerActor = PlayerFinder.getPlayerActor();
                    foreach (Buff buff in killedActor.statBuffs.buffs)
                    {
                        Buff b = buff;
                        b.remainingDuration = 20;
                        playerActor.statBuffs.addBuff(b);

                        string name = b.name;
                        string property = b.stat.property.ToString();
                        Main.logger_instance.Msg("Steal Buff : Name = " + name + ", Property = " + property);
                    }
                }
            }
        }
        public class Events
        {
            public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (Scenes.GameScene())
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
                        if (Steal.Enable_Steal) { Steal.StealBuffFromMobs(killedActor); }
                        if (RandomBuffs.Enable_RandomBuffs) { RandomBuffs.GenerateBuffs(); }
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
                        if (Steal.Enable_Steal) { Steal.StealBuffFromMobs(killedActor); }
                        if (RandomBuffs.Enable_RandomBuffs) { RandomBuffs.GenerateBuffs(); }
                    }
                }
            }
        }
        public class Refs
        {
            public static bool Tracker_Initialized = false;
            public static bool BaseItemAdd_Initialized = false;
            public static bool UniqueItemAdd_Initialized = false;
            public static bool SceneLoaded_Initialized = false;
        }
        public class Ui
        {
            private static Texture2D windowBackground = null;
            public static void InitTexture()
            {
                if (windowBackground == null) { windowBackground = Functions.MakeTextureFromColor(2, 2, Color.black); }

            }
            public static void Update()
            {
                if (PlayerFinder.getPlayerActor().itemContainersManager.hasUniqueEquipped(Unique.unique_id))
                {
                    float size_w = 200;
                    float size_h = 200;
                    float start_x = 0;
                    float start_y = (Screen.width / 2) - (size_h / 2);
                    float pos_x = start_x;
                    float pos_y = start_y;
                    GUI.DrawTexture(new Rect(pos_x, pos_y, size_w, size_h), windowBackground);
                    pos_x += 5;
                    pos_y += 5;

                }
            }
        }
    }
    public class HHTracker : MonoBehaviour
    {
        public HHTracker(System.IntPtr ptr) : base(ptr) { }
    }
}
