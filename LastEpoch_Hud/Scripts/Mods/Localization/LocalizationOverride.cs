using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;

namespace LastEpoch_Hud.Scripts.Mods.Localization
{
    public static class LocalizationOverride
    {
        static readonly Dictionary<string, Func<string>> resolvers =
            new Dictionary<string, Func<string>>();

        public static void Register(string key, Func<string> resolver)
        {
            if (!string.IsNullOrEmpty(key) && resolver != null)
            {
                resolvers[key] = resolver;
            }
        }

        public static void RegisterAll()
        {
            Craft.Craft_Locales.RegisterLocales();
            NewItems.Items_Mjolner.Locales.RegisterLocales();
            NewItems.Items_Heralds.Languagues.RegisterLocales();
            NewItems.Items_HeadHunter.HHLocales.RegisterLocales();
            NewItems.Items_EssentiaSanguis.ESLocales.RegisterLocales();
            NewItems.Items_SandsOfSilk.SOSLocales.RegisterLocales();
            NewItems.Items_ArakaalisFang.AFLocales.RegisterLocales();
        }

        [HarmonyPatch(typeof(Il2Cpp.Localization), "TryGetText")]
        public class Localization_TryGetText
        {
            [HarmonyPrefix]
            static bool Prefix(ref bool __result, string __0)
            {
                if (__0 != null && resolvers.ContainsKey(__0))
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Il2Cpp.Localization), "GetText")]
        public class Localization_GetText
        {
            [HarmonyPrefix]
            static bool Prefix(ref string __result, string __0)
            {
                if (__0 != null && resolvers.TryGetValue(__0, out Func<string> r))
                {
                    string value = r();
                    if (!string.IsNullOrEmpty(value))
                    {
                        __result = value;
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
