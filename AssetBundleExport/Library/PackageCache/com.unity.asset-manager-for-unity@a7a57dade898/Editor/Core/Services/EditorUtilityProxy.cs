using System;
using UnityEditor;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Unity.AssetManager.Core.Editor
{
    interface IEditorUtilityProxy : IService
    {
        bool IsDirty(Object obj);

        void ClearDirty(Object obj);

        void RevealInFinder(string path);

        void DisplayProgressBar(string title, string info, float progress);

        void ClearProgressBar();

        bool DisplayDialog(string title, string message, string ok);

        bool DisplayDialog(string title, string message, string ok, string cancel);

        int DisplayDialogComplex(string title, string message, string ok, string cancel, string alt);

        string OpenFolderPanel(string title, string folder, string defaultName);
    }

    [Serializable]
    [ExcludeFromCoverage]
    class EditorUtilityProxy : BaseService<IEditorUtilityProxy>, IEditorUtilityProxy
    {
        public bool IsDirty(Object obj) => EditorUtility.IsDirty(obj);

        public void ClearDirty(Object obj) => EditorUtility.ClearDirty(obj);

        public void RevealInFinder(string path) => EditorUtility.RevealInFinder(path);

        public void DisplayProgressBar(string title, string info, float progress) =>
            EditorUtility.DisplayProgressBar(title, info, progress);

        public void ClearProgressBar() => EditorUtility.ClearProgressBar();

        public bool DisplayDialog(string title, string message, string ok) =>
            EditorUtility.DisplayDialog(title, message, ok);

        public bool DisplayDialog(string title, string message, string ok, string cancel) =>
            EditorUtility.DisplayDialog(title, message, ok, cancel);

        public int DisplayDialogComplex(string title, string message, string ok, string cancel, string alt) =>
            EditorUtility.DisplayDialogComplex(title, message, ok, cancel, alt);

        public string OpenFolderPanel(string title, string folder, string defaultName) =>
            EditorUtility.OpenFolderPanel(title, folder, defaultName);
    }
}
