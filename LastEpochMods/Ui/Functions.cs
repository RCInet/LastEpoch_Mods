using UnityEngine;

namespace LastEpochMods.Ui
{
    public class Functions
    {
        public static Texture2D MakeTextureFromColor(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) { pixels[i] = color; }
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }
    }
}
