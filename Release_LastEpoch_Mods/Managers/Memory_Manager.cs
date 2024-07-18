﻿using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace LastEpochMods.Managers
{
    public class Memory_Manager
    {
        public static string Stats;
        private static bool ShowUI = false;
        private static readonly float margin = 10;
        private static readonly float content_margin = 5;
        private static readonly float Size_w = 500;
        private static readonly float Size_h = 80;
        private static readonly float Button_w = 100;

        public static void QuickUpdate()
        {
            if (ShowUI)
            {
                var sb = new StringBuilder(500);
                sb.AppendLine($"Allocated Memory : {Profiler.GetTotalAllocatedMemory()}");
                sb.AppendLine($"Reserved Memory: {Profiler.GetTotalReservedMemory()}");
                sb.AppendLine($"Unused Reserved Memory: {Profiler.GetTotalUnusedReservedMemory()}");
                //sb.AppendLine($"System Used Memory: {System.GC.GetTotalMemory(false)}");
                sb.AppendLine($"Il2CppSystem Used Memory: {Il2CppSystem.GC.GetTotalMemory(false)}");
                Stats = sb.ToString();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F5)) { ShowUI = !ShowUI; }        
        }
        private static void ClearMemory()
        {
            Il2CppSystem.GC.Collect(); // 0, Il2CppSystem.GCCollectionMode.Optimized);
            System.GC.Collect(); // 0, System.GCCollectionMode.Optimized);
        }
        public static void OnGUI()
        {
            if (ShowUI)
            {
                GUI.DrawTexture(new Rect((Screen.width / 2) - (Size_w / 2), margin, Size_w, Size_h), GUI_Manager.Textures.black);
                GUI.Label(new Rect((Screen.width / 2) - (Size_w / 2) + content_margin, margin + content_margin, Size_w - Button_w - (3 * content_margin), Size_h - (2 * content_margin)), Stats);
                if (GUI.Button(new Rect((Screen.width / 2) + (Size_w / 2) - Button_w - content_margin, margin + content_margin, Button_w, Size_h - (2 * content_margin)), "Clear")) { ClearMemory(); }
            }
        }
    }
}
