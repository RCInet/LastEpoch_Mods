using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string ReimportWindow = "reimport-window";
        public const string ReimportWindowContent = ReimportWindow + "-content";
        public const string ReimportWindowFooter = ReimportWindow + "-footer";
        public const string ReimportWindowConflictsTitle = ReimportWindow + "-conflicts-title";
        public const string ReimportWindowWarningContainer = ReimportWindow + "-warning-container";
        public const string ReimportWindowWarningIcon = ReimportWindow + "-warning-icon";
        public const string ReimportWindowUpwardDependenciesTitle = ReimportWindow + "-upward-dependencies-title";
        public const string ReimportWindowGap = ReimportWindow + "-gap";
    }

    class ReimportWindow : EditorWindow
    {
        static readonly Vector2 k_MinWindowSize = new(350, 50);
        static readonly string k_WindowTitle = "Import";
        const string k_MainDarkUssName = "MainDark";
        const string k_MainLightUssName = "MainLight";

        Action<IEnumerable<ResolutionData>> m_Callback;
        Action m_CancelCallback;

        VisualElement m_ConflictsContainer;
        VisualElement m_NonConflictingContainer;
        VisualElement m_UpwardDependenciesContainer;

        readonly List<VisualElement> m_Gaps = new();
        readonly List<ReimportFoldout> m_ReimportFoldouts = new();
        readonly List<ReimportFoldout> m_ConflictsFoldouts = new();
        IEnumerable<ResolutionData> m_Resolutions;

        public static void CreateModalWindow(UpdatedAssetData data, ImportSettingsInternal importSettings, Action<IEnumerable<ResolutionData>> callback = null,
            Action cancelCallback = null)
        {
            ReimportWindow window = GetWindow<ReimportWindow>(k_WindowTitle);
            window.minSize = k_MinWindowSize;

            window.m_Callback = callback;
            window.m_CancelCallback = cancelCallback;
            window.CreateConflictsList(data, importSettings.AvoidRollingBackAssetVersion);
            window.CreateUpwardDependenciesList(data);

            window.ShowModal();
        }

        void OnDestroy()
        {
            if (m_Resolutions?.Any() ?? false)
            {
                m_Callback?.Invoke(m_Resolutions);
            }
            else
            {
                m_CancelCallback?.Invoke();
            }
        }

        void CreateGUI()
        {
            UIElementsUtils.LoadCommonStyleSheet(rootVisualElement);
            UIElementsUtils.LoadCustomStyleSheet(rootVisualElement,
                EditorGUIUtility.isProSkin ? k_MainDarkUssName : k_MainLightUssName);

            // Main container
            var content = new ScrollView();
            content.AddToClassList(UssStyle.ReimportWindowContent);
            rootVisualElement.Add(content);

            // Conflicts
            m_ConflictsContainer = new VisualElement();
            content.Add(m_ConflictsContainer);

            m_NonConflictingContainer = new VisualElement();
            content.Add(m_NonConflictingContainer);

            var conflictsTitle = new Label(L10n.Tr(Constants.ReimportWindowConflictsTitle));
            conflictsTitle.AddToClassList(UssStyle.ReimportWindowConflictsTitle);
            m_ConflictsContainer.Add(conflictsTitle);

            var conflictsWarningContainer = new VisualElement();
            conflictsWarningContainer.AddToClassList(UssStyle.ReimportWindowWarningContainer);
            m_ConflictsContainer.Add(conflictsWarningContainer);

            var conflictsWarningIcon = new Image();
            conflictsWarningIcon.AddToClassList(UssStyle.ReimportWindowWarningIcon);
            conflictsWarningContainer.Add(conflictsWarningIcon);

            var conflictsWarning = new Label(L10n.Tr(Constants.ReimportWindowConflictsWarning));
            conflictsWarningContainer.Add(conflictsWarning);

            var gap = new VisualElement();
            gap.AddToClassList(UssStyle.ReimportWindowGap);
            content.Add(gap);
            m_Gaps.Add(gap);

            // Upward Dependencies
            m_UpwardDependenciesContainer = new VisualElement();
            content.Add(m_UpwardDependenciesContainer);

            var upwardDependenciesTitle = new Label(L10n.Tr(Constants.ReimportWindowUpwardDependenciesTitle));
            upwardDependenciesTitle.AddToClassList(UssStyle.ReimportWindowUpwardDependenciesTitle);
            m_UpwardDependenciesContainer.Add(upwardDependenciesTitle);

            // Footer button
            var footer = new VisualElement();
            footer.AddToClassList(UssStyle.ReimportWindowFooter);
            rootVisualElement.Add(footer);

            var cancelButton = new Button(Close)
            {
                text = L10n.Tr(Constants.ReimportWindowCancel)
            };
            footer.Add(cancelButton);

            var okButton = new Button(ConfirmResolutions)
            {
                text = L10n.Tr(Constants.ReimportWindowImport)
            };
            footer.Add(okButton);
        }

        void CreateConflictsList(UpdatedAssetData updatedAssetData, bool avoidRollingBackAssetVersion)
        {
            var allConflictedData = updatedAssetData.Assets.Where(a=> a.HasConflicts).Union(updatedAssetData.Dependants.Where(a=>a.HasConflicts));

            UIElementsUtils.SetDisplay(m_ConflictsContainer, allConflictedData.Any());
            if (allConflictedData.Any())
            {
                var gap = new VisualElement();
                gap.AddToClassList(UssStyle.ReimportWindowGap);
                UIElementsUtils.Show(gap);
                m_NonConflictingContainer.Add(gap);
            }

            var showedData = updatedAssetData.Assets.Union(updatedAssetData.Dependants);

            foreach (var data in showedData)
            {
                var reimportFoldout = new ReimportFoldout(data, avoidRollingBackAssetVersion);
                m_ReimportFoldouts.Add(reimportFoldout);

                if (data.HasConflicts)
                {
                    m_ConflictsFoldouts.Add(reimportFoldout);
                    m_ConflictsContainer.Add(reimportFoldout);
                }
                else
                {
                    m_NonConflictingContainer.Add(reimportFoldout);
                }
            }
        }

        void CreateUpwardDependenciesList(UpdatedAssetData updatedAssetData)
        {
            if (!updatedAssetData.UpwardDependencies.Any())
            {
                UIElementsUtils.Hide(m_UpwardDependenciesContainer);
                return;
            }

            if (UIElementsUtils.IsDisplayed(m_ConflictsContainer))
            {
                UIElementsUtils.Show(m_Gaps[0]);
            }

            foreach (var data in updatedAssetData.UpwardDependencies)
            {
                var upwardDependencyItem = new UpwardDependencyItem(data);
                m_UpwardDependenciesContainer.Add(upwardDependencyItem);
            }
        }

        void ConfirmResolutions()
        {
            var assetDatabase = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();
            var editorUtility = ServicesContainer.instance.Resolve<IEditorUtilityProxy>();

            try
            {
                // Wait for all import operations to initiate before allowing the database to refresh.
                assetDatabase.StartAssetEditing();

                // Need to clear dirty and reimport assets that are going to be replaced to avoid state problems
                foreach (var foldout in m_ConflictsFoldouts)
                {
                    if (foldout.ResolutionSelection == ResolutionSelection.Replace)
                    {
                        foreach (var obj in foldout.AssetDataResolutionInfo.DirtyObjects)
                        {
                            editorUtility.ClearDirty(obj);

                            var assetPath = assetDatabase.GetAssetPath(obj);

                            if (!assetPath.EndsWith(".shader"))
                            {
                                assetDatabase.ImportAsset(assetPath);
                            }
                        }
                    }
                }

                assetDatabase.StopAssetEditing();
            }
            catch (Exception e)
            {
                Utilities.DevLogException(e);
            }

            m_Resolutions = m_ReimportFoldouts.Select(item => new ResolutionData
            {
                AssetData = item.AssetData,
                ResolutionSelection = item.ResolutionSelection
            });
            Close();
        }
    }
}
