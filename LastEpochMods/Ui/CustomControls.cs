using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LastEpochMods.Ui
{
    public class CustomControls
    {
        public static float Value(string text, float value, float pos_x, float pos_y)
        {
            float temp_value = value;
            GUI.TextField(new Rect(pos_x, pos_y, 150, 40), text, Styles.Title_Style());
            GUI.TextField(new Rect(pos_x, (pos_y + 45), 50, 40), value.ToString(), Styles.Title_Style());
            temp_value = GUI.HorizontalSlider(new Rect((pos_x + 50), (pos_y + 45 + 20), 150, 20), temp_value, 0f, 255f);

            return temp_value;
        }
    }
}
