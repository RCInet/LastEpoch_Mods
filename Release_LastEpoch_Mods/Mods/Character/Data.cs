namespace LastEpochMods.Mods.Character
{
    public class Data
    {
        public static int CharacterClass = 0;
        public static int Deaths = 0;
        public static bool Died = false;
        public static bool Hardcore = false;
        public static float LanternLuminance = 0f;
        public static bool Masochist = false;
        public static int MonolithDepth = 0;
        public static bool PortalUnlocked = false;
        public static bool SoloChallenge = false;
        public static bool SoloCharacterChallenge = false;
        public static int SoulEmbers = 0;

        public static void Load()
        {
            try
            {
                LE.Data.CharacterData character_data = PlayerFinder.getPlayerData();
                CharacterClass = character_data.CharacterClass;
                Deaths = character_data.Deaths;
                Died = character_data.Died;
                Hardcore = character_data.Hardcore;
                LanternLuminance = character_data.LanternLuminance;
                Masochist = character_data.Masochist;
                MonolithDepth = character_data.MonolithDepth;
                PortalUnlocked = character_data.PortalUnlocked;
                SoloChallenge = character_data.SoloChallenge;
                SoloCharacterChallenge = character_data.SoloCharacterChallenge;
                SoulEmbers = character_data.SoulEmbers;
                Factions.Load();
            }
            catch { }
        }
        public static void Save()
        {
            try
            {
                LE.Data.CharacterData character_data = PlayerFinder.getPlayerData();
                character_data.CharacterClass = CharacterClass;
                character_data.Deaths = Deaths;
                character_data.Died = Died;
                character_data.Hardcore = Hardcore;
                character_data.LanternLuminance = LanternLuminance;
                character_data.Masochist = Masochist;
                character_data.MonolithDepth = MonolithDepth;
                character_data.PortalUnlocked = PortalUnlocked;
                character_data.SoloChallenge = SoloChallenge;
                character_data.SoloCharacterChallenge = SoloCharacterChallenge;
                character_data.SoulEmbers = SoulEmbers;
                Factions.Save();

                character_data.SaveData();
            }
            catch { }
        }
        public static string GetClasseName(int classe)
        {
            string result = "";
            try
            {
                if (classe < CharacterClassList.instance.classes.Count)
                {
                    result = CharacterClassList.instance.classes[classe].className;
                }
            }
            catch { }

            return result;
        }
        public static int GetClasseCount()
        {
            int result = 0;
            try
            {
                result = CharacterClassList.instance.classes.Count;
            }
            catch { }

            return result;
        }

        public class Factions
        {
            public static int Favor = 0;
            public static int Id = 0;
            public static bool HasEverJoined = false;
            public static bool IsMember = false;
            public static int Rank = 0;
            public static int Reputation = 0;

            public static void Load()
            {
                try
                {
                    LE.Data.CharacterData character_data = PlayerFinder.getPlayerData();
                    var char_faction = character_data.Factions[0];
                    Favor = char_faction.Favor;
                    HasEverJoined = char_faction.HasEverJoined;
                    Id = char_faction.ID;
                    IsMember = char_faction.IsMember;
                    Rank = char_faction.Rank;
                    Reputation = char_faction.Reputation;
                }
                catch { }
            }
            public static void Save()
            {
                try
                {
                    LE.Data.CharacterData character_data = PlayerFinder.getPlayerData();
                    var char_faction = character_data.Factions[0];
                    char_faction.Favor = Favor;
                    char_faction.HasEverJoined = HasEverJoined;
                    char_faction.ID = Id;
                    char_faction.IsMember = IsMember;
                    char_faction.Rank = Rank;
                    char_faction.Reputation = Reputation;
                }
                catch { }
            }
        }
        
    }
}
