using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace LastEpochMods.Hooks
{
    public class Bottom_Screen_Menu
    {
        [HarmonyPatch(typeof(BottomScreenMenu), "OnEnable")]
        public class OnEnable
        {
            [HarmonyPostfix]
            static void Postfix(BottomScreenMenu __instance)
            {
                bool disable = true;
                if (Config.Data.mods_config.character.Enable_Cosmetic_Btn) { disable = false; }
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.UI.Button)))
                {
                    if (obj.name == "Cosmetics")
                    {
                        Selectable appearance_tab = obj.TryCast<Selectable>();
                        appearance_tab.interactable = !disable;
                        break;
                    }
                }
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.CanvasGroup)))
                {
                    if (obj.name == "Cosmetics")
                    {
                        UnityEngine.CanvasGroup canvas_group = obj.TryCast<UnityEngine.CanvasGroup>();
                        canvas_group.enabled = disable;
                        break;
                    }
                }
            }
        }
    }
}
