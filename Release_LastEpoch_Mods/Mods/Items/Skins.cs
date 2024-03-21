using HarmonyLib;
using LastEpochMods.Managers;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace LastEpochMods.Mods.Items
{
    public class Skins
    {
        public struct skin
        {
            public EquipmentType equipement_type;
            public int subtype;
            public bool unique;
            public bool set;
            public ushort unique_id;
            public Sprite icon;
        }        
        public struct equipment_type
        {
            public EquipmentType equipement_type;
            public byte id;
        }

        public static void Update()
        {
            if (!Visuals.Character_Loaded) { Visuals.Load(); }
            if (!Lists.Initialized) { Lists.InitList(); }
            if (!CosmeticPanel.Tabs.Initialized) { CosmeticPanel.Tabs.Init(); }
            if ((Content.NeedToReopen) && (Lists.Initialized))
            { Content.Open(Content.item_type); Content.NeedToReopen = false; }
        }
        public static void Reset()
        {
            CosmeticPanel.load = false;
            Lists.Initialized = false;
            Config.Initialized = false;            
            CosmeticPanel.Tabs.Initialized = false;
            Visuals.Character_Loaded = false;
        }

        public class Lists
        {
            public static string Character_Class = "";
            public static int Character_Class_Id = 0;

            public static System.Collections.Generic.List<skin> list_skins = new System.Collections.Generic.List<skin>();
            public static System.Collections.Generic.List<equipment_type> list_Equipement_types = new System.Collections.Generic.List<equipment_type>();

            public static bool Initialized = false;
            public static void InitList()
            {
                //if (Config.Initialized)
                //{
                    Initialized = false;
                    if (UniqueList.instance.IsNullOrDestroyed())
                    {
                        Main.logger_instance.Msg("Try to Init UniqueList");
                        try { UniqueList.getUnique(0); }
                        catch { Main.logger_instance.Error("Init UniqueList"); }
                    }

                    list_Equipement_types = new System.Collections.Generic.List<equipment_type>();
                    list_skins = new System.Collections.Generic.List<skin>();
                    int basic_count = 0;
                    try
                    {
                        foreach (ItemList.BaseEquipmentItem item in ItemList.instance.EquippableItems)
                        {                            
                            list_Equipement_types.Add(new equipment_type
                            {
                                equipement_type = item.type,
                                id = (byte)item.TryCast<ItemList.BaseItem>().baseTypeID
                            });

                            foreach (ItemList.EquipmentItem sub_item in item.subItems)
                            {
                                if ((sub_item.classRequirement == ItemList.ClassRequirement.None) || (sub_item.classRequirement.ToString() == Character_Class))
                                {
                                    ItemDataUnpacked new_item = new ItemDataUnpacked
                                    {
                                        LvlReq = 0,
                                        classReq = ItemList.ClassRequirement.Any,
                                        itemType = (byte)item.baseTypeID,
                                        subType = (ushort)sub_item.subTypeID,
                                        rarity = 0,
                                        sockets = 0,
                                        uniqueID = 0
                                    };
                                    list_skins.Add(new skin
                                    {
                                        equipement_type = item.type,
                                        subtype = sub_item.subTypeID,
                                        unique = false,
                                        set = false,
                                        unique_id = 0,
                                        icon = Helper.GetItemIcon(new_item)
                                    });
                                }
                                //else { Main.logger_instance.Msg("Class Requirement : " + sub_item.classRequirement); }
                            }
                        }
                        basic_count = list_skins.Count;
                    }
                    catch { Main.logger_instance.Error("Error ItemList"); }

                    try
                    {
                        foreach (UniqueList.Entry unique in UniqueList.instance.uniques)
                        {
                            if (unique.hasClassCompatibleSubType(Character_Class_Id, 0, false))
                            {
                                int item_rarity = 7;
                                if (unique.isSetItem) { item_rarity = 8; }
                                ItemDataUnpacked new_item = new ItemDataUnpacked
                                {
                                    LvlReq = 0,
                                    classReq = ItemList.ClassRequirement.Any,
                                    itemType = unique.baseType,
                                    subType = unique.subTypes[0],
                                    rarity = (byte)item_rarity,
                                    sockets = 0,
                                    uniqueID = unique.uniqueID
                                };

                                list_skins.Add(new skin
                                {
                                    equipement_type = Helper.GetEquipementTypeFromId(unique.baseType),
                                    subtype = (int)unique.subTypes[0],
                                    unique = true,
                                    set = unique.isSetItem,
                                    unique_id = unique.uniqueID,
                                    icon = Helper.GetItemIcon(new_item)
                                });
                            }
                        }

                    }
                    catch { /*Main.logger_instance.Error("Error UniqueList");*/ }

                    int unique_count = list_skins.Count - basic_count;
                    if (unique_count > 0)
                    {
                        //Main.logger_instance.Msg("List Skins Done : " + list_skins.Count + " items");
                        Initialized = true;
                    }
                //}
            }
        }
        public class UI
        {
            public static void UpdateGui()
            {
                if (Scenes_Manager.GameScene())
                {
                    if (CosmeticPanel.isOpen)
                    {
                        int margin_w = 3; //3%
                        int margin_h = 8; //3%
                        float position_x = CosmeticPanel.position.x - (Screen.width * margin_w / 100);
                        float position_y = CosmeticPanel.position.y + (Screen.height * margin_h / 100);

                        float position_center_x = position_x + (CosmeticPanel.size.x / 2);
                        float slot_size = CosmeticPanel.size.x * 30 / 100;
                        float slot_margin_w = (CosmeticPanel.size.x * 3 / 100);
                        float slot_margin_h = (CosmeticPanel.size.y * 4 / 100);

                        foreach (CosmeticPanel.Slots.SkinSlot slot in CosmeticPanel.Slots.list_slots)
                        {
                            float pos_x = 0;
                            float pos_y = 0;
                            float size_w = slot_size + ((slot_size / 2) * (slot.w - 1));
                            float size_h = slot_size + ((slot_size / 2) * (slot.h - 1));

                            float double_size_h = slot_size + (slot_size / 2);
                            Config.Load.saved_skin user_skin = new Skins.Config.Load.saved_skin()
                            {
                                type = -1,
                                subtype = -1,
                                unique = false,
                                set = false,
                                unique_id = -1
                            };
                            EquipmentType base_equipement_type = EquipmentType.IDOL_4x1;

                            if (slot.name == CosmeticPanel.Slots.helmet)
                            {
                                pos_x = position_center_x - (size_w / 2);
                                pos_y = position_y;

                                user_skin = Config.Data.UserData.helmet;
                                base_equipement_type = EquipmentType.HELMET;
                            }
                            else if (slot.name == CosmeticPanel.Slots.body_armor)
                            {
                                pos_x = position_center_x - (size_w / 2);
                                pos_y = position_y + slot_size + slot_margin_h;

                                user_skin = Config.Data.UserData.body;
                                base_equipement_type = EquipmentType.BODY_ARMOR;
                            }
                            else if (slot.name == CosmeticPanel.Slots.weapon)
                            {
                                float old_size = size_w;
                                size_w = CosmeticPanel.size.x * 20 / 100;
                                pos_x = position_center_x - (old_size / 2) - slot_margin_w - size_w;
                                pos_y = position_y + slot_size + slot_margin_h;

                                user_skin = Config.Data.UserData.weapon;
                                base_equipement_type = EquipmentType.ONE_HANDED_AXE;
                            }
                            else if (slot.name == CosmeticPanel.Slots.offhand)
                            {
                                float old_size = size_w;
                                size_w = CosmeticPanel.size.x * 20 / 100;
                                pos_x = position_center_x + (old_size / 2) + slot_margin_w;
                                pos_y = position_y + slot_size + slot_margin_h;

                                user_skin = Config.Data.UserData.offhand;
                                base_equipement_type = EquipmentType.SHIELD;
                            }
                            else if (slot.name == CosmeticPanel.Slots.gloves)
                            {
                                float old_size = size_w;
                                size_w = CosmeticPanel.size.x * 20 / 100;
                                size_h = size_w;
                                pos_x = position_center_x - (old_size / 2) - slot_margin_w - size_w;
                                pos_y = position_y + slot_size + double_size_h + (2 * slot_margin_h);

                                user_skin = Config.Data.UserData.gloves;
                                base_equipement_type = EquipmentType.GLOVES;
                            }
                            else if (slot.name == CosmeticPanel.Slots.boots)
                            {
                                pos_x = position_center_x - (size_w / 2);
                                pos_y = position_y + slot_size + double_size_h + (2 * slot_margin_h);

                                user_skin = Config.Data.UserData.boots;
                                base_equipement_type = EquipmentType.BOOTS;
                            }

                            try
                            {
                                GUI.DrawTexture(new Rect(pos_x, pos_y, size_w, size_h), slot.background.texture);
                            }
                            catch { Main.logger_instance.Error("Error Background"); }
                            //Selected_icon
                            pos_x += 5;
                            pos_y += 5;
                            size_w -= 10;
                            size_h -= 10;

                            GUIStyle style = new GUIStyle(GUI.skin.button);
                            style.normal.background = null;
                            style.normal.textColor = Color.grey;
                            style.hover.background = style.normal.background;
                            style.hover.textColor = style.normal.textColor;
                            style.alignment = TextAnchor.MiddleCenter;
                            style.fontSize = 16;
                            
                            if ((user_skin.type > -1) && (user_skin.subtype > -1) && (user_skin.unique_id > -1))
                            {
                                EquipmentType skin_type = EquipmentType.IDOL_4x1;
                                try { skin_type = Helper.GetEquipementTypeFromId((byte)user_skin.type); }
                                catch { Main.logger_instance.Error("Error Equipement Type"); }

                                try
                                {
                                    int count = Lists.list_skins.Count;
                                    if (count == 0) { Main.logger_instance.Error("list_skins count = 0"); }
                                }
                                catch { Main.logger_instance.Error("Error list_skins"); }

                                bool found = false;
                                int found_index = -1;
                                try
                                {
                                    for (int index = 0; index < Lists.list_skins.Count; index++)
                                    {
                                        if ((Lists.list_skins[index].equipement_type == skin_type) &&
                                            (Lists.list_skins[index].subtype == user_skin.subtype) &&
                                            (Lists.list_skins[index].unique == user_skin.unique) &&
                                            (Lists.list_skins[index].unique_id == user_skin.unique_id))
                                        {
                                            //Main.logger_instance.Msg("Found Skins : index = " + index);
                                            found = true;
                                            found_index = index;
                                            break;
                                        }
                                    }
                                }
                                catch { Main.logger_instance.Error("Error selected Skin"); }

                                if (found)
                                {
                                    if (found_index < Lists.list_skins.Count)
                                    {
                                        try
                                        {
                                            if (Lists.list_skins[found_index].icon != null)
                                            {
                                                style.normal.background = Lists.list_skins[found_index].icon.texture;
                                            }
                                            else { Lists.Initialized = false; }
                                        }
                                        catch { Main.logger_instance.Error("Error icon"); }

                                        try
                                        {                                            
                                            if (GUI.Button(new Rect(pos_x, pos_y, size_w, size_h), "", style))
                                            {
                                                if (slot.name == CosmeticPanel.Slots.weapon) { CosmeticPanel.Slots.ClickWeapon(); }
                                                else if (slot.name == CosmeticPanel.Slots.offhand)  { CosmeticPanel.Slots.ClickOffhand(); }
                                                else { CosmeticPanel.Slots.Click(Lists.list_skins[found_index]); }                                                
                                            }
                                        }
                                        catch { Main.logger_instance.Error("Error btn"); }
                                    }
                                    //else { Main.logger_instance.Error("Error index out of range"); }
                                }
                                //else { Main.logger_instance.Error("Error Skin not found"); }
                            }
                            else
                            {
                                if (GUI.Button(new Rect(pos_x, pos_y, size_w, size_h), "", style))
                                {
                                    if (slot.name == CosmeticPanel.Slots.weapon)
                                    {
                                        CosmeticPanel.Slots.ClickWeapon();
                                    }
                                    else if (slot.name == CosmeticPanel.Slots.offhand)
                                    {
                                        CosmeticPanel.Slots.ClickOffhand();
                                    }
                                    else
                                    {
                                        CosmeticPanel.Slots.Click(new skin()
                                        {
                                            equipement_type = base_equipement_type,
                                            subtype = 0,
                                            unique = false,
                                            unique_id = 0,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    if (Content.isOpen)
                    {
                        int margin_lef = 1;
                        int margin_right = 1;
                        int margin_top = 5;
                        int margin_bottom = 3;

                        int content_margin = 20;
                        float position_x = Content.Position.x + (Screen.width * margin_lef / 100);
                        float position_y = Content.Position.y + (Screen.height * margin_top / 100); // + content_margin;
                        float size_w = Content.Size.x - (Screen.width * margin_lef / 100) - (Screen.width * margin_right / 100);
                        float size_h = Content.Size.y - (Screen.height * margin_top / 100) - (Screen.height * margin_bottom / 100);
                        //GUI.DrawTexture(new Rect(Content.Position.x, position_y, Content.Size.x, size_h), GUI_Manager.Textures.grey);
                        float scrollview_size_w = size_w;
                        int row = 0;
                        int column = 0;
                        int column_max = 6;
                        float btn_size = ((scrollview_size_w - ((column_max + 1) * content_margin)) / column_max);
                        int nb_row = Content.list_skins_by_type.Count / column_max;
                        float scrollview_size_h = nb_row * (btn_size + content_margin);//(nb_row * btn_size) + ((nb_row + 1) * content_margin);

                        if (scrollview_size_h > size_h) { scrollview_size_w -= 20; }
                        else { size_h = scrollview_size_h; }
                        Content.dropdown_scrollview = GUI.BeginScrollView(new Rect(position_x, position_y, size_w, size_h), Content.dropdown_scrollview, new Rect(0, 0, scrollview_size_w, scrollview_size_h));

                        //button reset
                        GUIStyle none_style = new GUIStyle(GUI.skin.button);
                        none_style.normal.background = GUI_Manager.Textures.grey;
                        none_style.normal.textColor = Color.black;
                        none_style.hover.background = none_style.normal.background;
                        none_style.hover.textColor = none_style.normal.textColor;
                        none_style.alignment = TextAnchor.MiddleCenter;
                        none_style.fontSize = 20;
                        if (GUI.Button(new Rect(column * (btn_size + content_margin), (row * btn_size) + (row * content_margin), btn_size, btn_size), "Reset", none_style))
                        {
                            CosmeticPanelUI c_p_ui = InventoryPanelUI.instance.gameObject.GetComponent<InventoryPanelUI>().cosmeticPanel.GetComponent<CosmeticPanelUI>();
                            if (c_p_ui.flyoutTitle.text == Content.Weapons_Tile) { Content.SkinWeaponReset(); }
                            else if (c_p_ui.flyoutTitle.text == Content.Offhand_Tile) { Content.SkinOffhandReset(); }
                            else { Content.SkinReset(); }
                        }
                        //GUI.TextField(new Rect(column * (btn_size + content_margin), (row * btn_size) + (row * content_margin), btn_size, btn_size), "Cheats", Styles.Content_Title());
                        column++;

                        for (int index = 0; index < Content.list_skins_by_type.Count; index++)
                        {
                            if (column > (column_max - 1)) { row++; column = 0; }

                            float pos_x = column * (btn_size + content_margin);//(column * btn_size) + ((column + 1) * content_margin);
                            float pos_y = (row * btn_size) + (row * content_margin);

                            GUIStyle style = new GUIStyle(GUI.skin.button);
                            try
                            {
                                style.normal.background = Content.list_skins_by_type[index].icon.texture;
                            }
                            catch
                            {
                                Lists.Initialized = false;
                                Content.NeedToReopen = true;
                                break;
                            }
                            style.normal.textColor = Color.grey;
                            style.hover.background = style.normal.background;
                            style.hover.textColor = style.normal.textColor;
                            style.onFocused.background = style.normal.background;
                            style.alignment = TextAnchor.MiddleCenter;
                            style.fontSize = 16;

                            Texture2D background = GUI_Manager.Textures.black;
                            if (Content.list_skins_by_type[index].set) { background = GUI_Manager.Textures.texture_set; }
                            else if (Content.list_skins_by_type[index].unique) { background = GUI_Manager.Textures.texture_unique; }

                            GUI.DrawTexture(new Rect(pos_x, pos_y, btn_size, btn_size), background);
                            if (GUI.Button(new Rect(pos_x, pos_y, btn_size, btn_size), "", style))
                            {
                                CosmeticPanelUI c_p_ui = InventoryPanelUI.instance.gameObject.GetComponent<InventoryPanelUI>().cosmeticPanel.GetComponent<CosmeticPanelUI>();
                                if (c_p_ui.flyoutTitle.text == Content.Weapons_Tile)
                                {
                                    Content.SkinClickWeapon(Content.list_skins_by_type[index]);
                                }
                                else if (c_p_ui.flyoutTitle.text == Content.Offhand_Tile)
                                {
                                    Content.SkinClickOffHand(Content.list_skins_by_type[index]);
                                }
                                else { Content.SkinClick(Content.list_skins_by_type[index]); }                                
                            }
                            column++;
                        }
                        GUI.EndScrollView();

                    }
                }
            }
        }
        public class CosmeticPanel
        {
            public static bool load = false;
            public static bool isOpen = false;
            public static Vector2 size;
            public static Vector2 position;
            public class Tabs
            {
                public static bool Initialized = false;
                public static void Init()
                {
                    try
                    {
                        if (Lists.Initialized)
                        {
                            foreach (TabUIElement tab in InventoryPanelUI.instance.tabController.tabElements)
                            {
                                if (tab.gameObject.name == "AppearanceTab")
                                {
                                    tab.isDisabled = false;
                                    tab.canvasGroup.TryCast<Behaviour>().enabled = false;
                                    break;
                                }
                            }
                            Initialized = true;
                        }
                    }
                    catch { Main.logger_instance.Error("Skins : Tabs:Init(); -> TabElements"); }
                }
            }
            public class Slots
            {
                public static System.Collections.Generic.List<SkinSlot> list_slots = new System.Collections.Generic.List<SkinSlot>();
                public struct SkinSlot
                {
                    public string name;
                    public Sprite background;
                    public int h;
                    public int w;
                }
                public static string helmet = "Helmet";
                public static string body_armor = "Body";
                public static string weapon = "Weapon";
                public static string offhand = "OffHand";
                public static string gloves = "Gloves";
                public static string boots = "Boots";

                public static void InitSlots()
                {
                    list_slots = new System.Collections.Generic.List<SkinSlot>();
                    try
                    {
                        list_slots.Add(new SkinSlot
                        {
                            name = helmet,
                            background = GetSlotSprite(helmet),
                            h = 1,
                            w = 1
                        });
                        list_slots.Add(new SkinSlot
                        {
                            name = body_armor,
                            background = GetSlotSprite(body_armor),
                            h = 2,
                            w = 1
                        });
                        list_slots.Add(new SkinSlot
                        {
                            name = weapon,
                            background = GetSlotSprite(weapon),
                            h = 2,
                            w = 1
                        });
                        list_slots.Add(new SkinSlot
                        {
                            name = offhand,
                            background = GetSlotSprite(offhand),
                            h = 2,
                            w = 1
                        });
                        list_slots.Add(new SkinSlot
                        {
                            name = gloves,
                            background = GetSlotSprite(gloves),
                            h = 1,
                            w = 1
                        });
                        list_slots.Add(new SkinSlot
                        {
                            name = boots,
                            background = GetSlotSprite(boots),
                            h = 1,
                            w = 1
                        });
                    }
                    catch { Main.logger_instance.Error("Init Slots"); }                    
                }
                public static void Click(skin s)
                {
                    Content.Open(Helper.GetIdFromEquipementType(s.equipement_type));
                }
                public static void ClickWeapon()
                {
                    Content.OpenWeapon();
                }
                public static void ClickOffhand()
                {
                    Content.OpenOffhand();
                }
                private static Sprite GetSlotSprite(string slot_name)
                {
                    Sprite sprite = null;
                    try
                    {
                        System.IO.MemoryStream stream = new System.IO.MemoryStream();
                        if (slot_name == helmet)
                        {
                            Properties.Resources.Helmet.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if (slot_name == body_armor)
                        {
                            Properties.Resources.Armor.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if (slot_name == weapon)
                        {
                            Properties.Resources.Weapon.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if (slot_name == offhand)
                        {
                            Properties.Resources.Shield.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if (slot_name == gloves)
                        {
                            Properties.Resources.Gloves.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if (slot_name == boots)
                        {
                            Properties.Resources.Boots.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        Texture2D icon = new Texture2D(1, 1);
                        ImageConversion.LoadImage(icon, stream.ToArray(), true);
                        sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);
                    }
                    catch { Main.logger_instance.Error("GetSlotSprite"); }

                    return sprite;
                }
            }

            [HarmonyPatch(typeof(InventoryPanelUI), "OnEnable")]
            public class InventoryPanelUI_OnEnable
            {
                [HarmonyPostfix]
                static void Postfix(ref InventoryPanelUI __instance)
                {
                    load = true;
                    Tabs.Init();
                }
            }                        
            
            [HarmonyPatch(typeof(CosmeticPanelUI), "OnEnable")]
            public class CosmeticPanelUI_OnEnable
            {
                [HarmonyPostfix]
                static void Postfix(ref CosmeticPanelUI __instance)
                {
                    try
                    {
                        __instance.getCoinsButton.gameObject.active = false;
                        __instance.openShopButton.gameObject.active = false;
                        foreach (CosmeticItemSlot equip_slot in __instance.equipSlots) { equip_slot.gameObject.active = false; }
                        foreach (CosmeticItemSlot pet_slot in __instance.petSlots) { pet_slot.gameObject.active = false; }
                    }
                    catch { Main.logger_instance.Error("CosmeticPanelUI:OnEnable Error Hidding defaults slots"); }

                    try
                    {
                        //GameObject SkillNavigation = 
                            GUI_Manager.GeneralFunctions.GetChild(__instance.gameObject, "SkillNavigation").active = false;
                        //SkillNavigation.active = false;
                        //GameObject SkillCosmetics = 
                            GUI_Manager.GeneralFunctions.GetChild(__instance.gameObject, "Skill Cosmetics").active = false;
                    }
                    catch { }

                    try
                    {
                        RectTransform rect_transform = __instance.gameObject.GetComponent<RectTransform>();
                        position = new Vector2(Screen.width + rect_transform.rect.x, (Screen.height / 2) + rect_transform.rect.y); //Anchor Middle Right
                        size = new Vector2(rect_transform.rect.width, rect_transform.rect.height);
                        isOpen = true;
                    }
                    catch { Main.logger_instance.Error("CosmeticPanelUI:OnEnable Error Initializing UI"); }
                }
            } 
            
            [HarmonyPatch(typeof(CosmeticPanelUI), "OnDisable")]
            public class CosmeticPanelUI_OnDisable
            {
                [HarmonyPostfix]
                static void Postfix(ref CosmeticPanelUI __instance)
                {
                    isOpen = false;
                }
            }
        }
        public class Content
        {
            public static string Weapons_Tile = "Weapons Appearance";
            public static string Offhand_Tile = "OffHand Appearance";

            public static bool NeedToReopen = false;
            public static bool isOpen = false;
            public static Vector3 Position = Vector3.zero;
            public static Vector2 Size = Vector2.zero;
            public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
            public static Vector2 dropdown_scrollview = Vector2.zero;
            public static int item_type = 0;            
            public static System.Collections.Generic.List<skin> list_skins_by_type = new System.Collections.Generic.List<skin>();
               
            public static void Open(int type)
            {
                try
                {
                    item_type = type;
                    list_skins_by_type = new System.Collections.Generic.List<skin>();
                    foreach (skin s in Lists.list_skins)
                    {
                        if (s.equipement_type == Helper.GetEquipementTypeFromId((byte)item_type)) { list_skins_by_type.Add(s); }
                    }
                    CosmeticPanelUI cosmetic_panel_ui = InventoryPanelUI.instance.cosmeticPanel.GetComponent<CosmeticPanelUI>();
                    cosmetic_panel_ui.flyoutTitle.text = Helper.GetEquipementTypeFromId((byte)item_type) + " Appearance";
                    cosmetic_panel_ui.flyoutSelectionWindow.gameObject.active = true;
                }
                catch { Main.logger_instance.Error("Skins : Content Open"); }
            }
            public static void OpenWeapon()
            {
                try
                {
                    list_skins_by_type = new System.Collections.Generic.List<skin>();
                    foreach (skin s in Lists.list_skins)
                    {
                        if ((s.equipement_type == EquipmentType.ONE_HANDED_AXE) || (s.equipement_type == EquipmentType.TWO_HANDED_AXE) ||
                                (s.equipement_type == EquipmentType.ONE_HANDED_MACES) || (s.equipement_type == EquipmentType.TWO_HANDED_MACE) ||
                                (s.equipement_type == EquipmentType.ONE_HANDED_SWORD) || (s.equipement_type == EquipmentType.TWO_HANDED_SWORD) ||
                                (s.equipement_type == EquipmentType.ONE_HANDED_FIST) || (s.equipement_type == EquipmentType.ONE_HANDED_DAGGER) ||
                                (s.equipement_type == EquipmentType.TWO_HANDED_SPEAR) || (s.equipement_type == EquipmentType.TWO_HANDED_STAFF) ||
                                (s.equipement_type == EquipmentType.BOW) || (s.equipement_type == EquipmentType.CROSSBOW) ||
                                (s.equipement_type == EquipmentType.ONE_HANDED_SCEPTRE) || (s.equipement_type == EquipmentType.WAND))
                        {
                            list_skins_by_type.Add(s);
                        }
                    }
                    CosmeticPanelUI cosmetic_panel_ui = InventoryPanelUI.instance.cosmeticPanel.GetComponent<CosmeticPanelUI>();
                    cosmetic_panel_ui.flyoutTitle.text = Weapons_Tile;
                    cosmetic_panel_ui.flyoutSelectionWindow.gameObject.active = true;
                }
                catch { Main.logger_instance.Error("Skins : Weapons Open"); }
            }
            public static void OpenOffhand()
            {
                try
                {
                    list_skins_by_type = new System.Collections.Generic.List<skin>();
                    foreach (skin s in Lists.list_skins)
                    {
                        if ((s.equipement_type == EquipmentType.ONE_HANDED_AXE) || (s.equipement_type == EquipmentType.ONE_HANDED_MACES) ||
                                (s.equipement_type == EquipmentType.ONE_HANDED_SWORD) || (s.equipement_type == EquipmentType.ONE_HANDED_FIST) ||
                                (s.equipement_type == EquipmentType.ONE_HANDED_DAGGER) || (s.equipement_type == EquipmentType.ONE_HANDED_SCEPTRE) ||
                                (s.equipement_type == EquipmentType.WAND) || (s.equipement_type == EquipmentType.SHIELD) ||
                                (s.equipement_type == EquipmentType.QUIVER))
                        {
                            list_skins_by_type.Add(s);
                        }
                    }
                    CosmeticPanelUI cosmetic_panel_ui = InventoryPanelUI.instance.cosmeticPanel.GetComponent<CosmeticPanelUI>();
                    cosmetic_panel_ui.flyoutTitle.text = Offhand_Tile;
                    cosmetic_panel_ui.flyoutSelectionWindow.gameObject.active = true;
                }
                catch { Main.logger_instance.Error("Skins : Offhand Open"); }
            }
            public static void Close()
            {
                try
                {
                    CosmeticsFlyoutSelectionContentNavigable content = InventoryPanelUI.instance.cosmeticPanel.GetComponent<CosmeticPanelUI>().flyoutSelectionWindow;
                    content.gameObject.active = false;
                }
                catch { Main.logger_instance.Error("Skins : Content Close"); }
            }
            
            public static void SkinClick(skin s)
            {
                //Main.logger_instance.Msg("Skin Clicked : Type = " + s.equipement_type.ToString() +
                //    ", SubType = " + s.subtype + ", Unique = " + s.unique + ", Set = " + s.set + ", UniqueId = " + s.unique_id);
                if (s.equipement_type == EquipmentType.HELMET)
                {
                    Config.Data.UserData.helmet = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipementType(s.equipement_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                else if (s.equipement_type == EquipmentType.BODY_ARMOR)
                {
                    Config.Data.UserData.body = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipementType(s.equipement_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                else if (s.equipement_type == EquipmentType.GLOVES)
                {
                    Config.Data.UserData.gloves = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipementType(s.equipement_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                else if (s.equipement_type == EquipmentType.BOOTS)
                {
                    Config.Data.UserData.boots = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipementType(s.equipement_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                else if ((s.equipement_type == EquipmentType.ONE_HANDED_AXE) || (s.equipement_type == EquipmentType.TWO_HANDED_AXE) ||
                            (s.equipement_type == EquipmentType.ONE_HANDED_MACES) || (s.equipement_type == EquipmentType.TWO_HANDED_MACE) ||
                            (s.equipement_type == EquipmentType.ONE_HANDED_SWORD) || (s.equipement_type == EquipmentType.ONE_HANDED_SWORD) ||
                            (s.equipement_type == EquipmentType.ONE_HANDED_FIST) || (s.equipement_type == EquipmentType.ONE_HANDED_DAGGER) ||
                            (s.equipement_type == EquipmentType.TWO_HANDED_SPEAR) || (s.equipement_type == EquipmentType.TWO_HANDED_STAFF) ||
                            (s.equipement_type == EquipmentType.BOW) || (s.equipement_type == EquipmentType.CROSSBOW) ||
                            (s.equipement_type == EquipmentType.ONE_HANDED_SCEPTRE) || (s.equipement_type == EquipmentType.WAND))
                {
                    Config.Data.UserData.weapon = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipementType(s.equipement_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }   
                else if (s.equipement_type == EquipmentType.SHIELD)
                {
                    Config.Data.UserData.offhand = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipementType(s.equipement_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().EquipGear(s.equipement_type, 0, false, 0);
            }
            public static void SkinClickWeapon(skin s)
            {
                Config.Data.UserData.weapon = new Config.Load.saved_skin()
                {
                    type = Helper.GetIdFromEquipementType(s.equipement_type),
                    subtype = s.subtype,
                    unique = s.unique,
                    set = s.set,
                    unique_id = s.unique_id
                };
                Config.Save.Skins();
                int rarity = 0;
                if (s.unique) { rarity = 7; }
                if (s.set) { rarity = 8; }
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().EquipWeapon(Config.Data.UserData.weapon.type, s.subtype, rarity, s.unique_id, IMSlotType.MainHand, WeaponEffect.None);
            }
            public static void SkinClickOffHand(skin s)
            {
                Config.Data.UserData.offhand = new Config.Load.saved_skin()
                {
                    type = Helper.GetIdFromEquipementType(s.equipement_type),
                    subtype = s.subtype,
                    unique = s.unique,
                    set = s.set,
                    unique_id = s.unique_id
                };
                Config.Save.Skins();
                int rarity = 0;
                if (s.unique) { rarity = 7; }
                if (s.set) { rarity = 8; }
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().EquipWeapon(Config.Data.UserData.offhand.type, s.subtype, rarity, s.unique_id, IMSlotType.OffHand, WeaponEffect.None);
            }
            
            public static void SkinReset()
            {
                EquipmentType type = Helper.GetEquipementTypeFromId((byte)item_type);
                Config.Load.saved_skin null_skin = new Config.Load.saved_skin()
                {
                    type = -1,
                    subtype = -1,
                    unique = false,
                    set = false,
                    unique_id = -1
                };
                if (type == EquipmentType.HELMET)
                {
                    Config.Data.UserData.helmet = null_skin;
                    Config.Save.Skins();
                }
                else if (type == EquipmentType.BODY_ARMOR)
                {
                    Config.Data.UserData.body = null_skin;
                    Config.Save.Skins();
                }
                else if (type == EquipmentType.GLOVES)
                {
                    Config.Data.UserData.gloves = null_skin;
                    Config.Save.Skins();
                }
                else if (type == EquipmentType.BOOTS)
                {
                    Config.Data.UserData.boots = null_skin;
                    Config.Save.Skins();
                }
                else if ((type == EquipmentType.ONE_HANDED_AXE) || (type == EquipmentType.TWO_HANDED_AXE) ||
                            (type == EquipmentType.ONE_HANDED_MACES) || (type == EquipmentType.TWO_HANDED_MACE) ||
                            (type == EquipmentType.ONE_HANDED_SWORD) || (type == EquipmentType.ONE_HANDED_SWORD) ||
                            (type == EquipmentType.ONE_HANDED_FIST) || (type == EquipmentType.ONE_HANDED_DAGGER) ||
                            (type == EquipmentType.TWO_HANDED_SPEAR) || (type == EquipmentType.TWO_HANDED_STAFF) ||
                            (type == EquipmentType.BOW) || (type == EquipmentType.CROSSBOW) ||
                            (type == EquipmentType.ONE_HANDED_SCEPTRE) || (type == EquipmentType.WAND))
                {
                    Config.Data.UserData.weapon = null_skin;
                    Config.Save.Skins();
                }
                else if (type == EquipmentType.SHIELD)
                {
                    Config.Data.UserData.offhand = null_skin;
                    Config.Save.Skins();
                }
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().RemoveGear((byte)item_type);
            }
            public static void SkinWeaponReset()
            {
                EquipmentType type = Helper.GetEquipementTypeFromId((byte)item_type);
                Config.Load.saved_skin null_skin = new Config.Load.saved_skin()
                {
                    type = -1,
                    subtype = -1,
                    unique = false,
                    set = false,
                    unique_id = -1
                };
                Config.Data.UserData.weapon = null_skin;
                Config.Save.Skins();
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().RemoveWeapon(false, true);
            }
            public static void SkinOffhandReset()
            {
                EquipmentType type = Helper.GetEquipementTypeFromId((byte)item_type);
                Config.Load.saved_skin null_skin = new Config.Load.saved_skin()
                {
                    type = -1,
                    subtype = -1,
                    unique = false,
                    set = false,
                    unique_id = -1
                };
                Config.Data.UserData.offhand = null_skin;
                Config.Save.Skins();
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().RemoveWeapon(true, true);
            }

            [HarmonyPatch(typeof(CosmeticsFlyoutSelectionContentNavigable), "OnEnable")]
            public class CosmeticsFlyoutSelectionContentNavigable_OnEnable
            {
                [HarmonyPostfix]
                static void Postfix(ref CosmeticsFlyoutSelectionContentNavigable __instance)
                {
                    //Get Position & Size
                    Helper.Ui_Info infos = Helper.Get_UI_Infos_Center(__instance.gameObject);
                    Size = infos.size;
                    Position = infos.position;

                    //Hide All Cosmetics
                    GameObject content = null;
                    try { content = GUI_Manager.GeneralFunctions.GetChild(__instance.contentMaskRect.gameObject, "Content"); }
                    catch { content = null; }
                    if (!content.IsNullOrDestroyed())
                    {
                        for (int i = 0; i < content.transform.childCount; i++)
                        {
                            content.transform.GetChild(i).gameObject.active = false;
                        }
                    }
                    //Hide Buy Btn
                    __instance.shopButton.gameObject.active = false;
                    isOpen = true;
                }
            }

            [HarmonyPatch(typeof(CosmeticsFlyoutSelectionContentNavigable), "OnDisable")]
            public class CosmeticsFlyoutSelectionContentNavigable_OnDisable
            {
                [HarmonyPostfix]
                static void Postfix(ref CosmeticsFlyoutSelectionContentNavigable __instance)
                {
                    isOpen = false;
                }
            }
        }
        public class Config
        {
            public static string path = Directory.GetCurrentDirectory() + @"\Mods\LastEpochMods\Skins\";            
            public static string filename = "";
            public static bool Initialized = false;

            private static bool loading = false;
            public static void Init()
            {
                if (Scenes_Manager.GameScene())
                {
                    try
                    {
                        loading = true;
                        LE.Data.CharacterData char_data = PlayerFinder.getPlayerData();
                        string character_name = char_data.CharacterName;                        
                        if (character_name != "")
                        {
                            Lists.Character_Class_Id = char_data.CharacterClass;
                            Lists.Character_Class = char_data.GetCharacterClass().className;
                            if (char_data.Cycle == LE.Data.Cycle.Release)
                            {
                                path = Directory.GetCurrentDirectory() + @"\Mods\LastEpochMods\Skins\Cycle\";
                            }
                            else if (char_data.Cycle == LE.Data.Cycle.Legacy)
                            {
                                path = Directory.GetCurrentDirectory() + @"\Mods\LastEpochMods\Skins\Legacy\";
                            }
                            filename = character_name + ".json";
                            Load.UserConfig();
                        }
                        else
                        {
                            filename = "";
                            loading = false;
                            Main.logger_instance.Error("Error Loading Skin Config, character name is null");
                        }
                    }
                    catch { }
                }
            }            

            public class Data
            {
                public static UserSkin UserData = new UserSkin();

                public struct UserSkin
                {
                    public Load.saved_skin helmet;
                    public Load.saved_skin body;
                    public Load.saved_skin gloves;
                    public Load.saved_skin boots;
                    public Load.saved_skin weapon;
                    public Load.saved_skin offhand;
                }
            }            
            public class Load
            {
                public struct saved_skin
                {
                    public int type;
                    public int subtype;
                    public bool unique;
                    public bool set;
                    public int unique_id;
                }
                public static void UserConfig()
                {
                    Data.UserData = DefaultConfig();
                    if (File.Exists(path + filename))
                    {
                        try
                        {
                            Data.UserData = JsonConvert.DeserializeObject<Data.UserSkin>(File.ReadAllText(path + filename));
                            //Main.logger_instance.Msg("User Skins Loaded");
                        }
                        catch { Main.logger_instance.Error("Error loading file : " + filename); }
                    }
                    else { Save.Skins(); }
                    //Main.logger_instance.Msg("Config Init Done");
                    Initialized = true;
                    loading = false;
                }
                public static Data.UserSkin DefaultConfig()
                {
                    //Main.logger_instance.Msg("Skins Default Config Loaded");
                    saved_skin default_skin = new saved_skin()
                    {
                        type = -1,
                        subtype = -1,
                        unique = false,
                        set = false,
                        unique_id = -1
                    };
                    Data.UserSkin result = new Data.UserSkin
                    {
                        helmet = default_skin,
                        body = default_skin,
                        gloves = default_skin,
                        boots = default_skin,
                        weapon = default_skin,
                        offhand = default_skin
                    };

                    return result;
                }
            }
            public class Save
            {
                public static void Skins()
                {
                    string jsonString = JsonConvert.SerializeObject(Data.UserData, Formatting.Indented);
                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                    if (File.Exists(path + filename)) { File.Delete(path + filename); }
                    File.WriteAllText(path + filename, jsonString);
                }
            }
        }
        public class Helper
        {
            public static int GetIdFromEquipementType(EquipmentType type)
            {
                int result = -1;
                foreach (equipment_type eq_type in Lists.list_Equipement_types)
                {
                    if (type == eq_type.equipement_type) { result = eq_type.id; break; }
                }

                return result;
            }
            public static EquipmentType GetEquipementTypeFromId(byte id)
            {
                EquipmentType result = EquipmentType.IDOL_4x1;
                foreach (equipment_type eq_type in Lists.list_Equipement_types)
                {
                    if (id == eq_type.id) { result = eq_type.equipement_type; break; }
                }
                //Main.logger_instance.Msg("GetEquipementTypeFromId : Id = " + id + ", Type = " + result.ToString());

                return result;
            }
            public static Sprite GetItemIcon(ItemDataUnpacked item)
            {                
                Sprite result = null;
                try { result = UITooltipItem.SetItemSprite(item); }
                catch { Main.logger_instance.Error("Error GetItemIcon"); }
                if (result == null) { Main.logger_instance.Error("Error GetItemIcon = null"); }

                return result;
            }
            public struct Ui_Info
            {
                public Vector2 position;
                public Vector2 size;
            }
            public static Ui_Info Get_UI_Infos_Center(GameObject go)
            {
                Ui_Info result = new Ui_Info();
                try
                {
                    //Main.logger_instance.Msg("Found : " + go.name);
                    //Main.logger_instance.Msg("Position : X = " + go.transform.position.x + ", Y = " + go.transform.position.y);
                    //Main.logger_instance.Msg("Local Position : X = " + go.transform.localPosition.x + ", Y = " + go.transform.localPosition.y);
                    Vector3 position = go.transform.position;
                    Vector3 lossy = go.transform.lossyScale;
                    RectTransform rect_transform = go.GetComponent<RectTransform>();
                    Vector2 Size = new Vector2(rect_transform.rect.width * lossy.x, rect_transform.rect.height * lossy.y);
                    //Main.logger_instance.Msg("size : X = " + Size.x + ", Y = " + Size.y);
                    Vector3 Position = new Vector3(position.x - (Size.x / 2), position.y - (Size.y / 2) + 40);
                    //Main.logger_instance.Msg("Calculed Real position : X = " + Position.x + ", Y = " + Position.y);

                    result.position = Position;
                    result.size = Size;
                }
                catch { }

                return result;
            }
            public static Ui_Info Get_UI_Infos_Right(GameObject go)
            {
                Ui_Info result = new Ui_Info();
                try
                {
                    Main.logger_instance.Msg("Found : " + go.name);
                    Main.logger_instance.Msg("Position : X = " + go.transform.position.x + ", Y = " + go.transform.position.y);
                    Main.logger_instance.Msg("Local Position : X = " + go.transform.localPosition.x + ", Y = " + go.transform.localPosition.y);
                    Vector3 position = go.transform.position;
                    Vector3 lossy = go.transform.lossyScale;
                    RectTransform rect_transform = go.GetComponent<RectTransform>();
                    Vector2 Size = new Vector2(rect_transform.rect.width * lossy.x, rect_transform.rect.height * lossy.y);
                    Main.logger_instance.Msg("size : X = " + Size.x + ", Y = " + Size.y);
                    Vector3 Position = new Vector3(position.x - Size.x, position.y - (Size.y / 2));
                    Main.logger_instance.Msg("Calculed Real position : X = " + Position.x + ", Y = " + Position.y);

                    result.position = Position;
                    result.size = Size;
                }
                catch { }

                return result;
            }
            public static Ui_Info Get_UI_Infos(GameObject go)
            {
                Ui_Info result = new Ui_Info();
                try
                {
                    Main.logger_instance.Msg("Found : " + go.name);
                    Main.logger_instance.Msg("Position : X = " + go.transform.position.x + ", Y = " + go.transform.position.y);
                    Main.logger_instance.Msg("Local Position : X = " + go.transform.localPosition.x + ", Y = " + go.transform.localPosition.y);
                    Vector3 position = go.transform.position;
                    Vector3 lossy = go.transform.lossyScale;
                    RectTransform rect_transform = go.GetComponent<RectTransform>();
                    Vector2 Size = new Vector2(rect_transform.rect.width * lossy.x, rect_transform.rect.height * lossy.y);
                    Main.logger_instance.Msg("size : X = " + Size.x + ", Y = " + Size.y);
                    Vector3 Position = new Vector3(position.x, position.y);
                    Main.logger_instance.Msg("Calculed Real position : X = " + Position.x + ", Y = " + Position.y);

                    result.position = Position;
                    result.size = Size;
                }
                catch { }

                return result;
            }
        }
        public class Visuals
        {
            public static bool Character_Loaded = false;
            private static bool loading = false;
            public static bool CheckIsValid(Config.Load.saved_skin skin)
            {
                bool result = false;
                if ((skin.type > -1) && (skin.subtype > -1) && (skin.unique_id > -1))
                {
                    result = true;
                }

                return result;
            }
            public static void Load()
            {
                if ((Config.Initialized) && (Scenes_Manager.GameScene()) && (Lists.Initialized) && (!loading))
                {
                    try
                    {
                        loading = true;
                        EquipmentVisualsManager visual_manager = PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>();
                        if (CheckIsValid(Config.Data.UserData.helmet)) { visual_manager.EquipGear(EquipmentType.HELMET, 0, false, 0); }
                        if (CheckIsValid(Config.Data.UserData.body)) { visual_manager.EquipGear(EquipmentType.BODY_ARMOR, 0, false, 0); }
                        if (CheckIsValid(Config.Data.UserData.gloves)) { visual_manager.EquipGear(EquipmentType.GLOVES, 0, false, 0); }
                        if (CheckIsValid(Config.Data.UserData.boots)) { visual_manager.EquipGear(EquipmentType.BOOTS, 0, false, 0); }
                        if (CheckIsValid(Config.Data.UserData.offhand)) { visual_manager.EquipWeapon(0, 0, 0, 0, IMSlotType.OffHand, WeaponEffect.None); }
                        if (CheckIsValid(Config.Data.UserData.weapon)) { visual_manager.EquipWeapon(0, 0, 0, 0, IMSlotType.MainHand, WeaponEffect.None); }

                        Character_Loaded = true;
                        //Main.logger_instance.Msg("Skin Loaded");
                        loading = false;
                    }
                    catch { loading = false; }
                }
            }

            [HarmonyPatch(typeof(EquipmentVisualsManager), "EquipWeapon")]
            public class EquipmentVisualsManager_EquipWeapon
            {
                [HarmonyPrefix]
                static void Prefix(ref EquipmentVisualsManager __instance, ref int __0, ref int __1, ref int __2, ref ushort __3, ref IMSlotType __4, ref WeaponEffect __5)
                {
                    if ((Config.Initialized) && (Lists.Initialized))
                    {
                        //Main.logger_instance.Msg("EquipmentVisualsManager:EquipWeapon");
                        //Main.logger_instance.Msg("Default -> __0 = " + __0 + ", __1 = " + __1 + ", __2 = " + __2 + ", uniqueId = " + __3 + ", Slot = " + __4.ToString());
                        try
                        {
                            bool found = false;
                            Config.Load.saved_skin skin = new Config.Load.saved_skin()
                            {
                                type = -1,
                                subtype = -1,
                                unique = false,
                                set = false,
                                unique_id = -1
                            };
                            if (__4 == IMSlotType.MainHand)
                            {
                                skin = Config.Data.UserData.weapon;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }
                            else if (__4 == IMSlotType.OffHand)
                            {
                                skin = Config.Data.UserData.offhand;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }

                            if (found)
                            {
                                int rarity = 7;
                                if (skin.set) { rarity = 8; }
                                if ((skin.type > -1) && (skin.subtype > -1) && (skin.unique_id > -1))
                                {
                                    __0 = skin.type;
                                    __1 = skin.subtype;
                                    __2 = rarity;
                                    __3 = (ushort)skin.unique_id;
                                    //__5 = WeaponEffect.Fire; //not working
                                }
                                //Main.logger_instance.Msg("Override -> Type = " + __0.ToString() + ", SubType = " + __1 + ", isUnique = " + __2 + ", uniqueId = " + __3);
                            }
                        }
                        catch { Main.logger_instance.Error("EquipmentVisualsManager:EquipWeapon"); }
                    }
                }
            }

            [HarmonyPatch(typeof(EquipmentVisualsManager), "EquipGear")]
            public class EquipmentVisualsManager_EquipGear
            {
                [HarmonyPrefix]
                static void Prefix(ref EquipmentVisualsManager __instance, ref EquipmentType __0, ref int __1, ref bool __2, ref ushort __3)
                {
                    //Main.logger_instance.Msg("EquipmentVisualsManager:EquipGear");
                    //Main.logger_instance.Msg("Default -> Type = " + __0.ToString() + ", SubType = " + __1 + ", isUnique = " + __2 + ", uniqueId = " + __3);
                    if (Config.Initialized)
                    {
                        try
                        {
                            bool found = false;
                            Config.Load.saved_skin skin = new Config.Load.saved_skin()
                            {
                                type = -1,
                                subtype = -1,
                                unique = false,
                                set = false,
                                unique_id = -1
                            };
                            if (__0 == EquipmentType.HELMET)
                            {
                                skin = Config.Data.UserData.helmet;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }
                            else if (__0 == EquipmentType.BODY_ARMOR)
                            {
                                skin = Config.Data.UserData.body;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }
                            else if (__0 == EquipmentType.GLOVES)
                            {
                                skin = Config.Data.UserData.gloves;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }
                            else if (__0 == EquipmentType.BOOTS)
                            {
                                skin = Config.Data.UserData.boots;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }                            

                            //Set
                            if (found)
                            {
                                if ((skin.type > -1) && (skin.subtype > -1) && (skin.unique_id > -1))
                                {
                                    __0 = Helper.GetEquipementTypeFromId((byte)skin.type);
                                    __1 = skin.subtype;
                                    __2 = skin.unique;
                                    __3 = (ushort)skin.unique_id;
                                }
                                //Main.logger_instance.Msg("Override -> Type = " + __0.ToString() + ", SubType = " + __1 + ", isUnique = " + __2 + ", uniqueId = " + __3);
                            }
                        }
                        catch { Main.logger_instance.Error("EquipmentVisualsManager:EquipGear"); }
                    }
                }
            }
        }
    }
}
