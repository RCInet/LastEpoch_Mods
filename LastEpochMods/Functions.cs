using UnityEngine;
using UniverseLib;
using static LastEpochMods.Mods.LastEpochSaveEditor;

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
    }
}
