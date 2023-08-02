using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LastEpochMods.OnSceneChanged
{
    public class Notifications_
    {
        public static void Init()
        {
            if (Config.Data.mods_config.auto_loot.Hide_materials_notifications)
            {
                GameObject gui = null;
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
                {
                    if (obj.name == "GUI")
                    {
                        gui = obj.TryCast<UnityEngine.GameObject>();
                        break;
                    }
                }
                GameObject canvas_animated = null;
                if (gui != null)
                {
                    for (int i = 0; i < gui.transform.childCount; i++)
                    {
                        string obj_name = gui.transform.GetChild(i).gameObject.name;
                        if (obj_name == "Canvas (animated)")
                        {
                            canvas_animated = gui.transform.GetChild(i).gameObject;
                            break;
                        }
                    }
                }
                if (canvas_animated != null)
                {
                    for (int i = 0; i < canvas_animated.transform.childCount; i++)
                    {
                        string obj_name = canvas_animated.transform.GetChild(i).gameObject.name;
                        if (obj_name == "UiPopupText Canvas & Manager")
                        {
                            canvas_animated.transform.GetChild(i).gameObject.GetComponent<UIPopupTextController>().disableUIPopup();
                            break;
                        }
                    }
                }
            }
        }
    }
}
