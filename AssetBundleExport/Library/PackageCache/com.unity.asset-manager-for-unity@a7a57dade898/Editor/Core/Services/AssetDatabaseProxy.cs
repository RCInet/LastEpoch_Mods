using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Unity.AssetManager.Core.Editor
{
    interface IAssetDatabaseProxy : IService
    {
        event Action<string[] /*importedAssets*/, string[] /*deletedAssets*/, string[] /*movedAssets*/, string[] /*movedFromAssetPaths*/> PostprocessAllAssets;

        string[] FindAssets(string filter, string[] searchInFolders);

        bool DeleteAssets(string[] paths, List<string> outFailedPaths);
        string AssetPathToGuid(string assetPath);
        string GuidToAssetPath(string guid);
        bool IsValidFolder(string path);
        void Refresh();
        string GetAssetPath(Object obj);
        string GetTextMetaFilePathFromAssetPath(string fileName);
        string[] GetDependencies(string assetPath, bool recursive);
        void SaveAssetIfDirty(string assetPath);
        void ImportAsset(string assetPath);
        string[] GetLabels(Object obj);
        void StartAssetEditing();
        void StopAssetEditing();
        Object LoadAssetAtPath(string assetPath);
        Object LoadAssetAtPath(string assetPath, Type type);

        bool PingAssetByGuid(string guid);
        bool CanPingAssetByGuid(string guid);
        IEnumerable<string> GetAssetsInFolder(string folder);
    }

    [Serializable]
    [ExcludeFromCoverage]
    class AssetDatabaseProxy : BaseService<IAssetDatabaseProxy>, IAssetDatabaseProxy
    {
        class AssetPostprocessor : UnityEditor.AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (ServicesContainer.instance.Resolve<IAssetDatabaseProxy>() is AssetDatabaseProxy assetDatabaseProxy)
                {
                    assetDatabaseProxy.PostprocessAllAssets?.Invoke(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
                }
            }
        }

        public event Action<string[] /*importedAssets*/, string[] /*deletedAssets*/, string[] /*movedAssets*/, string[] /*movedFromAssetPaths*/> PostprocessAllAssets = delegate {};

        // Wrapper AssetDatabase methods

        public string[] FindAssets(string filter, string[] searchInFolders) => AssetDatabase.FindAssets(filter, searchInFolders);

        public bool DeleteAssets(string[] paths, List<string> outFailedPaths) => AssetDatabase.DeleteAssets(paths, outFailedPaths);

        public string AssetPathToGuid(string assetPath) => AssetDatabase.AssetPathToGUID(assetPath);

        public string GuidToAssetPath(string guid) => AssetDatabase.GUIDToAssetPath(guid);

        public bool IsValidFolder(string path) => AssetDatabase.IsValidFolder(path);

        public void Refresh() => AssetDatabase.Refresh();

        public string GetAssetPath(Object obj) => AssetDatabase.GetAssetPath(obj);

        public string GetTextMetaFilePathFromAssetPath(string fileName) => AssetDatabase.GetTextMetaFilePathFromAssetPath(fileName);

        public string[] GetDependencies(string assetPath, bool recursive) => AssetDatabase.GetDependencies(assetPath, recursive);
        
        public void SaveAssetIfDirty(string assetPath)
        {
            var asset = LoadAssetAtPath(assetPath);
            if (asset == null)
                return;

            if (asset is SceneAsset)
            {
                var scene = SceneManager.GetSceneByPath(assetPath);
                if (scene.isDirty)
                {
                    EditorSceneManager.SaveScene(scene);
                }
            }
            else
            {
                AssetDatabase.SaveAssetIfDirty(asset);
            }
        }

        public void ImportAsset(string assetPath) => AssetDatabase.ImportAsset(assetPath);

        public string[] GetLabels(Object obj) => AssetDatabase.GetLabels(obj);

        public void StartAssetEditing() => AssetDatabase.StartAssetEditing();

        public void StopAssetEditing() => AssetDatabase.StopAssetEditing();

        public Object LoadAssetAtPath(string assetPath) => AssetDatabase.LoadAssetAtPath<Object>(assetPath);

        public Object LoadAssetAtPath(string assetPath, Type type) => AssetDatabase.LoadAssetAtPath(assetPath, type);

        // End of wrapper AssetDatabase methods

        Object GetAssetObject(string guid)
        {
            return LoadAssetAtPath(GuidToAssetPath(guid));
        }

        public bool PingAssetByGuid(string guid)
        {
            var assetObject = GetAssetObject(guid);

            if (assetObject != null)
            {
                EditorGUIUtility.PingObject(assetObject);
                return true;
            }

            return false;
        }

        public bool CanPingAssetByGuid(string guid)
        {
            return GetAssetObject(guid) != null;
        }

        public IEnumerable<string> GetAssetsInFolder(string folder)
        {
            var subAssetGuids = FindAssets(string.Empty, new[] { folder });
            foreach (var subAssetGuid in subAssetGuids)
            {
                var path = GuidToAssetPath(subAssetGuid);
                if (!IsValidFolder(path))
                {
                    yield return subAssetGuid;
                }
            }
        }
    }
}
