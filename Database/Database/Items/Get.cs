namespace Database.Items
{
    public class Get
    {
        public struct Type_Structure
        {
            public int Id;
            public string Name;
            public string Path;
        }
        private static string ItemsPath = @"\Database\Items\";
        private static string ArmorsPath = ItemsPath + @"Armors\";
        private static string WeaponsPath = ItemsPath + @"Weapons\";
        private static string AccesoriesPath = ItemsPath + @"Accesories\";
        private static string IdolsPath = ItemsPath + @"Idols\";

        public static Type_Structure[] TypesArray = new Type_Structure[]
        {
            new Type_Structure { Id = 0, Name = "Helm", Path = ArmorsPath + @"Helms\" },
            new Type_Structure { Id = 1, Name = "Body", Path = ArmorsPath + @"Bodys\" },
            new Type_Structure { Id = 2, Name = "Belt", Path = ArmorsPath + @"Belts\" },
            new Type_Structure { Id = 3, Name = "Boot", Path = ArmorsPath + @"Boots\" },
            new Type_Structure { Id = 4, Name = "Glove", Path = ArmorsPath + @"Gloves\" },
            new Type_Structure { Id = 5, Name = "Axe", Path = WeaponsPath + @"Axes\1H\" },
            new Type_Structure { Id = 6, Name = "Dagger", Path = WeaponsPath + @"Daggers\" },
            new Type_Structure { Id = 7, Name = "Blunt", Path = WeaponsPath + @"Blunts\1H\" },
            new Type_Structure { Id = 8, Name = "Scepter", Path = WeaponsPath + @"Scepters\" },
            new Type_Structure { Id = 9, Name = "Sword", Path = WeaponsPath + @"Swords\1H\" },
            new Type_Structure { Id = 10, Name = "Wand", Path = WeaponsPath + @"Wands\" },
            new Type_Structure { Id = 11, Name = "Fist", Path = WeaponsPath + @"Fist\" },
            new Type_Structure { Id = 12, Name = "Two-handed Axe", Path = WeaponsPath + @"Axes\2H\" },
            new Type_Structure { Id = 13, Name = "Two-handed Blunt", Path = WeaponsPath + @"Blunts\2H\" },
            new Type_Structure { Id = 14, Name = "Polearm", Path = WeaponsPath + @"Polearms\" },
            new Type_Structure { Id = 15, Name = "Staff", Path = WeaponsPath + @"Staffs\" },
            new Type_Structure { Id = 16, Name = "Two-handed Sword", Path = WeaponsPath + @"Swords\2H\" },
            new Type_Structure { Id = 17, Name = "Quiver", Path = WeaponsPath + @"Quivers\" },
            new Type_Structure { Id = 18, Name = "Shield", Path = ArmorsPath + @"Shields\" },
            new Type_Structure { Id = 19, Name = "Catalyst", Path = AccesoriesPath + @"Catalysts\" },
            new Type_Structure { Id = 20, Name = "Amulet", Path = AccesoriesPath + @"Amulets\" },
            new Type_Structure { Id = 21, Name = "Ring", Path = AccesoriesPath + @"Rings\" },
            new Type_Structure { Id = 22, Name = "Relic", Path = AccesoriesPath + @"Relics\" },
            new Type_Structure { Id = 23, Name = "Bow", Path = WeaponsPath + @"Bows\" },
            new Type_Structure { Id = 24, Name = "CrossBow", Path = WeaponsPath + @"CrossBows\" },
            new Type_Structure { Id = 25, Name = "Small", Path = IdolsPath + @"Smalls\" },
            new Type_Structure { Id = 26, Name = "Small Lagonian", Path = IdolsPath + @"Small_Lagonians\" },
            new Type_Structure { Id = 27, Name = "Humble Eterran", Path = IdolsPath + @"Humble_Eterrans\" },
            new Type_Structure { Id = 28, Name = "Stout", Path = IdolsPath + @"Stouts\" },
            new Type_Structure { Id = 29, Name = "Grand", Path = IdolsPath + @"Grands\" },
            new Type_Structure { Id = 30, Name = "Large", Path = IdolsPath + @"Larges\" },
            new Type_Structure { Id = 31, Name = "Ornate", Path = IdolsPath + @"Ornates\" },
            new Type_Structure { Id = 32, Name = "Huge", Path = IdolsPath + @"Huges\" },
            new Type_Structure { Id = 33, Name = "Adorned", Path = IdolsPath + @"Adorneds\" },
            new Type_Structure { Id = 34, Name = "Blessings", Path = Blessings.Get.Path },            
            new Type_Structure { Id = 101, Name = "Shards", Path = Affix.Get.Path },
            new Type_Structure { Id = 102, Name = "Runes", Path = Materials.Runes.Get.Path },
            new Type_Structure { Id = 103, Name = "Glyphs", Path = Materials.Glyphs.Get.Path },        
            new Type_Structure { Id = 104, Name = "Key", Path = Items.Keys.Get.Path }
        };
    }
}
