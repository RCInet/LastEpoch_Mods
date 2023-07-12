using System.Globalization;
using UnityEngine;

namespace LastEpochMods.Ui
{
    public delegate void DelegateFunction();
    public class CustomControls
    {
        public static void Title(string text, float pos_x, float pos_y)
        {
            GUI.TextField(new Rect(pos_x, pos_y, 200, 40), text, Styles.TextField_Style());
        }

        public static string[] unique_dropdown_list = null;
        public static Vector2 unique_dropdown_scrollview = Vector2.zero;
        public static Rect unique_dropdown_rect = new Rect(125, 50, 190, 300);
        public static int unique_dropdown_index = -1;
        public static bool show_unique_dropdown = false;
        public static bool dropdowns_init = false;

        public static void DropUnique(float pos_x, float pos_y)
        {
            GUI.DrawTexture(new Rect(pos_x, pos_y, 200, 80), Menu.texture_grey);
            pos_x += 5;
            pos_y += 5;
            GUI.TextField(new Rect(pos_x, pos_y, 135, 40), "Unique / Set", Styles.TextField_Style());
            
            if (unique_dropdown_index < 0) { unique_dropdown_index = 0; }
            if (unique_dropdown_index > UniqueList.get().uniques.Count) { unique_dropdown_index = UniqueList.get().uniques.Count - 1;  }

            if (GUI.Button(new Rect((pos_x + 135), pos_y, 60, 40), "Drop", Styles.Button_Style(true)))
            {
                Mods.Character.DropUniqueOrSet(UniqueList.get().uniques[unique_dropdown_index].name);
            }
            pos_y += 45;            
            CustomControls.UniqueDropdown(pos_x, pos_y);
        }
        public static void InitDropdowns()
        {
            unique_dropdown_list = null;
            try
            {
                unique_dropdown_list = new string[UniqueList.get().uniques.Count];
                for (int i = 0; i < unique_dropdown_list.Length; i++)
                {
                    unique_dropdown_list[i] = UniqueList.get().uniques[i].name;
                }
                dropdowns_init = true;
            }
            catch { }
        }
        public static void UniqueDropdown(float pos_x, float pos_y)
        {
            if (GUI.Button(new Rect(pos_x, pos_y, unique_dropdown_rect.width, 25), "")) { show_unique_dropdown = !show_unique_dropdown; }
            if (show_unique_dropdown)
            {
                unique_dropdown_scrollview = GUI.BeginScrollView(new Rect(pos_x, (pos_y + 25),
                    unique_dropdown_rect.width, unique_dropdown_rect.height),
                    unique_dropdown_scrollview, new Rect(0, 0, unique_dropdown_rect.width,
                    Mathf.Max(unique_dropdown_rect.height, (unique_dropdown_list.Length * 25))));

                GUI.Box(new Rect(pos_x, pos_y + 25, unique_dropdown_rect.width, Mathf.Max(unique_dropdown_rect.height, (unique_dropdown_list.Length * 25))), "");
                for (int index = 0; index < unique_dropdown_list.Length - 1; index++)
                {
                    if (GUI.Button(new Rect(0, (index * 25), unique_dropdown_rect.height, 25), ""))
                    {
                        show_unique_dropdown = false;
                        unique_dropdown_index = index;
                    }
                    GUI.Label(new Rect(10, ((index * 25) + 5), unique_dropdown_rect.height, 25), unique_dropdown_list[index]);
                }
                GUI.EndScrollView();
            }
            else
            {
                pos_x += 5;
                pos_y += 3;
                GUI.Label(new Rect(pos_x, pos_y, unique_dropdown_rect.height, 25), unique_dropdown_list[unique_dropdown_index]);
            }
        }
        public static void EnableButton(string text, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            GUI.TextField(new Rect(pos_x, pos_y, 140, 40), text, Styles.TextField_Style());
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
            int result = value;
            float multiplier = maxvalue / 255;
            EnableButton(text, pos_x, pos_y, enable, function);
            float min = minvalue / multiplier;
            float max = maxvalue / multiplier;
            float temp_value = value / multiplier;
            GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Menu.texture_grey);
            temp_value = (GUI.HorizontalSlider(new Rect(pos_x, (pos_y + 40 + 20), 140, 20), temp_value, min, max) * multiplier);
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
            temp_value = GUI.HorizontalSlider(new Rect(pos_x, (pos_y + 40 + 20), 140, 20), temp_value, minvalue, maxvalue);
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
        public static byte ByteValue(string text, byte minvalue, byte maxvalue, byte value, float pos_x, float pos_y, bool enable, DelegateFunction function)
        {
            byte result = value;
            EnableButton(text, pos_x, pos_y, enable, function);
            float temp_value = System.Convert.ToSingle(value);
            GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Menu.texture_grey);
            float min = System.Convert.ToSingle(minvalue);
            float max = System.Convert.ToSingle(maxvalue);
            temp_value = GUI.HorizontalSlider(new Rect(pos_x, (pos_y + 40 + 20), 140, 20), temp_value, min, max);
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
            temp_value = GUI.HorizontalSlider(new Rect(pos_x, (pos_y + 40 + 20), 140, 20), temp_value, min, max);
            result = (ushort)(temp_value * multiplier);
            string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), (temp_value * multiplier).ToString(), Styles.TextArea_Style());
            try { result = ushort.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat); }
            catch { }

            return result;
        }
    }
}
