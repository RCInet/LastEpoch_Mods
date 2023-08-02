using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Inventory_Item_UI
    {
        [HarmonyPatch(typeof(InventoryItemUI), "GetSpriteImage")]
        public class GetSpriteImage
        {
            [HarmonyPostfix]
            static void Postfix(ref UnityEngine.Sprite __result, ItemData __0, ItemUIContext __1)
            {
                if (__0.TryCast<ItemDataUnpacked>().FullName == Mods.Items.HeadHunter.unique_name)
                {
                    __result = Mods.Items.HeadHunter.UniqueSprite();
                }
            }
        }
    }
}
