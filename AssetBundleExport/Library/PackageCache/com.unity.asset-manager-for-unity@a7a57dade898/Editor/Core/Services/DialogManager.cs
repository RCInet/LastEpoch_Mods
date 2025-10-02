using System;
using System.IO;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IDialogManager : IService
    {
        void DisplayProgressBar(string title, float progress, string info = null);

        void ClearProgressBar();

        string OpenFolderPanel(string title, string folder);
    }

    [Serializable]
    class DialogManager : BaseService<IDialogManager>, IDialogManager
    {
        [SerializeReference]
        IEditorUtilityProxy m_EditorUtility;

        [ServiceInjection]
        public void Inject(IEditorUtilityProxy editorUtilityProxy)
        {
            m_EditorUtility = editorUtilityProxy;
        }

        protected override void ValidateServiceDependencies()
        {
            base.ValidateServiceDependencies();

            m_EditorUtility ??= ServicesContainer.instance.Get<IEditorUtilityProxy>();
        }

        public void DisplayProgressBar(string title, float progress, string info = null)
        {
            m_EditorUtility.DisplayProgressBar(title, info, progress);
        }

        public void ClearProgressBar()
        {
            m_EditorUtility.ClearProgressBar();
        }

        public string OpenFolderPanel(string title, string folder)
        {
            bool isValidPath;
            string importLocation;

            do
            {
                importLocation = m_EditorUtility.OpenFolderPanel(title, folder, string.Empty);

                isValidPath = string.IsNullOrEmpty(importLocation) || Utilities.IsSubdirectoryOrSame(importLocation, folder);

                if (!isValidPath)
                {
                    m_EditorUtility.DisplayDialog("Select a valid folder",
                        "The default import location must be located inside the Assets folder of your project.", "Ok");
                }
            } while (!isValidPath);

            return importLocation;
        }
    }
}
