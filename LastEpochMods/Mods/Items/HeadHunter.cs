using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpochMods.Mods.Items
{
    public class HeadHunter
    {
        public static bool Initialized = false;
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
            if (!BaseItemAdd_Initialized)
            {
                try
                {
                    ItemList.instance.EquippableItems[2].subItems.Add(BaseItem());
                    BaseItemAdd_Initialized = true;
                }
                catch { }
            }
            if (!UniqueItemAdd_Initialized)
            {
                try
                {
                    UniqueList.instance.uniques.Add(UniqueItem());
                    UniqueItemAdd_Initialized = true;
                }
                catch { }
            }
            if (!SceneLoaded_Initialized)
            {
                try
                {
                    SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
                    SceneLoaded_Initialized = true;
                }
                catch { }
            }
            if ((Tracker_Initialized) && (BaseItemAdd_Initialized) && (UniqueItemAdd_Initialized) && (SceneLoaded_Initialized))
            {
                Initialized = true;
            }
        }

        #region Initialize
        private static bool Tracker_Initialized = false;
        private static bool BaseItemAdd_Initialized = false;
        private static bool UniqueItemAdd_Initialized = false;
        private static bool SceneLoaded_Initialized = false;        
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Scenes.GameScene())
            {
                if (!PlayerFinder.getPlayer().GetComponent<HHTracker>())
                {
                    PlayerFinder.getPlayer().GetComponent<AbilityEventListener>().add_onKillEvent(OnKillAction);
                    //PlayerFinder.getPlayer().GetComponent<SummonTracker>().OnMinionKill(OnMinionKillAction);
                    PlayerFinder.getPlayer().AddComponent<HHTracker>();
                }
            }
        }
        #endregion
        #region Base_Item
        private static int base_id = 99;
        private static ItemList.EquipmentItem BaseItem()
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
        #endregion
        #region Unique_Item
        private static System.Drawing.Bitmap icon = Properties.Resources.Headhunter;
        public const float BuffDuration = 20f;
        public static string unique_name = "Headhunter";
        public static ushort unique_id = 513;
        private static UniqueList.Entry UniqueItem()
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
                subTypes = Get_SubType(),
                mods = Get_Mods(),
                tooltipDescriptions = Get_TooltipDescription(),
                loreText = "A man's soul rules from a cavern of bone, learns and judges through flesh-born windows. " +
                "The heart is meat. The head is where the Man is. - Lavianga, Advisor to Kaom",
                tooltipEntries = Get_TooltipEntries(),
                oldSubTypeID = 0,
                oldUniqueID = 0
            };

            return item;
        }
        private static Il2CppSystem.Collections.Generic.List<byte> Get_SubType()
        {
            Il2CppSystem.Collections.Generic.List<byte> result = new Il2CppSystem.Collections.Generic.List<byte>();
            byte r = (byte)base_id;
            result.Add(r);

            return result;
        }
        private static Il2CppSystem.Collections.Generic.List<UniqueItemMod> Get_Mods()
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
        private static Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> Get_TooltipEntries()
        {
            Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry> result = new Il2CppSystem.Collections.Generic.List<UniqueModDisplayListEntry>();
            result.Add(new UniqueModDisplayListEntry(0));
            result.Add(new UniqueModDisplayListEntry(1));
            result.Add(new UniqueModDisplayListEntry(2));
            result.Add(new UniqueModDisplayListEntry(128));

            return result;
        }
        private static Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> Get_TooltipDescription()
        {
            Il2CppSystem.Collections.Generic.List<ItemTooltipDescription> result = new Il2CppSystem.Collections.Generic.List<ItemTooltipDescription>();
            result.Add(new ItemTooltipDescription
            {
                description = "When you Kill a monster, you gain " + Min_Generated + " to " + Max_Generated + " random Modifier(s) stackable for 20 seconds"
            });

            return result;
        }
        public static Sprite UniqueSprite()
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
        #endregion
        #region Buffs
        public static int Min_Generated = 1;
        public static int Max_Generated = 5;
        private static bool Enable_RandomBuffs = true;
        private static void GenerateBuffs(Actor playerActor)
        {
            int NbBuff = Random.Range(Min_Generated, Max_Generated);
            for (int i = 0; i < NbBuff; i++)
            {
                Buff b = Generate_RandomBuff();
                if (playerActor == null) { Main.logger_instance.Msg("playerActor is null"); }
                else
                {
                    if ((playerActor.statBuffs.buffs.Count == 0) || (playerActor.statBuffs.buffs == null))
                    {
                        playerActor.statBuffs.buffs = new Il2CppSystem.Collections.Generic.List<Buff>();
                    }
                    bool already_buff = false;
                    int z = 0;
                    string name = "";
                    foreach (Buff player_buff in playerActor.statBuffs.buffs)
                    {
                        if (player_buff.name == b.name)
                        {                            
                            bool error = false;
                            if (playerActor.statBuffs.buffs[z].name.Contains("Add "))
                            {
                                name = player_buff.name.Substring(4, player_buff.name.Length - 4);
                                float new_value = playerActor.statBuffs.buffs[z].stat.addedValue;
                                if (new_value < 254)
                                {
                                    new_value++;
                                    //Main.logger_instance.Msg("Buff : + " + new_value + " " + name);
                                    playerActor.statBuffs.buffs[z].stat.addedValue = new_value;
                                }
                                else
                                {
                                    //Main.logger_instance.Msg(player_buff.name + " Max Value");
                                    playerActor.statBuffs.buffs[z].stat.addedValue = float.MaxValue;
                                }                                
                            }
                            else if (playerActor.statBuffs.buffs[z].name.Contains("Increase "))
                            {
                                name = player_buff.name.Substring(9, player_buff.name.Length - 9);
                                float new_value = playerActor.statBuffs.buffs[z].stat.increasedValue;
                                if (new_value < 254)
                                {
                                    new_value++;
                                    int percent = (int)(new_value / 255) * 100;
                                    //Main.logger_instance.Msg("Increase Buff : + " + percent + " % " + name);
                                    playerActor.statBuffs.buffs[z].stat.increasedValue = new_value;                                    
                                }
                                else
                                {
                                    //Main.logger_instance.Msg(player_buff.name + " Max Value");
                                    playerActor.statBuffs.buffs[z].stat.increasedValue = float.MaxValue;                            
                                }
                            }
                            else
                            {
                                //Main.logger_instance.Msg("Not a Headhunter Buff");
                                error = true;
                            }
                            if (!error)
                            {
                                //Main.logger_instance.Msg("Reset " + name + " Cooldown");
                                playerActor.statBuffs.buffs[z].remainingDuration = BuffDuration;
                                already_buff = true;                                
                            }
                            break;
                        }
                        z++;
                    }
                    if (!already_buff)
                    {
                        //Main.logger_instance.Msg("Add Buff : " + b.name);
                        playerActor.statBuffs.buffs.Add(b);
                        playerActor.statBuffs.activeBuffNames.Add(b.name, b);
                    }                    
                }
            }
        }
        private static Buff Generate_RandomBuff()
        {
            float addedValue = 0;
            float increasedValue = 0;
            Il2CppSystem.Collections.Generic.List<float> moreValues = null;
            int increase = Random.Range(0, 1);
            string name = "Add ";
            if (increase == 1)
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
        #endregion
        #region Steal
        private static bool Enable_Steal = false;
        private static void StealBuffFromMobs(Actor playerActor, Actor killedActor)
        {
            if ((killedActor.tag == "Enemy") && (killedActor.rarity == Actor.Rarity.Rare) && (!killedActor.data.isBossOrMiniBoss()))
            {

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
        #endregion
        #region Events      
        private static readonly System.Action<Ability, Actor> OnKillAction = new System.Action<Ability, Actor>(OnKill);
        private static void OnKill(Ability ability, Actor killedActor)
        {
            Actor playerActor = null;
            try { playerActor = PlayerFinder.getPlayerActor(); }
            catch { }
            if (playerActor != null)
            {
                if (playerActor.itemContainersManager.hasUniqueEquipped(unique_id))
                {
                    if (Enable_Steal) { StealBuffFromMobs(playerActor, killedActor); }
                    if (Enable_RandomBuffs) { GenerateBuffs(playerActor); }
                }
            }
        }
        #endregion
    }
    public class HHTracker : MonoBehaviour
    {
        public HHTracker(System.IntPtr ptr) : base(ptr) { }
    }
}
