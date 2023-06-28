using UnityEngine;

namespace LastEpochMods.Ui
{
    public class Styles
    {
        public static GUIStyle Window_Style()
        {
            GUIStyle style = new GUIStyle(GUI.skin.window);
            float alpha = 0f;
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
            style.normal.background = Menu.windowBackground;
            style.normal.textColor = Color.white;
            style.hover.background = Menu.windowBackground;
            style.hover.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;            

            return style;
        }
        public static GUIStyle Infos_Style()
        {
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            style.normal.background = Menu.texture_grey;
            style.normal.textColor = Color.black;
            style.hover.background = Menu.texture_grey;
            style.hover.textColor = Color.black;
            style.focused.background = Menu.texture_grey;
            style.focused.textColor = Color.black;
            style.active.background = Menu.texture_grey;
            style.active.textColor = Color.black;
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = 14;

            return style;
        }
        public static GUIStyle TextArea_Style()
        {
            GUIStyle style = new GUIStyle(GUI.skin.textArea);
            style.normal.background = Menu.texture_grey;
            style.normal.textColor = Color.black;
            style.hover.background = Menu.texture_grey;
            style.hover.textColor = Color.black;
            style.alignment = TextAnchor.MiddleCenter;

            return style;
        }
        public static GUIStyle TextField_Style()
        {
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            style.normal.background = Menu.texture_grey;
            style.normal.textColor = Color.black;
            style.hover.background = Menu.texture_grey;
            style.hover.textColor = Color.black;
            style.focused.background = Menu.texture_grey;
            style.focused.textColor = Color.black;
            style.active.background = Menu.texture_grey;
            style.active.textColor = Color.black;
            style.alignment = TextAnchor.MiddleLeft;

            return style;
        }
        public static GUIStyle Button_Style(bool select)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            if (select)
            {
                style.normal.background = Menu.texture_green;
                style.normal.textColor = Color.black;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
            }
            else
            {
                style.normal.background = Menu.texture_grey;
                style.normal.textColor = Color.black;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
            }
            style.alignment = TextAnchor.MiddleCenter;

            return style;
        }
    }
}
