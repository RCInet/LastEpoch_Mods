using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LastEpochMods.Db
{
    public class Get
    {
        public class Type
        {
            public struct Type_Structure
            {
                public int Id;
                public string Name;
                public string Path;
            }
            public static Type_Structure[] TypesArray = new Type_Structure[]
            {
                new Type_Structure { Id = 0, Name = "Helm", Path = @"\Database\Items\Armors\Helms\" },
                new Type_Structure { Id = 1, Name = "Body", Path =  @"\Database\Items\Armors\Bodys\" },
                new Type_Structure { Id = 2, Name = "Belt", Path =  @"\Database\Items\Armors\Belts\" },
            new Type_Structure { Id = 3, Name = "Boot", Path = @"\Database\Items\Armors\Boots\" },
            new Type_Structure { Id = 4, Name = "Glove", Path = @"\Database\Items\Armors\Gloves\" },
            new Type_Structure { Id = 5, Name = "Axe", Path = @"\Database\Items\Weapons\Axes\1H\" },
            new Type_Structure { Id = 6, Name = "Dagger", Path = @"\Database\Items\Weapons\Daggers\" },
            new Type_Structure { Id = 7, Name = "Blunt", Path = @"\Database\Items\Weapons\Blunts\1H\" },
            new Type_Structure { Id = 8, Name = "Scepter", Path = @"\Database\Items\Weapons\Scepters\" },
            new Type_Structure { Id = 9, Name = "Sword", Path = @"\Database\Items\Weapons\Swords\1H\" },
            new Type_Structure { Id = 10, Name = "Wand", Path = @"\Database\Items\Weapons\Wands\" },
            new Type_Structure { Id = 11, Name = "Fist", Path = @"\Database\Items\Weapons\Fist\" },
            new Type_Structure { Id = 12, Name = "Two-handed Axe", Path = @"\Database\Items\Weapons\Axes\2H\" },
            new Type_Structure { Id = 13, Name = "Two-handed Blunt", Path = @"\Database\Items\Weapons\Blunts\2H\" },
            new Type_Structure { Id = 14, Name = "Polearm", Path = @"\Database\Items\Weapons\Polearms\" },
            new Type_Structure { Id = 15, Name = "Staff", Path = @"\Database\Items\Weapons\Staffs\" },
            new Type_Structure { Id = 16, Name = "Two-handed Sword", Path = @"\Database\Items\Weapons\words\2H\" },
            new Type_Structure { Id = 17, Name = "Quiver", Path = @"\Database\Items\Weapons\Quivers\" },
            new Type_Structure { Id = 18, Name = "Shield", Path = @"\Database\Items\Armors\Shields\" },
            new Type_Structure { Id = 19, Name = "Catalyst", Path = @"\Database\Items\Accesories\Catalysts\" },
            new Type_Structure { Id = 20, Name = "Amulet", Path = @"\Database\Items\Accesories\Amulets\" },
            new Type_Structure { Id = 21, Name = "Ring", Path = @"\Database\Items\Accesories\Rings\" },
            new Type_Structure { Id = 22, Name = "Relic", Path = @"\Database\Items\Accesories\Relics\" },
            new Type_Structure { Id = 23, Name = "Bow", Path = @"\Database\Items\Weapons\Bows\" },
            new Type_Structure { Id = 24, Name = "CrossBow", Path = @"\Database\Items\Weapons\CrossBows\" },
            new Type_Structure { Id = 25, Name = "Small", Path = @"\Database\Items\Idols\Smalls\" },
            new Type_Structure { Id = 26, Name = "Small Lagonian", Path = @"\Database\Items\dols\Small_Lagonians\" },
            new Type_Structure { Id = 27, Name = "Humble Eterran", Path = @"\Database\Items\Idols\Humble_Eterrans\" },
            new Type_Structure { Id = 28, Name = "Stout", Path = @"\Database\Items\Idols\Stouts\" },
            new Type_Structure { Id = 29, Name = "Grand", Path = @"\Database\Items\Idols\Grands\" },
            new Type_Structure { Id = 30, Name = "Large", Path = @"\Database\Items\Idols\Larges\" },
            new Type_Structure { Id = 31, Name = "Ornate", Path = @"\Database\Items\Idols\Ornates\" },
            new Type_Structure { Id = 32, Name = "Huge", Path = @"\Database\Items\Idols\Huges\" },
            new Type_Structure { Id = 33, Name = "Adorned", Path = @"\Database\Items\Idols\Adorneds\" },
            new Type_Structure { Id = 34, Name = "Blessings", Path = @"\Database\Blessings\" },
            new Type_Structure { Id = 101, Name = "Affixs", Path = @"\Database\Affixs\" },
            new Type_Structure { Id = 102, Name = "Runes", Path = @"\Database\Materials\Runes\" },
            new Type_Structure { Id = 103, Name = "Glyphs", Path = @"\Database\Materials\Glyphs\" },
            new Type_Structure { Id = 104, Name = "Key", Path = @"\Database\Items\Keys\" }
            };
        }
    }
}
