using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    static class DependencyUtils
    {
        // Reuse the existing internal method to get all script guids
        static readonly System.Reflection.MethodInfo s_GetAllScriptGuids = Type
            .GetType("UnityEditorInternal.InternalEditorUtility,UnityEditor.dll")
            ?.GetMethod("GetAllScriptGUIDs",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        // Some Unity dependencies like cginc files can only be fetched outside of the AssetDatabase and require the call the internal Unity methods
        static readonly System.Reflection.MethodInfo s_GetSourceAssetImportDependenciesAsGUIDs = Type
            .GetType("UnityEditor.AssetDatabase,UnityEditor.dll")
            ?.GetMethod("GetSourceAssetImportDependenciesAsGUIDs",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        static readonly System.Reflection.MethodInfo s_GetImportedAssetImportDependenciesAsGUIDs = Type
            .GetType("UnityEditor.AssetDatabase,UnityEditor.dll")
            ?.GetMethod("GetImportedAssetImportDependenciesAsGUIDs",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        public static IEnumerable<string> GetValidAssetDependencyGuids(string assetGuid, bool recursive, HashSet<string> processed = null)
        {
            processed ??= new HashSet<string>();

            var dependencies = new HashSet<string>();

            if (!processed.Add(assetGuid))
            {
                return dependencies;
            }

            var assetDatabaseProxy = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();
            var assetPath = assetDatabaseProxy.GuidToAssetPath(assetGuid);

            foreach (var path in assetDatabaseProxy.GetDependencies(assetPath, false))
            {
                var guid = assetDatabaseProxy.AssetPathToGuid(path);

                if (guid == null)
                    continue;

                if (guid == assetGuid)
                    continue;

                AddIfValidAssetPath(dependencies, guid, assetDatabaseProxy);
            }

            try
            {
                foreach (var guid in InvokeMethod(s_GetSourceAssetImportDependenciesAsGUIDs, assetPath))
                {
                    AddIfValidAssetPath(dependencies, guid, assetDatabaseProxy);
                }

                foreach (var guid in InvokeMethod(s_GetImportedAssetImportDependenciesAsGUIDs, assetPath))
                {
                    AddIfValidAssetPath(dependencies, guid, assetDatabaseProxy);
                }
            }
            catch (Exception e)
            {
                Utilities.DevLogException(e);
            }

            if (recursive)
            {
                var newDependencies = new HashSet<string>(dependencies);
                foreach (var dependency in newDependencies)
                {
                    dependencies.UnionWith(GetValidAssetDependencyGuids(dependency, true, processed));
                }
            }

            // Remove the main assetGuid in case we have circular dependencies
            dependencies.Remove(assetGuid);

            return dependencies;
        }

        static void AddIfValidAssetPath(HashSet<string> dependencies, string guid, IAssetDatabaseProxy assetDatabaseProxy)
        {
            if (string.IsNullOrEmpty(guid))
                return;

            if (dependencies.Contains(guid))
                return;

            var assetPath = assetDatabaseProxy.GuidToAssetPath(guid);

            if (string.IsNullOrEmpty(assetPath))
                return;

            if (IsPathInsideAssetsFolder(assetPath))
            {
                dependencies.Add(guid);
            }
        }

        public static IEnumerable<string> GetAllScriptGuids()
        {
            return InvokeMethod(s_GetAllScriptGuids);
        }

        static IEnumerable<string> InvokeMethod(System.Reflection.MethodInfo method, string assetPath = null)
        {
            var parameters = assetPath == null ? null : new object[] { assetPath };

            var results = method?.Invoke(null, parameters);

            if (results is not IEnumerable array)
                yield break;

            foreach (var item in array)
            {
                yield return (string)item;
            }
        }

        public static bool IsPathInsideAssetsFolder(string assetPath)
        {
            return assetPath.Replace('\\', '/').ToLower().StartsWith("assets/");
        }
    }
}
