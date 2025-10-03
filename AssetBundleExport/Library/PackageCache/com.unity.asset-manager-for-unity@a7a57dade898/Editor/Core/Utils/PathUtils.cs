using System;
using System.IO;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    static class PathUtils
    {
        public static string Combine(string path1, string path2)
        {
          if (path1 == null)
            throw new ArgumentNullException(nameof (path1));
          if (path2 == null)
            throw new ArgumentNullException(nameof (path2));
          if (path1.Length == 0)
            return path2;
          if (path2.Length == 0)
            return path1;
          if (path1.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            throw new ArgumentException("Illegal characters in path.");
          if (path2.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            throw new ArgumentException("Illegal characters in path.");
          if (Path.IsPathRooted(path2))
            return path2;
          char ch = path1[path1.Length - 1];
          return (int) ch != (int) Path.DirectorySeparatorChar && (int) ch != (int) Path.AltDirectorySeparatorChar && (int) ch != (int) Path.VolumeSeparatorChar ? path1 + '/' + path2 : path1 + path2;
        }
    }
}
