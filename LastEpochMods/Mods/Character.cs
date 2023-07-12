using UnityEngine;
using UniverseLib;
using static ItemList;

namespace LastEpochMods.Mods
{
    public class Character
    {
        public static void Launch_LevelUp()
        {            
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(ExperienceTracker)))
            {
                if (obj.name == "MainPlayer(Clone)")
                {
                    obj.TryCast<ExperienceTracker>().LevelUp(true);
                    break;
                }
            }
        }
        public static void DropUniqueOrSet(string name)
        {
            Actor player = PlayerFinder.getPlayerActor();
            UniqueList.Entry unique_item = null;          
            foreach (UniqueList.Entry unique in UniqueList.get().uniques)
            {
                if (name == unique.name) { unique_item = unique; break; }
            }
            if (unique_item != null)
            {
                int rarity = 7;
                if (unique_item.isSetItem) { rarity = 8; }
                ItemDataUnpacked item = player.generateItems.initialiseRandomItemData(false, 100,
                    false, ItemLocationTag.None, unique_item.baseType, unique_item.oldSubTypeID,
                    rarity, 0, unique_item.uniqueID, false, 0);                
                
                bool rarity_was_checked = false;
                if (Config.Data.mods_config.items.Enable_Rarity)
                {
                    rarity_was_checked = true;
                    Config.Data.mods_config.items.Enable_Rarity = false;
                }
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(GroundItemManager)))
                {                    
                    obj.TryCast<GroundItemManager>().dropItemForPlayer(player, item.TryCast<ItemData>(), player.position(), false);
                    break;
                }
                if (rarity_was_checked) { Config.Data.mods_config.items.Enable_Rarity = true; }
            }
        }
        public static void DropAllAffix(int quantity)
        {
            Actor player = PlayerFinder.getPlayerActor();
            System.Collections.Generic.List<int> affixs_id = new System.Collections.Generic.List<int>();
            foreach (AffixList.SingleAffix affix in AffixList.instance.singleAffixes)
            {
                if (!affix.affixName.Contains("Idol")) { affixs_id.Add(affix.affixId); }
            }
            foreach (AffixList.MultiAffix affix in AffixList.instance.multiAffixes)
            {
                if (!affix.affixName.Contains("Idol")) { affixs_id.Add(affix.affixId); }
            }
            bool rarity_was_checked = false;
            if (Config.Data.mods_config.items.Enable_Rarity)
            {
                rarity_was_checked = true;
                Config.Data.mods_config.items.Enable_Rarity = false;
            }
            GroundItemManager ground_item_manager = null;
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(GroundItemManager)))
            {
                ground_item_manager = obj.TryCast<GroundItemManager>();
                break;
            }
            if (ground_item_manager != null)
            {
                foreach (int affix_id in affixs_id)
                {
                    ItemDataUnpacked item = player.generateItems.initialiseRandomItemData(false, 100, false, ItemLocationTag.None, 101, affix_id, 0, 0, 0, false, 0);
                    for (int i = 0; i < quantity; i++)
                    {
                        ground_item_manager.dropItemForPlayer(player, item.TryCast<ItemData>(), player.position(), false);
                    }
                }
            }
            if (rarity_was_checked) { Config.Data.mods_config.items.Enable_Rarity = true; }
        }
    }
}
