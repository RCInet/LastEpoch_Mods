using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Death_Item_Drop
    {
        [HarmonyPatch(typeof(DeathItemDrop), "Start")]
        public class Start
        {
            [HarmonyPostfix]
            static void Postfix(ref DeathItemDrop __instance)
            {
                if (Config.Data.mods_config.items.Enable_DeathItemDrop_goldMultiplier)
                {
                    __instance.overrideBaseGoldDropChance = true;
                    __instance.goldDropChance = 1; //100%
                    __instance.goldMultiplier = Config.Data.mods_config.items.DeathItemDrop_goldMultiplier;
                }
                if (Config.Data.mods_config.items.Enable_DeathItemDrop_ItemMultiplier)
                {
                    __instance.overrideBaseItemDropChance = true;
                    __instance.itemDropChance = 1; //100%
                    __instance.itemMultiplier = Config.Data.mods_config.items.DeathItemDrop_ItemMultiplier;
                }
                if (Config.Data.mods_config.items.Enable_DeathItemDrop_Experience)
                {
                    __instance.experience = Config.Data.mods_config.items.DeathItemDrop_Experience;
                }
                if (Config.Data.mods_config.items.Enable_DeathItemDrop_AdditionalRare)
                {
                    __instance.guaranteedAdditionalRare = Config.Data.mods_config.items.DeathItemDrop_AdditionalRare;
                }
            }
        }
    }
}
