//Mod idea From https://github.com/exiledagain
//Blessing (need to unlock timeline first) Should be fixed soon

using Il2Cpp;
using MelonLoader;
using Newtonsoft.Json;
using System.Net.Http;
using UnityEngine;
using static LastEpoch_Hud.Scripts.Mods.Maxroll.Maxroll_import.Data;
using static LastEpoch_Hud.Scripts.Mods.Maxroll.Maxroll_import.Data.Json;

namespace LastEpoch_Hud.Scripts.Mods.Maxroll
{
    [RegisterTypeInIl2Cpp]
    public class Maxroll_import : MonoBehaviour
    {
        public Maxroll_import(System.IntPtr ptr) : base(ptr) { }
        public static Maxroll_import instance { get; private set; }

        public static ItemDataUnpacked item_data = null;

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if ((Data.loaded) && ((Data.root.IsNullOrDestroyed()) || (Data.data.IsNullOrDestroyed())))
            {
                Data.loaded = false;
                if ((Hud_Manager.Content.Maxroll.show) && (!Data.updating))
                {
                    Data.updating = true;
                    Hud_Manager.Content.Maxroll.Hide();
                    Data.updating = false;
                }
            }
            if ((Data.loaded) && (!Data.updating) && (!Hud_Manager.Content.Maxroll.show) && (!Data.root.IsNullOrDestroyed()) &&
                (!Data.data.IsNullOrDestroyed()) && (Data.selected_profile > -1)) { Data.Update(); }
        }

        

        public class Functions
        {
            public static ItemData MakeItem(int item_type, int sub_type, System.Collections.Generic.List<double> implicits, System.Collections.Generic.List<Data.Json.Affix> affixs,
                Data.Json.Affix sealed_affix, Data.Json.Affix primordial_affix, int? unique_id, System.Collections.Generic.List<double> unique_rolls, bool corrupted, System.Collections.Generic.List<Data.Json.Affix> corruptedAffixes)
            {
                if (implicits.IsNullOrDestroyed()) { implicits = new System.Collections.Generic.List<double>(); }
                if (affixs.IsNullOrDestroyed()) { affixs = new System.Collections.Generic.List<Data.Json.Affix>(); }
                if (sealed_affix.IsNullOrDestroyed()) { sealed_affix = new Data.Json.Affix { Id = -1, Tier = -1, Roll = -1 }; }
                if (primordial_affix.IsNullOrDestroyed()) { primordial_affix = new Data.Json.Affix { Id = -1, Tier = -1, Roll = -1 }; }
                if (unique_rolls.IsNullOrDestroyed()) { unique_rolls = new System.Collections.Generic.List<double> { -1, -1, -1, -1, -1, -1, -1, -1 }; }

                Il2CppSystem.Collections.Generic.List<ItemAffix> item_affixes = new Il2CppSystem.Collections.Generic.List<ItemAffix>();
                foreach (Data.Json.Affix affix in affixs)
                {
                    item_affixes.Add(new ItemAffix { affixId = (ushort)affix.Id, affixTier = (byte)(affix.Tier - 1), affixRoll = (byte)(affix.Roll * 255), sealedAffixType = SealedAffixType.None });
                }
                bool HasSeal = false;
                if ((sealed_affix.Id > -1) && (sealed_affix.Tier > -1) && (sealed_affix.Roll > -1))
                {
                    item_affixes.Add(new ItemAffix { affixId = (ushort)sealed_affix.Id, affixTier = (byte)(sealed_affix.Tier - 1), affixRoll = (byte)(sealed_affix.Roll * 255), sealedAffixType = SealedAffixType.Regular });
                    HasSeal = true;
                }
                bool HasPrimo = false;
                if ((primordial_affix.Id > -1) && (primordial_affix.Tier > -1) && (primordial_affix.Roll > -1))
                {
                    item_affixes.Add(new ItemAffix { affixId = (ushort)primordial_affix.Id, affixTier = (byte)(primordial_affix.Tier - 1), affixRoll = (byte)(primordial_affix.Roll * 255), sealedAffixType = SealedAffixType.Primordial });
                    HasPrimo = true;
                }
                bool HasCorruptedAffix = false;
                if (corruptedAffixes != null && corruptedAffixes.Count > 0 && corruptedAffixes[0].Id > -1 && corruptedAffixes[0].Tier > -1 && corruptedAffixes[0].Roll > -1) {
                    foreach (Data.Json.Affix affix in corruptedAffixes)
                    {
                        item_affixes.Add(new ItemAffix { affixId = (ushort)affix.Id, affixTier = (byte)(affix.Tier - 1), affixRoll = (byte)(affix.Roll * 255), sealedAffixType = SealedAffixType.FromCorruption });
                    }
                    HasCorruptedAffix = true;
                }
                byte item_rarity = (byte)item_affixes.Count;
                byte lp = 0; //Legendary potencial
                byte ww = 0; //Weaver will
                UniqueList.LegendaryType item_legendary_type = UniqueList.LegendaryType.LegendaryPotential;
                if (unique_id != null)
                {
                    UniqueList.Entry unique_item = UniqueList.getUnique((ushort)unique_id);
                    if (!unique_item.IsNullOrDestroyed())
                    {
                        if (unique_item.isSetItem) 
                        { 
                            // Set Item
                            item_rarity = (byte)(8); 
                        }
                        else if (item_affixes.Count > 0) {
                            // Legendary Item
                            item_rarity = (byte)(9); 
                        }
                        else {
                            // Unique Item, roll LP or WW
                            item_rarity = (byte)(7);
                            if (unique_item.legendaryType == UniqueList.LegendaryType.LegendaryPotential) { lp = (byte)Random.RandomRange(0f, 4f); }
                            else
                            {
                                item_legendary_type = UniqueList.LegendaryType.WeaversWill;
                                ww = (byte)Random.RandomRange(0f, 28f);
                            }
                        }
                    }
                }
                ItemDataUnpacked item = new ItemDataUnpacked { itemType = (byte)item_type, subType = (ushort)sub_type, rarity = item_rarity, affixes = item_affixes, hasSealedPrimordialAffix = HasPrimo, hasSealedRegularAffix = HasSeal, corrupted = corrupted, hasSealedAffixFromCorruption = HasCorruptedAffix};
                int i = 0;
                foreach (double implicit_value in implicits)
                {
                    if (i < item.implicitRolls.Count) { item.implicitRolls[i] = (byte)(implicit_value * 255); }
                    else { break; }
                    i++;
                }
                if (unique_id != null)
                {
                    item.uniqueID = (ushort)unique_id;
                    if (item_legendary_type == UniqueList.LegendaryType.LegendaryPotential) { item.legendaryPotential = lp; }
                    else { item.weaversWill = ww; }
                    i = 0;
                    foreach (double unique_roll_value in unique_rolls)
                    {
                        if ((i < item.uniqueRolls.Count) && (unique_roll_value > -1)) { item.uniqueRolls[i] = (byte)(unique_roll_value * 255); }
                        else { break; }
                        i++;
                    }
                }
                item.RefreshIDAndValues();

                return item.TryCast<ItemData>();
            }
            public static void DropItem(ItemData item)
            {
                if ((!Refs_Manager.ground_item_manager.IsNullOrDestroyed()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()) && (!item.IsNullOrDestroyed()))
                {
                    Refs_Manager.ground_item_manager.dropItemForPlayer(Refs_Manager.player_actor, item, Refs_Manager.player_actor.position(), false);
                }
            }
            public static void SetItem(ushort container_id, ItemData item)
            {
                if (!Refs_Manager.player_data_tracker.IsNullOrDestroyed())
                {
                    bool found = false;
                    foreach (Il2CppLE.Data.ItemLocationPair item_pair in Refs_Manager.player_data_tracker.charData.SavedItems)
                    {
                        if (item_pair.ContainerID == container_id)
                        {
                            if (item_pair.Data.Count > 7)
                            {
                                if (item_pair.Data[1] == 34)
                                {
                                    item_pair.Data[2] = (byte)item.subType; //(byte)sub_type;
                                    item_pair.Data[5] = item.implicitRolls[0];
                                    item_pair.Data[6] = item.implicitRolls[1];
                                    item_pair.Data[7] = item.implicitRolls[2];
                                    found = true;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    if (!found)
                    {
                        byte format_version = 3; //patch

                        Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<byte> Data = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<byte>(11);
                        Data[0] = format_version;
                        Data[1] = item.itemType;
                        Data[2] = (byte)item.subType;
                        Data[3] = 0;
                        Data[4] = 0;
                        Data[5] = item.implicitRolls[0];
                        Data[6] = item.implicitRolls[1];
                        Data[7] = item.implicitRolls[2];
                        Data[8] = 0;
                        Data[9] = 0;
                        Data[10] = 0;

                        Il2CppLE.Data.ItemLocationPair new_blessing = new Il2CppLE.Data.ItemLocationPair
                        {
                            ContainerID = container_id,
                            Data = Data,
                            FormatVersion = format_version,
                            InventoryPosition = new Il2CppLE.Data.ItemInventoryPosition(0, 0),
                            Quantity = 1,
                            TabID = 0
                        };

                        Refs_Manager.player_data_tracker.charData.SavedItems.Add(new_blessing);
                    }
                    Refs_Manager.player_data_tracker.charData.SaveData();
                }
            }
            public static async System.Threading.Tasks.Task SetBlessings(System.Collections.Generic.List<Blessings_structure> blessings)
            {
                Main.logger_instance.Msg("SetBlessings();");
                if ((!Refs_Manager.player_data_tracker.IsNullOrDestroyed()) && (!Refs_Manager.game_uibase.IsNullOrDestroyed()))
                {
                    Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                    while (Hud_Manager.IsPauseOpen()) { await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(1)); }
                    Main.logger_instance.Msg("Hud Closed");

                    System.Collections.Generic.List<TimelineID> timelines_id = new System.Collections.Generic.List<TimelineID>();
                    timelines_id.Add(TimelineID.UndeadAbom);
                    timelines_id.Add(TimelineID.OsprixWithLance);
                    timelines_id.Add(TimelineID.VoidRahyeh);
                    timelines_id.Add(TimelineID.FrostLich);
                    timelines_id.Add(TimelineID.Lagon);
                    timelines_id.Add(TimelineID.UndeadVsVoid);
                    timelines_id.Add(TimelineID.Dragons);
                    timelines_id.Add(TimelineID.Gaspar);
                    timelines_id.Add(TimelineID.Heorot);
                    timelines_id.Add(TimelineID.Volcano);

                    foreach (Blessings_structure b in blessings)
                    {
                        if (b.timeline < timelines_id.Count)
                        {
                            TimelineID selected_timeline = timelines_id[b.timeline];
                            Main.logger_instance.Msg("timeline = " + selected_timeline.ToString());
                            //BlessingRewardPanelManager.onOptionsPopulated(selected_timeline, 0, 3);


                            //if (!BlessingRewardPanelManager.instance.IsNullOrDestroyed()) { Object.Destroy(BlessingRewardPanelManager.instance); }
                            if ((!BlessingRewardPanelManager.instance.IsNullOrDestroyed()))// && (!Refs_Manager.game_uibase.chooseBlessingPanel.IsNullOrDestroyed()))
                            {
                                //Main.logger_instance.Msg("populateBlessingOptions");
                                //ItemContainersManager.Instance.populateBlessingOptions(selected_timeline, 1);

                                Main.logger_instance.Msg("Set option");
                                OneItemContainer container = BlessingRewardPanelManager.instance.blessingOptions[0].Container.TryCast<OneItemContainer>();
                                if (!container.IsNullOrDestroyed())
                                {
                                    container.content = new ItemContainerEntry(b.data, new Vector2Int(0, 0), new Vector2Int(1, 1));

                                    //GameObject ui = BlessingRewardPanelManager.instance.gameObject;
                                    //ui.active = true;
                                    Refs_Manager.game_uibase.OpenBlessingOptions(selected_timeline, 0, 3);
                                    //Refs_Manager.game_uibase.chooseBlessingPanel.Open();
                                    //while (!Refs_Manager.game_uibase.chooseBlessingPanel.isOpen) { await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(1)); } //Wait reward panel Open

                                    Main.logger_instance.Msg("Reward panel Open");

                                    BlessingRewardPanelManager.instance._selectedOption = 0;
                                    BlessingRewardPanelManager.instance.ReplaceButton();
                                    BlessingRewardPanelManager.instance.ConfirmSelection();
                                    //ui.active = false;

                                    //while (Refs_Manager.game_uibase.chooseBlessingPanel.isOpen) { await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(1)); } //Wait reward panel Close
                                    Refs_Manager.game_uibase.CloseBlessingOptions();

                                    Main.logger_instance.Msg("Reward panel Closed");
                                }
                            }
                        }
                    }
                }
            }
            public static int ObjectToInt(object obj)
            {
                int result = -1;
                if (!obj.IsNullOrDestroyed())
                {
                    string s = obj.ToString();
                    try { result = System.Convert.ToInt32(s); }
                    catch { }
                }

                return result;
            }
        }        
        public class Data
        {
            public static bool updating = false;
            public static int selected_profile = -1;
            public static bool loaded = false;
            public static Json.Root root = null;
            public static Json.Data data = null;
            public static System.Collections.Generic.List<string> profile_names = new System.Collections.Generic.List<string>();
            public static string build_name = "";
            public static string autor_name = "";
            public static bool youtube = false;
            public static string youtube_url = "";
            public static bool twitch = false;
            public static string twitch_url = "";
            public static string class_name = "";
            public static int character_level = 0;
            public static int nb_items = 0;
            public static int nb_idols = 0;
            public static int nb_blessings = 0;
            public static int nb_passives = 0;
            public static int nb_weavertree = 0;
            public static string mainskill_name = "";
            public static Sprite mainskill_icon = null;
            public static string[] specialized_names = { "", "", "", "", "" };
            public static string[] specialized_ids = { "", "", "", "", "" };
            public static Ability[] specialized_ability = { null, null, null, null, null };
            public static Sprite[] specialized_icons = { null, null, null, null, null };
            public static string[] active_names = { "", "", "", "", "" };
            public static Ability[] active_ability = { null, null, null, null, null };
            public static Sprite[] active_icons = { null, null, null, null, null };

            public static async System.Threading.Tasks.Task Load(string url)
            {
                loaded = false;
                if (url.Contains("https://maxroll.gg/last-epoch/planner/"))
                {
                    HttpResponseMessage response = await new HttpClient().GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    string[] p = url.Split('/');
                    string profile = p[p.Length - 1];
                    if (profile.Contains("#"))
                    {
                        selected_profile = System.Convert.ToInt32(profile.Split('#')[1]) - 1;
                        string p2 = profile.Split('#')[0];
                        profile = p2;
                    }
                    else { Data.selected_profile = 0; }
                    string json = "";
                    string[] r = jsonResponse.Split('>');
                    foreach (string s in r)
                    {
                        if (s.Contains(profile))
                        {
                            string s2 = s.Split('<')[0];
                            string remove = "window.__remixContext = ";
                            json = s2.Substring(remove.Length, s2.Length - remove.Length - 1);
                            break;
                        }
                    }
                    if (json != "")
                    {
                        root = JsonConvert.DeserializeObject<Json.Root>(json);
                        data = JsonConvert.DeserializeObject<Json.Data>(root.State.LoaderData.LastEpochPlannerById.Profile.Data);
                        loaded = true;
                    }
                }
                else { Main.logger_instance.Error("Not a Maxroll URL"); }
            }
            public static void Update()
            {
                updating = true;
                profile_names = Get.Profile_Names();
                build_name = Get.Build_Name();
                autor_name = Get.Autor_UserName();
                youtube = false;
                youtube_url = "";
                if (root.State.LoaderData.LastEpochPlannerById.Metadata.AuthorYoutube != "")
                {
                    youtube = true;
                    youtube_url = root.State.LoaderData.LastEpochPlannerById.Metadata.AuthorYoutube;
                }
                twitch = false;
                twitch_url = "";
                if (root.State.LoaderData.LastEpochPlannerById.Metadata.AuthorTwitch != "")
                {
                    twitch = true;
                    twitch_url = root.State.LoaderData.LastEpochPlannerById.Metadata.AuthorTwitch;
                }
                class_name = Get.Character_Class();
                character_level = Get.Character_Level();
                nb_items = Get.Nb_Items();
                nb_idols = Get.Nb_Idols();
                nb_blessings = Get.Nb_Blessings();
                nb_passives = Get.Nb_Passives();
                nb_weavertree = Get.Nb_WeaverTree();
                mainskill_name = root.State.LoaderData.LastEpochPlannerById.Profile.Mainset;
                foreach (Ability ability in Resources.FindObjectsOfTypeAll<Ability>())
                {
                    if (ability.name == mainskill_name)
                    {
                        mainskill_icon = ability.abilitySprite;
                        break;
                    }
                }
                string[] specialized_skills = { "", "", "", "", "" };
                int j = 0;
                foreach (string specialized_skill in data.Profiles[selected_profile].SpecializedSkills)
                {
                    if (j < specialized_skills.Length) { specialized_skills[j] = specialized_skill; }
                    j++;
                }
                specialized_names = new string[5] { "", "", "", "", "" };
                specialized_ids = new string[5] { "", "", "", "", "" };
                specialized_ability = new Ability[5] { null, null, null, null, null };
                specialized_icons = new Sprite[5] { null, null, null, null, null };
                j = 0;
                foreach (string specialized_skill in specialized_skills)
                {
                    foreach (Ability ability in Resources.FindObjectsOfTypeAll<Ability>())
                    {
                        if (ability.name == specialized_skill)
                        {
                            specialized_names[j] = ability.abilityName;
                            specialized_ids[j] = ability.playerAbilityID;
                            specialized_ability[j] = ability;
                            specialized_icons[j] = ability.abilitySprite;
                            break;
                        }
                    }
                    j++;
                }
                active_names = new string[5] { "", "", "", "", "" };
                active_ability = new Ability[5] { null, null, null, null, null };
                active_icons = new Sprite[5] { null, null, null, null, null };
                j = 0;
                foreach (string active_skill in data.Profiles[selected_profile].ActiveSkills)
                {
                    foreach (Ability ability in Resources.FindObjectsOfTypeAll<Ability>())
                    {
                        if (ability.name == active_skill)
                        {
                            active_names[j] = ability.abilityName;
                            active_ability[j] = ability;
                            active_icons[j] = ability.abilitySprite;
                            break;
                        }
                    }
                    j++;
                }
                Hud_Manager.Content.Maxroll.Show();
                updating = false;
            }
            
            public static void Load_AllEquipments()
            {
                Load_Equipments();
                Load_Idols();
                Load_Blessings();
            }
            public static void Load_Equipments()
            {
                if (selected_profile > -1)
                {
                    Json.Profile profile = data.Profiles[selected_profile];
                    System.Collections.Generic.List<int> items = new System.Collections.Generic.List<int>();
                    items.Add(Functions.ObjectToInt(profile.Items.Body));
                    items.Add(Functions.ObjectToInt(profile.Items.Feet));
                    items.Add(Functions.ObjectToInt(profile.Items.Finger1));
                    items.Add(Functions.ObjectToInt(profile.Items.Finger2));
                    items.Add(Functions.ObjectToInt(profile.Items.Hands));
                    items.Add(Functions.ObjectToInt(profile.Items.Head));
                    items.Add(Functions.ObjectToInt(profile.Items.Neck));
                    items.Add(Functions.ObjectToInt(profile.Items.Offhand));
                    items.Add(Functions.ObjectToInt(profile.Items.Relic));
                    items.Add(Functions.ObjectToInt(profile.Items.Waist));
                    items.Add(Functions.ObjectToInt(profile.Items.Weapon));
                    items.Add(Functions.ObjectToInt(profile.Items.Altar));
                    foreach (System.Collections.Generic.KeyValuePair<int, Json.Item> item in data.Items)
                    {
                        if (items.Contains(item.Key))
                        {
                            Functions.DropItem(Functions.MakeItem(item.Value.ItemType, item.Value.SubType, item.Value.Implicits, item.Value.Affixes, item.Value.SealedAffix, item.Value.PrimordialAffix, item.Value.UniqueID, item.Value.UniqueRolls, item.Value.Corrupted, item.Value.CorruptedAffixes));
                        }
                    }
                }
            }
            public static void Load_Idols()
            {
                if (selected_profile > -1)
                {
                    System.Collections.Generic.List<int> items = new System.Collections.Generic.List<int>();
                    foreach (object item in data.Profiles[Data.selected_profile].Idols) { items.Add(Functions.ObjectToInt(item)); }
                    foreach (System.Collections.Generic.KeyValuePair<int, Json.Item> item in data.Items)
                    {
                        if (items.Contains(item.Key))
                        {
                            Functions.DropItem(Functions.MakeItem(item.Value.ItemType, item.Value.SubType, item.Value.Implicits, item.Value.Affixes, item.Value.SealedAffix, item.Value.PrimordialAffix, item.Value.UniqueID, item.Value.UniqueRolls, item.Value.Corrupted, item.Value.CorruptedAffixes));
                        }
                    }
                }
            }
            public static async void Load_Blessings()
            {
                Main.logger_instance.Msg("Load_Blessings();" );
                if (selected_profile > -1)
                {
                    //Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                    //while (Hud_Manager.IsPauseOpen()) { await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(1)); } //Wait hud

                    //Main.logger_instance.Msg("Hud Closed");

                    //int blessing_container = 33;
                    int i = 0;
                    System.Collections.Generic.List<Blessings_structure> blessings = new System.Collections.Generic.List<Blessings_structure>();
                    foreach (Json.Blessing item in data.Profiles[selected_profile].Blessings)
                    {
                        if (!item.IsNullOrDestroyed())
                        {
                            Main.logger_instance.Msg("i = " + i);
                            blessings.Add(new Blessings_structure
                            {
                                timeline = i,
                                data = Functions.MakeItem(item.ItemType, item.SubType, item.Implicits, null, null, null, null, null, false, null)
                            });
                        }
                        i++;
                    }
                    await Functions.SetBlessings(blessings);
                }
            }

            

            public static void Load_Passives()
            {
                if ((selected_profile > -1) && (!Refs_Manager.player_treedata.IsNullOrDestroyed()) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
                {
                    Json.Profile profile = data.Profiles[selected_profile];
                    if (Refs_Manager.player_data.CharacterClass == profile.Class)
                    {
                        Refs_Manager.player_data.ChosenMastery = (byte)profile.Mastery;
                        Refs_Manager.player_data.ClickedUnlockMasteriesButton = true;
                        Refs_Manager.player_treedata.chosenMastery = (byte)profile.Mastery;
                        Refs_Manager.player_treedata.passiveTree.nodes.Clear();
                        foreach (int node_id in profile.Passives.History)
                        {
                            bool found = false;
                            foreach (LocalTreeData.NodeData node_data in Refs_Manager.player_treedata.passiveTree.nodes)
                            {
                                if (node_data.id == node_id)
                                {
                                    node_data.pointsAllocated++;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found) { Refs_Manager.player_treedata.passiveTree.nodes.Add(new LocalTreeData.NodeData((byte)node_id, (byte)1)); }
                        }
                        Refs_Manager.player_treedata.updateMasteryTotals();
                        Refs_Manager.player_treedata.savePassiveTreeData();
                        Refs_Manager.player_data.SaveData();
                    }
                }
            }
            public static void Load_WeaverTree()
            {
                if ((selected_profile > -1) && (!Refs_Manager.player_treedata.IsNullOrDestroyed()))
                {
                    Json.Profile profile = data.Profiles[selected_profile];
                    Refs_Manager.player_treedata.weaverTree.nodes.Clear();
                    foreach (int node_id in profile.Weaver.History)
                    {
                        bool found = false;
                        foreach (LocalTreeData.NodeData node_data in Refs_Manager.player_treedata.weaverTree.nodes)
                        {
                            if (node_data.id == node_id)
                            {
                                node_data.pointsAllocated++;
                                found = true;
                                break;
                            }
                        }
                        if (!found) { Refs_Manager.player_treedata.weaverTree.nodes.Add(new LocalTreeData.NodeData((byte)node_id, (byte)1)); }
                    }
                    Refs_Manager.player_treedata.SaveWeaverTreeData();
                    Refs_Manager.player_data.SaveData();
                }
            }
            public static void Load_SkillTrees()
            {
                if (selected_profile > -1)
                {
                    for (int i = 0; i < 5; i++) { Load_SkillTree(i, i); }
                }
            }
            public static void Load_SkillTree(int index, int slot)
            {
                if (selected_profile > -1)
                {
                    Json.Profile profile = data.Profiles[selected_profile];
                    if (!Refs_Manager.player_treedata.IsNullOrDestroyed())
                    {
                        string skill_id = specialized_ids[index];
                        if (skill_id != "")
                        {
                            Json.NodePoint points = null;
                            foreach (System.Collections.Generic.KeyValuePair<string, Json.NodePoint> skill in profile.Skill)
                            {
                                if (skill.Key == skill_id)
                                {
                                    points = skill.Value;
                                    break;
                                }
                            }
                            Ability ability_to_remove = Refs_Manager.player_treedata.getSpecialisedAbilityInSlot((byte)slot);
                            if (!ability_to_remove.IsNullOrDestroyed()) { Refs_Manager.player_treedata.Despecialise(ability_to_remove, true); }
                            Refs_Manager.player_treedata.Specialise(Data.specialized_ability[index], (byte)slot, true);
                            foreach (LocalTreeData.TreeData tree_data in Refs_Manager.player_treedata.specialisedSkillTrees)
                            {
                                if (tree_data.treeID == skill_id)
                                {
                                    foreach (int node_id in points.History)
                                    {
                                        bool found = false;
                                        foreach (LocalTreeData.NodeData node_data in tree_data.nodes)
                                        {
                                            if (node_data.id == node_id)
                                            {
                                                node_data.pointsAllocated++;
                                                found = true;
                                                break;
                                            }
                                        }
                                        if (!found) { tree_data.nodes.Add(new LocalTreeData.NodeData((byte)node_id, (byte)1)); }
                                    }
                                    LocalTreeData.SkillTreeData skill_tree_data = tree_data.TryCast<LocalTreeData.SkillTreeData>();
                                    if (!skill_tree_data.IsNullOrDestroyed())
                                    {
                                        skill_tree_data.level = 20;
                                        skill_tree_data.abilityXp = 999999999;
                                        Refs_Manager.player_treedata.saveSpecialisedSkillData(skill_tree_data);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            public static void Load_ActiveSkills()
            {
                if ((selected_profile > -1) && (!Refs_Manager.player_treedata.IsNullOrDestroyed()))
                {
                    for (int i = 0; i < Refs_Manager.player_treedata.playerAbilityList.equippedAbilities.Count; i++)
                    {
                        if (i < active_ability.Length)
                        {
                            if (!active_ability[i].IsNullOrDestroyed())
                            {
                                if (active_ability[i] != Refs_Manager.player_treedata.playerAbilityList.equippedAbilities[i])
                                {
                                    Refs_Manager.player_treedata.playerAbilityList.setAbility(active_ability[i], i);
                                }
                            }
                        }
                    }
                }
            }

            public class Get
            {
                public static System.Collections.Generic.List<string> Profile_Names()
                {
                    System.Collections.Generic.List<string> profile_names = new System.Collections.Generic.List<string>();
                    foreach (Json.Profile profil in data.Profiles) { profile_names.Add(profil.Name); }

                    return profile_names;
                }
                public static string Build_Name()
                {
                    string result = "";
                    if (!root.IsNullOrDestroyed()) { result = root.State.LoaderData.LastEpochPlannerById.Profile.Name; }

                    return result;
                }
                public static string Autor_UserName()
                {
                    string result = "";
                    if (!root.IsNullOrDestroyed())
                    {
                        try
                        {
                            result = root.State.LoaderData.LastEpochPlannerById.Profile.User.Username;
                        }
                        catch { }
                    }

                    return result;
                }
                public static string Character_Class()
                {
                    string result = "";
                    if ((selected_profile > -1) && (!Refs_Manager.character_class_list.IsNullOrDestroyed()))
                    {
                        int i = 0;
                        foreach (CharacterClass char_class in Refs_Manager.character_class_list.classes)
                        {
                            if (i == data.Profiles[selected_profile].Class) { result = char_class.className; break; }
                            i++;
                        }
                    }

                    return result;
                }
                public static int Character_Level()
                {
                    int result = 0;
                    if (selected_profile > -1) { result = data.Profiles[selected_profile].Level; }

                    return result;
                }
                public static int Nb_Items()
                {
                    int result = 0;
                    if (selected_profile > -1)
                    {
                        Json.Profile profile = data.Profiles[selected_profile];
                        System.Collections.Generic.List<int> items = new System.Collections.Generic.List<int>();
                        items.Add(Functions.ObjectToInt(profile.Items.Body));
                        items.Add(Functions.ObjectToInt(profile.Items.Feet));
                        items.Add(Functions.ObjectToInt(profile.Items.Finger1));
                        items.Add(Functions.ObjectToInt(profile.Items.Finger2));
                        items.Add(Functions.ObjectToInt(profile.Items.Hands));
                        items.Add(Functions.ObjectToInt(profile.Items.Head));
                        items.Add(Functions.ObjectToInt(profile.Items.Neck));
                        items.Add(Functions.ObjectToInt(profile.Items.Offhand));
                        items.Add(Functions.ObjectToInt(profile.Items.Relic));
                        items.Add(Functions.ObjectToInt(profile.Items.Waist));
                        items.Add(Functions.ObjectToInt(profile.Items.Weapon));
                        items.Add(Functions.ObjectToInt(profile.Items.Altar));
                        foreach (int item in items) { if (item > -1) { result++; } }
                    }

                    return result;
                }
                public static int Nb_Idols()
                {
                    int result = 0;
                    if (selected_profile > -1)
                    {
                        foreach (object item in data.Profiles[selected_profile].Idols)
                        {
                            if (Functions.ObjectToInt(item) > -1) { result++; }
                        }
                    }

                    return result;
                }
                public static int Nb_Blessings()
                {
                    int result = 0;
                    if (selected_profile > -1)
                    {
                        foreach (Json.Blessing item in data.Profiles[selected_profile].Blessings)
                        {
                            if (!item.IsNullOrDestroyed()) { result++; }
                        }
                    }

                    return result;
                }
                public static int Nb_Passives()
                {
                    int result = 0;
                    if (selected_profile > -1) { result = data.Profiles[selected_profile].Passives.History.Count; }

                    return result;
                }
                public static int Nb_WeaverTree()
                {
                    int result = 0;
                    if (selected_profile > -1) { result = data.Profiles[selected_profile].Weaver.History.Count; }

                    return result;
                }
            }
            public class Json
            {
                public class Root
                {
                    [JsonProperty("state")]
                    public State State;
                }
                public class State
                {
                    [JsonProperty("loaderData")]
                    public LoaderData LoaderData;
                }
                public class LoaderData
                {
                    [JsonProperty("last-epoch-planner-by-id")]
                    public LastEpochPlannerById LastEpochPlannerById;
                }
                public class LastEpochPlannerById
                {
                    [JsonProperty("profile")]
                    public LastEpochPlannerProfile Profile;

                    [JsonProperty("metadata")]
                    public Metadata Metadata;
                }
                public class LastEpochPlannerProfile
                {
                    [JsonProperty("name")]
                    public string Name;

                    [JsonProperty("data")]
                    public string Data;

                    [JsonProperty("mainset")]
                    public string Mainset;

                    [JsonProperty("user")]
                    public User User;
                }
                public class Metadata
                {
                    [JsonProperty("authorYoutube")]
                    public string AuthorYoutube;

                    [JsonProperty("authorTwitch")]
                    public string AuthorTwitch;
                }
                public class Data
                {
                    [JsonProperty("profiles")]
                    public System.Collections.Generic.List<Profile> Profiles;

                    [JsonProperty("items")]
                    public System.Collections.Generic.Dictionary<int, Item> Items;

                    [JsonProperty("activeEmbed")]
                    public int ActiveEmbed;

                    [JsonProperty("activeProfile")]
                    public int ActiveProfile;
                }
                public class Profile
                {
                    [JsonProperty("name")]
                    public string Name;

                    [JsonProperty("class")]
                    public int Class;

                    [JsonProperty("mastery")]
                    public int Mastery;

                    [JsonProperty("level")]
                    public int Level;

                    [JsonProperty("items")]
                    public Items Items;

                    [JsonProperty("idols")]
                    public System.Collections.Generic.List<object> Idols;

                    [JsonProperty("blessings")]
                    public System.Collections.Generic.List<Blessing> Blessings;

                    [JsonProperty("passives")]
                    public NodePoint Passives;

                    [JsonProperty("weaver")]
                    public NodePoint Weaver;

                    //[JsonProperty("weaverItems")]
                    //public System.Collections.Generic.List<Items> WeaverItems;

                    [JsonProperty("skillTrees")]
                    public System.Collections.Generic.Dictionary<string, NodePoint> Skill;

                    [JsonProperty("activeSkills")]
                    public System.Collections.Generic.List<string> ActiveSkills;

                    [JsonProperty("specializedSkills")]
                    public System.Collections.Generic.List<string> SpecializedSkills;

                    [JsonProperty("quests")]
                    public System.Collections.Generic.List<int> Quests;

                    [JsonProperty("season")]
                    public int Season;
                }
                public class User
                {
                    [JsonProperty("username")]
                    public string Username;
                }
                public class Items
                {
                    [JsonProperty("body")]
                    public object Body;

                    [JsonProperty("neck")]
                    public object Neck;

                    [JsonProperty("offhand")]
                    public object Offhand;

                    [JsonProperty("finger1")]
                    public object Finger1;

                    [JsonProperty("feet")]
                    public object Feet;

                    [JsonProperty("hands")]
                    public object Hands;

                    [JsonProperty("waist")]
                    public object Waist;

                    [JsonProperty("head")]
                    public object Head;

                    [JsonProperty("relic")]
                    public object Relic;

                    [JsonProperty("finger2")]
                    public object Finger2;

                    [JsonProperty("weapon")]
                    public object Weapon;

                    [JsonProperty("altar")]
                    public object Altar;
                }
                public class Item
                {
                    [JsonProperty("itemType")]
                    public int ItemType;

                    [JsonProperty("subType")]
                    public int SubType;

                    [JsonProperty("uniqueID")]
                    public int? UniqueID;

                    [JsonProperty("uniqueRolls")]
                    public System.Collections.Generic.List<double> UniqueRolls;

                    [JsonProperty("affixes")]
                    public System.Collections.Generic.List<Affix> Affixes;

                    [JsonProperty("sealedAffix")]
                    public Affix SealedAffix;

                    [JsonProperty("primordialAffix")]
                    public Affix PrimordialAffix;

                    [JsonProperty("implicits")]
                    public System.Collections.Generic.List<double> Implicits;
                    
                    [JsonProperty("corruptedAffixes")]
                    public System.Collections.Generic.List<Affix> CorruptedAffixes;

                    [JsonProperty("corrupted")]
                    public bool Corrupted;
                }
                public class Idol
                {
                    [JsonProperty("itemType")]
                    public int ItemType;

                    [JsonProperty("subType")]
                    public int SubType;

                    [JsonProperty("affixes")]
                    public System.Collections.Generic.List<Affix> Affixes;

                    [JsonProperty("uniqueID")]
                    public int? UniqueID;

                    [JsonProperty("uniqueRolls")]
                    public System.Collections.Generic.List<double> UniqueRolls;
                }
                public class Blessing
                {
                    [JsonProperty("itemType")]
                    public int ItemType;

                    [JsonProperty("subType")]
                    public int SubType;

                    [JsonProperty("implicits")]
                    public System.Collections.Generic.List<double> Implicits;
                }
                public class Affix
                {
                    [JsonProperty("id")]
                    public int Id;

                    [JsonProperty("tier")]
                    public int Tier;

                    [JsonProperty("roll")]
                    public double Roll;
                }
                public class NodePoint
                {
                    [JsonProperty("history")]
                    public System.Collections.Generic.List<int> History;

                    [JsonProperty("position")]
                    public int Position;
                }
            }
            public struct Blessings_structure
            {
                public int timeline;
                public ItemData data;
            }
        }
        
    }
}
