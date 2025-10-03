using System;
using Unity.AssetManager.UI.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    [Flags]
    enum UIEnabledStates
    {
        None = 0,
        CanImport = 1,
        InProject = 2,
        HasPermissions = 4,
        ServicesReachable = 8,
        ValidStatus = 16,
        IsImporting = 32,
    }

    /*
     * This class is used to debug the UIEnabledStates enum.
     * It will print the enum value, its binary representation and the flags that are set.
     *
     * Usage example:
     * UIEnabledStates myState = UIEnabledStates.CanImport | UIEnabledStates.InProject | UIEnabledStates.HasPermissions;
     * UIEnabledStatesDebugger.DebugUIEnabledStates(myState);
     */
    internal static class UIEnabledStatesDebugger
    {
        static string EnumToBinaryString(UIEnabledStates state)
        {
            return Convert.ToString((int)state, 2).PadLeft(32, '0');
        }

        internal static void DebugUIEnabledStates(UIEnabledStates state)
        {
            string binaryRepresentation = EnumToBinaryString(state);
            Debug.Log($"UIEnabledStates: {state}");
            Debug.Log($"Binary: {binaryRepresentation}");

            foreach (UIEnabledStates flag in Enum.GetValues(typeof(UIEnabledStates)))
            {
                if (flag != UIEnabledStates.None && state.HasFlag(flag))
                {
                    Debug.Log($"{flag} - {(int)flag}");
                }
            }
        }
    }
}
