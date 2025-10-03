using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;

namespace Unity.AssetManager.UI.Editor
{
    static class OpenInAssetManagerHook
    {
        static AssetManagerWindowHook s_AssetManagerWindowHook = new ();

        [MenuItem("Assets/Show in Asset Manager", false, 22)]
        static void ProjectOpenInAssetManagerMenuItem()
        {
            s_AssetManagerWindowHook.OrganizationLoaded += ProjectLoadInProjectPage;
            s_AssetManagerWindowHook.OpenAssetManagerWindow();
        }

        [MenuItem("Assets/Show in Asset Manager", true, 22)]
        static bool ProjectOpenInAssetManagerMenuItemValidation()
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.activeObject == null)
                return false;

            var assetData = GetAssetData(Selection.assetGUIDs);
            return assetData != null && assetData.Any();
        }

        [MenuItem("GameObject/Show in Asset Manager", false, 22)]
        static void HierarchyOpenInAssetManagerMenuItem()
        {
            s_AssetManagerWindowHook.OrganizationLoaded += HierarchyLoadInProjectPage;
            s_AssetManagerWindowHook.OpenAssetManagerWindow();
        }

        [MenuItem("GameObject/Show in Asset Manager", true, 22)]
        static bool HierarchyOpenInAssetManagerMenuItemValidation()
        {
            var selectedGameObjectGUIDs = GetSelectedGameObjectAssetGUIDs();
            if (selectedGameObjectGUIDs.Length == 0)
                return false;

            var assetData = GetAssetData(selectedGameObjectGUIDs);
            return assetData != null && assetData.Any();
        }

        static string[] GetSelectedGameObjectAssetGUIDs()
        {
            var selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects == null || selectedGameObjects.Length == 0)
                return Array.Empty<string>();

            var selectedGuids = new List<string>();
            foreach (var go in selectedGameObjects)
            {
                var prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(go);
                if (prefab != null)
                {
                    var prefabPath = AssetDatabase.GetAssetPath(prefab);
                    if (!string.IsNullOrEmpty(prefabPath))
                    {
                        var guid = AssetDatabase.AssetPathToGUID(prefabPath);
                        if (!string.IsNullOrEmpty(guid))
                            selectedGuids.Add(guid);
                    }
                }
            }

            return selectedGuids.ToArray();

        }

        static void ProjectLoadInProjectPage()
        {
            s_AssetManagerWindowHook.OrganizationLoaded -= ProjectLoadInProjectPage;
            LoadInProjectPage(Selection.assetGUIDs);
        }

        static void HierarchyLoadInProjectPage()
        {
            s_AssetManagerWindowHook.OrganizationLoaded -= HierarchyLoadInProjectPage;
            LoadInProjectPage(GetSelectedGameObjectAssetGUIDs());
        }

        static void LoadInProjectPage(string[] guids)
        {
            AssetManagerWindow.Instance.Focus();

            var projectOrganizationProvider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();
            if (string.IsNullOrEmpty(projectOrganizationProvider.SelectedOrganization?.Id))
                return;

            var pageManager = ServicesContainer.instance.Resolve<IPageManager>();
            if (pageManager.ActivePage is not InProjectPage)
                pageManager.SetActivePage<InProjectPage>();

            var inProjectPage = pageManager.ActivePage as InProjectPage;
            if (inProjectPage == null)
                return;

            var assetDatas = GetAssetData(guids);
            if (assetDatas == null || !assetDatas.Any())
                return;

            inProjectPage.ClearSelection();
            inProjectPage.SelectAssets(assetDatas.Select(asset => asset.Identifier).ToArray());
        }

        static IEnumerable<BaseAssetData> GetAssetData(string[] guids)
        {
            var assetDatas = new List<BaseAssetData>();

            foreach (var guid in guids)
            {
                var assetDataManager = ServicesContainer.instance.Resolve<IAssetDataManager>();
                var importedAssetInfos = assetDataManager.GetImportedAssetInfosFromFileGuid(guid);
                if (importedAssetInfos == null || importedAssetInfos.Count == 0)
                    continue;

                assetDatas.AddRange(importedAssetInfos.Select(info => info.AssetData));
            }

            return assetDatas;
        }
    }
}
