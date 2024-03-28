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
        public static void OnSceneWasInitialized()
        {
            if (Scenes_Manager.GameScene())
            {
                Mods.Items.Skins.Config.Init();
            }
        }
        public static void QuickUpdate()
        {
            //Debug Prefab //Instantiate append after loading a character
            if ((UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F5)) && (!Assets_Manager.PauseMenu.Hud.IsNullOrDestroyed()))
            {
                Assets_Manager.PauseMenu.Hud.active = !Assets_Manager.PauseMenu.Hud.active;
            }
            //


            if (!Mods.Items.HeadHunter.Initialized) { Mods.Items.HeadHunter.Init(); }
            if (Scenes_Manager.GameScene())
            {
                Mods.Character.Cheats.AutoPot.Update();
                if (UnityEngine.Input.GetKeyDown(Save_Manager.Data.UserData.KeyBinds.HeadhunterBuffs))
                {
                    Save_Manager.Data.UserData.Items.Headhunter.showui = !Save_Manager.Data.UserData.Items.Headhunter.showui;
                }
                if (UnityEngine.Input.GetKeyDown(Save_Manager.Data.UserData.KeyBinds.BankStashs))
                {
                    Mods.Items.Bank.OpenClose();
                }
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Mouse0))
                {
                    Mods.Character.Cheats.Blessings.Select.Update();
                }
                
                /*if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F5))
                    {
                        Mods.Scenes.Monoliths.RevealIslands();
                    }*/
                /*if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F6))
                {
                    Mods.Scenes.Monoliths.ConnectIslands();
                }*/
            }
        }
        public static void SlowUpdate()
        {            
            if (Scenes_Manager.GameScene())
            {
                Mods.Items.Skins.Update();
                Mods.Items.AutoPickup.AutoStoreMaterialsTimer.Update();
                Mods.Character.PermanentBuffs.Update();                
                Mods.Character.Cheats.GodMode.Update();                
            }
        }
    }
}
