using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    public class Passives_Points
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) && (!Refs_Manager.player_treedata.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Skills.Enable_PassivePoints;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(PassivePanelManager), "onTreeOpened")]
        public class PassivePanelManager_onTreeOpened
        {
            [HarmonyPrefix]
            static void Prefix(PassivePanelManager __instance, CharacterClass __0, byte __1)
            {
                if (CanRun())
                {
                    Refs_Manager.player_treedata.passiveTree.pointsEarnt = (ushort)Save_Manager.instance.data.Skills.PassivePoints;
                }
            }
        }
    }
}
