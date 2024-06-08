using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Mobs
{
    public class Mobs_Drop_Item_Multiplier
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_ItemDropMultiplier;
                }
                else { return false; }
            }
            else { return false; }
        }

        //Uncomment here to use a more generic function for item multiplier
        //(comment DeathItemDrop Start function if you use this)
        /*[HarmonyPatch(typeof(ItemDrop), "DropItem", new System.Type[] { typeof(int), typeof(UnityEngine.Vector3), typeof(float), typeof(bool), typeof(float), typeof(ItemDrop.BaseDropRates), typeof(bool), typeof(float), typeof(float), typeof(float), typeof(ItemDrop.DropFlags), typeof(UnityEngine.SceneManagement.Scene), typeof(bool) })]
        public class ItemDrop_DropItem
        {
            // __0 = int level
            // __1 = UnityEngine.Vector3 position
            // __2 = float itemDropChance
            // __3 = bool dropsAlwaysRequireClassCompatibility
            // __4 = float itemMultiplier
            // __5 = ItemDrop.BaseDropRates baseDropRates
            // __6 = bool guaranteedAdditionalRare
            // __7 = float goldMultiplier
            // __8 = float goldChance
            // __9 = float craftingOnlyDropChance
            // __10 = ItemDrop.DropFlags dropFlags
            // __11 = UnityEngine.SceneManagement.Scene scene
            // __12 = bool causedByEnemyDeath

            [HarmonyPrefix]
            static void Prefix(ref float __4, ItemDrop.BaseDropRates __5)
            {
                if ((CanRun()) && ((__5 == ItemDrop.BaseDropRates.Enemy) || (__5 == ItemDrop.BaseDropRates.Chest)))
                {
                    //__4 = 50f; // x50
                    __4 = Save_Manager.instance.data.Character.Cheats.ItemDropMultiplier;
                }
            }
        }*/

        [HarmonyPatch(typeof(DeathItemDrop), "Start")]
        public class DeathItemDrop_Start
        {
            [HarmonyPrefix]
            static void Prefix(ref DeathItemDrop __instance)
            {
                if (CanRun())
                {
                    __instance.itemMultiplier = Save_Manager.instance.data.Character.Cheats.ItemDropMultiplier;
                }
            }
        }
    }
}