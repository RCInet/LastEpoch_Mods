using HarmonyLib;
using ItemFiltering;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class AutoPickup
    {
        //AutoStore Materials all 10 sec
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

        //AutoStore Materials on Inventory Open
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

        //AutoPickup Materials, Keys and Items without Drop //AutoStore Materials //AutoSell
        [HarmonyPatch(typeof(GroundItemManager), "dropItemForPlayer")]
        public class dropItemForPlayer
        {
            [HarmonyPrefix]
            static bool Prefix(ref GroundItemManager __instance, ref Actor __0, ref ItemData __1, ref UnityEngine.Vector3 __2, bool __3)
            {
                bool result = true;
                if ((Item.isKey(__1.itemType)) || (ItemList.isCraftingItem(__1.itemType)))
                {
                    if (((Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Key) && (Item.isKey(__1.itemType))) ||
                        ((Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Materials) && (ItemList.isCraftingItem(__1.itemType))))
                    {
                        __2 = __0.position();
                        bool pickup = ItemContainersManager.instance.attemptToPickupItem(__1, __2);
                        if (pickup)
                        {
                            if ((Save_Manager.Data.UserData.Items.AutoPickup.AutoStore_Materials) && (ItemList.isCraftingItem(__1.itemType)))
                            {
                                InventoryPanelUI.instance.StoreMaterialsButtonPress();
                            }
                            result = false;
                        }
                    }
                }
                else if ((__1.itemType < 24) &&
                    ((Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Filter) ||
                    (Save_Manager.Data.UserData.Items.Pickup.RemoveItemNotInFilter)))
                {
                    ItemFilter filter = null;
                    try { filter = ItemFilterManager.Instance.Filter; }
                    catch { Main.logger_instance.Error("Error trying to get user Filter"); }
                    if (!filter.IsNullOrDestroyed())
                    {
                        bool FilterShow = false;
                        bool FilterHide = false;
                        foreach (Rule rule in filter.rules)
                        {
                            if ((rule.isEnabled) && (rule.Match(__1.TryCast<ItemDataUnpacked>())) &&
                                (((rule.levelDependent) && (rule.LevelInBounds(__0.stats.level))) ||
                                (!rule.levelDependent)))
                            {
                                if (rule.type == Rule.RuleOutcome.SHOW) { FilterShow = true; }
                                else if (rule.type == Rule.RuleOutcome.HIDE)
                                {
                                    FilterShow = false;
                                    FilterHide = true;
                                    break;
                                }
                            }
                        }
                        if ((FilterShow) && (Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Filter))
                        {
                            __2 = __0.position();
                            bool pickup = ItemContainersManager.instance.attemptToPickupItem(__1, __2); //Pickup
                            if (pickup) { result = false; }

                        }
                        else if ((FilterHide) && (Save_Manager.Data.UserData.Items.Pickup.RemoveItemNotInFilter))
                        {
                            var price = __1.TryCast<ItemDataUnpacked>().VendorSaleValue;
                            __0.goldTracker.modifyGold(price);
                            result = false;
                        }
                    }
                }

                return result;
            }            
        }

        //AutoPickup Gold without Drop
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
        }

        //AutoPickup Xp Tome (with drop)
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

        //Allow Drop if Player already have Max Pots
        [HarmonyPatch(typeof(GeneratePotions), "TryToDropPotion")]
        public class GeneratePotions_TryToDropPotion
        {
            [HarmonyPrefix]
            static void Prefix(ref UnityEngine.Vector3 __0, ref int __1)
            {
                __1 = 255;
            }
        }

        //AutoPickup Potions (with drop)
        [HarmonyPatch(typeof(GroundItemManager), "dropPotionForPlayer")]
        public class dropPotionForPlayer
        {
            [HarmonyPrefix]
            static void Prefix(GroundItemManager __instance, Actor __0, ref UnityEngine.Vector3 __1, bool __2)
            {
                if (Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Pots)
                {
                    __1 = PlayerFinder.getPlayerActor().position(); //Move to Player before drop
                    //Find a way to pickup here and don't drop
                }
            }
            [HarmonyPostfix]
            static void Postfix(GroundItemManager __instance, Actor __0, ref UnityEngine.Vector3 __1, bool __2)
            {
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
    }
}
