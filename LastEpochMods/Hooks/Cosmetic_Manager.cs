using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Cosmetic_Manager
    {
        [HarmonyPatch(typeof(CosmeticsManager), "OnPreSceneLoad")]
        public class OnPreSceneLoad
        {
            [HarmonyPostfix]
            static void Postfix(ref CosmeticsManager __instance, string __0, string __1)
            {
                //Main.logger_instance.Msg("CosmeticsManager : OnPreSceneLoad");
                Mods.Cosmetics.Manager.__instance = __instance;
                //__instance.cosmeticPoints = 999;                
            }
        }
        
        [HarmonyPatch(typeof(CosmeticsManager), "SendSelectCosmetic")]
        public class SendSelectCosmetic
        {
            [HarmonyPostfix]
            static void Postfix(CosmeticsManager __instance, CosmeticItemObject __0, LE.Networking.Cosmetics.CosmeticEquipSlot __1)
            {
                //Main.logger_instance.Msg("CosmeticsManager : SendSelectCosmetic");
            }
        }

        [HarmonyPatch(typeof(CosmeticsManager), "PurchaseCosmetic")]
        public class PurchaseCosmetic
        {
            [HarmonyPostfix]
            static void Postfix(CosmeticsManager __instance, CosmeticItemObject __0)
            {
                //Main.logger_instance.Msg("CosmeticsManager : PurchaseCosmetic");
            }
        }
    }
}
