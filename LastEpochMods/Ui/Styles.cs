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
        public static GUIStyle Window_Style()
        {
            GUIStyle style = new GUIStyle(GUI.skin.window);
            float alpha = 0.5f;
            Color new_color = Color.black;
            Color transparent_color = new Color(new_color.r, new_color.g, new_color.b, alpha);
            style.normal.background = Functions.MakeTextureFromColor(2, 2, transparent_color);
            style.normal.textColor = Color.white;
            style.hover.background = style.normal.background;
            style.hover.textColor = style.normal.textColor;

            return style;
        }
        public static GUIStyle Title_Style()
        {
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            style.normal.background = Functions.MakeTextureFromColor(2, 2, Color.black);
            style.normal.textColor = Color.white;
            style.hover.background = Functions.MakeTextureFromColor(2, 2, Color.black);
            style.hover.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;            

            return style;
        }
        public static GUIStyle TextArea_Style()
        {
            GUIStyle style = new GUIStyle(GUI.skin.textArea);
            style.normal.background = Functions.MakeTextureFromColor(2, 2, Color.grey);
            style.normal.textColor = Color.black;
            style.hover.background = Functions.MakeTextureFromColor(2, 2, Color.grey);
            style.hover.textColor = Color.black;
            style.alignment = TextAnchor.MiddleCenter;

            return style;
        }
        public static GUIStyle TextField_Style()
        {
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            style.normal.background = Functions.MakeTextureFromColor(2, 2, Color.grey);
            style.normal.textColor = Color.black;
            style.hover.background = Functions.MakeTextureFromColor(2, 2, Color.grey);
            style.hover.textColor = Color.black;
            style.alignment = TextAnchor.MiddleLeft;

            return style;
        }
        public static GUIStyle Button_Style(bool select)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);            
            if (select)
            {
                style.normal.background = Functions.MakeTextureFromColor(2, 2, Color.green);
                style.normal.textColor = Color.black;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
            }
            else
            {
                style.normal.background = Functions.MakeTextureFromColor(2, 2, Color.grey);
                style.normal.textColor = Color.black;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
            }
            style.alignment = TextAnchor.MiddleCenter;

            return style;
        }
    }
}
