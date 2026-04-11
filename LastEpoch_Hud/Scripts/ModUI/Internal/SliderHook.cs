using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Slider events via Harmony -- AddListener doesn't fire managed callbacks in IL2CPP.
    // Patches Slider.set_value globally, dispatches through dictionary by name.
    public static class SliderHook
    {
        private static readonly Dictionary<string, Action<float>> handlers = new();

        public static void Register(string sliderName, Action<float> handler)
        {
            handlers[sliderName] = handler;
        }

        public static void Clear()
        {
            handlers.Clear();
        }

        public static bool TryHandle(string name, float value)
        {
            if (!handlers.TryGetValue(name, out var handler)) return false;
            handler(value);
            return true;
        }

        [HarmonyPatch(typeof(Slider), "set_value")]
        public class SliderSetValuePatch
        {
            [HarmonyPostfix]
            static void Postfix(Slider __instance, float value)
            {
                if (SaveManager.instance == null) return;
                TryHandle(__instance.name, value);
            }
        }
    }
}
