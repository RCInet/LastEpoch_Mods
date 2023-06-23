using Newtonsoft.Json.Linq;
using System.Linq;
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
        public static float ItemsDrop_h = 90f;
        private static bool ShowItemDropUnlockSection = false;
        public static float ItemsDropUnlock_h = 255;
        private static bool ShowItemDropOnDeathSection = false;
        public static float ItemsOnDeath_h = 305f;
        private static bool ShowAutoPickupSection = false;
        public static float AutoPickup_h = 270f;
        private static bool ShowSceneSection = false;
        public static float Scene_h = 85f;


        public static void Update()
        {
            if (isMenuOpen)
            {
                Menu_Window_Rect = GUI.Window(WindowId, Menu_Window_Rect, (GUI.WindowFunction)ModMenu, "", Styles.Window_Style());
                void ModMenu(int windowID)
                {
                    Texture2D windowBackground = Functions.MakeTextureFromColor(2, 2, Color.black);
                    GUI.DrawTexture(new Rect(0, 0, Menu_Window_Rect.width, Menu_Size_h), windowBackground);
                    GUI.depth = 0;
                    if (GUI.Button(new Rect(5, 5, 200, 40), "Items Drop", Styles.Button_Style(ShowItemDropSection))) { Btn_ItemsDrop_Click(); }
                    if (ShowItemDropSection)
                    {
                        float pos_y = Menu_Size_h;
                        GUI.DrawTexture(new Rect(0, pos_y, 210, ItemsDrop_h), windowBackground);
                        float pos_x = 5f;                        
                        if (GUI.Button(new Rect(pos_x, pos_y, 200, 40), "Unlock", Styles.Button_Style(ShowItemDropUnlockSection))) { Btn_ItemsDrop_Unlock_Click(); }
                        pos_y += 45;
                        if (GUI.Button(new Rect(pos_x, pos_y, 200, 40), "On Death", Styles.Button_Style(ShowItemDropOnDeathSection))) { Btn_ItemsDrop_OnDeath_Click(); }
                        //pos_y += 45;
                        //Mods.Character_Mods.increase_equipment_droprate = CustomControls.Value("Equipment Multiplier", Mods.Character_Mods.increase_equipment_droprate, pos_x, pos_y, Mods.Character_Mods.Enable_increase_equipment_droprate, Ui.Menu.Btn_ItemsDrop_Increase_Items_Click);                        
                        //pos_y += 85;
                        //Mods.Character_Mods.increase_uniques_droprate = CustomControls.Value("Unique Multiplier", Mods.Character_Mods.increase_uniques_droprate, pos_x, pos_y, Mods.Character_Mods.Enable_increase_uniques_droprate, Ui.Menu.Btn_ItemsDrop_Increase_Unique_Click);
                        //pos_y += 85;
                        //Mods.Character_Mods.increase_equipmentshards_droprate = CustomControls.Value("Shard Multiplier", Mods.Character_Mods.increase_equipmentshards_droprate, pos_x, pos_y, Mods.Character_Mods.Enable_increase_equipmentshards_droprate, Ui.Menu.Btn_ItemsDrop_Increase_Shard_Click);                        
                        if (ShowItemDropUnlockSection)
                        {
                            ShowItemDropOnDeathSection = false;
                            pos_x = 205;
                            pos_y = Menu_Size_h + 5;
                            GUI.DrawTexture(new Rect(pos_x, pos_y, 210, ItemsDropUnlock_h), windowBackground);
                            pos_x += 5;
                            pos_y += 5;
                            GUI.TextField(new Rect(pos_x, pos_y, 200, 30), "Basic", Styles.Title_Style());
                            pos_y += 35;
                            CustomControls.EnableButton("Unlock Drop", pos_x, pos_y, Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForAll, Ui.Menu.Btn_ItemsDrop_Unlock_Basic_All_Click);
                            pos_y += 45;
                            CustomControls.EnableButton("Only UnDropable", pos_x, pos_y, Mods.Items_Mods.Basic.EquipmentItem_UnlockDropForUndropableOnly, Ui.Menu.Btn_ItemsDrop_Unlock_Basic_UnDropable_Click);
                            pos_y += 45;
                            GUI.TextField(new Rect(pos_x, pos_y, 200, 30), "Unique", Styles.Title_Style());
                            pos_y += 35;
                            CustomControls.EnableButton("Unlock Drop", pos_x, pos_y, Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForAll, Ui.Menu.Btn_ItemsDrop_Unlock_Unique_All_Click);
                            pos_y += 45;
                            CustomControls.EnableButton("Only UnDropable", pos_x, pos_y, Mods.Items_Mods.Unique.UniqueList_Entry_UnlockDropForUndropableOnly, Ui.Menu.Btn_ItemsDrop_Unlock_Unique_UnDropable_Click);                            
                        }
                        else if (ShowItemDropOnDeathSection)
                        {
                            ShowItemDropUnlockSection = false;                            
                            pos_x = 205;
                            pos_y = Menu_Size_h + 40;
                            GUI.DrawTexture(new Rect(pos_x, pos_y, 210, ItemsOnDeath_h), windowBackground);
                            pos_x += 5;
                            pos_y += 5;
                            Mods.Scene_Mods.DeathItemDrop_goldMultiplier = CustomControls.Value("Gold Multiplier", Mods.Scene_Mods.DeathItemDrop_goldMultiplier, pos_x, pos_y, Mods.Scene_Mods.Enable_DeathItemDrop_goldMultiplier, Ui.Menu.Btn_ItemsDrop_OnDeath_GoldMultiplier_Click);
                            pos_y += 85;
                            Mods.Scene_Mods.DeathItemDrop_ItemMultiplier = CustomControls.Value("Items Multiplier", Mods.Scene_Mods.DeathItemDrop_ItemMultiplier, pos_x, pos_y, Mods.Scene_Mods.Enable_DeathItemDrop_ItemMultiplier, Ui.Menu.Btn_ItemsDrop_OnDeath_ItemsMultiplier_Click);
                            pos_y += 85;
                            CustomControls.EnableButton("Additional Rare", pos_x, pos_y, Mods.Scene_Mods.Enable_DeathItemDrop_AdditionalRare, Ui.Menu.Btn_ItemsDrop_OnDeath_AdditionalRare_Click);
                            pos_y += 45; //
                            Mods.Scene_Mods.DeathItemDrop_Experience = CustomControls.LongValue("Experience Multiplier", Mods.Scene_Mods.DeathItemDrop_Experience, pos_x, pos_y, Mods.Scene_Mods.Enable_DeathItemDrop_Experience, Ui.Menu.Btn_ItemsDrop_OnDeath_Experience_Click);
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
                        float pos_x = 205;
                        float pos_y = Menu_Size_h;
                        GUI.DrawTexture(new Rect(pos_x, pos_y, 210, AutoPickup_h), windowBackground);
                        pos_x += 5;
                        CustomControls.EnableButton("Gold", pos_x, pos_y, Mods.Items_Mods.AutoLoot.AutoPickup_Gold, Ui.Menu.Btn_AutoPickup_Gold_Click);
                        pos_y += 45;
                        CustomControls.EnableButton("Keys", pos_x, pos_y, Mods.Items_Mods.AutoLoot.AutoPickup_Key, Ui.Menu.Btn_AutoPickup_Key_Click);
                        pos_y += 45;
                        CustomControls.EnableButton("Unique & Set", pos_x, pos_y, Mods.Items_Mods.AutoLoot.AutoPickup_UniqueAndSet, Ui.Menu.Btn_AutoPickup_UniqueAndSet_Click);
                        pos_y += 45;
                        CustomControls.EnableButton("Xp Tome", pos_x, pos_y, Mods.Items_Mods.AutoLoot.AutoPickup_XpTome, Ui.Menu.Btn_AutoPickup_XpTome_Click);
                        pos_y += 45;
                        CustomControls.EnableButton("Materials", pos_x, pos_y, Mods.Items_Mods.AutoLoot.AutoPickup_Craft, Ui.Menu.Btn_AutoPickup_Materials_Click);
                        pos_y += 45;
                        CustomControls.EnableButton("AutoStore Materials", pos_x, pos_y, Mods.Items_Mods.AutoLoot.AutoStore_Materials, Ui.Menu.Btn_AutoSrore_Materials_Click);
                    }
                    if (GUI.Button(new Rect(415, 5, 200, 40), "Scene", Styles.Button_Style(ShowSceneSection))) { Btn_Scene_Click(); }
                    if (ShowSceneSection)
                    {
                        float pos_x = 410;
                        float pos_y = Menu_Size_h;
                        GUI.DrawTexture(new Rect(pos_x, pos_y, 210, Scene_h), windowBackground);
                        pos_x += 5;
                        Mods.Scene_Mods.SpawnerPlacementManager_defaultSpawnerDensity = CustomControls.Value("Density Multiplier", Mods.Scene_Mods.SpawnerPlacementManager_defaultSpawnerDensity, pos_x, pos_y, Mods.Scene_Mods.Enable_SpawnerPlacementManager_defaultSpawnerDensity, Ui.Menu.Btn_Scene_Density_Click);
                        //pos_y += 85;
                        //Mods.Scene_Mods.SpawnerPlacementManager_IncreaseExperience = CustomControls.Value("Experience Multiplier", Mods.Scene_Mods.SpawnerPlacementManager_IncreaseExperience, pos_x, pos_y, Mods.Scene_Mods.Enable_SpawnerPlacementManager_IncreaseExperience, Ui.Menu.Btn_Scene_Experience_Click);                        
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
        public static void Btn_ItemsDrop_OnDeath_Experience_Click()
        {
            Mods.Scene_Mods.Enable_DeathItemDrop_Experience = !Mods.Scene_Mods.Enable_DeathItemDrop_Experience;
        }

        public static void Btn_ItemsDrop_Increase_Items_Click()
        {
            Mods.Character_Mods.Enable_increase_equipment_droprate = !Mods.Character_Mods.Enable_increase_equipment_droprate;
        }
        public static void Btn_ItemsDrop_Increase_Unique_Click()
        {
            Mods.Character_Mods.Enable_increase_uniques_droprate = !Mods.Character_Mods.Enable_increase_uniques_droprate;
        }
        public static void Btn_ItemsDrop_Increase_Shard_Click()
        {
            Mods.Character_Mods.Enable_increase_equipmentshards_droprate = !Mods.Character_Mods.Enable_increase_equipmentshards_droprate;
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
        public static void Btn_Scene_Density_Click()
        {
            Mods.Scene_Mods.Enable_SpawnerPlacementManager_defaultSpawnerDensity = !Mods.Scene_Mods.Enable_SpawnerPlacementManager_defaultSpawnerDensity;
        }
        public static void Btn_Scene_Experience_Click()
        {
            Mods.Scene_Mods.Enable_SpawnerPlacementManager_IncreaseExperience = !Mods.Scene_Mods.Enable_SpawnerPlacementManager_IncreaseExperience;
        }
    }
}
