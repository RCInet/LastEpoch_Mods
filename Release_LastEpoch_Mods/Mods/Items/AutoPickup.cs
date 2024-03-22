using HarmonyLib;
using ItemFiltering;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class AutoPickup
    {
        public class AutoStoreMaterialsTimer
        {
            public static void Update()
            {
                if (Save_Manager.Data.UserData.Items.AutoPickup.AutoStore_Materials_WithTimer)
                {
                    Save_Manager.Data.UserData.Items.AutoPickup.AutoStore_Materials = false;
                    Save_Manager.Data.UserData.Items.AutoPickup.AutoStore_Materials_WhenOpeningInventory = false;
                    if (!running) { Start(); }
                    if (running)
                    {
                        if (GetElapsedTime() > 10)
                        {
                            InventoryPanelUI.instance.StoreMaterialsButtonPress();
                            Start();
                        }
                    }
                }
                else { running = false; }
            }
            private static System.DateTime StartTime;
            private static System.DateTime LastTime;
            private static bool running;

            private static void Start()
            {
                StartTime = System.DateTime.Now;
                running = true;
            }
            private static double GetElapsedTime()
            {
                LastTime = System.DateTime.Now;
                var elaspedTime = LastTime - StartTime;

                return elaspedTime.TotalSeconds;
            }
        }

        [HarmonyPatch(typeof(InventoryPanelUI), "OnEnable")]
        public class InventoryPanelUI_OnEnable
        {
            [HarmonyPostfix]
            static void Postfix(ref InventoryPanelUI __instance)
            {
                try
                {
                    if (Save_Manager.Data.UserData.Items.AutoPickup.AutoStore_Materials_WhenOpeningInventory)
                    {
                        __instance.StoreMaterialsButtonPress();
                    }                        
                }
                catch { Main.logger_instance.Error("InventoryPanelUI:OnEnable"); }
            }
        }        

        [HarmonyPatch(typeof(GroundItemManager), "dropItemForPlayer")]
        public class dropItemForPlayer
        {
            [HarmonyPostfix]
            static void Postfix(ref GroundItemManager __instance, ref Actor __0, ref ItemData __1, ref UnityEngine.Vector3 __2, bool __3)
            {
                try
                {
                    if (!__instance.IsNullOrDestroyed())
                    {
                        bool pickup_with_filter = false;
                        System.UInt32 item_id = __instance.nextItemId - 1;
                        if (Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Filter)
                        {
                            if (item_id == item_filter_id) { pickup_with_filter = true; }
                        }
                        if (pickup_with_filter)
                        {
                            __2 = PlayerFinder.getPlayerActor().position();
                            __instance.pickupItem(__0, item_id);
                        }
                        else
                        {
                            if ((!__0.IsNullOrDestroyed()) && (!__1.IsNullOrDestroyed()))
                            {
                                if (((Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Key) && (Item.isKey(__1.itemType))) ||
                                        ((Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Materials) && (ItemList.isCraftingItem(__1.itemType))) ||
                                        ((Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_UniqueAndSet) && (Item.rarityIsUniqueSetOrLegendary(__1.rarity))))
                                {
                                    //__2 = PlayerFinder.getPlayerActor().position();
                                    __instance.pickupItem(__0, item_id);
                                    if ((Save_Manager.Data.UserData.Items.AutoPickup.AutoStore_Materials) &&
                                        (ItemList.isCraftingItem(__1.itemType)) &&
                                        (!InventoryPanelUI.instance.IsNullOrDestroyed()))
                                    { InventoryPanelUI.instance.StoreMaterialsButtonPress(); }
                                }
                            }
                        }
                    }
                }
                catch { Main.logger_instance.Error("GroundItemManager:dropItemForPlayer"); }
            }
        }

        [HarmonyPatch(typeof(GroundItemManager), "dropGoldForPlayer")]
        public class dropGoldForPlayer
        {
            [HarmonyPrefix]
            static bool Prefix(ref GroundItemManager __instance, ref Actor __0, ref int __1, ref UnityEngine.Vector3 __2, ref bool __3)
            {
                if (Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Gold)
                {
                    try
                    {
                        PlayerFinder.getLocalGoldTracker().modifyGold(__1);
                        return false;
                    }
                    catch { return true; }
                }
                else { return true; }
            }
            /*[HarmonyPostfix]
            static void Postfix(ref GroundItemManager __instance, ref Actor __0, ref int __1, ref UnityEngine.Vector3 __2, ref bool __3)
            {
                if ((!__instance.IsNullOrDestroyed()) && (Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Gold))
                {
                    Main.logger_instance.Msg("GroundItemManager:dropGoldForPlayer");
                    System.UInt32 gold_id = __instance.nextGoldId - 1;
                    foreach (GoldPickupInteraction gold_pickup_interaction in __instance.activeGoldPiles)
                    {
                        if (gold_pickup_interaction.id == gold_id)
                        {                            
                            __instance.pickupGold(__0, gold_id, gold_pickup_interaction);
                            Main.logger_instance.Msg("Gold Pickup");
                            break;
                        }
                    }
                }
            }*/
        }

        [HarmonyPatch(typeof(GroundItemManager), "dropXPTomeForPlayer")]
        public class dropXPTomeForPlayer
        {
            [HarmonyPostfix]
            static void Postfix(ref GroundItemManager __instance, ref Actor __0, ref int __1, ref UnityEngine.Vector3 __2, ref bool __3)
            {
                if ((!__instance.IsNullOrDestroyed()) && (Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_XpTome))
                {
                    System.UInt32 tome_id = __instance.nextXpTomeId - 1;
                    foreach (PickupExperiencePotionInteraction pick_exp_pot_interaction in __instance.activeXPTomes)
                    {
                        if (pick_exp_pot_interaction.id == tome_id)
                        {
                            __2 = PlayerFinder.getPlayerActor().position();
                            __instance.pickupXPTome(__0, tome_id, pick_exp_pot_interaction);
                            break;
                        }
                    }
                }
            }
        }

        //Potions
        [HarmonyPatch(typeof(GeneratePotions), "TryToDropPotion")]
        public class GeneratePotions_TryToDropPotion
        {
            [HarmonyPrefix]
            static void Prefix(ref UnityEngine.Vector3 __0, ref int __1)
            {
                //__1 = 255; //Allow Drop if Player already have Max Pots
                try
                {
                    if (Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Pots)
                    {
                        __0 = PlayerFinder.getPlayerActor().position();
                    }
                }                        
                catch { }
            }
        }

        [HarmonyPatch(typeof(GroundItemManager), "dropPotionForPlayer")]
        public class dropPotionForPlayer
        {
            [HarmonyPostfix]
            static void Postfix(GroundItemManager __instance, Actor __0, ref UnityEngine.Vector3 __1, bool __2)
            {
                Main.logger_instance.Msg("dropPotionForPlayer");
                try
                {
                    if ((!__instance.IsNullOrDestroyed()) && (Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Pots))
                    {
                        System.UInt32 pot_id = __instance.nextPotionId - 1;
                        foreach (PotionPickupInteraction pick_pot_interaction in __instance.activePotions)
                        {
                            if (pick_pot_interaction.id == pot_id)
                            {
                                __instance.pickupPotion(__0, pot_id, pick_pot_interaction);
                                break;
                            }
                        }
                    }
                }
                catch { Main.logger_instance.Error("GroundItemManager:dropPotionForPlayer PostFix"); }
            }
        }

        //From Filter
        public static ItemFilterManager item_filter_manager = null;
        public static Actor player_actor = null;
        public static uint default_item_filter_id = 9999;
        public static uint item_filter_id = default_item_filter_id;
        [HarmonyPatch(typeof(GroundItemVisuals), "initialise", new System.Type[] { typeof(ItemDataUnpacked), typeof(uint), typeof(GroundItemLabel), typeof(GroundItemRarityVisuals), typeof(bool) })]
        private static class GroundItemVisuals_initialise
        {
            [HarmonyPostfix]
            private static void Postfix(GroundItemVisuals __instance, ItemDataUnpacked __0, uint __1, GroundItemLabel __2, GroundItemRarityVisuals __3, bool __4)
            {
                item_filter_id = default_item_filter_id;
                if (Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Filter)
                {
                    ItemFilter filter = null;
                    try
                    {
                        if (item_filter_manager.IsNullOrDestroyed()) { item_filter_manager = ItemFilterManager.Instance; }
                        if (!item_filter_manager.IsNullOrDestroyed())
                        {
                            if (!item_filter_manager.Filter.IsNullOrDestroyed()) { filter = item_filter_manager.Filter; }
                        }
                    }
                    catch { Main.logger_instance.Error("Error trying to get user Filter"); }
                    try
                    {
                        if (player_actor.IsNullOrDestroyed()) { player_actor = PlayerFinder.getPlayerActor(); }                        
                    }
                    catch { Main.logger_instance.Error("Error trying to get Player Actor"); }
                    bool Show = false;
                    if ((!filter.IsNullOrDestroyed()) && (!__0.IsNullOrDestroyed()))
                    {
                        try
                        {
                            foreach (Rule rule in filter.rules)
                            {
                                if ((rule.isEnabled) && (rule.Match(__0)) && (((rule.levelDependent) && (rule.LevelInBounds(player_actor.stats.level))) || (!rule.levelDependent)))
                                {
                                    if (rule.type == Rule.RuleOutcome.SHOW) { Show = true; }
                                    else if (rule.type == Rule.RuleOutcome.HIDE) { Show = false; }
                                }
                            }
                        }
                        catch { Main.logger_instance.Error("Error check rules"); }
                    }
                    if (Show) //&& (!player_actor.IsNullOrDestroyed()))
                    {
                        try { item_filter_id = __1; }
                        catch { Main.logger_instance.Error("Error pickup item : " + __1); }
                    }
                }
            }
        }
    }
}
