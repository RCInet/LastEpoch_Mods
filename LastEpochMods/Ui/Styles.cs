using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LastEpochMods.Ui
{
    public class Styles
    {
        public static GUIStyle Title_Style()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            //style.normal.background = null;
            style.normal.background = Functions.MakeTextureFromColor(2, 2, Color.black);
            style.normal.textColor = Color.white;
            style.hover.background = Functions.MakeTextureFromColor(2, 2, Color.black);
            style.hover.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            return style;
        }
        public static GUIStyle Button_Style(bool select)
        {
            GUIStyle style = new GUIStyle(GUIStyle.none);            
            if (select)
            {
                style.normal.background = Functions.MakeTextureFromColor(2, 2, Color.green);
                style.normal.textColor = Color.black;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
            }
            else
            {
                style.normal.background = Functions.MakeTextureFromColor(2, 2, Color.black);
                style.normal.textColor = Color.white;
                style.hover.background = Functions.MakeTextureFromColor(2, 2, Color.white);
                style.hover.textColor = Color.black;
            }
            style.alignment = TextAnchor.MiddleCenter;

            return style;
        }
    }
}
