using UniverseLib;

namespace LastEpochMods.Mods
{
    public class UniqueMods
    {
        //Exemple Custom Mods
        public static Il2CppSystem.Collections.Generic.List<UniqueItemMod> CustomMods_0()
        {
            Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.AttackSpeed,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.CriticalChance,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.CriticalMultiplier,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.Damage,
                tags = AT.Physical
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Strength,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Intelligence,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Dexterity,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.IncreasedDropRate,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.IncreasedExperience,
                tags = AT.None
            });

            return mods;
        }
        public static Il2CppSystem.Collections.Generic.List<UniqueItemMod> CustomMods_1()
        {
            Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.AttackSpeed,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.CriticalChance,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.CriticalMultiplier,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.Damage,
                tags = AT.Physical
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Strength,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Intelligence,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Dexterity,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.IncreasedDropRate,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.IncreasedExperience,
                tags = AT.None
            });

            return mods;
        }

        //Functions
        public static void EditUniqueMods(int unique_id, Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods, Main main)
        {
            UnityEngine.Object obj = Functions.GetObject("UniqueList");
            System.Type type = obj.GetActualType();
            if (type == typeof(UniqueList))
            {
                UniqueList unique_list = obj.TryCast<UniqueList>();
                Il2CppSystem.Collections.Generic.List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
                foreach (UniqueList.Entry ul_entry in Uniques_List_Entry)
                {
                    if (ul_entry.uniqueID == unique_id)
                    {
                        ul_entry.mods = mods;
                        main.LoggerInstance.Msg(ul_entry.name + ": Unique Mods Edited");
                        break;
                    }
                }
            }
        }
        public static void AddUniqueModToAnother(int item_id, int item2_id)
        {
            UnityEngine.Object obj = Functions.GetObject("UniqueList");
            System.Type type = obj.GetActualType();
            if (type == typeof(UniqueList))
            {
                UniqueList unique_list = obj.TryCast<UniqueList>();
                Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();
                //Copy
                int index = -1;
                int i = 0;
                foreach (UniqueList.Entry ul_entry in unique_list.uniques)
                {
                    if ((ul_entry.baseType == item_id) | (ul_entry.baseType == item2_id))
                    {
                        if (ul_entry.baseType == item2_id) { index = i; }
                        foreach (UniqueItemMod m in ul_entry.mods) { mods.Add(m); }
                    }
                    i++;
                }
                //Paste
                if (index > -1) { unique_list.uniques[index].mods = mods; }

                foreach (UniqueList.Entry ul_entry in unique_list.uniques)
                {
                    if (ul_entry.baseType == item2_id) { ul_entry.mods = mods; }
                }
            }
        }
    }
}
