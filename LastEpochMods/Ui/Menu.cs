using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LastEpochMods.Ui
{
    public class Menu
    {
        public static bool isMenuOpen;
        public static int WindowId = 0;        
        public static Rect Menu_Window_Rect = new Rect(5f, 5f, 620f, 500f); //Debug for Drag
        private static float Menu_Size_h = 50f;
        private static bool ShowItemDropSection = false;
        public static float ItemsDrop_h = 105f;
        private static bool ShowItemDropUnlockSection = false;
        public static float ItemsDropUnlock_h = 265;
        private static bool ShowItemDropOnDeathSection = false;
        public static float ItemsOnDeath_h = 285f;
        private static bool ShowAutoPickupSection = false;
        public static float AutoPickup_h = 285f;
        private static bool ShowSceneSection = false;
        public static float Scene_h = 150f;

        public static void Update()
        {
            if (isMenuOpen)
            {
                Menu_Window_Rect = GUI.Window(WindowId, Menu_Window_Rect, (GUI.WindowFunction)ModMenu, "", GUIStyle.none);
                void ModMenu(int windowID)
                {
                    float alpha = 0.95f;
                    Color new_color = GUI.color; //Backup GUI Color                    
                    GUI.color = new Color(new_color.r, new_color.g, new_color.b, alpha);
                    Texture2D windowBackground = GUI.skin.window.normal.background as Texture2D;
                    GUI.DrawTexture(new Rect(0, 0, Menu_Window_Rect.width, Menu_Size_h), windowBackground);
                    GUI.color = new_color; // Reset GUI color
                    GUI.skin.window.normal.background = Functions.MakeTextureFromColor(2, 2, Color.black);
                    GUI.depth = 0;
                    if (GUI.Button(new Rect(5, 5, 200, 40), "Items Drop", Styles.Button_Style(ShowItemDropSection))) { Btn_ItemsDrop_Click(); }
                    if (ShowItemDropSection)
                    {
                        GUI.DrawTexture(new Rect(0, Menu_Size_h, 210, ItemsDrop_h), windowBackground);
                        if (GUI.Button(new Rect(5, 55, 200, 40), "Unlock", Styles.Button_Style(ShowItemDropUnlockSection))) { Btn_ItemsDrop_Unlock_Click(); }
                        if (GUI.Button(new Rect(5, 100, 200, 40), "On Death", Styles.Button_Style(ShowItemDropOnDeathSection))) { Btn_ItemsDrop_OnDeath_Click(); }
                        if (ShowItemDropUnlockSection)
                        {
                            ShowItemDropOnDeathSection = false;
                            GUI.DrawTexture(new Rect(210, Menu_Size_h, 210, ItemsDropUnlock_h), windowBackground);                            
                            GUI.TextField(new Rect(215, 55, 200, 30), "Basic", Styles.Title_Style());
                            if (GUI.Button(new Rect(215, 90, 200, 40), "Unlock", Styles.Button_Style(Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForAll))) { Btn_ItemsDrop_Unlock_Basic_All_Click(); }
                            if (GUI.Button(new Rect(215, 135, 200, 40), "Only UnDropable", Styles.Button_Style(Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForUndropableOnly))) { Btn_ItemsDrop_Unlock_Basic_UnDropable_Click(); }                            
                            GUI.TextField(new Rect(215, 180, 200, 30), "Unique", Styles.Title_Style());
                            if (GUI.Button(new Rect(215, 215, 200, 40), "Unlock", Styles.Button_Style(Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForAll))) { Btn_ItemsDrop_Unlock_Unique_All_Click(); }
                            if (GUI.Button(new Rect(215, 260, 200, 40), "Only UnDropable", Styles.Button_Style(Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForUndropableOnly))) { Btn_ItemsDrop_Unlock_Unique_UnDropable_Click(); }
                        }
                        else if (ShowItemDropOnDeathSection)
                        {
                            ShowItemDropUnlockSection = false;
                            GUI.DrawTexture(new Rect(210, Menu_Size_h, 210, ItemsOnDeath_h), windowBackground);
                            float pos_y = Menu_Size_h + 5;
                            if (GUI.Button(new Rect(365, pos_y, 50, 40), "Enable", Styles.Button_Style(Mods.Scene_Mods.Enable_DeathItemDrop_goldMultiplier))) { Btn_ItemsDrop_OnDeath_GoldMultiplier_Click(); }
                            Mods.Scene_Mods.DeathItemDrop_goldMultiplier = CustomControls.Value("Gold Multiplier", Mods.Scene_Mods.DeathItemDrop_goldMultiplier, 215, pos_y);
                            pos_y += 90;
                            if (GUI.Button(new Rect(365, pos_y, 50, 40), "Enable", Styles.Button_Style(Mods.Scene_Mods.Enable_DeathItemDrop_ItemMultiplier))) { Btn_ItemsDrop_OnDeath_ItemsMultiplier_Click(); }
                            Mods.Scene_Mods.DeathItemDrop_ItemMultiplier = CustomControls.Value("Items Multiplier", Mods.Scene_Mods.DeathItemDrop_ItemMultiplier, 215, pos_y);
                            pos_y += 90;
                            if (GUI.Button(new Rect(365, pos_y, 50, 40), "Enable", Styles.Button_Style(Mods.Scene_Mods.Enable_DeathItemDrop_AdditionalRare))) { Btn_ItemsDrop_OnDeath_AdditionalRare_Click(); }
                            GUI.TextField(new Rect(215, pos_y, 150, 40), "Additional Rare", Styles.Title_Style());
                        }
                    }
                    else
                    {
                        ShowItemDropUnlockSection = false;
                        ShowItemDropOnDeathSection = false;
                    }
                    if (GUI.Button(new Rect(210, 5, 200, 40), "Auto Pickup", Styles.Button_Style(ShowAutoPickupSection))) { Btn_AutoPickup_Click(); }
                    if (ShowAutoPickupSection)
                    {
                        GUI.DrawTexture(new Rect(210, Menu_Size_h, 210, AutoPickup_h), windowBackground);
                        if (GUI.Button(new Rect(215, 55, 200, 40), "Gold", Styles.Button_Style(Mods.Items_Mods.AutoLoot.AutoPickup_Gold))) { Btn_AutoPickup_Gold_Click(); }
                        if (GUI.Button(new Rect(215, 100, 200, 40), "Keys", Styles.Button_Style(Mods.Items_Mods.AutoLoot.AutoPickup_Key))) { Btn_AutoPickup_Key_Click(); }
                        if (GUI.Button(new Rect(215, 145, 200, 40), "Unique & Set", Styles.Button_Style(Mods.Items_Mods.AutoLoot.AutoPickup_UniqueAndSet))) { Btn_AutoPickup_UniqueAndSet_Click(); }
                        if (GUI.Button(new Rect(215, 190, 200, 40), "Xp Tome", Styles.Button_Style(Mods.Items_Mods.AutoLoot.AutoPickup_XpTome))) { Btn_AutoPickup_XpTome_Click(); }
                        if (GUI.Button(new Rect(215, 235, 200, 40), "Materials", Styles.Button_Style(Mods.Items_Mods.AutoLoot.AutoPickup_Craft))) { Btn_AutoPickup_Materials_Click(); }
                        if (GUI.Button(new Rect(215, 280, 200, 40), "AutoStore Materials", Styles.Button_Style(Mods.Items_Mods.AutoLoot.AutoStore_Materials))) { Btn_AutoSrore_Materials_Click(); }
                    }
                    if (GUI.Button(new Rect(415, 5, 200, 40), "Scene", Styles.Button_Style(ShowSceneSection))) { Btn_Scene_Click(); }
                    if (ShowSceneSection)
                    {
                        GUI.DrawTexture(new Rect(415, Menu_Size_h, 210, Scene_h), windowBackground);
                        if (GUI.Button(new Rect(420, 55, 200, 40), "Density", Styles.Button_Style(false))) { Btn_Scene_0_Click(); }
                        if (GUI.Button(new Rect(420, 100, 200, 40), "2", Styles.Button_Style(false))) { Btn_Scene_1_Click(); }
                        if (GUI.Button(new Rect(420, 145, 200, 40), "3", Styles.Button_Style(false))) { Btn_Scene_2_Click(); }
                    }
                    GUI.DragWindow(new Rect(0, 0, 10000, 10000));
                }
            }
        }
        public static void Btn_ItemsDrop_Click()
        {
            ShowAutoPickupSection = false;
            ShowSceneSection = false;
            ShowItemDropSection = !ShowItemDropSection;            
        }
        public static void Btn_ItemsDrop_Unlock_Click()
        {
            ShowItemDropOnDeathSection = false;
            ShowItemDropUnlockSection = !ShowItemDropUnlockSection;            
        }
        public static void Btn_ItemsDrop_Unlock_Basic_All_Click()
        {
            Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForAll = !Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForAll;
        }
        public static void Btn_ItemsDrop_Unlock_Basic_UnDropable_Click()
        {
            Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForUndropableOnly = !Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForUndropableOnly;
        }
        public static void Btn_ItemsDrop_Unlock_Unique_All_Click()
        {
            Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForAll = !Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForAll;
        }
        public static void Btn_ItemsDrop_Unlock_Unique_UnDropable_Click()
        {
            Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForUndropableOnly = !Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForUndropableOnly;
        }

        public static void Btn_ItemsDrop_OnDeath_Click()
        {
            ShowItemDropUnlockSection = false;
            ShowItemDropOnDeathSection = !ShowItemDropOnDeathSection;
        }
        public static void Btn_ItemsDrop_OnDeath_GoldMultiplier_Click()
        {
            Mods.Scene_Mods.Enable_DeathItemDrop_goldMultiplier = !Mods.Scene_Mods.Enable_DeathItemDrop_goldMultiplier;
        }
        public static void Btn_ItemsDrop_OnDeath_ItemsMultiplier_Click()
        {
            Mods.Scene_Mods.Enable_DeathItemDrop_ItemMultiplier = !Mods.Scene_Mods.Enable_DeathItemDrop_ItemMultiplier;
        }
        public static void Btn_ItemsDrop_OnDeath_AdditionalRare_Click()
        {
            Mods.Scene_Mods.Enable_DeathItemDrop_AdditionalRare = !Mods.Scene_Mods.Enable_DeathItemDrop_AdditionalRare;
        }

        #region AutoLoot
        public static void Btn_AutoPickup_Click()
        {
            ShowSceneSection = false;
            ShowItemDropSection = false;
            ShowAutoPickupSection = !ShowAutoPickupSection;            
        }
        public static void Btn_AutoPickup_Gold_Click()
        {
            Mods.Items_Mods.AutoLoot.AutoPickup_Gold = !Mods.Items_Mods.AutoLoot.AutoPickup_Gold;
        }
        public static void Btn_AutoPickup_Key_Click()
        {
            Mods.Items_Mods.AutoLoot.AutoPickup_Key = !Mods.Items_Mods.AutoLoot.AutoPickup_Key;
        }
        public static void Btn_AutoPickup_UniqueAndSet_Click()
        {
            Mods.Items_Mods.AutoLoot.AutoPickup_UniqueAndSet = !Mods.Items_Mods.AutoLoot.AutoPickup_UniqueAndSet;
        }
        public static void Btn_AutoPickup_XpTome_Click()
        {
            Mods.Items_Mods.AutoLoot.AutoPickup_XpTome = !Mods.Items_Mods.AutoLoot.AutoPickup_XpTome;
        }
        public static void Btn_AutoPickup_Materials_Click()
        {
            Mods.Items_Mods.AutoLoot.AutoPickup_Craft = !Mods.Items_Mods.AutoLoot.AutoPickup_Craft;
        }        
        public static void Btn_AutoSrore_Materials_Click()
        {
            Mods.Items_Mods.AutoLoot.AutoStore_Materials = !Mods.Items_Mods.AutoLoot.AutoStore_Materials;
        }
        #endregion
        public static void Btn_Scene_Click()
        {
            ShowItemDropSection = false;
            ShowAutoPickupSection = false;
            ShowSceneSection = !ShowSceneSection;            
        }
        public static void Btn_Scene_0_Click()
        {
            Main.logger_instance.Msg("Btn_Scene_0_Click");
        }
        public static void Btn_Scene_1_Click()
        {
            Main.logger_instance.Msg("Btn_Scene_1_Click");
        }
        public static void Btn_Scene_2_Click()
        {
            Main.logger_instance.Msg("Btn_Scene_2_Click");
        }
    }
}
