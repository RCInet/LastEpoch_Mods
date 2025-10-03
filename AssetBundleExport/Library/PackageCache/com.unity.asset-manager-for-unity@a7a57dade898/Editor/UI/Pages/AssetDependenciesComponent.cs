using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class AssetDependenciesComponent : VisualElement
    {
        readonly DependenciesFoldout m_DependenciesFoldout;
        readonly VisualElement m_NoDependenciesBox;

        public AssetDependenciesComponent(VisualElement parent, IPageManager pageManager, IStateManager stateManager = null)
        {
            m_NoDependenciesBox = new Label("No dependencies");
            m_NoDependenciesBox.AddToClassList("no-dependencies-label");
            m_NoDependenciesBox.Q<Label>().text = L10n.Tr(Constants.NoDependenciesText);
            UIElementsUtils.SetDisplay(m_NoDependenciesBox, false);
            parent.Add(m_NoDependenciesBox);
            var dependenciesContainer = new VisualElement
            {
                name = "dependencies-container",
                style =
                {
                    flexGrow = 1
                }
            };

            parent.Add(dependenciesContainer);
            m_DependenciesFoldout =
                new DependenciesFoldout(dependenciesContainer, Constants.DependenciesText, pageManager)
                {
                    Expanded = stateManager?.DependenciesFoldoutValue ?? false
                };

            if (stateManager != null)
            {
                m_DependenciesFoldout.RegisterValueChangedCallback(value =>
                {
                    stateManager.DependenciesFoldoutValue = value;
                });
            }
        }

        public void RefreshUI(BaseAssetData assetData, bool isLoading = false)
        {
            if (isLoading)
            {
                m_DependenciesFoldout.StartPopulating();
                return;
            }

            RefreshDependenciesInformationUI(assetData);
            m_DependenciesFoldout.RefreshFoldoutStyleBasedOnExpansionStatus();
        }

        void RefreshDependenciesInformationUI(BaseAssetData assetData)
        {
            var dependencies = assetData.Dependencies.ToList();
            m_DependenciesFoldout.Populate(assetData, dependencies);
            UIElementsUtils.SetDisplay(m_NoDependenciesBox, !dependencies.Any());
            m_DependenciesFoldout.StopPopulating();
        }
    }
}
