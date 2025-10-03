using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Unity.AssetManager.UI.Editor
{
    class InitOnLoadEditor
    {
        static readonly string k_AssetManagerDeepLinkRoute = "com.unity3d.kharma://com.unity.asset-manager-for-unity/";

        [InitializeOnLoadMethod]
        static void InitAssetManagerEditor()
        {
            UnityEditorApplicationFocusUtils.ApplicationFocusChange += receivedFocus =>
            {
                if (receivedFocus && EditorGUIUtility.systemCopyBuffer.Length > 0)
                {
                    var clipboardContent = EditorGUIUtility.systemCopyBuffer;
                    if (clipboardContent.StartsWith(k_AssetManagerDeepLinkRoute) &&
                        Uri.TryCreate(clipboardContent, UriKind.Absolute, out var assetManagerDeepLink))
                    {
                        var pathSegments = assetManagerDeepLink.Segments;
                        if (pathSegments.Length >= 7)
                        {
                            ExtractAssetIdAndVersion(pathSegments[6], out var id, out var version);

                            var assetIdentifier = new AssetIdentifier(RemoveSegmentDelimiter(pathSegments[2]),
                                RemoveSegmentDelimiter(pathSegments[4]), id, version);

                            // If the Asset Manager window is closed, select the project too, otherwise just select the asset
                            var selectProject = AssetManagerWindow.Instance == null;

                            var openAssetHook = new OpenAssetHook(assetIdentifier, selectProject);
                            openAssetHook.OpenAssetManagerWindow();
                        }
                        else
                        {
                            AssetManagerWindow.Open();
                        }

                        EditorGUIUtility.systemCopyBuffer = string.Empty;
                    }
                }
            };
        }

        static string RemoveSegmentDelimiter(string segment)
        {
            return segment.Trim('/');
        }

        static void ExtractAssetIdAndVersion(string str, out string id, out string version)
        {
            str = RemoveSegmentDelimiter(str);
            var delimiter = str.IndexOf(':');

            if (delimiter == -1)
            {
                id = str;
                version = "1";
            }
            else
            {
                id = str[..delimiter];
                version = str[(delimiter + 1)..];
            }
        }
    }

    [InitializeOnLoad]
    class UnityEditorApplicationFocusUtils
    {
        static bool s_HasFocus;

        public static event Action<bool> ApplicationFocusChange = _ => { };

        static UnityEditorApplicationFocusUtils()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            if (!s_HasFocus && InternalEditorUtility.isApplicationActive)
            {
                s_HasFocus = InternalEditorUtility.isApplicationActive;
                ApplicationFocusChange(true);
            }
            else if (s_HasFocus && !InternalEditorUtility.isApplicationActive)
            {
                s_HasFocus = InternalEditorUtility.isApplicationActive;
                ApplicationFocusChange(false);
            }
        }
    }

    class OpenAssetHook
    {
        readonly AssetIdentifier m_AssetIdentifier;
        readonly bool m_SelectProject;
        IPageManager m_PageManager;
        IProjectOrganizationProvider m_ProjectProvider;

        public OpenAssetHook(AssetIdentifier assetIdentifier, bool selectProject)
        {
            m_AssetIdentifier = assetIdentifier;
            m_SelectProject = selectProject;
        }

        public void OpenAssetManagerWindow()
        {
            var assetManagerWindowHook = new AssetManagerWindowHook();
            assetManagerWindowHook.OrganizationLoaded += OpenAsset;
            assetManagerWindowHook.OpenAssetManagerWindow();
        }

        void OpenAsset()
        {
            m_PageManager = ServicesContainer.instance.Resolve<IPageManager>();
            m_ProjectProvider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();

            if (string.IsNullOrEmpty(m_ProjectProvider.SelectedOrganization?.Id))
                return;

            if (m_ProjectProvider.SelectedOrganization.Id != m_AssetIdentifier.OrganizationId)
            {
                Debug.LogWarning("Organization mismatch. Cannot open asset details.");
                return;
            }

            var switchProject = false;

            if (m_ProjectProvider.SelectedProject?.Id != m_AssetIdentifier.ProjectId)
            {
                switchProject = m_SelectProject
                    || string.IsNullOrEmpty(m_ProjectProvider.SelectedProject?.Id)
                    || m_PageManager.ActivePage is not CollectionPage;
            }

            if (switchProject)
            {
                m_ProjectProvider.ProjectSelectionChanged += SelectAsset;
                m_ProjectProvider.SelectProject(m_AssetIdentifier.ProjectId);
            }
            else
            {
                SelectAsset(null, null);
            }
        }

        void SelectAsset(ProjectInfo _, CollectionInfo __)
        {
            m_ProjectProvider.ProjectSelectionChanged -= SelectAsset;
            var collectionPage = (CollectionPage)m_PageManager.ActivePage;
            collectionPage.SelectAsset(m_AssetIdentifier, false);
        }
    }
}
