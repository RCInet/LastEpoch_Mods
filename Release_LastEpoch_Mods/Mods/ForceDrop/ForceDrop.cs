using LastEpochMods.Managers;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using static LastEpochMods.Mods.ForceDrop.ForceDrop.affixs;

namespace LastEpochMods.Mods.ForceDrop
{
    public class ForceDrop
    {
        public static bool ShowDebug = false;

        public class main
        {
            public static Vector2 main_scrollview = Vector2.zero;
            public static void UI(float pos_x, float pos_y, float size_w)
            {
                dropdown.InitScrollViewPositions();

                float size_max_h = Screen.height * 80 / 100;
                float size_h = 0;                
                float scrollview_h = GetSizeH() - 10;

                bool scroll = false;
                if (scrollview_h > size_max_h)
                {
                    size_h = size_max_h;
                    scroll = true;
                }
                else { size_h = scrollview_h; }
                GUI.DrawTexture(new Rect(pos_x, pos_y, size_w, size_h), Managers.GUI_Manager.Textures.texture_grey);
                pos_y += 5;

                float scrollview_w = size_w;
                if (scroll) { scrollview_w -= 20; }

                main_scrollview = GUI.BeginScrollView(new Rect(pos_x, pos_y, size_w, size_h), main_scrollview, new Rect(0, 0, scrollview_w, scrollview_h));

                float new_pos_x = 5;
                float new_pos_y = 0;

                scrollview_w -= 10;

                float add = type.UI(new_pos_x, new_pos_y, pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = rarity.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = base_item.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = unique.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = set.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = legendary.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = implicits.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = forgin_potencial.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = affixs.MenuUI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = unique_mods.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = legenday_potencial.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = weaver_wil.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                add = quantity.UI(new_pos_x, new_pos_y, (int)scrollview_w);
                pos_y += add;
                new_pos_y += add;

                drop.UI(new_pos_x, new_pos_y, (int)scrollview_w);

                GUI.EndScrollView();

                if (affixs.seal.show)
                {
                    float content_margin = GUI_Manager.PauseMenu.UI.content_margin;
                    size_h = (int)(80 + (3 * content_margin));
                    if (seal.add)
                    {
                        size_h += affixs.seal.seal_add_h;
                        if (seal.override_value) { size_h += 25; }
                        if (seal.override_affix) { size_h += affix_btn_size_h + 5; }
                    }

                    float section_x = GUI_Manager.PauseMenu.UI.Section_2_X;
                    float section_y = GUI_Manager.PauseMenu.UI.Content_Y;
                    float section_w = GUI_Manager.PauseMenu.UI.Section_W;

                    InitScrollviewPositions();
                    
                    GUI.DrawTexture(new Rect(GUI_Manager.PauseMenu.UI.Section_2_X, GUI_Manager.PauseMenu.UI.Content_Y, GUI_Manager.PauseMenu.UI.Section_W, size_h), GUI_Manager.Textures.black);
                    section_x += content_margin;
                    section_y += content_margin;
                    float content_w = section_w - (2 * content_margin);

                    GUI.TextField(new Rect(section_x, section_y, content_w, 40), "Seal", GUI_Manager.Styles.Content_Title());
                    section_y += 40 + content_margin;
                    GUI.DrawTexture(new Rect(section_x, section_y, content_w, (size_h - 40 - (3 * content_margin))), GUI_Manager.Textures.grey);
                    section_y += content_margin;
                    section_x += content_margin;
                    pos_x = section_x;
                    pos_y = section_y;
                    size_w = content_w - (2 * content_margin);

                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 60, 30), "Add Seal", Managers.GUI_Manager.Styles.TextField_Style());
                    if (GUI.Button(new Rect((pos_x + size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(seal.add))) { seal.add = !seal.add; }
                    string btn_str = "Enable";
                    if (seal.add) { btn_str = "Disable"; }
                    GUI.Label(new Rect((pos_x + size_w - 60 + 5), pos_y, 50, 30), btn_str, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                    pos_y += 35;
                    if (seal.add)
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 120, 30), "Tier", Managers.GUI_Manager.Styles.TextField_Style());
                        GUI.Label(new Rect((pos_x + size_w - 60), pos_y, 60, 30), seal.tier.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        pos_y += 35;
                        seal.tier = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), seal.tier, 1, 7));
                        pos_y += 25;
                        GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 120, 30), "Override Roll", Managers.GUI_Manager.Styles.TextField_Style());
                        if (seal.override_value)
                        {
                            float value = (seal.values * 100) / 255;
                            string value_str = value.ToString() + " %";
                            GUI.Label(new Rect((pos_x + size_w - 120), pos_y, 60, 30), value_str, Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect((pos_x + size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(seal.override_value))) { seal.override_value = !seal.override_value; }
                        string btn_tier_value = "Enable";
                        if (seal.override_value) { btn_tier_value = "Disable"; }
                        GUI.Label(new Rect((pos_x + size_w - 60 + 5), pos_y, 50, 30), btn_tier_value, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 35;
                        if (seal.override_value)
                        {
                            seal.values = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), seal.values, 0, 255));
                            pos_y += 25;
                        }
                        GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 60, 30), "Override Affix", Managers.GUI_Manager.Styles.TextField_Style());
                        if (GUI.Button(new Rect((pos_x + size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(seal.override_affix))) { seal.override_affix = !seal.override_affix; }
                        string btn_affix = "Enable";
                        if (seal.override_affix) { btn_affix = "Disable"; }
                        GUI.Label(new Rect((pos_x + size_w - 60 + 5), pos_y, 50, 30), btn_affix, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 35;
                        if (seal.override_affix)
                        {
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
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), seal.dropdown_list[seal.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Choose a Prefix/Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((seal.idol_dropdown_index < seal.idol_dropdown_list.Length) && (seal.idol_dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), seal.idol_dropdown_list[seal.idol_dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Choose a Prefix/Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += affix_btn_size_h;
                        }
                        pos_y += 5;
                    }
                }
                if (affixs.affix_show)
                {
                    float content_margin = GUI_Manager.PauseMenu.UI.content_margin;
                    int base_h = (int)(90 + (3 * content_margin));
                    int affix_h = (int)(60 + (5 * content_margin) + affix_btn_size_h);

                    int menu_affix_h = (int)(40 + content_margin);
                    size_h = base_h + (menu_affix_h * nb_affixs);
                    int tier = (int)(20 + content_margin);
                    int value = (int)(20 + content_margin);

                    if (((Show_Affix_0) && (override_value_0)) || ((Show_Affix_1) && (override_value_1)) ||
                        ((Show_Affix_2) && (override_value_2)) || ((Show_Affix_3) && (override_value_3)) ||
                        ((Show_Affix_4) && (override_value_4)) || ((Show_Affix_5) && (override_value_5)))
                    {
                        affix_h += value;
                    }
                    if (((Show_Affix_0) && (override_tier_0)) || ((Show_Affix_1) && (override_tier_1)) ||
                        ((Show_Affix_2) && (override_tier_2)) || ((Show_Affix_3) && (override_tier_3)) ||
                        ((Show_Affix_4) && (override_tier_4)) || ((Show_Affix_5) && (override_tier_5)))
                    {
                        affix_h += tier;
                    }
                    if ((Show_Affix_0) || (Show_Affix_1) || (Show_Affix_2) || (Show_Affix_3) || (Show_Affix_4) || Show_Affix_5)
                    {
                        size_h += (int)(affix_h + content_margin);
                    }

                    float section_x = GUI_Manager.PauseMenu.UI.Section_2_X;
                    float section_y = GUI_Manager.PauseMenu.UI.Content_Y;
                    float section_w = GUI_Manager.PauseMenu.UI.Section_W;

                    GUI.DrawTexture(new Rect(GUI_Manager.PauseMenu.UI.Section_2_X, GUI_Manager.PauseMenu.UI.Content_Y, GUI_Manager.PauseMenu.UI.Section_W, size_h), Managers.GUI_Manager.Textures.black);
                    section_x += content_margin;
                    section_y += content_margin;
                    float content_w = section_w - (2 * content_margin);

                    GUI.TextField(new Rect(section_x, section_y, content_w, 40), "Affix", GUI_Manager.Styles.Content_Title());
                    section_y += 40 + content_margin;

                    GUI.DrawTexture(new Rect(section_x, section_y, content_w, 40 + (2 * content_margin)), GUI_Manager.Textures.grey);
                    section_y += content_margin;
                    section_x += content_margin;
                    pos_x = section_x;
                    pos_y = section_y;
                    size_w = content_w - (2 * content_margin);

                    InitScrollviewPositions();
                    
                    GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 65), 20), "Nb affixes", Managers.GUI_Manager.Styles.TextField_Style());
                    if (nb_affixs < 0) { nb_affixs = 0; }
                    if (nb_affixs > max_nb_affixs) { nb_affixs = (int)max_nb_affixs; }
                    GUI.Label(new Rect((pos_x + (size_w - 60)), (pos_y), 60, 20), nb_affixs.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 20 + content_margin;
                    nb_affixs = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), nb_affixs, 0, max_nb_affixs));
                    pos_y += 20 + content_margin;

                    if (nb_affixs > 0)
                    {
                        if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 1", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_0)))
                        {
                            Show_Affix_1 = false;
                            Show_Affix_2 = false;
                            Show_Affix_3 = false;
                            Show_Affix_4 = false;
                            Show_Affix_5 = false;
                            Show_Affix_0 = !Show_Affix_0;
                        }
                        pos_y += 40 + content_margin;
                        if (Show_Affix_0)
                        {
                            //Background
                            GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                            //Tier
                            pos_y += content_margin;
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_tier_0)
                            {
                                GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_0.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_0))) { override_tier_0 = !override_tier_0; }
                            string btn_affix_tier_label_0 = "Enable";
                            if (override_tier_0) { btn_affix_tier_label_0 = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label_0, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_tier_0)
                            {
                                affix_tier_0 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_0, 1, 7));
                                pos_y += 20 + content_margin;
                            }
                            //Value
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_value_0)
                            {
                                GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_0.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_0))) { override_value_0 = !override_value_0; }
                            string btn_affix_values_0 = "Enable";
                            if (override_value_0) { btn_affix_values_0 = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values_0, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_value_0)
                            {
                                affix_values_0 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_0, 0, 255));
                                pos_y += 20 + content_margin;
                            }
                            //Drop
                            if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                            {
                                seal.show_dropdown = false;

                                prefixs.slot_1.show_dropdown = false;
                                prefixs.slot_2.show_dropdown = false;

                                suffixs.slot_0.show_dropdown = false;
                                suffixs.slot_1.show_dropdown = false;
                                suffixs.slot_2.show_dropdown = false;

                                idols.slot_1.show_dropdown = false;
                                idols.slot_2.show_dropdown = false;
                                idols.slot_3.show_dropdown = false;
                                idols.slot_4.show_dropdown = false;
                                idols.slot_5.show_dropdown = false;

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
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), prefixs.dropdown_list[prefixs.slot_0.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((idols.slot_0.dropdown_index < idols.dropdown_list.Length) && (idols.slot_0.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_0.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += affix_btn_size_h + (3 * content_margin);
                        }
                        else { prefixs.slot_0.show_dropdown = false; }
                    }
                    if (nb_affixs > 1)
                    {
                        if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 2", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_1)))
                        {
                            Show_Affix_0 = false;
                            Show_Affix_2 = false;
                            Show_Affix_3 = false;
                            Show_Affix_4 = false;
                            Show_Affix_5 = false;
                            Show_Affix_1 = !Show_Affix_1;
                        }
                        pos_y += 40 + content_margin;
                        if (Show_Affix_1)
                        {
                            //Background
                            GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                            //Tier
                            pos_y += content_margin;
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_tier_1)
                            {
                                GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_1.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_1))) { override_tier_1 = !override_tier_1; }
                            string btn_affix_tier_label = "Enable";
                            if (override_tier_1) { btn_affix_tier_label = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_tier_1)
                            {
                                affix_tier_1 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_1, 1, 7));
                                pos_y += 20 + content_margin;
                            }
                            //Value
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_value_1)
                            {
                                GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_1.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_1))) { override_value_1 = !override_value_1; }
                            string btn_affix_values = "Enable";
                            if (override_value_1) { btn_affix_values = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_value_1)
                            {
                                affix_values_1 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_1, 0, 255));
                                pos_y += 20 + content_margin;
                            }
                            //Slot
                            if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                            {
                                seal.show_dropdown = false;

                                prefixs.slot_1.show_dropdown = false;
                                prefixs.slot_0.show_dropdown = false;
                                prefixs.slot_2.show_dropdown = false;

                                suffixs.slot_1.show_dropdown = false;
                                suffixs.slot_2.show_dropdown = false;

                                idols.slot_0.show_dropdown = false;
                                idols.slot_2.show_dropdown = false;
                                idols.slot_3.show_dropdown = false;
                                idols.slot_4.show_dropdown = false;
                                idols.slot_5.show_dropdown = false;

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
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), suffixs.dropdown_list[suffixs.slot_0.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((idols.slot_1.dropdown_index < idols.dropdown_list.Length) && (idols.slot_1.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_1.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += affix_btn_size_h + (3 * content_margin);
                        }
                        else { suffixs.slot_0.show_dropdown = false; }
                    }
                    if (nb_affixs > 2)
                    {
                        if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 3", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_2)))
                        {
                            Show_Affix_0 = false;
                            Show_Affix_1 = false;
                            Show_Affix_3 = false;
                            Show_Affix_4 = false;
                            Show_Affix_5 = false;
                            Show_Affix_2 = !Show_Affix_2;
                        }
                        pos_y += 40 + content_margin;
                        if (Show_Affix_2)
                        {
                            //Background
                            GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                            //Tier
                            pos_y += content_margin;
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_tier_2)
                            {
                                GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_2.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_2))) { override_tier_2 = !override_tier_2; }
                            string btn_affix_tier_label = "Enable";
                            if (override_tier_2) { btn_affix_tier_label = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_tier_2)
                            {
                                affix_tier_2 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_2, 1, 7));
                                pos_y += 20 + content_margin;
                            }
                            //Value
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_value_2)
                            {
                                GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_2.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_2))) { override_value_2 = !override_value_2; }
                            string btn_affix_values = "Enable";
                            if (override_value_2) { btn_affix_values = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_value_2)
                            {
                                affix_values_2 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_2, 0, 255));
                                pos_y += 20 + content_margin;
                            }
                            //Slot
                            if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                            {
                                seal.show_dropdown = false;

                                prefixs.slot_0.show_dropdown = false;
                                prefixs.slot_2.show_dropdown = false;

                                suffixs.slot_0.show_dropdown = false;
                                suffixs.slot_1.show_dropdown = false;
                                suffixs.slot_2.show_dropdown = false;

                                idols.slot_0.show_dropdown = false;
                                idols.slot_1.show_dropdown = false;
                                idols.slot_3.show_dropdown = false;
                                idols.slot_4.show_dropdown = false;
                                idols.slot_5.show_dropdown = false;

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
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), prefixs.dropdown_list[prefixs.slot_1.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((idols.slot_2.dropdown_index < idols.dropdown_list.Length) && (idols.slot_2.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_2.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += affix_btn_size_h + (3 * content_margin);
                        }
                        else { prefixs.slot_1.show_dropdown = false; }
                    }
                    if (nb_affixs > 3)
                    {
                        if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 4", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_3)))
                        {
                            Show_Affix_0 = false;
                            Show_Affix_1 = false;
                            Show_Affix_2 = false;
                            Show_Affix_4 = false;
                            Show_Affix_5 = false;
                            Show_Affix_3 = !Show_Affix_3;
                        }
                        pos_y += 40 + content_margin;
                        if (Show_Affix_3)
                        {
                            //Background
                            GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                            //Tier
                            pos_y += content_margin;
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_tier_3)
                            {
                                GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_3.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_3))) { override_tier_3 = !override_tier_3; }
                            string btn_affix_tier_label = "Enable";
                            if (override_tier_3) { btn_affix_tier_label = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_tier_3)
                            {
                                affix_tier_3 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_3, 1, 7));
                                pos_y += 20 + content_margin;
                            }
                            //Value
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_value_3)
                            {
                                GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_3.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_3))) { override_value_3 = !override_value_3; }
                            string btn_affix_values = "Enable";
                            if (override_value_3) { btn_affix_values = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_value_3)
                            {
                                affix_values_3 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_3, 0, 255));
                                pos_y += 20 + content_margin;
                            }
                            //
                            if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                            {
                                seal.show_dropdown = false;

                                prefixs.slot_0.show_dropdown = false;
                                prefixs.slot_1.show_dropdown = false;
                                prefixs.slot_2.show_dropdown = false;

                                suffixs.slot_0.show_dropdown = false;
                                suffixs.slot_2.show_dropdown = false;

                                idols.slot_0.show_dropdown = false;
                                idols.slot_1.show_dropdown = false;
                                idols.slot_2.show_dropdown = false;
                                idols.slot_4.show_dropdown = false;
                                idols.slot_5.show_dropdown = false;
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
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), suffixs.dropdown_list[suffixs.slot_1.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((idols.slot_3.dropdown_index < idols.dropdown_list.Length) && (idols.slot_3.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_3.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += affix_btn_size_h + (3 * content_margin);
                        }
                        else { suffixs.slot_1.show_dropdown = false; }
                    }
                    if (nb_affixs > 4)
                    {
                        if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 5", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_4)))
                        {
                            Show_Affix_0 = false;
                            Show_Affix_1 = false;
                            Show_Affix_2 = false;
                            Show_Affix_3 = false;
                            Show_Affix_5 = false;
                            Show_Affix_4 = !Show_Affix_4;
                        }
                        pos_y += 40 + content_margin;
                        if (Show_Affix_4)
                        {
                            //Background
                            GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                            //Tier
                            pos_y += content_margin;
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_tier_4)
                            {
                                GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_4.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_4))) { override_tier_4 = !override_tier_4; }
                            string btn_affix_tier_label = "Enable";
                            if (override_tier_3) { btn_affix_tier_label = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_tier_4)
                            {
                                affix_tier_4 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_4, 1, 7));
                                pos_y += 20 + content_margin;
                            }
                            //Value
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_value_4)
                            {
                                GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_4.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_4))) { override_value_4 = !override_value_4; }
                            string btn_affix_values = "Enable";
                            if (override_value_4) { btn_affix_values = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_value_4)
                            {
                                affix_values_4 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_4, 0, 255));
                                pos_y += 20 + content_margin;
                            }
                            //
                            if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                            {
                                seal.show_dropdown = false;

                                prefixs.slot_0.show_dropdown = false;
                                prefixs.slot_1.show_dropdown = false;

                                suffixs.slot_0.show_dropdown = false;
                                suffixs.slot_1.show_dropdown = false;
                                suffixs.slot_2.show_dropdown = false;

                                idols.slot_1.show_dropdown = false;
                                idols.slot_2.show_dropdown = false;
                                idols.slot_3.show_dropdown = false;
                                idols.slot_5.show_dropdown = false;
                                if (!type.is_idol)
                                {
                                    idols.slot_4.show_dropdown = false;
                                    prefixs.slot_2.show_dropdown = !prefixs.slot_2.show_dropdown;
                                }
                                else
                                {
                                    prefixs.slot_2.show_dropdown = false;
                                    idols.slot_4.show_dropdown = !idols.slot_4.show_dropdown;
                                }
                            }
                            if (!type.is_idol)
                            {
                                if ((prefixs.slot_2.dropdown_index < prefixs.dropdown_list.Length) && (prefixs.slot_2.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), prefixs.dropdown_list[prefixs.slot_2.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((idols.slot_4.dropdown_index < idols.dropdown_list.Length) && (idols.slot_4.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_4.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            pos_y += affix_btn_size_h + (3 * content_margin);
                        }
                        else { prefixs.slot_2.show_dropdown = false; }
                    }
                    if (nb_affixs > 5)
                    {
                        if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 6", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_5)))
                        {
                            Show_Affix_0 = false;
                            Show_Affix_1 = false;
                            Show_Affix_2 = false;
                            Show_Affix_3 = false;
                            Show_Affix_4 = false;
                            Show_Affix_5 = !Show_Affix_5;
                        }
                        pos_y += 40 + content_margin;
                        if (Show_Affix_5)
                        {
                            //Background
                            GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                            //Tier
                            pos_y += content_margin;
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_tier_5)
                            {
                                GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_5.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_5))) { override_tier_5 = !override_tier_5; }
                            string btn_affix_tier_label = "Enable";
                            if (override_tier_5) { btn_affix_tier_label = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_tier_5)
                            {
                                affix_tier_5 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_5, 1, 7));
                                pos_y += 20 + content_margin;
                            }
                            //Value
                            GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                            if (override_value_5)
                            {
                                GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_5.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                            }
                            if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_5))) { override_value_5 = !override_value_5; }
                            string btn_affix_values = "Enable";
                            if (override_value_5) { btn_affix_values = "Disable"; }
                            GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 30 + content_margin;
                            if (override_value_5)
                            {
                                affix_values_5 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_5, 0, 255));
                                pos_y += 20 + content_margin;
                            }
                            //
                            if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                            {
                                seal.show_dropdown = false;

                                prefixs.slot_0.show_dropdown = false;
                                prefixs.slot_1.show_dropdown = false;
                                prefixs.slot_2.show_dropdown = false;

                                suffixs.slot_0.show_dropdown = false;
                                suffixs.slot_1.show_dropdown = false;

                                idols.slot_0.show_dropdown = false;
                                idols.slot_1.show_dropdown = false;
                                idols.slot_2.show_dropdown = false;
                                idols.slot_3.show_dropdown = false;
                                idols.slot_4.show_dropdown = false;

                                if (!type.is_idol)
                                {
                                    idols.slot_5.show_dropdown = false;
                                    suffixs.slot_2.show_dropdown = !suffixs.slot_2.show_dropdown;
                                }
                                else
                                {
                                    suffixs.slot_2.show_dropdown = false;
                                    idols.slot_5.show_dropdown = !idols.slot_5.show_dropdown;
                                }
                            }
                            if (!type.is_idol)
                            {
                                if ((suffixs.slot_2.dropdown_index < suffixs.dropdown_list.Length) && (suffixs.slot_2.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), suffixs.dropdown_list[suffixs.slot_2.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((idols.slot_5.dropdown_index < idols.dropdown_list.Length) && (idols.slot_5.dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_5.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            //pos_y += affix_btn_size_h + (2 * content_margin);
                        }
                        else { suffixs.slot_2.show_dropdown = false; }
                    }
                }
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
                if (affixs.seal.enable) { result += affixs.seal.h; }
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
                    Dropdown(affixs.prefixs.slot_2.pos_x, affixs.prefixs.slot_2.pos_y, affixs.dropdown_rect, ref affixs.prefixs.slot_2.dropdown_scrollview, ref affixs.prefixs.slot_2.dropdown_index, ref affixs.prefixs.slot_2.show_dropdown, affixs.prefixs.dropdown_list);

                    Dropdown(affixs.suffixs.slot_0.pos_x, affixs.suffixs.slot_0.pos_y, affixs.dropdown_rect, ref affixs.suffixs.slot_0.dropdown_scrollview, ref affixs.suffixs.slot_0.dropdown_index, ref affixs.suffixs.slot_0.show_dropdown, affixs.suffixs.dropdown_list);
                    Dropdown(affixs.suffixs.slot_1.pos_x, affixs.suffixs.slot_1.pos_y, affixs.dropdown_rect, ref affixs.suffixs.slot_1.dropdown_scrollview, ref affixs.suffixs.slot_1.dropdown_index, ref affixs.suffixs.slot_1.show_dropdown, affixs.suffixs.dropdown_list);
                    Dropdown(affixs.suffixs.slot_2.pos_x, affixs.suffixs.slot_2.pos_y, affixs.dropdown_rect, ref affixs.suffixs.slot_2.dropdown_scrollview, ref affixs.suffixs.slot_2.dropdown_index, ref affixs.suffixs.slot_2.show_dropdown, affixs.suffixs.dropdown_list);

                    Dropdown(affixs.idols.slot_0.pos_x, affixs.idols.slot_0.pos_y, affixs.dropdown_rect, ref affixs.idols.slot_0.dropdown_scrollview, ref affixs.idols.slot_0.dropdown_index, ref affixs.idols.slot_0.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_1.pos_x, affixs.idols.slot_1.pos_y, affixs.dropdown_rect, ref affixs.idols.slot_1.dropdown_scrollview, ref affixs.idols.slot_1.dropdown_index, ref affixs.idols.slot_1.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_2.pos_x, affixs.idols.slot_2.pos_y, affixs.dropdown_rect, ref affixs.idols.slot_2.dropdown_scrollview, ref affixs.idols.slot_2.dropdown_index, ref affixs.idols.slot_2.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_3.pos_x, affixs.idols.slot_3.pos_y, affixs.dropdown_rect, ref affixs.idols.slot_3.dropdown_scrollview, ref affixs.idols.slot_3.dropdown_index, ref affixs.idols.slot_3.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_4.pos_x, affixs.idols.slot_4.pos_y, affixs.dropdown_rect, ref affixs.idols.slot_4.dropdown_scrollview, ref affixs.idols.slot_4.dropdown_index, ref affixs.idols.slot_4.show_dropdown, affixs.idols.dropdown_list);
                    Dropdown(affixs.idols.slot_5.pos_x, affixs.idols.slot_5.pos_y, affixs.dropdown_rect, ref affixs.idols.slot_5.dropdown_scrollview, ref affixs.idols.slot_5.dropdown_index, ref affixs.idols.slot_5.show_dropdown, affixs.idols.dropdown_list);
                }
                public static void InitScrollViewPositions() //float pos_x, float size_w)
                {
                    type.pos_x = GUI_Manager.PauseMenu.UI.Section_2_X;
                    type.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                    type.dropdown_rect.width = GUI_Manager.PauseMenu.UI.Section_W - (2 * GUI_Manager.PauseMenu.UI.content_margin);
                    type.dropdown_rect.height = (Screen.height * 80) / 100;

                    rarity.pos_x = GUI_Manager.PauseMenu.UI.Section_2_X;
                    rarity.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                    rarity.dropdown_rect.width = GUI_Manager.PauseMenu.UI.Section_W - (2 * GUI_Manager.PauseMenu.UI.content_margin);
                    rarity.dropdown_rect.height = (Screen.height * 80) / 100;

                    base_item.pos_x = GUI_Manager.PauseMenu.UI.Section_2_X;
                    base_item.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                    base_item.dropdown_rect.width = GUI_Manager.PauseMenu.UI.Section_W - (2 * GUI_Manager.PauseMenu.UI.content_margin);
                    base_item.dropdown_rect.height = (Screen.height * 80) / 100;

                    unique.pos_x = GUI_Manager.PauseMenu.UI.Section_2_X;
                    unique.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                    unique.dropdown_rect.width = GUI_Manager.PauseMenu.UI.Section_W - (2 * GUI_Manager.PauseMenu.UI.content_margin);
                    unique.dropdown_rect.height = (Screen.height * 80) / 100;

                    set.pos_x = GUI_Manager.PauseMenu.UI.Section_2_X;
                    set.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                    set.dropdown_rect.width = GUI_Manager.PauseMenu.UI.Section_W - (2 * GUI_Manager.PauseMenu.UI.content_margin);
                    set.dropdown_rect.height = (Screen.height * 80) / 100;

                    legendary.pos_x = GUI_Manager.PauseMenu.UI.Section_2_X;
                    legendary.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                    legendary.dropdown_rect.width = GUI_Manager.PauseMenu.UI.Section_W - (2 * GUI_Manager.PauseMenu.UI.content_margin);
                    legendary.dropdown_rect.height = (Screen.height * 80) / 100;                                        
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
                        GUI.DrawTexture(new Rect(pos_x, pos_y, rect.width + 10, size_h + 10),  Managers.GUI_Manager.Textures.windowBackground);
                        pos_x += 5;
                        pos_y += 5;
                        float scrollview_w = rect.width;
                        if (scroll) { scrollview_w = rect.width - 20; }
                        
                        scrollview = GUI.BeginScrollView(new Rect(pos_x, pos_y, rect.width, size_h), scrollview, new Rect(0, 0, scrollview_w, scrollview_max_h));
                        UnityEngine.GUIStyle style =  Managers.GUI_Manager.Styles.Button_Style(false);
                        for (int index = 0; index < dropdown_list.Length; index++)
                        {
                            if (GUI.Button(new Rect(0, (index * (button_h + margin)), rect.width - 25, button_h), "", style))
                            {
                                show = false;
                                dropdown_index = index;
                            }
                            GUI.Label(new Rect(5, ((index * (button_h + margin)) + 3), rect.width - 30, button_h - 3), dropdown_list[index],  Managers.GUI_Manager.Styles.DropdownLabelLeft_Style());
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

            public static float UI(float pos_x, float pos_y, float section_pos_y, int w)
            {
                type.pos_y = section_pos_y; //pos_y;
                if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                {
                    rarity.show_dropdown = false;
                    base_item.show_dropdown = false;
                    unique.show_dropdown = false;
                    set.show_dropdown = false;
                    legendary.show_dropdown = false;
                    affixs.seal.show = false;
                    affixs.affix_show = false;
                    affixs.Hide();                    
                    type.show_dropdown = !type.show_dropdown;
                }
                if ((type.dropdown_index < type.dropdown_list.Length) && (type.dropdown_index > -1))
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), type.dropdown_list[type.dropdown_index],  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                    if (type.dropdown_index_backup != type.dropdown_index)
                    {
                        type.dropdown_index_backup = type.dropdown_index;
                        if ((type.GetItemType() > 24) && (type.GetItemType() < 34) && (rarity.dropdown_index < 3)) { is_idol = true; }
                        else { is_idol = false; }
                        rarity.dropdown_index = -1;
                        rarity.enable = true;
                        //rarity.enable = rarity.EnableFromType(type.dropdown_list[type.dropdown_index]);
                        base_item.dropdown_index = -1;                        
                        base_item.InitFromType(type.dropdown_list[type.dropdown_index]);
                        base_item.enable = true;                             
                        //base_item.enable = base_item.EnableFromType(type.dropdown_list[type.dropdown_index]);
                        affixs.enable = false;
                        affixs.seal.enable = false;
                        unique.enable = false;
                        unique_mods.enable = false;
                        set.enable = false;
                        legendary.enable = false;
                        unique.UpdateList(type.dropdown_list[type.dropdown_index]);
                        quantity.enable = false;
                        drop.enable = false;
                        //quantity.enable = quantity.EnableFromType(type.dropdown_list[type.dropdown_index]);
                        //drop.enable = drop.EnableFromType(type.dropdown_list[type.dropdown_index]);
                        implicits.enable = false;
                        forgin_potencial.enable = false;
                        legenday_potencial.enable = false;
                        weaver_wil.enable = false;
                    }
                }
                else
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Type",  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                    rarity.enable = false;
                    quantity.enable = false;
                    base_item.enable = false;
                    affixs.enable = false;
                    affixs.seal.enable = false;
                    unique.enable = false;
                    unique_mods.enable = false;
                    set.enable = false;
                    legendary.enable = false;
                    drop.enable = false;
                    implicits.enable = false;
                    forgin_potencial.enable = false;
                    legenday_potencial.enable = false;
                    weaver_wil.enable = false;
                }
                //pos_y += 35;
                //section_pos_y += 35;

                return 35;
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
                        //Main.logger_instance.Msg(item.baseTypeID + " : " + item.displayName);
                        if (item.displayName != "") { type_names.Add(item.displayName); }
                    }
                    foreach (ItemList.BaseNonEquipmentItem item in ItemList.get().nonEquippableItems)
                    {
                        //Main.logger_instance.Msg(item.baseTypeID + " : " + item.displayName);
                        if (item.displayName != "") { type_names.Add(item.displayName); }
                    }
                    type.dropdown_list = new string[type_names.Count];
                    for (int i = 0; i < type.dropdown_list.Length; i++)
                    {
                        type.dropdown_list[i] = type_names[i];
                    }
                    type_names.Clear();
                    //Main.logger_instance.Msg("ForceDrop:InitList -> Found " + type_names.Count + " Types");
                }
                //else { Main.logger_instance.Error("ForceDrop:InitList -> can't get itemlist"); }
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
                        affixs.seal.show = false;
                        affixs.affix_show = false;
                        affixs.Hide();
                        rarity.show_dropdown = !rarity.show_dropdown;
                    }
                    if ((rarity.dropdown_index < rarity.dropdown_list.Length) && (rarity.dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), rarity.dropdown_list[rarity.dropdown_index],  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
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
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Rarity",  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
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

                return 35;
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
                float result = 0;
                if (base_item.enable)
                {
                    //base_item.pos_y = pos_y;
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
                        affixs.seal.show = false;
                        affixs.affix_show = false;
                        affixs.Hide();
                        base_item.show_dropdown = !base_item.show_dropdown;
                    }
                    if ((base_item.dropdown_index < base_item.dropdown_list.Length) && (base_item.dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), base_item.dropdown_list[base_item.dropdown_index],  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        if (type.is_idol) { affixs.max_nb_affixs = 2f; }
                        else { affixs.max_nb_affixs = 6f; }
                        implicits.enable = implicits.EnableFromType();
                        forgin_potencial.enable = forgin_potencial.EnableFromType();
                        affixs.enable = affixs.EnableFromType(); //true;
                        affixs.seal.enable = affixs.EnableFromType();  //true;                        
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Base Item",  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        implicits.enable = false;
                        forgin_potencial.enable = false;
                        affixs.enable = false;
                        affixs.seal.enable = false;
                        quantity.enable = false;
                        drop.enable = false;
                    }
                    pos_y += 35;
                    result += 35;
                }
                else { base_item.show_dropdown = false; }

                return result;
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
                    else { Main.logger_instance.Msg("Error Type Not Found : " + type_name); }
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
                float result = 0;
                if (unique.enable)
                {
                    //unique.pos_y = pos_y;
                    forgin_potencial.enable = false;
                    affixs.enable = false;

                    if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                    {
                        base_item.show_dropdown = false;
                        set.show_dropdown = false;
                        legendary.show_dropdown = false;
                        type.show_dropdown = false;
                        rarity.show_dropdown = false;
                        affixs.seal.show = false;
                        affixs.affix_show = false;
                        affixs.Hide();
                        affixs.seal.enable = false;
                        show_dropdown = !show_dropdown;
                    }
                    if ((dropdown_index < dropdown_list.Length) && (dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), dropdown_list[dropdown_index],  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        implicits.enable = implicits.EnableFromType();
                        unique_mods.enable = unique_mods.EnableFromRarity();
                        legenday_potencial.enable = legenday_potencial.EnableFromRarity();
                        weaver_wil.enable = weaver_wil.EnableFromRarity();
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Unique Item",  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        implicits.enable = false;
                        unique_mods.enable = false;
                        legenday_potencial.enable = false;
                        weaver_wil.enable = false;
                        quantity.enable = false;
                        drop.enable = false;
                    }
                    pos_y += 35;
                    result += 35;
                }
                else { unique.show_dropdown = false; }

                return result;
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
                float result = 0;
                if (set.enable)
                {
                    //set.pos_y = pos_y;
                    forgin_potencial.enable = false;
                    affixs.enable = false;
                    affixs.seal.enable = false;
                    legenday_potencial.enable = false;
                    weaver_wil.enable = false;
                    if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                    {
                        base_item.show_dropdown = false;
                        unique.show_dropdown = false;
                        legendary.show_dropdown = false;
                        type.show_dropdown = false;
                        rarity.show_dropdown = false;
                        affixs.seal.show = false;
                        affixs.affix_show = false;
                        affixs.Hide();
                        set.show_dropdown = !set.show_dropdown;
                    }
                    if ((set.dropdown_index < set.dropdown_list.Length) && (set.dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), set.dropdown_list[set.dropdown_index],  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        implicits.enable = implicits.EnableFromType();
                        unique_mods.enable = unique_mods.EnableFromRarity();
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Set Item",  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        implicits.enable = false;
                        unique_mods.enable = false;
                        quantity.enable = false;
                        drop.enable = false;
                    }
                    pos_y += 35;
                    result += 35;
                }
                else { set.show_dropdown = false; }

                return result;
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
                float result = 0;
                if (legendary.enable)
                {
                    forgin_potencial.enable = false;
                    legenday_potencial.enable = false;                    
                    //weaver_wil.enable = false;
                    affixs.seal.enable = false;
                    affixs.seal.add = false;
                    if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                    {
                        base_item.show_dropdown = false;
                        unique.show_dropdown = false;
                        set.show_dropdown = false;                        
                        type.show_dropdown = false;
                        rarity.show_dropdown = false;
                        affixs.seal.show = false;
                        affixs.affix_show = false;
                        affixs.Hide();
                        show_dropdown = !show_dropdown;
                    }
                    if ((dropdown_index < dropdown_list.Length) && (dropdown_index > -1))
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), dropdown_list[dropdown_index],  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        implicits.enable = implicits.EnableFromType();
                        unique_mods.enable = unique_mods.EnableFromRarity();
                        weaver_wil.enable = true;
                        affixs.max_nb_affixs = 6f;
                        //affixs.seal.enable = false;
                        affixs.enable = true;
                        quantity.enable = true;
                        drop.enable = true;
                    }
                    else
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, w, 30), "Legendary Item",  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        implicits.enable = false;
                        //affixs.seal.enable = false;
                        weaver_wil.enable = false;
                        affixs.enable = false;
                        quantity.enable = false;
                        drop.enable = false;
                    }
                    pos_y += 35;
                    result += 35;
                }
                else { show_dropdown = false; }

                return result;
            }
        }
        public class implicits
        {
            public static bool enable = false;
            public static float value_0 = 255;
            public static float value_1 = 255;
            public static float value_2 = 255;
            public static int h = 150;

            public static float UI(float pos_x, float pos_y, int w)
            {
                float result = 0;
                if (enable)
                {
                    float temp_value_0 = (value_0 / 255) * 100;
                    int temp_int_value_0 = (int)temp_value_0;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Impicits 0",  Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_0.ToString() + " %",  Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_0 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_0, 0, 255));
                    pos_y += 25;

                    float temp_value_1 = (value_1 / 255) * 100;
                    int temp_int_value_1 = (int)temp_value_1;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Impicits 1", Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_1.ToString() + " %", Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_1 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_1, 0, 255));
                    pos_y += 25;

                    float temp_value_2 = (value_2 / 255) * 100;
                    int temp_int_value_2 = (int)temp_value_2;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Impicits 2", Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_2.ToString() + " %", Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_2 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_2, 0, 255));
                    pos_y += 25;

                    result += 150;
                }

                return result;
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
            public static float max_nb_affixs = 6f;
            public static int h = 35; //35
            public static Rect dropdown_rect = new Rect(125, 50, 400, 400);
            public static int affix_btn_size_h = 40;
            public static bool affix_show = false;
            public static int affix_show_h = 125;

            public static bool Show_Affix_0 = false;
            public static bool Show_Affix_1 = false;
            public static bool Show_Affix_2 = false;
            public static bool Show_Affix_3 = false;
            public static bool Show_Affix_4 = false;
            public static bool Show_Affix_5 = false;

            public static bool override_tier_0 = false;
            public static bool override_tier_1 = false;
            public static bool override_tier_2 = false;
            public static bool override_tier_3 = false;
            public static bool override_tier_4 = false;
            public static bool override_tier_5 = false;

            public static int affix_tier_0 = 7;
            public static int affix_tier_1 = 7;
            public static int affix_tier_2 = 7;
            public static int affix_tier_3 = 7;
            public static int affix_tier_4 = 7;
            public static int affix_tier_5 = 7;

            public static bool override_value_0 = false;
            public static bool override_value_1 = false;
            public static bool override_value_2 = false;
            public static bool override_value_3 = false;
            public static bool override_value_4 = false;
            public static bool override_value_5 = false;

            public static int affix_values_0 = 255;
            public static int affix_values_1 = 255;
            public static int affix_values_2 = 255;
            public static int affix_values_3 = 255;
            public static int affix_values_4 = 255;
            public static int affix_values_5 = 255;

            public class seal
            {
                public static int h = 35;
                public static string[] dropdown_list = null;
                public static bool show_dropdown = false;
                public static int dropdown_index = -1;
                public static Vector2 dropdown_scrollview = Vector2.zero;
                public static bool enable = false;
                public static string[] idol_dropdown_list = null;
                public static bool idol_show_dropdown = false;
                public static int idol_dropdown_index = -1;
                public static Vector2 idol_dropdown_scrollview = Vector2.zero;
                public static int seal_add_h = 130;
                public static float pos_x;
                public static float pos_y;
                public static bool show = false;
                public static bool add = false;
                public static int tier = 7;
                public static bool override_value = false;
                public static int values = 255;
                public static bool override_affix = false;

                public static void UI()
                {
                    float content_margin = GUI_Manager.PauseMenu.UI.content_margin;
                    int size_h = (int)(80 + (3 * content_margin));
                    if (seal.add)
                    {
                        size_h += seal_add_h;
                        if (seal.override_value) { size_h += 25; }
                        if (seal.override_affix) { size_h += affix_btn_size_h + 5; }
                    }
                    
                    float section_x = GUI_Manager.PauseMenu.UI.Section_2_X;
                    float section_y = GUI_Manager.PauseMenu.UI.Content_Y;
                    float section_w = GUI_Manager.PauseMenu.UI.Section_W;

                    InitScrollviewPositions();

                    Main.logger_instance.Msg("Seal at X = " + section_x + ", Y = " + section_y + ", W = " + section_w);
                    GUI.DrawTexture(new Rect(GUI_Manager.PauseMenu.UI.Section_2_X, GUI_Manager.PauseMenu.UI.Content_Y, GUI_Manager.PauseMenu.UI.Section_W, size_h), GUI_Manager.Textures.black);
                    section_x += content_margin;
                    section_y += content_margin;
                    float content_w = section_w - (2 * content_margin);

                    GUI.TextField(new Rect(section_x, section_y, content_w, 40), "Seal", GUI_Manager.Styles.Content_Title());
                    section_y += 40 + content_margin;
                    GUI.DrawTexture(new Rect(section_x, section_y, content_w, (size_h - 40 - (3 * content_margin))), GUI_Manager.Textures.grey);
                    section_y += content_margin;
                    section_x += content_margin;
                    float pos_x = section_x;
                    float pos_y = section_y;
                    float size_w = content_w - (2 * content_margin);
                    
                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 60, 30), "Add Seal",  Managers.GUI_Manager.Styles.TextField_Style());
                    if (GUI.Button(new Rect((pos_x + size_w - 60), pos_y, 60, 30), "",  Managers.GUI_Manager.Styles.Button_Style(seal.add))) { seal.add = !seal.add; }
                    string btn_str = "Enable";
                    if (seal.add) { btn_str = "Disable"; }
                    GUI.Label(new Rect((pos_x + size_w - 60 + 5), pos_y, 50, 30), btn_str,  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                    pos_y += 35;
                    if (seal.add)
                    {
                        GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 120, 30), "Tier",  Managers.GUI_Manager.Styles.TextField_Style());
                        GUI.Label(new Rect((pos_x + size_w - 60), pos_y, 60, 30), seal.tier.ToString(),  Managers.GUI_Manager.Styles.TextArea_Style());
                        pos_y += 35;
                        seal.tier = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), seal.tier, 1, 7));
                        pos_y += 25;
                        GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 120, 30), "Override Roll",  Managers.GUI_Manager.Styles.TextField_Style());
                        if (seal.override_value)
                        {
                            float value = (seal.values * 100) / 255;
                            string value_str = value.ToString() + " %";
                            GUI.Label(new Rect((pos_x + size_w - 120), pos_y, 60, 30), value_str,  Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect((pos_x + size_w - 60), pos_y, 60, 30), "",  Managers.GUI_Manager.Styles.Button_Style(seal.override_value))) { seal.override_value = !seal.override_value; }
                        string btn_tier_value = "Enable";
                        if (seal.override_value) { btn_tier_value = "Disable"; }
                        GUI.Label(new Rect((pos_x + size_w - 60 + 5), pos_y, 50, 30), btn_tier_value,  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 35;
                        if (seal.override_value)
                        {
                            seal.values = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), seal.values, 0, 255));
                            pos_y += 25;
                        }
                        GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 60, 30), "Override Affix",  Managers.GUI_Manager.Styles.TextField_Style());
                        if (GUI.Button(new Rect((pos_x + size_w - 60), pos_y, 60, 30), "",  Managers.GUI_Manager.Styles.Button_Style(seal.override_affix))) { seal.override_affix = !seal.override_affix; }
                        string btn_affix = "Enable";
                        if (seal.override_affix) { btn_affix = "Disable"; }
                        GUI.Label(new Rect((pos_x + size_w - 60 + 5), pos_y, 50, 30), btn_affix,  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 35;
                        if (seal.override_affix)
                        {
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
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), seal.dropdown_list[seal.dropdown_index],  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Choose a Prefix/Suffix",  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                            }
                            else
                            {
                                if ((seal.idol_dropdown_index < seal.idol_dropdown_list.Length) && (seal.idol_dropdown_index > -1))
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), seal.idol_dropdown_list[seal.idol_dropdown_index],  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                                }
                                else
                                {
                                    GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Choose a Prefix/Suffix (Idol)",  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
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
                public class slot_2
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
                public class slot_2
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
                public class slot_4
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
                public class slot_5
                {
                    public static bool show_dropdown = false;
                    public static int dropdown_index = -1;
                    public static float pos_x;
                    public static float pos_y;
                    public static Vector2 dropdown_scrollview = Vector2.zero;
                }
            }

            public static void UI()
            {
                float content_margin = GUI_Manager.PauseMenu.UI.content_margin;
                int base_h = (int)(90 + (3 * content_margin));
                int affix_h = (int)(60 + (5 * content_margin) + affix_btn_size_h);
                
                int menu_affix_h = (int)(40 + content_margin);
                int size_h = base_h + (menu_affix_h * nb_affixs);
                int tier = (int)(20 + content_margin);
                int value = (int)(20 + content_margin); 
                
                if (((Show_Affix_0) && (override_value_0)) || ((Show_Affix_1) && (override_value_1)) ||
                    ((Show_Affix_2) && (override_value_2)) || ((Show_Affix_3) && (override_value_3)) ||
                    ((Show_Affix_4) && (override_value_4)) || ((Show_Affix_5) && (override_value_5)))
                {
                    affix_h += value;
                }
                if (((Show_Affix_0) && (override_tier_0)) || ((Show_Affix_1) && (override_tier_1)) ||
                    ((Show_Affix_2) && (override_tier_2)) || ((Show_Affix_3) && (override_tier_3)) ||
                    ((Show_Affix_4) && (override_tier_4)) || ((Show_Affix_5) && (override_tier_5)))
                {
                    affix_h += tier;
                }
                if ((Show_Affix_0) || (Show_Affix_1) || (Show_Affix_2) || (Show_Affix_3) || (Show_Affix_4) || Show_Affix_5)
                {
                    size_h += (int)(affix_h + content_margin);
                }

                float section_x = GUI_Manager.PauseMenu.UI.Section_2_X;
                float section_y = GUI_Manager.PauseMenu.UI.Content_Y;
                float section_w = GUI_Manager.PauseMenu.UI.Section_W;

                //Main.logger_instance.Msg("Affix Green at X = " + section_x + ", Y = " + section_y + ", W = " + section_w + ", H = " + size_h);
                GUI.DrawTexture(new Rect((Screen.width / 2) - 200, (Screen.height / 2) - 200, 400, 400), Managers.GUI_Manager.Textures.green);

                GUI.DrawTexture(new Rect(GUI_Manager.PauseMenu.UI.Section_2_X, GUI_Manager.PauseMenu.UI.Content_Y, GUI_Manager.PauseMenu.UI.Section_W, size_h), Managers.GUI_Manager.Textures.black);
                section_x += content_margin;
                section_y += content_margin;
                float content_w = section_w - (2 * content_margin);

                GUI.TextField(new Rect(section_x, section_y, content_w, 40), "Affix", GUI_Manager.Styles.Content_Title());
                section_y += 40 + content_margin;
                                
                GUI.DrawTexture(new Rect(section_x, section_y, content_w, 40 + (2 * content_margin)), GUI_Manager.Textures.grey);
                section_y += content_margin;
                section_x += content_margin;
                float pos_x = section_x;
                float pos_y = section_y;
                float size_w = content_w - (2 * content_margin);

                InitScrollviewPositions();

                Main.logger_instance.Msg("Affix Content at X = " + section_x + ", Y = " + section_y);
                GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 65), 20), "Nb affixes",  Managers.GUI_Manager.Styles.TextField_Style());
                if (nb_affixs < 0) { nb_affixs = 0; }
                if (nb_affixs > max_nb_affixs) { nb_affixs = (int)max_nb_affixs; }
                GUI.Label(new Rect((pos_x + (size_w - 60)), (pos_y), 60, 20), nb_affixs.ToString(),  Managers.GUI_Manager.Styles.TextArea_Style());
                pos_y += 20 + content_margin;
                nb_affixs = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), nb_affixs, 0, max_nb_affixs));
                pos_y += 20 + content_margin;

                if (nb_affixs > 0)
                {
                    if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 1", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_0)))
                    {
                        Show_Affix_1 = false;
                        Show_Affix_2 = false;
                        Show_Affix_3 = false;
                        Show_Affix_4 = false;
                        Show_Affix_5 = false;
                        Show_Affix_0 = !Show_Affix_0;
                    }
                    pos_y += 40 + content_margin;
                    if (Show_Affix_0)
                    {
                        //Background
                        GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                        //Tier
                        pos_y += content_margin;
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_tier_0)
                        {
                            GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_0.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_0))) { override_tier_0 = !override_tier_0; }
                        string btn_affix_tier_label_0 = "Enable";
                        if (override_tier_0) { btn_affix_tier_label_0 = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label_0, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_tier_0)
                        {
                            affix_tier_0 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_0, 1, 7));
                            pos_y += 20 + content_margin;
                        }
                        //Value
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_value_0)
                        {
                            GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_0.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_0))) { override_value_0 = !override_value_0; }
                        string btn_affix_values_0 = "Enable";
                        if (override_value_0) { btn_affix_values_0 = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values_0, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_value_0)
                        {
                            affix_values_0 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_0, 0, 255));
                            pos_y += 20 + content_margin;
                        }
                        //Drop
                        if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                        {
                            seal.show_dropdown = false;

                            prefixs.slot_1.show_dropdown = false;
                            prefixs.slot_2.show_dropdown = false;

                            suffixs.slot_0.show_dropdown = false;
                            suffixs.slot_1.show_dropdown = false;
                            suffixs.slot_2.show_dropdown = false;

                            idols.slot_1.show_dropdown = false;
                            idols.slot_2.show_dropdown = false;
                            idols.slot_3.show_dropdown = false;
                            idols.slot_4.show_dropdown = false;
                            idols.slot_5.show_dropdown = false;

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
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), prefixs.dropdown_list[prefixs.slot_0.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        else
                        {
                            if ((idols.slot_0.dropdown_index < idols.dropdown_list.Length) && (idols.slot_0.dropdown_index > -1))
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_0.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        pos_y += affix_btn_size_h + (3 * content_margin);
                    }
                    else { prefixs.slot_0.show_dropdown = false; }
                }
                if (nb_affixs > 1)
                {
                    if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 2", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_1)))
                    {
                        Show_Affix_0 = false;
                        Show_Affix_2 = false;
                        Show_Affix_3 = false;
                        Show_Affix_4 = false;
                        Show_Affix_5 = false;
                        Show_Affix_1 = !Show_Affix_1;
                    }
                    pos_y += 40 + content_margin;
                    if (Show_Affix_1)
                    {
                        //Background
                        GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                        //Tier
                        pos_y += content_margin;
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_tier_1)
                        {
                            GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_1.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_1))) { override_tier_1 = !override_tier_1; }
                        string btn_affix_tier_label = "Enable";
                        if (override_tier_1) { btn_affix_tier_label = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_tier_1)
                        {
                            affix_tier_1 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_1, 1, 7));
                            pos_y += 20 + content_margin;
                        }
                        //Value
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_value_1)
                        {
                            GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_1.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_1))) { override_value_1 = !override_value_1; }
                        string btn_affix_values = "Enable";
                        if (override_value_1) { btn_affix_values = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_value_1)
                        {
                            affix_values_1 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_1, 0, 255));
                            pos_y += 20 + content_margin;
                        }
                        //Slot
                        if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                        {
                            seal.show_dropdown = false;

                            prefixs.slot_1.show_dropdown = false;
                            prefixs.slot_0.show_dropdown = false;
                            prefixs.slot_2.show_dropdown = false;

                            suffixs.slot_1.show_dropdown = false;
                            suffixs.slot_2.show_dropdown = false;

                            idols.slot_0.show_dropdown = false;
                            idols.slot_2.show_dropdown = false;
                            idols.slot_3.show_dropdown = false;
                            idols.slot_4.show_dropdown = false;
                            idols.slot_5.show_dropdown = false;

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
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), suffixs.dropdown_list[suffixs.slot_0.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        else
                        {
                            if ((idols.slot_1.dropdown_index < idols.dropdown_list.Length) && (idols.slot_1.dropdown_index > -1))
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_1.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        pos_y += affix_btn_size_h + (3 * content_margin);
                    }
                    else { suffixs.slot_0.show_dropdown = false; }
                }
                if (nb_affixs > 2)
                {
                    if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 3", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_2)))
                    {
                        Show_Affix_0 = false;
                        Show_Affix_1 = false;
                        Show_Affix_3 = false;
                        Show_Affix_4 = false;
                        Show_Affix_5 = false;
                        Show_Affix_2 = !Show_Affix_2;
                    }
                    pos_y += 40 + content_margin;
                    if (Show_Affix_2)
                    {
                        //Background
                        GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                        //Tier
                        pos_y += content_margin;
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_tier_2)
                        {
                            GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_2.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_2))) { override_tier_2 = !override_tier_2; }
                        string btn_affix_tier_label = "Enable";
                        if (override_tier_2) { btn_affix_tier_label = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_tier_2)
                        {
                            affix_tier_2 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_2, 1, 7));
                            pos_y += 20 + content_margin;
                        }
                        //Value
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_value_2)
                        {
                            GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_2.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_2))) { override_value_2 = !override_value_2; }
                        string btn_affix_values = "Enable";
                        if (override_value_2) { btn_affix_values = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_value_2)
                        {
                            affix_values_2 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_2, 0, 255));
                            pos_y += 20 + content_margin;
                        }
                        //Slot
                        if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                        {
                            seal.show_dropdown = false;

                            prefixs.slot_0.show_dropdown = false;
                            prefixs.slot_2.show_dropdown = false;

                            suffixs.slot_0.show_dropdown = false;
                            suffixs.slot_1.show_dropdown = false;
                            suffixs.slot_2.show_dropdown = false;

                            idols.slot_0.show_dropdown = false;
                            idols.slot_1.show_dropdown = false;
                            idols.slot_3.show_dropdown = false;
                            idols.slot_4.show_dropdown = false;
                            idols.slot_5.show_dropdown = false;

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
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), prefixs.dropdown_list[prefixs.slot_1.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        else
                        {
                            if ((idols.slot_2.dropdown_index < idols.dropdown_list.Length) && (idols.slot_2.dropdown_index > -1))
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_2.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        pos_y += affix_btn_size_h + (3 * content_margin);
                    }
                    else { prefixs.slot_1.show_dropdown = false; }
                }
                if (nb_affixs > 3)
                {
                    if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 4", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_3)))
                    {
                        Show_Affix_0 = false;
                        Show_Affix_1 = false;
                        Show_Affix_2 = false;
                        Show_Affix_4 = false;
                        Show_Affix_5 = false;
                        Show_Affix_3 = !Show_Affix_3;
                    }
                    pos_y += 40 + content_margin;
                    if (Show_Affix_3)
                    {
                        //Background
                        GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                        //Tier
                        pos_y += content_margin;
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_tier_3)
                        {
                            GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_3.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_3))) { override_tier_3 = !override_tier_3; }
                        string btn_affix_tier_label = "Enable";
                        if (override_tier_3) { btn_affix_tier_label = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_tier_3)
                        {
                            affix_tier_3 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_3, 1, 7));
                            pos_y += 20 + content_margin;
                        }
                        //Value
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_value_3)
                        {
                            GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_3.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_3))) { override_value_3 = !override_value_3; }
                        string btn_affix_values = "Enable";
                        if (override_value_3) { btn_affix_values = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_value_3)
                        {
                            affix_values_3 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_3, 0, 255));
                            pos_y += 20 + content_margin;
                        }
                        //
                        if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                        {
                            seal.show_dropdown = false;

                            prefixs.slot_0.show_dropdown = false;
                            prefixs.slot_1.show_dropdown = false;
                            prefixs.slot_2.show_dropdown = false;

                            suffixs.slot_0.show_dropdown = false;
                            suffixs.slot_2.show_dropdown = false;

                            idols.slot_0.show_dropdown = false;
                            idols.slot_1.show_dropdown = false;
                            idols.slot_2.show_dropdown = false;
                            idols.slot_4.show_dropdown = false;
                            idols.slot_5.show_dropdown = false;
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
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), suffixs.dropdown_list[suffixs.slot_1.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        else
                        {
                            if ((idols.slot_3.dropdown_index < idols.dropdown_list.Length) && (idols.slot_3.dropdown_index > -1))
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_3.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        pos_y += affix_btn_size_h + (3 * content_margin);
                    }
                    else { suffixs.slot_1.show_dropdown = false; }
                }
                if (nb_affixs > 4)
                {
                    if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 5", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_4)))
                    {
                        Show_Affix_0 = false;
                        Show_Affix_1 = false;
                        Show_Affix_2 = false;
                        Show_Affix_3 = false;
                        Show_Affix_5 = false;
                        Show_Affix_4 = !Show_Affix_4;
                    }
                    pos_y += 40 + content_margin;
                    if (Show_Affix_4)
                    {
                        //Background
                        GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                        //Tier
                        pos_y += content_margin;
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_tier_4)
                        {
                            GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_4.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_4))) { override_tier_4 = !override_tier_4; }
                        string btn_affix_tier_label = "Enable";
                        if (override_tier_3) { btn_affix_tier_label = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_tier_4)
                        {
                            affix_tier_4 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_4, 1, 7));
                            pos_y += 20 + content_margin;
                        }
                        //Value
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_value_4)
                        {
                            GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_4.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_4))) { override_value_4 = !override_value_4; }
                        string btn_affix_values = "Enable";
                        if (override_value_4) { btn_affix_values = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_value_4)
                        {
                            affix_values_4 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_4, 0, 255));
                            pos_y += 20 + content_margin;
                        }
                        //
                        if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                        {
                            seal.show_dropdown = false;

                            prefixs.slot_0.show_dropdown = false;
                            prefixs.slot_1.show_dropdown = false;

                            suffixs.slot_0.show_dropdown = false;
                            suffixs.slot_1.show_dropdown = false;
                            suffixs.slot_2.show_dropdown = false;

                            idols.slot_1.show_dropdown = false;
                            idols.slot_2.show_dropdown = false;
                            idols.slot_3.show_dropdown = false;
                            idols.slot_5.show_dropdown = false;
                            if (!type.is_idol)
                            {
                                idols.slot_4.show_dropdown = false;
                                prefixs.slot_2.show_dropdown = !prefixs.slot_2.show_dropdown;
                            }
                            else
                            {
                                prefixs.slot_2.show_dropdown = false;
                                idols.slot_4.show_dropdown = !idols.slot_4.show_dropdown;
                            }
                        }
                        if (!type.is_idol)
                        {
                            if ((prefixs.slot_2.dropdown_index < prefixs.dropdown_list.Length) && (prefixs.slot_2.dropdown_index > -1))
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), prefixs.dropdown_list[prefixs.slot_2.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        else
                        {
                            if ((idols.slot_4.dropdown_index < idols.dropdown_list.Length) && (idols.slot_4.dropdown_index > -1))
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_4.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        pos_y += affix_btn_size_h + (3 * content_margin);
                    }
                    else { prefixs.slot_2.show_dropdown = false; }
                }
                if (nb_affixs > 5)
                {
                    if (GUI.Button(new Rect(section_x - content_margin, pos_y, content_w, 40), "Prefix / Suffix 6", Managers.GUI_Manager.Styles.Content_Enable_Button(Show_Affix_5)))
                    {
                        Show_Affix_0 = false;
                        Show_Affix_1 = false;
                        Show_Affix_2 = false;
                        Show_Affix_3 = false;
                        Show_Affix_4 = false;
                        Show_Affix_5 = !Show_Affix_5;
                    }
                    pos_y += 40 + content_margin;
                    if (Show_Affix_5)
                    {
                        //Background
                        GUI.DrawTexture(new Rect(section_x - content_margin, pos_y, content_w, affix_h), GUI_Manager.Textures.grey);
                        //Tier
                        pos_y += content_margin;
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Tier", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_tier_5)
                        {
                            GUI.Label(new Rect((pos_x + (size_w - 120)), pos_y, 60, 30), affix_tier_5.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect((pos_x + (size_w - 60)), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_tier_5))) { override_tier_5 = !override_tier_5; }
                        string btn_affix_tier_label = "Enable";
                        if (override_tier_5) { btn_affix_tier_label = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_tier_label, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_tier_5)
                        {
                            affix_tier_5 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_tier_5, 1, 7));
                            pos_y += 20 + content_margin;
                        }
                        //Value
                        GUI.Label(new Rect(pos_x + 5, pos_y, (size_w - 120), 30), "Override Values", Managers.GUI_Manager.Styles.TextField_Style());
                        if (override_value_5)
                        {
                            GUI.Label(new Rect(pos_x + (size_w - 120), pos_y, 60, 30), affix_values_5.ToString(), Managers.GUI_Manager.Styles.TextArea_Style());
                        }
                        if (GUI.Button(new Rect(pos_x + (size_w - 60), pos_y, 60, 30), "", Managers.GUI_Manager.Styles.Button_Style(override_value_5))) { override_value_5 = !override_value_5; }
                        string btn_affix_values = "Enable";
                        if (override_value_5) { btn_affix_values = "Disable"; }
                        GUI.Label(new Rect(pos_x + (size_w - 60) + 5, pos_y, 50, 30), btn_affix_values, Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                        pos_y += 30 + content_margin;
                        if (override_value_5)
                        {
                            affix_values_5 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, size_w, 20), affix_values_5, 0, 255));
                            pos_y += 20 + content_margin;
                        }
                        //
                        if (GUI.Button(new Rect(pos_x, pos_y, size_w, affix_btn_size_h), ""))
                        {
                            seal.show_dropdown = false;

                            prefixs.slot_0.show_dropdown = false;
                            prefixs.slot_1.show_dropdown = false;
                            prefixs.slot_2.show_dropdown = false;

                            suffixs.slot_0.show_dropdown = false;
                            suffixs.slot_1.show_dropdown = false;

                            idols.slot_0.show_dropdown = false;
                            idols.slot_1.show_dropdown = false;
                            idols.slot_2.show_dropdown = false;
                            idols.slot_3.show_dropdown = false;
                            idols.slot_4.show_dropdown = false;

                            if (!type.is_idol)
                            {
                                idols.slot_5.show_dropdown = false;
                                suffixs.slot_2.show_dropdown = !suffixs.slot_2.show_dropdown;
                            }
                            else
                            {
                                suffixs.slot_2.show_dropdown = false;
                                idols.slot_5.show_dropdown = !idols.slot_5.show_dropdown;
                            }
                        }
                        if (!type.is_idol)
                        {
                            if ((suffixs.slot_2.dropdown_index < suffixs.dropdown_list.Length) && (suffixs.slot_2.dropdown_index > -1))
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), suffixs.dropdown_list[suffixs.slot_2.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        else
                        {
                            if ((idols.slot_5.dropdown_index < idols.dropdown_list.Length) && (idols.slot_5.dropdown_index > -1))
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), idols.dropdown_list[idols.slot_5.dropdown_index], Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                            else
                            {
                                GUI.Label(new Rect(pos_x + 5, pos_y, size_w - 10, affix_btn_size_h), "Chosse a Prefix / Suffix (Idol)", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            }
                        }
                        //pos_y += affix_btn_size_h + (2 * content_margin);
                    }
                    else { suffixs.slot_2.show_dropdown = false; }
                }
            }
            public static float MenuUI(float pos_x, float pos_y, int w)
            {
                float result = 0;
                if (seal.enable)
                {
                    if (type.dropdown_index < type.dropdown_list.Length)
                    {
                        int type_id = type.GetIdFromName(type.dropdown_list[type.dropdown_index]);
                        if (type_id > 33) { affixs.enable = false; }
                        else
                        {
                            if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                            {
                                base_item.show_dropdown = false;
                                unique.show_dropdown = false;
                                set.show_dropdown = false;
                                type.show_dropdown = false;
                                rarity.show_dropdown = false;

                                Hide();
                                affix_show = false;
                                seal.show = !seal.show;
                            }
                            GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Seal",  Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 35;
                            result += 35;
                        }
                    }
                }
                else { seal.show_dropdown = false; }

                if (affixs.enable)
                {
                    if (type.dropdown_index < type.dropdown_list.Length)
                    {
                        int type_id = type.GetIdFromName(type.dropdown_list[type.dropdown_index]);
                        if (type_id > 33) { affixs.enable = false; }
                        else
                        {
                            if (GUI.Button(new Rect(pos_x, pos_y, w, 30), ""))
                            {
                                base_item.show_dropdown = false;
                                unique.show_dropdown = false;
                                set.show_dropdown = false;
                                type.show_dropdown = false;
                                rarity.show_dropdown = false;
                                seal.show = false;
                                Hide();
                                affix_show = !affix_show;
                            }
                            GUI.Label(new Rect(pos_x + 5, pos_y, w - 10, 30), "Affixs", Managers.GUI_Manager.Styles.DropdownLabelMidle_Style());
                            pos_y += 35;
                            result += 35;
                        }
                    }
                }
                else
                {
                    prefixs.slot_0.show_dropdown = false;
                    prefixs.slot_1.show_dropdown = false;
                    suffixs.slot_0.show_dropdown = false;
                    suffixs.slot_1.show_dropdown = false;
                    idols.slot_0.show_dropdown = false;
                    idols.slot_1.show_dropdown = false;
                    idols.slot_2.show_dropdown = false;
                    idols.slot_3.show_dropdown = false;
                }

                return result;
            }
            public static bool EnableFromType()
            {
                bool result = false;
                if (type.dropdown_index < 35) { result = true; }

                return result;
            }

            public static void InitList()
            {
                prefixs.dropdown_list = null;
                suffixs.dropdown_list = null;
                idols.dropdown_list = null;
                seal.dropdown_list = null;
                seal.idol_dropdown_list = null;
                
                System.Collections.Generic.List<string> idol_names = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<string> affix_names = new System.Collections.Generic.List<string>();
                bool affix_error = false;
                try { if (AffixList.instance == null) { affix_error = true; } }
                catch { }
                if (!affix_error)
                {
                    foreach (AffixList.SingleAffix affix in AffixList.instance.singleAffixes)
                    {
                        if (affix.rollsOn == AffixList.RollsOn.Idols) { idol_names.Add(affix.affixName); } //affixName
                        else { affix_names.Add(affix.affixName); }
                    }
                    foreach (AffixList.MultiAffix affix in AffixList.instance.multiAffixes)
                    {
                        if (affix.rollsOn == AffixList.RollsOn.Idols) { idol_names.Add(affix.affixName); }
                        else { affix_names.Add(affix.affixName); }
                    }
                    idol_names.Sort();
                    idols.dropdown_list = new string[idol_names.Count];
                    seal.idol_dropdown_list = new string[idol_names.Count];
                    int j = 0;
                    foreach (string name in idol_names)
                    {
                        idols.dropdown_list[j] = name;
                        seal.idol_dropdown_list[j] = name;
                        j++;
                    }
                    idol_names.Clear();

                    affix_names.Sort();
                    prefixs.dropdown_list = new string[affix_names.Count];
                    suffixs.dropdown_list = new string[affix_names.Count];
                    seal.dropdown_list = new string[affix_names.Count];
                    j = 0;
                    foreach (string name in affix_names)
                    {
                        prefixs.dropdown_list[j] = name;
                        suffixs.dropdown_list[j] = name;
                        seal.dropdown_list[j] = name;
                        j++;
                    }
                    affix_names.Clear();
                }
                else { Main.logger_instance.Error("ForceDrop:InitList -> can't get affixlist"); }
            }
            public static void Hide()
            {
                seal.show_dropdown = false;

                affixs.prefixs.slot_0.show_dropdown = false;
                affixs.prefixs.slot_1.show_dropdown = false;
                affixs.prefixs.slot_2.show_dropdown = false;

                affixs.suffixs.slot_0.show_dropdown = false;
                affixs.suffixs.slot_1.show_dropdown = false;
                affixs.suffixs.slot_2.show_dropdown = false;

                affixs.idols.slot_0.show_dropdown = false;
                affixs.idols.slot_1.show_dropdown = false;
                affixs.idols.slot_2.show_dropdown = false;
                affixs.idols.slot_3.show_dropdown = false;
                affixs.idols.slot_4.show_dropdown = false;
                affixs.idols.slot_5.show_dropdown = false;
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
            public static void InitScrollviewPositions()
            {
                
                


                affixs.dropdown_rect.width = GUI_Manager.PauseMenu.UI.Section_W - (2 * GUI_Manager.PauseMenu.UI.content_margin);
                affixs.dropdown_rect.height = (Screen.height * 80) / 100;

                affixs.seal.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.seal.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;

                affixs.prefixs.slot_0.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.prefixs.slot_0.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                affixs.prefixs.slot_1.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.prefixs.slot_1.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                affixs.prefixs.slot_2.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.prefixs.slot_2.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;

                affixs.suffixs.slot_0.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.suffixs.slot_0.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                affixs.suffixs.slot_1.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.suffixs.slot_1.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                affixs.suffixs.slot_2.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.suffixs.slot_2.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;

                affixs.idols.slot_0.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.idols.slot_0.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                affixs.idols.slot_1.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.idols.slot_1.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                affixs.idols.slot_2.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.idols.slot_2.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                affixs.idols.slot_3.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.idols.slot_3.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                affixs.idols.slot_4.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.idols.slot_4.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
                affixs.idols.slot_5.pos_x = GUI_Manager.PauseMenu.UI.Section_3_X;
                affixs.idols.slot_5.pos_y = GUI_Manager.PauseMenu.UI.Content_Y + 50;
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
                //Main.logger_instance.Msg("GetIdFromName : " + name + ", Id = " + result);

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
                        if ((prefixs.slot_0.dropdown_index > -1) && (ForceDrop.affixs.prefixs.slot_0.dropdown_index < ForceDrop.affixs.prefixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.prefixs.dropdown_list[ForceDrop.affixs.prefixs.slot_0.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 1)
                    {
                        if ((ForceDrop.affixs.suffixs.slot_0.dropdown_index > -1) && (ForceDrop.affixs.suffixs.slot_0.dropdown_index < ForceDrop.affixs.suffixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.suffixs.dropdown_list[ForceDrop.affixs.suffixs.slot_0.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 2)
                    {
                        if ((ForceDrop.affixs.prefixs.slot_1.dropdown_index > -1) && (ForceDrop.affixs.prefixs.slot_1.dropdown_index < ForceDrop.affixs.prefixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.prefixs.dropdown_list[ForceDrop.affixs.prefixs.slot_1.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 3)
                    {
                        if ((ForceDrop.affixs.suffixs.slot_1.dropdown_index > -1) && (ForceDrop.affixs.suffixs.slot_1.dropdown_index < ForceDrop.affixs.suffixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.suffixs.dropdown_list[ForceDrop.affixs.suffixs.slot_1.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 4)
                    {
                        if ((ForceDrop.affixs.prefixs.slot_2.dropdown_index > -1) && (ForceDrop.affixs.prefixs.slot_2.dropdown_index < ForceDrop.affixs.prefixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.prefixs.dropdown_list[ForceDrop.affixs.prefixs.slot_2.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 5)
                    {
                        if ((ForceDrop.affixs.suffixs.slot_2.dropdown_index > -1) && (ForceDrop.affixs.suffixs.slot_2.dropdown_index < ForceDrop.affixs.suffixs.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.suffixs.dropdown_list[ForceDrop.affixs.suffixs.slot_2.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                }
                else
                {
                    if (nb_affixs > 0)
                    {
                        if ((ForceDrop.affixs.idols.slot_0.dropdown_index > -1) && (ForceDrop.affixs.idols.slot_0.dropdown_index < ForceDrop.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.idols.dropdown_list[ForceDrop.affixs.idols.slot_0.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 1)
                    {
                        if ((ForceDrop.affixs.idols.slot_1.dropdown_index > -1) && (ForceDrop.affixs.idols.slot_1.dropdown_index < ForceDrop.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.idols.dropdown_list[ForceDrop.affixs.idols.slot_1.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 2)
                    {
                        if ((ForceDrop.affixs.idols.slot_2.dropdown_index > -1) && (ForceDrop.affixs.idols.slot_2.dropdown_index < ForceDrop.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.idols.dropdown_list[ForceDrop.affixs.idols.slot_2.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 3)
                    {
                        if ((ForceDrop.affixs.idols.slot_3.dropdown_index > -1) && (ForceDrop.affixs.idols.slot_3.dropdown_index < ForceDrop.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.idols.dropdown_list[ForceDrop.affixs.idols.slot_3.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 4)
                    {
                        if ((ForceDrop.affixs.idols.slot_4.dropdown_index > -1) && (ForceDrop.affixs.idols.slot_4.dropdown_index < ForceDrop.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.idols.dropdown_list[ForceDrop.affixs.idols.slot_4.dropdown_index]));
                        }
                        else { affixs_ids.Add(-1); }
                    }
                    if (nb_affixs > 5)
                    {
                        if ((ForceDrop.affixs.idols.slot_5.dropdown_index > -1) && (ForceDrop.affixs.idols.slot_5.dropdown_index < ForceDrop.affixs.idols.dropdown_list.Length))
                        {
                            affixs_ids.Add(ForceDrop.affixs.GetIdFromName(ForceDrop.affixs.idols.dropdown_list[ForceDrop.affixs.idols.slot_5.dropdown_index]));
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
                float result = 0;
                if (enable)
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Forgin Potencial",  Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), value.ToString(),  Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value, 0, 255));
                    pos_y += 25;
                    result += 50;
                }

                return result;
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
            public static float value_0 = 255;
            public static float value_1 = 255;
            public static float value_2 = 255;
            public static float value_3 = 255;
            public static float value_4 = 255;
            public static float value_5 = 255;
            public static float value_6 = 255;
            public static float value_7 = 255;
            public static int h = 400;

            public static float UI(float pos_x, float pos_y, int w)
            {
                float result = 0;
                if (enable)
                {
                    float temp_value_0 = (value_0 / 255) * 100;
                    int temp_int_value_0 = (int)temp_value_0;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Unique Modifier 0",  Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_0.ToString() + " %",  Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_0 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_0, 0, 255));
                    pos_y += 25;

                    float temp_value_1 = (value_1 / 255) * 100;
                    int temp_int_value_1 = (int)temp_value_1;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Unique Modifier 1", Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_1.ToString() + " %", Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_1 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_1, 0, 255));
                    pos_y += 25;

                    float temp_value_2 = (value_2 / 255) * 100;
                    int temp_int_value_2 = (int)temp_value_2;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Unique Modifier 2", Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_2.ToString() + " %", Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_2 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_2, 0, 255));
                    pos_y += 25;

                    float temp_value_3 = (value_3 / 255) * 100;
                    int temp_int_value_3 = (int)temp_value_3;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Unique Modifier 3", Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_3.ToString() + " %", Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_3 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_3, 0, 255));
                    pos_y += 25;

                    float temp_value_4 = (value_4 / 255) * 100;
                    int temp_int_value_4 = (int)temp_value_4;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Unique Modifier 4", Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_4.ToString() + " %", Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_4 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_4, 0, 255));
                    pos_y += 25;

                    float temp_value_5 = (value_5 / 255) * 100;
                    int temp_int_value_5 = (int)temp_value_5;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Unique Modifier 5", Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_5.ToString() + " %", Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_5 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_5, 0, 255));
                    pos_y += 25;

                    float temp_value_6 = (value_6 / 255) * 100;
                    int temp_int_value_6 = (int)temp_value_6;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Unique Modifier 6", Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_6.ToString() + " %", Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_6 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_6, 0, 255));
                    pos_y += 25;

                    float temp_value_7 = (value_7 / 255) * 100;
                    int temp_int_value_ = (int)temp_value_7;
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Unique Modifier 7", Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), temp_int_value_.ToString() + " %", Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value_7 = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value_7, 0, 255));
                    pos_y += 25;
                    result += 400;
                }

                return result;
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
                float result = 0;
                if (enable)
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Legendary Potencial",  Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), value.ToString(),  Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value, 0, 4));
                    pos_y += 25;
                    result += 50;
                }

                return result;
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
                float result = 0;
                if (enable)
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Weaver Will",  Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), value.ToString(),  Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value, 0, 28));
                    pos_y += 25;
                    result += 50;
                }

                return result;
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
                float result = 0;
                if (enable)
                {
                    GUI.Label(new Rect(pos_x + 5, pos_y, (w - 65), 20), "Quantity",  Managers.GUI_Manager.Styles.TextField_Style());
                    GUI.Label(new Rect((pos_x + (w - 60)), pos_y, 60, 20), value.ToString(),  Managers.GUI_Manager.Styles.TextArea_Style());
                    pos_y += 25;
                    value = System.Convert.ToInt32(GUI.HorizontalSlider(new Rect(pos_x, pos_y, w, 20), value, 1, 99));
                    pos_y += 25;
                    result += 50;
                }

                return result;
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
            public static int h = 45;

            public static float UI(float pos_x, float pos_y, int w)
            {
                if (drop.enable)
                {
                    if (GUI.Button(new Rect(pos_x, pos_y, w, 40), "Drop",  Managers.GUI_Manager.Styles.Content_Enable_Button(true)))
                    {
                        drop.ForceDrop();
                    }
                }

                return 45;
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
            public class Search
            {
                public static int SealIndex(ref ItemDataUnpacked __0)
                {
                    int result = 0;
                    foreach (ItemAffix affix in __0.affixes)
                    {
                        if (affix.isSealedAffix) { break; }
                        result++;
                    }

                    return result;
                }
            }
            public class Add
            {
                public static void Affix(ref ItemDataUnpacked item_data, bool item_sealed)
                {
                    item_data.affixes.Add(new ItemAffix
                    {
                        affixName = "",
                        affixId = (byte)UnityEngine.Random.RandomRange(0, 150),
                        affixTier = (byte)UnityEngine.Random.RandomRange(0, 7),
                        affixRoll = (byte)UnityEngine.Random.RandomRange(0, 255),
                        affixType = 0,
                        affixCountForSetProps = 0,
                        affixTitle = "",
                        isSealedAffix = item_sealed,
                        specialAffixType = 0,
                        titleType = 0
                    });
                    if (item_sealed)
                    {
                        item_data.affixes[item_data.affixes.Count - 1].isSealedAffix = true;
                        item_data.hasSealedAffix = true;
                    }
                    item_data.RefreshIDAndValues();
                }
            }
            public class Roll
            {
                public static void item_implicits(ref ItemDataUnpacked item_data)
                {
                    for (int i = 0; i < item_data.implicitRolls.Count; i++)
                    {
                        if (i == 0) { item_data.implicitRolls[i] = (byte)implicits.value_0; }
                        else if (i == 1) { item_data.implicitRolls[i] = (byte)implicits.value_1; }
                        else if (i == 2) { item_data.implicitRolls[i] = (byte)implicits.value_2; }
                        if (ShowDebug)
                        {
                            Main.logger_instance.Msg("ForceDrop : Implicit[" + i + "] set to " + Managers.GUI_Manager.GeneralFunctions.AffixRollPercent(item_data.implicitRolls[i]));
                        }
                    }
                    item_data.RefreshIDAndValues();
                }
                public static void item_forgin_potencial(ref ItemDataUnpacked item_data)
                {
                    item_data.forgingPotential = (byte)forgin_potencial.value;
                    if (ShowDebug)
                    {
                        Main.logger_instance.Msg("ForceDrop : Forgin Potencial set to " + item_data.forgingPotential);
                    }
                    item_data.RefreshIDAndValues();
                }
                public static void item_affixes(ref ItemDataUnpacked item_data)
                {
                    int nb_affixs = affixs.nb_affixs;   
                    if ((affixs.seal.add) && (item_data.rarity < 7)) { nb_affixs++; }
                    else { affixs.seal.add = false; }
                    item_data.affixes = new Il2CppSystem.Collections.Generic.List<ItemAffix>();
                    //Main.logger_instance.Msg("ForceDrop : Reset Affixs : Nb_affix = " + item_data.affixes.Count + " / " + nb_affixs);
                    item_data.RefreshIDAndValues();
                    for (int z = 0; z < nb_affixs; z++)
                    {                        
                        if ((affixs.seal.add) && (z == 0)) { Add.Affix(ref item_data, true); }
                        else { Add.Affix(ref item_data, false); }
                    }
                    //item_data.RefreshIDAndValues();
                    int i = 0;
                    //Main.logger_instance.Msg("Affixes : " + item_data.affixes.Count + " / " + nb_affixs);
                    foreach (ItemAffix aff in item_data.affixes)
                    {
                        if (aff.isSealedAffix)
                        {
                            //Main.logger_instance.Msg("Set Sealed");
                            aff.affixTier = (byte)(affixs.seal.tier - 1);
                            if (affixs.seal.override_value) { aff.affixRoll = (byte)affixs.seal.values; }
                            else { aff.affixRoll = (byte)UnityEngine.Random.RandomRangeInt(0, 256); }
                            string affix_name = "";
                            if (type.is_idol)
                            {
                                if ((affixs.seal.override_affix) && (affixs.seal.idol_dropdown_index > -1))
                                {
                                    affix_name = affixs.seal.idol_dropdown_list[affixs.seal.idol_dropdown_index];                                    
                                }
                                else
                                {
                                    int nb = affixs.seal.idol_dropdown_list.Length;
                                    int result = UnityEngine.Random.RandomRangeInt(0, nb);
                                    if ((result < -1) && (result < nb))
                                    {
                                        affix_name = affixs.seal.idol_dropdown_list[result];
                                    }
                                }
                            }
                            else
                            {
                                if ((affixs.seal.override_affix) && (affixs.seal.dropdown_index > -1))
                                {
                                    affix_name = affixs.seal.dropdown_list[affixs.seal.dropdown_index];                                    
                                }
                                else
                                {
                                    int nb = affixs.seal.dropdown_list.Length;
                                    int result = UnityEngine.Random.RandomRangeInt(0, nb);
                                    if ((result < -1) && (result < nb))
                                    {
                                        affix_name = affixs.seal.dropdown_list[result];
                                    }
                                }
                            }
                            //Main.logger_instance.Msg("affix_name = " + affix_name);
                            if (affix_name != "")
                            {
                                aff.affixId = (ushort)affixs.GetIdFromName(affix_name);
                                aff.affixName = affix_name;
                            }
                            else { aff.affixId = (ushort)UnityEngine.Random.RandomRangeInt(1, 300); }
                                                        
                            if (ShowDebug)
                            {
                                Main.logger_instance.Msg("ForceDrop : Seal : " +
                                "ID = " + aff.affixId +
                                ", Name = " + aff.affixName +
                                ", Tier = " + (aff.affixTier + 1) +
                                ", Roll = " + aff.affixRoll +
                                "(" + GUI_Manager.GeneralFunctions.AffixRollPercent(aff.affixRoll) + ")");
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                if (affixs.override_tier_0) { aff.affixTier = (byte)(affixs.affix_tier_0 - 1); }
                                else { aff.affixTier = (byte)UnityEngine.Random.RandomRangeInt(1, 7); }
                                if (affixs.override_value_0) { aff.affixRoll = (byte)affixs.affix_values_0; }
                                else { aff.affixRoll = (byte)UnityEngine.Random.RandomRangeInt(0, 256); }
                            }
                            else if (i == 1)
                            {
                                if (affixs.override_tier_1) { aff.affixTier = (byte)(affixs.affix_tier_1 - 1); }
                                else { aff.affixTier = (byte)UnityEngine.Random.RandomRangeInt(1, 7); }
                                if (affixs.override_value_1) { aff.affixRoll = (byte)affixs.affix_values_1; }
                                else { aff.affixRoll = (byte)UnityEngine.Random.RandomRangeInt(0, 256); }
                            }
                            else if (i == 2)
                            {
                                if (affixs.override_tier_2) { aff.affixTier = (byte)(affixs.affix_tier_2 - 1); }
                                else { aff.affixTier = (byte)UnityEngine.Random.RandomRangeInt(1, 7); }
                                if (affixs.override_value_2) { aff.affixRoll = (byte)affixs.affix_values_2; }
                                else { aff.affixRoll = (byte)UnityEngine.Random.RandomRangeInt(0, 256); }
                            }
                            else if (i == 3)
                            {
                                if (affixs.override_tier_3) { aff.affixTier = (byte)(affixs.affix_tier_3 - 1); }
                                else { aff.affixTier = (byte)UnityEngine.Random.RandomRangeInt(1, 7); }
                                if (affixs.override_value_3) { aff.affixRoll = (byte)affixs.affix_values_3; }
                                else { aff.affixRoll = (byte)UnityEngine.Random.RandomRangeInt(0, 256); }
                            }
                            else if (i == 4)
                            {
                                if (affixs.override_tier_4) { aff.affixTier = (byte)(affixs.affix_tier_4 - 1); }
                                else { aff.affixTier = (byte)UnityEngine.Random.RandomRangeInt(1, 7); }
                                if (affixs.override_value_4) { aff.affixRoll = (byte)affixs.affix_values_4; }
                                else { aff.affixRoll = (byte)UnityEngine.Random.RandomRangeInt(0, 256); }
                            }
                            else if (i == 5)
                            {
                                if (affixs.override_tier_5) { aff.affixTier = (byte)(affixs.affix_tier_5 - 1); }
                                else { aff.affixTier = (byte)UnityEngine.Random.RandomRangeInt(1, 7); }
                                if (affixs.override_value_5) { aff.affixRoll = (byte)affixs.affix_values_5; }
                                else { aff.affixRoll = (byte)UnityEngine.Random.RandomRangeInt(0, 256); }
                            }

                            bool idol = false;
                            if ((item_data.itemType > 24) && (item_data.itemType < 34)) { idol = true; }
                            System.Collections.Generic.List<int> list = affixs.GetIdList(idol);
                            if (i < list.Count)
                            {
                                int value = list[i];
                                if (value == -1)
                                {
                                    string affix_name = "";
                                    if (type.is_idol)
                                    {
                                        int nb = affixs.seal.idol_dropdown_list.Length;
                                        int result = UnityEngine.Random.RandomRangeInt(0, nb);
                                        if ((result < -1) && (result < nb))
                                        {
                                            affix_name = affixs.seal.idol_dropdown_list[result];
                                        }
                                    }
                                    else
                                    {
                                        int nb = affixs.seal.dropdown_list.Length;
                                        int result = UnityEngine.Random.RandomRangeInt(0, nb);
                                        if ((result < -1) && (result < nb))
                                        {
                                            affix_name = affixs.seal.dropdown_list[result];
                                        }
                                    }
                                    if (affix_name != "")
                                    {
                                        aff.affixId = (ushort)affixs.GetIdFromName(affix_name);
                                        aff.affixName = affix_name;
                                    }
                                    else { aff.affixId = (ushort)UnityEngine.Random.RandomRangeInt(1, 300); }
                                }
                                else if (value != -1)
                                {
                                    aff.affixId = (ushort)value;
                                    aff.affixName = affixs.GetNameFromId(value);
                                }
                                else
                                {
                                    string affix_name = "";
                                    if (type.is_idol)
                                    {
                                        int nb = affixs.seal.idol_dropdown_list.Length;
                                        int result = UnityEngine.Random.RandomRangeInt(0, nb);
                                        if ((result < -1) && (result < nb))
                                        {
                                            affix_name = affixs.seal.idol_dropdown_list[result];
                                        }
                                    }
                                    else
                                    {
                                        int nb = affixs.seal.dropdown_list.Length;
                                        int result = UnityEngine.Random.RandomRangeInt(0, nb);
                                        if ((result < -1) && (result < nb))
                                        {
                                            affix_name = affixs.seal.dropdown_list[result];
                                        }
                                    }
                                    if (affix_name != "")
                                    {
                                        aff.affixId = (ushort)affixs.GetIdFromName(affix_name);
                                        aff.affixName = affix_name;
                                    }
                                    else { aff.affixId = (ushort)UnityEngine.Random.RandomRangeInt(1, 300); }
                                }
                            }
                            if (ShowDebug)
                            {
                                Main.logger_instance.Msg("ForceDrop : Affixs : " + i +
                                " : ID = " + aff.affixId +
                                ", Name = " + aff.affixName +
                                ", Tier = " + (aff.affixTier + 1) +
                                ", Roll = " + aff.affixRoll +
                                "(" + GUI_Manager.GeneralFunctions.AffixRollPercent(aff.affixRoll) + ")");
                            }
                            i++;
                        }
                    }
                    item_data.RefreshIDAndValues();
                }
                public static void item_unique_mods(ref ItemDataUnpacked item_data)
                {
                    for (int k = 0; k < item_data.uniqueRolls.Count; k++)
                    {
                        if (k == 0) { item_data.uniqueRolls[k] = (byte)unique_mods.value_0; }
                        else if (k == 1) { item_data.uniqueRolls[k] = (byte)unique_mods.value_1; }
                        else if (k == 2) { item_data.uniqueRolls[k] = (byte)unique_mods.value_2; }
                        else if (k == 3) { item_data.uniqueRolls[k] = (byte)unique_mods.value_3; }
                        else if (k == 4) { item_data.uniqueRolls[k] = (byte)unique_mods.value_4; }
                        else if (k == 5) { item_data.uniqueRolls[k] = (byte)unique_mods.value_5; }
                        else if (k == 6) { item_data.uniqueRolls[k] = (byte)unique_mods.value_6; }
                        else if (k == 7) { item_data.uniqueRolls[k] = (byte)unique_mods.value_7; }
                        if (ShowDebug)
                        {
                            Main.logger_instance.Msg("ForceDrop : Unique Roll[" + k + "] set to " + Managers.GUI_Manager.GeneralFunctions.AffixRollPercent(item_data.uniqueRolls[k]));
                        }
                    }
                    item_data.RefreshIDAndValues();
                }
                public static void item_legendary_potencial(ref ItemDataUnpacked item_data)
                {
                    item_data.legendaryPotential = (byte)legenday_potencial.value;
                    if (Mods.ForceDrop.ForceDrop.ShowDebug)
                    {
                        Main.logger_instance.Msg("ForceDrop : legendary Potential set to " + item_data.legendaryPotential);
                    }
                    item_data.RefreshIDAndValues();
                }
                public static void item_weaver_will(ref ItemDataUnpacked item_data)
                {
                    item_data.weaversWill = (byte)weaver_wil.value;
                    if (Mods.ForceDrop.ForceDrop.ShowDebug)
                    {
                        Main.logger_instance.Msg("ForceDrop : Weaver Will set to " + item_data.weaversWill);
                    }
                    item_data.RefreshIDAndValues();
                }
            }
            
            public static void ForceDrop()
            {
                GroundItemManager ground_item_manager = null;
                foreach (UnityEngine.Object obj in UnityEngine.Object.FindObjectsOfType<GroundItemManager>())
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
                        if (ShowDebug)
                        {
                            Main.logger_instance.Msg("");
                            Main.logger_instance.Msg("ForceDrop : Generate new Item");
                        }
                        int item_base_id = 0;
                        int real_rarity = 0;
                        int item_rarity = 0;
                        int unique_id = 0;
                        int item_sockets = 0;
                        int legendary_potencial = 0;
                        if (rarity.dropdown_index == 0)
                        {
                            item_base_id = base_item.GetIdFromName(type.dropdown_list[type.dropdown_index], base_item.dropdown_list[base_item.dropdown_index]);                            
                            real_rarity = affixs.nb_affixs;                            
                            int rar = real_rarity;
                            if (rar > 4) { rar = 4; }                           
                            item_rarity = rar;
                        }
                        else if (rarity.dropdown_index == 1)
                        {
                            real_rarity = 7;
                            item_rarity = 7;
                            item_base_id = unique.GetBaseIdFromName(unique.dropdown_list[unique.dropdown_index]);
                            unique_id = unique.GetIdFromName(unique.dropdown_list[unique.dropdown_index]);
                        }
                        else if (rarity.dropdown_index == 2)
                        {
                            real_rarity = 8;
                            item_rarity = 8;
                            item_base_id = unique.GetBaseIdFromName(set.dropdown_list[set.dropdown_index]);
                            unique_id = unique.GetIdFromName(set.dropdown_list[set.dropdown_index]);
                        }
                        else if (rarity.dropdown_index == 3)
                        {
                            real_rarity = 9;
                            int rar = affixs.nb_affixs;
                            item_base_id = unique.GetBaseIdFromName(legendary.dropdown_list[legendary.dropdown_index]);
                            unique_id = unique.GetIdFromName(legendary.dropdown_list[legendary.dropdown_index]);                            
                            affixs.seal.add = false;
                        }
                        else
                        {
                            item_base_id = base_item.GetIdFromName(type.dropdown_list[type.dropdown_index], base_item.dropdown_list[base_item.dropdown_index]);
                            real_rarity = 0;
                            item_rarity = 0;
                        }
                        if (quantity.value < 1) { quantity.value = 1; }

                        if (ShowDebug)
                        {
                            Main.logger_instance.Msg("ForceDrop : With Type = " + type.GetItemType());
                            Main.logger_instance.Msg("ForceDrop : With Id to : " + item_base_id);
                            Main.logger_instance.Msg("ForceDrop : With Rarity to : " + item_rarity);
                            Main.logger_instance.Msg("ForceDrop : With NbAffixs to : " + item_sockets);
                        }

                        ItemDataUnpacked item = new ItemDataUnpacked
                        {
                            LvlReq = 0,
                            classReq = ItemList.ClassRequirement.Any,
                            itemType = (byte)type.GetItemType(),
                            subType = (ushort)item_base_id,
                            rarity = (byte)item_rarity,
                            sockets = (byte)0,
                            uniqueID = (ushort)unique_id,
                            legendaryPotential = (byte)legendary_potencial,
                            hasSealedAffix = false//contain_seal
                            
                        };
                        if (ShowDebug)
                        {
                            Main.logger_instance.Msg("ForceDrop : Item Generated");
                            Main.logger_instance.Msg("ForceDrop : Set Implicits");
                        }

                        if (implicits.enable) { Roll.item_implicits(ref item); }
                        if ((forgin_potencial.enable) && (((item_rarity < 5) && (real_rarity != 9))))
                        {
                            Roll.item_forgin_potencial(ref item);
                        }
                        if (real_rarity == 9)
                        {
                            item.rarity = 9;
                            if (ShowDebug)
                            {
                                Main.logger_instance.Msg("ForceDrop : Set Rarity to : " + item.rarity);
                                Main.logger_instance.Msg("ForceDrop : Set Unique Id for Legendary");
                            }
                            
                            item.uniqueID = (ushort)unique_id;
                            item.RefreshIDAndValues();
                        }
                        if ((affixs.enable) && (((item_rarity < 5) || (real_rarity == 9))))
                        {
                            if (ShowDebug) { Main.logger_instance.Msg("ForceDrop : Set Affixs"); }
                            Roll.item_affixes(ref item);
                        }
                        
                        if (item.rarity > 6)
                        {
                            if (unique_mods.enable) { Roll.item_unique_mods(ref item); }
                            foreach (UniqueList.Entry unique_item in UniqueList.instance.uniques)
                            {
                                if (unique_item.uniqueID == unique_id)
                                {
                                    if (unique_item.legendaryType == UniqueList.LegendaryType.LegendaryPotential)
                                    {
                                        if (legenday_potencial.enable)
                                        {
                                            if (ShowDebug) { Main.logger_instance.Msg("ForceDrop : Set Legendary Potencial"); }
                                            if (item.rarity == 9) { item.legendaryPotential = (byte)item.affixes.Count; }
                                            else { Roll.item_legendary_potencial(ref item); }
                                        }
                                    }
                                    else if (weaver_wil.enable)
                                    {
                                        if (ShowDebug) { Main.logger_instance.Msg("ForceDrop : Set Weaver Will"); }                                            
                                        Roll.item_weaver_will(ref item);
                                    }
                                    break;
                                }
                            }
                        }
                        item.RefreshIDAndValues();
                        ItemData final_item = item.TryCast<ItemData>();
                        generating_item = true;
                        for (int i = 0; i < quantity.value; i++)
                        {
                            ground_item_manager.dropItemForPlayer(player, final_item, player.position(), false);
                        }
                        generating_item = false;
                    }
                    else { Main.logger_instance.Error("Player Not Found"); }
                }
                else { Main.logger_instance.Error("Ground Item Manager Not Found"); }
            }
        }
    }
}