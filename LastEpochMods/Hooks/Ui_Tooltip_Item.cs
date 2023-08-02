using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Ui_Tooltip_Item
    {
        [HarmonyPatch(typeof(UITooltipItem), "SetItemSprite")]
        public class SetItemSprite
        {
            [HarmonyPostfix]
            static void Postfix(ref UnityEngine.Sprite __result, ItemDataUnpacked __0)
            {
                if (__0.FullName == Mods.Items.HeadHunter.unique_name)
                {
                    __result = Mods.Items.HeadHunter.UniqueSprite();
                }
            }
        }
    }
}
