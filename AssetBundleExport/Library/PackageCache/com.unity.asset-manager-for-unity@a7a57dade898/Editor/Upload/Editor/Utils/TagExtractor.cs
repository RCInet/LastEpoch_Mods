using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.Upload.Editor
{
    static class TagExtractor
    {
        public static ISet<string> ExtractFromAsset(string assetPath)
        {
            var assetDatabaseProxy = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();
            var asset = assetDatabaseProxy.LoadAssetAtPath(assetPath);

            if (asset == null)
            {
                Utilities.DevLogError($"Cannot load asset {assetPath} to extract all tags.");
            }

            var tags = new HashSet<string>();

            if (assetDatabaseProxy.IsValidFolder(assetPath))
            {
                tags.Add("Folder");
            }
            else
            {
                if (asset != null)
                {
                    tags.Add(asset.GetType().Name);
                }

                var extension = Path.GetExtension(assetPath);

                if (!string.IsNullOrWhiteSpace(extension))
                {
                    tags.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(extension.TrimStart('.')));
                }
            }

            if (asset != null)
            {
                tags.UnionWith(assetDatabaseProxy.GetLabels(asset));
            }

            tags.UnionWith(ExtractPackageTags(assetPath));

            return tags;
        }

        static IEnumerable<string> ExtractPackageTags(string assetPath)
        {
            var processedPackages = new HashSet<string>();

            var assetDatabaseProxy = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();

            foreach (var dependenciesPath in assetDatabaseProxy.GetDependencies(assetPath, true))
            {
                if (!dependenciesPath.StartsWith("packages", StringComparison.CurrentCultureIgnoreCase))
                    continue;

                if (ExtractStringBetweenPackages(dependenciesPath, out var packageName))
                {
                    if (!processedPackages.Add(packageName))
                        continue;

                    if (packageName.Equals("render-pipelines.high-definition", StringComparison.InvariantCultureIgnoreCase))
                        yield return "HDRP";

                    if (packageName.Equals("render-pipelines.universal", StringComparison.InvariantCultureIgnoreCase))
                        yield return "URP";

                    yield return packageName;
                }
            }
        }

        static bool ExtractStringBetweenPackages(string input, out string packageName)
        {
            input = input.Replace('\\', '/').ToLower();

            packageName = null;
            var match = Regex.Match(input, @"packages/(.*?)/");

            if (!match.Success)
            {
                return false;
            }

            packageName = match.Groups[1].Value.Replace("com.unity.", "");
            return true;
        }
    }
}
