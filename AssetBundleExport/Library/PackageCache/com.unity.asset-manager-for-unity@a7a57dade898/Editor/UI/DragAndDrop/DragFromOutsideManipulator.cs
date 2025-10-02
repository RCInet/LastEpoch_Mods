using System;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    class DragFromOutsideManipulator : PointerManipulator
    {
        [SerializeReference]
        IPageManager m_PageManager;

        [SerializeReference]
        IUploadManager m_UploadManager;

        bool m_CanDropOnPage;

        public DragFromOutsideManipulator(VisualElement rootTarget, IPageManager pageManager,
            IUploadManager uploadManager)
        {
            target = rootTarget;
            m_PageManager = pageManager;
            m_UploadManager = uploadManager;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragPerformEvent>(OnDragPerform);
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.RegisterCallback<DragEnterEvent>(OnDragEnter);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.UnregisterCallback<DragEnterEvent>(OnDragEnter);
        }

        void OnDragPerform(DragPerformEvent _)
        {
            if(DragAndDrop.objectReferences.Length == 0 || Array.TrueForAll(DragAndDrop.objectReferences, o => o is DraggableObjectToImport))
                return;

            DragAndDrop.AcceptDrag();

            if (!(m_PageManager.ActivePage is UploadPage))
            {
                m_PageManager.SetActivePage<UploadPage>();
            }

            var uploadPage = m_PageManager.ActivePage as UploadPage;
            uploadPage?.AddAssets(DragAndDrop.objectReferences.Where(o =>
                !string.IsNullOrEmpty(ServicesContainer.instance.Resolve<IAssetDatabaseProxy>().GetAssetPath(o))).ToList());
        }

        void OnDragUpdate(DragUpdatedEvent _)
        {
            DragAndDrop.visualMode = m_CanDropOnPage
                ? DragAndDropVisualMode.Generic
                : DragAndDropVisualMode.Rejected;
        }

        void OnDragEnter(DragEnterEvent _)
        {
            if (m_UploadManager.IsUploading)
            {
                m_CanDropOnPage = false;

                return;
            }

            m_CanDropOnPage = !Array.TrueForAll(DragAndDrop.objectReferences,
                o => string.IsNullOrEmpty(ServicesContainer.instance.Resolve<IAssetDatabaseProxy>().GetAssetPath(o)));
        }
    }
}
