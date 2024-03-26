namespace LastEpochMods.Managers
{
    public class Mods_Managers
    {
        public static void OnSceneWasLoaded(string sceneName)
        {
            Mods.Items.Skins.Reset();
            if (Scenes_Manager.GameScene())
            {
                Mods.Items.HeadHunter.Unique.Update(); //Add Headhunter to UniqueList
                Mods.ForceDrop.ForceDrop.type.InitList(); //Init Forcedrop items list
                Mods.ForceDrop.ForceDrop.affixs.InitList(); //Init Forcedrop affixs list
                Mods.Items.RemoveReq.Update();
                Mods.Character.Data.Load(); //Load CharacterData for Menu
                Mods.Scenes.Camera.LoadOnStart();
                Mods.SkillsTree.Options.Ability_Mutator.Init(); //Init Ability Mutators list
                Mods.Items.DropNotifications.OnSceneWasLoaded(); //Update Notifications
                Mods.Character.PermanentBuffs.OnSceneWasLoaded();
            }
        }
        public static void OnSceneWasInitialized(string sceneName)
        {
            if (Scenes_Manager.GameScene())
            {
                Mods.Items.Skins.CosmeticPanel.Slots.InitSlots(); //Textures
                Mods.Items.Skins.Config.Init();                
            }
        }
        public static void Update()
        {
            if (!Mods.Items.HeadHunter.Initialized) { Mods.Items.HeadHunter.Init(); }
            if (Scenes_Manager.GameScene())
            {
                Mods.Items.Skins.Update();
                Mods.Items.AutoPickup.AutoStoreMaterialsTimer.Update();
                Mods.Character.PermanentBuffs.Update();
                Mods.Character.Cheats.Blessings.Select.Update();
                Mods.Character.Cheats.GodMode.Update();
                Mods.Character.Cheats.AutoPot.Update();
            }
        }
    }
}
