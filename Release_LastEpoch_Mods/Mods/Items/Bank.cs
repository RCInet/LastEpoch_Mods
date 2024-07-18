namespace LastEpochMods.Mods.Items
{
    public class Bank
    {
        public static void OpenClose()
        {
            if (!Managers.GUI_Manager.Base.Refs.Game_UIBase.IsNullOrDestroyed())
            {
                //UIBase.instance.stashPanel.instance.active = !Managers.GUI_Manager.Base.Refs.Game_UIBase.stashPanel.instance.active;
                Managers.GUI_Manager.Base.Refs.Game_UIBase.stashPanel.instance.active = !Managers.GUI_Manager.Base.Refs.Game_UIBase.stashPanel.instance.active;
            }
        }
    }
}
