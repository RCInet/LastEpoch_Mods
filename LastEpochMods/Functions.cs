using System.Xml.Linq;
using UnityEngine;
using UniverseLib;

namespace LastEpochMods
{
    public class Functions
    {
        public static UnityEngine.Object GetObject(string name)
        {
            UnityEngine.Object objet = new UnityEngine.Object();
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
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
        public static Il2CppSystem.Collections.Generic.List<UnityEngine.Object> GetCharacter_Objets()
        {
            Il2CppSystem.Collections.Generic.List<UnityEngine.Object> list = new Il2CppSystem.Collections.Generic.List<Object>();
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if (obj.name == "MainPlayer(Clone)") { list.Add(obj); }
            }

            return list;
        }
    }
}
