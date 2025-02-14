// https://github.com/RolandSolymosi/LastEpoch_Mods/blob/master/LastEpoch_Hud/Scripts/Mods/Items/Items_Crafting_Eternity_Anywhere.cs

using HarmonyLib;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    [RegisterTypeInIl2Cpp]
    public class Items_Crafting_Eternity_Anywhere : MonoBehaviour
    {
        public static Items_Crafting_Eternity_Anywhere instance { get; private set; }
        public Items_Crafting_Eternity_Anywhere(System.IntPtr ptr) : base(ptr) { }

        private static bool isLegendaryCrafting = false;

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if ((Scenes.IsGameScene()) && (!Refs_Manager.game_uibase.IsNullOrDestroyed()) &&
                (Input.GetKeyDown(Save_Manager.instance.data.KeyBinds.EternityCache)))
            {
                var panel = Refs_Manager.EternityCachePanelUI;

                panel.gameObject.active = !panel.gameObject.active;
                if (panel.gameObject.active)
                {
                    panel.Open();
                    isLegendaryCrafting = true;
                }
                else
                {
                    panel.Close();
                    isLegendaryCrafting = false;
                }
            }
        }

        [HarmonyPatch(typeof(EternityCachePanelUI), "seal")]
        public class EternityCachePanelUI_seal
        {
            [HarmonyPrefix]
            static bool Prefix(ref EternityCachePanelUI __instance)
            {
                if (isLegendaryCrafting)
                {
                    if (__instance.beforeMain.IsNullOrDestroyed() 
                        && __instance.beforeMain.Container.IsNullOrDestroyed() 
                        && __instance.beforeMain.Container.GetContent().Count == 0) { return false; }
                    if (__instance.beforeOther.IsNullOrDestroyed()
                        || __instance.beforeOther.Container.IsNullOrDestroyed()
                        || __instance.beforeOther.Container.GetContent().Count == 0) { return false; }

                    var unique = __instance.beforeMain.Container.GetContent()[0].data;
                    var exalted = __instance.beforeOther.Container.GetContent()[0].data;

                    if (unique.IsNullOrDestroyed() || exalted.IsNullOrDestroyed()) { return false; }
                    unique.absorb4ModExaltedItemToBecomeLegendary(exalted);
                    return true;
                }
                else { return false; }
            }
        }
    }
}
