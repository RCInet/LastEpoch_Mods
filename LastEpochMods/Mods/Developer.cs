using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Developer
    {
        public static bool DevLoaded = false;        
        public static void ShowHide_Dev()
        {
            if (!Remove_Hide) { Remove_Hide = Remove_HideInBuilds(); }
            if (Remove_Hide)
            {
                bool show = true;
                if (DevLoaded) { show = false; }
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
                {
                    if (obj.name == "DeveloperMode")
                    {
                        UnityEngine.GameObject game_object = obj.TryCast<UnityEngine.GameObject>();
                        game_object.active = show;
                        DevLoaded = show;
                        break;
                    }
                }
            }
            else { Main.logger_instance.Msg("DeveloperMode : Can't Enable without Disable HideInBuilds"); }
        }

        private static bool Remove_Hide = false;
        private static bool Remove_HideInBuilds()
        {
            bool result = false;
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(HideInBuilds)))
            {
                if (obj.name == "DeveloperMode")
                {
                    HideInBuilds hide_in_build = obj.TryCast<HideInBuilds>();
                    hide_in_build.enabled = false;
                    result = true;
                    break;
                }                
            }
            if (!result) { Main.logger_instance.Msg("DeveloperMode : HideInBuilds Not Found"); }

            return result;
        }
    }
}
