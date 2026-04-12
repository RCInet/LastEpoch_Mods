using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI
{
    // Slider events via Harmony -- AddListener doesn't fire managed callbacks in IL2CPP.
    // Keyed by GetInstanceID so multiple sliders sharing a name don't collide.
    public static class SliderHook
    {
        private static readonly Dictionary<int, Action<float>> handlers = new();

        public static void Register(Slider slider, Action<float> handler)
        {
            if (slider == null || handler == null) return;
            handlers[slider.GetInstanceID()] = handler;
        }

        public static void Unregister(Slider slider)
        {
            if (slider == null) return;
            handlers.Remove(slider.GetInstanceID());
        }

        public static void Clear()
        {
            handlers.Clear();
        }

        [HarmonyPatch(typeof(Slider), "set_value")]
        public class SliderSetValuePatch
        {
            [HarmonyPostfix]
            static void Postfix(Slider __instance, float value)
            {
                if (SaveManager.instance == null) return;
                if (handlers.TryGetValue(__instance.GetInstanceID(), out var handler))
                    handler(value);
            }
        }
    }
}
