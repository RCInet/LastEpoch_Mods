using System;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    interface IProjectWindowProxy : IService
    {
        void Repaint();
    }

    class ProjectWindowProxy : BaseService<IProjectWindowProxy>, IProjectWindowProxy
    {
        EditorWindow m_ProjectWindow;

        public void Repaint()
        {
            m_ProjectWindow ??= FindProjectWindow();
            if (m_ProjectWindow == null)
                return;

            m_ProjectWindow.Repaint();
        }

        static EditorWindow FindProjectWindow()
        {
            var projectBrowserType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");

            var windows = Resources.FindObjectsOfTypeAll(projectBrowserType);

            if (windows.Length == 0)
                return null;

            return windows[0] as EditorWindow;
        }
    }
}
