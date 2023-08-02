using LastEpochMods.Hooks;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace LastEpochMods.Ui
{
    public class ForceDrop
    {
        public class main
        {
            public static int size_w = 200;
            public static void UI(float pos_x, float pos_y)
            {
                int h = GetSizeH();
                int w = size_w - 10;
                dropdown.InitScrollViewPositions(pos_x);
                GUI.DrawTexture(new Rect(pos_x, pos_y, size_w, h), Menu.texture_grey);
                pos_x += 5;
                pos_y += 5;

                pos_y = type.UI(pos_x, pos_y, w);
                pos_y = rarity.UI(pos_x, pos_y, w);
                pos_y = base_item.UI(pos_x, pos_y, w);
                pos_y = unique.UI(pos_x, pos_y, w);
                pos_y = set.UI(pos_x, pos_y, w);
                pos_y = legendary.UI(pos_x, pos_y, w);                
                pos_y = implicits.UI(pos_x, pos_y, w);
                pos_y = forgin_potencial.UI(pos_x, pos_y, w);
                pos_y = affixs.MenuUI(pos_x, pos_y, w);
                pos_y = unique_mods.UI(pos_x, pos_y, w);
                pos_y = legenday_potencial.UI(pos_x, pos_y, w);
                pos_y = weaver_wil.UI(pos_x, pos_y, w);
                pos_y = quantity.UI(pos_x, pos_y, w);
                pos_y = drop.UI(pos_x, pos_y, w);
            }
            public static void Reset()
            {
                type.dropdown_index_backup = 99;
                type.dropdown_index = -1;                
                rarity.dropdown_index = -1;
                base_item.dropdown_index -= 1;
                unique.dropdown_index -= 1;
                set.dropdown_index -= 1;
                legendary.dropdown_index -= 1;
                affixs.seal.dropdown_index = -1;
                affixs.prefixs.slot_0.dropdown_index = -1;
                affixs.prefixs.slot_1.dropdown_index = -1;
                affixs.suffixs.slot_0.dropdown_index -= 1;
                affixs.suffixs.slot_1.dropdown_index -= 1;
                affixs.idols.slot_0.dropdown_index += 1;
                affixs.idols.slot_1.dropdown_index += 1;
                affixs.idols.slot_2.dropdown_index += 1;
                affixs.idols.slot_3.dropdown_index += 1;
            }
            public static int GetSizeH()
            {
                int result = 5 + type.h;
                if (rarity.enable) { result += rarity.h; }
                if (base_item.enable) { result += base_item.h; }
                if (unique.enable) { result += unique.h; }
                if (set.enable) { result += set.h; }
                if (legendary.enable) { result += legendary.h; }
                if (implicits.enable) { result += implicits.h; }
                if (forgin_potencial.enable) { result += forgin_potencial.h; }
                if (affixs.enable) { result += affixs.h; }
                if (unique_mods.enable) { result += unique_mods.h; }
                if (legenday_potencial.enable) { result += legenday_potencial.h; }
                if (weaver_wil.enable) { result += weaver_wil.h; }
                if (quantity.enable) { result += quantity.h; }
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

                    Dropdown(affixs.seal.pos_x, affixs.seal.pos_y, affixs.dropdown_rect, ref affixs.seal.dropdown_scrollview, ref affixs.seal.dropdown_index, ref affixs.seal.show_dropdown, affixs.seal.dropdown_list);
                    Dropdown(affixs.seal.pos_x, affixs.seal.pos_y, affixs.dropdown_rect, ref affixs.seal.idol_dropdown_scrollview, ref affixs.seal.idol_dropdown_index, ref affixs.seal.idol_show_dropdown, affixs.seal.idol_dropdown_list);

                    Dropdown(affixs.prefixs.slot_0.pos_x, affixs.prefixs.slot_0.pos_y, affixs.dropdown_rect, ref affixs.prefixs.slot_0.dropdown_scrollview, ref affixs.prefixs.slot_0.dropdown_index, ref affixs.prefixs.slot_0.show_dropdown, affixs.prefixs.dropdown_list);
                    Dropdown(affixs.prefixs.slot_1.pos_x, affixs.prefixs.slot_1.pos_y, affixs.dropdown_rect, ref affixs.prefixs.slot_1.dropdown_scrollview, ref affixs.prefixs.slot_1.dropdown_index, ref affixs.prefixs.slot_1.show_dropdown, affixs.prefixs.dropdown_list);
                    Dropdown(affixs.suffixs.slot_0.pos_x, affixs.suffixs.slot_0.pos_y, affixs.dropdown_rect, ref affixs.suffixs.slot_0.dropdown_scrollview, ref affixs.suffixs.slot_0.dropdown_index, ref affixs.suffixs.slot_0.show_dropdown, affixs.suffixs.dropdown_list);
                    Dropdown(affixs.suffixs.slot_1.pos_x, affixs.suffixs.slot_1.pos_y, affixs.dropdown_rect, ref affixs.suffixs.slot_1.dropdown_scrollview, ref affixs.suffixs.slot_1.dropdown_index, ref affixs.suffixs.slot_1.show_dropdown, affixs.suffixs.dropdown_list);

                    Dropdown(affixs.idols.slot_0.pos_x, affixs.idols.slot_0.pos_y, affixs.dropdown_rect, ref affixs.idols.slot_0.dropdown_scrollview, ref affixs.idols.slot_0.dropdown_index, ref affixs.idols.slot_0.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_1.pos_x, affixs.idols.slot_1.pos_y, affixs.dropdown_rect, ref affixs.idols.slot_1.dropdown_scrollview, ref affixs.idols.slot_1.dropdown_index, ref affixs.idols.slot_1.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_2.pos_x, affixs.idols.slot_2.pos_y, affixs.dropdown_rect, ref affixs.idols.slot_2.dropdown_scrollview, ref affixs.idols.slot_2.dropdown_index, ref affixs.idols.slot_2.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_3.pos_x, affixs.idols.slot_3.pos_y, affixs.dropdown_rect, ref affixs.idols.slot_3.dropdown_scrollview, ref affixs.idols.slot_3.dropdown_index, ref affixs.idols.slot_3.show_dropdown, affixs.idols.dropdown_list);
                }
                public static void InitScrollViewPositions(float pos_x)
                {
                    type.pos_x = pos_x + size_w;
                    rarity.pos_x = pos_x + size_w;
                    base_item.pos_x = pos_x + size_w;
                    unique.pos_x = pos_x + size_w;
                    set.pos_x = pos_x + size_w;
                    legendary.pos_x = pos_x + size_w;
                }
                public static void Dropdown(float pos_x, float pos_y, Rect rect, ref Vector2 scrollview, ref int dropdown_index, ref bool show, string[] dropdown_list)
                {
                    int button_h = 40;
                    int margin = 2;
                    //pos_x += size_w;
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
            public static bool is_idol = false;
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
                        if ((type.GetItemType() > 24) && (type.GetItemType() < 34) && (rarity.dropdown_index < 3)) { is_idol = true; }
                        else { is_idol = false; }
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
                        implicits.enable = false;
                        forgin_potencial.enable = false;
                        legenday_potencial.enable = false;
                        weaver_wil.enable = false;
                    }
                }
                else
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Type", Styles.DropdownLabelMidle_Style());
                    rarity.enable = false;
                    quantity.enable = false;
                    base_item.enable = false;
                    affixs.enable = false;
                    unique.enable = false;
                    set.enable= false;
                    legendary.enable = false;
                    drop.enable = false;
                    implicits.enable = false;
                    forgin_potencial.enable = false;
                    legenday_potencial.enable = false;
                    weaver_wil.enable = false;
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
            public static int GetItemType()
            {
                int item_type = -1;
                if ((type.dropdown_list != null) && (type.dropdown_index > -1))
                {
                    if (type.dropdown_index < type.dropdown_list.Length) { item_type = type.GetIdFromName(type.dropdown_list[type.dropdown_index]); }
                }

                return item_type;
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
                    legenday_potencial.enable = false;
                    weaver_wil.enable = false;
                    unique_mods.enable = false;
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
                        if (type.is_idol) { affixs.max_nb_affixs = 2f; }
                        else { affixs.max_nb_affixs = 4f; }                        
                        implicits.enable = implicits.EnableFromType();
                        forgin_potencial.enable = forgin_potencial.EnableFromType();
                        affixs.enable = true;
                        quantity.enable = true;
                        drop.enable = true;                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Base Item", Styles.DropdownLabelMidle_Style());
                        implicits.enable = false;
                        forgin_potencial.enable = false;                        
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
                    forgin_potencial.enable = false;
                    affixs.enable = false;

                    if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                    {
                        base_item.show_dropdown = false;
                        set.show_dropdown = false;
                        legendary.show_dropdown = false;
                        type.show_dropdown = false;
                        rarity.show_dropdown = false;
                        show_dropdown = !show_dropdown;
                    }
                    if ((dropdown_index < dropdown_list.Length) && (dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), dropdown_list[dropdown_index], Styles.DropdownLabelMidle_Style());                        
                        implicits.enable = implicits.EnableFromType();                        
                        unique_mods.enable = unique_mods.EnableFromRarity();
                        legenday_potencial.enable = legenday_potencial.EnableFromRarity();
                        weaver_wil.enable = weaver_wil.EnableFromRarity();
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Unique Item", Styles.DropdownLabelMidle_Style());                        
                        implicits.enable = false;
                        unique_mods.enable = false;
                        legenday_potencial.enable = false;
                        weaver_wil.enable = false;
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
                    forgin_potencial.enable = false;
                    affixs.enable = false;
                    legenday_potencial.enable = false;
                    weaver_wil.enable = false;
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
                        implicits.enable = implicits.EnableFromType();
                        unique_mods.enable = unique_mods.EnableFromRarity();
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Set Item", Styles.DropdownLabelMidle_Style());
                        implicits.enable = false;
                        unique_mods.enable = false;
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
                    forgin_potencial.enable = false;
                    legenday_potencial.enable = false;
                    weaver_wil.enable = false;
                    if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                    {
                        base_item.show_dropdown = false;
                        unique.show_dropdown = false;
                        set.show_dropdown = false;
                        type.show_dropdown = false;
                        rarity.show_dropdown = false;
                        show_dropdown = !show_dropdown;
                    }
                    if ((dropdown_index < dropdown_list.Length) && (dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), dropdown_list[dropdown_index], Styles.DropdownLabelMidle_Style());
                        implicits.enable = implicits.EnableFromType();                        
                        unique_mods.enable = unique_mods.EnableFromRarity();                        
                        affixs.max_nb_affixs = 4f;                        
                        affixs.enable = true;
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Legendary Item", Styles.DropdownLabelMidle_Style());
                        implicits.enable = false;                        
                        affixs.enable = false;
                        quantity.enable = false;
                        drop.enable = false;
                    }
                    pos_y += 35;
                }
                else { show_dropdown = false; }

                return pos_y;
            }
        }
        public class implicits
        {
            public static bool enable = false;
            public static int value = 255;
            public static int h = 50;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (enable)
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Impicits", Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), value.ToString(), Styles.TextArea_Style());
                    pos_y += 25;
                    value = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value, 0, 255));
                    pos_y += 25;
                }

                return pos_y;
            }
            public static bool EnableFromType()
            {
                bool result = false;
                if (type.dropdown_index < 35) { result = true; }

                return result;
            }
        }
        public class affixs
        {
            public static bool enable = false;
            public static int nb_affixs = 4;
            public static float max_nb_affixs = 4f;
            public static int h = 70;
            public static Rect dropdown_rect = new Rect(125, 50, 400, 400);
            public static int affix_btn_size_h = 40;
            public static bool affix_show = false;
            public static int affix_show_h = 125;
            public static bool override_tier = true;
            public static int affix_tier = 7;
            public static bool override_value = true;
            public static int affix_values = 255;
            public static float size_w = 300;
            public class seal
            {
                public static string[] dropdown_list = null;                
                public static bool show_dropdown = false;
                public static int dropdown_index = -1;
                public static Vector2 dropdown_scrollview = Vector2.zero;

                public static string[] idol_dropdown_list = null;
                public static bool idol_show_dropdown = false;
                public static int idol_dropdown_index = -1;
                public static Vector2 idol_dropdown_scrollview = Vector2.zero;

                public static int h = 40;
                public static int seal_add_h = 130;
                public static float pos_x;
                public static float pos_y;
                public static float size_w = 300;
                public static bool show = false;
                public static bool add = false;
                public static int tier = 7;
                public static bool override_value = false;
                public static int values = 255;
                public static bool override_affix = false;

                public static void UI(float pos_x, float pos_y)
                {
                    int size_h = h;
                    if (seal.add)
                    {
                        size_h += seal_add_h;
                        if (seal.override_value) { size_h += 25; }
                        if (seal.override_affix) { size_h += affix_btn_size_h + 5; }
                    }
                    GUI.DrawTexture(new Rect(pos_x, pos_y, size_w + 20, size_h + 10), Menu.windowBackground);
                    pos_x += 5;
                    pos_y += 5;
                    GUI.DrawTexture(new Rect(pos_x, pos_y, size_w + 10, size_h), Menu.texture_grey);
                    pos_x += 5;
                    pos_y += 5;

                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 60, 30), "Add Seal", Styles.TextField_Style());
                    if (GUI.Button(new Rect((pos_x + size_w - 60), pos_y, 60, 30), "", Styles.Button_Style(seal.add))) { seal.add = !seal.add; }
                    string btn_str = "Enable";
                    if (seal.add) { btn_str = "Disable"; }
                    GUI.Label(new Rect((pos_x + size_w - 60 + 5), pos_y, 50, 30), btn_str, Styles.DropdownLabelMidle_Style());
                    pos_y += 35;
                    if (seal.add)
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 120, 30), "Tier", Styles.TextField_Style());
                        GUI.Label(new Rect((pos_x + size_w - 60), pos_y, 60, 30), seal.tier.ToString(), Styles.TextArea_Style());
                        pos_y += 35;
                        seal.tier = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), seal.tier, 1, 7));
                        pos_y += 25;
                        GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 120, 30), "Override Roll", Styles.TextField_Style());
                        if (seal.override_value)
                        {
                            float value = (seal.values * 100) / 255;
                            string value_str = value.ToString() + " %";
                            GUI.Label(new Rect((pos_x + size_w - 120), pos_y, 60, 30), value_str, Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect((pos_x + size_w - 60), pos_y, 60, 30), "", Styles.Button_Style(seal.override_value))) { seal.override_value = !seal.override_value; }
                        string btn_tier_value = "Enable";
                        if (seal.override_value) { btn_tier_value = "Disable"; }
                        GUI.Label(new Rect((pos_x + size_w - 60 + 5), pos_y, 50, 30), btn_tier_value, Styles.DropdownLabelMidle_Style());
                        pos_y += 35;
                        if (seal.override_value)
                        {
                            seal.values = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), seal.values, 0, 255));
                            pos_y += 25;
                        }
                        GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 60, 30), "Override Affix", Styles.TextField_Style());
                        if (GUI.Button(new Rect((pos_x + size_w - 60), pos_y, 60, 30), "", Styles.Button_Style(seal.override_affix))) { seal.override_affix = !seal.override_affix; }
                        string btn_affix = "Enable";
                        if (seal.override_affix) { btn_affix = "Disable"; }
                        GUI.Label(new Rect((pos_x + size_w - 60 + 5), pos_y, 50, 30), btn_affix, Styles.DropdownLabelMidle_Style());
                        pos_y += 35;
                        if (seal.override_affix)
                        {
                            seal.pos_x = pos_x + size_w + 5;
                            seal.pos_y = pos_y;
                            if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                            {
                                prefixs.slot_0.show_dropdown = false;
                                prefixs.slot_1.show_dropdown = false;
                                suffixs.slot_0.show_dropdown = false;
                                suffixs.slot_1.show_dropdown = false;
                                idols.slot_0.show_dropdown = false;
                                idols.slot_1.show_dropdown = false;
                                idols.slot_2.show_dropdown = false;
                                idols.slot_3.show_dropdown = false;
                                if (!type.is_idol)
                                {
                                    seal.idol_show_dropdown = false;
                                    seal.show_dropdown = !seal.show_dropdown;
                                }
                                else
                                {
                                    seal.show_dropdown = false;
                                    seal.idol_show_dropdown = !seal.idol_show_dropdown;
                                }
                            }
                            if (!type.is_idol)
                            {
                                if ((seal.dropdown_index < seal.dropdown_list.Length) && (seal.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), seal.dropdown_list[seal.dropdown_index], Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Choose a Prefix/Suffix", Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((seal.idol_dropdown_index < seal.idol_dropdown_list.Length) && (seal.idol_dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), seal.idol_dropdown_list[seal.idol_dropdown_index], Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Choose a Prefix/Suffix (Idol)", Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += affix_btn_size_h;
                        }
                        pos_y += 5;
                    }
                }                
            }
            public class prefixs
            {
                public static string[] dropdown_list = null;                
                public class slot_0
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_1
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
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
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_1
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
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
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_1
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_2
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_3
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
            }

            public static void UI(float pos_x, float pos_y)
            {
                InitScrollviewPositions(pos_x + size_w + 15);

                int size_h = affix_show_h + ((affix_btn_size_h + 5) * nb_affixs);   
                if (override_tier) { size_h += 25; }
                if (override_value) { size_h += 25; }

                GUI.DrawTexture(new Rect(pos_x, pos_y, size_w + 20, size_h + 10), Menu.windowBackground);
                pos_x += 5;
                pos_y += 5;
                GUI.DrawTexture(new Rect(pos_x, pos_y, size_w + 10, size_h), Menu.texture_grey);
                pos_x += 5;
                pos_y += 5;

                GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 65), 20), "Minimum Nb affixes", Styles.TextField_Style());
                if (nb_affixs < 0) { nb_affixs = 0; }
                if (nb_affixs > max_nb_affixs) { nb_affixs = (int)max_nb_affixs; }
                GUI.Label(new Rect((pos_x + (size_w - 60)), (pos_y), 60, 20), nb_affixs.ToString(), Styles.TextArea_Style());
                pos_y += 25;
                nb_affixs = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), nb_affixs, 0, max_nb_affixs));
                pos_y += 25;

                GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Styles.TextField_Style());
                if (override_tier)
                {
                    GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier.ToString(), Styles.TextArea_Style());
                }
                if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Styles.Button_Style(override_tier))) { override_tier = !override_tier; }
                string btn_affix_tier_label = "Eanble";
                if (override_tier) { btn_affix_tier_label = "Disable"; }
                GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Styles.DropdownLabelMidle_Style());
                pos_y += 35;
                if (override_tier)
                {
                    affix_tier = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier, 1, 7));
                    pos_y += 25;
                }

                GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Styles.TextField_Style());
                if (override_value)
                {
                    GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values.ToString(), Styles.TextArea_Style());
                }
                if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Styles.Button_Style(override_value))) { override_value = !override_value; }
                string btn_affix_values = "Enable";
                if (override_value) { btn_affix_values = "Disable"; }
                GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Styles.DropdownLabelMidle_Style());
                pos_y += 35;
                if (override_value)
                {
                    affix_values = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values, 0, 255));
                    pos_y += 25;
                }
                if (nb_affixs > 0) //prefix_0
                {
                    prefixs.slot_0.pos_y = pos_y + 5;
                    if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                    {
                        seal.show_dropdown = false;
                        prefixs.slot_1.show_dropdown = false;
                        suffixs.slot_0.show_dropdown = false;
                        suffixs.slot_1.show_dropdown = false;
                        idols.slot_1.show_dropdown = false;
                        idols.slot_2.show_dropdown = false;
                        idols.slot_3.show_dropdown = false;
                        if (!type.is_idol)
                        {
                            idols.slot_0.show_dropdown = false;
                            prefixs.slot_0.show_dropdown = !prefixs.slot_0.show_dropdown;
                        }
                        else
                        {
                            prefixs.slot_0.show_dropdown = false;
                            idols.slot_0.show_dropdown = !idols.slot_0.show_dropdown;
                        }
                    }
                    if (!type.is_idol)
                    {
                        if ((prefixs.slot_0.dropdown_index < prefixs.dropdown_list.Length) && (prefixs.slot_0.dropdown_index > -1))
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), prefixs.dropdown_list[prefixs.slot_0.dropdown_index], Styles.DropdownLabelMidle_Style());
                        }
                        else
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix", Styles.DropdownLabelMidle_Style());
                        }
                    }
                    else
                    {
                        if ((idols.slot_0.dropdown_index < idols.dropdown_list.Length) && (idols.slot_0.dropdown_index > -1))
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_0.dropdown_index], Styles.DropdownLabelMidle_Style());
                        }
                        else
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix (Idol)", Styles.DropdownLabelMidle_Style());
                        }
                    }
                    pos_y += affix_btn_size_h + 5;
                }
                if (nb_affixs > 1) //suffix_0
                {
                    suffixs.slot_0.pos_y = pos_y + 5;
                    if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                    {
                        seal.show_dropdown = false;
                        prefixs.slot_1.show_dropdown = false;
                        prefixs.slot_0.show_dropdown = false;
                        suffixs.slot_1.show_dropdown = false;
                        idols.slot_0.show_dropdown = false;
                        idols.slot_2.show_dropdown = false;
                        idols.slot_3.show_dropdown = false;
                        if (!type.is_idol)
                        {
                            idols.slot_1.show_dropdown = false;
                            suffixs.slot_0.show_dropdown = !suffixs.slot_0.show_dropdown;
                        }
                        else
                        {
                            suffixs.slot_0.show_dropdown = false;
                            idols.slot_1.show_dropdown = !idols.slot_1.show_dropdown;
                        }
                    }
                    if (!type.is_idol)
                    {
                        if ((suffixs.slot_0.dropdown_index < suffixs.dropdown_list.Length) && (suffixs.slot_0.dropdown_index > -1))
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), suffixs.dropdown_list[suffixs.slot_0.dropdown_index], Styles.DropdownLabelMidle_Style());
                        }
                        else
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Suffix", Styles.DropdownLabelMidle_Style());
                        }
                    }
                    else
                    {
                        if ((idols.slot_1.dropdown_index < idols.dropdown_list.Length) && (idols.slot_1.dropdown_index > -1))
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_1.dropdown_index], Styles.DropdownLabelMidle_Style());
                        }
                        else
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Suffix (Idol)", Styles.DropdownLabelMidle_Style());
                        }
                    }
                    pos_y += affix_btn_size_h + 5;
                }
                if (nb_affixs > 2) //prefix_1
                {
                    prefixs.slot_1.pos_y = pos_y + 5;
                    if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                    {
                        seal.show_dropdown = false;
                        prefixs.slot_1.show_dropdown = false;
                        suffixs.slot_0.show_dropdown = false;
                        suffixs.slot_1.show_dropdown = false;
                        idols.slot_0.show_dropdown = false;
                        idols.slot_1.show_dropdown = false;
                        idols.slot_3.show_dropdown = false;
                        if (!type.is_idol)
                        {
                            idols.slot_2.show_dropdown = false;
                            prefixs.slot_1.show_dropdown = !prefixs.slot_1.show_dropdown;
                        }
                        else
                        {
                            prefixs.slot_1.show_dropdown = false;
                            idols.slot_2.show_dropdown = !idols.slot_2.show_dropdown;
                        }
                    }
                    if (!type.is_idol)
                    {
                        if ((prefixs.slot_1.dropdown_index < prefixs.dropdown_list.Length) && (prefixs.slot_1.dropdown_index > -1))
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), prefixs.dropdown_list[prefixs.slot_1.dropdown_index], Styles.DropdownLabelMidle_Style());
                        }
                        else
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix", Styles.DropdownLabelMidle_Style());
                        }
                    }
                    else
                    {
                        if ((idols.slot_2.dropdown_index < idols.dropdown_list.Length) && (idols.slot_2.dropdown_index > -1))
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_2.dropdown_index], Styles.DropdownLabelMidle_Style());
                        }
                        else
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix (Idol)", Styles.DropdownLabelMidle_Style());
                        }
                    }
                    pos_y += affix_btn_size_h + 5;
                }
                if (nb_affixs > 3) //sufix_1
                {
                    suffixs.slot_1.pos_y = pos_y + 5;
                    if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                    {
                        seal.show_dropdown = false;
                        prefixs.slot_1.show_dropdown = false;
                        prefixs.slot_0.show_dropdown = false;
                        suffixs.slot_0.show_dropdown = false;
                        idols.slot_0.show_dropdown = false;
                        idols.slot_1.show_dropdown = false;
                        idols.slot_2.show_dropdown = false;
                        if (!type.is_idol)
                        {
                            idols.slot_3.show_dropdown = false;
                            suffixs.slot_1.show_dropdown = !suffixs.slot_1.show_dropdown;
                        }
                        else
                        {
                            suffixs.slot_1.show_dropdown = false;
                            idols.slot_3.show_dropdown = !idols.slot_3.show_dropdown;
                        }
                    }
                    if (!type.is_idol)
                    {
                        if ((suffixs.slot_1.dropdown_index < suffixs.dropdown_list.Length) && (suffixs.slot_1.dropdown_index > -1))
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), suffixs.dropdown_list[suffixs.slot_1.dropdown_index], Styles.DropdownLabelMidle_Style());
                        }
                        else
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Suffix", Styles.DropdownLabelMidle_Style());
                        }
                    }
                    else
                    {
                        if ((idols.slot_3.dropdown_index < idols.dropdown_list.Length) && (idols.slot_3.dropdown_index > -1))
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_3.dropdown_index], Styles.DropdownLabelMidle_Style());
                        }
                        else
                        {
                            GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Suffix (Idol)", Styles.DropdownLabelMidle_Style());
                        }
                    }
                    pos_y += affix_btn_size_h + 5;
                }

            }
            public static float MenuUI(float pos_x, float pos_y, int w)
            {
                if (affixs.enable)
                {
                    if (type.dropdown_index < type.dropdown_list.Length)
                    {
                        int type_id = type.GetIdFromName(type.dropdown_list[type.dropdown_index]);
                        if (type_id > 33) { affixs.enable = false; }
                        else
                        {
                            if (seal.show) { seal.UI((pos_x + w + 5), (pos_y - 10)); }
                            if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                            {
                                prefixs.slot_0.show_dropdown = false;
                                prefixs.slot_1.show_dropdown = false;
                                suffixs.slot_0.show_dropdown = false;
                                suffixs.slot_1.show_dropdown = false;
                                idols.slot_0.show_dropdown = false;
                                idols.slot_1.show_dropdown = false;
                                idols.slot_2.show_dropdown = false;
                                idols.slot_3.show_dropdown = false;
                                affix_show = false;
                                seal.show = !seal.show;
                            }
                            GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Seal", Styles.DropdownLabelMidle_Style());
                            pos_y += 35;

                            if (affix_show) { UI((pos_x + w + 5), (pos_y - 10)); }
                            if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                            {
                                seal.show_dropdown = false;
                                seal.show = false;
                                affix_show = !affix_show;
                            }
                            GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Affixs", Styles.DropdownLabelMidle_Style());
                            pos_y += 35;
                        }
                    }
                }
                else
                {
                    seal.show_dropdown = false;
                    prefixs.slot_0.show_dropdown = false;
                    prefixs.slot_1.show_dropdown = false;
                    suffixs.slot_0.show_dropdown = false;
                    suffixs.slot_1.show_dropdown = false;
                    idols.slot_0.show_dropdown = false;
                    idols.slot_1.show_dropdown = false;
                    idols.slot_2.show_dropdown = false;
                    idols.slot_3.show_dropdown = false;
                }

                return pos_y;
            }
            public static void InitList()
            {
                prefixs.dropdown_list = null;
                suffixs.dropdown_list = null;
                idols.dropdown_list = null;
                seal.dropdown_list = null;
                seal.idol_dropdown_list = null;

                System.Collections.Generic.List<string> prefix_names = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<string> suffix_names = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<string> idol_names = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<string> seal_names = new System.Collections.Generic.List<string>();
                bool affix_error = false;
                try { if (AffixList.instance == null) { affix_error = true; } }
                catch { }
                if (!affix_error)
                {
                    foreach (AffixList.SingleAffix affix in AffixList.instance.singleAffixes)
                    {
                        if (affix.rollsOn == AffixList.RollsOn.Idols) { idol_names.Add(affix.affixName); }
                        else
                        {
                            seal_names.Add(affix.affixName);
                            if (affix.type == AffixList.AffixType.SUFFIX) { suffix_names.Add(affix.affixName); }
                            else { prefix_names.Add(affix.affixName); }
                        }
                    }
                    foreach (AffixList.MultiAffix affix in AffixList.instance.multiAffixes)
                    {
                        if (affix.rollsOn == AffixList.RollsOn.Idols) { idol_names.Add(affix.affixName); }
                        else
                        {
                            seal_names.Add(affix.affixName);
                            if (affix.type == AffixList.AffixType.SUFFIX) { suffix_names.Add(affix.affixName); }
                            else { prefix_names.Add(affix.affixName); }
                        }
                    }
                    prefix_names.Sort();
                    prefixs.dropdown_list = new string[prefix_names.Count];
                    int j = 0;                    
                    foreach (string name in prefix_names) { prefixs.dropdown_list[j] = name; j++; }
                    suffix_names.Sort();
                    suffixs.dropdown_list = new string[suffix_names.Count];
                    j = 0;
                    foreach (string name in suffix_names) { suffixs.dropdown_list[j] = name; j++; }                                        
                    idol_names.Sort();
                    idols.dropdown_list = new string[idol_names.Count];
                    seal.idol_dropdown_list = new string[idol_names.Count];
                    j = 0;
                    foreach (string name in idol_names)
                    {
                        idols.dropdown_list[j] = name;
                        seal.idol_dropdown_list[j] = name;
                        j++;
                    }
                    seal_names.Sort();
                    seal.dropdown_list = new string[seal_names.Count];
                    j = 0;
                    foreach (string name in seal_names) { seal.dropdown_list[j] = name; j++; }
                }
            }
            public static void Hide()
            {
                seal.show_dropdown = false;
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
            public static void InitScrollviewPositions(float pos_x)
            {
                affixs.prefixs.slot_0.pos_x = pos_x;
                affixs.prefixs.slot_1.pos_x = pos_x;
                affixs.suffixs.slot_0.pos_x = pos_x;
                affixs.suffixs.slot_1.pos_x = pos_x;
                affixs.idols.slot_0.pos_x = pos_x;
                affixs.idols.slot_1.pos_x = pos_x;
                affixs.idols.slot_2.pos_x = pos_x;
                affixs.idols.slot_3.pos_x = pos_x;
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
                        if ((Ui.ForceDrop.affixs.prefixs.slot_0.dropdown_index > -1) && (Ui.ForceDrop.affixs.prefixs.slot_0.dropdown_index < Ui.ForceDrop.affixs.prefixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.ForceDrop.affixs.GetIdFromName(Ui.ForceDrop.affixs.prefixs.dropdown_list[Ui.ForceDrop.affixs.prefixs.slot_0.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1);}
                    }
                    if (nb_affixs > 1)
                    {
                        if ((Ui.ForceDrop.affixs.suffixs.slot_0.dropdown_index > -1) && (Ui.ForceDrop.affixs.suffixs.slot_0.dropdown_index < Ui.ForceDrop.affixs.suffixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.ForceDrop.affixs.GetIdFromName(Ui.ForceDrop.affixs.suffixs.dropdown_list[Ui.ForceDrop.affixs.suffixs.slot_0.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 2)
                    {
                        if ((Ui.ForceDrop.affixs.prefixs.slot_1.dropdown_index > -1) && (Ui.ForceDrop.affixs.prefixs.slot_1.dropdown_index < Ui.ForceDrop.affixs.prefixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.ForceDrop.affixs.GetIdFromName(Ui.ForceDrop.affixs.prefixs.dropdown_list[Ui.ForceDrop.affixs.prefixs.slot_1.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 3)
                    {
                        if ((Ui.ForceDrop.affixs.suffixs.slot_1.dropdown_index > -1) && (Ui.ForceDrop.affixs.suffixs.slot_1.dropdown_index < Ui.ForceDrop.affixs.suffixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.ForceDrop.affixs.GetIdFromName(Ui.ForceDrop.affixs.suffixs.dropdown_list[Ui.ForceDrop.affixs.suffixs.slot_1.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                }
                else
                {
                    if (nb_affixs > 0)
                    {
                        if ((Ui.ForceDrop.affixs.idols.slot_0.dropdown_index > -1) && (Ui.ForceDrop.affixs.idols.slot_0.dropdown_index < Ui.ForceDrop.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.ForceDrop.affixs.GetIdFromName(Ui.ForceDrop.affixs.idols.dropdown_list[Ui.ForceDrop.affixs.idols.slot_0.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 1)
                    {
                        if ((Ui.ForceDrop.affixs.idols.slot_1.dropdown_index > -1) && (Ui.ForceDrop.affixs.idols.slot_1.dropdown_index < Ui.ForceDrop.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.ForceDrop.affixs.GetIdFromName(Ui.ForceDrop.affixs.idols.dropdown_list[Ui.ForceDrop.affixs.idols.slot_1.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 2)
                    {
                        if ((Ui.ForceDrop.affixs.idols.slot_2.dropdown_index > -1) && (Ui.ForceDrop.affixs.idols.slot_2.dropdown_index < Ui.ForceDrop.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.ForceDrop.affixs.GetIdFromName(Ui.ForceDrop.affixs.idols.dropdown_list[Ui.ForceDrop.affixs.idols.slot_2.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 3)
                    {
                        if ((Ui.ForceDrop.affixs.idols.slot_3.dropdown_index > -1) && (Ui.ForceDrop.affixs.idols.slot_3.dropdown_index < Ui.ForceDrop.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(Ui.ForceDrop.affixs.GetIdFromName(Ui.ForceDrop.affixs.idols.dropdown_list[Ui.ForceDrop.affixs.idols.slot_3.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                }
                
                return affixs_ids;
            }
        }
        public class forgin_potencial
        {
            public static bool enable = false;
            public static int value = 255;
            public static int h = 50;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (enable)
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Forgin Potencial", Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), value.ToString(), Styles.TextArea_Style());
                    pos_y += 25;
                    value = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value, 0, 255));
                    pos_y += 25;
                }

                return pos_y;
            }
            public static bool EnableFromType()
            {
                bool result = false;
                if (type.dropdown_index < 35) { result = true; }

                return result;
            }
        }
        public class unique_mods
        {
            public static bool enable = false;
            public static int value = 255;
            public static int h = 50;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (enable)
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Mods", Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), value.ToString(), Styles.TextArea_Style());
                    pos_y += 25;
                    value = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value, 0, 255));
                    pos_y += 25;
                }

                return pos_y;
            }
            public static bool EnableFromRarity()
            {
                bool result = false;
                if (rarity.dropdown_index > 0) { result = true; }

                return result;
            }
        }
        public class legenday_potencial
        {
            public static bool enable = false;
            public static int value = 4;
            public static int h = 50;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (enable)
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Legendary Potencial", Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), value.ToString(), Styles.TextArea_Style());
                    pos_y += 25;
                    value = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value, 0, 4));
                    pos_y += 25;
                }

                return pos_y;
            }
            public static bool EnableFromRarity()
            {
                bool result = false;
                if (rarity.dropdown_index > 0) { result = true; }

                return result;
            }
        }
        public class weaver_wil
        {
            public static bool enable = false;
            public static int value = 28;
            public static int h = 50;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (enable)
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Weaver Will", Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), value.ToString(), Styles.TextArea_Style());
                    pos_y += 25;
                    value = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value, 0, 28));
                    pos_y += 25;
                }

                return pos_y;
            }
            public static bool EnableFromRarity()
            {
                bool result = false;
                if (rarity.dropdown_index > 0) { result = true; }

                return result;
            }
        }
        public class quantity
        {
            public static bool enable = false;
            public static int value = 1;
            public static int h = 50;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (enable)
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Quantity", Styles.TextField_Style());                    
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), value.ToString(), Styles.TextArea_Style());                    
                    pos_y += 25;
                    value = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value, 1, 99));
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
            public static bool generating_item = false;
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
                        if (quantity.value < 1) { quantity.value = 1; }
                        generating_item = true;
                        ItemDataUnpacked item = player.generateItems.initialiseRandomItemData(false,
                            100, false, ItemLocationTag.None, type.GetItemType(), item_base_id, item_rarity,
                            item_sockets, unique_id, false, 0);                        
                        if (item_rarity == 9)
                        {
                            bool a = false;
                            player.generateItems.RollAffixes(ref item, 100, true, false, out a);                           
                        }
                        //if (affixs.seal.enable) { item.AddRandomSealedAffix(affixs.seal.tier); }
                        item.RefreshIDAndValues();
                        generating_item = false;                      
                        for (int i = 0; i < quantity.value; i++)
                        {
                            ground_item_manager.dropItemForPlayer(player, item.TryCast<ItemData>(), player.position(), false);
                        }
                    }
                    else { Main.logger_instance.Error("Player Not Found"); }
                }
                else { Main.logger_instance.Error("Ground Item Manager Not Found"); }
            }
        }
    }
}
