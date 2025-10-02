using System;
using Unity.AssetManager.Core.Editor;
using UnityEditor;

namespace Unity.AssetManager.Core.Editor
{
    interface IDragAndDropProjectBrowserProxy : IService
    {
        void RegisterProjectBrowserHandler(DragAndDrop.ProjectBrowserDropHandler projectHandlerDelegate);
        void UnRegisterProjectBrowserHandler(DragAndDrop.ProjectBrowserDropHandler projectHandlerDelegate);
    }

    [Serializable]
    class DragAndDropProjectBrowserProxy : BaseService<IDragAndDropProjectBrowserProxy>, IDragAndDropProjectBrowserProxy
    {
        public void RegisterProjectBrowserHandler(DragAndDrop.ProjectBrowserDropHandler projectHandlerDelegate)
        {
            DragAndDrop.AddDropHandler(projectHandlerDelegate);
        }

        public void UnRegisterProjectBrowserHandler(DragAndDrop.ProjectBrowserDropHandler projectHandlerDelegate)
        {
            DragAndDrop.RemoveDropHandler(projectHandlerDelegate);
        }
    }
}
