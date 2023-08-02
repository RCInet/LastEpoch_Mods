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
        private static ushort unique_id = 513;
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
                legendaryType = UniqueList.LegendaryType.WeaversWill,
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
                description = "When you Kill a Rare monster, you gain its Modifiers for 20 seconds"
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
        private static bool Enable_RandomBuffs = true;
        private static void GenerateBuffs(Actor playerActor)
        {
            int NbBuff = Random.Range(0, 10);
            for (int i = 0; i < NbBuff; i++)
            {
                Buff b = Generate_RandomBuff();
                bool update = false;
                foreach (Buff buff in playerActor.statBuffs.buffs)
                {
                    if (buff.name == b.name)
                    {
                        update = true;
                        string msg = "";
                        if (buff.name.Contains("Increase"))
                        {
                            float value = buff.stat.increasedValue;
                            if ((value + 1) > 255) { value = 255; }
                            else { value = value + 1; }
                            buff.stat.increasedValue = value;
                            msg = "Increase Value to ";
                        }
                        else
                        {
                            float value = buff.stat.addedValue;
                            if ((value + 1) > 255) { value = 255; }
                            else { value = value + 1; }
                            buff.stat.addedValue = value;
                            msg = "Added Value to ";
                        }
                        buff.remainingDuration = BuffDuration;
                        msg += buff.stat.property.ToString() + " Buff";

                        break;
                    }
                }
                if (!update)
                {
                    Main.logger_instance.Msg("Add Buff : " + b.name);
                    playerActor.statBuffs.addBuff(b);
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
        private static bool Enable_Steal = true;
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
                    Main.logger_instance.Msg("Add Buff : Name = " + name + ", Property = " + property);
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
        //Minions Events
        /*private static readonly System.Action<Summoned, Ability, Actor> OnMinionKillAction = new System.Action<Summoned, Ability, Actor>(OnMinionKill);
        private static void OnMinionKill(Summoned summmoned, Ability ability, Actor killedActor)
        {

        }*/
        #endregion
    }
    public class HHTracker : MonoBehaviour
    {
        public HHTracker(System.IntPtr ptr) : base(ptr) { }
    }
}
