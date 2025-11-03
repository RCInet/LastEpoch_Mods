//Mod From https://github.com/exiledagain

//At this time, can import AllEquipement, Equipement, Idols, Passives and WeaverTree
//Blessing (need to unlock timeline first) Should be fixed soon
//Skills (not writen at this time)

using Il2Cpp;
using MelonLoader;
using Newtonsoft.Json;
using System.Net.Http;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Maxroll
{
    [RegisterTypeInIl2Cpp]
    public class Maxroll_import : MonoBehaviour
    {
        public Maxroll_import(System.IntPtr ptr) : base(ptr) { }
        public static Maxroll_import instance { get; private set; }

        private static readonly int blessing_container = 33;
        public static Url.Root root = null; //Here for debug only
        public static Url.Data data = null; //Here for debug only
        public static string[] specialized_names = { "", "", "", "", "" };
        public static string[] specialized_ids = { "", "", "", "", "" };
        public static Ability[] specialized_ability = { null, null, null, null, null };

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if ((Url.loaded) && ((root.IsNullOrDestroyed()) || (data.IsNullOrDestroyed())))
            {
                Url.loaded = false;
                if (Hud_Manager.Content.Maxroll.show) { Hud_Manager.Content.Maxroll.Hide(); }
            }
            if ((Url.loaded) && (!Hud_Manager.Content.Maxroll.show) && (!root.IsNullOrDestroyed()) && (!data.IsNullOrDestroyed()) && (Url.selected_profile > -1))
            {
                System.Collections.Generic.List<string> profile_names = new System.Collections.Generic.List<string>();
                foreach (Url.Profile profil in data.Profiles) { profile_names.Add(profil.Name); }
                Url.Profile profile = data.Profiles[Url.selected_profile];
                string class_name = "";
                if (!Refs_Manager.character_class_list.IsNullOrDestroyed())
                {
                    int i = 0;
                    foreach (CharacterClass char_class in Refs_Manager.character_class_list.classes)
                    {
                        if (i == profile.Class) { class_name = char_class.className; break; }
                        i++;
                    }
                }
                bool youtube = false;
                string youtube_url = "";
                if (root.State.LoaderData.LastEpochPlannerById.Metadata.AuthorYoutube != "")
                {
                    youtube = true;
                    youtube_url = root.State.LoaderData.LastEpochPlannerById.Metadata.AuthorYoutube;
                }
                bool twitch = false;
                string twitch_url = "";
                if (root.State.LoaderData.LastEpochPlannerById.Metadata.AuthorTwitch != "")
                {
                    twitch = true;
                    twitch_url = root.State.LoaderData.LastEpochPlannerById.Metadata.AuthorTwitch;
                }
                int nb_items = 0;
                System.Collections.Generic.List<int> items = new System.Collections.Generic.List<int>();
                items.Add(ObjectToInt(profile.Items.Body));
                items.Add(ObjectToInt(profile.Items.Feet));
                items.Add(ObjectToInt(profile.Items.Finger1));
                items.Add(ObjectToInt(profile.Items.Finger2));
                items.Add(ObjectToInt(profile.Items.Hands));
                items.Add(ObjectToInt(profile.Items.Head));
                items.Add(ObjectToInt(profile.Items.Neck));
                items.Add(ObjectToInt(profile.Items.Offhand));
                items.Add(ObjectToInt(profile.Items.Relic));
                items.Add(ObjectToInt(profile.Items.Waist));
                items.Add(ObjectToInt(profile.Items.Weapon));
                foreach (int item in items) { if (item > -1) { nb_items++; } }
                int nb_idols = 0;
                foreach (object item in profile.Idols)
                {
                    if (ObjectToInt(item) > -1) { nb_idols++; }
                }
                int nb_blessings = 0;
                foreach (Blessing item in profile.Blessings)
                {
                    if (!item.IsNullOrDestroyed()) { nb_blessings++; }                    
                }
                int nb_passives = profile.Passives.History.Count;
                int nb_weavertree = profile.Weaver.History.Count;

                string[] specialized_skills = { "", "", "", "", ""};
                int j = 0;
                foreach (string specialized_skill in profile.SpecializedSkills)
                {
                    if (j < specialized_skills.Length) { specialized_skills[j] = specialized_skill; }
                    j++;
                }
                specialized_names = new string[5] { "", "", "", "", "" };
                specialized_ids = new string[5] { "", "", "", "", "" };
                specialized_ability = new Ability[5] { null, null, null, null, null };
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
                            break;
                        }
                    }
                    j++;
                }

                Hud_Manager.Content.Maxroll.Show(profile_names, Url.selected_profile, root.State.LoaderData.LastEpochPlannerById.Profile.Name,
                    root.State.LoaderData.LastEpochPlannerById.Profile.User.Username, youtube, youtube_url, twitch, twitch_url, class_name,
                    profile.Level, nb_items, nb_idols, nb_blessings, nb_passives, nb_weavertree, root.State.LoaderData.LastEpochPlannerById.Profile.Mainset,
                    profile.ActiveSkills[0], profile.ActiveSkills[1], profile.ActiveSkills[2], profile.ActiveSkills[3], profile.ActiveSkills[4]);
            }
        }

        public static async System.Threading.Tasks.Task Load_FromUrl(string url)
        {
            Url.loaded = false;
            //string url = "https://maxroll.gg/last-epoch/planner/a72xm0qj#2"; //Here for debug only
            if (url.Contains("https://maxroll.gg/last-epoch/planner/"))
            {
                HttpResponseMessage response = await new HttpClient().GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                string[] p = url.Split('/');
                string profile = p[p.Length - 1];
                if (profile.Contains("#"))
                {
                    Url.selected_profile = System.Convert.ToInt32(profile.Split('#')[1]) - 1;
                    string p2 = profile.Split('#')[0];                    
                    profile = p2;                    
                }
                else { Url.selected_profile = 0; }
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
                    root = JsonConvert.DeserializeObject<Url.Root>(json);
                    data = JsonConvert.DeserializeObject<Url.Data>(root.State.LoaderData.LastEpochPlannerById.Profile.Data);
                    Url.loaded = true;
                }
            }
            else { Main.logger_instance.Error("Not a Maxroll URL"); }
        } 
        public static void Load_FromClipbboard()
        {
            string JsonString = GUIUtility.systemCopyBuffer;
            if ((JsonString.Substring(2, 5) == "items") && (JsonString.Contains("idols")) && (JsonString.Contains("blessings"))) { Clipboard.Load_AllEquipments(JsonString); }
            else if (JsonString.Substring(2, 5) == "items") { Clipboard.Load_Equipments(JsonString); }
            else if (JsonString.Substring(2, 5) == "idols") { Clipboard.Load_Idols(JsonString); }
            else if (JsonString.Substring(2, 9) == "blessings") { Clipboard.Load_Blessings(JsonString); }
            else if (JsonString.Substring(2, 8) == "passives") { Clipboard.Load_Passives(JsonString); }
            else if (JsonString.Substring(2, 11) == "weaverItems") { Clipboard.Load_Weaver(JsonString); }
            else if (JsonString.Substring(2, 10) == "skillTrees") { Clipboard.Load_SkillTrees(JsonString); }
        }

        public static void DropItem(System.Collections.Generic.List<Affix> affixs, System.Collections.Generic.List<int> implicits,
                int item_type, Affix sealed_affix, Affix primordial_affix, int sub_type, int? unique_id, System.Collections.Generic.List<int> unique_rolls)
        {
            if ((!Refs_Manager.ground_item_manager.IsNullOrDestroyed()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                if (sealed_affix.IsNullOrDestroyed()) { sealed_affix = new Affix { Id = -1, Tier = -1, Roll = -1 }; }
                if (primordial_affix.IsNullOrDestroyed()) { primordial_affix = new Affix { Id = -1, Tier = -1, Roll = -1 }; }
                if (unique_rolls.IsNullOrDestroyed()) { unique_rolls = new System.Collections.Generic.List<int> { -1, -1, -1, -1, -1, -1, -1, -1 }; }


                Il2CppSystem.Collections.Generic.List<ItemAffix> item_affixes = new Il2CppSystem.Collections.Generic.List<ItemAffix>();
                foreach (Affix affix in affixs)
                {
                    ItemAffix new_affix = new ItemAffix { affixId = (ushort)affix.Id, affixTier = (byte)(affix.Tier - 1), affixRoll = (byte)affix.Roll, sealedAffixType = SealedAffixType.None };
                    item_affixes.Add(new_affix);
                }
                bool HasSeal = false;
                if ((sealed_affix.Id > -1) && (sealed_affix.Tier > -1) && (sealed_affix.Roll > -1))
                {
                    ItemAffix new_affix = new ItemAffix { affixId = (ushort)sealed_affix.Id, affixTier = (byte)(sealed_affix.Tier - 1), affixRoll = (byte)sealed_affix.Roll, sealedAffixType = SealedAffixType.Regular };
                    item_affixes.Add(new_affix);
                    HasSeal = true;
                }
                bool HasPrimo = false;
                if ((primordial_affix.Id > -1) && (primordial_affix.Tier > -1) && (primordial_affix.Roll > -1))
                {
                    ItemAffix new_affix = new ItemAffix { affixId = (ushort)primordial_affix.Id, affixTier = (byte)(primordial_affix.Tier - 1), affixRoll = (byte)primordial_affix.Roll, sealedAffixType = SealedAffixType.Primordial };
                    item_affixes.Add(new_affix);
                    HasPrimo = true;
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
                        if ((item_affixes.Count > 0) && (unique_item.isSetItem)) { item_rarity = (byte)(8); }
                        else if (item_affixes.Count > 0) { item_rarity = (byte)(7); }
                        else { item_rarity = (byte)(9); }

                        if (unique_item.legendaryType == UniqueList.LegendaryType.LegendaryPotential)
                        {
                            lp = (byte)Random.RandomRange(0f, 4f);
                        }
                        else
                        {
                            item_legendary_type = UniqueList.LegendaryType.WeaversWill;
                            ww = (byte)Random.RandomRange(0f, 28f);
                        }
                    }
                }
                ItemDataUnpacked item = new ItemDataUnpacked { itemType = (byte)item_type, subType = (ushort)sub_type, rarity = item_rarity, affixes = item_affixes, hasSealedPrimordialAffix = HasPrimo, hasSealedRegularAffix = HasSeal };
                
                int i = 0;
                foreach (int implicit_value in implicits)
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
                    foreach (int unique_roll_value in unique_rolls)
                    {
                        if ((i < item.uniqueRolls.Count) && (unique_roll_value > -1)) { item.uniqueRolls[i] = (byte)(unique_roll_value * 255); }
                        else { break; }
                        i++;
                    }
                }
                item.RefreshIDAndValues();
                Refs_Manager.ground_item_manager.dropItemForPlayer(Refs_Manager.player_actor, item.TryCast<ItemData>(), Refs_Manager.player_actor.position(), false);
            }
        }
        public static void SetBlessings(ushort container_id, System.Collections.Generic.List<double> implicits, int item_type, int sub_type)
        {
            if ((!Refs_Manager.ground_item_manager.IsNullOrDestroyed()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                ItemDataUnpacked item = new ItemDataUnpacked
                {
                    itemType = (byte)item_type,
                    subType = (ushort)sub_type,
                    rarity = 0
                };
                int i = 0;
                foreach (int implicit_value in implicits)
                {
                    if (i < item.implicitRolls.Count) { item.implicitRolls[i] = (byte)(implicit_value * 255); }
                    else { break; }
                    i++;
                }
                item.RefreshIDAndValues();

                bool found = false;
                foreach (Il2CppLE.Data.ItemLocationPair item_pair in Refs_Manager.player_data_tracker.charData.SavedItems)
                {
                    if (item_pair.ContainerID == container_id)
                    {
                        if (item_pair.Data.Count > 7)
                        {
                            if (item_pair.Data[1] == 34)
                            {
                                item_pair.Data[2] = (byte)sub_type;
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
                    byte format_version = 2; //patch

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
        public static void DropIdol(System.Collections.Generic.List<Affix> affixs, int item_type, int sub_type, int? unique_id, System.Collections.Generic.List<int> unique_rolls)
        {
            if ((!Refs_Manager.ground_item_manager.IsNullOrDestroyed()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                if (unique_rolls.IsNullOrDestroyed()) { unique_rolls = new System.Collections.Generic.List<int> { -1, -1, -1, -1, -1, -1, -1, -1 }; }

                Il2CppSystem.Collections.Generic.List<ItemAffix> item_affixes = new Il2CppSystem.Collections.Generic.List<ItemAffix>();
                foreach (Affix affix in affixs)
                {
                    ItemAffix new_affix = new ItemAffix { affixId = (ushort)affix.Id, affixTier = (byte)(affix.Tier - 1), affixRoll = (byte)affix.Roll, sealedAffixType = SealedAffixType.None };
                    item_affixes.Add(new_affix);
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
                        if ((item_affixes.Count > 0) && (unique_item.isSetItem)) { item_rarity = (byte)(8); }
                        else if (item_affixes.Count > 0) { item_rarity = (byte)(7); }
                        else { item_rarity = (byte)(9); }

                        if (unique_item.legendaryType == UniqueList.LegendaryType.LegendaryPotential)
                        {
                            lp = (byte)Random.RandomRange(0f, 4f);
                        }
                        else
                        {
                            item_legendary_type = UniqueList.LegendaryType.WeaversWill;
                            ww = (byte)Random.RandomRange(0f, 28f);
                        }
                    }
                }

                ItemDataUnpacked item = new ItemDataUnpacked { itemType = (byte)item_type, subType = (ushort)sub_type, affixes = item_affixes, rarity = item_rarity };

                if (unique_id != null)
                {
                    item.uniqueID = (ushort)unique_id;
                    if (item_legendary_type == UniqueList.LegendaryType.LegendaryPotential) { item.legendaryPotential = lp; }
                    else { item.weaversWill = ww; }
                }

                int i = 0;
                foreach (int unique_roll_value in unique_rolls)
                {
                    if ((i < item.uniqueRolls.Count) && (unique_roll_value > -1)) { item.uniqueRolls[i] = (byte)(unique_roll_value * 255); }
                    else { break; }
                    i++;
                }

                item.RefreshIDAndValues();
                Refs_Manager.ground_item_manager.dropItemForPlayer(Refs_Manager.player_actor, item.TryCast<ItemData>(), Refs_Manager.player_actor.position(), false);
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

        public class UI
        {
            public static bool Initialized = false;
            public static void InitRefs()
            {

            }
            public static void Set()
            {

            }
        }
        public class Url
        {
            public static int selected_profile = -1;
            public static bool loaded = false;

            public static void Load_AllEquipments()
            {
                if (selected_profile > -1)
                {
                    Profile profile = data.Profiles[selected_profile];
                    System.Collections.Generic.List<int> items = new System.Collections.Generic.List<int>();
                    items.Add(ObjectToInt(profile.Items.Body));
                    items.Add(ObjectToInt(profile.Items.Feet));
                    items.Add(ObjectToInt(profile.Items.Finger1));
                    items.Add(ObjectToInt(profile.Items.Finger2));
                    items.Add(ObjectToInt(profile.Items.Hands));
                    items.Add(ObjectToInt(profile.Items.Head));
                    items.Add(ObjectToInt(profile.Items.Neck));
                    items.Add(ObjectToInt(profile.Items.Offhand));
                    items.Add(ObjectToInt(profile.Items.Relic));
                    items.Add(ObjectToInt(profile.Items.Waist));
                    items.Add(ObjectToInt(profile.Items.Weapon));
                    foreach (System.Collections.Generic.KeyValuePair<int, Item> item in data.Items)
                    {
                        if (items.Contains(item.Key))
                        {
                            DropItem(item.Value.Affixes, item.Value.Implicits, item.Value.ItemType, item.Value.SealedAffix, item.Value.PrimordialAffix, item.Value.SubType, item.Value.UniqueID, item.Value.UniqueRolls);
                        }
                    }
                    int i = 0;
                    foreach (Blessing item in profile.Blessings)
                    {
                        if (!item.IsNullOrDestroyed()) { SetBlessings((ushort)(i + blessing_container), item.Implicits, item.ItemType, item.SubType); }
                        i++;
                    }

                    items = new System.Collections.Generic.List<int>();
                    foreach (object item in  profile.Idols)
                    {
                        items.Add(ObjectToInt(item));
                    }
                    foreach (System.Collections.Generic.KeyValuePair<int, Item> item in data.Items)
                    {
                        if (items.Contains(item.Key))
                        {
                            DropIdol(item.Value.Affixes, item.Value.ItemType, item.Value.SubType, item.Value.UniqueID, item.Value.UniqueRolls);
                        }
                    }
                }
            }
            public static void Load_Equipments()
            {
                if (selected_profile > -1)
                {
                    Profile profile = data.Profiles[selected_profile];
                    System.Collections.Generic.List<int> items = new System.Collections.Generic.List<int>();
                    items.Add(ObjectToInt(profile.Items.Body));
                    items.Add(ObjectToInt(profile.Items.Feet));
                    items.Add(ObjectToInt(profile.Items.Finger1));
                    items.Add(ObjectToInt(profile.Items.Finger2));
                    items.Add(ObjectToInt(profile.Items.Hands));
                    items.Add(ObjectToInt(profile.Items.Head));
                    items.Add(ObjectToInt(profile.Items.Neck));
                    items.Add(ObjectToInt(profile.Items.Offhand));
                    items.Add(ObjectToInt(profile.Items.Relic));
                    items.Add(ObjectToInt(profile.Items.Waist));
                    items.Add(ObjectToInt(profile.Items.Weapon));
                    foreach (System.Collections.Generic.KeyValuePair<int, Item> item in data.Items)
                    {
                        if (items.Contains(item.Key))
                        {
                            DropItem(item.Value.Affixes, item.Value.Implicits, item.Value.ItemType, item.Value.SealedAffix, item.Value.PrimordialAffix, item.Value.SubType, item.Value.UniqueID, item.Value.UniqueRolls);
                        }
                    }
                }
            }
            public static void Load_Idols()
            {
                if (selected_profile > -1)
                {
                    Profile profile = data.Profiles[selected_profile];
                    System.Collections.Generic.List<int> items = new System.Collections.Generic.List<int>();
                    foreach (object item in profile.Idols)
                    {
                        items.Add(ObjectToInt(item));
                    }
                    foreach (System.Collections.Generic.KeyValuePair<int, Item> item in data.Items)
                    {
                        if (items.Contains(item.Key))
                        {
                            DropIdol(item.Value.Affixes, item.Value.ItemType, item.Value.SubType, item.Value.UniqueID, item.Value.UniqueRolls);
                        }
                    }
                }
            }
            public static void Load_Blessings()
            {
                if (selected_profile > -1)
                {
                    int i = 0;
                    foreach (Blessing item in data.Profiles[selected_profile].Blessings)
                    {
                        if (!item.IsNullOrDestroyed()) { SetBlessings((ushort)(i + blessing_container), item.Implicits, item.ItemType, item.SubType); }
                        i++;
                    }
                }
            }
            public static void Load_Passives()
            {
                if ((selected_profile > -1) && (!Refs_Manager.player_treedata.IsNullOrDestroyed()) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
                {
                    Profile profile = data.Profiles[selected_profile];
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
                    else { Main.logger_instance.Error("Not the good class"); }
                }
            }
            public static void Load_Weaver()
            {
                if ((selected_profile > -1) && (!Refs_Manager.player_treedata.IsNullOrDestroyed()))
                {
                    Profile profile = data.Profiles[selected_profile];
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
                    for (int i = 0; i < 5; i++)
                    {
                        Load_SkillTree(i, i);
                    }
                }
            }
            public static void Load_SkillTree(int index, int slot)
            {
                if (selected_profile > -1)
                {
                    Profile profile = data.Profiles[selected_profile];
                    if (!Refs_Manager.player_treedata.IsNullOrDestroyed())
                    {
                        string skill_id = specialized_ids[index];                        
                        if (skill_id != "")
                        {
                            NodePoint points = null;
                            foreach (System.Collections.Generic.KeyValuePair<string, NodePoint> skill in profile.Skill)
                            {
                                if (skill.Key == skill_id)
                                {
                                    points = skill.Value;
                                    break;
                                }
                            }
                            Ability ability_to_remove = Refs_Manager.player_treedata.getSpecialisedAbilityInSlot((byte)slot);
                            if (!ability_to_remove.IsNullOrDestroyed()) { Refs_Manager.player_treedata.Despecialise(ability_to_remove, true); }
                            Refs_Manager.player_treedata.Specialise(specialized_ability[index], (byte)slot, true);
                            //Refs_Manager.player_treedata.specialisedSkillTrees.Add(new LocalTreeData.SkillTreeData(skill_id, (byte)slot));
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
                if (selected_profile > -1)
                {
                    Profile profile = data.Profiles[selected_profile];
                    //profile.ActiveSkills[0]
                    //profile.ActiveSkills[1]
                    //profile.ActiveSkills[2]
                    //profile.ActiveSkills[3]
                    //profile.ActiveSkills[4]
                }
            }

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
            }
        }
        public class Clipboard
        {
            public static void Load_AllEquipments(string JsonString)
            {
                AllEquipments AllEquipmentsData = JsonConvert.DeserializeObject<AllEquipments>(JsonString);
                Item[] items = { AllEquipmentsData.Items.Body, AllEquipmentsData.Items.Feet, AllEquipmentsData.Items.Finger1,
                    AllEquipmentsData.Items.Finger2, AllEquipmentsData.Items.Hands, AllEquipmentsData.Items.Head,
                    AllEquipmentsData.Items.Neck, AllEquipmentsData.Items.Offhand, AllEquipmentsData.Items.Relic,
                    AllEquipmentsData.Items.Waist, AllEquipmentsData.Items.Weapon };
                foreach (Item item in items)
                {
                    if (!item.IsNullOrDestroyed())
                    {
                        if (item.SealedAffix.IsNullOrDestroyed()) { item.SealedAffix = new Affix { Id = -1, Tier = -1, Roll = -1 }; }
                        if (item.PrimordialAffix.IsNullOrDestroyed()) { item.PrimordialAffix = new Affix { Id = -1, Tier = -1, Roll = -1 }; }
                        if (item.UniqueRolls.IsNullOrDestroyed()) { item.UniqueRolls = new System.Collections.Generic.List<int> { -1, -1, -1, -1, -1, -1, -1, -1 }; }
                        DropItem(item.Affixes, item.Implicits, item.ItemType, item.SealedAffix, item.PrimordialAffix, item.SubType, item.UniqueID, item.UniqueRolls);
                    }
                }
                int i = 0;
                foreach (Blessing item in AllEquipmentsData.Blessings)
                {
                    if (!item.IsNullOrDestroyed()) { SetBlessings((ushort)(i + blessing_container), item.Implicits, item.ItemType, item.SubType); }
                    i++;
                }
                foreach (Idol item in AllEquipmentsData.Idols)
                {
                    if (!item.IsNullOrDestroyed())
                    {
                        if (item.UniqueRolls.IsNullOrDestroyed())
                        {
                            item.UniqueRolls = new System.Collections.Generic.List<int> { -1, -1, -1, -1, -1, -1, -1, -1 };
                        }
                        DropIdol(item.Affixes, item.ItemType, item.SubType, item.UniqueID, item.UniqueRolls);
                    }
                }
            }
            public static void Load_Equipments(string JsonString)
            {
                Equipment EquipmentData = JsonConvert.DeserializeObject<Equipment>(JsonString);
                Item[] items = { EquipmentData.Items.Body, EquipmentData.Items.Feet, EquipmentData.Items.Finger1,
                    EquipmentData.Items.Finger2, EquipmentData.Items.Hands, EquipmentData.Items.Head,
                    EquipmentData.Items.Neck, EquipmentData.Items.Offhand, EquipmentData.Items.Relic,
                    EquipmentData.Items.Waist, EquipmentData.Items.Weapon };
                foreach (Item item in items)
                {
                    if (!item.IsNullOrDestroyed())
                    {
                        if (item.SealedAffix.IsNullOrDestroyed()) { item.SealedAffix = new Affix { Id = -1, Tier = -1, Roll = -1 }; }
                        if (item.PrimordialAffix.IsNullOrDestroyed()) { item.PrimordialAffix = new Affix { Id = -1, Tier = -1, Roll = -1 }; }
                        if (item.UniqueRolls.IsNullOrDestroyed()) { item.UniqueRolls = new System.Collections.Generic.List<int> { -1, -1, -1, -1, -1, -1, -1, -1 }; }
                        DropItem(item.Affixes, item.Implicits, item.ItemType, item.SealedAffix, item.PrimordialAffix, item.SubType, item.UniqueID, item.UniqueRolls);
                    }
                }
            }
            public static void Load_Idols(string JsonString)
            {
                AllIdols IdolsData = JsonConvert.DeserializeObject<AllIdols>(JsonString);
                foreach (Idol item in IdolsData.Idols)
                {
                    if (!item.IsNullOrDestroyed())
                    {
                        if (item.UniqueRolls.IsNullOrDestroyed())
                        {
                            item.UniqueRolls = new System.Collections.Generic.List<int> { -1, -1, -1, -1, -1, -1, -1, -1 };
                        }
                        DropIdol(item.Affixes, item.ItemType, item.SubType, item.UniqueID, item.UniqueRolls);
                    }
                }
            }
            public static void Load_Blessings(string JsonString)
            {
                AllBlessings BlessingsData = JsonConvert.DeserializeObject<AllBlessings>(JsonString);
                int i = 0;
                foreach (Blessing item in BlessingsData.Blessings)
                {
                    if (!item.IsNullOrDestroyed()) { SetBlessings((ushort)(i + blessing_container), item.Implicits, item.ItemType, item.SubType); }
                    i++;
                }
            }
            public static void Load_Passives(string JsonString)
            {
                AllPassives PassivesData = JsonConvert.DeserializeObject<AllPassives>(JsonString);
                if ((!Refs_Manager.player_treedata.IsNullOrDestroyed()) && (!Refs_Manager.player_data.IsNullOrDestroyed()))
                {
                    if (Refs_Manager.player_data.CharacterClass == PassivesData.Class)
                    {
                        Refs_Manager.player_data.ChosenMastery = (byte)PassivesData.Mastery;
                        Refs_Manager.player_data.ClickedUnlockMasteriesButton = true;
                        Refs_Manager.player_treedata.chosenMastery = (byte)PassivesData.Mastery;
                        Refs_Manager.player_treedata.passiveTree.nodes.Clear();
                        foreach (int node_id in PassivesData.Passives.History)
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
                    else { Main.logger_instance.Error("Not the good class"); }
                }
            }
            public static void Load_Weaver(string JsonString)
            {
                WeaverTree WeaverTreeData = JsonConvert.DeserializeObject<WeaverTree>(JsonString);
                if (!Refs_Manager.player_treedata.IsNullOrDestroyed())
                {
                    Refs_Manager.player_treedata.weaverTree.nodes.Clear();
                    foreach (int node_id in WeaverTreeData.Weaver.History)
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
            public static void Load_SkillTrees(string JsonString)
            {
                Skills SkillsData = JsonConvert.DeserializeObject<Skills>(JsonString);

            }

            public class AllEquipments
            {
                [JsonProperty("items")]
                public Items Items;

                [JsonProperty("idols")]
                public System.Collections.Generic.List<Idol> Idols;

                [JsonProperty("blessings")]
                public System.Collections.Generic.List<Blessing> Blessings;
            }
            public class Equipment
            {
                [JsonProperty("items")]
                public Items Items;
            }
            public class AllIdols
            {
                [JsonProperty("idols")]
                public System.Collections.Generic.List<Idol> Idols;
            }
            public class AllBlessings
            {
                [JsonProperty("blessings")]
                public System.Collections.Generic.List<Blessing> Blessings;
            }
            public class AllPassives
            {
                [JsonProperty("passives")]
                public NodePoint Passives;

                [JsonProperty("class")]
                public int Class;

                [JsonProperty("mastery")]
                public int Mastery;
            }
            public class WeaverTree
            {
                [JsonProperty("weaverItems")] //items placed inside the weaver tree
                public System.Collections.Generic.List<Item> WeaverItems;

                [JsonProperty("weaver")]
                public NodePoint Weaver;
            }
            public class Skills
            {
                //https://github.com/exiledagain/LastEpoch_Mods/blob/f881533806e869b15218e5c0c29abedb8b3dface/LastEpoch_Hud/Scripts/Mods/Maxroll/Maxroll_import.cs#L242
                [JsonProperty("skillTrees")]
                public System.Collections.Generic.Dictionary<string, NodePoint> Skill;
            }
            public class Items
            {
                [JsonProperty("body")]
                public Item Body;

                [JsonProperty("offhand")]
                public Item Offhand;

                [JsonProperty("waist")]
                public Item Waist;

                [JsonProperty("feet")]
                public Item Feet;

                [JsonProperty("finger1")]
                public Item Finger1;

                [JsonProperty("finger2")]
                public Item Finger2;

                [JsonProperty("neck")]
                public Item Neck;

                [JsonProperty("relic")]
                public Item Relic;

                [JsonProperty("hands")]
                public Item Hands;

                [JsonProperty("head")]
                public Item Head;

                [JsonProperty("weapon")]
                public Item Weapon;
            }
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
            public System.Collections.Generic.List<int> UniqueRolls;

            [JsonProperty("affixes")]
            public System.Collections.Generic.List<Affix> Affixes;

            [JsonProperty("sealedAffix")]
            public Affix SealedAffix;

            [JsonProperty("primordialAffix")]
            public Affix PrimordialAffix;

            [JsonProperty("implicits")]
            public System.Collections.Generic.List<int> Implicits;
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
            public System.Collections.Generic.List<int> UniqueRolls;
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
            public int Roll;
        }
        public class NodePoint
        {
            [JsonProperty("history")]
            public System.Collections.Generic.List<int> History;

            [JsonProperty("position")]
            public int Position;
        }
    }
}
