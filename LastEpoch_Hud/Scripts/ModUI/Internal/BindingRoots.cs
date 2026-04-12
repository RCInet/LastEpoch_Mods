using System.Collections.Generic;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI
{
    public enum RootKind
    {
        Generic,        // bind groups directly to the root, no Tab/Menu
        HudWithTabs,    // walks Content + Menu/Content, runs TabManager
    }

    internal static class BindingRoots
    {
        public const string HudRootName = "Hud";

        private static readonly Dictionary<string, Entry> roots = new();

        internal struct Entry
        {
            public GameObject Root;
            public RootKind Kind;
        }

        public static void Register(string name, GameObject root, RootKind kind)
        {
            if (string.IsNullOrEmpty(name) || root == null) return;
            roots[name] = new Entry { Root = root, Kind = kind };
        }

        public static bool TryGet(string name, out GameObject root, out RootKind kind)
        {
            if (roots.TryGetValue(name, out var e) && e.Root != null && !e.Root.IsNullOrDestroyed())
            {
                root = e.Root;
                kind = e.Kind;
                return true;
            }
            root = null;
            kind = RootKind.Generic;
            return false;
        }

        public static bool IsSameRoot(string name, GameObject root)
        {
            return roots.TryGetValue(name, out var e) && ReferenceEquals(e.Root, root);
        }

        public static void Unregister(string name) => roots.Remove(name);

        public static IEnumerable<string> AllNames => roots.Keys;
    }
}
