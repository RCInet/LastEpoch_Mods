using UniverseLib;

namespace LastEpochMods.Mods
{
    public class ItemsDrop
    {
        //DeathItemDrop
        public static int goldMultiplier = 99;
        public static int ItemMultiplier = 99;
        public static int Experience = 9999;
        public static void EditMonstersDeathDrop()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                System.Type type = obj.GetActualType();
                if (type == typeof(DeathItemDrop))
                {
                    DeathItemDrop drop = obj.TryCast<DeathItemDrop>();
                    drop.overrideBaseGoldDropChance = true;
                    drop.goldMultiplier = goldMultiplier;
                    drop.overrideBaseItemDropChance = true;
                    drop.itemMultiplier = ItemMultiplier;
                    drop.itemDropChance = 1; //100%
                    drop.experience = Experience;
                }
            }
        }
        public static bool OnlyUndropablesBasic = false;
        public static void UnlockForAllBasic(Main main)
        {
            UnityEngine.Object obj = Functions.GetObject("MasterItemsList");
            System.Type type = obj.GetActualType();
            if (type == typeof(ItemList))
            {
                ItemList item_list = obj.TryCast<ItemList>();
                for (int i = 0; i < 34; i++)
                {
                    if ((i != 11) && (i != 24))
                    {
                        Il2CppSystem.Collections.Generic.List<ItemList.EquipmentItem> items = item_list.GetEquipmentSubItems(i);
                        foreach (var item in items)
                        {
                            if (item.cannotDrop)
                            {
                                main.LoggerInstance.Msg("Basic : " + item.name + " : Can Drop Now");
                                item.cannotDrop = false;
                            }
                            else if (OnlyUndropablesBasic)
                            {
                                main.LoggerInstance.Msg("Basic : " + item.name + " : Can't Drop");
                                item.cannotDrop = true;
                            }
                        }
                    }
                }
            }
        }
        public static bool OnlyUndropablesUnique = false;
        public static void UnlockForAllUniques(Main main)
        {
            UnityEngine.Object obj = Functions.GetObject("UniqueList");
            System.Type type = obj.GetActualType();
            if (type == typeof(UniqueList))
            {
                UniqueList unique_list = obj.TryCast<UniqueList>();
                Il2CppSystem.Collections.Generic.List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
                foreach (UniqueList.Entry ul_entry in Uniques_List_Entry)
                {
                    if (!ul_entry.canDropRandomly)
                    {
                        main.LoggerInstance.Msg("Unique : " + ul_entry.name + " : Can Drop Now");
                        ul_entry.canDropRandomly = true;
                    }
                    else if (OnlyUndropablesUnique)
                    {
                        main.LoggerInstance.Msg("Unique : " + ul_entry.name + " : Can't Drop");
                        ul_entry.canDropRandomly = false;
                    }
                }
            }
        }
        public static void SetLevelReqForLegendaryPotencial(int level_req)
        {
            UnityEngine.Object obj = Functions.GetObject("UniqueList");
            System.Type type = obj.GetActualType();
            if (type == typeof(UniqueList))
            {
                UniqueList unique_list = obj.TryCast<UniqueList>();
                Il2CppSystem.Collections.Generic.List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
                foreach (UniqueList.Entry ul_entry in Uniques_List_Entry)
                {
                    if (ul_entry.effectiveLevelForLegendaryPotential != (byte)level_req)
                    {
                        ul_entry.effectiveLevelForLegendaryPotential = (byte)level_req;
                    }                    
                }
            }
        }
    }
}
