using HarmonyLib;
using LE.MicrotransactionSystem;

namespace LastEpoch_Hud.Scripts.Mods.Cosmetics
{
    public class Cosmetics_Add
    {
        /*[HarmonyPatch(typeof(CosmeticsManager), "GetOwnedCosmetics")]
        public class CosmeticsManager_GetOwnedCosmetics
        {
            [HarmonyPostfix]
            static void Postfix(CosmeticsManager __instance, ref Cysharp.Threading.Tasks.UniTask<Il2CppSystem.Collections.Generic.List<string>> __result)
            {
                Main.logger_instance.Msg("CosmeticsManager : GetOwnedCosmeticIds");
                //if (!__instance.IsNullOrDestroyed())
                //{
                    Il2CppSystem.Collections.Generic.List<string> new_result = new Il2CppSystem.Collections.Generic.List<string>();
                    for (int i = 4; i < 9; i++) { new_result.Add(i.ToString()); }

                    //Cysharp.Threading.Tasks.UniTask<Il2CppSystem.Collections.Generic.List<string>> final = Cysharp.Threading.Tasks.UniTask.FromResult<Il2CppSystem.Collections.Generic.List<string>>(new_result);

                    __result = Cysharp.Threading.Tasks.UniTask.FromResult<Il2CppSystem.Collections.Generic.List<string>>(new_result);
                //}
                //else { Main.logger_instance.Msg("CosmeticsManager is null"); }
            }
        }*/    

        /*[HarmonyPatch(typeof(CosmeticsManager), "OnPlayerSpawnedAtLocationEvent")]
        public class CosmeticsManager_OnPlayerSpawnedAtLocationEvent
        {
            [HarmonyPostfix]
            static void Postfix(ref CosmeticsManager __instance, UnityEngine.Vector3 __0)
            {
                if (!__instance.IsNullOrDestroyed())
                {
                    if (__instance.player.IsNullOrDestroyed()) { __instance.player = PlayerFinder.getPlayer(); }
                    //System.Collections.Generic.List<LE.MicrotransactionSystem.UserInventoryItem> user_inventory_items = new System.Collections.Generic.List<LE.MicrotransactionSystem.UserInventoryItem>(); 
                    //__instance.userInventory.GetOwnedCosmeticIds();
                }
            }
        }*/        
    }
}
