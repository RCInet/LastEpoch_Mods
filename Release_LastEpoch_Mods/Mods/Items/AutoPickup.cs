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
            [HarmonyPrefix]
            static bool Prefix(ref GroundItemManager __instance, ref Actor __0, ref ItemData __1, ref UnityEngine.Vector3 __2, bool __3)
            {
                bool result = true;
                item_filter_pickup = default_item_filter_id;
                System.UInt32 item_id = __instance.nextItemId - 1;
                if ((Item.isKey(__1.itemType)) || (ItemList.isCraftingItem(__1.itemType)))
                {
                    if (((Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Key) && (Item.isKey(__1.itemType))) ||
                        ((Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Materials) && (ItemList.isCraftingItem(__1.itemType))))
                    {
                        //Main.logger_instance.Msg("Move Material or Key before Pickup");
                        __2 = PlayerFinder.getPlayerActor().position();
                        item_filter_pickup = item_id;
                    }
                }
                else if (__1.itemType < 24)
                {
                    if (((Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Filter) ||
                    (Save_Manager.Data.UserData.Items.Pickup.RemoveItemNotInFilter)))
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
                        if ((!filter.IsNullOrDestroyed()) && (!__0.IsNullOrDestroyed()))
                        {
                            bool FilterShow = false;
                            bool FilterRemove = false;
                            foreach (Rule rule in filter.rules)
                            {
                                if ((rule.isEnabled) && (rule.Match(__1.TryCast<ItemDataUnpacked>())) && (((rule.levelDependent) && (rule.LevelInBounds(player_actor.stats.level))) || (!rule.levelDependent)))
                                {
                                    if (rule.type == Rule.RuleOutcome.SHOW) { FilterShow = true; }
                                    else if (rule.type == Rule.RuleOutcome.HIDE)
                                    {
                                        FilterShow = false;
                                        FilterRemove = true;
                                        break;
                                    }
                                }
                            }
                            if ((FilterShow) && (Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Filter))
                            {
                                //Main.logger_instance.Msg("Move " + item_id + " before Pickup");
                                __2 = PlayerFinder.getPlayerActor().position();
                                item_filter_pickup = item_id;
                            }
                            else if ((FilterRemove) && (Save_Manager.Data.UserData.Items.Pickup.RemoveItemNotInFilter))
                            {
                                var price = __1.TryCast<ItemDataUnpacked>().VendorSaleValue;
                                PlayerFinder.getPlayerActor().goldTracker.modifyGold(price);
                                //Main.logger_instance.Msg("Sell Item : " + price + " gold");
                                result = false;
                            }
                        }
                    }
                }

                return result;
            }
            [HarmonyPostfix]
            static void Postfix(ref GroundItemManager __instance, ref Actor __0, ref ItemData __1, ref UnityEngine.Vector3 __2, bool __3)
            {
                if (item_filter_pickup != default_item_filter_id)
                {
                    System.UInt32 item_id = __instance.nextItemId - 1;
                    //Main.logger_instance.Msg("Pickup Item");
                    __instance.pickupItem(__0, item_id);

                    if ((Save_Manager.Data.UserData.Items.AutoPickup.AutoStore_Materials) && (ItemList.isCraftingItem(__1.itemType)))
                    {
                        InventoryPanelUI.instance.StoreMaterialsButtonPress();
                    }
                }
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

        //Move Potion to Player before Drop
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
                //Main.logger_instance.Msg("dropPotionForPlayer");
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
        public static uint default_item_filter_id = uint.MaxValue;
        public static uint item_filter_pickup = default_item_filter_id;        
    }
}
