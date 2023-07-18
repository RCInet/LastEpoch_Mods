using System.Globalization;
using System.Xml.Linq;
using UnityEngine;
using static AffixList;
using static LastEpochMods.Ui.GenerateItem.affixs;

namespace LastEpochMods.Ui
{
    public class GenerateItem
    {
        public static void Init()
        {
            type.InitList();
            affixs.InitList();
        }
        public class main
        {
            public static void UI(float pos_x, float pos_y, float btn_w)
            {
                int h = GetSizeH();
                int w = (int)(btn_w) - 10;
                dropdown.InitScrollViewPositions(pos_x);
                GUI.DrawTexture(new Rect(pos_x, pos_y, btn_w, h), Menu.texture_grey);
                pos_x += 5;
                pos_y += 5;

                pos_y = type.UI(pos_x, pos_y, w);
                pos_y = rarity.UI(pos_x, pos_y, w);
                pos_y = base_item.UI(pos_x, pos_y, w);
                pos_y = unique.UI(pos_x, pos_y, w);
                pos_y = set.UI(pos_x, pos_y, w);
                pos_y = legendary.UI(pos_x, pos_y, w);
                pos_y = affixs.UI(pos_x, pos_y, w);
                pos_y = quantity.UI(pos_x, pos_y, w);
                pos_y = drop.UI(pos_x, pos_y, w);
            }
            public static void Reset()
            {
                type.dropdown_index = -1;
                rarity.enable = false;
                rarity.dropdown_index = -1;
                base_item.enable = false;
                base_item.dropdown_index -= 1;
                unique.enable = false;
                unique.dropdown_index -= 1;
                set.enable = false;
                set.dropdown_index -= 1;
                legendary.enable = false;
                legendary.dropdown_index -= 1;
                affixs.enable = false;
                affixs.prefixs.slot_0.dropdown_index = -1;
                affixs.prefixs.slot_1.dropdown_index = -1;
                affixs.suffixs.slot_0.dropdown_index -= 1;
                affixs.suffixs.slot_1.dropdown_index -= 1;
                affixs.idols.slot_0.dropdown_index += 1;
                affixs.idols.slot_1.dropdown_index += 1;
                affixs.idols.slot_2.dropdown_index += 1;
                affixs.idols.slot_3.dropdown_index += 1;
                quantity.enable = false;
                drop.enable = false;
            }
            public static int GetSizeH()
            {
                int result = 5 + type.h;
                if (rarity.enable) { result += rarity.h; }
                if (base_item.enable) { result += base_item.h; }
                if (unique.enable) { result += unique.h; }
                if (set.enable) { result += set.h; }
                if (legendary.enable) { result += legendary.h; }
                if (quantity.enable) { result += quantity.h; }
                if (affixs.enable) { result += affixs.h + (affixs.affix_h * affixs.nb_affixs); }
                if (drop.enable) { result += drop.h; }

                return result;
            }

            public class dropdown
            {
                public static void DropdownsUI()
                {
                    Dropdown(type.pos_x, type.pos_y, type.dropdown_rect, ref type.dropdown_scrollview, ref type.dropdown_index, ref type.show_dropdown, type.dropdown_list);
                    Dropdown(rarity.pos_x, rarity.pos_y, rarity.dropdown_rect, ref rarity.dropdown_scrollview, ref rarity.dropdown_index, ref rarity.show_dropdown, rarity.dropdown_list);
                    Dropdown(base_item.pos_x, base_item.pos_y, base_item.dropdown_rect, ref base_item.dropdown_scrollview, ref base_item.dropdown_index, ref base_item.show_dropdown, base_item.dropdown_list);
                    Dropdown(unique.pos_x, unique.pos_y, unique.dropdown_rect, ref unique.dropdown_scrollview, ref unique.dropdown_index, ref unique.show_dropdown, unique.dropdown_list);
                    Dropdown(set.pos_x, set.pos_y, set.dropdown_rect, ref set.dropdown_scrollview, ref set.dropdown_index, ref set.show_dropdown, set.dropdown_list);
                    Dropdown(legendary.pos_x, legendary.pos_y, legendary.dropdown_rect, ref legendary.dropdown_scrollview, ref legendary.dropdown_index, ref legendary.show_dropdown, legendary.dropdown_list);

                    Dropdown(affixs.prefixs.slot_0.pos_x, affixs.prefixs.slot_0.pos_y, affixs.prefixs.slot_0.dropdown_rect, ref affixs.prefixs.slot_0.dropdown_scrollview, ref affixs.prefixs.slot_0.dropdown_index, ref affixs.prefixs.slot_0.show_dropdown, affixs.prefixs.dropdown_list);
                    Dropdown(affixs.prefixs.slot_1.pos_x, affixs.prefixs.slot_1.pos_y, affixs.prefixs.slot_1.dropdown_rect, ref affixs.prefixs.slot_1.dropdown_scrollview, ref affixs.prefixs.slot_1.dropdown_index, ref affixs.prefixs.slot_1.show_dropdown, affixs.prefixs.dropdown_list);
                    Dropdown(affixs.suffixs.slot_0.pos_x, affixs.suffixs.slot_0.pos_y, affixs.suffixs.slot_0.dropdown_rect, ref affixs.suffixs.slot_0.dropdown_scrollview, ref affixs.suffixs.slot_0.dropdown_index, ref affixs.suffixs.slot_0.show_dropdown, affixs.suffixs.dropdown_list);
                    Dropdown(affixs.suffixs.slot_1.pos_x, affixs.suffixs.slot_1.pos_y, affixs.suffixs.slot_1.dropdown_rect, ref affixs.suffixs.slot_1.dropdown_scrollview, ref affixs.suffixs.slot_1.dropdown_index, ref affixs.suffixs.slot_1.show_dropdown, affixs.suffixs.dropdown_list);

                    Dropdown(affixs.idols.slot_0.pos_x, affixs.idols.slot_0.pos_y, affixs.idols.slot_0.dropdown_rect, ref affixs.idols.slot_0.dropdown_scrollview, ref affixs.idols.slot_0.dropdown_index, ref affixs.idols.slot_0.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_1.pos_x, affixs.idols.slot_1.pos_y, affixs.idols.slot_1.dropdown_rect, ref affixs.idols.slot_1.dropdown_scrollview, ref affixs.idols.slot_1.dropdown_index, ref affixs.idols.slot_1.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_2.pos_x, affixs.idols.slot_2.pos_y, affixs.idols.slot_2.dropdown_rect, ref affixs.idols.slot_2.dropdown_scrollview, ref affixs.idols.slot_2.dropdown_index, ref affixs.idols.slot_2.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_3.pos_x, affixs.idols.slot_3.pos_y, affixs.idols.slot_3.dropdown_rect, ref affixs.idols.slot_3.dropdown_scrollview, ref affixs.idols.slot_3.dropdown_index, ref affixs.idols.slot_3.show_dropdown, affixs.idols.dropdown_list);
                }
                public static void InitScrollViewPositions(float pos_x)
                {
                    type.pos_x = pos_x;
                    rarity.pos_x = pos_x;
                    base_item.pos_x = pos_x;
                    unique.pos_x = pos_x;
                    set.pos_x = pos_x;
                    legendary.pos_x = pos_x;
                    affixs.prefixs.slot_0.pos_x = pos_x;
                    affixs.prefixs.slot_1.pos_x = pos_x;
                    affixs.suffixs.slot_0.pos_x = pos_x;
                    affixs.suffixs.slot_1.pos_x = pos_x;
                    affixs.idols.slot_0.pos_x = pos_x;
                    affixs.idols.slot_1.pos_x = pos_x;
                    affixs.idols.slot_2.pos_x = pos_x;
                    affixs.idols.slot_3.pos_x = pos_x;
                }
                public static void Dropdown(float pos_x, float pos_y, Rect rect, ref Vector2 scrollview, ref int dropdown_index, ref bool show, string[] dropdown_list)
                {
                    int button_h = 40;
                    int margin = 2;
                    pos_x += 200;
                    pos_y -= 10;
                    if (show)
                    {
                        float size_h = 0;
                        float scrollview_max_h = (dropdown_list.Length * (button_h + margin));
                        bool scroll = false;
                        if (rect.height > scrollview_max_h) { size_h = scrollview_max_h; }
                        else
                        {
                            size_h = rect.height;
                            scroll = true;
                        }
                        GUI.DrawTexture(new Rect(pos_x, pos_y, rect.width + 10, size_h + 10), Menu.windowBackground);
                        pos_x += 5;
                        pos_y += 5;
                        float scrollview_w = rect.width;
                        if (scroll) { scrollview_w = rect.width - 20; }
                        scrollview = GUI.BeginScrollView(new Rect(pos_x, pos_y, rect.width, size_h), scrollview, new Rect(0, 0, scrollview_w, scrollview_max_h));
                        UnityEngine.GUIStyle style = Styles.Button_Style(false);
                        for (int index = 0; index < dropdown_list.Length; index++)
                        {
                            if (GUI.Button(new Rect(0, (index * (button_h + margin)), rect.width - 25, button_h), "", style))
                            {
                                show = false;
                                dropdown_index = index;
                            }
                            GUI.Label(new Rect(5, ((index * (button_h + margin)) + 3), rect.width - 30, button_h - 3), dropdown_list[index], Styles.DropdownLabelLeft_Style());
                        }
                        GUI.EndScrollView();
                    }
                }
            }
        }
        public class type
        {
            public static bool show_dropdown = false;
            public static string[] dropdown_list = null;
            public static int dropdown_index = -1;
            public static int dropdown_index_backup = 99;
            public static float pos_x;
            public static float pos_y;
            public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
            public static Vector2 dropdown_scrollview = Vector2.zero;
            public static int h = 35;

            public static float UI(float pos_x, float pos_y, int w)
            {
                type.pos_y = pos_y;
                if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                {
                    rarity.show_dropdown = false;
                    base_item.show_dropdown = false;
                    unique.show_dropdown = false;
                    set.show_dropdown = false;
                    legendary.show_dropdown = false;
                    affixs.Hide();
                    type.show_dropdown = !type.show_dropdown;
                }
                if ((type.dropdown_index < type.dropdown_list.Length) && (type.dropdown_index > -1))
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), type.dropdown_list[type.dropdown_index], Styles.DropdownLabelMidle_Style());
                    if (type.dropdown_index_backup != type.dropdown_index)
                    {
                        type.dropdown_index_backup = type.dropdown_index;
                        rarity.dropdown_index = -1;
                        rarity.enable = rarity.EnableFromType(type.dropdown_list[type.dropdown_index]);
                        base_item.dropdown_index = -1;
                        base_item.InitFromType(type.dropdown_list[type.dropdown_index]);
                        base_item.enable = base_item.EnableFromType(type.dropdown_list[type.dropdown_index]);
                        affixs.enable = false;
                        unique.enable = false;
                        set.enable = false;
                        legendary.enable = false;
                        unique.UpdateList(type.dropdown_list[type.dropdown_index]);
                        quantity.enable = quantity.EnableFromType(type.dropdown_list[type.dropdown_index]);
                        drop.enable = drop.EnableFromType(type.dropdown_list[type.dropdown_index]);
                    }
                }
                else
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Type", Styles.DropdownLabelMidle_Style());
                    rarity.enable = false;
                    quantity.enable = false;
                    base_item.enable = false;
                    unique.enable = false;
                    drop.enable = false;
                }
                pos_y += 35;

                return pos_y;
            }
            public static void InitList()
            {
                type.dropdown_list = null;
                bool type_error = false;
                try { if (ItemList.get() == null) { type_error = true; } }
                catch { }
                if (!type_error)
                {
                    System.Collections.Generic.List<string> type_names = new System.Collections.Generic.List<string>();
                    foreach (ItemList.BaseEquipmentItem item in ItemList.get().EquippableItems)
                    {
                        if (item.displayName != "") { type_names.Add(item.displayName); }
                    }
                    foreach (ItemList.BaseNonEquipmentItem item in ItemList.get().nonEquippableItems)
                    {
                        if (item.displayName != "") { type_names.Add(item.displayName); }
                    }
                    type.dropdown_list = new string[type_names.Count];
                    for (int i = 0; i < type.dropdown_list.Length; i++)
                    {
                        type.dropdown_list[i] = type_names[i];
                    }
                }                
            }
            public static int GetIdFromName(string name)
            {
                bool type_error = false;
                int result = 0;
                try { if (ItemList.get() == null) { type_error = true; } }
                catch { }
                if (!type_error)
                {
                    bool found = false;
                    foreach (ItemList.BaseEquipmentItem item in ItemList.get().EquippableItems)
                    {
                        if (item.displayName == name) { result = item.baseTypeID; found = true; break; }
                    }
                    if (!found)
                    {
                        foreach (ItemList.BaseNonEquipmentItem item in ItemList.get().nonEquippableItems)
                        {
                            if (item.displayName == name) { result = item.baseTypeID; found = true; break; }
                        }
                    }                    
                }

                return result;
            }
        }
        public class rarity
        {
            public static bool enable = false;
            public static bool show_dropdown = false;
            public static string[] dropdown_list = new string[] { "Basic", "Unique", "Set", "Legendary" };
            public static int dropdown_index = -1;
            public static float pos_x;
            public static float pos_y;
            public static Rect dropdown_rect = new Rect(125, 50, 200, 400);
            public static Vector2 dropdown_scrollview = Vector2.zero;
            public static int h = 35;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (rarity.enable)
                {
                    rarity.pos_y = pos_y;
                    if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                    {
                        type.show_dropdown = false;
                        base_item.show_dropdown = false;
                        base_item.dropdown_index = -1;
                        unique.show_dropdown = false;
                        unique.dropdown_index = -1;
                        set.show_dropdown = false;
                        set.dropdown_index = -1;
                        legendary.show_dropdown = false;
                        legendary.dropdown_index = -1;
                        rarity.show_dropdown = !rarity.show_dropdown;
                    }
                    if ((rarity.dropdown_index < rarity.dropdown_list.Length) && (rarity.dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), rarity.dropdown_list[rarity.dropdown_index], Styles.DropdownLabelMidle_Style());
                        if (rarity.dropdown_index == 0)
                        {
                            unique.enable = false;
                            set.enable = false;
                            legendary.enable = false;
                            base_item.enable = true;

                        }
                        else if (rarity.dropdown_index == 1)
                        {
                            base_item.enable = false;
                            set.enable = false;
                            legendary.enable = false;
                            unique.enable = true;
                        }
                        else if (rarity.dropdown_index == 2)
                        {
                            base_item.enable = false;
                            unique.enable = false;
                            legendary.enable = false;
                            set.enable = true;
                        }
                        else if (rarity.dropdown_index == 3)
                        {
                            base_item.enable = false;
                            unique.enable = false;
                            set.enable = false;
                            legendary.enable = true;
                        }
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Rarity", Styles.DropdownLabelMidle_Style());
                        base_item.enable = false;
                        unique.enable = false;
                        set.enable = false;
                        legendary.enable = false;
                    }
                    pos_y += 35;
                }
                else
                {
                    rarity.show_dropdown = false;
                    unique.show_dropdown = false;
                    set.show_dropdown = false;
                    legendary.show_dropdown = false;
                }

                return pos_y;
            }
            public static bool EnableFromType(string type_name)
            {
                bool result = false;
                if (type_name.Contains("Idol")) { result = true; }
                else if ((type_name != "Affix Shard") && (type_name != "Blessing") && 
                    (type_name != "Rune") && (type_name != "Glyph") &&
                    (type_name != "Key") && (type_name != "Lost Memory"))
                {                    
                    result = true;
                }                

                return result;
            }
        }
        public class base_item
        {
            public static bool enable = false;
            public static bool show_dropdown = false;
            public static string[] dropdown_list = null;
            public static int dropdown_index = -1;
            public static float pos_x;
            public static float pos_y;
            public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
            public static Vector2 dropdown_scrollview = Vector2.zero;
            public static int h = 35;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (base_item.enable)
                {
                    base_item.pos_y = pos_y;
                    if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                    {
                        unique.show_dropdown = false;
                        set.show_dropdown = false;
                        legendary.show_dropdown = false;
                        type.show_dropdown = false;
                        rarity.show_dropdown = false;
                        base_item.show_dropdown = !base_item.show_dropdown;
                    }
                    if ((base_item.dropdown_index < base_item.dropdown_list.Length) && (base_item.dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), base_item.dropdown_list[base_item.dropdown_index], Styles.DropdownLabelMidle_Style());                        
                        affixs.enable = true;
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Base Item", Styles.DropdownLabelMidle_Style());
                        affixs.enable = false;
                        quantity.enable = false;
                        drop.enable = false;
                    }
                    pos_y += 35;
                }
                else { base_item.show_dropdown = false; }

                return pos_y;
            }
            public static void InitFromType(string type_name)
            {
                bool type_error = false;
                try { if (ItemList.get() == null) { type_error = true; } }
                catch { }
                if (!type_error)
                {
                    bool found = false;
                    System.Collections.Generic.List<string> base_names = new System.Collections.Generic.List<string>();
                    foreach (ItemList.BaseEquipmentItem item in ItemList.get().EquippableItems)
                    {
                        if (type_name == item.displayName)
                        {
                            found = true;
                            foreach (ItemList.EquipmentItem subitem in item.subItems)
                            {
                                string result = subitem.name;
                                try
                                {
                                    int str = int.Parse(subitem.name, CultureInfo.InvariantCulture.NumberFormat);
                                    result = subitem.displayName;
                                }
                                catch { }
                                base_names.Add(result);
                            }
                            break;
                        }
                    }
                    if (!found)
                    {
                        foreach (ItemList.BaseNonEquipmentItem item in ItemList.get().nonEquippableItems)
                        {
                            if (type_name == item.displayName)
                            {
                                found = true;
                                foreach (ItemList.NonEquipmentItem subitem in item.subItems)
                                {
                                    base_names.Add(subitem.name);
                                }
                                break;
                            }
                        }
                    }
                    if (found)
                    {
                        base_item.dropdown_list = new string[base_names.Count];
                        for (int i = 0; i < base_item.dropdown_list.Length; i++) { base_item.dropdown_list[i] = base_names[i]; }
                    }
                    else { Main.logger_instance.Msg("Error Type Not Found : " + type_name + " Fin"); }
                }
                else { Main.logger_instance.Msg("Error ItemList is null"); }
            }
            public static bool EnableFromType(string type_name)
            {
                bool result = false;
                if ((type_name == "Affix Shard") || (type_name == "Blessing") ||
                    (type_name == "Rune") || (type_name == "Glyph") ||
                    (type_name == "Key") || (type_name == "Lost Memory"))
                {
                    result = true;
                }

                return result;
            }
            public static int GetIdFromName(string type_name, string base_name)
            {
                bool type_error = false;
                int result = 0;
                try { if (ItemList.get() == null) { type_error = true; } }
                catch { }
                if (!type_error)
                {
                    bool found = false;
                    foreach (ItemList.BaseEquipmentItem item in ItemList.get().EquippableItems)
                    {
                        if (type_name == item.displayName)
                        {
                            foreach (ItemList.EquipmentItem subitem in item.subItems)
                            {
                                if ((base_name == subitem.name) || (base_name == subitem.displayName))
                                {
                                    result = subitem.subTypeID;
                                    found = true;
                                    break;
                                }
                            }                            
                            break;
                        }
                    }
                    if (!found)
                    {
                        foreach (ItemList.BaseNonEquipmentItem item in ItemList.get().nonEquippableItems)
                        {
                            if (type_name == item.displayName)
                            {
                                Main.logger_instance.Msg("Type Found");
                                foreach (ItemList.NonEquipmentItem subitem in item.subItems)
                                {
                                    if ((base_name == subitem.name) || (base_name == subitem.displayName))
                                    {                                        
                                        result = subitem.subTypeID;
                                        Main.logger_instance.Msg("Base Found : " + result);
                                        found = true;
                                        break;
                                    }
                                }
                                break;
                            }
                        }                        
                    }
                    if (!found) { Main.logger_instance.Msg("Base Item Not Found"); }
                }

                return result;
            }
        }                
        public class unique
        {
            public static bool enable = false;
            public static bool show_dropdown = false;
            public static string[] dropdown_list = null;
            public static int dropdown_index = -1;
            public static float pos_x;
            public static float pos_y;
            public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
            public static Vector2 dropdown_scrollview = Vector2.zero;
            public static int h = 35;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (unique.enable)
                {
                    unique.pos_y = pos_y;
                    if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                    {
                        base_item.show_dropdown = false;
                        set.show_dropdown = false;
                        legendary.show_dropdown = false;
                        type.show_dropdown = false;
                        rarity.show_dropdown = false;
                        unique.show_dropdown = !unique.show_dropdown;
                    }
                    if ((unique.dropdown_index < unique.dropdown_list.Length) && (unique.dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), unique.dropdown_list[unique.dropdown_index], Styles.DropdownLabelMidle_Style());
                        affixs.enable = false;
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Unique Item", Styles.DropdownLabelMidle_Style());
                        quantity.enable = false;
                        drop.enable = false;
                    }
                    pos_y += 35;
                }
                else { unique.show_dropdown = false; }

                return pos_y;
            }
            public static int GetIdFromName(string name)
            {
                bool type_error = false;
                int result = 0;
                try { if (UniqueList.get() == null) { type_error = true; } }
                catch { }
                if (!type_error)
                {
                    foreach (UniqueList.Entry item in UniqueList.get().uniques)
                    {
                        if (item.name == name) { result = item.uniqueID; break; }
                    }
                }

                return result;
            }
            public static int GetBaseIdFromName(string name)
            {
                bool type_error = false;
                int result = 0;
                try { if (UniqueList.get() == null) { type_error = true; } }
                catch { }
                if (!type_error)
                {
                    foreach (UniqueList.Entry item in UniqueList.get().uniques)
                    {
                        if (item.name == name) { result = item.subTypes[0]; break; }
                    }
                }

                return result;
            }
            public static void UpdateList(string type_name)
            {
                bool type_error = false;
                try { if (ItemList.get() == null) { type_error = true; } }
                catch { }
                if (!type_error)
                {
                    int base_id = 0;
                    bool found = false;
                    System.Collections.Generic.List<string> base_names = new System.Collections.Generic.List<string>();
                    foreach (ItemList.BaseEquipmentItem item in ItemList.get().EquippableItems)
                    {
                        if (type_name == item.displayName)
                        {
                            found = true;
                            base_id = item.baseTypeID;
                            break;
                        }
                    }
                    if (!found)
                    {
                        foreach (ItemList.BaseNonEquipmentItem item in ItemList.get().nonEquippableItems)
                        {
                            if (type_name == item.displayName)
                            {
                                found = true;
                                base_id = item.baseTypeID;
                                break;
                            }
                        }
                    }
                    if (found)
                    {
                        unique.dropdown_list = null;
                        set.dropdown_list = null;
                        legendary.dropdown_list = null;
                        int items_count = 0;
                        try { items_count = UniqueList.get().uniques.Count; }
                        catch { }
                        if (items_count > 0)
                        {
                            System.Collections.Generic.List<string> unique_items = new System.Collections.Generic.List<string>();
                            System.Collections.Generic.List<string> set_items = new System.Collections.Generic.List<string>();
                            System.Collections.Generic.List<string> legendary_items = new System.Collections.Generic.List<string>();
                            foreach (UniqueList.Entry item in UniqueList.get().uniques)
                            {
                                if (item.baseType == base_id)
                                {
                                    legendary_items.Add(item.name);
                                    if (item.isSetItem) { set_items.Add(item.name); }
                                    else { unique_items.Add(item.name); }
                                }
                            }
                            unique.dropdown_list = new string[unique_items.Count];
                            for (int i = 0; i < unique.dropdown_list.Length; i++) { unique.dropdown_list[i] = unique_items[i]; }
                            set.dropdown_list = new string[set_items.Count];
                            for (int i = 0; i < set.dropdown_list.Length; i++) { set.dropdown_list[i] = set_items[i]; }
                            legendary.dropdown_list = new string[legendary_items.Count];
                            for (int i = 0; i < legendary.dropdown_list.Length; i++) { legendary.dropdown_list[i] = legendary_items[i]; }
                        }
                    }
                    else { Main.logger_instance.Msg("Error Type Not Found : " + type_name); }
                }
                else { Main.logger_instance.Msg("Error ItemList is null"); }
            }
        }
        public class set
        {
            public static bool enable = false;
            public static bool show_dropdown = false;
            public static string[] dropdown_list = null;
            public static int dropdown_index = -1;
            public static float pos_x;
            public static float pos_y;
            public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
            public static Vector2 dropdown_scrollview = Vector2.zero;
            public static int h = 35;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (set.enable)
                {
                    set.pos_y = pos_y;
                    if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                    {
                        base_item.show_dropdown = false;
                        unique.show_dropdown = false;
                        legendary.show_dropdown = false;
                        type.show_dropdown = false;
                        rarity.show_dropdown = false;
                        set.show_dropdown = !set.show_dropdown;
                    }
                    if ((set.dropdown_index < set.dropdown_list.Length) && (set.dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), set.dropdown_list[set.dropdown_index], Styles.DropdownLabelMidle_Style());
                        affixs.enable = false;
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Set Item", Styles.DropdownLabelMidle_Style());
                        quantity.enable = false;
                        drop.enable = false;
                    }
                    pos_y += 35;
                }
                else { set.show_dropdown = false; }

                return pos_y;
            }
        }
        public class legendary
        {
            public static bool enable = false;
            public static bool show_dropdown = false;
            public static string[] dropdown_list = null;
            public static int dropdown_index = -1;
            public static float pos_x;
            public static float pos_y;
            public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
            public static Vector2 dropdown_scrollview = Vector2.zero;
            public static int h = 35;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (legendary.enable)
                {
                    legendary.pos_y = pos_y;
                    if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                    {
                        base_item.show_dropdown = false;
                        unique.show_dropdown = false;
                        set.show_dropdown = false;
                        type.show_dropdown = false;
                        rarity.show_dropdown = false;
                        legendary.show_dropdown = !legendary.show_dropdown;
                    }
                    if ((legendary.dropdown_index < legendary.dropdown_list.Length) && (legendary.dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), legendary.dropdown_list[legendary.dropdown_index], Styles.DropdownLabelMidle_Style());
                        affixs.enable = true;
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Legendary Item", Styles.DropdownLabelMidle_Style());
                        affixs.enable = false;
                        quantity.enable = false;
                        drop.enable = false;
                    }
                    pos_y += 35;
                }
                else { legendary.show_dropdown = false; }

                return pos_y;
            }
        }
        public class affixs
        {
            public static bool enable = false;
            public static int nb_affixs = 4;
            public static int h = 50;
            public static int affix_h = 35;
            public class prefixs
            {
                public static string[] dropdown_list = null;
                public class slot_0
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_1
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
            }
            public class suffixs
            {
                public static string[] dropdown_list = null;
                public class slot_0
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_1
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
            }
            public class idols
            {
                public static string[] dropdown_list = null;
                public class slot_0
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_1
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_2
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_3
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
            }

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (affixs.enable)
                {
                    int type_id = type.GetIdFromName(type.dropdown_list[type.dropdown_index]);
                    if (type_id > 33) { affixs.enable = false; }
                    else
                    {
                        bool idol = false;
                        if ((type_id > 24) && (type_id < 34)) { idol = true; }
                        GUI.TextField(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Nb affixes", Styles.TextField_Style());
                        pos_y += 25;
                        float temp_value = affixs.nb_affixs;
                        temp_value = GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), temp_value, 0f, 4f);
                        string value_str = GUI.TextArea(new Rect((pos_x + (w - 60)), (pos_y - 25), 60, 20), temp_value.ToString(), Styles.TextArea_Style());
                        try { temp_value = float.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat); }
                        catch { }
                        affixs.nb_affixs = System.Convert.ToInt32(temp_value);
                        pos_y += 25;
                        if (affixs.nb_affixs > 0) //prefix_0
                        {
                            affixs.prefixs.slot_0.pos_y = pos_y;
                            if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                            {
                                affixs.prefixs.slot_1.show_dropdown = false;
                                affixs.suffixs.slot_0.show_dropdown = false;
                                affixs.suffixs.slot_1.show_dropdown = false;
                                affixs.idols.slot_1.show_dropdown = false;
                                affixs.idols.slot_2.show_dropdown = false;
                                affixs.idols.slot_3.show_dropdown = false;
                                if (!idol)
                                {
                                    affixs.idols.slot_0.show_dropdown = false;
                                    affixs.prefixs.slot_0.show_dropdown = !affixs.prefixs.slot_0.show_dropdown;
                                }
                                else
                                {
                                    affixs.prefixs.slot_0.show_dropdown = false;                                    
                                    affixs.idols.slot_0.show_dropdown = !affixs.idols.slot_0.show_dropdown;
                                }
                            }
                            if (!idol)
                            {
                                if ((affixs.prefixs.slot_0.dropdown_index < affixs.prefixs.dropdown_list.Length) && (affixs.prefixs.slot_0.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), affixs.prefixs.dropdown_list[affixs.prefixs.slot_0.dropdown_index], Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Prefix 0", Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((affixs.idols.slot_0.dropdown_index < affixs.idols.dropdown_list.Length) && (affixs.idols.slot_0.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), affixs.idols.dropdown_list[affixs.idols.slot_0.dropdown_index], Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Idol Prefix 0", Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += 35;
                        }
                        if (affixs.nb_affixs > 1) //suffix_0
                        {
                            affixs.suffixs.slot_0.pos_y = pos_y;
                            if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                            {
                                affixs.prefixs.slot_1.show_dropdown = false;
                                affixs.prefixs.slot_0.show_dropdown = false;
                                affixs.suffixs.slot_1.show_dropdown = false;
                                affixs.idols.slot_0.show_dropdown = false;
                                affixs.idols.slot_2.show_dropdown = false;
                                affixs.idols.slot_3.show_dropdown = false;
                                if (!idol)
                                {
                                    affixs.idols.slot_1.show_dropdown = false;
                                    affixs.suffixs.slot_0.show_dropdown = !affixs.suffixs.slot_0.show_dropdown;
                                }
                                else
                                {
                                    affixs.suffixs.slot_0.show_dropdown = false;
                                    affixs.idols.slot_1.show_dropdown = !affixs.idols.slot_1.show_dropdown;
                                }
                            }
                            if (!idol)
                            {
                                if ((affixs.suffixs.slot_0.dropdown_index < affixs.suffixs.dropdown_list.Length) && (affixs.suffixs.slot_0.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), affixs.suffixs.dropdown_list[affixs.suffixs.slot_0.dropdown_index], Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Suffix 0", Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((affixs.idols.slot_1.dropdown_index < affixs.idols.dropdown_list.Length) && (affixs.idols.slot_1.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), affixs.idols.dropdown_list[affixs.idols.slot_1.dropdown_index], Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Idol Suffix 0", Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += 35;
                        }
                        if (affixs.nb_affixs > 2) //prefix_1
                        {
                            affixs.prefixs.slot_1.pos_y = pos_y;
                            if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                            {
                                affixs.prefixs.slot_1.show_dropdown = false;
                                affixs.suffixs.slot_0.show_dropdown = false;
                                affixs.suffixs.slot_1.show_dropdown = false;
                                affixs.idols.slot_0.show_dropdown = false;
                                affixs.idols.slot_1.show_dropdown = false;
                                affixs.idols.slot_3.show_dropdown = false;
                                if (!idol)
                                {
                                    affixs.idols.slot_2.show_dropdown = false;
                                    affixs.prefixs.slot_1.show_dropdown = !affixs.prefixs.slot_1.show_dropdown;
                                }
                                else
                                {
                                    affixs.prefixs.slot_1.show_dropdown = false;
                                    affixs.idols.slot_2.show_dropdown = !affixs.idols.slot_2.show_dropdown;
                                }
                            }
                            if (!idol)
                            {
                                if ((affixs.prefixs.slot_1.dropdown_index < affixs.prefixs.dropdown_list.Length) && (affixs.prefixs.slot_1.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), affixs.prefixs.dropdown_list[affixs.prefixs.slot_1.dropdown_index], Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Prefix 1", Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((affixs.idols.slot_2.dropdown_index < affixs.idols.dropdown_list.Length) && (affixs.idols.slot_2.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), affixs.idols.dropdown_list[affixs.idols.slot_2.dropdown_index], Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Idol Prefix 1", Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += 35;
                        }
                        if (affixs.nb_affixs > 3) //sufix_1
                        {
                            affixs.suffixs.slot_1.pos_y = pos_y;
                            if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                            {
                                affixs.prefixs.slot_1.show_dropdown = false;
                                affixs.prefixs.slot_0.show_dropdown = false;
                                affixs.suffixs.slot_0.show_dropdown = false;
                                affixs.idols.slot_0.show_dropdown = false;
                                affixs.idols.slot_1.show_dropdown = false;
                                affixs.idols.slot_2.show_dropdown = false;
                                if (!idol)
                                {
                                    affixs.idols.slot_3.show_dropdown = false;
                                    affixs.suffixs.slot_1.show_dropdown = !affixs.suffixs.slot_1.show_dropdown;
                                }
                                else
                                {
                                    affixs.suffixs.slot_1.show_dropdown = false;
                                    affixs.idols.slot_3.show_dropdown = !affixs.idols.slot_3.show_dropdown;
                                }
                            }
                            if (!idol)
                            {
                                if ((affixs.suffixs.slot_1.dropdown_index < affixs.suffixs.dropdown_list.Length) && (affixs.suffixs.slot_1.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), affixs.suffixs.dropdown_list[affixs.suffixs.slot_1.dropdown_index], Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Suffix 1", Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((affixs.idols.slot_3.dropdown_index < affixs.idols.dropdown_list.Length) && (affixs.idols.slot_3.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), affixs.idols.dropdown_list[affixs.idols.slot_3.dropdown_index], Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Idol Suffix 1", Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += 35;
                        }
                    }
                }
                else
                {
                    affixs.prefixs.slot_0.show_dropdown = false;
                    affixs.prefixs.slot_1.show_dropdown = false;
                    affixs.suffixs.slot_0.show_dropdown = false;
                    affixs.suffixs.slot_1.show_dropdown = false;
                    affixs.idols.slot_0.show_dropdown = false;
                    affixs.idols.slot_1.show_dropdown = false;
                    affixs.idols.slot_2.show_dropdown = false;
                    affixs.idols.slot_3.show_dropdown = false;
                }

                return pos_y;
            }
            public static void InitList()
            {
                affixs.prefixs.dropdown_list = null;
                affixs.suffixs.dropdown_list = null;
                affixs.idols.dropdown_list = null;

                System.Collections.Generic.List<string> prefix_names = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<string> suffix_names = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<string> idol_names = new System.Collections.Generic.List<string>();
                bool affix_error = false;
                try { if (AffixList.instance == null) { affix_error = true; } }
                catch { }
                if (!affix_error)
                {
                    foreach (AffixList.SingleAffix affix in AffixList.instance.singleAffixes)
                    {
                        if (affix.rollsOn == RollsOn.Idols) { idol_names.Add(affix.affixName); }
                        else if (affix.type == AffixType.SUFFIX) { suffix_names.Add(affix.affixName); }
                        else { prefix_names.Add(affix.affixName); }
                    }
                    foreach (AffixList.MultiAffix affix in AffixList.instance.multiAffixes)
                    {
                        if (affix.rollsOn == RollsOn.Idols) { idol_names.Add(affix.affixName); }
                        else if (affix.type == AffixType.SUFFIX) { suffix_names.Add(affix.affixName); }
                        else { prefix_names.Add(affix.affixName); }
                    }
                    prefix_names.Sort();
                    affixs.prefixs.dropdown_list = new string[prefix_names.Count];
                    int j = 0;                    
                    foreach (string name in prefix_names) { affixs.prefixs.dropdown_list[j] = name; j++; }

                    suffix_names.Sort();
                    affixs.suffixs.dropdown_list = new string[suffix_names.Count];
                    j = 0;
                    foreach (string name in suffix_names) { affixs.suffixs.dropdown_list[j] = name; j++; }

                    idol_names.Sort();
                    affixs.idols.dropdown_list = new string[idol_names.Count];
                    j = 0;
                    foreach (string name in idol_names) { affixs.idols.dropdown_list[j] = name; j++; }
                }
            }
            public static void Hide()
            {
                affixs.prefixs.slot_0.show_dropdown = false;
                affixs.prefixs.slot_1.show_dropdown = false;
                affixs.suffixs.slot_0.show_dropdown = false;
                affixs.suffixs.slot_1.show_dropdown = false;
                affixs.idols.slot_0.show_dropdown = false;
                affixs.idols.slot_1.show_dropdown = false;
                affixs.idols.slot_2.show_dropdown = false;
                affixs.idols.slot_3.show_dropdown = false;

            }
            public static void Init()
            {
                affixs.prefixs.slot_0.dropdown_index = -1;
                affixs.prefixs.slot_1.dropdown_index = -1;
                affixs.suffixs.slot_0.dropdown_index = -1;
                affixs.suffixs.slot_1.dropdown_index = -1;
                affixs.idols.slot_0.dropdown_index = -1;
                affixs.idols.slot_1.dropdown_index = -1;
                affixs.idols.slot_2.dropdown_index = -1;
                affixs.idols.slot_3.dropdown_index = -1;
            }
            public static int GetIdFromName(string name)
            {
                int result = -1;

                bool affix_error = false;
                try { if (AffixList.instance == null) { affix_error = true; } }
                catch { }
                if (!affix_error)
                {
                    bool found = false;
                    foreach (AffixList.SingleAffix affix in AffixList.instance.singleAffixes)
                    {
                        if (name == affix.affixName) { result = affix.affixId; found = true; break; }
                    }
                    if (!found)
                    {
                        foreach (AffixList.MultiAffix affix in AffixList.instance.multiAffixes)
                        {
                            if (name == affix.affixName) { result = affix.affixId; break; }
                        }
                    }
                }

                return result;
            }
            public static string GetNameFromId(int id)
            {
                string result = "";
                bool affix_error = false;
                try { if (AffixList.instance == null) { affix_error = true; } }
                catch { }
                if (!affix_error)
                {
                    bool found = false;
                    foreach (AffixList.SingleAffix affix in AffixList.instance.singleAffixes)
                    {
                        if (id == affix.affixId) { result = affix.affixName; found = true; break; }
                    }
                    if (!found)
                    {
                        foreach (AffixList.MultiAffix affix in AffixList.instance.multiAffixes)
                        {
                            if (id == affix.affixId) { result = affix.affixName; break; }
                        }
                    }
                }

                return result;
            }
            public static System.Collections.Generic.List<int> GetIdList(bool idol)
            {
                System.Collections.Generic.List<int> affixs_ids = new System.Collections.Generic.List<int>();
                if (!idol)
                {
                    if (nb_affixs > 0)
                    {
                        if ((Ui.GenerateItem.affixs.prefixs.slot_0.dropdown_index > -1) && (Ui.GenerateItem.affixs.prefixs.slot_0.dropdown_index < Ui.GenerateItem.affixs.prefixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.GenerateItem.affixs.GetIdFromName(Ui.GenerateItem.affixs.prefixs.dropdown_list[Ui.GenerateItem.affixs.prefixs.slot_0.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1);}
                    }
                    if (nb_affixs > 1)
                    {
                        if ((Ui.GenerateItem.affixs.suffixs.slot_0.dropdown_index > -1) && (Ui.GenerateItem.affixs.suffixs.slot_0.dropdown_index < Ui.GenerateItem.affixs.suffixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.GenerateItem.affixs.GetIdFromName(Ui.GenerateItem.affixs.suffixs.dropdown_list[Ui.GenerateItem.affixs.suffixs.slot_0.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 2)
                    {
                        if ((Ui.GenerateItem.affixs.prefixs.slot_1.dropdown_index > -1) && (Ui.GenerateItem.affixs.prefixs.slot_1.dropdown_index < Ui.GenerateItem.affixs.prefixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.GenerateItem.affixs.GetIdFromName(Ui.GenerateItem.affixs.prefixs.dropdown_list[Ui.GenerateItem.affixs.prefixs.slot_1.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 3)
                    {
                        if ((Ui.GenerateItem.affixs.suffixs.slot_1.dropdown_index > -1) && (Ui.GenerateItem.affixs.suffixs.slot_1.dropdown_index < Ui.GenerateItem.affixs.suffixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.GenerateItem.affixs.GetIdFromName(Ui.GenerateItem.affixs.suffixs.dropdown_list[Ui.GenerateItem.affixs.suffixs.slot_1.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                }
                else
                {
                    if (nb_affixs > 0)
                    {
                        if ((Ui.GenerateItem.affixs.idols.slot_0.dropdown_index > -1) && (Ui.GenerateItem.affixs.idols.slot_0.dropdown_index < Ui.GenerateItem.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.GenerateItem.affixs.GetIdFromName(Ui.GenerateItem.affixs.idols.dropdown_list[Ui.GenerateItem.affixs.idols.slot_0.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 1)
                    {
                        if ((Ui.GenerateItem.affixs.idols.slot_1.dropdown_index > -1) && (Ui.GenerateItem.affixs.idols.slot_1.dropdown_index < Ui.GenerateItem.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.GenerateItem.affixs.GetIdFromName(Ui.GenerateItem.affixs.idols.dropdown_list[Ui.GenerateItem.affixs.idols.slot_1.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 2)
                    {
                        if ((Ui.GenerateItem.affixs.idols.slot_2.dropdown_index > -1) && (Ui.GenerateItem.affixs.idols.slot_2.dropdown_index < Ui.GenerateItem.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.GenerateItem.affixs.GetIdFromName(Ui.GenerateItem.affixs.idols.dropdown_list[Ui.GenerateItem.affixs.idols.slot_2.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 3)
                    {
                        if ((Ui.GenerateItem.affixs.idols.slot_3.dropdown_index > -1) && (Ui.GenerateItem.affixs.idols.slot_3.dropdown_index < Ui.GenerateItem.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.GenerateItem.affixs.GetIdFromName(Ui.GenerateItem.affixs.idols.dropdown_list[Ui.GenerateItem.affixs.idols.slot_3.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                }
                
                return affixs_ids;
            }
        }
        public class quantity
        {
            public static bool enable = false;
            public static int value = 1;
            public static int h = 50;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (quantity.enable)
                {
                    GUI.TextField(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Quantity", Styles.TextField_Style());
                    float temp_value = quantity.value;
                    string value_str = GUI.TextArea(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_value.ToString(), Styles.TextArea_Style());
                    try
                    {
                        int str = int.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat);
                        temp_value = str;
                    }
                    catch { }
                    pos_y += 25;
                    temp_value = GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), temp_value, 1, 99);
                    quantity.value = (int)temp_value;
                    pos_y += 25;
                }

                return pos_y;
            }
            public static bool EnableFromType(string type_name)
            {
                bool result = false;
                if ((type_name == "Affix Shard") || (type_name == "Blessing") ||
                    (type_name == "Rune") || (type_name == "Glyph") ||
                    (type_name == "Key") || (type_name == "Lost Memory"))
                {
                    result = true;
                }

                return result;
            }
        }
        public class drop
        {
            public static bool enable = false;
            public static bool isIdol = false;
            public static int affix_index = -1;
            public static bool generating_random_item = false;
            public static int h = 25;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (drop.enable)
                {
                    if (GUI.Button(new Rect(pos_x, pos_y, w, 20), "Drop", Styles.Button_Style(true)))
                    {
                        drop.ForceDrop();
                    }
                }

                return pos_y;
            }
            public static bool EnableFromType(string type_name)
            {
                bool result = false;
                if ((type_name == "Affix Shard") || (type_name == "Blessing") ||
                    (type_name == "Rune") || (type_name == "Glyph") || (type_name == "Key"))
                {
                    result = true;
                }
                else
                {
                    //Other Items
                }

                return result;
            }
            public static void ForceDrop()
            {
                int item_type = type.GetIdFromName(type.dropdown_list[type.dropdown_index]);
                if ((item_type > 24) && (item_type < 34)) { isIdol = true; }
                else { isIdol = false; }
                int item_base_id = 0;
                int item_rarity = 0;
                int unique_id = 0;
                int item_sockets = 0;
                if (rarity.dropdown_index == 0)
                {
                    item_base_id = base_item.GetIdFromName(type.dropdown_list[type.dropdown_index], base_item.dropdown_list[base_item.dropdown_index]);
                    item_sockets = affixs.nb_affixs;
                    item_rarity = item_sockets;
                }
                else if (rarity.dropdown_index == 1)
                {
                    item_rarity = 7;
                    item_base_id = unique.GetBaseIdFromName(unique.dropdown_list[unique.dropdown_index]);
                    unique_id = unique.GetIdFromName(unique.dropdown_list[unique.dropdown_index]);
                }
                else if (rarity.dropdown_index == 2)
                {
                    item_rarity = 8;
                    item_base_id = unique.GetBaseIdFromName(set.dropdown_list[set.dropdown_index]);
                    unique_id = unique.GetIdFromName(set.dropdown_list[set.dropdown_index]);
                }
                else if (rarity.dropdown_index == 3)
                {
                    item_rarity = 9;
                    item_sockets = affixs.nb_affixs;
                    item_base_id = unique.GetBaseIdFromName(legendary.dropdown_list[legendary.dropdown_index]);
                    unique_id = unique.GetIdFromName(legendary.dropdown_list[legendary.dropdown_index]);                    
                }
                else
                {
                    item_base_id = base_item.GetIdFromName(type.dropdown_list[type.dropdown_index], base_item.dropdown_list[base_item.dropdown_index]);
                    item_rarity = 0;
                }

                //Main.logger_instance.Msg("Generate Item : type_id = " + item_type + ", base_id = " + item_base_id +
                //    ", rarity = " + item_rarity + ", unique_id = " + unique_id + ", sockets = " + item_sockets);

                
                bool rarity_was_checked = false;                
                GroundItemManager ground_item_manager = null;
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(GroundItemManager)))
                {
                    ground_item_manager = obj.TryCast<GroundItemManager>();
                    break;
                }
                if (ground_item_manager != null)
                {
                    Actor player = null;
                    try { player = PlayerFinder.getPlayerActor(); }
                    catch { }
                    if (player != null)
                    {
                        if (quantity.value < 1) { quantity.value = 1; }
                        generating_random_item = true;
                        affix_index = 0;
                        ItemDataUnpacked item = player.generateItems.initialiseRandomItemData(false,
                            100, false, ItemLocationTag.None, item_type, item_base_id, item_rarity,
                            item_sockets, unique_id, false, 0);                        
                        if (item_rarity == 9)
                        {
                            bool a = false;
                            player.generateItems.RollAffixes(ref item, 100, true, false, out a);                            
                            player.generateItems.GenerateAffixes(ref item, 100, true, GenerateItems.VendorType.None, false);
                            item.ReRollAffixRolls(false);
                        }
                        generating_random_item = false;
                        if (Config.Data.mods_config.items.Enable_Rarity)
                        {
                            rarity_was_checked = true;
                            Config.Data.mods_config.items.Enable_Rarity = false;
                        }
                        for (int i = 0; i < quantity.value; i++)
                        {
                            ground_item_manager.dropItemForPlayer(player, item.TryCast<ItemData>(), player.position(), false);
                        }
                    }
                    else { Main.logger_instance.Error("Player Not Found"); }
                }
                else { Main.logger_instance.Error("Ground Item Manager Not Found"); }
                if (rarity_was_checked) { Config.Data.mods_config.items.Enable_Rarity = true; }
            }
        }
    }
}
