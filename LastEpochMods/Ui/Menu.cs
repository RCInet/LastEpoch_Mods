using UnityEngine;

namespace LastEpochMods.Ui
{
    public class Menu
    {
        public static Texture2D windowBackground = null;
        public static Texture2D texture_grey = null;
        public static Texture2D texture_green = null;

        public static void Update()
        {
            if (isMenuOpen)
            {
                //Texture2D Refs
                if (windowBackground == null) { windowBackground = Functions.MakeTextureFromColor(2, 2, Color.black); }
                if (texture_grey == null) { texture_grey = Functions.MakeTextureFromColor(2, 2, Color.grey); }
                if (texture_green == null) { texture_green = Functions.MakeTextureFromColor(2, 2, Color.green); }
                
                GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), "");
                float start_x = (Screen.width / 2) - (Menu_Window_Rect.width / 2);
                float start_y = 0;

                float pos_x = start_x;
                float pos_y = start_y;
                GUI.DrawTexture(new Rect(pos_x, pos_y, Menu_Window_Rect.width, Menu_Size_h), windowBackground);
                GUI.depth = 0;
                pos_x += btn_margin_w;
                pos_y += btn_margin_h;

                if (GUI.Button(new Rect(pos_x, pos_y, btn_size_w, btn_size_h), "Items Drop", Styles.Button_Style(ShowItemDropSection))) { Btn_ItemsDrop_Click(); }
                pos_x += btn_size_w + btn_margin_w;
                if (GUI.Button(new Rect(pos_x, pos_y, btn_size_w, btn_size_h), "Auto Pickup", Styles.Button_Style(ShowAutoPickupSection))) { Btn_AutoPickup_Click(); }
                pos_x += btn_size_w + btn_margin_w;
                if (GUI.Button(new Rect(pos_x, pos_y, btn_size_w, btn_size_h), "Scene", Styles.Button_Style(ShowSceneSection))) { Btn_Scene_Click(); }
                pos_x += btn_size_w + btn_margin_w;
                if (GUI.Button(new Rect(pos_x, pos_y, btn_size_w, btn_size_h), "Remove Req", Styles.Button_Style(ShowRemoveReqSection))) { Btn_RemoveReq_Click(); }
                pos_x += btn_size_w + btn_margin_w;
                if (GUI.Button(new Rect(pos_x, pos_y, btn_size_w, btn_size_h), "Dungeons", Styles.Button_Style(ShowDungeonsSection))) { Btn_Dungeons_Click(); }
                pos_x += btn_size_w + btn_margin_w;
                if (GUI.Button(new Rect(pos_x, pos_y, btn_size_w, btn_size_h), "Monoliths", Styles.Button_Style(ShowMonolithsSection))) { Btn_Monoliths_Click(); }

                if (ShowItemDropSection)
                {
                    pos_x = start_x;
                    pos_y = start_y + Menu_Size_h;
                    GUI.DrawTexture(new Rect(pos_x, pos_y, (btn_size_w + (2 * btn_margin_w)), ItemsDrop_h), windowBackground);
                    pos_x += 5f;
                    Mods.Items_Mods.Drop_Mods.GenerateItem_Rarity = CustomControls.ByteValue("Item Rarity", 0, 9, Mods.Items_Mods.Drop_Mods.GenerateItem_Rarity, pos_x, pos_y, Mods.Items_Mods.Drop_Mods.Enable_Rarity, Ui.Menu.Btn_ItemsDrop_Rarity_Roll_Click);
                    pos_y += 80;
                    CustomControls.RarityInfos(pos_x, pos_y);
                    pos_y += 65;
                    Mods.Items_Mods.Drop_Mods.Roll_Implicit = CustomControls.ByteValue("Implicits", 0, 255, Mods.Items_Mods.Drop_Mods.Roll_Implicit, pos_x, pos_y, Mods.Items_Mods.Drop_Mods.Enable_RollImplicit, Ui.Menu.Btn_ItemsDrop_Implicit_Roll_Click);
                    pos_y += 85;
                    Mods.Items_Mods.Drop_Mods.Roll_AffixValue =CustomControls.ByteValue("Affix Values", 0, 255, Mods.Items_Mods.Drop_Mods.Roll_AffixValue, pos_x, pos_y, Mods.Items_Mods.Drop_Mods.Enable_AffixsValue, Ui.Menu.Btn_ItemsDrop_Affix_Values_Click);
                    pos_y += 85;
                    Mods.Items_Mods.Drop_Mods.Roll_AffixTier = CustomControls.ByteValue("Affix Tier", 1, 7, Mods.Items_Mods.Drop_Mods.Roll_AffixTier, pos_x, pos_y, Mods.Items_Mods.Drop_Mods.Enable_AffixsTier, Ui.Menu.Btn_ItemsDrop_Affix_Tier_Click);
                    pos_y += 85;
                    Mods.Items_Mods.Drop_Mods.Roll_UniqueMod = CustomControls.ByteValue("Unique Mods", 0, 255, Mods.Items_Mods.Drop_Mods.Roll_UniqueMod, pos_x, pos_y, Mods.Items_Mods.Drop_Mods.Enable_UniqueMod, Ui.Menu.Btn_ItemsDrop_Unique_Mods_Click);
                    pos_y += 85;
                    Mods.Items_Mods.Drop_Mods.Roll_Legendary_Potencial = System.Convert.ToInt32(CustomControls.FloatValue("Legendary Potencial", 0f, 4f, Mods.Items_Mods.Drop_Mods.Roll_Legendary_Potencial, pos_x, pos_y, Mods.Items_Mods.Drop_Mods.Enable_RollLegendayPotencial, Ui.Menu.Btn_ItemsDrop_Legendary_Potencial_Click));
                    pos_y += 85;
                    Mods.Items_Mods.Drop_Mods.Roll_Weaver_Will = System.Convert.ToInt32(CustomControls.FloatValue("Weaver Will", 5f, 28f, Mods.Items_Mods.Drop_Mods.Roll_Weaver_Will, pos_x, pos_y, Mods.Items_Mods.Drop_Mods.Enable_RollWeaverWill, Ui.Menu.Btn_ItemsDrop_Weaver_Will_Click));
                }
                if (ShowAutoPickupSection)
                {
                    pos_x = start_x + (btn_size_w + btn_margin_w);
                    pos_y = start_y + Menu_Size_h;
                    GUI.DrawTexture(new Rect(pos_x, pos_y, (btn_size_w + (2 * btn_margin_w)), AutoPickup_h), windowBackground);
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
                if (ShowSceneSection)
                {
                    pos_x = start_x + ((btn_size_w + btn_margin_w) * 2);
                    pos_y = start_y + Menu_Size_h;
                    GUI.DrawTexture(new Rect(pos_x, pos_y, (btn_size_w + (2 * btn_margin_w)), Scene_h), windowBackground);
                    pos_x += 5;
                    Mods.Scene_Mods.SpawnerPlacementManager_defaultSpawnerDensity = CustomControls.FloatValue("Density Multiplier", 0f, 255f, Mods.Scene_Mods.SpawnerPlacementManager_defaultSpawnerDensity, pos_x, pos_y, Mods.Scene_Mods.Enable_SpawnerPlacementManager_defaultSpawnerDensity, Ui.Menu.Btn_Scene_Density_Click);
                    pos_y += 85;
                    Mods.Scene_Mods.DeathItemDrop_goldMultiplier = CustomControls.FloatValue("Gold Multiplier", 0f, 255f, Mods.Scene_Mods.DeathItemDrop_goldMultiplier, pos_x, pos_y, Mods.Scene_Mods.Enable_DeathItemDrop_goldMultiplier, Ui.Menu.Btn_ItemsDrop_OnDeath_GoldMultiplier_Click);
                    pos_y += 85;
                    Mods.Scene_Mods.DeathItemDrop_ItemMultiplier = CustomControls.FloatValue("Items Multiplier", 0f, 255f, Mods.Scene_Mods.DeathItemDrop_ItemMultiplier, pos_x, pos_y, Mods.Scene_Mods.Enable_DeathItemDrop_ItemMultiplier, Ui.Menu.Btn_ItemsDrop_OnDeath_ItemsMultiplier_Click);
                    pos_y += 85;
                    Mods.Scene_Mods.DeathItemDrop_Experience = CustomControls.LongValue("Experience Multiplier", long.MinValue, long.MaxValue, Mods.Scene_Mods.DeathItemDrop_Experience, pos_x, pos_y, Mods.Scene_Mods.Enable_DeathItemDrop_Experience, Ui.Menu.Btn_ItemsDrop_OnDeath_Experience_Click);
                    pos_y += 85;
                    CustomControls.EnableButton("Waypoint Unlock", pos_x, pos_y, Mods.Scene_Mods.Waypoints_Mods.Enable_Waypoint_Unlock, Ui.Menu.Btn_Scene_Waypoint_Unlock_Click);
                }                
                if (ShowRemoveReqSection)
                {
                    pos_x = start_x + ((btn_size_w + btn_margin_w) * 3);
                    pos_y = start_y + Menu_Size_h;
                    GUI.DrawTexture(new Rect(pos_x, pos_y, (btn_size_w + (2 * btn_margin_w)), RemoveReq_h), windowBackground);
                    pos_x += 5;
                    CustomControls.EnableButton("Remove Level", pos_x, pos_y, Mods.Items_Mods.RemoveReq.Remove_LevelReq, Ui.Menu.Btn_Items_Remove_Level_Req_Click);
                    pos_y += 45;
                    CustomControls.EnableButton("Remove Class", pos_x, pos_y, Mods.Items_Mods.RemoveReq.Remove_ClassReq, Ui.Menu.Btn_Items_Remove_Class_Req_Click);
                    pos_y += 45;
                    CustomControls.EnableButton("Remove SubClass", pos_x, pos_y, Mods.Items_Mods.RemoveReq.Remove_SubClassReq, Ui.Menu.Btn_Items_Remove_SubClass_Req_Click);
                    pos_y += 45;

                }
                if (ShowDungeonsSection)
                {
                    pos_x = start_x + ((btn_size_w + btn_margin_w) * 4);
                    pos_y = start_y + Menu_Size_h;
                    GUI.DrawTexture(new Rect(pos_x, pos_y, (btn_size_w + (2 * btn_margin_w)), Dungeons_h), windowBackground);
                    pos_x += 5;
                    CustomControls.EnableButton("No Key Need", pos_x, pos_y, Mods.Scene_Mods.Dungeons_Mods.Enable_Dungeons_WithoutKey, Ui.Menu.Btn_Dungeons_WithoutKey_Click);
                    pos_y += 45;
                    CustomControls.EnableButton("Objective Reveal", pos_x, pos_y, Mods.Scene_Mods.Dungeons_Mods.Enable_Dungeons_ObjectiveReveal, Ui.Menu.Btn_Dungeon_Objective_Reveal_OnStart_Click);
                    pos_y += 45;
                }
                if (ShowMonolithsSection)
                {
                    pos_x = start_x + ((btn_size_w + btn_margin_w) * 5);
                    pos_y = start_y + Menu_Size_h;
                    GUI.DrawTexture(new Rect(pos_x, pos_y, (btn_size_w + (2 * btn_margin_w)), Monolith_h), windowBackground);
                    pos_x += 5;
                    CustomControls.EnableButton("Stability Max", pos_x, pos_y, Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_Stability, Ui.Menu.Btn_Monolith_Stability_Click);
                    pos_y += 45;
                    Mods.Scene_Mods.Monoliths_mods.Max_Stability = System.Convert.ToInt32(CustomControls.FloatValue("Max Stability", 0f, 100f, Mods.Scene_Mods.Monoliths_mods.Max_Stability, pos_x, pos_y, Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_Overide_Max_Stability, Ui.Menu.Btn_Monolith_Override_Max_Stability_Click));
                    pos_y += 85;
                    Mods.Scene_Mods.Monoliths_mods.Monolith_EnnemiesDefeat_OnStart = System.Convert.ToInt32(CustomControls.FloatValue("Enemies Defeat", 0f, 100f, Mods.Scene_Mods.Monoliths_mods.Monolith_EnnemiesDefeat_OnStart, pos_x, pos_y, Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_EnnemiesDefeat_OnStart, Ui.Menu.Btn_Monolith_Enemies_Defeat_OnStart_Click));
                    pos_y += 85;
                    CustomControls.EnableButton("Objective Reveal", pos_x, pos_y, Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_ObjectiveReveal, Ui.Menu.Btn_Monolith_Objective_Reveal_OnStart_Click);
                    pos_y += 45;
                    CustomControls.EnableButton("Complete Objective", pos_x, pos_y, Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_Complete_Objective, Ui.Menu.Btn_Monolith_Complete_Objective_OnStart_Click);
                    pos_y += 45;
                    CustomControls.EnableButton("No Lost When Die", pos_x, pos_y, Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_NoDie, Ui.Menu.Btn_Monolith_NoDie_Click);
                    pos_y += 45;
                }
            }
        }
        
        #region Menu
        public static bool isMenuOpen;
        public static int WindowId = 0;
        public static Rect Menu_Window_Rect = new Rect(5f, 5f, 1235f, 860f); //Debug for Drag
        private static float Menu_Size_h = 50f;

        //Menu Refs
        private static float margin_x = 10f;
        private static float margin_y = 10f;
        private static float btn_size_w = 200f;
        private static float btn_size_h = 40f;
        private static float btn_margin_w = 5f;
        private static float btn_margin_h = 5f;
        #endregion
        #region ItemDrop
        private static bool ShowItemDropSection = false;
        public static float ItemsDrop_h = 655f;

        public static void Btn_ItemsDrop_Click()
        {
            ShowAutoPickupSection = false;
            ShowSceneSection = false;
            ShowRemoveReqSection = false;
            ShowDungeonsSection = false;
            ShowMonolithsSection = false;
            ShowItemDropSection = !ShowItemDropSection;            
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
        public static void Btn_ItemsDrop_Rarity_Roll_Click()
        {
            Mods.Items_Mods.Drop_Mods.Enable_Rarity = !Mods.Items_Mods.Drop_Mods.Enable_Rarity;
        }
        public static void Btn_ItemsDrop_Implicit_Roll_Click()
        {
            Mods.Items_Mods.Drop_Mods.Enable_RollImplicit = !Mods.Items_Mods.Drop_Mods.Enable_RollImplicit;
        }
        public static void Btn_ItemsDrop_Affix_Values_Click()
        {
            Mods.Items_Mods.Drop_Mods.Enable_AffixsValue = !Mods.Items_Mods.Drop_Mods.Enable_AffixsValue;
        }
        public static void Btn_ItemsDrop_Affix_Tier_Click()
        {
            Mods.Items_Mods.Drop_Mods.Enable_AffixsTier = !Mods.Items_Mods.Drop_Mods.Enable_AffixsTier;
        }
        public static void Btn_ItemsDrop_Unique_Mods_Click()
        {
            Mods.Items_Mods.Drop_Mods.Enable_UniqueMod = !Mods.Items_Mods.Drop_Mods.Enable_UniqueMod;
        }
        public static void Btn_ItemsDrop_Legendary_Potencial_Click()
        {
            Mods.Items_Mods.Drop_Mods.Enable_RollLegendayPotencial = !Mods.Items_Mods.Drop_Mods.Enable_RollLegendayPotencial;
        }
        public static void Btn_ItemsDrop_Weaver_Will_Click()
        {
            Mods.Items_Mods.Drop_Mods.Enable_RollWeaverWill = !Mods.Items_Mods.Drop_Mods.Enable_RollWeaverWill;
        }
        #endregion
        #region AutoLoot
        private static bool ShowAutoPickupSection = false;
        public static float AutoPickup_h = 270f;

        public static void Btn_AutoPickup_Click()
        {
            ShowSceneSection = false;
            ShowItemDropSection = false;
            ShowRemoveReqSection = false;
            ShowDungeonsSection = false;
            ShowMonolithsSection = false;
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
        #region Scenes
        private static bool ShowSceneSection = false;
        public static float Scene_h = 385f;

        public static void Btn_Scene_Click()
        {
            ShowItemDropSection = false;
            ShowAutoPickupSection = false;
            ShowRemoveReqSection = false;
            ShowDungeonsSection = false;
            ShowMonolithsSection = false;
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
        public static void Btn_Scene_Waypoint_Unlock_Click()
        {
            Mods.Scene_Mods.Waypoints_Mods.Enable_Waypoint_Unlock = !Mods.Scene_Mods.Waypoints_Mods.Enable_Waypoint_Unlock;
        }
        #endregion
        #region Remove Req
        private static bool ShowRemoveReqSection = false;
        public static float RemoveReq_h = 135f;

        public static void Btn_RemoveReq_Click()
        {
            ShowItemDropSection = false;
            ShowAutoPickupSection = false;
            ShowSceneSection = false;
            ShowDungeonsSection = false;
            ShowMonolithsSection = false;
            ShowRemoveReqSection = !ShowRemoveReqSection;
        }
        public static void Btn_Items_Remove_Level_Req_Click()
        {
            Mods.Items_Mods.RemoveReq.Remove_LevelReq = !Mods.Items_Mods.RemoveReq.Remove_LevelReq;
        }
        public static void Btn_Items_Remove_Class_Req_Click()
        {
            Mods.Items_Mods.RemoveReq.Remove_ClassReq = !Mods.Items_Mods.RemoveReq.Remove_ClassReq;
        }
        public static void Btn_Items_Remove_SubClass_Req_Click()
        {
            Mods.Items_Mods.RemoveReq.Remove_SubClassReq = !Mods.Items_Mods.RemoveReq.Remove_SubClassReq;
        }
        #endregion
        #region Dungeons
        private static bool ShowDungeonsSection = false;
        public static float Dungeons_h = 90f;

        public static void Btn_Dungeons_Click()
        {
            ShowItemDropSection = false;
            ShowAutoPickupSection = false;
            ShowSceneSection = false;
            ShowRemoveReqSection = false;            
            ShowMonolithsSection = false;
            ShowDungeonsSection = !ShowDungeonsSection;
        }        
        public static void Btn_Dungeons_WithoutKey_Click()
        {
            Mods.Scene_Mods.Dungeons_Mods.Enable_Dungeons_WithoutKey = !Mods.Scene_Mods.Dungeons_Mods.Enable_Dungeons_WithoutKey;
        }
        public static void Btn_Dungeon_Objective_Reveal_OnStart_Click()
        {
            Mods.Scene_Mods.Dungeons_Mods.Enable_Dungeons_ObjectiveReveal = !Mods.Scene_Mods.Dungeons_Mods.Enable_Dungeons_ObjectiveReveal;
        }
        #endregion
        #region Monoliths
        private static bool ShowMonolithsSection = false;
        public static float Monolith_h = 350f;

        public static void Btn_Monoliths_Click()
        {
            ShowItemDropSection = false;
            ShowAutoPickupSection = false;
            ShowSceneSection = false;
            ShowRemoveReqSection = false;
            ShowDungeonsSection = false;
            ShowMonolithsSection = !ShowMonolithsSection;
        }
        public static void Btn_Monolith_Stability_Click()
        {
            Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_Stability = !Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_Stability;
        }
        public static void Btn_Monolith_Override_Max_Stability_Click()
        {
            Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_Overide_Max_Stability = !Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_Overide_Max_Stability;
        }
        public static void Btn_Monolith_Enemies_Defeat_OnStart_Click()
        {
            Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_EnnemiesDefeat_OnStart = !Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_EnnemiesDefeat_OnStart;
        }
        public static void Btn_Monolith_Objective_Reveal_OnStart_Click()
        {
            Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_ObjectiveReveal = !Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_ObjectiveReveal;
        }
        public static void Btn_Monolith_Complete_Objective_OnStart_Click()
        {
            Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_Complete_Objective = !Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_Complete_Objective;
        }
        public static void Btn_Monolith_NoDie_Click()
        {
            Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_NoDie = !Mods.Scene_Mods.Monoliths_mods.Enable_Monolith_NoDie;
        }
        #endregion
    }
}
