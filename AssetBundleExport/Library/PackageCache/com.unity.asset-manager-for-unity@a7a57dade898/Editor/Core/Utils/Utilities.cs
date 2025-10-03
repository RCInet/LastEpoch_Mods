using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    static class Utilities
    {
        static readonly string[] k_SizeSuffixes = {"B", "Kb", "Mb", "Gb", "Tb"};

        internal static string BytesToReadableString(double bytes)
        {
            if (bytes == 0)
            {
                return $"0 {k_SizeSuffixes[0]}";
            }

            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            var value = Math.Sign(bytes) * num;

            return place >= k_SizeSuffixes.Length ? $"{bytes} {k_SizeSuffixes[0]}" : $"{value} {k_SizeSuffixes[place]}";
        }

        public static string EscapeBackslashes(string str)
        {
            return string.IsNullOrWhiteSpace(str) ? str : str.Replace(@"\", @"\\");
        }

        public static long DatetimeToTimestamp(DateTime value)
        {
            return (long) (value - AssetManagerCoreConstants.UnixEpoch).TotalMilliseconds;
        }

        public static string DatetimeToString(DateTime? value)
        {
            return value?.ToLocalTime().ToString("G");
        }

        public static int ConvertTo12HourTime(int hour24)
        {
            return hour24 == 12 ? 12 : hour24 % 12;
        }

        public static int ConvertTo24HourTime(int hour12, bool isPm)
        {
            if (isPm)
            {
                return hour12 % 12 + 12;
            }

            if (hour12 == 12)
            {
                return 0;
            }

            return hour12;
        }

        public static string PascalCaseToSentence(this string input)
        {
            return Regex.Replace(input, "(\\B[A-Z])", " $1");
        }

        [System.Diagnostics.Conditional("AM4U_DEV")]
        public static void DevLog(string message)
        {
            Debug.Log(message);
        }

        [System.Diagnostics.Conditional("AM4U_DEV")]
        public static void DevAssert(bool condition, string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.Assert(condition);
            }
            else
            {
                Debug.Assert(condition, message);
            }
        }

        [System.Diagnostics.Conditional("AM4U_DEV")]
        public static void DevLogError(string message)
        {
            Debug.LogError(message);
        }

        [System.Diagnostics.Conditional("AM4U_DEV")]
        public static void DevLogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        [System.Diagnostics.Conditional("AM4U_DEV")]
        public static void DevLogException(Exception e)
        {
            Debug.LogException(e);
        }

        public static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return string.Empty;
            }

            var cleanFullName = Regex.Replace(fullName, @"[^\p{L}\p{Z}-]+", " ").Trim();
            cleanFullName = Regex.Replace(cleanFullName, @"\s*(Jr|Sr|[IVX]+)\.?$", "", RegexOptions.IgnoreCase).Trim();

            var words = cleanFullName.Split(new char[]
            {
                ' '
            }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 1)
            {
                return words[0][..1].ToUpperInvariant();
            }

            var initials = new StringBuilder();
            initials.Append(words[0][..1]);
            initials.Append(words[^1][..1]);

            return initials.ToString().ToUpperInvariant();
        }

        public static int DivideRoundingUp(int x, int y)
        {
            // TODO: Define behaviour for negative numbers
            var quotient = Math.DivRem(x, y, out var remainder);
            return remainder == 0 ? quotient : quotient + 1;
        }

        public static bool CompareListsBeginnings(IList baseList, IList extendedList)
        {
            if (baseList == null && extendedList == null)
            {
                return true;
            }

            if (baseList == null || extendedList == null || baseList.Count > extendedList.Count || baseList.Count == 0)
            {
                return false;
            }

            for (var i = 0; i < baseList.Count; i++)
            {
                var baseListObject = baseList[i];
                var extendedListObject = extendedList[i];
                if (
                    (baseListObject == null && extendedListObject != null) ||
                    (baseListObject != null && extendedListObject == null) ||
                    baseListObject != null && !baseListObject.Equals(extendedListObject))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetPathRelativeToAssetsFolder(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return null;

            var application = ServicesContainer.instance.Resolve<IApplicationProxy>();
            var relativePath = Path.GetRelativePath(application.DataPath, assetPath);
            return NormalizePathSeparators(relativePath);
        }

        public static string GetPathRelativeToAssetsFolderIncludeAssets(string assetPath)
        {
            var str = GetPathRelativeToAssetsFolder(assetPath);

            if (string.IsNullOrEmpty(str))
                return null;

            return Path.Combine("Assets", str);
        }

        public static bool ComparePaths(string path1, string path2)
        {
            return string.Equals(NormalizePathSeparators(path1), NormalizePathSeparators(path2),
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSubdirectoryOrSame(string subdirectoryPath, string directoryPath)
        {
            if (string.IsNullOrEmpty(subdirectoryPath))
                return false;

            var directory = new DirectoryInfo(directoryPath);
            var subdirectory = new DirectoryInfo(subdirectoryPath);

            return subdirectory.FullName.StartsWith(directory.FullName);
        }
        public static string NormalizePathSeparators(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            var application = ServicesContainer.instance.Resolve<IApplicationProxy>();

            // Path normalization depends on the current OS
            var str = application.Platform == RuntimePlatform.WindowsEditor ?
                path.Replace('/', Path.DirectorySeparatorChar) :
                path.Replace('\\', Path.DirectorySeparatorChar);

            var pattern = Path.DirectorySeparatorChar == '\\' ? "\\\\+" : "/+";
            return Regex.Replace(str, pattern, Path.DirectorySeparatorChar.ToString());
        }

        public static string GetUniqueFilename(ICollection<string> allFilenames, string filename)
        {
            var uniqueFilename = filename;
            var counter = 1;

            while (allFilenames.Contains(uniqueFilename))
            {
                var extension = Path.GetExtension(filename);
                var fileWithoutExtension = string.IsNullOrEmpty(extension) ? filename : filename[..^extension.Length];

                uniqueFilename = $"{fileWithoutExtension} ({counter}){extension}";
                ++counter;
            }

            return uniqueFilename;
        }

        public static string ExtractCommonFolder(ICollection<string> filePaths)
        {
            if (filePaths.Count == 0)
            {
                return string.Empty;
            }

            var sanitizedPaths = filePaths.Select(NormalizePathSeparators).ToList();

            var reference = sanitizedPaths[0]; // We can optimize this by selecting the shortest path

            if (filePaths.Count == 1)
            {
                return reference[..^Path.GetFileName(reference).Length];
            }

            var folders = reference.Split(Path.DirectorySeparatorChar);

            if (folders.Length == 0)
            {
                return string.Empty;
            }

            var result = string.Empty;

            foreach (var folder in folders)
            {
                var attempt = result + folder + Path.DirectorySeparatorChar;

                if (sanitizedPaths.TrueForAll(p => p.StartsWith(attempt, StringComparison.OrdinalIgnoreCase)))
                {
                    result = attempt;
                }
                else
                {
                    break;
                }
            }

            if (result.Length < 2) // Avoid returning empty folders
            {
                return string.Empty;
            }

            return NormalizePathSeparators(result);
        }
    }
}
