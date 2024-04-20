using HarmonyLib;
using ItemFiltering;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    //Have to edit AutoSell (making 2 types)
    //One with AutoSell all items matching any "Hide" rules
    //One with AutoSell all items matching any "Hide" rules and don't matching any "Show" rules

    public class Items_AutoPickup_Items
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    if ((Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Keys) ||
                        (Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Materials) ||
                        (Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_FromFilter))
                    {
                        return true;
                    }
                    else { return false; }
                }
                else { return false; }
            }
            else { return false; }
        }        
        [HarmonyPatch(typeof(GroundItemManager), "dropItemForPlayer")]
        public class dropItemForPlayer
        {
            [HarmonyPrefix]
            static bool Prefix(ref GroundItemManager __instance, ref Actor __0, ref ItemData __1, ref UnityEngine.Vector3 __2, bool __3)
            {
                bool result = true;
                if ((Item.isKey(__1.itemType)) || (ItemList.isCraftingItem(__1.itemType)))
                {
                    if (((Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Keys) && (Item.isKey(__1.itemType))) ||
                        ((Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Materials) && (ItemList.isCraftingItem(__1.itemType))))
                    {
                        bool pickup = ItemContainersManager.instance.attemptToPickupItem(__1, __0.position()); // __2);
                        if (pickup)
                        {
                            if ((Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_OnDrop) && (ItemList.isCraftingItem(__1.itemType)))
                            {
                                InventoryPanelUI.instance.StoreMaterialsButtonPress();
                            }
                            result = false;
                        }
                    }
                }
                else if ((__1.itemType < 24) && (!Refs_Manager.filter_manager.IsNullOrDestroyed()) &&
                    ((Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_FromFilter) ||
                    (Save_Manager.instance.data.Items.Pickup.Enable_AutoSell_Hide) ||
                    (Save_Manager.instance.data.Items.Pickup.Enable_AutoSell_Both)))
                {
                    if (!Refs_Manager.filter_manager.Filter.IsNullOrDestroyed())
                    {
                        bool FilterShow = false;
                        bool FilterHide = false;
                        bool DontSell = false;
                        foreach (Rule rule in Refs_Manager.filter_manager.Filter.rules)
                        {
                            if ((rule.isEnabled) && (rule.Match(__1.TryCast<ItemDataUnpacked>())) &&
                                (((rule.levelDependent) && (rule.LevelInBounds(__0.stats.level))) ||
                                (!rule.levelDependent)))
                            {
                                if (rule.type == Rule.RuleOutcome.SHOW) { FilterShow = true; DontSell = true; }
                                else if (rule.type == Rule.RuleOutcome.HIDE)
                                {
                                    FilterShow = false;
                                    FilterHide = true;
                                    break;
                                }
                            }
                        }
                        if ((FilterShow) && (Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_FromFilter))
                        {
                            bool pickup = ItemContainersManager.instance.attemptToPickupItem(__1, __0.position()); //__2); //Pickup
                            if (pickup) { result = false; }

                        }
                        else if ((FilterHide) && (!DontSell) && (Save_Manager.instance.data.Items.Pickup.Enable_AutoSell_Hide))
                        {
                            __0.goldTracker.modifyGold(__1.TryCast<ItemDataUnpacked>().VendorSaleValue);
                            result = false;
                        }
                        else if ((FilterHide) && (Save_Manager.instance.data.Items.Pickup.Enable_AutoSell_Both))
                        {
                            __0.goldTracker.modifyGold(__1.TryCast<ItemDataUnpacked>().VendorSaleValue);
                            result = false;
                        }
                    }
                }

                return result;
            }
        }
    }
}
