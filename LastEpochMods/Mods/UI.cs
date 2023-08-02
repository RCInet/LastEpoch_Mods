using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LastEpochMods.Mods.UI;

namespace LastEpochMods.Mods
{
    public class UI
    {
        private static UIBase UiBase = null;
        private static void Get()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UIBase)))
            {
                if (obj.name == "GUI")
                {
                    UiBase = obj.TryCast<UIBase>();
                    break;
                }
            }
        }

        public class MasterieUI
        {
            public static void Show()
            {
                if (UiBase == null) { Get(); }
                if (UiBase != null) { UiBase.openMasteryPanel(false); }
            }            
        }
    }
}
