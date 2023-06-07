using UniverseLib;

namespace LastEpochMods.Mods
{
    public class UniqueMods
    {
        public struct unique_mod
        {
            public int id;
            public Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods;
        }
        public static System.Collections.Generic.List<unique_mod> Uniques_Mods = new System.Collections.Generic.List<unique_mod>();
        public static bool Enable_UniqueMods = false;
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
            //Create new Array of UniqueItemMod
            Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods = new Il2CppSystem.Collections.Generic.List<UniqueItemMod>();
            //Add UniqueItemMod to Array
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.AttackSpeed,
                tags = AT.None
            });
            //Add More

            return mods;
        }
    }
}
