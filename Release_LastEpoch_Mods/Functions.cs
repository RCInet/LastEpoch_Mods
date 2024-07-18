using UnityEngine;

namespace LastEpochMods
{
    public static class Functions
    {
        public static bool IsNullOrDestroyed(this object obj)
        {
            try
            {
                if (obj == null) { return true; }
                else if (obj is Object unityObj && !unityObj) { return true; }
                return false;
            }
            catch { return true; }
        }
    }
}
