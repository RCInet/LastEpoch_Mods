using UnityEngine;
using UniverseLib;

namespace LastEpochMods
{
    public class Functions
    {
        public static UnityEngine.Object GetObject(string name)
        {
            UnityEngine.Object objet = new UnityEngine.Object();
            foreach (UnityEngine.Object obj in RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((name != "") && (obj.name.Contains(name)))
                {
                    System.Type type = obj.GetActualType();
                    if (type != typeof(TextAsset))
                    {
                        objet = obj;
                        break;
                    }
                }
            }
            return objet;
        }        
        public static UnityEngine.Texture2D GetTexture2D(string name)
        {
            UnityEngine.Texture2D picture = null;
            foreach (UnityEngine.Object obj in RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((name != "") && (obj.name.Contains(name)))
                {
                    System.Type type = obj.GetActualType();
                    if (type == typeof(UnityEngine.Texture2D))
                    {
                        picture = obj.TryCast<UnityEngine.Texture2D>();
                        break;
                    }
                }
            }

            return picture;
        }
    }
}
