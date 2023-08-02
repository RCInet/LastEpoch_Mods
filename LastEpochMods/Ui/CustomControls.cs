using MelonLoader;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using static Il2CppSystem.Linq.Expressions.Interpreter.NullableMethodCallInstruction;

namespace LastEpochMods.Ui
{
    public delegate void DelegateFunction();
    public class CustomControls
    {
        public static void Title(string text, float pos_x, float pos_y)
        {
            GUI.TextField(new Rect(pos_x, pos_y, 200, 40), text, Styles.TextField_Style());
        }
        public static void EnableButton(string text, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            GUI.Label(new Rect(pos_x, pos_y, 140, 40), text, Styles.TextField_Style());
            string btn_enable = "Enable";
            if (enable) { btn_enable = "Disable"; }
            if (GUI.Button(new Rect((pos_x + 140), pos_y, 60, 40), btn_enable, Styles.Button_Style(enable))) { function(); }            
        }
        public static void RarityInfos(float pos_x, float pos_y)
        {
            float w = 100f;
            float h = 20f;
            GUI.TextField(new Rect(pos_x, pos_y, w, h), "Basic : 0", Styles.Infos_Style());
            GUI.TextField(new Rect(pos_x + 100, pos_y, w, h), "Unique : 7", Styles.Infos_Style());
            pos_y += h;
            GUI.TextField(new Rect(pos_x, pos_y, w, h), "Magic : 1-2", Styles.Infos_Style());
            GUI.TextField(new Rect(pos_x + 100, pos_y, w, h), "Set : 8", Styles.Infos_Style());
            pos_y += h;
            GUI.TextField(new Rect(pos_x, pos_y, w, h), "Rare : 3-4", Styles.Infos_Style());
            GUI.TextField(new Rect(pos_x + 100, pos_y, w, h), "Legendary : 9", Styles.Infos_Style());
        }
        public static int IntValue(string text, int minvalue, int maxvalue, int value, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            float multiplier = maxvalue / 255;
            EnableButton(text, pos_x, pos_y, enable, function);
            GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Menu.texture_grey);

            int result = value;
            float min = minvalue / multiplier;
            float max = maxvalue / multiplier;
            float temp_value = value / multiplier;            
            temp_value = (GUI.HorizontalSlider(new Rect(pos_x + 5, (pos_y + 40 + 20), 135, 20), temp_value, min, max) * multiplier);
            string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), (temp_value * multiplier).ToString(), Styles.TextArea_Style());
            try
            {
                int str = int.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat);
                temp_value = str / multiplier;
            }
            catch { }
            result = (int)(temp_value * multiplier);

            return result;
        }
        public static float FloatValue(string text, float minvalue, float maxvalue, float value, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            EnableButton(text, pos_x, pos_y, enable, function);            
            float temp_value = value;
            GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Menu.texture_grey);
            temp_value = GUI.HorizontalSlider(new Rect(pos_x + 5, (pos_y + 40 + 20), 135, 20), temp_value, minvalue, maxvalue);
            string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), temp_value.ToString(), Styles.TextArea_Style());
            try { temp_value = float.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat); }
            catch { }

            return temp_value;
        }
        public static long LongValue(string text, long minvalue, long maxvalue, long value, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            long result = value;
            EnableButton(text, pos_x, pos_y, enable, function);
            float temp_value = System.Convert.ToSingle(value);
            GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Menu.texture_grey);
            float min = System.Convert.ToSingle(minvalue);
            float max = System.Convert.ToSingle(maxvalue);
            temp_value = GUI.HorizontalSlider(new Rect(pos_x + 5, (pos_y + 40 + 20), 135, 20), temp_value, min, max);
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
        public static byte ByteValue(string text, byte minvalue, byte maxvalue, byte value, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            byte result = value;
            EnableButton(text, pos_x, pos_y, enable, function);
            float temp_value = System.Convert.ToSingle(value);
            GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Menu.texture_grey);
            float min = System.Convert.ToSingle(minvalue);
            float max = System.Convert.ToSingle(maxvalue);
            temp_value = GUI.HorizontalSlider(new Rect(pos_x + 5, (pos_y + 40 + 20), 135, 20), temp_value, min, max);
            result = (byte)temp_value;
            string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), temp_value.ToString(), Styles.TextArea_Style());
            try
            {
                temp_value = float.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat);
                result = (byte)temp_value;
            }
            catch { }

            return result;
        }
        public static ushort UshortValue(string text, int minvalue, int maxvalue, ushort value, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            ushort result = value;
            int multiplier = maxvalue / 255;
            EnableButton(text, pos_x, pos_y, enable, function);
            float temp_value = (System.Convert.ToSingle(value) / multiplier);
            GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Menu.texture_grey);
            float min = 0;
            float max = 255;
            temp_value = GUI.HorizontalSlider(new Rect(pos_x + 5, (pos_y + 40 + 20), 135, 20), temp_value, min, max);
            result = (ushort)(temp_value * multiplier);
            string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), (temp_value * multiplier).ToString(), Styles.TextArea_Style());
            try { result = ushort.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat); }
            catch { }

            return result;
        }
    }
}
