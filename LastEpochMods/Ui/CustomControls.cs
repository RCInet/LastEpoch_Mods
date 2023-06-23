using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LastEpochMods.Ui
{
    public delegate void DelegateFunction();
    public class CustomControls
    {
        public static void EnableButton(string text, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            GUI.TextField(new Rect(pos_x, pos_y, 140, 40), text, Styles.TextField_Style());
            string btn_enable = "Enable";
            if (enable) { btn_enable = "Disable"; }
            if (GUI.Button(new Rect((pos_x + 140), pos_y, 60, 40), btn_enable, Styles.Button_Style(enable))) { function(); }            
        }
        public static float Value(string text, float value, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            EnableButton(text, pos_x, pos_y, enable, function);            
            float temp_value = value;
            GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Styles.TextField_Style().normal.background);
            temp_value = GUI.HorizontalSlider(new Rect(pos_x, (pos_y + 40 + 20), 140, 20), temp_value, 0f, 255f);
            string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), temp_value.ToString(), Styles.TextArea_Style());
            try { temp_value = float.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat); }
            catch { }

            return temp_value;
        }
        public static long LongValue(string text, long value, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            long result = value;
            EnableButton(text, pos_x, pos_y, enable, function);
            float temp_value = System.Convert.ToSingle(value);
            GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Styles.TextField_Style().normal.background);
            float min = System.Convert.ToSingle(long.MinValue);
            float max = System.Convert.ToSingle(long.MaxValue);
            temp_value = GUI.HorizontalSlider(new Rect(pos_x, (pos_y + 40 + 20), 140, 20), temp_value, min, max);
            result = System.Convert.ToInt64(temp_value);
            string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), temp_value.ToString(), Styles.TextArea_Style());
            try
            {
                temp_value = float.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat);
                result = System.Convert.ToInt64(temp_value);                
            }
            catch { }
            
            return result;
        }
    }
}
